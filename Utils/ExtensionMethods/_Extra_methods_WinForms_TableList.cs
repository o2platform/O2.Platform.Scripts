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
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;

using O2.Views.ASCX.classes.MainGUI;

namespace O2.XRules.Database.Utils
{	
		public static class _Extra_TableList_ExtensionMethods
	{
		public static ascx_TableList setWidthToContent(this ascx_TableList tableList)
		{
			tableList.makeColumnWidthMatchCellWidth();
			tableList.Resize+=(e,s)=> {	 tableList.makeColumnWidthMatchCellWidth();};
			tableList.getListViewControl().ColumnClick+=(e,s)=> { tableList.makeColumnWidthMatchCellWidth();};
			return tableList;
		}
		
		public static ascx_TableList show_In_ListView<T>(this IEnumerable<T> data)
		{
			return data.show_In_ListView("View data in List Viewer", 600,400);
		}
		
		public static ascx_TableList show_In_ListView<T>(this IEnumerable<T> data, string title, int width, int height)
		{
			return O2Gui.open<Panel>(title, width, height).add_TableList().show(data);
		}
		
		public static ascx_TableList columnsWidthToMatchControlSize(this ascx_TableList tableList)
		{		
			tableList.parent().widthAdd(1);		// this trick forces it (need to find how to invoke it directly
			return tableList;
		}
		
		public static ascx_TableList onDoubleClick_get_Row(this ascx_TableList tableList,  Action<ListViewItem> callback)
		{
			tableList.invokeOnThread(
				()=>{
						tableList.listView().DoubleClick+= (sender,e)
							=>{
									var selectedRow =tableList.selected();
									if (selectedRow.notNull())
										callback(selectedRow);
							  };	
					});
			return tableList;					
		}
		
		public static ascx_TableList onDoubleClick<T>(this ascx_TableList tableList,  Action<T> callback)
		{
			tableList.invokeOnThread(
				()=>{
						tableList.listView().DoubleClick+= (sender,e)
							=>{
									var selectedRows = tableList.listView().SelectedItems;
									if (selectedRows.size()==1)
									{
									 	var selectedRow = selectedRows[0]; 
									 	if (selectedRow.Tag.notNull() && selectedRow.Tag is T)
									 		callback((T)selectedRow.Tag);
									}
								};
					});
			return tableList;
		}
		
		public static ascx_TableList onDoubleClick_ShowTagObject<T>(this ascx_TableList tableList)
		{
			return tableList.onDoubleClick<T>((t)=> show.info(t));
		}
		
		
		public static ascx_TableList afterSelect<T>(this ascx_TableList tableList,  Action<T> callback)
		{
			tableList.afterSelect(
				(selectedRows)=>{			
									if (selectedRows.size()==1)
									{
									 	var selectedRow = selectedRows[0]; 
									 	if (selectedRow.Tag.notNull() && selectedRow.Tag is T)
									 		callback((T)selectedRow.Tag);
									}
								});
			return tableList;
		}
		
		public static ascx_TableList afterSelects<T>(this ascx_TableList tableList,  Action<List<T>> callback)			
		{
			tableList.afterSelect(
				(selectedRows)=>{	
									var tags = new List<T>();
									foreach(var selectedRow in selectedRows)
									{									 	
									 	if (selectedRow.Tag.notNull() && selectedRow.Tag is T)
									 		tags.add((T)selectedRow.Tag);
									}
									if (tags.size() > 0)
										callback(tags);
								});
			return tableList;
		}		
		
		public static ascx_TableList afterSelect_get_Cell(this ascx_TableList tableList, int rowNumber, Action<string> callback)
		{
			tableList.afterSelect(
				(selectedRows)=>{			
						if (selectedRows.size()==1)
						{
						 	var selectedRow = selectedRows[0]; 
						 	var values = selectedRow.values();
						 	if (values.size() > rowNumber)
						 		callback(values[rowNumber]);
						}
					});
			return tableList;
		}
						
		public static ascx_TableList afterSelect_set_Cell(this ascx_TableList tableList, int rowNumber, TextBox textBox)
		{
			return tableList.afterSelect_get_Cell(rowNumber,(value)=> textBox.set_Text(value));			
		}
		
		public static ascx_TableList afterSelect_get_Row(this ascx_TableList tableList, Action<ListViewItem> callback)
		{
			tableList.afterSelect(
				(selectedRows)=>{			
						if (selectedRows.size()==1)
						 	callback(selectedRows[0]);						
					});
			return tableList;
		}
		
		public static ascx_TableList afterSelect_get_RowIndex(this ascx_TableList tableList, Action<int> callback)
		{
			tableList.afterSelect(
				(selectedRows)=>{			
						if (selectedRows.size()==1)
						 	callback(selectedRows[0].Index);						
					});
			return tableList;
		}
		
		public static ascx_TableList afterSelect<T>(this ascx_TableList tableList, List<T> items, Action<T> callback)
		{
			tableList.afterSelect(
				(selectedRows)=>{			
						if (selectedRows.size()==1)
						{
							var index = selectedRows[0].Index;							
							if (index < items.size())
						 		callback(items[index]);						
						}
					});
			return tableList;
		}
		
		public static ascx_TableList selectFirst(this ascx_TableList tableList)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						var listView = tableList.getListViewControl();
						listView.SelectedIndices.Clear();
						listView.SelectedIndices.Add(0);
						return tableList;
					});
		}
		
		public static ListViewItem selected(this ascx_TableList tableList)
		{
			return (ListViewItem)tableList.invokeOnThread(
				()=>{
						if (tableList.listView().SelectedItems.Count >0)
							return tableList.listView().SelectedItems[0];
						return null;
					});
		}
		
		public static object tag(this ascx_TableList tableList)
		{
			return (object)tableList.invokeOnThread(
				()=>{
						var selectedItem = tableList.selected();
						if (selectedItem.notNull())
							return selectedItem.Tag;							
						return null;
					});
		}
		public static T tag<T>(this ascx_TableList tableList)
		{
			return tableList.selected<T>();
		}
		
		public static T selected<T>(this ascx_TableList tableList)
		{
			var tag = tableList.tag();
			if (tag.notNull() && tag is T)
				return (T)tag;
			return default(T);
		}
		
		
		public static ascx_TableList clearRows(this ascx_TableList tableList)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						tableList.getListViewControl().Items.Clear();
						return tableList;
					});
		}				
		
		public static ascx_TableList set_ColumnAutoSizeForLastColumn(this ascx_TableList tableList)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						var listView = tableList.getListViewControl();
						if (listView.Columns.size()>0)
							listView.Columns[listView.Columns.size() -1].Width = -2;
						return tableList;
					});
		}
		public static ascx_TableList column_Width(this ascx_TableList tableList, int columnNumber, int width)
		{
			return tableList.set_ColumnWidth(columnNumber, width);
		}
		
		public static ascx_TableList set_ColumnWidth(this ascx_TableList tableList, int columnNumber, int width)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						var listView = tableList.getListViewControl();
						//if (listView.Columns.size()>columnNumber)
							tableList.getListViewControl().Columns[columnNumber].Width = width;
						return tableList;
					});
		}
		
		public static ascx_TableList columns_Width(this ascx_TableList tableList, params int[] widths)
		{
			return tableList.set_ColumnsWidth(widths)
							.set_ColumnsWidth(widths);    // BUG: there some extra event that gets fired on the fist column which reverts it to the original size
													  	// the current solution is to invoke set_ColumnWidth twice 	
		}
		public static ascx_TableList set_ColumnsWidth(this ascx_TableList tableList, params int[] widths)
		{
			return tableList.set_ColumnsWidth(true, widths);
		}
		
		public static ascx_TableList set_ColumnsWidth(this ascx_TableList tableList,bool lastColumnShouldAutoSize,  params int[] widths)
		{
			return (ascx_TableList)tableList.invokeOnThread(
				()=>{
						var listView = tableList.getListViewControl();
						for(var i = 0 ; i < widths.Length ; i++)
							listView.Columns[i].Width = widths[i];
						if (lastColumnShouldAutoSize)	
							tableList.set_ColumnAutoSizeForLastColumn();
						return tableList.set_ColumnAutoSizeForLastColumn();
					});
		}
		
		//very similar to the code in add_Row (prob with that one is that it doens't return the new ListViewItem object
		public static ascx_TableList add_Row<T>(this ascx_TableList tableList, T _objectToAdd, Func<List<string>> getRowData)
		{		
			 return (ascx_TableList)tableList.invokeOnThread(
                () =>{
                		var rowData = getRowData();
						if (rowData.Count > 0)
                    	{	                		
	                		var listView = tableList.getListViewControl();
	                        var listViewItem = new ListViewItem();
	                        listViewItem.Text = rowData[0]; // hack because SubItems starts adding on the 2nd Column :(
	                        rowData.RemoveAt(0);
	                        listViewItem.SubItems.AddRange(rowData.ToArray());	                        
	                        listView.Items.Add(listViewItem);                        
	                        listViewItem.Tag = _objectToAdd; // only difference with main add_Row
	                    }
                    	return tableList;
					});
		}
	}
}