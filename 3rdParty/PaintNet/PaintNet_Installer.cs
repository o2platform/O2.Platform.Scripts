using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class PaintNet_Installer_Test
	{
		public void test()
		{
			new PaintNet_Installer().start(); 
		}
	}
	public class PaintNet_Installer : Tool_API 
	{	
		public PaintNet_Installer()
		{
			config("PaintNet", 			
				   "http://www.dotpdn.com/files/Paint.NET.3.5.10.Install.zip".uri(),
				   @"PaintDotNet.exe"); 
				   			
			if (isInstalled().isFalse())
			{
				installFromZip_Web(); //downloads the zip and puts it on the target dir:_ToolsOrApis\PaintNet				
				var arguments = "/auto TARGETDIR=\"{0}\" DESKTOPSHORTCUT=0".format(this.Install_Dir);		
				var installExe = this.Install_Dir.pathCombine("Paint.NET.3.5.10.Install.exe");			
				installExe.startProcess(arguments);
			}			    		
		}
		
				
		
		public Process start()
		{						
			if (isInstalled())				
				return this.Executable.startProcess();
			return null;
		}		
	}
}