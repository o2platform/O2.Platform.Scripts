using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:API_NuGet.cs

namespace O2.XRules.Database.APIs 
{	
	public class MarkdownSharp_Installer
	{	
		public MarkdownSharp_Installer()
		{			
			var expectedDll = @"NuGet\MarkdownSharp.1.13.0.0\lib\35\MarkdownSharp.dll";
			var nugetPackage = "MarkdownSharp";
			if (expectedDll.assembly().isNull())
			{
				var nuGet = new API_NuGet();
				nuGet.install(nugetPackage);
			}			
		}
						
	}
}