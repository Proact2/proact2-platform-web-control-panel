using System;
namespace Proact_WebApp.Models {
    public class SurveyUpdateMultipleChoiceQuestionOfSetRequest : SurveyUpdateQuestionOfSetRequest {
        public Guid AnswersBlockId { get; set; }
    }
}
