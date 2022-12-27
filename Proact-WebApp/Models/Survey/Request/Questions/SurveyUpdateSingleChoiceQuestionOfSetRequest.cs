using System;
namespace Proact_WebApp.Models {
    public class SurveyUpdateSingleChoiceQuestionOfSetRequest : SurveyUpdateQuestionOfSetRequest {
        public Guid AnswersBlockId { get; set; }
    }
}
