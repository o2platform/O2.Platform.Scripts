using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.XRules.Database.Utils;
//O2File:WatiN_IE_ExtensionMethods.cs
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2Ref:WatiN.Core.1x.dll
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class JavaJdk : Tool_API 
	{	
		public JavaJdk() : this(true)
		{
		}
		
		public JavaJdk(bool installNow)
		{
			ToolName = "JavaJdk"; 
    		Version = "JavaJdk ";
    		
//    		Install_Dir = @"C:\Program Files\Java\";
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{				
			Func<string> getJdkDownloadUrl = 
				()=>{			
						var topPanel = O2Gui.open<Panel>("Get JDK download link", 900,600);
						var ie = topPanel.add_IE().silent(true);
						ie.open("http://www.oracle.com/technetwork/java/javase/downloads/index.html");
						ie.button("Download JDK").click();
						ie.selectLists()[0].options()[7].select(); 
						ie.checkBox("dnld_license").check(); 
						ie.button("Continue »").click();
						string url = null;
						foreach(var link in ie.links())
							if (link.text().regEx("jdk.*.exe"))
								return link.url(); 
						"couldn't find JDK download link".error();
						topPanel.parentForm().close();
						return url;
					};
					
			Func<string, string> downloadJdk = 
				(downloadUrl)=>{
									if (downloadUrl.valid())
									{
										var targetDir = @"..\_O2Downloads".tempDir(false).fullPath() ; 
										var targetFile = targetDir.pathCombine(downloadUrl.uri().Segments.Last());
										if (targetFile.fileExists())
											return targetFile;
										downloadUrl.download(targetFile); 
										if (targetFile.fileExists())
											return targetFile;							
									}
									return null;
								};
			
			var Install_Uri	 = getJdkDownloadUrl();
			var localFile = downloadJdk(Install_Uri);

			"Installing {0}".info(ToolName);
			localFile.startProcess();
			return localFile.fileExists();
			
			//return installFromExe_Web();
			
			
			//For some weird reason the mouse doesn't work as soon as the installer has the focus
			// so for now we will have to do this manually
			/*
			var guiAutomation = new API_GuiAutomation();
			var window = guiAutomation.desktopWindow("Java(TM) SE Development Kit 6 Update 24 - Setup"); 
			//window.bringToFront();
			//window.minimized(true);  
			var button = window.button("Next >").mouse();
			window.bringToFront();
			guiAutomation.mouse_Click (); 
			*/
		}
		
	}
}