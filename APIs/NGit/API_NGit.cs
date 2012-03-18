// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using NGit.Api;
using NGit;

//O2File:_Extra_methods_Collections.cs
//O2File:_Extra_methods_Items.cs

//O2Ref:NGit.dll
//O2Ref:NSch.dll
//O2Ref:Mono.Security.dll
//O2Ref:Sharpen.dll

namespace O2.XRules.Database.APIs
{
    public class API_NGit
    {    
    	 public string  	Path_Local_Repository 	{ get; set; }
    	 public Git 		Git					  	{ get; set; }
    	 public Repository	Repository			 	{ get; set; }    	    	 
    }
    
    public class GitProgress : TextProgressMonitor  
    {
    	public Action<string, string,int> onMessage   { get; set; }
    	public StringWriter 			  FullMessage { get; set; }
    	
    	public GitProgress() : this(new StringWriter())
    	{
    	}
    	public GitProgress(StringWriter stringWriter) : base(stringWriter)
    	{
    		FullMessage = stringWriter;    		
    		
			onMessage = (type, taskName, workCurr)=>
							{
								"[GitProgress] {0} : {1} : {2}".info( type, taskName, workCurr);
							};
    	}
    	
    	public override void Start(int totalTasks)
    	{
    		//onMessage("Start","",totalTasks);
    		base.Start(totalTasks);
    	}
    	
		public override void BeginTask(string title, int work)
		{ 
			onMessage("BeginTask", title, work);
			base.BeginTask(title,work);
		}
		
    	public override void Update(int completed)
    	{
    		//onMessage("Update", "", completed);
    		base.Update(completed);
    	}
    	
		public override void EndTask()
		{
			//onMessage("EndTask", "", -1);
			base.EndTask();
		}
    }
    
    public static class API_NGit_ExtensionMethods
    {
    	public static API_NGit init(this API_NGit nGit, string pathToLocalRepository)
    	{
    		"[API_NGit] init: {0}".debug(pathToLocalRepository);
    		var init_Command 			= Git.Init();
    		
    		init_Command.SetDirectory(pathToLocalRepository);    		
    		nGit.Git 		 			= init_Command.Call();
    		nGit.Repository  			= nGit.Git.GetRepository();     		
    		nGit.Path_Local_Repository  = pathToLocalRepository;
    		return nGit;
    	}
    	
    	public static API_NGit open(this API_NGit nGit, string pathToLocalRepository)
    	{
    		"[API_NGit] open: {0}".debug(pathToLocalRepository);
    		
    		nGit.Git 					= Git.Open(pathToLocalRepository);
    		nGit.Repository 		 	= nGit.Git.GetRepository(); 
    		nGit.Path_Local_Repository  = pathToLocalRepository;
    		return nGit;
    	}
    	
    	public static API_NGit add(this API_NGit nGit, string filePattern )
    	{
    		"[API_NGit] add: {0}".debug(filePattern);
    		
    		var add_Command = nGit.Git.Add();
    		add_Command.AddFilepattern(filePattern);
    		add_Command.Call();
    		return nGit;
    	}
    	public static API_NGit commit(this API_NGit nGit, string commitMessage )
    	{
    		"[API_NGit] commit: {0}".debug(commitMessage);
    		
    		if(commitMessage.valid())
    		{
    			var commit_Command = nGit.Git.Commit();
    			commit_Command.SetMessage(commitMessage);
    			commit_Command.Call();	
    		}
    		else
    			"[API_NGit] commit was called with no commitMessage".error();
    		return nGit;
    	}
    	
    	public static GitProgress push(this API_NGit nGit)
    	{
    		return nGit.push("origin");
    	}
    	
    	public static GitProgress push(this API_NGit nGit, string remote)
    	{
    		"[API_NGit] push: {0}".debug(remote);
    		
    		var push_Command = nGit.Git.Push();		
			push_Command.SetRemote(remote);	
			var gitProgress = new GitProgress();
			push_Command.SetProgressMonitor(gitProgress);			
			push_Command.Call(); 
			
			"[API_NGit] push completed".debug(); 
			return gitProgress;
		}
    	
    	public static API_NGit add_and_commit_using_Status(this API_NGit nGit )
    	{
    		nGit.add(".");
    		nGit.commit_using_Status();
    		return nGit;
    	}
    	public static API_NGit commit_using_Status(this API_NGit nGit )
    	{
    		nGit.commit(nGit.status());
    		return nGit;
    	}
    	
    	public static string status(this API_NGit nGit)
    	{
    		var statusCommand = nGit.Git.Status();
			var status = statusCommand.Call();
			
			var added 		= status.GetAdded().toList();
			var changed 	= status.GetChanged().toList();
			var removed 	= status.GetRemoved().toList();
			var missing 	= status.GetMissing().toList();
			var modified 	= status.GetModified().toList();
			var untracked 	= status.GetUntracked().toList();
			var conflicting = status.GetConflicting().toList();
			 
			
			var statusDetails = ((added.Count 		> 0) ? "Added: {0}\n"		.format(added.join(" , ")) : "")  + 
								((changed.Count 	> 0) ? "changed: {0}\n"		.format(changed.join(" , ")) : "") + 
								((removed.Count 	> 0) ? "removed: {0}\n"		.format(removed.join(" , ")) : "") +
								((missing.Count 	> 0) ? "missing: {0}\n"		.format(missing.join(" , ")) : "") +
								((modified.Count 	> 0) ? "modified: {0}\n"	.format(modified.join(" , ")) : "") +
								((untracked.Count	> 0) ? "untracked: {0}\n"	.format(untracked.join(" , ")) : "") + 
								((conflicting.Count > 0) ? "conflicting: {0}\n"	.format(conflicting.join(" , ")) : "");
			return statusDetails;
    	}
    	
    	
    }
    
    
    public static class API_NGit_ExtensionMethods_File_Utils
    {
    	public static API_NGit writeFile(this API_NGit nGit, string virtualFileName, string fileContents)
    	{
    		var fileToWrite= nGit.Path_Local_Repository.pathCombine(virtualFileName);
    		fileContents.saveAs(fileToWrite);
    		return nGit;
    	}
    }
}
