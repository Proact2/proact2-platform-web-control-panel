using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class PatientCreateRequest : UserCreateRequest {

        [MaxLength( 100 )]
        [DisplayName( "First name" )]
        public new string FirstName { get; set; }

        [MaxLength( 100 )]
        [DisplayName( "Last name" )]
        public new string LastName { get; set; }

        public DateTime? Birthday { get; set; }

        public int BirthYear {
            get {
                return !Birthday.HasValue ? 0 : Birthday.Value.Year;
            }
        }

        public string Gender { get; set; }
        public string UserId { get; set; }
    }
}
