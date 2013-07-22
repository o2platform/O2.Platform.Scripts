// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_TextSearch.Designer.cs

namespace O2.Tool.SearchEngine.Ascx
{
    public partial class ascx_TextSearch : UserControl
    {
        // move into DI class
        
        public ascx_TextSearch()
        {
            InitializeComponent();
        }

        

       /* private void tbTextToSearch1_TextChanged(object sender, EventArgs e)
        {
        }

        private void tbTextToSearch1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter && tbTextToSearch1.Text != "")
            {
                executeSearchAndLoadResults();

                //List<String> lSearchItems =  new List<String>();
                //lSearchItems.Add(tbTextToSearch1.Text);
                //textSearch.executeSearch(lSearchItems, this.dLoadedFilesCache);
            }
        }*/

        private void tbTextToSearch2_TextChanged(object sender, EventArgs e)
        {
        }

        private void tbTextToSearch2_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
                   

       

        private void ascx_TextSearch_Load(object sender, EventArgs e)
        {
            
        }

        private void tbFilesToLoad_TextChanged(object sender, EventArgs e)
        {
        }

       

       

        private void lbLoadedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

      

      /* private void lbLoadedFiles_DoubleClick(object sender, EventArgs e)
        {
            String sFile = lbLoadedFiles.Text;
            dLoadedFilesCache.Remove(sFile);
            TextSearch.loadListBoxWithListOfFilesLoaded(lbLoadedFiles, dLoadedFilesCache);
        }*/

        private void btRemoveAllLoadedFiles_Click(object sender, EventArgs e)
        {
          
        }

        public void setSearchStrings(String sSearchString1, String sSearchString2)
        {
            tbTextToSearch1.Text = sSearchString1;
            tbTextToSearch2.Text = sSearchString2;
        }

       /**/

        private void cbLoadXmlAsAssessmentRun_CheckedChanged(object sender, EventArgs e)
        {
        }

       

        

        

        private void lbLoadedFiles_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }
    }
}
