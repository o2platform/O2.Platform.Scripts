// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;

//Installer:Clojure_Installer.cs!Clojure\Debug 4.0\Clojure.Main.exe
//O2Ref:Clojure\Debug 4.0\Clojure.dll

namespace O2.XRules.Database.APIs 
{
	public class API_Clojure
	{
		public string RootFolder { get; set; }
		public string ClojureExe { get; set; }
		
		public API_Clojure()
		{
			ClojureExe = @"Clojure\Debug 4.0\Clojure.Main.exe".assembly().Location;
			RootFolder = ClojureExe.parentFolder();
		}				
	}
	
	public static class API_Clojure_ExtensionMethods
	{
		public static O2AppDomainFactory openO2ReplInClojureFolder(this API_Clojure apiClojure)
		{			
			return apiClojure.openO2ReplInClojureFolder(false);
		}  
		
		public static O2AppDomainFactory openO2ReplInClojureFolder(this API_Clojure apiClojure, bool openScriptInEditor_InsteadOfExecutingIt)
		{			
			var defaultAssemblies = new List<string>() 
				{ 
					"FluentSharp.CoreLib.dll".assembly_Location(), 
					"FluentSharp.BCL.dll".assembly_Location(), 
					"FluentSharp.REPL.exe".assembly_Location(),
					"O2_Platform_External_SharpDevelop.dll".assembly_Location()};

			var name = "Clojure".add_RandomLetters(4);
			var baseFolder = apiClojure.RootFolder;
			//var o2ScriptsFolder = apiClojure.RootFolder.append("O2");
			var o2ScriptsFolder = apiClojure.RootFolder;
			var clojureExe = apiClojure.ClojureExe;
			if (baseFolder.dirExists().isFalse())
			{
				"in openO2ReplInClojureFolder, provided base folder didn't exist: {0}".error(baseFolder);
				apiClojure.script_Me();
				return null;
			}
			"Clojure-icon.png".local().file_Copy(o2ScriptsFolder);
			"Launch Clojure REPL.h2".local().file_Copy(o2ScriptsFolder);
			"API_Clojure.cs".local().file_Copy(o2ScriptsFolder);
			
			"[openO2ReplInClojureFolder] creating AppDomain on folder {0}".info(baseFolder);
			var o2AppDomain = new O2AppDomainFactory(name, baseFolder, defaultAssemblies);
					
			var scriptToExecute = "Launch Clojure REPL.h2".local().fileContents();		
			//"Script to execute: {0}".info(scriptToExecute);
			var script_Base64Encoded = scriptToExecute.base64Encode();
			if (openScriptInEditor_InsteadOfExecutingIt)
				scriptToExecute = "open.scriptEditor().inspector.set_Script(\"{0}\".base64Decode()).waitForClose();".line().format(script_Base64Encoded);
			
			o2AppDomain.executeScriptInAppDomain(scriptToExecute, false, false);  
			return o2AppDomain;
		}
	}
}	