// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using FluentSharp.REPL;

//Installer:Jad_Installer.cs!jad/jad.exe
 

namespace FluentSharp.CoreLib.API
{
    public class API_Jad
    {
    	public string Executable 		 { get; set;}
    	public string JadDecompilations  { get; set; }
    	public string LastJadExtraction  { get; set; }
    	public API_Jad()
    	{
    		this.Executable = PublicDI.config.ToolsOrApis.pathCombine(@"jad/jad.exe");
    		this.JadDecompilations = "..\\_Jad".tempDir(false).fullPath();
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
    		return classFile.valid() && classFile.fileExists()
						? jad.execute("-p \"{0}\"".format(classFile))    						
    					: "";
    	}
    	public static string decompile_From_JavaSignature(this API_Jad jad, string javaSignature)
    	{
    		return jad.decompile(jad.getClassFile_From_JavaSignature(javaSignature));
    	}
    	
    	public static string jad_Decompile(this string classFile)
    	{
    		return new API_Jad().decompile(classFile);
    	}
    	
    	public static string getClassFile_From_JavaSignature(this API_Jad jad, string javaSignature)
    	{    		
    		return javaSignature.valid()
    				? jad.LastJadExtraction.pathCombine(javaSignature.replace(".",@"\").add(".class"))
    				: "";    		
    	}
    }
    
    public static class API_Jad_ExtensionMethods_JarFiles
    {
    	public static string extractJarIntoTempFolder(this API_Jad jad, string jarFile)
    	{    		    		
			if (jarFile.extension(".jar"))
			{
				var targetFolder = jad.JadDecompilations;				
				var extractFolder = targetFolder.pathCombine(jarFile.fileName().safeFileName().replace(".","_")).createDir();
				if (extractFolder.folderExists().isFalse() || extractFolder.files(true).empty())
				{
					"Extracting Classes from Jar: {0}".info(jarFile);				
					jarFile.unzip(extractFolder);
				}
				jad.LastJadExtraction = extractFolder;
				return extractFolder;	
			}
			return null;
			
    	}
    	
    }
}

