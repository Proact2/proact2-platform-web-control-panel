using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Proact_WebApp.Models;

namespace Proact_WebApp.Controllers {

    [AuthorizeForScopes( ScopeKeySection = "ProactWebAppScope" )]
    public class MedicalTeamInfoController : BaseController {

        private IMedicalTeamsService _medicalTeamService;

        public MedicalTeamInfoController( IMedicalTeamsService medicalTeamsService ) {
            _medicalTeamService = medicalTeamsService;
        }

        public async Task<ActionResult> Index( string medicalTeamId ) {

            var medicalTeams = await _medicalTeamService
                .GetMedicalTeamsWhereCurrentUserIsAdmin();

            if ( medicalTeams == null || medicalTeams.Count == 0 ) {
                return RedirectToAction( nameof( Empty ) );
            }

            Guid selectedMedicalTeamId;
            if ( medicalTeamId != null ) {
                selectedMedicalTeamId = new Guid( medicalTeamId );
            }
            else {
                selectedMedicalTeamId = medicalTeams[0].MedicalTeamId;
            }

            ViewBag.medicalTeams = medicalTeams;
            ViewBag.medicalTeamId = selectedMedicalTeamId.ToString();

            var selectedMedicalTeam = medicalTeams
                .Find( s => s.MedicalTeamId == selectedMedicalTeamId );

            var admins = await _medicalTeamService.GetAdminsAsync(
              selectedMedicalTeam.Project.ProjectId.ToString(),
              selectedMedicalTeam.MedicalTeamId.ToString() );

            var model = new MedicalTeamDetailsModel() {
                MedicalTeam = selectedMedicalTeam,
                Admins = admins.ToList()
            };

            return View( model );
        }

        public ActionResult Empty() {
            return View();
        }
    }
}
