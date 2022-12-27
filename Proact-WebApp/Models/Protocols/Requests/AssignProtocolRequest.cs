using System;
namespace Proact_WebApp.Models {

    public enum AssignRequestType {
        AssignToProject,
        AssignToPatient
    }

    public class AssignProtocolRequest {
        public AssignRequestType Type { get; set; }
        public Guid ProtocolId { get; set; }
        public Guid TargetId { get; set; }
    }
}
