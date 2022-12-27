using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class MedicalTeamCreateRequest : GeneralityModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string ProjectId { get; set; }
    }
}
