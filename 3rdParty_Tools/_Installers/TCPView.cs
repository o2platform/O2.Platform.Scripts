using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_TcpView_Test
	{
		public void test()
		{
			new TcpView().start();
		}
	}
	public class TcpView : Tool_API 
	{	
		public TcpView() : this(true)
		{
		}
		
		public TcpView(bool installNow)
		{
			config("TcpView", "TcpView v3.05", "TCPView.zip");
    		Install_Uri = "http://download.sysinternals.com/files/TCPView.zip".uri();
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("tcpview.exe").startProcess();
			return null;
		}		
	}
}