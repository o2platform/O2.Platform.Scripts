// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;

namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6
{
    public class O2TraceBlock_OunceV6
    {
        public Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6> dGluedSinks =
            new Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6>();

        // on this one we will NOT check for compatible nodes (use when Gluing traces

        public Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6> dGluedSources =
            new Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6>();

        public Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6> dSinks =
            new Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6>();

        // on these we will check for compatible nodes

        public Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6> dSources =
            new Dictionary<AssessmentAssessmentFileFinding, O2AssessmentData_OunceV6>();

        public String sFile = "";
        public String sLineNumber = "";
        public String sSignature = "";
        public String sTraceRootText = "";
        public String sUniqueName = "";
    }
}
