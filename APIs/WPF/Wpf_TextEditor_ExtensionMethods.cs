using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using O2.API.Visualization.Ascx;
using GraphSharp.Controls;
using O2.API.Visualization.Xaml;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:Xaml_ExtensionMethods.cs
//O2File:WPF_Controls_ExtensionMethods.cs
//O2File:GraphSharp_ExtensionMethods.cs
//O2File:WpfTextEditor.cs
//O2Ref:FluentSharp.WPF.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationCore.dll 

namespace O2.XRules.Database.Utils
{
    public static class WPF_TextEditor_ExtensionMethods
    {

        #region add_WpfTextEditor

        public static WpfTextEditor add_WpfTextEditor(this System.Windows.Forms.Control control)
        {
            return control.add_WPF_Control<WpfTextEditor>();
        }

        public static WpfTextEditor add_WpfTextEditor(this ContentControl control)
        {
            return control.add_Control_Wpf<WpfTextEditor>();
        }

        public static WpfTextEditor add_WpfTextEditor(this ElementHost control)
        {
            return control.add_Control_Wpf<WpfTextEditor>();
        }

        public static WpfTextEditor add_WpfTextEditor(this ascx_Xaml_Host control)
        {
            return control.add_Control<WpfTextEditor>();
        }

        public static WpfTextEditor add_WpfTextEditor(this GraphLayout control)
        {
            return control.add_UIElement<WpfTextEditor>();
        }

        #endregion

        #region setters

        public static WpfTextEditor set_Text(this TextEditor textEditor, string text)
        {
            return (WpfTextEditor)textEditor.wpfInvoke(
                () =>
                {
                    textEditor.Text = text;
                    return textEditor;
                });
        }

        #endregion

        #region gui options

        public static WpfTextEditor xml(this WpfTextEditor wpfTextEditor)
        {
            wpfTextEditor.SyntaxHighlighting = "XML";
            return wpfTextEditor;
        }

        public static WpfTextEditor html(this WpfTextEditor wpfTextEditor)
        {
            wpfTextEditor.SyntaxHighlighting = "HTML";
            return wpfTextEditor;
        }

        public static WpfTextEditor csharp(this WpfTextEditor wpfTextEditor)
        {
            wpfTextEditor.SyntaxHighlighting = "C#";
            return wpfTextEditor;
        }

        #endregion
    }
}
