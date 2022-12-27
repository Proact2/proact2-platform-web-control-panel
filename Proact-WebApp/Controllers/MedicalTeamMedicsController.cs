using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamMedicsController : BaseController {

        private IMedicalTeamsService _medicalTeamService;
        private IMedicsService _medicsService;

        public MedicalTeamMedicsController(
            IMedicalTeamsService medicalTeamsService,
            IMedicsService medicsService ) {
            _medicalTeamService = medicalTeamsService;
            _medicsService = medicsService;
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

                var medicsModel = await _medicsService
                    .GetMedicsAssignedToMedicalTeam( medicalTeamId );

                AddMedicalTeamsToViewBag( medicalTeams );
                AddMedicalTeamIdToViewBag( medicalTeamId );
                return View( medicsModel );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Associate( string userId, string medicalTeamId ) {
            ShowMessage();
            var medicalTeams = await _medicalTeamService
                .GetMedicalTeamsWhereCurrentUserIsAdmin();

            var medics = await _medicsService
                .GetAllMedics();

            AddMedicalTeamsToViewBag( medicalTeams );
            AddMedicsToViewBag( medics );
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
                await _medicsService.AssignMedicToMedicalTeam(
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
            AddMedicAssignedFromTeamMessage();
            return RedirectToAction(
                nameof( Index ),
                new { medicalTeamId = request.MedicalTeamId } );
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
                nameof( Index ),
                new { medicalTeamId = medicalTeamId } );
        }

        private void AddMedicAssignedFromTeamMessage() {
            AddSuccessMessage(
            "The medic was successfully associated with the team",
            string.Empty );
        }

        private void AddMedicRemovedToMedicalTeamMessage() {
            AddSuccessMessage(
            "Medic removed from the Team",
            string.Empty );
        }

        private void AddMedicalTeamsToViewBag( List<MedicalTeamModel> medicalTeams ) {
            ViewBag.medicalTeams = medicalTeams;
        }

        private void AddMedicsToViewBag( List<MedicModel> medics ) {
            ViewBag.medics = medics;
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
