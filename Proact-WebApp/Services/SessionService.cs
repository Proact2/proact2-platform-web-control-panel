using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Proact_WebApp {

    public static class SessionServiceExtensions {
        public static void AddSessionService(
            this IServiceCollection services, IConfiguration configuration ) {
            services.AddHttpClient<ISessionService, SessionService>();
        }
    }

    public class SessionService : BaseService, ISessionService {

        private string _avatarUrlKey = "session_url";
        private string _sessionIsInitKey = "session_is_init";
        private string _roleKey = "session_role";

        public SessionService( ITokenAcquisition tokenAcquisition,
         HttpClient httpClient,
         IConfiguration configuration,
         IHttpContextAccessor contextAccessor )
         : base( tokenAcquisition, httpClient, configuration, contextAccessor ) {
        }

        public void InitSession( ISession session, UserModel userModel ) {
            session.Clear();
            session.SetInt32( _sessionIsInitKey, 1 );

            string avatar = userModel.AvatarUrl == null ? "" : userModel.AvatarUrl;
            session.SetString( _avatarUrlKey, avatar );
            SaveRole( session, userModel );
        }

        public bool IsSessionInitialized( ISession session ) {
            int? isInit = session.GetInt32( _sessionIsInitKey );
            if(isInit != null && isInit == 1 ) {
                return true;
            }
            else {
                return false;
            }
        }

        public string GetAdminAvatarUrl( ISession session ) {
            if ( IsSessionInitialized( session ) ) {
                return session.GetString( _avatarUrlKey );
            }
            else {
                return string.Empty;
            }
        }

        public bool IsSystemAdmin( ISession session ) {
            if ( IsSessionInitialized( session ) ) {
                return session.GetString( _roleKey ) == UserRoles.SystemAdmin;
            }
            else {
                return false;
            }
        }

        public bool IsInstituteAdmin( ISession session ) {
            if ( IsSessionInitialized( session ) ) {
                return session.GetString( _roleKey ) == UserRoles.InstituteAdmin;
            }
            else {
                return false;
            }
        }

        public bool IsMedicalTeamAdmin( ISession session ) {
            if ( IsSessionInitialized( session ) ) {
                return session.GetString( _roleKey ) == UserRoles.MedicalTeamAdmin;
            }
            else {
                return false;
            }
        }

        public bool IsMedicalProfessional( ISession session ) {
            if ( IsSessionInitialized( session ) ) {
                return session.GetString( _roleKey ) == UserRoles.MedicalProfessional;
            }
            else {
                return false;
            }
        }

        private void SaveRole( ISession session, UserModel userModel ) {
            if ( userModel.IsSystemAdmin() ) {
                session.SetString( _roleKey, UserRoles.SystemAdmin );
            }
            else if ( userModel.IsInstituteAdmin() ) {
                session.SetString( _roleKey, UserRoles.InstituteAdmin );
            }
            else if ( userModel.IsMedicalTeamAdmin() ) {
                session.SetString( _roleKey, UserRoles.MedicalTeamAdmin );
            }
            else if ( userModel.IsMedicalProfessional() ) {
                session.SetString( _roleKey, UserRoles.MedicalProfessional );
            }
            else if ( userModel.IsResearcher() ) {
                session.SetString( _roleKey, UserRoles.Researcher );
            }
        }

      
    }
}
