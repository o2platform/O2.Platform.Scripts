using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.DotNetWrappers.ExtensionMethods;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.NRefactory.Ast;
using O2.API.AST.CSharp;
//O2Ref:System.Core.dll
//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

namespace O2.API.AST.Graph 
{
    public class CodeStreamGraphNode : Label
    {
        public O2CodeStream CodeStream { get; set; }
        public O2CodeStreamNode CodeStreamNode { get; set; }
        
        public string NodeText { get; set; }
        public Action<CodeStreamGraphNode> onDoubleClick { get; set; }
        public Action<CodeStreamGraphNode> onMouseEnter { get; set; }
        public Action<CodeStreamGraphNode> onMouseLeave { get; set; }


        public CodeStreamGraphNode(O2CodeStream codeStream , O2CodeStreamNode codeStreamNode)                            
        {
            CodeStream = codeStream;
            CodeStreamNode = codeStreamNode;

            NodeText = CodeStreamNode.Text;
            this.Content = NodeText;
            setColorBasedOnObjectType();
            this.MouseDoubleClick+=(sender,e) => { if (onDoubleClick != null) onDoubleClick(this);};
            this.MouseEnter += (sender, e) => { if (onMouseEnter != null) onMouseEnter(this); };
            this.MouseLeave += (sender, e) => { if (onMouseLeave != null) onMouseLeave(this); };


        }

        /*public override string ToString()
        {
            return NodeText ?? "[null]";
        }*/
        public void setColorBasedOnObjectType()
        {
            switch (CodeStreamNode.typeName())
            { 
                case "ParameterDeclarationExpression":
                    Foreground = Brushes.Gray;
                    break;
                case "MethodDeclaration":
                case "MemberReferenceExpression":
                    Foreground = Brushes.Blue;
                    break;
                case "LocalVariableDeclaration":    
                case "VariableDeclaration":
                    Foreground = Brushes.DarkGreen;
                    break;                
                case "PrimitiveExpression":
                    Foreground = Brushes.Brown;
                    break;
                case "ReturnStatement":
                    Foreground = Brushes.Orange;
                    break;
                case "IdentifierExpression":
                    Foreground = Brushes.DarkViolet;
                    break;
                
                default:
                    //Foreground = Brushes.Red;
                    Foreground = Brushes.Black;
                    break;
            }
        }

    }
}
