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

    public static class PatientsServiceExtensions {
        public static void AddPatientsService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IPatientsService, PatientsService>();
        }
    }

    public class PatientsService : BaseService, IPatientsService {

        public PatientsService( ITokenAcquisition tokenAcquisition,
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor contextAccessor )
        : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        private string BaseUrl {
            get {
                return $"{ _apiBaseAddress }/{ _culture }/Patient";
            }
        }

        public async Task<List<PatientModel>> GetAllPatients() {
            var url = $"{ BaseUrl}/all";
            return await GetPatientsAsync( url );
        }

        public async Task<List<PatientModel>> GetUnassignedPatients() {
            var list = await GetAllPatients();
            if ( list != null ) {
                return list.FindAll( x => !x.IsAssociatedToMedicalTeam ).ToList();
            }
            else {
                return new List<PatientModel>();
            }
        }

        public async Task<List<PatientModel>> GetPatientsAssignedToMedicalTeam(
           string medicalTeamID ) {
            var url = $"{ BaseUrl}/{medicalTeamID}";
            return await GetPatientsAsync( url );
        }

        public async Task<List<PatientModel>> GetPatientsAssignedToProject(
            string projectId ) {
            var url = $"{ BaseUrl}/projects/{projectId}";
            return await GetPatientsAsync( url );
        }

        public async Task<PatientModel> CreatePatient( PatientCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ BaseUrl}/create";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var patientModel
                    = await DeserializeResponse<PatientModel>( response );
                return patientModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<PatientModel> AssignPatientToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );
            var url = $"{ BaseUrl}/{medicalTeamId}/assign";
            var response = await _httpClient.PostAsync( url, jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var patientModel
                    = await DeserializeResponse<PatientModel>( response );
                return patientModel;
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        private async Task<List<PatientModel>> GetPatientsAsync( string url ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient.GetAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                List<PatientModel> medics = JsonConvert
                    .DeserializeObject<List<PatientModel>>( content );

                return medics.OrderBy( x => x.Name ).ToList();
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException( errorMessage, null,
                response.StatusCode );
        }

        public async Task<bool> RemovePatientFromMedicalTeam(
            string medicalTeamId, string userId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/{ _culture }/Patient/{ medicalTeamId }/{userId}";
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
