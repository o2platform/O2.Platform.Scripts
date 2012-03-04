// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using O2.Kernel;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Kernel.ExtensionMethods;
using O2.API.Visualization.ExtensionMethods;
using Microsoft.Windows.Controls.Ribbon;
using O2.XRules.Database.APIs;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:WPF_Controls_ExtensionMethods.cs
//O2File:WPF_WinFormIntegration_ExtensionMethods.cs
//O2File:ElementHost_ExtensionMethods.cs
//O2File:API_InputSimulator.cs
//O2Ref:RibbonControlsLibrary.dll
//O2Ref:Microsoft.Windows.Shell.dll
//O2File:WPF_Ribbon_ExtensionMethods.cs

namespace O2.XRules.Database.Utils
{

	public class WPF_Ribbon_Test
	{
		public void launchGui()
		{
			var wpfRibbon = O2Gui.open<WPF_Ribbon>("Test - O2 WPF Ribbon",600,400);			
			var tab1 = wpfRibbon.Ribbon.add_RibbonTab("tab1");
			var group1 = tab1.add_RibbonGroup("group 1");
			//wpfRibbon.buildGui();					
		}
	}
	
	
	public class WPF_Ribbon : System.Windows.Forms.Control
	{
		public Ribbon Ribbon {get;set;}
		public WPF_Ribbon()
		{
			buildGui();
		}
		
		public WPF_Ribbon buildGui() 
		{			
			Ribbon = this.add_Wpf_Ribbon();
    		return this;		
		}
	}
	    
    
}	
    	