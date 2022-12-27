using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace Proact_WebApp {

    public static class ProactMobileSettingsServicextensions {
        public static void AddProactMobileSettings( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IProactMobileSettingsService, ProactMobileSettingsService>();
        }
    }

    public class ProactMobileSettingsService : BaseService , IProactMobileSettingsService{

        public ProactMobileSettingsService(
             ITokenAcquisition tokenAcquisition,
             HttpClient httpClient,
             IConfiguration configuration,
             IHttpContextAccessor contextAccessor )
         : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
            }

        public async Task<RequiredUpdateModel> GetLastAppVersion() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ApplicationVersion" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                RequiredUpdateModel model = JsonConvert
                    .DeserializeObject<RequiredUpdateModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> SetLastAppVersion( RequiredUpdateModel request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
               = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/ApplicationVersion", jsoncontent );

            return response.StatusCode == HttpStatusCode.OK;
            
            throw new HttpRequestException(
               $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }
    }
}
