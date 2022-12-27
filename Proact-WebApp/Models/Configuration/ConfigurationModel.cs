using System;
namespace Proact_WebApp {
    public class ConfigurationModel {
        public int Index { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string RouteId { get; set; }
    }
}
