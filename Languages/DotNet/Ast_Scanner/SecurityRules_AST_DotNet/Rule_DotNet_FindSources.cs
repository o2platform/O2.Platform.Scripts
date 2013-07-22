// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.Interfaces;
using ICSharpCode.NRefactory.Ast;
//O2File:IAST_Rules.cs
//O2File:TextEditor_O2CodeStream_ExtensionMethods.cs
//O2File:Findings_ExtensionMethods.cs
//O2File:Ast_Engine_ExtensionMethods.cs

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class Rule_DotNet_FindSources : IAST_Rules
    {    
    	public O2CodeStreamTaintRules TaintRules { get; set;}
    	
    	/*public Rule_DotNet_FindSources()
    	{
    		var taintRules = new O2CodeStreamTaintRules();
    	}*/
    	
    	public List<string> execute(List<string> sourceFiles, string targetFolder)
    	{
    		sourceFiles.Add("aaa");
    		return sourceFiles;
    	}
    	
    	public List<IO2Finding> createFindings(string targetFile)
    	{    	
    		var o2Findings = new List<IO2Finding>();
    		var codeStreams = createCodeStreams(targetFile);
    		foreach(var codeStream in codeStreams)
    		{    		
	    		var vulnName = "Malicious Data";
	    		var vulnType = "Web Sources";
	    		var rootNodeText = "{0} - {1}".format(vulnName, vulnType);
	    		o2Findings.AddRange(codeStream.o2Findings(vulnName, vulnType, rootNodeText));
	    	}
	    	if (o2Findings.size()>0)
	    		"[Rule_DotNet_FindSources] Created {0} findings for: {1}".debug(o2Findings.size(),targetFile);
    		return o2Findings;
    	}
    	
    	public List<O2CodeStream> createCodeStreams(string targetFile)
    	{    								
    		var codeStreams = new List<O2CodeStream>();
    		var astData = Ast_Engine_Cache.get(targetFile);    		
			var firstMethod = astData.firstMethodDeclaration();
			if (firstMethod.notNull())
			{
				//create traces for methods called
				foreach (var methodCalled in astData.calledINodesReferences(astData.iMethod(firstMethod)))
				{
					if (methodCalled != null)
						if (methodCalled is MemberReferenceExpression)
                        {
							var memberReference = (MemberReferenceExpression)methodCalled;
							//"method Called: {0}".info(methodCalled);
							var iMethodOrProperty = astData.fromMemberReferenceExpressionGetIMethodOrProperty(memberReference);
					
							if (iMethodOrProperty != null)
							{
								"Method: {0}".info(iMethodOrProperty.DotNetName);
								codeStreams.Add(astData.createO2CodeStream(TaintRules, targetFile,methodCalled));
							}
							//	MethodsCalledTreeView.add_Node(iMethodOrProperty.DotNetName, methodCalled);
						}
					
				}
				//return codeStreams;
				
				//create traces for global Fields											
				foreach(var variable in  astData.firstTypeDeclaration().ast_Fields().ast_Field_Variables())		//var astFields = astData.firstTypeDeclaration().ast_Fields("System.Web.UI.WebControls.TextBox");	
					codeStreams.Add(astData.createO2CodeStream(TaintRules, targetFile,variable));
					
				//create traces for global properties
				foreach(var property in  astData.firstTypeDeclaration().ast_Properties())			
					codeStreams.Add(astData.createO2CodeStream(TaintRules, targetFile,property));				
				
				//create traces for methods parameters properties
				foreach (var parameter in firstMethod.parameters())			
					codeStreams.Add(astData.createO2CodeStream(TaintRules, targetFile,parameter));
			}
				
			return codeStreams;
    	}
    }
}
