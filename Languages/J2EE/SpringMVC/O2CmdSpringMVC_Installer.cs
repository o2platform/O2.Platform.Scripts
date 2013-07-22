using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

public class DynamicType
{
	public void dynamicMethod()
	{
		new O2.XRules.Database.APIs.O2CmdSpringMVC_Installer().start();  
	}
} 
	
namespace O2.XRules.Database.APIs 
{
	
	public class O2CmdSpringMVC_Installer : Tool_API  
	{	
		public O2CmdSpringMVC_Installer()
		{
			config("O2CmdSpringMVC", 				   
				   "http://s3.amazonaws.com/O2_Downloads/O2_Cmd_SpringMvc.msi".uri(),
				   "SourceDir\\O2_Cmd_SpringMvc.exe");
			install_JustMsiExtract_into_TargetDir();
		}
				
		
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}		
	}
}