// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using FluentSharp.WinForms.Utils;

namespace O2.Script
{			
	
    public class ascx_Start_Tools: UserControl
    {        	
    	public static List<ProcessDetail> processDetails;
    	public string xmlFile =  Path.Combine(Environment.CurrentDirectory,"StartTools.Xml");
		public DataGridView dataGridView;
		
		
        public static void startControl()
    	{   			
    	 	processDetails = new List<ProcessDetail>(); 
    		processDetails.createTypeAndAddToList<ProcessDetail>("Reflector",@"C:\_DinisTest\tools\reflector\reflector.exe");
            processDetails.createTypeAndAddToList<ProcessDetail>("Notepad", @"Notepad.exe", "test");
            processDetails.createTypeAndAddToList<ProcessDetail>("Cmd", @"Cmd.exe");
    
    		O2Gui.open<ascx_Start_Tools>("Start Tools v1.1",400,200);    		
    	}    	
    	
    	public ascx_Start_Tools()
    	{    		    	    		    		
    		dataGridView = this.add_DataGridView();
    		dataGridView.AllowUserToAddRows = true;
    		dataGridView.AllowUserToDeleteRows = true;
    		dataGridView.add_Columns(typeof(ProcessDetail));    		
    		dataGridView.add_Column_Link("Start", true);
    		dataGridView.onClick(startProcess);    		
    		dataGridView.onDrop(loadFile);
    		
    		var menu  = dataGridView.add_ContextMenu();
            menu.add_MenuItem("save", (menuItem) => saveCurrentList());
    		loadList();    		
     	}   
		
		public void startProcess(int row, int column)
		{
			if (dataGridView.value(row,column).ToString() == "Start")
			{
				var processName = dataGridView.value(row,0).ToString();
				var processPath = dataGridView.value(row,1).ToString();
				var processArguments = dataGridView.value(row,2).ToString();			
				var process = Processes.startProcess(processPath, processArguments);
			}
		}
		
		public void loadFile(string file)
		{
			if (file.exists())
			{
                "loading file: {0}".info(file);	
				var loadedObject = file.deserialize<List<ProcessDetail>>();
				if (loadedObject != null)
				{
					processDetails = loadedObject;
					loadList();
				}
			}
		}
		
		public void loadList()
		{	
			dataGridView.remove_Rows();
			dataGridView.add_Rows(processDetails);			
		}
				
		public void saveCurrentList()
		{						
			xmlFile = O2Forms.askUserForFileToSave(xmlFile.directoryName(),xmlFile.fileName());
			if (xmlFile.valid())
			{
				syncProcessDetailsList();				
				processDetails.serialize(xmlFile);
			}
		}
		
		public void syncProcessDetailsList()
		{
			processDetails.Clear();
			foreach(var row in dataGridView.rows())
			{				
				if (row[0].validString() && row[1].validString())
				{
					row.toString().info();
                    processDetails.createTypeAndAddToList<ProcessDetail>(row[0], row[1], row[2]);
				}
			}
		}			
    }
    
    [Serializable]
	public class ProcessDetail
	{
		public string Name 		{ get; set; }
		public string Path 		{ get; set; }
		public string Argument 	{ get; set; }
	}
}
