using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class HomeController : BaseController {

        private readonly ILogger<HomeController> _logger;
        private IUsersService _userService;
        private ISessionService _sessionService;

        public HomeController( ILogger<HomeController> logger,
            IUsersService usersService,
            ISessionService sessionService ) {
            _logger = logger;
            _userService = usersService;
            _sessionService = sessionService;
        }

        public async Task<IActionResult> Index() {
            var currentUser = await _userService.GetCurrentUser();
            _sessionService.InitSession( HttpContext.Session, currentUser );

            if ( _sessionService.IsInstituteAdmin( HttpContext.Session ) ) {
                return RedirectToAction( "Index", "InstituteDashboard" );
            }
            else if ( _sessionService.IsMedicalTeamAdmin( HttpContext.Session ) ) {
                return RedirectToAction( "Index", "MedicalTeamAdminDashboard" );
            }

            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
