using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;

//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll

//O2File:_Extra_methods_WinForms_TreeView.cs

namespace O2.XRules.Database.Utils
{
    public static class HtmlAgilityPack_ExtensionMethods
    {
        #region HtmlAgilityPack.HtmlDocument

        public static HtmlAgilityPack.HtmlDocument htmlDocument(this string htmlCode)
        {
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(htmlCode);
            return htmlDocument;
        }

        public static string html(this HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            return htmlDocument.DocumentNode.OuterHtml;
        }

		public static List<HtmlAgilityPack.HtmlNode> filter(this HtmlAgilityPack.HtmlDocument htmlDocument, string query)
		{
			return htmlDocument.select(query);
		}
		
        public static List<HtmlAgilityPack.HtmlNode> select(this HtmlAgilityPack.HtmlDocument htmlDocument, string query)
        {
            return htmlDocument.DocumentNode.SelectNodes(query).toList<HtmlAgilityPack.HtmlNode>();
        }

        public static List<HtmlAgilityPack.HtmlNode> links(this HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            return htmlDocument.select("//a");
        }

        #endregion
        
        #region HtmlAgilityPack.HtmlNode

        public static List<string> html(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            return htmlNodes.outerHtml();
        }

        public static List<string> outerHtml(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            var outerHtml = new List<string>();
            foreach (var htmlNode in htmlNodes)
                outerHtml.add(htmlNode.outerHtml());
            return outerHtml;
        }

        public static string html(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.outerHtml();
        }

        public static string outerHtml(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.OuterHtml;
        }

        public static string innerHtml(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.InnerHtml;
        }

        public static List<string> innerHtml(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            var outerHtml = new List<string>();
            foreach (var htmlNode in htmlNodes)
                outerHtml.add(htmlNode.innerHtml());
            return outerHtml;
        }

        public static string value(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.innerHtml();
        }

        public static List<string> values(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            return htmlNodes.innerHtml();
        }
		
		public static string value(this HtmlAgilityPack.HtmlAttribute attribute)
        {
            return attribute.Value;
        }
        #endregion
    }
    
    public static class HtmlAgilityPack_ExtensionMethods_Elements
    {
    
        public static List<HtmlAgilityPack.HtmlNode> nodes(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            return htmlNodes.nodes("");
        }

        public static List<HtmlAgilityPack.HtmlNode> nodes(this List<HtmlAgilityPack.HtmlNode> htmlNodes, string nodeName)
        {
            var allNodes = new List<HtmlAgilityPack.HtmlNode>();
            foreach (var htmlNode in htmlNodes)
                allNodes.add(htmlNode.nodes(nodeName));
            return allNodes;
        }

        public static List<HtmlAgilityPack.HtmlNode> nodes(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.nodes("");
        }
        public static List<HtmlAgilityPack.HtmlNode> nodes(this HtmlAgilityPack.HtmlNode htmlNode, string nodeName)
        {
            var htmlNodes = new List<HtmlAgilityPack.HtmlNode>();
            foreach (var node in htmlNode.ChildNodes)
                if (nodeName.valid().isFalse() || node.Name == nodeName)
                    htmlNodes.add(node);
            return htmlNodes;
        }

		public static HtmlAgilityPack.HtmlNode node(this HtmlAgilityPack.HtmlNode htmlNode, string nodeName)
        {            
            foreach (var node in htmlNode.ChildNodes)
                if (nodeName.valid().isFalse() || node.Name == nodeName)
                    return htmlNode;
            return null;
        }               
    }
    
    public static class HtmlAgilityPack_ExtensionMethods_Attributes
    {
        public static List<HtmlAgilityPack.HtmlAttribute> attributes(this List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            return htmlNodes.attributes("");
        }

        public static List<HtmlAgilityPack.HtmlAttribute> attributes(this List<HtmlAgilityPack.HtmlNode> htmlNodes, string attributeName)
        {
            var allAttributes = new List<HtmlAgilityPack.HtmlAttribute>();
            foreach (var htmlNode in htmlNodes)
                allAttributes.add(htmlNode.attributes(attributeName));
            return allAttributes;
        }

        public static List<HtmlAgilityPack.HtmlAttribute> attributes(this HtmlAgilityPack.HtmlNode htmlNode)
        {
            return htmlNode.attributes("");
        }
        public static List<HtmlAgilityPack.HtmlAttribute> attributes(this HtmlAgilityPack.HtmlNode htmlNode, string attributeName)
        {
            var attributes = new List<HtmlAgilityPack.HtmlAttribute>();
            foreach (var htmlAttribute in htmlNode.Attributes)
                if (attributeName.valid().isFalse() || htmlAttribute.Name == attributeName)
                    attributes.add(htmlAttribute);
            return attributes;
        }

		public static HtmlAgilityPack.HtmlAttribute attribute(this HtmlAgilityPack.HtmlNode htmlNode, string attributeName)
        {            
            foreach (var htmlAttribute in htmlNode.Attributes)
                if (attributeName.valid().isFalse() || htmlAttribute.Name == attributeName)
                    return htmlAttribute;
            return null;
        }               

        public static List<string> names(this List<HtmlAgilityPack.HtmlAttribute> htmlAttributes)
        {
            var names = new List<string>();
            foreach (var htmlAttribute in htmlAttributes)
                if (names.Contains(htmlAttribute.Name).isFalse())
                    names.add(htmlAttribute.Name);
            return names;
        }

        public static List<string> values(this List<HtmlAgilityPack.HtmlAttribute> htmlAttributes)
        {
            var values = new List<string>();
            foreach (var htmlAttribute in htmlAttributes)
                values.add(htmlAttribute.Value);
            return values;
        }

	}	
	
	public static class HtmlAgilityPack_ExtensionMethods_TreeView
	{
        #region TreeView mappings

        public static TreeView add_Node(this TreeView treeView, HtmlAgilityPack.HtmlDocument htmlDocument)
        {
            return treeView.add_Node(htmlDocument.DocumentNode);
        }

        public static TreeView add_Node(this TreeView treeView, HtmlAgilityPack.HtmlNode htmlNode)
        {
            treeView.rootNode().add_Node(htmlNode);
            return treeView;
        }

        public static TreeNode add_Node(this TreeNode treeNode, HtmlAgilityPack.HtmlNode htmlNode)
        {
            return treeNode.add_Node(htmlNode.Name, htmlNode, true);
        }

        public static TreeView add_Nodes(this TreeView treeView, List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            treeView.rootNode().add_Nodes(htmlNodes);
            return treeView;
        }

        public static TreeNode add_Nodes(this TreeNode treeNode, List<HtmlAgilityPack.HtmlNode> htmlNodes)
        {
            foreach (var htmlNode in htmlNodes)
                treeNode.add_Node(htmlNode);
            return treeNode;
        }

        #endregion        
	}
	
	public static class HtmlAgilityPack_ExtensionMethods_Others	
	{
        #region ascx_SourceCodeViewer mappings

        public static ascx_SourceCodeViewer showHtmlNodeLocation(this ascx_SourceCodeViewer codeViewer, HtmlAgilityPack.HtmlNode htmlNode)
        {
            codeViewer.editor().showHtmlNodeLocation(htmlNode);
            return codeViewer;
        }

        public static ascx_SourceCodeEditor showHtmlNodeLocation(this ascx_SourceCodeEditor codeEditor, HtmlAgilityPack.HtmlNode htmlNode)
        {

            var startLine = htmlNode.Line;
            var startColumn = htmlNode.LinePosition;

            var endLine = startLine;
            var endColumn = startColumn;

            if (htmlNode.NextSibling != null)
            {
                endLine = htmlNode.NextSibling.Line;
                endColumn = htmlNode.NextSibling.LinePosition;
            }
            else
                endColumn += htmlNode.html().size();
            "selecting CodeEditor location: {0}:{1} -> {2}:{3}".info(startLine, startColumn, endLine, endColumn);
            codeEditor.clearMarkers();
            codeEditor.selectTextWithColor(startLine, startColumn, endLine, endColumn);
            codeEditor.caret_Line(startLine);
            codeEditor.refresh();

            return codeEditor;
        }

        #endregion

        #region string mappings

        public static string htmlToXml(this string htmlCode)
        {
        	return htmlCode.htmlToXml(true);
        }
        
        public static string htmlToXml(this string htmlCode, bool xmlFormat)
        {
        	try
        	{
        		var stringWriter = new StringWriter();
				var xmlWriter = XmlWriter.Create(stringWriter); 				
				xmlWriter.Flush();
				var htmlDocument = htmlCode.htmlDocument();
				
				htmlDocument.Save(xmlWriter);				
				if (xmlFormat)
					return stringWriter.str().xmlFormat();
				return stringWriter.str();
        	}
        	catch(Exception ex)
        	{
        		ex.log("[string.htmlToXml]");
        		return ex.Message;
        	}
        }
        
        public static string tidyHtml(this string htmlCode)
		{			
			var htmlDocument = htmlCode.htmlDocument();
			var tidiedhtml = htmlDocument.tidyHtml();
			if (tidiedhtml.valid())
				return tidiedhtml;
			return htmlCode;
		}
		
		public static string tidyHtml(this HtmlAgilityPack.HtmlDocument htmlDocument)
		{
			try
			{	
				htmlDocument.OptionCheckSyntax = true;
				htmlDocument.OptionFixNestedTags = true;
				htmlDocument.OptionAutoCloseOnEnd = true;
				htmlDocument.OptionOutputAsXml = true;
				//htmlDocument.OptionDefaultStreamEncoding = Encoding.Default;
				var formatedCode = htmlDocument.DocumentNode.OuterHtml.xmlFormat().xRoot().innerXml().trim();
				return formatedCode;			
			}
        	catch(Exception ex)
        	{
        		ex.log("[string.tidyHtml]");
        		return null;
        	}
		}
        #endregion
    }
    
    public static class HtmlAgilityPack_ExtensionMethods_GuiHelpers
    {
    	public static TextBox add_HtmlTags_Viewer_with_Filter(this Control control, string htmlCode)
    	{    		
    		var htmlTags_TreeView = control.add_TreeView_for_HtmlTags();
    		return htmlTags_TreeView.add_HtmlTagFilter(htmlCode);
    		//return htmlTags_TreeView;
    	}
    	
    	public static TreeView add_TreeView_for_HtmlTags(this Control control)
    	{
    		return control.add_TreeView_for_HtmlTags(null);
    	}    	    	
    	
    	public static TreeView add_TreeView_for_HtmlTags(this Control control, Action<string> applyFilter)
    	{
    		control.clear();
    		var htmlTags_TreeView = control.add_TreeView();
    		htmlTags_TreeView.beforeExpand<HtmlAgilityPack.HtmlNode>(
					(treeNode, htmlNode)=>{																
											  if (htmlNode.Attributes != null)
											  	  foreach(var attribute in htmlNode.Attributes)
											 	  	  treeNode.add_Node("a: {0}={1}".format(attribute.Name, attribute.Value)); 
											  treeNode.add_Node("v: {0}".format(htmlNode.InnerHtml));  	
											  if (htmlNode.ChildNodes != null)
											  	  foreach(var childNode in htmlNode.ChildNodes)
												  	  if (childNode.html().valid()) 
													  	  treeNode.add_Node("n: {0}".format(childNode.Name), childNode, true);  
										  });	
										  
			var treeView_ContextMenu = htmlTags_TreeView.add_ContextMenu();			
			treeView_ContextMenu.add_MenuItem("Sort Nodes", ()=> htmlTags_TreeView.sort());
			treeView_ContextMenu.add_MenuItem("Don't Sort Nodes", ()=> htmlTags_TreeView.sort(false));
			//treeView_ContextMenu.add_MenuItem("Show all nodes",()=> htmlNodeFilter.sendKeys("//*".line()));   
			
			if (applyFilter.notNull())
				htmlTags_TreeView.insert_Below_HtmlTagFilter(applyFilter);
			return htmlTags_TreeView;										  
    	}
    	
    	public static TextBox insert_Below_HtmlTagFilter(this Control control, Action<string> applyFilter)
    	{
    		var sampleXPathQueries = new List<string> {		"//*",  
    														"//a",
															"//img",
															"//a[contains(@href,'news')]",
															"//a[contains(text(),'S')]",
															"//a[text()='Blogs']"	};
    		var htmlNodeFilter = control.insert_Below<TextBox>(25).fill();
			var sampleQueries_MenuItem = htmlNodeFilter.add_ContextMenu().add_MenuItem("Sample queries");
						
			foreach(var xPathQuery in sampleXPathQueries)
				sampleQueries_MenuItem.add_MenuItem(xPathQuery , (text) => htmlNodeFilter.set_Text(text.str()));
						
			htmlNodeFilter.onEnter(
					(text)=>{
								applyFilter(text);
							});
							
			return htmlNodeFilter;
    	}    	    	
    	
    	public static TextBox add_HtmlTagFilter(this TreeView htmlTags_TreeView, string htmlCode)
    	{
    		TextBox htmlNodeFilter = null;
    		Action<string> applyFilter = 
    			(filter)=>{ 
    						htmlCode.showFilteredHtmlContentInTreeView(filter, htmlTags_TreeView, htmlNodeFilter);
    					  };
    			
    		htmlNodeFilter = htmlTags_TreeView.insert_Below_HtmlTagFilter(applyFilter);
    		
    		return htmlNodeFilter;
    	}
    	
    	public static string showFilteredHtmlContentInTreeView(this string htmlCode, string filter, TreeView htmlTags_TreeView, TextBox htmlNodeFilter)
    	{
    		htmlTags_TreeView.clear();
			try
			{
				">showing htmlcode with size: {0}".info(htmlCode.size());
				htmlNodeFilter.backColor(Color.White);
				var htmlDocument = htmlCode.htmlDocument();  	
				if (filter.valid())
					htmlTags_TreeView.add_Nodes(htmlDocument.select(filter));
				else 
				{
					htmlTags_TreeView.add_Node(htmlDocument);
					htmlTags_TreeView.expand();
				}
				"HtmlTags_TreeView nodes: {0}".info(htmlTags_TreeView.nodes().size());
				
			}
			catch(System.Exception ex)
			{
				ex.log("in htmlNodeFilter.onEnter");
				htmlNodeFilter.backColor(Color.Red);
			} 			
			htmlTags_TreeView.applyPathFor_1NodeMissingNodeBug(); 
			return htmlCode;
    	}
    	
    	public static TreeView after_TagSelect_showIn_SouceCodeViewer(this TreeView htmlTags_TreeView, ascx_SourceCodeViewer htmlCodeViewer)
    	{
    		htmlTags_TreeView.afterSelect<HtmlAgilityPack.HtmlNode>(
							  	(htmlNode)=>{ 
							  					try
							  					{							  						
							  						htmlCodeViewer.showHtmlNodeLocation(htmlNode);
							  						htmlCodeViewer.editor().caret(htmlNode.Line, htmlNode.LinePosition);
							  					}
							  					catch(Exception ex)
							  					{
							  						"[after_TagSelect_showIn_SouceCodeViewer] Error: {0}".error(ex.Message);
							  					}
							  				});
			return htmlTags_TreeView;
		}
    }
    
    
}
