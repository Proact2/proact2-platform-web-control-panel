using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp {
    public class AssignUserToMedicalTeamRequest {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid MedicalTeamId { get; set; }
    }
}