// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Zip;
using Ionic.Zip;
//O2Ref:Ionic.Zip.dll

namespace O2.XRules.Database.Utils
{	
	public static class _Extra_Zip_ExtensionMethods
	{	
		public static string zip_File(this string filesToZip)
		{
			return filesToZip.zip_File(".zip".tempFile());
		}
		
		public static string zip_Folder(this string filesToZip)
		{
			return filesToZip.zip_Folder(".zip".tempFile());
		}
		
		public static string zip_Files(this List<string> filesToZip)
		{
			return filesToZip.zip_Files(".zip".tempFile());
		}
		
		public static string zip_Files(this List<string> filesToZip, string targetZipFile)//, string baseFolder)
		{		
			"Creating ZipFile with {0} files to {1}".info(filesToZip.size(), targetZipFile);
			if (targetZipFile.fileExists())
				Files.deleteFile(targetZipFile);
            var zpZipFile = new ZipFile(targetZipFile);            
            foreach(var fileToZip in filesToZip)
            {            	
            	{
            		zpZipFile.AddFile(fileToZip);            
            	}
            	//catch(Exception ex)
            	{
            	//	"[zip_Files] {0} in file {1}".error(ex.Message, fileToZip);
            	}
            }
            zpZipFile.Save();
            zpZipFile.Dispose();
            return targetZipFile;        
		}
	}	
}    	