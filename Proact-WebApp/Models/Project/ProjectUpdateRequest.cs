using System;
using System.ComponentModel.DataAnnotations;

namespace Proact_WebApp.Models {
    public class ProjectUpdateRequest : ProjectCreateRequest {

        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public ProjectState Status { get; set; }


        public bool IsStatusOpen {
            set {
                if ( value ) {
                    Status = ProjectState.Open;
                }
                else {
                    Status = ProjectState.Closed;
                }
            }

            get {
                return Status == ProjectState.Open;
            }
        }
    }
}
