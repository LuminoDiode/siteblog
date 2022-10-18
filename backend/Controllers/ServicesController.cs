using backend.Models.API.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class ServiceController
	{

		public ServiceController()
		{

		}

		[HttpGet]
		[AllowAnonymous]
		public IEnumerable<ServiceInfoResponse> GetServicesInfo()
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		[AllowAnonymous]
		[Route("{id:int}")]
		public ServiceInfoResponse GetServiceInfo([FromRoute][Required] int Id)
		{
			throw new NotImplementedException();
		}

		[HttpPut]
		[Authorize]
		public ServiceInfoResponse PutService([FromBody][Required] ServicePutRequest NewService)
		{
			throw new NotImplementedException();
		}
		[HttpPatch]
		[Authorize]
		public ServiceInfoResponse PatchService([FromBody][Required] ServicePutRequest NewService)
		{
			throw new NotImplementedException();
		}
	}
}
