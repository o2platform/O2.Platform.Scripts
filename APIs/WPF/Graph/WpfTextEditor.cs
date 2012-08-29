using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using ICSharpCode.AvalonEdit.Highlighting;

//O2Ref:O2_FluentSharp_WPF.dll
//O2Ref:ICSharpCode.AvalonEdit.dll

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

namespace O2.API.Visualization.Xaml
{
    public class WpfTextEditor : TextEditor
    {
        public WpfTextEditor()
        {
            setDefaultGui();                        
        }

        public void setDefaultGui()
        {
            this.Width = 100;
            this.Width = 100;
            //this.csharp();                        
        }

        #region thread-safe version of TextEditor functions

        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                this.wpfInvoke(() => this.Text = value);
                //this.set_Text(value);
            }
        }

        public new double FontSize
        {
            get
            {
                return base.FontSize;
            }
            set 
            {
                this.wpfInvoke(()=> base.FontSize = value );                
            }
        }

        public new string SyntaxHighlighting
        {
            get 
            {
                return base.SyntaxHighlighting.Name;
            }

            set
            {
                this.wpfInvoke(()=> base.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition(value));
            }
        }
        #endregion        
    }
}
