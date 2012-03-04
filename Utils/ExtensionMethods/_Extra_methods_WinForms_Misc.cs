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


namespace O2.XRules.Database.Utils
{	
	
	//RichTextBox
	public static class _Extra_RichTextBox_ExtensionMethods
	{
		public static RichTextBox scrollToCaret(this RichTextBox richTextBox)
		{
			return (RichTextBox)richTextBox.invokeOnThread(
						()=>{
								richTextBox.ScrollToCaret();
								return richTextBox;
							});						
		}
		
		public static RichTextBox scrollToEnd(this RichTextBox richTextBox)
		{
			return (RichTextBox)richTextBox.invokeOnThread(
						()=>{
								richTextBox.SelectionStart = richTextBox.get_Text().size()-1;
								richTextBox.ScrollToCaret();
								return richTextBox;
							});						
		}
		public static RichTextBox scrollToStart(this RichTextBox richTextBox)
		{
			return (RichTextBox)richTextBox.invokeOnThread(
						()=>{
								richTextBox.SelectionStart = 0;
								richTextBox.ScrollToCaret();
								return richTextBox;
							});						
		}
		
		public static RichTextBox wordWrap(this RichTextBox richTextBox, bool value)
		{
			return (RichTextBox)richTextBox.invokeOnThread(
						()=>{
								richTextBox.WordWrap = value;
								return richTextBox;
							});						
		}
		
		public static RichTextBox hideSelection(this RichTextBox richTextBox, bool value)
		{
			return (RichTextBox)richTextBox.invokeOnThread(
						()=>{
								richTextBox.HideSelection = value;
								return richTextBox;
							});						
		}
		
		public static RichTextBox hideSelection(this RichTextBox richTextBox)
		{
			return richTextBox.hideSelection(true);
		}
		
		public static RichTextBox showSelection(this RichTextBox richTextBox)
		{
			return richTextBox.hideSelection(false);
		}
	}
	//TextBox
	public static class _Extra_TextBox_ExtensionMethods
	{
		public static TextBox add_TextBox(this Control control, string text)
		{
			var textBox = control.add_TextBox();
			textBox.set_Text(text);
			return textBox;
		}
		
		public static TextBox add_TextArea(this Control control, string text)
		{
			var textBox = control.add_TextArea();
			textBox.set_Text(text);
			return textBox;
		}
				
	}
	
		//PropertyGrid
	public static class _Extra_PropertyGrid_ExtensionMethods
	{
		public static PropertyGrid onValueChange(this PropertyGrid propertyGrid, Action callback)
		{
			propertyGrid.invokeOnThread(()=>propertyGrid.PropertyValueChanged+=(sender,e)=>callback() );
			return propertyGrid;
		}
	}
	
	//SplitContainer
	public static class _Extra_SplitContainer_ExtensionMethods
	{
		public static T splitterDistance<T>(this T control, int distance)
			where T : Control
		{
			var splitContainer = control.splitContainer();
			if (splitContainer.notNull())
				Ascx_ExtensionMethods.splitterDistance(splitContainer,distance);
			return control;
		}
		
		
		//Split Container
		
		public static SplitContainer splitContainer(this Control control)
		{
			return control.parent<SplitContainer>();
		}
		
		public static SplitContainer splitterWidth(this SplitContainer splitContainer, int value)
		{
			splitContainer.invokeOnThread(()=> splitContainer.SplitterWidth = value);
			return splitContainer;
		}
		
		public static SplitContainer splitContainerFixed(this Control control)
		{
			return control.splitContainer().isFixed(true);
		}
		
		public static SplitContainer @fixed(this SplitContainer splitContainer, bool value)
		{
			return 	splitContainer.isFixed(value);
		}
		
		public static SplitContainer isFixed(this SplitContainer splitContainer, bool value)
		{
			splitContainer.invokeOnThread(()=> splitContainer.IsSplitterFixed = value);
			return splitContainer;
		}
	}		
	
	//TabControl
	public static class _Extra_TabControl_ExtensionMethods
	{
		public static TabControl selectedIndex(this TabControl tabControl, int index)
		{
			return (TabControl)tabControl.invokeOnThread(
											()=>{
													tabControl.SelectedIndex = index;
													return tabControl;
												});
		}
	}
}
    	