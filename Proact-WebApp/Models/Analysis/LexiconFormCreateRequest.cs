using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp.Models {
    public class LexiconFormCreateRequest {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        [DisplayName( "File" )]
        [AllowedExtensions( new string[] { ".xls", ".xlsx" } )]
        [MaxFileSize( 10 )]
        public IFormFile ImportFile { set; get; }
    }
}
