// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

using win32 = TransparencyMenu.Win32; 
//O2File:WilsonGlobalSystemHooks\Win32.cs

namespace O2.XRules.Database.APIs
{
   	public class GenericWindow : NativeWindow
	{
		private const int WM_LBUTTONDBLCLK = 0x0203;
		
		public delegate void WindowsMessageDelegate(ref Message m);
		public WindowsMessageDelegate WindowsMessage;
				
		
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				// Handle the double click message	
				case WM_LBUTTONDBLCLK:
				{
					MessageBox.Show("You double clicked!!!!");
					break;
				}
			}
			if (WindowsMessage!= null)
			{
				/*count ++;
				if ((count % 100) ==0)
					"WindowsMessage #{0}".info(count);*/
				WindowsMessage(ref m);
			}
		
			// Call base WndProc for default handling
			base.WndProc(ref m);
		}
	
		int count  = 0;
		public void GetMsg_GetMsg(IntPtr Handle, IntPtr Message, IntPtr wParam, IntPtr lParam)
		{		
			if (count ++ < 10)
				"in GetMsg_GetMsg".info();
			switch(Message.ToInt32()) 
			{
				case win32.WM_CREATE:
					"(GetMsg_GetMsg) WM_CREATE".info();
					break;
				case win32.WM_DESTROY:
					"(GetMsg_GetMsg) WM_DESTROY".info();
					break;	
				case win32.WM_MOVE:
					"(GetMsg_GetMsg) WM_MOVE".info();
					break;		
				case win32.WM_SIZE:
					"(GetMsg_GetMsg) WM_SIZE".info();
					break;
				case win32.WM_ACTIVATE:
					"(GetMsg_GetMsg) WM_ACTIVATE".info();
					break;
				case win32.WM_COMMAND:
					"(GetMsg_GetMsg) WM_COMMAND".info();
					break;	
				case win32.WM_SYSCOMMAND:
					"(GetMsg_GetMsg) WM_SYSCOMMAND".info();
					break;
				case win32.WM_MENUCOMMAND:
					"(GetMsg_GetMsg) WM_MENUCOMMAND".info();
					break;	
				case win32.WM_MENUSELECT:
					"(GetMsg_GetMsg) WM_MENUSELECT".info();
					break;										
			}
		}
	}
}
