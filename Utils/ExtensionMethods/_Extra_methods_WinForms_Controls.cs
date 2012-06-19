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
using O2.Views.ASCX.Ascx.MainGUI;
using O2.Views.ASCX.O2Findings;
using O2.External.SharpDevelop.ExtensionMethods;
using System.ComponentModel;
//O2File:Scripts_ExtensionMethods.cs

//O2Ref:O2_FluentSharp_BCL.dll

namespace O2.XRules.Database.Utils
{	
	public static class _Extra_ExtensionMethods_ComponentResourceManager
	{
		public static ComponentResourceManager componentResourceManager(this Control control)			
		{
			return control.type().componentResourceManager();
		}
		
		public static ComponentResourceManager componentResourceManager(this Type type)
		{
			return new ComponentResourceManager(type);
		}
		
//		public T ToolStripItem toolStripItem, string name)

		public static T applyResources<T>(this T toolStripItem, string name)
			where T : ToolStripItem
		{
			toolStripItem.toolStrip().applyResources(toolStripItem, name);
			return (T)toolStripItem;
		}
		
		public static T applyResources<T>(this T control, ToolStripItem toolStripItem, string name)
			where T : Control
		{			
			if (control.isNull())
				return control;
			try
			{
				control.componentResourceManager()
					   .ApplyResources(toolStripItem, name);
			}
			catch
			{				
				control.Parent.applyResources(toolStripItem, name);
			}
			return (T)control;
		}
		 
		public static T with_Icon_Open<T>(this T toolStripItem) where T : ToolStripItem
		{
			return toolStripItem.with_Icon(typeof(ascx_FindingsViewer), "btOpenFile");
		}
		
		public static T with_Icon_Save<T>(this T toolStripItem) where T : ToolStripItem
		{
			return toolStripItem.with_Icon(typeof(ascx_FindingsViewer), "btSave");
		}
		
		public static T with_Icon<T>(this T toolStripItem, Type hostType, string name) where T : ToolStripItem
		{
			if (toolStripItem.isNull())			
				"[toolStripItem][with_Icon]: provided toolStripItem value was null".error();
			else
				hostType.componentResourceManager().ApplyResources(toolStripItem,name);
			return toolStripItem;
		}
		
		
		/*public static T applyResources<T>(this T controlWithResourceManager, Control targetControl, string name)
			where T : Control
		{
			"in applyResources 2".info();
			var componentResourceManager = controlWithResourceManager.componentResourceManager();			
			componentResourceManager.ApplyResources(targetControl, name);
			return (T)controlWithResourceManager;
		}*/
		 
	}

	public static class _Extra_extensionMethods_Component
	{
		public static List<Tuple<object,Delegate>> eventHandlers(this Component component)			
		{
			var mappedEvents = new List<Tuple<object,Delegate>>();
			var events = (EventHandlerList)component.prop("Events");
			var next = events.field("head");
			while(next!=null)
			{
				var key = next.field("key");
				
				var handler = (Delegate)next.field("handler");	
				mappedEvents.Add(new Tuple<object,Delegate>(key,handler));
				next = next.field("next");
			}
			return mappedEvents;
		}
		
		public static string eventHandlers_MethodSignatures(this Component component)			
		{
			var signatures = "";
			foreach(var eventHandler in component.eventHandlers())
				if (eventHandler.Item2 != null)
					signatures += eventHandler.Item2.Method.str().line();
			return signatures;
		}
		
		public static T remove_EventHandler<T>(this T component, string eventId)
			where T : Component
		{			
			var eventIdObject =  typeof(T).ctor().field(eventId);
			
			//get private 'Events' property
			var events = (EventHandlerList)component.prop("Events");
			//invoke private method 'find' in order to get the  SelectedIndexChanged event entry 
			var listEntry = events.invoke("Find", eventIdObject);
			//get the private field 'handler'
			var handler = (EventHandler)listEntry.field("handler");
			//now that we have the EventHandler object we can remove it normaly
			events.invoke("RemoveHandler", eventIdObject, handler);
			return (T)component;
		}				
		
		public static T remove_Event_SelectedIndexChanged<T>(this T component)
			where T : Component
		{
			return component.remove_EventHandler("EVENT_SELECTEDINDEXCHANGED");
		}
		
	}
	
	public static class _Extra_extensionMethods_Panel
	{
		public static Panel add_Panel(this Control control, bool clear)
		{
			if (clear)
				control.clear();
			return control.add_Panel();			
		}
	}
	
	public static class _Extra_extensionMethods_TrackBar
	{
		public static TrackBar insert_Above_Slider(this Control control)
		{					
			return control.insert_Above(20).add_TrackBar();
		}
		
		public static TrackBar add_Slider(this Control control)
		{
			return control.add_TrackBar();
		}
		public static TrackBar add_TrackBar(this Control control)
		{
			return control.add_Control<TrackBar>();  
		}
		
		public static TrackBar maximum(this TrackBar trackBar, int value)
		{
			return (TrackBar)trackBar.invokeOnThread(
				()=>{
						trackBar.Maximum = value;
						return trackBar;
					});
		}
		
		public static TrackBar set_Data<T>(this TrackBar trackBar, List<T> data)
		{
			trackBar.Tag = data;  
			trackBar.maximum(data.size());
			return trackBar;
		}
		public static TrackBar onSlide<T>(this TrackBar trackBar, Action<T> onSlide)
		{
			return trackBar.onSlide((index)=>
				{
					var tag = trackBar.Tag;
					if (tag is List<T>)
					{
						var items = (List<T>)tag;
						if (index > items.size())
							"[TrackBar][onSlide] provided index is bigger that items list".error();
						else
							onSlide(items[index]);
					}					
				});				
		}
		
		public static TrackBar onSlide(this TrackBar trackBar, Action<int> onSlideCallback)
		{
			return (TrackBar)trackBar.invokeOnThread(
				()=>{
						trackBar.Scroll+= (sender,e) => onSlideCallback(trackBar.Value);
						return trackBar;
					});
		}
		
	}

	public static class _Extra_extensionMethods_MDIForms
	{
		public static Form mdiForm(this string title)
		{
			return title.mdiHost();
		}
		
		public static Form mdiHost(this string title)
		{
			return title.popupWindow()
						.parentForm()
						.isMdiContainer();								
		}
		
		public static Form isMdiContainer(this Form form, bool value = true)
		{
			return (Form)form.invokeOnThread(
				()=>{
						form.Controls.Clear();
						form.IsMdiContainer = true;
						return form;
					});
		}
		
		public static Form add_MdiChild(this Form parentForm, string title = "")
		{
			return parentForm.add_MdiChild<Form>(title);
		}
		
		public static T add_MdiChild<T>(this Form parentForm, string title)
			where T : Form
		{
			return (T)parentForm.invokeOnThread(
				()=>{			
						var mdiChild = (Form)typeof(T).ctor(); 
						mdiChild.Text = title;
						mdiChild.MdiParent = parentForm;							
						mdiChild.Show();						
						return mdiChild;
					});
		}	
		
		public static Form add_MdiChild(this Form parentForm, Func<Form> formCtor)					
		{
			return (Form)parentForm.invokeOnThread(
				()=>{			
						var mdiChild = formCtor();							
						mdiChild.MdiParent = parentForm;							
						mdiChild.Show();						
						return mdiChild;
					});
		}		
		
		
		public static Form layout(this Form parentForm, MdiLayout layout)					
		{
			return (Form)parentForm.invokeOnThread(
				()=>{			
						parentForm.LayoutMdi(layout);
						return parentForm;
					});
		}
		
		public static Form layout_TileHorizontal(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.TileHorizontal);
		}		
		
		public static Form layout_TileVertical(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.TileVertical);
		}	
		
		public static Form layout_Cascade(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.Cascade);
		}	
		
		public static Form layout_ArrangeIcons(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.ArrangeIcons);
		}					
		
		public static Form tileHorizontaly(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.TileHorizontal);
		}		
		
		public static Form tileVerticaly(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.TileVertical);
		}	
		
		public static Form cascade(this Form parentForm)
		{
			return parentForm.layout(MdiLayout.Cascade);
		}	
									
	}
	
	public static class _Extra_extensionMethods_MDIForms_O2Controls
	{
		public static ascx_LogViewer add_Mdi_LogViewer(this Form parentForm)
		{
			return parentForm.add_MdiChild()
							 .add_LogViewer();
		}
		
		public static ascx_Simple_Script_Editor  add_Mdi_ScriptEditor(this Form parentForm)
		{
			return parentForm.add_MdiChild()
							 .add_Script();
		}
	}
	
	public static class _Extra_extensionMethods_From
	{	
		public static Form maximized(this Form form)
		{
			return (Form)form.invokeOnThread(
				()=>{
						form.WindowState = FormWindowState.Maximized;
						return form;
					});
		}
	}
	
	// ToolStrip
	public static class _Extra_extensionMethods_ToolStrip
	{	
		public static T item<T>(this ToolStrip toolStrip)
			where T : ToolStripItem
		{
			foreach(var item in toolStrip.items())
				if (item is T)
					return (T)item;
			return null;	
		}
		public static T item<T>(this ToolStrip toolStrip, string text)
			where T : ToolStripItem
		{
			var item = toolStrip.item(text);
			if (item.notNull() && item is T)
				return (T)item;
			return null;
		}
		
		public static ToolStrip toolStrip(this ToolStripItem toolStripItem)
		{
			if (toolStripItem.isNull())
			{
				"[ToolStripItem] in toolStrip() provided toolStripItem was null".error();
				//debug.@break();
				return null;
			}
			return toolStripItem.Owner;
		}
		
		public static ToolStripItem item(this ToolStrip toolStrip, string text)
		{
			return toolStrip.items().where((item)=>item.str()== text).first();
		}
		
		public static List<ToolStripItem> items(this ToolStrip toolStrip)
		{
			//return (from item in toolStrip.Items			//doesn't work
			//		select item).toList();
			var items = new List<ToolStripItem>();
			foreach(ToolStripItem item in toolStrip.Items)
				items.add(item);
			return items;
		}
		
		public static ToolStrip clearItems(this ToolStrip toolStrip)
		{
			return (ToolStrip)toolStrip.invokeOnThread(
				()=>{
						toolStrip.Items.Clear();
						return toolStrip;
					});
		}
		
		public static T add_Control<T>(this ToolStripItem toolStripItem, Action<T> onCtor = null)
			where T : ToolStripItem
		{
			return toolStripItem.toolStrip().add_Control(onCtor);
		}
		
		public static T add_Control<T>(this ToolStrip toolStrip, Action<T> onCtor = null)
			where T : ToolStripItem
		{
			return (T)toolStrip.invokeOnThread(
				()=>{
						var item = (T)typeof(T).ctor();
                        if (toolStrip.Items.IsReadOnly)
                        {
                            "[ToolStrip][add_Control] Items collection is in ReadOnly mode".error();
                            return null;
                        }
                        else
                        {
                            toolStrip.Items.Add(item);
                            if (onCtor.notNull())
                                onCtor(item);
                            return item;
                        }
					});
		}
		
		public static ToolStripSeparator add_Separator(this ToolStripItem toolStripItem)
		{
			return toolStripItem.toolStrip().add_Control<ToolStripSeparator>();
		}
		
		public static ToolStripLabel add_Label(this ToolStripItem toolStripItem, string text)
		{
			return toolStripItem.toolStrip().add_Label(text);
		}
		
		public static ToolStripLabel add_Label(this ToolStrip toolStrip, string text)
		{
			return toolStrip.add_Control<ToolStripLabel>((label)=> label.Text = text);			
		}
		
		public static ToolStripButton add_Button_Open(this ToolStripItem toolStripItem, Action onClick = null)
		{
			return toolStripItem.add_Button_Open("open",onClick);
		}
		
		public static ToolStripButton add_Button_Open(this ToolStripItem toolStripItem, string text, Action onClick = null)
		{
			return toolStripItem.add_Button(text, onClick).with_Icon_Open();
		}
		
		public static ToolStripButton add_Button(this ToolStripItem toolStrip, string text, Action onClick = null)
		{
			return toolStrip.add_Button(text, null, onClick);
		}
		
		public static ToolStripButton add_Button(this ToolStrip toolStrip, string text, Action onClick = null)
		{
			return toolStrip.add_Button(text, null, onClick);
		}
		
		public static ToolStripButton add_Button(this ToolStripItem toolStripItem, string text, string resourceName = null,  Action onClick = null)
		{
			return toolStripItem.toolStrip().add_Button(text, resourceName, onClick);
		}				
		
		public static ToolStripButton add_Button(this ToolStrip toolStrip, string text, string resourceName = null, Action onClick = null)
		{			
			if (toolStrip.isNull())
			{
				"[ToolStripButton][add_Button] provided toolStrip was null".error();
				return null;
			}
			return (ToolStripButton)toolStrip.invokeOnThread(
				()=>{						
						var button = new ToolStripButton();						
						toolStrip.Items.Add(button);	                        
						try
						{
							if (resourceName.valid())
							{							
								button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
								button.applyResources(resourceName);
							}
							if (text.valid())						
								button.Text = text;
								
							button.Click+= (sender,e)=>
								{
									if (onClick.notNull())
										onClick();
								};
						}
						catch(Exception ex)
						{
							ex.log("inside toolStrip add_Button");
				 		}
						Application.DoEvents();
						if (button.toolStrip() != toolStrip)
                        {
                            "[ToolStripButton][add_Button] parent was not set, so setting it manuallly".error();
                            button.prop("Parent", toolStrip);
                        }
						return button;
					 } );			
		}
		public static ToolStripTextBox add_TextBox(this ToolStripItem toolStripItem, string text)
		{
			return toolStripItem.toolStrip().add_TextBox(text);			
		}
		
		public static ToolStripTextBox add_TextBox(this ToolStrip toolStrip, string text)
		{			
			return (ToolStripTextBox)toolStrip.invokeOnThread(
				()=>{
						var textBox = new ToolStripTextBox();
						toolStrip.Items.Add(textBox);	
						textBox.Text = text;						
						return textBox;
					 } );			
		}
		
		//public static ToolStripButton add_Link(this ToolStrip toolStrip, string text, string resourceName = null, Action onClick = null)
		
		//Events
		public static ToolStripButton click(this ToolStripButton button)
		{
			return (ToolStripButton)button.toolStrip().invokeOnThread(
				()=>{
						button.PerformClick();
						return button;
					});
		}
		
		public static string get_Text(this ToolStripControlHost toolStripControlHost)
		{
			return (string)toolStripControlHost.toolStrip().invokeOnThread(()=> toolStripControlHost.Text);
		}
		
		
		//Prob:
		// if I use the original: 
		//   	public static T onKeyPress<T>(this T control, Keys onlyFireOnKey, Action<String> callback) where T : Control
		// I get the error:
		//    [12:02:19 PM] ERROR: [CSharp_FastCompiler] Compilation Error: 32::3::CS0311::The type 'System.Windows.Forms.ToolStripTextBox' cannot be used as type parameter 'T' in the generic type or method 'O2.DotNetWrappers.ExtensionMethods.WinForms_ExtensionMethods_Control_Object.onKeyPress<T>(T, System.Windows.Forms.Keys, System.Action<string>)'. There is no implicit reference conversion from 'System.Windows.Forms.ToolStripTextBox' to 'System.Windows.Forms.Control'.::c:\Users\o2\AppData\Local\Temp\gxrzax0r.0.cs
		// but it I use: 
		//  	public static T _onKeyPress<T>(this T toolStripControlHost, Keys onlyFireOnKey, Action<String> callback) where T : ToolStripControlHost
		// I get the error:
		//    [12:04:09 PM] ERROR: [CSharp_FastCompiler] Compilation Error: 32::3::CS0121::The call is ambiguous between the following methods or properties: 'O2.DotNetWrappers.ExtensionMethods.WinForms_ExtensionMethods_Control_Object.onKeyPress<System.Windows.Forms.ToolStripTextBox>(System.Windows.Forms.ToolStripTextBox, System.Windows.Forms.Keys, System.Action<string>)' and 'O2.XRules.Database.Utils._Extra_extensionMethods_ToolStrip.onKeyPress<System.Windows.Forms.ToolStripTextBox>(System.Windows.Forms.ToolStripTextBox, System.Windows.Forms.Keys, System.Action<string>)'::c:\Users\o2\AppData\Local\Temp\pgzwszmn.0.cs
		//
		// so the solution is to hard-code the onEnter and onKeyPress calls to ToolStripTextBox (which is not ideal)
		
		public static ToolStripTextBox onEnter(this ToolStripTextBox toolStripTextBox, Action<String> callback)	
        {
            return toolStripTextBox.onKeyPress(Keys.Enter, callback);
        }
        
        public static ToolStripTextBox onKeyPress(this ToolStripTextBox toolStripTextBox, Keys onlyFireOnKey, Action<String> callback)
        {
        	
            toolStripTextBox.KeyDown += (sender, e) =>
            {
                if (e.KeyData == onlyFireOnKey)
                {
                    callback(toolStripTextBox.get_Text());                    
                    e.SuppressKeyPress = true;      
                }
            };
            return toolStripTextBox;
        }		
		
	}
	
	// ListView
	public static class _Extra_extensionMethods_ListView
	{		
		public static ListView showSelection(this ListView listView,bool value = true)
		{
			return (ListView)listView.invokeOnThread(
				()=>{
						listView.HideSelection = value.isFalse();
						return listView;
					});
		}
		
		public static ListViewItem add_Row(this ListView listView, params string[] items)
		{
			return listView.add_Row(items.toList());
		}
		
		public static ListViewItem add_Row(this ListView listView, List<string> items)
		{
			return (ListViewItem)listView.invokeOnThread(
				()=>{
						if (items.size() < 2)
						{
							listView.Items.Add(items.first() ?? "");
							return listView;
						}													
						var listViewItem = new ListViewItem();
						listViewItem.Text = items.first();
						items.remove(0);
						listViewItem.SubItems.AddRange(items.ToArray());
						listView.Items.Add(listViewItem);
						return listViewItem;						
					});				
		}				
		
		public static ListViewItem tag(this ListViewItem listViewItem, object tag)
		{
			return (ListViewItem)listViewItem.ListView.invokeOnThread(
				()=>{
						listViewItem.Tag = tag;
						return listViewItem;
					});
		}
		
		public static List<ListViewItem> items(this ListView listView)
		{
			return (List<ListViewItem>)listView.invokeOnThread(
				()=>{				
						var items = new List<ListViewItem>();
						foreach(ListViewItem item in listView.Items)
							items.add(item);
						return items;
					});
			
		}
		
		public static List<ListViewItem> selectedItems(this ListView listView)
		{
			return (List<ListViewItem>)listView.invokeOnThread(
				()=>{				
						var items = new List<ListViewItem>();
						foreach(ListViewItem item in listView.SelectedItems)
							items.add(item);
						return items;
					});
			
		}
		
		public static ListViewItem selected(this ListView listView)
		{
			return listView.selectedItem();
		}
		public static ListViewItem selectedItem(this ListView listView)
		{
			return listView.selectedItems().first();
		}
		
		public static ListViewItem select(this ListView listView, int position)
		{
			var items = listView.items();
			if (position < 1 || position > items.size() + 1)
			{
				"[ListViewItem] in select, invalid position value '{0}' (there are {1} items in ListView)".error(position, items.size());
				return null;
			}
			return items[position -1].select();
		}
		
		public static ListViewItem select(this ListViewItem listViewItem)
		{
			return (ListViewItem)listViewItem.ListView.invokeOnThread(
				() =>{
						listViewItem.Selected = true;
						return listViewItem;
					 });
		}
		
		public static object tag(this ListViewItem listViewItem)
		{
			return (object)listViewItem.ListView.invokeOnThread(() => listViewItem.Tag );
		}
		
		public static object tag<T>(this ListViewItem listViewItem)
		{
			try
			{
				if (listViewItem.notNull())
				{
					var tag = listViewItem.tag();
					if (tag.notNull() && tag is T)
						return (T)tag;
				}	
			}
			catch(Exception ex)
			{
				ex.log("[ListViewItem] tag");
			}
			return default(T);
		}
		
		
		public static ListView afterSelected(this ListView listView, Action<ListViewItem> onSelectedCallback)
		{
			return (ListView)listView.invokeOnThread(
				()=>{		
						listView.SelectedIndexChanged+=(sender,e)=>
							{
								try
								{
									if (listView.selected() != null)
										onSelectedCallback(listView.selected());
								}
								catch(Exception ex)
								{
									ex.log("[ListViewItem] afterSelected");
								}
							};
						return listView;
					});	
		}
		
		public static ListView afterSelected<T>(this ListView listView, Action<T> onSelectedCallback)
		{
			return (ListView)listView.invokeOnThread(
				()=>{		
						listView.SelectedIndexChanged+=(sender,e)=>
							{
								try
								{
									if (listView.selected() != null)
									{
										var tag = listView.selected().tag();
										if (tag.notNull() && tag is T)
											onSelectedCallback((T)tag);
									}
								} 
								catch(Exception ex)
								{
									ex.log("[ListViewItem] afterSelected<T>");
								}
							};
						return listView;
					});	
		}
	}	
				
		
	// TreeView
	public static class _Extra_extensionMethods_TreeView_TreeNode
	{		
		public static TreeNode collapse(this TreeNode treeNode)
		{
			return (TreeNode)treeNode.treeView().invokeOnThread(
				()=>{				
						treeNode.Collapse();
						return treeNode;
					});			
		}
		
		public static TreeNode expandAndCollapse(this TreeNode treeNode)
		{
			return treeNode.expand().collapse();
		}
	}	
		
}    	