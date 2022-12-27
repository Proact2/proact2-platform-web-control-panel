using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Proact_WebApp.Models {
    public class QuestionModel {
        public Guid Id { get; set; }
        public Guid QuestionsSetId { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public int Order { get; set; }
        public SurveyState State { get; set; }

        [JsonConverter( typeof( SurveyQuestionPropertiesJsonConverter ) )]
        public ISurveysQuestionModelProperties Properties { get; set; }

        [JsonConverter( typeof( SurveyQuestionAnswersContainerJsonConverter ) )]
        public ISurveyQuestionModelAnswersContainer AnswersContainer { get; set; }

        public bool Selected { get; set; }
    }
}
