using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proact_WebApp.Controllers;
using Proact_WebApp.Models;


namespace Proact_WebApp {

    public class ProtocolsController : BaseController {

        private IProtocolService _protocolService;
        private IProjectService _projectService;

        public ProtocolsController(
            IProtocolService protocolService,
            IProjectService projectService ) {
            _protocolService = protocolService;
            _projectService = projectService;
        }

        public async Task<ActionResult> Index( string id ) {
            ShowMessage();
            try {
                var projects = await _projectService.GetOpenProjectAsync();
                var selectedProject = projects.FirstOrDefault();
                if ( !string.IsNullOrEmpty( id ) ) {
                    selectedProject = projects.Find( x => x.ProjectId.ToString() == id );
                }

                ViewBag.projects = projects;
                ViewBag.selectedProject = selectedProject;

                ProtocolModel protocol = await _protocolService
                    .GetProtocolAssignedToProject( selectedProject.ProjectId.ToString() );
               
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

        public ActionResult Create( string id ) {
            var request = new CreateProtocolAssignedToProjectRequest() {
                ProjectId = new Guid( id )
            };
            return View( request );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( CreateProtocolAssignedToProjectRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                var protocol = await _protocolService
                    .CreateProtocolAssignedToProjectAsync( request );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                      ex.StatusCode.ToString(),
                      ex.Message );

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Protocol created",
                string.Empty );

            return RedirectToAction(
                nameof( Index ),
                new { id = request.ProjectId }
                );
        }

        public async Task<ActionResult> Update( string id ) {
            try {
                ProtocolModel protocol = await _protocolService
                    .GetProtocolAssignedToProject( id );

                var model = new CreateProtocolAssignedToProjectRequest() {
                    Name = protocol.Name,
                    InternalCode = protocol.InternalCode,
                    InternationalCode = protocol.InternationalCode,
                    ProjectId = new Guid( id )
                };
                return View( model );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( CreateProtocolAssignedToProjectRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _protocolService
                    .CreateProtocolAssignedToProjectAsync( request );
            }
            catch ( HttpRequestException ex ) {
                AddErrorMessage(
                      ex.StatusCode.ToString(),
                      ex.Message );

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Protocol updated",
                string.Empty );

            return RedirectToAction(
                nameof( Index ),
                new { id = request.ProjectId } );
        }
    }
}
