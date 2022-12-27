using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class PatientModel : UserModel {

        [Required]
        [DisplayName( "Patient code" )]
        public string Code { get; set; }

        [Required]
        public bool ShowBroadcastMessages { get; set; }

        public int BirthYear { get; set; }

        public string Gender { get; set; }

        public DateTime? TreatmentStartDate { get; set; }
        public DateTime? TreatmentEndDate { get; set; }

        public IEnumerable<Guid> DrugTargets { get; set; }

        public List<MedicalTeamModel> MedicalTeam { get; set; }

        public bool IsAssociatedToMedicalTeam {
            get {
                return MedicalTeam != null && MedicalTeam.Count > 0;
            }
        }

        [DisplayName( "Treatment Start Date" )]
        public string FormatedTreatmentStartDate {
            get {
                if ( TreatmentStartDate?.Date != DateTime.MinValue.Date ) {
                    return TreatmentStartDate?.ToString( "d" );
                }
                else {
                    return "∞";
                }
            }
        }


        [DisplayName( "Treatment End Date" )]
        public string FormattedTreatmentEndDate {
            get {
                if ( TreatmentEndDate?.Date != DateTime.MaxValue.Date ) {
                    return TreatmentEndDate?.ToString( "d" );
                }
                else {
                    return "∞";
                }
            }
        }

        public string CodeAndName {
            get {
                var codeAndName = Code;
                if ( !string.IsNullOrWhiteSpace(Name) ) {
                    codeAndName += " - " + Name;
                }
                return codeAndName;
            }
        }
    }
}
