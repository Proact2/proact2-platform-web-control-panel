using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Proact_WebApp.Models;

namespace Proact_WebApp {
    public interface ILexiconService {
        Task<List<LexiconModel>> GetAsync();
        Task<LexiconModel> GetAsync( string id );
        Task<List<LexiconModel>> GetPublishedAsync();
        Task<LexiconModel> CreateAsync( LexiconCreateRequest request );
        Task<bool> Delete( string id );
        Task<bool> Validate( LexiconPublishRequest request );
    }

}
