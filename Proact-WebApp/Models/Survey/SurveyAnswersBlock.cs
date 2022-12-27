using System;
using System.Collections.Generic;

namespace Proact_WebApp {

    public class SurveyAsnwersBlockLabelModel {
        public Guid Id { get; set; }
        public string Label { get; set; }
    }
        
    public class SurveyAnswersBlockModel {
        public Guid Id { get; set; }
        public List<SurveyAsnwersBlockLabelModel> Answers { get; set; }

        public string FullAnswers {
            get {
                var separator = " - ";
                var answers = string.Empty;
                foreach(var answer in Answers ) {
                    answers += answer.Label + separator;
                }
                answers = answers.Remove( answers.Length - separator.Length, separator.Length );
                return answers;
            }
        }
    }
}
