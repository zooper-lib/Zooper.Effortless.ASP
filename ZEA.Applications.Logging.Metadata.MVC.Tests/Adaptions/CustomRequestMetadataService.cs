using System.Security.Claims;
using ZEA.Applications.Logging.Metadata.Abstractions.Interfaces;
using ZEA.Applications.Logging.Metadata.MVC.Services;

namespace ZEA.Applications.Logging.Metadata.MVC.Tests.Adaptions;

/// <summary>
/// Custom service for retrieving request metadata.
/// </summary>
public class CustomRequestMetadataService : BaseRequestMetadataService<RequestMetadata>
{
	private const string ServiceRole = "Service";

	public override Task<RequestMetadata> GetMetadataAsync(
		HttpContext context,
		CancellationToken cancellationToken)
	{
		var sessionId = GetSessionId(context);
		var timestamp = GetTimestamp();
		var ipAddress = GetIpAddress(context);

		var user = context.User;
		var roles = user.Claims
			.Where(c => c.Type == ClaimTypes.Role)
			.Select(c => c.Value)
			.ToList();

		return roles.Contains(ServiceRole)
			? GetServiceRequestMetadataAsync(context, sessionId, timestamp, ipAddress, cancellationToken)
			: GetUserRequestMetadataAsync(context, sessionId, timestamp, ipAddress, roles, cancellationToken);
	}

	private Task<RequestMetadata> GetServiceRequestMetadataAsync(
		HttpContext context,
		Guid sessionId,
		DateTime timestamp,
		string ipAddress,
		CancellationToken cancellationToken)
	{
		var serviceName = context.Request.Headers["X-Service-Name"].ToString();
		var instanceId = context.Request.Headers["X-Instance-Id"].ToString();

		var metadata = new RequestMetadata
		{
			SessionId = sessionId,
			Timestamp = timestamp,
			RequesterMetadata = new ServiceRequestMetadata
			{
				ServiceName = serviceName,
				InstanceId = instanceId,
				IpAddress = ipAddress
			}
		};

		return Task.FromResult(metadata);
	}

	private Task<RequestMetadata> GetUserRequestMetadataAsync(
		HttpContext context,
		Guid sessionId,
		DateTime timestamp,
		string ipAddress,
		List<string> roles,
		CancellationToken cancellationToken)
	{
		var requesterIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);

		var requesterId = string.Empty;

		requesterId = requesterIdClaim != null
			? requesterIdClaim.Value
			: "Anonymous"; // Or assign null or any default value

		var metadata = new RequestMetadata
		{
			SessionId = sessionId,
			Timestamp = timestamp,
			RequesterMetadata = new UserRequestMetadata
			{
				RequesterId = requesterId,
				Roles = roles,
				IpAddress = ipAddress
			}
		};

		return Task.FromResult(metadata);
	}
}