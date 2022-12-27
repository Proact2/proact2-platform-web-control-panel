using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IMedicsService {

        Task<List<MedicModel>> GetAllMedics();

        Task<List<MedicModel>> GetMedicsUnassigned();

        Task<List<MedicModel>> GetMedicsAssignedToMedicalTeam( string medicalTeamID );

        Task<MedicModel> CreateMedic( UserCreateRequest request );

        Task<MedicModel> AssignMedicToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request );

        Task<bool> RemoveMedicFromMedicalTeam(
            string medicalTeamId, string userId );

        Task<MedicModel> CreateMedicWithinMedicalTeamAsync(
            string medicalTeamId, AssignUserToMedicalTeamRequest request );
    }
}
