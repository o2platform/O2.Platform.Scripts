// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods ; 
using O2.Kernel;
using O2.Tool.HostLocalWebsite;
using O2.Tool.HostLocalWebsite.classes;
using O2.XRules.Database.Utils;

//O2File:webservices.cs
//O2File:ascx_HostLocalWebsite.designer.cs

namespace O2.Tool.HostLocalWebsite.ascx
{
	public class test_ascx_HostLocalWebsite
	{
		public void test()
		{	
			"test_ascx_HostLocalWebsite".popupWindow(800,300)
										.add_Control<ascx_HostLocalWebsite>();
		}
	}
    public partial class ascx_HostLocalWebsite : UserControl
    {
    	public WebServices webservices;
    	
        public ascx_HostLocalWebsite()
        {
            InitializeComponent();
            ascx_DropObject1.Text = "Drop Folder here To Start WebServer (on that Folder)";           
            btStartWebService.Text = "Restart WebServer";
            webservices = new WebServices();
            
            //extra GUI tweeks
            
            tbUrlToLoad.width(btReloadUrl.left() - tbUrlToLoad.left() -10);
			tbUrlToLoad.anchor_TopLeftRight();
			
			btStartWebService.append_Control<Button>()
				 .set_Text("Stop ")
				 .width(40)
				 .height(btStartWebService.height())
				 .onClick(()=> webservices.StopWebService());
				 
			this.control<GroupBox>().align_Right();
			tbSettings_Path.widthAdd(-50); 
			tbSettings_Exe.widthAdd(-50); 	
			tbSettings_Path.append_Link("explore",()=>tbSettings_Path.get_Text().startProcess()).anchor_TopRight();
			tbSettings_Exe.append_Link("explore",()=>tbSettings_Exe.get_Text().parentFolder().startProcess()).anchor_TopRight();
        }

        private void onDispose()
        {	
			if (webservices.notNull())
				webservices.StopWebService();
        }

        private void ascx_WebServiceScan_Load(object sender, EventArgs e) // this is not being fired at the moent
        {
            if (DesignMode == false)
            {                
                setupGui();
            }
        }

        public void setupGui()
        {          
            tbSettings_Exe.Text = webservices.sExe;
            tbSettings_Port.Text = webservices.sPort;
            tbSettings_Path.Text = webservices.sPath;
            tbSettings_VPath.Text = webservices.sVPath;
        }
 		public void start()
 		{
 			btStartWebService_Click(null,null);
 		}
 		
        private void btStartWebService_Click(object sender, EventArgs e)
        {
        	try
        	{
	            if (tbSettings_Path.Text == "")
	                setupGui();
	
	            if (tbSettings_Path.Text[tbSettings_Path.Text.Length - 1] == '\\')
	                tbSettings_Path.Text = tbSettings_Path.Text.Substring(0, tbSettings_Path.Text.Length - 1);
	
	            webservices.StopWebService();
	            webservices.StartWebService();
	        }
	        catch (Exception ex)
	        {
	        	PublicDI.log.error("in btStartWebService_Click: {0}", ex);
	        }
	        var webServiceUrl = webservices.GetWebServiceURL();
            tbUrlToLoad.Text = webServiceUrl;
        }

        private void btSetTestEnvironment_Click(object sender, EventArgs e)
        {
            setupGui();
        }

        private void tbSettings_TextChanged(object sender, EventArgs e)
        {
            webservices.setExe(tbSettings_Exe.Text);
            webservices.setPort(tbSettings_Port.Text);
            webservices.setPath(tbSettings_Path.Text);
            webservices.setVPath(tbSettings_VPath.Text);
        }

        private void tbUrlToLoad_TextChanged(object sender, EventArgs e)
        {
            wbWebServices.Navigate(tbUrlToLoad.Text);
        }

        private void btReloadUrl_Click(object sender, EventArgs e)
        {
            wbWebServices.Navigate(tbUrlToLoad.Text);
        }

        private void ascx_DropObject1_eDnDAction_ObjectDataReceived_Event(object oObject)
        {
            String sItemToLoad = oObject.ToString();
            if (Directory.Exists(sItemToLoad))
                startWebServerOnDirectory(sItemToLoad);
            else
                PublicDI.log.error("Item dropped must be a folder");
        }

		public string startWebServerOnDirectory(string sDirectoryToProcess)	
		{
			var randomPort = (50000 + new Random().Next(10000)).ToString();
			return startWebServerOnDirectory(sDirectoryToProcess,randomPort);
			
		}
		
        public string startWebServerOnDirectory(string sDirectoryToProcess, string port)
        {
        	return (string)this.invokeOnThread(
        			()=> {        				
				            if (tbSettings_Exe.Text == "")
				                tbSettings_Exe.Text = webservices.sExe;
				            tbSettings_Port.Text = port;
				            tbSettings_Path.Text = sDirectoryToProcess;
				            tbSettings_VPath.Text = "/" + Path.GetFileName(sDirectoryToProcess);
				            btStartWebService_Click(null, null);
				            return tbUrlToLoad.Text;
				         });
        }
    }
}
