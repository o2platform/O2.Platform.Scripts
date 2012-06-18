// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.AST;
using O2.External.SharpDevelop.ExtensionMethods; 

using O2.XRules.Database.Utils;

//O2File:ascx_Simple_Script_Editor.cs.o2
//O2File:Scripts_ExtensionMethods.cs


namespace O2.XRules.Database.Utils
{
    public class ascx_IE_ScriptExecution : Control
    {   
        public Panel topPanel { get; set; }
        public ascx_Simple_Script_Editor script {get; set;}        
		public bool EnableCodeComplete { get; set;}
		
		
		public static ascx_IE_ScriptExecution launchGui()
		{
			return O2Gui.open<ascx_IE_ScriptExecution>("IE Script Execution", 700,600)
						.buildGui();
		}
		
		public static ascx_IE_ScriptExecution launchGui_NoCodeComplete()
		{
			var host = O2Gui.open<Panel>("IE Script Execution (no code complete)", 600,400);
			return (ascx_IE_ScriptExecution)host.invokeOnThread(
				()=>{
						var ieExecution = new ascx_IE_ScriptExecution(false).fill();						
						ieExecution.buildGui();
						host.add_Control(ieExecution);
						return ieExecution;
					});
			
						
		}
		
    	public ascx_IE_ScriptExecution() : this (true)
    	{    		
    	}
    	
    	public ascx_IE_ScriptExecution(bool enableCodeComplete)
    	{
    		this.Width = 600;
    		this.Height = 400;
    		EnableCodeComplete = enableCodeComplete;
    	}
    	    	
    	
		public ascx_IE_ScriptExecution buildGui() 
		{
			topPanel = this.add_Panel();			

			script = topPanel.insert_Below<Panel>().add_Script(EnableCodeComplete);
			script.InvocationParameters.Add("panel",topPanel); 
			script.onCompileExecuteOnce();			
			script.set_Command(getDefaultScript());
			return this;
		}
		
		public string getDefaultScript()
		{
/*****************************/		
/*		
			var scriptWrapper = 	
@"var topPanel = panel.clear().add_Panel();
var ie = topPanel.add_IE().silent(true);

ie.open(""http://www.google.com"");  

//O2File:WatiN_IE_ExtensionMethods.cs 
//using O2.XRules.Database.Utils.O2
//O2Ref:WatiN.Core.1x.dll

//O2Tag_DontAddExtraO2Files;";
*/

/*****************************/
//this version percists the IE object
/*
var scriptWrapper = 
@"var ie = ""ie_{0}"".o2Cache<WatiN_IE>(()=> panel.clear().add_IE()).silent(true);

ie.open(""http://www.google.com"");  

return ie.links();

//O2File:WatiN_IE_ExtensionMethods.cs 
//O2Ref:WatiN.Core.1x.dll
//O2Tag_DontAddExtraO2Files;
";*/

/*****************************/
//this one shows a number of the IE Scripting object capabilities
var scriptWrapper = 
@"var ie = ""ie_" + 5.randomLetters() + @""".o2Cache<WatiN_IE>(()=> panel.clear().add_IE()).silent(true);  // ie ramdon value for o2cache makes this object to unique amongst multiple instances of this control

//ie.open(""about:blank"");
ie.if_NoPageLoaded(
		()=>{ 
				ie.open(""http://www.google.com"");    
				ie.link(""Images"").click();  
				ie.waitForField(""Search Images"").value(""O2 Platform""); 				
//				ie.inject_FirebugLite();
				ie.inject_jQuery();
				//ie.invokeEval(""alert(jQuery)"");
				ie.invokeEval(""jQuery('form').submit()"");				
				//ie.button(""Search Images"");//.click(); 				
			});
			
""Page is open, so returning all image's links"".info();			
return ie.links()	      
	     .uris()
	     .queryParameters_Values(""imgurl"");
	     	 
//O2File:WatiN_IE_ExtensionMethods.cs 
//O2Ref:WatiN.Core.1x.dll
//O2Tag_DontAddExtraO2Files;			
";
			return scriptWrapper;
		}
		
		public ascx_IE_ScriptExecution setScript(string scriptCode)
		{
			script.Code = scriptCode;
			return this;
			
		}
		
		public ascx_IE_ScriptExecution add_SourceCodeEditor(string sourceFile)		
 		{
 			var sourceCodeEditor = topPanel.insert_Right().add_SourceCodeEditor();				
 			sourceCodeEditor.open(sourceFile);
 			return this;
 		}
 		
 		public ascx_IE_ScriptExecution enableCodeComplete(bool value)
 		{
 			this.EnableCodeComplete = value;
 			return this;
 		}
 		
 		
 		public ascx_IE_ScriptExecution createSpecialTestGui(Action<Panel> buildControl)
 		{
 			
 			var treeViewPanel = topPanel.insert_Left(200,"Gui Actions");
 			Action buildMethodsInvocationList = null;
 			
 			buildControl(treeViewPanel);
 			
			//buildMethodsInvocationList = 
			//	()=>{
			//			buildControl(treeViewPanel);
			return this;
		}		
					
 		/*public ascx_IE_ScriptExecution createSpecialTestGui(string file, string baseTypeName, WatiN_IE ie)
 		{
 			
 			var treeViewPanel = topPanel.insert_Left(200,"Gui Actions");
 			Action buildMethodsInvocationList = null;
 			
			buildMethodsInvocationList = 
				()=>{
						buildControl(treeViewPanel);
						var assembly = file.compile();
						var methodsToInvoke = file.compile().withAttribute("ShowInGui");		
						var typeObject = assembly.type(baseTypeName).ctor(ie);
						treeViewPanel.clear()
									 .add_TreeView()
									 .sort()
									 .add_Nodes(methodsToInvoke, (method)=>method.Name)
									 
					.afterSelect<MethodInfo>(	
					   		  (methodToInvoke)=>
					   	  		O2Thread.mtaThread(
					   	  			()=> methodToInvoke.DeclaringType.invokeStatic(methodToInvoke.Name, typeObject) ))
					   	  	;
					   	 //.add_ContextMenu().add_MenuItem("Reload", ()=> buildMethodsInvocationList());
									 
					};
					
			buildMethodsInvocationList();		
 			return this;
 		}*/
 		
 		
		
    }
}
