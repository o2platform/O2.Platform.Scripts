// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2File:ascx_SearchTargets.Controllers.cs
//O2File:ascx_SearchTargets.cs 

namespace O2.Tool
{
    partial class ascx_SearchTargets
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
            this.btLoad = new System.Windows.Forms.Button();
            this.cbLoadXmlAsAssessmentRun = new System.Windows.Forms.CheckBox();
            this.tbFilesToLoad_Extension = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbLoadFileMode_RecursiveSearch = new System.Windows.Forms.CheckBox();
            this.tbFilesToLoad = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lbLoadedFiles = new System.Windows.Forms.ListBox();
            this.llRemoveLoadedFiles = new System.Windows.Forms.LinkLabel();
            this.llReloadFiles = new System.Windows.Forms.LinkLabel();
            this.lbLoadDroppedFiles = new System.Windows.Forms.Label();
            this.lbNumberOfFilesLoaded = new System.Windows.Forms.Label();
            this.cbOpenFileOnSelect = new System.Windows.Forms.CheckBox();
            this.llRemoveSelectedFiles = new System.Windows.Forms.LinkLabel();
            this.btRefresh = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btLoad
            // 
            this.btLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLoad.Location = new System.Drawing.Point(204, 3);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(75, 46);
            this.btLoad.TabIndex = 46;
            this.btLoad.Text = "load";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // cbLoadXmlAsAssessmentRun
            // 
            this.cbLoadXmlAsAssessmentRun.AutoSize = true;
            this.cbLoadXmlAsAssessmentRun.Enabled = false;
            this.cbLoadXmlAsAssessmentRun.Location = new System.Drawing.Point(217, 55);
            this.cbLoadXmlAsAssessmentRun.Name = "cbLoadXmlAsAssessmentRun";
            this.cbLoadXmlAsAssessmentRun.Size = new System.Drawing.Size(165, 17);
            this.cbLoadXmlAsAssessmentRun.TabIndex = 45;
            this.cbLoadXmlAsAssessmentRun.Text = "Load *.xml as assessment run";
            this.cbLoadXmlAsAssessmentRun.UseVisualStyleBackColor = true;
            this.cbLoadXmlAsAssessmentRun.Visible = false;
            // 
            // tbFilesToLoad_Extension
            // 
            this.tbFilesToLoad_Extension.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.tbFilesToLoad_Extension.Location = new System.Drawing.Point(92, 29);
            this.tbFilesToLoad_Extension.Name = "tbFilesToLoad_Extension";
            this.tbFilesToLoad_Extension.Size = new System.Drawing.Size(54, 20);
            this.tbFilesToLoad_Extension.TabIndex = 43;
            this.tbFilesToLoad_Extension.Text = "*.cs";
            this.tbFilesToLoad_Extension.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFilesToLoad_Extension_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "File Extension:";
            // 
            // cbLoadFileMode_RecursiveSearch
            // 
            this.cbLoadFileMode_RecursiveSearch.AutoSize = true;
            this.cbLoadFileMode_RecursiveSearch.Checked = true;
            this.cbLoadFileMode_RecursiveSearch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLoadFileMode_RecursiveSearch.Location = new System.Drawing.Point(6, 54);
            this.cbLoadFileMode_RecursiveSearch.Name = "cbLoadFileMode_RecursiveSearch";
            this.cbLoadFileMode_RecursiveSearch.Size = new System.Drawing.Size(74, 17);
            this.cbLoadFileMode_RecursiveSearch.TabIndex = 41;
            this.cbLoadFileMode_RecursiveSearch.Text = "Recursive";
            this.cbLoadFileMode_RecursiveSearch.UseVisualStyleBackColor = true;
            // 
            // tbFilesToLoad
            // 
            this.tbFilesToLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilesToLoad.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.tbFilesToLoad.Location = new System.Drawing.Point(92, 3);
            this.tbFilesToLoad.Name = "tbFilesToLoad";
            this.tbFilesToLoad.Size = new System.Drawing.Size(106, 20);
            this.tbFilesToLoad.TabIndex = 40;
            this.tbFilesToLoad.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFilesToLoad_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 39;
            this.label8.Text = "Load  file(s) from:";
            // 
            // lbLoadedFiles
            // 
            this.lbLoadedFiles.AllowDrop = true;
            this.lbLoadedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLoadedFiles.FormattingEnabled = true;
            this.lbLoadedFiles.Location = new System.Drawing.Point(3, 98);
            this.lbLoadedFiles.Name = "lbLoadedFiles";
            this.lbLoadedFiles.Size = new System.Drawing.Size(273, 199);
            this.lbLoadedFiles.TabIndex = 38;
            this.lbLoadedFiles.SelectedIndexChanged += new System.EventHandler(this.lbLoadedFiles_SelectedIndexChanged);
            this.lbLoadedFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbLoadedFiles_DragDrop);
            this.lbLoadedFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbLoadedFiles_DragEnter);
            // 
            // llRemoveLoadedFiles
            // 
            this.llRemoveLoadedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.llRemoveLoadedFiles.AutoSize = true;
            this.llRemoveLoadedFiles.Location = new System.Drawing.Point(200, 301);
            this.llRemoveLoadedFiles.Name = "llRemoveLoadedFiles";
            this.llRemoveLoadedFiles.Size = new System.Drawing.Size(76, 13);
            this.llRemoveLoadedFiles.TabIndex = 47;
            this.llRemoveLoadedFiles.TabStop = true;
            this.llRemoveLoadedFiles.Text = "remove all files";
            this.llRemoveLoadedFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llRemoveLoadedFiles_LinkClicked);
            // 
            // llReloadFiles
            // 
            this.llReloadFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.llReloadFiles.AutoSize = true;
            this.llReloadFiles.Location = new System.Drawing.Point(3, 301);
            this.llReloadFiles.Name = "llReloadFiles";
            this.llReloadFiles.Size = new System.Drawing.Size(57, 13);
            this.llReloadFiles.TabIndex = 48;
            this.llReloadFiles.TabStop = true;
            this.llReloadFiles.Text = "reload files";
            this.llReloadFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llReloadFiles_LinkClicked);
            // 
            // lbLoadDroppedFiles
            // 
            this.lbLoadDroppedFiles.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbLoadDroppedFiles.BackColor = System.Drawing.SystemColors.Window;
            this.lbLoadDroppedFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLoadDroppedFiles.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lbLoadDroppedFiles.Location = new System.Drawing.Point(42, 167);
            this.lbLoadDroppedFiles.Name = "lbLoadDroppedFiles";
            this.lbLoadDroppedFiles.Size = new System.Drawing.Size(201, 73);
            this.lbLoadDroppedFiles.TabIndex = 49;
            this.lbLoadDroppedFiles.Text = "Loading Dropped Files";
            this.lbLoadDroppedFiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbLoadDroppedFiles.Visible = false;
            // 
            // lbNumberOfFilesLoaded
            // 
            this.lbNumberOfFilesLoaded.AutoSize = true;
            this.lbNumberOfFilesLoaded.Location = new System.Drawing.Point(5, 78);
            this.lbNumberOfFilesLoaded.Name = "lbNumberOfFilesLoaded";
            this.lbNumberOfFilesLoaded.Size = new System.Drawing.Size(16, 13);
            this.lbNumberOfFilesLoaded.TabIndex = 50;
            this.lbNumberOfFilesLoaded.Text = "...";
            // 
            // cbOpenFileOnSelect
            // 
            this.cbOpenFileOnSelect.AutoSize = true;
            this.cbOpenFileOnSelect.Checked = true;
            this.cbOpenFileOnSelect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbOpenFileOnSelect.Location = new System.Drawing.Point(92, 55);
            this.cbOpenFileOnSelect.Name = "cbOpenFileOnSelect";
            this.cbOpenFileOnSelect.Size = new System.Drawing.Size(119, 17);
            this.cbOpenFileOnSelect.TabIndex = 51;
            this.cbOpenFileOnSelect.Text = "Open File on Select";
            this.cbOpenFileOnSelect.UseVisualStyleBackColor = true;
            this.cbOpenFileOnSelect.CheckedChanged += new System.EventHandler(this.cbOpenFileOnSelect_CheckedChanged);
            // 
            // llRemoveSelectedFiles
            // 
            this.llRemoveSelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.llRemoveSelectedFiles.AutoSize = true;
            this.llRemoveSelectedFiles.Location = new System.Drawing.Point(85, 301);
            this.llRemoveSelectedFiles.Name = "llRemoveSelectedFiles";
            this.llRemoveSelectedFiles.Size = new System.Drawing.Size(106, 13);
            this.llRemoveSelectedFiles.TabIndex = 52;
            this.llRemoveSelectedFiles.TabStop = true;
            this.llRemoveSelectedFiles.Text = "remove selected files";
            this.llRemoveSelectedFiles.Visible = false;
            this.llRemoveSelectedFiles.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llRemoveSelectedFiles_LinkClicked);
            // 
            // btRefresh
            // 
            this.btRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRefresh.AutoSize = true;
            this.btRefresh.Location = new System.Drawing.Point(237, 82);
            this.btRefresh.Name = "btRefresh";
            this.btRefresh.Size = new System.Drawing.Size(39, 13);
            this.btRefresh.TabIndex = 53;
            this.btRefresh.TabStop = true;
            this.btRefresh.Text = "refresh";
            this.btRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btRefresh_LinkClicked);
            // 
            // ascx_SearchTargets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btRefresh);
            this.Controls.Add(this.llRemoveSelectedFiles);
            this.Controls.Add(this.cbOpenFileOnSelect);
            this.Controls.Add(this.lbNumberOfFilesLoaded);
            this.Controls.Add(this.lbLoadDroppedFiles);
            this.Controls.Add(this.llReloadFiles);
            this.Controls.Add(this.llRemoveLoadedFiles);
            this.Controls.Add(this.btLoad);
            this.Controls.Add(this.cbLoadXmlAsAssessmentRun);
            this.Controls.Add(this.tbFilesToLoad_Extension);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbLoadFileMode_RecursiveSearch);
            this.Controls.Add(this.tbFilesToLoad);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lbLoadedFiles);
            this.Name = "ascx_SearchTargets";
            this.Size = new System.Drawing.Size(279, 317);
            this.Load += new System.EventHandler(this.ascx_SearchTargets_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.CheckBox cbLoadXmlAsAssessmentRun;
        private System.Windows.Forms.TextBox tbFilesToLoad_Extension;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbLoadFileMode_RecursiveSearch;
        private System.Windows.Forms.TextBox tbFilesToLoad;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox lbLoadedFiles;
        private System.Windows.Forms.LinkLabel llRemoveLoadedFiles;
        private System.Windows.Forms.LinkLabel llReloadFiles;
        private System.Windows.Forms.Label lbLoadDroppedFiles;
        private System.Windows.Forms.Label lbNumberOfFilesLoaded;
        private System.Windows.Forms.CheckBox cbOpenFileOnSelect;
        private System.Windows.Forms.LinkLabel llRemoveSelectedFiles;
        private System.Windows.Forms.LinkLabel btRefresh;

    }
}
