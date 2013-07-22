using FluentSharp.CoreLib;

//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class Roslyn_Test
	{
		public static void test() 
		{
			new Roslyn();
		}
	}
	
	public class Roslyn : Tool_API 
	{				
		public Roslyn()
		{			
			//install Roslyn.Compilers.Common
			/*config("Roslyn_1.1", 
				   "https://nugetgallery.blob.core.windows.net/packages/Roslyn.Compilers.Common.1.1.20524.4.nupkg".uri(),
				   "Roslyn.Compilers.Common.nuspec");
			installFromZip_Web();*/
			installNupkg("Roslyn.Compilers.Common","1.1.20524.4");
			installNupkg("Roslyn.Services.Common", "1.1.20524.4");
			installNupkg("Roslyn.Compilers.CSharp","1.1.20524.4");
			installNupkg("Roslyn.Services.CSharp", "1.1.20524.4");			
		}
		
		public void installNupkg(string name, string version)
		{
			config("Roslyn", 
				   "https://nugetgallery.blob.core.windows.net/packages/{0}.{1}.nupkg".format(name, version).uri(),
				   "{0}.nuspec".format(name));
			installFromZip_Web();
		}
	}
}