// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)


//Todo:add svn download support to O2 and use it to checkout the source code of Blind Elephant
//O2Tag:SkipGlobalCompilation



using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
//using O2.Core.XRules.Classes;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
    public class API_BlindElephant  : Tool_API
    {   
    
    	public string BlindElephant_Py {get;set;}    
    	public string Python_Exe { get; set; }
    	public Process PythonExe_Process { get; set; }
    	public Action<string> ConsoleMessage { get; set; }
    	public bool ShowConsoleMessageInLogViewer { get; set; }
    	
    	public API_BlindElephant()
    	{
			config("Blind Elephant", "Blind Elephant 2.0", "");			
			Install_Uri = "https://blindelephant.svn.sourceforge.net/svnroot/blindelephant/trunk".uri();    		    		    		
			BlindElephant_Py = Install_Dir.pathCombine(@"src/blindelephant/BlindElephant.py"); 
			Python_Exe = @"C:\Python27\python.exe";
			ShowConsoleMessageInLogViewer = true;			
		}
            
    
    	public override bool isInstalled()
    	{
    		return Python_Exe.fileExists() && BlindElephant_Py.fileExists();
    	}
    		 
    		 
		public bool install()
		{	
			if (this.isInstalled().isFalse())
			{  
				"[API_BlindElephant] Starting Blind Elephant installation process".info();
				Install_Dir.createDir();
				var svnUrl = Install_Uri.str();
				var targetFolder = Install_Dir;
				var svnMappedUrls = SvnApi.HttpMode.getSvnMappedUrls(svnUrl, true);
				//return svnMappedUrls;
				foreach (var svnMappedUrl in svnMappedUrls)
				{
				    "   * Downloading: {0}".info(WebEncoding.urlDecode(svnMappedUrl.FullPath.Replace(svnUrl, "")));
				    SvnApi.HttpMode.download(svnMappedUrl, svnUrl, targetFolder);
				}
				//var fiddlerInstaller = this.installerFile();
			}
    		return BlindElephant_Py.fileExists();
		}
		
		public void executeCommand(string commands)
		{
			var executionDir = BlindElephant_Py.directoryName();
			var parameters = "{0} {1}".format(BlindElephant_Py.fileName(), commands);			
			PythonExe_Process = Processes.startProcessAndRedirectIO(Python_Exe, parameters, executionDir, handleConsoleMessage);

		}
		
		public void handleConsoleMessage(string message)
		{
			if (ConsoleMessage.notNull())
				ConsoleMessage(message);
			if (ShowConsoleMessageInLogViewer)
				"[BlindElephant] {0}".info(message);			
		}
	}
}
