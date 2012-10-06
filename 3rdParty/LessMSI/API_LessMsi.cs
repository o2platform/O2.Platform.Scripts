using System;
using System.Threading;
using System.Diagnostics;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
using LessMsi.Msi;
//O2File:LessMsi.cs 
//Installer:LessMsi_Install.cs!LessMsi/lessmsi.exe
//O2Ref:LessMsi/lessmsi.exe

namespace O2.XRules.Database.APIs 
{

	public class LessMsi_Test
	{
		public void launchGui()
		{
			new API_LessMSI().launchGui();
		}
	}
	
	public class API_LessMSI
	{
		public API_LessMSI()
		{			
		}	
		
		public bool extractMsi(string msi, string targetFolder)
		{
			"[extractMsi] extracting {0} into {1}".info(msi, targetFolder);
			try
			{
				var aSync = new AutoResetEvent(false);
				AsyncCallback updateProgress = 
				 	(result)=>{
				 				var extractionProgress = (Wixtracts.ExtractionProgress)result; 				
				 				if (extractionProgress.FilesExtractedSoFar % 10 == 0)
									"[{0} / {1}] {2} : {3}".info(extractionProgress.FilesExtractedSoFar, extractionProgress.TotalFileCount,  extractionProgress.Activity, extractionProgress.CurrentFileName);
								if (extractionProgress.FilesExtractedSoFar == extractionProgress.TotalFileCount)
									aSync.Set();
				 			  };
				
				LessMsi.Msi.Wixtracts.ExtractFiles(msi.fileInfo(), targetFolder.directoryInfo(), null, updateProgress);
				if (aSync.WaitOne(20000).isFalse())
				{
					"MSI Extact took more than 20 seconds".error();
					return false;
				}
				return true;				
			}
			catch(Exception ex)
			{
				ex.log();
				return false;
			}
		}
	}
	
	public static class API_LessMSI_ExtensionMethods
	{
		public static API_LessMSI launchGui(this API_LessMSI lessMSI)
		{
			"lessmsi.exe".assembly_Location().startProcess();
			return lessMSI;
		}
	}
}