using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamNursesController : BaseController {

        private IMedicalTeamsService _medicalTeamService;
        private INursesService _nursesService;

        public MedicalTeamNursesController(
            IMedicalTeamsService medicalTeamsService,
            INursesService nurseService ) {
            _medicalTeamService = medicalTeamsService;
            _nursesService = nurseService;
        }

        public async Task<ActionResult> Index( string medicalTeamId ) {
            ShowMessage();
            try {
                var medicalTeams = await _medicalTeamService
                    .GetMedicalTeamsWhereCurrentUserIsAdmin();

                if ( medicalTeams == null || medicalTeams.Count == 0 ) {
                    return BadRequest();
                }

                if ( string.IsNullOrEmpty( medicalTeamId ) ) {
                    medicalTeamId = medicalTeams[0].MedicalTeamId.ToString();
                }

                var nursesModel = await _nursesService
                    .GetNurseAssignedToMedicalTeam( medicalTeamId );

                AddMedicalTeamsToViewBag( medicalTeams );
                AddMedicalTeamIdToViewBag( medicalTeamId );
                return View( nursesModel );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Associate( string userId, string medicalTeamId ) {
            ShowMessage();
            var medicalTeams = await _medicalTeamService
                .GetMedicalTeamsWhereCurrentUserIsAdmin();

            var nurses = await _nursesService
                .GetAllNurse();

            AddMedicalTeamsToViewBag( medicalTeams );
            AddNursesToViewBag( nurses );
            AddUserIdToViewBag( userId );
            AddMedicalTeamIdToViewBag( medicalTeamId );
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Associate( AssignUserToMedicalTeamRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _nursesService.AssignNurseToMedicalTeam(
                    request.MedicalTeamId.ToString(), request );
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );

                return RedirectToAction(
                    nameof( Associate ),
                    new {
                        userId = request.UserId,
                        medicalTeamId = request.MedicalTeamId
                    } );
            }
            AddNurseAssignedFromTeamMessage();
            return RedirectToAction(
                nameof( Index ),
                new { medicalTeamId = request.MedicalTeamId } );
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
                nameof( Index ),
                new { medicalTeamId = medicalTeamId } );
        }

        private void AddNurseAssignedFromTeamMessage() {
            AddSuccessMessage(
            "The medic was successfully associated with the team",
            string.Empty );
        }

        private void AddNurseRemovedToMedicalTeamMessage() {
            AddSuccessMessage(
            "Medic removed from the Team",
            string.Empty );
        }

        private void AddMedicalTeamsToViewBag( List<MedicalTeamModel> medicalTeams ) {
            ViewBag.medicalTeams = medicalTeams;
        }

        private void AddNursesToViewBag( List<NurseModel> nurses ) {
            ViewBag.nurses = nurses;
        }

        private void AddUserIdToViewBag( string userId ) {
            if ( !string.IsNullOrEmpty( userId ) ) {
                ViewBag.userId = userId;
            }
        }

        private void AddMedicalTeamIdToViewBag( string medicalTeamId ) {
            if ( !string.IsNullOrEmpty( medicalTeamId ) ) {
                ViewBag.medicalTeamId = medicalTeamId;
            }
        }
    }
}
