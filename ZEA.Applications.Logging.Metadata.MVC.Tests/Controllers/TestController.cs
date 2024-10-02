using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Accessors;

namespace ZEA.Applications.Logging.Metadata.MVC.Tests.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
	[HttpGet("unauthorized")]
	public IActionResult GetUnauthorized([FromServices] IRequestMetadataAccessor<RequestMetadata> accessor)
	{
		var metadata = accessor.Metadata;

		if (metadata != null)
		{
			// Use the metadata as needed
			return Ok(
				new
				{
					metadata.SessionId,
					metadata.Timestamp
				}
			);
		}
		else
		{
			return BadRequest("Metadata not available.");
		}
	}

	[Authorize]
	[HttpGet("authorized")]
	public IActionResult GetAuthorized([FromServices] IRequestMetadataAccessor<RequestMetadata> accessor)
	{
		var metadata = accessor.Metadata;

		if (metadata != null)
		{
			// Use the metadata as needed
			return Ok(
				new
				{
					metadata.SessionId,
					metadata.Timestamp
				}
			);
		}
		else
		{
			return BadRequest("Metadata not available.");
		}
	}
}