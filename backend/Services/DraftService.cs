using backend.Models.API.Common;
using backend.Models.Runtime;
using backend.Repository;
using System.Threading;

namespace backend.Services
{
	
	public class DraftService<T> :BackgroundService
	{
		protected readonly BlogContext _blogContext;
		protected readonly DraftServiceSettingsProvider _settings;
		protected readonly Stack<T> RamStored;


		public DraftService(BlogContext blogContext,SettingsProviderService settingsProvider)
		{
			_blogContext = blogContext;
			_settings=	settingsProvider.DraftServiceSettings;

			RamStored = new();

			throw new NotImplementedException();
		}

		public void PushDraft(T entity)
		{
			this.RamStored.Push(entity);
		}

		public T? GetDraft(Func<T,bool> Selector)
		{
			return RamStored.FirstOrDefault<T>(Selector);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay((int)(_settings.updateStoredEntitiesIntervalMinutes*60*100));

				throw new NotImplementedException();
			}
		}
		
		protected async Task SaveOutdatedFromRamToDbAsync()
		{
			throw new NotImplementedException();
		}

		protected async Task DeleteOutdatedFromDbAsync()
		{
			throw new NotImplementedException();
		}

	}
}

