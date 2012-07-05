//File based on the VS project from http://www.codeproject.com/KB/system/WilsonSystemGlobalHooks.aspx
using System;
using System.Text;
using System.Windows.Forms;
using System.Threading;
//O2File:Win32.cs
//O2File:Form1.cs
namespace TransparencyMenu
{
	public class Window
	{
		public IntPtr _Handle;
		public bool _IsManaged;
		public int _OriginalWindowLongExStyle;
		public Thread _FadeOutThread;
		public Thread _FadeInThread;
		public IntPtr _TransparencyMenuHandle;

		public Window(IntPtr Handle)
		{
			_Handle = Handle;
			_IsManaged = true;

			// This gets a value specifying, among other things, whether or not a window is layered (capable
			// of being transparent). We need to restore this value to its original when we're done manipulating
			// the window.
			_OriginalWindowLongExStyle = Win32.GetWindowLong(_Handle, Win32.GWL_EXSTYLE);

			AttachWindowMenu();
		}

		~Window()
		{
			StopManaging();
		}

		public void StopManaging()
		{
			if (_IsManaged)
			{
				// Kill any threads still running to handle fading
				if (_FadeInThread != null && _FadeInThread.IsAlive)
					_FadeInThread.Abort();
				if (_FadeOutThread != null && _FadeOutThread.IsAlive)
					_FadeOutThread.Abort();

				DetachWindowMenu();

				// Restore the original layered bit
				Win32.SetWindowLong(_Handle, Win32.GWL_EXSTYLE, _OriginalWindowLongExStyle);
			}

			_IsManaged = false;
		}

		public override string ToString()
		{
			return WindowText;
		}

		public IntPtr Handle
		{
			get { return _Handle; }
		}

		public string WindowText
		{
			get
			{
				StringBuilder Title = new StringBuilder(256);
				Win32.GetWindowText(_Handle, Title, 256);

				return Title.ToString().Trim();
			}
		}

		public bool IsVisible
		{
			get { return Win32.IsWindowVisible(_Handle); }
		}

		public IntPtr GetOwner
		{
			get { return Win32.GetWindow(_Handle, Win32.GW_OWNER); }
		}

		public void AttachWindowMenu()
		{
			// This gets a handle to the system menu for a window. Once we have that handle, we can add our
			// own menu items.
			IntPtr WindowMenuHandle = Win32.GetSystemMenu(_Handle, false);
			int Index = Win32.GetMenuItemCount(WindowMenuHandle);

			_TransparencyMenuHandle = Win32.CreateMenu();
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS100, "100%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS95, "95%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS90, "90%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS85, "85%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS80, "80%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS75, "75%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANS70, "70%");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION | Win32.MF_SEPARATOR, 0, "");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_FADEOUT, "Fade Out");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_FADEIN, "Fade In");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION | Win32.MF_SEPARATOR, 0, "");
			Win32.InsertMenu(_TransparencyMenuHandle, -1, Win32.MF_BYPOSITION, Form1.SC_TRANSCLEAR, "Default Transparency");

			Win32.InsertMenu(WindowMenuHandle, Index - 2, Win32.MF_BYPOSITION | Win32.MF_POPUP, _TransparencyMenuHandle, "Transparency");
			Win32.InsertMenu(WindowMenuHandle, Index - 2, Win32.MF_BYPOSITION | Win32.MF_SEPARATOR, IntPtr.Zero, "");
		}

		public void DetachWindowMenu()
		{
			// This function removes the Transparency menu we added to the system menu.
			IntPtr WindowMenuHandle = Win32.GetSystemMenu(_Handle, false);
			int Index = Win32.GetMenuItemCount(WindowMenuHandle);
			Win32.RemoveMenu(WindowMenuHandle, Index - 3, Win32.MF_BYPOSITION);
			Win32.RemoveMenu(WindowMenuHandle, Index - 3, Win32.MF_BYPOSITION);

			Win32.DestroyMenu(_TransparencyMenuHandle);
		}

		public void SetTransparency(byte Trans)
		{
			Win32.SetWindowLong(_Handle, Win32.GWL_EXSTYLE, Win32.GetWindowLong(_Handle, Win32.GWL_EXSTYLE) | Win32.WS_EX_LAYERED);
			Win32.SetLayeredWindowAttributes(_Handle, 0, Trans, Win32.LWA_ALPHA);
		}

		public void ClearTransparency()
		{
			Win32.SetWindowLong(_Handle, Win32.GWL_EXSTYLE, _OriginalWindowLongExStyle);
		}

		public void FadeOut()
		{
			// This starts a thread which gradually fades out a window.
			if (_FadeInThread != null && _FadeInThread.IsAlive)
				_FadeInThread.Abort();

			if (_FadeOutThread == null || !_FadeOutThread.IsAlive)
			{
				_FadeOutThread = new Thread(new ThreadStart(FadeOutProc));
				_FadeOutThread.Start();
			}
		}

		public void FadeOutProc()
		{
			for (byte i = 255; i > 120; --i)
			{
				SetTransparency(i);
				Thread.Sleep(50);
			}
		}

		public void FadeIn()
		{
			// This starts a thread which gradually fades in a window. Fading in is faster than fading out. Not
			// sure why I did that, but I know I had a good reason at the time.
			if (_FadeOutThread != null && _FadeOutThread.IsAlive)
				_FadeOutThread.Abort();

			if (_FadeInThread == null || !_FadeInThread.IsAlive)
			{
				_FadeInThread = new Thread(new ThreadStart(FadeInProc));
				_FadeInThread.Start();
			}
		}

		public void FadeInProc()
		{
			for (byte i = 120; i < 255; ++i)
			{
				SetTransparency(i);
				Thread.Sleep(10);
			}

			ClearTransparency();
		}
	}
}
