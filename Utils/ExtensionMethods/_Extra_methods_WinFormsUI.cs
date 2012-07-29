// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms; 
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using WeifenLuo.WinFormsUI.Docking;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;

//O2Ref:WeifenLuo.WinFormsUI.Docking

namespace O2.XRules.Database.Utils
{		
	public static class _Extra_methods_WinFormsUI
	{	
		public static WebBrowser add_Document_WebBrowser(this DockPanel dockPanel)
		{			
			return dockPanel.dock_Middle<WebBrowser>("Web Browser");
		}
		
		public static ascx_Simple_Script_Editor add_Document_Script(this DockPanel dockPanel)
		{			
			return dockPanel.dock_Middle("C# REPL Script").add_Script();
		}
        
        public static DockPanel dockPanel(this Control control)
        {
        	return control.dockContent().DockPanel;
        }
        
        public static T dock_Left<T>(this T control) where T : Control
        {
        	WinFormsUI_ExtensionMethods_DockContent.dock_Left(control.dockContent());
        	return control;
        }
        
        public static T dock_Right<T>(this T control) where T : Control
        {
        	WinFormsUI_ExtensionMethods_DockContent.dock_Right(control.dockContent());
        	return control;
        }
        public static T dock_Top<T>(this T control) where T : Control
        {
        	WinFormsUI_ExtensionMethods_DockContent.dock_Top(control.dockContent());
        	return control;
        }
        public static T dock_Bottom<T>(this T control) where T : Control
        {
        	WinFormsUI_ExtensionMethods_DockContent.dock_Bottom(control.dockContent());
        	return control;
        }
        
        public static ascx_Simple_Script_Editor script_Me_in_Dock(this Control control)
        {
        	return control.dockPanel().dock_Bottom("Script").add_Script_Me(control);
        }
                
        
	}
}