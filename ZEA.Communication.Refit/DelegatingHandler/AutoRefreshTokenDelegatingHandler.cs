using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using ZEA.Communication.Refit.Stores;

namespace ZEA.Communication.Refit.DelegatingHandler;

public class AutoRefreshTokenDelegatingHandler(
	IAccessDataStore accessDataStore,
	ILogger<AutoRefreshTokenDelegatingHandler> logger,
	string clientId,
	string clientSecret,
	Uri tokenEndpoint)
	: System.Net.Http.DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		if (!await IsAuthenticated(cancellationToken))
			await RefreshAccessTokenAsync(cancellationToken);
		else if (await IsAccessTokenExpiredAsync(cancellationToken))
			await RefreshAccessTokenAsync(cancellationToken);

		var accessData = await accessDataStore.GetAsync(cancellationToken);
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
		var accessData = await accessDataStore.GetAsync(cancellationToken);

		return accessData != null;
	}

	private async Task<bool> IsAccessTokenExpiredAsync(CancellationToken cancellationToken)
	{
		var accessData = await accessDataStore.GetAsync(cancellationToken);

		if (accessData == null) return true;

		return accessData.ExpiresAt < DateTime.UtcNow;
	}

	private async Task RefreshAccessTokenAsync(CancellationToken cancellationToken)
	{
		var address = tokenEndpoint + "connect/token";

		var tokenRequest = new ClientCredentialsTokenRequest
		{
			Address = address,
			ClientId = clientId,
			ClientSecret = clientSecret,
			GrantType = "client_credentials"
		};

		var tokenResponse = await new HttpClient().RequestClientCredentialsTokenAsync(
			tokenRequest,
			cancellationToken
		);

		if (tokenResponse.IsError)
		{
			logger.LogCritical(
				"Failed to obtain token from {TokenEndpoint}: {TokenResponseError}",
				address,
				tokenResponse.Error
			);
			return;
		}


		// Cache the new Access Token
		await accessDataStore.SetAsync(
			new()
			{
				AuthResponse = tokenResponse,
				ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
			},
			cancellationToken
		);
	}
}