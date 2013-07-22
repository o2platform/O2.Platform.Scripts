// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows;
using FluentSharp.WPF;
using Gong = GongSolutions.Wpf.DragDrop	;

//O2Ref:FluentSharp.WPF.dll
//O2Ref:System.Core.dll
//O2Ref:WPFToolkit.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:PresentationFramework.dll
 

namespace O2.XRules.Database.APIs
{
    public static class API_AvalonDock_DnD
    {    
		public static T enableDrag<T>(this T uiElement)
			where T : UIElement
		{
			uiElement.wpfInvoke(()=> Gong.DragDrop.SetIsDragSource(uiElement,true));
			return uiElement;
		}
		
		public static T enableDrop<T>(this T uiElement)
			where T : UIElement
		{
			uiElement.wpfInvoke(()=> Gong.DragDrop.SetIsDropTarget(uiElement,true));
			return uiElement;
		}
			

    }
}
