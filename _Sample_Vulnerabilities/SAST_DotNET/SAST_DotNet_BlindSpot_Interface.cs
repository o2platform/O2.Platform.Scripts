using System;
using System.Web.UI;
using System.Web.Services;
using System.Data.SqlClient; 
//O2Ref:System.Web.Services.dll
   
namespace O2.SAST_DotNet
{
	public class BlindSpot_Interface : Page
	{
		AnInterface AnObject { get ; set; }
		
		BlindSpot_Interface()
		{
			AnObject = new AnImplementation();
		}
	
		[WebMethod]
		public void callme(string tainted)
		{			
			//write(tainted);
			AnObject.write(tainted);
		}						
		
		public void write(string maybeSafe)
		{
			Response.Write(maybeSafe);
		}
		
	}
	
	class AnImplementation : Page, AnInterface
	{
		public void write(string maybeSafe)
		{
			Response.Write(maybeSafe);
		}
	}
	
	interface AnInterface
	{
		void write(string maybeSafe);
	}

}