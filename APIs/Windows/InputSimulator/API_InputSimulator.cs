// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX;
using WindowsInput.Native;
using WindowsInput;
//O2Ref:O2_FluentSharp_WPF.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:WindowsBase.dll
//O2Ref:PresentationCore.dll


namespace O2.XRules.Database.APIs
{
    public class API_InputSimulator
    {    
    	public InputSimulator Input_Simulator { get; set; }
    	public double ScreenWidth  { get; set;}
    	public double ScreenHeight  { get; set;}
    	public double XDelta { get; set; }
    	public double YDelta { get; set; }
    	public bool DebugMode { get; set; }
    	public int Move_SleepValue { get; set; }    
    	public int Move_SkipValue { get; set; }    
    	public int Mouse_MoveTo_X_Offset { get; set; }
    	public int Mouse_MoveTo_Y_Offset { get; set; }
    	
    	public API_InputSimulator()
    	{
    		Input_Simulator = new InputSimulator(); 
    		ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
			ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
			
			XDelta =  (double)65535 / (double)ScreenWidth;  
			YDelta =  (double)65535 / (double)ScreenHeight;// -3;   	
			Move_SleepValue = 1;
			Move_SkipValue = 3;
			DebugMode = false;
    	}    		    	    	    	    	    
    }
    
	public class API_InputSimulator_NativeMethods
	{		
		[DllImport("User32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);
		
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);
		
		[DllImport("user32.dll", SetLastError=true)]		
		public static extern bool GetWindowRect(HandleRef hWnd, out Rect rc);
 
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
		    public int left;
		    public int top;
		    public int right;
		    public int bottom;		    
		}

 

    }
    
	public static class API_InputSimulator_ExtensionMethods
	{
		public static API_InputSimulator window_Normal(this API_InputSimulator inputSimulator, Process process)
		{	
			var windowHandle = process.MainWindowHandle;
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle, (int)WindowsInput.Native.NativeMethods.SW.SHOWNORMAL);
			return inputSimulator;
		}
		public static API_InputSimulator window_Maximized(this API_InputSimulator inputSimulator, Process process)
		{
			var windowHandle = process.MainWindowHandle; 
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle,(int)WindowsInput.Native.NativeMethods.SW.SHOWMAXIMIZED);
			return inputSimulator;
		}
		public static API_InputSimulator window_Minimized(this API_InputSimulator inputSimulator, Process process)
		{
			var windowHandle = process.MainWindowHandle; 
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle, (int)WindowsInput.Native.NativeMethods.SW.SHOWMINIMIZED);
			return inputSimulator;
		}
		public static API_InputSimulator alwaysOnTop(this API_InputSimulator inputSimulator, Process process)
		{				
			return inputSimulator.alwaysOnTop(process,true);
		}
		
		public static API_InputSimulator alwaysOnTop(this API_InputSimulator inputSimulator, Process process, bool value)
		{
			inputSimulator.window_Normal(process);		// make sure the window is not minimized
		
			var HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
			var HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
			
			var windowHandle =  new HandleRef(null, process.MainWindowHandle); 
			HandleRef hWndInsertAfter = value ? HWND_TOPMOST : HWND_NOTOPMOST;
            API_InputSimulator_NativeMethods.SetWindowPos(windowHandle, hWndInsertAfter, 0, 0, 0, 0, 3);
           	return inputSimulator;
		}

		public static API_InputSimulator bringToFront(this API_InputSimulator inputSimulator, Process process)
		{
			var windowHandle = process.MainWindowHandle; 
    		WindowsInput.Native.NativeMethods.SetForegroundWindow(windowHandle);  
    		return inputSimulator;    		
		}
		
		public static API_InputSimulator moveWindow(this API_InputSimulator inputSimulator, Process process, int left, int top, int width, int height)
		{
			var windowHandle = process.MainWindowHandle; 
    		API_InputSimulator_NativeMethods.MoveWindow(windowHandle, left, top, width, height, true); 
    		return inputSimulator;    		
		}
		public static API_InputSimulator_NativeMethods.Rect windowRectangle(this API_InputSimulator inputSimulator, Process process)
		{
			var rect = new API_InputSimulator_NativeMethods.Rect();
			var windowHandle = new HandleRef(null, process.MainWindowHandle); 
			API_InputSimulator_NativeMethods.GetWindowRect(windowHandle, out rect);
			return rect;
		}
		
		public static  API_InputSimulator set_XY_OffsetToWindow(this API_InputSimulator inputSimulator, Process process)
		{					
			var windowRectangle = inputSimulator.windowRectangle( process);
			inputSimulator.Mouse_MoveTo_X_Offset = windowRectangle.left-10;
			inputSimulator.Mouse_MoveTo_Y_Offset = windowRectangle.top-10;
			return inputSimulator;
		}
		
		public static API_InputSimulator wait(this API_InputSimulator inputSimulator, int sleepValue)
  		{
  			Sleep_ExtensionMethods.sleep(null,sleepValue); 
  			return inputSimulator;
  		}
  		
  		/*public static double xPos<T>(this API_InputSimulator inputSimulator, T control)
    		where T : Control
    	{
    		return (double)control.invokeOnThread(
    			()=>{
	    				var location1 = control.PointToScreen(Point.Empty); 
						return (double)location1.X + control.width()/2;
					});
		}
		
		public static double yPos<T>(this API_InputSimulator inputSimulator, T control)
    		where T : Control
    	{
    		return (double)control.invokeOnThread(
    			()=>{
	    				var location1 = control.PointToScreen(Point.Empty); 
						return (double)location1.Y  + control.height()/2;
					});
		}*/						
	}
	
  	public static class API_InputSimulator_ExtensionMethods_Keyboard
  	{
  		public static API_InputSimulator text_Send(this API_InputSimulator inputSimulator, string text)
  		{
  			inputSimulator.Input_Simulator.Keyboard.TextEntry(text);
  			return inputSimulator;
  		}
  		
  		public static API_InputSimulator key_Send(this API_InputSimulator inputSimulator, VirtualKeyCode keyCode)
  		{
  			inputSimulator.Input_Simulator.Keyboard.KeyPress(keyCode);
  			Application.DoEvents();
  			return inputSimulator;
  		}
  		
  		public static API_InputSimulator send_Tab(this API_InputSimulator inputSimulator)
  		{
  			"sending Tab".info();
  			inputSimulator.key_Send(VirtualKeyCode.TAB);
  			return inputSimulator;
  		}
  		public static API_InputSimulator send_Enter(this API_InputSimulator inputSimulator)
  		{
  			inputSimulator.key_Send(VirtualKeyCode.RETURN);
  			"sending Tab".info();
  			return inputSimulator;
  		}
  	}

    public static class API_InputSimulator_ExtensionMethods_Mouse
    {    	
    	public static Point mouse_CurrentPosition(this API_InputSimulator inputSimulator)
    	{
    		return Cursor.Position;
    	}    	
    
    	public static API_InputSimulator mouse_SetPosition(this API_InputSimulator inputSimulator, double x, double y)
    	{
			return inputSimulator.mouse_SetPosition(x,y, true);
    	}
    	
    	public static API_InputSimulator mouse_SetPosition(this API_InputSimulator inputSimulator, double x, double y, bool sleepAfterSetPosition)
    	{
    		var xPos = x * inputSimulator.XDelta; 
			var yPos = y * inputSimulator.YDelta;
			inputSimulator.DebugMode.ifInfo("Setting mouse location to: {0}/{1} :: {2}/{3}".format(x,y, xPos,yPos));
			inputSimulator.Input_Simulator.Mouse.MoveMouseToPositionOnVirtualDesktop(xPos, yPos);
			if (sleepAfterSetPosition)
				inputSimulator.sleep(inputSimulator.Move_SleepValue, false);
			Cursor.Show();
			Application.DoEvents();
			return inputSimulator;
    	}
    	
    	public static API_InputSimulator mouse_MoveBy(this API_InputSimulator inputSimulator, double x, double y)
    	{
    		return inputSimulator.mouse_MoveBy(x,y,true);
    	}
    	
    	public static API_InputSimulator mouse_MoveBy(this API_InputSimulator inputSimulator, double x, double y, bool animate)
    	{
    		if (x != 0 || y != 0)
    		{
    			"moving mouse by:{0} {1}".info(x,y);
    			double currentX = inputSimulator.mouse_CurrentPosition().X;
    			double currentY = inputSimulator.mouse_CurrentPosition().Y;    		
    			double numberOfSteps = (Math.Abs(x) > Math.Abs(y)) ? Math.Abs(x) : Math.Abs(y);
    			double xStep = ((x != 0) ? x / numberOfSteps : 0) * inputSimulator.Move_SkipValue;
    			double yStep = ((y != 0) ? y / numberOfSteps : 0) * inputSimulator.Move_SkipValue;
    			for(int i =0 ; i < numberOfSteps ; i+=inputSimulator.Move_SkipValue)  
    			{
    				currentX += xStep; //(x >0) ? -xStep : -xStep;
    				currentY += yStep; //(y >0) ? -yStep : -yStep;
    				inputSimulator.mouse_SetPosition(currentX, currentY,animate);
    			}
    		}
    		return inputSimulator;    		
    	}
    	
    	public static API_InputSimulator mouse_MoveTo<T>(this API_InputSimulator inputSimulator, T control)
    		where T : Control
    	{
    		return inputSimulator.mouse_MoveTo(control, true);
    	}
    	
    	public static API_InputSimulator mouse_MoveTo<T>(this API_InputSimulator inputSimulator, T control, bool animate)
    		where T : Control
    	{
    		return (API_InputSimulator)control.invokeOnThread(
    			()=>{
	    				var location1 = control.PointToScreen(Point.Empty); 
						var xPos = (double)location1.X + control.width()/2;
						var yPos = (double)location1.Y  + control.height()/2;
						//return inputSimulator;
						return inputSimulator.mouse_MoveTo(xPos, yPos, animate);
    				});    		
    	
    	}
    	
    	public static API_InputSimulator mouse_MoveTo_Wpf<T>(this API_InputSimulator inputSimulator, T uiElement)
    		where T : System.Windows.UIElement
    	{
    		return (API_InputSimulator)O2.DotNetWrappers.ExtensionMethods.WPF_Threading_ExtensionMethods.wpfInvoke(
    			uiElement, 
    				()=>{
    						try
    						{
    							var point = uiElement.PointToScreen(new System.Windows.Point(0, 0)); 
    							return inputSimulator.mouse_MoveTo_Wpf(point);
    						}
    						catch(Exception ex)
    						{
    							ex.log("in mouse_MoveTo_Wpf");
    							return null;
    						}
    					});
    	}
    	
    	public static API_InputSimulator mouse_MoveTo_Wpf(this API_InputSimulator inputSimulator, System.Windows.Point point)
    	{
    		return inputSimulator.mouse_MoveTo(point.X+10, point.Y+2);    		
    	}			
			
			
    	public static API_InputSimulator mouse_MoveTo(this API_InputSimulator inputSimulator, Point point)
    	{
    		return inputSimulator.mouse_MoveTo(point, true);
    	}
    	
    	public static API_InputSimulator mouse_MoveTo(this API_InputSimulator inputSimulator, Point point, bool animate)
    	{
    		return inputSimulator.mouse_MoveTo(point.X, point.Y, animate);
    	}
    	
    	public static API_InputSimulator mouse_MoveTo(this API_InputSimulator inputSimulator, double x, double y)
    	{
    		return inputSimulator.mouse_MoveTo(x,y, true);
    	}
    	
    	public static API_InputSimulator mouse_MoveTo(this API_InputSimulator inputSimulator, double x, double y, bool animate)
    	{
    		x += inputSimulator.Mouse_MoveTo_X_Offset;
    		y += inputSimulator.Mouse_MoveTo_Y_Offset;
    		var currentPosition = inputSimulator.mouse_CurrentPosition();
    		inputSimulator.mouse_MoveBy(x - currentPosition.X, y - currentPosition.Y, animate);    		
			return inputSimulator;
    	}
    	
    	public static API_InputSimulator click(this API_InputSimulator inputSimulator)
    	{
    		return inputSimulator.mouse_Click();
    	}
    	
    	public static API_InputSimulator mouse_LeftDown(this API_InputSimulator inputSimulator)
    	{
    		inputSimulator.Input_Simulator.Mouse.LeftButtonDown();			
    		return inputSimulator;
    	}
    	
    	public static API_InputSimulator mouse_LeftUp(this API_InputSimulator inputSimulator)
    	{
    		inputSimulator.Input_Simulator.Mouse.LeftButtonUp();			
    		return inputSimulator;
    	}
    	
    	public static API_InputSimulator mouse_Click(this API_InputSimulator inputSimulator)
    	{    		
    		inputSimulator.Input_Simulator.Mouse.LeftButtonDown();			
    		inputSimulator.wait(300);
			inputSimulator.Input_Simulator.Mouse.LeftButtonUp();
//			inputSimulator.sleep(500);	
			return inputSimulator;    	
    	}
    	
    	public static API_InputSimulator mouse_RightClick(this API_InputSimulator inputSimulator)
    	{
    		inputSimulator.Input_Simulator.Mouse.RightButtonUp();			
    		//inputSimulator.Input_Simulator.Mouse.LeftButtonDown();			
    		//inputSimulator.sleep(300);
			//inputSimulator.Input_Simulator.Mouse.LeftButtonUp();
//			inputSimulator.sleep(500);	
			return inputSimulator;
    	}    	
    	
    
    }
        
    
    
    	
    	/*
    	var x = (double)0;
var y = (double)0;
var interval = 50;
for(int i=0 ; i < interval ; i ++) 
{	
	x += xDelta * (screenWidth / (double)interval); 
	y += yDelta * (screenHeight / (double)interval);
	"X: {0}   Y: {1}".debug(x,y);
	sim.Mouse.MoveMouseToPositionOnVirtualDesktop(x, y);
	this.sleep(50);	
}
	
return xDelta;
*/    
}
