using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Controllers;
using Proact_WebApp.Models;

namespace Proact_WebApp {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class LexiconsController : BaseController {

        private ILexiconService _lexiconService;
        private ILexiconImportFileReaderService _lexiconImportFileReaderService;

        public LexiconsController(
            ILexiconService lexiconService,
            ILexiconImportFileReaderService lexiconImportFileReaderService ) {
            _lexiconService = lexiconService;
            _lexiconImportFileReaderService = lexiconImportFileReaderService;
        }

        public async Task<ActionResult> Index() {
            try {
                ShowMessage();
                return View( await _lexiconService.GetAsync() );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Categories( string id ) {
            try {
                ShowMessage();
                return View( await _lexiconService.GetAsync( id ) );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create() {
            return View( new LexiconFormCreateRequest() );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( LexiconFormCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {

                var stream = request.ImportFile.OpenReadStream();
                var categories = _lexiconImportFileReaderService
                    .ImportCategories( stream );

                var createRequest = new LexiconCreateRequest() {
                    Categories = categories,
                    Name = request.Name,
                    Description = request.Description
                };

                var lexiconModel = await _lexiconService.CreateAsync( createRequest );

                AddSuccessCreationMessage();

                return RedirectToAction(
                    nameof( Validate ), new { lexiconModel.Id } );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                       ex.StatusCode.ToString(),
                       ex.Message );

                ShowMessage();
                return View();
            }
            catch ( Exception ex ) {
                AddErrorMessage(
                       string.Empty,
                       ex.Message );

                ShowMessage();
                return View();
            }
        }

        public async Task<ActionResult> Validate( string id ) {
            try {
                ShowMessage();
                return View( await _lexiconService.GetAsync( id ) );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Validate( Guid id ) {
            try {

                var request = new LexiconPublishRequest() {
                    LexiconId = id
                };

                await _lexiconService.Validate( request );
                AddSuccessValidatedMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Validate request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index" );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete( Guid id ) {
            try {
                await _lexiconService.Delete( id.ToString() );
                AddSuccessDeletedMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index" );
        }

        private void AddSuccessDeletedMessage() {
            AddSuccessMessage(
            "Lexicon deleted.",
            string.Empty );
        }

        private void AddSuccessValidatedMessage() {
            AddSuccessMessage(
            "Lexicon validated.",
            string.Empty );
        }

        private void AddSuccessCreationMessage() {
            AddSuccessMessage(
            "Lexicon created.",
            string.Empty );
        }
    }
}
