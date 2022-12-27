using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Proact_WebApp.Service {

    public static class ProjectsServiceExtensions {
        public static void AddProjectService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IProjectService, ProjectsService>();
        }
    }

    public class ProjectsService : BaseService, IProjectService {

        public ProjectsService(
            ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor )
            : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<List<ProjectModel>> GetAllAsync() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Projects" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                
                List<ProjectModel> ProjectList = JsonConvert
                    .DeserializeObject<List<ProjectModel>>( content );

                return ProjectList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<List<ProjectModel>> GetOpenProjectAsync() {
            var projects =  await GetAllAsync();
            var openProjects = projects
                .Where( x => x.Status == ProjectState.Open )
                .ToList();
            return openProjects;
        }

        public async Task<List<ProjectModel>> GetClosedProjectAsync() {
            var projects = await GetAllAsync();
            var closedProjects = projects
                .Where( x => x.Status == ProjectState.Open )
                .ToList();
            return closedProjects;
        }

        public async Task<ProjectModel> GetAsync( string id ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Projects/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                ProjectModel projectDetail = JsonConvert
                    .DeserializeObject<ProjectModel>( content );

                return projectDetail;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<ProjectModel> CreateAsync( ProjectCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Projects", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var projectModel
                    = JsonConvert
                    .DeserializeObject<ProjectModel>( content );

                return projectModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> DeleteAsync( Guid projectId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
              .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/Projects/{projectId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<ProjectModel> UpdateAsync( ProjectUpdateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
               = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/Projects/{request.ProjectId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var projectModel
                    = JsonConvert
                    .DeserializeObject<ProjectModel>( content );

                return projectModel;
            }

            throw new HttpRequestException(
               $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> CloseAsync( string projectId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/" +
               $"{ _culture }/" +
               $"Projects/" +
               $"{projectId}/" +
               $"Close";

            var response = await _httpClient
               .PutAsync( url, null );

            if ( response.IsSuccessStatusCode ) {
                return true;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async  Task<bool> OpenAsync( string projectId ) {
            await PrepareAuthenticatedClient();
            var url = $"{ _apiBaseAddress }/" +
              $"{ _culture }/" +
              $"Projects/" +
              $"{projectId}/" +
              $"Open";

            var response = await _httpClient
               .PutAsync( url, null );

            if ( response.IsSuccessStatusCode ) {
                return true;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

       
    }
}
