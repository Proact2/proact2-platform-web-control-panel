using System;
using System.Collections.Generic;

namespace Proact_WebApp {
    public class LexiconCreateRequest {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<LexiconCreateCategoryRequest> Categories { get; set; }
    }

    public class LexiconCreateCategoryRequest {
        public string Name { get; set; }
        public bool MultipleSelection { get; set; }
        public List<LexiconCreateLabelRequest> Labels { get; set; }
    }

    public class LexiconCreateLabelRequest {
        public string Label { get; set; }
        public string GroupName { get; set; }
    }
}
