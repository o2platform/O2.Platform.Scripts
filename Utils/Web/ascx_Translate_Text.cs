// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;
using Jayrock.Json;
using Jayrock.Json.Conversion;

//O2Ref:Jayrock.Json.dll

namespace O2.Script
{
    public class ascx_Translate_Text : UserControl
    {    
    	private static IO2Log log = PublicDI.log;
		private TextBox source_TextBox;
		private TextBox translation_TextBox;
        public static void startControl()
    	{       		    		    		    		  
			typeof(ascx_Translate_Text).showAsForm("Translate Text",400, 400);		    		
    	}    	
    	    
    	public ascx_Translate_Text()
    	{    		    	
    		buildGui();	            
        }
    
        private void buildGui()
        {
        	var controls = this.add_1x1("original text", "translated text");
        	
        	source_TextBox = controls[0].add_TextBox(true);
        	translation_TextBox = controls[1].add_TextBox(true);       	
			source_TextBox.TextChanged +=(sender,e) => translateText(source_TextBox.Text);
			
			source_TextBox.set_Text("Good morning from London");
     	}   
     	
     	public void translateText(string textToTranslate)
     	{
     		if (textToTranslate.valid())
     		{
     			O2Thread.mtaThread(
     				()=>{     		
							var htmlText = translateViaGoogle(textToTranslate, "en", "pt");
    						var translation = extractTranslationText(htmlText);
    						translation_TextBox.set_Text(translation);			
    					});
    		}
     	}
     	
     	public static string translateViaGoogle(string textToTranslate, string sourceLanguage, string targetLanguage)
    	{
    		var urlRequest = "http://translate.google.com/translate_a/t?client=t&text={0}&sl={1}&tl={2}&pc=0"
    						 .format(textToTranslate,sourceLanguage,targetLanguage);
    		return new Web().getUrlContents(urlRequest);		
    	}
    	
    	public static string extractTranslationText(string translatedText)
    	{
    		var data = (JsonObject)JsonConvert.Import(translatedText);
    		var sentences = (JsonArray)data["sentences"];   		
    		var itemData = (JsonObject)sentences[0];
    		return itemData["trans"].ToString();    			
    		return data.typeName();
    	}

    	    	    	    	    
    }
}
