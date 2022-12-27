using System;
using System.Collections.Generic;
using System.Linq;
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

    public static class ResearcherServiceExtensions {
        public static void AddResearcherService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IResearcherService, ResearcherService>();
        }
    }

    public class ResearcherService : BaseService, IResearcherService {

        public ResearcherService( ITokenAcquisition tokenAcquisition,
           HttpClient httpClient,
           IConfiguration configuration,
           IHttpContextAccessor contextAccessor )
           : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        private string BaseUrl {
            get {
                return $"{ _apiBaseAddress }/{ _culture }/Researchers";
            }
        }

        public async Task<List<ResearcherModel>> GetAll() {
            string url = $"{BaseUrl}/all";
            return await GetResearcherAsync( url );
        }

        public async Task<List<ResearcherModel>> GetUnassigned() {
            var list = await GetAll();
            if ( list != null ) {
                return list.FindAll( x => !x.IsAssociatedToMedicalTeam ).ToList();
            }
            else {
                return new List<ResearcherModel>();
            }
        }

        public async Task<List<ResearcherModel>> GetAssignedToMedicalTeam(
          string medicalTeamId ) {

            string url = $"{BaseUrl}/{medicalTeamId}";
            return await GetResearcherAsync( url );
        }

        public async Task<ResearcherModel> Create(
          UserCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{BaseUrl}/create";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var model
                    = await DeserializeResponse<ResearcherModel>( response );
                return model;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<ResearcherModel> AssignToMedicalTeam(
            AssignUserToMedicalTeamRequest request ) {

            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{BaseUrl}/{request.MedicalTeamId}/assign";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var model
                    = await DeserializeResponse<ResearcherModel>( response );
                return model;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }


        public async Task<bool> RemoveFromMedicalTeam(
            string medicalTeamId, string userId ) {

            await PrepareAuthenticatedClient();

            var url = $"{BaseUrl}/{ medicalTeamId }/{userId}";
            var response = await _httpClient.DeleteAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }


        private async Task<List<ResearcherModel>> GetResearcherAsync( string url ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                List<ResearcherModel> nurses = JsonConvert
                    .DeserializeObject<List<ResearcherModel>>( content );

                return nurses.OrderBy( x => x.Name ).ToList();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }
    }
}
