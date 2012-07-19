using System;
using System.Web.UI;
using System.Web.Services;
using System.Data.SqlClient; 
//O2Ref:System.Web.Services.dll
   
namespace O2.SAST_DotNet
{
	public class Template : Page
	{
		
		[WebMethod]
		public void callme(string tainted)
		{			
			//write(tainted);
		}						
		
		public void write(string maybeSafe)
		{
			Response.Write(maybeSafe);
		}
		
	}

}