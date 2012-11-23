// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.Objects;
using O2.DotNetWrappers.ExtensionMethods;
//Installer:Clojure_Installer.cs!Debug 4.0\Clojure.Main.exe
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
					"O2_FluentSharp_CoreLib.dll".assembly_Location(), 
					"O2_FluentSharp_BCL.dll".assembly_Location(), 
					"O2_FluentSharp_REPL.exe".assembly_Location()};

			var name = "Clojure".add_RandomLetters(4);
			var baseFolder = apiClojure.RootFolder;
			var clojureExe = apiClojure.ClojureExe;
			if (baseFolder.dirExists().isFalse())
			{
				"in openO2ReplInClojureFolder, provided base folder didn't exist: {0}".error(baseFolder);
				apiClojure.script_Me();
				return null;
			}
			"Clojure-icon.png".local().file_Copy(baseFolder);
			"Launch Clojure REPL.h2".local().file_Copy(baseFolder);
			"API_Clojure.cs".local().file_Copy(baseFolder);
			
			"[openO2ReplInClojureFolder] creating AppDomain on folder {0}".info(baseFolder);
			var o2AppDomain = new O2AppDomainFactory(name, baseFolder, defaultAssemblies);
					
			var scriptToExecute = "Launch Clojure REPL.h2".local().fileContents();		
			var script_Base64Encoded = scriptToExecute.base64Encode();
			if (openScriptInEditor_InsteadOfExecutingIt)
				scriptToExecute = "open.scriptEditor().inspector.set_Script(\"{0}\".base64Decode()).waitForClose();".line().format(script_Base64Encoded);
			
			o2AppDomain.executeScriptInAppDomain(scriptToExecute, false, false);  
			return o2AppDomain;
		}
	}
}