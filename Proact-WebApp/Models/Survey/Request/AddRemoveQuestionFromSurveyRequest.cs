using System;
namespace Proact_WebApp {
    public class AddRemoveQuestionFromSurveyRequest {
        public Guid SurveyId { get; set; }
        public Guid QuestionsSetId { get; set; }
        public Guid QuestionId { get; set; }
    }
}
