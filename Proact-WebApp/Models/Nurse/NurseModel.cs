using System;
using System.ComponentModel;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public class NurseModel : UserModel {

        [DisplayName( "Medical Team" )]
        public MedicalTeamModel MedicalTeam { get; set; }

        public bool IsAssociatedToMedicalTeam {
            get {
                return MedicalTeam != null;
            }
        }

    }
}
