using FluentSharp.CSharpAST.Utils;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;

//O2File:Ast_Engine_ExtensionMethods.cs


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public static class TextEditor_O2CodeStream_ExtensionMethods
    {
        public static O2CodeStream show(this O2CodeStream o2CodeStream, SourceCodeEditor codeEditor)
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
