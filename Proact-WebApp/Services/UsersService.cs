using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Proact_WebApp.Models;

namespace Proact_WebApp {

    public static class UsersServiceExtensions {
        public static void AddUsersService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IUsersService, UsersService>();
        }
    }

    public class UsersService : BaseService, IUsersService {

        public UsersService( ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor )
            : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        private string BaseUrl {
            get {
                return $"{ _apiBaseAddress }/{ _culture }/Users";
            }
        }

        public async Task<IEnumerable<UserModel>> GetAsync() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ BaseUrl }" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<UserModel> usersList = JsonConvert
                    .DeserializeObject<IEnumerable<UserModel>>( content );

                return usersList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> AssignRole( AssignRoleToUserRequest request ) {
            await PrepareAuthenticatedClient();

            var serielizedRequest = SerializeRequest( request );
            var response = await _httpClient
                .PostAsync( $"{ BaseUrl }/AssignRole",
                serielizedRequest );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            var errorMessage
                = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<UserModel> CreateAsync( UserCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var serielizedRequest = SerializeRequest( request );
            var response = await _httpClient
                .PostAsync( $"{ BaseUrl }",
                serielizedRequest );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var userModel
                    = JsonConvert
                    .DeserializeObject<UserModel>( content );

                return userModel;
            }

            var errorMessage
                = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> DeleteUser( string userId ) {
            await PrepareAuthenticatedClient();

            var url = $"{ BaseUrl}/{userId}";
            var response = await _httpClient.DeleteAsync( url );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
                response.StatusCode.ToString(),
                null,
                response.StatusCode );
        }

        public async Task<UserModel> GetCurrentUser() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ BaseUrl }/me" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                UserModel currentUser = JsonConvert
                    .DeserializeObject<UserModel>( content );

                return currentUser;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> SuspendUser( string userId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .PutAsync( $"{ BaseUrl }/suspend/{userId}", null );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> ActivateUser( string userId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .PutAsync( $"{ BaseUrl }/activate/{userId}", null );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> DeactivateUser( string userId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .PutAsync( $"{ BaseUrl }/deactivate/{userId}", null );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }
    }
}
