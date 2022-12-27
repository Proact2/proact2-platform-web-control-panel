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

    public static class MedicsServiceExtensions {
        public static void AddMedicsService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IMedicsService, MedicsService>();
        }
    }

    public class MedicsService : BaseService, IMedicsService {

        public MedicsService( ITokenAcquisition tokenAcquisition,
         HttpClient httpClient,
         IConfiguration configuration,
         IHttpContextAccessor contextAccessor )
         : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<List<MedicModel>> GetAllMedics() {
            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/all";
            return await GetMedicsAsync( url );
        }

        public async Task<List<MedicModel>> GetMedicsAssignedToMedicalTeam( string medicalTeamID ) {
            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/{medicalTeamID}";
            return await GetMedicsAsync( url );
        }

        public async Task<List<MedicModel>> GetMedicsUnassigned() {
            var list = await GetAllMedics();
            if ( list != null ) {
                return list.FindAll( x => !x.IsAssociatedToMedicalTeam ).ToList();
            }
            else {
                return new List<MedicModel>();
            }
        }

        public async Task<MedicModel> CreateMedic( UserCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/create";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var medicModel
                    = await DeserializeResponse<MedicModel>( response );
                return medicModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<MedicModel> AssignMedicToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/{medicalTeamId}/assign";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var medicModel
                    = await DeserializeResponse<MedicModel>( response );
                return medicModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<bool> RemoveMedicFromMedicalTeam(
            string medicalTeamId, string userId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/{ medicalTeamId }/{userId}";
            var response = await _httpClient.DeleteAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<MedicModel> CreateMedicWithinMedicalTeamAsync(
            string medicalTeamId, AssignUserToMedicalTeamRequest request ) {
            await PrepareAuthenticatedClient(); 

            var jsonContent = SerializeRequest( request );
            var url = $"{ _apiBaseAddress }/{ _culture }/Medics/{ medicalTeamId }";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var meedicModel
                    = await DeserializeResponse<MedicModel>( response );
                return meedicModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<List<MedicModel>> GetMedicsAsync( string url ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync( url);

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                List<MedicModel> medics = JsonConvert
                    .DeserializeObject<List<MedicModel>>( content );

                return medics.OrderBy(x => x.Name).ToList();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        
    }
}
