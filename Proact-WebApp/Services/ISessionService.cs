using System;
using Microsoft.AspNetCore.Http;

namespace Proact_WebApp {
    public interface ISessionService {
        void InitSession( ISession session, UserModel userModel );
        bool IsSessionInitialized( ISession session );
        bool IsInstituteAdmin( ISession session );
        bool IsSystemAdmin( ISession session );
        bool IsMedicalTeamAdmin( ISession session );
        bool IsMedicalProfessional( ISession session );
        string GetAdminAvatarUrl( ISession session );
    }
}
