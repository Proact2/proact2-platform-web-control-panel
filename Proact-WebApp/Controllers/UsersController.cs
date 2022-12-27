using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class UsersController : BaseController {

        private IUsersService _usersService;

        public UsersController( IUsersService usersService ) {
            _usersService = usersService;
        }

        public async Task<ActionResult> Index() {
            try {
                ShowMessage();
                return View( await _usersService.GetAsync() );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( UserCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _usersService.CreateAsync( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddUserConflictErrorMessage();
                }
                else {
                    AddGenericHttpRequestErrorMessage( ex );
                }

                ShowMessage();
                return View();
            }
            AddUserCreatedMessage();
            return RedirectToAction( nameof( Index ) );
        }

        private void AddUserCreatedMessage() {
            AddSuccessMessage(
            "New user created",
            string.Empty );
        }

        private void AddUserConflictErrorMessage() {
            AddErrorMessage(
            "Conflict Error",
            "A User with this email address already exists" );
        }
    }
}
