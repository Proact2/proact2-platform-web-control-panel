using System;
using System.ComponentModel;

namespace Proact_WebApp.Models {
    public class ProtocolModel {
        public Guid Id { get; set; }
        [DisplayName("Protocol name")]
        public string Name { get; set; }
        [DisplayName("Internal code")]
        public string InternalCode { get; set; }
        [DisplayName("International code")]
        public string InternationalCode { get; set; }
        [DisplayName("Protocol file")]
        public string Url { get; set; }

        public bool IsEmpty {
            get => Id == Guid.Empty;
        }
    }
}
