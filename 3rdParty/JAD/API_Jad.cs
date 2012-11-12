// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.Kernel; 
using O2.DotNetWrappers.ExtensionMethods;
//Installer:Jad_Installer.cs!jad/jad.exe
 

namespace O2.XRules.Database.APIs
{
    public class API_Jad
    {
    	public string Executable { get; set;}
    	
    	public API_Jad()
    	{
    		this.Executable = PublicDI.config.ToolsOrApis.pathCombine(@"jad/jad.exe");
    	}    	
    	public string execute(string commands) 
    	{
    		return this.Executable.startProcess_getConsoleOut(commands);
    	}
    }
    
    public static class API_Jad_ExtensionMethods
    {
    	public static string help(this API_Jad jad)
    	{
    		return jad.execute("");
    	}
    	
    	public static string decompile(this API_Jad jad, string classFile)
    	{
    		return 	jad.execute("-p \"{0}\"".format(classFile));
    	}
    }
}

