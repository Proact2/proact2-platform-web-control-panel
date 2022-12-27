using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class CreateInstituteRequest {
        [Required]
        [MaxLength( 32, ErrorMessage = "Max 32 characters" )]
        public string Name { get; set; }
    }
}
