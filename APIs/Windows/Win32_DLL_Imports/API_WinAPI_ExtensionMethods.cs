using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using O2.DotNetWrappers.ExtensionMethods;
//O2File:API_WinAPI.cs

namespace O2.XRules.Database.APIs
{

	public static class WinAPI_ExtensionMethods
	{
		public static int window_ThreadId(this IntPtr windowHandle)
		{
			IntPtr processId;
			return WinAPI.GetWindowThreadProcessId(windowHandle, out processId).ToInt32();
		}
		
		public static IntPtr window_Move(this IntPtr windowHandle, int x, int y, int width, int height)
		{
			WinAPI.MoveWindow(windowHandle,x,y,width, height,true);
			return windowHandle; 
		}
		
		public static IntPtr window_Show(this IntPtr windowHandle)
		{
			WinAPI.ShowWindow(windowHandle,WinAPI.ShowWindowCommands.Show);
			return windowHandle; 
		}
		public static 	IntPtr window_Hide(this IntPtr windowHandle)
		{
			WinAPI.ShowWindow(windowHandle,WinAPI.ShowWindowCommands.Hide);
			return windowHandle; 
		}
		
		public static IntPtr window_AlwaysOnTop(this IntPtr windowHandle)
		{
			WinAPI.SetWindowPos(windowHandle, WinAPI.HWND_TOPMOST, 0, 0, 0, 0, WinAPI.SetWindowPosFlags.IgnoreMoveAndResize);
			return windowHandle;
		}
	}
}