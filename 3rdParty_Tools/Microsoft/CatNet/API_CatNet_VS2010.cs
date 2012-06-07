// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using Microsoft.ACESec.CATNet.UI;
using EnvDTE;

//O2File:API_VisualStudio_2010.cs
//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll
//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.UI.VSAddIn.dll

namespace O2.XRules.Database.APIs
{
	public class API_CatNet_VS2010 : API_VisualStudio_2010
	{		
		public VsConnect 		VsConnect 	  	{ get; set; }
		public Window 			CatNetWindow 	{ get; set; }
		public SummaryView 		SummaryView  	{ get; set; }
		public ListView			Summary			{ get; set; }
		public ToolStrip		Actions			{ get; set; }			
		
		
		public API_CatNet_VS2010()
		{			
			hookCatNet();
		}		
		
		public API_CatNet_VS2010 hookCatNet()
		{
			"Starting hookCatNet".info();
			try
			{
				CatNetWindow = this.window("CAT.NET Code Analysis");
				if (CatNetWindow.isNull())
					this.executeCommand("Microsoft.ACESec.CATNet.UI.VsConnect.Start", "");
				CatNetWindow = this.window_waitFor("CAT.NET Code Analysis");	
				if (CatNetWindow.isNull())			
					"Could not get window called 'CAT.NET Code Analysis'".error();
				else
				{			
					CatNetWindow.Visible = true;
					SummaryView = (SummaryView)CatNetWindow.Object;
					Summary 	= (ListView) SummaryView.field("_lvSummary");
					Actions		= (ToolStrip)SummaryView.field("_tsActions");
					VsConnect 	= SummaryView.Controller;
					if (VsConnect.notNull())
						"Hooked CatNet VsConnect object ok".info();			
						
					//"Adding Extra Gui Features".info();
					//this.add_Feature_OpenReportOnFileDrop();
				}
				return this;
			}
			catch(Exception ex)
			{
				ex.log("[API_CatNet_VS2012] hookCatNet", true);
				return null;
			}			
		}

    }


    public static class API_CatNet_VS_2010_ExtensionMethods_New_Gui_Features
    {
    	public static API_CatNet_VS2010 add_Feature_OpenReportOnFileDrop(this API_CatNet_VS2010 vsCatNet)
    	{
    		"[API_CatNet_VS2012] add_Feature_OpenReportOnFileDrop".debug();
    		vsCatNet.Summary.onDrop(
				(file)=>{
					"[CatNet] a file was droppped: {0}".info(file);
					vsCatNet.openReport(file);
				});    
			return vsCatNet;
    	}
    }

			
    public static class API_CatNet_VS_2012_ExtensionMethods
    {
    	public static API_CatNet_VS2010 openReport(this API_CatNet_VS2010 catNet, string pathToReport)
    	{
    		catNet.VsConnect.OpenReport(pathToReport);
    		return catNet;
    	}
    	 
    }
}