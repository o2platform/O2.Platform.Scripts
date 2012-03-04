// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
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

namespace O2.XRules.Database.Utils
{	
		
	public static class _Extra_sourceCodeViewer_ExtensionMethods
	{
		public static ascx_SourceCodeViewer maximizeViewer(this ascx_SourceCodeViewer codeViewer)
		{
			codeViewer.textEditorControl().fill().bringToFront();
			return codeViewer;
		}

		public static ascx_SourceCodeEditor editor(this ascx_SourceCodeEditor codeEditor)
		{
			return codeEditor;
		}
		
		public static ascx_SourceCodeViewer onTextChange(this ascx_SourceCodeViewer codeViewer, Action<string> callback)
		{
			codeViewer.editor().onTextChange(callback);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor onTextChange(this ascx_SourceCodeEditor codeEditor, Action<string> callback)
		{
			codeEditor.invokeOnThread(
				()=>{
						codeEditor.eDocumentDataChanged+= callback;
					});
			return codeEditor;
		}
		
		public static ascx_SourceCodeViewer open(this ascx_SourceCodeViewer codeViewer, string file , int line)
		{
			codeViewer.editor().open(file, line);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor open(this ascx_SourceCodeEditor codeEditor, string file , int line)
		{
			codeEditor.open(file);
			codeEditor.gotoLine(line);
			return codeEditor;
		}
		
		public static ascx_SourceCodeViewer gotoLine(this ascx_SourceCodeViewer codeViewer, int line)
		{
			codeViewer.editor().gotoLine(line);
			return codeViewer;
		}
		
		public static ascx_SourceCodeViewer gotoLine(this ascx_SourceCodeViewer codeViewer, int line, int showLinesBelow)
		{
			codeViewer.editor().gotoLine(line,showLinesBelow);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor gotoLine(this ascx_SourceCodeEditor codeEditor, int line, int showLinesBelow)
		{
			if (line>0)
			{
				codeEditor.caret_Line(line,-showLinesBelow);			
				codeEditor.caret_Line(line,showLinesBelow);						
				codeEditor.gotoLine(line);
			}
			return codeEditor;
		}
		
		
		

	}}    	