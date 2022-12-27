using System;
using System.Collections.Generic;
using System.Linq;

namespace Proact_WebApp {
    public class MedicalTeamAdminDashboardModel {
        public List<ConfigurationModel> ServicesConfigurationStatus { get; set; }

        public int PatientsCount { get; set; }
        public int MedicsCount { get; set; }
        public int NursesCount { get; set; }

        public int CompletedServicePercentage {
            get {
                if ( ServicesConfigurationStatus == null ) {
                    return 0;
                }

                var totalService = ServicesConfigurationStatus.Count;
                var completedService = ServicesConfigurationStatus
                    .Where( x => x.Completed )
                    .ToList()
                    .Count;

                if ( totalService == 0 ) {
                    return 0;
                }
                else {
                    return ( completedService * 100 ) / totalService;
                }
            }
        }
    }
}
