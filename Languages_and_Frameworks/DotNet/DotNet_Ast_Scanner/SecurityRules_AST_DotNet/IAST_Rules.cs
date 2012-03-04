// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using O2.API.AST.CSharp;
//O2Ref:O2_API_AST.dll

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public interface IAST_Rules
    {    
    	O2CodeStreamTaintRules TaintRules { get; set;}
    	
    	List<string> execute(List<string> sourceFiles, string targetFolder);
    }
}
