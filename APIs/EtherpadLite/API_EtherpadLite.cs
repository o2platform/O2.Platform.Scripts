// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using FluentSharp.Watin;
using Etherpad;

//O2File:WatiN_IE_ExtensionMethods.cs 
//O2File:EtherpadLiteDotNet.cs
//O2Ref:WatiN.Core.1x.dll


namespace O2.XRules.Database.APIs
{
	public class API_EtherpadLite
	{
		public string 	Host 					{ get; set; }
    	public string 	ApiKey 					{ get; set; }
    	public string 	CurrentPad 				{ get; set; }
    	public bool 	PadExists 				{ get; set; }
    	public string 	LastKnownText 			{ get; set; }
		public EtherpadLiteDotNet EtherpadLite 	{ get; set; }
		
		public WatiN_IE ie;
		
		public API_EtherpadLite()
    	{
    		 Host = "beta.etherpad.org";
    		 ApiKey = "EtherpadFTW";
    		 EtherpadLite = new EtherpadLiteDotNet(ApiKey,Host);    	
    		 CurrentPad = "O2_TestPad".add_RandomLetters(5);
    	}
    	
    	public API_EtherpadLite(WatiN_IE _ie) : this()
    	{
    		ie = _ie;
    	}    	    	
	}
	
	public static class API_EtherpadLite_ExtensionMethods
	{
		public static string pad_Url(this API_EtherpadLite etherPad)
		{
			return "http://{0}/p/{1}".format(etherPad.Host, etherPad.CurrentPad);
		}
		
		
		public static API_EtherpadLite create_Pad(this API_EtherpadLite etherPad, string padName)
		{
			"[API_EtherpadLite] creating pad: {0}".info(padName);
			var result = etherPad.EtherpadLite.CreatePad(padName);
			if (result.Code != EtherpadReturnCodeEnum.Ok)
				"[API_EtherpadLite][createPad]: {0}".error(result.Message);
			{
				etherPad.CurrentPad = padName;	
				etherPad.PadExists = true;
			}
			return etherPad;
		}
		
		public static API_EtherpadLite open(this API_EtherpadLite etherPad, string padName)
		{
			return 	etherPad.open_Pad(padName);
		}
		
		public static API_EtherpadLite open_Pad(this API_EtherpadLite etherPad, string padName)
		{
			return etherPad.ensure_Pad_Exists(padName);
		}
		
		public static API_EtherpadLite ensure_Pad_Exists(this API_EtherpadLite etherPad)
		{
			return etherPad.ensure_Pad_Exists(etherPad.CurrentPad);
		}
		public static API_EtherpadLite ensure_Pad_Exists(this API_EtherpadLite etherPad, string padName)
		{
			if (etherPad.PadExists)
				return etherPad;
			var result = etherPad.EtherpadLite.GetText(padName);
			if (result.Message == "padID does not exist")
				etherPad.create_Pad(padName);
			else
				if (result.Code == EtherpadReturnCodeEnum.Ok)
				{
					etherPad.CurrentPad = padName;
					etherPad.PadExists = true;
				}
				else
					"[API_EtherpadLite][ensure_Pad_Exists] unsupported response code: {0}".error(result.Code);
			return etherPad;
		}
		public static string contents(this API_EtherpadLite etherPad)
		{
			return etherPad.get_Text();
		}
		
		public static string get_Text(this API_EtherpadLite etherPad)
		{			
			etherPad.ensure_Pad_Exists();			
			var result = etherPad.EtherpadLite.GetText(etherPad.CurrentPad);
			if (result.Code == EtherpadReturnCodeEnum.Ok)
			{
				etherPad.LastKnownText = result.Data.Text;
				return etherPad.LastKnownText;
			}
			"[API_EtherpadLite][get_Text]: {0}".error(result.Message);
			return null;				
		}
		
		public static API_EtherpadLite contents(this API_EtherpadLite etherPad, string text)
		{
			return etherPad.set_Text(text);
		}
		
		public static API_EtherpadLite set_Text(this API_EtherpadLite etherPad, string text)
		{
			if (etherPad.LastKnownText == text)
			{
				"[API_EtherpadLite][set_Text]: skiping update since the content is the same".info();
			}
			//text = text.fixCRLF();
			etherPad.ensure_Pad_Exists();
			"[API_EtherpadLite][set_Text]: updating pad with text with size: {0}".info(text.size());
			var result = etherPad.EtherpadLite.SetText(etherPad.CurrentPad, text);
			if (result.Code != EtherpadReturnCodeEnum.Ok)
				"[API_EtherpadLite][set_Text]: {0}".error(result.Message);
			else
				etherPad.LastKnownText=text;
			return etherPad;
		}
	}
}

	