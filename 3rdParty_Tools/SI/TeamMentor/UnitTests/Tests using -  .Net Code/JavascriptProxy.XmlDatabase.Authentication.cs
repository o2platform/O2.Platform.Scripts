// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Security;
using System.Collections.Generic;    
using System.Security.Permissions;	
using NUnit.Framework; 
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;   
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

using SecurityInnovation.TeamMentor.WebClient.WebServices; 
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.WebClient;

//O2File:API_Moq_HttpContext.cs

//O2File:TM_Test_XmlDatabase.cs 

namespace O2.SecurityInnovation.TeamMentor.WebClient.JavascriptProxy_XmlDatabase
{		  
	[TestFixture] 
    public class Test_Authentication : TM_Test_XmlDatabase
    {
    	public string adminUser { get; set;}
    	public string adminPwd { get; set;}
    	
	    static Test_Authentication()
     	{
     		TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;       		
     	} 
     	
    	public Test_Authentication() 
    	{     		    		    	 	    	     		
			var httpContextApi = new API_Moq_HttpContext();   
			HttpContextFactory.Context = httpContextApi.HttpContextBase;
			
			adminUser = TMConfig.Current.DefaultAdminUserName;
			adminPwd  = TMConfig.Current.DefaultAdminPassword;
			
			//HttpContextFactory.Current.SetCurrentUserRoles(UserGroup.Admin);			
			//UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin						
    	}     	
    	

		[Test]
		public void tmWebServices_Login_PwdInClearText()
		{
			var sessionId = tmWebServices.Login_PwdInClearText(adminUser, adminPwd);
			Assert.That(sessionId != Guid.Empty,"sessionID was empty");			
		}
		
		[Test]
		public void checkLoginSessionValues()
		{			
			var sessionId = tmWebServices.Login_PwdInClearText(adminUser, adminPwd);
			Assert.That(sessionId != Guid.Empty,"sessionID was empty");
			Assert.AreEqual(tmWebServices.tmAuthentication.sessionID, sessionId, "tmXmlDatabase.sessionId");			
			Assert.AreEqual(adminUser, tmWebServices.tmAuthentication.currentUser.UserName, "tmXmlDatabase.currentUser");			
		}
		
		[Test]
		public void tmWebServices_Current_SessionID_Current_User_GetCurrentUserRoles()
		{
			//create test user
			var user = "test_user_aaa";
			var pwd = "bb";						
			var newUser = tmXmlDatabase.newUser_ClearTextPassword(user, pwd);
			
			//test on tmXmlDatabase
			var sessionId = tmXmlDatabase.login_PwdInClearText(user, pwd); 
			Assert.That(sessionId != Guid.Empty, "sessionId was empty");
			var userGroup = sessionId.session_UserGroup();			//new users currently default to Reader
			Assert.AreEqual(userGroup, UserGroup.Reader, "user group was not reader");
			var userRoles = sessionId.session_UserRoles(); 
			Assert.AreEqual(userRoles.size(), 2, "userRoles size");
			Assert.AreEqual(UserRole.ReadArticlesTitles, userRoles[0], "first userRole");						
			
			//test on tmWebServices
			sessionId = tmWebServices.Login_PwdInClearText(user, pwd);
			Assert.AreEqual(sessionId, tmWebServices.Current_SessionID(), "tmWebServices.CurrentSessionID");
			Assert.AreEqual(user, tmWebServices.Current_User().UserName, "tmWebServices.CurrentSessionID");
			var roles = tmWebServices.GetCurrentUserRoles();			
			Assert.AreEqual(roles.size(), 2, "userRoles size");
			Assert.AreEqual("ReadArticlesTitles", roles[0], "first userRole");						
			 
			"deleting user".info(); 
			UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
			
			//delete user
			Assert.That(tmWebServices.javascriptProxy.DeleteUser(newUser).isTrue() , "failed to test user");						
			
		}
		
		[Test] 
		public void RBAC_test_Security_Demands()
		{
			//anonymous
			UserGroup.Anonymous.setThreadPrincipalWithRoles();			
			Assert.Throws<SecurityException>(()=> UserGroup.Reader.demand()  			, "Anonymous: UserGroup.Reader Demand");
			Assert.Throws<SecurityException>(()=> UserGroup.Editor.demand()  			, "Anonymous: UserGroup.Editor Demand");
			Assert.Throws<SecurityException>(()=> UserGroup.Admin.demand()  			, "Anonymous: UserGroup.Admin Demand");			
			Assert.Throws<SecurityException>(()=> UserRole.Admin.demand()  				, "Anonymous: UserRole.Admin Demand");
			Assert.Throws<SecurityException>(()=> UserRole.EditArticles.demand()  		, "Anonymous: UserRole.EditArticles Demand");
			Assert.Throws<SecurityException>(()=> UserRole.ReadArticles.demand()  		, "Anonymous: UserRole.ReadArticles Demand");
			Assert.Throws<SecurityException>(()=> UserRole.ManageUsers.demand()  		, "Anonymous: UserRole.ManageUsers Demand");
			Assert.DoesNotThrow(			 ()=> UserRole.ReadArticlesTitles.demand()  , "Anonymous: UserRole.ReadArticlesTitles Demand");
			
			//Reader
			UserGroup.Reader.setThreadPrincipalWithRoles();
			Assert.DoesNotThrow 			(()=> UserGroup.Reader.demand()  			, "Reader: UserGroup.Reader Demand");
			Assert.Throws<SecurityException>(()=> UserGroup.Editor.demand()  			, "Reader: UserGroup.Editor Demand");
			Assert.Throws<SecurityException>(()=> UserGroup.Admin.demand()  			, "Reader: UserGroup.Admin Demand");			
			Assert.Throws<SecurityException>(()=> UserRole.Admin.demand()  				, "Reader: UserRole.Admin Demand");
			Assert.Throws<SecurityException>(()=> UserRole.EditArticles.demand()  		, "Reader: UserRole.EditArticles Demand");
			Assert.DoesNotThrow 			(()=> UserRole.ReadArticles.demand()  		, "Reader: UserRole.ReadArticles Demand");
			Assert.Throws<SecurityException>(()=> UserRole.ManageUsers.demand()  		, "Reader: UserRole.ManageUsers Demand");
			Assert.DoesNotThrow			    (()=> UserRole.ReadArticlesTitles.demand()  , "Reader: UserRole.ReadArticlesTitles Demand");
			
			//Editor
			UserGroup.Editor.setThreadPrincipalWithRoles();
			Assert.DoesNotThrow 			(()=> UserGroup.Reader.demand()  			, "Editor: UserGroup.Reader Demand");
			Assert.DoesNotThrow 			(()=> UserGroup.Editor.demand()  			, "Editor: UserGroup.Editor Demand");
			Assert.Throws<SecurityException>(()=> UserGroup.Admin.demand()  			, "Editor: UserGroup.Admin Demand");			
			Assert.Throws<SecurityException>(()=> UserRole.Admin.demand()  				, "Editor: UserRole.Admin Demand");
			Assert.DoesNotThrow 			(()=> UserRole.EditArticles.demand()  		, "Editor: UserRole.EditArticles Demand");
			Assert.DoesNotThrow 			(()=> UserRole.ReadArticles.demand()  		, "Editor: UserRole.ReadArticles Demand");
			Assert.Throws<SecurityException>(()=> UserRole.ManageUsers.demand()  		, "Editor: UserRole.ManageUsers Demand");
			Assert.DoesNotThrow			    (()=> UserRole.ReadArticlesTitles.demand()  , "Editor: UserRole.ReadArticlesTitles Demand");

			//Admin
			UserGroup.Admin.setThreadPrincipalWithRoles();
			Assert.DoesNotThrow 			(()=> UserGroup.Reader.demand()  			, "Admin: UserGroup.Reader Demand");
			Assert.DoesNotThrow 			(()=> UserGroup.Editor.demand()  			, "Admin: UserGroup.Editor Demand");
			Assert.DoesNotThrow 			(()=> UserGroup.Admin.demand()  			, "Admin: UserGroup.Admin Demand");
			Assert.DoesNotThrow 			(()=> UserRole.Admin.demand()  				, "Admin: UserRole.Admin Demand");
			Assert.DoesNotThrow 			(()=> UserRole.EditArticles.demand()  		, "Admin: UserRole.EditArticles Demand");
			Assert.DoesNotThrow 			(()=> UserRole.ReadArticles.demand()  		, "Admin: UserRole.ReadArticles Demand");
			Assert.DoesNotThrow 			(()=> UserRole.ManageUsers.demand()  		, "Admin: UserRole.ManageUsers Demand");
			Assert.DoesNotThrow			    (()=> UserRole.ReadArticlesTitles.demand()  , "Admin: UserRole.ReadArticlesTitles Demand");

			
			//check string based demands
			Assert.DoesNotThrow			    (()=> "ReadArticles".demand()  				, "Reader: string Demand #1");
			Assert.DoesNotThrow			    (()=> "readArticles".demand()  				, "Reader: string Demand #1");
			Assert.DoesNotThrow			    (()=> "readarticles".demand()  				, "Reader: string Demand #1");
			Assert.Throws<SecurityException>(()=> "_readarticles".demand()    			, "Reader: string Demand #1");
			Assert.Throws<SecurityException>(()=> "Reader".demand()    					, "Reader: string Demand #1");
		}		
		
		[Test]
		public void RBAC_UserCreation()
		{			
			//Readers cannot get users
			UserGroup.Reader.setThreadPrincipalWithRoles();
			Assert.Throws<SecurityException>(()=> tmWebServices.GetUser_byID(111111111), "Reader: GetUser_byID");
			
			//Anonymous can create users
			UserGroup.Anonymous.setThreadPrincipalWithRoles();
			var newUser = new NewUser();
			newUser.username = "test_user_".add_RandomLetters(4);
			var userId = tmWebServices.CreateUser(newUser);
			Assert.That(userId > 0 , "Anonymous: CreateUser");
			
			// confirm that new user role is 2 (Reader)
			UserGroup.Admin.setThreadPrincipalWithRoles();
			var user = tmWebServices.GetUser_byID(userId); 
			Assert.AreEqual(user.GroupID, 2, "Anonymous created user: group id");
			
			//only admins can delete user
			UserGroup.Anonymous	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(()=> tmWebServices.DeleteUser(userId), "Anonymous: DeleteUser");
			UserGroup.Reader	.setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(()=> tmWebServices.DeleteUser(userId), "Reader	  : DeleteUser");
			UserGroup.Editor	.setThreadPrincipalWithRoles();	Assert.Throws<SecurityException>(()=> tmWebServices.DeleteUser(userId), "Editor	  : DeleteUser");
			UserGroup.Admin		.setThreadPrincipalWithRoles();	Assert.DoesNotThrow(			 ()=> tmWebServices.DeleteUser(userId), "Admin    : DeleteUser");
			
			//check that only admins can create users with GroupId specificed			
			userId = 0;
			newUser = new NewUser();
			newUser.username = "test_user_".add_RandomLetters(4);
			newUser.groupId = 10;
			UserGroup.Anonymous .setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(()=> 		   tmWebServices.CreateUser(newUser), "Anonnymous: CreateUser with groupd ID");
			UserGroup.Reader	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(()=>		   tmWebServices.CreateUser(newUser), "Reader	 : CreateUser with groupd ID");
			UserGroup.Editor	.setThreadPrincipalWithRoles(); Assert.Throws<SecurityException>(()=> 		   tmWebServices.CreateUser(newUser), "Editor	 : CreateUser with groupd ID");
			UserGroup.Admin 	.setThreadPrincipalWithRoles(); Assert.DoesNotThrow				(()=> userId = tmWebServices.CreateUser(newUser), "Admin	 : CreateUser with groupd ID");
			Assert.That(userId > 0 , "Admin: CreateUser with groupID");
			user = tmWebServices.GetUser_byID(userId); 
			Assert.AreEqual(user.GroupID, 10, "Admin created user: group id");
			tmWebServices.DeleteUser(userId);
			
			//check that only admins can call BatchUserCreation
			var batchUserCreation ="";
			UserGroup.Anonymous .setThreadPrincipalWithRoles();   Assert.Throws<SecurityException>(()=> tmWebServices.BatchUserCreation(batchUserCreation), "Anonymous: BatchUserCreation");
			UserGroup.Reader 	.setThreadPrincipalWithRoles();   Assert.Throws<SecurityException>(()=> tmWebServices.BatchUserCreation(batchUserCreation), "Reader	  : BatchUserCreation");
			UserGroup.Editor 	.setThreadPrincipalWithRoles();   Assert.Throws<SecurityException>(()=> tmWebServices.BatchUserCreation(batchUserCreation), "Editor   : BatchUserCreation");
			UserGroup.Admin 	.setThreadPrincipalWithRoles();   Assert.DoesNotThrow			  (()=> tmWebServices.BatchUserCreation(batchUserCreation), "Admin	  : BatchUserCreation");
		}
		
		[Test]
		public void RBAC_batchUserCreation()
		{	
			//3 users to create
			var userName1 = "test_user_".add_RandomLetters(4);
			var userName2 = "test_user_".add_RandomLetters(4);
			var userName3 = "test_user_".add_RandomLetters(4);
			var batchUserCreation =  userName1 + ",pwd,firstname,lastname, 1".line() + 
									 userName2 + ",pwd,firstname,lastname, 3".line() + 
									 userName3.line() + 
									 userName3;
				

			//create users			 
			UserGroup.Admin.setThreadPrincipalWithRoles();  			  
			var newUsers = tmWebServices.BatchUserCreation(batchUserCreation);
			Assert.NotNull(newUsers[0], "user1 ok");
			Assert.NotNull(newUsers[1], "user1 ok");
			Assert.NotNull(newUsers[2], "user1 ok");
			Assert.IsNull (newUsers[3], "duplicate user was created"); 
			Assert.AreEqual(newUsers[0].UserName, userName1, "userName1");
			Assert.AreEqual(newUsers[1].UserName, userName2, "userName1"); 
			Assert.AreEqual(newUsers[2].UserName, userName3, "userName1"); 
			
			//check if users where created
			var userIds = newUsers.userIds();
			newUsers = tmWebServices.GetUsers_byID(userIds);
			Assert.AreEqual(newUsers.size(), 3, "fetched new users");
			
			//delete users
			var result = tmWebServices.DeleteUsers(userIds);
			Assert.That(result[0] && result[1] && result[2], "users deleted ok");
			
			//check if users where deleted
			var deletedUsers = tmWebServices.GetUsers_byID(userIds);
			Assert.That(deletedUsers[0].isNull() && deletedUsers[1].isNull() && deletedUsers[2].isNull() , "users deleted where not there");			
		}
/*    	 
    	///**********************
		///***  TMLoginHelper methods
		///*** 
    	
		[Test]
    	public string deleteAllUsers_ExceptAdmin()
    	{
    		var adminSessionID = TMLoginHelper.login_As_Admin();
    		var users = authentication.users(adminSessionID); 
			"There are {0} users in the database".info(users.size());
			foreach(var user in users)
				if (user.UserName!= "admin")
				{
					"deleting user: '{0}' with ID: '{1}'".info(user.UserName, user.UserID); 
					authentication.DeleteUser(adminSessionID,user.UserID);
				}
			users =  authentication.users(adminSessionID);			
			Assert.That(users.size() == 1 , "There should be 1 users in the db and there were {0}".format(users.size()));
			return "ok: deleteAllUsers_ExceptTestOnes";
    	}
    	
    	[Test] 
    	public string checkIfUsersExist_Reader_and_Test()
    	{
    		var adminSessionID = TMLoginHelper.login_As_Admin();
    		if (authentication.user(adminSessionID,"test").isNull()) 
    		{
    			"Creating user 'test'".info();
				authentication.CreateUser(adminSessionID ,"test" , new TMUtils().createPasswordHash("test", "123qwe") ,"email@nowhere.com" ,"test" ,"..." ,"note"); 
			}
			Assert.That(authentication.user(adminSessionID,"test").notNull(), "failed to create/get user 'test'");
			if (authentication.user(adminSessionID,"reader").isNull()) 
			{
				"Creating user 'reader'".info();
				authentication.CreateUser(adminSessionID ,"reader" , new TMUtils().createPasswordHash("reader", "123qwe") ,"email@nowhere.com" ,"reader" ,"..." ,"note"); 	
			}
			Assert.That(authentication.user(adminSessionID,"reader").notNull(), "failed to create/get user 'reader'");	

    		return "ok: checkIfUsersExist_Reader_and_Test";
    	}
    	

    	[Test] 
    	public Guid TMHelper_login_As_Admin()
    	{
    		var adminSessionID = TMLoginHelper.login_As_Admin();
    		Assert.That(adminSessionID.notNull() && adminSessionID != Guid.Empty, "Failed to login_As_Admin");
    		return adminSessionID;
    	}
    	
    	[Test] 
    	public Guid TMHelper_login_As_Reader()
    	{
    		var adminSessionID = TMLoginHelper.login_As_Reader();
    		Assert.That(adminSessionID.notNull() && adminSessionID != Guid.Empty, "Failed to login_As_Reader");
    		return adminSessionID;
    	
    	}
    	[Test] 
    	public Guid TMHelper_login_As_Test()
    	{
    		var adminSessionID = TMLoginHelper.login_As_Test();
    		Assert.That(adminSessionID.notNull() && adminSessionID != Guid.Empty, "Failed to login_As_Test");
    		return adminSessionID;
    	}
    	
    	///**********************
		///*** webMethod_IsRbacDisabled 
		///*** 
    	[Test]
    	public string webMethod_IsRbacDisabled()
    	{   
    		var rbacDisabled = TMLoginHelper.authentication.IsRbacDisabled();
			Assert.That(rbacDisabled.isFalse(), "RCBAC is Disabled");
			return "RBAC checks are enabled";
    	}
    	
    	
    	///**********************
		///*** webMethod_IsUserAdmin
		///*** 
    	[Test]
    	public string webMethod_IsUserAdmin()
    	{   
    		var adminGuid = TMLoginHelper.login_As_Admin(); 
    		var readerGuid = TMLoginHelper.login_As_Reader(); 
    		Assert.That(adminGuid != Guid.Empty && readerGuid!=Guid.Empty , "could not get test SessionID Guids");
    		
			teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = adminGuid } ;
			var response = teamMentorSecurity.IsUserAdmin();
			Assert.That(response, "Is User Admin was false");
			
			teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = readerGuid } ;
			response = teamMentorSecurity.IsUserAdmin();
			Assert.That(response.isFalse(), "Is User Reader was maked as Admin");
			
			return "ok: webMethod_IsUserAdmin";
		}    	
    	
    	///**********************
		///*** webMethod_IsUserAdmin
		///*** 
    	[Test]
    	public string webMethod_LoginUsingSoapCredentials()
    	{   
    	 	var adminSessionID = Guid.NewGuid();
    	 	teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = adminSessionID } ;
    	 	Assert.That(teamMentorSecurity.IsUserAdmin().isFalse() ,"random adminSessionID should not be an admin");    	 	
			//login as Admin    		
    		teamMentorSecurity.CredentialsValue = new Credentials() { User = "admin",Password = TMLoginHelper.password(1) , AdminSessionID= adminSessionID};    					
    		//"teamMentorSecurity.LoginUsingSoapCredentials():{0}".debug(teamMentorSecurity.LoginUsingSoapCredentials());
			Assert.That(onlineStorage.LoginUsingSoapCredentials(), "(via soap) failed to login as admin");
			Assert.That(teamMentorSecurity.IsUserAdmin() ," adminSessionID after correct authentication be be an admin");    	 	
			//fail to login as admin
			teamMentorSecurity.CredentialsValue = new Credentials() { User = "admin",Password = 10.randomLetters() };						
			Assert.That(onlineStorage.LoginUsingSoapCredentials().isFalse(), "should had not successed");
			Assert.That(teamMentorSecurity.IsUserAdmin().isFalse() ," adminSessionID after faild correct authentication be be NOT an admin");    	 	
			return "ok: webMethod_LoginUsingSoapCredentials";
		}
		
		///**********************
		///*** webMethods_DemandPrivileges_Admin_and_DemandPrivileges_ReadArticles
		///*** 
    	[Test]
    	public string webMethods_DemandPrivileges_Admin_and_DemandPrivileges_ReadArticles()
		{
			var randomSessionID = Guid.NewGuid();			//TMLoginHelper.login_As_Admin();  ;//Guid.NewGuid();			
			teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = randomSessionID } ;
			
			try { teamMentorSecurity.DemandPrivileges_Admin(); } 
			catch(Exception ex) { Assert.That(ex.Message.contains("Request for principal permission failed"), "wrong exception");}
			
			try { teamMentorSecurity.DemandPrivileges_ReadArticles(); } 
			catch(Exception ex) { Assert.That(ex.Message.contains("Request for principal permission failed"), "wrong exception."); }
						
			var adminSessionID = TMLoginHelper.login_As_Admin();  
			teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = adminSessionID } ;
			teamMentorSecurity.DemandPrivileges_Admin();
			
			var readerSessionID = TMLoginHelper.login_As_Reader();  
			teamMentorSecurity.CredentialsValue = new Credentials() {AdminSessionID = readerSessionID } ;
			teamMentorSecurity.DemandPrivileges_ReadArticles();
						
			return "ok: webMethods_DemandPrivileges_Admin_and_DemandPrivileges_ReadArticles";
		}
    	    
    }
    */
    	
    }
}
