using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface ISurveysQuestionsSetsService {

        Task<SurveyQuestionSetModel> GetQuestionsSetAsync( string questionSetId );
        Task<List<SurveyQuestionSetModel>> GetQuestionsSetsAsync( string projectId );
        Task<SurveyQuestionSetModel> CreateQuestionsSetAsync(
            SurveyQuestionSetCreateRequest request );
        Task<SurveyQuestionSetModel> UpdateQuestionsSetAsync(
          SurveyQuestionSetUpdateRequest request );
        Task<bool> DeleteQuestionsSet( string id );

        Task<QuestionModel> AddOpenQuestionToSet(
            SurveyAddQuestionToSetRequest request );

        Task<QuestionModel> AddBoolQuestionToSet(
            SurveyAddQuestionToSetRequest request );

        Task<QuestionModel> AddMoodQuestionToSet(
            SurveyAddQuestionToSetRequest request );

        Task<QuestionModel> AddRatingQuestionToSet(
           SurveyAddRatingQuestionToSetRequest request );

        Task<QuestionModel> AddSingleChoiceQuestionToSet(
            SurveyAddSingleChoiceQuestionToSetRequest request );

        Task<QuestionModel> AddMultipleChoiceQuestionToSet(
            SurveyAddMultipleChoiceQuestionToSetRequest request );

        Task<QuestionModel> UpdateOpenQuestionOfSet(
           SurveyUpdateQuestionOfSetRequest request );

        Task<QuestionModel> UpdateBoolQuestionOfSet(
          SurveyUpdateQuestionOfSetRequest request );

        Task<QuestionModel> UpdateMoodQuestionOfSet(
          SurveyUpdateQuestionOfSetRequest request );

        Task<QuestionModel> UpdateRatingQuestionOfSet(
           SurveyUpdateRatingQuestionOfSetRequest request );

        Task<QuestionModel> UpdateSingleChoiceQuestionOfSet(
            SurveyUpdateSingleChoiceQuestionOfSetRequest request );

        Task<QuestionModel> UpdateMultipleChoiceQuestionOfSet(
            SurveyUpdateMultipleChoiceQuestionOfSetRequest request );


        Task<bool> RemoveQuestionFromSet( string questionSetId, string questionId );

        Task<SurveyAnswersBlockModel> CreateAnswersBlock( SurveyAnswersBlockCreateRequest request );
        Task<List<SurveyAnswersBlockModel>> GetAnswersBlocks( string projectId );
        Task<SurveyAnswersBlockModel> GetAnswersBlock( string id );
        Task<bool> Validate( SurveyValidateQuestionsSetRequest request );
    } 
}
