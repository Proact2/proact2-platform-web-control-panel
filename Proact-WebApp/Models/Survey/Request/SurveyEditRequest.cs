using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveyEditRequest : SurveyCreateRequest {
        [Required]
        public Guid SurveyId { get; set; }
    }
}
