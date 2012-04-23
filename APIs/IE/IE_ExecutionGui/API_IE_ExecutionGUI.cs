// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:WatiN_IE_ExtensionMethods.cs    
//O2Ref:WatiN.Core.1x.dll

//O2File:_Extra_methods_WinForms_Controls.cs
//O2File:_Extra_methods_Reflection.cs
//O2File:_Extra_methods_WinForms_TreeView.cs

namespace O2.XRules.Database.APIs
{	
	public class API_IE_ExecutionGUI_Test
	{
		public void testGui()
		{
			var popupWindow = "IE Execution GUI".popupWindow(700,500);
			popupWindow.add_IE_ExecutionGui("");
			//var ieExecution = new API_IE_ExecutionGUI();
		}
	}
	
    public class API_IE_ExecutionGUI
    {    
    	public WatiN_IE ie { get; set;}    	
    	public string TargetServer  { get; set;}
    	public string FolderToPutNotMappedMethods { get; set; }
    	
    	public API_IE_ExecutionGUI()
    	{
    		FolderToPutNotMappedMethods = "_not mapped";
    		applyConfigOptions();
    	}
    	
    	public API_IE_ExecutionGUI(WatiN_IE _ie) : this()
    	{
    		ie = _ie;    		
    	}        	
    	
    	public API_IE_ExecutionGUI(WatiN_IE _ie, string targetServer) : this(_ie)
    	{
    		TargetServer = targetServer;
    	}
    	
    	public API_IE_ExecutionGUI(Control hostControl)	: this()
    	{    		
    		API_IE_Execution_BuildGui.getMethodMappings(this);    		
    		API_IE_Execution_BuildGui.create_IE_ExecutionGui(this, hostControl);    		
    	}
    	
    	
    	public API_IE_ExecutionGUI applyConfigOptions()
		{
			Web.Https.ignoreServerSslErrors();
			return this;
		}
    	
    	public T open<T>(string virtualPath) where T : API_IE_ExecutionGUI
    	{
    		return (T)open(virtualPath);
    	}
    	
    	public API_IE_ExecutionGUI open(string virtualPath) 
		{
			if (virtualPath.isUri())
				ie.open(virtualPath);
			else
			{
				if (virtualPath.starts("/").isFalse())
					virtualPath = "/{0}".format(virtualPath);
				var fullUri = "{0}{1}".format(TargetServer, virtualPath).uri();
				ie.open(fullUri.str());
			}
			return this;
		}
			
		
    }
    
    public class ShowInGuiAttribute : Attribute 
	{
		public string Folder {get;set;}
	}
	
/*	public static class API_IE_ExecutionGUI_ExtensionMethods_Actions
	{
		[ShowInGui(Folder ="root")]
		public static API_IE_ExecutionGUI homePage(this API_IE_ExecutionGUI ieExecution)
		{
			return ieExecution.open(""); 
		}
		
	}*/
	
	public static class API_IE_Execution_BuildGui
	{
	
		public static Dictionary<string,List<MethodInfo>> getMethodMappings<T>(this T executionGui)
			where T : API_IE_ExecutionGUI
		{
			var type = executionGui.type();
			var methodsToInvoke = type.Assembly.methodsWithAttribute<ShowInGuiAttribute>();
			var methodMappings = new Dictionary<string,List<MethodInfo>>();
			foreach(var methodToInvoke in methodsToInvoke)
			{
				var folderAttribute = methodToInvoke.attribute<ShowInGuiAttribute>();
				if (folderAttribute.Folder.notNull())
					methodMappings.add(folderAttribute.Folder, methodToInvoke);
				else
					methodMappings.add(executionGui.FolderToPutNotMappedMethods, methodToInvoke);					
				//methodMappings.add("_not mapped", methodToInvoke);
			}
			//methodMappings.add("folder", methodsToInvoke); //"Folder");
			return methodMappings;
		}
				
		public static T create_IE_ExecutionGui<T>(this T executionGui, Control control)
			where T : API_IE_ExecutionGUI
		{															
			Action<Control> buildGui = 
				(panel)=>{		
							var topPanel = panel.clear().add_Panel();		
							topPanel.insert_LogViewer();
							var ie = topPanel.add_IE().silent(true);	
							executionGui.ie = ie;
							//executionGui.prop("ie",ie);
							//var executionGui = executionGuiType.ctor(ie);// new API_IE_ExecutionGUI(ie);
							var treeViewPanel = topPanel.insert_Left(200,"Gui Actions");
							Action buildMethodsInvocationList = null;
							buildMethodsInvocationList = 
								()=>{
										Action<MethodInfo> executeMethod = 
											  (methodToInvoke)=> O2Thread.mtaThread(
													()=> {
															"executing method: {0}".info(methodToInvoke.Name);															
															methodToInvoke.DeclaringType.invokeStatic(methodToInvoke.Name, executionGui); 
														  });
												   	  			
										
										var methodMappings = executionGui.getMethodMappings();
										var treeView = treeViewPanel .clear()
																	 .add_TreeView()
																	 .sort()										 
																     .afterSelect<MethodInfo>(executeMethod)
																   	 .onDoubleClick<MethodInfo>(executeMethod);
																   	 
										treeView.add_ContextMenu().add_MenuItem("Reload", ()=> buildMethodsInvocationList());
										foreach(var methodMapping in methodMappings)
											if (methodMapping.Key !="root")
												treeView.add_Node(methodMapping.Key)
														.add_Nodes(methodMapping.Value, (method)=>method.Name.replace("_"," "));
												else
													treeView.add_Nodes(methodMapping.Value, (method)=>method.Name.replace("_"," "));
										
										treeView.expandAll().selectFirst();
										//treeView.add_Nodes(methodsToInvoke, (method)=>method.Name);
									};
									
							buildMethodsInvocationList();
							var textBox = topPanel.insert_Above(20).add_Label("Url loaded").top(4).append_TextBox("").align_Right(topPanel).onEnter((text)=>ie.open_ASync(text)); 
							ie.onNavigate((url)=>textBox.set_Text(url));
							topPanel.insert_Below(24).add_Label("Invoke Javascript").top(4).append_TextBox("").align_Right(topPanel).onEnter((text)=>ie.invokeEval(text)); 
							
					};
			buildGui(control);
			return executionGui;
		}
		
		public static T add_IE_ExecutionGui<T>(this T panel, string script)
			where T : Control
		{
			var scriptToLoad = script;
			//Action<string> loadScriptFile  =    
			//	(script)=>{					 
							var topPanel = panel.clear().add_Panel();							
							Action loadGui =  
								()=>{			
										if (scriptToLoad.valid())
							//			var buildGui =  (Action<Control>)script.compile_H2Script().executeFirstMethod();  			
							//			buildGui(topPanel.add_Panel());		
										{											
											"compiling and executing: {0}".info(scriptToLoad);
											scriptToLoad.compile().types().first().ctor(topPanel);
										}
											
									};
							Action<string> loadFile = 
								(fileToLoad)=>{
												"trying to load file: {0}".info(fileToLoad);
												if (fileToLoad.fileExists().isFalse())
													fileToLoad = fileToLoad.local();
												if (fileToLoad.fileExists())
												{
													scriptToLoad = fileToLoad;
													loadGui();
												}
											  };									
									
							topPanel.insert_Above(20)									
									.add_Link("Reload Gui",0,0, ()=> loadGui())
									.append_Link("Open Gui SourceCode", ()=> scriptToLoad.showInCodeEditor())
									.append_Link("Open IE Automation GUI", ()=> "IE Automation (Simple mode).h2".local().executeH2Script())
									.append_Link("Test file: Reload IE_Google.cs", ()=> loadFile("IE_Google.cs")) 
									.append_Link("Load another IE Automation Script", ()=> loadFile("which script do you want to load".askUser()) ) ;
									//.append_Link("Open IE Automation Development Environment", ()=> devEnvironment.executeH2Script());;
							
							if (scriptToLoad.valid())
								loadFile(scriptToLoad);
							else
								loadFile("IE_Google.cs");
			//			};
			//loadScriptFile(script);
			return (T)panel;
		}
	}
    
}
