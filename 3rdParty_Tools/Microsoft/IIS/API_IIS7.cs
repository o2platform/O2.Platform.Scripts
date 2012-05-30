// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using Microsoft.Web.Administration;
using O2.XRules.Database.Utils;
//O2Ref:C:\Windows\System32\InetSrv\Microsoft.Web.Administration.dll 
//O2File:ascx_FolderViewer.cs
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_IIS7_Test
	{
		public void test()
		{
			new API_IIS7().show_IIS_Viewer();
		}
	}
	
    public class API_IIS7
    {   
    	public ServerManager serverManager { get; set;}
		public Site CurrentSite { get; set; }    	
		public bool ShowErrorIfTryingToCreateAWebsiteThatAlreadyExists {get; set; }
		
    	public API_IIS7()
		{
			ShowErrorIfTryingToCreateAWebsiteThatAlreadyExists = true;
			loadServerManager();
		}
		
		public API_IIS7 loadServerManager()
		{
			this.serverManager = new ServerManager();
			return this;
		}
				
	}
	
	public static class API_IIS7_ExtensionMethods_API_IIS7
	{
		public static bool currentUserHasEnoughPermissions(this API_IIS7 iis7)
		{
			try
			{
				iis7.websites();  
				return true;
			}
			catch(Exception ex)
			{	
				"[API_IIS7] could not get website list: {0}".error(ex.Message);			
			}
			return false;
		}
		
		public static bool checkIfUserHasEnoughPermissions(this API_IIS7 iis7)
		{
			if(iis7.currentUserHasEnoughPermissions().isFalse())
			{
				if ("It looks like your current account doesn't have the rights to access the IIS data, do you want to try running this script with full priviledges?".askUserQuestion())				
					PublicDI.CurrentScript.executeH2_as_Admin();
				return false;
			}														
			return true;
		}
		
		public static List<Site> websites(this API_IIS7 iis7)
		{
			return iis7.serverManager.Sites.toList();
		}
		
		public static Site website(this API_IIS7 iis7, string name)
		{
			foreach(var site in iis7.websites())
				if(site.Name == name)
					return site;
			return null;
		}
		
		public static bool hasWebsite(this API_IIS7 iis7, string name)
		{
			return iis7.website(name).notNull();
		}						
		
		public static API_IIS7 commit(this API_IIS7 iis7)
		{
			return iis7.commitChanges();
		}
		public static API_IIS7 commitChanges(this API_IIS7 iis7)
		{
			iis7.serverManager.CommitChanges();
			iis7.loadServerManager();
			return iis7;
		}
		
		public static Site create_Website(this API_IIS7 iis7, string websiteName, string serverRoot)
		{
			return iis7.add_Website(websiteName,serverRoot); 
		}
		
		public static Site add_Website(this API_IIS7 iis7)
		{
			return iis7.add_Website("_Temp_Website_{0}".format(10.randomLetters()));
		}
		
		public static Site add_Website(this API_IIS7 iis7, string websiteName)
		{
			var serverRoot = websiteName.tempDir();			 
			return iis7.add_Website(websiteName, serverRoot);
		}
		
		public static Site add_Website(this API_IIS7 iis7, string websiteName,string serverRoot)
		{			
			var port = 8080 + 2000.random(); 
			return iis7.add_Website(websiteName, serverRoot, port);
		}
		
		public static Site add_Website(this API_IIS7 iis7, string websiteName, string serverRoot, int port)
		{
			if (iis7.hasWebsite(websiteName).isFalse()) 
			{				
				var site = iis7.serverManager.Sites.Add(websiteName, serverRoot,  port);
				iis7.commitChanges();
				if (site.notNull())				
					"Created new website called {0} on port {1} at folder {2}".info(websiteName, serverRoot,  port);									
				else
				{
					"Could not created website with the provided details".error();
					return null;
				}
			}
			else
				if (iis7.ShowErrorIfTryingToCreateAWebsiteThatAlreadyExists)
					"Could not add site since it already existed a website with the same name: {0}".error(websiteName); 					
			return iis7.website(websiteName);
		}
		
		public static API_IIS7 delete_Website(this API_IIS7 iis7, Site site)
		{
			iis7.serverManager.Sites.Remove(site);
			iis7.commitChanges();
			"Deleted Website with Id {0} and name {1}".info(site.Id, site.str());
			return iis7;
		}
		
		public static Configuration config(this API_IIS7 iis7, string siteName)
		{
			return iis7.config(iis7.website(siteName));
		}
		
		public static Configuration config(this API_IIS7 iis7, Site site)
		{
			if (site.notNull())
				return iis7.serverManager.GetWebConfiguration(site.Name);
			return null;
		}				
	}
	
	public static class API_IIS7_ExtensionMethods_Site
	{		
		public static Site start(this Site site)
		{
			site.Start();
			return site;
		}
		
		public static Site stop(this Site site)
		{
			site.Stop();
			return site;
		}
		
		public static string state(this Site site)
		{
			return site.State.str();
		}
		
		public static string url(this Site site)	//todo: add support for when there are multiple URLs mapped
		{
			var binding = site.Bindings[0];
			var ip = (binding.EndPoint.str().contains("0.0.0.0"))  
						? binding.EndPoint.str().replace("0.0.0.0", "127.0.0.1")
						: binding.EndPoint.str();							
			var url = "{0}://{1}".format(binding.Protocol, ip);
			return url;
		}
		
		public static string path(this Site site)	
		{
			return site.rootFolder();
		}
		
		public static string rootFolder(this Site site)		//todo: see if there are cases where the root is not the first virtual deirectly
		{
			var virtualDirectory = site.Applications[0].VirtualDirectories[0];
			var path = virtualDirectory.PhysicalPath;
			return path;
		}
		
		public static string logFolder(this Site site)
		{
			var baseLogPath  = Environment.ExpandEnvironmentVariables(site.LogFile.Directory);
			return baseLogPath.pathCombine("W3SVC{0}".format(site.Id));
		}								
		
		public static List<Microsoft.Web.Administration.Application> applications(this Site site)
		{
			return site.Applications.toList();
		}
		
		public static List<VirtualDirectory> virtualDirs(this Site site)
		{
			return site.applications().virtualDirs();
		}
		
		public static Microsoft.Web.Administration.Application add_Application(this Site site, string virtualPath, string localPath)
		{
			if (virtualPath.starts("/").isFalse())
				virtualPath = "/"+ virtualPath;
			return site.Applications.Add(virtualPath, localPath);
		}
		
		public static VirtualDirectory add_VirtualDir(this Site site, string virtualDir, string localPath)
		{
			if (virtualDir.starts("/").isFalse())
				virtualDir = "/"+ virtualDir;
			return site.applications()[0].VirtualDirectories.Add(virtualDir, localPath);
		}
	}
	
	public static class API_IIS7_ExtensionMethods_Application
	{
		public static List<VirtualDirectory> virtualDirs(this List<Microsoft.Web.Administration.Application> applications)
		{
			return (from application in applications
					from virtualDir in application.virtualDirs()
					select virtualDir).toList();
		}
		
		public static List<VirtualDirectory> virtualDirs(this Microsoft.Web.Administration.Application application)
		{
			return application.VirtualDirectories.toList();
		}
	}
	
	public static class API_IIS7_ExtensionMethods_Configuration
	{
		public static ConfigurationSection section(this Configuration configuration, string sectionName)
		{
			if (configuration.notNull())
			{
				try
				{
					return configuration.GetSection(sectionName);
				}
				catch
				{
					"could not find section: {0}".error(sectionName);
				}
			}
			return null;					
		}
		
		public static ConfigurationSection directoryBrowse(this Configuration configuration)
		{
			return configuration.section("system.webServer/directoryBrowse"); 
		}
	}
	
	public static class API_IIS7_ExtensionMethods_ConfigurationSection
	{
		public static object value(this API_IIS7 iis7, ConfigurationSection section, string attributeName, object value)
		{
			section.SetAttributeValue(attributeName, value);
			iis7.commit();
			return section;
		}
		
		public static object value(this ConfigurationSection section, string attributeName)
		{
			return section.GetAttributeValue(attributeName);
		}
		
		public static T value<T>(this ConfigurationSection section, string attributeName)
		{
			var value = section.GetAttributeValue(attributeName);
			if (value is T)
				return (T)value;
			return default(T);
		}
	}
	public static class API_IIS7_ExtensionMethods_GUI_Helpers
	{					
		public static Panel show_IIS_Viewer(this API_IIS7 apiIIS)
		{
			if (apiIIS.checkIfUserHasEnoughPermissions())
				return apiIIS.add_IIS_Viewer("Tool- IIS 7.x Viewer".showAsForm());
			return null;
		}
		
		public static T add_IIS_Viewer<T>(this API_IIS7 iis7, T control)
			where T : Control
		{			
			var site_WebBrowser = control.add_GroupBox("Selected Site: Web view").add_WebBrowser_Control();
			
			var sites_TreeView = site_WebBrowser.parent().insert_Left(200,"IIS Websites").add_TreeView();	
			
			var site_PropertyGrid = sites_TreeView.insert_Below(150).add_PropertyGrid().helpVisible(false); 
			var site_Options = sites_TreeView.insert_Below(50,"Options");
			
			var site_folderViewer = site_WebBrowser.parent().insert_Below("Selected Site: File System View").add_FolderViewer(); 
			var site_Logs = site_folderViewer.parent().parent().insert_Right("Seleted Site: Logs").add_FolderViewer(); 			

			site_WebBrowser.add_NavigationBar();
			
			//bug: this is currently being enforeced on all websites
			//site_Options.add_Link("Allow Directory Browsing (globally)", 0 ,0, ()=> iis7.config(iis7.CurrentSite).directoryBrowse().value("enabled"));
			site_Options.add_CheckBox("Directory Browse on all websites", 0,0, 
										(value)=>{
													iis7.value(iis7.config(iis7.websites()[0]).directoryBrowse(),"enabled",value);
												 }).@checked(iis7.config(iis7.websites()[0]).directoryBrowse().value<bool>("enabled"))
						.autoSize();
			
			sites_TreeView.afterSelect<object>(
				(item)=> site_PropertyGrid.show(item)); 
				
			sites_TreeView.afterSelect<Site>(
				(site)=>{
							iis7.CurrentSite = site;							
							site_WebBrowser.open(site.url()); 
							site_folderViewer.open(site.rootFolder());
							site_Logs.open(site.logFolder());
						});
			
			site_folderViewer.FolderView.afterSelect<string>(
				(fileOrFolder)=>{
									
									var url = "{0}{1}".format(iis7.CurrentSite.url(), site_folderViewer.virtualPath(fileOrFolder));									
									"Showing Url:{0}".info(url);
									site_WebBrowser.open(url);	
									//var virtualPath = fileOrFolder.remove(
								});
								
			iis7.show_IIS_Websites(sites_TreeView);
			
			sites_TreeView.afterSelect<VirtualDirectory>(
				(virtualDirectory)=>{
										var url = "{0}{1}".format(iis7.CurrentSite.url(), virtualDirectory.Path);
										"Showing Virtual Directory that is mapped to path:{0}".info(url);
										site_WebBrowser.open(url);	
									});
									
			sites_TreeView.afterSelect<Microsoft.Web.Administration.Application>(
				(application)=>{
										var url = "{0}{1}".format(iis7.CurrentSite.url(), application.Path);
										"Showing Application that is mapped to path:{0}".info(url);
										site_WebBrowser.open(url);	
									});									
			
			sites_TreeView.add_ContextMenu().add_MenuItem("Refresh", ()=>iis7.show_IIS_Websites(sites_TreeView));
			
			return control;
		}
		
		//public static TreeView show_IIS_Websites_In_TreeView<T>(this API_IIS7 apiIIS, T control)
		public static TreeView show_IIS_Websites_In_TreeView(this API_IIS7 iis7)
		{
			return iis7.show_IIS_Websites_In_TreeView("TreeView with IIS 7.x Websites".showAsForm());
		}
		
		public static TreeView show_IIS_Websites_In_TreeView<T>(this API_IIS7 iis7, T control)
			where T : Control
		{			
			var treeView = control.clear().add_TreeView();
			iis7.show_IIS_Websites(treeView);
			return treeView;
		}
		
		public static API_IIS7 show_IIS_Websites(this API_IIS7 iis7, TreeView treeView)
		{	
			iis7.loadServerManager();
			treeView.clear();
			iis7.setup_TreeView_IISView(treeView); 
			try
			{
				treeView.add_Node("_Application Pools").add_Nodes(iis7.serverManager.ApplicationPools,true);
				treeView.add_Node("_Worker Processes(w3wp.exe)").add_Nodes(iis7.serverManager.WorkerProcesses,true);
				foreach(var site in iis7.serverManager.Sites.toList())
				{
					var sideNode = treeView.add_Node(site.str(),site, true);
					sideNode.color((site.state() =="Started") ? Color.DarkGreen : Color.Red );
				}
					
				treeView.selectFirst();
			}
			catch(UnauthorizedAccessException uaex)
			{
				treeView.add_Node(uaex.Message);
				treeView.backColor(Color.LightPink);
			}
			catch(Exception ex)
			{
				"Error: {0} : {1}".error(ex.Message, ex.typeName());
			}
			
			return iis7;
			//mapSites(serverManager.Sites.toList(), sites_TreeView.rootNode());
		}
		
		public static TreeView setup_TreeView_IISView(this API_IIS7 iis7, TreeView treeView)
		{
			Action<ConfigurationElement, TreeNode> map_Attributes =
				(configurationElement, treeNode)=>{
											treeNode.add_Nodes(configurationElement.Attributes, (attribute) => "{0}: {1}".format(attribute.Name, attribute.Value));	      
										};
						
			treeView.beforeExpand<Site>(
				(treeNode,site ) => {
										map_Attributes(site, treeNode); 
										treeNode.add_Node("Applications").add_Nodes(site.Applications,true);
										treeNode.add_Node("Bindings").add_Nodes(site.Bindings);
										map_Attributes(site.LogFile, treeNode.add_Node("LogFile"));
										
										treeNode.color((site.state() =="Started") 
															? Color.DarkGreen
															: Color.Red );
									});
			
			treeView.beforeExpand<Microsoft.Web.Administration.Application>(
				(treeNode,application )=>{		
											map_Attributes(application, treeNode);  
											treeNode.add_Nodes(application.VirtualDirectories,true ); 
									});						
									
			treeView.beforeExpand<VirtualDirectory>(
				(treeNode,virtualDirectory )=>{		
												map_Attributes(virtualDirectory, treeNode);  
											  });		
											  
			treeView.beforeExpand<ApplicationPool>(
				(treeNode,applicationPool )=>{		
												map_Attributes(applicationPool, treeNode);  
											  });		
			treeView.beforeExpand<WorkerProcess>(
				(treeNode,workerProcess )=>{		
												map_Attributes(workerProcess, treeNode);  
											  });		
											  
			return treeView;
		}
	}
}

	