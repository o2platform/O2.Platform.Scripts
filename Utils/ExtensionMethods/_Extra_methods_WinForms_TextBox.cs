// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
		
		public static TextBox add_TextArea_With_SearchPanel(this Control control)
		{
			return control.add_TextBox(true).insert_Below_RegEx_SearchPanel();
		}
		
		
		public static TextBox scrollToSelection(this TextBox textBox)
		{
			return textBox.scrollToCaret();
		}
		
		public static TextBox scrollToCaret(this TextBox textBox)
		{
			return (TextBox)textBox.invokeOnThread(
				()=>{
						textBox.ScrollToCaret();
						return textBox;
					});
		}
		
		public static TextBox showSelection(this TextBox textBox, bool value = true)
		{
			return textBox.hideSelection(!value);
		}
		
		public static TextBox hideSelection(this TextBox textBox, bool value = true)
		{
			return (TextBox)textBox.invokeOnThread(
				()=>{
						textBox.HideSelection = value;
						return textBox;
					});
		}
		
		public static TextBox selectionLength(this TextBox textBox, int value)
		{
			return (TextBox)textBox.invokeOnThread(
				()=>{
						textBox.SelectionLength = value;
						return textBox;
					});
		}
		
		public static TextBox selectionStart(this TextBox textBox, int value)
		{
			return (TextBox)textBox.invokeOnThread(
				()=>{
						textBox.SelectionStart = value;
						return textBox;
					});
		}		
		
		public static TextBox insert_Below_RegEx_SearchPanel(this TextBox targetTextBox)
		{
			//var targetTextBox 			= textBox.showSelection(); 
			targetTextBox.showSelection();
			var searchPanel 			= targetTextBox.insert_Below(45, "Search for: (RegEx)");
			var searchText_TextBox 		= searchPanel.add_TextBox();
			var searchResults_ComboBox 	= searchText_TextBox.append_Control<ComboBox>().dropDownList();
			var searchResults_Label 	= searchResults_ComboBox.append_Label("...").autoSize().top(3);
			
			Func<string> textToSearch = ()=> targetTextBox.get_Text();
			
			Action<string> runSearch = 
				(matchPattern)=>{	
									targetTextBox.selectionLength(0);
									searchResults_ComboBox.clear();
									if (matchPattern.valid())
									{
										var matches = textToSearch().regEx_Matches(matchPattern);						 						
										searchResults_ComboBox.add_Items(matches.ToArray());
										if (matches.size()>0)
											searchResults_ComboBox.select_Item(0);
									}
									searchResults_Label.set_Text("{0} results".format(searchResults_ComboBox.items().size()));									
								 };
			
			searchResults_ComboBox.onSelection<Match>(
				(match)=>{
							targetTextBox.select(match.Index, match.Length)
										 .scrollToSelection();
						 });
			 
			searchText_TextBox.onTextChange(runSearch); 
			return targetTextBox;
		}
	}
}
    	