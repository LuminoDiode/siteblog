using backend.Models.Database;
using backend.Models.Runtime;

namespace backend.Services
{
	public class FileUrnService
	{
		private IConfiguration _configuration;

		private string _staticFilesUrnPath => this._configuration.GetSection(nameof(StorageSettings))?.Get<StorageSettings?>()?.staticFilesUrnPath ?? "staticfiles";
		private string _imageUrnDirectoryPath => this._configuration.GetSection(nameof(StorageSettings))?.Get<StorageSettings?>()?.imagesDirectoryPath ?? "user_added/images";
		private string _imageFileExtension => this._configuration.GetSection(nameof(StorageSettings))?.Get<StorageSettings?>()?.imageFileExtension ?? "jpeg";
		public FileUrnService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string UrnToGetImage(Image img) => '/' + Path.Join(_staticFilesUrnPath, _imageUrnDirectoryPath, img.Id.ToString() + '.' + _imageFileExtension);
		public string UrnToSetImage(Image img) => '/' + Path.Join(_imageUrnDirectoryPath, img.Id.ToString() + '.' + _imageFileExtension);
	}
}
