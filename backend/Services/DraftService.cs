using backend.Models.API.Common;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace backend.Services
{

	public /* abstract */ class DraftService<TEntity, TContext> 
		: BackgroundService, IDisposable, IAsyncDisposable
		where TEntity : class
		where TContext : DbContext
	{
		protected class StoredDraft
		{
			public DateTime LastUpdatedDateTimeUtc;
			public TEntity Entity;

			public StoredDraft(DateTime lastUpdatedDateTime, TEntity entity)
			{
				this.LastUpdatedDateTimeUtc = lastUpdatedDateTime;
				this.Entity = entity;
			}
		}

		protected readonly TContext? _blogContext;
		protected readonly SettingsProviderService _settingsProvider;
		protected virtual DraftServiceSettings _settings => _settingsProvider.DraftServiceSettings;
		protected readonly List<StoredDraft> RamStored;
		protected readonly Func<TContext, DbSet<TEntity>> _dbSetSelector;
		protected readonly Func<TEntity, bool> _entitySelector;
		protected readonly Func<TEntity, DateTime> _entityLastUpdateSelector;

		protected Func<StoredDraft, bool> inRamOutdatedPredicate;
		protected Func<TEntity, bool> inDbOutdatedPredicate;

		/// <summary>
		/// Initializes a new instance of the <see cref="DraftService"/> class.
		/// </summary>
		/// <param name="dbContext">
		/// Nullable db context to be used for saving to db. 
		/// In case of null only ram will be used for storing drafts.
		/// </param>
		/// <param name="settingsProvider">
		/// SettingsProvider for the service.
		/// </param>
		/// <param name="dbSetSelector">
		/// Delegate for getting needed dbSet from the TContext.
		/// </param>
		/// <param name="entitySelector">
		/// Delegate for getting needed entity from the collections.
		/// This delegate must be translateable for Queryable in case of
		/// using TContext (if it is not null).
		/// </param>
		/// <param name="entityLastUpdateFieldSelector">
		/// Delegate for getting last update time from the stored entities.
		/// </param>
		public DraftService(
			TContext? dbContext,
			SettingsProviderService settingsProvider,
			Func<TContext, DbSet<TEntity>> dbSetSelector,
			Func<TEntity, bool> entitySelector,
			Func<TEntity, DateTime> entityLastUpdateFieldSelector)
		{
			_blogContext = dbContext;
			_settingsProvider = settingsProvider;
			_dbSetSelector = dbSetSelector;
			_entitySelector = entitySelector;
			_entityLastUpdateSelector = entityLastUpdateFieldSelector;

			RamStored = new(_settings.maxEntitiesStoredInRam);

			inRamOutdatedPredicate = (x =>
				x.LastUpdatedDateTimeUtc.AddMinutes(_settings.maxTimeEntityStoredInRamMinutes)
				< DateTime.UtcNow);
			inDbOutdatedPredicate = (x =>
				_entityLastUpdateSelector(x).AddMinutes(_settings.maxTimeEntityStoredInDbMinutes)
				< DateTime.UtcNow);
		}

		public void PushDraft(TEntity entity)
		{
			this.RamStored.Add(new(DateTime.UtcNow, entity));
		}

		public TEntity? GetDraftFromRam(Func<TEntity, bool> predicate)
		{
			var found = RamStored.FirstOrDefault(x => predicate(x.Entity));
			return found is null ? default(TEntity) : found.Entity;
		}

		public Task<TEntity?> GetDraftFromDbAsync(Func<TEntity, bool> predicate)
		{
			if (_blogContext is null) return Task.FromResult(default(TEntity));
			return _dbSetSelector(_blogContext).FirstOrDefaultAsync(x => predicate(x));
		}


		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay((int)(_settings.updateStoredEntitiesIntervalMinutes * 60 * 100));

				if (_blogContext is null)
				{
					DeleteOutdatedFromRam();
				}
				else
				{
					await DeleteOutdatedFromDbAsync(_blogContext);
					await SaveOutdatedFromRamToDbAsync(_blogContext);
				}
			}
		}

		protected void DeleteOutdatedFromRam()
		{
			RamStored.RemoveAll(x => inRamOutdatedPredicate(x));
		}

		protected async Task SaveOutdatedFromRamToDbAsync(TContext ctx)
		{
			// discovering all the entities to save
			var toBeSaved = this.RamStored.Where(x => inRamOutdatedPredicate(x));

			// saving all the discovered to db
			foreach (var ramStored in toBeSaved)
			{
				var foundInDb = await _dbSetSelector(ctx).AsTracking().SingleOrDefaultAsync(x => _entitySelector(x));

				// if such id not found - add new row
				if (foundInDb is null) await _dbSetSelector(ctx).AddAsync(ramStored.Entity);
				// if it is - update in the db
				else ctx.Entry(foundInDb).CurrentValues.SetValues(ramStored.Entity);

				this.RamStored.Remove(ramStored);
			}

			// save to the db
			await ctx.SaveChangesAsync();

			// detach all the entities
			foreach (var ramStored in toBeSaved)
			{
				ctx.Entry(ramStored).State = EntityState.Detached;
			}
		}

		protected async Task DeleteOutdatedFromDbAsync(TContext ctx)
		{
			var toDelete = _dbSetSelector(ctx).Where(
				x => (DateTime.UtcNow - _entityLastUpdateSelector(x))
				> TimeSpan.FromMinutes(_settings.maxTimeEntityStoredInDbMinutes));

			_dbSetSelector(ctx).RemoveRange(toDelete);

			await ctx.SaveChangesAsync();
		}

		// saving all the entities to db
		public override void Dispose()
		{
			if (_blogContext is not null)
			{
				inRamOutdatedPredicate = x => true;
				SaveOutdatedFromRamToDbAsync(_blogContext).Wait();
			}

			base.Dispose();
		}
		public async ValueTask DisposeAsync()
		{
			if (_blogContext is not null)
			{
				inRamOutdatedPredicate = x => true;
				await SaveOutdatedFromRamToDbAsync(_blogContext);
			}

			base.Dispose();
		}
	}
}

