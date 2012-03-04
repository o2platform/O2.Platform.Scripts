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
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:_Extra_methods_WinForms_Controls.cs

namespace O2.XRules.Database.Utils
{	
	//TreeNode & TreeView
	public static class _Extra_TreeNode_and_TreeView_ExtensionMethods
	{
		public static TreeView add_TreeView_with_PropertyGrid<T>(this T control)
			where T : Control
		{
			return control.add_TreeView_with_PropertyGrid(true);
		}
		
		public static TreeView add_TreeView_with_PropertyGrid<T>(this T control, bool insertBelow)
			where T : Control
		{			
			var treeView = control.clear().add_TreeView();				
			var targetPanel = (insertBelow) ? treeView.insert_Below() : treeView.insert_Right();
			var propertyGrid = targetPanel.add_PropertyGrid().helpVisible(false);	 	
			treeView.showSelected(propertyGrid);;
			return treeView;
		}
		
		// this is one of O2's weirdest bugs in the .NET Framework, but there are cases where 
		// a treeview only has 1 node and it is not shown
		public static TreeView applyPathFor_1NodeMissingNodeBug(this TreeView treeView)
		{
			if (treeView.nodes().size()==1)
			{
				var firstNode = treeView.nodes()[0];
				firstNode.set_Text(firstNode.get_Text() + "");	
			}
			return treeView;
		}
		
		public static TreeView collapse(this TreeView treeView)
		{
			if (treeView.notNull())
				treeView.invokeOnThread(()=> treeView.CollapseAll());
			return treeView;
		}
		
		public static TreeView showSelected(this TreeView treeView, PropertyGrid propertyGrid)			
		{
			return treeView.showSelected<object>(propertyGrid);
		}
		
		public static TreeView showSelected<T>(this TreeView treeView, PropertyGrid propertyGrid)			
		{
			return treeView.afterSelect<T>((item)=> propertyGrid.show(item));			
		}
		
		public static TreeView hideSelection(this TreeView treeView)
		{
			return treeView.showSelection(false);
		}

		public static TreeView showSelection(this TreeView treeView, bool value)
		{
			return (TreeView)treeView.invokeOnThread(
									()=>{
											treeView.HideSelection = value.isFalse();
											return treeView;
										});
		}		
		
		public static TreeView allow_TreeNode_Edits(this TreeView treeView)
		{
			if (treeView.notNull())
				treeView.invokeOnThread(()=> treeView.LabelEdit = true);		
			return treeView;
		}
		
		public static TreeNode beginEdit(this TreeNode treeNode)
		{
			if (treeNode.notNull())
				treeNode.treeView().invokeOnThread(()=> treeNode.BeginEdit());
			return treeNode;
		}				
		
		public static TreeNode set_Tag(this TreeNode treeNode, object value)
		{
			return (TreeNode)treeNode.treeView().invokeOnThread(
				()=>{
						treeNode.Tag = value;
						return treeNode;
					});
		}
		
		public static TreeNode insert_Node(this TreeView treeView, string text)
		{
			return treeView.insert_Node(text,0);
		}
		
		public static TreeNode insert_Node(this TreeView treeView, string text, int position)
		{
			return treeView.insert_TreeNode(text,text,position);
		}
		
		public static TreeNode insert_TreeNode(this TreeView treeView, string text, object tag, int position)
		{
			return treeView.rootNode().insert_TreeNode(text,tag, position);
		}
		public static TreeNode insert_TreeNode(this TreeNode treeNode, string text, object tag, int position)
		{
			return (TreeNode)treeNode.treeView().invokeOnThread(
				()=>{
						var newNode = treeNode.Nodes.Insert(position, text);
						newNode.Tag = tag;
						return treeNode;
					});
		}
		
		//not working
		/*public static TreeView add_Nodes<T>(this TreeView treeView, params string[] nodes)
		{
			treeView.rootNode().add_Nodes(nodes);
			return treeView;
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, params string[] nodes)
		{
			Ascx_ExtensionMethods.add_Nodes(treeNode,nodes);			
			return treeNode;
		}*/
		
		//IEnumerable<T> collection, bool addDummyNode
		
		public static TreeView add_Nodes(this TreeView treeView, Dictionary<string, List<string>> dictionary)
		{
			foreach(var item in dictionary)
			{
				var nodeText = "{0}    ({1} items)".format(item.Key, item.Value.size());
				treeView.add_Node(nodeText, item.Value).add_Nodes(item.Value);
			}
			return treeView;
		}
		
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, bool addDummyNode)
		{
				
			treeView.rootNode().add_Nodes(collection, (item)=> item.str() ,(item)=> item,(item)=> addDummyNode);			
			return treeView;
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, bool addDummyNode)
		{
			return treeNode.add_Nodes(collection, (item)=> item.str() ,(item)=> item, (item)=> addDummyNode);						
		}
		
		//IEnumerable<T> collection, Func<T, bool> getAddDummyNode
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T, bool> getAddDummyNode)
		{
			treeView.rootNode().add_Nodes(collection, (item)=> item.str() ,(item)=> item, getAddDummyNode);			
			return treeView;
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T, bool> getAddDummyNode)
		{
			return treeNode.add_Nodes(collection, (item)=> item.str() ,(item)=> item, getAddDummyNode);						
		}
		
		//IEnumerable<T> collection, Func<T,string> getNodeName)
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName)
		{
			treeView.rootNode().add_Nodes(collection, getNodeName,(item)=> item,(item)=> false);			
			return treeView;
		}
				
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName)
		{
			return treeNode.add_Nodes(collection, getNodeName, (item)=> item, (item)=> false);			
		}
		
		
		//IEnumerable<T> collection, Func<T,string> getNodeName, getColor)
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, Color> getColor)
		{
			return treeView.add_Nodes<T>(collection, getNodeName, (item)=> false, getColor);
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, Color> getColor)
		{
			return treeNode.add_Nodes<T>(collection, getNodeName, (item)=> false, getColor);
		}
		
		//IEnumerable<T> collection, Func<T,string> getNodeName, addDummyNode, getColor)
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, bool> getAddDummyNode, Func<T, Color> getColor)
		{
			return treeView.add_Nodes<T>(collection, getNodeName, (item)=>item, getAddDummyNode, getColor);
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, bool> getAddDummyNode, Func<T, Color> getColor)
		{
			return treeNode.add_Nodes<T>(collection, getNodeName, (item)=>item, getAddDummyNode, getColor);
		}
		
		//IEnumerable<T> collection, Func<T,string> getNodeName, getTagValue, addDummyNode, getColor)
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T,object> getTagValue,  Func<T, bool> getAddDummyNode, Func<T, Color> getColor)
		{
			treeView.rootNode().add_Nodes(collection, getNodeName,getTagValue, getAddDummyNode, getColor);
			return treeView;
		}
				
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T,object> getTagValue, Func<T, bool> getAddDummyNode, Func<T, Color> getColor)
		{
			foreach(var item in collection)
			{
				var newNode = Ascx_ExtensionMethods.add_Node(treeNode,getNodeName(item), getTagValue(item), getAddDummyNode(item));
				newNode.color(getColor(item));
			}
			return treeNode;
		}
		
		//IEnumerable<T> collection, Func<T,string> getNodeName, bool addDummyNode
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName, bool addDummyNode)
		{
			treeView.rootNode().add_Nodes(collection, getNodeName, (item)=> item,(item)=> addDummyNode);			
			return treeView;
		}
				
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName, bool addDummyNode)
		{
			return treeNode.add_Nodes(collection, getNodeName, (item)=> item,(item)=> addDummyNode);			
		}
		
		//Func<T,string> getNodeName, Func<T, object> getTagValue, Func<T,bool> getAddDummyNode
		public static TreeView add_Nodes<T>(this TreeView treeView, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, object> getTagValue, Func<T,bool> getAddDummyNode)
		{
			treeView.rootNode().add_Nodes(collection, getNodeName, getTagValue, getAddDummyNode);
			return treeView;
		}
		
		public static TreeNode add_Nodes<T>(this TreeNode treeNode, IEnumerable<T> collection, Func<T,string> getNodeName, Func<T, object> getTagValue, Func<T,bool> addDummyNode)
		{
			foreach(var item in collection)
				Ascx_ExtensionMethods.add_Node(treeNode,getNodeName(item), getTagValue(item), addDummyNode(item));
			return treeNode;
		}
		
		
		//Events
		
		public static TreeView onDoubleClick(this TreeView treeView, Action  callback)
		{
			return treeView.onDoubleClick<object>((tag)=>callback());
		}
		public static TreeView onDoubleClick(this TreeView treeView, Action<object> callback)
		{
			return treeView.onDoubleClick<object>((tag)=>callback(tag));
		}
		
		//so that it doesn't conflict with the version from O2.DotNetWrappers.ExtensionMethods.Ascx_ExtensionMethods
		public static TreeView onDoubleClk<T>(this TreeView treeView, Action<T> callback)
		{
			treeView.invokeOnThread(
				()=>{
						treeView.DoubleClick+= 
							(sender,e)=>{															
											object tag = Ascx_ExtensionMethods.get_Tag(treeView.selected());
											if (tag is T)
												O2Thread.mtaThread(()=> callback((T)tag));
										 };
					});
			return treeView;		
		}
	
		
		//add Files to TreeView
		public static TreeView add_Files(this TreeView treeView, String folder)
		{
		return treeView.add_Files(folder, "*.*",true);
		}
		
		public static TreeView add_Files(this TreeView treeView, String folder, string filter)
		{
		return treeView.add_Files(folder, filter,true);
		}
		
		public static TreeView add_Files(this TreeView treeView, String folder, string filter, bool recursive)
		{
			return treeView.add_Files(folder.files(filter,recursive));
		}
		
		public static TreeView add_Files(this TreeView treeView, List<string> files)
		{
			return treeView.add_Nodes(files, (file)=>file.fileName());
		}
		
		public static TreeView add_File(this TreeView treeView, string file)
		{
			return treeView.add_Files(file.wrapOnList());
		}
				
		
		public static List<string> texts(this List<TreeNode> treeNodes)
		{
			return (from treeNode in treeNodes
					select treeNode.get_Text()).toList();
		}
		
		public static List<T> tags<T>(this TreeView treeView)
		{
			return treeView.nodes().tags<T>();
		}
		
		public static List<T> tags<T>(this List<TreeNode> treeNodes)
		{
			return (from treeNode in treeNodes
					where Ascx_ExtensionMethods.get_Tag(treeNode) is T
					select (T)Ascx_ExtensionMethods.get_Tag(treeNode)).toList();
		}

	
	
		//SourceCodeEditor
		
		public static ascx_SourceCodeEditor showCodeEditorForFilesInFolder(this string path, string filter)		
		{	
			var topPanel = "Code Editor for {0} files in folder {1}".format(filter,path).popupWindow(900,400);
			var codeEditor = topPanel.add_GroupBox("SourceCode Editor")
									 .add_SourceCodeEditor();										 
									 
			var treeView = topPanel .insert_Left(200,"Files: {0}".format(filter))
									.add_TreeView()
									.onDrag<string>()					
									.afterSelect<string>((file)=>codeEditor.open(file));
			Action loadFiles = 
				()=>{
						treeView.clear();
						treeView.add_Files(path,filter)
								.selectFirst();
					};
			treeView.add_ContextMenu().add_MenuItem("reload files", ()=>loadFiles());			
			loadFiles();
			return codeEditor;		  
		}
	}
	
}
    	