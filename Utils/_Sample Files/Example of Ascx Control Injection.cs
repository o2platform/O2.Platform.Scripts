// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;

namespace O2.Script
{
    public class ascx_GuiTest: UserControl
    {    
    	private static IO2Log log = PublicDI.log;

        public static void startControl()
    	{       		
			var control = (ascx_GuiTest)typeof(ascx_GuiTest).showAsForm(
				"PoC for SplitContainer Control Injection", 400, 400);		    					
    	}    	
    	
    	public ascx_GuiTest()
    	{    		    		
    		this.Load+= (sender, e) => buildGui();      // fire this on the onLoad event so that the ParentForm is already loaded    		
    		
    		//buildGui();  								// usually just doing this works, but in this case we need the Parent form size to be set
        }
		    
        private void buildGui()
        {
        	var textBox = this.add_TextBox(true);        	        	        	
        	var distance = 100;
        	var border3D = false;        	
        	textBox.insert_Above(new Label().set_Text("Top"), distance, border3D);
        	textBox.insert_Below(new Label().set_Text("Bottom") , distance,border3D);
			textBox.insert_Left(new Label().set_Text("Left"), distance , border3D);
        	textBox.insert_Right(new Label().set_Text("Right"), distance , border3D);
        	textBox.Select();
     	}   
    	    	    	    	    
    }
        
}
