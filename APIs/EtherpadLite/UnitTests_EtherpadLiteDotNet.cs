// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using NUnit.Framework;
using Etherpad;

//O2File:EtherpadLiteDotNet.cs
//O2Ref:nunit.framework.dll

//for more details about this API see: https://github.com/Pita/etherpad-lite/wiki/HTTP-API

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class Test_EtherpadLite
    {        	    
    	public string Host { get; set; }
    	public string ApiKey { get; set; }
		public EtherpadLiteDotNet EtherpadLite { get; set; }
		
    	public Test_EtherpadLite()
    	{
    		 Host = "beta.etherpad.org";
    		 ApiKey = "EtherpadFTW";
    		 EtherpadLite = new EtherpadLiteDotNet(ApiKey,Host);    	
    	}
    	
    	[Test]
    	public string CreateAuthor_CreateGroup()
    	{    		
    		//CreateAuthor
			var result = EtherpadLite.CreateAuthor();								
			Assert.That(result.Code == EtherpadReturnCodeEnum.Ok && result.Message == "ok", "code or  message for CreateAuthor()" );						
			result = EtherpadLite.CreateAuthor("testAuthor");			
			Assert.That(result.Code == EtherpadReturnCodeEnum.Ok && result.Message == "ok", "code or  message for CreateAuthor(\"testAuthor\")" );			
			
			//CreateGroup
			var group = EtherpadLite.CreateGroup();
			Assert.That(group.Data.GroupID.valid(), "CreateGroup");			
			
    		return "ok: CreateAuthor_CreateGroup";
    	}
    	
    	[Test]
    	public string CreateGroupPad_ListPads()
    	{
    		var groupId = EtherpadLite.CreateGroup()
    								  .Data.GroupID;
    		var padIds = EtherpadLite.ListPads(groupId).Data.PadIDs;
    		Assert.That(padIds.notNull() && padIds.size() ==0, "padIds.size()");
    		
    		var pad1Name = "testPad11";
    		var pad2Name = "testPad12";
    		
    		var padId1 = EtherpadLite.CreateGroupPad(groupId,pad1Name).Data.PadID;
    		var padId2 = EtherpadLite.CreateGroupPad(groupId,pad2Name).Data.PadID;
    		Assert.That(padId1.notNull() && padId1.contains(pad1Name), "padId1");
    		Assert.That(padId2.notNull() && padId2.contains(pad2Name), "padId1");
			padIds = EtherpadLite.ListPads(groupId).Data.PadIDs;
						
			Assert.That(padIds.notNull() && padIds.size() == 2, "There should be 2 padIds");
			
			Assert.AreEqual(padId1 , padIds.Keys.toList()[0], "padId1");
			Assert.AreEqual(padId2 , padIds.Keys.toList()[1], "padId2");
			
			Assert.AreEqual(1, padIds[padId1], "padId1 value should be 1");
			Assert.AreEqual(1, padIds[padId2], "padId2 value should be 1");
    		return "ok: CreateGroupPad_ListPads";
    	} 
    	
    	[Test]
    	public string Create_Session()
    	{    	
    		var groupId = EtherpadLite.CreateGroup().Data.GroupID;   
			var authorId = EtherpadLite.CreateAuthor("an author").Data.AuthorID;
			//creating a valid session (with 100 seconds to live)
 			var session =  EtherpadLite.CreateSession( groupId,authorId, 10.unixTime_Now().str()); 
 			Assert.AreEqual(session.Code, EtherpadReturnCodeEnum.Ok,"session.Code");
 			Assert.That(session.Data.SessionID.valid(), "SessionID was not valid"); 			 			 			
 			
 			//creating a invalid session (bad groupId)
 			session =  EtherpadLite.CreateSession( groupId + "AAAA" ,authorId, 10.unixTime_Now().str()); 
 			Assert.AreEqual(session.Code,EtherpadReturnCodeEnum.InvalidParameters,"session.Code");
 			Assert.AreEqual(session.Message,"groupID does not exist","session.Message ");
 			
 			//creating a invalid session (bad authorId)
 			session =  EtherpadLite.CreateSession( groupId,authorId + "AAAA", 10.unixTime_Now().str()); 
 			Assert.AreEqual(session.Code,EtherpadReturnCodeEnum.InvalidParameters,"session.Code");
 			Assert.AreEqual(session.Message,"authorID does not exist","session.Message ");
 			
 			//creating a invalid session (with -1 seconds to live)
 			session =  EtherpadLite.CreateSession( groupId,authorId, (-1).unixTime_Now().str()); 
 			Assert.AreEqual(session.Code,EtherpadReturnCodeEnum.InvalidParameters,"session.Code");
 			Assert.AreEqual(session.Message,"validUntil is in the past","session.Message ");
 			 			
 			return "ok: Create_Session";
 		}
 		
 		[Test]
 		public string CreatePad_DeletePad()
 		{
 			var padName= "testPad".add_RandomLetters(5);
 			//create pad
 			var result = EtherpadLite.CreatePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"CreatePad session.Code (1st)");
 			
 			//created pad again
 			result = EtherpadLite.CreatePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.InvalidParameters,"CreatePad session.Code (2nd)");
 			Assert.AreEqual(result.Message,"padID does already exist","session.Message ");
 			
 			//delete pad
 			result = EtherpadLite.DeletePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"DeletePad session.Code");
 			 			
 			//create again
 			result = EtherpadLite.CreatePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"CreatePad session.Code (3rd)");
 			
 			return "ok: CreatePad_DeletePad"; 			
 		}
 		
 		[Test]
 		public string GetText_SetText_GetRevisionsCount()
 		{
 			var padName= "testPad".add_RandomLetters(5);
 			//create pad
 			var result = EtherpadLite.CreatePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"CreatePad session.Code");
 			
 			//get revisions
 			var revisions = EtherpadLite.GetRevisionsCount(padName).Data.Revisions;
 			Assert.AreEqual(0, revisions, "Revisions after Creation");
 			
 			//get text
 			var text = EtherpadLite.GetText(padName).Data.Text;
 			Assert.That(text.valid(), "text was not valid");
 			var textWithRevisionIndex = EtherpadLite.GetText(padName,0).Data.Text;
 			Assert.AreEqual(text, textWithRevisionIndex, "text != textWithRevisionIndex");	
 			
 			//set text
 			var newText  = "this is a change";
 			result = EtherpadLite.SetText(padName, newText);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"SetText result.Code");
 			
 			//get new text
 			text = EtherpadLite.GetText(padName).Data.Text;
 			Assert.AreEqual(newText, text.trim(), "newText != text");
 			//check that there is one revision
 			revisions = EtherpadLite.GetRevisionsCount(padName).Data.Revisions;
 			Assert.AreEqual(1, revisions, "There should be 1 revision ");
 			//confirm new text
 			textWithRevisionIndex = EtherpadLite.GetText(padName,1).Data.Text;
 			Assert.AreEqual(newText, textWithRevisionIndex.trim(), "newtext != textWithRevisionIndex");	
 			 			
 			//delete temp pad
 			result = EtherpadLite.DeletePad(padName);
 			Assert.AreEqual(result.Code, EtherpadReturnCodeEnum.Ok,"session.Code (1st)");
 			
 			
 			return "ok: GetRevisionsCount_GetText_SetText";
 		}

    }
}
