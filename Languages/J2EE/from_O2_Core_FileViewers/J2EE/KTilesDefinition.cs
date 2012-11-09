// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.J2EE
{
    [Serializable]
    public class KTilesDefinitions : ITilesDefinitions
    {
        public List<ITilesDefinition> definitions { get; set;} 

        public KTilesDefinitions()
        {
            definitions = new List<ITilesDefinition>();
        }
    }

    [Serializable]
    public class KTilesDefinition : ITilesDefinition
    {
        public string name { get; set; }
        public string path { get; set; }
        public string page { get; set; }
        public string extends { get; set; }
        public Dictionary<string, string> puts { get; set; }

        public KTilesDefinition()
        {
            puts = new Dictionary<string, string>();
        }
        
    }
}
