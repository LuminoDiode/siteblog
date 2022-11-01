using backend.Models.Database;
using backend.Models.Runtime;

namespace backend.Services
{
	public class FileUrnService
	{
		protected virtual SettingsProviderService _settingsProvider { get; init; }
		protected virtual FileUrnServiceSettings _settings => _settingsProvider.FileUrnServiceSettings;

		private string _staticFilesUrnPath => _settings.staticFilesUrnPath;
		private string _imageUrnDirectoryPath => _settings.imagesDirectoryPath;
		private string _imageFileExtension => _settings.imageFileExtension;
		public FileUrnService(SettingsProviderService settingsProvider)
		{
			_settingsProvider = settingsProvider;
		}

		public string UrnToGetImage(Image img) => '/' + Path.Join(_staticFilesUrnPath, _imageUrnDirectoryPath, img.Id.ToString() + '.' + _imageFileExtension);
		public string UrnToSetImage(Image img) => '/' + Path.Join(_imageUrnDirectoryPath, img.Id.ToString() + '.' + _imageFileExtension);
	}
}
