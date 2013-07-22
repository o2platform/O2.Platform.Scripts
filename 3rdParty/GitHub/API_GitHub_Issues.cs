// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Utils;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using GithubSharp.Core.API;
using GithubSharp.Core.Models;
using GithubSharp.Plugins.LogProviders.SimpleLogProvider;

//O2Ref:GithubSharp.Core.dll
//O2Ref:BasicCacher.dll
//O2Ref:GithubSharp.Plugins.LogProviders.SimpleLogProvider.dll


namespace O2.XRules.Database.APIs
{
	public class API_GitHub_Issues_Test
	{
		public void showGui()
		{
			//var repository = "What is the repository you want to see?".askUser();
			//var repository =  "SecurityInnovation/YASAT";						
			var repository =  "SecurityInnovation/TeamMentor";						
			var gitHubIssues = new API_GitHub_Issues(); 									
			gitHubIssues.setupRepository(repository);
			gitHubIssues.login();
			gitHubIssues.showEditor();
		}
	}
	
    public class API_GitHub_Issues
    {    	
    	public Issues IssuesAPI { get; set; }
    	public string Repository { get; set; }
    	public bool LoggedIn {get; set;}
    	
    	public API_GitHub_Issues()
    	{
    		
    	}    	    
    	public void refreshCacher() // need to see why this is needed between certain types of requests
    	{
    		if (LoggedIn)
    			IssuesAPI.CacheProvider = new BasicCacher.BasicCacher();
    	}
	}
	
	public static class API_GitHub_Issues_ExtensionMethods_SetupAndLogin
	{
		public static API_GitHub_Issues setupRepository(this API_GitHub_Issues gitHubIssues , string repository)
    	{
    		gitHubIssues.Repository = repository;    		
			return gitHubIssues;    		
    	}
    	
    	public static API_GitHub_Issues login(this API_GitHub_Issues gitHubIssues )
    	{
    		var gitHubLogin = @"C:\O2\_USERDATA\accounts.xml".credential("githubApiKey");   
			var name = gitHubLogin.username();
			var apiToken = gitHubLogin.password();
			return gitHubIssues.login(name, apiToken);
    	}
    	
    	public static API_GitHub_Issues login(this API_GitHub_Issues gitHubIssues, string name, string apiToken)
    	{
    		try
    		{
	    		//var userAPI = new Api.User(Cache, Log); 
	    		//var user = new GithubUser { Name = gitHubLogin.username(), APIToken = gitHubLogin.password() };
	    		//userAPI.Authenticate(user);
	    		var Cache = new BasicCacher.BasicCacher(); 
				var Log = new SimpleLogProvider();
							 
				var user = new GithubUser { Name = name, APIToken = apiToken };									
				gitHubIssues.IssuesAPI = new Issues(Cache, Log);
				gitHubIssues.IssuesAPI.Authenticate(user);   
				gitHubIssues.LoggedIn = true;
			}
			catch(Exception ex)
			{
				"Error while logging in to GitHub using user {0}".info(name);
				ex.log("in API_GitHub_Issues.login");
			}
			return gitHubIssues;
    	}
    }
    
    public static class API_GitHub_Issues_ExtensionMethods_Issues
    {
    	public static List<Issue> issues(this API_GitHub_Issues gitHubIssues)
    	{
    		return gitHubIssues.issues_open();
    	}
    	
    	public static List<Issue> issues_open(this API_GitHub_Issues gitHubIssues)
    	{
    		return gitHubIssues.issues(IssueState.Open);
    	}
    	
    	public static List<Issue> issues_closed(this API_GitHub_Issues gitHubIssues)
    	{
    		return gitHubIssues.issues(IssueState.Closed);
    	}
    	
		public static List<Issue> issues(this API_GitHub_Issues gitHubIssues, IssueState state)
		{			
			if (gitHubIssues.LoggedIn.isFalse())
				return new List<Issue>();
			gitHubIssues.refreshCacher();
			var issues = gitHubIssues.IssuesAPI.List(gitHubIssues.Repository,"", state)
											   .toList();
			if (issues.isNull())
			{
				"There was a problem fetching the issues from repository {0}".error(gitHubIssues.Repository);
				return new List<Issue>();
			}
			"There were {0} issues with state {1} fetched from repository {2}".info(issues.size(), state, gitHubIssues.Repository);
			return issues;
		}
		
		public static List<Comment> comments(this API_GitHub_Issues gitHubIssues, int id)
		{
			gitHubIssues.refreshCacher();
			return gitHubIssues.IssuesAPI.Comments(gitHubIssues.Repository,"", id).toList();  
		}
		
		public static List<string> labels(this API_GitHub_Issues gitHubIssues)
		{
			return (gitHubIssues.LoggedIn) 
						? gitHubIssues.IssuesAPI.Labels(gitHubIssues.Repository,"").toList()
						: new List<string>();
		}
		
		public static API_GitHub_Issues edit(this API_GitHub_Issues gitHubIssues, Issue issue)
		{
			return gitHubIssues.edit(issue.Number, issue.Title, issue.Body); 
		}
		
		public static API_GitHub_Issues edit(this API_GitHub_Issues gitHubIssues, int id, string title, string body)
		{
			return gitHubIssues.save(id, title, body);
		}
		public static API_GitHub_Issues save(this API_GitHub_Issues gitHubIssues, int id, string title, string body)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.Edit(gitHubIssues.Repository, "", id, title, body);
			return gitHubIssues;			
		}
		
		public static API_GitHub_Issues open(this API_GitHub_Issues gitHubIssues, string title, string body)
		{
			return gitHubIssues.@new(title, body);
		}
		
		public static API_GitHub_Issues @new(this API_GitHub_Issues gitHubIssues, string title, string body)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.Open(gitHubIssues.Repository, "", title, body);
			return gitHubIssues;
		}
		
		public static API_GitHub_Issues close(this API_GitHub_Issues gitHubIssues, int id)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.Close(gitHubIssues.Repository, "", id);
			return gitHubIssues;
		}
		
		public static API_GitHub_Issues add_Label(this API_GitHub_Issues gitHubIssues, int id, string label)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.AddLabel(gitHubIssues.Repository, "", id, label);
			return gitHubIssues;
		}
		
		public static API_GitHub_Issues remove_Label(this API_GitHub_Issues gitHubIssues, int id, string label)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.RemoveLabel(gitHubIssues.Repository, "", id, label);
			return gitHubIssues;
		}
		
		public static API_GitHub_Issues add_Comment(this API_GitHub_Issues gitHubIssues, int id, string comment)
		{
			gitHubIssues.refreshCacher();
			gitHubIssues.IssuesAPI.CommentOnIssue(gitHubIssues.Repository, "", id, comment);
			return gitHubIssues;
		}
		
	}	
	public static class API_GitHub_Issues_ExtensionMethods_GUI
	{
		public static TreeView showIssues(this TreeView treeView, List<Issue> issues)
		{			
			treeView.clear();
			if (issues.isNull())
				return treeView;
			var byUser = new Dictionary<string, List<Issue>>();
			var byLabel = new Dictionary<string, List<Issue>>();

			foreach(var issue in issues)
			{
				byUser.add(issue.User, issue);
				foreach(var label in issue.Labels)
					byLabel.add(label, issue);
			}
			var allIssues = treeView.add_Node("All Issues")
									.showIssues(issues)
									.expand();
						
			treeView.add_Node("By User")
					.showIssues(byUser);
			treeView.add_Node("By Label")
					.showIssues(byLabel);
			return treeView;
		}
		
		public static TreeNode showIssues(this TreeNode treeNode, List<Issue> issues)
		{
			foreach(var issue in issues)			
				treeNode.add_Node(issue.Title, issue);
			return treeNode;		
		}
		
		public static TreeNode showIssues(this TreeNode treeNode, Dictionary<string, List<Issue>> issuesByKey)
		{
			foreach(var issueByKey in issuesByKey)
			{
				var keyTreeNode = treeNode.add_Node(issueByKey.Key);
				foreach(var issue in issueByKey.Value)			
					keyTreeNode.add_Node(issue.Title, issue);
			}
			return treeNode;		
		}
		
		
		public static Panel showEditor(this API_GitHub_Issues gitHubIssues, string repository)
		{
			gitHubIssues.setupRepository(repository);
			gitHubIssues.login();
			return gitHubIssues.showEditor();
		}
		
		public static Panel showEditor(this API_GitHub_Issues gitHubIssues)			
		{
			var topPanel = O2Gui.open<Panel>("GitHub Issues Editor", 1000,400);	
			return gitHubIssues.showEditor(topPanel);
		}

		public static T showEditor<T>(this API_GitHub_Issues gitHubIssues, T control, string repository)
			where T : Control
		{
			gitHubIssues.setupRepository(repository);
			gitHubIssues.login();
			return gitHubIssues.showEditor(control);
		}
		
		public static T showEditor<T>(this API_GitHub_Issues gitHubIssues, T control)
			where T : Control
		{
			if (gitHubIssues.Repository.inValid())
			{
				gitHubIssues.setupRepository("What is the repository you want to see?".askUser());
				gitHubIssues.login();
			}
			if (control.height() != 362)
				"in API_GitHub_Issues showEditor the height was not 362, which will make some of the controls to show out of place".error();
			var topPanel = control.add_Panel();
			topPanel.enabled(false); 
			/// this code was developed as an H2 script
			var treeView = topPanel.insert_Left<Panel>(200).add_TreeView().sort(); 
			treeView.splitContainer().splitterDistance(300);
			var actionsPanel = treeView.insert_Above<Panel>(30);
			var newIssue = treeView.insert_Below<GroupBox>(90).set_Text("New Issue").add_Panel();  
			
			Label Id = null;
			Label User = null;
			Label SavedMessage = null;
			Label CommentsLabel = null;
			Label NewCommentsLabel = null;
			TextBox Title= null;
			TextBox Body = null;
			Button SaveButton = null;
			Button CloseButton = null;
			TreeView Comments = null;
			TextBox NewComment = null;
			CheckBox EnableCloseButton = null;
			Dictionary<string, CheckBox> Labels = new Dictionary<string, CheckBox>();
			Issue CurrentIssue = null;
			
			Action closeIssue = 
				()=>{
						"Closing issue with id: {0}".info(CurrentIssue.Number);
						gitHubIssues.close(CurrentIssue.Number);
						CurrentIssue.State = "Closed"; 
						SavedMessage.set_Text("Issue closed");
					};
					
			Action saveIssue = 
				()=>{									
						var updatedLabels  = new List<string>();
						foreach(var label in Labels)
							if (label.Value.@checked())
							{
								if(CurrentIssue.Labels.toList().Contains(label.Key).isFalse())
									gitHubIssues.add_Label(CurrentIssue.Number, label.Key);																	
								updatedLabels.Add(label.Key);	 
							}
							else 
								if(CurrentIssue.Labels.toList().Contains(label.Key))
									gitHubIssues.remove_Label(CurrentIssue.Number, label.Key);	
						CurrentIssue.Labels = updatedLabels.ToArray();		
						
						if (CurrentIssue.Title != Title.get_Text() || CurrentIssue.Body != Body.get_Text())
						{
							"values changed so executing an edit".debug();
							CurrentIssue.Title = Title.get_Text();
							CurrentIssue.Body = Body.get_Text(); 
								
							gitHubIssues.edit(CurrentIssue);
						}
						var newCommentValue = NewComment.get_Text();	
						if (newCommentValue.valid())
						{
							gitHubIssues.add_Comment(CurrentIssue.Number, newCommentValue);
							CurrentIssue.Comments++; 
							Comments.add_Node(newCommentValue);
							NewComment.set_Text("");  	 							
						}
						SavedMessage.set_Text("Issue saved").textColor(Color.Green);
					};
					
			Action enableSaveButton = 
				()=>{
						SaveButton.enabled(true);
						SavedMessage.set_Text("Unsaved data").textColor(Color.Red); 
					};
					
			Action buildEditor = 
				()=>{
						var editPanel = topPanel.add_GroupBox("Selected Issue").add_Panel();
						var left = 75;			 					
						Id = editPanel.add_Label("id:",5).append_Label("").left(left);    //.font_bold() 
						User = editPanel.add_Label("user:",25).append_Label("").left(left).bringToFront() ;  
						Title = editPanel.add_TextBox(45 , "title:   ", "").left(left).align_Right(editPanel).bringToFront() ; 
						Body = editPanel.add_TextBox(65, "body: ", "").left(left).align_Right(editPanel)
								.multiLine()  
								.height(65)
								.scrollBars();								
												
						SaveButton = editPanel.add_Button("Save",280,left, ()=> saveIssue() ) 
											  .enabled(false); 				
						SavedMessage  = SaveButton.append_Label("...").topAdd(5).autoSize(); 								  						
						
						CloseButton = editPanel.add_Button("Close Issue",310,left, ()=> closeIssue()).enabled(false);
						EnableCloseButton = CloseButton.append_Control<CheckBox>().set_Text("Enable 'Close Issue' button")
																				  .autoSize().topAdd(2)
																				  .checkedChanged((value)=> CloseButton.enabled(value)); 
						
						CommentsLabel = editPanel.add_Label("comments:",150);
						Comments = CommentsLabel.append_Control<TreeView>().fill(false).left(left).align_Right(editPanel);
						NewCommentsLabel = editPanel.add_Label( "new comment:", 250);
						NewComment = NewCommentsLabel.append_TextBox("").left(left).align_Right(editPanel).bringToFront();
						
						foreach(var label in gitHubIssues.labels().sort())			
						{
							var checkBox = editPanel.add_CheckBox(130 ,left,label);
							Labels.add(label, checkBox);	     	 					
							left += checkBox.width(); 				
							checkBox.checkedChanged((value)=>enableSaveButton()); 
							checkBox.anchor_BottomLeft();
						}
						
						Title.onTextChange((text)=>enableSaveButton());
						Body.onTextChange((text)=>enableSaveButton());
						NewComment.onTextChange((text)=>enableSaveButton());
						
						Body.anchor_All();
						EnableCloseButton.anchor_BottomLeft(); 
						CloseButton.anchor_BottomLeft();
						SavedMessage.anchor_BottomLeft();
						SaveButton.anchor_BottomLeft();
						Comments.anchor_BottomLeftRight();
						CommentsLabel.anchor_BottomLeftRight();
						NewComment.anchor_BottomLeftRight();
						NewCommentsLabel.anchor_BottomLeftRight();												
					};
							
			Action<Issue> showIssue = 
				(issue)=>{							
							CurrentIssue = issue;
							"showing issue: {0}".info(issue.Title);
							topPanel.enabled(true);				
							Id.set_Text("{0}    ({1})".format(issue.Number, issue.State)); 
							Title.set_Text(issue.Title);
							User.set_Text(issue.User);
							Body.set_Text(issue.Body.fix_CRLF());  
							foreach(var label in Labels)
								label.Value.@checked(issue.Labels.toList().Contains(label.Key));				
							Comments.clear();
							Comments.enabled(false);
							if (issue.Comments > 0)				
								O2Thread.mtaThread(
									()=>{								
											foreach(var comment in gitHubIssues.comments(issue.Number))
												Comments.add_Node(comment.Body);
											Comments.enabled(true);
										});
							SavedMessage.set_Text(""); 
							SaveButton.enabled(false);
							CloseButton.enabled(false);
							EnableCloseButton.@checked(false);				
						 };
						 
			treeView.afterSelect<Issue>(
				(issue)=>{
							treeView.selected().set_Text(issue.Title);
							showIssue(issue);
						 }); 
			
			Action showOpenIssues = ()=> treeView.showIssues(gitHubIssues.issues_open());
			
			actionsPanel.add_Link("Open Issues",0,0,()=> showOpenIssues()) 
						.append_Link("Closed Issues",()=> treeView.showIssues(gitHubIssues.issues_closed()))
						.append_Link("Show all data", ()=> O2Gui.open<Panel>("Open issues Data", 1000,400 ).add_TableList().show(gitHubIssues.issues_open()));
					  
			
			Action buildNewIssue = 
				()=>{
						var newIssue_Title = newIssue.add_TextBox(0,"title:  ", "").left(40);
						var newIssue_Body = newIssue.add_TextBox(20,"body:", "").left(40);
						var newIssue_Button = newIssue.add_Button(42 , "Create").left(40);
						newIssue_Button.onClick(()=>{
														var title = newIssue_Title.get_Text();
														var body = newIssue_Body.get_Text();
														"Creating a new issue with title: {0} and body {1}".info(title, body);
														gitHubIssues.@new(title, body); 
														showOpenIssues();
														newIssue_Title.set_Text("");
														newIssue_Body.set_Text("");
													});
					};
			
			Action buildGui =
				()=>{
						buildEditor(); 
						buildNewIssue();
						showOpenIssues();
					};
					
			buildGui();		
			return control;
		}
	}	
}