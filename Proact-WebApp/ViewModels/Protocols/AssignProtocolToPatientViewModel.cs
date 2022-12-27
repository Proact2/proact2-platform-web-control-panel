using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp.ViewModels {
    public class AssignProtocolToPatientViewModel {

        public Guid ProtocolId { get; set; }
        public Guid UserId { get; set; }
        public Guid MedicalTeamId { get; set; }

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
        public IFormFile ImportFile { set; get; }

        public string Url { get; set; }

        public bool IsCreationModel {
            get => ProtocolId == Guid.Empty;
        }
    }
}
