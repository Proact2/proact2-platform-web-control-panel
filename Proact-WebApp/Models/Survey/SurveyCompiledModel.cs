using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Proact_WebApp.Models {

    public class SurveyCompiledQuestionAnswerModel {
        public Guid? AnswerId { get; set; }
        public string Value { get; set; }
    }

    public class SurveyCompiledQuestionModel {
        public Guid QuestionsSetId { get; set; }
        public Guid QuestionId { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public int Order { get; set; }
        public SurveyQuestionType Type { get; set; }
        public List<SurveyCompiledQuestionAnswerModel> CompiledAnswers { get; set; }

        [JsonConverter( typeof( SurveyQuestionPropertiesJsonConverter ) )]
        public ISurveysQuestionModelProperties Properties { get; set; }
    }

    public class SurveyCompiledModel {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid AssignationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public SurveyState SurveyState { get; set; } = SurveyState.DRAW;
        public DateTime StartTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public List<SurveyCompiledQuestionModel> Questions { get; set; }
    }
}
