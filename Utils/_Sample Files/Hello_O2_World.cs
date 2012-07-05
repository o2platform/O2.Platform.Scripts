// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using O2.Kernel;
using O2.Views.ASCX.classes.MainGUI;
using O2.DotNetWrappers.ExtensionMethods;


namespace O2.XRules.Database.Samples
{
    public class myFirstScripts
    {        
        public static void sayHelloO2World()
        {       
        	var response = "Hello there, do you want me to say hello to you?".askUserQuestion();
        	if (response)        	
        		DebugMsg.showMessageBox("Thanks and Hello from the OWASP O2 Platform :)");            
        }                         
    }

}
