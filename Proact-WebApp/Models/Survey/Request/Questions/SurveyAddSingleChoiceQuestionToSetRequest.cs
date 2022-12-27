using System;
namespace Proact_WebApp.Models {
    public class SurveyAddSingleChoiceQuestionToSetRequest : SurveyAddQuestionToSetRequest {
        public Guid AnswersBlockId { get; set; }
    }
}
