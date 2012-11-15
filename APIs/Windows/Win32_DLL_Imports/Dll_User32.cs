using System;
using System.Text;
using System.Runtime.InteropServices;

//O2File:API_WinAPI.cs
 
namespace O2.XRules.Database.APIs
{
	public partial class WinAPI
	{
		public delegate bool 			IsWow64ProcessHandler(IntPtr processHandle, out bool is64Process);
		public delegate int 			WindowEnumProc(IntPtr hwnd, IntPtr lparam);

		[DllImport("user32.dll")]		public static extern IntPtr 	AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
		
		[DllImport("user32.dll")]		public static extern IntPtr 	ChildWindowFromPoint(IntPtr hWndParent, POINT Point);				
		[DllImport("user32.dll")]		public static extern bool 		CloseWindow(IntPtr hWnd);

		[DllImport("user32.dll")]		public static extern bool 		DestroyWindow(IntPtr hwnd);

		[DllImport("user32.dll")]		public static extern bool 		EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
		[DllImport("user32.dll")]		public static extern bool 		EnumThreadWindows(int threadId, WindowEnumProc func, IntPtr lParam);
		
		[DllImport("user32.dll")]		public static extern bool 		IsWindowVisible(IntPtr hwnd);
		[DllImport("user32.dll")]		public static extern bool 		InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
		
		[DllImport("user32.dll")]		public static extern IntPtr 	GetDesktopWindow();		
		[DllImport("user32.dll")]		public static extern IntPtr 	GetWindowRect(IntPtr hwnd, ref RECT rc);
		[DllImport("user32.dll")]		public static extern int 		GetWindowTextLength(IntPtr hwnd);
		[DllImport("user32.dll")]		public static extern int 		GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		[DllImport("user32.dll")]		public static extern IntPtr 	GetClientRect(IntPtr hwnd, ref RECT rc);
		[DllImport("user32.dll")]		public static extern int 		GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll")]		public static extern IntPtr 	GetWindowThreadProcessId(IntPtr hwnd, out IntPtr processID);				
		[DllImport("user32.dll")]		public static extern IntPtr 	GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]		public static extern IntPtr 	GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		[DllImport("user32.dll")]		public static extern IntPtr 	GetWindowDC(IntPtr hWnd);
		
		[DllImport("user32.dll")]		public static extern bool 		MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);
		
		[DllImport("user32.dll")]		public static extern bool 		RedrawWindow(IntPtr hWnd, IntPtr lpRect, IntPtr hrgnUpdate, uint flags);
		[DllImport("user32.dll")]		public static extern int 		ReleaseDC(IntPtr hWnd, IntPtr hDC);

		
/*		[DllImport("user32.dll")]		public static extern bool 		SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);
        [DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hwnd, UInt32 msg, IntPtr wparam, IntPtr lparam);   		  
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hwnd, UInt32 msg, int wparam, int lparam);   		
*/
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, StringBuilder lParam); 							 //If you use '[Out] StringBuilder', initialize the string builder with proper length first.
		[DllImport("user32.dll")] 		public static extern IntPtr	 	SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPStr)] string lParam); //Also can add 'ref' or 'out' ahead of 'String lParam', don't use CharSet.Auto since we use MarshalAs(..) 				
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref RECT lParam);
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, ref POINT lParam);
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hwnd, UInt32 msg, IntPtr wparam, IntPtr lparam); 
		[DllImport("user32.dll")]		public static extern IntPtr 	SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);		
		
		[DllImport("user32.dll")]		public static extern bool 		ScreenToClient(IntPtr hWnd, ref POINT lpPoint);				
		[DllImport("user32.dll")]		public static extern bool 		SetWindowText(IntPtr hwnd, String lpString);		
		[DllImport("user32.dll")]		public static extern bool 		SetForegroundWindow(IntPtr hwnd);		
		[DllImport("user32.dll")]		public static extern IntPtr 	SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
 		[DllImport("user32.dll")]	    public static extern bool 		ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);		
		
		[DllImport("user32.dll")]		public static extern bool 		UpdateWindow(IntPtr hWnd);
		[DllImport("user32.dll")]		public static extern IntPtr 	WindowFromPoint(POINT Point);
	}
}