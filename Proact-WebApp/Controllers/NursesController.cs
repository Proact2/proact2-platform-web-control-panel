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
    public class NursesController : BaseController {

        private INursesService _nursesService;
        private IUsersService _usersService;

        public NursesController(
            INursesService nursesSercice,
            IUsersService usersService ) {
            _nursesService = nursesSercice;
            _usersService = usersService;
        }

        public async Task<ActionResult> Index() {
            try {
                ShowMessage();
                return View( await _nursesService.GetAllNurse() );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create( string medicalTeamId ) {
            UserCreateRequest model = new UserCreateRequest();
            if ( !string.IsNullOrEmpty( medicalTeamId ) ) {
                model.MedicalTeamIdToAssociate = medicalTeamId;
                model.RedirectToAssociationAction = true;
            }

            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( UserCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }
            UserModel userModel;
            try {
             
               userModel = await _nursesService.CreateNurse( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddNurseConflictErrorMessage();
                }
                else {
                    AddGenericHttpRequestErrorMessage( ex );
                }

                ShowMessage();
                return View();
            }

            AddNurseCreatedMessage();
            if ( request.RedirectToAssociationAction ) {
                return RedirectToAction(
                    "Associate",
                    "MedicalTeamNurses",
                    new {
                        UserId = userModel.UserId,
                        MedicalTeamId = request.MedicalTeamIdToAssociate
                    } );
            }
            else {
                return RedirectToAction( nameof( Index ) );
            }
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFromTeam( string medicalTeamId, string userId ) {
            try {
                await _nursesService
                    .RemoveNurseFromMedicalTeam( medicalTeamId, userId );

                AddNurseRemovedToMedicalTeamMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
                nameof( Index ) );
        }

        [HttpPost]
        public async Task<ActionResult> Delete( string userId ) {
            try {
                await _usersService.DeleteUser( userId );
                AddNurseDeletedMessage();
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Index ) );
        }

        private void AddNurseRemovedToMedicalTeamMessage() {
            AddSuccessMessage(
            "Nurse removed from the Team",
            string.Empty );
        }

        private void AddNurseCreatedMessage() {
            AddSuccessMessage(
            "New Nurse created",
            string.Empty );
        }

        private void AddNurseDeletedMessage() {
            AddSuccessMessage(
            "Nurse Deteled",
            string.Empty );
        }

        private void AddNurseConflictErrorMessage() {
            AddErrorMessage(
            "Conflict Error",
            "A User with this email address already exists" );
        }

    }
}
