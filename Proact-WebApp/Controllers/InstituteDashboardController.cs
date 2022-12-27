using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class InstituteDashboardController : BaseController {

        private InstituteDashboardModel _dashboardModel;

        private InstituteModel _instituteModel;
        private List<ProjectModel> _projectsModel;

        private IInstituteService _instituteService;
        private IProjectService _projectService;
        private IMedicsService _medicsService;
        private IMedicalTeamsService _medicalTeamsService;
        private ILexiconService _lexiconService;

        public InstituteDashboardController(
            IInstituteService instituteService,
            IProjectService projectService,
            IMedicsService medicsService,
            IMedicalTeamsService medicalTeamsService,
            ILexiconService lexiconService ) {
            _instituteService = instituteService;
            _projectService = projectService;
            _medicsService = medicsService;
            _medicalTeamsService = medicalTeamsService;
            _lexiconService = lexiconService;
        }

        public async Task<IActionResult> Index() {
            await CheckServices();
            return View( _dashboardModel );
        }

        private async Task CheckServices() {
            _dashboardModel = new InstituteDashboardModel();
            _dashboardModel.ServicesConfigurationStatus
                = new List<ConfigurationModel>();

            var isTermsSetted
                = await IsInstituteTermsAndCOnditionsSetted();
            var isPrivacySetted
                = await IsInstitutePrivacyPolicySetted();
            var isProjectSetted
                = await IsProjectSetted();
            var isTeamsAdminSetted
                = await IsMedicalTeamAdminSetted();
            var isMedicalTeamSetted
                = await IsMedicalTeamSetted();
            var isLexiconSetted
                = await IsLexiconSetted();

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert Terms & Conditions",
                Controller = "TermsAndConditions",
                Action = "Index",
                Completed = isTermsSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Insert Privacy policy",
                Controller = "PrivacyPolicy",
                Action = "Index",
                Completed = isPrivacySetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Create a Study",
                Controller = "Projects",
                Action = "Create",
                Completed = isProjectSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Create a Medical team Admin",
                Controller = "Medics",
                Action = "Create",
                Completed = isTeamsAdminSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Create a Medical team",
                Controller = "MedicalTeams",
                Action = "Create",
                Completed = isMedicalTeamSetted
            } );

            _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                Title = "Create a Lexicon",
                Controller = "Lexicons",
                Action = "Index",
                Completed = isLexiconSetted
            } );

            await CheckProjectsWithLexicon();
        }

        private async Task<bool> IsInstitutePrivacyPolicySetted() {
            if(_instituteModel == null ) {
                _instituteModel = await _instituteService
                    .GetInstituteWhereCurrentUserIsAdmin();
            }
           
            if( _instituteModel.Properties != null
                && _instituteModel.Properties.PrivacyPolicy != null ) {
                return true;
            }
            return false;
        }

        private async Task<bool> IsInstituteTermsAndCOnditionsSetted() {
            if ( _instituteModel == null ) {
                _instituteModel = await _instituteService
                    .GetInstituteWhereCurrentUserIsAdmin();
            }

            if ( _instituteModel.Properties != null
                && _instituteModel.Properties.TermsAndConditions != null ) {
                return true;
            }
            return false;
        }

        private async Task<bool> IsProjectSetted() {
            _projectsModel = await _projectService.GetAllAsync();
            var count = _projectsModel.Count;
            _dashboardModel.ProjectsCount = count;

            return count != 0;
        }

        private async Task<bool> IsMedicalTeamAdminSetted() {
            var medics = await _medicsService.GetAllMedics();
            var count = medics.Count;
            _dashboardModel.MedicsCount = count;
            
            return count != 0;
        }

        private async Task<bool> IsLexiconSetted() {
            var lexicons = await _lexiconService.GetAsync();
            return lexicons.Count != 0;
        }

        private async Task<bool> IsMedicalTeamSetted() {
            var count = 0;

            if (_projectsModel == null ) {
                _projectsModel = await _projectService.GetAllAsync();
            }

            foreach( var project in _projectsModel ) {
                var teams = await _medicalTeamsService
                    .GetAllAsync( project.ProjectId.ToString() );

                count += teams.ToList().Count;
            }

            _dashboardModel.MedicalTeamsCount = count;

            return count > 0;
        }

        private async Task CheckProjectsWithLexicon() { 
            if ( _projectsModel == null ) {
                _projectsModel = await _projectService.GetAllAsync();
            }

            var projectWithAnalystConsole = _projectsModel
                .Where( x => x.Properties.IsAnalystConsoleActive );

            foreach (var project in projectWithAnalystConsole ) {
                _dashboardModel.ServicesConfigurationStatus.Add( new ConfigurationModel() {
                    Index = _dashboardModel.ServicesConfigurationStatus.Count + 1,
                    Title = $"Add a Lexicon to the Study \"{project.Name}\"",
                    Controller = "Projects",
                    Action = "associateLexicon",
                    RouteId = project.ProjectId.ToString(),
                    Completed = project.Properties.Lexicon != null
                } );
            }
        }
    }


}
