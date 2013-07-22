// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using NUnit.Framework;
using O2.XRules.Database.APIs;
//O2Ref:nunit.framework.dll
//O2File:API_Fiddler.cs

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class UnitTest_API_Fiddler
    {        	    
    	[Test]
    	public string getInstallationExe()
    	{
    		var fiddler = new API_Fiddler(); 
    		var fiddlerInstaller = fiddler.installerFile(); 
    		Assert.That(fiddlerInstaller.fileExists(), "Could not find file: {0}".info(fiddlerInstaller));
    		return "ok - getInstallationExe";
    	}
    	
    	[Test]
    	public string test_Installation_Process()
    	{
    		var fiddler = new API_Fiddler();   
    		if (fiddler.isInstalled())
    		{    		
    			"[UnitTest_API_Fiddler] Fiddler is already installed, so need to uninstalled first".info();
    			test_UnInstallation_Process();
    		}
    		    		
    		fiddler.install();
			Assert.That(fiddler.isInstalled(), "After installation process, could not detect installation");    		 		
    		return "ok - test_Install";
    	}
    	
    	[Test]
    	public string test_UnInstallation_Process()
    	{
    		var fiddler = new API_Fiddler(); 
    		Assert.That(fiddler.isInstalled(), "Fiddler was NOT installed");
			fiddler.unInstall();
    		Assert.That(fiddler.isInstalled().isFalse(), "After uninstall, Fiddler should not be there anymore");
    		return "ok - test_UnInstall";
    	}
    	
    	
    }
}
