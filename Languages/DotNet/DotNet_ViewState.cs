// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;

//O2Ref:System.Web.dll

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{

	// this script (namely the ViewStateXmlBuilder and PopulateTree bit)
	// contains mutiple code snippets from Pluralsight Fritz Onion ViewState Decoder (2.2)
	// (see http://mercury.pluralsight.com/tools.aspx )
	// the code snippets were created by Reflector 
	
	public class DotNet_ViewState
	{		
		public string ViewState_XmlString { get; set; }
		public string ControlState_XmlString { get; set; }
		
		public XmlDocument ViewState_XmlDocument  { get; set; }
		public XmlDocument ControlState_XmlDocument  { get; set; }
		
		public XElement ViewState_XElement  { get; set; }
		public XElement ControlState_XElement  { get; set; }		
		
		public List<string> ViewState_Values { get; set; }
		public List<string> ControlState_Values { get; set; }
		
		public DotNet_ViewState()
		{
			ViewState_Values = new List<string>();
			ControlState_Values = new List<string>();
		}
		
		public DotNet_ViewState(string rawViewState) : this()
		{			
			decodeRawViewState(rawViewState);
		}
		
		public DotNet_ViewState decodeRawViewState(string rawViewState)
		{		
			if (rawViewState.valid())
			{
				try
				{
					LosFormatter formatter = new LosFormatter();			
					XmlDocument controlState;
					var viewState = ViewStateXmlBuilder.BuildXml(formatter.Deserialize(rawViewState), out controlState);
					
					ViewState_XmlDocument = viewState;
					ControlState_XmlDocument = controlState;
					
					ViewState_XmlString = viewState.xmlString();
					ControlState_XmlString = controlState.xmlString();
										
					ViewState_XElement = ViewState_XmlString.xRoot();
					ViewState_Values = (from element in ViewState_XElement.elementsAll()
									where element.elements().size() == 0
									select element.Value).toList();		
					
					
					ControlState_XElement = ControlState_XmlString.xRoot();																				
					ControlState_Values = (from element in ControlState_XElement.elementsAll()
										  where element.elements().size() == 0
										  select element.Value).toList();	
				}
				catch(Exception ex)
				{
					ex.log("in DotNet_ViewState decodeRawViewState");
				}
			}
			else
				"in DotNet_ViewState: invalid ViewState provided".error();
			return this;
		}
	}	

	public static class DotNet_ViewState_ExtensionMethods
	{
	
		public static T showValues<T>(this DotNet_ViewState dotNetViewState, T control )
			where T : System.Windows.Forms.Control
		{			
			control.clear();
			control.add_TextArea()
				   .scrollBars()
				   .set_Text(StringsAndLists.fromStringList_getText(dotNetViewState.ViewState_Values));
				   
			return control;
		}
			
		public static T show<T>(this DotNet_ViewState dotNetViewState, T control )
			where T : System.Windows.Forms.Control
		{			
			control.clear();
			
			var topControls = control.add_1x1("ViewState","ControlState", false);
			var viewStateControls =  topControls[0].add_1x1x1("TreeView", "Xml", "Values");
			var controlStateControls = topControls[1].add_1x1x1("TreeView", "Xml", "Values");
			
			
			
			var viewState_TreeView = viewStateControls[0].add_TreeView().visible(false);
			var controlState_TreeTree = controlStateControls[0].add_TreeView().visible(false);			
			
			var viewState_XmlViewer = viewStateControls[1].add_SourceCodeViewer().visible(false);
			var controlState_XmlViewer =  controlStateControls[1].add_SourceCodeViewer().visible(false);
			
			var viewState_TextBox = viewStateControls[2].add_TextArea()
														.scrollBars();		
			var controlState_TextBox = controlStateControls[2].add_TextBox()
															  .multiLine()
															  .fill()
															  .scrollBars();
						
			viewState_TreeView.PopulateTree(dotNetViewState.ViewState_XmlDocument);
			controlState_TreeTree.PopulateTree(dotNetViewState.ControlState_XmlDocument);
			
			viewState_XmlViewer.set_Text(dotNetViewState.ViewState_XmlString, ".xml").visible(true);
			controlState_XmlViewer.set_Text(dotNetViewState.ControlState_XmlString, ".xml").visible(true);							  
			
			viewState_TextBox.set_Text(StringsAndLists.fromStringList_getText(dotNetViewState.ViewState_Values));			
			controlState_TextBox.set_Text(StringsAndLists.fromStringList_getText(dotNetViewState.ControlState_Values));

			viewState_TreeView.visible(true);
			controlState_TreeTree.visible(true);
			
			return control;
		}
		
		public static TreeView PopulateTree(this TreeView treeView, XmlDocument dom)
		{
			return (TreeView)treeView.invokeOnThread(
				()=>{
					    treeView.Nodes.Clear();
					    TreeNode node = new TreeNode(dom.DocumentElement.Name);
					    treeView.Nodes.Add(node);
					    dom.DocumentElement.AddChildren(node);
					    treeView.ExpandAll();
					    return treeView;
					 });
				    
		}

		private static void AddChildren(this XmlNode elem, TreeNode node)
		{
		    foreach (XmlNode node2 in elem.ChildNodes)
		    {
		        if (node2.NodeType == XmlNodeType.Element)
		        {
		            TreeNode node3 = new TreeNode(node2.Name);
		            node.Nodes.Add(node3);
		            if (node2.HasChildNodes)
		            {
		                node2.AddChildren(node3);
		            }
		        }
		        else
		        {
		            TreeNode node4 = new TreeNode(node2.InnerText);
		            node.Nodes.Add(node4);
		        }
		    }
		}
	}
	
	
	public class ViewStateXmlBuilder
	{
			    // Methods
	    private static void BuildElement(XmlDocument dom, XmlElement elem, object treeNode, ref XmlDocument controlstateDom)
	    {
	        if (treeNode != null)
	        {
	            XmlElement element;
	            Type type = treeNode.GetType();
	            if (type == typeof(Triplet))
	            {
	                element = dom.CreateElement(GetShortTypename(treeNode));
	                elem.AppendChild(element);
	                BuildElement(dom, element, ((Triplet) treeNode).First, ref controlstateDom);
	                BuildElement(dom, element, ((Triplet) treeNode).Second, ref controlstateDom);
	                BuildElement(dom, element, ((Triplet) treeNode).Third, ref controlstateDom);
	            }
	            else if (type == typeof(Pair))
	            {
	                element = dom.CreateElement(GetShortTypename(treeNode));
	                elem.AppendChild(element);
	                BuildElement(dom, element, ((Pair) treeNode).First, ref controlstateDom);
	                BuildElement(dom, element, ((Pair) treeNode).Second, ref controlstateDom);
	            }
	            else if (type == typeof(ArrayList))
	            {
	                element = dom.CreateElement(GetShortTypename(treeNode));
	                elem.AppendChild(element);
	                foreach (object obj2 in (ArrayList) treeNode)
	                {
	                    BuildElement(dom, element, obj2, ref controlstateDom);
	                }
	            }
	            else if (treeNode is Array)
	            {
	                element = dom.CreateElement("Array");
	                elem.AppendChild(element);
	                foreach (object obj3 in (Array) treeNode)
	                {
	                    BuildElement(dom, element, obj3, ref controlstateDom);
	                }
	            }
	            else if (treeNode is HybridDictionary)
	            {
	                element = controlstateDom.CreateElement(GetShortTypename(treeNode));
	                controlstateDom.DocumentElement.AppendChild(element);
	                foreach (object obj4 in (HybridDictionary) treeNode)
	                {
	                    BuildElement(controlstateDom, element, obj4, ref controlstateDom);
	                }
	            }
	            else if (treeNode is DictionaryEntry)
	            {
	                element = dom.CreateElement(GetShortTypename(treeNode));
	                elem.AppendChild(element);
	                DictionaryEntry entry = (DictionaryEntry) treeNode;
	                BuildElement(dom, element, entry.Key, ref controlstateDom);
	                DictionaryEntry entry2 = (DictionaryEntry) treeNode;
	                BuildElement(dom, element, entry2.Value, ref controlstateDom);
	            }
	            else
	            {
	                element = dom.CreateElement(GetShortTypename(treeNode));
	                if (type == typeof(IndexedString))
	                {
	                    element.InnerText = ((IndexedString) treeNode).Value;
	                }
	                else
	                {
	                    element.InnerText = treeNode.ToString();
	                }
	                elem.AppendChild(element);
	            }
	        }
	    }
	
	    public static XmlDocument BuildXml(object tree, out XmlDocument controlstateDom)
	    {
	        XmlDocument dom = new XmlDocument();
	        controlstateDom = new XmlDocument();
	        dom.AppendChild(dom.CreateElement("viewstate"));
	        controlstateDom.AppendChild(controlstateDom.CreateElement("controlstate"));
	        BuildElement(dom, dom.DocumentElement, tree, ref controlstateDom);
	        return dom;
	    }
	
	    private static string GetShortTypename(object obj)
	    {
	        string str = obj.GetType().ToString();
	        return str.Substring(str.LastIndexOf(".") + 1);
	    }		
	}	            	
}
