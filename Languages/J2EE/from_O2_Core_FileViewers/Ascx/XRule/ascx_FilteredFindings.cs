// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using O2.Core.FileViewers.JoinTraces;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Interfaces.O2Findings;

namespace O2.Core.FileViewers.Ascx.O2Rules
{
    public partial class ascx_FilteredFindings : UserControl
    {

        public List<IO2Finding> findingsToFilter = new List<IO2Finding>();
        public Func<List<IO2Finding> ,List<IO2Finding>> MapJointPointsCallback { get; set;}

        public ascx_FilteredFindings()
        {
            InitializeComponent();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [Bindable(true)]
        public string _Title
        {
            get { return lbFilteredFindingTitle.Text; }
            set { lbFilteredFindingTitle.Text = value; }           
        }

        private void llCalculateFindings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            calculateFindings();
        }

        public void calculateFindings()
        {
            var results = O2FindingsHelpers.calculateFindings(findingsToFilter, tbSourceSignatures.Text, tbSinkSignatures.Text);


            if (MapJointPointsCallback != null)
                results = MapJointPointsCallback(results);

            findingsViewer_Results.loadO2Findings(results,true);
            tcFilteredFindings.invokeOnThread(() => tcFilteredFindings.SelectedTab = tpResults);            
        }

        public void setFindingsResult(List<IO2Finding> results)
        {
            findingsViewer_Results.loadO2Findings(results, true);
        }

        public void setFindingsToFilter(List<IO2Finding> _findingsToFilter)
        {
            findingsToFilter = _findingsToFilter;
            showFindingsToFilterDetails();
        }

        public void setSourceSignatureRegEx(string regexSignature)
        {            
            tbSourceSignatures.invokeOnThread(
                () =>
                    {
                        tbSourceSignatures.Text = regexSignature;
                        return true;
                    });

        }
        
        public void setSinkSignatureRegEx(string regexSignature)
        {            
            tbSinkSignatures.invokeOnThread(
             () =>
             {
                 tbSinkSignatures.Text = regexSignature;
                 return true;
             });            
        }
        
        public void setMapJointPointsCallback(Func<List<IO2Finding>, List<IO2Finding>> mapJointPointsCallback)
        {
            MapJointPointsCallback = mapJointPointsCallback;
        }


        private void showFindingsToFilterDetails()
        {
            tableList_LoadedFindingsDetails.setDataTable(O2FindingsHelpers.getDataTableWithFindingsDetails(findingsToFilter));
        }


        public List<IO2Finding> calculateFindings(string sourcesRegEx, string sinksRegEx, List<IO2Finding> _findingsToFilter)
        {
            return calculateFindings(sourcesRegEx, sinksRegEx, _findingsToFilter);
        }

        public List<IO2Finding> calculateFindings(string sourcesRegEx, string sinksRegEx, List<IO2Finding> _findingsToFilter, Func<List<IO2Finding>, List<IO2Finding>> mapJointPointsCallback)
        {
            setSourceSignatureRegEx(sourcesRegEx);
            setSinkSignatureRegEx(sinksRegEx);
            setFindingsToFilter(_findingsToFilter);
            MapJointPointsCallback = mapJointPointsCallback;
            calculateFindings();
            var results = getResults();            
            return results;
        }

        public void setFindingsViewerFilters(string filter1, string filter2)
        {
            findingsViewer_Results.setFilter1Value(filter1);
            findingsViewer_Results.setFilter2Value(filter2);
        }

        public List<IO2Finding> getResults()
        {
            return findingsViewer_Results.currentO2Findings;
        }

        private void tableList_LoadedFindingsDetails__onTableListDrop(DragEventArgs e)
        {
            findingsToFilter = (List<IO2Finding>)Dnd.tryToGetObjectFromDroppedObject(e, typeof(List<IO2Finding>));
            showFindingsToFilterDetails();
        }
    }
}
