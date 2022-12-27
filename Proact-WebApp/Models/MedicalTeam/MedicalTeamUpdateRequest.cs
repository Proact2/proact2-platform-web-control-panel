using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class MedicalTeamUpdateRequest : GeneralityModel {
        [Required]
        public string Name { get; set; }
    }
}
