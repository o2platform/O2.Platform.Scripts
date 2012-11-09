// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using O2.Core.FileViewers.JoinTraces;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.Interfaces.O2Findings;
using O2.Views.ASCX.O2Findings;

namespace O2.Core.FileViewers.Ascx.O2Rules
{
    partial class ascx_O2Rules_Struts
    {
        private bool runOnLoad = true;

        private void onLoad()
        {
            if (DesignMode == false && runOnLoad)
            {                
            }
            
        }
        
        public void loadO2MappingsFile(string fileToLoad)
        {
            strutsMappingsControl.loadO2StrutsMappingFile(fileToLoad);
        }

        public Thread loadBaseO2Findings(string fileToLoad)
        {
            return findingsViewer_BaseFindings.loadO2Assessment(fileToLoad);
        }

        public void loadBaseO2Findings(List<IO2Finding> baseFindingsToLoad)
        {
            findingsViewer_BaseFindings.loadO2Findings(baseFindingsToLoad);
        }        

        public List<IO2Finding> getBaseO2Findings()
        {            
            return findingsViewer_BaseFindings.currentO2Findings;
        }

        /*private void calculateFinalResults(string taintSources_SourceRegEx, string taintSources_SinkRegEx, string finalSinks_SourceRegEx, string finalSinks_SinkRegEx)
        {
            runFilterFor_TaintSources(taintSources_SourceRegEx, taintSources_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings, O2FindingsHelpers.mapJoinPoints_HashTagsOn_Sinks);
            runFilterFor_FinalSinks(finalSinks_SourceRegEx, finalSinks_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings, O2FindingsHelpers.mapJoinPoints_HashTagsOn_Sources);

            createFindingsFromStrutsMappings();

            //calculateResults();
            
        }*/

    }
}
