// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace O2.XRules.Database.APIs
{
	[Serializable]
	public class CWE_Data
    {
    	public List<CWE_Weakness> Weaknesses { get; set; }
    	
    	public CWE_Data()
    	{
    	 	Weaknesses = new List<CWE_Weakness>();
    	}
    }
    
    public class CWE_Weakness
    {
    	[XmlAttribute] 	public string Title 		{ get; set;}    	
    	[XmlAttribute] 	public string Technology { get; set;}
    	[XmlAttribute] 	public string Phase 		{ get; set;}
    	[XmlAttribute] 	public string Type 		{ get; set;}
    	[XmlAttribute] 	public string Category 	{ get; set;}
    	[XmlElement] 	public string Content 	{ get; set;}
    }
}
    	