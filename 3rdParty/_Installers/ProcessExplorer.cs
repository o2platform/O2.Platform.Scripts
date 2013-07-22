using System.Diagnostics;
using FluentSharp.CoreLib;
using O2.XRules.Database.APIs;

//O2File:Tool_API.cs

public class DynamicType
{
	public void dynamicMethod()
	{
		new ProcessExplorer_Installer().start(); 
	}
}
	
namespace O2.XRules.Database.APIs
{	
	public class ProcessExplorer_Installer : Tool_API 
	{				
		public ProcessExplorer_Installer()
		{
			config("ProcessExplorer", 
				   "http://download.sysinternals.com/files/ProcessExplorer.zip".uri(),
				   "procexp.exe");
			//config("ProcessExplorer", "ProcessExplorer v14.1", "ProcessExplorer.zip");			
    		//Install_Uri = "http://download.sysinternals.com/files/ProcessExplorer.zip".uri();
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
				this.Executable.startProcess();
				//return Install_Dir.pathCombine("procexp.exe").startProcess();
			return null;
		}		
	}
}