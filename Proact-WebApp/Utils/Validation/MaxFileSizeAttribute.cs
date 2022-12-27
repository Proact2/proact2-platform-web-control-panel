using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp {
    public class MaxFileSizeAttribute : ValidationAttribute {
        private readonly int _maxFileSizeInMB;
        public MaxFileSizeAttribute( int maxFileSizeInMB ) {
            _maxFileSizeInMB = maxFileSizeInMB;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext ) {
            var file = value as IFormFile;
            if ( file != null ) {
                if ( file.Length > _maxFileSizeInMB * 1024 * 1024  ) {
                    return new ValidationResult( GetErrorMessage() );
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage() {
            return $"Maximum allowed file size is { _maxFileSizeInMB } MB.";
        }
    }
}
