// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.Utils
{
    public class AspNetControlEncodings_Raw : List<AspNetControlEncoding_Raw>
    {        	     	    
    }
    
    public class AspNetControlEncoding_Raw
    {    
    	[XmlAttribute] public string @Type 						{ get; set;}
		[XmlAttribute] public string PropertyName				{ get; set;}
		[XmlAttribute] public string AttributeName_Script		{ get; set;}
		[XmlAttribute] public string HtmlEncode_scriptEncode	{ get; set;}
		[XmlAttribute] public string UrlEncode					{ get; set;}
		
		public override string ToString()
	    {
	    	return "{0} - {1}".format(@Type, PropertyName);;
	    }
    }
    
    
}
