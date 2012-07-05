// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Interfaces.Views;
using O2.Kernel;
using System.Windows.Forms; 
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX;

using NUnit.Framework; 
using O2.Views.ASCX.classes;
using O2.DotNetWrappers.Network;
using O2.Core.XRules.Ascx;
using O2.XRules.Database.Utils;
//O2File:ascx_SvnBrower.cs.o2

//O2Ref:O2_Core_XRules.dll
//O2Ref:HTMLparserLibDotNet20.dll
//O2Ref:nunit.framework.dll



namespace O2.Script
{	
	
	[TestFixture]
    public class SvnTest
    {    
    	private static IO2Log log = PublicDI.log;
    	
    	public static string svnServer = @"http://o2platform.googlecode.com/svn/trunk";
    	public static string XRulesDatabase = svnServer + @"/O2%20-%20All%20Active%20Projects/O2_XRules_Database/_Rules/a";
    	
        public SvnTest()
    	{    	
    		
    	}    	    	    	    	    
    	
    	[Test] 
    	public bool browseSvnArchive()
    	{    		
    		var svnBrowser = O2Gui.open<ascx_SvnBrowser>("Svn Browser");
    		//svnBrowser.openSvnUrl(svnServer);
    		return true;
    	}
    	
    	[Test]
    	public bool checkIfSvnServerIsOnline()
    	{
            var urlContents = new Web().getUrlContents(svnServer);
    		Assert.That(false == string.IsNullOrEmpty(urlContents), "urlContents was empty");    		
    		return true;
    	}
    	
    	[Test]
    	public bool checkIfRulesDatabaseIsAvailable()
    	{
            var urlContents = new Web().getUrlContents(XRulesDatabase);
    		Assert.That(false == string.IsNullOrEmpty(urlContents), "urlContents was empty");    		
    		return true;
    	}    	
    	
    	
    }
}
