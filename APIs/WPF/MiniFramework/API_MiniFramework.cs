// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Text;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.XRules.Database.Utils;

using MiniFramework.Windows;
using MiniFramework.Windows.Controls;
using MiniFramework.Resources;

//O2File:Xaml_ExtensionMethods.cs
//O2Ref:MiniFramework.Windows.dll 
//O2Ref:MiniFramework.Core.dll 
//O2Ref:MiniFramework.Resources.dll 
//O2Ref:WindowsFormsIntegration.dll

namespace O2.XRules.Database.APIs
{
    public class API_MiniFramework 
    {        	
    	//return API_MiniFramework.askQuestion("title","subTitle"); 
    	
    	public static string askQuestion(string title, string subTitle)
    	{
    		var sync = new AutoResetEvent(false);
    		var result = "";
    		O2Thread.staThread(
				()=>{
						var inputDialog = new InputDialog();
			    		inputDialog.InstructionText = title;			    
			    		inputDialog.Text = subTitle;
			    		result = inputDialog.GetText();
			    		sync.Set();
					});
			sync.WaitOne();
			return result;
    	}
    	//askTask("Message", "DetailsText", , "InstructionText", "Footer")
    	public static MessageBoxResult askTask(string message, string detailsText, string instructionText, string footerText)
    	{
    		var sync = new AutoResetEvent(false);
    		MessageBoxResult result = MessageBoxResult.None;
    		O2Thread.staThread(
				()=>{
		    			var taskDialog = new TaskDialog();
					    taskDialog.DetailsText = detailsText;
					    taskDialog.FooterImage = StockIcons.Information.SmallBitmapImage;
					    taskDialog.FooterText = footerText;
					    taskDialog.Image = StockIcons.Information.LargeBitmapImage;
					    taskDialog.InstructionText = instructionText;					    
					    taskDialog.Text = message;
					    //TaskDialog dialog = dialog2;
					    taskDialog.AddButton("_Accept", MessageBoxResult.Yes, StockIcons.Shield.SmallBitmapImage);
					    taskDialog.DefaultButton = taskDialog.AddButton("_Reject", MessageBoxResult.No);
					    result = taskDialog.ShowDialog();
					    sync.Set();
    				});
			sync.WaitOne();
			return result;
    	}
    	
    	public static WhoisControl whoIs()
    	{
    		return O2Gui.open<Panel>("WhoIs Query",500,300)
    				 	.add_WPF_Control<WhoisControl>();
    	}
    }
    
    
    
}
