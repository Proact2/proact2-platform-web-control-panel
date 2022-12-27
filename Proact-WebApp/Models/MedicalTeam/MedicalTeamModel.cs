using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {

    public enum MedicalTeamState {
        Open,
        ClosedByProject,
        ClosedByMedicalTeam
    }

    public class MedicalTeamModel : GeneralityModel {
        [Required]
        public Guid MedicalTeamId { get; set; }
        [Required]
        public string Name { get; set; }
        public MedicalTeamState State { get; set; }
        public ProjectModel Project { get; set; }
    }
}
