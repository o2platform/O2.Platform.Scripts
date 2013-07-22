// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.REPL.Utils;
using FluentSharp.WinForms;
//O2Ref:nunit.framework.dll
using NUnit.Framework;

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class GUI_Tests
    {        	    
    	[Test]
    	public void CreatingMultipleO2ScriptsAndDevEnvironments()
    	{
    		open.scriptEditor().closeForm_InNSeconds(3);
			open.devEnvironment().closeForm_InNSeconds(3);
			open.scriptEditor().closeForm_InNSeconds(3);
			open.devEnvironment().closeForm_InNSeconds(3);
			open.scriptEditor().closeForm_InNSeconds(3);
			open.devEnvironment().closeForm_InNSeconds(3);
			open.scriptEditor().closeForm_InNSeconds(3);
			open.devEnvironment().closeForm_InNSeconds(3);
			open.scriptEditor().closeForm_InNSeconds(3);
			open.devEnvironment().closeForm_InNSeconds(3);
    	}
    	
    }
}
