using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;

//O2File:Tool_API.cs
//O2File:API_IKVM.cs
//Installer:IKVM_Installer.cs!IKVM\ikvm-7.2.4630.5\bin\ikvm.exe

namespace O2.XRules.Database.APIs
{
	public class Groovy_Installer_Test
	{
		public void test() 
		{
			new Groovy_Installer().start(); 
		}
	}
	public class Groovy_Installer : Tool_API 
	{		
		public string Groovy_Version    	{ get; set; }
		public string Groovy_DotNet_Folder  { get; set; }
		public string Groovy_DotNet_Dll 	{ get; set; }
		
		public Groovy_Installer()
		{
			Groovy_Version = "2.1.6";
			config("Groovy", 				   
			   	   "http://dist.groovy.codehaus.org/distributions/groovy-binary-{0}.zip".format(Groovy_Version).uri(),				   
			   	   @"groovy-{0}\bin\groovyConsole.bat".format(Groovy_Version));
    		installFromZip_Web(); 						
    		    		
    		Groovy_DotNet_Folder = this.Install_Dir.pathCombine("DotNet_IKVM");
			Groovy_DotNet_Dll = Groovy_DotNet_Folder.pathCombine("groovy-all-{0}.dll".format(Groovy_Version));    								
    								
    		if (Groovy_DotNet_Folder.dirExists().isFalse() || Groovy_DotNet_Dll.fileExists().isFalse())
    			Create_DotNetDll_Using_IKVM();   
    		if (Groovy_DotNet_Dll.fileExists())
				"Groovy DLL is ready for use: {0}".debug(Groovy_DotNet_Dll);
			else
				"Could not find Groovy DLL: {0}".error(Groovy_DotNet_Dll);	
		}		
		
		public void Create_DotNetDll_Using_IKVM()
		{
			"Creating Groovy .NET Dll using IKVM: {0}".info();			
			Groovy_DotNet_Folder.createDir();
			var groovyJar = this.Install_Dir.pathCombine(@"groovy-{0}\embeddable\groovy-all-{0}.jar".format(Groovy_Version));
			//"Groovy Jar: {0} {1}".info(groovyJar, groovyJar.fileExists());
			
			var apiIKVM = new API_IKVM(); 

			var result = apiIKVM.convertJarFileIntoDotNetAssembly(groovyJar,Groovy_DotNet_Folder);
			"Result: {0}".info(result);						
		}

		public Process start()
		{
			//if (isInstalled())
			//	return this.Executable.startProcess();
			return null;
		}
	}
}