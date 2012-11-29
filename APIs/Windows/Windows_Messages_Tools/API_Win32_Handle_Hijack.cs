using System;
using System.Linq;
using System.Text;
using System.Drawing;  
using System.Windows.Forms;
using System.Drawing.Imaging; 
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using O2.DotNetWrappers.ExtensionMethods;


//O2File:WindowFinder.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_Win32_Handle_Hijack_test
	{
		public void test()
		{
			"API Win32 Handle Hijack test".popupWindow().add_Handle_HijackGui();
		}		
	} 
	
	public class API_Win32_Handle_Hijack
	{
		
	}
	
	public static class API_Win32_Handle_Hijack_ExtensionMethods
	{
	//var hostPanel  = panel.clear().add_Panel();
			
		
		public static T add_Handle_HijackGui<T>(this T hostPanel) where T : Control
		{
			WindowFinder windowFinder 	= null;
			TextBox targetHandle  	  	= null;
			TextBox parentHandle 		= null;
			//TextBox test 				= null;
			Panel hijackedWindow 		= null;
			PictureBox 	pictureBox 		= null;
			Label 		className		= null;
			IntPtr		hijackedHandle  = IntPtr.Zero;
			IntPtr		hijackedParent  = IntPtr.Zero;
						
			Action restore = 
				()=>{												
						if (hijackedHandle != IntPtr.Zero)
						{
							"restoring {0} to parent {1}".info(hijackedHandle, hijackedParent);
							hijackedHandle.setParent(hijackedParent);
							hijackedParent.window_Redraw();
							hijackedHandle.window_Redraw();
						}
					};
					
			Action hijack = 
				()=>{
						restore();
						var handle = targetHandle.get_Text().toInt().intPtr();						
						var newParent = hijackedWindow.handle();
						"Hijacking {0} into window {1}".info(handle, newParent);
						hijackedHandle = handle;
						hijackedParent = parentHandle.get_Text().toInt().intPtr();
						handle.setParent(newParent);						
					};					
					
			Action<IntPtr> setTarget = 
				(handle)=>{
							 	targetHandle.set_Text(handle.str());
							 	className.set_Text(handle.className());
							 	pictureBox.show(handle.window_ScreenShot());
								//pictureBox
							};
							
			hostPanel.insert_Above(35).splitContainerFixed()
						.add_WindowFinder(ref windowFinder) 
						.append_Label("Handle:").top(10).append_TextBox(ref targetHandle)
						.append_Label("Parent:").top(10).append_TextBox(ref parentHandle)
						.append_Link("Hijack", ()=> hijack()).top(10)
						.append_Link("Restore", ()=> restore()) 
						.append_PictureBox(ref pictureBox)
//						.append_TextBox(ref test).set_Text("Hijack me").top(10) 
						.append_Label(ref className).topAdd(2);
						
						
			hijackedWindow = hostPanel.add_GroupBox("Hijacked Window/Control").add_Panel();		
			
			targetHandle.onTextChange((text)=> parentHandle.set_Text(text.toInt().intPtr().parent().str())); 
			windowFinder.Window_Changed = setTarget; 
				
//			setTarget(test.handle()); 
			
			pictureBox.layout_Zoom();
			hostPanel.onClosed(
				()=>{
						"On Closed".info();
						restore();
					});
			return hostPanel;
		}
	}
}