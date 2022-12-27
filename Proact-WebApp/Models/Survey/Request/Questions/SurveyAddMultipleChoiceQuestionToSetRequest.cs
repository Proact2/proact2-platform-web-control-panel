using System;
namespace Proact_WebApp.Models {
    public class SurveyAddMultipleChoiceQuestionToSetRequest : SurveyAddQuestionToSetRequest {
        public Guid AnswersBlockId { get; set; }
    }
}
