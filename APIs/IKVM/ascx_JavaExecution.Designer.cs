// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
//O2File:ascx_JavaExecution.cs
//O2File:ascx_JavaExecution.Controllers.cs

namespace O2.XRules.Database.APIs.IKVM
{
    partial class ascx_JavaExecution
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dotNetAssembliesToConvert = new O2.Views.ASCX.CoreControls.ascx_FileMappings();
            this.label2 = new System.Windows.Forms.Label();
            this.llDeleteEmptyJarStubs = new System.Windows.Forms.LinkLabel();
            this.llDeleteJarStubs = new System.Windows.Forms.LinkLabel();
            this.progressBarForJarStubCreation = new System.Windows.Forms.ProgressBar();
            this.btCreateJarStubFiles = new System.Windows.Forms.Button();
            this.directoryWithJarStubFiles = new O2.Views.ASCX.CoreControls.ascx_Directory();
            this.label3 = new System.Windows.Forms.Label();
            this.llLoadDefaultSetOfFilesToConvert = new System.Windows.Forms.LinkLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.directoryToDropJarsToConvertIntoDotNetAssemblies = new O2.Views.ASCX.CoreControls.ascx_Directory();
            this.groupBox1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IKVM Execution";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.splitContainer1);
            this.groupBox1.Location = new System.Drawing.Point(7, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(647, 398);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create IKVM Jar stubs";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(3, 16);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dotNetAssembliesToConvert);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.llDeleteEmptyJarStubs);
            this.splitContainer1.Panel2.Controls.Add(this.llDeleteJarStubs);
            this.splitContainer1.Panel2.Controls.Add(this.progressBarForJarStubCreation);
            this.splitContainer1.Panel2.Controls.Add(this.btCreateJarStubFiles);
            this.splitContainer1.Panel2.Controls.Add(this.directoryWithJarStubFiles);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Size = new System.Drawing.Size(639, 379);
            this.splitContainer1.SplitterDistance = 195;
            this.splitContainer1.TabIndex = 0;
            // 
            // dotNetAssembliesToConvert
            // 
            this.dotNetAssembliesToConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dotNetAssembliesToConvert.Location = new System.Drawing.Point(6, 55);
            this.dotNetAssembliesToConvert.Name = "dotNetAssembliesToConvert";
            this.dotNetAssembliesToConvert.Size = new System.Drawing.Size(182, 324);
            this.dotNetAssembliesToConvert.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = ".Net Assemblies To Convert";
            // 
            // llDeleteEmptyJarStubs
            // 
            this.llDeleteEmptyJarStubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llDeleteEmptyJarStubs.AutoSize = true;
            this.llDeleteEmptyJarStubs.Location = new System.Drawing.Point(160, 39);
            this.llDeleteEmptyJarStubs.Name = "llDeleteEmptyJarStubs";
            this.llDeleteEmptyJarStubs.Size = new System.Drawing.Size(170, 13);
            this.llDeleteEmptyJarStubs.TabIndex = 7;
            this.llDeleteEmptyJarStubs.TabStop = true;
            this.llDeleteEmptyJarStubs.Text = "Delete Empty Jar Stubs (0k in size)";
            this.llDeleteEmptyJarStubs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDeleteEmptyJarStubs_LinkClicked);
            // 
            // llDeleteJarStubs
            // 
            this.llDeleteJarStubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llDeleteJarStubs.AutoSize = true;
            this.llDeleteJarStubs.Location = new System.Drawing.Point(348, 39);
            this.llDeleteJarStubs.Name = "llDeleteJarStubs";
            this.llDeleteJarStubs.Size = new System.Drawing.Size(85, 13);
            this.llDeleteJarStubs.TabIndex = 3;
            this.llDeleteJarStubs.TabStop = true;
            this.llDeleteJarStubs.Text = "Delete Jar Stubs";
            this.llDeleteJarStubs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llDeleteJarStubs_LinkClicked);
            // 
            // progressBarForJarStubCreation
            // 
            this.progressBarForJarStubCreation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarForJarStubCreation.Location = new System.Drawing.Point(150, 6);
            this.progressBarForJarStubCreation.Name = "progressBarForJarStubCreation";
            this.progressBarForJarStubCreation.Size = new System.Drawing.Size(283, 23);
            this.progressBarForJarStubCreation.TabIndex = 6;
            // 
            // btCreateJarStubFiles
            // 
            this.btCreateJarStubFiles.Location = new System.Drawing.Point(4, 6);
            this.btCreateJarStubFiles.Name = "btCreateJarStubFiles";
            this.btCreateJarStubFiles.Size = new System.Drawing.Size(140, 23);
            this.btCreateJarStubFiles.TabIndex = 5;
            this.btCreateJarStubFiles.Text = "Create Jar Sub files";
            this.btCreateJarStubFiles.UseVisualStyleBackColor = true;
            this.btCreateJarStubFiles.Click += new System.EventHandler(this.btCreateJarStubFiles_Click);
            // 
            // directoryWithJarStubFiles
            // 
            this.directoryWithJarStubFiles._ProcessDroppedObjects = true;
            this.directoryWithJarStubFiles._ShowFileSize = true;
            this.directoryWithJarStubFiles._ShowLinkToUpperFolder = true;
            this.directoryWithJarStubFiles._ViewMode = O2.Views.ASCX.CoreControls.ascx_Directory.ViewMode.Simple_With_LocationBar;
            this.directoryWithJarStubFiles._WatchFolder = true;
            this.directoryWithJarStubFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryWithJarStubFiles.BackColor = System.Drawing.SystemColors.Control;
            this.directoryWithJarStubFiles.ForeColor = System.Drawing.Color.Black;
            this.directoryWithJarStubFiles.Location = new System.Drawing.Point(4, 55);
            this.directoryWithJarStubFiles.Name = "directoryWithJarStubFiles";
            this.directoryWithJarStubFiles.Size = new System.Drawing.Size(429, 321);
            this.directoryWithJarStubFiles.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Converted Jar Files";
            // 
            // llLoadDefaultSetOfFilesToConvert
            // 
            this.llLoadDefaultSetOfFilesToConvert.AutoSize = true;
            this.llLoadDefaultSetOfFilesToConvert.Location = new System.Drawing.Point(120, 11);
            this.llLoadDefaultSetOfFilesToConvert.Name = "llLoadDefaultSetOfFilesToConvert";
            this.llLoadDefaultSetOfFilesToConvert.Size = new System.Drawing.Size(181, 13);
            this.llLoadDefaultSetOfFilesToConvert.TabIndex = 2;
            this.llLoadDefaultSetOfFilesToConvert.TabStop = true;
            this.llLoadDefaultSetOfFilesToConvert.Text = "Load Default Set Of Files To Convert";
            this.llLoadDefaultSetOfFilesToConvert.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llLoadDefaultSetOfFilesToConvert_LinkClicked);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.directoryToDropJarsToConvertIntoDotNetAssemblies);
            this.groupBox2.Location = new System.Drawing.Point(659, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 390);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create dotNet assemblies from Jars";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(187, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Drop jar file to create dotNet Assembly";
            // 
            // directoryToDropJarsToConvertIntoDotNetAssemblies
            // 
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._ProcessDroppedObjects = false;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._ShowFileSize = true;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._ShowLinkToUpperFolder = true;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._ViewMode = O2.Views.ASCX.CoreControls.ascx_Directory.ViewMode.Simple_With_LocationBar;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._WatchFolder = true;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.BackColor = System.Drawing.SystemColors.Control;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.ForeColor = System.Drawing.Color.Black;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.Location = new System.Drawing.Point(6, 45);
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.Name = "directoryToDropJarsToConvertIntoDotNetAssemblies";
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.Size = new System.Drawing.Size(219, 339);
            this.directoryToDropJarsToConvertIntoDotNetAssemblies.TabIndex = 5;
            this.directoryToDropJarsToConvertIntoDotNetAssemblies._onTreeViewDrop += new O2.Kernel.CodeUtils.Callbacks.dMethod_String(this.directoryToDropJarsToConvertIntoDotNetAssemblies__onTreeViewDrop);
            // 
            // ascx_JavaExecution
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.llLoadDefaultSetOfFilesToConvert);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "ascx_JavaExecution";
            this.Size = new System.Drawing.Size(893, 439);
            this.Load += new System.EventHandler(this.ascx_JavaExecution_Load);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private O2.Views.ASCX.CoreControls.ascx_FileMappings dotNetAssembliesToConvert;
        private System.Windows.Forms.LinkLabel llLoadDefaultSetOfFilesToConvert;
        private System.Windows.Forms.Button btCreateJarStubFiles;
        private O2.Views.ASCX.CoreControls.ascx_Directory directoryWithJarStubFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBarForJarStubCreation;
        private System.Windows.Forms.LinkLabel llDeleteJarStubs;
        private System.Windows.Forms.LinkLabel llDeleteEmptyJarStubs;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private O2.Views.ASCX.CoreControls.ascx_Directory directoryToDropJarsToConvertIntoDotNetAssemblies;
    }
}
