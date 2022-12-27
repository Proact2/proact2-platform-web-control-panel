using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface INursesService {

        Task<List<NurseModel>> GetAllNurse();

        Task<List<NurseModel>> GetNursesUnassigned();

        Task<List<NurseModel>> GetNurseAssignedToMedicalTeam( string medicalTeamID );

        Task<NurseModel> CreateNurse( UserCreateRequest request );

        Task<NurseModel> AssignNurseToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request );

        Task<bool> RemoveNurseFromMedicalTeam(
            string medicalTeamId, string userId );
    }
}
