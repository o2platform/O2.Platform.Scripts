// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.Ascx.MainGUI;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

using TransparencyMenu;
using win32 = TransparencyMenu.Win32; 
//O2File:GenericWindow.cs
//O2File:WilsonGlobalSystemHooks\Win32.cs
//O2File:WilsonGlobalSystemHooks\GlobalHooks.cs
//O2File:WilsonGlobalSystemHooks\Form1.cs
//O2File:WilsonGlobalSystemHooks\Window.cs

namespace O2.XRules.Database.APIs
{
   	public class API_InjectMenu_AnotherProcess
	{	
		public Process InjectedProcess { get; set; }
		public string ProcessToInject { get; set; }
		public string InjectedProcess_WindowTitle { get;set; }
		public string MenuTitle { get; set; }
		public Dictionary<int, string> MenuCommands { get; set; }		
		public Action<int, string> CommandSelected { get; set;}
		
		public int MaxWaitForWaitForInputIdle {get;set;}
		public int ExtraSleepAfterWaitForInputIdle {get;set;}		
		
		public API_InjectMenu_AnotherProcess()
		{
			MenuCommands = new Dictionary<int, string>();
			MaxWaitForWaitForInputIdle = 4000;
			ExtraSleepAfterWaitForInputIdle = 0;
		}
		
		public API_InjectMenu_AnotherProcess buildGui()
		{
			"******** API_InjectMenu_AnotherProcess ********:{0}".info(ProcessToInject);
			"Injecting O2 Menu into new Process :{0}".info(ProcessToInject);
			
			Action showProcessToHook = null;			
			
			Action<int, IntPtr> handleCommand = 
				(command, handle)=> {
										if (handle == this.InjectedProcess.MainWindowHandle)
										{
											"Handing command: {0} from handle {1}".info(command, handle);
											CommandSelected(command, MenuCommands[command]);
										}										
									};
			Action<IntPtr,IntPtr , IntPtr, IntPtr> GetMsg_GetMsg = 
				( handle,  message,  wParam,  lParam) =>   
					{		  			
						var messageId = message.ToInt32(); 
						if (messageId == win32.WM_COMMAND)
						{
							var menuCommand = (wParam.ToInt32() & 0x0000FFFF);
							handleCommand(menuCommand, handle);		  	
							//"(GetMsg_GetMsg) WM_COMMAND".info();										
						}
					};
			 
					
			//Create window to host messages received
			var popupWindow = O2Gui.open<Panel>("TM - Notepad++ Controller", 400,400);//.popupWindow();
			var genericWindow = new GenericWindow();   
			genericWindow.AssignHandle(popupWindow.Handle); 
			var _GlobalHooks = new GlobalHooks(popupWindow.Handle);
			_GlobalHooks.GetMsg.GetMsg += new TransparencyMenu.GlobalHooks.WndProcEventHandler(GetMsg_GetMsg);// genericWindow.GetMsg_GetMsg); //
			genericWindow.WindowsMessage = _GlobalHooks.ProcessWindowMessage; 
			
			//Build GUI
			popupWindow.add_Control<ascx_LogViewer>();//insert_LogViewer();
			
			Action<Process> addMenuItem =  
				(process)=>{						
						
						var _Handle = process.MainWindowHandle;//Processes.getProcessCalled("notepad").MainWindowHandle;
					
						"Main Window HANDLE: {0}".info(_Handle);
						if (_Handle==IntPtr.Zero)
						{
							"Could not get main window handle from process.MainWindowHandle (stopping addMenuItem sequence)".error();
							return;
						}
						//insert normal menu
						
						var windowMenuHandle = win32.GetMenu(_Handle);
						var numberOfMenuItems = win32.GetMenuItemCount(windowMenuHandle);
						var menuHandle  = win32.CreateMenu(); 
						
						foreach(var menuCommand in MenuCommands)						
							win32.InsertMenu(menuHandle, -1, win32.MF_BYPOSITION, menuCommand.Key, menuCommand.Value);
							
						//win32.InsertMenu(menuHandle, -1, win32.MF_BYPOSITION, 02001, "Send Command 02004");
						if (win32.InsertMenu(windowMenuHandle, numberOfMenuItems - 1, win32.MF_BYPOSITION | win32.MF_POPUP, menuHandle, this.MenuTitle))
							"O2's top level menu added".info();
						else	
							"Failed to add O2 menu".error();			 					
						
					};						
			
			Action hookSelectedProcess =
				()=>{
						GlobalHooks.HookIndex = InjectedProcess.Threads[0].Id; 
						if (GlobalHooks.HookIndex ==1)
						{
							"GlobalHooks.HookIndex was set to 0, stop execution since global hooks are not supported".error();
							return;
						}
						"Process Thread id: {0}".info(GlobalHooks.HookIndex);
						popupWindow.invokeOnThread(()=> _GlobalHooks.GetMsg.Start() );
					};
			
			Action unHookSelectedProcess =
				()=>{
						"unHookSelectedProcess".info();
						 _GlobalHooks.GetMsg.Stop();
					};	
					
			
			Action startProcessToInject =
				()=>{
						InjectedProcess = Processes.startProcess(ProcessToInject);						
						"waiting for inputIdle: {0}s".info(MaxWaitForWaitForInputIdle/1000);
						if (InjectedProcess.WaitForInputIdle(MaxWaitForWaitForInputIdle).isFalse())
							"waited 4s for input idle so continuing...".debug();
						"after inputIdle".info();						
						if (InjectedProcess.MainWindowHandle == IntPtr.Zero)
							for(var i=0 ; i < 10 ; i++)
							{
								"waiting for process.MainWindowHandle".info();
								InjectedProcess = Processes.getProcess(InjectedProcess.Id); //help to make sure the InjectedProcess.MainWindowHandle is set
								if (InjectedProcess.MainWindowHandle != IntPtr.Zero)
									break;
								this.sleep(5000);	
							}						
						if (ExtraSleepAfterWaitForInputIdle > 0)
							this.sleep(MaxWaitForWaitForInputIdle);
						win32.SetWindowText(InjectedProcess.MainWindowHandle,InjectedProcess_WindowTitle);
					};
					
			/*popupWindow.insert_Above(40,"Actions").add_Link("Add Menu item", 0,0,()=> addMenuItem(selectedProcess))
			/			.append_Link("hook selected Process", 
							()=> {	
									hookSelectedProcess();
								  })					  
						.append_Link("Stop Hooks", ()=> unHookSelectedProcess())
						.append_Link("start notepad++", ()=> { 
																startNotepadPP();		
																//currentFilter_TextBox.set_Text("notepad");
																//showProcessToHook();
														    });
			*/														    
			
			
			startProcessToInject();
			addMenuItem(InjectedProcess);
			hookSelectedProcess(); 
			
			
			
			
			O2Thread.mtaThread(
				()=>{
						"Waiting hooked process to exit".info();								
						InjectedProcess.WaitForExit();								
						"selectedProcess.Exited".info();
						popupWindow.parentForm().close();
					});

			return this;
		}	
	}	
}
