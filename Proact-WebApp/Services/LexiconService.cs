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

    public static class LexiconServiceExtensions {
        public static void AddLexiconService( this IServiceCollection services,
            IConfiguration configuration ) {
            services.AddHttpClient<ILexiconService, LexiconService>();
        }
    }

    public class LexiconService : BaseService, ILexiconService {

        public LexiconService(
            ITokenAcquisition tokenAcquisition,
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor )
            : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<List<LexiconModel>> GetAsync() {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Lexicons" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                List<LexiconModel> lexiconList = JsonConvert
                    .DeserializeObject<List<LexiconModel>>( content );

                return lexiconList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<LexiconModel> GetAsync( string id ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/Lexicons/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

               LexiconModel model = JsonConvert
                    .DeserializeObject<LexiconModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<LexiconModel> CreateAsync( LexiconCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/Lexicons/Create", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var projectModel
                    = JsonConvert
                    .DeserializeObject<LexiconModel>( content );

                return projectModel;
            }

            string errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<bool> Delete( string id ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
               .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/Lexicons/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> Validate( LexiconPublishRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
             = JsonConvert.SerializeObject( request );
            var jsoncontent
                = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await _httpClient
                 .PutAsync( $"{ _apiBaseAddress }/{ _culture }/Lexicons/{request.LexiconId}/Publish", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<List<LexiconModel>> GetPublishedAsync() {
            var lexicons = await GetAsync();
            return lexicons
                .Where( x => x.State == LexiconState.PUBLISHED )
                .ToList();
        }
    }
}
