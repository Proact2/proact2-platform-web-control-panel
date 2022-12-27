using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Controllers;
using Proact_WebApp.Models;

namespace Proact_WebApp {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class TermsAndConditionsController : BaseController {

        private IInstituteService _instituteService;

        public TermsAndConditionsController(
            IInstituteService instituteService ) {
            _instituteService = instituteService;
        }

        public async Task<ActionResult> Index() {
            ShowMessage();
            try {
                var institute = await _instituteService
                  .GetInstituteWhereCurrentUserIsAdmin();

                if ( institute.Properties == null || institute.Properties.TermsAndConditions == null ) {
                    return RedirectToAction(
                        nameof( Create ),
                        new { id = institute.Id } );
                }
                else {
                    return View( institute.Properties.TermsAndConditions );
                }
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create( string id) {
            var request = new CreateInstituteDocumentRequest() {
                InstituteId = new Guid( id )
            };

            return View( request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( CreateInstituteDocumentRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _instituteService.AddTermsAndConditions( request );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                       ex.StatusCode.ToString(),
                       ex.Message );

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Terms and Conditions added",
                string.Empty );

            return RedirectToAction( nameof( Index ) );
        }

        public async Task<ActionResult> Update() {
            var institute = await _instituteService
             .GetInstituteWhereCurrentUserIsAdmin();

            var document = institute.Properties.TermsAndConditions;
            if(document == null ) {
                return BadRequest();
            }

            var request = new CreateInstituteDocumentRequest() {
                Title = document.Title,
                Description = document.Description
            };
            return View( request );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( CreateInstituteDocumentRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _instituteService.UpdateTermsAndConditions( request );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                       ex.StatusCode.ToString(),
                       ex.Message );

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Terms and Conditions updated",
                string.Empty );

        
            return RedirectToAction( nameof( Index ) );
        }

    }
}
