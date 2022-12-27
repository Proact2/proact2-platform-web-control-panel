using System;
using System.Threading.Tasks;

namespace Proact_WebApp {
    public interface IProactMobileSettingsService {
        public Task<bool> SetLastAppVersion( RequiredUpdateModel request );
        public Task<RequiredUpdateModel> GetLastAppVersion();
    }
}
