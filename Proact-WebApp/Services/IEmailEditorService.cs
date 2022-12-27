using System;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IEmailEditorService {
        public Task<HtmlContentModel> GetWelcomeEmailBody( string projectId );
        public Task<HtmlContentModel> GetUserDeactivatedEmailBody( string projectId );
        public Task<HtmlContentModel> GetUserSuspendedEmailBody( string projectId );
        public Task<bool> SetWelcomeEmailBody( HtmlContentRequest request );
        public Task<bool> SetUserDeactivatedEmailBody( HtmlContentRequest request );
        public Task<bool> SetUserSuspendedEmailBody( HtmlContentRequest request );
    }
}
