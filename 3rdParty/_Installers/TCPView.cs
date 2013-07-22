using System.Diagnostics;
using FluentSharp.CoreLib;

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
			config("TcpView", 			
				   "http://download.sysinternals.com/files/TCPView.zip".uri(),
				   @"tcpview.exe");
    		installFromZip_Web(); 		
    		
			/*config("TcpView", "TcpView v3.05", "TCPView.zip");
    		Install_Uri = "http://download.sysinternals.com/files/TCPView.zip".uri();
    		if (installNow)
    			install();		*/
		}
				
		public Process start()
		{
			if (isInstalled())				
				return this.Executable.startProcess();
			return null;
		}		
	}
}