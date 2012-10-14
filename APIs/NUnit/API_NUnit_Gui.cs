// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.XRules.Database.Utils;
using NUnit.UiKit;
using NUnit.Gui;
using NUnit.Util;
using NUnit.Core;

//O2File:API_NUnit.cs

namespace O2.XRules.Database.APIs
{
    public class API_NUnit_Gui : API_NUnit
    {        	
    	public NUnitForm nUnitForm;
    	public bool NunitGuiLoaded	{ get; set; }
    	
    	public API_NUnit_Gui() : this(true)
    	{    		    		
    	}
    	
    	public API_NUnit_Gui(bool autoLoadGui)
    	{
    		if(autoLoadGui)
    		{
    			loadNUnitGui();
    		}
    	}
    	
    	public API_NUnit_Gui(GuiOptions guiOptions)
    	{
    		loadNUnitGui(guiOptions);
    	}
    	
    	public API_NUnit_Gui loadNUnitGui()
    	{
    		var guiOptions = new GuiOptions(new string[] {});
    		guiOptions.noload = true;
    		return loadNUnitGui(guiOptions);
    	}
    	
    	public API_NUnit_Gui loadNUnitGui(GuiOptions guiOptions)
    	{
    		this.nUnitForm = this.nUnitGui_ShowGui(guiOptions);
    		NunitGuiLoaded = true;
    		return this;
    	}
    }
    
    public static class API_NUnit_GUI_ExtensionMethods_Actions
    {
    	public static bool guiNotLoaded(this API_NUnit_Gui nUnitGui)
    	{
    		return !nUnitGui.NunitGuiLoaded;
    	} 
    	
    	public static API_NUnit_Gui closeProject(this API_NUnit_Gui nUnitGui)
    	{
    		TestLoaderUI.CloseProject(nUnitGui.form());
    		return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui openProject(this API_NUnit_Gui nUnitGui, string projectOrAssembly)
    	{
    		if (nUnitGui.guiNotLoaded())
    			nUnitGui.loadNUnitGui();
    		nUnitGui.form().invokeOnThread(
    			()=>{    			
			    		nUnitGui.form().visible(true);	
			    		TestLoaderUI.OpenProject(nUnitGui.form(), projectOrAssembly);
			    	});
    		return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui runAllTests(this API_NUnit_Gui nUnitGui)
    	{
    		nUnitGui.treeView_Tests().RunAllTests();
    		return nUnitGui;
    	}
    	
    	public static bool compileAndOpen(this API_NUnit_Gui nUnitGui, string fileToCompile)
    	{
    		var assembly = new CompileEngine().compileSourceFile(fileToCompile);
			if (assembly.notNull())
			{
				var location = assembly.Location; 			
				nUnitGui.openProject(location);			
				return true;
			}
			return false;
    	}
    	
    	public static API_NUnit_Gui compileOpenAndRun(this API_NUnit_Gui api_NUnitGui, string fileToCompile)
    	{
    		if(api_NUnitGui.compileAndOpen(fileToCompile))
    			api_NUnitGui.runAllTests();
    		return api_NUnitGui;
    	}
   
    	
    }
    public static class API_NUnit_GUI_ExtensionMethods_GuiControls
    {
    	public static TestSuiteTreeView treeView_Tests(this API_NUnit_Gui nUnitGui)
    	{
    		return nUnitGui.form().control<TestSuiteTreeView>(true);
    	}
    }
    
    public static class API_NUnit_GUI_ExtensionMethods_GuiCustomization
    {
    	public static API_NUnit_Gui set_FormTitle(this API_NUnit_Gui nUnitGui, string newTitle)
    	{
    		nUnitGui.nUnitForm.set_Text(newTitle);
    		return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui visible(this API_NUnit_Gui nUnitGui, bool value)
    	{
    		nUnitGui.nUnitForm.visible(value);
    		return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui add_LogViewer(this API_NUnit_Gui nUnitGui)
    	{
    		nUnitGui.nUnitForm.insert_LogViewer();
    		return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui insert_REPL_Below(this API_NUnit_Gui nUnitGui)
    	{
    		var _panel = nUnitGui.nUnitForm.insert_Below("Script NUnit GUI");
			nUnitGui.nUnitForm.script_Me(_panel);
			return nUnitGui;
    	}
    	
    }
    
    public static class API_NUnit_GUI_ExtensionMethods_LoadGui
    {
    	public static NUnitForm form(this API_NUnit_Gui nUnitGui)
    	{
    		return nUnitGui.nUnitForm;
    	}
    	
    	public static API_NUnit_Gui openNUnitGui(this API_NUnit_Gui nUnitGui)
    	{    		
			var nunitGuiRunner = nUnitGui.Executable.parentFolder().pathCombine("lib\\nunit-gui-runner.dll");
			nunitGuiRunner.loadAssemblyAndAllItsDependencies();
			nunitGuiRunner.type("AppEntry").method("Main").invokeStatic_StaThread(new string[] {} )  ;
			return nUnitGui;
    	}
    	
    	public static API_NUnit_Gui openNUnitGui_in_SeparateAppDomain(this API_NUnit_Gui nUnitGui)
    	{
    		var script = @"
    						var nunitAPi = new API_NUnit();
							nunitAPi.openNUnitGui();
							//O2File:API_NUnit.cs";

			script.execute_InScriptEditor_InSeparateAppDomain();
			return nUnitGui;
		}
		
		public static NUnitForm nUnitGui_ShowGui(this API_NUnit_Gui nUnitGui)
		{
			return nUnitGui.nUnitGui_ShowGui(new GuiOptions(new string[] {}));			
		}
		
		public static NUnitForm nUnitGui_ShowGui(this API_NUnit_Gui nUnitGui, GuiOptions guiOptions)	
		{						
			"[API_NUnit_Gui] in nUnitGui_ShowGui".info();
			Func<NUnitForm> createNUnitForm = 
				()=>{		
						var sync = new AutoResetEvent(false);
						NUnitForm nUnitForm = null;
						O2Thread.staThread(
						()=>{
								if(NUnit.Util.Services.TestAgency.isNull())
								{
										var nunitGuiRunner = nUnitGui.Executable.parentFolder().pathCombine("lib\\nunit-gui-runner.dll");
										nunitGuiRunner.loadAssemblyAndAllItsDependencies();
								
										SettingsService settingsService = new SettingsService();
										InternalTrace.Initialize("nunit-gui_%p.log", (InternalTraceLevel)settingsService.GetSetting("Options.InternalTraceLevel", InternalTraceLevel.Default));			
										ServiceManager.Services.AddService(settingsService);
										ServiceManager.Services.AddService(new DomainManager());
										ServiceManager.Services.AddService(new RecentFilesService());
										ServiceManager.Services.AddService(new ProjectService());		
										ServiceManager.Services.AddService(new AddinRegistry());
										ServiceManager.Services.AddService(new AddinManager());
										ServiceManager.Services.AddService(new TestAgency());
										ServiceManager.Services.InitializeServices();
								}
								else 
									"[API_NUnit_Gui] in nUnitGui_ShowGui: NUnit.Util.Services.TestAgency was not null: {0}".debug(NUnit.Util.Services.TestAgency);
														
								ServiceManager.Services.AddService(new TestLoader(new GuiTestEventDispatcher()));											
								nUnitForm = new NUnitForm(guiOptions);												
								"NUnitForm".o2Cache(nUnitForm);		
								sync.Set();
								nUnitForm.ShowDialog();	
								"NUnitForm".o2Cache(null);
							});
						sync.WaitOne();
						return nUnitForm;
					};
					
			return "NUnitForm".o2Cache(createNUnitForm);			
		}
    }
}