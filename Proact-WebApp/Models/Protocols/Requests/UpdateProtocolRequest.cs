using System;
namespace Proact_WebApp.Models {
    public class UpdateProtocolRequest : CreateProtocolRequest  {
        public Guid Id { get; set; } 
        public Guid TargetId { get; set; }
    }
}
