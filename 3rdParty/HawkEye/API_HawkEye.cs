using System;
using System.Windows.Forms;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:WindowFinder.cs

namespace O2.XRules.Database.APIs
{
	public class API_HawkEye
	{
		public Panel launch()
		{
			return new API_HawkEye().openControlFinder();
		}
	}
	
	public static class API_HawkEye_ExtensionMethods
	{
		public static Panel openControlFinder(this API_HawkEye hawkEye)
		{
			return "Util - Find WinForms Control and REPL it".popupWindow(300,300)
															  .openControlFinder();
		}
		public static Panel openControlFinder(this string title)
		{
			return title.openControlFinder();
		}
		
		public static T openControlFinder<T>(this T topPanel) where T: Control
		{			
			var activeControl = topPanel.title("Active Control").add_PropertyGrid().helpVisible(false);
			   
			var hawkeye = topPanel.insert_Above(30).add_Control<WindowFinder>().width(30).fill(false);
			var currentHandle =  topPanel.insert_Above(20).add_Label("Current Handle").top(2).append_TextBox("").align_Right(topPanel);
			Action<Control> setTarget =   
				(control)=> { 
								activeControl.parent<GroupBox>().set_Text("Active Control: {0}".format(control.typeName()));
								activeControl.show(control);								
							};
			hawkeye.ActiveWindowChanged += 
				(sender, e)=>{
								var control = hawkeye.SelectedControl;								
								setTarget(control);
							 }; 
			hawkeye.append_Link("Open ScriptMe for this control" ,()=> activeControl.SelectedObject.script_Me() )
				   .top(9).leftAdd(40);				 
			 
			setTarget(topPanel); 
			return topPanel;
		}
	}
}


