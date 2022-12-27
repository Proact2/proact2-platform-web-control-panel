using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IProjectService {
        Task<List<ProjectModel>> GetAllAsync();
        Task<List<ProjectModel>> GetOpenProjectAsync();
        Task<List<ProjectModel>> GetClosedProjectAsync();
        Task<ProjectModel> GetAsync( string id );
        Task<ProjectModel> CreateAsync( ProjectCreateRequest request );
        Task<bool> DeleteAsync( Guid projectId );
        Task<ProjectModel> UpdateAsync( ProjectUpdateRequest request );
        Task<bool> CloseAsync( string projectId );
        Task<bool> OpenAsync( string projectId );
    }
}
