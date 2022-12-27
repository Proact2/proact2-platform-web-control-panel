using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface IInstituteService {
        Task<List<InstituteModel>> GetAllAsync();
        Task<List<InstituteModel>> GetOpenAsync();
        Task<List<InstituteModel>> GetClosedAsync();
        Task<InstituteModel> GetAsync( string id );
        Task<InstituteModel> GetInstituteWhereCurrentUserIsAdmin();
        Task<InstituteModel> CreateAsync( CreateInstituteRequest request );
        Task<InstituteModel> UpdateAsync( UpdateInstituteRequest request );
        Task<bool> CloseAsync( string id );
        Task<bool> OpenAsync( string id );
        Task<bool> CreateAdmin( InstituteAdminCreateRequest request );
        Task<bool> AddTermsAndConditions( CreateInstituteDocumentRequest request );
        Task<bool> AddPrivacyPolicy( CreateInstituteDocumentRequest request );
        Task<bool> UpdateTermsAndConditions( CreateInstituteDocumentRequest request );
        Task<bool> UpdatePrivacyPolicy( CreateInstituteDocumentRequest request );
    }
}
