using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp {
    public class RequiredUpdateModel {
        [Required]
        [DisplayName( "Android required build" )]
        public int AndroidLastBuildRequired { get; set; }
        [Required]
        [DisplayName( "iOS required build" )]
        public int IOSLastBuildRequired { get; set; }
        [Required]
        [DisplayName( "Google Play Store URL" )]
        public string AndroidStoreUrl { get; set; }
        [Required]
        [DisplayName( "Apple App Store URL" )]
        public string IosStoreUrl { get; set; }
    }
}
