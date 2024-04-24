using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using StreetPerfect;
using Refit;
using StreetPerfect.Http;
using StreetPerfect.Models;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class SPConfigServiceCollectionExtensions
	{
		public static IServiceCollection AddStreetPerfectClient(
			 this IServiceCollection services, Action<StreetPerfectHttpClientConfig> setconf)
		{
			var config = new StreetPerfectHttpClientConfig();
			setconf(config);
			StreetPerfect.Http.SPTokenService._clientId = config.ClientId;
			StreetPerfect.Http.SPTokenService._clientSecret = config.ClientSecret;

			if (config.ApiVersion < 1)
				config.ApiVersion = 1;

			// user can set ANY base address themselves
			if (String.IsNullOrWhiteSpace(config.BaseAddress))
			{
				config.BaseAddress = "https://api.streetperfect.com/api";
			}

			config.BaseAddress = config.BaseAddress.TrimEnd('/');

			// always append api if needed
			if (!config.BaseAddress.ToLower().EndsWith("/api"))
				config.BaseAddress += "/api";


			var builder = services.AddRefitClient<IStreetPerfectHttpClient>().ConfigureHttpClient(c =>
			{
				c.BaseAddress = new Uri($"{config.BaseAddress}/{config.ApiVersion}");
				c.DefaultRequestHeaders.Add("Accept", "application/json");
				if (!String.IsNullOrEmpty(config.ApiKey))
					c.DefaultRequestHeaders.Add("X-Api-Key", config.ApiKey);
			});

			// jwt enabled only if client id & secret and NOT apikey
			if (String.IsNullOrEmpty(config.ApiKey)
				&& !String.IsNullOrEmpty(config.ClientId) && !String.IsNullOrEmpty(config.ClientSecret))
			{
				services.AddSingleton<ISPTokenService, SPTokenService>();

				// token endpoints have no version component
				services.AddRefitClient<IStreetPerfectTokenClient>().ConfigureHttpClient(c =>
				{
					c.BaseAddress = new Uri(config.BaseAddress);
				});
				services.AddTransient<SpRestAuthHandler>();
				builder.AddHttpMessageHandler<SpRestAuthHandler>();
			}
			return services;
		}
	}
}

namespace StreetPerfect.Http
{
	public class StreetPerfectHttpClientConfig
	{
		public string ClientId { get; set; } // deprecated...
		public string ClientSecret { get; set; }
		public string ApiKey { get; set; }
		public string BaseAddress { get; set; } // this takes priority
		public int ApiVersion { get; set; } = 1; // default is 1
	}


	/// <summary>
	/// refit interface for StreetPerfect rest api
	/// </summary>
	public interface IStreetPerfectHttpClient
	{
		[Post("/ca/typeahead")]
		Task<caTypeaheadResponse> caTypeaheadAsync(caTypeaheadRequest req);

		[Post("/ca/typeahead/rec")]
		Task<caTypeaheadResponse> caTypeaheadRecAsync(caTypeaheadRequest req);

		[Post("/ca/typeahead/fetch")]
		Task<caTypeaheadFetchResponse> caTypeaheadFetchAsync(caTypeaheadFetchRequest req);

		[Post("/ca/fetch")]
		Task<caFetchAddressResponse> caFetchAddressAsync(caFetchAddressRequest req);

		[Post(path: ("/ca/format"))]
		Task<caFormatAddressResponse> caFormatAddressAsync(caFormatAddressRequest req);

		[Post(path: ("/ca/correction"))]
		Task<caCorrectionResponse> caProcessCorrectionAsync(caAddressRequest req);

		[Post(path: ("/ca/parse"))]
		Task<caParseResponse> caProcessParseAsync(caAddressRequest req);

		[Post(path: ("/ca/search"))]
		Task<caSearchResponse> caProcessSearchAsync(caAddressRequest req);

		[Post(path: ("/ca/query"))]
		Task<caQueryResponse> caQueryAsync(caQueryRequest req);

		[Post(path: ("/ca/validate"))]
		Task<caValidateAddressResponse> caValidateAddressAsync(caValidateAddressRequest req);

		[Get(path: (""))] 
		Task<GetInfoResponse> GetInfoAsync();

		[Post(path: ("/us/correction"))]
		Task<usCorrectionResponse> usProcessCorrectionAsync(usAddressRequest req);

		// just realized I haven't implemented this
		[Post(path: ("/us/x"))]
		Task<usDeliveryInformationResponse> usProcessDeliveryInfoAsync(usAddressRequest req);

		[Post(path: ("/us/parse"))]
		Task<usParseResponse> usProcessParseAsync(usAddressRequest req);

		[Post(path: ("/us/search"))]
		Task<usSearchResponse> usProcessSearchAsync(usAddressRequest req);

	}

	/// <summary>
	/// I had to create the token stuff separate so it wouldn't go through the same handler 
	/// -- that I use to call them!
	/// end-user doesn't need to call these directly
	/// </summary>
	public interface IStreetPerfectTokenClient
	{
		[Post("/token")]
		Task<TokenResponse> GetTokenAsync(TokenRequest req);

		[Post("/token/refresh")]
		Task<TokenResponse> RefreshTokenAsync(TokenRefreshRequest req);
	}


	/// <summary>
	/// This service contains the token client - via injection
	/// it is injected in to the SpRestAuthHandler 
	/// The SpRestAuthHandler grabs a token from it and adds to the header of every call
	/// </summary>
	public interface ISPTokenService
	{
		Task<TokenResponse> GetTokenAsync(bool fForce = false);
		Task<TokenResponse> RefreshTokenAsync();
	}

	public class SPTokenService : ISPTokenService
	{
		// these statics are set from the AddStreetPerfectClient() extension
		public static string _clientId { get; set; }
		public static string _clientSecret { get; set; }

		private readonly IStreetPerfectTokenClient _tokenClient;
		private TokenResponse _token { get; set; }

		// should we run a timer for refresh?

		// keep track of token life using local computer time
		private DateTime lastRefreshed { get; set; }
		public SPTokenService(IStreetPerfectTokenClient tokenService)
		{
			_tokenClient = tokenService;
		}

		public async Task<TokenResponse> GetTokenAsync(bool fForce = false)
		{
			if (_token == null || fForce)
			{
				_token = await _tokenClient.GetTokenAsync(new TokenRequest() { ClientId = _clientId, ClientSecret = _clientSecret });
				lastRefreshed = DateTime.Now;
			}
			else if ((DateTime.Now - lastRefreshed).Minutes > _token.Expires - 2)
			{
				_token = await RefreshTokenAsync();
			}
			return _token;
		}

		public async Task<TokenResponse> RefreshTokenAsync()
		{
			var req = new TokenRefreshRequest()
			{
				AccessToken = _token.AccessToken,
				RefreshToken = _token.RefreshToken,
			};
			var resp = await _tokenClient.RefreshTokenAsync(req);
			if (resp != null && resp.Msg != "ok")
			{
				//_lastErrorMsg = resp.Msg;
			}
			lastRefreshed = DateTime.Now;
			return resp;
		}
	}



	/// <summary>
	/// refit auth handler for handling SP api tokens
	/// right, we need to call GetTokenAsync first....
	/// </summary>
	public class SpRestAuthHandler : DelegatingHandler
	{
		private readonly ISPTokenService _tokenService;
		public SpRestAuthHandler(ISPTokenService tokenService)
		{
			_tokenService = tokenService;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			TokenResponse token = await _tokenService.GetTokenAsync();
			HttpResponseMessage resp = null;

			for (int cnt = 0; cnt < 2 && !cancellationToken.IsCancellationRequested; cnt++)
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
				resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

				if (resp?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					resp?.Dispose(); // I think we need to do this
					token = await _tokenService.GetTokenAsync(true); // we really should never get here
				}
				else
					break;
			}

			return resp;
		}
	}

}
