using System;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IProtocolService {
        Task<ProtocolModel> GetProtocolAssignedToProject( string projectId );
        Task<ProtocolModel> GetProtocolAssignedToPatient( string userId );
        Task<bool> CreateProtocolAssignedToProjectAsync( CreateProtocolAssignedToProjectRequest request );
        Task<bool> CreateProtocolAssignedToPatientAsync( CreateProtocolAssignedToPatientRequest request );
    }
}
