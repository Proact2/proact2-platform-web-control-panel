using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class ProjectCreateRequest {
        [Required]
        [DisplayName( "Sponsor name" )]
        public string SponsorName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public ProjectPropertiesCreateRequest Properties { get; set; }
            = new ProjectPropertiesCreateRequest();
    }

    public class ProjectPropertiesCreateRequest {
        [Required]
        [DisplayName( "Medics can see other medics's analisys" )]
        public bool MedicsCanSeeOtherAnalisys { get; set; }
        [Required]
        [DisplayName( "Message can be deleted" )]
        [Range( 0, 999999 )]
        public int MessageCanNotBeDeletedAfterMinutes { get; set; } = 999999;

        [Required]
        [DisplayName( "Message can be replied" )]
        [Range( 0, 99999 )]
        public int MessageCanBeRepliedAfterMinutes { get; set; }
        [Required]
        [DisplayName( "Message can be analized" )]
        [Range( 0, 99999 )]
        public int MessageCanBeAnalizedAfterMinutes { get; set; }
        [Required]
        [DisplayName( "Messaging enabled" )]
        public bool IsMessagingActive { get; set; } = true;
        [Required]
        [DisplayName( "Analyst console enabled" )]
        public bool IsAnalystConsoleActive { get; set; }
        [Required]
        [DisplayName( "Surveys enabled" )]
        public bool IsSurveysSystemActive { get; set; }
    }

    public class ProjectPropertiesUpdateRequest : ProjectPropertiesCreateRequest {
        public Guid Id { get; set; }
    }
}
