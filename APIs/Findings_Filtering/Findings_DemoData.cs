// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.Interfaces.O2Core;
using O2.Interfaces.O2Findings;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.Views.ASCX.O2Findings;

//O2File:Findings_ExtensionMethods.cs


namespace O2.XRules.Database.Findings
{

	public static class Findings_DemoData
	{
		public static string tempFindingsFolder = "".tempDir().pathCombine("_demoFindings");
		
		
		public static List<IO2Finding> hacmeBank_AllFindings()
		{
			try
			{
				tempFindingsFolder.createDir();
				var findingsFile = tempFindingsFolder.pathCombine("HacmeBank_COMPLETE_TRACES.ozasmt");
				if (findingsFile.fileExists().isFalse())			
				{						
					var tempFolder = "HacmeBank_COMPLETE_TRACES.zip".local().unzip_File();
					var tempUnzipedFile = tempFolder.files()[0];				
					Files.moveFile(tempUnzipedFile, findingsFile);
					Files.deleteFolder(tempFolder);
				}
				return findingsFile.loadFindingsFile();
			}
			catch(Exception ex)
			{
				ex.log("in Findings_DemoData.hacmeBank_AllFindings");
				return new List<IO2Finding>();
			}
			
			//var testFile = "HacmeBank_COMPLETE_TRACES.zip".local();
			//var tempFile = testFile.unzip_FileAndReturtListOfUnzipedFiles()[0];
			//var o2Findings = tempFile.loadFindingsFile();
			//return 
		}
	}
}
