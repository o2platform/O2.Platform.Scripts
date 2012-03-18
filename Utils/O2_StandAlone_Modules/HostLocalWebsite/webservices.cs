// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System; 
using System.Diagnostics;
using System.IO;
using O2.Kernel;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Windows;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2Ref:MS_VS_WebDev.WebServer.exe

namespace O2.Tool.HostLocalWebsite.classes
{
    public class WebServices
    { 
        public Process pWebServiceProcess;
        //public static String sExe = @"C:\Program Files\Common Files\Microsoft Shared\DevServer\9.0\WebDev.WebServer.exe";
        public String sExe = "MS_VS_WebDev.WebServer.exe".assembly().Location;
        public String sParamsString = "/port:\"{0}\" /path:\"{1}\" /vpath:\"{2}\"";
        public String sPath = PublicDI.config.O2TempDir;
        public String sPort = (8000 + 2000.random()).str();
        public String sVPath = "/" + Path.GetFileName(PublicDI.config.O2TempDir);

        public String sWebServiceURL = @"http://localhost:{0}{1}";

        public void StartWebService()
        {
        	try
        	{        		
            	pWebServiceProcess = Processes.startProcess(sExe, String.Format(sParamsString, sPort, sPath, sVPath));
            }
            catch(Exception ex)
            {
            	PublicDI.log.error("in StartWebService: {0}", ex.Message);
            }
        }

        public String GetWebServiceURL()
        {
            return String.Format(sWebServiceURL, sPort, sVPath);
        }

        public void StopWebService()
        {
            if (pWebServiceProcess != null && pWebServiceProcess.HasExited == false)
                pWebServiceProcess.Kill();
        }

        public void setExe(String sNewValueFor_Exe)
        {
            if (sNewValueFor_Exe != "")
                sExe = sNewValueFor_Exe;
        }

        public void setPort(String sNewValueFor_Port)
        {
            if (sNewValueFor_Port != "")
                sPort = sNewValueFor_Port;
        }

        public void setPath(String sNewValueFor_Path)
        {
            if (sNewValueFor_Path != "")
                sPath = sNewValueFor_Path;
        }

        public void setVPath(String sNewValueFor_VPath)
        {
            if (sNewValueFor_VPath != "")
                sVPath = sNewValueFor_VPath;
        }
    }
}
