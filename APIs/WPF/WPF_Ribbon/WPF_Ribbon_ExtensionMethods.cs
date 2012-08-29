// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Windows;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using O2.Kernel;
using O2.DotNetWrappers.H2Scripts;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.Kernel.ExtensionMethods;
using Microsoft.Windows.Controls.Ribbon;
using O2.XRules.Database.APIs;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:WPF_Ribbon.cs

namespace O2.XRules.Database.Utils
{	
	public static class WPF_Ribbon_ExtensionMethods
    {
    	
    	public static Ribbon ribbon(this WinForms.Control control)
    	{
    		var ribbon = control.control<System.Windows.Forms.Integration.ElementHost>(true)
    						    .Child.control_Wpf<Ribbon>();
			if (ribbon.notNull())
				return ribbon;
				
			"Could not find Ribbon control in control: {0}".error(control.str());
			return null;			
    	}
    	
    	public static WPF_Ribbon wpfRibbon(this WinForms.Control control)
    	{
			var wpfRibbon = control.control<WPF_Ribbon>(true); 
			if (wpfRibbon.isNull())
				"Could not find WPF_Ribbon control in control: {0}".error(control.str());
			return wpfRibbon;
    	}
    	
    	public static WPF_Ribbon title(this WPF_Ribbon wpfRibbon, string title)
    	{
    		wpfRibbon.invokeOnThread(()=> 
			{
				var label = new System.Windows.Controls.Label();
				label.Content =title;
				label.FontSize = 10;  
				label.Padding = new System.Windows.Thickness(0,0,0,0);
				wpfRibbon.Ribbon.Title = label; 
			});
			return wpfRibbon;
    	}
    	
    	public static List<RibbonTab> tabs(this WPF_Ribbon wpfRibbon)
    	{
    		return wpfRibbon.Ribbon.tabs();
    	}
    	
    	public static RibbonTab tab(this WPF_Ribbon wpfRibbon, string header)
    	{
    		return wpfRibbon.Ribbon.tab(header);
    	}
    	
    	public static List<RibbonTab> tabs(this Ribbon ribbon)
    	{
    		return ribbon.items<RibbonTab>();    		
    	}
    	
    	public static List<RibbonGroup> groups(this Ribbon ribbon)
    	{
    		var groups = new List<RibbonGroup>();
    		foreach(var tab in ribbon.tabs())
    			groups.add(tab.groups());
    		return groups;    		
    	}
    	
    	public static RibbonGroup group(this WPF_Ribbon wpfRibbon, string tabHeader, string groupHeader)
    	{
    		var tab =  wpfRibbon.Ribbon.tab(tabHeader);    		
    		return tab.group(groupHeader);
    	}
    	
    	public static List<RibbonButton> buttons(this Ribbon ribbon)
    	{
    		var buttons = new List<RibbonButton>();
    		foreach(var group in ribbon.groups())
    			buttons.add(group.buttons());
    		return buttons;    		
    	}
    	
    	public static RibbonButton button(this WPF_Ribbon wpfRibbon, string tabHeader, string groupHeader, string buttonLabel)
    	{
    		var tab =  wpfRibbon.Ribbon.tab(tabHeader);    		
    		var group =  tab.group(groupHeader);
    		return group.button(buttonLabel);
    	}
    	
    	public static RibbonTab tab(this Ribbon ribbon, string header)
    	{
    		foreach(var tab in ribbon.tabs())
    			if (tab.header()== header)
    				return tab;    		
    		return ribbon.add_RibbonTab(header);    		
    	}    	    	
		
		public static List<string> headers(this List<RibbonTab> tabs)
		{
			return (from tab in tabs
				    select tab.header()).toList();
		}
		
    	public static string header(this RibbonTab ribbonTab)
    	{
    		return (string)ribbonTab.wpfInvoke(
    			()=> { return ribbonTab.Header; });    		
    	}    	    	
    	    	
    	public static List<RibbonGroup> groups(this RibbonTab ribbonTab)
    	{
    		return ribbonTab.items<RibbonGroup>();    		
    	}
    	
    	public static RibbonGroup group(this RibbonTab ribbonTab, string header)
    	{
    		foreach(var group in ribbonTab.groups())
    			if (group.header()== header)
    				return group;
    		return null;
    	}    	
    	
    	public static string header(this RibbonGroup ribonGroup)
    	{
    		return (string)ribonGroup.wpfInvoke(
    			()=> { return ribonGroup.Header; });    		
    	}
    	
    	public static Ribbon ribbon(this RibbonGroup ribonGroup)
    	{
    		return (Ribbon)ribonGroup.wpfInvoke(
    			()=> { return ribonGroup.Ribbon; });    		
    	}
    	
    	public static Ribbon ribbon(this RibbonTab ribbonTab)
    	{
    		return (Ribbon)ribbonTab.wpfInvoke(
    			()=> { return ribbonTab.Ribbon; });    		
    	}
    	
    	public static RibbonTab tab(this RibbonGroup ribonGroup, string header)
    	{
    		return ribonGroup.ribbon().tab(header);
    	}
    	
    	public static RibbonTab tab(this RibbonTab ribbonTab, string header)
    	{
    		return ribbonTab.ribbon().tab(header);
    	}
    	
    	
    	public static List<RibbonButton> buttons(this RibbonTab ribonTab)
    	{
    		var buttons = new List<RibbonButton>();
    		foreach(var group in ribonTab.groups())
    			buttons.AddRange(group.buttons());
    		return buttons;
    	}
    	
    	public static RibbonButton button(this RibbonTab ribonTab, string label)
    	{
    		foreach(var button in ribonTab.buttons())
    			if (button.label().trim() == label.trim())
    				return button;
    		return null;
    	}
    	
    	public static List<RibbonButton> buttons(this RibbonGroup ribonGroup)
    	{
    		return ribonGroup.items<RibbonButton>();    		
    	}
    	
    	public static RibbonButton button(this RibbonGroup ribonGroup, string label)
    	{
    		if (ribonGroup.notNull())
    			foreach(var button in ribonGroup.buttons())
    				if (button.label().trim() == label.trim())
    					return button;
    		return null;
    	}    	
    	
    	public static string label(this RibbonButton ribbonButton)
    	{
    		return (string)ribbonButton.wpfInvoke(
    			()=> { return ribbonButton.Label; });    		
    	}
    	
    	public static List<string> labels(this List<RibbonButton> ribbonButtons)
    	{
    		return (from ribbonButton in ribbonButtons
    				select ribbonButton.label()).toList();
    	}
    	
    	
    	/*public static RibbonTab menu(this WPF_Ribbon wpfRibbon, string name)
    	{
    		return wpfRibbon.Ribbon.tab(header);
    	}*/    	    	    	
    	
    }
    
    
    public static class WPF_Ribbon_ExtensionMethods_Mouse
    {
		public static RibbonTab click(this RibbonTab ribbonTab)
    	{
    		return ribbonTab.selected();
    	}
    	
    	public static RibbonTab selected(this RibbonTab ribbonTab)
    	{
    		return (RibbonTab)ribbonTab.wpfInvoke(
    			()=>{    					
    					ribbonTab.IsSelected=true;
    					return ribbonTab;
    				});    		
    	}		    	    	
    	
    	public static RibbonButton button_Click(this WPF_Ribbon wpfRibbon, string tabHeader, string groupHeader, string buttonLabel)
    	{
    		return wpfRibbon.Ribbon.button_Click(tabHeader, groupHeader, buttonLabel);
    	}
    	
    	public static RibbonButton button_Click(this Ribbon Ribbon, string tabHeader, string groupHeader, string buttonLabel)
    	{
    		var tab =  Ribbon.tab(tabHeader);    		
    		var group =  tab.group(groupHeader);
    		return (RibbonButton)group.button(buttonLabel).click();
    	}
    }
    public static class WPF_Ribbon_ExtensionMethods_Add
    {
    	public static WPF_Ribbon add_Ribbon(this WinForms.Control control)
    	{    		
    		return control.add_Ribbon(false);
    	}
    	
    	public static WPF_Ribbon add_Ribbon(this WinForms.Control control, string versionName)
    	{
    		return control.add_Ribbon(versionName, false);
    	}
    	
    	public static WPF_Ribbon add_Ribbon(this WinForms.Control control, string versionName, bool addAbove)
    	{    		
    		if (versionName.valid())
    			control.add_CustomO2_Text(versionName);
    		return control.add_Ribbon(addAbove);
    	}
    	  	
    	public static WPF_Ribbon add_Ribbon(this WinForms.Control control, bool addAbove)
    	{
    		if (addAbove)
    			return control.add_Ribbon_Above();    		
    		return control.add_Control<WPF_Ribbon>();     		    		
    	}
    	
    	public static WPF_Ribbon add_Ribbon_Above(this WinForms.Control control)
    	{ 
    		var wpfRibbon = control.insert_Above<WinForms.Panel>(136).add_Control<WPF_Ribbon>(); 
    		control.splitContainer().splitterWidth(1); 
    		return wpfRibbon;
    	}
    	
    	public static WPF_Ribbon add_Ribbon_WithLogViewer(this WinForms.Control control)
    	{
    		return control.add_Ribbon_WithLogViewer(null);
    	}
    	
    	public static WPF_Ribbon add_Ribbon_WithLogViewer(this WinForms.Control control, string versionName)
    	{
    		var wpfRibbon = control.insert_Above<WinForms.Panel>(136).add_Ribbon(versionName);
    		control.splitContainer().splitterWidth(1); 
    		control.add_LogViewer();
			//var leftPanel = topPanel.insert_Right<Panel>(200).fill(false).width(200).height(153);
    		//var wpfRibbon = control.add_Ribbon();//insert_Above<WinForms.Panel>(153).add_Control<WPF_Ribbon>(); 
    		return wpfRibbon;
    	}    	    	
    	
    	public static T add_CustomO2_Text<T>(this T control, string versionName)    	
    		where T : WinForms.Control
    	{
    		var pathToImage = "O2Logo_Small.gif".local();     		
    		var leftPanel = control.insert_Right<WinForms.Panel>(200);//.fill(false).width(200).height(153);	  	    
    		control.splitContainer().backColor(System.Drawing.Color.White);//.splitterDistance(200); 
			leftPanel.add_PictureBox().show(pathToImage).fill(false).width(71).height(61);  
			leftPanel.add_Label("version for").left(70).top(15).size(16).font("Arial").font_bold();
			leftPanel.add_Label("").left(0)
					 .top(50).height(80).width(leftPanel.width())
					 .size(16).font("Comic Sans MS")
					 .font_bold().text_Center()
					 .set_Text(versionName);
			//topPanel.backColor("Control");   			
			return control;
    	}
    	
    	public static T add_CustomO2_Image<T>(this T control, string pathToImage)
    		where T : WinForms.Control
    	{
    		if (pathToImage.fileExists().isFalse())
    			pathToImage = pathToImage.local();
    			
    		var leftPanel = control.insert_Right<WinForms.Panel>(200).fill(false).width(200).height(153);		
			leftPanel.add_PictureBox().show(pathToImage).fill(false).width(190).height(140);;
			//topPanel.backColor("Control"); 
			control.parent<WinForms.SplitContainer>().backColor(System.Drawing.Color.White);//.splitterDistance(200); 
			return control;
    	}
    	
    	// helper methods (to make the api easy to use)
    	public static RibbonTab add_Tab(this WPF_Ribbon wpfRibbon, string title)
    	{
    		return wpfRibbon.Ribbon.add_RibbonTab(title);
    	}
    	
    	public static WPF_Ribbon add_Tabs(this WPF_Ribbon wpfRibbon, params string[] titles)
    	{	    		
    		foreach(var title in titles)
    			wpfRibbon.add_Tab(title);
    		return wpfRibbon;
    	}
    	
    	public static RibbonGroup add_Group(this RibbonTab ribbonTab, string title)
    	{
    		return ribbonTab.add_RibbonGroup(title);
    	}
    	
    	public static RibbonGroup add_Button(this RibbonGroup ribbonGroup, string label)
    	{
    		return ribbonGroup.add_RibbonButton(label);
    	}
    	
    	public static RibbonGroup add_Button(this RibbonGroup ribbonGroup, string label, Action onClick)
    	{
    		return ribbonGroup.add_RibbonButton(label,onClick);
    	}
    	
    	public static RibbonGroup add_Button_New(this RibbonGroup ribbonGroup, Action onClick)
    	{
    		return ribbonGroup.add_Button_New("New",onClick);
    	}
    	
    	public static RibbonGroup add_Button_New(this RibbonGroup ribbonGroup, string label, Action onClick)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,"NewDocument_16x16.png", onClick);
    	}
    	
    	public static RibbonGroup add_Button_Open(this RibbonGroup ribbonGroup, Action onClick)
    	{
    		return ribbonGroup.add_Button_Open("Open",onClick);
    	}
    	
    	public static RibbonGroup add_Button_Open(this RibbonGroup ribbonGroup, string label, Action onClick)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,"Open_16x16.png", onClick);
    	}
    	
    	public static RibbonGroup add_Button_Save(this RibbonGroup ribbonGroup, Action onClick)
    	{
    		return ribbonGroup.add_Button_Save("Save",onClick);
    	}
    	
    	public static RibbonGroup add_Button_Save(this RibbonGroup ribbonGroup, string label, Action onClick)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,"Save_16x16.png", onClick);
    	}
    	
    	public static RibbonGroup add_Button(this RibbonGroup ribbonGroup, string label, string smallImage, Action onClick)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,smallImage, onClick);
    	}
    	
    	public static RibbonGroup add_Button_WithImage(this RibbonGroup ribbonGroup, string label, string smallImage, Action onClick)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,smallImage, onClick);
    	}
    	
    	public static RibbonGroup add_Button_WithSmallImage(this RibbonGroup ribbonGroup, string label, string smallImage, Action onClick)
    	{
    		if (smallImage.fileExists().isFalse())
    			smallImage = smallImage.local();
    		return ribbonGroup.add_RibbonButton(label,smallImage, true, onClick);
    	}
    	
    	public static RibbonGroup add_Button_Open_Folder(this RibbonGroup ribbonGroup, string label, Action<string> onValidFolder)
    	{
    		return ribbonGroup.add_Button_WithSmallImage(label,"Open_16x16.png", 
    								()=> O2Thread.staThread(
							 				()=>{ 
							 						var folder = O2Forms.askUserForDirectory("Choose Folder With Images To load");
							 						if (folder.valid() && folder.dirExists())
					    									onValidFolder(folder);
					    						})
					    			);
				    						
    	}   
    	
    	public static RibbonGroup add_Button_Open_Folder(this RibbonGroup ribbonGroup, string label, string path) 
    	{
    		return ribbonGroup.add_RibbonButton_StartProcess(label,path);
    	}
    }
    
    public static class Ribbon_ExtensionMethods
    {
    	public static Ribbon add_Wpf_Ribbon<T>(this T control)
    		where T : WinForms.Control
    	{    	        		
    		return (Ribbon)control.invokeOnThread(
    			()=>{    		
			    		var wpfHost = control.add_WPF_Host();
			    		var grid = wpfHost.add_Grid_Wpf();
							
						grid.backColor(System.Windows.Media.Brushes.White);				
						var rowDefinition = new RowDefinition();  
						rowDefinition.Height = GridLength.Auto;
						grid.RowDefinitions.Add(rowDefinition);
									
						var ribbon = grid.add_Control_Wpf<Ribbon>(); 
						ribbon.ShowQuickAccessToolBarOnTop = false;  // this is not being used						
						return ribbon;
					});
    	}
    	
    	public static RibbonTab add_RibbonTab(this Ribbon ribbon, string header)
    	{
    		var ribbonTab = ribbon.add_Control_Wpf<RibbonTab>(); 
			ribbonTab.prop("Header", header);
			return ribbonTab;
    	}    	    	
    	
    	public static RibbonGroup add_RibbonGroup(this RibbonTab ribbonTab, string title)
    	{
			var ribbonGroup = ribbonTab.add_Control_Wpf<RibbonGroup>(); 
			ribbonGroup.prop("Header", title);
			return ribbonGroup;
    	}
    	
    	
		// this needs the extra dropdown stuff (or is just like the add_RibbonButton)
		/*public static RibbonSplitButton add_RibbonSplitButton(this RibbonGroup ribbonGroup, string imageFile, bool smallImage,  Action onClick)
		{
			return (RibbonSplitButton)ribbonGroup.wpfInvoke(
    			()=>{
						var splitButton = ribbonGroup.add_Control_Wpf<RibbonSplitButton>();												
						splitButton.Label = "Asd";
						if (imageFile.valid())	
						{
							if (imageFile.fileExists().isFalse())
								imageFile = imageFile.local();
							if(smallImage)
								splitButton.SmallImageSource = imageFile.image_Wpf().Source;
							else
								splitButton.LargeImageSource = imageFile.image_Wpf().Source;
						}
						return splitButton;
					});
		}*/
    	
    	public static T add_RibbonButton<T>(this T frameworkElement, string label)
    		where T : FrameworkElement
    	{
    		return frameworkElement.add_RibbonButton(label, null,false, ()=>{});
    	}
    	public static T add_RibbonButton<T>(this T frameworkElement, string label, Action onClick)
    		where T : FrameworkElement
    	{
    		return frameworkElement.add_RibbonButton(label, null,false, onClick);
    	}
    	    
    	public static T add_RibbonButton<T>(this T frameworkElement, string label, string imageFile, bool smallImage,  Action onClick)
    		where T : FrameworkElement
    	{
    		return (T)frameworkElement.wpfInvoke(
    			()=>{
					var ribbonButton = frameworkElement.add_Control_Wpf<RibbonButton>(); 
					ribbonButton.Label = label;
					ribbonButton.onClick_Wpf(onClick);//()=> " I was clicked".info()); 
					if (imageFile.valid())	
					{
						if (imageFile.fileExists().isFalse())
							imageFile = imageFile.local();
						if(smallImage)
						{
							ribbonButton.SmallImageSource =  imageFile.image_Wpf().Source;
							ribbonButton.Label = " " + ribbonButton.Label; // adds an extra space to the right of the image (since the default is too close)
						}
						else
							ribbonButton.LargeImageSource =  imageFile.image_Wpf().Source;
					}					
					return frameworkElement;
				});						
    	}
    	
    	public static RibbonGroup add_Script(this RibbonGroup ribbonGroup, string label, string script)
    	{
    		if (script.extension(".h2"))
    			return ribbonGroup.add_RibbonButton_H2Script(label,script);
    		else
    			return ribbonGroup.add_RibbonButton_Script(label,script);
    	}    
    	
		public static RibbonGroup add_RibbonButton_H2Script(this RibbonGroup ribbonGroup, string label, string h2Script)
		{
			if (h2Script.fileExists().isFalse())
				h2Script = h2Script.local();
			return ribbonGroup.add_RibbonButton(label, ()=>h2Script.executeH2Script());
		}
		
		public static RibbonGroup add_RibbonButton_Script(this RibbonGroup ribbonGroup, string label, string script)
		{
			if (script.fileExists().isFalse())
				script = script.local();
			return ribbonGroup.add_RibbonButton(label, ()=> script.executeFirstMethod());				
		}
		
		public static RibbonGroup add_RibbonButton_H2Script(this RibbonGroup ribbonGroup, WinForms.Control winFormsControl, string label, string h2Script)
		{
			if (h2Script.extension(".h2"))
			{
				if (h2Script.fileExists().isFalse())
					h2Script = h2Script.local();
				var h2 = H2.load(h2Script);
				var scriptText = h2.notNull() ? h2.SourceCode : "";
				return ribbonGroup.add_RibbonButton_Script_GUI(winFormsControl,label, scriptText);
			}
			return ribbonGroup.add_RibbonButton_Script(winFormsControl,label, h2Script);
		}
		
		public static RibbonGroup add_RibbonButton_Script(this RibbonGroup ribbonGroup, WinForms.Control winFormsControl, string label, string script)
		{
			if (script.fileExists().isFalse())
				script = script.local();
			return ribbonGroup.add_RibbonButton_Script_GUI(winFormsControl, label, script.fileContents());
		}
		
		public static RibbonGroup add_RibbonButton_Script_GUI(this RibbonGroup ribbonGroup, WinForms.Control winFormsControl, string label, string sourceCode)
		{
			return ribbonGroup.add_RibbonButton(label,  
				()=>{
						winFormsControl.clear();
						var scriptHost = winFormsControl.add_Script(false ); 						
						
						scriptHost.onCompileOK = 
							()=>{
									"Compiled OK, moving mouse to click".info(); 
									var inputSimulator = new API_InputSimulator();	
									inputSimulator.Move_SkipValue= 3;			// makes it faster 
									//double currentX = inputSimulator.mouse_CurrentPosition().X;
    								//double currentY = inputSimulator.mouse_CurrentPosition().Y;    		
									inputSimulator.mouse_MoveTo(scriptHost.executeButton).mouse_Click(); 									
									//inputSimulator.mouse_MoveTo(currentX, currentY);
									scriptHost.onCompileOK = null;
							 	};
												 
						scriptHost.set_Command(sourceCode); 
						
					});
		}
		
		public static RibbonGroup add_StartProcess(this RibbonGroup ribbonGroup , string label, string process)
		{
			return ribbonGroup.add_RibbonButton_StartProcess(label, process);
		}
		public static RibbonGroup add_RibbonButton_StartProcess(this RibbonGroup ribbonGroup , string label, string process)
		{
			return ribbonGroup.add_RibbonButton_StartProcess(label,process, "");
		}
		public static RibbonGroup add_RibbonButton_StartProcess(this RibbonGroup ribbonGroup , string label, string process, string arguments)
		{
			return ribbonGroup.add_RibbonButton(label, ()=> Processes.startProcess(process, arguments));
		}
		
		public static RibbonGroup add_RibbonButton_ShowCodeFile(this RibbonGroup ribbonGroup, string label, string pathToFile)
		{			
			return ribbonGroup.add_RibbonButton_ShowCodeFile(null, label, pathToFile);
		}
		
		public static RibbonGroup add_RibbonButton_ShowCodeFile(this RibbonGroup ribbonGroup, WinForms.Control winFormsControl, string label, string pathToFile)
		{
			return ribbonGroup.add_RibbonButton(label,  
				()=>{
						if (winFormsControl.isNull())
							winFormsControl = O2Gui.open<WinForms.Panel>("Showing File:{0}".format(pathToFile),800,600);
						if (pathToFile.fileExists().isFalse())
							pathToFile = pathToFile.local();
						winFormsControl.clear();
						winFormsControl.add_SourceCodeViewer().open(pathToFile);
					});
		}
		
		public static RibbonGroup add_RibbonButton_Web(this RibbonGroup ribbonGroup, string label, string url)
		{			
			return ribbonGroup.add_RibbonButton(label,  
				()=>{						
						var panel = O2Gui.open<WinForms.Panel>("Web Browser: " + label, 600,400);
						"Opening url: {0}".info(url);
						panel.add_Control<WinForms.WebBrowser>().open(url);;						
					});
		}
		
		public static RibbonGroup add_RibbonButton_Web(this RibbonGroup ribbonGroup, WinForms.Control winFormsControl, string label, string url)
		{
			return ribbonGroup.add_RibbonButton(label,  
				()=>{						
						winFormsControl.clear();
						"Opening url: {0}".info(url);
						winFormsControl.add_Control<WinForms.WebBrowser>().open(url);;						
					});
		}
		
		//Checkbox
		public static T add_RibbonCheckBox<T>(this T frameworkElement, string label, Action<bool> onClick)
			where T : FrameworkElement
		{
			return frameworkElement.add_RibbonCheckBox(label, false, onClick);
		}
		
		public static T add_RibbonCheckBox<T>(this T frameworkElement, string label, bool value, Action<bool> onClick)		
			where T : FrameworkElement
    	{
			var checkBox = frameworkElement.add_Control_Wpf<RibbonCheckBox>(); 
			checkBox.prop("Label", label);	
			checkBox.prop("IsChecked",value);
			checkBox.Checked+=(sender,e)=>onClick(true);
			checkBox.Unchecked+=(sender,e)=>onClick(false);
			return frameworkElement;
    	}
		
		// TextBox
		
		public static RibbonTextBox add_TextBox(this FrameworkElement frameworkElement, string label,  string text)			
		{
			return frameworkElement.add_RibbonTextBox(label, text, -1);
		}
		
		public static RibbonTextBox add_TextBox(this FrameworkElement frameworkElement, string label,  string text, int width)			
		{
			return frameworkElement.add_RibbonTextBox(label, text, width);
		}
		
		public static RibbonTextBox add_RibbonTextBox(this FrameworkElement frameworkElement, string label,  string text, int width)			
    	{
			var textBox = frameworkElement.add_Control_Wpf<RibbonTextBox>(); 
			if (label.valid())
				textBox.prop("Label", label);
			if (width > -1)
				textBox.prop("TextBoxWidth", (double)width);			
			textBox.set_Text_Wpf(text);
			//textBox.wpfInvoke(()=> textBox.Text = text);
			return textBox;
    	}
    	
    	public static string get_Text_Wpf(this RibbonTextBox textBox)
        {
            return (string)textBox.wpfInvoke(
                () =>
                {
                    return textBox.Text;
                });
        }
        
        public static RibbonTextBox set_Text_Wpf(this RibbonTextBox textBox, string text)
        {
            return (RibbonTextBox)textBox.wpfInvoke(
                () =>
                {
                    textBox.Text = text;
                    return textBox;
                });
        }

		// ComboBox
		
		public static ComboBox add_ComboBox(this FrameworkElement frameworkElement)
		{
			return frameworkElement.add_RibbonComboBox(null, -1);
		}
		
		public static ComboBox add_ComboBox(this FrameworkElement frameworkElement, string text)			
		{
			return frameworkElement.add_RibbonComboBox(text, -1);
		}
				
		public static ComboBox add_RibbonComboBox(this FrameworkElement frameworkElement, string text, int width)			
    	{
			var comboBox = frameworkElement.add_Control_Wpf<ComboBox>(); 			
			if (width > -1)
				//comboBox.prop("ComboBoxWidth", (double)width);			
				width = 100;
			
			comboBox.width_Wpf(width);
			if (text.notNull())
			{
				comboBox.add_Item(text);
				comboBox.selectedIndex(0);
			}
			/*comboBox.wpfInvoke(
				()=>{
						comboBox.SelectionChanged += 
							(sender,e)=>{
											"Selection changed".info();
											comboBox.Text = comboBox.SelectedItem.Content.ToString();
										};
					});
			//textBox.wpfInvoke(()=> textBox.Text = text);*/
			return comboBox;
    	}
    	
    	public static string selectedItem(this ComboBox comboBox)
        {
            return (string)comboBox.wpfInvoke(
                () =>
                {
                    return comboBox.SelectedItem.str();
                });
        }

		public static ComboBox add_Item(this ComboBox comboBox, object itemToAdd)
        {
            return (ComboBox)comboBox.wpfInvoke(
                () =>
                {
                	if (itemToAdd.notNull())                	
                		comboBox.Items.Add(itemToAdd);
                    return comboBox;
                });
        }
        
        public static ComboBox add_Items(this ComboBox comboBox, List<string> itemsToAdd)
        {
            return (ComboBox)comboBox.wpfInvoke(
                () =>
                {
                	if (itemsToAdd.notNull()) 
                		foreach(var item in itemsToAdd)
                			comboBox.Items.Add(item);
                    return comboBox;
                });
        }
        
        public static ComboBox clear(this ComboBox comboBox)
        {
            return (ComboBox)comboBox.wpfInvoke(
                () =>
                {
                	comboBox.Items.Clear();
                    return comboBox;
                });
        }
        
        public static ComboBox selectedIndex(this ComboBox comboBox, int index)
        {
            return (ComboBox)comboBox.wpfInvoke(
                () =>
                {
                	comboBox.SelectedIndex = index;
                	
                	//comboBox.Text = comboBox.Items[index].str();
                    return comboBox;
                });
        }
        
		
//var button1 = ribbonGroup2.add_Control_Wpf<RibbonButton>();
//	button1.Label = "Button1";
	
//	button1.SmallImageSource = "cut16.png".local().image_Wpf().Source;
    	
    }
    
    public static class WPF_Ribbon_ExtensionMethods_API_InputSimulator
    {
    	public static API_InputSimulator click(this API_InputSimulator inputSimulator, RibbonGroup ribbonGroup, int itemToClick)
    	{
    		if (ribbonGroup.notNull() && ribbonGroup.Items.Count > itemToClick)
    		{
				var uiElement = (UIElement)ribbonGroup.Items[itemToClick];
				inputSimulator.mouse_MoveTo_Wpf((uiElement)).click();
			}
			return inputSimulator;
    	}
    }


	public static class WPF_Ribbon_ExtensionMethods_API_CommonlyUsed_Groups
    {
    
    	public static RibbonTab add_Group_FindAndExecuteScripts(this RibbonTab ribbonTab)	
    	{
    		Func<string,List<String>> getLocalFileMappings = 
				(filter)=>{							 
							 var currentFileMappings = CompileEngine.LocalScriptFileMappings;
							 return (from item in currentFileMappings.Keys
							 		 where item.regEx(filter)
							 		 select item).toList().sort();
						  };
						  
			var searchTextBox = ribbonTab.add_Group("Find and Execute O2 Scripts") 
					 				   	 .add_TextBox("RegEx:","",200);

			var comboBox = ribbonTab.group("Find and Execute O2 Scripts") 
	  				 				.add_ComboBox( "(enter a search term above)");
	  				 
			searchTextBox.onTextChange(  
				(text)=> {   														
							var results = getLocalFileMappings(text);							
							comboBox.clear(); 
							comboBox.add_Item("[Found {0} Scripts]".format(results.size()));   
							comboBox.add_Items(results); 
							comboBox.selectedIndex(0);							
						});
			
			ribbonTab.group("Find and Execute O2 Scripts")  
					 .add_Button("Execute Selected Script", 
					  	()=>{ 
					  			var scriptToExecute = comboBox.selectedItem().local(); 
					  			if (scriptToExecute.fileExists())
					  			{
						  			"Executing script:{0}".info(scriptToExecute);
						  			if (scriptToExecute.extension(".h2"))
						  				scriptToExecute.executeH2Script();
						  			else
						  				scriptToExecute.executeFirstMethod();
						  		}
					  		});
			searchTextBox.set_Text(".*h2");
			return ribbonTab;					  		
		}
	}
    
    
}	
    	