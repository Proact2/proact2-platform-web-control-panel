using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {
    public class SurveysSingleChoiceQuestionModelAnswersContainer
        : ISurveyQuestionModelAnswersContainer {
        public SurveyQuestionType Type { get; set; }
        public Guid AnswersBlockId { get; set; }
        public List<SurveysSelectableAnswer> SelectableAnswers { get; set; }
    }
}
