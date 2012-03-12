// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;  
using System.Collections.Generic; 
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Languages_and_Frameworks.DotNet;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;
//O2File:Watin_IE_ExtensionMethods.cs
//O2Ref:WatiN.Core.1x.dll 
//O2Ref:White.Core.dll
//O2File:API_IE_ExecutionGUI.cs
//O2File:Test_TM_Config.cs

namespace O2.SecurityInnovation.TeamMentor
{
	public class API_TeamMentor_IE : API_IE_ExecutionGUI
    {    		    	    	
    
        public API_TeamMentor_IE(WatiN_IE ie) : base(ie)
    	{  
    	 	config();
    	}

		public API_TeamMentor_IE(Control hostControl)	: base(hostControl) 
    	{    		
    		config();
    	}
    	
		public void config()
    	{
    		WatiN_IE_ExtensionMethods.WAITFORJSVARIABLE_MAXSLEEPTIMES = 20;
    		this.TargetServer = Test_TM.tmServer.removeLastChar();
    	}    
    }
	
    public static class API_TeamMentor_IE_ExtensionMethods
    {
    	
    	public static string fullUri(this API_TeamMentor_IE teamMentor, string virtualPath)
    	{
    		return new Uri(teamMentor.TargetServer.uri(), virtualPath).str();
    	}
    	
    	public static API_TeamMentor_IE open(this API_TeamMentor_IE teamMentor, string virtualPath)
    	{    		
			teamMentor.ie.open(teamMentor.fullUri(virtualPath));     								
    		return teamMentor;
    	}
    	
    	public static API_TeamMentor_IE open_ASync(this API_TeamMentor_IE teamMentor, string virtualPath)
    	{    		
			teamMentor.ie.open_ASync(teamMentor.fullUri(virtualPath));     								
    		return teamMentor;
    	}
    }
    
    public static class API_TeamMentor_IE_ExtensionMethods_GuiActions
    {
    	[ShowInGui(Folder = "Gui Actions")]
    	public static API_IE_ExecutionGUI homePage(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("");    		
    		teamMentor.ie.waitForJsVariable("TM.HomePageLoaded"); 
    		return teamMentor;
    	}
    	
    	[ShowInGui(Folder = "Gui Actions")]
    	public static API_IE_ExecutionGUI mainPage_Old(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.open("/Aspx_Pages/TM_3_0.aspx");    		
    	}    	    	    	    
    	
    	[ShowInGui(Folder = "Gui Actions")]
    	public static API_IE_ExecutionGUI mainPage_New(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.open("/Aspx_Pages/TM_3_0_With_Panels.aspx");    		
    	}
    }	
    
    public static class API_TeamMentor_IE_ExtensionMethods_Login
    {
    	
    	
    	[ShowInGui(Folder = "Login")]
    	public static API_TeamMentor_IE loginPage(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.invokeScript("loadDialog","/Html_Pages/Gui/Login.Html");  
    		return teamMentor;
    		//return teamMentor.open("/Aspx_Pages/LoginUser.aspx");    		  
    	}
    	
    	[ShowInGui(Folder = "Login")]
    	public static API_TeamMentor_IE logout(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.link("Logout").click();
    		return teamMentor;
    	}
    	
    	public static API_TeamMentor_IE login(this API_TeamMentor_IE teamMentor, string username, string password)
    	{
    		teamMentor.loginPage();
    		var ie = teamMentor.ie;
			ie.waitForField("UsernameBox").value(username);
			ie.field("PasswordBox").value(password);     
			ie.button("login").click();
			return teamMentor;
		}
	}		    
    
    public static class API_TeamMentor_IE_ExtensionMethods_Website_ControlPanel
    {
    	[ShowInGui(Folder = "Control Panel")]
    	public static API_TeamMentor_IE controlPanel_in_MainGui(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.invokeScript("controlPanel");
    		return teamMentor;
    	}
    
    	[ShowInGui(Folder = "Control Panel")]
    	public static API_IE_ExecutionGUI controlPanel_StandAlone(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.open("/Aspx_Pages/ControlPanel.aspx");    		
    	}	
		
		[ShowInGui(Folder = "Control Panel")]
    	public static API_TeamMentor_IE open_FireBugLite(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.invokeEval("loadAdminPage('/Javascript/Firebug/beta/Firebug.html');");
    		return teamMentor;
    	}	
    	
    	[ShowInGui(Folder = "Control Panel")]
    	public static API_TeamMentor_IE library_Editor(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.waitForLink("Library Editor").click();
    		return teamMentor;
    	}
    	

    	    	
    }
    public static class API_TeamMentor_IE_ExtensionMethods_Website_JavaScriptMethods
    {
    	[ShowInGui(Folder = "Javascript methods")]
    	public static API_TeamMentor_IE js_homePage(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.ie.invokeEval("homePage()");
    		return teamMentor;
    	}	
    }	
    public static class API_TeamMentor_IE_ExtensionMethods_Website_TestData
    {
		[ShowInGui(Folder = "Login")]
    	public static API_TeamMentor_IE loginAs_Admin(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.login("admin", "!!tmbeta");
    	}
    	
    	[ShowInGui(Folder = "Login")]
	   	public static API_TeamMentor_IE loginAs_Editor(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.login("editor", "b");
    	}
    	
    	[ShowInGui(Folder = "Login")]
	   	public static API_TeamMentor_IE loginAs_A(this API_TeamMentor_IE teamMentor)
    	{
    		return teamMentor.login("a", "b");
    	}
    	
    	[ShowInGui(Folder = "Login")]
	   	public static API_TeamMentor_IE loginAs_3_types_of_users(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.loginAs_A();
    		teamMentor.loginAs_Editor();
    		teamMentor.loginAs_Admin();
    		return teamMentor;
    	}
    }
    
    
    
    //TO REIMLEMENT
    public static class API_TeamMentor_IE_ExtensionMethods_To_Reimplement
	{
		[ShowInGui (Folder="_to reimplement")]
		public static API_IE_ExecutionGUI __viewGuidance(this API_TeamMentor_IE teamMentor, string guid)
    	{
    		return teamMentor.open("/Aspx_Pages/ViewGuidanceItem.aspx?ItemID={0}".format(guid));    		
    	}
		
		[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __select_AuthenticationAndAuthorization(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("/");    		
    		var ie = teamMentor.ie;
    		ie.link("OWASP Top 10 2010").click();
			ie.link("Fundamentals of Security").click();
			ie.link("Authentication and Authorization").click();
    		return teamMentor;
    	}
		
		[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __clear_Applied_Filters(this API_TeamMentor_IE teamMentor) 
    	{
    		teamMentor.open("/");    		    		
    		teamMentor.ie.eval("eraseCookie('Filter')");
    		return teamMentor;
    	}
				
		[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __open_GuidanceItem(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("/");    		
    		var ie = teamMentor.ie;
    		ie.link("A01: Injection").click(); 
			while(ie.hasLink("AJAX Injection Attack").isFalse()){ ie.sleep(500);};	// replace this with an Ajax call detector
			ie.link("AJAX Injection Attack").click();
    		return teamMentor;
    	}
    	
    	[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __manageUsers(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("Aspx_Pages/ManageUsers.aspx");    		
    		var ie = teamMentor.ie;
    		
    		return teamMentor;
    	}
    	
    	[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __createNewUser(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("/");    		
    		var ie = teamMentor.ie;
    		teamMentor.logout();  
			teamMentor.open("/Aspx_Pages/CreateEdit.aspx");
			
			string username = 6.randomLetters(); 
			string password =  6.randomLetters(); ;
			//string passwordHash = "3";
			string repeatPassword = password;
			string companyBox = "aaaaa";
			string emailBox = "{0}@{0}.com".format(username); 
			
			string titleBox = "bbbbb";
			string firstName = "cccc";
			string lastName = "dddd";
			string notesBox = "eeee"; 
			
			
			ie.field("ctl00_ContentPlaceHolder1_UsernameBox",username);
			ie.field("ctl00_ContentPlaceHolder1_PasswordBox",password);
			//ie.field("ctl00_ContentPlaceHolder1_PasswordHash",passwordHash);
			ie.field("ctl00_ContentPlaceHolder1_RepeatPasswordBox",repeatPassword);
			
			ie.field("ctl00_ContentPlaceHolder1_CompanyBox",companyBox);
			ie.field("ctl00_ContentPlaceHolder1_EmailBox",emailBox);
			ie.field("ctl00_ContentPlaceHolder1_FNameBox",firstName);
			ie.field("ctl00_ContentPlaceHolder1_LNameBox",lastName);
			ie.field("ctl00_ContentPlaceHolder1_NotesBox",notesBox);
			ie.field("ctl00_ContentPlaceHolder1_TitleBox", titleBox);
			ie.button("Sign Up").click();  
    		return teamMentor;
    	}

		/*
		[ShowInGui]
    	public static API_TeamMentor_IE ...(this API_TeamMentor_IE teamMentor)
    	{
    		teamMentor.open("/");    		
    		var ie = teamMentor.IE;
    		
    		return teamMentor;
    	}
    	*/
		
		[ShowInGui (Folder="_to reimplement")]
    	public static API_TeamMentor_IE __delete2ndUser(this API_TeamMentor_IE teamMentor)
    	{
    		
    	/*	var ie = teamMentor.IE;
    		teamMentor.loginAs_Admin();
			teamMentor.manageUsers();
			ie.link("delete").click();
			ie.button("Delete User").click();
			*/
    		return teamMentor;
    	}		
    	
		
			
    }
}