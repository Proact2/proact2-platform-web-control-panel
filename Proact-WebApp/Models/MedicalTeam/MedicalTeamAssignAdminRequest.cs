using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp {
    public class MedicalTeamAssignAdminRequest {

        [Required]
        public Guid UserId { get; set; }
    }
}
