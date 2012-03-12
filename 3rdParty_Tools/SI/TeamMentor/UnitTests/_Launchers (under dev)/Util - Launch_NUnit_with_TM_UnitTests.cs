// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (<a href="http://www.apache.org/licenses/LICENSE-2.0">http://www.apache.org/licenses/LICENSE-2.0</a>)
using System;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;  
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.Ascx.MainGUI; 
using O2.Views.ASCX.classes.MainGUI;
using O2.XRules.Database.APIs;
//O2File:API_NUnit_Gui.cs

namespace O2.TeamMentor.UnitTests
{
    public class Launcher
    {   
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
       
        public static void Main()
        {
            //if (Control.ModifierKeys == Keys.Shift)
                showLogViewer();                                    
            "Current AppDomain: {0}".info(AppDomain.CurrentDomain.BaseDirectory);            
            
            Console.WriteLine("...Launching NUnit Gui");
            launchNUnitGui();
			//var scriptToExecute = @"Util - Execute O2ified tests on NUnitGui.h2".local();
			//"Executing Script: {0}".info(scriptToExecute);
			//scriptToExecute.compile_H2Script().executeFirstMethod();
            Console.WriteLine("all done");
        }   
       		
        public static ascx_LogViewer showLogViewer()
        {
            return O2Gui.open<ascx_LogViewer>();                       
        }           
        
        public static bool launchNUnitGui()
		{
			var nUnitGui = new API_NUnit_Gui(); 
			
			var file = @"C:\O2\_tempDir\12-12-2011\tmp7579.tmp.cs";
			return nUnitGui.compileAndOpen(file);	
		}
    }
}