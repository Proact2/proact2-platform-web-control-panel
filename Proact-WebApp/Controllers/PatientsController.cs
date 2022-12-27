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
    public class PatientsController : BaseController {

        private IUsersService _usersService;
        private IPatientsService _patientsService;

        public PatientsController(
            IUsersService usersService,
            IPatientsService patientsService ) {
            _usersService = usersService;
            _patientsService = patientsService;
        }

        public async Task<ActionResult> Index() {
            try {
                ShowMessage();
                return View( await _patientsService.GetAllPatients() );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create( string medicalTeamId ) {
            PatientCreateRequest model = new PatientCreateRequest();
            if ( !string.IsNullOrEmpty( medicalTeamId ) ) {
                model.MedicalTeamIdToAssociate = medicalTeamId;
                model.RedirectToAssociationAction = true;
            }

            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( PatientCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }
            PatientModel userModel;
            try {
                userModel = await _patientsService.CreatePatient( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddPatientConflictErrorMessage( ex.Message );
                }
                else {
                    AddGenericHttpRequestErrorMessage( ex );
                }

                ShowMessage();
                return View();
            }

            AddPatientCreatedMessage();
            if ( request.RedirectToAssociationAction ) {
                return RedirectToAction(
                    "Associate",
                    "MedicalTeamPatients",
                    new { UserId = userModel.UserId,
                          MedicalTeamId = request.MedicalTeamIdToAssociate } );
            }
            else {
                return RedirectToAction( nameof( Index ) );
            }
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFromTeam( string medicalTeamId, string userId ) {
            try {
                await _patientsService
                    .RemovePatientFromMedicalTeam( medicalTeamId, userId );

                AddPatientRemovedFromTeamMessage();
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
                AddPatientDeletedMessage();
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Index ) );
        }

        private void AddPatientRemovedFromTeamMessage() {
            AddSuccessMessage(
            "Paient removed from the Team",
            string.Empty );
        }

        private void AddPatientDeletedMessage() {
            AddSuccessMessage(
            "Paient Deteled",
            string.Empty );
        }

        private void AddPatientCreatedMessage() {
            AddSuccessMessage(
            "New patient created",
            string.Empty );
        }

        private void AddPatientConflictErrorMessage( string message ) {
            AddErrorMessage(
            "Conflict Error",
            message );
        }
    }
}
