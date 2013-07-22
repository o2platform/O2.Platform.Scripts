// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using FluentSharp.CoreLib.API;

namespace O2.Scanner.MsCatNet.Scan
{
    public class MsCatNetConfig
    {
    	public static String pathToCatCmdInstallDir = "";
        public static String pathToCatCmdExe = "";
        public static List<string> possibleCatNetExeNames = new List<string> {"CATNetCmd64.exe", "CATNetCmd.exe"};

        public static List<string> possibleCatNetInstallFolder = new List<string>
                                                                     {
                                                                         @"C:\Program Files (x86)\Microsoft\CAT.NET",
                                                                         @"C:\Program Files\Microsoft\CAT.NET"
                                                                     };

        public static Dictionary<string, string> supportLinks = new Dictionary<string, string>
                                                                    {
                                                                        {"Google", "http://www.google.com"},
                                                                        {
                                                                            "Cat.Net Install",
                                                                            "http://www.microsoft.com/downloads/details.aspx?FamilyId=0178e2ef-9da8-445e-9348-c93f24cc9f9d&displaylang=en"
                                                                            },
                                                                        {
                                                                            "Cat.Net Install (x64)",
                                                                            "http://www.microsoft.com/downloads/details.aspx?familyid=E0052BBA-2D50-4214-B65B-37E5EF44F146&displaylang=en"
                                                                            },
                                                                        {"O2 Website", "http://www.o2-OunceOpen.com"}
                                                                    };

        // can't use there here
        /* var Links = new[]
                            {
                                new {name = "el google", url = ""},
                                new {name = "el o2", url = ""},
                                new {name = "el ms", url = "http://www.Microsoft.com"}
                            };*/


        public static bool isCatScannerAvailable()
        {
            if (File.Exists(pathToCatCmdExe))
                return true;
            foreach (string folder in possibleCatNetInstallFolder)
                foreach (string file in possibleCatNetExeNames)
                {
                    pathToCatCmdExe = Path.Combine(folder, file);
                    if (File.Exists(pathToCatCmdExe))
                    {
                    	pathToCatCmdInstallDir = folder;
                        return true; 
                    }
                }
            
            PublicDI.log.error("Could not find MSCatNet Scanner on this box");            
            return false;
        }


/*        internal static void populateControlWithSupportLinks(Control controlToAddSupportLinks,
                                                             ascx_WebAutomation webAutomation)
        {
            foreach (string name in supportLinks.Keys)
            {
                var linkLabel = new LinkLabel {Text = name};
                string nameLocalCopy = name; // so that we deal with the closure issue of the delegate below
                linkLabel.Click += delegate { webAutomation.open(supportLinks[nameLocalCopy]); };
                controlToAddSupportLinks.Controls.Add(linkLabel);
            }
        }*/
    }
}
