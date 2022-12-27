using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class UserCreateRequest {

        [Required]
        [MaxLength( 100 )]
        [DisplayName( "First name" )]
        public string FirstName { get; set; }
        [Required]
        [MaxLength( 100 )]
        [DisplayName( "Last name" )]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool RedirectToAssociationAction { get; set; }
        public string MedicalTeamIdToAssociate { get; set; }
    }
}
