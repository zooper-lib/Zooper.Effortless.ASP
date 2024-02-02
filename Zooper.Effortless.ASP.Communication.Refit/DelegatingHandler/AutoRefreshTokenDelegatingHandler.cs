using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Zooper.Effortless.ASP.Communication.Refit.Stores;

namespace Zooper.Effortless.ASP.Communication.Refit.DelegatingHandler;

public class AutoRefreshTokenDelegatingHandler : System.Net.Http.DelegatingHandler
{
	private readonly IAccessDataStore _accessDataStore;
	private readonly string _clientId;
	private readonly string _clientSecret;
	private readonly ILogger<AutoRefreshTokenDelegatingHandler> _logger;

	private readonly Uri _tokenEndpoint;

	public AutoRefreshTokenDelegatingHandler(
		IAccessDataStore accessDataStore,
		ILogger<AutoRefreshTokenDelegatingHandler> logger,
		string clientId,
		string clientSecret,
		Uri tokenEndpoint)
	{
		_accessDataStore = accessDataStore;
		_logger = logger;
		_clientId = clientId;
		_clientSecret = clientSecret;
		_tokenEndpoint = tokenEndpoint;
	}

	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		if (!await IsAuthenticated(cancellationToken))
			await RefreshAccessTokenAsync(cancellationToken);
		else if (await IsAccessTokenExpiredAsync(cancellationToken))
			await RefreshAccessTokenAsync(cancellationToken);

		var accessData = await _accessDataStore.GetAsync(cancellationToken);
		request.Headers.Authorization = new(
			JwtBearerDefaults.AuthenticationScheme,
			accessData?.AuthResponse.AccessToken
		);

		return await base.SendAsync(
			request,
			cancellationToken
		);
	}

	private async Task<bool> IsAuthenticated(CancellationToken cancellationToken)
	{
		var accessData = await _accessDataStore.GetAsync(cancellationToken);

		return accessData != null;
	}

	private async Task<bool> IsAccessTokenExpiredAsync(CancellationToken cancellationToken)
	{
		var accessData = await _accessDataStore.GetAsync(cancellationToken);

		if (accessData == null) return true;

		return accessData.ExpiresAt < DateTime.UtcNow;
	}

	private async Task RefreshAccessTokenAsync(CancellationToken cancellationToken)
	{
		var address = _tokenEndpoint + "connect/token";

		var tokenRequest = new ClientCredentialsTokenRequest
		{
			Address = address,
			ClientId = _clientId,
			ClientSecret = _clientSecret,
			GrantType = "client_credentials"
		};

		var tokenResponse = await new HttpClient().RequestClientCredentialsTokenAsync(
			tokenRequest,
			cancellationToken
		);

		if (tokenResponse.IsError)
		{
			_logger.LogCritical(
				"Failed to obtain token from {TokenEndpoint}: {TokenResponseError}",
				address,
				tokenResponse.Error
			);
			return;
		}


		// Cache the new Access Token
		await _accessDataStore.SetAsync(
			new()
			{
				AuthResponse = tokenResponse,
				ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
			},
			cancellationToken
		);
	}
}