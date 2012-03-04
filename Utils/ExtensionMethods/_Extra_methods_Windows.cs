// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
 
//O2File:_Extra_methods_Misc.cs
 
namespace O2.XRules.Database.Utils
{	
	public static class _Extra_Windows_ExtensionMethods
	{
		
	}
	
		public static class _Extra_Processes_ExtensionMethods
	{		
		public static string startProcess_getConsoleOut(this string processExe)
		{
			return processExe.startProcess_getConsoleOut("");
		}
		
		public static string startProcess_getConsoleOut(this string processExe, string arguments)
		{
			return Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(processExe, arguments);
		}
		
		public static string startProcess_getConsoleOut(this string processExe, string arguments, string workingDirectory)
		{
			return Processes.startAsCmdExe(processExe, arguments, workingDirectory);
		}
		
		
		public static Process startProcess(this string processExe, Action<string> onDataReceived)
		{
			return processExe.startProcess("", onDataReceived);
		}
		
		public static Process startProcess(this string processExe, string arguments, Action<string> onDataReceived)
		{
			return Processes.startProcessAndRedirectIO(processExe, arguments, onDataReceived);			
		}
		
		public static Process startProcess(this string processExe, string arguments)
		{
			return Processes.startProcess(processExe, arguments);
		}
		
		public static Process startProcess(this string processExe)
		{			
			return Processes.startProcess(processExe);
		}
		
		public static Process close(this Process process)
		{
			return process.stop();
		}
		
		public static Process closeInNSeconds(this Process process, int seconds)
		{
			O2Thread.mtaThread(
				()=>{
						process.sleep(seconds*1000);
						"Closing Process:{0}".info(process.ProcessName);
						process.stop();
					});
			return process;
		}
		
		public static Process startH2(this string scriptFile)
		{
			return scriptFile.executeH2_or_O2_in_new_Process();
		}
		
		public static Process executeH2_or_O2_in_new_Process(this string scriptFile)
		{
			"[executeH2_or_O2_in_new_Process] executing: {0}".info(scriptFile);
			if(scriptFile.fileExists())
				return scriptFile.startProcess();
			else
			{
				scriptFile = scriptFile.local();
				if(scriptFile.fileExists())
					return scriptFile.startProcess();
			}
			"[executeH2_or_O2_in_new_Process] could not find O2 or H2 script to execute: {0}".error(scriptFile);
			return null;
		}
		
		public static Process executeH2_as_Admin_askUserBefore(this string scriptName)
		{
			if ("It looks like your current account doesn't have the rights to run this script, do you want to try running this script with full priviledges?".askUserQuestion())				
				return scriptName.executeH2_as_Admin();
			return null;
		}
		
		public static Process executeH2_as_Admin(this string scriptToExecute)
		{
			var process = new Process();
			process.StartInfo.FileName  = PublicDI.config.CurrentExecutableDirectory.pathCombine("O2_XRules_Database.exe");
			process.StartInfo.Arguments = "\"{0}\"".format(scriptToExecute);
			process.StartInfo.Verb = "runas";
			process.Start();
			return process;

		}
	}

	
		//REGISTRY
	public static class _Extra_RegistryKeyExtensionMethods
    {    
    	public static string makeDomainTrusted(this string rootDomain, string subDomain)
    	{
			try
			{				
				var ieKeysLocation = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\";
				//var domainsKeyLocation =  ieKeysLocation + "Domains";
				var domainsKeyLocation =  ieKeysLocation + "EscDomains";			    
				var trustedSiteZone = 0x2;
				RegistryKey currentUserKey = Registry.CurrentUser; 
				currentUserKey.getOrCreateSubKey(domainsKeyLocation, rootDomain, false); 
				currentUserKey.createSubDomainKeyAndValue(domainsKeyLocation, rootDomain, subDomain, "http",trustedSiteZone); 
				currentUserKey.createSubDomainKeyAndValue(domainsKeyLocation, rootDomain, subDomain, "https",trustedSiteZone); 
				var message = "Added as truted the domain: {1}.{0}".format(rootDomain,subDomain);
				return message;
			}
			catch(Exception ex)
			{
				ex.log("in makeDomainTrusted");
				return ex.Message;
			}
		}
    
        public static RegistryKey getOrCreateSubKey(this RegistryKey registryKey, string parentKeyLocation, string key, bool writable)
        {
            string keyLocation = string.Format(@"{0}\{1}", parentKeyLocation, key);
            RegistryKey foundRegistryKey = registryKey.OpenSubKey(keyLocation, writable);
            return foundRegistryKey ?? registryKey.createSubKey(parentKeyLocation, key);
        }

        public static RegistryKey createSubKey(this RegistryKey registryKey, string parentKeyLocation, string key)
        {
            RegistryKey parentKey = registryKey.OpenSubKey(parentKeyLocation, true); //must be writable == true
            if (parentKey == null) 
            	 throw new NullReferenceException(string.Format("Missing parent key: {0}", parentKeyLocation)); 
            RegistryKey createdKey = parentKey.CreateSubKey(key);
            if (createdKey == null) 
            	throw new Exception(string.Format("Key not created: {0}", key));
            return createdKey;
        }
        
        //IE Specific
        public static void createSubDomainKeyAndValue(this RegistryKey currentUserKey, string domainsKeyLocation, string domain, 
        											   string subDomainKey, string subDomainValue, int zone)
        {
            RegistryKey subdomainRegistryKey = currentUserKey.getOrCreateSubKey(string.Format(@"{0}\{1}", domainsKeyLocation, domain), subDomainKey, true);
            object objSubDomainValue = subdomainRegistryKey.GetValue(subDomainValue);
            if (objSubDomainValue == null || Convert.ToInt32(objSubDomainValue) != zone)            
                subdomainRegistryKey.SetValue(subDomainValue, zone, RegistryValueKind.DWord);           
        }
	}   

		public static class _Extra_Console_ExtensionMethods
	{
		public static MemoryStream capture_Console(this string firstLine)
		{
			var memoryStream = new MemoryStream();
			memoryStream.capture_Console();  
			Console.WriteLine(firstLine);
			return memoryStream;
		}
		public static MemoryStream capture_Console(this MemoryStream memoryStream)
		{
			return memoryStream.capture_ConsoleOut()
							   .capture_ConsoleError(); 
		}
		public static MemoryStream capture_ConsoleOut(this MemoryStream memoryStream)
		{
			var streamWriter = new StreamWriter(memoryStream);
			System.Console.SetOut(streamWriter);
			streamWriter.AutoFlush = true;
			return memoryStream;
		}
		
		public static MemoryStream capture_ConsoleError(this MemoryStream memoryStream)
		{
			var streamWriter = new StreamWriter(memoryStream);
			System.Console.SetError(streamWriter);
			streamWriter.AutoFlush = true;
			return memoryStream;
		}
		
		public static string readToEnd(this MemoryStream memoryStream)
		{
			memoryStream.Position =0;
			return new StreamReader(memoryStream).ReadToEnd();
		}
	}
}
    	