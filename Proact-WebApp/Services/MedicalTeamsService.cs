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

namespace Proact_WebApp {

    public static class MedicalTeamsServiceExtensions {
        public static void AddMedicalTeamsService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IMedicalTeamsService, MedicalTeamsService>();
        }
    }

    public class MedicalTeamsService : BaseService, IMedicalTeamsService {

        public MedicalTeamsService(
            ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor ) : base(
                tokenAcquisition,
                httpClient,
                configuration,
                contextAccessor ) {
        }

        public async Task<IEnumerable<MedicalTeamModel>> GetAllAsync( string projectId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/{ projectId }";
            var response = await _httpClient.GetAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var medicalTeams
                    = await DeserializeResponse<IEnumerable<MedicalTeamModel>>( response );

                return medicalTeams;
            }

            string errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<IEnumerable<MedicalTeamModel>> GetOpenMedicalTeamsAsync( string projectId ) {
            var medicalTeams = await GetAllAsync( projectId );
            var openMedicalTeams = medicalTeams
                .Where( x => x.State == MedicalTeamState.Open )
                .ToList();

            return openMedicalTeams;
        }

        public async Task<IEnumerable<MedicalTeamModel>> GetClosedMedicalTeamsAsync( string projectId ) {
            var medicalTeams = await GetAllAsync( projectId );
            var closedMedicalTeams = medicalTeams
            .Where( x => x.State != MedicalTeamState.Open )
            .ToList();

            return closedMedicalTeams;
        }

        public async Task<List<MedicalTeamModel>> GetMedicalTeamsWhereCurrentUserIsAdmin() {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/admins/my";
            var response = await _httpClient.GetAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var medicalTeams
                    = await DeserializeResponse<List<MedicalTeamModel>>( response );
                return medicalTeams;
            }

            throw new HttpRequestException(
                response.StatusCode.ToString(),
                null,
                response.StatusCode );
        }

        public async Task<MedicalTeamModel> CreateAsync( string projectId, MedicalTeamCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/{ projectId }", jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var medicalTeamModel
                    = await DeserializeResponse<MedicalTeamModel>( response );

                return medicalTeamModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<IEnumerable<UserModel>> AssignMedicalTeamAdministrator(
            string projectId,
            string medicalTeamId,
            MedicalTeamAssignAdminRequest request ) {

            await PrepareAuthenticatedClient();

            var jsonContent = SerializeRequest( request );

            var response = await _httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/{ projectId }/{ medicalTeamId }/setadmin?userId={request.UserId}", jsonContent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var userModel
                    = await DeserializeResponse<IEnumerable<UserModel>>( response );

                return userModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> DeleteAsync( string projectId, string medicalTeamId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/{projectId}/{medicalTeamId}" );


            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<MedicalTeamModel> DetailsAsync( string projectId, string medicalTeamId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/MedicalTeam/{projectId}/{medicalTeamId}" );

            if( response.IsSuccessStatusCode ) {

                var medicalTeamModel
                    = await DeserializeResponse<MedicalTeamModel>( response );

                return medicalTeamModel;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<IEnumerable<UserModel>> GetAdminsAsync(
            string projectId,
            string medicalTeamId ) {

            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/" +
                $"{ _culture }/" +
                $"MedicalTeam/" +
                $"{projectId}/" +
                $"{medicalTeamId}/" +
                $"GetAdmins" );

            if ( response.IsSuccessStatusCode ) {
                var admins = await DeserializeResponse<IEnumerable<UserModel>>( response );

                return admins;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<MedicalTeamModel> UpdateAsync(
            MedicalTeamEditRequest request ) {

            await PrepareAuthenticatedClient();

            var content = SerializeRequest( request );

            var response = await _httpClient
               .PutAsync( $"{ _apiBaseAddress }/" +
               $"{ _culture }/" +
               $"MedicalTeam/" +
               $"{request.ProjectId}/" +
               $"{request.MedicalTeamId}",
               content );

            if ( response.IsSuccessStatusCode ) {
                var medicalTeamModel
                    = await DeserializeResponse<MedicalTeamModel>( response );

                return medicalTeamModel;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> CloseAsync( string projectId, string medicalTeamId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
               .PutAsync( $"{ _apiBaseAddress }/" +
               $"{ _culture }/" +
               $"MedicalTeam/" +
               $"{projectId}/" +
               $"{medicalTeamId}/" +
               $"close",
               null );

            if ( response.IsSuccessStatusCode ) {
                return true;
            }

            var errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> OpenAsync( string projectId, string medicalTeamId ) {
            await PrepareAuthenticatedClient();

            // var content = SerializeRequest( request );

            var response = await _httpClient
               .PutAsync( $"{ _apiBaseAddress }/" +
               $"{ _culture }/" +
               $"MedicalTeam/" +
               $"{projectId}/" +
               $"{medicalTeamId}/" +
               $"open",
               null );

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
