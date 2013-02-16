// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.Core.FileViewers.JoinTraces;
using O2.DotNetWrappers.DotNet;

namespace O2.Core.FileViewers.Ascx.tests
{
    public partial class ascx_StrutsMappings_ManualMapping : UserControl
    {
        

        public ascx_StrutsMappings_ManualMapping()
        {
            InitializeComponent();
        }
        
        private void btMapStrutsFindings_Click(object sender, EventArgs e)
        {

//            JoinTraces.JoinOnAttributes.fixSinkVulnNamesBasedOnSinkContextHashMapKey("AAAAA", findingsViewer_SourceFindings.currentO2Findings);
            //           JoinTraces.JoinOnAttributes.fixSourceVulnNamesBasedOnSinkContextHashMapKey("AAAAA", findingsViewer_SourceFindings.currentO2Findings);
            //          return;            

            var strutsMappingObject = strutsMappings.getStrutsMappingObject();
            if (strutsMappingObject != null)
            {

                var createConsolidatedView = cbShowConsolidatedView.Checked;

                var mappedFindings = mapStrutsFindings(strutsMappingObject, findingsViewer_SourceFindings.currentO2Findings, createConsolidatedView);

                if (mappedFindings != null)
                    findingsViewer_MappedFindings.loadO2Findings(mappedFindings, true);
            }

        }        
        

        private void btCreateFindingsFromStrutsMapings_Click(object sender, EventArgs e)
        {
            var strutsMappingObject = strutsMappings.getStrutsMappingObject();
            if (strutsMappingObject != null)            
            {
                var createdFindings = StrutsMappingHelpers.createFindingsFromStrutsMappings(strutsMappingObject);                
                findingsViewer_FromStrutsMappings.setTraceTreeViewVisibleStatus(true);
                findingsViewer_FromStrutsMappings.setFilter2Value("(no filter)");
                findingsViewer_FromStrutsMappings.loadO2Findings(createdFindings,true);
            }
        }
        
    }
}
