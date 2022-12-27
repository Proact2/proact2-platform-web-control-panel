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
using Proact_WebApp.Models;

namespace Proact_WebApp {

    public static class ContactServiceExtensions {
        public static void AddContactService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IContactService, ContactService>();
        }
    }

    public class ContactService : BaseService, IContactService {
        
        public ContactService( ITokenAcquisition tokenAcquisition,
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor contextAccessor )
        : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<HtmlContentModel> GetContact( string projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectContacts/{projectId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                HtmlContentModel model = JsonConvert
                    .DeserializeObject<HtmlContentModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> CreateContact( HtmlContentRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectContacts/{request.ProjectId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

    }

}
