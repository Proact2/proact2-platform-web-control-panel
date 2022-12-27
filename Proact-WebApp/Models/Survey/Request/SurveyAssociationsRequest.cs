using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace Proact_WebApp.Models {
    public enum SurveyReccurence {
        Once = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public class UserModelSelectable {
        public bool Selected { get; set; }
        public UserModel UserModel { get; set; }
    }

    public class SurveyAssignationsRequest: IValidatableObject {
        public Guid SurveyId { get; set; }

        [Required]
        [DisplayName( "Start date" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime StartTime { get; set; }

        [Required]
        [DisplayName( "End date" )]
        [DisplayFormat( DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true )]
        public DateTime ExpireTime { get; set; }

        [Required]
        public SurveyReccurence Reccurence { get; set; }

        public string MedicalTeamId { get; set; }

        public List<Guid> UserIds {
            get {
                return Users
                    .Where( u => u.Selected )
                    .Select( x => x.UserModel.UserId )
                    .ToList();
            }
        }

        [JsonIgnore]
        public List<UserModelSelectable> Users { get; set; }

        public IEnumerable<ValidationResult> Validate( ValidationContext validationContext ) {

            if ( ExpireTime < StartTime ) {
                yield return new ValidationResult(
                    errorMessage: "Expire date must be greater than Start date",
                    memberNames: new[] { "ExpireTime" }
               );
            }
        }

        public SurveyAssignationsRequest() {
            Users = new List<UserModelSelectable>();
        }
    }
}
