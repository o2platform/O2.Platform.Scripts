using System;
using System.Linq;
using System.Text;
using System.Drawing;  
using System.Windows.Forms;
using System.Drawing.Imaging; 
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.DotNet;
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
	
	public class Win32_Handle_Hijack : Control
	{
		public WindowFinder 	WindowFinder	{ get; set; }
		public ToolStrip		ToolStrip		{ get; set; }
		public Panel 			TopPanel 		{ get; set; }		
		public ToolStripTextBox TargetHandle	{ get; set; }
		public ToolStripTextBox ParentHandle	{ get; set; }
		public Panel 			HijackedWindow	{ get; set; }	
		public CheckBox			AutoResize		;//{ get; set; }
		
		public IntPtr			HijackedHandle	{ get; set; }
		public IntPtr			HijackedParent	{ get; set; }		
		
		public WinAPI.RECT		HijackedHandleRECT;	
		
		//public PictureBox 		PictureBox		;//{ get; set; }
		//public Label 			ClassName		;//{ get; set; }
		
		public Win32_Handle_Hijack()
		{
			this.width(400);
			this.height(400);
			buildGui();
		}
		
		public void setTarget(IntPtr handle)
		{
		 	TargetHandle.set_Text(handle.str());
		 	//ClassName.set_Text(handle.className());
		 	//PictureBox.show(handle.window_ScreenShot());			
		}
		
		public void updateParentValue()
		{
			var target = TargetHandle.get_Text().toInt().intPtr();
			var targetParent = target.parent().str();
			ParentHandle.set_Text(targetParent);			
		}
		
		public void setTargetValueToItsParent()
		{
			if(ParentHandle.get_Text().isInt())
				if (ParentHandle.get_Text().toInt() > 0)
					TargetHandle.set_Text(ParentHandle.get_Text());
		}
		
		public void restore()
		{
			if (HijackedHandle != IntPtr.Zero)
			{
				"restoring {0} to parent {1}".info(HijackedHandle, HijackedParent);
				HijackedHandle.setParent(HijackedParent);
				if(HijackedHandleRECT.Width >0 && HijackedHandleRECT.Width > 0)
				{
					HijackedHandle.window_Move(HijackedHandleRECT.Left,HijackedHandleRECT.Top, HijackedHandleRECT.Width,HijackedHandleRECT.Height);
					HijackedHandleRECT = default(WinAPI.RECT);
				}
				HijackedParent.window_Redraw();
				HijackedHandle.window_Redraw();
				HijackedHandle = IntPtr.Zero;
			}
		}
					
		public void hijack()
		{
			restore();
			var handle = TargetHandle.get_Text().toInt().intPtr();						
			var newParent = HijackedWindow.clear().handle();
			"Hijacking {0} into window {1}".info(handle, newParent);			
			HijackedHandle = handle;
			
			WinAPI.GetWindowRect(handle, ref HijackedHandleRECT);
			HijackedParent = ParentHandle.get_Text().toInt().intPtr();
			handle.setParent(newParent);						
			
			adjustHandleSizeToTargetWindow();
		}			
		public void screenShot()
		{
			restore();
			try
			{
				var handle = TargetHandle.get_Text().toInt().intPtr();						
				var bitmap = handle.window_ScreenShot();
				HijackedWindow.clear().add_PictureBox().layout_Zoom().show(bitmap);
			}
			catch(Exception ex)
			{
				ex.log();
			}
		}
		
		public void adjustHandleSizeToTargetWindow()
		{
			if (AutoResize.@checked())
				HijackedHandle.window_Move(0,0, HijackedWindow.width(), HijackedWindow.height());
		}
		public Process startProcessAndHijackMainWindow(string processToStart, string processParams)
		{
			return hijackProcessMainWindow(processToStart.startProcess(processParams));
		}
		public Process hijackProcessMainWindow(Process process)
		{	
		    var mainHandle = process.waitFor_MainWindowHandle().MainWindowHandle;
		    O2Thread.mtaThread(
		    	()=>{
					    setTarget(mainHandle);
					    hijack();	
					    adjustHandleSizeToTargetWindow();
					});
		    return process;
		}
		
		public Win32_Handle_Hijack buildGui()
		{									
			TopPanel = this.add_Panel();	
			createToolStrip();
			/*			
			TopPanel.insert_Above(35).splitContainerFixed()
//						.add_WindowFinder(ref WindowFinder) 
//.add_WindowFinder()
						//.add_Label("Handle:").top(10).append_TextBox(ref TargetHandle).width(60)
						//.append_Label("Parent:").top(10).append_TextBox(ref ParentHandle).width(60)
						.append_Link("Hijack", ()=> hijack()).top(10)
						.append_Link("Restore", ()=> restore()) 
						.append_Link("Screenshot", ()=> screenShot()) 
						.append_CheckBox("Auto Size", (value)=> AutoResize = value) 
						.append_PictureBox(ref PictureBox)
						.append_TextBox(ref Test).set_Text("Hijack me").top(10) 
						.append_Label(ref ClassName).topAdd(2);

			*/							
			HijackedWindow = TopPanel.add_GroupBox("(click here to hide tool bar)").add_Panel();		
					
			//TargetHandle.onTextChange((value)=> ParentHandle.set_Text(value.toInt().intPtr().parent().str())); 
			//TargetHandle.TextChanged+= (sender,e)=> updateParentValue();
			//WindowFinder.Window_Changed = setTarget; 
				
//			setTarget(test.handle()); 
			
			//PictureBox.layout_Zoom();					  
			this.onClosed(
				()=>{
						"On Closed".info();
						restore();
					});
			var groupBox = HijackedWindow.parent();;
			var originalText = groupBox.get_Text();
			var splitcontainer = groupBox.splitContainer();
			
			groupBox.DoubleClick+=(sender,e)=>
				{		
					var collapsed = splitcontainer.Panel1Collapsed;
					if (collapsed)
					{
						splitcontainer.panel1Collapsed(false);					
						groupBox.set_Text(originalText);		
					}
					else
					{
						splitcontainer.panel1Collapsed(true);		
						groupBox.set_Text(".");			
					}
				};	
				
			return this;	
		}
		
		public void createToolStrip()
		{
			ToolStrip = TopPanel.insert_Above_ToolStrip();
	 
			WindowFinder  = ToolStrip.insert_Left(30).add_WindowFinder(); 			
			
			WindowFinder.Window_Changed = setTarget; 
			 
			ToolStrip.add_Button("Hijack","btExecuteOnExternalEngine_Image".formImage(), hijack) 
					 .add_Button("Restore","edit_undo".formImage(),restore)
					 .add_Button("Screenshot","camera_photo".formImage(),screenShot)				 
					 .toolStrip()
			 		 .add_CheckBox("Size", ref AutoResize);
			  
			TargetHandle = ToolStrip.add_Label("Handle").add_TextBox("");
			ParentHandle = ToolStrip.add_Label("Parent").add_TextBox("").width(40);
			TargetHandle.width(40); // didn't work if set above
			
			TargetHandle.TextChanged += (sender,e)=> updateParentValue();
			ParentHandle.DoubleClick += (sender,e)=> setTargetValueToItsParent();
			
			addExamples();


			ToolStrip.add_DropDown("REPL")
					 .add_Button("Hijacked Handle","ViewCode".formImage(),()=> HijackedHandle.script_Me("handle").set_Code("return handle;\n\n//"+"O2File:Api_WinApi.cs"))
					 .add_Button("Hijack UI","ViewCode".formImage(),()=> this.script_Me("hijackUI"))
					 .add_Button("Parent Form","ViewCode".formImage(),()=> TopPanel.parentForm().script_Me("form"));
			
			AutoResize.check();
			TopPanel.Resize+= (sender,e)=> adjustHandleSizeToTargetWindow();
		}
		
		public void addExamples()
		{
			var examples = ToolStrip.add_DropDown("Hijack Processes");
			
			examples.add_Button("---- (start new process and Hijack) ----", ()=>{})
					.add_Button("Notepad",()=> startProcessAndHijackMainWindow("notepad",""))
			        .add_Button("Calculator",()=> startProcessAndHijackMainWindow("calc",""))
			        .add_Button("Cmd",()=> startProcessAndHijackMainWindow("cmd",""));
			        
			examples.add_Button("---- (Hijack existing processes) --", ()=>{});
						
			//     .add_Button("Chrome (Web Browser)",()=> startProcessAndHijackMainWindow("chrome","--chrome-frame"));			     			
			var targetProcesses = Processes.getProcesses().where((process)=>process.MainWindowHandle != IntPtr.Zero);
			foreach(var targetProcess in targetProcesses)
			{
				var process = targetProcess;		// need to pin it
				examples.add_Button(targetProcess.MainWindowTitle, ()=> hijackProcessMainWindow(process));
			}
		}
	}
	
	public static class API_Win32_Handle_Hijack_ExtensionMethods
	{	
					
		public static Win32_Handle_Hijack add_Handle_HijackGui(this Control hostPanel)
		{											
			return hostPanel.add_Control<Win32_Handle_Hijack>();;
		}
	}
}