using backend.Models.API.Common;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace backend.Services
{
	
	public abstract class DraftService<TEntity, TContext>
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

		protected readonly TContext? _dbContext;
		protected readonly SettingsProviderService _settingsProvider;
		protected virtual DraftServiceSettings _settings => _settingsProvider.DraftServiceSettings;
		protected readonly List<StoredDraft> _ramStored;
		protected readonly Func<TContext, DbSet<TEntity>>? _dbSetSelector;
		protected readonly Func<TEntity, bool>? _entityInDbPredicate;
		protected readonly Func<TEntity, DateTime>? _entityLastUpdateSelector;
		protected readonly Action<TEntity, DateTime>? _entityLastUpdateFieldSetter;

		protected Func<StoredDraft, bool> _inRamOutdatedPredicate;
		protected Func<TEntity, bool>? _inDbOutdatedPredicate;

		protected const string cannotUseDbMethodsWhileContextIsNullExceptionMessage = 
			$"Cannot use any of the db methods when any of " +
			$"{nameof(_dbContext)}, {nameof(_dbSetSelector)} or {nameof(_entityInDbPredicate)} is null.";

		/// <summary>
		/// Initializes a new instance of the <see cref="DraftService"/> class.<br/>
		/// The primary purpose of this class is to save user-draft, <br/>
		/// like drafts of the articles e.t.c
		/// </summary>
		/// <param name="dbContext">
		/// Nullable db context to be used for saving to db. 
		/// In case of null only ram will be used for storing drafts.
		/// </param>
		/// <param name="settingsProvider">
		/// SettingsProvider for the service.
		/// In case of inheritance, override _settings property to chage used provided settings.
		/// </param>
		/// <param name="dbSetSelector">
		/// Delegate for getting target dbSet from the TContext.<br/>
		/// Not used if <paramref name="dbContext"/> is null.
		/// </param>
		/// <param name="entityInDbPredicate">
		/// Delegate for getting the entity from db. Prefer using Id selection.<br/>
		/// When trying to save draft entities to the db, service will try to use this<br/>
		/// predicate in the SignleOrDefault method to detect already existing draft in db and update it.<br/>
		/// This delegate must be translateable for Queryable in case of
		/// using TContext (if it is not null).<br/>
		/// Not used if <paramref name="dbContext"/> is null.
		/// </param>
		/// <param name="entityLastUpdateFieldSelector">
		/// Delegate for getting last update time from the stored entities.<br/>
		/// Not used if <paramref name="dbContext"/> is null.
		/// In case of null when <paramref name="dbContext"/> is not null, <br/>
		/// all the entities in db will be counted as not outdated.
		/// </param>
		/// <param name="entityLastUpdateFieldSetter">
		/// Delegate for setting last update time from the stored entities.<br/>
		/// Not used if <paramref name="dbContext"/> is null.
		/// </param>
		public DraftService(
			SettingsProviderService settingsProvider,
			TContext? dbContext=null,
			Func<TContext, DbSet<TEntity>>? dbSetSelector = null,
			Func<TEntity, bool>? entityInDbPredicate = null,
			Func<TEntity, DateTime>? entityLastUpdateFieldSelector = null,
			Action<TEntity, DateTime>? entityLastUpdateFieldSetter = null)
		{
			_dbContext = dbContext;
			_settingsProvider = settingsProvider;
			_dbSetSelector = dbSetSelector;
			_entityInDbPredicate = entityInDbPredicate;
			_entityLastUpdateSelector = entityLastUpdateFieldSelector;
			_entityLastUpdateFieldSetter = entityLastUpdateFieldSetter;

			_ramStored = new(_settings.maxEntitiesStoredInRam);

			if (dbContext is not null)
			{
				if (dbSetSelector is null || entityInDbPredicate is null)
				{
					throw new ArgumentNullException($"If the {nameof(dbContext)} parameter is not null, " +
						$"both {nameof(dbSetSelector)} and {nameof(entityInDbPredicate)} parameters " +
						$"must be not null, dbContext usage cannot be determinated otherwise.");
				}
			}

			_inRamOutdatedPredicate = (x =>
				x.LastUpdatedDateTimeUtc.AddMinutes(_settings.maxTimeEntityStoredInRamMinutes)
				< DateTime.UtcNow);

			_inDbOutdatedPredicate = entityLastUpdateFieldSelector is null ? null : (x =>
				entityLastUpdateFieldSelector(x).AddMinutes(_settings.maxTimeEntityStoredInDbMinutes)
				< DateTime.UtcNow);
		}

		public void PushDraft(TEntity entity)
		{
			this._ramStored.Add(new(DateTime.UtcNow, entity));
		}

		protected TEntity? GetDraftFromRam(Func<TEntity, bool> predicate)
		{
			var found = _ramStored.FirstOrDefault(x => predicate(x.Entity));
			return found is null ? default(TEntity) : found.Entity;
		}

		protected async Task<TEntity?> GetDraftFromDbAsync(Func<TEntity, bool> predicate)
		{
			if (this._dbContext is null || this._dbSetSelector is null || this._entityInDbPredicate is null)
				throw new ArgumentNullException(cannotUseDbMethodsWhileContextIsNullExceptionMessage);

			return await _dbSetSelector(_dbContext).FirstOrDefaultAsync(x => predicate(x)); // 
		}

		public async Task<TEntity?> GetDraftAsync(Func<TEntity, bool> predicate)
		{
			if (this._dbContext is null || this._dbSetSelector is null || this._entityInDbPredicate is null)
				return GetDraftFromRam(predicate);
			else
				return GetDraftFromRam(predicate) ?? await GetDraftFromDbAsync(predicate);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				if (_dbContext is null)
				{
					DeleteOutdatedFromRam();
				}
				else
				{
					await DeleteOutdatedFromDbAsync();
					await SaveOutdatedFromRamToDbAsync();
				}

				await Task.Delay((int)(_settings.updateStoredEntitiesIntervalMinutes * 60 * 100));
			}
		}

		protected void DeleteOutdatedFromRam()
		{
			_ramStored.RemoveAll(x => _inRamOutdatedPredicate(x));
		}

		protected async Task SaveOutdatedFromRamToDbAsync()
		{
			if (this._dbContext is null || this._dbSetSelector is null || this._entityInDbPredicate is null)
				throw new ArgumentNullException(cannotUseDbMethodsWhileContextIsNullExceptionMessage);

			// discovering all the entities to save
			var toBeSaved = this._ramStored.Where(x => _inRamOutdatedPredicate(x));

			// saving all the discovered to db
			foreach (var ramStored in toBeSaved)
			{
				var foundInDb = _dbSetSelector(_dbContext).AsTracking().SingleOrDefaultAsync(x => _entityInDbPredicate(x));

				// if such id not found - add new row
				if (foundInDb is null) await _dbSetSelector(_dbContext).AddAsync(ramStored.Entity);
				// if it is - update in the db
				else
				{
					if (_entityLastUpdateFieldSetter is not null)
						_entityLastUpdateFieldSetter(ramStored.Entity, ramStored.LastUpdatedDateTimeUtc);
					_dbContext.Entry(foundInDb).CurrentValues.SetValues(ramStored.Entity);
				}

				this._ramStored.Remove(ramStored);
			}

			// save to the db
			await _dbContext.SaveChangesAsync();

			// detach all the entities
			foreach (var ramStored in toBeSaved)
			{
				_dbContext.Entry(ramStored).State = EntityState.Detached;
			}
		}

		protected async Task DeleteOutdatedFromDbAsync()
		{
			if (this._dbContext is null || this._dbSetSelector is null || this._entityInDbPredicate is null)
				throw new ArgumentNullException(cannotUseDbMethodsWhileContextIsNullExceptionMessage);

			var toDelete = _dbSetSelector(_dbContext).Where(x => (_inDbOutdatedPredicate == null) ? false : _inDbOutdatedPredicate(x));

			_dbSetSelector(_dbContext).RemoveRange(toDelete);

			await _dbContext.SaveChangesAsync();
		}

		// saving all the entities to db
		public override void Dispose()
		{
			if (_dbContext is not null)
			{
				_inRamOutdatedPredicate = x => true;
				SaveOutdatedFromRamToDbAsync().Wait();
			}

			base.Dispose();
		}
		public async ValueTask DisposeAsync()
		{
			if (_dbContext is not null)
			{
				_inRamOutdatedPredicate = x => true;
				await SaveOutdatedFromRamToDbAsync();
			}

			base.Dispose();
		}
	}
}

