using System;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IProjectPropertiesService {

        Task<ProjectPropertiesModel> GetAsync(
            string projectId );

        Task<ProjectPropertiesModel> UpdateAsync(
            ProjectPropertiesUpdateRequest request );

        Task<bool> AssociateLexicon(
            AssociateLexiconToProjectRequest request );
    }
}
