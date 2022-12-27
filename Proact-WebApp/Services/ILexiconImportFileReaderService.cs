using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface ILexiconImportFileReaderService {

        bool ValidateFileExtention( IFormFile file );
        List<LexiconCreateCategoryRequest> ImportCategories( Stream fileStream );
    }
}
