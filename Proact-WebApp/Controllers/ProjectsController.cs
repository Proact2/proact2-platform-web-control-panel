using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class ProjectsController : BaseController {

        private IProjectService _projectService;
        private ISessionService _sessionService;
        private IProjectPropertiesService _projectPropertiesService;
        private ILexiconService _lexiconService;

        public ProjectsController(
            IProjectService projectService,
            ISessionService sessionService,
            IProjectPropertiesService projectPropertiesService,
            ILexiconService lexiconService ) {
            _projectService = projectService;
            _sessionService = sessionService;
            _projectPropertiesService = projectPropertiesService;
            _lexiconService = lexiconService;
        }

        public async Task<ActionResult> Index( string showAll ) {
            var showOnlyOpenProject
                = string.IsNullOrEmpty( showAll );
            ViewBag.openOnly
                = showOnlyOpenProject;

            try {
                ShowMessage();
                if ( showOnlyOpenProject ) {
                    return View( await _projectService
                        .GetOpenProjectAsync() );
                }
                return View( await _projectService
                    .GetAllAsync() );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create() {
            return View( new ProjectCreateRequest() );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( ProjectCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _projectService.CreateAsync( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Conflict ) {
                    AddErrorMessage(
                        "Conflict Error",
                        "A Study with this name already exists" );
                }
                else {
                    AddErrorMessage(
                        ex.StatusCode.ToString(),
                        ex.Message );
                }

                ShowMessage();
                return View();
            }

            AddSuccessMessage(
                "Study created",
                string.Empty );

            return RedirectToAction( nameof( Index ) );
        }

        [HttpGet]
        public async Task<ActionResult> Update( string id ) {
            try {
                var projectModel = await _projectService.GetAsync( id );

                ProjectUpdateRequest request = new ProjectUpdateRequest {
                    ProjectId = new Guid( id ),
                    Name = projectModel.Name,
                    Description = projectModel.Description,
                    SponsorName = projectModel.SponsorName,
                    Status = projectModel.Status
                };

                return View( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update( ProjectUpdateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View( request );
            }
            try {
                var projectModel
                    = await _projectService.UpdateAsync( request );

                AddSuccessMessage(
                    "Study updated",
                    string.Empty );

                return RedirectToAction( nameof( Index ) );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return View( request );
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Properties( string id ) {
            try {
                var model = await _projectPropertiesService.GetAsync( id );

                var request = new ProjectPropertiesUpdateRequest() {
                    Id = new Guid( id ),
                    IsMessagingActive = model.IsMessagingActive,
                    IsAnalystConsoleActive = model.IsAnalystConsoleActive,
                    IsSurveysSystemActive = model.IsSurveysSystemActive,
                    MedicsCanSeeOtherAnalisys = model.MedicsCanSeeOtherAnalisys,
                    MessageCanBeAnalizedAfterMinutes = model.MessageCanBeAnalizedAfterMinutes,
                    MessageCanBeRepliedAfterMinutes = model.MessageCanBeRepliedAfterMinutes,
                    MessageCanNotBeDeletedAfterMinutes = model.MessageCanNotBeDeletedAfterMinutes 
                };

                return View( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction( nameof( Index ) );
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Properties( ProjectPropertiesUpdateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View( request );
            }
            try {
                var model
                    = await _projectPropertiesService.UpdateAsync(  request );

                AddSuccessMessage(
                    "Properties updated",
                    string.Empty );

                return RedirectToAction( nameof( Index ) );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return View( request );
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete( string id ) {
            try {
                await _projectService.DeleteAsync( new Guid( id ) );

                AddSuccessMessage(
                    "Study deleted",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }


        [HttpGet]
        public async Task<ActionResult> AssociateLexicon( string id ) {
            try {
                ShowMessage();
                await AddLexiconToViewBag();
                var request = new AssociateLexiconToProjectRequest() {
                    ProjectId = new Guid( id )
                };

                return View( request );
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction( nameof( Index ) );
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssociateLexicon( AssociateLexiconToProjectRequest request ) {
            try {

                if ( !ModelState.IsValid ) {
                    await AddLexiconToViewBag();
                    return View( request );
                }
                try {
                    await _projectPropertiesService.AssociateLexicon( request );

                    AddSuccessMessage(
                        "Lexicon associated to the Study",
                        string.Empty );

                    return RedirectToAction( nameof( Index ) );
                }
                catch ( HttpRequestException ex ) {
                    await AddLexiconToViewBag();
                    if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                        return GetErrorView( ex.StatusCode );
                    }
                    else {
                        AddErrorMessage(
                            ex.Message,
                            ex.StatusCode.ToString() );
                        ShowMessage();
                        return View( request );
                    }
                }
            }
            catch ( HttpRequestException ex ) {
                if ( ex.StatusCode == System.Net.HttpStatusCode.Forbidden ) {
                    return GetErrorView( ex.StatusCode );
                }
                else {
                    AddErrorMessage(
                        ex.Message,
                        ex.StatusCode.ToString() );
                    ShowMessage();
                    return RedirectToAction( nameof( Index ) );
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Close( string projectId ) {
            try {
                await _projectService
                    .CloseAsync( projectId );

                AddSuccessMessage(
                    "Study closed",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Close request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }

        [HttpPost]
        public async Task<ActionResult> Open( string projectId ) {
            try {
                await _projectService
                    .OpenAsync( projectId );

                AddSuccessMessage(
                    "Study opened",
                    string.Empty );
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Open request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }

        private async Task AddLexiconToViewBag() {
            ViewBag.lexicons = await _lexiconService.GetPublishedAsync();
        }
    }
}
