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

    public static class SurveyQuestionsSetsServiceExtensions {
        public static void AddSurveysQuestionsSetsService(
            this IServiceCollection services, IConfiguration configuration ) {
            services.AddHttpClient<ISurveysQuestionsSetsService, SurveysQuestionsSetsService>();
        }
    }

    public class SurveysQuestionsSetsService : BaseService, ISurveysQuestionsSetsService {

        private const string _setsServiceUrlName  = "SurveysQuestionsSets";
        private const string _answersBlockServiceUrlName = "SurveyAnswersBlocks";
        private const string _questionsServiceUrlName = "SurveysQuestions";

        public SurveysQuestionsSetsService( ITokenAcquisition tokenAcquisition,
              HttpClient httpClient,
              IConfiguration configuration,
              IHttpContextAccessor contextAccessor )
              : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public async Task<List<SurveyQuestionSetModel>> GetQuestionsSetsAsync( string projectId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{ _setsServiceUrlName}/{projectId}/all" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();

                var newcontent =content.Replace( @"\", @"" );

                List<SurveyQuestionSetModel> surveysSetsList = JsonConvert
                    .DeserializeObject<List<SurveyQuestionSetModel>>( newcontent );

                return surveysSetsList;
            }

            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyQuestionSetModel> GetQuestionsSetAsync( string questionsSetId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{_setsServiceUrlName}/{questionsSetId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                SurveyQuestionSetModel questionsSetDetails = JsonConvert
                    .DeserializeObject<SurveyQuestionSetModel>( content );
                return questionsSetDetails;
            }
            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyQuestionSetModel>
            CreateQuestionsSetAsync( SurveyQuestionSetCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _setsServiceUrlName }/{request.ProjectId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var surveysSetModel
                    = JsonConvert
                    .DeserializeObject<SurveyQuestionSetModel>( content );

                return surveysSetModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<SurveyQuestionSetModel> UpdateQuestionsSetAsync(
            SurveyQuestionSetUpdateRequest request ) {

            await PrepareAuthenticatedClient();

            var jsonRequest
               = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/{_setsServiceUrlName}/{request.QuestionSetId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var questionsSetModel
                    = JsonConvert
                    .DeserializeObject<SurveyQuestionSetModel>( content );

                return questionsSetModel;
            }

            throw new HttpRequestException(
               $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> DeleteQuestionsSet( string id ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
              .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/{_setsServiceUrlName}/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }


        private async Task<QuestionModel> AddQuestionToSet( SurveyAddQuestionToSetRequest request, SurveyQuestionType type ) {
            await PrepareAuthenticatedClient();

            string endpoint = string.Empty;
            string jsonRequest = string.Empty;
            switch ( type ) {
                case SurveyQuestionType.OpenAnswer:
                endpoint = "Open";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Boolean:
                endpoint = "Bool";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Mood:
                endpoint = "Mood";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Rating:
                endpoint = "Rating";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyAddRatingQuestionToSetRequest )request );
                break;
                case SurveyQuestionType.SingleChoice:
                endpoint = "SingleChoice";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyAddSingleChoiceQuestionToSetRequest )request );
                break;
                case SurveyQuestionType.MultipleChoice:
                endpoint = "MultipleChoice";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyAddMultipleChoiceQuestionToSetRequest )request );
                break;
            }

            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _questionsServiceUrlName }/{request.QuestionSetId}/{endpoint}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var questionModel
                    = JsonConvert
                    .DeserializeObject<QuestionModel>( content );

                return questionModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<QuestionModel> AddOpenQuestionToSet( SurveyAddQuestionToSetRequest request ) {
           return await AddQuestionToSet( request, SurveyQuestionType.OpenAnswer );
        }

        public async Task<QuestionModel> AddBoolQuestionToSet( SurveyAddQuestionToSetRequest request ) {
            return await AddQuestionToSet( request, SurveyQuestionType.Boolean );
        }

        public async Task<QuestionModel> AddMoodQuestionToSet( SurveyAddQuestionToSetRequest request ) {
            return await AddQuestionToSet( request, SurveyQuestionType.Mood );
        }

        public async Task<QuestionModel> AddRatingQuestionToSet( SurveyAddRatingQuestionToSetRequest request ) {
            return await AddQuestionToSet( request, SurveyQuestionType.Rating );
        }

        public async Task<QuestionModel> AddSingleChoiceQuestionToSet( SurveyAddSingleChoiceQuestionToSetRequest request ) {
            return await AddQuestionToSet( request, SurveyQuestionType.SingleChoice );
        }

        public async Task<QuestionModel> AddMultipleChoiceQuestionToSet( SurveyAddMultipleChoiceQuestionToSetRequest request ) {
            return await AddQuestionToSet( request, SurveyQuestionType.MultipleChoice );
        }

        public async Task<bool> RemoveQuestionFromSet(  string questionSetId, string questionId ) {
            await PrepareAuthenticatedClient();

            var response = await _httpClient
              .DeleteAsync( $"{ _apiBaseAddress }/{ _culture }/{_questionsServiceUrlName}/{questionSetId}/{questionId}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            throw new HttpRequestException(
              $"Request error: {response.StatusCode}.", null, response.StatusCode );

        }

        private async  Task<QuestionModel> UpdateQuestionOfSet( SurveyUpdateQuestionOfSetRequest request, SurveyQuestionType type ) {
            await PrepareAuthenticatedClient();

            string endpoint = string.Empty;
            string jsonRequest = string.Empty;
            switch ( type ) {
                case SurveyQuestionType.OpenAnswer:
                endpoint = "Open";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Boolean:
                endpoint = "Bool";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Mood:
                endpoint = "Mood";
                jsonRequest = JsonConvert.SerializeObject( request );
                break;
                case SurveyQuestionType.Rating:
                endpoint = "Rating";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyUpdateRatingQuestionOfSetRequest )request );
                break;
                case SurveyQuestionType.SingleChoice:
                endpoint = "SingleChoice";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyUpdateSingleChoiceQuestionOfSetRequest )request );
                break;
                case SurveyQuestionType.MultipleChoice:
                endpoint = "MultipleChoice";
                jsonRequest = JsonConvert
                    .SerializeObject( ( SurveyUpdateMultipleChoiceQuestionOfSetRequest )request );
                break;
            }

            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PutAsync( $"{ _apiBaseAddress }/{ _culture }/{ _questionsServiceUrlName }/{request.QuestionSetId}/{endpoint}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content
                    .ReadAsStringAsync();
                var questionModel
                    = JsonConvert
                    .DeserializeObject<QuestionModel>( content );

                return questionModel;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<QuestionModel> UpdateOpenQuestionOfSet( SurveyUpdateQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.OpenAnswer );
        }

        public async Task<QuestionModel> UpdateBoolQuestionOfSet( SurveyUpdateQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.Boolean );
        }

        public async Task<QuestionModel> UpdateMoodQuestionOfSet( SurveyUpdateQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.Mood );
        }

        public async Task<QuestionModel> UpdateRatingQuestionOfSet( SurveyUpdateRatingQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.Rating );
        }

        public async Task<QuestionModel> UpdateSingleChoiceQuestionOfSet( SurveyUpdateSingleChoiceQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.SingleChoice );
        }

        public async Task<QuestionModel> UpdateMultipleChoiceQuestionOfSet( SurveyUpdateMultipleChoiceQuestionOfSetRequest request ) {
            return await UpdateQuestionOfSet( request, SurveyQuestionType.MultipleChoice );
        }

        public async Task<SurveyAnswersBlockModel> CreateAnswersBlock( SurveyAnswersBlockCreateRequest request ) {
            await PrepareAuthenticatedClient();

            var jsonRequest
                = JsonConvert.SerializeObject( request );
            var jsoncontent = new StringContent( jsonRequest, Encoding.UTF8, "application/json" );

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _answersBlockServiceUrlName }/{request.ProjectId}", jsoncontent );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content
                    = await response.Content.ReadAsStringAsync();
                var answersBlock = JsonConvert.DeserializeObject<SurveyAnswersBlockModel>( content );
                return answersBlock;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }

        public async Task<List<SurveyAnswersBlockModel>> GetAnswersBlocks( string projectId ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{_answersBlockServiceUrlName}/{projectId}/all" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                List<SurveyAnswersBlockModel> answersBlocks = JsonConvert
                    .DeserializeObject<List<SurveyAnswersBlockModel>>( content );
                return answersBlocks;
            }
            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<SurveyAnswersBlockModel> GetAnswersBlock( string id ) {
            await PrepareAuthenticatedClient();
            var response = await _httpClient
                .GetAsync( $"{ _apiBaseAddress }/{ _culture }/{_answersBlockServiceUrlName}/{id}" );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                var content = await response.Content.ReadAsStringAsync();
                SurveyAnswersBlockModel answersBlock = JsonConvert
                    .DeserializeObject<SurveyAnswersBlockModel>( content );
                return answersBlock;
            }
            throw new HttpRequestException(
                $"Request error: {response.StatusCode}.", null, response.StatusCode );
        }

        public async Task<bool> Validate( SurveyValidateQuestionsSetRequest request ) {
            await PrepareAuthenticatedClient();

            var response = await this._httpClient
                .PostAsync( $"{ _apiBaseAddress }/{ _culture }/{ _setsServiceUrlName }/{request.QuestionsSetId}/Publish", null );

            if ( response.StatusCode == HttpStatusCode.OK ) {
                return true;
            }

            string errorMessage = await GetContentError( response );

            throw new HttpRequestException(
                errorMessage,
                null,
                response.StatusCode );
        }
    }
}
