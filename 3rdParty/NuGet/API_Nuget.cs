// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms; 
using System.Text; 
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
	public class API_NuGet
	{
		public string NuGet_Exe { get; set;}
		public string NuGet_Exe_DownloadUrl { get; set;}
		
		public API_NuGet()
		{
			this.NuGet_Exe = PublicDI.config.ToolsOrApis
									 .pathCombine("NuGet").createDir()
									 .pathCombine("NuGet.exe");
			this.NuGet_Exe_DownloadUrl = "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=nuget&DownloadId=412077&FileTime=129851621946970000&Build=19310";
			this.checkInstall();
		}	
		
		public string execute(string command)
		{
			return this.NuGet_Exe.startProcess_getConsoleOut(command);
		}
	}
	
	public static class API_NuGet_ExtensionMethods
	{
		public static API_NuGet checkInstall(this API_NuGet nuGet)
		{
			if (nuGet.NuGet_Exe.fileExists())
				"[API_NuGet] found NuGet.exe: {0}".info(nuGet.NuGet_Exe);
			else
			{
				"[API_NuGet] NuGet.exe not found, so downloading it".debug();
				nuGet.NuGet_Exe_DownloadUrl.download(nuGet.NuGet_Exe);
			}		
			return nuGet;
		}
		
		public static string list(this API_NuGet nuGet, string filter)
		{
			return nuGet.execute("list " + filter);
		}
		public static string install(this API_NuGet nuGet, string packageName)
		{
			return nuGet.execute("install " + packageName);
		}
		
		public static string setAPI(this API_NuGet nuGet, string apiKey)
		{
			return nuGet.execute("SetApiKey " + apiKey);
		}
		
		public static string pack(this API_NuGet nuGet, string pathToNuSpec)		
		{
			if (pathToNuSpec.fileExists())
				return nuGet.execute("Pack " + pathToNuSpec);
			return "[API_NuGet] could not find provided NuSpec file: {0}".error(pathToNuSpec);			
		}
		
		public static string push(this API_NuGet nuGet, string pathToNuSpec)		
		{
			if (pathToNuSpec.fileExists())
				return nuGet.execute("Push " + pathToNuSpec);
			return "[API_NuGet] could not find provided NuSpec file: {0}".error(pathToNuSpec);			
		}
	}
}