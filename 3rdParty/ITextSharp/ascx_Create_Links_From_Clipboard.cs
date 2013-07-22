// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

//O2Ref:ITextSharp/itextsharp.dll
//Installer:ITextSharp_Installer.cs!ITextSharp/itextsharp.dll


namespace O2.XRules.Database.Utils
{
    public class ascx_Create_Links_From_Clipboard : Control
    {    
    	private static IO2Log log = PublicDI.log;
		public static RichTextBox textBox;
		public static WebBrowser webBrowser;
		
		public static void startControl()
		{
			O2Gui.open<ascx_Create_Links_From_Clipboard>("Create Links from Clipboard", 400,400);
		}
        public ascx_Create_Links_From_Clipboard()
        {
        	this.Width = 400;
        	this.Height = 400;
        	buildGui();	
        }
        
        public void buildGui()
        {
        	var controls = this.add_1x1("Text to convert","converted links",true,200);
        	textBox = controls[0].add_RichTextBox();
        	webBrowser = controls[1].add_WebBrowser();        	
        	// events
        	textBox.onDrop(convertFile);
        	textBox.TextChanged += (sender,e) => convertText(textBox.Text);
        }
        
        public void convertFile(string fileToConvert)
        {        	
        	O2Thread.mtaThread(
        		()=>{
			        	//fileToConvert.error();
			        	if (fileToConvert.fileExists())            
			                if (fileToConvert.extension(".pdf"))
			                {
			                    textBox.set_Text("...processing pdf file: " + fileToConvert);
			                    var pdfReader = new PdfReader(fileToConvert);

								var text = "";
								for(int i=1; i < pdfReader.NumberOfPages + 1 ; i++)
									text += "Page: {0}\n\n{1}\n\n".format(i, PdfTextExtractor.GetTextFromPage(pdfReader,i));
    
			                    textBox.set_Text(text);
							}
			                else
			                    textBox.set_Text(fileToConvert.contents());        	
			        });
        }
        
        public void convertText(string textToConvert)
        {
	        O2Thread.mtaThread(
        		()=>{
			        	//textToConvert.size().str().info();
			        	textToConvert = textToConvert.replaceAllWith(" ","\"","'",Environment.NewLine,"\n"); 
			        	var itemsToConvert = new List<String>();        	
			        	foreach(var textSnippet in textToConvert.split_onSpace())
			        		itemsToConvert.Add(textSnippet);
			        	textToConvert.info();
			        	var htmlCode = "";
			        	foreach(var item in itemsToConvert)
			        		if (item.validUri() && item.ToLower().starts("http"))
				        		htmlCode += "<a href=\"{0}\">{0}</a><br/>".format(item).line();
			        		
			        	webBrowser.set_Html(htmlCode);
			        });
        }
    	    	    	    	    
    }
}
