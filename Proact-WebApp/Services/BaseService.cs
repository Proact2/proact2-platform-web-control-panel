using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace Proact_WebApp {
    public class BaseService {

        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly HttpClient _httpClient;
        protected readonly string _webappScope = string.Empty;
        protected readonly string _apiBaseAddress = string.Empty;
        protected readonly ITokenAcquisition _tokenAcquisition;

        protected string _culture = "en-US"; //TOdo implementare i18n


        public BaseService( ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor ) {

            _httpClient = httpClient;
            _tokenAcquisition = tokenAcquisition;
            _contextAccessor = contextAccessor;
            _webappScope = configuration["ProactWebAppScope"];
            _apiBaseAddress = configuration["ProactApiBaseAddress"];
        }

        protected async Task PrepareAuthenticatedClient() {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync( new[] { _webappScope } );
            Debug.WriteLine( $"access token-{accessToken}" );
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", accessToken );
            _httpClient.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
        }

        protected StringContent SerializeRequest( object request ) {
            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent
                = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            return jsoncontent;
        }

        protected async Task<T> DeserializeResponse<T>( HttpResponseMessage response ) {
            var content
                = await response.Content.ReadAsStringAsync();
            var deserializedContent
                = JsonConvert.DeserializeObject<T>( content );

            return deserializedContent;
        }

        protected async Task<string> GetContentError( HttpResponseMessage response ) {
            var content
                = await response.Content.ReadAsStringAsync();
            return content;
        }

    }
}
