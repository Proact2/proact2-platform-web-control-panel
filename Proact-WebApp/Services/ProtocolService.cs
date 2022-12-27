using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Proact_WebApp.Models;

namespace Proact_WebApp {


    public static class ProtocolServiceExtensions {
        public static void AddProtocolService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IProtocolService, ProtocolService>();
        }
    }

    public class ProtocolService : BaseService, IProtocolService {

        public ProtocolService(
           ITokenAcquisition tokenAcquisition,
           HttpClient httpClient,
           IConfiguration configuration,
           IHttpContextAccessor contextAccessor )
           : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<ProtocolModel> GetProtocolAssignedToProject( string id ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Protocols/Project/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                ProtocolModel model = JsonConvert
                    .DeserializeObject<ProtocolModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<ProtocolModel> GetProtocolAssignedToPatient( string userId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Protocols/Patient/{userId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                ProtocolModel model = JsonConvert
                    .DeserializeObject<ProtocolModel>( content );

                return model;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }


        public async Task<bool> CreateProtocolAssignedToProjectAsync( CreateProtocolAssignedToProjectRequest request ) {
            await PrepareAuthenticatedClient();

            var content = new MultipartFormDataContent();
            content.Add( new StringContent( request.Name ),
                nameof( request.Name ) );
            content.Add( new StringContent( request.InternalCode ),
                nameof( request.InternalCode ) );
            content.Add( new StringContent( request.InternationalCode ),
                nameof( request.InternationalCode ) );
            content.Add( new StreamContent( request.PdfFile.OpenReadStream() ),
                nameof( request.PdfFile ),
                request.PdfFile.FileName );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Protocols/Project/{request.ProjectId}", content );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

       
        public async Task<bool> CreateProtocolAssignedToPatientAsync( CreateProtocolAssignedToPatientRequest request ) {
            await PrepareAuthenticatedClient();

            var content = new MultipartFormDataContent();
            content.Add( new StringContent( request.Name ),
                nameof( request.Name ) );
            content.Add( new StringContent( request.InternalCode ),
                nameof( request.InternalCode ) );
            content.Add( new StringContent( request.InternationalCode ),
                nameof( request.InternationalCode ) );
            content.Add( new StreamContent( request.PdfFile.OpenReadStream() ),
                nameof( request.PdfFile ),
                request.PdfFile.FileName );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Protocols/Patient/{request.UserId}", content );

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
