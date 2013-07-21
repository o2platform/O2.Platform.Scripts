using System;
using System.Text;
using System.Runtime.InteropServices;

//O2File:API_WinAPI.cs
 
namespace O2.XRules.Database.APIs
{
	public partial class WinAPI
	{		
		[DllImport("gdi32.dll")]		public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);				
	}
}