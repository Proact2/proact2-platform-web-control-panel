using System;
using System.Collections.Generic;
using Proact_WebApp.Models;

namespace Proact_WebApp.Models {
    public class SurveyAddQuestionToSetRequest {        
        public Guid QuestionSetId { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
    }
}
