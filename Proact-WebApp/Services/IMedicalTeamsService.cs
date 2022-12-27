using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IMedicalTeamsService {

        Task<IEnumerable<MedicalTeamModel>> GetAllAsync( string projectId );
        Task<IEnumerable<MedicalTeamModel>> GetOpenMedicalTeamsAsync( string projectId );
        Task<IEnumerable<MedicalTeamModel>> GetClosedMedicalTeamsAsync( string projectId );

        Task<List<MedicalTeamModel>> GetMedicalTeamsWhereCurrentUserIsAdmin();

        Task<MedicalTeamModel> CreateAsync(
            string projectId,
            MedicalTeamCreateRequest request );

        Task<IEnumerable<UserModel>> AssignMedicalTeamAdministrator(
            string projectId,
            string medicalTeamId,
            MedicalTeamAssignAdminRequest request );

        Task<bool> DeleteAsync(
            string projectId,
            string medicalTeamId );

        Task<MedicalTeamModel> DetailsAsync(
            string projectId,
            string medicalTeamId );

        Task<IEnumerable<UserModel>> GetAdminsAsync(
            string projectId,
            string medicalTeamId );

        Task<MedicalTeamModel> UpdateAsync(
           MedicalTeamEditRequest request );

        Task<bool> CloseAsync(
         string projectId,
         string medicalTeamId );


        Task<bool> OpenAsync(
         string projectId,
         string medicalTeamId );

    }
}
