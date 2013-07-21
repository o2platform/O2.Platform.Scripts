// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Xml.Linq;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;



namespace O2.XRules.Database.APIs
{
    public class API_Armorize
    {    
    	public XElement XmlFindingsFile { get;set;}
    	
    	public API_Armorize(string xmlFindingsFile)
    	{
    		XmlFindingsFile = xmlFindingsFile.xRoot();
    	}
	}
	
	public static class API_Armorize_ExtensionMethods_Xml_Parsing
	{
		public static List<XElement> vulnerabilities(this API_Armorize armorize)
		{
			return armorize.XmlFindingsFile.elements("{http://ns.armorize.com/Report}vulnerability");
		}
		
		public static List<XElement> traceBacks(this XElement vulnerability)
		{
			return vulnerability.elements("{http://ns.armorize.com/Traceback}traceback");
		}						
		
		public static string vulnName(this XElement vulnerability)
		{		
			return vulnerability.notNullAttributeValue("source-type-name");
		}
		
		public static string vulnType(this XElement vulnerability)
		{
			return vulnerability.notNullAttributeValue("type");
		}
		
		public static string sig(this XElement element)
		{
			return element.notNullAttributeValue("sig");			
		}
		
		public static string funScope(this XElement element)
		{
			return element.notNullAttributeValue("fun-scope");			
		}
		
		public static string _name(this XElement element)
		{
			return element.notNullAttributeValue("name");			
		}
		 
		public static string indent(this XElement element)
		{
			return element.notNullAttributeValue("indent");			
		}
		
		public static string file(this XElement vulnerability)
		{
			return vulnerability.element("{http://ns.armorize.com/Traceback}scp")
							    .element("{http://ns.armorize.com/Traceback}file")
							    .attribute("path").value();//.removeFirstChar();
		}				
		
		public static UInt32 line(this XElement vulnerability)
		{
			return vulnerability.element("{http://ns.armorize.com/Traceback}scp")						
							    .attribute("sl").value().toUInt(); 
		}
		
		public static string notNullAttributeValue(this XElement vulnerability, string name)		
		{
			var attribute = vulnerability.attribute(name);
			return attribute.notNull() ? attribute.value() :  ""; 
		}		
		
	}
	
	public static class API_Armorize_ExtensionMethods_GuiHelpers
	{
		public static TreeView view_XmlFile_in_TreeView(this string file)
		{
			return "Xml View of Armorize Findings file".popupWindow()
													   .add_TreeView()
													   .view_XmlFile_in_TreeView(file);
		}
		
		public static TreeView view_XmlFile_in_TreeView(this TreeView treeView, string file)
		{
			return treeView.xmlShow(file);
		}
	}
}