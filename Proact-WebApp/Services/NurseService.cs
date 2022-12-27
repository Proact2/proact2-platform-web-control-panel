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

    public static class NursesServiceExtensions {
        public static void AddNursesService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<INursesService, NursesService>();
        }
    }

    public class NursesService : BaseService, INursesService {

        public NursesService( ITokenAcquisition tokenAcquisition,
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor contextAccessor )
        : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        private string BaseUrl {
            get {
                return $"{ _apiBaseAddress }/{ _culture }/Nurses";
            }
        }

        public async Task<List<NurseModel>> GetAllNurse() {
            var url = $"{ BaseUrl}/all";
            return await GetNursesAsync( url );
        }

        public async Task<List<NurseModel>> GetNursesUnassigned() {
            var list = await GetAllNurse();
            if ( list != null ) {
                return list.FindAll( x => !x.IsAssociatedToMedicalTeam ).ToList();
            }
            else {
                return new List<NurseModel>();
            }
        }

        public async Task<List<NurseModel>> GetNurseAssignedToMedicalTeam(
            string medicalTeamID ) {
            var url = $"{ BaseUrl}/{medicalTeamID}";
            return await GetNursesAsync( url );
        }

        private async Task<List<NurseModel>> GetNursesAsync( string url ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                List<NurseModel> nurses = JsonConvert
                    .DeserializeObject<List<NurseModel>>( content );

                return nurses.OrderBy( x => x.Name ).ToList();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<NurseModel> CreateNurse( UserCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ BaseUrl}/create";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var patientModel
                    = await DeserializeResponse<NurseModel>( response );
                return patientModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<NurseModel> AssignNurseToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ BaseUrl}/{medicalTeamId}/assign";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var nurseModel
                    = await DeserializeResponse<NurseModel>( response );
                return nurseModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

    
        public async Task<bool> RemoveNurseFromMedicalTeam(
            string medicalTeamId, string userId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ BaseUrl}/{ medicalTeamId }/{userId}";
            var response = await _httpClient.DeleteAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }
    }
}
