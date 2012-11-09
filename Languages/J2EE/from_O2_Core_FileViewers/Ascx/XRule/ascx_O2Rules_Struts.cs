// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:ascx_O2Rules_Struts.Controllers.cs
//O2File:ascx_O2Rules_Struts.Designer.cs
//O2File:ascx_FilteredFindings.cs
//O2File:ascx_StrutsMappings.cs

namespace O2.Core.FileViewers.Ascx.O2Rules
{
	public class ascx_O2Rules_Struts_Test
	{
		public void launch()
		{
			"ascx_O2Rules_Struts".popupWindow<ascx_O2Rules_Struts>(1300,800)
								 .insert_LogViewer();
		}
	}

    public partial class ascx_O2Rules_Struts : UserControl
    {
        public ascx_O2Rules_Struts()
        {
            InitializeComponent();
        }

        private void llReCalculateResults_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //calculateResults();
        }

        private void llCreateFindingsFromStrutsMappings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //createFindingsFromStrutsMappings();
        }

        private void llCalculateFinalFindings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*var taintSources_SourceRegEx = @"getParameter\(java.lang.String\)";
            var taintSources_SinkRegEx = @"setAttribute\(java.lang.String";

            var finalSinks_SourceRegEx = @"getAttribute\(java.lang.String\)";
            var finalSinks_SinkRegEx = @"print";
            calculateFinalResults(taintSources_SourceRegEx, taintSources_SinkRegEx, finalSinks_SourceRegEx,
                                  finalSinks_SinkRegEx);*/
        }

        private void ascx_O2Rules_Struts_Load(object sender, EventArgs e)
        {
            onLoad();
        }                       


    }
}
