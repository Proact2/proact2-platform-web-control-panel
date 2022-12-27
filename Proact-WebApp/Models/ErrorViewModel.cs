using System;
using System.Net;

namespace Proact_WebApp.Models {
    public class ErrorViewModel {
        public string RequestId { get; set; }
        public HttpStatusCode? StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty( RequestId );
    }
}
