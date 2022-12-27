using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveyAssignationModel {

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SurveyId { get; set; }

        [DisplayName( "Start date" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime StartTime { get; set; }

        [DisplayName( "End date" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime ExpireTime { get; set; }

        [DisplayName( "Completed" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime? CompletedDateTime { get; set; }

        [Required]
        public SurveyReccurence Reccurence { get; set; }
        public bool Completed { get; set; }
        public bool Expired { get; set; }
        public UserModel User { get; set; }

        public SurveySchedulerModel Scheduler { get; set; }
    }
}
