using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {
    public class MedicalTeamDetailsModel {

        public MedicalTeamModel MedicalTeam { get; set; }
        public List<UserModel> Admins { get; set; }
    }
}
