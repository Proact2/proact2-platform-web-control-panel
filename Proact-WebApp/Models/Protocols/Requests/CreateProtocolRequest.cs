using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp.Models {
    public class CreateProtocolRequest {

        [Required]
        [DisplayName( "Protocol name" )]
        public string Name { get; set; }

        [DisplayName( "Internal code" )]
        public string InternalCode { get; set; }

        [DisplayName( "International code" )]
        public string InternationalCode { get; set; }

        [Required]
        [DisplayName( "Protocol file" )]
        [AllowedExtensions( new string[] { ".pdf" } )]
        [MaxFileSize( 10 )]
        public IFormFile PdfFile { set; get; }
    }

    public class CreateProtocolAssignedToProjectRequest : CreateProtocolRequest {
        [Required]
        public Guid ProjectId { get; set; }
    }

    public class CreateProtocolAssignedToPatientRequest : CreateProtocolRequest {
        [Required]
        public Guid UserId { get; set; }
    }
}
