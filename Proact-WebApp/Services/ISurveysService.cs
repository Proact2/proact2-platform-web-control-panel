using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface ISurveysService {
        Task<List<SurveyModel>> GetSurveysAsync( string projectId );
        Task<SurveyModel> GetSurveyAsync( string surveyId );
        Task<SurveyModel> CreateSurvey( SurveyCreateRequest request );
        Task<SurveyModel> UpdateSurvey( SurveyEditRequest request );
        Task<bool> DeleteSurvey( string id );
        Task<QuestionModel> AddQuestion(
            AddRemoveQuestionFromSurveyRequest request );
        Task<bool> RemoveQuestion(
            AddRemoveQuestionFromSurveyRequest request );
        Task<bool> PerformAssignations(
            SurveyAssignationsRequest request );
        Task<List<SurveyAssignationModel>> GetPatientsAssignedToSurvey( string id );
        Task<SurveyCompiledModel> GetCompiledSurvey( string assignationId );
        Task<SurveyStatsResume> GetSurveyStatsResume( string surveyId );
    }
}
