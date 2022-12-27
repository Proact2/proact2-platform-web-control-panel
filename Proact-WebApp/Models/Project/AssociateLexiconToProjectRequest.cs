using System;
namespace Proact_WebApp.Models {
    public class AssociateLexiconToProjectRequest {
        public Guid ProjectId { get; set; }
        public Guid LexiconId { get; set; }
    }
}
