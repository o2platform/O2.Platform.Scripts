// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Languages_and_Frameworks.DotNet; 
using O2.API.AST.CSharp;

//O2File:Rule_DotNet_FindSources.cs

//O2Ref:O2_API_AST.dll

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class Rule_DotNet_ScanFiles: IAST_Rules
    {    
    	public O2CodeStreamTaintRules TaintRules { get; set;}
    	public static void test()
    	{
    	}
    	
    	public List<string> execute(List<string> sourceFiles, string targetFolder)
    	{
    		sourceFiles.Add("aaa");
    		new Rule_DotNet_FindSources();
    		return sourceFiles;
    	}
    	
    	
    }
}
