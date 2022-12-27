using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {
    public class SurveyAnswersBlockCreateRequest {
        public List<string> Labels { get; set; }
        public Guid ProjectId { get; set; }
    }
}
