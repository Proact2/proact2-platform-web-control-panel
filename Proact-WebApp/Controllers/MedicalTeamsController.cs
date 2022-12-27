using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamsController : BaseController {

        private IMedicalTeamsService _medicalTeamsService;
        private IProjectService _projectService;
        private IUsersService _userService;
        private IMedicsService _medicsService;

        public MedicalTeamsController(
            IMedicalTeamsService medicalTeamsService,
            IProjectService projectService,
            IUsersService usersService,
            IMedicsService medicsService) {

            _medicalTeamsService = medicalTeamsService;
            _projectService = projectService;
            _userService = usersService;
            _medicsService = medicsService;
        }

        public async Task<ActionResult> Index( string projectId, string showAll ) {
            try {
                ShowMessage();
                var showOnlyOpenMedicalTeam =
                    string.IsNullOrEmpty( showAll );

                var projects = await _projectService
                    .GetOpenProjectAsync();

                IEnumerable<MedicalTeamModel> medicalTeams
                    = new List<MedicalTeamModel>();

                if (projects.Count > 0 ) {
           
                    if ( string.IsNullOrEmpty( projectId ) ) {
                        var selectedProject
                            = projects.FirstOrDefault();

                        if ( selectedProject != null ) {
                            projectId = selectedProject.ProjectId.ToString();
                        }
                    }

                    if ( showOnlyOpenMedicalTeam ) {
                        medicalTeams = await _medicalTeamsService
                            .GetOpenMedicalTeamsAsync( projectId );
                    }
                    else {
                        medicalTeams = await _medicalTeamsService
                            .GetAllAsync( projectId );
                    }
                }

                MedicalTeamIndexModel model = new MedicalTeamIndexModel() {
                    Projects = projects,
                    MedicalTeams = medicalTeams
                };

                ViewBag.projectId = projectId;
                ViewBag.openOnly = showOnlyOpenMedicalTeam;
                return View( model );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Details( string projectId, string medicalTeamId ) {
            try {
                var medicalTeam = await _medicalTeamsService
                    .DetailsAsync( projectId, medicalTeamId );

                var admins = await _medicalTeamsService
                    .GetAdminsAsync( projectId, medicalTeamId );

                var detailsModel = new MedicalTeamDetailsModel() {
                    MedicalTeam = medicalTeam,
                    Admins = admins.ToList()
                };

                ViewBag.projectId = projectId;
                return View( detailsModel );
            }
            catch(HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Create( string projectId ) {
            ViewBag.projectId = projectId;
            try {

                await LoadDataForCreation();
                return View();
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create( MedicalTeamCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                await LoadDataForCreation();
                return View();
            }

            Guid userId = new Guid( request.UserId );        
            try {
                MedicalTeamModel medicalTeam =  await _medicalTeamsService
                    .CreateAsync( request.ProjectId, request );

                await AssignMedicalTeamAdmin(
                    userId, medicalTeam.MedicalTeamId, request.ProjectId );
            }
            catch ( HttpRequestException ex ) {

                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddMedicalTeamConflictError( ex.Message );
                }
                else {
                    AddGenericHttpRequestErrorMessage( ex );
                }

                await LoadDataForCreation();
                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Medical team created",
                string.Empty );

            return RedirectToAction(
                "Index",
                new { projectId = request.ProjectId } );
        }

        private async Task AssignMedicalTeamAdmin(
            Guid userId, Guid medicalTeamId, string projectId ) {
            AssignMedicalTeamAdminRoleToUser( userId );
            var assignAdminRequest = new MedicalTeamAssignAdminRequest() {
                UserId = userId
            };

            await _medicalTeamsService.AssignMedicalTeamAdministrator(
                projectId,
                medicalTeamId.ToString(),
                assignAdminRequest );
        }

        private async Task AddMedicToMedicalTeam( Guid userId, Guid medicalTeamId ) {
            AssignMedicalProfessionalRoleToUser( userId );

            var request = new AssignUserToMedicalTeamRequest() {
                UserId = userId
            };
            await _medicsService.CreateMedicWithinMedicalTeamAsync(
                medicalTeamId.ToString(), request );
        }

        private async void AssignMedicalTeamAdminRoleToUser( Guid userId ) {
            try {
                var request = new AssignRoleToUserRequest() {
                    UserId = userId,
                    Role = UserRoles.MedicalTeamAdmin
                };
                await _userService.AssignRole( request );
            }
            catch(HttpRequestException) { }         
        }

        private async void AssignMedicalProfessionalRoleToUser( Guid userId ) {
            try {
                var request = new AssignRoleToUserRequest() {
                    UserId = userId,
                    Role = UserRoles.MedicalProfessional
                };
                await _userService.AssignRole( request );
            }
            catch ( HttpRequestException ) { }
        }

        public async Task<ActionResult> Update( string projectId, string medicalTeamId ) {
            try {
                var medicalTeam = await _medicalTeamsService
                    .DetailsAsync( projectId, medicalTeamId );

                var model = new MedicalTeamEditRequest() {
                    ProjectId = projectId,
                    MedicalTeamId = medicalTeamId,
                    Name = medicalTeam.Name,
                    AddressLine1 = medicalTeam.AddressLine1,
                    AddressLine2 = medicalTeam.AddressLine2,
                    City = medicalTeam.City,
                    Country = medicalTeam.Country,
                    Phone = medicalTeam.Phone,
                    PostalCode = medicalTeam.PostalCode,
                    RegionCode = medicalTeam.RegionCode,
                    StateOrProvince = medicalTeam.StateOrProvince,
                    TimeZone = medicalTeam.RegionCode
                };

                ViewBag.admins = await _medicalTeamsService
                .GetAdminsAsync( projectId, medicalTeamId );

                ViewBag.projectId = projectId;
                return View( model );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Update( MedicalTeamEditRequest request ) {

            ViewBag.admins = await _medicalTeamsService
                .GetAdminsAsync( request.ProjectId, request.MedicalTeamId );

            ViewBag.projectId = request.ProjectId;
            if ( !ModelState.IsValid ) {
                return View(request);
            }
            try {

                await _medicalTeamsService.UpdateAsync( request );

                AddSuccessMessage( "Medical team updated", string.Empty );
                ShowMessage();
                return Redirect(nameof(Index));
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddMedicalTeamConflictError( ex.Message );
                }
                else {
                    AddErrorMessage(
                        ex.StatusCode.ToString(),
                        ex.Message );
                }

                ShowMessage();
                return View(request);
            }
        }


        [HttpPost]
        public async Task<ActionResult> Delete( string projectId, string medicalTeamId ) {
            try {

                await _medicalTeamsService
                    .DeleteAsync( projectId, medicalTeamId );

                AddSuccessMessage(
                    "Medical Team Deleted",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index", new { projectId = projectId } );
        }

        [HttpPost]
        public async Task<ActionResult> Close( string projectId, string medicalTeamId ) {
            try {
                await _medicalTeamsService
                    .CloseAsync( projectId, medicalTeamId );

                AddSuccessMessage(
                    "Medical Team closed",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Close request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index", new { projectId = projectId } );
        }

        [HttpPost]
        public async Task<ActionResult> Open( string projectId, string medicalTeamId ) {
            try {
                await _medicalTeamsService
                    .OpenAsync( projectId, medicalTeamId );

                AddSuccessMessage(
                    "Medical Team opened",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Open request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index", new { projectId = projectId } );
        }

        private async Task LoadDataForCreation() {
            var projects = await _projectService
            .GetOpenProjectAsync();

            TempData["projects"] = projects
                  .OrderBy( p => p.Name );

            var medics = await _medicsService
                .GetAllMedics();

            TempData["medics"] = medics
                .FindAll( u => u.State == UserSubscriptionState.Active )
                .OrderBy( u => u.Name );
        }

        private void AddMedicalTeamConflictError() {
            AddErrorMessage(
            "Conflict Error",
            "A Medica team with this name already exists" );
        }

        private void AddMedicalTeamConflictError( string message ) {
            AddErrorMessage(
            "Conflict Error",
            message );
        }
    }
}
