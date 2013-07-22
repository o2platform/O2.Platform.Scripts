// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using System.IO;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Utils;

//O2File:ascx_SearchTargets.cs
//O2File:ascx_SearchTargets.Designer.cs 
//O2File:SearchEngineGui.cs

namespace O2.Tool.SearchEngine.Ascx
{
    public partial class ascx_SearchTargets
    {
        private bool runOnLoad = true;        

        private void onLoad()
        {
            if (false == DesignMode && runOnLoad)
            {
                //ascx_DropObject1.setText("file to load");
                tbFilesToLoad.Text = PublicDI.config.O2TempDir;
                SearchEngineGui.searchEngineAPI.clearLoadedFiles();
                runOnLoad = false;
            }
        }

        

        private void LoadFiles()
        {
            SearchEngineGui.searchEngineAPI.loadAllFilesFromDirectoryThatMatchExtension(tbFilesToLoad.Text, tbFilesToLoad_Extension.Text,
                                                                   cbLoadFileMode_RecursiveSearch.Checked);
                                                                   //cbLoadXmlAsAssessmentRun.Checked);
            SearchEngineGui.searchEngineAPI.loadListBoxWithListOfFilesLoaded(lbLoadedFiles);
        }

        public bool loadFiles(List<string> filesToLoad)
        {
            if (filesToLoad != null)
            {
                int numberOfFilesToLoad = filesToLoad.Count;
                SearchEngineGui.searchEngineAPI.loadFiles(filesToLoad,
                    (numberOfFilesLoaded =>
                        this.invokeOnThread(() => 
                            lbNumberOfFilesLoaded.Text = string.Format("{0} / {1}", numberOfFilesLoaded, numberOfFilesToLoad))));
                refreshListOfLoadedFiles();
                return true;
            }
            return false;
        }

        public bool loadFile(String fileToLoad)
        {
            if (fileToLoad != null && File.Exists(fileToLoad))
            {
                SearchEngineGui.searchEngineAPI.loadFiles(new List<String>(new[] { fileToLoad }));
                refreshListOfLoadedFiles();
                return true;
            }
            return false;

        }

        private void refreshListOfLoadedFiles()
        {
            SearchEngineGui.searchEngineAPI.loadListBoxWithListOfFilesLoaded(lbLoadedFiles);
            this.invokeOnThread(
                () =>
                    lbNumberOfFilesLoaded.Text = string.Format("There are {0} Files Loaded", SearchEngineGui.searchEngineAPI.dLoadedFilesCache.Keys.Count));
        }

        public void setLoadXmlAsAssessmentRun(bool bValue)
        {
            cbLoadXmlAsAssessmentRun.Checked = bValue;
        }

        private void handleDrop(DragEventArgs e)
        {
            O2Thread.mtaThread(
                () =>
                    {
                        this.invokeOnThread(() => lbLoadDroppedFiles.Visible = true);
                        if (!loadFiles((List<string>) Dnd.tryToGetObjectFromDroppedObject(e, typeof (List<string>))))
                            loadFile((string) Dnd.tryToGetObjectFromDroppedObject(e, typeof (string)));
                        this.invokeOnThread(() => lbLoadDroppedFiles.Visible = false);
                    });
        }

        private void removeFiles(List<String> filesToRemove)
        {
            foreach (String sFile in filesToRemove)
                SearchEngineGui.searchEngineAPI.dLoadedFilesCache.Remove(sFile);
            SearchEngineGui.searchEngineAPI.loadListBoxWithListOfFilesLoaded(lbLoadedFiles);
        }

        private void reloadCurrentFiles()
        {
            SearchEngineGui.searchEngineAPI.dLoadedFilesCache = new Dictionary<string, List<string>>();
            SearchEngineGui.searchEngineAPI.loadFiles(getListOfLoadedFiles());      
        }

        private List<string> getListOfLoadedFiles()
        {
            var loadedFiles = new List<string>();
            foreach (var fileToRemove in lbLoadedFiles.Items)
                loadedFiles.Add(fileToRemove.ToString());
            return loadedFiles;
        }
    }
}
