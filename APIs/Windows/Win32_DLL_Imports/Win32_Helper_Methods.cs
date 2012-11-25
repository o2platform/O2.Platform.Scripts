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
	public partial class WinAPI
	{	
		//********************************
		//using DllImports from DLL_User32
		//********************************
		public static Point NativeScreenToClient(IntPtr window, Point originalPoint)
		{
			POINT pOINT = new POINT(originalPoint.X, originalPoint.Y);
			if (ScreenToClient(window, ref pOINT))
			{
				return new Point(pOINT.x, pOINT.y);
			}
			return Point.Empty;
		}	
		
		public static List<IntPtr> GetChildWindows(IntPtr hwnd)
		{
			var childWindows = new List<IntPtr>();
			try
			{
				WinAPI.WindowEnumProc addChildWindow  =
					(IntPtr childHwnd, IntPtr lparam)=>{
														childWindows.Add(childHwnd);
														return 1;
									  			   };
				WinAPI.EnumChildWindows(hwnd,addChildWindow, IntPtr.Zero);
			}
			catch(Exception ex)
			{
				ex.log("in GetChildWindows");
			}
			return childWindows;
		}
		public static List<string> GetChildWindows_Texts(IntPtr hwnd)
		{
			var hwnds = GetChildWindows(hwnd);
			"There where {0} to find the text:{0}".info(hwnd);
			return GetChildWindows_Texts(hwnds);
		}
		public static List<string> GetChildWindows_Texts(List<IntPtr> hwnds)
		{
			return (from hwnd in hwnds
					let text = GetWindowText(hwnd)
					where text.valid()
					select text).toList();
		}
		
		public static string GetControlText(IntPtr hwnd)
		{			
			var size =  (int)WinAPI.SendMessage(hwnd,  WinAPI.WM_GETTEXTLENGTH, 0, 0 );
			if (size > 0)
			{			
				var text = new StringBuilder(size + 1);				 				
				WinAPI.SendMessage(hwnd,( int)WinAPI.WM_GETTEXT, text.Capacity, text);
				return text.str();
			}
			return null;
		}
		
		public static IntPtr SetControlText(IntPtr hwnd, string text)
		{	
			var stringBuilder = new StringBuilder(text);
			return WinAPI.SendMessage(hwnd, WinAPI.WM_SETTEXT, stringBuilder.Capacity, stringBuilder);			
		}
		

		
		public static string GetWindowText(IntPtr hwnd)
		{
			int num = GetWindowTextLength(hwnd) + 1;
			StringBuilder stringBuilder = new StringBuilder(num);
			GetWindowText(hwnd, stringBuilder, num);
			return stringBuilder.ToString();
		}
		
		public static string GetClassName(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			GetClassName(hwnd, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}
		public static bool IsTargetInDifferentProcess(IntPtr targetWindowHandle)
		{
			IntPtr intPtr;
			GetWindowThreadProcessId(targetWindowHandle, out intPtr);
			if (intPtr == IntPtr.Zero)
			{
				return true;
			}
			IntPtr value = new IntPtr(Process.GetCurrentProcess().Id);
			return value != intPtr;
		}
				
		
		public static IsWow64ProcessHandler IsWow64Process
		{
			get
			{		
				IntPtr moduleHandle = GetModuleHandle("kernel32");
				IntPtr procAddress = GetProcAddress(moduleHandle, "IsWow64Process");
				if (procAddress != IntPtr.Zero)
				{
					return (IsWow64ProcessHandler)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(IsWow64ProcessHandler));
				}
				return null;
			}
		}
		public static bool IsCurrentProcessX64
		{
			get
			{
				return Marshal.SizeOf(typeof(IntPtr)) == 8;
			}
		}
		private static bool IsWindowsX64
		{
			get
			{
				bool result;
				try
				{					
					StringBuilder lpBuffer = new StringBuilder(256);
					int systemWow64Directory = GetSystemWow64Directory(lpBuffer, 256u);					
					return(systemWow64Directory != 0);					
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}
		public static bool IsProcessX64(IntPtr processId)
		{			
			int dwProcessId = processId.ToInt32();
			IntPtr intPtr = OpenProcess(1024, false, dwProcessId);
			bool flag = false;
			try
			{
				IsWow64Process(intPtr, out flag);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 0)
				{
					"IsWow64Process LastError: {0}".error(lastWin32Error);
				}
			}
			finally
			{
				CloseHandle(intPtr);
			}
			return !flag;
		}
		public static IntPtr GetProcessForWindow(IntPtr windowHandle)
		{
			IntPtr result;
			GetWindowThreadProcessId(windowHandle, out result); 
			return result;
		}
		public static void GetWindowThreadAndProcess(IntPtr windowHandle, out IntPtr threadId, out IntPtr processId)
		{
			threadId = GetWindowThreadProcessId(windowHandle, out processId);
		}	
	}
	
}