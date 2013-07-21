// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2File:WebService.cs
//O2Ref:System.Web.Services.dll

namespace O2.XRules.Database.APIs
{
    public class API_Azure_via_WebREPL
    {   
    	public WebService webService;
    	
    	public string 	 Last_ExecuteScript { get; set;}
    	public string 	 Last_ResponseData { get; set;}
    	
    	public API_Azure_via_WebREPL()
    	{
    		webService = new WebService();
    	}
    	public API_Azure_via_WebREPL(string server) : this()
    	{
			this.set_Server(server);    	
    	}    	
    }
    
     public static class API_Azure_via_WebREPL_Helpers
     {
     	public static string wsdl(this API_Azure_via_WebREPL apiAzure)
     	{
     		return apiAzure.webService.Url.str();
     	}
     	
     	public static API_Azure_via_WebREPL set_Server(this API_Azure_via_WebREPL apiAzure, string server)
     	{
     		var serverTemplate = "http://{0}/csharprepl/CSharp_REPL.asmx";
     		apiAzure.webService.Url = serverTemplate.format(server);
     		return apiAzure;
     	}
     	
     	public static string executeScript(this API_Azure_via_WebREPL apiAzure, string script)
     	{
     		"Executing script: {0}".info(script);
     		apiAzure.Last_ExecuteScript = script;
     		var response = apiAzure.webService.ExecuteCSharpCode(script);
     		apiAzure.Last_ResponseData = response;
     		if (response.starts("[compileAndExecuteCodeSnippet] Compilation failed: "))
     		{
     			"[API_Azure_via_WebREPL][executeScript]: server compilation error: \n\n {0}".error(response);
     			return null;
     		}
     			"[API_Azure_via_WebREPL][executeScript]: response size: {0}".info(response.size());
     		return response;
     	}
     	
     	public static List<string> executeScript_ConvertTo_StringList(this API_Azure_via_WebREPL apiAzure, string script)
     	{     		
			var response = apiAzure.executeScript(script);
     		if (response.valid())
	     		return response.json_Deserialize<List<string>>();
	     	return new List<string>();
     	}
     	
     	public static Dictionary<string,string> executeScript_ConvertTo_DicionaryStringString(this API_Azure_via_WebREPL apiAzure, string script, string nameKey, string valueKey)
     	{
     		var data = new Dictionary<string,string>();
     		var response = apiAzure.executeScript(script);
     		if (response.valid())
     		{
     			var items = (Object[])response.json_Deserialize();			
				if (items.notNull())						
					foreach(Dictionary<string,object> item in items)				
						data.add(item[nameKey].str(),item[valueKey].str());						
     		}
     		return data;
     	}	
     }
     
     public static class API_Azure_via_WebREPL_Commands
     {       
     	public static string cmd_Execute(this API_Azure_via_WebREPL apiAzure, string exePath, string arguments)
     	{
     		return apiAzure.executeScript(@"return @""{0}"".startProcess_getConsoleOut(@""{1}"");"
     					  .format(exePath, arguments));
     	}
     	
     
     	public static List<string> folders(this API_Azure_via_WebREPL apiAzure, string path)
     	{
     		return apiAzure.executeScript_ConvertTo_StringList(@"return @""{0}"".folders();".format(path));     		
     	}
     	public static List<string> files(this API_Azure_via_WebREPL apiAzure, string path)
     	{
     		return apiAzure.executeScript_ConvertTo_StringList(@"return @""{0}"".files();".format(path));     		
     	}
     	
     	public static string fileContents(this API_Azure_via_WebREPL apiAzure, string filePath)
     	{
     		return apiAzure.executeScript(@"return @""{0}"".fileContents();".format(filePath));     		
     	}
     	
     	public static Dictionary<string,string> environmentVariables(this API_Azure_via_WebREPL apiAzure)
     	{
     		var rawData = apiAzure.executeScript(@"return Environment.GetEnvironmentVariables();");
     		var rawDictionary = rawData.json_Deserialize() as Dictionary<string,object>;
     		var environmentVariables = new Dictionary<string,string>();
     		foreach(var item in rawDictionary)
     			environmentVariables.add(item.Key, item.Value.str());
     		return environmentVariables;     		
     	}
     	
     	public static Dictionary<string,string> specialFolders(this API_Azure_via_WebREPL apiAzure)
     	{
     		var script = @"var specialFolders = from Environment.SpecialFolder specialFolder in Enum.GetValues(typeof(Environment.SpecialFolder))
											    let folderPath = Environment.GetFolderPath(specialFolder)
											    where folderPath.valid()
											    select new { specialFolder = specialFolder.str(), folderPath = folderPath};
												return 	specialFolders;";
			return apiAzure.executeScript_ConvertTo_DicionaryStringString(script, "specialFolder", "folderPath");			
     	}
     	
     	public static Dictionary<string,string> propertyValues_Static(this API_Azure_via_WebREPL apiAzure, string typeName)
     	{     		
     		var script = @"var propertyValues = from name in typeof(" + typeName + @").properties().names()
							     let value = typeof(" + typeName + @").prop(name).str()
							     where value.valid()
							     select new { propName = name , propValue = value};
						  return 	propertyValues;";
			return apiAzure.executeScript_ConvertTo_DicionaryStringString(script, "propName", "propValue");     	
		}
		
		public static Dictionary<string,string> propertyValues_Object(this API_Azure_via_WebREPL apiAzure, string targetObject)
     	{
     	
     		var script = @"var propertyValues = from name in " + targetObject + @".type().properties().names()
							     let value = "+ targetObject + @".prop(name).str()
							     where value.valid()
							     select new { propName = name , propValue = value};
						  return 	propertyValues;";
			return apiAzure.executeScript_ConvertTo_DicionaryStringString(script, "propName", "propValue");     	
		}
		
		public static string applicationPath(this API_Azure_via_WebREPL apiAzure)
		{
			return apiAzure.propertyValues_Object("System.Web.HttpContext.Current.Request")
					   							 ["PhysicalApplicationPath"];
		}
     }     
     
     
     public static class API_Azure_via_WebREPL_GuiHelpers
     {
     	public static Dictionary<string,string> view_EnvironmentVariables(this API_Azure_via_WebREPL apiAzure)
     	{
     		var values = apiAzure.environmentVariables();
			values.show_In_ListView()
     			   .title("Special Folders")
     				.parentForm();
     		return values;
     	}
     	
     	public static Dictionary<string,string> view_Object(this API_Azure_via_WebREPL apiAzure, string objectValue)
     	{
     		var values = apiAzure.propertyValues_Object(objectValue);
			values.show_In_ListView()
     			  .title(objectValue)
     			  .parentForm();
     		return values;
     	}
     	
     	public static Dictionary<string,string> view_HttpContext_Object(this API_Azure_via_WebREPL apiAzure)
     	{
     		return apiAzure.view_Object("System.Web.HttpContext.Current");
     	}
     }     
}
    