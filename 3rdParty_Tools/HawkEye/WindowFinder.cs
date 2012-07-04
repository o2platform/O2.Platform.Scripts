//based on the code from HawkEye
using System;
using System.IO;
using System.Text;
using System.Drawing;   
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel; 
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices; 

using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
	public class test_WindowFinder
	{
		public void test()
		{
			var topPanel = "WindowFinder".popupWindow(400,400);
			var activeControl = topPanel.title("Active Control").add_PropertyGrid().helpVisible(false);
			var hawkeye = topPanel.insert_Above(30).add_Control<WindowFinder>().width(30).fill(false);
			var targetProcess = activeControl.insert_Right("Target Process").add_PropertyGrid().helpVisible(false);
			var windowFinder = targetProcess.parent().parent().insert_Below("Window Finder Object").add_PropertyGrid().helpVisible(false);
			
			hawkeye.ActiveWindowChanged += 
				(sender, e)=>{
								windowFinder.show(hawkeye.Window);								
								var control = hawkeye.SelectedControl;								
								activeControl.parent<GroupBox>().set_Text("Active Control: {0}".format(control.typeName()));
								activeControl.show(control);
								var process = hawkeye.TargetProcess;
								targetProcess.show(process);
								targetProcess.parent<GroupBox>().set_Text("Target Process: {0} : {1}".format(process.Id, process.ProcessName));
							 };
		}
	}
	
	[DefaultEvent("ActiveWindowChanged")]
	public class WindowFinder : UserControl
	{
		private bool searching;		
		private Point lastPoint = Point.Empty;
		
		public event EventHandler ActiveWindowChanged;
		
		public event EventHandler ActiveWindowSelected;		
		
		public WindowProperties Window { get; set;}
		
		public Control SelectedControl
		{
			get
			{
				return this.Window.ActiveControl;
			}
		}
		
		public Process TargetProcess 
		{
			get
			{
				var processId = NativeUtils.GetProcessForWindow(this.Window.DetectedWindow); 
				return Process.GetProcessById((int)processId);
			}
		}
	
		[Browsable(false)]
		public IntPtr SelectedHandle
		{
			get
			{
				return this.Window.DetectedWindow;
			}
			set
			{
				this.Window.SetWindowHandle(value, Point.Empty);
				this.InvokeActiveWindowChanged();
			}
		}
		public bool IsManagedByClassName
		{
			get
			{
				return this.Window.IsManagedByClassName;
			}
		}
		public WindowFinder()
		{			
			this.Window = new WindowProperties();
			base.MouseDown += new MouseEventHandler(this.WindowFinder_MouseDown);
			base.Size = new Size(32, 32);
			this.InitializeComponent();
			this.BackgroundImage = "Hawkeye.gif".local().bitmap(); //SystemUtils.LoadImage("Hawkeye.gif");
		}
		protected override void Dispose(bool disposing)
		{
			this.Window.Dispose();
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			this.BackColor = Color.White;
			base.Name = "WindowFinder";
			base.Size = new Size(32, 32);
		}
		public void StartSearch()
		{
			this.searching = true;
			//Cursor.Current = new Cursor(base.GetType().Assembly.GetManifestResourceStream("ACorns.Hawkeye.Resources.Eye.cur"));
			Cursor.Current = new Cursor("Eye.cur".local());
			base.Capture = true;
			base.MouseMove += new MouseEventHandler(this.WindowFinder_MouseMove);
			base.MouseUp += new MouseEventHandler(this.WindowFinder_MouseUp);
		}
		public void EndSearch()
		{
			base.MouseMove -= new MouseEventHandler(this.WindowFinder_MouseMove);
			base.Capture = false;
			this.searching = false;
			Cursor.Current = Cursors.Default;
			if (this.ActiveWindowSelected != null)
			{
				this.ActiveWindowSelected(this, EventArgs.Empty);
			}
		}
		private void WindowFinder_MouseDown(object sender, MouseEventArgs e)
		{
			if (!this.searching)
			{
				this.StartSearch();
			}
		}
		private void WindowFinder_MouseMove(object sender, MouseEventArgs e)
		{
			if (!this.searching)
			{
				this.EndSearch();
			}
			
			Point point = base.PointToScreen(new Point(e.X, e.Y));
			POINT point2 = POINT.FromPoint(point);
			
			IntPtr intPtr = NativeUtils.WindowFromPoint(point2);
			if (intPtr != IntPtr.Zero && NativeUtils.ScreenToClient(intPtr, ref point2))
			{
				IntPtr intPtr2 = NativeUtils.ChildWindowFromPoint(intPtr, point2);
				if (intPtr2 != IntPtr.Zero)
				{
					intPtr = intPtr2;
				}
			}
			if (this.lastPoint != point)
			{
				this.lastPoint = point;
				if (this.Window.SetWindowHandle(intPtr, this.lastPoint))
				{
					this.InvokeActiveWindowChanged();
				}
			}
		}
		private void InvokeActiveWindowChanged()
		{
			if (this.ActiveWindowChanged != null)
			{
				this.ActiveWindowChanged(this, EventArgs.Empty);
			}
		}
		private void WindowFinder_MouseUp(object sender, MouseEventArgs e)
		{
			this.EndSearch();
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			pevent.Graphics.FillRectangle(SystemBrushes.Control, pevent.ClipRectangle);
			base.OnPaintBackground(pevent);
		}
	}
	
	
	
	//was in WindowProperties.cs
	
	public class WindowProperties : IDisposable
	{
		private static Pen drawPen = new Pen(Brushes.Red, 2f);
		private IntPtr detectedWindow = IntPtr.Zero;		
		//private object selectedObject;		
		public IntPtr DetectedWindow { get { return this.detectedWindow; } }				
		private Point lastPoint = Point.Empty;		
		public Control selectedControl;
		
		public Control ActiveControl
		{
			get
			{				
				if (this.detectedWindow != IntPtr.Zero)
				{
					var control = Control.FromHandle(this.detectedWindow);										
					return control;
				}
				return null;
			}
		}
		public string ClassName
		{
			get
			{
				if (!this.IsValid)
				{
					return null;
				}				
				return NativeUtils.GetClassName(this.detectedWindow);
			}
		}
		public bool IsManagedByClassName
		{
			get
			{
				string className = this.ClassName;
				return className != null && className.StartsWith("WindowsForms10");
			}
		}
		public bool IsValid
		{
			get
			{
				return this.detectedWindow != IntPtr.Zero;
			}
		}
		public bool IsManaged
		{
			get
			{
				return this.ActiveControl != null;
			}
		}
		
		internal bool SetWindowHandle(IntPtr handle, Point lastPoint)
		{
			if (detectedWindow != handle)
				this.Refresh();
			this.lastPoint = lastPoint;
			this.detectedWindow = handle;	
			this.Highlight();
			bool result = false;
			var activeControl = this.ActiveControl;			
			if (activeControl != this.selectedControl)
			{
				result = true;
				this.selectedControl = activeControl;					
			}
			Point point = lastPoint;
			if (this.detectedWindow != IntPtr.Zero)
			{
				point = NativeUtils.NativeScreenToClient(this.detectedWindow, lastPoint);
			}
			
			object lastKnowSelectedObject = this.detectedWindow;
			
			/*if (point != Point.Empty && PluginManager.Instance.ResolveSelection(point, lastKnowSelectedObject, ref this.selectedObject))
			{
				result = true;
			}*/
			return result;
		}
		public void Refresh()
		{
			if (!this.IsValid)
			{
				return;
			}
			IntPtr hWnd = this.detectedWindow;
			IntPtr parent = NativeUtils.GetParent(hWnd);
			if (parent != IntPtr.Zero)
			{
				hWnd = parent;
			}
			NativeUtils.InvalidateRect(hWnd, IntPtr.Zero, true);
			NativeUtils.UpdateWindow(hWnd);
			NativeUtils.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 1921u);
		}
		public void Highlight()
		{
			RECT rECT = new RECT(0, 0, 0, 0);
			NativeUtils.GetWindowRect(this.detectedWindow, ref rECT);
			NativeUtils.GetParent(this.detectedWindow);
			IntPtr windowDC = NativeUtils.GetWindowDC(this.detectedWindow);
			if (windowDC != IntPtr.Zero)
			{
				Graphics graphics = Graphics.FromHdc(windowDC, this.detectedWindow);
				graphics.DrawRectangle(WindowProperties.drawPen, 1, 1, rECT.Width - 2, rECT.Height - 2);
				graphics.Dispose();
				NativeUtils.ReleaseDC(this.detectedWindow, windowDC);
			}
		}
		public void Dispose()
		{
			this.Refresh();
		}
	}
	
	
	
	// was in Rect.cs
	
	[Serializable]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
		public int Height
		{
			get
			{
				return this.Bottom - this.Top;
			}
		}
		public int Width
		{
			get
			{
				return this.Right - this.Left;
			}
		}
		public Size Size
		{
			get
			{
				return new Size(this.Width, this.Height);
			}
		}
		public Point Location
		{
			get
			{
				return new Point(this.Left, this.Top);
			}
		}
		public RECT(int Left, int Top, int Right, int Bottom)
		{
			this.Left = Left;
			this.Top = Top;
			this.Right = Right;
			this.Bottom = Bottom;
		}
		public Rectangle ToRectangle()
		{
			return Rectangle.FromLTRB(this.Left, this.Top, this.Right, this.Bottom);
		}
		public static RECT FromRectangle(Rectangle rectangle)
		{
			return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
		}
	}

	//was in Point.cs
	
		public struct POINT
	{
		public int x;
		public int y;
		public POINT(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public POINT ToPoint()
		{
			return new POINT(this.x, this.y);
		}
		public static POINT FromPoint(Point pt)
		{
			return new POINT(pt.X, pt.Y);
		}
		public override bool Equals(object obj)
		{
			if (obj is POINT)
			{
				POINT pOINT = (POINT)obj;
				if (pOINT.x == this.x)
				{
					return pOINT.y == this.y;
				}
			}
			return false;
		}
		public override int GetHashCode()
		{
			return this.x ^ this.y;
		}
		public override string ToString()
		{
			return string.Format("{{X={0}, Y={1}}}", this.x, this.y);
		}
	}


	//was in NativeUtils.cs
	
	public class NativeUtils
	{
		public class WINDOWCMD
		{
			public const int GW_HWNDFIRST = 0;
			public const int GW_HWNDLAST = 1;
			public const int GW_HWNDNEXT = 2;
			public const int GW_HWNDPREV = 3;
			public const int GW_OWNER = 4;
			public const int GW_CHILD = 5;
		}
		public class PROCESSACCESS
		{
			public const int PROCESS_QUERY_INFORMATION = 1024;
		}
		public class WM
		{
			public const int WM_CREATE = 1;
		}
		public class RDW
		{
			public const uint RDW_INVALIDATE = 1u;
			public const uint RDW_INTERNALPAINT = 2u;
			public const uint RDW_ERASE = 4u;
			public const uint RDW_VALIDATE = 8u;
			public const uint RDW_NOINTERNALPAINT = 16u;
			public const uint RDW_NOERASE = 32u;
			public const uint RDW_NOCHILDREN = 64u;
			public const uint RDW_ALLCHILDREN = 128u;
			public const uint RDW_UPDATENOW = 256u;
			public const uint RDW_ERASENOW = 512u;
			public const uint RDW_FRAME = 1024u;
			public const uint RDW_NOFRAME = 2048u;
		}
		public delegate bool IsWow64ProcessHandler(IntPtr processHandle, out bool is64Process);
		public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
		private static bool wow64DirectoryChecked;
		private static bool wow64DirectoryExists;
		private static NativeUtils.IsWow64ProcessHandler isWow64Process;
		private static bool isWow64ProcessChecked;
		private static NativeUtils.IsWow64ProcessHandler IsWow64Process
		{
			get
			{
				if (NativeUtils.isWow64ProcessChecked)
				{
					return NativeUtils.isWow64Process;
				}
				IntPtr moduleHandle = NativeUtils.GetModuleHandle("kernel32");
				IntPtr procAddress = NativeUtils.GetProcAddress(moduleHandle, "IsWow64Process");
				if (procAddress != IntPtr.Zero)
				{
					NativeUtils.isWow64Process = (NativeUtils.IsWow64ProcessHandler)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(NativeUtils.IsWow64ProcessHandler));
				}
				NativeUtils.isWow64ProcessChecked = true;
				return NativeUtils.isWow64Process;
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
					if (!NativeUtils.wow64DirectoryChecked)
					{
						StringBuilder lpBuffer = new StringBuilder(256);
						int systemWow64Directory = NativeUtils.GetSystemWow64Directory(lpBuffer, 256u);
						NativeUtils.wow64DirectoryChecked = true;
						NativeUtils.wow64DirectoryExists = (systemWow64Directory != 0);
					}
					result = NativeUtils.wow64DirectoryExists;
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}
		[DllImport("user32.dll")]
		public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT Point);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);
		[DllImport("gdi32.dll")]
		public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);
		[DllImport("user32.dll")]
		public static extern bool EnumChildWindows(IntPtr hwnd, NativeUtils.WindowEnumProc func, IntPtr lParam);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowRect(IntPtr hwnd, ref RECT rc);
		[DllImport("user32.dll")]
		public static extern bool EnumThreadWindows(int threadId, NativeUtils.WindowEnumProc func, IntPtr lParam);
		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hwnd);
		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hwnd);
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hwnd);
		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hwnd, out IntPtr processID);
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
		[DllImport("user32.dll")]
		public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);
		public static Point NativeScreenToClient(IntPtr window, Point originalPoint)
		{
			POINT pOINT = new POINT(originalPoint.X, originalPoint.Y);
			if (NativeUtils.ScreenToClient(window, ref pOINT))
			{
				return new Point(pOINT.x, pOINT.y);
			}
			return Point.Empty;
		}
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
		[DllImport("user32.dll")]
		public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lpRect, IntPtr hrgnUpdate, uint flags);
		[DllImport("user32.dll")]
		public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
		[DllImport("user32.dll")]
		public static extern bool UpdateWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(POINT Point);
		[DllImport("user32.dll")]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		[DllImport("user32.dll")]
		public static extern IntPtr GetClientRect(IntPtr hwnd, ref RECT rc);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetParent(IntPtr hWnd);
		[DllImport("user32.dll")]
		public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetModuleHandle(string lpModuleName);
		[DllImport("kernel32.dll")]
		public static extern int GetSystemWow64Directory([In] [Out] StringBuilder lpBuffer, [MarshalAs(UnmanagedType.U4)] uint size);
		public static string GetWindowText(IntPtr hwnd)
		{
			int num = NativeUtils.GetWindowTextLength(hwnd) + 1;
			StringBuilder stringBuilder = new StringBuilder(num);
			NativeUtils.GetWindowText(hwnd, stringBuilder, num);
			return stringBuilder.ToString();
		}
		public static string GetClassName(IntPtr hwnd)
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			NativeUtils.GetClassName(hwnd, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}
		public static bool IsTargetInDifferentProcess(IntPtr targetWindowHandle)
		{
			IntPtr intPtr;
			NativeUtils.GetWindowThreadProcessId(targetWindowHandle, out intPtr);
			if (intPtr == IntPtr.Zero)
			{
				return true;
			}
			IntPtr value = new IntPtr(Process.GetCurrentProcess().Id);
			return value != intPtr;
		}
		public static bool IsProcessX64(IntPtr processId)
		{
			if (!NativeUtils.IsWindowsX64)
			{
				return false;
			}
			if (NativeUtils.IsWow64Process == null)
			{
				return false;
			}
			int dwProcessId = processId.ToInt32();
			IntPtr intPtr = NativeUtils.OpenProcess(1024, false, dwProcessId);
			bool flag = false;
			try
			{
				NativeUtils.IsWow64Process(intPtr, out flag);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 0)
				{
					"IsWow64Process LastError: {0}".error(lastWin32Error);
				}
			}
			finally
			{
				NativeUtils.CloseHandle(intPtr);
			}
			return !flag;
		}
		public static IntPtr GetProcessForWindow(IntPtr windowHandle)
		{
			IntPtr result;
			NativeUtils.GetWindowThreadProcessId(windowHandle, out result); 
			return result;
		}
		public static void GetWindowThreadAndProcess(IntPtr windowHandle, out IntPtr threadId, out IntPtr processId)
		{
			threadId = NativeUtils.GetWindowThreadProcessId(windowHandle, out processId);
		}
	}
}
