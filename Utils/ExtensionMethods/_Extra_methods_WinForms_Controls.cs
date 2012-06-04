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
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:Scripts_ExtensionMethods.cs

namespace O2.XRules.Database.Utils
{	
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
	}
	
	// ListView
	public static class _Extra_extensionMethods_ListView
	{		
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
}    	