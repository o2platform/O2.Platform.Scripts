using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
//using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	[Serializable]
    public class DotNet_SDK_GacUtil
    {
    	public string GacUtil_Folder { get; set;}    	
    	public string GacUtil_Exe { get; set;}    	
    	public string Original_Wsdl_FileOrUrl { get; set;}
    	public string Created_CSharpFile { get; set;}    	
    	public string Created_AssemblyPath { get; set;}    	
		public string Wsdl_Data { get; set;}
		
		public DotNet_SDK_GacUtil()
		{
			GacUtil_Folder = @"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\";
			GacUtil_Exe = GacUtil_Folder.pathCombine("gacUtil.exe");		
		}
	}
	
	public static class DotNet_SDK_GacUtil_ExtensionMethods
	{
		public static string exe(this DotNet_SDK_GacUtil gacUtil)
		{
			return gacUtil.GacUtil_Exe;
		}
		
		public static bool gacUtil_exe_exists(this DotNet_SDK_GacUtil gacUtil)
		{			
			return gacUtil.exe().fileExists();
		}
		
		public static string execute(this DotNet_SDK_GacUtil gacUtil, string arguments)
		{
			return gacUtil.exe().startProcess_getConsoleOut(arguments);
		}
		
		public static string help(this DotNet_SDK_GacUtil gacUtil)
		{
			return gacUtil.execute("");
		}
		
		public static List<string> list(this DotNet_SDK_GacUtil gacUtil)
		{
			return gacUtil.list("");
		}
		
		public static List<string> list(this DotNet_SDK_GacUtil gacUtil, string criteria)
		{
			return gacUtil.execute("/l {0}".format(criteria))
						  .split_onLines()
						  .where((line)=> line.regEx("Version.*PublicKey"))
						  .toList();;
		}
		
		public static List<string> withName(this DotNet_SDK_GacUtil gacUtil, string criteria)
		{
			return gacUtil.list(criteria);
		}
		
		public static List<string> names(this List<string> fullNames)
		{
			return(from fullName in fullNames
				   select fullName.split(",").first()).toList();
		}
		
		public static bool add_To_GAC(this DotNet_SDK_GacUtil gacUtil, string assemblyToInstall)
		{
			return gacUtil.install_In_Gac(assemblyToInstall);
		}
		 
		public static bool install(this DotNet_SDK_GacUtil gacUtil, string assemblyToInstall)
		{
			return gacUtil.install_In_Gac(assemblyToInstall);
		}
		
		public static bool install_In_Gac(this DotNet_SDK_GacUtil gacUtil, string assemblyToInstall)
		{
			"Installing Assembly into GAC: {0}".info(assemblyToInstall);
			var result = gacUtil.execute("/i {0}".format(assemblyToInstall)); 			
			if (result.contains("Assembly successfully added to the cache"))
			{
				"Assembly successfully added to the cache".info();
				return true;
			}
			result.error();
			return false;
		}						
	}
}