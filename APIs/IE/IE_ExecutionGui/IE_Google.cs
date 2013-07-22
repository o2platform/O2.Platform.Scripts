// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows.Forms;
using FluentSharp.Watin;

//O2File:API_IE_ExecutionGUI.cs
//O2File:WatiN_IE_ExtensionMethods.cs    
//O2Ref:WatiN.Core.1x.dll

namespace O2.XRules.Database.APIs
{	
	
    public class IE_Google : API_IE_ExecutionGUI
    {   
    	
    	public IE_Google(WatiN_IE ie) : base(ie)
    	{
    	 	config();
    	}
    	
    	public IE_Google(Control hostControl)	: base(hostControl) 
    	{    		
    		config();
    	}  
    	
    	public void config()
    	{
    		this.TargetServer = "http://www.google.com";    		
    	}
    }
    
    public static class IE_Google_Actions
    {    					
		
		[ShowInGui(Folder ="root")]
		public static API_IE_ExecutionGUI homepage(this IE_Google ieExecution)
		{
			return ieExecution.open(""); 
		}
		
		[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI images(this IE_Google ieExecution)
		{
			return ieExecution.open("imghp?hl=en&tab=wi"); 
		}
		
		[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI videos(this IE_Google ieExecution)
		{
			return ieExecution.open("http://videos.google.com/?hl=en&tab=wv"); 
		}

		[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI maps(this IE_Google ieExecution)
		{
			return ieExecution.open("/maps?hl=en&tab=wl"); 
		}
		
		
		
	}    
}   