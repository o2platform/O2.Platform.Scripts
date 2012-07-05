// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using System.Management;
using O2.XRules.Database.Utils;

//O2Ref:System.Management.dll
//using O2.XRules.Database.Utils

namespace O2.XRules.Database.APIs
{
    public class API_WMI
    {   
    	public static List<string> SampleQueries = new List<string>  {  
    																	"SELECT Name, CommandLine,Caption, Description, ExecutablePath FROM Win32_Process",
																		"SELECT * FROM Win32_Desktop", 
																		"SELECT * FROM Win32_Service",
																		"SELECT * FROM Win32_Process",
																	 };	
    }	
    
    public static class API_WMI_ExtensionMethods_ManagementObjectSearcher
    {
    	public static List<ManagementBaseObject> wmi(this string searchQuery)
    	{
    		return searchQuery.wmiSearch();    		
    	}
    	
    	public static List<ManagementBaseObject> wmiSearch(this string searchQuery)
    	{	    	
    		try
    		{
    			var searcher = new ManagementObjectSearcher(searchQuery);						
    			return searcher.Get().toList();
				
			}
			catch(Exception ex)
			{
				"[wmiSearch]: {0}".error(ex.Message);
			}
			return new List<ManagementBaseObject>();		
    	}
    	
    	public static List<ManagementBaseObject> toList(this ManagementObjectCollection managementObjectCollection)
    	{
    		var results = new List<ManagementBaseObject>();
    		foreach(var result in managementObjectCollection)
				results.Add(result);				
			return results;
    	}
    	
    	public static Type createTypeFromSearchResults(this List<ManagementBaseObject> searchResults)
    	{
    		var dynamicType = "WmiQuery".dynamicType();		 
    		foreach(var searchResult in searchResults)					
				foreach(var property in searchResult.Properties)  										
					dynamicType.add_Property<string>(property.Name);								 
			return dynamicType.create();						
    	}
    	
    	public static List<object> createObjectWithSearchResults(this List<ManagementBaseObject> searchResults)    		
    	{
    		var o2Timer = new O2Timer("[createTypeFromSearchResults] Wmi Object mapping").start(); 
    		var wmiQueryType = searchResults.createTypeFromSearchResults();
    		var wmiQueryItems = wmiQueryType.ctor().wrapOnList();
			wmiQueryItems.Clear(); 
			foreach(var searchResult in searchResults)					
			{
				var wmiQueryItem = wmiQueryType.ctor();
				foreach(var property in searchResult.Properties)  
				{
					wmiQueryItem.property(property.Name, property.Value.str().remove("[null value]") );								
				}
				wmiQueryItems.add(wmiQueryItem);									
			}  
			o2Timer.stop();			
			return wmiQueryItems;
    	}
    	
    	public static ascx_TableList show(this List<ManagementBaseObject> searchResults)
    	{
    		var panel = O2Gui.open<Panel>("WMI Query Results Viewer",700,400);
    		return searchResults.show_in_TableList(panel);
    	}
    	
    	public static ascx_TableList show<T>(this List<ManagementBaseObject> searchResults, T control)
    		where T : Control
    	{
    		return searchResults.show_in_TableList(control);
    	}
    	
    	public static ascx_TableList show_in_TableList<T>(this List<ManagementBaseObject> searchResults, T control)
    		where T : Control
    	{    		
    		var tableList =  control.clear().add_TableList();
    		searchResults.show_in_TableList(tableList);    		
    		return tableList;
    	}
    	
    	public static ascx_TableList show_in_TableList(this List<ManagementBaseObject> searchResults, ascx_TableList tableList)
    	{
    		if (searchResults.isNull() || tableList.isNull())
    			return tableList;
    		var wmiQueryItems = searchResults.createObjectWithSearchResults();
    		if (wmiQueryItems.isNull() || wmiQueryItems.size() ==0)
    		{
    			tableList.title("No items in WMI search results");
    			return tableList;
    		}
    		tableList.visible(false); 
    		
    		var properties = wmiQueryItems[0].type().properties();
    		
    		foreach(var property in  properties)
				tableList.add_Column(property.Name);
				
			foreach(var wmiQueryItem in  wmiQueryItems)
				tableList.add_Row(wmiQueryItem.getProperties_AsArray().Select<object,string>(o=>o.str()).toList());
			
			tableList.title("Showing {0} values form {1} search results".format(properties.size(), searchResults.size()));
			tableList.visible(true);
			return tableList;	
    	}
    	
    	
    	public static DataGridView show_in_DataGridView<T>(this List<ManagementBaseObject> searchResults, T control)
    		where T : Control
    	{    		
    		var dataGridView =  control.clear().add_DataGridView();
    		searchResults.show_in_DataGridView(dataGridView);    		
    		return dataGridView;
    	}
    	
    	public static DataGridView show_in_DataGridView(this List<ManagementBaseObject> searchResults, DataGridView dataGridView)
    	{
    		if (searchResults.isNull() || dataGridView.isNull())
    			return dataGridView;
    		var wmiQueryItems = searchResults.createObjectWithSearchResults();
    		if (wmiQueryItems.isNull() || wmiQueryItems.size() ==0)    				
    			return dataGridView;
    		
    		dataGridView.visible(false); 
    		
    		var properties = wmiQueryItems[0].type().properties();
    		
    		foreach(var property in  properties)
				dataGridView.add_Column(property.Name);
				
			foreach(var wmiQueryItem in  wmiQueryItems)							
				dataGridView.add_Row(wmiQueryItem.getProperties_AsStringList().ToArray());
						
			dataGridView.visible(true);
			return dataGridView;	
    	}
    	
    	public static ascx_ShowInfo show_in_PropertyGrid<T>(this List<ManagementBaseObject> searchResults, T control)
    		where T : Control
    	{    		
    		var showInfo =  control.clear().add_Control<ascx_ShowInfo>();
    		searchResults.show_in_PropertyGrid(showInfo);    		
    		return showInfo;
    	}
    	
    	public static ascx_ShowInfo show_in_PropertyGrid(this List<ManagementBaseObject> searchResults, ascx_ShowInfo showInfo)
    	{    		
    		showInfo.show(searchResults.createObjectWithSearchResults()); 
    		return showInfo;
    	}
    	
    	public static TreeView show_in_TreeView<T>(this List<ManagementBaseObject> searchResults, T control)
    		where T : Control
    	{    		
    		var treeView =  control.clear().add_TreeView().sort();
    		searchResults.show_in_TreeView(treeView);    		
    		return treeView;
    	}
    	
    	public static TreeView show_in_TreeView(this List<ManagementBaseObject> searchResults, TreeView treeView)
    	{    		
    		var data =  searchResults.createObjectWithSearchResults();    	
			    				    				   			
			if (data.size() > 0)
			{
				var properties = data[0].type().properties();				
				treeView.visible(false);
				for(int i =0 ; i <  data.size() ; i++)
				{
					var item = data[i];
					var node = treeView.add_Node("{0} #{1}".format(item, i));
					foreach(var property in properties)
					{
						var propertyValue = item.property(property.Name);
						node.add_Node("{0}: {1}".format(property.Name, propertyValue),propertyValue);
					}
				} 
				treeView.visible(true);
			}
			return treeView;
		}
    }
}