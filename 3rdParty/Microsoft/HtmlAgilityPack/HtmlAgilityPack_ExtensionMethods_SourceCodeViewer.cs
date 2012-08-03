using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}
