using System;
using System.ComponentModel;
using System.Linq;
using Proact_WebApp.Models;

namespace Proact_WebApp {

    public enum UserSubscriptionState {
        Active,
        Suspended,
        Deactivated
    }

    public class UserModel {
        [DisplayName( "User ID" )]
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string Title { get; set; }

        [DisplayName( "Azure ID" )]
        public string AccountId { get; set; }
        public UserSubscriptionState State { get; set; }
        public string[] Roles { get; set; }

        public Guid InstituteId { get; set; }

        public bool IsSystemAdmin() {
            return Roles.ToList().Contains( UserRoles.SystemAdmin );
        }

        public bool IsInstituteAdmin() {
            return Roles.ToList().Contains( UserRoles.InstituteAdmin );
        }

        public bool IsMedicalTeamAdmin() {
            return Roles.ToList().Contains( UserRoles.MedicalTeamAdmin );
        }

        public bool IsMedicalProfessional() {
            return Roles.ToList().Contains( UserRoles.MedicalProfessional );
        }

        public bool IsResearcher() {
            return Roles.ToList().Contains( UserRoles.Researcher );
        }

        public bool IsAnAdmin() {
            return IsMedicalTeamAdmin() || IsSystemAdmin();
        }
    }
}
