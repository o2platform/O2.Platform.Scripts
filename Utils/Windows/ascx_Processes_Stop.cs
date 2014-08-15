// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Diagnostics;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;

//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll

namespace O2.Script
{
    public class ascx_Processes_Stop: UserControl
    {    
    	private static IO2Log log = PublicDI.log;
		public DataGridView dataGridView;
        public static void startControl()
    	{   
            "Stop Running Processes".popupWindow<ascx_Processes_Stop>(400,400);    		
    	}    	
    	
    	public ascx_Processes_Stop()
    	{    		    	
    		buildGui();	            
        }
    
        private void buildGui()
        {        	 
        	 loadProcessData();        	         	 
     	}   

		public void loadProcessData()
		{
            "Loading Processes Data".info();
			this.invokeOnThread(
				()=> {
						 this.clear();
						 dataGridView = this.add_DataGridView();  
						 dataGridView.CellContentClick+= (sender,e) => 	cellClicked(e.RowIndex,e.ColumnIndex);
						 dataGridView.Columns.Clear();

                         var menu = dataGridView.add_ContextMenu();
                         menu.add_MenuItem("refresh", (menuItem) => loadProcessData());
                         //menu.add("close", (menuItem) => ParentForm.Close());

			        	 //dataGridView.SelectionChanged+=(sender,e)=> dataGridView.ClearSelection();
			        	 // use this to see all available properties
			        	 //foreach(var property in Processes.getCurrentProcess().type().properties())
			        	 //		dataGridView.column(property.Name,150);
			        	 dataGridView.add_Column("Name");
			        	 dataGridView.add_Column("Window Title");
			        	 dataGridView.add_Column("Id");
			        	 dataGridView.add_Column("Handle");
			        	 dataGridView.add_Column("Paged Memory Size (Mb)");
			        	 dataGridView.add_Column("Working Set (Mb)");
			        	 dataGridView.add_Column("Priority Class");
			        	 dataGridView.add_Column_Link("stop process");        	 
			        	 var megaByte = 1024*1024;
			        	 foreach(var process in Processes.getProcesses())      	 
			        	 {        	 
			        	 	var id = dataGridView.add_Row(   process.ProcessName,
							        	 					 process.MainWindowTitle,
							        	 					 process.Id,
							        	 					 process.prop("Handle"),
							        	 					 (double)process.PagedMemorySize64 / megaByte,
							        	 					 (double)process.WorkingSet64 / megaByte,
							        	 					 process.prop("PriorityClass"),
							        	 					 "stop");
			        	 	dataGridView.Rows[id].Tag = process;
			        	 
			    	 		// use this to see all available properties
			    	 		/*var rowCells = new List<object>();
			    	 		foreach(var property in process.type().properties())
			    	 			rowCells.Add(process.prop(property.Name));
			    	 		dataGridView.row(rowCells.ToArray());*/        	 		
			        	 }        	 
			        });
		}
		
     	public void cellClicked(int rowId,int columnId)
     	{
     		var cell = dataGridView.value(rowId,columnId).ToString();
     		//this.info(cell);
     		if (cell.equal("stop"))
     		{
     			dataGridView.set_Value(rowId,columnId,"stopping");
     			O2Thread.mtaThread(
	     			()=>{	   
	     					var row = dataGridView.get_Row(rowId); 					
				     		var process = (Process)dataGridView.Rows[rowId].Tag;
				     		"Stopping process: {0}".info(process.ProcessName);
				     		process.ProcessName.info();
				     		process.Kill();
				     		process.WaitForExit();
				     		dataGridView.remove_Row(row);				     						     		
				     	});
			}
     	}
     	         	    	    	    	   
    }
}
