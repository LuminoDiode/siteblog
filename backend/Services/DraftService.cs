using backend.Models.API.Common;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace backend.Services
{

	public class DraftService<T> : BackgroundService where T: class
	{
		protected class StoredDraft
		{
			public DateTime LastUpdatedDateTimeUtc;
			public T Entity;

			public StoredDraft(DateTime lastUpdatedDateTime, T entity)
			{
				this.LastUpdatedDateTimeUtc = lastUpdatedDateTime;
				this.Entity = entity;
			}
		}

		protected readonly BlogContext _blogContext;
		protected virtual SettingsProviderService _settingsProvider { get; init; }
		protected virtual DraftServiceSettings _settings => _settingsProvider.DraftServiceSettings;
		protected readonly List<StoredDraft> RamStored;
		protected readonly Func<DbContext, DbSet<T>> _dbSetSelector;
		protected readonly Func<T, bool> _entitySelector;

		public DraftService(
			BlogContext blogContext, 
			SettingsProviderService settingsProvider, 
			Func<DbContext,DbSet<T>> dbSetSelector,
			Func<T, bool> EntitySelector)
		{
			_blogContext = blogContext;
			_settingsProvider = settingsProvider;
			_dbSetSelector = dbSetSelector;
			_entitySelector = EntitySelector;

			RamStored = new(_settings.maxEntitiesStoredInRam);

			throw new NotImplementedException();
		}

		public void PushDraft(T entity)
		{
			this.RamStored.Add(new(DateTime.UtcNow, entity));
		}

		public T? GetDraft(Func<T, bool> EntitySelector)
		{
			var found = RamStored.FirstOrDefault(x => EntitySelector(x.Entity));
			return found is null ? default(T) : found.Entity;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay((int)(_settings.updateStoredEntitiesIntervalMinutes * 60 * 100));

				await DeleteOutdatedFromDbAsync();
				await SaveOutdatedFromRamToDbAsync();
			}
		}

		protected async Task SaveOutdatedFromRamToDbAsync()
		{
			var toBeSaved = this.RamStored.Where(x =>
				x.LastUpdatedDateTimeUtc.AddMinutes(_settings.maxTimeEntityStoredInRamMinutes)
				< DateTime.UtcNow);

			foreach (var ramStored in toBeSaved)
			{
				var foundInDb = await _dbSetSelector(_blogContext).AsTracking().SingleOrDefaultAsync(x=> _entitySelector(x));
				if (foundInDb is null) await _dbSetSelector(_blogContext).AddAsync(ramStored.Entity);
				else _blogContext.Entry(foundInDb).CurrentValues.SetValues(ramStored.Entity);

				this.RamStored.Remove(ramStored);
			}

			await _blogContext.SaveChangesAsync();
		}

		protected async Task DeleteOutdatedFromDbAsync()
		{
			throw new NotImplementedException();
		}

	}
}

