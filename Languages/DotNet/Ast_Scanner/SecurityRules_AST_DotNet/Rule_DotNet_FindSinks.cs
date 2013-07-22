// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using FluentSharp.CSharpAST.Utils;

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
