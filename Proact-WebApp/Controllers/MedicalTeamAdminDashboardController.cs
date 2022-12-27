using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamAdminDashboardController : BaseController {

        private MedicalTeamAdminDashboardModel _dashboardModel;

        private List<ProjectModel> _projectsModel;
        private ProjectModel _selectedProject;

        private IProjectService _projectService;
        private IProtocolService _protocolService;
        private IContactService _contactService;
        private IEmailEditorService _emailEditorService;

        public MedicalTeamAdminDashboardController(
            IProjectService projectService,
            IProtocolService protocolService,
            IContactService contactService,
            IEmailEditorService emailEditorService ) {

            _projectService = projectService;
            _protocolService = protocolService;
            _contactService = contactService;
            _emailEditorService = emailEditorService;
        }

        public async Task<IActionResult> Index( string id ) {
            try {
                _projectsModel = await _projectService.GetOpenProjectAsync();
                _selectedProject = _projectsModel.FirstOrDefault();
                if ( !string.IsNullOrEmpty( id ) ) {
                    _selectedProject = _projectsModel.Find( x => x.ProjectId.ToString() == id );
                }

                ViewBag.projects = _projectsModel;
                ViewBag.selectedProject = _selectedProject;

                await CheckServices();
                return View( _dashboardModel );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }       
        }

        private async Task CheckServices() {
            _dashboardModel = new MedicalTeamAdminDashboardModel();
            _dashboardModel.ServicesConfigurationStatus
                = new List<ConfigurationModel>();

            var isProtocolSetted
                = await IsProtocolSetted();

            var isContactPageSetted
                = await IsContactPageSetted();

            var isUserActivateEmailBodySetted
                = await IsUserActivateEmailBodySetted();

            var isUserSuspendedEmailBodySetted
                = await IsUserSuspendedEmailBodySetted();

            var isUserDeactivatedEmailBodySetted
                = await IsUserDeactivatedEmailBodySetted();


            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert Protocol",
                Controller = "Protocols",
                Action = "Index",
                RouteId = _selectedProject.ProjectId.ToString(),
                Completed = isProtocolSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert Contact page",
                Controller = "Contact",
                Action = "Update",
                RouteId = _selectedProject.ProjectId.ToString(),
                Completed = isContactPageSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert User activated email body",
                Controller = "EmailEditor",
                Action = "UpdateWelcome",
                RouteId = _selectedProject.ProjectId.ToString(),
                Completed = isUserActivateEmailBodySetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert User suspended email body",
                Controller = "EmailEditor",
                Action = "UpdateUserSuspended",
                RouteId = _selectedProject.ProjectId.ToString(),
                Completed = isUserSuspendedEmailBodySetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert User Deactivated email body",
                Controller = "EmailEditor",
                Action = "UpdateUserDeactivated",
                RouteId = _selectedProject.ProjectId.ToString(),
                Completed = isUserDeactivatedEmailBodySetted
            } );

        }

        private async Task<bool> IsProtocolSetted() {
            try {
                ProtocolModel protocol = await _protocolService
                    .GetProtocolAssignedToProject( _selectedProject.ProjectId.ToString() );
                return true;
            }
            catch ( HttpRequestException ex ) {
                return false;
            }
        }

        private async Task<bool> IsContactPageSetted() {
            try {
                var protocol = await _contactService
                    .GetContact( _selectedProject.ProjectId.ToString() );
                return true;
            }
            catch ( HttpRequestException ex ) {
                return false;
            }
        }

        private async Task<bool> IsUserActivateEmailBodySetted() {
            try {
                var protocol = await _emailEditorService
                    .GetWelcomeEmailBody( _selectedProject.ProjectId.ToString() );
                return true;
            }
            catch ( HttpRequestException ex ) {
                return false;
            }
        }

        private async Task<bool> IsUserSuspendedEmailBodySetted() {
            try {
                var protocol = await _emailEditorService
                    .GetUserSuspendedEmailBody( _selectedProject.ProjectId.ToString() );
                return true;
            }
            catch ( HttpRequestException ex ) {
                return false;
            }
        }

        private async Task<bool> IsUserDeactivatedEmailBodySetted() {
            try {
                var protocol = await _emailEditorService
                    .GetUserDeactivatedEmailBody( _selectedProject.ProjectId.ToString() );
                return true;
            }
            catch ( HttpRequestException ex ) {
                return false;
            }
        }
    }


}
