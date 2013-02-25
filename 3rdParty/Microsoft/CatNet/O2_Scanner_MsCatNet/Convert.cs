// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.DotNetWrappers.DotNet;
using O2.Scanner.MsCatNet.RnD.Xsd;
using O2.Kernel;

//O2File:MsCatNet_Report.cs

namespace O2.Scanner.MsCatNet.Utils
{
    public class Convert
    {
        public static String sConvertMsCatNetResultsFileIntoOzasmt(String sCatNetFileToConvert)
        {
            var rReport = new Report();
            Object oSerializedObject = null;
            try
            {
                oSerializedObject = Serialize.getDeSerializedObjectFromXmlFile(sCatNetFileToConvert, rReport.GetType());
                rReport = (Report) oSerializedObject;


                //o2.Scanners.MSCatNet.xsd.Report
            }
            catch
            {
                try
                {
                    String sTempFile = PublicDI.config.TempFileNameInTempDirectory;

                    Serialize.createSerializedXmlFileFromObject(oSerializedObject, sTempFile);

                    rReport = (Report) Serialize.getDeSerializedObjectFromXmlFile(sTempFile, typeof (Report));
                }
                catch (Exception ex)
                {
                    PublicDI.log.error("In sConvertMsCatNetResultsFileIntoOzasmt: {0}", ex.Message);
                }
            }
            return "";
        }
    }
}
