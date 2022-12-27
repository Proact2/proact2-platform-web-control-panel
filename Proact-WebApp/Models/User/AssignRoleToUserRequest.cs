using System;
namespace Proact_WebApp {
    public class AssignRoleToUserRequest {
        public Guid UserId { get; set; }
        public string Role { get; set; }
    }
}
