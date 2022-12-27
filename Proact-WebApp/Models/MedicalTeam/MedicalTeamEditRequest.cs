using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class MedicalTeamEditRequest : GeneralityModel {

        [Required]
        public string Name { get; set; }
        [Required]
        public string MedicalTeamId { get; set; }
        [Required]
        public string ProjectId { get; set; }
    }
}