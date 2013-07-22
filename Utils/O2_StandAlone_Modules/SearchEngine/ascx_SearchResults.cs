// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_SearchResults.Controllers.cs
//O2File:ascx_SearchResults.Designer.cs 

namespace O2.Tool.SearchEngine.Ascx
{
    public partial class ascx_SearchResults : UserControl
    {
        public ascx_SearchResults()
        {
            InitializeComponent();
        }

        private void btExecuteSearch_Click(object sender, EventArgs e)
        {            
            executeSearchAndLoadResults();
        }

       /* private void dgvSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvSearchResults_SelectionChanged(null, null);
        }*/

        private void dgvSearchResults_SelectionChanged(object sender, EventArgs e)
        {
            if (cbOpenSelectedItemInMainGUIWindow.Checked && dgvSearchResults.SelectedRows.Count == 1)
            {
                var tsrSearchResult = (TextSearchResult)dgvSearchResults.SelectedRows[0].Tag;
                if (tsrSearchResult != null)
                {                    
                    O2Messages.fileOrFolderSelected(tsrSearchResult.sFile, tsrSearchResult.iLineNumber + 1);
                    //asceSourceCodeEditor.gotoLine(tsrSearchResult.sFile, tsrSearchResult.iLineNumber + 1);                    

                    
                    O2Thread.mtaThread(
                    () =>
                        {
                            Thread.Sleep(200);
                            var searchResultsForm = O2Forms.findParentThatHostsControl(this);
                            //var searchResultsForm = O2DockUtils.getO2DockContentForm("Search Results");
                            searchResultsForm.invokeOnThread(
                                () =>
                                    {
                                        searchResultsForm.Focus();
                                        dgvSearchResults.Focus();
                                    });
                           // DI.dO2LoadedO2DockContent[name].dockContent.invokeOnThread(
                           //     () => { DI.dO2LoadedO2DockContent[name].dockContent.Focus(); });
                        });
                    
                }
            }
        }

        private void dgvSearchCriteria_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            runSearch();
        }

        private void dgvSearchCriteria_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                setNewRowDefaultValues(i);
            }
        }

        private void ascx_SearchResults_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        private void tbSingleSearch_KeyDown(object sender, KeyEventArgs e)
        {
            var currentSearchRegEx = tbSingleSearch.Text;            
            if (e.KeyCode == Keys.Enter)
                searchFor(currentSearchRegEx);
        }

        private void tbSingleSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            var currentSearchRegEx = tbSingleSearch.Text;
            if (null == RegEx.createRegEx(currentSearchRegEx))
                tbSingleSearch.BackColor = Color.LightSalmon;
            else
                tbSingleSearch.BackColor = Color.White;

        }

        private void tbSingleSearch_TextChanged(object sender, EventArgs e)
        {
            dgvSearchCriteria.Rows.Clear();
        }

        private void llClearSelected_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach(DataGridViewRow selectedRow in dgvSearchCriteria.SelectedRows)
                dgvSearchCriteria.Rows.Remove(selectedRow);
        }

        private void llRunMultipleSearches_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            runSearch();
        }

        private void dgvSearchResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvSearchResults.Rows.Count == 1)       // handle the case where there is only one finding
                dgvSearchResults_SelectionChanged(null, null);
        }

        private void tcSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            showResults();
        }

        private void llRefreshSearchResultsView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            showResults();
        }

        private void tvSearchResults_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var currentNode = e.Node;
            currentNode.Nodes.Clear();
            if (currentNode.Tag != null & currentNode.Tag is List<TextSearchResult>)
            {
                var currentFilter = currentNode.Name;
                var childNodes = SearchUtils.getNodesForTreeViewSearchResults(currentFilter, "",
                                                                              (List<TextSearchResult>) currentNode.Tag);
                currentNode.Nodes.AddRange(childNodes.ToArray());
            }
        }

       

 

    

 

      

       
    }
}
