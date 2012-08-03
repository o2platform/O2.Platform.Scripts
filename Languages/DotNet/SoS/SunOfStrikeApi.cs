// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.IO;
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
//O2File:CmdExeAPI.cs

namespace O2.XRules.Database.APIs
{
	public class SonOfStrikeGui : Control
	{
		public CmdExeApi cmdApi { get; set; }
    	public TextBox cdbOut { get; set; } 
    	public ComboBox cdbIn { get; set; }
    	public SunOfStrikeApi sosApi { get; set; }
    	public AutoResetEvent ExecutingRequest { get; set;}
    	public Queue<string> ExecutionQueue { get; set; }
    	//public IntPtr GuiHandle { get; set;}
    	    	
		public string StartArguments { get; set; }		
		//public string pathToCdb = @"C:\Program Files\Debugging Tools for Windows (x86)\cdb.exe";
		public string pathToCdb = @"E:\O2_V4\_O2_V4_TempDir\6_30_2012\Cbd\cdb.exe";
		
		public StringBuilder LastCommandResult {get;set;}
		public string endExecutionString = ">           ^ Syntax error in '.o2.'";
   		public string extraExecutionCommand = ".o2.";
    	//public string cmdDefaultDebuggedProcess = "calc.exe";    
    	
    	public static void launchO2UnderDebugger()
    	{
    	 	//var pathToO2Exe =  PublicDI.config.CurrentExecutableDirectory.pathCombine(PublicDI.config.CurrentExecutableFileName);
    	 	//var pathToO2Exe =  PublicDI.config.CurrentExecutableDirectory.pathCombine("DiagramDesigner.exe");
    	 	var pathToO2Exe =@"E:\O2_V4\_O2_V4_TempDir\6_30_2012\Util - LogViewer [08145]\Util - LogViewer.exe";
    	 	"launching O2 under debugger: {0}".info(pathToO2Exe);
    	 	O2Gui.open<SonOfStrikeGui>("SunOfStrike API",1000,400)
    	 		 .insert_LogViewer()
    	 		 .launch(pathToO2Exe);
    	 		 
    		//new SonOfStrikeGui().launch(pathToO2Exe);
    	}
    	
    	public SonOfStrikeGui()
    	{
    		try
    		{
    			
    			ExecutingRequest = new AutoResetEvent(true);
    			ExecutionQueue =new Queue<string>();
    			LastCommandResult = new StringBuilder();
    			sosApi = new SunOfStrikeApi(executeCmd);
    			buildGui(); 
    			//StartArguments = cmdDefaultDebuggedProcess;
    			   			    			
    		}
    		catch(Exception ex)
    		{
    			ex.log("in SonOfStrikeGui.ctor");
    		}
    	}
    	
    	public void launch(string executableToLaunch)
    	{
    		if (pathToCdb.fileExists().isFalse())
    		{
    			"Aborting launch since could not find .NET Debugger (cdb.exe) in the expected path: {0}".error(pathToCdb);
    			return ;    		
    		}
			cmdApi = new CmdExeApi(pathToCdb, executableToLaunch , dataReceived);
    		cmdApi.MinimizeHostWindow = true;    		    			
    		cmdApi.start();    			    		    		
    		cmdApi.hostCmd(extraExecutionCommand);
    	}
    	
    	
    	public void buildGui()
    	{
    		this.Width = 600;
    		this.Height = 300;
    		// add controls
    		var groupBoxes = this.add_1x1("Debug Commands","'Cdb & SoS' GUI",true, 100);
    		cdbOut = groupBoxes[1].add_TextBox(true).scrollBars().wordWrap(false);
    		cdbIn = cdbOut.insert_Above<ComboBox>(20);    		
    		//var flowLayoutPanel = groupdBoxes[0].add<FlowLayoutPanel>();    	
    		
    		var treeView = groupBoxes[0].add_TreeView();
    		treeView.add_Nodes(sosApi.type().methods_public().names())
    		        .onDoubleClick<string>(invokeSoSApiMethod);
    		
    		// events 
    		cdbIn.onKeyPress(Keys.Enter,executeCdbInText);
    		
    		cdbIn.focus();
    	}
    	
    	public void dataReceived(string text)
    	{
    		if (text.contains(endExecutionString))
			{				
				var size = LastCommandResult.str().size();
				try
				{
					var snipptet = (size == 0) ? "" : LastCommandResult.str().Substring(0,(size > 40) ? 40 : size);
					var stats = "Execution Result: {0} chars [{1}]".format(size,snipptet).remove("".line()).lineBeforeAndAfter();
					stats.info();
					writeToOutput(stats);
					if (size==0)
						writeToOutput("[no data returned]".lineBeforeAndAfter());
				}
				catch(Exception ex)
				{
					ex.log("in SonOfStrikeApi.dataReceived");
				}
				ExecutingRequest.Set();
				//processExecutionQueue();
				return;
			}
			//if (text.starts(cdbPrompt))
			//	text = text.Substring(cdbPrompt.Length);
			LastCommandResult.AppendLine(text);			
			if (sosApi.showReceivedData)
				writeToOutput(text);    		    		
    	}
    	
    	public void writeToOutput(string text)
    	{    		
    		cdbOut.append_Line(text);    		
    		cdbIn.focus();	
    	}
    	
    	public void executeCdbInText()
    	{
    		var text = cdbIn.get_Text();
    		cmd(text);
			cdbIn.insert_Item(text);    										    						 				
		}
    	
    	public void addToExecutionQueue(string cmdToExecute)		// for use by the SonOfStrikeApi callback and implements the multithread queue
    	{
    		"Queueing command:{0}".format(cmdToExecute).debug();
    		ExecutionQueue.Enqueue(cmdToExecute);    		
    		processExecutionQueue();
    	}
    	
    	public Thread processExecutionQueue()
		{
			return O2Thread.mtaThread(
				()=>{
						if (ExecutionQueue.Count == 0)
							return;						

						while (ExecutingRequest.WaitOne(1000).isFalse())		// after one second check if the next command is Ctrl+C
						{
							if (ExecutionQueue.Peek() == "Ctrl+C")
							{
								//ExecutionQueue.Dequeue();								
								"breaking loop".error();
								//cmdApi.hostCmd_Ctrl_C();								
								break;
							}															
						}						
   						//ExecutingRequest.Reset();
						//if (ExecutionQueue.Count == 0)
						//	return;   						
						
						LastCommandResult = new StringBuilder();
						
   						var cmdToExecute = ExecutionQueue.Dequeue(); 
   						"executing Queued command:{0}".format(cmdToExecute).debug();
   						//"from queue, executing cmd:{0}".format(cmdToExecute).debug();
				    	//cmd(cmdToExecute);
				    	sosApi.setShowHideReceivedDataForCommand(cmdToExecute);
				    	if (cmdToExecute == "Ctrl+C")
				    		cmdApi.hostCmd_Ctrl_C();
				    	else
				    	{
				    		cmdApi.hostCmd(cmdToExecute);
							cmdApi.hostCmd(extraExecutionCommand);	// so we can detect when the execution is completed
						}
				    });
		}
    	
    	public void executeCmd(string cmdToExecute)
    	{
    		cmd(cmdToExecute);
    	}
    	
    	public SonOfStrikeGui cmd(string cmdToExecute)
    	{    		    		
    		if (cmdToExecute.starts(">"))
    			sosApi.invoke(cmdToExecute.removeFirstChar());
    		else
    			switch(cmdToExecute)
    			{    				
    				//case "Ctrl+C":			
    				//	cmdApi.hostCmd_Ctrl_C();
    				//	break;    		
    				case "queue":		// while debugging
    					"there are {0} items in the queue".format(ExecutionQueue.Count).error();
    					//ExecutingRequest.Set();
    					break;    	
    				case "queueSet":		// while debugging
    					"ExecutingRequest.Set()".error();
    					ExecutingRequest.Set();
    					break;    	
    				default:
    					addToExecutionQueue(cmdToExecute);
    					break;
    			}    			
    		selectThis();    		    		
    		return this;
    	}    	    	
    	
    	public SonOfStrikeGui selectThis()
    	{
    		cmdApi.selectWindow(this.Handle);
    		return this;
    	}
    	
    	public SonOfStrikeGui stop()
    	{
    		cmdApi.hostCmd_Ctrl_C();
    		cmdApi.cmd("q");
    		cmdApi.hostStop();
    		return this;
    	}
    	
    	public void invokeSoSApiMethod(string methodName)
    	{    		
    		cdbIn.set_Text(">" + methodName);
    		executeCdbInText();    		
    	}
	}
	
	public class SunOfStrikeApi
	{			
		public bool showReceivedData = true;
    	public Action<string> executeCommand;
    	public string pathToSosDll = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\sos.dll";	

		public SunOfStrikeApi()
		{			
		}
    	public SunOfStrikeApi(Action<string> _executeCommand) : this()
    	{    		    		
    		executeCommand = _executeCommand;
    	}
    	
    	private SunOfStrikeApi execute(string commandToExecute, params string[] arguments)
    	{
    		var commandString = commandToExecute;
    		foreach(var argument in arguments)
    			commandString+= " {0}".format(argument);
    		return execute(commandString);
    	}
    	
    	private SunOfStrikeApi execute(string commandToExecute)
    	{
    		executeCommand(commandToExecute);
    		return this;
    	}
    	
    	#region SosApi commands

    	public SunOfStrikeApi quit()
    	{
    		showReceivedData = true;
    		execute("q");
    		execute("exit");
    		return this;
    	}
    	
    	public SunOfStrikeApi go()
    	{
    		showReceivedData = true;
    		return execute("g");
    	}
    	
    	public SunOfStrikeApi showHideReceivedData()
    	{
    		showReceivedData = !showReceivedData;
    		return this;
    	}
    	
    	// white list of commands that (for performance reasons) should not be automatically viewed
    	public SunOfStrikeApi setShowHideReceivedDataForCommand(string command)
    	{
    		switch(command)
    		{
    			case "!DumpHeap -stat":
    			case "!DumpHeap":
    				showReceivedData = false;
    				break;
    			default:
    				showReceivedData = true;
    				break;
    		}
    		return this;
    	}
    	
    	#endregion
    	
    	#region dotNet commands
    	
    	public SunOfStrikeApi dotNet_loadSoS()
    	{
    		return execute(".load ", pathToSosDll);
    	}
    	
    	public SunOfStrikeApi dotNet_help()
    	{    		
    		return execute("!help");
    	}    	
    	
    	public SunOfStrikeApi dotNet_breakInto()
    	{    		
    		return execute("Ctrl+C");
    	}
    	
    	public SunOfStrikeApi dotNet_threads()
    	{    		
    		return execute("!Threads");
    	}
    	
    	public SunOfStrikeApi dotNet_CLRStack()
    	{    		
    		return execute("!CLRStack");
    	}
    	
    	public SunOfStrikeApi dotNet_dumpHeap_Stats()
    	{    		
    		return execute("!DumpHeap -stat");
    	}
    	
    	public SunOfStrikeApi dotNet_dumpHeap_Raw()
    	{    		
    		return execute("!DumpHeap");
    	}
    	
    	#endregion
    	
    	#region unmanaged commands
    	
    	public SunOfStrikeApi win32_threads()
    	{
    		showReceivedData = true;
    		return execute("~");
    	}
    	
    	public SunOfStrikeApi win32_processInfo()
    	{
    		showReceivedData = false;
    		return execute("|");
    	}
    	
    	#endregion
    
    	/*public CmdExeApi getCmdExeApi()
    	{
    		
    	}*/
	}	
}
