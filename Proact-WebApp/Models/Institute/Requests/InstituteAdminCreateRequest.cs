using System;
namespace Proact_WebApp.Models {
    public class InstituteAdminCreateRequest {
        public Guid InstituteId { get; set; }
        public UserCreateRequest User { get; set; }
    }
}
