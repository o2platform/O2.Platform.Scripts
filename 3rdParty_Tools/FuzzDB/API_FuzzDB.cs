// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.Zip;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
    public class API_FuzzDB_View
    {
    	public static void show()
    	{
    		var panel = O2Gui.open<Panel>("FuzzDb",400,500);
    		var fuzzDb = new API_FuzzDB();
    		var treeView = panel.add_TreeView();
    		if (fuzzDb.isInstalled().isFalse())    		
    			treeView.add_Node("Error:FuzzDB Not installed");
    		else
    		{
    			treeView.add_Node("XSS Payloads")
    					.add_Nodes(fuzzDb.payloads_Xss());
    		}
    	}
    }
            
    public class API_FuzzDB
    {    
    	public string _currentVersion = "fuzzdb-1.08";
    	public string currentVersionZipFile = "fuzzdb-1.08.zip";
    	public string installDir = PublicDI.config.O2TempDir.pathCombine("..\\");
    	
    	public string PathToFuzzDB { get; set;}
    	
    	public API_FuzzDB()
    	{
    		PathToFuzzDB = installDir.pathCombine(_currentVersion);
    		install();
    	}
    	    	    	
    	public bool isInstalled()
    	{
    		return PathToFuzzDB.dirExists();
    	}
    	
    	public bool install()
    	{
    		if (isInstalled())
    			return true;
    		var localFilePath = PublicDI.config.O2TempDir.pathCombine(currentVersionZipFile);
    		var webLocation = "{0}{1}".format(PublicDI.config.O2SVN_FilesWithNoCode, currentVersionZipFile);
    		"downloading file {0} from {1} to {2}".info(currentVersionZipFile, webLocation,localFilePath);
            if (new Web().httpFileExists(webLocation))
            {
                new Web().downloadBinaryFile(webLocation, localFilePath);
                if (localFilePath.fileExists())                                	
                	new zipUtils().unzipFile(localFilePath,installDir);                               
            }
            if (isInstalled())
            {
            	"{0} installed ok".info(_currentVersion);
            	return true;
            }
            "There was a problem installing the {0}".error(_currentVersion);
            return false;
    	}    	    	    	    	    
    	
    	public List<String> getPayloads(params string[] virtualFilePaths)
    	{
    		var payloads = new List<String>();
    		foreach(var virtualFilePath in virtualFilePaths)
    		{
    			var targetFile = PathToFuzzDB.pathCombine(virtualFilePath);
    			if (targetFile.fileExists())    		
    				payloads.AddRange(targetFile.fileContents()
								  				.fixCRLF()
							      				.lines()
							 	  				.remove(0));
				else
					"[API_FuzzDB] in getPayloads the request virtual path could not be found: {0}".error(virtualFilePath);
			}
			return payloads;
    	}
    }

	public static class API_FuzzDB_Generic_Lists
	{
		public static List<int> ints_getN(this API_FuzzDB fuzzDB, int end)
		{
			return fuzzDB.ints_getN(0, end);
		}
		
		public static List<int> ints_getN(this API_FuzzDB fuzzDB, int start, int end)
		{
			var ints = new List<int>();
			for(int i = start; i < end  ; i++)	
				ints.Add(i);
			return ints;
		}
		
		public static List<int> ints_getN_Random(this API_FuzzDB fuzzDB, int count)
		{
			return fuzzDB.ints_getN_Random(count, 64000);
		}
		public static List<int> ints_getN_Random(this API_FuzzDB fuzzDB, int count, int range)
		{
			var ints = new List<int>();
			for(int i = 0; i < count  ; i++)	
				ints.Add(range.random());
			return ints;
		}
	}
    public static class API_FuzzDB_ExtensionMethods_Fuzzing_Databases
    {
    	public static List<String> payloads_Xss(this API_FuzzDB fuzzDB)
    	{
    		var xssPayloads = fuzzDB.getPayloads(@"attack-payloads\xss\xss-rsnake.txt",
    											 @"attack-payloads\xss\xss-uri.txt");
    		    		
    		
											   
			return xssPayloads;
    	}    	    	
    	
    	public static List<String> payloads_SQLi_Generic(this API_FuzzDB fuzzDB)
    	{
			var xssPayloads = fuzzDB.getPayloads(@"attack-payloads\sql-injection\detect\generic\sql-injection.txt",
    											 @"attack-payloads\sql-injection\detect\generic\sql-injection-active.txt",
    											 @"attack-payloads\sql-injection\detect\generic\sql-injection-passive.txt");
			return xssPayloads;    											 
    	}
    	
    	public static List<String> payloads_SQLi_SqlServer(this API_FuzzDB fuzzDB)
    	{
    		var xssPayloads = fuzzDB.getPayloads(@"attack-payloads\sql-injection\detect\ms-sql\sql-injection-ms-sql.txt",
    											 @"attack-payloads\sql-injection\detect\ms-sql\sql-injection-ms-sql-blind-ninja.txt");
			return xssPayloads;
    	} 
    	
    	public static List<String> payloads_SQLi_MySql(this API_FuzzDB fuzzDB)
    	{
    		var xssPayloads = fuzzDB.getPayloads(@"attack-payloads\sql-injection\exploit\mysql-injection-login-bypass.txt",
    											 @"attack-payloads\sql-injection\exploit\mysql-read-local-files.txt");
			return xssPayloads;
    	}    
    }
}
