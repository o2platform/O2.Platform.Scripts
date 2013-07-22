// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;
using FluentSharp.REPL;

namespace O2.Script
{
    public class Empty_XRule : KXRule
    {    
    	private static IO2Log log = PublicDI.log;

        public Empty_XRule()
    	{
            Name = "Empty_XRule";
    	}
    	    	    	    	    
    }
}