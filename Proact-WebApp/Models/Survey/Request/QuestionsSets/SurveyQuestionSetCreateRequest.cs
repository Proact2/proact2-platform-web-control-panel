using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveyQuestionSetCreateRequest {

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
    }
}
