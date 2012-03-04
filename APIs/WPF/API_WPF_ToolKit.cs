// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.API.Visualization.ExtensionMethods;

using Gong = GongSolutions.Wpf.DragDrop	;

//O2Ref:WPFToolkit.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:O2_API_Visualization.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:PresentationFramework.dll
 

namespace O2.XRules.Database.APIs
{
    public static class API_AvalonDock
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
