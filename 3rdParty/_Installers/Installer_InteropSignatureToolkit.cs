using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
//O2File:API_LessMsi.cs

namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Installer_InteropSignatureToolkit().start(); 
		}
	}
	 
	public class Installer_InteropSignatureToolkit : Tool_API    
	{				
		public Installer_InteropSignatureToolkit()
		{						
			config("InteropSignatureToolkitSetup", "InteropSignatureToolkitSetup v1.0", "InteropSignatureToolkitSetup.zip");			
		    this.Install_Uri = "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=clrinterop&DownloadId=36769&FileTime=128576859141370000&Build=74".uri();
		    this.Executable = this.Install_Dir.pathCombine(@"InteropSignatureToolkitSetup.msi");
		    		
		    installFromZip_Web();
		    this.Executable = this.Install_Dir.pathCombine(@"SourceDir\winsiggen.exe");		    
		    if (Executable.fileExists().isFalse())
		    {
		    	var msi = this.Install_Dir.pathCombine("InteropSignatureToolkitSetup.msi");
		    	msi.extractMsi(this.Install_Dir);		    	
		    }
		}
		//
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}