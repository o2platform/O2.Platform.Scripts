using FluentSharp.WPF;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;

//O2Ref:FluentSharp.WPF.dll
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
