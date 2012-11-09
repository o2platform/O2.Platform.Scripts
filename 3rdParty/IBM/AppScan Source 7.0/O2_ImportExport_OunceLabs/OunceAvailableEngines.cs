// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using O2.Kernel;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6_1;

//O2File:O2AssessmentLoad_OunceV6.cs
//O2File:O2AssessmentSave_OunceV6.cs
//O2File:O2AssessmentLoad_OunceV6_1.cs

namespace O2.ImportExport.OunceLabs
{
    public class OunceAvailableEngines
    {
        public static void addAvailableEnginesToControl(Type controlType)           // we need to do this like this since there is no dependencies between "O2 ImportExport OunceLabs" and the "O2 Ascx Views" modules
        {
            if (controlType.Name == "ascx_FindingsViewer")
            {    
                // add load engines
                var loadMethodToInvoke = PublicDI.reflection.getMethod(controlType, "addO2AssessmentLoadEngine_static");
                if (loadMethodToInvoke != null)
                {
                    PublicDI.reflection.invoke(null, loadMethodToInvoke, new object[] {new O2AssessmentLoad_OunceV6()});
                    PublicDI.reflection.invoke(null, loadMethodToInvoke, new object[] { new O2AssessmentLoad_OunceV6_1() });
                }

                // add save engines
                var saveMethodToInvoke = PublicDI.reflection.getMethod(controlType, "addO2AssessmentSaveEngine_static");
                if (saveMethodToInvoke != null)
                {
                    PublicDI.reflection.invoke(null, saveMethodToInvoke, new object[] { new O2AssessmentSave_OunceV6() });                    
                }                
                
            }

        }
    }
}
