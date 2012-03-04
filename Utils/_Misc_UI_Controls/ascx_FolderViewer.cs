// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;

using O2.XRules.Database.Utils;

//O2File:_Extra_methods_WinForms_TreeView.cs

namespace O2.XRules.Database.Utils
{
    public class test_ascx_FolderView : ContainerControl
    {       		
		public static void launchGui()
		{
			var folderView = O2Gui.open<ascx_FolderView>("Util - FolderView", 500,400);						
			folderView.loadFolder(PublicDI.config.LocalScriptsFolder);
		}
	}

    public class ascx_FolderView : Control
    {   
    	public string RootFolder { get; set; }
    	
    	public TreeView FolderView { get; set; }
    	
    	
    	public ascx_SourceCodeViewer CodeViewer { get; set; }
    	
		public ascx_FolderView()
    	{
    		this.Width = 300;
    		this.Height = 300;    		
    		buildGui();
    	}
 
 	
 
    	public void buildGui()
    	{
			CodeViewer = this.add_SourceCodeViewer();
			FolderView = CodeViewer.insert_Left().add_TreeView();
			
			FolderView.afterSelect<string>(
				(fileOrFolder)=>{
									if (fileOrFolder.fileExists())
										CodeViewer.open(fileOrFolder);
								});										
			
			FolderView.beforeExpand<string>((treeNode, path) => loadFolder(treeNode,path));
			
			FolderView.onDrop((fileOrfolder) => {
													FolderView.clear();
													if (fileOrfolder.dirExists())
														loadFolder(FolderView.rootNode(),fileOrfolder); 
												});						
			FolderView.add_ContextMenu()
						.add_MenuItem("Refresh",true, ()=> refresh())
						.add_MenuItem("Open in Windows Explorer", 
								()=> FolderView.selected().get_Tag().str().startProcess() );
						
			CodeViewer.set_Text("....select file on the left to view its contents here...");												
		}		
		
		public ascx_FolderView refresh()
		{
			this.loadFolder(RootFolder);
			return this;
		}
		
		public ascx_FolderView loadFolder(TreeNode treeNode, string path)
		{
			"Loading Folder: {0}".info(path);
			if (path.valid())
			{  
				path = Environment.ExpandEnvironmentVariables(path);  // in case there are Environment variables like %SystemDrive%
				//"There are {0} files {0}".info(path.files().size());
				//"There are {0} folders {0}".info(path.folder.folders().size());
				var folders = path.folders().sort();
				foreach(var folder in folders)
					treeNode.add_Node(folder.fileName(), folder, folder.files().size() >0 || folder.folders().size()>0)
							.color(Color.DarkOrange);
				var files = path.files();
				treeNode.add_Nodes(files, (file)=> file.fileName());
			}
			return this;
		}
		
		public ascx_FolderView loadFolder(string path)
		{			
			if (path.notNull())
				RootFolder = Environment.ExpandEnvironmentVariables(path);
			FolderView.clear();
			return loadFolder(FolderView.rootNode(),RootFolder);
		}
		
	}
	
	public static class ascx_FolderView_ExtensionMethods
	{
		public static ascx_FolderView open(this ascx_FolderView folderView, string path)
		{
			if (folderView.notNull())
				return folderView.loadFolder(path);			
			return folderView;
		}
		
		public static ascx_FolderView add_FolderViewer<T>(this T control)
			where T: Control
		{
			return control.add_FolderViewer(null);
		}
		
		public static ascx_FolderView add_FolderViewer<T>(this T control, string path)
			where T: Control
		{
			control.clear();
			return control.add_Control<ascx_FolderView>().open(path);
		}
		
		public static string virtualPath(this ascx_FolderView folderView, string path)
		{
			var virtualPath = path.remove(folderView.RootFolder);
			if (virtualPath.valid())
			{
				if (virtualPath[0]=='\\' || virtualPath[0]=='/')
					return virtualPath;
				else
					return "\\{0}".format(virtualPath);
			}
			return virtualPath;
		}
	}
 
}
