using System;
using System.ComponentModel;

namespace Proact_WebApp.Models {
    public class DocumentModel {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [DisplayName("Document")]
        public object Url { get; set; }
    }
}
