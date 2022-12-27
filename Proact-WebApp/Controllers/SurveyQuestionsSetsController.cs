using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Controllers;
using Proact_WebApp.Models;
using Proact_WebApp.ViewModels;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class SurveyQuestionsSetsController : BaseController {

        private ISurveysQuestionsSetsService _questionSetService;
        private IProjectService _projectService;
        private int _answersCapacity = 10;

        public SurveyQuestionsSetsController(
            ISurveysQuestionsSetsService surveysService,
            IProjectService projectService ) {
            _questionSetService = surveysService;
            _projectService = projectService;
        }

        public async Task<ActionResult> Index( string id ) {

            ShowMessage();

            try {

                var projects = await _projectService.GetOpenProjectAsync();
                var selectedProject = projects.FirstOrDefault();
                if ( !string.IsNullOrEmpty( id ) ) {
                    selectedProject = projects
                        .Find( x => x.ProjectId.ToString() == id );
                }

                ViewBag.projects = projects;
                ViewBag.selectedProject = selectedProject;

                return View( await _questionSetService
                    .GetQuestionsSetsAsync( selectedProject.ProjectId.ToString() ) );
            }
            catch ( HttpRequestException ex ) {
                return GetErrorView( ex.StatusCode );
            }
        }

        public ActionResult Create( string projectId ) {
            var model = new SurveyQuestionSetCreateRequest() {
                ProjectId = new Guid( projectId )
            };
            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( SurveyQuestionSetCreateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View();
            }

            try {
                await _questionSetService.CreateQuestionsSetAsync( request );
            }
            catch ( HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
                ShowMessage();
                return View();
            }
            AddCreateSuccessMessage();
            return RedirectToAction( nameof( Index ) );
        }

        [HttpGet]
        public async Task<ActionResult> Edit( string id ) {
            try {
                var model = await _questionSetService.GetQuestionsSetAsync( id );

                var request = new SurveyQuestionSetUpdateRequest {
                    QuestionSetId = new Guid( id ),
                    Title = model.Title,
                    Description = model.Description,
                    Version = model.Version
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
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit( SurveyQuestionSetUpdateRequest request ) {
            if ( !ModelState.IsValid ) {
                return View( request );
            }
            try {
                var model
                    = await _questionSetService.UpdateQuestionsSetAsync( request );

                AddUpdateSuccessMessage();
                return RedirectToAction( "Index" );
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
        public async Task<ActionResult> Delete( string id ) {
            try {
                await _questionSetService.DeleteQuestionsSet( id );
                AddSetDeletedMessage();
            }
            catch ( HttpRequestException ex ) {

                AddErrorMessage(
                    "Delete request error",
                    ex.StatusCode.ToString() );
            }

            return RedirectToAction( "Index" );
        }

        public async Task<ActionResult> QuestionsList( string id ) {
            ShowMessage();
            var model = await _questionSetService.GetQuestionsSetAsync(id); ;
            return View( model );
        }

        public async Task<ActionResult> AddQuestionToSet( string id, string projectId ) {
            var answersBlocks = await _questionSetService.GetAnswersBlocks( projectId );
            var request = new SurveyAddQuestionToSetViewModel() {
                QuestionSetId = new Guid( id ),
                RatingQuestionParams = new SurveyRatingQuestionParamsModel(),
                AnswersBlocks = answersBlocks,
                ProjectId = new Guid( projectId )
            };
            return View( request );
        }

        [HttpPost]
        public async Task<ActionResult> AddQuestionToSet( SurveyAddQuestionToSetViewModel viewModel ) {

            try {

                var baseRequest = new SurveyAddQuestionToSetRequest() {
                    QuestionSetId = viewModel.QuestionSetId,
                    Title = viewModel.Title,
                    Question = viewModel.Question
                };

                switch ( viewModel.Type ) {
                    case SurveyQuestionType.OpenAnswer:
                        
                    await _questionSetService.AddOpenQuestionToSet( baseRequest );

                    break;
                    case SurveyQuestionType.Boolean:

                    await _questionSetService.AddBoolQuestionToSet( baseRequest );
                    break;
                    case SurveyQuestionType.Mood:

                    await _questionSetService.AddMoodQuestionToSet( baseRequest );
                    break;
                    case SurveyQuestionType.Rating:

                    var ratingRequest = new SurveyAddRatingQuestionToSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        Min = viewModel.RatingQuestionParams.MinValue,
                        Max = viewModel.RatingQuestionParams.MaxValue,
                        MinLabel = viewModel.RatingQuestionParams.MinLabel,
                        MaxLabel = viewModel.RatingQuestionParams.MaxLabel,
                    };
                    await _questionSetService.AddRatingQuestionToSet( ratingRequest );

                    break;
                    case SurveyQuestionType.SingleChoice:

                    var scRequest = new SurveyAddSingleChoiceQuestionToSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        AnswersBlockId = viewModel.SelectedAnswersBlockId
                    };
                    await _questionSetService.AddSingleChoiceQuestionToSet( scRequest );

                    break;
                    case SurveyQuestionType.MultipleChoice:

                    var mcRequest = new SurveyAddMultipleChoiceQuestionToSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        AnswersBlockId = viewModel.SelectedAnswersBlockId
                    };
                    await _questionSetService.AddMultipleChoiceQuestionToSet( mcRequest );

                    break;
                }

                AddQuestionAddedToSetSuccessMessage();
            }
            catch ( HttpRequestException ex) {
                AddQuestionAddedToSetErrorMessage();
            }

            return RedirectToAction(
                nameof( QuestionsList ),
                new { id = viewModel.QuestionSetId.ToString() } );
        }

        [HttpGet]
        public async Task<ActionResult> UpdateQuestionOfSet( string questionsSetId, string questionId, string projectId ) {
            try {
                var questionSetModel = await _questionSetService
                    .GetQuestionsSetAsync( questionsSetId );

                var questionModel = questionSetModel.Questions
                    .Find( q => q.Id.ToString() == questionId );

                var answersBlocks = await _questionSetService.GetAnswersBlocks( questionSetModel.ProjectId.ToString() );

                var vm = new SurveyUpdateQuestionOfSetViewModel {
                    QuestionSetId = new Guid( questionsSetId ),
                    Title = questionModel.Title,
                    Question = questionModel.Question,
                    QuestionId = questionModel.Id,
                    AnswersBlocks = answersBlocks,
                    Type = questionModel.Properties.Type,
                    ProjectId = new Guid( projectId )
                };

                if ( questionModel.Properties.Type == SurveyQuestionType.SingleChoice ) {

                    var answersContainer
                     = ( SurveysSingleChoiceQuestionModelAnswersContainer )questionModel
                     .AnswersContainer;
                    vm.SelectedAnswersBlockId = answersContainer.AnswersBlockId;
                }
                else if ( questionModel.Properties.Type == SurveyQuestionType.MultipleChoice ) {

                    var answersContainer
                     = ( SurveysMultipleChoiceQuestionModelAnswersContainer )questionModel
                     .AnswersContainer;
                    vm.SelectedAnswersBlockId = answersContainer.AnswersBlockId;
                }
                else if ( questionModel.Properties.Type == SurveyQuestionType.Rating ) {

                    var ratingProperties
                        = ( SurveysRatingQuestionModelProperties )questionModel
                        .Properties;

                    vm.RatingQuestionParams = new SurveyRatingQuestionParamsModel() {
                        MinValue = ratingProperties.Min,
                        MaxValue = ratingProperties.Max,
                        MinLabel = ratingProperties.MinLabel,
                        MaxLabel = ratingProperties.MaxLabel
                    };
                }

                return View( vm );
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
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateQuestionOfSet( SurveyUpdateQuestionOfSetViewModel viewModel ) {
            try {

                var baseRequest = new SurveyUpdateQuestionOfSetRequest() {
                    QuestionSetId = viewModel.QuestionSetId,
                    QuestionId = viewModel.QuestionId,
                    Title = viewModel.Title,
                    Question = viewModel.Question
                };

                switch ( viewModel.Type ) {
                    case SurveyQuestionType.OpenAnswer:
                    await _questionSetService.UpdateOpenQuestionOfSet( baseRequest );

                    break;
                    case SurveyQuestionType.Boolean:
                    await _questionSetService.UpdateBoolQuestionOfSet( baseRequest );

                    break;
                    case SurveyQuestionType.Mood:
                    await _questionSetService.UpdateMoodQuestionOfSet( baseRequest );

                    break;
                    case SurveyQuestionType.Rating:

                    var rRequest = new SurveyUpdateRatingQuestionOfSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        QuestionId = viewModel.QuestionId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        Min = viewModel.RatingQuestionParams.MinValue,
                        Max = viewModel.RatingQuestionParams.MaxValue,
                        MinLabel = viewModel.RatingQuestionParams.MinLabel,
                        MaxLabel = viewModel.RatingQuestionParams.MaxLabel
                    };

                    await _questionSetService.UpdateRatingQuestionOfSet( rRequest );
                    break;
                    case SurveyQuestionType.SingleChoice:

                    var scRequest = new SurveyUpdateSingleChoiceQuestionOfSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        QuestionId = viewModel.QuestionId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        AnswersBlockId = viewModel.SelectedAnswersBlockId
                    };
                    await _questionSetService.UpdateSingleChoiceQuestionOfSet( scRequest );
                    break;

                    case SurveyQuestionType.MultipleChoice:

                    var mcRequest = new SurveyUpdateMultipleChoiceQuestionOfSetRequest() {
                        QuestionSetId = viewModel.QuestionSetId,
                        QuestionId = viewModel.QuestionId,
                        Title = viewModel.Title,
                        Question = viewModel.Question,
                        AnswersBlockId = viewModel.SelectedAnswersBlockId
                    };
                    await _questionSetService.UpdateMultipleChoiceQuestionOfSet( mcRequest );
                    break;
                }

                AddQuestionUpdatedSuccessMessage();
            }
            catch ( HttpRequestException ) {
                AddGenericErrorMessage();
            }
            
            return RedirectToAction(
                nameof( QuestionsList ),
                new { id = viewModel.QuestionSetId.ToString() } );
        }

        [HttpPost]
        public async Task<ActionResult> RemoveQuestionFromSet( string questionId, string questionSetId ) {
            try {
                var removed = await _questionSetService.RemoveQuestionFromSet( questionSetId, questionId );
                if ( removed ) {
                    AddQuestionDeletedMessage();
                }
                else {
                    AddGenericErrorMessage();
                }
            }
            catch(HttpRequestException ex ) {
                AddGenericHttpRequestErrorMessage( ex );
            }

            return RedirectToAction(
                   nameof( QuestionsList ),
                   new { id = questionSetId } );
        }

        [HttpPost]
        public async Task<ActionResult> SaveAnswersAsync( string[] answers, string projectId ) {
            var request = new SurveyAnswersBlockCreateRequest();
            request.ProjectId = new Guid( projectId );
            request.Labels = answers.Where( s => !string.IsNullOrWhiteSpace( s ) )
                .Distinct().ToList();

            try {
                var result = await _questionSetService.CreateAnswersBlock( request );
                KeyValuePair<string, string> valuePair
                    = new KeyValuePair<string, string>( result.Id.ToString(), result.FullAnswers );
                return Json( result );
            }
            catch ( Exception ) {
                return Json( false );
            }
        }

        [HttpGet]
        public async Task<ActionResult> Validate( string id ) {
            try {
                var model = await _questionSetService.GetQuestionsSetAsync( id );
                return View( model );
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
                    return View();
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Validate( Guid id ) {
            try {
                var request = new SurveyValidateQuestionsSetRequest() {
                    QuestionsSetId = id
                };
                await _questionSetService.Validate( request );
                AddQuestionsSetValidationSuccess();
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
                    return View();
                }
            }
        }

        private void AddCreateSuccessMessage() {
            AddSuccessMessage(
            "Questions sets created.",
            string.Empty );
        }

        private void AddUpdateSuccessMessage() {
            AddSuccessMessage(
            "Questions sets updated.",
            string.Empty );
        }

        private void AddSetDeletedMessage() {
            AddSuccessMessage(
            "Questions sets deleted.",
            string.Empty );
        }

        private void AddQuestionAddedToSetSuccessMessage() {
            AddSuccessMessage(
            "The question was successfully added to the set",
            string.Empty );
        }

        private void AddQuestionAddedToSetErrorMessage() {
            AddErrorMessage(
            "An error has occurred, it was not possible to add the question to the set.",
            string.Empty );
        }

        private void AddQuestionDeletedMessage() {
            AddSuccessMessage(
            "Questions successfully deleted.",
            string.Empty );
        }

        private void AddQuestionUpdatedSuccessMessage() {
            AddSuccessMessage(
            "The question was successfully updated.",
            string.Empty );
        }

        private void AddQuestionsSetValidationSuccess() {
            AddSuccessMessage(
            "The question set has been validated. It can't be changed anymore.",
            string.Empty );
        }

    }
}
