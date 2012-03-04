// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Linq; 
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using HTTPProxyServer;
 
//O2File:O2_Web_Proxy.cs
//O2File:O2_Web_Proxy_ExtensionMethods_Waf_Rules.cs

//O2File:_Extra_methods_Files.cs
//_O2File:_Extra_methods_Reflection.cs
//_O2File:_Extra_methods_WinForms_Controls.cs
//O2File:_Extra_methods_WinForms_TreeView.cs
//O2File:_Extra_methods_WinForms_TableList.cs
//O2File:_Extra_methods_ObjectDetails.cs
 
//_O2Ref:System.Configuration.dll
  
  
namespace O2.XRules.Database.APIs
{
	public class O2_Web_Proxy_Test
	{	
		public Panel launchTestGui_Proxy_SimpleView()
		{
			var topPanel = "Testing O2 Web Proxy".popupWindow(300,400);
			new O2_Web_Proxy().createGui_Proxy_SimpleView(topPanel)
						   	  .startWebProxy();
			"http://google.com".uri().getHtml();
			return topPanel;
		}
		
 		public void launchTestGui_Proxy_LargeView()
		{
			var topPanel = "Testing O2 Web Proxy".popupWindow(1000,700);
			new O2_Web_Proxy().createGui_Proxy_LargeView(topPanel)
						   	  .startWebProxy();
			"http://google.com".uri().getHtml();
		}
		
		public void launchTestGui_WAF()
		{
			var topPanel = "Testing O2 Web Proxy".popupWindow(1000,500);
			new O2_Web_Proxy().createGui_WAF(topPanel)
						   	  .startWebProxy();
			topPanel.controls<WebBrowser>(true)[0]
					.open("http://www.google.com");
		}
	}		
	
	public static class O2_Web_Proxy_ExtensionMethods_GUI_Helpers
	{								
		public static O2_Web_Proxy createGui_Proxy_SimpleView(this O2_Web_Proxy o2WebProxy,  Control panel)		
		{	
			var topPanel = panel.clear().add_Panel(); 
			//topPanel.insert_LogViewer();
			o2WebProxy.stopOnFormClose(topPanel);		  
			var actions_Panel = topPanel.insert_Above(40,"");
			var requests = topPanel.add_GroupBox("Requests").add_TableList().title("O2 Web Proxy");
			var properties = requests.insert_Below(100,"Request & Response Object").add_PropertyGrid()
																				   .helpVisible(false)
																				   .toolBarVisible(false);
			//actions_Panel.insert_Right(40).add_Proxy_ToolsPanel();																				   
			var showLiveRequests = false;
			
			var requestId = 0;
			requests.add_Columns("#","Method","Status","Size", "Url");
			
			Action setColumnsWidth = ()=> requests.set_ColumnsWidth(30, 60,60);			 
			setColumnsWidth();
			
			requests.onDoubleClick<RequestResponseData>(
				(requestResponseData)=> {
											var responseCode = "Html for: {0}".format(requestResponseData.Request_Uri)
																			  .popupWindow()
																			  .add_GroupBox("Response Data")
																			  .add_SourceCodeViewer().maximizeViewer()
																			  .set_Text(requestResponseData.Response_String, ".html");
											if (requestResponseData.Request_PostString.valid())
												responseCode.parent()
															.insert_Above(200,"Request Post Data")
															.add_SourceCodeViewer().maximizeViewer()
															.set_Text(requestResponseData.Request_PostString, ".html");	
										});																	  
			  
			Action<RequestResponseData> add_Row = 
				(rrData)=>{
						 	requests.add_Row(rrData, 
								()=>{
										var rowData = new List<string>()
											{	(++requestId).str(),
												rrData.WebRequest.Method.str(),
												rrData.WebResponse.StatusCode.str(),
												rrData.Response_String.size().str(),												
												rrData.WebRequest.RequestUri.str()
											 };
										return rowData;	   	
									});							 
						  };			
			
			requests.afterSelect<RequestResponseData>(
				(requestResponseData)=>	{
											properties.show(requestResponseData);
									   	});
															
			Action showPastRequests =   
				()=>{ 
						requests.clearRows();					
						requests.title("Showing {0} web Requests".format(o2WebProxy.Proxy.requests().size()));
						foreach(var request in o2WebProxy.Proxy.requests())
							add_Row(request);
						requests.setWidthToContent();	
					};
			
			Action copySelectedUtlToClipboard =   
				()=>{
						var selected = requests.selected<RequestResponseData>();
						if (selected.notNull())													
							selected.Request_Uri.str().clipboardText_Set();
					};
			ProxyServer.OnResponseReceived =  
				(requestResponseData)=> add_Row(requestResponseData);
										      
			requests.add_ContextMenu().add_MenuItem("Copy Utl to clipboard", true,()=> copySelectedUtlToClipboard() )
									  .add_MenuItem("See Selected Row RequestResponseData Object Details", true,()=>requests.tag().details())
									  .add_MenuItem("Refresh request list ({0} at start)".format(o2WebProxy.Proxy.requests().size()), true, ()=> showPastRequests())
									  .add_MenuItem("Clear list", true, ()=> requests.clearRows())
									  .add_MenuItem("Reset Columns Width", ()=> { setColumnsWidth();setColumnsWidth(); });
						 
			actions_Panel.add_Label("Actions:")
						 .append_Link("Proxy Start",()=> o2WebProxy.startWebProxy())
						 .append_Link("Proxy Stop",()=> o2WebProxy.stopWebProxy())			 
						 .append_CheckBox("Real-Time View", (value)=> showLiveRequests = value).@check()
						 .append_CheckBox("Extra logging", (value)=> ProxyServer.ExtraLogging = value);
						 
			
			actions_Panel.add_Label("Test sites:",20,0)		 
						 .append_Link("BBC", ()=> "http://news.bbc.co.uk".get_Html())
						 .append_Link("OWASP",  ()=> "http://www.owasp.org".get_Html())
						 .append_Link("google",  ()=> "http://www.google.com".get_Html())
						 .append_Label("O2 Utils")
						 .append_Link("O2 Log Viewer",  ()=> "O2 Log Viewer".popupWindow().add_LogViewer());
			return o2WebProxy;						 
		}
		
		
		//createGui_Proxy_LargeView
		public static O2_Web_Proxy createGui_Proxy_LargeView(this O2_Web_Proxy o2WebProxy,  Control panel)		
		{
			var topPanel = panel.clear().add_Panel();
			topPanel.insert_LogViewer();
			o2WebProxy.stopOnFormClose(topPanel)		  
					  .add_Proxy_ActionsPanel(topPanel)
					  .add_Proxy_ToolsPanel();           			
			
			var requestsList = topPanel.add_GroupBox("Requests List").add_TreeView();
			var requestDetails = topPanel.insert_Right();
			var requestProperties = requestDetails.add_GroupBox("Request details").add_PropertyGrid().helpVisible(false);
			var responseProperties = requestDetails.insert_Right("Response details").add_PropertyGrid().helpVisible(false);
			var requestData = requestProperties.insert_Below(100,"Request Data").add_TextArea();
			var responseData = responseProperties.insert_Below(100,"Response Data").add_SourceCodeViewer();
			responseData.textEditorControl().fill().bringToFront();
			
			
			requestsList.add_ContextMenu()						
						.add_MenuItem("clear list", false,()=> requestsList.clear());
						
			ProxyServer.OnResponseReceived 	= (requestResponseData)=> requestsList.add_Node(requestResponseData);
			
			requestsList.afterSelect<RequestResponseData>( 
							(requestResponseData)=>{
														requestProperties.show(requestResponseData.WebRequest);
														responseProperties.show(requestResponseData.WebResponse); 
														responseData.set_Text(requestResponseData.Response_String);
														requestData.set_Text(requestResponseData.Request_PostString);
													});			
			requestsList.onDoubleClick<RequestResponseData>(
							(requestResponseData)=> {
														"showing requestResponseData details".info();
														requestResponseData.details();
														"done".debug();
													});
			
			return o2WebProxy;
		}
		
		
		//createGui_WAF
		public static O2_Web_Proxy createGui_WAF<T>(this O2_Web_Proxy o2WebProxy,  T panel)		
			where T : Control
		{
			var topPanel = panel.clear().add_Panel();
			topPanel.insert_LogViewer();
			
			o2WebProxy.stopOnFormClose(topPanel)
					  .add_Proxy_ActionsPanel(panel)
					  .add_Proxy_ToolsPanel();
					
			
			//var actionsPanel = topPanel.insert_Above(40,"actions");
			//var toolsPanel = actionsPanel.parent().insert_Right("tools");
			var browserPanel = topPanel.add_GroupBox("Browser");
			var browser = browserPanel.add_WebBrowser_Control().add_NavigationBar();			
			var httpData = topPanel.insert_Right(200).add_TreeView();			
			var sourceCodeViewer = httpData.insert_Below().add_SourceCodeViewer();			
			var rules = topPanel.insert_Left(400,"WAF Rules").add_TreeView(); 			

			httpData.add_ContextMenu()
					.add_MenuItem("clear list", false,()=> httpData.clear());
					//.add_MenuItem("Write Script on ;
					
			httpData.afterSelect<RequestResponseData>(
				(requestResponseData)=> {
											//requestResponseData.str().debug();
											//show.info(requestResponseData.WebResponse);
											//show.info(requestResponseData.WebRequest);
											//"Content for {0} with size {1}".info(requestResponseData, htmlContent.size());
											//htmlContent.info();
											sourceCodeViewer.set_Text(requestResponseData.Response_String, ".html");
										});
//O2File:_Extra_methods_WinForms_Controls.cs		 	
//O2File:Scripts_ExtensionMethods.cs
		 											
/*			httpData.onDoubleClick<RequestResponseData>(
				(requestResponseData)=> {																			
											"RequestResponseData".popupWindow()
																.add_Script(false)
																.InvocationParameters.Add("bytes",requestResponseData.ResponseBytes);
										});
*/			
/*			Action setWafRuleLinkViewerCallback = 
						()=>{								
								Action<HttpWebRequest,HttpWebResponse, string> onRequestReceived = 
									(webRequest, webResponse, htmlContent)=> 
										links.add_Node(webRequest.RequestUri.str(),htmlContent );
										
								o2WebProxy.WafRule.prop("OnRequestReceived" , onRequestReceived);																
							};
*/
			ProxyServer.OnResponseReceived 	= (requestResponseData)=> httpData.add_Node(requestResponseData);
			Action<string> loadWafRule = 	
				(wafRuleFile)=> { 
									O2Thread.mtaThread(
										()=>{
												if (o2WebProxy.loadWafRule(wafRuleFile))
													browserPanel.set_Text("Loaded Rule: {0}".format(wafRuleFile.fileName()));
												//setWafRuleLinkViewerCallback(); 
											});
								};
								
			rules.onDrop(
				(fileOrFolder)=>{ 
									if (fileOrFolder.fileExists())
										rules.add_File(fileOrFolder);
									else
									{
										rules.clear();
										rules.add_Files(fileOrFolder);
									}
								 });
			
			rules.add_ContextMenu().add_MenuItem("Edit Rule", ()=> (rules.selected().get_Tag() as String).showInCodeEditor()); 
			
			rules.add_Files("WAF_Rule_NoGoogle.cs".local().directoryName().files("WAF*.cs"));
			//rules.add_Node("WAF_Rule_NoPorto.cs", "WAF_Rule_NoPorto.cs".local());
			
			o2WebProxy.loadWafRule("WAF_Rule.cs", true);
			
			/*actionsPanel.add_Label("O2 Proxy")		
						.append_Link("Proxy Start", ()=> o2WebProxy.startWebProxy())
						.append_Link("Proxy Stop",  ()=> o2WebProxy.stopWebProxy()) 
						.append_Link("Set Browser Proxy",  ()=> o2WebProxy.setBrowserProxy())
						.append_Link("Clear Browser Proxy",  ()=> o2WebProxy.clearBrowserProxy())
						.append_Link("Stop Current Process", ()=> Processes.getCurrentProcess().stop());
			*/
			
			
			/*toolsPanel.add_Link("Javascript Format (and Beautify)", ()=> "Util - Javascript Format (and Beautify).h2".local().executeH2Script())
					  .append_Link("Web Encoding and Decoding",		()=> "Util - Web Encoder (with AntiXss Support).h2".local().executeH2Script());
			*/
						;
		
			rules.afterSelect<string>  (loadWafRule);
			rules.onDoubleClick<string>(loadWafRule); //(file) => { o2WebProxy.loadWafRule(file); setWafRuleLinkViewerCallback(); } );
			//setWafRuleLinkViewerCallback();
			return o2WebProxy;
		}
		
		public static Panel add_Proxy_ActionsPanel(this O2_Web_Proxy o2WebProxy, Control topPanel)
		{		
			var actionsPanel = topPanel.insert_Above(40,"actions");
			actionsPanel.add_Label("O2 Proxy")	
						.append_Link("View Proxy Object", ()=> o2WebProxy.details())
						.append_Link("Proxy Start", ()=> o2WebProxy.startWebProxy())
						.append_Link("Proxy Stop",  ()=> o2WebProxy.stopWebProxy()) 
						.append_Link("Set Browser Proxy",  ()=> o2WebProxy.setBrowserProxy())
						.append_Link("Clear Browser Proxy",  ()=> o2WebProxy.clearBrowserProxy())
						.append_Link("Stop Current Process", ()=> Processes.getCurrentProcess().stop());
			return actionsPanel;
		}
		
		public static Panel add_Proxy_ToolsPanel(this Control actionsPanel)
		{		
			var toolsPanel = actionsPanel.parent().insert_Right("tools");
			toolsPanel.add_Link("Javascript Format (and Beautify)", ()=> "Util - Javascript Format (and Beautify).h2".local().executeH2Script())
					  .append_Link("Web Encoding and Decoding",		()=> "Util - Web Encoder (with AntiXss Support).h2".local().executeH2Script())
					  .append_Link("IE Automation",					()=> "IE Automation (Simple mode).h2".local().executeH2Script());
					  
			return toolsPanel;
		}
		
		
		public static O2_Web_Proxy stopOnFormClose(this O2_Web_Proxy o2WebProxy, Control topPanel)
		{
			topPanel.onClosed(()=> o2WebProxy.stopWebProxy());
			return o2WebProxy;
		}
			
	}
}
	