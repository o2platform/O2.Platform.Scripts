// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;


namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6
{
    [Serializable]
    public class O2AssessmentData_OunceV6
    {
        public AssessmentRun arAssessmentRun { get; set; }
        public Dictionary<UInt32, List<AssessmentAssessmentFileFinding>> dActionObjects { get; set; }
        public Dictionary<AssessmentAssessmentFile, List<AssessmentAssessmentFileFinding>> dAssessmentFiles { get; set; }
        public Dictionary<AssessmentAssessmentFileFinding, AssessmentAssessmentFile> dFindings { get; set; }

        public Dictionary<AssessmentAssessmentFileFinding, List<CallInvocation>> dFindings_CallInvocation { get; set; }
        // contains list of methods called (i.e. invoked) by this finding                

        public Dictionary<string, List<AssessmentAssessmentFileFinding>> dVulnerabilityType { get; set; }

        public List<AssessmentAssessmentFileFinding> lfAllFindingsThatMatchCriteria { get; set; }
        public String sDb_id { get; set; }

        public override string ToString()
        {
            if (arAssessmentRun != null && arAssessmentRun.Assessment != null)
            {
                string sToString = Path.GetFileName(arAssessmentRun.Assessment.owner_name);
                foreach (Assessment aAssessment in arAssessmentRun.Assessment.Assessment)
                    sToString += "  :  " + Path.GetFileName(aAssessment.owner_name) + " (" + dFindings.Keys.Count + "," +
                                 dActionObjects.Count + ")";
                return sToString;
            }

            return "Error: arAssessmentRun object is null";
        }
    }
}
