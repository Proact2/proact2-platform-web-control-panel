using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class GeneralityModel {

        public string _phone;
        public string Phone {
            get {
                return _phone;
            }
            set {

                if ( value == null ) {
                    _phone = "";
                }
                else {
                    _phone = value;
                }
            }
        }

        [Required]
        [DisplayName( "Address Line 1" )]
        public string AddressLine1 { get; set; }

        [DisplayName( "Address Line 2" )]
        public string AddressLine2 { get; set; }

        [Required]
        public string City { get; set; }

        [DisplayName( "State or Province" )]
        public string StateOrProvince { get; set; }

        [DisplayName( "Region code" )]
        public string RegionCode { get; set; }

        [Required]
        [DisplayName( "Postal code" )]
        public string PostalCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [DisplayName( "Time zone" )]
        public string TimeZone { get; set; }
    }
}
