using System;
using System.Collections.Generic;

namespace Proact_WebApp.Models {
    public class MedicalTeamIndexModel {
        public IEnumerable<ProjectModel> Projects { get; set; }
        public IEnumerable<MedicalTeamModel> MedicalTeams { get; set; }
    }
}
