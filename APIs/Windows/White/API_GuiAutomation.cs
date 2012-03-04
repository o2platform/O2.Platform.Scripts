// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using White.Core;
using White.Core.UIA;
using White.Core.UIItems;
using White.Core.Factory;
using White.Core.WindowsAPI;
using White.Core.InputDevices;
using White.Core.UIItems.Finders;
using White.Core.UIItems.TabItems;
using White.Core.UIItems.TreeItems;
using White.Core.UIItems.MenuItems;
using White.Core.UIItems.WindowItems;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.PropertyGridItems;
using White.Core.UIItems.WindowStripControls;
using White.Core.AutomationElementSearch;
using System.Windows.Automation;

//O2Ref:White.Core.dll
//O2Ref:Bricks.dll
//O2Ref:Bricks.RuntimeFramework.dll
//O2Ref:Castle.Core.dll
//O2Ref:Castle.DynamicProxy2.dll 
//O2Ref:UIAutomationTypes.dll
//O2Ref:UIAutomationClient.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2File:Api_Cropper.cs

namespace O2.XRules.Database.APIs
{
    public class API_GuiAutomation
    {        	
    	public Process TargetProcess { get; set; }
    	public Application Application { get; set; }
    	
    	//Global static var
    	public static int Mouse_Move_SkipValue = 1;		
    	public static int Mouse_Move_SleepValue = 1;	// modify if the mouse moves too fast
    	
        public API_GuiAutomation()
    	{    	
    	}
    	
    	public API_GuiAutomation(Process process)
		{
			TargetProcess = process;
			this.attach(TargetProcess);
		}
		
		public API_GuiAutomation(string title)
		{
			this.attach(title);			
		}
		
		public API_GuiAutomation(int processId)
		{
			this.attach(processId);			
		}
		
		public static API_GuiAutomation currentProcess()
		{
			return new API_GuiAutomation(O2.DotNetWrappers.Windows.Processes.getCurrentProcessID());
		}
    }
    
    public static class API_GuiAutomation_ExtensionMethods
    {
    	public static API_GuiAutomation guiAutomation(this Process process)
    	{
    		return process.automation();
    	}
    	public static API_GuiAutomation automation(this Process process)
    	{
    		return new API_GuiAutomation(process);
    	}
    	
    	public static API_GuiAutomation attach(this API_GuiAutomation guiAutomation, Process process)
    	{
    		guiAutomation.Application = Application.Attach(process);
    		return guiAutomation;
    	}    	    	
    	
    	public static API_GuiAutomation attach(this API_GuiAutomation guiAutomation, string title)
    	{
    		try
    		{
    			guiAutomation.Application = Application.Attach(title);
    			guiAutomation.TargetProcess = guiAutomation.Application.Process;
    			return guiAutomation;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in attach");
    			return null;
    		}
    	}    	    	
    	
    	public static API_GuiAutomation attach(this API_GuiAutomation guiAutomation, int processId)
    	{
    		guiAutomation.Application = Application.Attach(processId);
    		guiAutomation.TargetProcess = guiAutomation.Application.Process;
    		return guiAutomation;
    	}
    	
    	public static API_GuiAutomation launch(this API_GuiAutomation guiAutomation, string executable)
    	{
    		guiAutomation.Application = Application.Launch(executable);
    		guiAutomation.TargetProcess = guiAutomation.Application.Process;
    		return guiAutomation;
    	}
    	
    	public static API_GuiAutomation waitWhileBusy(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.Application.WaitWhileBusy();
    		return guiAutomation;
    	}    	    	    	    	
    	
    	public static API_GuiAutomation stop(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.Application.Kill();
    		return guiAutomation;
    	}
    }
    
    public static class API_GuiAutomation_ExtensionMethods_UIItemContainer
    {
    	public static List<IUIItem> items(this UIItemContainer container)    		
    	{
    		return container.Items;    			   
    	}
    	
    	public static List<T> items<T>(this UIItemContainer container)
    		where T : IUIItem
    	{
    		if (container.notNull())
	    		return (from item in container.Items
	    				where item is T
	    				select (T)item).toList();
	    	return new List<T>();
    	}
    	
    	public static List<Tab> tabs<T>(this T container)
    		where T : UIItemContainer
    	{
    		return container.Tabs;    			   
    	} 
    	
    	public static T find<T>(this IUIItemContainer container, string text)
    		where T : UIItem
    	{
    		return container.get<T>(text);
    	}
    	
    	public static T get<T>(this IUIItemContainer container, string text)
    		where T : UIItem
    	{
    		if (container.notNull())
    		{
	    		var match = container.Get<T>(text);
	    		if (match.notNull())
	    			return match;
	    		return container.Get<T>(SearchCriteria.ByText(text));
	    	}
	    	return null;
    	}    	    	
    	
    	public static T id<T>(this List<T> items, int id)
    		where T : IUIItem
    	{
    		foreach(var item in items)
    			if (item.Id == id.str())
    				return item;
    		return default(T);
    	}
    		
    	public static MenuBar menu(this UIItemContainer container)
    	{
    		if (container.notNull())
    			return container.MenuBar;
    		return null;
    	}
    }	
    
    public static class API_GuiAutomation_ExtensionMethods_IUIItem
    {
    	public static List<string> names(this List<IUIItem> uiItems)
    	{
    		return (from uiItem in uiItems
    			 	select uiItem.Name).toList();
    	}
    	
    	public static string name(this IUIItem uiItem)
    	{
    		return uiItem.Name;
    	}
    	
    	public static int handle(this IUIItem uiItem)
    	{
    		try
    		{
    			if (uiItem.notNull())
    				return uiItem.AutomationElement.Current.NativeWindowHandle;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in IUIItem handle(...)");
    		}
    		return -1;
    	}
    	
    	
    	public static T setFocus<T>(this T uiItem)
    		where T : IUIItem
    	{
    		uiItem.Focus();
    		return uiItem;    		
    	}
    	
    	public static T wait<T>(this T uiItem, int miliseconds)
    		where T : IUIItem
    	{
    		return 	uiItem.wait(miliseconds, false);
    	}
    	
    	public static T wait<T>(this T uiItem, int miliseconds, bool showInLog)
    		where T : IUIItem
    	{
    		Sleep_ExtensionMethods.sleep(uiItem,miliseconds,showInLog);
    		return uiItem;    		
    	}
    	
    	
    	/*public static T sleep<T>(this T uiItem, int delay)  
    		where T : IUIItem
    	{
    		return uiItem.wait(delay);
    	}*/
    	    	
    	
    	public static T waitNSeconds<T>(this T uiItem, int seconds)    		
    		where T : IUIItem
    	{
    		uiItem.wait(seconds * 1000,true);
    		return uiItem;    		
    	}
    }
    
    public static class API_GuiAutomation_ExtensionMethods_UIItem_Helpers
    {        	    	    
    	//Button
    	public static Button button(this UIItemContainer container, string text)    		
    	{
    		if (container.notNull())
    			return container.get<Button>(text);
    		return null;
    	}
    	 
    	public static List<Button> buttons(this UIItemContainer container)    		
    	{
    		return container.items<Button>();
    	}
    	
    	public static List<Button> buttons(this List<Window> windows)    		
    	{
    		var buttons = new List<Button>();
    		foreach(var window in windows)
    			buttons.AddRange(window.buttons());
    		return buttons;
    	}
    	
    	public static bool hasButton(this API_GuiAutomation guiAutomation, string text) //search on the first window
    	{
    		return guiAutomation.button(text).notNull();
    	}
    	
    	public static Button button(this API_GuiAutomation guiAutomation, string text) //search on the first window
    	{
    		return guiAutomation.firstWindow().button(text);
    	}
    	
    	public static Button button(this API_GuiAutomation guiAutomation, string text, int waitCount)
    	{
    		"Trying {0} times to find button '{1}'".info(waitCount,text);
    		for(int i = 0 ; i< waitCount; i ++) 
			{
				guiAutomation.sleep(2000,true); // wait 2 secs and try again  				
				try
				{
					var button = guiAutomation.button(text);
					if (button.notNull())
						return button;
				}
				catch
				{}
			}
			"Could not find button '{0}'".error(text);
    		return null;
    	}
    	
    	public static List<Button> buttons(this API_GuiAutomation guiAutomation)    		
    	{
    		return guiAutomation.firstWindow().buttons();
    	}

		//Link		
    	public static Hyperlink link(this UIItemContainer container, string text)    		
    	{
    		if (container.notNull())
    			return container.get<Hyperlink>(text);
    		return null;
    	}
    	 
    	public static List<Hyperlink> links(this UIItemContainer container)    		
    	{
    		return container.items<Hyperlink>();
    	}
    	
    	public static List<Hyperlink> links(this List<Window> windows)    		
    	{
    		var links = new List<Hyperlink>();
    		foreach(var window in windows)
    			links.AddRange(window.links());
    		return links;
    	}
    	
    	public static bool hasLink(this API_GuiAutomation guiAutomation, string text) //search on the first window
    	{
    		return guiAutomation.link(text).notNull();
    	}
    	
    	public static Button link(this API_GuiAutomation guiAutomation, string text) //search on the first window
    	{
    		return guiAutomation.firstWindow().button(text);
    	}
    	    	    	       	
		public static List<Hyperlink> links(this API_GuiAutomation guiAutomation)    		
    	{
    		return guiAutomation.firstWindow().links();
    	}
    	//Label    	
    	public static Label label(this UIItemContainer container, string text)    		
    	{
    		return container.get<Label>(text);
    	}
    	 
    	public static List<Label> labels(this UIItemContainer container)    		
    	{
    		return container.items<Label>();
    	}
    	
    	public static string text(this Label label)
    	{
    		return label.get_Text();
    	}
    	
    	public static string get_Text(this Label label)    		
    	{
    		return label.Text;
    	}
    	
    	/*public static Label text(this Label label, string text)
    	{
    		return label.set_Text(text);
    	}
    	
    	public static Label set_Text(this Label label, string text)
    	{
    		label.Text = text;
    		return label;
    	}*/
    	
    	//TextBox
    	public static TextBox textBox(this UIItemContainer container, int automationId)    		
    	{
    		foreach(var textBox in container.textBoxes())
    			if(textBox.Id == automationId.str())
    				return textBox;
    		return null;
    	}
    	public static TextBox textBox(this UIItemContainer container, string text)    		
    	{
    		return container.get<TextBox>(text);
    	}
    	 
    	public static List<TextBox> textBoxes(this UIItemContainer container)    		
    	{
    		return container.items<TextBox>();
    	}
    	
    	public static string text(this TextBox textBox)
    	{
    		return textBox.get_Text();
    	}
    	
    	public static string get_Text(this TextBox textBox)    		
    	{
    		return  (textBox.notNull())
    					? textBox.Text
    					: null;
    	}
    	
    	public static TextBox text(this TextBox textBox, string text)
    	{
    		return textBox.set_Text(text);
    	}
    	
    	public static TextBox set_Text(this TextBox textBox, string text)
    	{
    		if (textBox.notNull() && text.notNull())
    			textBox.Text = text;
    		return textBox;
    	}
    	//TabPages
    	public static List<ITabPage> tabPages(this UIItemContainer container)    		
    	{
    		if (container.isNull())
    			return null;    		
    		return (from tab in container.Tabs
		    		from tabPage in tab.Pages
		    		select tabPage).toList();
    	}
    	public static ITabPage tabPage(this UIItemContainer container, string name)
    	{
    		foreach(var tabPage in container.tabPages())
    			if (tabPage.Name == name)
    				return tabPage;
    		return null;
    	}
    	
    	//TreeView
    	public static Tree treeView(this UIItemContainer container)    		
    	{
    		if (container.notNull())
    			return container.Get<Tree>();
    		return null;
    	}
    	
    	public static Tree treeView(this UIItemContainer container, string name)    		
    	{
    		foreach(var treeView in container.treeViews())
    			if (treeView.Name == name)
    				return treeView;
    		return null;
    	}
    	
    	public static List<Tree> treeViews(this UIItemContainer container)    		
    	{
    		return container.items<Tree>();
    	}
    	
    	
    	public static List<TreeNode> treeNodes(this Tree tree)    		
    	{
    		return tree.Nodes;
    	}    	    	
    	
    	//Panels
    	
    	public static Panel panel(this UIItemContainer container, string name)    		
    	{
    		foreach(var panel in container.panels())
    			if (panel.Name == name)
    				return panel;
    		return null;
    	}
    	
    	public static List<Panel> panels(this UIItemContainer container)    		
    	{
    		return container.items<Panel>();
    	}
    	
    	//PropertyGrid
    	public static PropertyGrid propertyGrid(this UIItemContainer container)    		
    	{
    		var propertyGrids = container.items<PropertyGrid>(); 
    		if (propertyGrids.notNull() && propertyGrids.size() >0)
    			return propertyGrids[0];
    		return null;
    	}
    	
    	public static Dictionary<string,string> properties(this PropertyGrid propertyGrid)
    	{
    		var properties = new Dictionary<string,string>();
    		try
    		{
	    		// I have to do this because the code from 	PropertyGridCategoty and PropertyGridProperties has a couple bugs						
				var finder = propertyGrid .field("finder");
				var rows =  (List<AutomationElement>)finder.invoke("FindRows"); 
				foreach(var element in rows)  
				{	
					AutomationPatterns automationPatterns = new AutomationPatterns(element);
				 		var propertyGridProperty = (PropertyGridProperty)typeof(PropertyGridProperty).ctor(element, finder, propertyGrid.ActionListener);	
					properties.add(propertyGridProperty.Text, propertyGridProperty.Value);
				}
			}
			catch(Exception ex)
			{
				ex.log("in PropertyGrid properties");
			}
			"Found {0} propeties in PropertyGridView".info(properties.size());
			return properties;
    	}
    	
    	//Menu    	
    	public static List<Menu> items(this PopUpMenu popUpMenu)
    	{
    		return popUpMenu.menus();
    	}
    	public static List<Menu> menus(this PopUpMenu popUpMenu)
    	{
    		if (popUpMenu.notNull())
    			return (from menu in popUpMenu.Items
    					select menu).toList();
    		return new List<Menu>();
    	}
    	public static MenuBar menuBar(this Window window, string name)
    	{
    		foreach(var menu in window.menuBars())
    			if (menu.Name ==name)
    				return menu;
    		return null;
    	}
    	
    	public static List<MenuBar> menuBars(this Window window)
    	{
    		return window.items<MenuBar>();    		
    	}
    	
    	public static List<Menu> menus(this Window window)
    	{
    		return window.menu().menus();
    	}
    	
    	public static List<Menu> menus(this MenuBar menuBar)
    	{
    		if (menuBar.notNull())
	    		return menuBar.TopLevelMenu;
	    	return null;
    	}
    	
    	public static List<Menu> menus(this Menu menu)
    	{
    		if (menu.notNull())
    			return menu.ChildMenus;
    		return new List<Menu>();
    	}
    	
    	public static List<string> names(this List<Menu> menus)
    	{
    		return (from menu in menus
    				select menu.name()).toList();
    	}
		
		public static Menu menu(this PopUpMenu popUpMenu, string menuToFind)
		{
			return popUpMenu.menus().menu(menuToFind);
		}
		
		public static Menu menu(this List<Menu> menus, string menuToFind)
		{
			foreach(var menu in menus)
				if (menu.name() == menuToFind)
					return menu;
			return null;
    	}
    	//this was not working
    	/*public static Menu menu_Click(this Window window, string menuName, string menuItemName)
    	{	
    		return window.menu_Click(menuName,menuItemName,true);
    	}*/
    	
    	
    	public static Menu menu_Click(this Window window, string menuName, string menuItemName) //,bool animateMouse)
    	{
    		var menu = window.menu(menuName,menuItemName);    		    		
    		//menu.sleep(200);
			return menu.mouse().click();
    	}
    	public static Menu menu(this Window window, string menuName, string menuItemName)
    	{
    		return window.menu(menuName).mouse().click().menu(menuItemName);
    	}
    	
    	public static Menu menu_Click(this Window window, string menuName)
    	{
    		return window.menu(menuName).mouse().click();
    	}
    	
    	public static Menu menu_Click(this Menu menu, string menuName)
    	{
    		return menu.menu(menuName).mouse().click();
    	}
    	
    	public static Menu menu(this Window window, string name)
    	{
    		if (window.isNull())
    			return null;
    		// first use the menus() list
    		foreach(var menu in window.menus())
    			if (menu.name()== name)
    				return menu;
    		// then do a search for it		
    		return window.find<Menu>(name);    		
    	}
    	
    	public static Menu menu(this Menu menu, string name)
    	{
    		foreach(var childMenu in menu.menus())
    			if (childMenu.name()== name)
    				return childMenu;
    		return null;
    	}
    	
    	public static string name(this Menu menu)
    	{
    		return menu.Name;
    	}
    	
    	public static PopUpMenu getContextMenu(this API_GuiAutomation guiAutomation)    	
		{
			try
			{
				var emptyWindow = guiAutomation.desktopWindow("");
				return emptyWindow.Popup;
			}
			catch
			{
			}
			return null;
		}

    	
    	//window
    	public static API_GuiAutomation button_Click(this API_GuiAutomation guiAutomation, string windowName, string buttonName)
    	{
    		var window = guiAutomation.window(windowName);
    		if (window.notNull())
    		{
    			var button = window.button(buttonName);
    			if (button.notNull())
    				button.mouse().click();    			
    		}
    		return guiAutomation;
    	}
    	
    	//radioButton
    	public static RadioButton radioButton(this UIItemContainer container, string text)    		
    	{
    		return container.get<RadioButton>(text);
    	}
    	 
    	public static List<RadioButton> radioButtons(this UIItemContainer container)    		
    	{
    		return container.items<RadioButton>();
    	}
    	
    	public static List<RadioButton> radioButtons(this List<Window> windows)    		
    	{
    		var radioButtons = new List<RadioButton>();
    		foreach(var window in windows)
    			radioButtons.AddRange(window.radioButtons()); 
    		return radioButtons;
    	}
    	
    	//checkBox
    	public static CheckBox checkBox(this UIItemContainer container, string text)    		
    	{
    		return container.get<CheckBox>(text);
    	}
    	 
    	public static List<CheckBox> checkBoxes(this UIItemContainer container)    		
    	{
    		return container.items<CheckBox>();
    	}
    	
    	public static List<CheckBox> checkBoxes(this List<Window> windows)    		
    	{
    		var checkBoxes = new List<CheckBox>();
    		foreach(var window in windows)
    			checkBoxes.AddRange(window.checkBoxes()); 
    		return checkBoxes;
    	}
    	//GroupBox
    	
    	//checkBox
    	public static GroupBox groupBox(this UIItemContainer container, string text)    		
    	{
    		return container.get<GroupBox>(text);
    	}
    	 
    	public static List<GroupBox> groupBoxes(this UIItemContainer container)    		
    	{
    		return container.items<GroupBox>();
    	}
    }
    
    
    public static class API_GuiAutomation_ExtensionMethods_Mouse
    {
    	public static T click<T>(this T uiItem)
    		where T : IUIItem
    	{
    		if (uiItem.notNull())
    			uiItem.Click();
    		return uiItem;    		
    	}
    	
    	public static T doubleClick<T>(this T uiItem)
    		where T : IUIItem
    	{
    		uiItem.DoubleClick();
    		return uiItem;    		
    	}
    	
    	public static T rightClick<T>(this T uiItem)
    		where T : IUIItem
    	{
    		uiItem.RightClick();
    		return uiItem;    		
    	}
    	
    	public static T mouse<T>(this T uiItem)
    		where T : IUIItem
    	{    		
    		if (uiItem.notNull())	    			
    			return uiItem.mouse_MoveTo();
			return default(T);    		
    	}
    	
    	public static T mouse_MoveTo<T>(this T uiItem)
    		where T : IUIItem
    	{
    		if (uiItem.notNull())
    		{    			
    			var location = uiItem.Bounds.Center();
    			Mouse.Instance.mouse_MoveTo(location.X, location.Y,true);
    		}
    		return uiItem;    		
    	}
		
		public static T mouse_MoveTo_WinForm<T>(this T control)		
			where T : System.Windows.Forms.Control 
		{
			var location1 = control.PointToScreen(System.Drawing.Point.Empty); 
			var xPos = (double)location1.X + control.width()/2;
			var yPos = (double)location1.Y  + control.height()/2;
			Mouse.Instance.mouse_MoveTo(xPos, yPos, true);		
			return control;
		}
    	
    	public static API_GuiAutomation mouse_MoveTo(this API_GuiAutomation guiAutomation, double x, double y)
    	{    		
    		Mouse.Instance.mouse_MoveTo(x, y, true);
    		return guiAutomation;
    	}    	
    	
    	public static Mouse mouse_MoveTo(this Mouse mouse, double x, double y, bool animate)
    	{
    		System.Windows.Forms.Application.DoEvents();
    		//"__moving mouse to:{0} {1}".debug(x,y);
    		return 	mouse.mouse_MoveBy(x - mouse.Location.X , y - mouse.Location.Y, animate);
    	}
    	
    	public static API_GuiAutomation mouse_MoveBy(this API_GuiAutomation guiAutomation, double x, double y)
    	{
    		Mouse.Instance.mouse_MoveBy(x, y, true);
    		return guiAutomation;
    	}
    	
    	public static T mouse_MoveBy<T>(this T uiItem, double x, double y)
    		where T : IUIItem
    	{
    		Mouse.Instance.mouse_MoveBy(x, y, true);
    		return uiItem;
    	}
    	
    	public static Mouse mouse_MoveBy(this Mouse mouse, double x, double y, bool animate)
    	{
    		var originalX = mouse.Location.X;
    		var originalY = mouse.Location.Y;
    		"moving mouse by:{0} {1}".debug(x,y);
    		if (animate)
    		{    			    		
	    		if (x != 0 || y != 0)
	    		{    			
	    			double currentX = mouse.Location.X;
	    			double currentY = mouse.Location.Y;
	    			//"__Start position mouse to :{0}x{1}".debug(currentX, currentY);
	    			double numberOfSteps = (Math.Abs(x) > Math.Abs(y)) ? Math.Abs(x) : Math.Abs(y);
	    			double xStep = ((x != 0) ? x / numberOfSteps : 0) * API_GuiAutomation.Mouse_Move_SkipValue;
	    			double yStep = ((y != 0) ? y / numberOfSteps : 0) * API_GuiAutomation.Mouse_Move_SkipValue;
	    			for(int i =0 ; i < numberOfSteps ; i += API_GuiAutomation.Mouse_Move_SkipValue)  
	    			{
	    				currentX += xStep; //(x >0) ? -xStep : -xStep;
	    				currentY += yStep; //(y >0) ? -yStep : -yStep;
	    				//"Moving mouse to :{0}x{1}".info(currentX, currentY);
	    				mouse.Location = new System.Windows.Point(currentX, currentY);
	    				//System.Windows.Forms.Application.DoEvents();
	    				if (API_GuiAutomation.Mouse_Move_SleepValue >0)
	    					mouse.sleep(API_GuiAutomation.Mouse_Move_SleepValue,false);
	    			}
	    			
	    		}
    		}
    		// this makese sure we always end up in the desired location (namely for the cases where the Mouse_Move_SkipValue is quite high)
    		mouse.Location = new System.Windows.Point(originalX + x, originalY + y);    			
    		return mouse;    		
    	}
    	
    	public static Mouse mouse(this API_GuiAutomation guiAutomation)
    	{
    		return Mouse.Instance;
    	}
    	
    	public static Mouse click(this Mouse mouse)
    	{
    		mouse.Click();
    		return mouse;
    	}
    	
    	public static Mouse doubleClick(this Mouse mouse)
    	{
    		var location = mouse.Location;
    		mouse.DoubleClick(location);
    		return mouse;
    	}
    	
    	public static Mouse rightClick(this Mouse mouse)
    	{
    		mouse.RightClick();
    		return mouse;
    	}
    	
    	public static API_GuiAutomation mouse_Click(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.mouse().click();
    		return guiAutomation;
    	}
    	
    	public static API_GuiAutomation mouse_DoubleClick(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.mouse().doubleClick();    		
    		return guiAutomation;
    	}
    	
    	public static API_GuiAutomation mouse_RightClick(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.mouse().rightClick();
    		return guiAutomation;
    	}
    }
    
    public static class API_GuiAutomation_ExtensionMethods_Keyboard
    {
    	public static Keyboard keyboard (this API_GuiAutomation guiAutomation)
    	{
    		return Keyboard.Instance;
    	}
    	
    	public static Keyboard text (this Keyboard keyboard,string text)
    	{
    		keyboard.Enter(text);
    		return keyboard;
    	}
    	
    	public static API_GuiAutomation keyboard_sendText(this API_GuiAutomation guiAutomation, string text)
    	{
    		guiAutomation.keyboard().text(text);
    		return guiAutomation;
    	}
    	
    	public static Keyboard enter (this Keyboard keyboard)
    	{
    		return keyboard.text("".line());
    	}
    	
    	public static Keyboard tab (this Keyboard keyboard)
    	{
    		keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.TAB);
    		return keyboard;
    	}
    }
    
    public static class API_GuiAutomation_ExtensionMethods_Desktop
    {
    	public static List<ListItem> desktopIcons(this API_GuiAutomation guiAutomation)
    	{
    		return Desktop.Instance.Icons;    		    	
    	}
    	
		public static ListItem desktopIcon(this API_GuiAutomation guiAutomation, string name)
    	{
    		foreach(var icon in guiAutomation.desktopIcons())
    			if (icon.Name == name)
    				return icon;
    		return null;
    	}
    	
    	public static List<Window> desktopWindows(this API_GuiAutomation guiAutomation)
    	{
    		for(int i=0; i < 5 ; i++)
    		{
    			try
    			{
    				return WindowFactory.Desktop.DesktopWindows();
    			}
    			catch(Exception ex)
    			{
    				ex.log("in API_GuiAutomation in desktopWindows");
    			}
    		}
    		return null;
    	}
    	
    	public static Window desktopWindow(this API_GuiAutomation guiAutomation, string name)
    	{
    		foreach(var window in guiAutomation.desktopWindows())
    			if(window.Name == name)
    				return window;
    		return null;
    	}
    	
    	public static Window desktopWindow(this API_GuiAutomation guiAutomation, string name, int numberOfTries)
    	{
			for(int i=0; i < numberOfTries ; i++)
			{				
				var window = guiAutomation.desktopWindow(name);
				if (window.notNull())
				{
					"after {0} tries, found window with title: {1}".info(i, name);
					return window;
				}
				guiAutomation.sleep(1000,false);
			}
			"after {0} tries, cound not find window with title: {1}".info(numberOfTries, name);
			return null;
		}
    	
    	public static List<Window> showDesktop(this API_GuiAutomation guiAutomation)
    	{
    		var desktopWindows = guiAutomation.desktopWindows();
    		foreach(var window in desktopWindows)
    			window.minimized();
			return desktopWindows;
		}
    }    
    
    
    public static class API_GuiAutomation_ExtensionMethods_Window
    {	
    	public static List<Window> windows(this API_GuiAutomation guiAutomation)
    	{
    		try
    		{
    			if (guiAutomation.notNull() && guiAutomation.Application.notNull())
    				return guiAutomation.Application.GetWindows();
    		}
    		catch(Exception ex)
    		{
    			ex.log();
    		}
    		return new List<Window>();			// do a soft landing
    	}
    	    	    	
    	public static List<string> names(this List<Window> windows)
    	{
    		return (from window in windows
    		        select window.Name).toList();
    	}
    	
    	public static Window firstWindow(this API_GuiAutomation guiAutomation)
    	{
    		guiAutomation.sleep(100);			// needed on install wizards that change windows quite quickly
    		var windows = guiAutomation.windows();
    		if (windows.size() > 0)
    			return windows[0];
    		return null;
    	}
    	
    	public static Window window(this API_GuiAutomation guiAutomation, string windowName)
    	{    
    		if (guiAutomation.notNull())
    			foreach(var window in guiAutomation.windows())
    				if (window.Name == windowName) 
    					return window;
    		return null;
    	}
    	
    	public static Window window(this API_GuiAutomation guiAutomation, string windowName, int numberOfTries)
    	{
    		for(int i=0; i < numberOfTries ; i++)
			{				
				var window = guiAutomation.window(windowName);
				if (window.notNull())
				{
					"after {0} tries, found window with title: {1}".info(i, windowName);
					return window;
				}
				guiAutomation.sleep(1000,false);
			}
			"after {0} tries, cound not find window with title: {1}".info(numberOfTries, windowName);
			return null;
		}
    	
    	public static Window bringToFront(this Window window)
    	{
    		if (window.notNull())
    		{
    			var windowHandle = window.AutomationElement.Current.NativeWindowHandle;
    			WindowsInput.Native.NativeMethods.SetForegroundWindow(new IntPtr(windowHandle));  
    		}
    		return window;    			
    	}
    	
    	public static Window minimized(this Window window)
    	{
    		try
    		{
    			window.DisplayState = DisplayState.Minimized;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in Window minimized");
    		}
    		return window;
    	}
    	
    	public static Window maximized(this Window window)
    	{
    		try
    		{
    			window.DisplayState = DisplayState.Maximized;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in Window maximized");
    		}	
    		return window;
    	}
    	
    	public static Window restored(this Window window)
    	{
    		try
    		{
    			window.DisplayState = DisplayState.Restored;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in Window restored");
    		}	
    		return window;
    	}
    	
    	
    	public static Window move(this Window window,int left, int top)
    	{
    		var handle = window.handle();
    		var rect = API_GuiAutomation_ExtensionMethods_Window_using_WindowHandle.windowRectangle(null, handle);
    		var right = (rect.right - rect.left); //+ left;
    		var bottom = (rect.bottom - rect.top); //+ top;
    		window.move(left,top, right, bottom);
    		return window;
    	}
    	
    	public static Window move(this Window window,int left, int top, int width, int height)
    	{
    		API_GuiAutomation_ExtensionMethods_Window_using_WindowHandle.moveWindow(null, window.handle(), left, top, width,height);
    		return window;
    	}
    	
    	public static Window alwaysOnTop(this Window window, bool value)
		{
			var windowHandle = window.handle();
			window.restored();		// make sure the window is not minimized
			
			var HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
			var HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
			
			
			HandleRef hWndInsertAfter = value ? HWND_TOPMOST : HWND_NOTOPMOST;
            API_GuiAutomation_NativeMethods.SetWindowPos(windowHandle, hWndInsertAfter, 0, 0, 0, 0, 3);
           	return window;
		}
		
		public static Window syncWithControl(this Window window, System.Windows.Forms.Control control)
		{
			if (window.isNull())
			{
				"[API_GuiAutomation] in syncWithControl, provided window value was null".error();
				return null;
			}
			Action moveToControl = 
				()=>{
						window.alwaysOnTop(true); 
						var xPos =  control.PointToScreen(System.Drawing.Point.Empty).X;
						var yPos =  control.PointToScreen(System.Drawing.Point.Empty).Y;
						var width = control.width();
						var height = control.height();
						window.move(xPos, yPos, width, height);  
					};	
						
			control.parentForm().Move += 
				(sender,e)=> moveToControl();
			 
			control.Resize +=  
				(sender,e)=> moveToControl();
			moveToControl();	
			return window;
		}
		
		public static API_GuiAutomation_NativeMethods.Rect windowRectangle(this Window window)
		{
			var rect = new API_GuiAutomation_NativeMethods.Rect();			
			API_GuiAutomation_NativeMethods.GetWindowRect(window.handle(), out rect);
			return rect;
		}
    }
    
    
    // Legacy (refactor to use methods above
    public static class API_GuiAutomation_ExtensionMethods_Window_using_WindowHandle
    {
    	public static API_GuiAutomation window_Normal(this API_GuiAutomation guiAutomation, int windowHandle)
		{				
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle, (int)WindowsInput.Native.NativeMethods.SW.SHOWNORMAL);
			return guiAutomation;
		}
		public static API_GuiAutomation window_Maximized(this API_GuiAutomation guiAutomation, int windowHandle)
		{			
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle,(int)WindowsInput.Native.NativeMethods.SW.SHOWMAXIMIZED);
			return guiAutomation;
		}
		public static API_GuiAutomation window_Minimized(this API_GuiAutomation guiAutomation, int windowHandle)
		{			
			WindowsInput.Native.NativeMethods.ShowWindow((int)windowHandle, (int)WindowsInput.Native.NativeMethods.SW.SHOWMINIMIZED);
			return guiAutomation;
		}
    	
    	public static API_GuiAutomation alwaysOnTop(this API_GuiAutomation guiAutomation, int windowHandle, bool value)
		{
			guiAutomation.window_Normal(windowHandle);		// make sure the window is not minimized
		
			var HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
			var HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
			
			
			HandleRef hWndInsertAfter = value ? HWND_TOPMOST : HWND_NOTOPMOST;
            API_GuiAutomation_NativeMethods.SetWindowPos(windowHandle, hWndInsertAfter, 0, 0, 0, 0, 3);
           	return guiAutomation;
		}

		public static API_GuiAutomation bringToFront(this API_GuiAutomation guiAutomation, int windowHandle)
		{			
    		WindowsInput.Native.NativeMethods.SetForegroundWindow(new IntPtr(windowHandle));  
    		return guiAutomation;    		
		}
		
		public static API_GuiAutomation moveWindow(this API_GuiAutomation guiAutomation, int windowHandle, int left, int top, int width, int height)
		{			
    		API_GuiAutomation_NativeMethods.MoveWindow(windowHandle, left, top, width, height, true); 
    		return guiAutomation;    		
		}
		
		public static API_GuiAutomation_NativeMethods.Rect windowRectangle(this API_GuiAutomation guiAutomation, int windowHandle)
		{
			var rect = new API_GuiAutomation_NativeMethods.Rect();			
			API_GuiAutomation_NativeMethods.GetWindowRect(windowHandle, out rect);
			return rect;
		}
    }
    
    public static class API_GuiAutomation_ExtensionMethods_Find
    {
/*    	public static AutomationElement.AutomationElementInformation findElement_Dialog(this API_GuiAutomation guiAutomation)
    	{
    		var processID = guiAutomation.TargetProcess.Id;
    		var finder = new AutomationElementFinder(AutomationElement.RootElement);
			var automationElementInformation =  finder.FindWindow(SearchCriteria.ByControlType(ControlType.Window),processID).Current;
			return automationElementInformation;
		}*/
		
    	public static AutomationElement.AutomationElementInformation findElement_Image(this API_GuiAutomation guiAutomation)
    	{
    		var processID = guiAutomation.TargetProcess.Id;
    		var finder = new AutomationElementFinder(AutomationElement.RootElement);
			var automationElementInformation =  finder.FindWindow(SearchCriteria.ByControlType(ControlType.Image),processID).Current;
			return automationElementInformation;
		}
		
		public static Window findWindow_viaImage(this API_GuiAutomation guiAutomation)
    	{
    		return guiAutomation.Application.GetWindow(SearchCriteria.ByControlType(ControlType.Image),InitializeOption.NoCache);
    	}
    }
    
    public static class API_GuiAutomation_Screenshots
    {    
    	public static Bitmap screenshot(this Window window)
    	{
    		var left = window.windowRectangle().left;
			var top = window.windowRectangle().top;
			var right = window.windowRectangle().right;
			var bottom = window.windowRectangle().bottom;
			
			return API_Cropper.captureSta(left,top,right-left,bottom-top ); 			
    	}
    }
    
    //Similar to the ones in API_InputSimulator
    public class API_GuiAutomation_NativeMethods
	{		
		[DllImport("User32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		public static extern bool MoveWindow(int hWnd, int x, int y, int cx, int cy, bool repaint);
		
		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
		public static extern bool SetWindowPos(int hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);
		
		[DllImport("user32.dll", SetLastError=true)]		
		public static extern bool GetWindowRect(int hWnd, out Rect rc);
 
		[StructLayout(LayoutKind.Sequential)]
		public struct Rect
		{
		    public int left;
		    public int top;
		    public int right;
		    public int bottom;		    
		}    	
	}
	
	
	public static class API_GuiAutomation_ExtensionMethods_UseCases
	{	
		public static bool click_Button_in_Window(this API_GuiAutomation guiAutomation, string windowTitle, string buttonText)
		{
			return guiAutomation.click_Button_in_Window(windowTitle, buttonText, false);
		}
		public static bool click_Button_in_Window(this API_GuiAutomation guiAutomation, string windowTitle, string buttonText, bool animateMouse)
		{
			return guiAutomation.click_Button_in_Window(windowTitle,buttonText, animateMouse,  5);
		}
		
		public static bool click_Button_in_Window(this API_GuiAutomation guiAutomation, string windowTitle, string buttonText, bool animateMouse, int timesToTry)
		{			
			Func<bool> check = 
				()=>{
						var o2Timer = new O2Timer("click_Button_in_Window").start();
						
						var scriptErrorWindow = guiAutomation.window(windowTitle);
						if (scriptErrorWindow.isNull()) {}
							 //"didn't find window with title: {0}".error(windowTitle);
						else
						{
							var button = scriptErrorWindow.button(buttonText);
							if (button.isNull()) {}
								// "didn't find button with text: {0}".error(buttonText);	
							else
							{
								"Found it: Clicking on button '{0}' in window '{1}' after {2} tries".debug(windowTitle, buttonText, timesToTry);
								if (animateMouse)
									button.mouse();
								try
								{
									button.click();	
								}
								catch(Exception ex)
								{
									"[API_GuiAutomation][click_Button_in_Window] on or after clicking on button".error(ex.Message);
								}
								o2Timer.stop();
								return true;
							}
						}						
						return false;
					};
					
			"Trying to find  button '{0}' in window '{1}' for {2} tries".info(windowTitle, buttonText, timesToTry);		
					
			for(int i = 0 ; i < timesToTry ; i++)
			{
				var result = check();
				if (result)
					return true;				
			}			
			"Didn't find  button '{0}' in window '{1}' after {2} tries".error(windowTitle, buttonText, timesToTry);
			return false;
		}
		
		public static bool click_Button_in_Window(this string buttonText, string windowTitle)
		{
			return buttonText.click_Button_in_Window(windowTitle,false,5);
		}
		
		public static bool click_Button_in_Window(this string buttonText, string windowTitle, int timesToTry)
		{
			return buttonText.click_Button_in_Window(windowTitle,false, timesToTry);
		}
		
		public static bool click_Button_in_Window(this string buttonText, string windowTitle, bool animateMouse)
		{
			return buttonText.click_Button_in_Window(windowTitle,animateMouse, 5);
		}
		
		public static bool click_Button_in_Window(this string buttonText, string windowTitle, bool animateMouse, int timesToTry)
		{
			return API_GuiAutomation.currentProcess()
									.click_Button_in_Window(windowTitle,buttonText, animateMouse, timesToTry);			
		}		
	}
}