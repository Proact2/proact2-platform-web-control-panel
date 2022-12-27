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
    public class MedicsController : BaseController {

        private IMedicsService _medicsService;
        private IUsersService _usersService;

        public MedicsController(
            IMedicsService medicsService,
            IUsersService usersService) {
            _medicsService = medicsService;
            _usersService = usersService;
        }

        public async Task<ActionResult> Index() {
            try {
                ShowMessage();
                return View( await _medicsService.GetAllMedics() );
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
            MedicModel medicModel;
            try {
                
               medicModel= await _medicsService.CreateMedic( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddMedicConflictErrorMessage();
                }
                else {
                    AddGenericHttpRequestErrorMessage( ex );
                }

                ShowMessage();
                return View();
            }

            AddMedicCreatedMessage();
            if ( request.RedirectToAssociationAction ) {
                return RedirectToAction(
                    "Associate",
                    "MedicalTeamMedics",
                    new { UserId = medicModel.UserId,
                          MedicalTeamId = request.MedicalTeamIdToAssociate } );
            }
            else {
                return RedirectToAction( nameof( Index ) );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete( string userId ) {
            try {
                await _usersService.DeleteUser( userId );
                AddMedicDeletedMessage();
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
        public async Task<ActionResult> RemoveFromTeam( string medicalTeamId, string userId ) {
            try {
                await _medicsService
                    .RemoveMedicFromMedicalTeam( medicalTeamId, userId );

                AddMedicRemovedToMedicalTeamMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
                nameof( Index ) );
        }

        private void AddMedicRemovedToMedicalTeamMessage() {
            AddSuccessMessage(
            "Medic removed from the Team",
            string.Empty );
        }

        private void AddMedicCreatedMessage() {
            AddSuccessMessage(
            "New Medic created",
            string.Empty );
        }

        private void AddMedicDeletedMessage() {
            AddSuccessMessage(
            "Nurse Deteled",
            string.Empty );
        }

        private void AddMedicConflictErrorMessage() {
            AddErrorMessage(
            "Conflict Error",
            "A User with this email address already exists" );
        }

    }
}
