// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//Code from the O2_Tool_SearchEngine project

/*using O2.Core.CIR.Ascx.DotNet;
using O2.Core.FileViewers.Ascx;
using O2.Core.FileViewers.Ascx.O2Rules;
using O2.Core.FileViewers.Ascx.tests;*/

using FluentSharp.CoreLib;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.REPL.Controls;
using FluentSharp.REPL.Utils;
using FluentSharp.WinFormUI.Utils;
using FluentSharp.WinForms.Controls;
using O2.ImportExport.OunceLabs;
using O2.Tool.SearchEngine.Ascx;
//O2File:ascx_SearchResults.cs
//O2File:ascx_SearchTargets.cs
//O2File:ascx_TextSearch.cs
//O2File:OunceAvailableEngines.cs

namespace O2.Tool.SearchEngine
{
	public class SearchEngineGui_Test
	{
		public void open()
		{				
			new SearchEngineGui().buildGui();
		}
	}
	
	public class SearchEngineGui
	{
		public static FluentSharp.CoreLib.API.SearchEngine searchEngineAPI { get; set; }        
		
		public SearchEngineGui()
		{
			searchEngineAPI = new FluentSharp.CoreLib.API.SearchEngine();
		}
		
		public void buildGui()
		{				            
	        // this will make files to be opened on the main Document window   
	        HandleO2MessageOnSD.setO2MessageFileEventListener();        
	
	        if (O2AscxGUI.launch("O2 Tool - Search Engine"))
	        // O2Messages.openGUI();
	        {
	            var fileMappings = (FileMappings)O2AscxGUI.openAscx(typeof (FileMappings), O2DockState.DockLeft, "File Mappings");
	
	            //fileMappings.loadFilesFromFolder(DI.config.CurrentExecutableDirectory);
	         //   O2Messages.openControlInGUI(typeof (ascx_TextSearch), O2DockState.Document, "Text Search");

	            O2AscxGUI.openAscx(typeof(ascx_SearchTargets), O2DockState.DockRight, "Search Targets");                               
	            O2AscxGUI.openAscx(typeof (ascx_SearchResults), O2DockState.DockTop, "Search Results");

	            O2AscxGUI.addControlToMenu(typeof(ascx_FunctionsViewer), O2DockState.Document, "Signatures Viewer");

	            OunceAvailableEngines.addAvailableEnginesToControl(typeof (ascx_FindingsViewer));
	            //ascx_FindingsViewer.o2AssessmentLoadEngines.Add(new O2AssessmentLoad_OunceV6());
	            O2AscxGUI.addControlToMenu(typeof(ascx_FindingsViewer), O2DockState.Document, "Findings Viewer");
	            O2AscxGUI.addControlToMenu(typeof(SourceCodeEditor), O2DockState.Document, "Source Code Editor");

/*	            O2AscxGUI.addControlToMenu(typeof (ascx_TilesDefinition_xml));
	            O2AscxGUI.addControlToMenu(typeof(ascx_J2EE_web_xml));
	            O2AscxGUI.addControlToMenu(typeof (ascx_Validation_xml));
	            O2AscxGUI.addControlToMenu(typeof (ascx_Struts_config_xml));
	            O2AscxGUI.addControlToMenu(typeof (ascx_StrutsMappings_ManualMapping));
	            O2AscxGUI.addControlToMenu(typeof (ascx_DotNet_Dependencies));
                            
	
	            O2AscxGUI.addControlToMenu(typeof(ascx_O2Rules_Struts),O2DockState.Document, "O2 Rules Struts");
*/
			}
		}
	}
}