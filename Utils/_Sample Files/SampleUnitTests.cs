// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.Interfaces.XRules;
using O2.DotNetWrappers.Windows;

//O2Ref:nunit.framework.dll
using NUnit.Framework;

namespace O2.XRules.v0_1
{	
	// Description: this sample show a number of multiple Unit tests and how they can be used
	[TestFixture]
    public class SampleUnitTests
    {    
    	private static IO2Log log = PublicDI.log;    	    	    	    	
    	private string badDir = @"C:\AAAAABBBBB";
    	private string goodDir = PublicDI.config.O2TempDir;
    	    	
    	[Test]
    	public void Assert_That_true()
    	{
    		Assert.That(1 == 1,"Humm, 1 should be equal to 1");
    	}
    	
    	[Test]
    	public void Assert_That_false()
    	{
    		Assert.That(1 == 2, "This is expected (1 is not equal to 2)");
    	}
    	
    	[Test]
    	public void Assert_Fail()
    	{
    		Assert.Fail();		// this will fail this test
    	}
    	
    	[Test]
    	public string Assert_Succeeds()
    	{
    		Assert.That(true);
    		return "When the assert succeeds (or there is no Exceptions \r\n" + 
    			   "the greenbox will show the returned data";
    	}
    	
    	[Test]
    	public void directoryExists__badDir()
    	{
    		Assert.That(Directory.Exists(badDir),"badDir did not exist: " + badDir);
    	}

		[Test]
    	public string directoryExists_goodDir()
    	{
    		Assert.That(Directory.Exists(goodDir),"goodDir did not exist: " + goodDir);
    		return "Directory exists";
    	}
    	    	
		[Test]
    	public string returnMultipleLines()
    	{
    		var result = "line 1" + Environment.NewLine +  
    		 			"line 2" + Environment.NewLine +  
    		  			"line 3" + Environment.NewLine +  
    		   			"line 4" + Environment.NewLine  + 
						"line 5" + Environment.NewLine +  
    		  			"line 6" + Environment.NewLine +  
    		  			"line 7" + Environment.NewLine +  
    		 			"line 8" + Environment.NewLine +  
    		  			"line 9" + Environment.NewLine +  
    		   			"line 10" + Environment.NewLine  + 
						"line 11" + Environment.NewLine +  
    		  			"line 12" + Environment.NewLine +  
    		   			"line 13";
    		return result;
    	}
    	
    	[Test]
    	public List<string> returnListWithFiles()
    	{
    		String sPath = "c:\\";
    		String sSearchPattern = "*.*";    		
			bool bSearchRecursively = false;
    		var files = Files.getFilesFromDir_returnFullPath(sPath, sSearchPattern,bSearchRecursively);
    		return files;
    	}
    	
    	// description: to show how an non List<String> is viewed on the results green text box
    	[Test]
    	public List<Process> returnListOfCurrentProcessest()		
    	{
    		return Processes.getProcesses();
    	}
    	
    	[Test]
    	public List<Process> returnNull()		
    	{
    		return null;
    	}
    }
}
