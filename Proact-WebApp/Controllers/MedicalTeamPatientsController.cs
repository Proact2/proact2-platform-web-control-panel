using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;
using Proact_WebApp.ViewModels;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamPatientsController : BaseController {

        private IMedicalTeamsService _medicalTeamService;
        private IPatientsService _patientsService;
        private IProtocolService _protocolService;
        private IUsersService _userService;

        public MedicalTeamPatientsController(
            IMedicalTeamsService medicalTeamsService,
            IPatientsService patientsService,
            IProtocolService protocolService,
            IUsersService usersService ) {
            _medicalTeamService = medicalTeamsService;
            _patientsService = patientsService;
            _protocolService = protocolService;
            _userService = usersService;
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

                var patientsmodel = await _patientsService
                    .GetPatientsAssignedToMedicalTeam( medicalTeamId );


                AddPatientsCountToViewBag( patientsmodel );
                AddMedicalTeamsToViewBag( medicalTeams );
                AddMedicalTeamIdToViewBag( medicalTeamId );
                return View( patientsmodel );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Associate( string userId, string medicalTeamId ) {
            await PrepareAssociationView( userId, medicalTeamId );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Associate( AssignPatientToMedicalTeamRequest request ) {
            if ( !ModelState.IsValid ) {
                await PrepareAssociationView( request.UserId.ToString(), request.MedicalTeamId.ToString() );
                return View();
            }

            try {
                await _patientsService.AssignPatientToMedicalTeam(
                    request.MedicalTeamId.ToString(), request );
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
                //ShowMessage();
                return RedirectToAction(
                nameof( Index ),
                new { medicalTeamId = request.MedicalTeamId } );
            }
            AddPatientAssignedToTeamMessage();
            return RedirectToAction(
                nameof( Index ),
                new { medicalTeamId = request.MedicalTeamId } );
        }

        private async Task PrepareAssociationView( string userId, string medicalTeamId ) {
            var medicalTeams = await _medicalTeamService
               .GetMedicalTeamsWhereCurrentUserIsAdmin();
            var patients = await _patientsService
                .GetUnassignedPatients();

            AddMedicalTeamsToViewBag( medicalTeams );
            AddPatientsToViewBag( patients );
            AddUserIdToViewBag( userId );
            AddMedicalTeamIdToViewBag( medicalTeamId );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFromTeam( string userId, string medicalTeamId ) {
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
              nameof( Index ),
              new { medicalTeamId = medicalTeamId } );
        }

        public async Task<ActionResult> Protocol( string userId, string medicalTeamId ) {
            ShowMessage();
            try {

                ViewBag.userId = userId;
                ViewBag.medicalTeamId = medicalTeamId;

                var protocol = await _protocolService
                    .GetProtocolAssignedToPatient( userId );

                return View( protocol );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.NotFound
                    || ex.StatusCode == System.Net.HttpStatusCode.NoContent ) {
                    return View( new ProtocolModel() );
                }
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> AssignProtocol( string userId, string medicalTeamId ) {

            var viewModel = new AssignProtocolToPatientViewModel() {
                UserId = new Guid( userId ),
                MedicalTeamId = new Guid( medicalTeamId )
            };

            try {
                var protocol = await _protocolService
                    .GetProtocolAssignedToPatient( userId );

                viewModel.ProtocolId = protocol.Id;
                viewModel.Name = protocol.Name;
                viewModel.InternalCode = protocol.InternalCode;
                viewModel.InternationalCode = protocol.InternationalCode;
                viewModel.Url = protocol.Url;

            }
            catch ( HttpRequestException) {}

            return View( viewModel );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignProtocol( AssignProtocolToPatientViewModel viewModel ) {
            try {
                var request = new CreateProtocolAssignedToPatientRequest() {
                    Name = viewModel.Name,
                    InternalCode = viewModel.InternalCode,
                    InternationalCode = viewModel.InternationalCode,
                    PdfFile = viewModel.ImportFile,
                    UserId = viewModel.UserId
                };

                var protocol = await _protocolService
                    .CreateProtocolAssignedToPatientAsync( request );

                AddAssignProtocolMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Protocol ),
              new {
                  userId = viewModel.UserId,
                  medicalTeamId = viewModel.MedicalTeamId
              } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SuspendPatient( string userId, string medicalTeamId ) {
            try {
                await _userService
                    .SuspendUser( userId );

                AddPatientSuspendMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Suspend user request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Index ),
              new { medicalTeamId = medicalTeamId } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ActivatePatient( string userId, string medicalTeamId ) {
            try {
                await _userService
                    .ActivateUser( userId );

                AddActivatePatientMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Activate user request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Index ),
              new { medicalTeamId = medicalTeamId } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeactivatePatient( string userId, string medicalTeamId ) {
            try {
                await _userService
                    .DeactivateUser( userId );

                AddDeactivatePatientMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Deactivate user request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction(
              nameof( Index ),
              new { medicalTeamId = medicalTeamId } );
        }

        private void AddPatientSuspendMessage() {
            AddSuccessMessage(
            "The patient was suspended.",
            string.Empty );
        }

        private void AddActivatePatientMessage() {
            AddSuccessMessage(
            "The patient was activated.",
            string.Empty );
        }

        private void AddDeactivatePatientMessage() {
            AddSuccessMessage(
            "The patient has been deactivated.",
            string.Empty );
        }

        private void AddPatientAssignedToTeamMessage() {
            AddSuccessMessage(
            "The patient was successfully associated with the team",
            string.Empty );
        }

        private void AddPatientRemovedFromTeamMessage() {
            AddSuccessMessage(
            "Paient removed from the Team",
            string.Empty );
        }

        private void AddAssignProtocolMessage() {
            AddSuccessMessage(
            "Protocol assigned to patient",
            string.Empty );
        }

        private void AddMedicalTeamsToViewBag( List<MedicalTeamModel> medicalTeams ) {
            ViewBag.medicalTeams = medicalTeams;
        }

        private void AddPatientsToViewBag( List<PatientModel> patients ) {
            ViewBag.patients = patients;
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

        private void AddPatientsCountToViewBag( List<PatientModel> patientsmodel ) {
            ViewBag.PatientsCount = patientsmodel.Count;

            ViewBag.ActivePatientsCount = patientsmodel
                .Where( x => x.State == UserSubscriptionState.Active )
                .ToList().Count;

            ViewBag.SuspendedPatientsCount = patientsmodel
              .Where( x => x.State == UserSubscriptionState.Suspended )
              .ToList().Count;

            ViewBag.DeactivatedPatientsCount = patientsmodel
              .Where( x => x.State == UserSubscriptionState.Deactivated )
              .ToList().Count;
        }
    }
}
