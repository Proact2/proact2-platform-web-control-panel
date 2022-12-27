using System;
using System.Collections.Generic;
using System.ComponentModel;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public class MedicModel : UserModel {

        [DisplayName("Medical Teams")]
        public List<MedicalTeamModel> MedicalTeams { get; set; }

        public bool IsAssociatedToMedicalTeam {
            get => MedicalTeams != null && MedicalTeams.Count > 0;
        }

    }
}
