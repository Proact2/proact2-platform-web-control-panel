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

    public static class SurveyServiceExtensions {
        public static void AddSurveysService(
            this IServiceCollection services, IConfiguration configuration ) {
            services.AddHttpClient<ISurveysService, SurveysService>();
        }
    }

    public class SurveysService : BaseService, ISurveysService {

        private const string _surveyServiceUrlName = "Survey";
        private const string _surveyAssignationsServiceUrlName = "SurveyAssegnations";

        public SurveysService( ITokenAcquisition tokenAcquisition,
              HttpClient httpClient,
              IConfiguration configuration,
              IHttpContextAccessor contextAccessor )
              : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<List<SurveyModel>> GetSurveysAsync( string projectId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient    
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyServiceUrlName}/{projectId}/all" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                List<SurveyModel> surveysSetsList = JsonConvert
                    .DeserializeObject<List<SurveyModel>>( content );

                return surveysSetsList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyModel> GetSurveyAsync( string surveyId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyServiceUrlName}/{surveyId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                SurveyModel model = JsonConvert
                    .DeserializeObject<SurveyModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyModel> CreateSurvey(
          SurveyCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyServiceUrlName }/{request.ProjectId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var surveyModel
                    = JsonConvert
                    .DeserializeObject<SurveyModel>( content );

                return surveyModel;
            }

            string errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<SurveyModel> UpdateSurvey(
           SurveyEditRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyServiceUrlName }", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var surveyModel
                    = JsonConvert
                    .DeserializeObject<SurveyModel>( content );

                return surveyModel;
            }

            string errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public Task<QuestionModel> AddQuestion(
            AddRemoveQuestionFromSurveyRequest request ) {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveQuestion(
            AddRemoveQuestionFromSurveyRequest request ) {

            await PrepareAuthenticatedClient();
            var response = await _httpClient
              .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/{_surveyServiceUrlName}/{request.SurveyId}/{request.QuestionsSetId}/{request.QuestionId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }
            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> DeleteSurvey( string id ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
              .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/{_surveyServiceUrlName}/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }
            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> PerformAssignations(
            SurveyAssignationsRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyAssignationsServiceUrlName }/{request.SurveyId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );
            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<List<SurveyAssignationModel>> GetPatientsAssignedToSurvey( string id ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyAssignationsServiceUrlName}/{id}/Patients" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                List<SurveyAssignationModel> model = JsonConvert
                    .DeserializeObject<List<SurveyAssignationModel>>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyCompiledModel> GetCompiledSurvey( string assignationId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyAssignationsServiceUrlName}/{assignationId}/Compiled/" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                SurveyCompiledModel model = JsonConvert
                    .DeserializeObject<SurveyCompiledModel>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyStatsResume> GetSurveyStatsResume( string surveyId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _surveyServiceUrlName}/{surveyId}/Stats" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                SurveyStatsResume model = JsonConvert
                    .DeserializeObject<SurveyStatsResume>( content );

                return model;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }
    }

}
