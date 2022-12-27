using System;
namespace Proact_WebApp.Models {
    public class SurveyAddRatingQuestionToSetRequest : SurveyAddQuestionToSetRequest {
        public int Min { get; set; }
        public int Max { get; set; }
        public string MinLabel { get; set; }
        public string MaxLabel { get; set; }
    }
}
