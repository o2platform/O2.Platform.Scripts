using System;
using System.Text;
using System.Runtime.InteropServices;

//O2File:API_WinAPI.cs
 
namespace O2.XRules.Database.APIs
{
	public partial class WinAPI
	{		
		[DllImport("kernel32.dll")]		public static extern bool 	CloseHandle(IntPtr hObject);				
		
		[DllImport("kernel32.dll")]		public static extern IntPtr GetModuleHandle(string lpModuleName);
		[DllImport("kernel32.dll")]		public static extern int 	GetSystemWow64Directory([In] [Out] StringBuilder lpBuffer, [MarshalAs(UnmanagedType.U4)] uint size);
		[DllImport("kernel32.dll")]		public static extern int 	GetCurrentThreadId();
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)] 
										public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
		
		[DllImport("kernel32.dll")]		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

	}
}