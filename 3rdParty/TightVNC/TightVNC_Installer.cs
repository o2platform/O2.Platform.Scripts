using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs

public class DynamicType
{
	public void dynamicMethod()
	{
		new O2.XRules.Database.APIs.TightVNC_Installer().start();  
	}
} 
	
namespace O2.XRules.Database.APIs 
{
	
	public class TightVNC_Installer : Tool_API  
	{	
		public TightVNC_Installer()
		{
			config("TightVNC", 				   
				   "http://www.tightvnc.com/download/2.6.4/tightvnc-2.6.4-setup-32bit.msi".uri(),
				   "tvnviewer.exe");
			if (Executable.fileExists().isFalse())	   
			{
				install_JustMsiExtract_into_TargetDir();
				var expectedFile = @"SourceDir\PFiles\TightVNC\tvnviewer.exe";
				var vncViewer = this.Install_Dir.pathCombine(expectedFile);
				if(vncViewer.fileExists())
					vncViewer.file_Copy(this.Install_Dir);
				else
					"Failed to find file at: {0}".error(expectedFile);
			}
		}

		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}		
	}
}