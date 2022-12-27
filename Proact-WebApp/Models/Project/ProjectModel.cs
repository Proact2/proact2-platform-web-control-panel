using System;
using System.ComponentModel;

namespace Proact_WebApp.Models {

    public enum ProjectState {
        Open,
        Closed,
    }

    public class ProjectModel {
        [DisplayName("Study Id")]
        public Guid ProjectId { get; set; }
        public string Name { get; set; }      
        public string Description { get; set; }
        public string SponsorName { get; set; }
        public ProjectState Status { get; set; }
        public ProjectPropertiesModel Properties { get; set; }
    }

    public class ProjectPropertiesModel {
        
        public LexiconModel Lexicon { get; set; }
        public bool MedicsCanSeeOtherAnalisys { get; set; }
        public int MessageCanNotBeDeletedAfterMinutes { get; set; }
        public int MessageCanBeRepliedAfterMinutes { get; set; }
        public int MessageCanBeAnalizedAfterMinutes { get; set; }
        public bool IsMessagingActive { get; set; }
        public bool IsAnalystConsoleActive { get; set; }
        public bool IsSurveysSystemActive { get; set; }
    }
}