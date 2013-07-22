// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.REPL;

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
