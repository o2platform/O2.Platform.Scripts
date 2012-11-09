// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using O2.Core.FileViewers.J2EE;
using O2.DotNetWrappers.DotNet;
using O2.Core.FileViewers.ViewHelpers;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.Ascx
{
    public partial class ascx_J2EE_web_xml : UserControl
    {
        private string loadedFile = "";

        public void onLoad()
        {
            //tableList_Filter.setTableListTitle("Filter");
        }

        public ascx_J2EE_web_xml()
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
            loadedFile = fileToMap;
            var webXml = J2eeConfigFiles.getWebXml(fileToMap);
            this.invokeOnThread(()=> refreshView(webXml));            
        }

        private void refreshView(IWebXml webXml)
        {
            lbDescription.Text = webXml.description;
            lbDisplayName.Text = webXml.displayName;
            tableList_Filter.setDataTable(CreateDataTable_Local.fromGenericList(webXml.filters));
            tableList_FilterMappings.setDataTable(CreateDataTable_Local.fromGenericList(webXml.filterMappings));
            tableList_Servlets.setDataTable(CreateDataTable_Local.fromGenericList(webXml.servlets));
            tableList_ServletMappings.setDataTable(CreateDataTable_Local.fromGenericList(webXml.servletMappings));    
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
