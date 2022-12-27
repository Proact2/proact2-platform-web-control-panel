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

    public static class EmailEditorServiceExtensions {
        public static void AddEmailEditorService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IEmailEditorService, EmailEditorService>();
        }
    }

    public class EmailEditorService : BaseService, IEmailEditorService {
       
        public EmailEditorService( ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor )
            : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }


        public async Task<HtmlContentModel> GetUserDeactivatedEmailBody( string projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{projectId}/UserDeactivated" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                HtmlContentModel model = JsonConvert
                    .DeserializeObject<HtmlContentModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<HtmlContentModel> GetUserSuspendedEmailBody( string projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{projectId}/UserSuspended" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                HtmlContentModel model = JsonConvert
                    .DeserializeObject<HtmlContentModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<HtmlContentModel> GetWelcomeEmailBody( string projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{projectId}/UserWelcome" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                HtmlContentModel model = JsonConvert
                    .DeserializeObject<HtmlContentModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> SetUserDeactivatedEmailBody( HtmlContentRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{request.ProjectId}/UserDeactivated", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> SetUserSuspendedEmailBody( HtmlContentRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{request.ProjectId}/UserSuspended", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> SetWelcomeEmailBody( HtmlContentRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectEmailsBodies/{request.ProjectId}/UserWelcome", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        //public static class ContactMock {

        public static HtmlContentModel GetMock() {
            var model = new HtmlContentModel() {
                HtmlContent = "<ul><li>Morbi in sem quis dui placerat ornare. Pellentesque odio nisi, euismod in, pharetra a, ultricies in, diam. Sed arcu. Cras consequat.</li><li>Praesent dapibus, neque id cursus faucibus, tortor neque egestas augue, eu vulputate magna eros eu erat. Aliquam erat volutpat. Nam dui mi, tincidunt quis, accumsan porttitor, facilisis luctus, metus.</li><li>Phasellus ultrices nulla quis nibh. Quisque a lectus. Donec consectetuer ligula vulputate sem tristique cursus. Nam nulla quam, gravida non, commodo a, sodales sit amet, nisi.</li><li>Pellentesque fermentum dolor. Aliquam quam lectus, facilisis auctor, ultrices ut, elementum vulputate, nunc.</li></ul>",
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid()
            };
            return model;
        }
    }

}
