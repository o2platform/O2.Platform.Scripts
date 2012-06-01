//File based on the VS project from http://www.codeproject.com/KB/system/WilsonSystemGlobalHooks.aspx

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace TransparencyMenu
{
	public class Win32
	{
	
		//DC extra
		[DllImport("user32.dll")]
		public static extern bool SetWindowText(IntPtr hwnd, String lpString);
		//TransparencyMenu.Win32.SetWindowText(_Handle,"this is a test");  
		[DllImport("user32.dll")]
		public static extern IntPtr GetMenu(IntPtr hWnd);
		//Original	
		public delegate bool EnumWindowDelegate(IntPtr hwnd, int lParam);

		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowDelegate x, int y);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern int RegisterWindowMessage(string lpString);
		[DllImport("user32.dll")]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool Revert);		
		[DllImport("user32.dll")]
		public static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, IntPtr uIDNewItem, string lpNewItem);
		[DllImport("user32.dll")]
		public static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);
		[DllImport("user32.dll")]
		public static extern bool RemoveMenu(IntPtr hMenu, int uPosition, int uFlags);
		[DllImport("user32.dll")]
		public static extern int GetMenuItemCount(IntPtr hMenu);
		[DllImport("user32.dll")]
		public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, int dwFlags);
		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		public static extern IntPtr CreateMenu();
		[DllImport("user32.dll")]
		public static extern bool DestroyMenu(IntPtr hMenu);

		// Menus
		public static int MF_STRING = 0x00000000;
		public static int MF_POPUP = 0x00000010;
		public static int MF_BYCOMMAND = 0x00000000;
		public static int MF_BYPOSITION = 0x00000400;
		public static int MF_SEPARATOR = 0x00000800;

		// GetWindow
		public static int GW_OWNER = 4;

		// LayeredWindowAttributes
		public static int LWA_COLORKEY = 0x00000001;
		public static int LWA_ALPHA = 0x00000002;

		// WindowLong
		public static int GWL_WNDPROC         = (-4);
		public static int GWL_HINSTANCE       = (-6);
		public static int GWL_HWNDPARENT      = (-8);
		public static int GWL_STYLE           = (-16);
		public static int GWL_EXSTYLE         = (-20);
		public static int GWL_USERDATA        = (-21);
		public static int GWL_ID              = (-12);

		// WindowStyle
		public static int WS_EX_LAYERED = 0x00080000;

		// Window Messages
		public const int WM_CREATE = 0x0001;
		public const int WM_DESTROY = 0x0002;
		public const int WM_MOVE = 0x0003;
		public const int WM_SIZE = 0x0005;
		public const int WM_ACTIVATE = 0x0006;
		public const int WM_COMMAND = 0x0111;
		public const int WM_SYSCOMMAND = 0x0112;
		public const int WM_MENUCOMMAND = 0x0126;
		public const int WM_MENUSELECT = 0x011F;
	}
}
