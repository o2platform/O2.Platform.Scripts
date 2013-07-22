// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Utils;
using O2.Core.FileViewers.J2EE;
using O2.Core.FileViewers.ViewHelpers;
using System.IO;

//O2File:ascx_Struts_config_xml.Designer.cs
//O2File:CreateDataTable.cs
//O2File:J2eeConfigFiles.cs

namespace O2.Core.FileViewers.Ascx
{
	public class ascx_Struts_config_xml_Test 
	{
		public void launch() 
		{
			var strutsMappings = "ascx_Struts_config_xml".popupWindow<ascx_Struts_config_xml>(1200,600)
							     			    	  	 .insert_LogViewer();	
		}
	}

    public partial class ascx_Struts_config_xml : UserControl
    {
        private string loadedFile = "";

        public void onLoad()
        {

        }

        public ascx_Struts_config_xml()
        {
            InitializeComponent();
        }

        private void btMapLoadedFile_Click(object sender, EventArgs e)
        {
            maploadedFile();
        }

        public void maploadedFile()
        {
            mapFile(loadedFile);
        }

        public void mapFile(string fileToMap)
        {
            if (File.Exists(fileToMap))
            {
                loadedFile = fileToMap;
                var strutsConfigXml = J2eeConfigFiles.getStrutsConfig(fileToMap);
                tableList_FormBeans.setDataTable(CreateDataTable_Local.fromGenericList(strutsConfigXml.formBeans));
                tableList_GlobalForwards.setDataTable(CreateDataTable.fromDictionary_StringString(strutsConfigXml.globalForwards, "name", "path"));
                tableList_ActionMappings.setDataTable(CreateDataTable_Local.fromGenericList(strutsConfigXml.actionmappings));
                tableList_PlugIns.setDataTable(CreateDataTable_Local.fromGenericList(strutsConfigXml.plugIns));
                //lbDescription.Text = webXml.description;
                //lbDisplayName.Text = webXml.displayName;
                //tableList_Filter.setDataTable(CreateDataTable.fromGenericList(webXml.filters));
                //tableList_FilterMappings.setDataTable(CreateDataTable.fromGenericList(webXml.filterMappings));
                //tableList_Servlets.setDataTable(CreateDataTable.fromGenericList(webXml.servlets));
                //tableList_ServletMappings.setDataTable(CreateDataTable.fromGenericList(webXml.servletMappings));        
            }            
        }

        

        private void tvMappings_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void tvMappings_DragDrop(object sender, DragEventArgs e)
        {
            mapFile(Dnd.tryToGetFileOrDirectoryFromDroppedObject(e));
        }

        private void ascx_J2EE_web_xml_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        private void _onTableListDrop(DragEventArgs e)
        {
            handleDrop(e);
        }    

        private void handleDrop(DragEventArgs e)
        {
            var file = Dnd.tryToGetFileOrDirectoryFromDroppedObject(e);
            mapFile(file);
        }


       
    }
}
