// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx.O2Rules
{
    partial class ascx_FilteredFindings
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
            this.lbFilteredFindingTitle = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbSourceSignatures = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbSinkSignatures = new System.Windows.Forms.TextBox();
            this.llCalculateFindings = new System.Windows.Forms.LinkLabel();
            this.findingsViewer_Results = new O2.Views.ASCX.O2Findings.ascx_FindingsViewer();
            this.tableList_LoadedFindingsDetails = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.tcFilteredFindings = new System.Windows.Forms.TabControl();
            this.tpResults = new System.Windows.Forms.TabPage();
            this.tpSignatureFilters = new System.Windows.Forms.TabPage();
            this.tpLoadedFindingsDetails = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tcFilteredFindings.SuspendLayout();
            this.tpResults.SuspendLayout();
            this.tpSignatureFilters.SuspendLayout();
            this.tpLoadedFindingsDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFilteredFindingTitle
            // 
            this.lbFilteredFindingTitle.AutoSize = true;
            this.lbFilteredFindingTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFilteredFindingTitle.Location = new System.Drawing.Point(4, 8);
            this.lbFilteredFindingTitle.Name = "lbFilteredFindingTitle";
            this.lbFilteredFindingTitle.Size = new System.Drawing.Size(100, 13);
            this.lbFilteredFindingTitle.TabIndex = 0;
            this.lbFilteredFindingTitle.Text = "Filtered Findings";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbSourceSignatures);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 63);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source Signatures";
            // 
            // tbSourceSignatures
            // 
            this.tbSourceSignatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSourceSignatures.Location = new System.Drawing.Point(3, 16);
            this.tbSourceSignatures.Multiline = true;
            this.tbSourceSignatures.Name = "tbSourceSignatures";
            this.tbSourceSignatures.Size = new System.Drawing.Size(348, 44);
            this.tbSourceSignatures.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(354, 135);
            this.splitContainer1.SplitterDistance = 63;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbSinkSignatures);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(354, 68);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sink Signatures";
            // 
            // tbSinkSignatures
            // 
            this.tbSinkSignatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSinkSignatures.Location = new System.Drawing.Point(3, 16);
            this.tbSinkSignatures.Multiline = true;
            this.tbSinkSignatures.Name = "tbSinkSignatures";
            this.tbSinkSignatures.Size = new System.Drawing.Size(348, 49);
            this.tbSinkSignatures.TabIndex = 1;
            // 
            // llCalculateFindings
            // 
            this.llCalculateFindings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.llCalculateFindings.AutoSize = true;
            this.llCalculateFindings.Location = new System.Drawing.Point(249, 8);
            this.llCalculateFindings.Name = "llCalculateFindings";
            this.llCalculateFindings.Size = new System.Drawing.Size(109, 13);
            this.llCalculateFindings.TabIndex = 3;
            this.llCalculateFindings.TabStop = true;
            this.llCalculateFindings.Text = "Re-calculate Findings";
            this.llCalculateFindings.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llCalculateFindings_LinkClicked);
            // 
            // findingsViewer_Results
            // 
            this.findingsViewer_Results._ShowNoEnginesLoadedAlert = false;
            this.findingsViewer_Results._SimpleViewMode = true;
            this.findingsViewer_Results.Dock = System.Windows.Forms.DockStyle.Fill;
            this.findingsViewer_Results.Location = new System.Drawing.Point(3, 3);
            this.findingsViewer_Results.Name = "findingsViewer_Results";
            this.findingsViewer_Results.Size = new System.Drawing.Size(354, 135);
            this.findingsViewer_Results.TabIndex = 4;
            // 
            // tableList_LoadedFindingsDetails
            // 
            this.tableList_LoadedFindingsDetails._DefaultColumnsTitles = "name,value";
            this.tableList_LoadedFindingsDetails._Title = "";
            this.tableList_LoadedFindingsDetails.AllowDrop = true;
            this.tableList_LoadedFindingsDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_LoadedFindingsDetails.Location = new System.Drawing.Point(3, 3);
            this.tableList_LoadedFindingsDetails.Name = "tableList_LoadedFindingsDetails";
            this.tableList_LoadedFindingsDetails.Size = new System.Drawing.Size(354, 135);
            this.tableList_LoadedFindingsDetails.TabIndex = 5;
            this.tableList_LoadedFindingsDetails._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this.tableList_LoadedFindingsDetails__onTableListDrop);
            // 
            // tcFilteredFindings
            // 
            this.tcFilteredFindings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcFilteredFindings.Controls.Add(this.tpResults);
            this.tcFilteredFindings.Controls.Add(this.tpSignatureFilters);
            this.tcFilteredFindings.Controls.Add(this.tpLoadedFindingsDetails);
            this.tcFilteredFindings.Location = new System.Drawing.Point(0, 30);
            this.tcFilteredFindings.Name = "tcFilteredFindings";
            this.tcFilteredFindings.SelectedIndex = 0;
            this.tcFilteredFindings.Size = new System.Drawing.Size(368, 167);
            this.tcFilteredFindings.TabIndex = 6;
            // 
            // tpResults
            // 
            this.tpResults.Controls.Add(this.findingsViewer_Results);
            this.tpResults.Location = new System.Drawing.Point(4, 22);
            this.tpResults.Name = "tpResults";
            this.tpResults.Padding = new System.Windows.Forms.Padding(3);
            this.tpResults.Size = new System.Drawing.Size(360, 141);
            this.tpResults.TabIndex = 2;
            this.tpResults.Text = "results";
            this.tpResults.UseVisualStyleBackColor = true;
            // 
            // tpSignatureFilters
            // 
            this.tpSignatureFilters.Controls.Add(this.splitContainer1);
            this.tpSignatureFilters.Location = new System.Drawing.Point(4, 22);
            this.tpSignatureFilters.Name = "tpSignatureFilters";
            this.tpSignatureFilters.Padding = new System.Windows.Forms.Padding(3);
            this.tpSignatureFilters.Size = new System.Drawing.Size(360, 141);
            this.tpSignatureFilters.TabIndex = 0;
            this.tpSignatureFilters.Text = "signature filters";
            this.tpSignatureFilters.UseVisualStyleBackColor = true;
            // 
            // tpLoadedFindingsDetails
            // 
            this.tpLoadedFindingsDetails.Controls.Add(this.tableList_LoadedFindingsDetails);
            this.tpLoadedFindingsDetails.Location = new System.Drawing.Point(4, 22);
            this.tpLoadedFindingsDetails.Name = "tpLoadedFindingsDetails";
            this.tpLoadedFindingsDetails.Padding = new System.Windows.Forms.Padding(3);
            this.tpLoadedFindingsDetails.Size = new System.Drawing.Size(360, 141);
            this.tpLoadedFindingsDetails.TabIndex = 1;
            this.tpLoadedFindingsDetails.Text = "loaded findings details";
            this.tpLoadedFindingsDetails.UseVisualStyleBackColor = true;
            // 
            // ascx_FilteredFindings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcFilteredFindings);
            this.Controls.Add(this.llCalculateFindings);
            this.Controls.Add(this.lbFilteredFindingTitle);
            this.Name = "ascx_FilteredFindings";
            this.Size = new System.Drawing.Size(368, 197);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tcFilteredFindings.ResumeLayout(false);
            this.tpResults.ResumeLayout(false);
            this.tpSignatureFilters.ResumeLayout(false);
            this.tpLoadedFindingsDetails.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbFilteredFindingTitle;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbSourceSignatures;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbSinkSignatures;
        private System.Windows.Forms.LinkLabel llCalculateFindings;
        private O2.Views.ASCX.O2Findings.ascx_FindingsViewer findingsViewer_Results;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_LoadedFindingsDetails;
        private System.Windows.Forms.TabControl tcFilteredFindings;
        private System.Windows.Forms.TabPage tpSignatureFilters;
        private System.Windows.Forms.TabPage tpLoadedFindingsDetails;
        private System.Windows.Forms.TabPage tpResults;
    }
}
