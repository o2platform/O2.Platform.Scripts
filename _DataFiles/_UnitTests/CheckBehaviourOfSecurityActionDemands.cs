// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:nunit.framework.dll
using NUnit.Framework;

using System.Security.Principal;
using System.Security.Permissions;
using O2.XRules.Database.Utils;
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class CheckBehaviourOfSecurityActionDemands
    {        	    
		[PrincipalPermission(SecurityAction.Demand, Role="Admin")]
    	public string protectedByDemand()
    	{
    		return "some secret";
    	}
    	
    	[PrincipalPermission(SecurityAction.Demand, Role="Admin", Authenticated=false)] 
    	public string protectedByDemandWithNoAuthentication()
    	{
    		return "some secret";
    	}
    	
    	[Test]
    	public string CheckCurrentThreadPrincipalIsNull()
    	{
    		System.Threading.Thread.CurrentPrincipal =  new GenericPrincipal(new GenericIdentity(""),new string[] {});    		
    		Assert.That(System.Threading.Thread.CurrentPrincipal.notNull() && 
    				    System.Threading.Thread.CurrentPrincipal.Identity.Name == "" &&
    				    System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated.isFalse(), "problem setting default value of System.Threading.Thread.CurrentPrincipal");
    		return "ok: default principal defined";
    	}    	    	
    	
    	[Test]
    	public string call_protectedByDemand_no_roles()
    	{
    		try
    		{
    			var value = protectedByDemand();
    			Assert.Fail("we should not be here");
    			return "error";
    		}
    		catch(Exception ex)
    		{
    			Assert.That(ex is System.Security.SecurityException, "Was expecting a SecurityException object, and had: {0}".format(ex.Message));
    			return "got expected security exception";
    		}    		
    	}       	
    	
    	[Test]
    	public string call_protectedByDemand_with_admin2_role()
    	{
    		try
    		{
    			var user = System.Threading.Thread.CurrentPrincipal;
    			var principal = new GenericPrincipal(user.Identity, new string[] {"Admin2"});
    			
    			System.Threading.Thread.CurrentPrincipal = principal;    			
    			protectedByDemand();
    			 
    			Assert.Fail("we should not be here");
    			return "error";
    		}
    		catch(Exception ex)
    		{
    			Assert.That(ex is System.Security.SecurityException, "Was expecting a SecurityException object, and had: {0}".format(ex.Message));
    			return "got expected security exception";
    		}    		
    	}
    	
    	[Test]
    	public string call_protectedByDemand_with_admin_role()
    	{
    		try
    		{
    			var user = System.Threading.Thread.CurrentPrincipal;
    			var principal = new GenericPrincipal(user.Identity, new string[] {"Admin"});
    			
    			System.Threading.Thread.CurrentPrincipal = principal;    			
    			protectedByDemand();    			
    			
    			Assert.Fail("we should not be here");
    			return "error";
    		}
    		catch(Exception ex)
    		{
    			Assert.That(ex is System.Security.SecurityException, "Was expecting a SecurityException object, and had: {0}".format(ex.Message));
    			return "got expected security exception";
    		}    		
    	}
    	
    	[Test]
    	public string call_protectedByDemandWithNoAuthentication_with_no_roles()
    	{
    		try
    		{    			    			
    			var principal = new GenericPrincipal(new GenericIdentity(""), new string[] {""});
    			Assert.That(principal.Identity.IsAuthenticated.isFalse(), "IsAuthenticated should be false");
    			Assert.That(principal.IsInRole("Admin").isFalse(), "IsInRole(\"Admin\") should be false");
    			
    			System.Threading.Thread.CurrentPrincipal = principal;
    			protectedByDemandWithNoAuthentication();
    							 		
    			return "Note: the Demand for Role=\"Admin\" was not enforced";
    		}
    		catch(Exception ex)
    		{
    			Assert.That(ex is System.Security.SecurityException, "Was expecting a SecurityException object, and had: {0}".format(ex.Message));
    			return "got expected security exception";
    		}    		
    	}
    	
    	[Test]
    	public string call_protectedByDemandWithNoAuthentication_with_admin2_role()
    	{
    		try
    		{    			    			
    			var principal = new GenericPrincipal(new GenericIdentity(""), new string[] {"Admin2"});
    			Assert.That(principal.Identity.IsAuthenticated.isFalse(), "IsAuthenticated should be false");
    			Assert.That(principal.IsInRole("Admin").isFalse(), "IsInRole(\"Admin\") should be false");
    			
    			System.Threading.Thread.CurrentPrincipal = principal;
    			protectedByDemandWithNoAuthentication();
    			    			   		
    			return "Note: the Demand for Role=\"Admin\" was not enforced";
    		}
    		catch(Exception ex)
    		{
    			Assert.That(ex is System.Security.SecurityException, "Was expecting a SecurityException object, and had: {0}".format(ex.Message));
    			return "got expected security exception";
    		}    		
    	}
    	
    	
    	
    	[Test]
    	public string call_protectedByDemandWithNoAuthentication_with_admin_role()
    	{
    		try
    		{
    			var principal = new GenericPrincipal(new GenericIdentity("a name"), new string[] {"Admin"});    			
    			Assert.That(principal.Identity.IsAuthenticated.isTrue(), "IsAuthenticated should be false");
    			Assert.That(principal.IsInRole("Admin").isTrue(), "IsInRole(\"Admin\") should be true");
    			
    			System.Threading.Thread.CurrentPrincipal = principal;
    			var value = protectedByDemandWithNoAuthentication();
    			
    			return "Note: the fact that IsAuthenticateds is true also makes no difference";
    		}
    		catch(Exception ex)
    		{
    			Assert.Fail("a security excption was not expected");    			
				return "fail";
    		}    		
    	}
    	
    	
    	
    }
}
