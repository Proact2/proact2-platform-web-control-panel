using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveySchedulerModel {
        public Guid Id { get; set; }
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime StartTime { get; set; }
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime ExpireTime { get; set; }
        public DateTime LastSubmission { get; set; }
        public Guid UserId { get; set; }
        public Guid SurveyId { get; set; }
        public SurveyReccurence Reccurence { get; set; }
    }
}
