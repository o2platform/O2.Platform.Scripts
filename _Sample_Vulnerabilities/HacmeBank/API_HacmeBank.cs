// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.IE.ExtensionMethods;
using SHDocVw;
using WatiN.Core;
using O2.XRules.Database.Utils.O2;
using O2.XRules.Database.APIs;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Interfaces.O2Findings;
using O2.Views.ASCX.O2Findings;
using O2.XRules.Database.Findings;
using O2.DotNetWrappers.O2Findings;
using O2.XRules.Database.Languages_and_Frameworks.DotNet; 
using O2.API.AST.CSharp;
using O2.API.AST.ExtensionMethods;
using O2.API.AST.ExtensionMethods.CSharp;

//O2File:Findings_ExtensionMethods.cs
//O2File:Ast_Engine_ExtensionMethods.cs

//O2File:WatiN_IE_ExtensionMethods.cs    
//O2File:WatiN_IE.cs
//O2File:DotNet_ViewState.cs
//O2Ref:Interop.SHDocVw.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:O2_API_AST.dll   
 
 
namespace O2.XRules.Database.HacmeBank
{
    public class API_HacmeBank
    {    
    	public string Url_Website { get; set; }
    	public string Url_WebServices { get; set; }
 		
 		public WatiN_IE ie;   
 		
    	public API_HacmeBank() : this("80")
    	{}
 
    	public API_HacmeBank(string websitePort) : this(websitePort, null)
    	{    	    	
    	}
    	
    	public API_HacmeBank(string websitePort, WatiN_IE watinIE) 
    	{
    		Url_Website = "http://localhost:{0}/HacmeBank_v2_Website".format(websitePort);
    		if (watinIE.isNull())
    			ie = "".ie(0,450,800,700);  
    		else
    			ie = watinIE;
    	}
    	
 
    	public API_HacmeBank login()
    	{
    		return(login(1));
    	}
    	
    	public API_HacmeBank login(int id)
    	{
    		switch (id)
    		{
    			case 1:
    				return login("jm", "jm789");    				
    			case 2:
    				return login("jv", "jv789");
    			case 3:
    				return login("jc", "jc789");    				
    		}
    		return this;
    	}
 		public API_HacmeBank loginPage()
 		{
 			var loginUrl = "{0}/aspx/login.aspx".format(Url_Website);
    		ie.open(loginUrl); 
    		return this;
 		}
 		
    	public API_HacmeBank login(string userName, string password)
    	{
    		loginPage();
			ie.field("txtUserName").value(userName);
			ie.field("txtPassword").value(password);
			ie.button("Submit").click();
			return this;
    	}
    	
    	public API_HacmeBank adminSection()
    	{
    		var adminLink = "Admin Section";
    		if (ie.hasLink(adminLink))
    			ie.click(adminLink);
    		else
    			"[API_HacmeBank][adminSection] could not find admin link: {0}".format(adminLink); 
    		return this;
    	}
    	
    	public API_HacmeBank loginToadminSection()
    	{
    		this.adminSection(); 
    		var response = ie.viewState().ViewState_Values[12];
			ie.set_Value("_ctl3:txtResponse", response); 
			ie.click("Login");
			return this;
    	}
    }
    
    public class API_HacmeBank_CustomScanner
    {
    
    	public static List<IO2Finding> calculate_Url_to_EntryMethod_Mappings(string pathWithSourceFiles)
    	{
    		return calculate_Url_to_EntryMethod_Mappings(pathWithSourceFiles, "22222", null);
    	}
		public static List<IO2Finding> calculate_Url_to_EntryMethod_Mappings(string pathWithSourceFiles, string port, ProgressBar progressBar)
		{			
			var urlBase = "http://localhost:{0}/Hacmebank_v2/main.aspx?TargetPage={1}";			
			return calculate_Url_to_EntryMethod_Mappings(pathWithSourceFiles, urlBase, port, progressBar);
		}
		
    	public static List<IO2Finding> calculate_Url_to_EntryMethod_Mappings(string pathWithSourceFiles, string urlBase, string port, ProgressBar progressBar)
    	{
    		var o2Findings = new List<IO2Finding>();
			var filesToAnalyze = pathWithSourceFiles.files("*.cs",true);
			progressBar.maximum(filesToAnalyze.size());
			foreach(var file in filesToAnalyze)
			{	
					"Processing file:{0}".info(file);
				var url = urlBase.format(port, file.replace(pathWithSourceFiles,"").replace(".ascx.cs",""));
				
				foreach(var type in file.csharpAst().types(true))
					foreach(var baseType in type.BaseTypes)			
						if (baseType.str() == "System.Web.UI.UserControl")				
						{
							var astData = new O2MappedAstData();
							astData.loadFile(file);
							foreach(var iMethod in astData.iMethods())
							{
								var o2Finding = new O2Finding();
								o2Finding.vulnName = url;
								o2Finding.vulnType = "Web EntryPoint";
								var source = new O2Trace(url);
								var sink = new O2Trace(iMethod.fullName());
								source.traceType = TraceType.Source;
								sink.traceType = TraceType.Known_Sink;					
								source.childTraces.Add(sink);
								o2Finding.o2Traces.Add(source);					
								o2Findings.Add(o2Finding);
							}									
						}	
				progressBar.increment(1);				
			}		
			return o2Findings;
    	}
    	
    	public static List<IO2Finding> calculate_WebLayer_tracesInto_WebServices_with_URL_as_Source(List<IO2Finding> sourceO2Findings, List<IO2Finding> urlMappings)
    	{
    		var webLayerFindingsWithUrl = new List<IO2Finding>();

			var mappedFindings = sourceO2Findings.getFindingsWith_WebServicesInvoke()
												 .makeSinks_WebServicesInvokeTarget();
												 
			var indexedByRootMethod = mappedFindings.indexBy(
				(o2Finding)=>{
								if (o2Finding.o2Traces[0].childTraces.size() > 1)
									return o2Finding.o2Traces[0].childTraces[1].context;
								else
									return "no root method";
							 });
									
			 
			foreach(var sinkValue in urlMappings.indexBy_Sink())
			{
				if (indexedByRootMethod.hasKey(sinkValue.Key))
				{
					foreach(var findingA in sinkValue.Value)
						foreach(var findingB in indexedByRootMethod[sinkValue.Key])
						{				
							//var newFinding = findingA.copy();
							var newFinding = OzasmtGlue.createCopyAndGlueTraceSinkWithTrace(findingA, findingB.o2Traces);
							webLayerFindingsWithUrl.add(newFinding);
						}		
				} 
			//	sinkValue.Key.info(); 
			}
			webLayerFindingsWithUrl.removeFirstSource();
			return webLayerFindingsWithUrl;
    	}
    	
    	public static List<IO2Finding> join_WebLayer_traces_with_WebServices_traces(List<IO2Finding> sourceO2Findings, List<IO2Finding> webLayerFindingsWithUrl)
    	{
    		var bySink = webLayerFindingsWithUrl.indexBy_Sink();
			var bySource = sourceO2Findings.indexBy_Source();
			
			var jointFindings = new List<IO2Finding>();
			foreach(var source in bySource) 
			{
				var fixedSource = source.Key.replace("HacmeBank_v2_WS.WS_UserManagement", "WS_UserManagement");
				fixedSource = fixedSource.replace("HacmeBank_v2_WS.WS_AccountManagement", "WS_AccountManagement");
				fixedSource = fixedSource.replace("HacmeBank_v2_WS.WS_UsersCommunity", "WS_UsersCommunity");
				if (fixedSource.contains("Login"))
				{
					fixedSource.info();
					//source.Key.info();
				}
				if (bySink.hasKey(fixedSource))
				{
					foreach(var findingA in bySink[fixedSource]) 
						foreach(var findingB in source.Value) 			
						{
							var newFinding = OzasmtGlue.createCopyAndGlueTraceSinkWithTrace(findingA, findingB.o2Traces);
							jointFindings.add(newFinding);
						}
				}				
			}
			return jointFindings; 
    	}
    	
    	public static List<IO2Finding> create_SqlInjection_Findings(List<IO2Finding> jointFindings)
    	{
    		var sqlInjectionFindings = jointFindings.whereSink_Contains("Sql");
    		var finalFindings = sqlInjectionFindings.whereSource_Contains("field ->").add(
    				            sqlInjectionFindings.whereSource_Contains("method ->"));
    		
    		finalFindings. set_VulnType("HacmeBank -> SQL Injection (via Web Site)");
    		return finalFindings;
    	}
    }
}
