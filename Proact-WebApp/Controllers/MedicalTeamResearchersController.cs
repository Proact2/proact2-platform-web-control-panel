using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamResearchersController : BaseController {

        private IMedicalTeamsService _medicalTeamService;
        private IResearcherService _researcherService;

        public MedicalTeamResearchersController(
            IMedicalTeamsService medicalTeamsService,
            IResearcherService researcherService ) {
            _medicalTeamService = medicalTeamsService;
            _researcherService = researcherService;
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

                var model = await _researcherService
                    .GetAssignedToMedicalTeam( medicalTeamId );

                AddMedicalTeamsToViewBag( medicalTeams );
                AddMedicalTeamIdToViewBag( medicalTeamId );
                return View( model );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create( string medicalTeamId ) {
            UserCreateRequest model = new UserCreateRequest();
            model.MedicalTeamIdToAssociate = medicalTeamId;
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

                userModel = await _researcherService.Create( request );
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

            return RedirectToAction(
                   nameof( Associate ),
                   new {
                       UserId = userModel.UserId,
                       MedicalTeamId = request.MedicalTeamIdToAssociate
                   } );
        }

        public async Task<ActionResult> Associate( string userId, string medicalTeamId ) {
            ShowMessage();
            var medicalTeams = await _medicalTeamService
                .GetMedicalTeamsWhereCurrentUserIsAdmin();

            var researchers = await _researcherService
                .GetAll();

            AddMedicalTeamsToViewBag( medicalTeams );
            AddResearchersToViewBag( researchers );
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
                await _researcherService.AssignToMedicalTeam( request );
            }
            catch ( HttpRequestException ex ) {

                AddGenericHttpRequestErrorMessage( ex );

                return RedirectToAction(
                  nameof( Associate ),
                  new {
                      UserId = request.UserId,
                      MedicalTeamId = request.MedicalTeamId
                  } );
            }
            AddResearcherAssignedFromTeamMessage();
            return RedirectToAction(
                nameof( Index ),
                new { medicalTeamId = request.MedicalTeamId } );
        }

        [HttpPost]
        public async Task<ActionResult> RemoveFromTeam( string medicalTeamId, string userId ) {
            try {
                await _researcherService
                    .RemoveFromMedicalTeam( medicalTeamId, userId );

                AddResearcherRemovedToMedicalTeamMessage();
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

        private void AddResearcherAssignedFromTeamMessage() {
            AddSuccessMessage(
            "The Researcher was successfully associated",
            string.Empty );
        }

        private void AddAssignConflictError() {
            AddErrorMessage( "Conflict error",
             "The User is already associated" );
        }

        private void AddResearcherRemovedToMedicalTeamMessage() {
            AddSuccessMessage(
            "Researcher removed",
            string.Empty );
        }


        private void AddUserConflictErrorMessage() {
            AddErrorMessage(
            "Conflict Error",
            "A User with this email address already exists" );
        }

        private void AddMedicalTeamsToViewBag( List<MedicalTeamModel> medicalTeams ) {
            ViewBag.medicalTeams = medicalTeams;
        }

        private void AddResearchersToViewBag( List<ResearcherModel> researchers ) {
            ViewBag.researchers = researchers;
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
