using System;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IContactService {
        public Task<HtmlContentModel> GetContact( string projectId );
        public Task<bool> CreateContact( HtmlContentRequest request );
    }
}
