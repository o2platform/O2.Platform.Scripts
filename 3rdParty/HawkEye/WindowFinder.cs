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
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.ExtensionMethods; 


//O2File:API_WinAPI.cs

namespace O2.XRules.Database.APIs
{
	public class WindowFinder_SimpleGui
	{
		public void launch()
		{			
			var panel = "WindowFinder test".popupWindow(700,450).add_LogViewer();
			var windowFinder =panel.insert_Above(30).add_Control<WindowFinder>().width(30).fill(false);
			//var script = panel.insert_Below().add_Script_Me(windowFinder);
		}
	}
	
	[DefaultEvent("ActiveWindowChanged")]
	public class WindowFinder : UserControl
	{
		public bool searching;		
		public Point lastPoint = Point.Empty;
		public IntPtr Last_Handle				{ get; set; }
		public IntPtr Last_Window				{ get; set; }		
		
		public event EventHandler ActiveWindowChanged;
		
		public event EventHandler ActiveWindowSelected;		
		
		public WindowProperties Window { get; set;}
		
		public Action<IntPtr> Handle_Changed { get; set;}
		public Action<IntPtr> Window_Changed { get; set;}
		
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
				var processId = WinAPI.GetProcessForWindow(this.Window.DetectedWindow); 
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
			/*if ("Hawkeye.gif".local().fileExists())
				this.BackgroundImage = "Hawkeye.gif".local().bitmap(); //SystemUtils.LoadImage("Hawkeye.gif");
			else
				this.pink();*/
			base.Name = "WindowFinder";
			base.Size = new Size(32, 32);
			this.BackgroundImage = O2.Platform.BCL.O2_Views_ASCX.FormImages.target.ToBitmap();
			this.BackgroundImageLayout = ImageLayout.Stretch;
			
		}
		protected override void Dispose(bool disposing)
		{
			this.Window.Dispose();
			base.Dispose(disposing);
		}
		
		public void StartSearch()
		{
			this.searching = true;
			//Cursor.Current = new Cursor(base.GetType().Assembly.GetManifestResourceStream("ACorns.Hawkeye.Resources.Eye.cur"));
			//if ("Eye.cur".local().fileExists())
			//	Cursor.Current = new Cursor("Eye.cur".local());
			//else
			//	Cursor.Current	= Cursors.Cross;
			Cursor.Current = new Cursor(new System.IO.MemoryStream(O2.Platform.BCL.O2_Views_ASCX.FormImages.Eye));
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
			WinAPI.POINT point2 = WinAPI.POINT.FromPoint(point);
			
			IntPtr intPtr = WinAPI.WindowFromPoint(point2);			
			if (intPtr != IntPtr.Zero && WinAPI.ScreenToClient(intPtr, ref point2))
			{
				if(intPtr!= Last_Handle)
				{
					"Handle Change: {0} : {1}".info(intPtr, WinAPI.GetControlText(intPtr));
					Last_Handle.window_Redraw();
					Last_Handle = intPtr;
					Last_Handle.window_Highlight();
					Handle_Changed.invoke(Last_Handle);
				}
				IntPtr intPtr2 = WinAPI.ChildWindowFromPoint(intPtr, point2);
				
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
					"Window Change: {0} : {1}".debug(intPtr, WinAPI.GetWindowText(intPtr));				
					Last_Window = intPtr;					
					Window_Changed.invoke(intPtr);
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
				return WinAPI.GetClassName(this.detectedWindow);
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
				point = WinAPI.NativeScreenToClient(this.detectedWindow, lastPoint);
			}
			
			object lastKnowSelectedObject = this.detectedWindow;
						
			return result;
		}
		public void Refresh()
		{
			/*if (!this.IsValid)
			{
				return;
			}
			IntPtr hWnd = this.detectedWindow;
			hWnd.window_Redraw();*/
			//this.Last_Window.window_Redraw();
			/*
			IntPtr parent = WinAPI.GetParent(hWnd);
			if (parent != IntPtr.Zero)
			{
				hWnd = parent;
			}
			WinAPI.InvalidateRect(hWnd, IntPtr.Zero, true);
			WinAPI.UpdateWindow(hWnd);
			WinAPI.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 1921u);*/
		}
		
		public void Highlight()
		{
			//this.Last_Window.window_Highlight();
			//this.detectedWindow.window_Highlight();
		}
		public void Dispose()
		{
			this.Refresh();
		}
	}
	
	public static class WindowFinder_ExtensionMethods
	{
		public static WindowFinder add_WindowFinder(this Control hostControl)
		{
			return hostControl.add_Control<WindowFinder>();
		}
		
		public static WindowFinder add_WindowFinder(this Control hostControl, ref WindowFinder windowFinder)
		{
			return windowFinder = hostControl.add_Control<WindowFinder>();
		}
		public static WindowFinder add_WindowFinder(this Control hostControl, Action<IntPtr> onHandleChange, ref WindowFinder windowFinder)
		{
			windowFinder = hostControl.add_Control<WindowFinder>();
			windowFinder.Handle_Changed = onHandleChange; 
			return windowFinder;
		}
	}
}
