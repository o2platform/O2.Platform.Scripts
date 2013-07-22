// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using FluentSharp.WinForms.Utils;
using O2.Scanner.MsCatNet.Converter;
using O2.Scanner.MsCatNet.Scan;
//O2File:Convert.cs
//O2File:CatNetConverter.cs
//O2File:MsCatNetScanner.cs
//O2File:ascx_MsCatNet.Designer.cs
//O2File:ScanTarget_CatNet.cs
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.Scanner.MsCatNet.Ascx
{
	public class Test_ascx_MsCatNet
	{
		public void testControl()
		{
			"ascx_MsCatNet".popupWindow().add_Control<ascx_MsCatNet>();
		}
	}
    public partial class ascx_MsCatNet : UserControl
    {
        public static string dotNetFrameworkFiles =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),
                         @"..\Microsoft.NET\Framework\v2.0.50727");

        public static string dotNetTempASPNETFiles = Path.Combine(dotNetFrameworkFiles, "Temporary ASP.NET Files");

        private readonly List<IScanTarget> scanTargetsQueue = new List<IScanTarget>();

        public ascx_MsCatNet()
        {
            InitializeComponent();
            
            var imageList = tvTargetFiles.imageList();
            imageList.Images.Add("folder", "folder".formImage());
            imageList.Images.Add("target", "application_x_executable".formImage());
            
            

            if (DesignMode == false)
            {
                tbPathToCatNetExecutable.Text = MsCatNetConfig.pathToCatCmdExe;
                checkIfCatNetScannerIsAvailable();
                tbPathToCatNetExecutable.Text = MsCatNetConfig.pathToCatCmdExe;
                //MsCatNetConfig.populateControlWithSupportLinks(supportLinks, webAutomation);

                //tcCatNetScanner.SelectedTab = tpScanDllOrWebsite;
                string defaultDirectoryWithOzasmtConversions = Path.Combine(PublicDI.config.O2TempDir,
                                                                            "MsCatNet to Ozasmt files");

                directoryWithCreatedOzasmtFiles.openDirectory(defaultDirectoryWithOzasmtConversions);
            }
        }

        private void lbDragAndDropHelpText_DragDrop(object sender, DragEventArgs e)
        {
            processDroppedFileorFolder(e);
        }


        private void lbDragAndDropHelpText_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }


        private void processDroppedFileorFolder(DragEventArgs e)
        {
            string fileOrFolder = Dnd.tryToGetFileOrDirectoryFromDroppedObject(e);
            if (!processFolder(fileOrFolder, cbOnFolderDropSearchRecursively.Checked))
                processFile(fileOrFolder);
        }

        private void processFile(string fileToProcess)
        {
            processFile(fileToProcess, "");
        }

        private void processFile(string fileToProcess, string workDirectory)
        {
            if (taskHostControl.InvokeRequired)
                taskHostControl.Invoke(new EventHandler(delegate { processFile(fileToProcess); }));
            else
            {
                if (File.Exists(fileToProcess))
                {
                    if (Path.GetExtension(fileToProcess) == ".zip")
                    {
                        //fileToProcess.unzip_File(workDirectory);
                        //TaskUtils.unzipFileAndInvokeCallback(fileToProcess, taskHostControl, processFile);
                    }
                    else
                    { 
                        switch (Path.GetExtension(fileToProcess))
                        {
                            case ".dll":
                            case ".exe":
                                if (workDirectory == "")
                                    addScanTargetToTreeView(new ScanTarget_CatNet { Target = fileToProcess });
                                else
                                    addScanTargetToTreeView(new ScanTarget_CatNet
                                                                {
                                                                    WorkDirectory = workDirectory,
                                                                    useFileNameOnWorkDirecory = false,
                                                                    Target = fileToProcess
                                                                });
                                break;
                            default:
                                PublicDI.log.error("In processFile: File type not supported by CatNetScanner: {0} ",
                                               Path.GetExtension(fileToProcess));
                                break;
                        }
                    }
                }
            }
        }


        private bool processFolder(string folderPath, bool recursiveSearch)
        {
            return processFolder(folderPath, Path.GetFileNameWithoutExtension(folderPath), recursiveSearch);
        }

        private bool processFolder(string folderPath, string folderName, bool recursiveSearch)
        {
            if (Directory.Exists(folderPath))
            {
                //     tvTargetFiles.Nodes.Clear();
                var targetFiles = new List<String>();
                targetFiles.AddRange(Files.getFilesFromDir_returnFullPath(folderPath, "*.dll", recursiveSearch));
                targetFiles.AddRange(Files.getFilesFromDir_returnFullPath(folderPath, "*.exe", recursiveSearch));
                //  var scanTargets = new List<IScanTarget>();
                //var folderName = "O2 Assemblies";
                string workDirectory = Path.Combine(PublicDI.config.O2TempDir, folderName);

                foreach (string file in targetFiles)
                    processFile(file, workDirectory);
                //scanTargets.Add(new ScanTarget_CatNet { WorkDirectory = workDirectory, useFileNameOnWorkDirecory = false, Target = file });
                //  addScanTargetsToTreeViewAsFolder(scanTargets, folderName);

                directoryWithScanCatNetResults.openDirectory(workDirectory);

                /*   foreach (var file in Files.getFilesFromDir_returnFullPath(fileOrFolder))
                    processFile(file);*/
            }
            return false;
        }

        private void llDownloadDemoFile_HacmeBank_WebServices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WebRequests.downloadFileUsingAscxDownload(O2CoreResources.DemoScanTarget_Dll_Hacmebank_WebService,
            //                                          processFile);
        }

        private void llDownloadDemoFile_HacmeBank_Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WebRequests.downloadFileUsingAscxDownload(O2CoreResources.DemoScanTarget_Dll_Hacmebank_WebSite, processFile);
        }

        private void checkIfCatNetScannerIsAvailable()
        {
            bool catNetAvailable = MsCatNetConfig.isCatScannerAvailable();
            lbCantFindCatNetExecutable.Visible = ! catNetAvailable;
            btInstallCatNet.Enabled = !catNetAvailable;
            btCompileAspNet.Enabled = false; //catNetAvailable;
            btScanDllOrWebSite.Enabled = catNetAvailable;
            btConvertCatNetFindingsFile.Enabled = catNetAvailable;
            btCatNetDocumentation.Enabled = false; // catNetAvailable;

            tbPathToCatNetExecutable.BackColor = (catNetAvailable) ? Color.LightGreen : Color.LightPink;
        }

        private void tbPathToCatNetExecutable_TextChanged(object sender, EventArgs e)
        {
            MsCatNetConfig.pathToCatCmdExe = tbPathToCatNetExecutable.Text;
            checkIfCatNetScannerIsAvailable();
        }

        private void tbCatNetShell_CommandToExecute_TextChanged(object sender, EventArgs e)
        {
        }

        private void tbCatNetShell_CommandToExecute_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
                executeCatNetCommand(tbCatNetShell_CommandToExecute.Text, callbackCatNetLogEvent_rtbCatNetShell_Log);
        }

        public void executeCatNetCommand(string commandToExecute, Callbacks.dMethod_String callbackCatNetLogEvent)
        {
            if (commandToExecute != "")
            {
                new MsCatNetScanner().startCatNetProcessWithParameters(commandToExecute, callbackCatNetLogEvent);
                    //, callbackCatNetScanComplete);
            }
        }


        public void callbackCatNetLogEvent_rtbCatNetShell_Log(string text)
        {
            if (rtbCatNetShell_Log.okThread(delegate { callbackCatNetLogEvent_rtbCatNetShell_Log(text); }))
                rtbCatNetShell_Log.Text = text + Environment.NewLine + rtbCatNetShell_Log.Text;
            //rtbCatNetShell_Log.AppendText(text + Environment.NewLine);                
        }

        public void callbackCatNetLogEvent_rtbLogFileForCurrentScan(string text)
        {
            if (rtbLogFileForCurrentScan.okThread(delegate { callbackCatNetLogEvent_rtbLogFileForCurrentScan(text); }))
                rtbLogFileForCurrentScan.Text = text + Environment.NewLine + rtbLogFileForCurrentScan.Text;
        }

        public void callbackCatNetScanComplete(object text)
        {
            //EventHandler target = delegate { () => callbackCatNetScanComplete(text); };
            //rtbLogFileForCurrentScan.Invoke(new EventHandler(target));           
            if (rtbLogFileForCurrentScan.okThread(delegate { callbackCatNetScanComplete(text); }))
            {
                 PublicDI.log.info("Process ended");
            }
        }

        private void tbDllToScan_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void tbDllToScan_DragDrop(object sender, DragEventArgs e)
        {
            tbDllToScan.Text = Dnd.tryToGetFileOrDirectoryFromDroppedObject(e);
            new MsCatNetScanner().executeCatNet(tbDllToScan.Text, callbackCatNetLogEvent_rtbCatNetShell_Log, null, false);
        }

        private void llUninstallCatNet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }


        private void llPopulateLinks_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }


/*        private void llLinkO2Website_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webAutomation.open("http://www.o2-ounceopen.com");
        }

        private void llLinkGoogle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            webAutomation.open("http://www.google.com");
        }*/

        private void btScanDllOrWebSite_Click(object sender, EventArgs e)
        {
            tcCatNetScanner.SelectedTab = tpScanDllOrWebsite;
        }

        private void btInstallCatNet_Click(object sender, EventArgs e)
        {
            tcCatNetScanner.SelectedTab = tpCatNetInstall;
        }

        private void btCompileAspNet_Click(object sender, EventArgs e)
        {
        //    tcCatNetScanner.SelectedTab = tpCompileWebsite;
        }

        private void btCatNetDocumentation_Click(object sender, EventArgs e)
        {
        //    tcCatNetScanner.SelectedTab = tpCatNetDocumentation;
        }

        private void btConvertCatNetFindingsFile_Click(object sender, EventArgs e)
        {
            tcCatNetScanner.SelectedTab = tpConvertCatNetOzasmt;
        }

        private void btRegMsHtmlDll_Click(object sender, EventArgs e)
        {
        }

        private void llDemoFiles_AddO2Assemblies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            processFolder(PublicDI.config.CurrentExecutableDirectory, "O2 Assemblies", false);
        }

        private void llDemoFiles_AddDotNetAssemblies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadAsScanTarget_DotNetAssemblies();
        }

        private void loadAsScanTarget_DotNetAssemblies()
        {
            processFolder(dotNetFrameworkFiles, "DotNet Frameworkd Assemblies", false);
        }

        private void llDemoFiles_TempASPNETFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            processFolder(dotNetTempASPNETFiles, "DotNet Temp ASP.NET Assemblies", true);
        }

        /*    private void addScanTargetsToTreeViewAsFolder(List<IScanTarget> scanTargets, string folderName)
        {
            foreach (var scanTarget in scanTargets)
                addScanTargetToTreeView(scanTarget);
                
            //    tvTargetFiles.Nodes.Add(file);
            //tvTargetFiles.Nodes.Add(scanTarget.ToString());
        }*/

        private void addScanTargetToTreeView(IScanTarget scanTarget)
        {
            if (lbDragAndDropHelpText.Visible)
                lbDragAndDropHelpText.Visible = false;
            O2Forms.newTreeNode(tvTargetFiles.Nodes, Path.GetFileName(scanTarget.Target), scanTarget.Target, 2,
                                scanTarget);
            llNumberOfAssembliesLoaded.Text = tvTargetFiles.Nodes.Count + " Assemblies Loaded";
        }

        private void tvTargetFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void tvTargetFiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (tvTargetFiles.SelectedNode.Tag != null && tvTargetFiles.SelectedNode.Tag is IScanTarget)
                runScan((IScanTarget) tvTargetFiles.SelectedNode.Tag);
        }

        private void btScanUsingCatNetScanner_Click(object sender, EventArgs e)
        {
            if (tvTargetFiles.SelectedNode == null && tvTargetFiles.Nodes.Count > 1)
                tvTargetFiles.SelectedNode = tvTargetFiles.Nodes[0];
            if (tvTargetFiles.SelectedNode != null && tvTargetFiles.SelectedNode.Tag != null &&
                tvTargetFiles.SelectedNode.Tag is IScanTarget)
                runScan((IScanTarget) tvTargetFiles.SelectedNode.Tag);
        }

        public void runScan(IScanTarget scanTarget)
        {
            rtbLogFileForCurrentScan.Text = " \n\n STARTING SCAN FOR:" + scanTarget + "\n";
            directoryWithScanCatNetResults.openDirectory(scanTarget.WorkDirectory);
            new MsCatNetScanner().scan(scanTarget, callbackCatNetLogEvent_rtbLogFileForCurrentScan, null, false
                /*convertToOzasmt*/);
        }

        private void directoryWithScanCatNetResults__onFileWatchEvent(FolderWatcher folderWatcher)
        {
            convertCatNetFileIntoOzasmt(folderWatcher.file, directoryWithCreatedOzasmtFiles.getCurrentDirectory());
        }

        public void convertCatNetFileIntoOzasmt(string file, string targetFolder)
        {
            if (File.Exists(file) && Path.GetExtension(file) == ".xml")
            {
                string ozamstFileToCreate = Path.Combine(targetFolder,
                                                         Path.GetFileNameWithoutExtension(file) + ".ozasmt");
                new CatNetConverter(file).convert(ozamstFileToCreate);
            }
        }

        private void directoryWithCreatedOzasmtFiles_eDirectoryEvent_DoubleClick(string fileSelected)
        {
            PublicDI.log.debug("Opening selected ozasmt file in a new window: {0}", fileSelected);
			ascx_FindingsViewer.openInFloatWindow(fileSelected);
        }

        private void directoryWithCreatedOzasmtFiles__onTreeViewDrop(string fileDropped)
        {
            convertCatNetFileIntoOzasmt(fileDropped, directoryWithCreatedOzasmtFiles.getCurrentDirectory());
        }

        private void btScanAllLoadedTargets_Click(object sender, EventArgs e)
        {
            if (scanTargetsQueue.Count > 0)
            {
                string abortMessage =
                    string.Format(
                        "ABORTING SCAN QUEUE, there where {0} targets skipped (note that the current scan will not the terminated)",
                        scanTargetsQueue.Count);
                callbackCatNetLogEvent_rtbLogFileForCurrentScan(abortMessage + Environment.NewLine);
                PublicDI.log.debug(abortMessage);
                scanTargetsQueue.Clear();
                btScanAllLoadedTargets.Text = "Scan all Loaded targets";
            }
            else
            {
                foreach (TreeNode treeNode in tvTargetFiles.Nodes)
                    scanTargetsQueue.Add((IScanTarget) treeNode.Tag);
                if (scanTargetsQueue.Count > 0)
                {
                    processScanTargetQueueItem();
                    btScanAllLoadedTargets.Text = "Abort Scan";
                }
            }
        }

        private void processScanTargetQueueItem()
        {
            if (scanTargetsQueue.Count > 0)
            {
                PublicDI.log.info("Using Scan Queue. There are {0} scans on the queue", scanTargetsQueue.Count);
                IScanTarget scanTarget = scanTargetsQueue[0];
                directoryWithScanCatNetResults.openDirectory(scanTarget.WorkDirectory);
                callbackCatNetLogEvent_rtbLogFileForCurrentScan("\n STARTING QUEUED SCAN FOR:" + scanTarget.Target +
                                                                Environment.NewLine);

                new MsCatNetScanner().scan(scanTarget, callbackCatNetLogEvent_rtbLogFileForCurrentScan,
                                           delegate { processScanTargetQueueItem(); }
                                           , false);
                scanTargetsQueue.Remove(scanTarget);
                if (cbAfterScanRemoveFromScanTargets.Checked)
                    removeScanTargetFromTreeView(scanTarget, tvTargetFiles);
            }
        }

        private void removeScanTargetFromTreeView(IScanTarget scanTarget, TreeView targetTreeView)
        {
            if (targetTreeView.okThread(delegate { removeScanTargetFromTreeView(scanTarget, targetTreeView); }))
            {
                TreeNode treeNodeWithScanTarget = tvTargetFiles.Nodes[scanTarget.Target];
                if (treeNodeWithScanTarget != null)
                    targetTreeView.Nodes.Remove(treeNodeWithScanTarget);
            }
        }

        private void tpCompileWebsite_Click(object sender, EventArgs e)
        {
        }

        private void directoryToDropCatNetFilesForConversion__onTreeViewDrop(string fileDropped)
        {
            convertCatNetFileIntoOzasmt(fileDropped, directoryToDropCatNetFilesForConversion.getCurrentDirectory());
        }

        private void tvTargetFiles_DragDrop(object sender, DragEventArgs e)
        {
            processDroppedFileorFolder(e);
        }

        private void tvTargetFiles_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void btScanDllOrWebSite_DragDrop(object sender, DragEventArgs e)
        {
            tcCatNetScanner.SelectedTab = tpScanDllOrWebsite;
            processDroppedFileorFolder(e);
        }

        private void btScanDllOrWebSite_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void llClearScanQueue_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tvTargetFiles.Nodes.Clear();
        }

    }
}
