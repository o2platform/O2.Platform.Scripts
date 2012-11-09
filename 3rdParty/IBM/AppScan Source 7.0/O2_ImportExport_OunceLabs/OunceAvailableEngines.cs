// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using O2.Kernel;
using O2.XRules.ThirdPary.IBM;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6_1;
using O2.Views.ASCX.O2Findings;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:O2AssessmentLoad_OunceV6.cs
//O2File:O2AssessmentSave_OunceV6.cs
//O2File:O2AssessmentLoad_OunceV6_1.cs
//O2File:O2AssessmentLoad_OunceV7_0.cs

namespace O2.ImportExport.OunceLabs
{
    public class OunceAvailableEngines
    {    	
 	    public static void addAvailableEnginesToControl(Type targetControl)      //legacy way to call it       
        {
        	add_AvailableEngines();
        }
        
        public static void add_AvailableEngines()           
        {
            // add load engines
            ascx_FindingsViewer.addO2AssessmentLoadEngine_static( new O2AssessmentLoad_OunceV6());
            ascx_FindingsViewer.addO2AssessmentLoadEngine_static( new O2AssessmentLoad_OunceV6_1());
            ascx_FindingsViewer.addO2AssessmentLoadEngine_static( new O2AssessmentLoad_OunceV7_0());


			// add save engines
            ascx_FindingsViewer.addO2AssessmentSaveEngine_static( new O2AssessmentSave_OunceV6());            
        }
    }
    public static class OunceAvailableEngines_ExtensionMethods
    {
    	public static ascx_FindingsViewer add_AvailableEngines_Ounce(this ascx_FindingsViewer findingsViewer)
    	{    		
    		OunceAvailableEngines.add_AvailableEngines();
    		return findingsViewer;
    	}    	
    }
}
