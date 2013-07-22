// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib.API;
using FluentSharp.WinForms.Controls;
//O2File:ascx_MsCatNet.cs

namespace O2.Scanner.MsCatNet.Ascx
{
    partial class ascx_MsCatNet
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ascx_MsCatNet));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.llDemoFiles_TempASPNETFolder = new System.Windows.Forms.LinkLabel();
            this.label22 = new System.Windows.Forms.Label();
            this.llDemoFiles_AddDotNetAssemblies = new System.Windows.Forms.LinkLabel();
            this.label21 = new System.Windows.Forms.Label();
            this.llDemoFiles_AddO2Assemblies = new System.Windows.Forms.LinkLabel();
            this.llDownloadDemoFile_HacmeBank_Website = new System.Windows.Forms.LinkLabel();
            this.llDownloadDemoFile_HacmeBank_WebServices = new System.Windows.Forms.LinkLabel();
            this.btScanUsingCatNetScanner = new System.Windows.Forms.Button();
            this.tcCatNetScanner = new System.Windows.Forms.TabControl();
            this.tbSplashPage = new System.Windows.Forms.TabPage();
            this.btCatNetDocumentation = new System.Windows.Forms.Button();
            this.btInstallCatNet = new System.Windows.Forms.Button();
            this.btConvertCatNetFindingsFile = new System.Windows.Forms.Button();
            this.btCompileAspNet = new System.Windows.Forms.Button();
            this.btScanDllOrWebSite = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.tpScanDllOrWebsite = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.llNumberOfAssembliesLoaded = new System.Windows.Forms.Label();
            this.llClearScanQueue = new System.Windows.Forms.LinkLabel();
            this.cbOnFolderDropSearchRecursively = new System.Windows.Forms.CheckBox();
            this.lbDragAndDropHelpText = new System.Windows.Forms.Label();
            this.tvTargetFiles = new System.Windows.Forms.TreeView();
            //this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cbAfterScanRemoveFromScanTargets = new System.Windows.Forms.CheckBox();
            this.btScanAllLoadedTargets = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label10 = new System.Windows.Forms.Label();
            this.rtbLogFileForCurrentScan = new System.Windows.Forms.RichTextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.directoryWithScanCatNetResults = new  DirectoryViewer();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.directoryWithCreatedOzasmtFiles = new DirectoryViewer();
            this.label3 = new System.Windows.Forms.Label();
            this.tpConvertCatNetOzasmt = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.directoryToDropCatNetFilesForConversion = new DirectoryViewer();
            this.tpCatNetInstall = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.supportLinks = new System.Windows.Forms.FlowLayoutPanel();
            this.lbCantFindCatNetExecutable = new System.Windows.Forms.Label();
            this.tbPathToCatNetExecutable = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            //this.webAutomation = new O2.External.Firefox.Ascx.WebAutomation.ascx_WebAutomation();
            this.tpConfig = new System.Windows.Forms.TabPage();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.label11 = new System.Windows.Forms.Label();
            this.ascx_Directory3 = new DirectoryViewer();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.ascx_Directory4 = new DirectoryViewer();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbDllToScan = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbCatNetShell_CommandToExecute = new System.Windows.Forms.TextBox();
            this.rtbCatNetShell_Log = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.taskHostControl = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.tcCatNetScanner.SuspendLayout();
            this.tbSplashPage.SuspendLayout();
            this.tpScanDllOrWebsite.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.tpConvertCatNetOzasmt.SuspendLayout();
            this.tpCatNetInstall.SuspendLayout();
            this.tpConfig.SuspendLayout();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.llDemoFiles_TempASPNETFolder);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.llDemoFiles_AddDotNetAssemblies);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.llDemoFiles_AddO2Assemblies);
            this.groupBox1.Controls.Add(this.llDownloadDemoFile_HacmeBank_Website);
            this.groupBox1.Controls.Add(this.llDownloadDemoFile_HacmeBank_WebServices);
            this.groupBox1.Location = new System.Drawing.Point(4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(280, 173);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            // 
            // llDemoFiles_TempASPNETFolder
            // 
            this.llDemoFiles_TempASPNETFolder.AutoSize = true;
            this.llDemoFiles_TempASPNETFolder.Location = new System.Drawing.Point(30, 55);
            this.llDemoFiles_TempASPNETFolder.Name = "llDemoFiles_TempASPNETFolder";
            this.llDemoFiles_TempASPNETFolder.Size = new System.Drawing.Size(228, 13);
            this.llDemoFiles_TempASPNETFolder.TabIndex = 45;
            this.llDemoFiles_TempASPNETFolder.TabStop = true;
            this.llDemoFiles_TempASPNETFolder.Text = ".Net Framework v2.x Temporary ASP.NET files";
            this.llDemoFiles_TempASPNETFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDemoFiles_TempASPNETFolder_LinkClicked);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(10, 16);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(165, 13);
            this.label22.TabIndex = 3;
            this.label22.Text = "Load demo targets from local disk";
            // 
            // llDemoFiles_AddDotNetAssemblies
            // 
            this.llDemoFiles_AddDotNetAssemblies.AutoSize = true;
            this.llDemoFiles_AddDotNetAssemblies.Location = new System.Drawing.Point(30, 78);
            this.llDemoFiles_AddDotNetAssemblies.Name = "llDemoFiles_AddDotNetAssemblies";
            this.llDemoFiles_AddDotNetAssemblies.Size = new System.Drawing.Size(160, 13);
            this.llDemoFiles_AddDotNetAssemblies.TabIndex = 44;
            this.llDemoFiles_AddDotNetAssemblies.TabStop = true;
            this.llDemoFiles_AddDotNetAssemblies.Text = ".Net Framework v2.x Assemblies";
            this.llDemoFiles_AddDotNetAssemblies.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDemoFiles_AddDotNetAssemblies_LinkClicked);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 110);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(152, 13);
            this.label21.TabIndex = 2;
            this.label21.Text = "Download Demo Scan targets:";
            // 
            // llDemoFiles_AddO2Assemblies
            // 
            this.llDemoFiles_AddO2Assemblies.AutoSize = true;
            this.llDemoFiles_AddO2Assemblies.Location = new System.Drawing.Point(30, 32);
            this.llDemoFiles_AddO2Assemblies.Name = "llDemoFiles_AddO2Assemblies";
            this.llDemoFiles_AddO2Assemblies.Size = new System.Drawing.Size(131, 13);
            this.llDemoFiles_AddO2Assemblies.TabIndex = 43;
            this.llDemoFiles_AddO2Assemblies.TabStop = true;
            this.llDemoFiles_AddO2Assemblies.Text = "O2 MsCatNet Assesmblies";
            this.llDemoFiles_AddO2Assemblies.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDemoFiles_AddO2Assemblies_LinkClicked);
            // 
            // llDownloadDemoFile_HacmeBank_Website
            // 
            this.llDownloadDemoFile_HacmeBank_Website.AutoSize = true;
            this.llDownloadDemoFile_HacmeBank_Website.Location = new System.Drawing.Point(30, 147);
            this.llDownloadDemoFile_HacmeBank_Website.Name = "llDownloadDemoFile_HacmeBank_Website";
            this.llDownloadDemoFile_HacmeBank_Website.Size = new System.Drawing.Size(146, 13);
            this.llDownloadDemoFile_HacmeBank_Website.TabIndex = 1;
            this.llDownloadDemoFile_HacmeBank_Website.TabStop = true;
            this.llDownloadDemoFile_HacmeBank_Website.Text = "HacmeBank  - WebSite (.net)";
            this.llDownloadDemoFile_HacmeBank_Website.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDownloadDemoFile_HacmeBank_Website_LinkClicked);
            // 
            // llDownloadDemoFile_HacmeBank_WebServices
            // 
            this.llDownloadDemoFile_HacmeBank_WebServices.AutoSize = true;
            this.llDownloadDemoFile_HacmeBank_WebServices.Location = new System.Drawing.Point(30, 126);
            this.llDownloadDemoFile_HacmeBank_WebServices.Name = "llDownloadDemoFile_HacmeBank_WebServices";
            this.llDownloadDemoFile_HacmeBank_WebServices.Size = new System.Drawing.Size(166, 13);
            this.llDownloadDemoFile_HacmeBank_WebServices.TabIndex = 0;
            this.llDownloadDemoFile_HacmeBank_WebServices.TabStop = true;
            this.llDownloadDemoFile_HacmeBank_WebServices.Text = "HacmeBank - WebServices (.net)";
            this.llDownloadDemoFile_HacmeBank_WebServices.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDownloadDemoFile_HacmeBank_WebServices_LinkClicked);
            // 
            // btScanUsingCatNetScanner
            // 
            this.btScanUsingCatNetScanner.Location = new System.Drawing.Point(10, 4);
            this.btScanUsingCatNetScanner.Name = "btScanUsingCatNetScanner";
            this.btScanUsingCatNetScanner.Size = new System.Drawing.Size(134, 41);
            this.btScanUsingCatNetScanner.TabIndex = 43;
            this.btScanUsingCatNetScanner.Text = "Scan selected target";
            this.btScanUsingCatNetScanner.UseVisualStyleBackColor = true;
            this.btScanUsingCatNetScanner.Click += new System.EventHandler(this.btScanUsingCatNetScanner_Click);
            // 
            // tcCatNetScanner
            // 
            this.tcCatNetScanner.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tcCatNetScanner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcCatNetScanner.Controls.Add(this.tbSplashPage);
            this.tcCatNetScanner.Controls.Add(this.tpScanDllOrWebsite);
            this.tcCatNetScanner.Controls.Add(this.tpConvertCatNetOzasmt);
            this.tcCatNetScanner.Controls.Add(this.tpCatNetInstall);
            this.tcCatNetScanner.Controls.Add(this.tpConfig);
            this.tcCatNetScanner.Location = new System.Drawing.Point(3, 3);
            this.tcCatNetScanner.Multiline = true;
            this.tcCatNetScanner.Name = "tcCatNetScanner";
            this.tcCatNetScanner.SelectedIndex = 0;
            this.tcCatNetScanner.Size = new System.Drawing.Size(899, 554);
            this.tcCatNetScanner.TabIndex = 45;
            // 
            // tbSplashPage
            // 
            this.tbSplashPage.Controls.Add(this.btCatNetDocumentation);
            this.tbSplashPage.Controls.Add(this.btInstallCatNet);
            this.tbSplashPage.Controls.Add(this.btConvertCatNetFindingsFile);
            this.tbSplashPage.Controls.Add(this.btCompileAspNet);
            this.tbSplashPage.Controls.Add(this.btScanDllOrWebSite);
            this.tbSplashPage.Controls.Add(this.label15);
            this.tbSplashPage.Location = new System.Drawing.Point(4, 4);
            this.tbSplashPage.Name = "tbSplashPage";
            this.tbSplashPage.Padding = new System.Windows.Forms.Padding(3);
            this.tbSplashPage.Size = new System.Drawing.Size(891, 528);
            this.tbSplashPage.TabIndex = 3;
            this.tbSplashPage.Text = "Splash Page";
            this.tbSplashPage.UseVisualStyleBackColor = true;
            // 
            // btCatNetDocumentation
            // 
            this.btCatNetDocumentation.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCatNetDocumentation.Location = new System.Drawing.Point(453, 80);
            this.btCatNetDocumentation.Name = "btCatNetDocumentation";
            this.btCatNetDocumentation.Size = new System.Drawing.Size(172, 155);
            this.btCatNetDocumentation.TabIndex = 6;
            this.btCatNetDocumentation.Text = "View Cat.Net Documentation";
            this.btCatNetDocumentation.UseVisualStyleBackColor = true;
            this.btCatNetDocumentation.Click += new System.EventHandler(this.btCatNetDocumentation_Click);
            // 
            // btInstallCatNet
            // 
            this.btInstallCatNet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btInstallCatNet.Location = new System.Drawing.Point(242, 80);
            this.btInstallCatNet.Name = "btInstallCatNet";
            this.btInstallCatNet.Size = new System.Drawing.Size(172, 155);
            this.btInstallCatNet.TabIndex = 4;
            this.btInstallCatNet.Text = "Install Cat.Net this computer";
            this.btInstallCatNet.UseVisualStyleBackColor = true;
            this.btInstallCatNet.Click += new System.EventHandler(this.btInstallCatNet_Click);
            // 
            // btConvertCatNetFindingsFile
            // 
            this.btConvertCatNetFindingsFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btConvertCatNetFindingsFile.Location = new System.Drawing.Point(31, 290);
            this.btConvertCatNetFindingsFile.Name = "btConvertCatNetFindingsFile";
            this.btConvertCatNetFindingsFile.Size = new System.Drawing.Size(172, 155);
            this.btConvertCatNetFindingsFile.TabIndex = 3;
            this.btConvertCatNetFindingsFile.Text = "Convert an existing Cat.Net findings results file into an Ozsmt findings file";
            this.btConvertCatNetFindingsFile.UseVisualStyleBackColor = true;
            this.btConvertCatNetFindingsFile.Click += new System.EventHandler(this.btConvertCatNetFindingsFile_Click);
            // 
            // btCompileAspNet
            // 
            this.btCompileAspNet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCompileAspNet.Location = new System.Drawing.Point(242, 290);
            this.btCompileAspNet.Name = "btCompileAspNet";
            this.btCompileAspNet.Size = new System.Drawing.Size(172, 155);
            this.btCompileAspNet.TabIndex = 2;
            this.btCompileAspNet.Text = "Compile an Asp.Net project";
            this.btCompileAspNet.UseVisualStyleBackColor = true;
            this.btCompileAspNet.Click += new System.EventHandler(this.btCompileAspNet_Click);
            // 
            // btScanDllOrWebSite
            // 
            this.btScanDllOrWebSite.AllowDrop = true;
            this.btScanDllOrWebSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btScanDllOrWebSite.Location = new System.Drawing.Point(31, 80);
            this.btScanDllOrWebSite.Name = "btScanDllOrWebSite";
            this.btScanDllOrWebSite.Size = new System.Drawing.Size(172, 155);
            this.btScanDllOrWebSite.TabIndex = 1;
            this.btScanDllOrWebSite.Text = "Scan a dll or precompiled website";
            this.btScanDllOrWebSite.UseVisualStyleBackColor = true;
            this.btScanDllOrWebSite.Click += new System.EventHandler(this.btScanDllOrWebSite_Click);
            this.btScanDllOrWebSite.DragDrop += new System.Windows.Forms.DragEventHandler(this.btScanDllOrWebSite_DragDrop);
            this.btScanDllOrWebSite.DragEnter += new System.Windows.Forms.DragEventHandler(this.btScanDllOrWebSite_DragEnter);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(27, 26);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(451, 20);
            this.label15.TabIndex = 0;
            this.label15.Text = "Welcome to O2 Cat.NET Tool. What do you want to do?";
            // 
            // tpScanDllOrWebsite
            // 
            this.tpScanDllOrWebsite.Controls.Add(this.splitContainer1);
            this.tpScanDllOrWebsite.Location = new System.Drawing.Point(4, 4);
            this.tpScanDllOrWebsite.Name = "tpScanDllOrWebsite";
            this.tpScanDllOrWebsite.Padding = new System.Windows.Forms.Padding(3);
            this.tpScanDllOrWebsite.Size = new System.Drawing.Size(891, 528);
            this.tpScanDllOrWebsite.TabIndex = 0;
            this.tpScanDllOrWebsite.Text = "Scan Dll or WebSite";
            this.tpScanDllOrWebsite.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.llNumberOfAssembliesLoaded);
            this.splitContainer1.Panel1.Controls.Add(this.llClearScanQueue);
            this.splitContainer1.Panel1.Controls.Add(this.cbOnFolderDropSearchRecursively);
            this.splitContainer1.Panel1.Controls.Add(this.lbDragAndDropHelpText);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.tvTargetFiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbAfterScanRemoveFromScanTargets);
            this.splitContainer1.Panel2.Controls.Add(this.btScanAllLoadedTargets);
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.btScanUsingCatNetScanner);
            this.splitContainer1.Size = new System.Drawing.Size(885, 522);
            this.splitContainer1.SplitterDistance = 291;
            this.splitContainer1.TabIndex = 44;
            // 
            // llNumberOfAssembliesLoaded
            // 
            this.llNumberOfAssembliesLoaded.AutoSize = true;
            this.llNumberOfAssembliesLoaded.Location = new System.Drawing.Point(4, 193);
            this.llNumberOfAssembliesLoaded.Name = "llNumberOfAssembliesLoaded";
            this.llNumberOfAssembliesLoaded.Size = new System.Drawing.Size(107, 13);
            this.llNumberOfAssembliesLoaded.TabIndex = 48;
            this.llNumberOfAssembliesLoaded.Text = "0 Assemblies Loaded";
            // 
            // llClearScanQueue
            // 
            this.llClearScanQueue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llClearScanQueue.AutoSize = true;
            this.llClearScanQueue.Location = new System.Drawing.Point(231, 192);
            this.llClearScanQueue.Name = "llClearScanQueue";
            this.llClearScanQueue.Size = new System.Drawing.Size(50, 13);
            this.llClearScanQueue.TabIndex = 47;
            this.llClearScanQueue.TabStop = true;
            this.llClearScanQueue.Text = "Clear List";
            this.llClearScanQueue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llClearScanQueue_LinkClicked);
            // 
            // cbOnFolderDropSearchRecursively
            // 
            this.cbOnFolderDropSearchRecursively.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbOnFolderDropSearchRecursively.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbOnFolderDropSearchRecursively.Checked = true;
            this.cbOnFolderDropSearchRecursively.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOnFolderDropSearchRecursively.Location = new System.Drawing.Point(7, 481);
            this.cbOnFolderDropSearchRecursively.Name = "cbOnFolderDropSearchRecursively";
            this.cbOnFolderDropSearchRecursively.Size = new System.Drawing.Size(203, 39);
            this.cbOnFolderDropSearchRecursively.TabIndex = 46;
            this.cbOnFolderDropSearchRecursively.Text = "On Folder Drop Search Recursively for scan Targets";
            this.cbOnFolderDropSearchRecursively.UseVisualStyleBackColor = true;
            // 
            // lbDragAndDropHelpText
            // 
            this.lbDragAndDropHelpText.AllowDrop = true;
            this.lbDragAndDropHelpText.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbDragAndDropHelpText.BackColor = System.Drawing.Color.White;
            this.lbDragAndDropHelpText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDragAndDropHelpText.ForeColor = System.Drawing.Color.DarkBlue;
            this.lbDragAndDropHelpText.Location = new System.Drawing.Point(65, 276);
            this.lbDragAndDropHelpText.Name = "lbDragAndDropHelpText";
            this.lbDragAndDropHelpText.Size = new System.Drawing.Size(162, 63);
            this.lbDragAndDropHelpText.TabIndex = 39;
            this.lbDragAndDropHelpText.Text = "Please Drag and drop here the Dot.NET assemblies  to scan                    (*.o" +
                "zamst) to load            (you can drop folders or zip files)";
            this.lbDragAndDropHelpText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbDragAndDropHelpText.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbDragAndDropHelpText_DragDrop);
            this.lbDragAndDropHelpText.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbDragAndDropHelpText_DragEnter);
            // 
            // tvTargetFiles
            // 
            this.tvTargetFiles.AllowDrop = true;
            this.tvTargetFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvTargetFiles.HideSelection = false;
            this.tvTargetFiles.ImageIndex = 0;
            //this.tvTargetFiles.ImageList = this.imageList1;
            this.tvTargetFiles.Location = new System.Drawing.Point(7, 215);
            this.tvTargetFiles.Name = "tvTargetFiles";
            this.tvTargetFiles.SelectedImageIndex = 0;
            this.tvTargetFiles.Size = new System.Drawing.Size(277, 248);
            this.tvTargetFiles.TabIndex = 45;
            this.tvTargetFiles.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvTargetFiles_NodeMouseDoubleClick);
            this.tvTargetFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvTargetFiles_DragDrop);
            this.tvTargetFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvTargetFiles_AfterSelect);
            this.tvTargetFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvTargetFiles_DragEnter);
            // 
            // imageList1
            // 
            //this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            //this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            //this.imageList1.Images.SetKeyName(0, "Folder.ico");
            //this.imageList1.Images.SetKeyName(1, "target.ico");
            // 
            // cbAfterScanRemoveFromScanTargets
            // 
            this.cbAfterScanRemoveFromScanTargets.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.cbAfterScanRemoveFromScanTargets.Checked = true;
            this.cbAfterScanRemoveFromScanTargets.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAfterScanRemoveFromScanTargets.Location = new System.Drawing.Point(340, 5);
            this.cbAfterScanRemoveFromScanTargets.Name = "cbAfterScanRemoveFromScanTargets";
            this.cbAfterScanRemoveFromScanTargets.Size = new System.Drawing.Size(105, 41);
            this.cbAfterScanRemoveFromScanTargets.TabIndex = 46;
            this.cbAfterScanRemoveFromScanTargets.Text = "After Scan remove from list";
            this.cbAfterScanRemoveFromScanTargets.UseVisualStyleBackColor = true;
            // 
            // btScanAllLoadedTargets
            // 
            this.btScanAllLoadedTargets.Location = new System.Drawing.Point(201, 3);
            this.btScanAllLoadedTargets.Name = "btScanAllLoadedTargets";
            this.btScanAllLoadedTargets.Size = new System.Drawing.Size(133, 41);
            this.btScanAllLoadedTargets.TabIndex = 45;
            this.btScanAllLoadedTargets.Text = "Scan all Loaded targets";
            this.btScanAllLoadedTargets.UseVisualStyleBackColor = true;
            this.btScanAllLoadedTargets.Click += new System.EventHandler(this.btScanAllLoadedTargets_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Location = new System.Drawing.Point(3, 50);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label10);
            this.splitContainer2.Panel1.Controls.Add(this.rtbLogFileForCurrentScan);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(578, 465);
            this.splitContainer2.SplitterDistance = 188;
            this.splitContainer2.TabIndex = 44;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(4, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(138, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Scanner execution log:";
            // 
            // rtbLogFileForCurrentScan
            // 
            this.rtbLogFileForCurrentScan.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogFileForCurrentScan.Location = new System.Drawing.Point(3, 31);
            this.rtbLogFileForCurrentScan.Name = "rtbLogFileForCurrentScan";
            this.rtbLogFileForCurrentScan.Size = new System.Drawing.Size(568, 150);
            this.rtbLogFileForCurrentScan.TabIndex = 19;
            this.rtbLogFileForCurrentScan.Text = "";
            this.rtbLogFileForCurrentScan.WordWrap = false;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.directoryWithScanCatNetResults);
            this.splitContainer3.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.label4);
            this.splitContainer3.Panel2.Controls.Add(this.label5);
            this.splitContainer3.Panel2.Controls.Add(this.directoryWithCreatedOzasmtFiles);
            this.splitContainer3.Panel2.Controls.Add(this.label3);
            this.splitContainer3.Size = new System.Drawing.Size(578, 273);
            this.splitContainer3.SplitterDistance = 288;
            this.splitContainer3.TabIndex = 0;
            // 
            // directoryWithScanCatNetResults
            // 
            this.directoryWithScanCatNetResults._FileFilter = "*.*";
            this.directoryWithScanCatNetResults._HandleDrop = true;
            this.directoryWithScanCatNetResults._HideFiles = false;
            this.directoryWithScanCatNetResults._ProcessDroppedObjects = false;
            this.directoryWithScanCatNetResults._ShowFileContentsOnTopTip = false;
            this.directoryWithScanCatNetResults._ShowFileSize = true;
            this.directoryWithScanCatNetResults._ShowLinkToUpperFolder = false;
            this.directoryWithScanCatNetResults._ViewMode = DirectoryViewer.ViewMode.Simple_With_LocationBar;
            this.directoryWithScanCatNetResults._WatchFolder = true;
            this.directoryWithScanCatNetResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryWithScanCatNetResults.BackColor = System.Drawing.SystemColors.Control;
            this.directoryWithScanCatNetResults.ForeColor = System.Drawing.Color.Black;
            this.directoryWithScanCatNetResults.Location = new System.Drawing.Point(1, 16);
            this.directoryWithScanCatNetResults.Name = "directoryWithScanCatNetResults";
            this.directoryWithScanCatNetResults.Size = new System.Drawing.Size(279, 250);
            this.directoryWithScanCatNetResults.TabIndex = 22;
            this.directoryWithScanCatNetResults._onFileWatchEvent += new FolderWatcher.CallbackOnFolderWatchEvent(this.directoryWithScanCatNetResults__onFileWatchEvent);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "CatNet Results File";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Location = new System.Drawing.Point(10, 219);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(243, 31);
            this.label4.TabIndex = 23;
            this.label4.Text = "Drop previously created CatNet scan files to convert them";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 250);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(252, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Double Click on Ozasmt file to open Findings Viewer";
            // 
            // directoryWithCreatedOzasmtFiles
            // 
            this.directoryWithCreatedOzasmtFiles._FileFilter = "*.*";
            this.directoryWithCreatedOzasmtFiles._HandleDrop = true;
            this.directoryWithCreatedOzasmtFiles._HideFiles = false;
            this.directoryWithCreatedOzasmtFiles._ProcessDroppedObjects = false;
            this.directoryWithCreatedOzasmtFiles._ShowFileContentsOnTopTip = false;
            this.directoryWithCreatedOzasmtFiles._ShowFileSize = true;
            this.directoryWithCreatedOzasmtFiles._ShowLinkToUpperFolder = false;
            this.directoryWithCreatedOzasmtFiles._ViewMode = DirectoryViewer.ViewMode.Simple_With_LocationBar;
            this.directoryWithCreatedOzasmtFiles._WatchFolder = true;
            this.directoryWithCreatedOzasmtFiles.AllowDrop = true;
            this.directoryWithCreatedOzasmtFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryWithCreatedOzasmtFiles.BackColor = System.Drawing.SystemColors.Control;
            this.directoryWithCreatedOzasmtFiles.ForeColor = System.Drawing.Color.Black;
            this.directoryWithCreatedOzasmtFiles.Location = new System.Drawing.Point(7, 17);
            this.directoryWithCreatedOzasmtFiles.Name = "directoryWithCreatedOzasmtFiles";
            this.directoryWithCreatedOzasmtFiles.Size = new System.Drawing.Size(272, 199);
            this.directoryWithCreatedOzasmtFiles.TabIndex = 24;
            this.directoryWithCreatedOzasmtFiles._onTreeViewDrop += new Callbacks.dMethod_String(this.directoryWithCreatedOzasmtFiles__onTreeViewDrop);
            this.directoryWithCreatedOzasmtFiles.eDirectoryEvent_DoubleClick += new DirectoryViewer.dDirectoryEvent(this.directoryWithCreatedOzasmtFiles_eDirectoryEvent_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Converted Ozasmt Files";
            // 
            // tpConvertCatNetOzasmt
            // 
            this.tpConvertCatNetOzasmt.Controls.Add(this.label18);
            this.tpConvertCatNetOzasmt.Controls.Add(this.label19);
            this.tpConvertCatNetOzasmt.Controls.Add(this.label20);
            this.tpConvertCatNetOzasmt.Controls.Add(this.directoryToDropCatNetFilesForConversion);
            this.tpConvertCatNetOzasmt.Location = new System.Drawing.Point(4, 4);
            this.tpConvertCatNetOzasmt.Name = "tpConvertCatNetOzasmt";
            this.tpConvertCatNetOzasmt.Padding = new System.Windows.Forms.Padding(3);
            this.tpConvertCatNetOzasmt.Size = new System.Drawing.Size(891, 528);
            this.tpConvertCatNetOzasmt.TabIndex = 6;
            this.tpConvertCatNetOzasmt.Text = "Convert Cat.Net -> Ozasmt";
            this.tpConvertCatNetOzasmt.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(3, 34);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(285, 19);
            this.label18.TabIndex = 27;
            this.label18.Text = "Drop files in the box below to convert files";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(3, 53);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(252, 13);
            this.label19.TabIndex = 29;
            this.label19.Text = "Double Click on Ozasmt file to open Findings Viewer";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(3, 12);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(362, 13);
            this.label20.TabIndex = 26;
            this.label20.Text = "Converted Previously created CAT.NET files into  Ozasmt Files";
            // 
            // directoryToDropCatNetFilesForConversion
            // 
            this.directoryToDropCatNetFilesForConversion._FileFilter = "*.*";
            this.directoryToDropCatNetFilesForConversion._HandleDrop = true;
            this.directoryToDropCatNetFilesForConversion._HideFiles = false;
            this.directoryToDropCatNetFilesForConversion._ProcessDroppedObjects = false;
            this.directoryToDropCatNetFilesForConversion._ShowFileContentsOnTopTip = false;
            this.directoryToDropCatNetFilesForConversion._ShowFileSize = true;
            this.directoryToDropCatNetFilesForConversion._ShowLinkToUpperFolder = false;
            this.directoryToDropCatNetFilesForConversion._ViewMode = DirectoryViewer.ViewMode.Advanced;
            this.directoryToDropCatNetFilesForConversion._WatchFolder = true;
            this.directoryToDropCatNetFilesForConversion.AllowDrop = true;
            this.directoryToDropCatNetFilesForConversion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryToDropCatNetFilesForConversion.BackColor = System.Drawing.SystemColors.Control;
            this.directoryToDropCatNetFilesForConversion.ForeColor = System.Drawing.Color.Black;
            this.directoryToDropCatNetFilesForConversion.Location = new System.Drawing.Point(6, 76);
            this.directoryToDropCatNetFilesForConversion.Name = "directoryToDropCatNetFilesForConversion";
            this.directoryToDropCatNetFilesForConversion.Size = new System.Drawing.Size(879, 432);
            this.directoryToDropCatNetFilesForConversion.TabIndex = 28;
            this.directoryToDropCatNetFilesForConversion._onTreeViewDrop += new Callbacks.dMethod_String(this.directoryToDropCatNetFilesForConversion__onTreeViewDrop);
            // 
            // tpCatNetInstall
            // 
            this.tpCatNetInstall.Controls.Add(this.label16);
            this.tpCatNetInstall.Controls.Add(this.supportLinks);
            this.tpCatNetInstall.Controls.Add(this.lbCantFindCatNetExecutable);
            this.tpCatNetInstall.Controls.Add(this.tbPathToCatNetExecutable);
            this.tpCatNetInstall.Controls.Add(this.label1);
            //this.tpCatNetInstall.Controls.Add(this.webAutomation);
            this.tpCatNetInstall.Location = new System.Drawing.Point(4, 4);
            this.tpCatNetInstall.Name = "tpCatNetInstall";
            this.tpCatNetInstall.Padding = new System.Windows.Forms.Padding(3);
            this.tpCatNetInstall.Size = new System.Drawing.Size(891, 528);
            this.tpCatNetInstall.TabIndex = 2;
            this.tpCatNetInstall.Text = "CatNet Install";
            this.tpCatNetInstall.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 73);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(75, 13);
            this.label16.TabIndex = 57;
            this.label16.Text = "Support Links:";
            // 
            // supportLinks
            // 
            this.supportLinks.Location = new System.Drawing.Point(4, 98);
            this.supportLinks.Name = "supportLinks";
            this.supportLinks.Size = new System.Drawing.Size(133, 314);
            this.supportLinks.TabIndex = 53;
            // 
            // lbCantFindCatNetExecutable
            // 
            this.lbCantFindCatNetExecutable.AutoSize = true;
            this.lbCantFindCatNetExecutable.ForeColor = System.Drawing.Color.Red;
            this.lbCantFindCatNetExecutable.Location = new System.Drawing.Point(155, 29);
            this.lbCantFindCatNetExecutable.Name = "lbCantFindCatNetExecutable";
            this.lbCantFindCatNetExecutable.Size = new System.Drawing.Size(271, 13);
            this.lbCantFindCatNetExecutable.TabIndex = 50;
            this.lbCantFindCatNetExecutable.Text = "Can\'t find Microsoft Cat.Net executable in this computer!";
            // 
            // tbPathToCatNetExecutable
            // 
            this.tbPathToCatNetExecutable.Location = new System.Drawing.Point(158, 6);
            this.tbPathToCatNetExecutable.Name = "tbPathToCatNetExecutable";
            this.tbPathToCatNetExecutable.Size = new System.Drawing.Size(319, 20);
            this.tbPathToCatNetExecutable.TabIndex = 49;
            this.tbPathToCatNetExecutable.TextChanged += new System.EventHandler(this.tbPathToCatNetExecutable_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 48;
            this.label1.Text = "Path to MsCatNet executable";
            // 
            // webAutomation
            // 
/*            this.webAutomation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webAutomation.BackColor = System.Drawing.SystemColors.Control;
            this.webAutomation.ForeColor = System.Drawing.Color.Black;
            this.webAutomation.Location = new System.Drawing.Point(152, 68);
            this.webAutomation.Name = "webAutomation";
            this.webAutomation.Size = new System.Drawing.Size(733, 454);
            this.webAutomation.TabIndex = 52;*/
            // 
            // tpConfig
            // 
            this.tpConfig.Controls.Add(this.splitContainer5);
            this.tpConfig.Controls.Add(this.groupBox3);
            this.tpConfig.Controls.Add(this.label6);
            this.tpConfig.Controls.Add(this.taskHostControl);
            this.tpConfig.Location = new System.Drawing.Point(4, 4);
            this.tpConfig.Name = "tpConfig";
            this.tpConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tpConfig.Size = new System.Drawing.Size(891, 528);
            this.tpConfig.TabIndex = 1;
            this.tpConfig.Text = "Config";
            this.tpConfig.UseVisualStyleBackColor = true;
            // 
            // splitContainer5
            // 
            this.splitContainer5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer5.Location = new System.Drawing.Point(301, 303);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.label11);
            this.splitContainer5.Panel1.Controls.Add(this.ascx_Directory3);
            this.splitContainer5.Panel1.Controls.Add(this.label12);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.label13);
            this.splitContainer5.Panel2.Controls.Add(this.ascx_Directory4);
            this.splitContainer5.Panel2.Controls.Add(this.label14);
            this.splitContainer5.Size = new System.Drawing.Size(462, 144);
            this.splitContainer5.SplitterDistance = 221;
            this.splitContainer5.TabIndex = 52;
            this.splitContainer5.Visible = false;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 119);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(243, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "You can drop previously created CatNet scan files";
            // 
            // ascx_Directory3
            // 
            this.ascx_Directory3._FileFilter = "*.*";
            this.ascx_Directory3._HandleDrop = true;
            this.ascx_Directory3._HideFiles = false;
            this.ascx_Directory3._ProcessDroppedObjects = true;
            this.ascx_Directory3._ShowFileContentsOnTopTip = false;
            this.ascx_Directory3._ShowFileSize = false;
            this.ascx_Directory3._ShowLinkToUpperFolder = true;
            this.ascx_Directory3._ViewMode = DirectoryViewer.ViewMode.Simple_With_LocationBar;
            this.ascx_Directory3._WatchFolder = false;
            this.ascx_Directory3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ascx_Directory3.BackColor = System.Drawing.SystemColors.Control;
            this.ascx_Directory3.ForeColor = System.Drawing.Color.Black;
            this.ascx_Directory3.Location = new System.Drawing.Point(1, 16);
            this.ascx_Directory3.Name = "ascx_Directory3";
            this.ascx_Directory3.Size = new System.Drawing.Size(212, 119);
            this.ascx_Directory3.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(5, 2);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "CatNet Results File";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 119);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(222, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "You can drag these files into a Finding Viewer";
            // 
            // ascx_Directory4
            // 
            this.ascx_Directory4._FileFilter = "*.*";
            this.ascx_Directory4._HandleDrop = true;
            this.ascx_Directory4._HideFiles = false;
            this.ascx_Directory4._ProcessDroppedObjects = false;
            this.ascx_Directory4._ShowFileContentsOnTopTip = false;
            this.ascx_Directory4._ShowFileSize = false;
            this.ascx_Directory4._ShowLinkToUpperFolder = true;
            this.ascx_Directory4._ViewMode = DirectoryViewer.ViewMode.Simple_With_LocationBar;
            this.ascx_Directory4._WatchFolder = false;
            this.ascx_Directory4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ascx_Directory4.BackColor = System.Drawing.SystemColors.Control;
            this.ascx_Directory4.ForeColor = System.Drawing.Color.Black;
            this.ascx_Directory4.Location = new System.Drawing.Point(7, 17);
            this.ascx_Directory4.Name = "ascx_Directory4";
            this.ascx_Directory4.Size = new System.Drawing.Size(223, 119);
            this.ascx_Directory4.TabIndex = 24;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(10, 1);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(140, 13);
            this.label14.TabIndex = 23;
            this.label14.Text = "Converted Ozasmt Files";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.tbDllToScan);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.tbCatNetShell_CommandToExecute);
            this.groupBox3.Controls.Add(this.rtbCatNetShell_Log);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(879, 278);
            this.groupBox3.TabIndex = 51;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Cat.Net Shell";
            // 
            // tbDllToScan
            // 
            this.tbDllToScan.AllowDrop = true;
            this.tbDllToScan.Location = new System.Drawing.Point(368, 14);
            this.tbDllToScan.Name = "tbDllToScan";
            this.tbDllToScan.Size = new System.Drawing.Size(147, 20);
            this.tbDllToScan.TabIndex = 54;
            this.tbDllToScan.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbDllToScan_DragDrop);
            this.tbDllToScan.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbDllToScan_DragEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(292, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 53;
            this.label8.Text = "dll to scan";
            // 
            // tbCatNetShell_CommandToExecute
            // 
            this.tbCatNetShell_CommandToExecute.Location = new System.Drawing.Point(118, 15);
            this.tbCatNetShell_CommandToExecute.Name = "tbCatNetShell_CommandToExecute";
            this.tbCatNetShell_CommandToExecute.Size = new System.Drawing.Size(158, 20);
            this.tbCatNetShell_CommandToExecute.TabIndex = 52;
            this.tbCatNetShell_CommandToExecute.TextChanged += new System.EventHandler(this.tbCatNetShell_CommandToExecute_TextChanged);
            this.tbCatNetShell_CommandToExecute.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCatNetShell_CommandToExecute_KeyPress);
            // 
            // rtbCatNetShell_Log
            // 
            this.rtbCatNetShell_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbCatNetShell_Log.Location = new System.Drawing.Point(6, 36);
            this.rtbCatNetShell_Log.Name = "rtbCatNetShell_Log";
            this.rtbCatNetShell_Log.Size = new System.Drawing.Size(867, 236);
            this.rtbCatNetShell_Log.TabIndex = 50;
            this.rtbCatNetShell_Log.Text = "";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 13);
            this.label7.TabIndex = 49;
            this.label7.Text = "command to execute";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 48;
            this.label6.Text = "Task Host Control";
            // 
            // taskHostControl
            // 
            this.taskHostControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.taskHostControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.taskHostControl.Location = new System.Drawing.Point(12, 303);
            this.taskHostControl.Name = "taskHostControl";
            this.taskHostControl.Size = new System.Drawing.Size(269, 153);
            this.taskHostControl.TabIndex = 46;
            this.taskHostControl.Visible = false;
            // 
            // ascx_MsCatNet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcCatNetScanner);
            this.Name = "ascx_MsCatNet";
            this.Size = new System.Drawing.Size(905, 560);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tcCatNetScanner.ResumeLayout(false);
            this.tbSplashPage.ResumeLayout(false);
            this.tbSplashPage.PerformLayout();
            this.tpScanDllOrWebsite.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.tpConvertCatNetOzasmt.ResumeLayout(false);
            this.tpConvertCatNetOzasmt.PerformLayout();
            this.tpCatNetInstall.ResumeLayout(false);
            this.tpCatNetInstall.PerformLayout();
            this.tpConfig.ResumeLayout(false);
            this.tpConfig.PerformLayout();
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel1.PerformLayout();
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            this.splitContainer5.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel llDownloadDemoFile_HacmeBank_Website;
        private System.Windows.Forms.LinkLabel llDownloadDemoFile_HacmeBank_WebServices;
        private System.Windows.Forms.Button btScanUsingCatNetScanner;
        private System.Windows.Forms.TabControl tcCatNetScanner;
        private System.Windows.Forms.TabPage tpScanDllOrWebsite;
        private System.Windows.Forms.TabPage tpConfig;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbDragAndDropHelpText;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.RichTextBox rtbLogFileForCurrentScan;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label2;
        private DirectoryViewer directoryWithScanCatNetResults;
        private DirectoryViewer directoryWithCreatedOzasmtFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.FlowLayoutPanel taskHostControl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbCatNetShell_CommandToExecute;
        private System.Windows.Forms.RichTextBox rtbCatNetShell_Log;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbDllToScan;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabPage tpCatNetInstall;
        private System.Windows.Forms.Label lbCantFindCatNetExecutable;
        private System.Windows.Forms.TextBox tbPathToCatNetExecutable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tbSplashPage;
        private System.Windows.Forms.Button btInstallCatNet;
        private System.Windows.Forms.Button btConvertCatNetFindingsFile;
        private System.Windows.Forms.Button btCompileAspNet;
        private System.Windows.Forms.Button btScanDllOrWebSite;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button btCatNetDocumentation;
        private System.Windows.Forms.FlowLayoutPanel supportLinks;
        //private ascx_WebAutomation webAutomation;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TabPage tpConvertCatNetOzasmt;
        private System.Windows.Forms.LinkLabel llDemoFiles_AddDotNetAssemblies;
        private System.Windows.Forms.LinkLabel llDemoFiles_AddO2Assemblies;
        private System.Windows.Forms.TreeView tvTargetFiles;
        //private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btScanAllLoadedTargets;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private DirectoryViewer directoryToDropCatNetFilesForConversion;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox cbAfterScanRemoveFromScanTargets;
        private System.Windows.Forms.CheckBox cbOnFolderDropSearchRecursively;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.LinkLabel llDemoFiles_TempASPNETFolder;
        private System.Windows.Forms.LinkLabel llClearScanQueue;
        private System.Windows.Forms.Label llNumberOfAssembliesLoaded;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.Label label11;
        private DirectoryViewer ascx_Directory3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private DirectoryViewer ascx_Directory4;
        private System.Windows.Forms.Label label14;
    }
}
