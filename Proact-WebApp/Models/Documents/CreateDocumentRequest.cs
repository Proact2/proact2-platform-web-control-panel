using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp.Models {
    public class CreateDocumentRequest {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DisplayName( "File" )]
        [AllowedExtensions( new string[] { ".pdf" } )]
        [MaxFileSize(10)]
        public IFormFile PdfFile { set; get; }
    }

    public class CreateInstituteDocumentRequest : CreateDocumentRequest {
        public Guid InstituteId { get; set; }
    }
}
