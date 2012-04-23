// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.Utils
{		
	public static class _Extra_XmlLinq_ExtensiomMethods
	{					
		public static XElement parent(this XElement element)
		{
			return element.Parent;
		}
		
		public static XElement element(this XElement elementToSearch, string name, bool createIfNotExistant)
		{
			var foundElement = elementToSearch.element(name);
			if (foundElement.notNull())
				return foundElement;
			return createIfNotExistant 
					? elementToSearch.add_Element(name)
					: null;
		}
		
		public static XElement add_Element(this XElement rootElement, string text)
		{
			var newElement = new XElement(text);
			rootElement.Add(newElement);
			return newElement;
		}
		
		public static XAttribute add_Attribute(this XElement rootElement, string text, object value)
		{
			var newAttribute = new XAttribute(text.xName(), value);
			rootElement.Add(newAttribute);
			return newAttribute;
		}
		
		public static XAttribute attribute(this XElement elementToSearch, string name, bool createIfNotExistant)
		{
			return elementToSearch.attribute(name, 	createIfNotExistant, null);
		}
		
		public static XAttribute attribute(this XElement elementToSearch, string name, bool createIfNotExistant, object value)
		{
			var foundAttribute = elementToSearch.attribute(name);
			if (foundAttribute.notNull())
				return foundAttribute;
			return createIfNotExistant 
					? elementToSearch.add_Attribute(name, value ?? "")
					: null;
		}

		public static string add_xmlns(this string name, XElement xElement)				
		{
			return name.prepend_AttributeValue(xElement, "xmlns");
		}
		
		public static string prepend_AttributeValue(this string name, XElement xElement, string attributeName)
		{
			var xmlns =  xElement.attribute(attributeName).value();
			return "{" + xmlns + "}" + name; 
		}
		
		public static XElement innerXml(this XElement xElement, string value)
		{
			return xElement.value(value);			
		}
		
		public static XElement value(this XElement xElement, string value)
		{
			xElement.Value = value;
			return xElement;
		}						 
	
		
		public static XAttribute value(this XAttribute xAttribute, string value)
		{	
			if (xAttribute.notNull())
				xAttribute.SetValue(value);
			return xAttribute;
		}				
	}
	
	public static class XmlLinq_ExtensionMethods_ProcessingInstruction
	{
		public static XProcessingInstruction processingInstruction(this XDocument xDocument, string target)
		{
			foreach(var processingInstruction in xDocument.processingInstructions())
				if (processingInstruction.Target == target)
					return processingInstruction;
			return null;
		}
		
		public static List<XProcessingInstruction> processingInstructions(this XDocument xDocument)
		{
			return xDocument.Document.Nodes().OfType<XProcessingInstruction>().toList();
		}
		
		public static XDocument set_ProcessingInstruction(this XDocument xDocument, string target, string data)
		{
			var processingInstruntion = xDocument.processingInstruction(target);
			if (processingInstruntion.notNull())
				processingInstruntion.Data = data;
			else
			{
				var newProcessingInstruction = new XProcessingInstruction(target, data);//"xsl-stylesheet", "type=\"text/xsl\" href=\"LogStyle.xsl\"");				
				xDocument.AddFirst(newProcessingInstruction);
			}
			return xDocument;
		}
		

	}
}    	