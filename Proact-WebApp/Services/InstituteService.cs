using System;
using System.Collections.Generic;
using System.IO;
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

    public static class InstituteServiceExtensions {
        public static void AddInstituteService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<IInstituteService, InstituteService>();
        }
    }

    public class InstituteService : BaseService, IInstituteService {

        public InstituteService(
           ITokenAcquisition tokenAcquisition,
           HttpClient httpClient,
           IConfiguration configuration,
           IHttpContextAccessor contextAccessor )
           : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }


        public async Task<List<InstituteModel>> GetAllAsync() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                List<InstituteModel> ProjectList = JsonConvert
                    .DeserializeObject<List<InstituteModel>>( content );

                return ProjectList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<List<InstituteModel>> GetOpenAsync() {
            var allInstitutes = await GetAllAsync();
            return allInstitutes.Where( x => x.State == InstituteState.Open ).ToList();
        }

        public async Task<List<InstituteModel>> GetClosedAsync() {
            var allInstitutes = await GetAllAsync();
            return allInstitutes.Where( x => x.State == InstituteState.Closed ).ToList();
        }

        public async Task<InstituteModel> GetAsync( string id ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                InstituteModel model = JsonConvert
                    .DeserializeObject<InstituteModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<InstituteModel> GetInstituteWhereCurrentUserIsAdmin() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes/Admin/me" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                InstituteModel indtituteDetail = JsonConvert
                    .DeserializeObject<InstituteModel>( content );

                return indtituteDetail;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<InstituteModel> CreateAsync( CreateInstituteRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var model
                    = JsonConvert
                    .DeserializeObject<InstituteModel>( content );

                return model;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<InstituteModel> UpdateAsync( UpdateInstituteRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes/{request.Id}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var model
                    = JsonConvert
                    .DeserializeObject<InstituteModel>( content );

                return model;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> CloseAsync( string id ) {
            await PrepareAuthenticatedClient();

            var url = $"{ _apiBaseAddress }/" +
               $"{ _culture }/" +
               $"Institutes/" +
               $"{id}/" +
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

        public async Task<bool> OpenAsync( string id ) {
            await PrepareAuthenticatedClient();
            var url = $"{ _apiBaseAddress }/" +
              $"{ _culture }/" +
              $"Institutes/" +
              $"{id}/" +
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

        public async Task<bool> CreateAdmin( InstituteAdminCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Institutes/{request.InstituteId}/Admin", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }


        public async Task<bool> AddTermsAndConditions( CreateInstituteDocumentRequest request ) {
            await PrepareAuthenticatedClient();

            var content = new MultipartFormDataContent();
            content.Add( new StringContent( request.Title ), nameof( request.Title ) );
            content.Add( new StringContent( request.Description ), nameof( request.Description ) );
            content.Add( new StreamContent( request.PdfFile.OpenReadStream() ), nameof( request.PdfFile ), request.PdfFile.FileName );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Documents/Terms", content );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> AddPrivacyPolicy( CreateInstituteDocumentRequest request ) {
            await PrepareAuthenticatedClient();

            var content = new MultipartFormDataContent();
            content.Add( new StringContent( request.Title ), nameof( request.Title ) );
            content.Add( new StringContent( request.Description ), nameof( request.Description ) );
            content.Add( new StreamContent( request.PdfFile.OpenReadStream() ), nameof( request.PdfFile ), request.PdfFile.FileName );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Documents/Privacy", content );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> UpdateTermsAndConditions( CreateInstituteDocumentRequest request ) {
            return await AddTermsAndConditions( request );
        }

        public async Task<bool> UpdatePrivacyPolicy( CreateInstituteDocumentRequest request ) {
            return await AddPrivacyPolicy( request );
        }

    }
}


