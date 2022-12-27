using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Proact_WebApp.Models {
    public class SurveyCreateRequest {
        [Required]
        public Guid QuestionsSetId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Version { get; set; }
        [Required]
        public Guid ProjectId { get; set; }


        [JsonIgnore]
        public List<QuestionModel> Questions { get; set; }

        public Guid[] questionsIds { get; set; }

        public SurveyCreateRequest() {
            Questions = new List<QuestionModel>();
        }
    }
}