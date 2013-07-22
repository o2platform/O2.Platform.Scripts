using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.REPL;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class ITextSharp_Installer_Test
	{
		public static void Test()  
		{
			new ITextSharp_Installer().start(); 
		}
	}
	 
	public class ITextSharp_Installer : Tool_API    
	{				
		public ITextSharp_Installer()
		{			
			config("ITextSharp", 
				   "itextsharp-all-5.3.5.zip",
				   "http://downloads.sourceforge.net/project/itextsharp/itextsharp/iTextSharp-5.3.5/itextsharp-all-5.3.5.zip?r=&ts=1359122258&use_mirror=heanet".uri(),
				   "itextsharp.dll");
			installFromZip_Web();	
			
			var expectedDll = this.Install_Dir.pathCombine("itextsharp.dll");
			if (expectedDll.fileExists().isFalse())
			{				
				this.Install_Dir.pathCombine("itextsharp-dll-core.zip").unzip(this.Install_Dir);
				this.Install_Dir.pathCombine("itextsharp-dll-pdfa.zip").unzip(this.Install_Dir);
				this.Install_Dir.pathCombine("itextsharp-dll-xtra.zip").unzip(this.Install_Dir);
			}			
		}			
		
		public Process start()
		{			
			if (this.isInstalled())
				this.Install_Dir.startProcess();
			//	return this.Executable.startProcess(); 
			return null;
		}		
	}
}