namespace backend.Models.Runtime
{
	/* Данный класс используется только для nameof. */
	public abstract class StorageSettings
	{
		/* This string to be passed with request url to tell ASP that we want to retrieve some static.
		 * It means, that when we want to add static file, we are using path like /images/imageName.jpeg,
		 * but when we want to retrieve it we need to use path like /staticfiles/images/imageName.jpeg.
		 * (so this variable is this *staticfiles*).
		 */
		public virtual string staticFilesUrnPath { get; set; } = null!; 
		public virtual string imagesDirectoryPath { get; set; } = null!;
		public virtual string imageFileExtension { get; set; } = null!;
	}
}
