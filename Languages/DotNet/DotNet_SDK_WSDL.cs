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
using O2.External.SharpDevelop.ExtensionMethods;
//O2Ref:MS_SDK_wsdl.exe

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	[Serializable]
    public class DotNet_SDK_WSDL
    {
    	public string Wsdl_Exe { get; set;}    	
    	public string Original_Wsdl_FileOrUrl { get; set;}
    	public string Created_CSharpFile { get; set;}    	
    	public string Created_AssemblyPath { get; set;}    	
		public string Wsdl_Data { get; set;}
		
		public DotNet_SDK_WSDL()
		{
			var assembly = "MS_SDK_wsdl.exe".assembly();			
			Wsdl_Exe = assembly.notNull() ? assembly.Location
										  : PublicDI.config.ReferencesDownloadLocation.pathCombine("MS_SDK_wsdl.exe");
		}
		
		public bool wsdl_exe_exists()
		{			
			return Wsdl_Exe.fileExists();
		}
		
		public string wsdl_CreateCSharp(string wsdlSourceFileOrUrl)
		{
			return wsdl_CreateCSharp(wsdlSourceFileOrUrl,"wsdl".tempDir(),null);
		}
		
		public string wsdl_CreateCSharp(string wsdlSourceFileOrUrl, string targetFolder)
		{
			return wsdl_CreateCSharp(wsdlSourceFileOrUrl, targetFolder ,null);
		}
		
		//tryed to support /sharetypes
        /*public string wsdl_CreateCSharp(string wsdlSourceFileOrUrl, string targetFolder, string extraWsdlParameres)
        {
        	return  wsdl_CreateCSharp(wsdlSourceFileOrUrl.wrapOnList(), targetFolder, extraWsdlParameres);
        }
        //public string wsdl_CreateCSharp(List<string> wsdlsSourceFilesOrUrls, string targetFolder, string extraWsdlParameres)
        */
        
        public string wsdl_CreateCSharp(string wsdlSourceFileOrUrl, string targetFolder, string extraWsdlParameres)
        {        	
        	if (wsdl_exe_exists())
        	{        		     
        		if (targetFolder.Last() =='\\') 
					targetFolder = targetFolder.removeLastChar();
        		this.Original_Wsdl_FileOrUrl = wsdlSourceFileOrUrl;
        		this.Created_CSharpFile = "";
        		var wsdlTargets = wsdlSourceFileOrUrl;
        		/*
        		foreach(var wsdlSourceFileOrUrl in wsdlsSourceFilesOrUrls)
        			wsdlTargets += "\"{0}\" ".format(wsdlSourceFileOrUrl);
        		*/
            	var parameters = " {2} {0} /out:\"{1}\"".format(wsdlTargets, targetFolder, extraWsdlParameres ?? "");
            	var executionResult = Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(Wsdl_Exe, parameters);            	            	
            	executionResult.info();
            	if (executionResult.valid())
            	{
            		var splitData = executionResult.split("'");            		
            		foreach(var fileCreated in splitData)
            		//if(splitData.size()==3)
            		{
            		//var fileCreated = splitData[1];
            			if (fileCreated.fileExists())
            			{	
            				// add reference so that we can compile it in O2
            				var extraLineToAdd = "//O2Ref:System.Web.Services.dll".line();
            				fileCreated.fileInsertAt(0, extraLineToAdd); 
            				
            				this.Created_CSharpFile = fileCreated;
            				"Found file: {0}".debug(fileCreated);
            				return fileCreated;
            			}
            		}
            		"Could not find file in string {0}".error(executionResult);;
            	}   
            	else
            		"in wsdl_CreateCSharp, executionResult was not valid".error();
			}       
			"[wsdl_CreateCSharp] wsdl_exe_exists returned false".error();
            return "";
        }
              	       	
		public string wsdl_CreateAssembly(string wsdlSourceFileOrUrl)
		{
			return wsdl_CreateAssembly(wsdlSourceFileOrUrl,null, null);
		}
		
        public string wsdl_CreateAssembly(string wsdlSourceFileOrUrl, string targetFolder, string extraWsdlParameres) 
        {        	
        	if(targetFolder.isNull())
        		targetFolder = "wsdl".tempDir();
        	var cSharpFile = wsdl_CreateCSharp(wsdlSourceFileOrUrl,targetFolder, extraWsdlParameres);
			cSharpFile.fileInsertAt(0, "//O2Ref:System.Web.Services.dll".line()); 		
			var assembly = cSharpFile.compile(true); 	
			if (assembly.isNull())
			{
				"[wsdl_CreateAssembly] failed to create CSharp file from wsdl: {0}".error(wsdlSourceFileOrUrl);
				return null;
			}
			this.Created_AssemblyPath = Files.Copy(assembly.Location,targetFolder.pathCombine(cSharpFile.fileName().replace(".cs",".dll")));
			this.Wsdl_Data = cSharpFile + ".wsdl_Data.xml";
			this.saveAs(this.Wsdl_Data);
			return cSharpFile;
			
			//return Created_AssemblyPath;
		}				
    }
    
    public static class DotNet_SDK_WSDL_ExtensionMethods
    {
    	public static string wsdl_CreateCSharp(this string wsdlSourceFileOrUrl)
    	{
    		return new DotNet_SDK_WSDL().wsdl_CreateCSharp(wsdlSourceFileOrUrl);
    	}
    	
    	public static string wsdl_CreateCSharp(this string wsdlSourceFileOrUrl, string targetFolder)
    	{
    		return new DotNet_SDK_WSDL().wsdl_CreateCSharp(wsdlSourceFileOrUrl, targetFolder);
    	}
    	
    	public static string wsdl_CreateCSharp(this string wsdlSourceFileOrUrl, string targetFolder, string extraWsdlParameters)
    	{
    		return new DotNet_SDK_WSDL().wsdl_CreateCSharp(wsdlSourceFileOrUrl, targetFolder, extraWsdlParameters);
    	}
    }
}
