using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.API.AST.CSharp;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.API.AST.ExtensionMethods;
using O2.API.AST.ExtensionMethods.CSharp;

//O2File:Ast_Engine_ExtensionMethods.cs
//O2Ref:O2_API_AST.dll


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public static class TextEditor_O2CodeStream_ExtensionMethods
    {
        public static O2CodeStream show(this O2CodeStream o2CodeStream, ascx_SourceCodeEditor codeEditor)
        {
            if (o2CodeStream != null)
            {
                codeEditor.open(o2CodeStream.SourceFile);
                codeEditor.colorINodes(o2CodeStream.iNodes());
            }

            //var iNodes =
            //var file = o2CodeStream.O2MappedAstData.file(node.INode);
            //if (iNodes.size() > 0
            //{

            //}

            /*foreach(var node in o2CodeStream.Nodes)
            {    			
                var file = o2CodeStream.O2MappedAstData.file(node.INode);
                "{0}".format(file).error();
                codeEditor.open(file); 
                codeEditor.selectTextWithColor(
                //return o2CodeStream;
            }*/
            return o2CodeStream;
        }
    }
}
