// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.Objects;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.Ascx.MainGUI;
using DiagramDesigner;

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:QuickGraph.dll   
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:DiagramDesigner.exe

//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

namespace O2.XRules.Database.APIs
{		
    public class DiagramDesigner_NewAppDomain
    {    
    	public static void launchInNewAppDomain()
    	{
    		var script = "DiagramDesigner_NewAppDomain.launchDiagramDesignerGui(true); ".line() + 
						"//using: O2.XRules.Database.APIs".line() + 
						@"//O2File:" + "DiagramDesigner_NewAppDomain.cs".local();
			"Executing script in new appdomain:{0}".info(script.lineBefore());							
    		ExtraAppDomain_Script.executeInAppDomain("AppDomain for DiagramDesigner",script);
    	}
    	
    	public static void launchDiagramDesignerGui()
    	{
    		launchDiagramDesignerGui(true); 
    	}
    	
    	public static void launchDiagramDesignerGui(bool showLogViewer)
    	{
    		if (showLogViewer)
    			O2Gui.open<ascx_LogViewer>("LogViewer", 400,200);

			O2Thread.staThread(
				()=>{
						"start Sta thread".info();
						Func<System.Windows.Application> createApp = 
							()=>{
									var app = new DiagramDesigner.App();
									Uri resourceLocater = new Uri("/DiagramDesigner;component/app.xaml", UriKind.Relative);
									System.Windows.Application.LoadComponent(app, resourceLocater);
									return app;
								};
									
						var application = System.Windows.Application.Current.isNull() 
													  	? createApp()
													  	: System.Windows.Application.Current;				
						"WPF Application created".info();						
						var window1 = new Window1();  
						"DiagramDesigner Main Window created".info();						
						//show.info(window1);
						window1.Top = 10;
						window1.Left  = 10;
						window1.Width= 700;
						window1.Height= 600;
						"Launching WPF Application".info();
						application.Run(window1);
						"WPF Application ended".info();
						
						//System.Windows.Application.Current.run
					});
		}			  				
    }
    
    public class ExtraAppDomain_Script
	{
		public static void executeInAppDomain(string appDomainName,string scriptToExecute)
		{
				O2Thread.mtaThread(
					()=>{
							var o2AppDomain =  new O2.Kernel.Objects.O2AppDomainFactory(appDomainName);
							try
							{
								o2AppDomain.load("O2_XRules_Database"); 	
								o2AppDomain.load("O2_Kernel");
								o2AppDomain.load("O2_DotNetWrappers");
							
								var o2Proxy =  (O2Proxy)o2AppDomain.getProxyObject("O2Proxy");
								 
								o2Proxy.InvokeInStaThread = true;
								o2Proxy.staticInvocation("O2_External_SharpDevelop","FastCompiler_ExtensionMethods","executeSourceCode",new object[]{ scriptToExecute });								
							}
							catch(Exception ex)
							{
								ex.log("inside new AppDomain"); 
							}
							
							DebugMsg.showMessageBox("Click OK to close the '{0}' AppDomain (and close all open windows)".format(appDomainName));							
							o2AppDomain.unLoadAppDomain();
						});
		  }
	}
}
