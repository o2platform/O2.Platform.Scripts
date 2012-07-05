// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.Interfaces.O2Findings;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Languages_and_Frameworks.DotNet; 
using O2.API.AST.CSharp;
using O2.API.AST.ExtensionMethods.CSharp;
//O2File:IAST_Rules.cs
//O2File:TextEditor_O2CodeStream_ExtensionMethods.cs
//O2File:Findings_ExtensionMethods.cs
//O2File:Ast_Engine_ExtensionMethods.cs


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class Rule_DotNet_FindSinks : IAST_Rules
    {    
		public O2CodeStreamTaintRules TaintRules { get; set;}
    	
    	/*public Rule_DotNet_FindSources()
    	{
    		var taintRules = new O2CodeStreamTaintRules();
    	}*/
    	
    	public List<string> execute(List<string> sourceFiles, string targetFolder)
    	{
    		return null;
    	}
    }
    
}
