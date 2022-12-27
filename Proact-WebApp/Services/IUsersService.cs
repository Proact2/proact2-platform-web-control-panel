using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IUsersService {
        Task<IEnumerable<UserModel>> GetAsync();
        Task<UserModel> CreateAsync( UserCreateRequest request );
        Task<bool> AssignRole( AssignRoleToUserRequest request );
        Task<bool> DeleteUser( string userId );
        Task<UserModel> GetCurrentUser();
        Task<bool> SuspendUser( string userId );
        Task<bool> ActivateUser( string userId );
        Task<bool> DeactivateUser( string userId );
    }
}
