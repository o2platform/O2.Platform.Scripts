// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Linq;    
using System.Collections.Generic; 


namespace O2.XRules.Database.APIs
{
    public class API_VulnCat
    {            
    	
    }  
    
    public class VulnCat
    {
    	public string 				 Server		{ get; set; }
    	public List<VulnCat_Mapping> Mappings	{ get; set; }    	
    	public VulnCat()
    	{
    		Mappings = new List<VulnCat_Mapping>();
    	}
    }
    
    public class VulnCat_Mapping
    {
    	[XmlAttribute] 	public int 		Id 		 { get; set; }
    	[XmlAttribute] 	public int 		ParentId { get; set; }
    	[XmlAttribute] 	public string 	Title	 { get; set; }
    	[XmlAttribute] 	public string 	Url		 { get; set; } 
    	[XmlElement] 	public string 	Html	 { get; set; } 
    }
    
    public static class VulnCat_ExtensionMethods
    {
    	public static VulnCat add_Mapping(this VulnCat vulnCat, int id, int parentId, string title, string url, string html = "")
    	{
    		var mapping = new VulnCat_Mapping()
							{
								Id = id,
								ParentId = parentId,
								Title = title,
								Url = url
							};
			vulnCat.Mappings.Add(mapping);
			return vulnCat;
    	}
    }
}


