using System;
namespace Proact_WebApp.Models {
    public class SurveysRatingQuestionModelProperties : ISurveysQuestionModelProperties {
        public SurveyQuestionType Type { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string MinLabel { get; set; }
        public string MaxLabel { get; set; }
    }
}
