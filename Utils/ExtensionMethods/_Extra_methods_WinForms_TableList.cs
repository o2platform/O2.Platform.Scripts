// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;

using O2.Views.ASCX.classes.MainGUI;

namespace O2.XRules.Database.Utils
{	
	public static class _Extra_TableList_ExtensionMethods
	{
		public static ascx_TableList ascending(this ascx_TableList tableList)
		{
			return tableList.sort(SortOrder.Ascending);
		}
		
		public static ascx_TableList descending(this ascx_TableList tableList)
		{
			return tableList.sort(SortOrder.Descending);
		}
		
		public static ascx_TableList sort(this ascx_TableList tableList, SortOrder sortOrder = SortOrder.Ascending)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						tableList.listView().Sorting = sortOrder;
						return tableList;
					});
		}
        
	}
}