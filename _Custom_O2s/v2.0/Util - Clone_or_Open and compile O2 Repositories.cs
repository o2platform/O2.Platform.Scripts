// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.XRules.Database.APIs; 
//O2File:API_MSBuild.cs
//O2Ref:O2_FluentSharp_NGit.dll
//O2Tag_DontAddExtraO2Files

public class DynamicType
	{
		public void dynamicMethod()
		{
			O2.XRules.Database.Utils.Util_Sync_and_Compile_O2_Repositories.Main();
		}
	}
	
namespace O2.XRules.Database.Utils
{
	
    public class Util_Sync_and_Compile_O2_Repositories
    { 
    	public static Action gitSync;
    	public static Action compileProjects;
    	public static Action startO2;
    	
    	public Util_Sync_and_Compile_O2_Repositories()
    	{
    		
    	}
    	
    	public static void Main()
    	{
    		buildGui();
    		
    		gitSync();
    		
    		compileProjects();
    		    		
    		
    	}
		public static void buildGui()
		{
			var topPanel = O2Gui.open<Panel>("Util - Sync and Compile O2 Repositories",1000,600); 
			//var topPanel = panel.clear().add_Panel();  
			 
			topPanel.insert_LogViewer(); 
			var msBuildGui = topPanel.add_MSBuild_Gui();   
			
			var nGit_O2 = new API_NGit_O2Platform();			
			
			//for the O2 installer:			
			nGit_O2.LocalGitRepositories 	  = PublicDI.config.CurrentExecutableDirectory.pathCombine("..").fullPath(); // by default it is one  above the current one
			
			gitSync = 
				()=>{
						nGit_O2	.cloneOrPull("O2.FluentSharp")
							  	.cloneOrPull("O2.Platform.Projects")
							   	.cloneOrPull("O2.Platform.Scripts"); 
						"gitSync completed".info();
					};
			
			compileProjects = 
				()=>{
						msBuildGui.buildProjects(nGit_O2.LocalGitRepositories, startO2);
					};
			startO2 =
				()=>{
						nGit_O2.LocalGitRepositories.pathCombine(@"O2.Platform.Projects\binaries\O2 Platform.exe").startProcess();
					};
			//msBuildGui.tableList.title("O2 Platform - MSBuild results");   
			
			msBuildGui.topPanel.insert_Above(40,"Actions")
					  .add_Link	 	 ("Git Sync", 		 ()=> gitSync())
					  .append_Link	 ("Compile Projects", ()=> compileProjects())		  
					  .append_Link	 ("Open Target Folder in Explorer", ()=> nGit_O2.LocalGitRepositories.startProcess())
					  .append_Link	 ("Start 'O2 Platform.exe'", ()=> startO2())
					  .append_Label  ("Target Folder:").autoSize().top(2)
					  .append_TextBox(nGit_O2.LocalGitRepositories).align_Right()
					  											   .onTextChange((text)=> nGit_O2.LocalGitRepositories = text);

		}
	}
}
