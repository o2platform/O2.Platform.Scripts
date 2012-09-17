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
using TheWorldsWorst.ApiWrapper;
using TheWorldsWorst.ApiWrapper.Model;

//O2Ref:TheWorldsWorst.ApiWrapper.dll

//O2Ref:System.dll
//O2Ref:System.Core.dll
//O2Ref:System.Data.dll
//O2Ref:System.Data.DataSetExtensions.dll
//O2Ref:System.RunTime.Serialization.dll
//O2Ref:System.ServiceModel.Web.dll
//O2Ref:System.Web.dll
//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll

namespace O2.XRules.Database.APIs
{
    public class API_StackOverflow
    {    
    	public ApiProxy SOProxy { get; set; }
    	
		public API_StackOverflow()
		{
			SOProxy = new ApiProxy();
		}
    }
    
    public static class API_StackOverflow_ExtensionMethods_Misc
    {
    	public static API_StackOverflow disableGzip(this API_StackOverflow soApi)
    	{
    		ApiProxy.DownloadUsingGzipEncoding = false;
    		return soApi;
    	}    	    	
    }
    
    public static class API_StackOverflow_ExtensionMethods_Search
    {
    	public static List<Question> search(this API_StackOverflow soApi, string searchText)
    	{    		
			var tagged = new List<string>();
			var notTagged = new List<string>();
			var  sort = "";
			var order = ""; 
			return soApi.search(searchText, tagged, notTagged, sort, order);
    	}
    	    	
    	public static List<Question> search(this API_StackOverflow soApi, string searchText, List<string> tagged, List<string> notTagged, string sort, string order)
    	{   
    		return ApiProxy.SearchQuestions(searchText,tagged, notTagged, sort, order).toList();  
    	}
    }
    
    public static class API_StackOverflow_ExtensionMethods_Questions
    {
    	public static List<string> titles(this List<Question> questions)
    	{
    		return (from question in questions
    				select question.title()).toList();
    	}
    	
    	public static string title(this Question question)
    	{
    		if (question.notNull())
    			return question.Title;
    		return "";
    	}
    	
    	public static int id(this Question question)
    	{
    		if (question.notNull())
    			return question.QuestionId;
    		return 0;
    	}
    	
    	public static string body(this Question question)    	
    	{
    		return question.body(true);
    	}
    	
    	public static string body(this Question question, bool includeComments)
    	{
    		var answer = ApiProxy.GetQuestionFor(question.id());
    		var body = answer.Body ?? "";
    		if (includeComments)
    			body += answer.commentsHtml();
    		return body;
    	}
    	
    	public static string commentsHtml(this Question question)
    	{
    		if (question.isNull() || question.Comments.isNull())
    			return "";
    		return question.Comments.toList().commentsHtml();
    	}
    }
            
    public static class API_StackOverflow_ExtensionMethods_Answers
    {
    	public static List<Answer> answers(this Question question)
    	{
    		return ApiProxy.GetAnswersFor(question).toList();
    	}
    	
    	public static string title(this Answer answer)
    	{
    		if (answer.notNull())
    			return answer.Title;
    		return "";
    	}
    	
    	public static int id(this Answer answer)
    	{
    		if (answer.notNull())
    			return answer.AnswerId;
    		return 0;
    	}
    	
    	public static string body(this Answer answer)
    	{
    		return answer.body(true);
    	}
    	
    	public static string body(this Answer answer, bool includeComments)
    	{
    		var result = ApiProxy.GetAnswerFor(answer.id());    		
    		var body = result.Body ?? "";
    		if (includeComments)
    			body += result.commentsHtml();
    		return body;    		
    	}
    	
    	public static string commentsHtml(this Answer answer)
    	{
    		if (answer.isNull() || answer.Comments.isNull())
    			return "";
    		return answer.Comments.toList().commentsHtml();
    	}

	}
	
    public static class API_StackOverflow_ExtensionMethods_Comments
    {
    	public static string commentsHtml(this List<Comment> comments)
    	{
    		if (comments.size() ==0)
    			return "";
    		var commentsHtml = "<h3>Comments</h3>";
    		commentsHtml+="<ul>";
    		foreach(var comment in comments)
    			commentsHtml+="<li>{0}</li>".format(comment.Body);
    		commentsHtml+="</ul>";	
    		return commentsHtml;
        		
    	}
    }    
    
}
