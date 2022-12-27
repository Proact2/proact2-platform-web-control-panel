using System;
using System.Collections.Generic;
using System.Linq;

namespace Proact_WebApp.Models {
   
    public class QuestionAnswer {
        public string Value { get; set; }
        public int Count { get; set; }
    }

    public class QuestionWithAnswers {
        public string Title { get; set; }
        public string Question { get; set; }
        public SurveyQuestionType Type { get; set; }
        public List<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();

        public List<string> AswerValues {
            get {
                if(Type == SurveyQuestionType.Mood ) {
                    return new List<string> { "Very bad", "Bad", "Good", "Very good" };
                }
                else {
                    return Answers.Select( answer => answer.Value ).ToList();
                }
            }
        }

        public List<int> AswerCounts {
            get {
                return Answers.Select( answer => answer.Count ).ToList();
            }
        }
    }

    public class SurveyStatsResume {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public int Participants { get; set; }
        public int Completed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ExpireTime { get; set; }
        public List<QuestionWithAnswers> QuestionWithAnswers { get; set; } = new List<QuestionWithAnswers>();

        public List<QuestionWithAnswers> QuestionsWithoutOpenAnswers {
            get {
                return QuestionWithAnswers
                    .Where( item => item.Type != SurveyQuestionType.OpenAnswer )
                    .ToList();
            }
        }

        public int Incompleted {
            get { return Participants - Completed; }
        }
    }
}
