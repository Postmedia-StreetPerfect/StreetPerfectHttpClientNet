using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using StreetPerfect;
using StreetPerfect.Models;
using Refit;


namespace Microsoft.Extensions.DependencyInjection
{
	public static class SPConfigServiceCollectionExtensions
	{
		public static IServiceCollection AddStreetPerfectClient(
			 this IServiceCollection services, Action<StreetPerfectRestClientConfig> setconf)
		{
			var config = new StreetPerfectRestClientConfig();
			setconf(config);
			StreetPerfect.SPTokenService._clientId = config.ClientId;
			StreetPerfect.SPTokenService._clientSecret = config.ClientSecret;

			if (config.ApiVersion < 1)
				config.ApiVersion = 1;

			// user can set ANY base address themselves
			if (String.IsNullOrWhiteSpace(config.BaseAddress))
			{
				config.BaseAddress = config.UseDevEndpoint ? "https://apidev.streetperfect.com/api" : "https://api.streetperfect.com/api";
			}
			else
			{
				if (!config.BaseAddress.ToLower().TrimEnd('/').EndsWith("/api"))
					config.BaseAddress += "/api";
			}

			services.AddSingleton<ISPTokenService, SPTokenService>();

			// token endpoints have no version component
			services.AddRefitClient<IStreetPerfectTokenClient>().ConfigureHttpClient(c =>
			{
				c.BaseAddress = new Uri(config.BaseAddress);
			});

			services.AddTransient<SpRestAuthHandler>();
			services.AddRefitClient<IStreetPerfectRestClient>().ConfigureHttpClient(c =>
			{
				c.BaseAddress = new Uri($"{config.BaseAddress}/{config.ApiVersion}");
			}).AddHttpMessageHandler<SpRestAuthHandler>();

			return services;
		}
	}
}

namespace StreetPerfect
{
	public class StreetPerfectRestClientConfig
	{
		public string ClientId { get; set; } // deprecated...
		public string ClientSecret { get; set; }
		public string BaseAddress { get; set; } // this takes priority
		public bool UseDevEndpoint { get; set; } = false; // default is PRODUCTION
		public int ApiVersion { get; set; } = 1; // default is 1
	}


	/// <summary>
	/// refit interface for StreetPerfect rest api
	/// </summary>
	public interface IStreetPerfectRestClient
	{
		[Post("/ca/typeahead")]
		public Task<caTypeaheadResponse> caTypeahead(caTypeaheadRequest req);

		[Post("/ca/typeahead/rec")]
		public Task<caTypeaheadResponse> caTypeaheadRec(caTypeaheadRequest req);

		[Post("/ca/typeahead/fetch")]
		public Task<caTypeaheadFetchResponse> caTypeaheadFetch(caTypeaheadFetchRequest req);

		[Post("/ca/fetch")]
		public Task<caFetchAddressResponse> caFetchAddress(caFetchAddressRequest req);

		[Post(path: ("/ca/format"))]
		public Task<caFormatAddressResponse> caFormatAddress(caFormatAddressRequest req);

		[Post(path: ("/ca/correction"))]
		public Task<caCorrectionResponse> caProcessCorrection(caAddressRequest req);

		[Post(path: ("/ca/parse"))]
		public Task<caParseResponse> caProcessParse(caAddressRequest req);

		[Post(path: ("/ca/search"))]
		public Task<caSearchResponse> caProcessSearch(caAddressRequest req);

		[Post(path: ("/ca/query"))]
		public Task<caQueryResponse> caQuery(caQueryRequest req);

		[Post(path: ("/ca/validate"))]
		public Task<caValidateAddressResponse> caValidateAddress(caValidateAddressRequest req);

		[Get(path: ("/ca/query"))]
		public Task<GetInfoResponse> GetInfo();

		[Post(path: ("/us/correection"))]
		public Task<usCorrectionResponse> usProcessCorrection(usAddressRequest req);

		// just realized I haven't implemented this
		[Post(path: ("/us/x"))]
		public Task<usDeliveryInformationResponse> usProcessDeliveryInfo(usAddressRequest req);

		[Post(path: ("/us/parse"))]
		public Task<usParseResponse> usProcessParse(usAddressRequest req);

		[Post(path: ("/us/search"))]
		public Task<usSearchResponse> usProcessSearch(usAddressRequest req);

	}

	/// <summary>
	/// I had to create the token stuff separte so it wouldn't go through the same handler 
	/// -- that I use to call them!
	/// end-user doesn't need to call these directly
	/// </summary>
	public interface IStreetPerfectTokenClient
	{
		[Post("/token")]
		public Task<TokenResponse> GetToken(TokenRequest req);

		[Post("/token/refresh")]
		public Task<TokenResponse> RefreshToken(TokenRefreshRequest req);
	}


	/// <summary>
	/// This service contains the token client - via injection
	/// it is injected in to the SpRestAuthHandler 
	/// The SpRestAuthHandler grabs a token from it and adds to the header of every call
	/// </summary>
	public interface ISPTokenService
	{
		public Task<TokenResponse> GetToken(bool fForce = false);
		public Task<TokenResponse> RefreshToken();
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

		public async Task<TokenResponse> GetToken(bool fForce = false)
		{
			if (_token == null || fForce)
			{
				_token = await _tokenClient.GetToken(new TokenRequest() { ClientId=_clientId, ClientSecret=_clientSecret});
				lastRefreshed = DateTime.Now;
			}
			else if ((DateTime.Now - lastRefreshed).Minutes > _token.Expires - 2)
			{
				_token = await RefreshToken();
			}
			return _token;
		}

		public async Task<TokenResponse> RefreshToken()
		{
			var req = new TokenRefreshRequest()
			{
				AccessToken = _token.AccessToken,
				RefreshToken = _token.RefreshToken,
			};
			var resp = await _tokenClient.RefreshToken(req); 
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
	/// right, we need to call GetToken first....
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
			TokenResponse token = await _tokenService.GetToken();
			HttpResponseMessage resp = null;

			for (int cnt = 0; cnt < 2 && !cancellationToken.IsCancellationRequested; cnt++)
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
				resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

				if (resp?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					resp?.Dispose(); // I think we need to do this
					token = await _tokenService.GetToken(true); // we really should never get here
				}
				else
					break;
			}

			return resp;
		}
	}

}
