using NpgsqlTypes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models.API.Service
{
	public class ServicePutRequest
	{
		[Required]
		public string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public string Url { get; set; }

	}
}
