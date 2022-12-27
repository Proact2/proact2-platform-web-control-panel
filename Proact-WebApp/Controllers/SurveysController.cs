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
    public class SurveysController : BaseController {

        private ISurveysService _surveysService;
        private ISurveysQuestionsSetsService _surveysQuestionsSetsService;
        private IMedicalTeamsService _medicalTeamService;
        private IPatientsService _patientsService;
        private IProjectService _projectService;

        private List<SurveyQuestionSetModel> _surveysQuestionsSets;

        public SurveysController(
            ISurveysService surveysService,
            ISurveysQuestionsSetsService surveysQuestionsSetsService,
            IMedicalTeamsService medicalTeamsService,
            IPatientsService patientsService,
            IProjectService projectService ) {
            _surveysService = surveysService;
            _surveysQuestionsSetsService = surveysQuestionsSetsService;
            _medicalTeamService = medicalTeamsService;
            _patientsService = patientsService;
            _projectService = projectService;
        }

        public async Task<ActionResult> Index( string id ) {
            try {
                ShowMessage();

                var projects = await _projectService.GetOpenProjectAsync();
                var selectedProject = projects.FirstOrDefault();
                if ( !string.IsNullOrEmpty( id ) ) {
                    selectedProject = projects.Find( x => x.ProjectId.ToString() == id );
                }

                ViewBag.projects = projects;
                ViewBag.selectedProject = selectedProject;

                var surveys = await _surveysService.GetSurveysAsync( selectedProject.ProjectId.ToString() );    
                return View( surveys );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public async Task<ActionResult> Create( string questionSetId, string projectId ) {
            ShowMessage();
            AddQuestionsSetIdToViewBag( questionSetId );
            await LoadDataForCreation( projectId );

            SurveyCreateRequest request = new SurveyCreateRequest();
            if ( _surveysQuestionsSets != null && _surveysQuestionsSets.Count > 0 ) {

                var selectedSet = _surveysQuestionsSets[0];
                AddQuestionsSetIdToViewBag( selectedSet.Id.ToString() );

                if ( !string.IsNullOrEmpty( questionSetId ) ) {
                    selectedSet = _surveysQuestionsSets
                        .Find( x => x.Id.ToString() == questionSetId );
                }
                request.QuestionsSetId = selectedSet.Id;
                request.Title = selectedSet.Title;
                request.Description = selectedSet.Description;
                request.Version = selectedSet.Version;
                request.Questions = selectedSet.Questions;
                request.ProjectId = new Guid( projectId );
            }
            else {
                ShowNoPublishedSetsAvailable();
                return RedirectToAction( nameof( Index ) );
            }
            return View( request );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( SurveyCreateRequest request ) {
            bool invalidRequest = !ModelState.IsValid;
            if(request.questionsIds == null || request.questionsIds.Length == 0 ) {
                AddAQuestionMustCheckedErrorMessage();
                invalidRequest = true;
            }

            if ( invalidRequest ) {
                return RedirectToAction(
                    nameof( Create ),
                    new {
                        questionSetId = request.QuestionsSetId,
                        projectId = request.ProjectId
                    }
                    );
            }

            try {
                await _surveysService.CreateSurvey( request );
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
                ShowMessage();
                return View();
            }
            AddCreateSuccessMessage();
            return RedirectToAction( nameof( Index ) );
        }

        [HttpPost]
        public async Task<ActionResult> Delete( string id ) {
            try {
                await _surveysService.DeleteSurvey( id );
                AddDeleteMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( nameof( Index ) );
        }

        public async Task<ActionResult> Edit( string id ) {
            ShowMessage();
            try {
                var surveyModel = await _surveysService.GetSurveyAsync( id );
                var questionSetModel = await _surveysQuestionsSetsService
                    .GetQuestionsSetAsync( surveyModel.QuestionsSetId.ToString() );

                SurveyEditRequest request = new SurveyEditRequest() {
                    SurveyId = surveyModel.Id,
                    QuestionsSetId = surveyModel.QuestionsSetId,
                    Title = surveyModel.Title,
                    Description = surveyModel.Description,
                    Version = surveyModel.Version,
                    Questions=  new List<QuestionModel>()
                };

                foreach ( var question in questionSetModel.Questions ) {

                    if ( surveyModel.Questions.Find( q => q.Id == question.Id ) != null ) {
                        question.Selected = true;
                    }
                    request.Questions.Add( question );
                }
             
                return View( request ); 

            }catch(HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
                return RedirectToAction( nameof( Index ) );
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit( SurveyEditRequest request ) {
            bool invalidRequest = !ModelState.IsValid;
            if ( request.questionsIds.Length == 0) {
                AddAQuestionMustCheckedErrorMessage();
                invalidRequest = true;
            }

            if ( invalidRequest ) {
                return RedirectToAction(
                    nameof( Edit ),
                    new { id = request.SurveyId } );
            }

            try {
                await _surveysService.UpdateSurvey( request );
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
                ShowMessage();
                return View();
            }

            AddUpdateSuccessMessage();
            return RedirectToAction( nameof( Index ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Duplicate( string id ) {
            try {
                var surveyModel = await _surveysService.GetSurveyAsync( id );
                foreach(var question in surveyModel.Questions ) {
                    question.Selected = true;
                }
                SurveyCreateRequest request = new SurveyCreateRequest() {
                    QuestionsSetId = surveyModel.QuestionsSetId,
                    Description = surveyModel.Description,
                    Title = surveyModel.Title,
                    Version = surveyModel.Version + "(Clone)",
                    Questions = surveyModel.Questions
                };
                await _surveysService.CreateSurvey( request );
                AddDuplicateSuccessMessage();
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
            }
            return RedirectToAction( nameof( Index ) );
        }

        public async Task<ActionResult> QuestionsList( string id ) {
            ShowMessage();
            var model = await _surveysService.GetSurveyAsync( id ); ;
            return View( model );
        }

        [HttpGet]
        public async Task<ActionResult> AssignationsConfigurations(string id ) {
            ViewBag.published = false;
            ViewBag.expired = false;

            var surveyModel = await _surveysService.GetSurveyAsync( id );

            SurveyAssignationsRequest request = new SurveyAssignationsRequest();
            request.SurveyId = new Guid( id );

            if (surveyModel.SurveyState == SurveyState.PUBLISHED ) {
                ViewBag.published = true;

                if ( surveyModel.ExpireTime != null
                    && surveyModel.ExpireTime < DateTime.UtcNow ) {

                    ViewBag.expired = true;
                    request.StartTime = (DateTime)surveyModel.StartTime;
                }
                else {
                    ViewBag.expired = false;
                    request.StartTime = DateTime.UtcNow;
                }

                request.ExpireTime = surveyModel.ExpireTime != null
                    ? ( DateTime )surveyModel.ExpireTime
                    : DateTime.UtcNow;
                request.Reccurence = surveyModel.Reccurence != null
                    ? ( SurveyReccurence)surveyModel.Reccurence
                    :  SurveyReccurence.Once;
            }
            else {
                ViewBag.published = false;
                request.StartTime = DateTime.Now;
                request.ExpireTime = DateTime.Now.AddDays( 7 );
                request.Reccurence = SurveyReccurence.Once;
            }

            return View( request );
        }

        [HttpPost]
        public ActionResult AssignationsConfigurations( SurveyAssignationsRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }
            else {
                return RedirectToAction(
                    nameof( AssignationsPatientsSelection ),
                    new {
                        SurveyId = request.SurveyId,
                        StartTime = request.StartTime,
                        ExpireTime = request.ExpireTime,
                        Reccurence = request.Reccurence } );
            }
        }

        [HttpGet]
        public async Task<ActionResult> AssignationsPatientsSelection( SurveyAssignationsRequest request ) {
            ShowMessage();
            try {
             
                var surveyModel = await _surveysService.GetSurveyAsync( request.SurveyId.ToString() );
                var projectId = surveyModel.ProjectId;

                var patientsmodel = await _patientsService
                    .GetPatientsAssignedToProject( projectId.ToString() );

                request.Users = new List<UserModelSelectable>();
                foreach( var patientModel in patientsmodel ) {
                    request.Users.Add( new UserModelSelectable() {
                        UserModel = patientModel
                    } ); ;
                }

                return View( request );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpPost]
        public async Task<ActionResult> AssignationsPatients( SurveyAssignationsRequest request ) {
            if ( request.UserIds.Count == 0 ) {
                AddUsersMustCheckedErrorMessage();
                return RedirectToAction( nameof( AssignationsPatientsSelection ), new { request } );
            }
            try {
                await _surveysService.PerformAssignations( request );
                AddSurveyAssignationSuccessMessage();
                return RedirectToAction( nameof( Index ) );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpGet]
        public async Task<ActionResult> PatientsAssigned(string id ) {
            try {
                var patientsList =
                    await _surveysService.GetPatientsAssignedToSurvey( id );

                return View( patientsList );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpGet]
        public async Task<ActionResult> Compiled( string id ) {
            try {
                var compiledSurveyModel =
                    await _surveysService.GetCompiledSurvey( id );

                return View( compiledSurveyModel );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        [HttpGet]
        public async Task<ActionResult> Stats( string id ) {
            try {
                var surveyStats =
                    await _surveysService.GetSurveyStatsResume( id );

                return View( surveyStats );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        private async Task LoadDataForCreation( string projectId ) {
            var allQuestionsSets = await _surveysQuestionsSetsService
               .GetQuestionsSetsAsync( projectId );

            _surveysQuestionsSets = allQuestionsSets
                .Where( s => s.State == SurveyState.PUBLISHED )
                .ToList();

            List<KeyValuePair<string, string>> questionsSetsViewBinded
              = new List<KeyValuePair<string, string>>();

            if ( _surveysQuestionsSets != null ) {
                foreach ( var questionSet in _surveysQuestionsSets ) {
                    questionsSetsViewBinded
                        .Add( new KeyValuePair<string, string>(
                            questionSet.Id.ToString(),
                            $"{questionSet.Title} ({questionSet.Version})" ) );
                }
            }
            ViewBag.questionsSets = questionsSetsViewBinded;
        }

        private void AddMedicalTeamsToViewBag( List<MedicalTeamModel> medicalTeams ) {
            ViewBag.medicalTeams = medicalTeams;
        }

        private void AddMedicalTeamIdToViewBag( string medicalTeamId ) {
            if ( !string.IsNullOrEmpty( medicalTeamId ) ) {
                ViewBag.medicalTeamId = medicalTeamId;
            }
        }

        private void AddQuestionsSetIdToViewBag( string questionSetId ) {
            ViewBag.questionsSetId = questionSetId;
        }

        private void AddPatientsToViewBag( List<PatientModel> patients ) {
            ViewBag.patients = patients;
        }

        private void AddCreateSuccessMessage() {
            AddSuccessMessage(
            "Survey created",
            string.Empty );
        }

        private void AddDuplicateSuccessMessage() {
            AddSuccessMessage(
            "Survey duplicated",
            string.Empty );
        }

        private void AddUpdateSuccessMessage() {
            AddSuccessMessage(
            "Survey updated",
            string.Empty );
        }

        private void AddDeleteMessage() {
            AddSuccessMessage(
            "Survey deleted",
            string.Empty );
        }

        private void AddAQuestionMustCheckedErrorMessage() {
            AddErrorMessage(
                "You must check at least 1 question",
                string.Empty );
        }

        private void AddSurveyAssignationSuccessMessage() {
            AddSuccessMessage(
            "the survey was assigned to patients",
            string.Empty );
        }

        private void AddUsersMustCheckedErrorMessage() {
            AddErrorMessage(
                "You must check at least 1 user",
                string.Empty );
        }

        private void ShowNoPublishedSetsAvailable() {
            AddErrorMessage(
                "You must publish at least one Questions Set to be able to create a survey",
                string.Empty );
        }
    }
}

