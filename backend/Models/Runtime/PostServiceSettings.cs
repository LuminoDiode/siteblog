namespace backend.Models.Runtime
{
	public class PostConstraints
	{
		public bool titleRequired { get; set; } = true;
		public bool subtitleRequired { get; set; } = false;
		public bool mainTextRequired { get; set; } = true;

		public int titleMinLen { get; set; } = 1;
		public int subtitleMinLen { get; set; } = 1;
		public int mainTextMinLen { get; set; } = 100;

		public int titleMaxLen { get; set; } = 256;
		public int subtitleMaxLen { get; set; } = 256;
		public int mainTextMaxLen { get; set; } = short.MaxValue*2;
	}
	public class PostProcessingSettings
	{
		public bool trimTitle { get; set; } = true;
		public bool trimSubtitle { get; set; } = true;
		public bool trimMainText { get; set; } = true;
	}
	public class PostServiceSettings
	{
		public PostConstraints _postConstraints { get; set; } = new();
		public PostProcessingSettings _postProcessingSettings { get; set; } = new();
	}
}
