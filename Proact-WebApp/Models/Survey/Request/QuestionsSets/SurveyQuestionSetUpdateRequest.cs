using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveyQuestionSetUpdateRequest : SurveyQuestionSetCreateRequest {

        [Required]
        public Guid QuestionSetId { get; set; }
    }
}
