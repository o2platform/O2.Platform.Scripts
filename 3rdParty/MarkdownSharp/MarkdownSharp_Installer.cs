using FluentSharp.CoreLib;

//O2File:API_NuGet.cs

namespace O2.XRules.Database.APIs 
{	
    //NOT FULLLY IMPLEMENTED
	public class MarkdownSharp_Installer
	{	
		public MarkdownSharp_Installer()
		{			
			var expectedDll = @"NuGet\MarkdownSharp.1.13.0.0\lib\35\MarkdownSharp.dll";
			var nugetPackage = "MarkdownSharp";
			if (expectedDll.assembly().isNull())
			{
				var nuGet = new API_NuGet();
				//nuGet.install(nugetPackage);
			}			
		}
						
	}
}