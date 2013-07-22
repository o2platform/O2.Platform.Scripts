//File based on the VS project from http://www.codeproject.com/KB/system/WilsonSystemGlobalHooks.aspx

using System;
using System.Collections;
using System.Windows.Forms;
using FluentSharp.CoreLib;

//O2Ref:System.Windows.Forms.dll
//O2File:GlobalHooks.cs
//O2File:Window.cs
//O2File:Win32.cs
namespace TransparencyMenu
{
    /*
     * This application demonstrates using global system hooks. After you click Start, a Transparency menu is added to all open windows, and will automatically be added automatically to any new top-level windows created, even if they're not created by this application. Clicking Stop will, obviously, stop this behavior. The list below shows all windows currently being monitored by this program -- that is, all windows that will have a transparency menu.*/
	public class Form1 : System.Windows.Forms.Form
	{
		// These are the MenuIDs used when items are added to the system menu on a window.
		public static int SC_TRANS100 = 0x4740;
		public static int SC_TRANS95 = 0x4741;
		public static int SC_TRANS90 = 0x4742;
		public static int SC_TRANS85 = 0x4743;
		public static int SC_TRANS80 = 0x4744;
		public static int SC_TRANS75 = 0x4745;
		public static int SC_TRANS70 = 0x4746;
		public static int SC_TRANSCLEAR = 0x4750;
		public static int SC_FADEOUT = 0x4751;
		public static int SC_FADEIN = 0x4752;

		public System.Windows.Forms.Button BtnStart;
		public System.Windows.Forms.Button BtnStop;
		public System.ComponentModel.Container components = null;

		public GlobalHooks _GlobalHooks;
		public bool _IsRunning;
		public System.Windows.Forms.Label label1;
		public System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.ListBox ListWindows;
		public ArrayList _Windows;

		public Form1()
		{
            "In Ctor".info();

			InitializeComponent();

			// Create our GlobalHooks object and setup our hook events
			_GlobalHooks = new GlobalHooks(this.Handle);
//			_GlobalHooks.Shell.WindowCreated += new TransparencyMenu.GlobalHooks.WindowEventHandler(Shell_WindowCreated);
//			_GlobalHooks.Shell.WindowDestroyed += new TransparencyMenu.GlobalHooks.WindowEventHandler(Shell_WindowDestroyed);
			_GlobalHooks.GetMsg.GetMsg += new TransparencyMenu.GlobalHooks.WndProcEventHandler(GetMsg_GetMsg);
		}

		protected override void Dispose( bool disposing )
		{
			// Make sure we remove the hooks before closing the program!
			Stop();

			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		public void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ListWindows = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(8, 120);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(112, 32);
            this.BtnStart.TabIndex = 0;
            this.BtnStart.Text = "Start";
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Location = new System.Drawing.Point(128, 120);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(112, 32);
            this.BtnStop.TabIndex = 1;
            this.BtnStop.Text = "Stop";
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 96);
            this.label1.TabIndex = 3;
            this.label1.Text = "This application demonstrates using global system hooks. After you click Start, a Transparency menu is added to all open windows, and will automatically be added automatically to any new top-level windows created, even if they're not created by this application. Clicking Stop will, obviously, stop this behavior. The list below shows all windows currently being monitored by this program -- that is, all windows that will have a transparency menu.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ListWindows);
            this.groupBox1.Location = new System.Drawing.Point(8, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(352, 280);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Managed Windows";
            // 
            // ListWindows
            // 
            this.ListWindows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListWindows.Location = new System.Drawing.Point(3, 16);
            this.ListWindows.Name = "ListWindows";
            this.ListWindows.Size = new System.Drawing.Size(346, 261);
            this.ListWindows.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(368, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnStop);
            this.Controls.Add(this.BtnStart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		public void Start()
		{
            "starting (and continuing).........".debug();
            
			if (!_IsRunning)
			{
				_IsRunning = true;
				
				// Return a list of all open, titled, top-level windows (excluding the "Program Manager" window, which Microsoft has kept as a top-level window since Windows 3.0, just to be a real pain in the butt to us programmers
				_Windows = new EnumWindowList(true, true, true, new string[] { "Program Manager" });

				// Start hooking
				//_GlobalHooks.Shell.Start();
				_GlobalHooks.GetMsg.Start();
			}
		}

		public void Stop()
		{
			if (_IsRunning)
			{
				// Remove our hooks
				//_GlobalHooks.Shell.Stop();
				_GlobalHooks.GetMsg.Stop();

				// Iterate through all windows we've been monitoring, undoing any changes we've done to them
				if (_Windows != null)
					foreach (Window W in _Windows)
						W.StopManaging();

				_Windows = null;
				_IsRunning = false;
			}
		}	

		public void GetMsg_GetMsg(IntPtr Handle, IntPtr Message, IntPtr wParam, IntPtr lParam)
		{		
			switch(Message.ToInt32())
			{
				case Win32.WM_CREATE:
					"(GetMsg_GetMsg) WM_CREATE".info();
					break;
				case Win32.WM_DESTROY:
					"(GetMsg_GetMsg) WM_DESTROY".info();
					break;	
				case Win32.WM_MOVE:
					"(GetMsg_GetMsg) WM_MOVE".info();
					break;		
				case Win32.WM_SIZE:
					"(GetMsg_GetMsg) WM_SIZE".info();
					break;
				case Win32.WM_ACTIVATE:
					"(GetMsg_GetMsg) WM_ACTIVATE".info();
					break;
				case Win32.WM_COMMAND:
					"(GetMsg_GetMsg) WM_COMMAND".info();
					break;	
				case Win32.WM_SYSCOMMAND:
					"(GetMsg_GetMsg) WM_SYSCOMMAND".info();
					break;
				case Win32.WM_MENUCOMMAND:
					"(GetMsg_GetMsg) WM_MENUCOMMAND".info();
					break;	
				case Win32.WM_MENUSELECT:
					"(GetMsg_GetMsg) WM_MENUSELECT".info();
					break;										
			}
		
		
			// When the user selects an item from the system menu on a window, Windows sends a WM_SYSCOMMAND
			// message to the application, with the MenuID in the low-order word of wParam. If that sentence
			// meant nothing to you, it means you have yet to really experience the joys of programming the
			// Windows API. Anyways, this function is called whenever a message is posted somewhere, so we
			// filter out only the WM_SYSCOMMAND messages, then check to see if they apply to a window that
			// we're monitoring, and then see if it was one of our menu items that was selected.
			if (Message.ToInt32() == Win32.WM_SYSCOMMAND)
			{
				"Received a Win32.WM_SYSCOMMAND".info();
				int LowOrder = (wParam.ToInt32() & 0x0000FFFF);
				int HighOrder = wParam.ToInt32() - LowOrder;

				Window W = FindWindow(Handle);
				if (W != null)
				{
					if (LowOrder == Form1.SC_FADEOUT)
						W.FadeOut();
					else if (LowOrder == Form1.SC_FADEIN)
						W.FadeIn();
					else if (LowOrder == Form1.SC_TRANS100)
						W.SetTransparency(255);
					else if (LowOrder == Form1.SC_TRANS95)
						W.SetTransparency(242);
					else if (LowOrder == Form1.SC_TRANS90)
						W.SetTransparency(230);
					else if (LowOrder == Form1.SC_TRANS85)
						W.SetTransparency(217);
					else if (LowOrder == Form1.SC_TRANS80)
						W.SetTransparency(204);
					else if (LowOrder == Form1.SC_TRANS75)
						W.SetTransparency(191);
					else if (LowOrder == Form1.SC_TRANS70)
						W.SetTransparency(179);
					else if (LowOrder == Form1.SC_TRANSCLEAR)
						W.ClearTransparency();
				}
			}
		}

		public Window FindWindow(IntPtr Handle)
		{
			foreach (Window W in _Windows)
				if (W.Handle == Handle)
					return W;

			return null;
		}

		public void BtnStart_Click(object sender, System.EventArgs e)
		{
			Start();
			UpdateWindowList();
		}

		public void BtnStop_Click(object sender, System.EventArgs e)
		{
			Stop();
			UpdateWindowList();
		}

		public void UpdateWindowList()
		{
			this.ListWindows.Items.Clear();
			if (_Windows != null)
				foreach (Window W in _Windows)
					this.ListWindows.Items.Add(W);
		}
		
		//public int count= 0;
		//count++;
		protected override void WndProc(ref Message m)
		{
			
			// This lets the GlobalHooks class check the message queue for this window to see if it's received
			// any hook messages.
			if (_IsRunning)
				_GlobalHooks.ProcessWindowMessage(ref m);

			base.WndProc(ref m);
		}

		public class EnumWindowList : ArrayList
		{
			public int _EnumComplete = 0;
			public bool _FilterBlanks;
			public bool _FilterHidden;
			public bool _FilterChildren;
			public string[] _FilterStrings;

			public EnumWindowList(bool FilterBlanks, bool FilterHidden, bool FilterChildren, string[] FilterStrings)
			{
				// Creating a new instance of this class returns an ArrayList of Window objects, with one for
				// each window matching the search criteria. I think there must be a better way to create code
				// like this than what I've implemented here -- since EnumWindows is asynchronous, we wait until
				// our callback begins receiving the second round of windows, and then know (or hope) that the
				// callback has been called once for each window.
				_FilterBlanks = FilterBlanks;
				_FilterHidden = FilterHidden;
				_FilterChildren = FilterChildren;
				_FilterStrings = FilterStrings;

				Win32.EnumWindows(new Win32.EnumWindowDelegate(EnumWindowCallBack), 0);
				Win32.EnumWindows(new Win32.EnumWindowDelegate(EnumWindowCallBack), 1);

				while (_EnumComplete == 0)
					System.Windows.Forms.Application.DoEvents();
			}

			public bool EnumWindowCallBack(IntPtr hwnd, int lParam)
			{
				if (lParam == 0)
				{
					Window W = new Window(hwnd);
					bool IsAdd = true;

					if (_FilterBlanks && W.WindowText == "")
						IsAdd = false;
					if (_FilterHidden && !W.IsVisible)
						IsAdd = false;
					if (_FilterChildren && W.GetOwner != IntPtr.Zero)
						IsAdd = false;
					foreach (string S in _FilterStrings)
						if (W.WindowText == S)
							IsAdd = false;

                    //IsAdd = W.WindowText.lower().contains("notepad");
                    if (IsAdd)
                    {
                        "adding window:{0}".debug(W.WindowText);
                        this.Add(W);
                    }
				}

				_EnumComplete = lParam;

				return true;
			}
		}
	}
}
