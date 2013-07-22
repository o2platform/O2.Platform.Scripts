// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;

//O2Ref:System.ServiceProcess.dll
//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll

namespace O2.Script
{
    public class ascx_Services_Stop: UserControl
    {        	
		public DataGridView dataGridView;
        public static void startControl()
    	{   
    		WinForms_Show.showAscxInForm(
				typeof(ascx_Services_Stop), 
				"Stop Running Services", 
				400, 
				400);		    		
    	}    	
    	
    	public ascx_Services_Stop()
    	{    		    	
    		buildGui();	            
        }
    
        private void buildGui()
        {        	 
        	 loadServicesData();          	 
     	}   

		public void loadServicesData()
		{
            "Loading Services Data".info();
			this.invokeOnThread(
				()=> {
						 this.clear();
						 dataGridView = this.add_DataGridView();  
						 dataGridView.CellContentClick+= (sender,e) => 	cellClicked(e.RowIndex,e.ColumnIndex);
						 dataGridView.Columns.Clear();						 			        	 
						 
						 
						 var menu  = dataGridView.add_ContextMenu();
    		 			 menu.add_MenuItem("refresh", (menuItem) => loadServicesData());
    		 
			        	 var services = ServiceController.GetServices();
			        	 			        	
			        	 dataGridView.add_Column("Display Name");
			        	 dataGridView.add_Column("Service Name");
			        	 dataGridView.add_Column("Service Type");			        	 
			        	 dataGridView.add_Column_Link("stop service");        	
			        	 foreach(var service in services)      	 
			        	 {        	 
			        	 	if (service.CanStop)			        	 	
			        	 	{			        	 		
			        	 		var id = dataGridView.add_Row(
			        	 					 service.DisplayName,
			        	 					 service.ServiceName,			        	 					 
			        	 					 service.ServiceType,
			        	 					 "stop");								
			        	 		dataGridView.Rows[id].Tag = service;
			        	 	}	    	 		
			        	 }        	 
			        });
		}
		
     	public void cellClicked(int rowId,int columnId)
     	{
     		var cell = dataGridView.value(rowId,columnId).ToString();
     		   		
     		if (cell.eq("stop"))
     		{
     			dataGridView.set_Value(rowId,columnId,"stopping");
     			O2Thread.mtaThread(
	     			()=>{	     		
							var row = dataGridView.get_Row(rowId); 					
				     		var service = (ServiceController)row.Tag;
				     		"Stopping service: {0}".info(service.ServiceName);				     		
				     		service.Stop();
				     		service.WaitForStatus(ServiceControllerStatus.Stopped);
				     		service.Refresh();
				     		"Service status: {0}".info(service.Status);
				     		if (service.Status == ServiceControllerStatus.Stopped)
				     			dataGridView.remove_Row(row);				     		
				     	});
			};
     	}
    	    	    	    	    
   }

    public class ServiceController
    {
    	private static Assembly serviceProcessAssembly;
    	private static object serviceControllerStatus_Stopped;
    	
        public object obj;
    	                  	
        public ServiceController(object _obj)
    	{
    		obj = _obj;
    	}

        public static List<ServiceController> GetServices()
        {
        	// resolve required assembly, enum and type
        	serviceProcessAssembly = "System.ServiceProcess".assembly();           
            serviceControllerStatus_Stopped = serviceProcessAssembly.type("ServiceControllerStatus").fieldValue("Stopped");            
                                       
            var serviceControllerType = serviceProcessAssembly.type("ServiceController");
            
            var wrappedServices = new List<ServiceController>();                        
                                                
            var services = serviceControllerType.invokeStatic("GetServices");
            foreach (var service in (ICollection)services)
                wrappedServices.Add(new ServiceController(service));
            return wrappedServices;
        }           
    
        public bool CanStop
        {
            get { return (bool)obj.invoke("get_CanStop"); } 
        }

        public string DisplayName
        {
            get { return obj.invoke("get_DisplayName").ToString(); }
        }

        public string ServiceName
        {
            get { return obj.invoke("get_ServiceName").ToString(); }
        }

        public string ServiceType
        {
            get { return obj.invoke("get_ServiceType").ToString(); }
        }

 		public ServiceControllerStatus Status
        {
        	get {
        			var status = obj.invoke("get_Status");        			
					if (status.ToString() == "Stopped")
						return ServiceControllerStatus.Stopped;
                    return ServiceControllerStatus.Running;
				}
        }


         public void Stop()
        {
            obj.invoke("Stop");
        }

		// hardcoded for now to ServiceControllerStatus.Stopped
        public void WaitForStatus(ServiceControllerStatus status)
        {
        	if (status == ServiceControllerStatus.Stopped)  
        	{        		
            	obj.invoke("WaitForStatus", serviceControllerStatus_Stopped);
            }
        }

        public void Refresh()
        {
            obj.invoke("Refresh");	
        }
    }

    public enum ServiceControllerStatus
    {
        ContinuePending = 5,
        Paused = 7,
        PausePending = 6,
        Running = 4,
        StartPending = 2,
        Stopped = 1,
        StopPending = 3

    }
}

// extra code snippet

 // use this to see all available properties
	 /*foreach(var property in services[0].type().properties())
	 		dataGridView.column(property.Name,150);
	 foreach(var service in services)      	 
	 {  
	 	var rowCells = new List<object>();
 		foreach(var property in service.type().properties())
 			rowCells.Add(service.prop(property.Name));
 		dataGridView.row(rowCells.ToArray());        	 		
	 }*/
			        	 
			        	 
