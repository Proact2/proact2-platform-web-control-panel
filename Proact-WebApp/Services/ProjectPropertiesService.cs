using System;
using System.Collections.Generic;
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

    public static class ProjectsPropertiesServiceExtensions {
        public static void AddProjectPropertiesService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IProjectPropertiesService, ProjectPropertiesService>();
        }
    }

    public class ProjectPropertiesService : BaseService, IProjectPropertiesService {

        public ProjectPropertiesService(
         ITokenAcquisition tokenAcquisition,
         HttpClient httpClient,
         IConfiguration configuration,
         IHttpContextAccessor contextAccessor )
         : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<ProjectPropertiesModel> GetAsync( string projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectProperties/{projectId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                ProjectPropertiesModel properties = JsonConvert
                    .DeserializeObject<ProjectPropertiesModel>( content );

                return properties;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<ProjectPropertiesModel> UpdateAsync( ProjectPropertiesUpdateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
               = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectProperties/{request.Id}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var properties
                    = JsonConvert
                    .DeserializeObject<ProjectPropertiesModel>( content );

                return properties;
            }

            throw new HttpRequestException(
               $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> AssociateLexicon(
            AssociateLexiconToProjectRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
              = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/ProjectProperties/{request.ProjectId}/AssociateToProject", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
               $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

    }
}
