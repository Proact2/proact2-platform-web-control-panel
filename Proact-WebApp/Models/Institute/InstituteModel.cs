using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {

    public enum InstituteState {
        Open,
        Closed,
    }

    public class InstituteModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public InstitutePropertiesModel Properties { get; set; }
        public InstituteState State { get; set; }
        public List<UserModel> Admins { get; set; }
    }
}

