//O2Ref:FluentSharp.WPF.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll


namespace O2.XRules.Database.Utils
{
    public static class ElementHost_ExtensioMethods
    {
        /*public static Control getHost(this string xamlFileName)
        {
            if (xamlFileName.fileExists())
            { }
            return null;
        }*/
        #region ElementHost add		
        
/*		public static T add_Control_Wpf<T>(this ElementHost elementHost) where T : UIElement
    	{
    		return (T)elementHost.invokeOnThread(
    			()=>{
    					try
			            {
	    					var wpfControl = typeof(T).ctor();					    				
	    					if (wpfControl is UIElement)
	    					{
	    						elementHost.Child = (UIElement)wpfControl;
	    						return (T)wpfControl;			    								    					
	    					}
						}			
			            catch (System.Exception ex)
			            {
			                ex.log("in add_Control");
			            }
			            return null;			    				
    				});     		
    	}
*/
		#endregion 
				
		    	

		#region ElementHost - WPF Controls
/*		public static Label add_Label_Wpf(this ElementHost elementHost, string text)
		{
			return elementHost.add_Control_Wpf<Label>().set_Text_Wpf(text);
		}
		
		public static Grid add_Grid_Wpf(this ElementHost elementHost)
		{
			return elementHost.add_Control_Wpf<Grid>();
		}
		
		public static StackPanel add_StackPanel_Wpf(this ElementHost elementHost)
		{
			return elementHost.add_Control_Wpf<StackPanel>();
		}
		
        public static System.Windows.Controls.ListView add_ListView_Wpf(this ElementHost elementHost)
        {
            return elementHost.add_Control_Wpf<System.Windows.Controls.ListView>();
        }
*/                
        #endregion

    }
}
