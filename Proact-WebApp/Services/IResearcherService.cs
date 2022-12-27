using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IResearcherService {

        Task<List<ResearcherModel>> GetAll();

        Task<List<ResearcherModel>> GetUnassigned();

        Task<List<ResearcherModel>> GetAssignedToMedicalTeam( string medicalTeamId );

        Task<ResearcherModel> Create( UserCreateRequest request );

        Task<ResearcherModel> AssignToMedicalTeam(
            AssignUserToMedicalTeamRequest request );

        Task<bool> RemoveFromMedicalTeam(
            string medicalTeamId, string userId );
    }
}
