using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IPatientsService {

        Task<List<PatientModel>> GetAllPatients();

        Task<List<PatientModel>> GetUnassignedPatients();

        Task<List<PatientModel>> GetPatientsAssignedToMedicalTeam(
            string medicalTeamID );

        Task<List<PatientModel>> GetPatientsAssignedToProject(
          string projectId );

        Task<PatientModel> CreatePatient(
            PatientCreateRequest request );

        Task<PatientModel> AssignPatientToMedicalTeam(
            string medicalTeamId, AssignUserToMedicalTeamRequest request );

        Task<bool> RemovePatientFromMedicalTeam(
            string medicalTeamId, string userId );
    }
}
