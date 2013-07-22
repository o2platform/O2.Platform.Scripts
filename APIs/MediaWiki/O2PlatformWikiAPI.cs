// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;

//O2File:O2MediaWikiApi.cs    

namespace O2.XRules.Database.APIs
{

	public class O2PlatformWikiAPI : O2MediaWikiAPI
	{

		public O2PlatformWikiAPI() 
		{
			init("http://www.o2platform.com/api.php");
			this.Styles = styles();
		}
		
		// dynamically (one per session) grab the current header scripts used in OWASP
		public string styles()
		{
			var wikiStyles = "<link rel=\"stylesheet\" href=\"http://www.o2platform.com/skins/common/diff.css?207\" type=\"text/css\" />".line() + 
							 "<link rel=\"stylesheet\" href=\"http://www.o2platform.com/skins/common/shared.css?207\" type=\"text/css\" media=\"screen\" />".line() +
			     		     "<link rel=\"stylesheet\" href=\"http://www.o2platform.com/skins/common/commonPrint.css?207\" type=\"text/css\" media=\"print\" /> ".line() +
				 		     "<link rel=\"stylesheet\" href=\"http://www.o2platform.com/skins/gumax/gumax_main.css?207\" type=\"text/css\" media=\"screen\" /> ".line();
			return wikiStyles;
		}
    }
}
