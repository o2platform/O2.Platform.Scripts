// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
//using O2.Core.FileViewers.Struts_1_5;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_StrutsMappings.Designer.cs
//O2File:ascx_StrutsMappings.Controllers.cs

namespace O2.Core.FileViewers.Ascx
{ 
	public class ascx_StrutsMappings_Test 
	{
		public void launch() 
		{
			var strutsMappings = "ascx_StrutsMappings".popupWindow<ascx_StrutsMappings>(1200,600)
							     			    	  .insert_LogViewer();	
		}
	}
	 
    public partial class ascx_StrutsMappings : UserControl
    {
        
        public ascx_StrutsMappings()
        {
            InitializeComponent();                        
        }

        private void llTempRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            showStrutsMappings();
        }

        private void tvStrutsMappings_DragDrop(object sender, DragEventArgs e)
        {
            loadFileOrFolder(Dnd.tryToGetFileOrDirectoryFromDroppedObject(e));
        }
 
        private void tvStrutsMappings_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void tvStrutsMappings_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {

            if (e.Node != null && e.Node.Tag != null)
            {
                onTreeViewBeforeExpand(e.Node, e.Node.Tag);
            }
        }

               

        private void btOpenStrutsMappingsFile_Click(object sender, EventArgs e)
        {
            openMappingsFile();
        }

        private void btSaveCurrentMappings_Click(object sender, EventArgs e)
        {
            saveCurrentMappings(tbTargetDirectoryOrFolder.Text);
        }

        private void ascx_StrutsMappings_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        private void btRemoveLoadedMappings_Click(object sender, EventArgs e)
        {
            refreshTreeView(null);
        }

        
    }
}
