using System;
using System.Collections.Generic;
using Proact_WebApp.Models;

namespace Proact_WebApp.ViewModels {
    public class SurveyAddQuestionToSetViewModel {
        public Guid QuestionSetId { get; set; }
        public Guid ProjectId { get; set; }
        public SurveyQuestionType Type { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public Guid SelectedAnswersBlockId { get; set; }
        public List<SurveyAnswersBlockModel> AnswersBlocks { get; set; }
        public SurveyRatingQuestionParamsModel RatingQuestionParams { get; set; }
    }
}
