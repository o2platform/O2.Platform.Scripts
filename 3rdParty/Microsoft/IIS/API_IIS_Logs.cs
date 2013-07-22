// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Xml.Serialization;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;


namespace O2.XRules.Database.APIs
{
	public class API_IIS_Logs
	{
		public List<IIS_Log_File> LogFiles { get; set; } 
		
		public API_IIS_Logs()
		{
			LogFiles = new List<IIS_Log_File>();
		}
	}
	
	public class IIS_Log_File
	{
		public string File		 				{ get; set; } 
		public List<IIS_Log_Entry> LogEntries	{ get; set; }
		public List<string> Comments 			{ get; set; }
		
		public IIS_Log_File()
		{
			LogEntries = new List<IIS_Log_Entry>();
			Comments   = new List<string>();	
		}
	}
	
	public class IIS_Log_Entry
	{
		//[XmlAttribute]public string RawLine  			{ get; set; }
		[XmlAttribute] public string Date				{ get; set; }	
		[XmlAttribute] public string Time				{ get; set; }	
		[XmlAttribute] public string S_sitename		{ get; set; }	
		[XmlAttribute] public string S_ip				{ get; set; }	
		[XmlAttribute] public string Cs_method			{ get; set; }	
		[XmlAttribute] public string Cs_uri_stem		{ get; set; }	
		[XmlAttribute] public string Cs_uri_query		{ get; set; }	
		[XmlAttribute] public string S_port			{ get; set; }	
		[XmlAttribute] public string Cs_username		{ get; set; }	
		[XmlAttribute] public string C_ip				{ get; set; }	
		[XmlAttribute] public string Cs_User_Agent_		{ get; set; }	
		[XmlAttribute] public string Cs_host			{ get; set; }	
		[XmlAttribute] public string Cs_Referer_		{ get; set; }	
		[XmlAttribute] public string Cs_Cookie_			{ get; set; }	
		[XmlAttribute] public string Sc_status			{ get; set; }	
		[XmlAttribute] public string Sc_substatus		{ get; set; }	
		[XmlAttribute] public string Sc_win32_status	{ get; set; }	
		
		public IIS_Log_Entry()
		{
		}
				
	}	
	
	
	public static class IIS_Log_File_ExtensionMethods 
	{
		public static IIS_Log_File logFile(this string file)
		{
			return file.load<IIS_Log_File>();
		}
	}
	
	public static class IIS_Log_File_ExtensionMethods_Convert
	{
		public static List<string> logFiles_Convert_and_Save(this List<string> files, string targetFolder)
		{
			var convertedFiles = new List<string>();
			foreach(var file in files)
				convertedFiles.add(file.logFile_Convert_and_Save(targetFolder));
			return convertedFiles; 
		}
		
		public static string logFile_Convert_and_Save(this string file, string targetFolder)
		{
			return file.logFile_Convert().saveToFolder(targetFolder);
		}
		
		public static IIS_Log_File logFile_Convert(this string file)
		{
			var o2Timer = new O2Timer("created IIS_Log_File object from file: {0}".format(file)).start();
			var logFile = new IIS_Log_File();
			logFile.File = file;
			logFile.convertData();
			o2Timer.stop();
			return logFile;
		}
		
		public static IIS_Log_File convertData(this IIS_Log_File logFile)
		{
			var columns= new List<string>();
			if(logFile.File.fileExists())
			{
				foreach(var line in logFile.File.fileContents().split_onLines())
				{
					if (line.starts("#"))
					{
						logFile.Comments.add(line);
						if (line.starts("#Fields: "))
						{
							columns = line.remove("#Fields: ").split_onSpace();
							//"there are {0} columns".debug(columns.size());
						}
					}					
					else
					{
						var logEntry = new IIS_Log_Entry();
						//logEntry.RawLine = line;
						var items = line.split_onSpace();
						for(int i=0; i < items.size() ; i++)
						{
							//"item #{0} : {1}".info(columns[i], items[i]);
							var propertyName = columns[i].upperCaseFirstLetter().replace("-","_").replace("(","_").replace(")","_");
							logEntry.prop(propertyName, items[i]);
						}
						logFile.LogEntries.Add(logEntry);						
					}
				}
			}
			return logFile;
		}
		
		public static string saveToFolder(this IIS_Log_File logFile, string targetFolder)
		{
			if(logFile.File.fileExists())
			{
				var targetFile = targetFolder.pathCombine(logFile.File.fileName() + ".xml");
				if (logFile.saveAs(targetFile))
					return targetFile;
			}
			
			return null;
		}
	}
}
    