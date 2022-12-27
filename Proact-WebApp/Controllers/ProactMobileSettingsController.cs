using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Controllers;

namespace Proact_WebApp {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class ProactMobileSettingsController : BaseController {

        IProactMobileSettingsService _proactMobileSettingsService;

        public ProactMobileSettingsController(
            IProactMobileSettingsService proactMobileSettingsService ) {
            _proactMobileSettingsService = proactMobileSettingsService;
        }

        public async Task<ActionResult> Update() {
            ShowMessage();
            try {
                var lastAppVersion = await _proactMobileSettingsService
                  .GetLastAppVersion();

                return View( lastAppVersion );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( RequiredUpdateModel request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _proactMobileSettingsService.SetLastAppVersion( request );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                       ex.StatusCode.ToString(),
                       ex.Message );

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Last app version updated",
                string.Empty );
            ShowMessage();
            return View();
        }
    }
}
