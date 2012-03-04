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

//O2File:_Extra_methods_Reflection.cs
//O2File:_Extra_methods_Collections.cs

namespace O2.XRules.Database.Utils
{			
	public static class DataGridView_ExtensionMethods
	{
	
		public static DataGridView dataSource(this DataGridView dataGridView, System.Data.DataTable dataTable)
		{
			dataGridView.invokeOnThread(
				()=>{
						dataGridView.DataSource = dataTable;
					});
			return dataGridView;
		}
		
		public static DataGridView ignoreDataErrors(this DataGridView dataGridView)
		{
			return dataGridView.ignoreDataErrors(false);
		}
		
		public static DataGridView ignoreDataErrors(this DataGridView dataGridView, bool showErrorInLog)
		{
			dataGridView.invokeOnThread(
				()=>{
						dataGridView.DataError+= 
								(sender,e) => { 
													if (showErrorInLog)
														" dataGridView error: {0}".error(e.Context);
											  };
					});
			return dataGridView;		
		}
		
		
		public static DataGridView onDoubleClick<T>(this DataGridView dataGridView, Action<T> callback)
		{
			dataGridView.onDoubleClick(
				(dataGridViewRow)=>{
										if (dataGridViewRow.Tag.notNull() && dataGridViewRow.Tag is T)
											callback((T)dataGridViewRow.Tag);
								   });
			return dataGridView;
		}						
		
		public static DataGridView onDoubleClick(this DataGridView dataGridView, Action<DataGridViewRow> callback)
		{
			dataGridView.invokeOnThread(
				()=>{
						dataGridView.DoubleClick+= 
							(sender,e)=>{
											if (dataGridView.SelectedRows.size() == 1)
											{
												var selectedRow = dataGridView.SelectedRows[0];
												callback(selectedRow);
											}
										 };
					});
			return dataGridView;		
		}
		
		public static DataGridView afterSelect<T>(this DataGridView dataGridView, Action<T> callback)
		{
			dataGridView.afterSelect(
				(dataGridViewRow)=>{
										if (dataGridViewRow.Tag.notNull() && dataGridViewRow.Tag is T)
											callback((T)dataGridViewRow.Tag);
								   });
			dataGridView.onDoubleClick<T>(callback);					   
			return dataGridView;
		}
		
		public static DataGridView afterSelect(this DataGridView dataGridView, Action<DataGridViewRow> callback)
		{
			dataGridView.invokeOnThread(
				()=>{
						dataGridView.SelectionChanged+= 
							(sender,e)=>{
											if (dataGridView.SelectedRows.size() == 1)
											{
												var selectedRow = dataGridView.SelectedRows[0];
												callback(selectedRow);
											}
										 };
					});
			return dataGridView;		
		}
		
		public static DataGridView row_Height(this DataGridView dataGridView, int value)
		{
			dataGridView.invokeOnThread(()=>dataGridView.RowTemplate.Height = value);
			return dataGridView;
		}
		
		public static List<string> values(this DataGridViewRow dataViewGridRow)
		{
			return ( List<string>)dataViewGridRow.DataGridView.invokeOnThread(
				()=>{
						var values = new List<string>();
						foreach(var cell in dataViewGridRow.Cells)
							values.add(cell.property("Value").str());
						return values;
					});			
		}
		
		
		public static DataGridView show(this DataGridView dataGridView, object _object)
		{
			dataGridView.Tag = _object;	
			dataGridView.remove_Columns();
			if(_object is IEnumerable)
			{
				var list = (_object as IEnumerable); 
				var first = list.first();  
				var names = first.type().properties().names(); 
				dataGridView.add_Columns(names);
				foreach(var item in list)
				{
					var rowId = dataGridView.add_Row(item.propertyValues().ToArray()); 
					dataGridView.get_Row(rowId).Tag = item;										
				}
			}
			else
			{
				var names = _object.type().properties().names(); 
				dataGridView.add_Column("Property name",150); 
				dataGridView.add_Column("Property value");
				foreach(var name in names)
					dataGridView.add_Row(name, _object.prop(name));												
			}		
			return dataGridView;
		 }

		
	}

	
}
    	