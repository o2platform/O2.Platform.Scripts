using System;
using System.Windows.Forms;
using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.WinForms;


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
		public String			GroupBoxText	{ get; set; }
		public CheckBox			AutoResize		;//{ get; set; }
		
		
		public IntPtr			HijackedHandle	{ get; set; }
		public IntPtr			HijackedParent	{ get; set; }		
		public Process 			HijackedProcess	{ get; set; }	
		
		public WinAPI.RECT		HijackedHandleRECT;	
		
		//public PictureBox 		PictureBox		;//{ get; set; }
		//public Label 			ClassName		;//{ get; set; }
				
		public Win32_Handle_Hijack()
		{
			GroupBoxText = "(click here to hide tool bar)";
			this.width(400);
			this.height(400);
			buildGui();			
		}
		
		public Win32_Handle_Hijack setTarget(IntPtr handle)
		{
		 	TargetHandle.set_Text(handle.str());
		 	return this;
		 	//ClassName.set_Text(handle.className());
		 	//PictureBox.show(handle.window_ScreenShot());			
		}
		
		public Win32_Handle_Hijack updateParentValue()
		{
			var target = TargetHandle.get_Text().toInt().intPtr();
			var targetParent = target.parent().str();
			ParentHandle.set_Text(targetParent);			
			return this;
		}
		
		public Win32_Handle_Hijack setTargetValueToItsParent()
		{
			if(ParentHandle.get_Text().isInt())
				if (ParentHandle.get_Text().toInt() > 0)
					TargetHandle.set_Text(ParentHandle.get_Text());
			return this;
		}
		
		public Win32_Handle_Hijack restore()
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
			return this;
		}
					
		public Win32_Handle_Hijack hijack()
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
			return this;
		}			
		public Win32_Handle_Hijack screenShot()
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
			return this;
		}
		
		public void adjustHandleSizeToTargetWindow()
		{
			if (AutoResize.@checked())
				HijackedHandle.window_Move(0,0, HijackedWindow.width(), HijackedWindow.height());
		}
		public Win32_Handle_Hijack startProcessAndHijackMainWindow(string processToStart, string processParams)
		{
			return hijackProcessMainWindow(processToStart.startProcess(processParams));
		}
		
		public Win32_Handle_Hijack hijackProcessMainWindow(Process process)
		{	
		    return hijackProcessWindow(process, (mainWindowHandle)=>mainWindowHandle);
		}
		
		public Win32_Handle_Hijack hijackProcessWindow(Process process, Func<IntPtr,IntPtr> windowToHijack)
		{	
		    var mainHandle = process.waitFor_MainWindowHandle().MainWindowHandle;
		    O2Thread.mtaThread(
		    	()=>{
		    			var targetWindow = windowToHijack(mainHandle);
					    setTarget(targetWindow);
					    hijack();	
					    adjustHandleSizeToTargetWindow();
					    HijackedProcess = process;
					});
		    return this;
		}
		
		public Win32_Handle_Hijack buildGui()
		{									
			TopPanel = this.add_Panel();	
			createToolStrip();
			
			HijackedWindow = TopPanel.add_GroupBox(GroupBoxText).add_Panel();		
											
			var groupBox = HijackedWindow.parent();;
			GroupBoxText = groupBox.get_Text();
			
			groupBox.DoubleClick+=(sender,e)=> toolStrip_HideShow();				
			
			//do this on a seprate thread because the parentForm will be null at this stage (since this is a Control)
			O2Thread.mtaThread(
				()=>{
						1000.sleep();
						this.parentForm().onClosed(
							()=>{									
									restore();
								});
					});
			return this;	
		}
		
		public Win32_Handle_Hijack toolStrip_HideShow()
		{			
			var groupBox = HijackedWindow.parent();;
			var splitcontainer = groupBox.splitContainer();
			
			var collapsed = splitcontainer.Panel1Collapsed;
			if (collapsed)
			{
				splitcontainer.panel1Collapsed(false);					
				groupBox.set_Text(GroupBoxText);		
			}
			else
			{
				splitcontainer.panel1Collapsed(true);		
				groupBox.set_Text(".");			
			}
			return this;
		}
		public void createToolStrip()
		{
			ToolStrip = TopPanel.insert_Above_ToolStrip();
	 
			WindowFinder  = ToolStrip.insert_Left(30).add_WindowFinder(); 			
			
			WindowFinder.Window_Changed = (intPtr)=>setTarget(intPtr); 
			 
			ToolStrip.add_Button("Hijack","btExecuteOnExternalEngine_Image".formImage(), ()=> hijack()) 
					 .add_Button("Restore","edit_undo".formImage(),()=> restore())
					 .add_Button("Screenshot","camera_photo".formImage(),()=> screenShot())
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
			return hostPanel.add_Handle_HijackGui(true);
		}
		public static Win32_Handle_Hijack add_Handle_HijackGui(this Control hostPanel, bool showToolStrip)		
		{						
			var handleHijack = hostPanel.add_Control<Win32_Handle_Hijack>();;
			if (!showToolStrip)
				handleHijack.toolStrip_HideShow();
			return handleHijack;			
		}
	}
}