using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp {
    public class AssignPatientToMedicalTeamRequest : AssignUserToMedicalTeamRequest {

        [Required]
        [MaxLength( 12 )]
        [DisplayName( "Patient code" )]
        public string Code { get; set; }

        [DisplayName( "Treatment Start Date" )]
        public DateTime? NullableTreatmentStartDate { get; set; }

        public DateTime TreatmentStartDate {
            get {
                if ( NullableTreatmentStartDate.HasValue ) {
                    return NullableTreatmentStartDate.Value;
                }
                else {
                    return DateTime.MinValue;
                }
            }
        }

        [DisplayName( "Treatment End Date" )]
        public DateTime? NullableTreatmentEndDate { get; set; }

        public DateTime TreatmentEndDate {

            get {
                if ( NullableTreatmentEndDate.HasValue ) {
                    return NullableTreatmentEndDate.Value;
                }
                else {
                    return DateTime.MaxValue;
                }
            }
        }
    }
}
