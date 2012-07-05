// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:API_IIS_Logs


namespace O2.XRules.Database.APIs
{
	public class WebFolder
	{	
		[XmlAttribute]
		public string Name				{ get; set; }
		public List<WebFolder> Folders 	{ get; set; }
		public List<WebFile> Files 	   	{ get; set; }
		
		public WebFolder()
		{
			Folders = new List<WebFolder>();
			Files 	= new List<WebFile>();			
		}
		
		public WebFolder(string name) : this()
		{
			Name = name;
		}
		
		public override string ToString()
		{
			return "folder: {0}".format(Name);
		}
	}
	
	public class WebFile
	{	
		[XmlAttribute]
		public string Name				{ get; set; }
		
		public WebFile()
		{
		}
		
		public WebFile(string name) : this()
		{
			Name = name;
		}
		
		public override string ToString()
		{
			return "file: {0}".format(Name);
		}
	}
	
	
	public static class WebFolder_ExtensionMethods_Folders
	{
		public static WebFolder folder(this WebFolder webFolder, string name)
		{
			var subFolder = (from folder in webFolder.Folders
							 where folder.Name == name
							 select folder).first();
							 
			return subFolder ?? webFolder.add_Folder(name);			
		}
		
		public static WebFolder add_Folder(this WebFolder webFolder, string name)
		{
			var subFolder = new WebFolder(name);
			webFolder.Folders.Add(subFolder);
			return subFolder;
		}
		
		public static bool hasFolders(this WebFolder webFolder)
		{
			return webFolder.Folders.size() > 0;
		}
	}
	
	public static class WebFolder_ExtensionMethods_Files
	{
	
		public static WebFile file(this WebFolder webFolder, string name)
		{
			var _file = (from file in webFolder.Files
						 where file.Name == name
						 select file).first();
						 
			return _file ?? webFolder.add_File(name);			
		}
		
		public static WebFile add_File(this WebFolder webFolder, string name)
		{
			var file = new WebFile(name);
			webFolder.Files.Add(file);
			return file;
		}
		
		public static bool hasFiles(this WebFolder webFolder)
		{
			return webFolder.Files.size() > 0;
		}
	}
	
/*	public class API_IIS_Request
	{
		public List<IIS_Log_Entry> LogEntries { get; set; } 
		
		public API_IIS_Request()
		{
			LogEntries = new List<IIS_Log_Entry>();
		}
						
		public class Folder
		{
			public string Name { get; set; }
			//public string Name { get; set; }
		}		
	}
*/	
}