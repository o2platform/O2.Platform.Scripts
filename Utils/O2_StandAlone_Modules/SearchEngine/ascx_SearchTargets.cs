// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_SearchTargets.Controllers.cs
//O2File:ascx_SearchTargets.Designer.cs 

namespace O2.Tool.SearchEngine.Ascx
{
    public partial class ascx_SearchTargets : UserControl
    {
        public ascx_SearchTargets()
        {
            InitializeComponent();
        }
        
        private void tbFilesToLoad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && tbFilesToLoad.Text != "")
            {
                LoadFiles();
            }
        }

        private void tbFilesToLoad_Extension_KeyPress(object sender, KeyPressEventArgs e)
        {
            tbFilesToLoad_KeyPress(sender, e);
        }

        private void btLoad_Click(object sender, EventArgs e)
        {
            LoadFiles();
        }

        private void lbLoadedFiles_DragDrop(object sender, DragEventArgs e)
        {
            handleDrop(e);
        }

        private void llRemoveLoadedFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            removeFiles(getListOfLoadedFiles());
        }
        

        private void ascx_SearchTargets_Load(object sender, EventArgs e)
        {
            onLoad();
        }
        

        private void lbLoadedFiles_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void lbLoadedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbOpenFileOnSelect.Checked)
            {
                var selectedFile = lbLoadedFiles.Text;
                O2Messages.fileOrFolderSelected(selectedFile);
            }
        }

        private void llReloadFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            reloadCurrentFiles();
        }
        

        private void llRemoveSelectedFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (lbLoadedFiles.SelectedItems != null)
            {
                var filesToRemove = new List<string>();
                foreach (var fileToRemove in lbLoadedFiles.SelectedItems)
                    filesToRemove.Add(fileToRemove.ToString());
                removeFiles(filesToRemove);
            }
        }

        private void cbOpenFileOnSelect_CheckedChanged(object sender, EventArgs e)
        {
            lbLoadedFiles.SelectionMode = (cbOpenFileOnSelect.Checked)  ? SelectionMode.One : SelectionMode.MultiExtended;
            llRemoveSelectedFiles.Visible = !cbOpenFileOnSelect.Checked;
        }

        private void btRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            refreshListOfLoadedFiles();
        }

    }
}
