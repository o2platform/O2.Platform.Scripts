using System;
using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class MarkdownDeep_Installer_Test
	{
		public void test()
		{
			new MarkdownDeep_Installer().start();
		}
	}
	public class MarkdownDeep_Installer : Tool_API 
	{			
		
		public MarkdownDeep_Installer()
		{
			config("MarkdownDeep", 
				   "http://www.toptensoftware.com/downloads/MarkdownDeep.zip".uri(),
				   "bin/MarkdownDeep.dll");
			installFromZip_Web(); 							
		}
						
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Install_Dir.startProcess();
			return null;
		}		
	}
}