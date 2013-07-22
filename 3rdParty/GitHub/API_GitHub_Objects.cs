// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Utils;
using FluentSharp.REPL;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using GitModel = GithubSharp.Core.Models;
using GitAPI = GithubSharp.Core.API;
using GithubSharp.Plugins.LogProviders.SimpleLogProvider; 
//O2Ref:GithubSharp.Core.dll
//O2Ref:BasicCacher.dll
//O2Ref:GithubSharp.Plugins.LogProviders.SimpleLogProvider.dll


namespace O2.XRules.Database.APIs
{
	public class API_GitHub_Objects_Test
	{
		public void showGui()
		{
/*			//var repository = "What is the repository you want to see?".askUser();
			//var repository =  "SecurityInnovation/YASAT";						
			var repository =  "SecurityInnovation/TeamMentor";						
			var gitHubIssues = new API_GitHub_Issues(); 									
			gitHubIssues.setupRepository(repository);
			gitHubIssues.login();
			gitHubIssues.showEditor();*/
		}
	}
	
    public class API_GitHub_Objects
    {    	
    	public GitAPI.Object ObjectsAPI { get; set; }
    	public string Repository { get; set; }
    	public bool LoggedIn {get; set;}
    	
    	public API_GitHub_Objects()
    	{
    		
    	}    	    
    	public void refreshCacher() // need to see why this is needed between certain types of requests
    	{
    		checkIfCanMakeRequest();
    		if (LoggedIn)
    			ObjectsAPI.CacheProvider = new BasicCacher.BasicCacher();
    	}
    	
    	public DateTime currentTime;
    	public int requestsThisMinute;
    	
    	public API_GitHub_Objects checkIfCanMakeRequest()
    	{
    		if (currentTime.AddMinutes(1) < DateTime.Now)
    		{
    			currentTime = DateTime.Now;
    			requestsThisMinute=0;
    		}
    		else
    		{    				
	    		if (requestsThisMinute > 55)
	    		{
	    			"in checkIfCanMakeRequest, requestsThisMinute are {0}, so we will wait a little bit".error(requestsThisMinute);
	    			this.sleep(10*1000);
	    			checkIfCanMakeRequest();
	    		}
	    		else
	    			requestsThisMinute++ ;  
	    	}
	    	//"requestsThisMinute: {0} ".info(requestsThisMinute);
	    	return this;
    	}
    	
	}
	
	//make this generic (since both IssuesAPI and ObjectsAPI use this)
	public static class API_GitHub_Objects_ExtensionMethods_SetupAndLogin
	{
		public static API_GitHub_Objects setupRepository(this API_GitHub_Objects gitHubObjects , string repository)
    	{
    		gitHubObjects.Repository = repository;    		
			return gitHubObjects;    		
    	}
    	
    	public static API_GitHub_Objects login(this API_GitHub_Objects gitHubObjects )
    	{
    		var gitHubLogin = @"C:\O2\_USERDATA\accounts.xml".credential("githubApiKey");   
			var name = gitHubLogin.username();
			var apiToken = gitHubLogin.password();
			return gitHubObjects.login(name, apiToken);
    	}
    	
    	public static API_GitHub_Objects login(this API_GitHub_Objects gitHubObjects , string name, string apiToken)
    	{
    		gitHubObjects.checkIfCanMakeRequest();
    		try
    		{	    		
	    		var Cache = new BasicCacher.BasicCacher(); 
				var Log = new SimpleLogProvider();
							 
				var user = new GitModel.GithubUser { Name = name, APIToken = apiToken };									
				gitHubObjects.ObjectsAPI = new GitAPI.Object(Cache, Log);
				gitHubObjects.ObjectsAPI.Authenticate(user);   
				gitHubObjects.LoggedIn = true;
			}
			catch(Exception ex)
			{
				"Error while logging in to GitHub using user {0}".info(name);
				ex.log("in API_GitHub_Objects.login");
			}
			return gitHubObjects;
    	}
    }    
    
    public static class API_GitHub_Objects_ExtensionMethods_Objects
    {
    	public static  List<string> names(this List<GitModel.Object> objects)
    	{
    		return (from _object in objects
    			    select _object.Name).toList(); 
    	}
    	
    	public static  List<GitModel.Object> folders(this List<GitModel.Object> objects)
    	{
    		return (from _object in objects
    				where _object.Type == "tree"	
    			    select _object ).toList();  
    	}
    	
    	public static  List<GitModel.Object> files(this List<GitModel.Object> objects)
    	{
    		return (from _object in objects
    				where _object.Type == "blob"	
    			    select _object).toList(); 
    	}
    	
    	public static string fileContent(this API_GitHub_Objects gitObjects, string fileName, string sha)
    	{    		    		
    		gitObjects.refreshCacher();
    		"Fetching file {0} with sha {1}".debug(fileName, sha);
    		var blob = gitObjects.ObjectsAPI.Blob(gitObjects.Repository, sha ,fileName);	
    		if (blob.notNull())  
    		{
    			if (blob.Data.starts("﻿"))
    				return blob.Data.Substring(3);
    			return blob.Data;
    		}
    		return null;    		    		
    	}
    	/*
    	if (doBinaryDownload)
											{
												var blobBinaryContent = objects.RawBinary(currentRepository, sha);								
												fileContents = blobBinaryContent.ascii();
											}
											else
											{
												var blobContent = objects.Blob(currentRepository, sha ,fileName);				
												fileContents = blobContent.Data;
											}
		*/											
    	
    }
    
    public static class API_GitHub_Objects_ExtensionMethods_Trees
    {
    	public static  List<GitModel.Object> tree(this API_GitHub_Objects gitObjects, string shaTree)
    	{    		    		
    		gitObjects.refreshCacher();
    		var results = gitObjects.ObjectsAPI.Trees(gitObjects.Repository, shaTree).toList();
    			"Fectched {0} objects from tree: {0}".info(results.size(),shaTree);
    			return results;    		    		
    	}						
	}    
    
    public static class API_GitHub_Objects_ExtensionMethods_GUIs
	{
	    public static API_GitHub_Objects show_GitHub_Browser(this API_GitHub_Objects gitHubObjects)
	    {
	    	var topPanel = O2Gui.open<Panel>("Util - GitHub Browser",1000,400);
	    	return gitHubObjects.login().add_GitHub_Browser(topPanel);
	    }
	    
	    
	    public static API_GitHub_Objects add_GitHub_Browser(this API_GitHub_Objects gitHubObjects , Control control)
	   	{
	    	var objects = gitHubObjects.ObjectsAPI;
	    	
	    	var topPanel = control;
	    	var actionPanel = topPanel.insert_Above(40,"Load repositories"); 
	    	
	
			var currentRepository = "";
			var doBinaryDownload = false;
			var saveFechedFiles = false;
			var folderToSaveSelectedFiles = "";
			Action<string, TreeNode> addTree =
				(shaTree, treeNode)=>
					{
						"on repository {0} , fetching tree for ShaTree: {1}".info(currentRepository, shaTree);
						objects.CacheProvider = new BasicCacher.BasicCacher(); 
						var treeItems = objects.Trees(currentRepository, shaTree);
						
						treeNode.add_Nodes(treeItems, 
										   (treeItem)=>treeItem.Name, 
										   (treeItem)=> (treeItem.Type == "tree") ? (object)treeItem   		// if this is tree return the object
										   										  : (doBinaryDownload)		// if it is a file
										   										  			? (object)treeItem.Sha	//for binary downloads we need to store the item's sha
										   										  			: shaTree,				//for ascii downloads we need to store the tree's sha
										   (treeItem)=> treeItem.Type == "tree" 
										  );
						"There are {0} nodes".info(treeNode.nodes().size());		 						  
						
						foreach(var node in treeNode.nodes())		
						{
							node.color( (node.nodes().size()==0)
												? Color.DarkBlue // it is a file
												: Color.DarkOrange // it is a tree (i.e. a folder
										  );
						}
			
						//treeNode.nodes()[0].selected();
						
					};
			
			var treeView = topPanel.add_TreeView();
			
			var sourceCodeViewer = treeView.insert_Right().add_SourceCodeViewer(); 
			var propertyGrid = treeView.insert_Below(100).add_PropertyGrid().helpVisible(false); 
			
			
			treeView.beforeExpand<GitModel.Object>(
				(treeNode, _object)=>{							
										addTree(_object.Sha, treeNode); 
									 });
			
			treeView.afterSelect<string>(
				(sha)=> {						
							treeView.backColor(Color.Azure);
							O2Thread.mtaThread(
								()=>{
										try
										{
											var fileName = treeView.selected().get_Text().urlEncode();
											objects.CacheProvider = new BasicCacher.BasicCacher(); 		
											string fileContents = "";
											if (doBinaryDownload)
											{
												var blobBinaryContent = objects.RawBinary(currentRepository, sha);								
												fileContents = blobBinaryContent.ascii();
											}
											else
											{
												var blobContent = objects.Blob(currentRepository, sha ,fileName);				
												fileContents = blobContent.Data;
											}
																			
											sourceCodeViewer.set_Text(fileContents, fileName);  								
											
											if (saveFechedFiles)  // need a better solution since this is not taking into account the current folder
											{
												var localFile = folderToSaveSelectedFiles.pathCombine(fileName);																
												"saving fetched file to: {0}".info(localFile);									
												fileContents.saveAs(localFile);
											}
										}
										catch(Exception ex)
										{
											sourceCodeViewer.set_Text(ex.Message); 
										}
										
										treeView.backColor(Color.White); 
									});					
						});
						
			treeView.afterSelect<GitModel.Object>(
				(_object)=> {					
								propertyGrid.show(_object); 
								if (_object.Type == "blob")
								{ 
									var sha = treeView.selected().get_Tag();
									"Parent sha is: {0}".info(sha);
									return ;																		
									
								}
								
							});
			
			
			var actionsPanel = topPanel.insert_Above(50);
			var repository_TextBox = actionsPanel.add_TextBox(0, "repository", "").left(100);
			var rootSha_TextBox = actionsPanel.add_TextBox(20,"root tree (Sha)", "").left(100);
			
			
			Action loadRootTree = 
				()=>{ 
						currentRepository = repository_TextBox.get_Text();
						treeView.clear();
						var rootObject = new GitModel.Object { Name = repository_TextBox.get_Text() , Sha = rootSha_TextBox.get_Text() };
						addTree(rootObject.Sha, treeView.rootNode());
					};
					
			rootSha_TextBox.onEnter((text)=>loadRootTree());
			
			Action<string,string> load = 
				(repositoryText, rootShaText)=> {
													repository_TextBox.set_Text(repositoryText);
													rootSha_TextBox.set_Text(rootShaText);
													loadRootTree();
												};
			
			actionPanel.add_Link("SecurityInnovation/YASAT", 0,0, ()=> load("SecurityInnovation/YASAT", "2d94d5d39168e38d2a10"))
					   .append_Link("SecurityInnovation/TeamMentor", ()=> load("SecurityInnovation/TeamMentor", "71419d1c49f6d9c5a6c1"))		   
					   .append_Link("DinisCruz/TeamMentor", ()=> load("DinisCruz/TeamMentor", "dc03519a8e0d766f224f")).click();
			
			actionPanel.add_CheckBox("Download selected files as Binaries Blobs", 0,500, (value)=> doBinaryDownload = value).autoSize();
			actionPanel.add_CheckBox("Save fecthed files to folder", 0,750, (value)=> saveFechedFiles = value).autoSize()
					   .append_TextBox("").align_Right(actionPanel)
					   .onTextChange((text)=> folderToSaveSelectedFiles = text)
					   .set_Text("_SavedGitFiles".tempDir(false));
	
	    return gitHubObjects;
	    }	    
	}
}