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
    public class InstituteController : BaseController {

        private IInstituteService _instituteService;
        private IUsersService _userService;

        public InstituteController(
            IInstituteService instituteService,
            IUsersService usersService) {
            _instituteService = instituteService;
            _userService = usersService;
        }


        public async Task<ActionResult> Index( string showAll ) {
            var showOnlyOpenProject
                = string.IsNullOrEmpty( showAll );
            ViewBag.openOnly
                = showOnlyOpenProject;
            try {
                ShowMessage();

                if ( showOnlyOpenProject ) {
                    return View( await _instituteService.GetOpenAsync() );
                }
                else {
                    return View( await _instituteService.GetAllAsync() );
                }

            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create() {
            return View( new CreateInstituteRequest() );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( CreateInstituteRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _instituteService.CreateAsync( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddErrorMessage(
                        "Conflict Error",
                        "An institude with this name already exists" );
                }
                else {
                    AddErrorMessage(
                        ex.StatusCode.ToString(),
                        ex.Message );
                }

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Institute created",
                string.Empty );

            return RedirectToAction( nameof( Index ) );
        }

        [HttpGet]
        public async Task<ActionResult> Update( string id ) {
            try {
                var instituteModel
                    = await _instituteService.GetAsync( id );

                UpdateInstituteRequest request = new UpdateInstituteRequest {
                    Id = new Guid( id ),
                    Name = instituteModel.Name
                };
                return View( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction( nameof( Index ) );
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( UpdateInstituteRequest request ) {
            if ( !ModelState.IsValid ) {
                return View( request );
            }
            try {
                await _instituteService.UpdateAsync( request );

                AddSuccessMessage(
                    "Institute updated",
                    string.Empty );

                return RedirectToAction( nameof( Index ) );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return View( request );
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Close( string instituteId ) {
            try {
                await _instituteService
                    .CloseAsync( instituteId );

                AddSuccessMessage(
                    "Institute closed",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Close request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }

        [HttpPost]
        public async Task<ActionResult> Open( string instituteId ) {
            try {
                await _instituteService
                    .OpenAsync( instituteId );

                AddSuccessMessage(
                    "Institute opened",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Open request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }

        [HttpGet]
        public async Task<ActionResult> AssociateAdmin( string id ) {
            try {
                var instituteModel
                    = await _instituteService.GetAsync( id );

                InstituteAdminCreateRequest request = new InstituteAdminCreateRequest() {
                    InstituteId = new Guid( id )
                };
                return View( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction( nameof( Index ) );
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssociateAdmin( InstituteAdminCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View( request );
            }
            try {

               await _instituteService
                    .CreateAdmin( request );

                AddSuccessMessage(
                    "Institute Admin created",
                    string.Empty );

                return RedirectToAction( nameof( Index ) );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return View( request );
                }
            }
        }


    }

}

