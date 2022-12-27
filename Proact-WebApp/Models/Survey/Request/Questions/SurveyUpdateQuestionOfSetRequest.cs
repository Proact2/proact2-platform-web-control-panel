using System;

namespace Proact_WebApp.Models {
    public class SurveyUpdateQuestionOfSetRequest : SurveyAddQuestionToSetRequest {
        public Guid QuestionId { get; set; }
    }
}
