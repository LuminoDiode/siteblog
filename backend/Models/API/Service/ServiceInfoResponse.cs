using backend.Models.API.Common;

namespace backend.Models.API.Service
{
    public class ServiceInfoResponse
    {
		public int Id { get; set; }

		public string? Name { get; set; } = null!;
		public string? Description { get; set; }
		public string? Url { get; set; }
		public string? TopImageLink { get; set; }
	}
}
