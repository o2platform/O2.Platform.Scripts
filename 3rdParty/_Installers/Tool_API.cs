// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.REPL;
using FluentSharp.REPL.Utils;
using FluentSharp.WinForms;
//O2File:_7_Zip.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	// this calls exists to simplify the creation of new Tool APIs
	public class Tool_API
    {   
    	public static string CODEPLEX_BUILD_ID	= "20748"; 
    	
    	public string ToolName 					{ get; set; }
    	public string Version 					{ get; set; }    	
    	public string VersionWebDownload 		{ get; set; } 
    	public string Install_File 				{ get; set; }
    	public Uri 	  Install_Uri 				{ get; set; }
    	public string Install_Dir 				{ get; set; }     	    
    	public string DownloadedInstallerFile 	{ get; set; }
    	public string Executable 				{ get; set; } 
    	public string Executable_Name 			{ get; set; }
    	public string ToolsDir 					{ get; set; }
    	public string InstallProcess_Arguments 	{ get; set; }
    	//public string  toolsDir = PublicDI.config.O2TempDir.pathCombine("..\\_ToolsOrAPIs").createDir();
    	public string  localDownloadsDir = PublicDI.config.O2TempDir.pathCombine("..\\_O2Downloads").createDir();
    	public string  s3DownloadFolder = "http://s3.amazonaws.com/O2_Downloads/";
    	public Process Install_Process {get;set;}
    	public string ProgramFilesFolder = Environment.ExpandEnvironmentVariables("%ProgramFiles%");
    	//public bool Instaled { get { return isInstalled(); } }
    	
    	public Tool_API()
    	{
    		ToolsDir = PublicDI.config.ToolsOrApis;
    		ToolName = "";
    		Install_File = "";    		
    		Install_Dir = "";
    		Version = "";
    		VersionWebDownload = "";    		
    	}    	    	    	    	
    	 
    	public void config()
    	{
    		if (!Install_Dir.valid())
    			Install_Dir = ToolsDir.pathCombine(ToolName);
    		if (!Executable.valid() && Executable_Name.valid())
    			Executable = Install_Dir.pathCombine(Executable_Name);    		
    		if (!Install_File.valid() && Install_Uri.notNull() && this.Install_Uri.Segments.size()>0)
				Install_File = Install_Uri.Segments.Last();	
    	}
    	
    	public void config(string toolName, Uri installUri)
    	{
    		config(toolName,installUri,null);
    	}
    	public void config(string toolName, Uri installUri, string pathToExecutable)
    	{
    		var installFile = installUri.AbsolutePath.split("/").last();
    		config(toolName, installFile, installUri ,pathToExecutable);
    	}
    	
    	public void config(string toolName, string installFile,  Uri installUri_)
    	{
    		config(	toolName,installFile,installUri_);
    	}
    	public void config(string toolName, string installFile,  Uri installUri, string pathToExecutable)
    	{
    		ToolName = toolName;
    		Install_Dir = ToolsDir.pathCombine(toolName);    		
    		Install_Uri = installUri;
    		Install_File = installFile;
    		if (pathToExecutable.valid())
    			Executable = Install_Dir.pathCombine(pathToExecutable);
    		
    	
    	}
    	
    	public void config(string toolName)
    	{
    		config(toolName,default(string),default(string));
    	}
    	
    	public void config(string toolName, string version)
    	{
    		config(toolName,version, default(string));
    	}
    	
    	public void config(string toolName, string version, string installFile)
    	{
    		ToolName = toolName;
    		Version = version;
    		Install_File = installFile;
    		Install_Dir = ToolsDir.pathCombine(toolName);    		
    	}    	    	
    	
    	public void config(string toolName, string version, string instalDir, string installFile, Uri installUri)
    	{
    		ToolName = toolName;
    		Version = version;    		
    		Install_Dir = instalDir;
    		Install_Uri = installUri;
    		Install_File = installFile;
    		VersionWebDownload =  Install_Uri.str();
    	}
    	
    	public virtual bool isInstalled()
    	{
    		return isInstalled(true);
    	}
    	
    	public virtual bool isInstalled(bool showLogMessage)
    	{    	
    		if (Install_Dir.dirExists() && Executable.fileExists()) // we need both or some install sequences will fail
    		{    		    			
    				"{0} tool is installed because installation and main executable dir was found: {1} : {2}".debug(ToolName, Install_Dir, Executable);
    				return true;    			
    		}
    		if (showLogMessage)
    			"{0} tool is NOT installed because installation dir or main Executable was NOT found: {1} : {2}".debug(ToolName, Install_Dir, Executable);
    		return false;
    	}    	    	
    	    	
    	public virtual bool installFromMsi_Local(string pathToMsi)
    	{    		
    		if (isInstalled(false).isFalse() && pathToMsi.fileExists())    							
				Processes.startProcess(pathToMsi, this.InstallProcess_Arguments).WaitForExit();
			return isInstalled();
    	}
    	
    	public virtual bool startInstaller_FromMsi_Web()
    	{    		
    		return install((msiFile)=> startInstaller_FromMsi_Local(msiFile));
    	}
    	
    	public virtual bool startInstaller_FromMsi_Local(string pathToMsi)
    	{    		
    		if (isInstalled(false).isFalse() && pathToMsi.fileExists())    							
				Install_Process = Processes.startProcess(pathToMsi,this.InstallProcess_Arguments);
			return isInstalled();
    	}
    	
    	//this will just download the installer (with the file in 
    	public virtual string installerFile()
    	{    		
    		DownloadedInstallerFile = download(Install_File);
    		if (DownloadedInstallerFile.fileExists())
    			return DownloadedInstallerFile;    		
    		"[Tool_API] could not download Install_File: {0}".error(DownloadedInstallerFile);
    		return null;    		
    	}
    	
    	public virtual bool installFromMsi_Web(string url)
    	{
    		VersionWebDownload = url;
    		return installFromMsi_Web();
    	}
    	
    	public virtual bool installFromExe_Web()
    	{
    		return installFromMsi_Web();
    	}
    	
    	public virtual bool installFromMsi_Web()
    	{    		
    		return install((msiFile)=> installFromMsi_Local(msiFile));
    	}    	    	
    	 
    	public virtual bool installFromZip_Web()
    	{    	
    		if (Install_Dir.valid().isFalse())
    		{
    			"Install_Dir is not set, aborting installation".error();
    			return false;
    		}
    		Action<string> onDownload = 
    			(zipFile)=>{    							
    							if (zipFile.fileExists())  
    							{
    								if (zipFile.extension(".7z") || zipFile.extension(".rar"))
    									new _7_Zip().unzip(zipFile,Install_Dir);
    								else
    									new zipUtils().unzipFile(zipFile,Install_Dir);                               
    							}
    					   };
    		
    		return install(onDownload);
    			
    	}
    	
    	public virtual bool installFromWeb_Jar()
    	{
    		if (Install_Dir.valid().isFalse())
    		{
    			"Install_Dir is not set, aborting installation".error();
    			return false;
    		}
    		Action<string> onDownload = 
    			(jarFile)=>{
    							if (jarFile.fileExists())    							
    							{
    								Install_Dir.createDir();
    								Files.moveFile(jarFile, Install_Dir);    								
    							}
    					   };    		
    		return install(onDownload);
    	}
    	
    	public bool install_JustDownloadFile_into_TargetDir()
    	{
			if (isInstalled())    		
    			return true;
    		if (Executable.valid().isFalse())
    		{
    			"in install_JustDownloadFile_into_TargetDir for Tool {0} , a valid Executable value must be provided: {1}".error(ToolName, Executable);
    			return false;
    		}
    		Executable.str().directoryName().createDir();
    		Install_Uri.str().download(Executable);
    		return isInstalled();	
		}
				
		public bool install_JustMsiExtract_into_TargetDir()
		{
			if (isInstalled())    		
    			return true;
    		DownloadedInstallerFile = download(Install_Uri);			
			"LessMsi_Install.cs".local().executeFirstMethod(); // this will install LessMsi
			 var assembly = "API_LessMsi.cs".local().compile();
			 var extractMsi = assembly.type("API_LessMSI").ctor();
			 return (bool)extractMsi.invoke("extractMsi",DownloadedInstallerFile,Install_Dir); 
		}
		
		
    	public bool install(Action<string> onDownload)
    	{
			if (isInstalled())    		
    			return true;

    		if (Install_File.valid().isFalse())
    		{
    			"in Install for Tool {0} , a valid InstallFile must be provided: {1}".error(ToolName, Install_File);
    			return false;
    		}
    		"Installing: {0}".debug(ToolName);
    		"Application not installed, so installing it now".info();
    		DownloadedInstallerFile = download(Install_File);
			if (DownloadedInstallerFile.fileExists())   
                	onDownload(DownloadedInstallerFile);                	    		
    		if (isInstalled())
            {
            	"{0} installed ok".info(Version);
            	return true;
            }
            "There was a problem installing the {0}".error(Version);
            return false;
    	}
    	    	
    	public string download(string file)
    	{
    		"Downloading file: {0}".info(file);    	    		
    		var localPath = localDownloadsDir.pathCombine(file);
    		if (localPath.fileExists())
    		{
    			if (localPath.extension(".zip") && localPath.isZipFile().isFalse())
    			{    				
    				"found previously downloaded zip file, but it was corrupted, so downloading it again: {0}".error(localPath);
    				localPath.delete_File();
    			}
    			else
    			{
    				"found previously downloaded copy: {0}".info(localPath);
    				return localPath;
    			}
    		}    		
    		
    		var s3Path = "{0}{1}".format(s3DownloadFolder, file);    		
    		if (s3Path.httpFileExists())
    		{
    			"found file at S3: {0}".info(s3Path);
    			var downloadedFile =  download(s3Path.uri());
    			//"S3 file downloaded to: {0}".info(downloadedFile);
    			return downloadedFile;
    		}    		
    		return download(Install_Uri);
    	}
    		    	
    	public string download(Uri uri)
    	{
    		var targetFile = tempLocationOf_Install_File();//localDownloadsDir.pathCombine(Install_File);
    		if (targetFile.fileExists())
    		{
    			"found previously downloaded copy: {0}".info(targetFile);
    			return targetFile;
    		}
    		"Downloading from Uri: {0}".info(uri);
    		if (uri.isNull())
    			return null;
    		VersionWebDownload = uri.str();
    		
            var downloadedFile = VersionWebDownload.download();
             
            if (downloadedFile.fileExists())
			{
             	for(int i =0 ; i< 5; i++) // try to get the file 5 times since sometimes the file is still not available (due to AV)
             	{
					this.sleep(1000);	            
	            	"Copying file: {0}".info(targetFile);
	            	if (Files.copy(downloadedFile, targetFile).valid())
	            	{
	            		"Deleting file: {0}".info(downloadedFile);
	            		Files.deleteFile(downloadedFile);                	
	            	}
	            	if (targetFile.fileExists())
	            		return targetFile;	            	
	            		            	
	            }	            
	         };
            return null;            
    	}
    	
    	public virtual bool unInstall()
    	{
    		if (Install_Dir.valid() && Install_Dir.dirExists())
    		{
    			"Uninstalling tool {0} by deleting folder {1}".debug(ToolName, Install_Dir);
    			Files.deleteFolder(Install_Dir,true);
    			return isInstalled().isFalse();
    		}
    		return false;
    	}
    	
    	public string executableFullPath()
    	{
    		return Install_Dir.pathCombine(Executable_Name);
    	}
    	
    	public string tempLocationOf_Install_File()
    	{
    		return localDownloadsDir.pathCombine(Install_File);;
    	}    	    	
		
		public void delete_Install_Dir()
		{
			if(Install_Dir.valid())
				if(Install_Dir.contains(PublicDI.config.O2TempDir.directoryName()))
				{
					"[delete_Install_Dir] deleting folder: {0}".info(Install_Dir);
					Files.deleteFolder(Install_Dir,true);
				}
				else
				{
					"[delete_Install_Dir] deleting is only supported for folders inside the O2 Temp dir: {0}".error(Install_Dir);
				}
			else
				"[delete_Install_Dir] Install_Dir value was not set".error();					
		}
		
		//use this if the intallation failed and we need to delete the temp download file and/or the Install directory
		public void cleanInstallTargets()		
		{
			var tempLocalDownload = tempLocationOf_Install_File().info();
			if (tempLocalDownload.fileExists())
				Files.deleteFile(tempLocalDownload);
			delete_Install_Dir();
		}
		
		public string buildCodePlexDownloadUri(string projectName, int downloadId, long fileTime)
		{			
			 var codePlexBuildId =  "codePlex_Html".o2Cache<string>("http://www.codeplex.com/".GET)
										.lines_RegEx("<li>Version.*</li>")
									  	.first().trim()
									   	.split(".").last()
									   	.remove("</li>");
			if(codePlexBuildId.valid())
			{
				"[ToolAPI] Found CodePlex Build Id: {0}".info(codePlexBuildId);
				var downloadUrl = "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName={0}&DownloadId={1}&FileTime={2}&Build={3}"
									.format(projectName, downloadId, fileTime, codePlexBuildId);
				"[ToolAPI] Mapped DownloadUrl to : {0}".info(downloadUrl);
				return downloadUrl;
			}
			"[ToolAPI] Could not find CodePlex Build id".error();
			return null;
		}
		
    }
}