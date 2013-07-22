// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_SearchResults.Controllers.cs
//O2File:ascx_SearchResults.cs 
//O2File:SearchEngineGui.cs

namespace O2.Tool.SearchEngine.Ascx
{
    public partial class ascx_SearchResults
    {
        private List<TextSearchResult> currentSearchResults = new List<TextSearchResult>();

        private bool runOnLoad = true;

        public void onLoad()
        {
            if (DesignMode == false && runOnLoad)
            {
                setNewRowDefaultValues(0);
                runOnLoad = false;
                loadFilterTypesInComboBox(cbTreeView_FilterType1);
                loadFilterTypesInComboBox(cbTreeView_FilterType2);
                loadFilterTypesInComboBox(cbTreeView_FilterType3);
                loadFilterTypesInComboBox(cbTreeView_FilterType4);
                cbTreeView_FilterType1.SelectedIndex = 0;
                cbTreeView_FilterType2.SelectedIndex = 1;
                cbTreeView_FilterType3.SelectedIndex = 2;
            }
        }

        public void loadFilterTypesInComboBox(ComboBox comboBox)
        {
            comboBox.Items.AddRange(new object[]{"directory","file","matched text", "line"});
        }

        public void executeSearchAndLoadResults()
        {
            executeSearchAndLoadResults(calculateSearchRegEx());
        }

        public void executeSearchAndLoadResults(string searchRegEx)
        {
            executeSearchAndLoadResults(new List<Regex>{new Regex(searchRegEx)});
        }

        public void executeSearchAndLoadResults(List<Regex> searchRegExes)
        {
            this.invokeOnThread(() => lbSearchResultsStats.Text = "Executing search");
            // execute search
            var timer = new O2Timer("Search execution").start();
            var tsrSearchResults = executeSearch(searchRegExes);
            timer.stop();
                        
            this.invokeOnThread(
                () => lbSearchResultsStats.Text =
                        String.Format("{0} and returned {1} matches", timer.TimeSpanString, tsrSearchResults.Count));

            // show Results
            showResults(tsrSearchResults);
        }

        public List<TextSearchResult> executeSearch(string searchRegEx)
        {
            return executeSearch(new List<Regex> {new Regex(searchRegEx)});
        }

        public List<TextSearchResult> executeSearch(List<Regex> searchRegExes)
        {            
            List<TextSearchResult> tsrSearchResults = SearchEngineGui.searchEngineAPI.executeSearch(searchRegExes);            
            return tsrSearchResults;
        }

        public void showResults(List<TextSearchResult> tsrSearchResults)
        {
            currentSearchResults = tsrSearchResults;
            showResults();
        }    

        public void showResults()
        {
            if (tcSearchResults.SelectedTab == tpDataGridView)                           
                SearchUtils.loadInDataGridView_textSearchResults(currentSearchResults, dgvSearchResults);
            else if (tcSearchResults.SelectedTab == tpTreeView)                          
                SearchUtils.loadInTreeView_textSearchResults(
                    currentSearchResults, tvSearchResults,
                    cbTreeView_FilterType1.Text, tbTreeView_FilterText1.Text,
                    cbTreeView_FilterType2.Text, tbTreeView_FilterText2.Text,
                    cbTreeView_FilterType3.Text, tbTreeView_FilterText3.Text,
                    cbTreeView_FilterType4.Text, tbTreeView_FilterText4.Text);
            else if (tcSearchResults.SelectedTab == tpTextView)                          
                SearchUtils.loadInTextBox_textSearchResults(currentSearchResults, tbSearchResults);

        }

        private List<Regex> calculateSearchRegEx()
        {
            var lreSearchRegEx = new List<Regex>();
            foreach (DataGridViewRow row in dgvSearchCriteria.Rows)
            {
                if (row.Cells.Count > 0)
                {
                    var criteriaText = row.Cells[0].Value;
                    var negativeSearch = (bool)(row.Cells[1].Value ?? false) ;
                    var searchItemEnabled = (bool)(row.Cells[2].Value ?? false);      
              
                    if (searchItemEnabled && criteriaText != null)
                    {
                        Regex reNewRegex = RegEx.createRegEx(criteriaText.ToString());
                        if (reNewRegex != null)
                            lreSearchRegEx.Add(reNewRegex);
                    }
                }
            }
            /*if (tbTextToSearch1.Text != "")
            {
                
            }

            if (tbTextToSearch2.Text != "")
            {
                Regex reNewRegex = RegEx.createRegEx(tbTextToSearch2.Text);
                if (reNewRegex != null)
                    lreSearchRegEx.Add(reNewRegex);
            }*/

            return lreSearchRegEx;
        }

        

        public void runSearch()
        {
            executeSearchAndLoadResults();
        }

        private void setNewRowDefaultValues(int rowId)
        {
            if (dgvSearchCriteria.Rows.Count > rowId)
            {
                dgvSearchCriteria.Rows[rowId].Cells[1].Value = false;
                dgvSearchCriteria.Rows[rowId].Cells[1].ReadOnly = true; // make it read only for now
                dgvSearchCriteria.Rows[rowId].Cells[2].Value = true;
            }
        }

        public void searchFor(string searchCriteria)
        {
            executeSearchAndLoadResults(searchCriteria);
        }

        public Dictionary<String, List<String>> getLoadedFilesCache()
        {
            return SearchEngineGui.searchEngineAPI.dLoadedFilesCache;
        }

        public void setOpenSelectedItemInMainGUIWindowCheckedState(bool value)
        {
            this.invokeOnThread(()=>cbOpenSelectedItemInMainGUIWindow.Checked = value );
        }

        public void addSearchCriteria(string searchCriteria)
        { 
            this.invokeOnThread(
                ()=> dgvSearchCriteria.Rows.Add(new object[] {searchCriteria}));
        }
    }
}
