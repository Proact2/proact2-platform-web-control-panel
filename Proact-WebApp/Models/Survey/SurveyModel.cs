using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class SurveyModel {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public SurveyState SurveyState { get; set; }
        public List<QuestionModel> Questions { get; set; }

        [DisplayName( "Creation date" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}" )]
        public DateTime Created { get; set; }

        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}" )]
        public DateTime? StartTime { get; set; }
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}" )]
        public DateTime? ExpireTime { get; set; }
        public SurveyReccurence? Reccurence { get; set; }

        public Guid QuestionsSetId {
            get {
                if(Questions.Count > 0 ) {
                    return Questions[0].QuestionsSetId;
                }
                else {
                    return Guid.NewGuid();
                }
            }
        }

        public bool Expired {
            get {
                return SurveyState == SurveyState.PUBLISHED
                    && ( ExpireTime == null || ExpireTime < DateTime.UtcNow );
            }
        }
    }
}
