using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using System.Xml;
using System.IO;
using System.Windows.Forms;

//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

namespace O2.XRules.Database.Utils
{

    public static class HtmlAgilityPack_ExtensionMethods_SourceCodeViewer
    {
        
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
        public static TreeView after_TagSelect_showIn_SouceCodeViewer(this TreeView htmlTags_TreeView, ascx_SourceCodeViewer htmlCodeViewer)
        {
            htmlTags_TreeView.afterSelect<HtmlAgilityPack.HtmlNode>(
                                (htmlNode) =>
                                {
                                    try
                                    {
                                        htmlCodeViewer.showHtmlNodeLocation(htmlNode);
                                        htmlCodeViewer.editor().caret(htmlNode.Line, htmlNode.LinePosition);
                                    }
                                    catch (Exception ex)
                                    {
                                        "[after_TagSelect_showIn_SouceCodeViewer] Error: {0}".error(ex.Message);
                                    }
                                });
            return htmlTags_TreeView;
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
                    (treeNode, htmlNode) =>
                    {
                        if (htmlNode.Attributes != null)
                            foreach (var attribute in htmlNode.Attributes)
                                treeNode.add_Node("a: {0}={1}".format(attribute.Name, attribute.Value));
                        treeNode.add_Node("v: {0}".format(htmlNode.InnerHtml));
                        if (htmlNode.ChildNodes != null)
                            foreach (var childNode in htmlNode.ChildNodes)
                                if (childNode.html().valid())
                                    treeNode.add_Node("n: {0}".format(childNode.Name), childNode, true);
                    });

            var treeView_ContextMenu = htmlTags_TreeView.add_ContextMenu();
            treeView_ContextMenu.add_MenuItem("Sort Nodes", () => htmlTags_TreeView.sort());
            treeView_ContextMenu.add_MenuItem("Don't Sort Nodes", () => htmlTags_TreeView.sort(false));
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

            foreach (var xPathQuery in sampleXPathQueries)
                sampleQueries_MenuItem.add_MenuItem(xPathQuery, (text) => htmlNodeFilter.set_Text(text.str()));

            htmlNodeFilter.onEnter(
                    (text) =>
                    {
                        applyFilter(text);
                    });

            return htmlNodeFilter;
        }

        public static TextBox add_HtmlTagFilter(this TreeView htmlTags_TreeView, string htmlCode)
        {
            TextBox htmlNodeFilter = null;
            Action<string> applyFilter =
                (filter) =>
                {
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
            catch (System.Exception ex)
            {
                ex.log("in htmlNodeFilter.onEnter");
                htmlNodeFilter.backColor(Color.Red);
            }
            htmlTags_TreeView.applyPathFor_1NodeMissingNodeBug();
            return htmlCode;
        }

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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                ex.log("[string.tidyHtml]");
                return null;
            }
        }
        #endregion
    }
}
