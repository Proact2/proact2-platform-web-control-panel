using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {
    public class SurveyQuestionSetModel {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public SurveyState State { get; set; }
        public List<QuestionModel> Questions { get; set; }
    }
}
