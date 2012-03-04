// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX;
using O2.Views.ASCX.classes.MainGUI;
using WindowsInput;
using WindowsInput.Native;
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

namespace O2.XRules.Database.APIs
{
    public class CmdExeGui : Control 
    {         	
    	public CmdExeApi CmdExeApi { get; set; }
    	public TextBox consoleOutTextBox { get; set; } 
    	public TextBox consoleInTextBox { get; set; }
    	
    	//public IntPtr GuiHandle { get; set;}
    	
    	public string cmdProcessName = "cmd.exe";
    	public string cmdStartArguments = "";
    	
    	public static void launchControl()
    	{
    		O2Gui.open<CmdExeGui>("Cmd Exe GUI", 400,300); 
    	}
    	
    	public CmdExeGui()
    	{
    		CmdExeApi = new CmdExeApi(cmdProcessName, cmdStartArguments , dataReceived);	
    		//CmdExeApi.MinimizeHostWindow = tr;
    		buildGui();
    		CmdExeApi.start();   
    		CmdExeApi.hideHost();
    		//wait(500);
    		selectThis();
    		consoleInTextBox.focus();
    	}
    	
    	public void buildGui()
    	{
    		this.Width = 400;
    		this.Height = 300;
    		// add controls
    		var groupdBoxes = this.add_1x1("Help Commands","Cmd.Exe GUI",false, 100);
    		consoleOutTextBox = groupdBoxes[1].add_TextBox(true);    		
    		consoleInTextBox = consoleOutTextBox.insert_Above<TextBox>(20);    		
    		var flowLayoutPanel = groupdBoxes[0].add<FlowLayoutPanel>();
    		
    		// setup events    		
    		consoleInTextBox.onKeyPress(Keys.Enter,(text)=>cmd(text));
    		
    		// finetune layout
    		consoleOutTextBox.multiLine().scrollBars();
    		
    		// add helper comamnds
    		
    		flowLayoutPanel.add_Button("dir").onClick(() => cmd("dir"));
    		flowLayoutPanel.add_Button("cd \\").onClick(() => cmd("cd \\"));
    		flowLayoutPanel.add_Button("ipconfig").onClick(() => cmd("ipconfig"));
    		flowLayoutPanel.add_Button("net users").onClick(() => cmd("net users"));
    		flowLayoutPanel.add_Button("ping google").onClick(() => cmd("ping www.google.com"));
    		flowLayoutPanel.add_Button("[show cmd.Exe]").onClick(() => CmdExeApi.showHost());
    		flowLayoutPanel.add_Button("[stop cmd.Exe]").onClick(() => stop());
    	}    	    	
    	
    	public void dataReceived(string text)
    	{
    		consoleOutTextBox.append_Line(text);
    		//text.debug();
    	}
    	
    	public CmdExeGui stop()
    	{
    		CmdExeApi.cmd("exit");
    		CmdExeApi.hostStop();
    		return this;
    	}
    	    	
    	public CmdExeGui wait(int miliseconds)
    	{
    		this.sleep(miliseconds);
    		return this;
    	}
    	
    	public CmdExeGui cmd(string cmdToExecute)
    	{    		
    		cmd(cmdToExecute,-1);
    		return this;
    	}
    	    	
    	public CmdExeGui cmd(string cmdToExecute, int postExecutionDelay)
    	{
    		//GuiHandle = CmdExeApi.currentWindow();
    		CmdExeApi.hostCmd(cmdToExecute,postExecutionDelay);    		    		
    		selectThis();
    		return this;
    	}    	    	    	    	    
    	
    	// restore the focus on the current window (or every keypress from now on will be sent to the HostControl 
    	// this method should be called everytime we send a command to the CmdExeApi
    	public CmdExeGui selectThis()
    	{
    		CmdExeApi.selectWindow(this.Handle);
    		return this;
    	}
    }
    
    public class CmdExeApi
    {
    	public InputSimulator Input_Simulator {get; set;}
    	public string PipeName { get; set;}
    	public string FullPipeName { get; set;}
    	public string ProcessToStart { get; set;}
    	public string Arguments { get; set;}
    	public Action<string> ConsoleOut { get; set;}
    	public Process HostCmdExeProcess { get; set;}
    	public Process ChildProcess { get; set;}    	
    	public IntPtr HostHandle { get; set;}
    	public IntPtr UserCurrentWindowHandle { get; set;}    	
    	public bool ChildProcessStarted { get; set;}
    	public bool HostProcessStarted { get; set;}
    	public bool HostProcessEnded { get; set;}
    	public bool MinimizeHostWindow { get; set; }
    	
    	public CmdExeApi()
    	{
    		Input_Simulator= new InputSimulator();
    		PipeName = 64000.random().str();	// give it a random name
    		FullPipeName = @"\\.\pipe\"+ PipeName;
    		ProcessToStart = "cmd";				// default
    		Arguments = "";
    		MinimizeHostWindow = true;
    		ConsoleOut = (text) => text.info();
    	}
    	
    	public CmdExeApi(string processToStart, string arguments, Action<string> consoleOut) : this()
    	{
    		ProcessToStart = processToStart;
    		Arguments = arguments;
    		ConsoleOut = consoleOut;
    	}    	    	
    	
    	public CmdExeApi start()
    	{
    		try
    		{
	    		PipeName.namePipeServer(ConsoleOut); // setup namedpipe
	    		saveUsersWindow();    	
	    		this.sleep(1000);
	    		// host process
	    		HostProcessStarted = true;    		
	    		HostCmdExeProcess = Processes.startProcess("cmd.exe","",MinimizeHostWindow);
	    		HostCmdExeProcess.Exited += (sender,e)=> HostProcessEnded = true;
				findHostProcessHandle();    					
	    		// child process
	    		// the ReadTimeout can be reset on next recompile    		     		
	    		// see http://www.microsoft.com/resources/documentation/windows/xp/all/proddocs/en-us/redirection.mspx?mfr=true for details on cmd.exe redirection
	    		var CreateChildProcessWithRedirection = "\"{0}\" {1} > \\\\.\\pipe\\{2} 2>&1".format(ProcessToStart,Arguments , PipeName);
	    		
	    		restoreUsersWindow();
	    		
				hostCmd(CreateChildProcessWithRedirection);   		
	    		//ChildProcess = Processes.startProcess(ProcessToStart,ChildProcessAgumentsWithRedirection);
	    		
	    		//this.sleep(1000);    		
	    	}
	    	catch(Exception ex)
	    	{
	    		ex.log("in CmdExeApi.start");
	    	}
    		return this;
    	}
    	
    	// using reflection to use an internal .Net method to find the MainWindowHandle
    	public CmdExeApi findHostProcessHandle()
    	{
    		HostHandle = HostCmdExeProcess.MainWindowHandle;
    		if (HostHandle == null || HostHandle.ToInt32() == 0)
    		{
    			"in CmdExeAPI, HostCmdExeProcess.MainWindowHandle was null, so trying to find it using reflection".info();
    			
	    		var processInfo = HostCmdExeProcess.field("processInfo");
	    		if (processInfo == null)
	    			"Process info was null".error();
	    		else
	    		{
					var processManagerType = "System".type("ProcessManager");
					if (processManagerType == null)
						"processManagerType was null".error();
					else					
						HostHandle = (IntPtr)processManagerType.invokeStatic("GetMainWindowHandle", processInfo);
				}
			}
			if (HostHandle == null || HostHandle.ToInt32() == 0)
				"in CmdExeAPI, HostCmdExeProcess.MainWindowHandle, failed to get the MainWindowHandle".error();
			else
				"handle:{0}".format(HostHandle.ToInt32()).info();				
			return this;
    	}
    	
    	
    	public CmdExeApi selectHost()
    	{
    		selectWindow(HostHandle);
    		return this;
    	}
    	
    	public IntPtr currentWindow()
    	{    		
			return (IntPtr)"WindowsBase".type("UnsafeNativeMethods")
								        .invokeStatic("GetForegroundWindow");
    	}
    	
    	public CmdExeApi selectWindow(IntPtr windowHandle)
    	{
    		if (windowHandle == null || windowHandle.ToInt32() == 0)
    			"in CmdExeApi.selectWindow provided windowHandle was 0)".error();
    		else    		
    			NativeMethods.SetForegroundWindow(windowHandle); 
    		return this;
    	}    	
    	
    	public CmdExeApi saveUsersWindow()
    	{
    		// get the handle of the current window (i.e. the window the user is on
    		UserCurrentWindowHandle = currentWindow();    		
    		return this;
    	}
    	
    	public CmdExeApi restoreUsersWindow()
    	{    		
    	    // restore focus on user's window
    	    /*
    		selectWindow(UserCurrentWindowHandle);    		
    		*/
    		// I removed this since it was causing a couple messages to be lost 
    		// (for now to hide the host window from the user set HideHostWindow to true)
    		return this;
    	}
    	    	
    	public CmdExeApi showHost()
    	{
    		NativeMethods.ShowWindow((int)HostHandle, (int)NativeMethods.SW.SHOW);
    		return this;
    	}
    	
    	public CmdExeApi hideHost()
    	{
    		NativeMethods.ShowWindow((int)HostHandle, (int)NativeMethods.SW.HIDE);
    		return this;
    	}

		public CmdExeApi cmd(string cmdString)
    	{
    		return hostCmd(cmdString);
    	}
    	
    	public CmdExeApi hostCmd(string cmdString)
    	{
    		return hostCmd(cmdString,-1);
    	}
    	    	
    	public CmdExeApi hostCmd(string cmdString, int postExecutionDelay)
    	{    		
			//"sending cmd:{0}".format(cmdString).error();
			saveUsersWindow();			
    		// now select the host so that the cmdString text is sent to it
    		selectHost();
    		// send text
    		Input_Simulator.Keyboard.TextEntry(cmdString.line());
    		// option to run delay (useful for debugging and demos)
    		if (postExecutionDelay > 0)
    			this.sleep(postExecutionDelay);
    		this.sleep(100);					// add a small delay to make sure the text is received (doesn't seem necessary all the time)
    										    // need to figure out if there is a way to get a confirmation that the text was delived to the correct window
    		restoreUsersWindow ();
    		return this;
    	}
    	
    	public CmdExeApi hostCmd_Exit()
    	{
    		selectHost();
    		hostCmd("exit");
    		return this;
    	}
    	
    	public CmdExeApi hostCmd_Ctrl_C()
    	{
    		saveUsersWindow();
    		selectHost();
    		Input_Simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL,VirtualKeyCode.VK_C);
    		restoreUsersWindow();    	   	
    		return this;
    	}
    	
    	public void hostStop()
    	{    		
    		HostCmdExeProcess.stop();
    	}
    }
}
