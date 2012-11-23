using System;
using System.Linq;
using System.Text;
using System.Drawing;  
using System.Drawing.Imaging; 
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using O2.DotNetWrappers.ExtensionMethods;
//O2File:API_WinAPI.cs

namespace O2.XRules.Database.APIs
{
	public static class WinAPI_ExtensionMethods_IntPtr
	{
		public static IntPtr intPtr(this int value)
		{
			return new IntPtr(value);
		}				
	}
	
	public static class WinAPI_ExtensionMethods_Text
	{
		public static string get_ControlText(this IntPtr handle)
		{
			return WinAPI.GetControlText(handle);
		}
		public static string get_WindowText(this IntPtr handle)
		{
			return WinAPI.GetWindowText(handle);
		}
		public static IntPtr set_ControlText(this IntPtr handle, string text)
		{
			WinAPI.SetControlText(handle, text);
			return handle;
		}
		
		public static IntPtr set_WindowText(this IntPtr handle, string text)
		{
			WinAPI.SetWindowText(handle, text);
			return handle;
		}
	}
	public static class WinAPI_ExtensionMethods_Misc
	{
		public static IntPtr get_Parent(this IntPtr handle)
		{
			return WinAPI.GetParent(handle);
		}
	}
	public static class WinAPI_ExtensionMethods_Window
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
		public static IntPtr window_BringToTop(this IntPtr windowHandle)
		{
			WinAPI.SetWindowPos(windowHandle, WinAPI.HWND_TOP, 0, 0, 0, 0, WinAPI.SetWindowPosFlags.IgnoreMoveAndResize);
			return windowHandle;
		}
		
		public static IntPtr window_MoveTo(this IntPtr windowHandle, int top, int left)
		{
			WinAPI.SetWindowPos(windowHandle, WinAPI.HWND_TOP, top, left, 0, 0, WinAPI.SetWindowPosFlags.IgnoreResize);
			return windowHandle;
		}
		
		
		public static IntPtr window_Redraw(this IntPtr windowHandle)
		{
			var hWnd = windowHandle;
			IntPtr parent = windowHandle.get_Parent();
			if (parent != IntPtr.Zero)
			{
				hWnd = parent;
			}
			WinAPI.InvalidateRect(hWnd, IntPtr.Zero, true);
			WinAPI.UpdateWindow(hWnd);
			WinAPI.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 1921u);
			
			return windowHandle;
		}		
		
		public static IntPtr window_Highlight(this IntPtr windowHandle)
		{
			
			WinAPI.RECT rECT = new WinAPI.RECT(0, 0, 0, 0);
			WinAPI.GetWindowRect(windowHandle, ref rECT);
			WinAPI.GetParent(windowHandle);
			IntPtr windowDC = WinAPI.GetWindowDC(windowHandle);
			if (windowDC != IntPtr.Zero)
			{
				Graphics graphics = Graphics.FromHdc(windowDC, windowHandle);
				var drawPen = new Pen(Brushes.Red, 2f);
				graphics.DrawRectangle(drawPen, 1, 1, rECT.Width - 2, rECT.Height - 2);
				graphics.Dispose();
				WinAPI.ReleaseDC(windowHandle, windowDC);
			}
			
			return windowHandle;
		}
		//public static Bitmap PrintWindow(IntPtr hwnd)    
		public static Bitmap window_ScreenShot(this IntPtr windowHandle)
		{					
		    WinAPI.RECT rc = default(WinAPI.RECT);         
		    WinAPI.GetWindowRect(windowHandle, ref rc);
		
		    Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);        
		    Graphics gfxBmp = Graphics.FromImage(bmp);        
		    IntPtr hdcBitmap = gfxBmp.GetHdc();        
		
		    WinAPI.PrintWindow(windowHandle, hdcBitmap, 0);  
		
		    gfxBmp.ReleaseHdc(hdcBitmap);               
		    gfxBmp.Dispose(); 
		
		    return bmp;   		
		}
	}
}