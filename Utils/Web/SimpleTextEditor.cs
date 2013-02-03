// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.Script
{
    public class SimpleTextEditor
    {    
		public static RichTextBox launch()
		{
			var richTextBox = "Simple TextEditor (*.RTF based)".popupWindow<RichTextBox>(400,300);
								//.insert_LogViewer();

			richTextBox.onDrop( 
						(file)=>{
									if (file.isFile() && file.extension("*.rtf"))
										richTextBox.set_Rtf(file.fileContents());
								});
			richTextBox.add_ContextMenu()
					   .add_MenuItem("Save", 
					  	 ()=>{
								var fileToSave = O2Forms.askUserForFileToSave(Environment.CurrentDirectory,"*.rtf");
								richTextBox.SaveFile(fileToSave); 
							 });
			
			return richTextBox;
		}
    }
}
