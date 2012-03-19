// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:_Extra_methods_Items.cs
//O2File:_Extra_methods_Collections.cs
//O2File:_Extra_methods_Windows.cs 

namespace O2.XRules.Database.Utils
{	

	public static class _Extra_Control_extensionMethods_PopupWindow
	{		
		//Control		
		public static Panel popupWindow(this string title)
		{
			return title.showAsForm();
		}		
		
		public static Panel popupWindow(this string title, int width, int height)
		{
			return title.showAsForm(width, height);
		}
			
		public static Panel createForm(this string title)			
		{
			return title.showAsForm();
		}
		
		public static Panel showAsForm(this string title)			
		{
			return title.showAsForm<Panel>(600,400);
		}
		
		public static Panel showAsForm(this string title, int width, int height)			
		{
			return  O2Gui.open<Panel>(title, width,height);
		}
		
		public static T showAsForm<T>(this string title)
			where T : Control
		{
			return title.showAsForm<T>(600,400);
		}
		
		public static T showAsForm<T>(this string title, int width, int height)
			where T : Control
		{
			return (T) O2Gui.open<T>(title, width,height)
						  	.add_H2Icon();
		}		
	}			

	public static class _Extra_Control_extensionMethods
	{
		public static T closeForm<T>(this T control)
			where T : Control
		{
			control.parentForm().close();
			return control;
		}
		
		public static T closeForm_InNSeconds<T>(this T control, int seconds)
			where T : Control
		{
			O2Thread.mtaThread(
				()=>{
						control.sleep(seconds*1000);
						control.closeForm();
					});
			return control;
		}
		
		// new one
		public static T resizeFormToControlSize<T>(this T control, Control controlToSync)
			where T : Control
		{
			if (controlToSync.notNull())
			{
				var parentForm = control.parentForm();
				if (parentForm.notNull())
				{
					var top = controlToSync.PointToScreen(System.Drawing.Point.Empty).Y;
					var left = controlToSync.PointToScreen(System.Drawing.Point.Empty).X;
					var width = controlToSync.Width;
					var height = controlToSync.Height;
					"Setting parentForm location to {0}x{1} : {2}x{3}".info(top, left, width, height);
					parentForm.Top = top;
					parentForm.Left = left;
					parentForm.Width = width;
					parentForm.Height = height;
				}
			}
			return control;
		}
		
		public static string saveImageFromClipboardToFolder(this string targetFolder)
		{
			var targetFile = targetFolder.pathCombine(DateTime.Now.safeFileName() + ".jpg");
			return targetFile.saveImageFromClipboardToFile();
		}
		
		public static string saveImageFromClipboardToFile(this string targetFile)
		{
			var clipboardImagePath = targetFile.saveImageFromClipboard();
			if (clipboardImagePath.fileExists())
			{				
				var fileToSave = (targetFile.valid()) 
										? targetFile
										: O2Forms.askUserForFileToSave(PublicDI.config.O2TempDir,"*.jpg");
				if (fileToSave.valid())
				{
					Files.MoveFile(clipboardImagePath, fileToSave);
					"Clipboard Image saved to: {0}".info(fileToSave);
					return fileToSave;
				}				
			}
			return null;
		}						
		
		
		
		
		//Label

		public static Label autoSize(this Label label, bool value)
		{
			label.invokeOnThread(
				()=>{						
						label.AutoSize = value;
					});
			return label;
		}
		
		public static Label text_Center(this Label label)			
		{			
			label.invokeOnThread(
				()=>{						
						label.autoSize(false);
						label.TextAlign = ContentAlignment.MiddleCenter;
					});
			return label;
		}				
		
		//LinkLabel
		
		public static List<LinkLabel> links(this Control control)
		{
			return control.controls<LinkLabel>(true);
		}
		
		public static LinkLabel link(this Control control, string text)
		{
			foreach(var link in control.links())
				if (link.get_Text() == text)
					return link;
			return null;
		}
		
		public static LinkLabel onClick(this LinkLabel link, Action callback)
		{
			link.invokeOnThread(	
				()=>{
						link.Click += (sender,e) => callback();
					});
			return link;
		}
		
		//Control (Font)			
				
		public static T size<T>(this T control, int value)
			where T : Control
		{
			return control.textSize(value);
		}
		
		public static T size<T>(this T control, string value)
			where T : Control
		{
			return control.textSize(value.toInt());
		}
		
		public static T font<T>(this T control, string fontFamily, string size)
			where T : Control
		{
			return control.font(fontFamily, size.toInt());
		}
		
		public static T font<T>(this T control, string fontFamily, int size)
			where T : Control
		{
			return control.font(new FontFamily(fontFamily), size);
		}
		
		public static T font<T>(this T control, FontFamily fontFamily, string size)
			where T : Control
		{
			return control.font(fontFamily, size.toInt());
		}
		
		public static T font<T>(this T control, FontFamily fontFamily, int size)
			where T : Control
		{
			if (control.isNull())
				return null;
			control.invokeOnThread(
				()=>{
						if (fontFamily.isNull())
							fontFamily = control.Font.FontFamily;
						control.Font = new Font(fontFamily, size);
					});
			return control;
		}
		
		public static T font<T>(this T control, string fontFamily)
			where T : Control
		{
			return control.fontFamily(fontFamily);
		}
		
		public static T fontFamily<T>(this T control, string fontFamily)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font(new FontFamily(fontFamily), control.Font.Size));			
			return control;
		}
		
		public static T textSize<T>(this T control, int value)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font(control.Font.FontFamily, value));			
			return control;
		}
		
		public static T font_bold<T>(this T control)		// just bold() conficts with WPF version
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font( control.Font, control.Font.Style | FontStyle.Bold ));
			return control;
		}
		
		public static T font_italic<T>(this T control)
			where T : Control
		{
			control.invokeOnThread(
				()=> control.Font = new Font( control.Font, control.Font.Style | FontStyle.Italic ));
			return control;
		}
		
		
		
		//Control (KeyPress)			
		
		public static T onKeyPress_getChar<T>(this T control, Func<char, bool> callback)
            where T : Control
        {
            control.KeyPress += (sender, e) => e.Handled = callback(e.KeyChar);
            return control;
        }
        
		public static T onKeyPress_getChar<T>(this T control, Action<char> callback)
            where T : Control
        {
            control.KeyPress += (sender, e) => callback(e.KeyChar);
            return control;
        }
		
		//CheckBox
		public static CheckBox append_CheckBox(this Control control, string text, Action<bool> action)
		{
			return control.append_Control<CheckBox>()
						  .set_Text(text)
						  .autoSize()
						  .onChecked(action);
		}
		public static CheckBox onClick(this CheckBox checkBox, Action<bool> action)
		{
			return checkBox.onChecked(action);
		}
		
		public static CheckBox onChecked(this CheckBox checkBox, Action<bool> action)
		{
			return checkBox.checkedChanged(action);
		}
		public static CheckBox checkedChanged(this CheckBox checkBox, Action<bool> action)
		{
			checkBox.invokeOnThread(
				()=> checkBox.CheckedChanged+= (sender,e) => {action(checkBox.value());});
			return checkBox;
		}
		//WebBrowser
		
		public static WebBrowser onNavigated(this WebBrowser webBrowser, Action<string> callback)
		{
			webBrowser.invokeOnThread(()=> webBrowser.Navigated+= (sender,e)=> callback(e.Url.str()));
			return webBrowser;													
		}
		
		public static WebBrowser add_WebBrowser_with_NavigationBar(this Control control)
		{
			return control.add_WebBrowser_Control().add_NavigationBar();
		}
		
		public static WebBrowser add_NavigationBar(this WebBrowser webBrowser)
		{		
//			var navigationBar = webBrowser.insert_Above(20).add_TextBox("url","");
//			webBrowser.onNavigated((url)=> navigationBar.set_Text(url));
//			navigationBar.onEnter((text)=>webBrowser.open(text));
			Action<string> openUrl = 
				(url)=> {
							"[WebBrowser] opening: {0}".info(url);
							webBrowser.open(url);
						};
						
			var actionPanel = webBrowser.insert_Above(40,"location")
						 	     	    .add_LabelAndComboBoxAndButton("Url","","Go",(text)=>{});
			var comboBox = actionPanel.controls<ComboBox>();
			var button = actionPanel.controls<Button>().onClick(()=> openUrl(comboBox.get_Text())) ;
			comboBox.onEnter(openUrl);
			webBrowser.onNavigated(
				(url)=>{
							if(url != "about:blank")
							 	comboBox.add_Item(url).selectLast();
					   });
			

			return webBrowser;
		}
		//ListBox
		
		public static ListBox add_ListBox(this Control control)
		{
			return control.add_Control<ListBox>();
		}
		
		public static ListBox add_Item(this ListBox listBox, object item)
		{
			return listBox.add_Items(item);
		}
		
		public static ListBox add_Items(this ListBox listBox, params object[] items)
		{
			return (ListBox)listBox.invokeOnThread(
				()=>{
						listBox.Items.AddRange(items);
						return listBox;
					});					
		}
		
		public static object selectedItem(this ListBox listBox)
		{
			return (object)listBox.invokeOnThread(
				()=>{	
						return listBox.SelectedItem;	
					});
		}
		
		public static T selectedItem<T>(this ListBox listBox)
		{			
			var selectedItem = listBox.selectedItem();
			if (selectedItem is T) 
				return (T) selectedItem;
			return default(T);					
		}
		
		public static ListBox select(this ListBox listBox, int selectedIndex)
		{
			return (ListBox)listBox.invokeOnThread(
				()=>{
						if (listBox.Items.size() > selectedIndex)
							listBox.SelectedIndex = selectedIndex;
						return listBox;
					});					
		}
		
		public static ListBox selectFirst(this ListBox listBox)
		{
			return listBox.select(0);
		}
		
		
		//TabControl
		
		public static TabControl remove_Tab(this TabControl tabControl, string text)
		{
			var tabToRemove = tabControl.tab(text);
			if (tabToRemove.notNull())
				tabControl.remove_Tab(tabToRemove);
			return tabControl;
		}
		
		public static bool has_Tab(this TabControl tabControl, string text)
		{
			return tabControl.tab(text).notNull();
		}
		
		public static TabPage tab(this TabControl tabControl, string text)
		{
			foreach(var tab in tabControl.tabs())
				if (tab.get_Text() == text)
					return tab;
			return null;
		}
		public static List<TabPage> tabs(this TabControl tabControl)
		{
			return tabControl.tabPages();
		}
		
		public static List<TabPage> tabPages(this TabControl tabControl)
		{
			return (List<TabPage>)tabControl.invokeOnThread(
									()=>{
											var tabPages = new List<TabPage>();
											foreach(TabPage tabPage in tabControl.TabPages)
												tabPages.Add(tabPage);
											return tabPages;											
										});
		}
		
		//ComboBox
		public static ComboBox add_Items(this ComboBox comboBox, params object[] items)
		{
			foreach(var item in items)			
				comboBox.add_Item(item);			
			return comboBox;
		}				
		
		public static ComboBox selectLast(this ComboBox comboBox)
		{
			return comboBox.select_Item(comboBox.items().size()-1);
		}
		
		public static object selectedItem(this ComboBox comboBox)
		{
			return comboBox.invokeOnThread(
				()=>{
						return comboBox.SelectedItem;
					});
		}
		
		public static ComboBox onSelection(this ComboBox comboBox, Action<object> callback)
		{			
			comboBox.onSelection(
				()=>{						
						callback(comboBox.selectedItem());
					});
			return comboBox;
		}
		
		public static ComboBox executeScriptOnSelection(this ComboBox comboBox, Items mappings)
		{			
			comboBox.onSelection<string>(
						(key)=>{
									if (mappings.hasKey(key))
									{										
										"executing script mapped to '{0}: {1}".info(key, mappings[key]);
										var itemToExecute = mappings[key];
										if (itemToExecute.isUri())
											Processes.startProcess(itemToExecute);
											//itemToExecute.startProcess();
										else
										{
											if(itemToExecute.fileExists().isFalse() && itemToExecute.local().fileExists().isFalse())																							
												CompileEngine.clearLocalScriptFileMappings();											
											"itemToExecute: {0}".debug(itemToExecute);	
											"itemToExecuteextension: {0}".debug(itemToExecute.extension(".o2"));
											if (itemToExecute.extension(".h2") || itemToExecute.extension(".o2"))											
												if (Control.ModifierKeys == Keys.Shift)
												{
													"Shift Key pressed, so launching in new process: {0}".info(itemToExecute);
													itemToExecute.executeH2_or_O2_in_new_Process();
													return;
												}
												
/*												else
												{
													"Shift Key was pressed, so running script in current process".debug(itemToExecute);													
													O2Thread.mtaThread(()=>itemToExecute.executeFirstMethod());
												}
											else*/
											O2Thread.mtaThread(()=>itemToExecute.executeFirstMethod());
										}
											
									}
								});		
			return comboBox;			
		}
		
		public static ComboBox add_ExecutionComboBox(this Control control, string labelText, int top, int left, Items scriptMappings)
		{
			return control.add_ExecutionComboBox(labelText, top, left, scriptMappings, scriptMappings.keys());
		}
		
		public static ComboBox add_ExecutionComboBox(this Control control, string labelText, int top, int left, Items scriptMappings, List<string> comboBoxItems)
		{						
			return control.add_Label(labelText, top, left)
		 				  .append_Control<ComboBox>().top(top-3).dropDownList() // .width(100)		 				  
 						  .add_Items(comboBoxItems.insert("... select one...").ToArray())
 						  .executeScriptOnSelection(scriptMappings)		 							
 						  .selectFirst(); 
		}
		

		
		/*public static ComboBox onSelection<T>(this ComboBox comboBox, Action<T> callback)
		{			
			comboBox.onSelection(
				()=>{						
						var itemValue = comboBox.selectedItem();
						if (itemValue is T)
							callback((T)itemValue);
					});
			return comboBox;
		}*/
		
		public static ComboBox comboBoxHeight(this ComboBox comboBox, int height)
		{
			return comboBox.dropDownHeight(height);
		}
		
		public static ComboBox dropDownHeight(this ComboBox comboBox, int height)
		{
			return (ComboBox)comboBox.invokeOnThread(
				()=>{
						comboBox.DropDownHeight = height;
						return comboBox;
					});
		}
	}
	
	public static class _Extra_WinFormControls_ExtensionMethods
	{
		public static GroupBox title(this Control control, string title)
		{
			return control.add_GroupBox(title);
		}
		
		public static List<Control> add_1x1(this Control control, Action<Control> buildPanel1,  Action<Control> buildPanel2)
		{
			var controls = control.add_1x1();
			buildPanel1(controls[0].add_Panel());
			buildPanel2(controls[1].add_Panel());
			return controls;
		}
		
		public static T insert_LogViewer<T>(this T control)
			where T : Control
		{
			return control.insert_LogViewer(false);
		}
			
		public static T insert_LogViewer<T>(this T control, bool make_Panel1_Fixed)
			where T : Control	
		{
			control.insert_Below(100)
				   .add_LogViewer();
			if (make_Panel1_Fixed)
				control.parent<SplitContainer>()
					   .fixedPanel1();
			return control;
		}
		
		
		// insert_...()
		public static Panel insert_Left(this Control control)
		{
			return control.insert_Left(control.width()/2);			
		}
		
		public static Panel insert_Right(this Control control)
		{
			return control.insert_Right<Panel>(control.width()/2);
		}
		
		public static Panel insert_Above(this Control control)
		{			
			return control.insert_Above<Panel>(control.height()/2);
		}
		
		public static Panel insert_Below(this Control control)
		{
			return control.insert_Below<Panel>(control.height()/2);
		}		
		// insert_...(width)
		public static Panel insert_Left(this Control control, int width)
		{			
			var panel = control.insert_Left<Panel>(width); 
			// to deal with bug in insert_Left<Panel>
			//replace with (when merging extension methods): panel.splitterDistance(width);
			{
				var splitContainer = control.parent<SplitContainer>();
				Ascx_ExtensionMethods.splitterDistance(splitContainer,width);
			}
			
			return panel;
		}
		
		public static Panel insert_Right(this Control control, int width)
		{
			return control.insert_Right<Panel>(width);
		}
		
		public static Panel insert_Above(this Control control, int width)
		{
			return control.insert_Above<Panel>(width);
		}
		
		public static Panel insert_Below(this Control control, int width)
		{
			return control.insert_Below<Panel>(width);
		}
		// insert_...(title)
		public static Panel insert_Left(this Control control, string title)
		{
			return control.insert_Left(control.width()/2, title);
		}
		
		public static Panel insert_Right(this Control control, string title)
		{
			return control.insert_Right(control.width()/2, title);
		}
		
		public static Panel insert_Above(this Control control, string title)
		{
			return control.insert_Above(control.height()/2, title);
		}
		
		public static Panel insert_Below(this Control control, string title)
		{
			return control.insert_Below(control.height()/2, title);
		}
		// insert_...(width, title)
		public static Panel insert_Left(this Control control, int width, string title)
		{
			return control.insert_Left<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Right(this Control control, int width, string title)
		{
			return control.insert_Right<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Above(this Control control, int width, string title)
		{
			return control.insert_Above<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static Panel insert_Below(this Control control, int width, string title)
		{
			return control.insert_Below<Panel>(width).add_GroupBox(title).add_Panel();
		}
		
		public static T white<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.White);
		}
		
		public static T pink<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.LightPink);
		}
		
		public static T green<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.LightGreen);
		}	
		
		public static T azure<T>(this T control)
			where T : Control
		{
			return control.backColor(Color.Azure);
		}	
		
		public static int top<T>(this T control)
			where T : Control
		{
			return (int)control.invokeOnThread(()=>  control.Top);
		}
		public static int left<T>(this T control)
			where T : Control
		{
			return (int)control.invokeOnThread(()=>  control.Left);
		}
		
		public static int width<T>(this T control)
			where T : Control
		{
			return (int)control.invokeOnThread(()=>  control.Width);
		}
		
		public static int height<T>(this T control)
			where T : Control
		{
			return (int)control.invokeOnThread(()=>  control.Height);
		}
		
		public static T align_Right<T>(this T control)
			where T : Control
		{
			return control.align_Right(control.parent());
		}
		
		//add links
		public static LinkLabel add_Link(this Control control, string label, Action onClickCallback)
		{
			return control.add_Link(label, 0,0, ()=> onClickCallback());
		}
		
		public static LinkLabel append_Link_Below(this Control control, string label,int left, Action onClickCallback)
		{
			return control.append_Link_Below(label,onClickCallback).left(left);
		}
		
		public static LinkLabel append_Link_Below(this Control control, string label, Action onClickCallback)
		{
			return control.append_Below_Link(label,onClickCallback);
		}
		
		public static LinkLabel append_Below_Link(this Control control, string label, Action onClickCallback)
		{
			return control.parent().add_Link(label, control.top() + 20 , control.left(), ()=> onClickCallback());
		}
		
		public static Label append_Below_Label(this Control control, string label)
		{
			return control.parent().add_Label(label, control.top() + 22 , control.left());
		}				
		
		//add links (which execute o2 scripts
		
		public static LinkLabel add_Link(this Control control, string label, string script)
		{
			return control.add_Link(label, ()=> script.executeH2Script());
		}
		
		public static LinkLabel append_Link(this Control control, string label, string script)
		{
			return control.append_Link(label, ()=> script.executeH2Script());
		}
		
		public static LinkLabel append_Below_Link(this Control control, string label, string script)
		{
			return control.append_Below_Link(label, ()=> script.executeH2Script());
		}				
		
	}
	
	public static class _extra_Form_ExtensionMethod
	{
		public static Form form_Currently_Running(this string titleOrType)
		{
			return titleOrType.applicationWinForm();
		}
		
		public static Form applicationWinForm(this string titleOrType)
		{
			return titleOrType.applicationWinForms().first();		
		}
		
		public static List<Form> applicationWinForms(this string titleOrType)
		{
			var forms = new List<Form>();
            foreach (Form form in Application.OpenForms)
            	if (form.get_Text() == titleOrType 		||  form.str() == titleOrType || 
				    form.typeFullName() == titleOrType  ||  form.typeName() == titleOrType)
				{
                	forms.Add(form);
                }
            return forms;				    
		}
		
		public static T minimized<T>(this T control)
			where T : Control
		{
			return control.windowState(FormWindowState.Minimized);	
		}
		
		public static T maximized<T>(this T control)
			where T : Control
		{
			return control.windowState(FormWindowState.Maximized);	
		}
		
		public static T normal<T>(this T control)
			where T : Control
		{
			return control.windowState(FormWindowState.Normal);	
		}
		
		public static T windowState<T>(this T control, FormWindowState state)
			where T : Control
		{
			return (T)control.invokeOnThread(
				()=>{
						control.parentForm().WindowState = state;
						return control;
					});
		}
	}
	
	public static class _extra_Form_Icons_ExtensionMethod
	{	
		public static Icon icon(this string iconFile)
		{
			try
			{
				return new Icon(iconFile);
			}
			catch(Exception ex)
			{
				"[icon] {0}".error(ex.Message);
				return null;
			}
		}
		
		public static T set_Form_Icon<T>(this T control, string iconFile)
			where T : Control
		{
			return control.set_Form_Icon(iconFile.icon());
		}
		
		public static T set_Form_Icon<T>(this T control, Icon icon)
			where T : Control
		{
			control.invokeOnThread(()=> control.parentForm().Icon = icon);
			return control;
		}						
		
		public static T add_H2Icon<T>(this T control)
			where T : Control
		{
			return control.set_Form_Icon("H2Logo.ico".local());
		}
		
		public static T parentForm_AlwaysOnTop<T>(this T control)
			where T : Control
		{
			control.parentForm().alwaysOnTop();
			return control;
		}
				
		public static T alwaysOnTop<T>(this T form)
			where T : Form
		{
			form.invokeOnThread(()=> form.TopMost= true);
			return form;
		}
	}

}    	