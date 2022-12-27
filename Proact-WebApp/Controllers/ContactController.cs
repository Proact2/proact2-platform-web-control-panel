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
    public class ContactController : BaseController {

        private IContactService _contactService;
        private IProjectService _projectService;

        private List<ProjectModel> _projects;
        private ProjectModel _selectedProject;

        public ContactController(
            IContactService contactService,
            IProjectService projectService ) {
            _contactService = contactService;
            _projectService = projectService;
        }

        public async Task<ActionResult> Update( string id ) {
            ShowMessage();
            try {
                await LoadProjects( id );
                UpdateViewbag();

                HtmlContentModel model = await _contactService
                    .GetContact( _selectedProject.ProjectId.ToString() );

                HtmlContentRequest request = new HtmlContentRequest();
                request.ProjectId = _selectedProject.ProjectId;
                request.HtmlContent = model.HtmlContent;

                return View( request );
            }
            catch ( HttpRequestException ex ) {

                if ( ex.StatusCode == System.Net.HttpStatusCode.NotFound ) {
                    HtmlContentRequest request = new HtmlContentRequest();
                    request.ProjectId = _selectedProject.ProjectId;
                    return View( request );
                }

                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( HtmlContentRequest request ) {

            await LoadProjects( request.ProjectId.ToString() );
            UpdateViewbag();

            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _contactService.CreateContact( request );

            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                       ex.StatusCode.ToString(),
                       ex.Message );
            }

            AddSuccessMessage(
                "Contact updated",
                string.Empty );

            ShowMessage();
            return View();
        }


        private async Task LoadProjects( string projectId ) {
            _projects = await _projectService.GetOpenProjectAsync();
            _selectedProject = _projects.FirstOrDefault();
            if ( !string.IsNullOrEmpty( projectId ) ) {
                _selectedProject = _projects.Find( x => x.ProjectId.ToString() == projectId );
            }
        }

        private void UpdateViewbag() {
            ViewBag.projects = _projects;
            ViewBag.selectedProject = _selectedProject;
        }
    }
}
