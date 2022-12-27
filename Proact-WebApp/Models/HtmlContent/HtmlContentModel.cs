using System;
namespace Proact_WebApp.Models {
    public class HtmlContentModel {
        public Guid Id { get; set; }
        public string HtmlContent { get; set; }
        public Guid ProjectId { get; set; }
    }
}

