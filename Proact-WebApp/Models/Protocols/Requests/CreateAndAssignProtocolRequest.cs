using System;
namespace Proact_WebApp.Models {
    public class CreateAndAssignProtocolRequest {
        public CreateProtocolRequest CreateRequest { get; set; }
        public AssignProtocolRequest AssignRequest { get; set; }
    }
}