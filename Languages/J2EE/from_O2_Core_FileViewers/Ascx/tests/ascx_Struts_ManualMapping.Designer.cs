// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx.tests
{
    partial class ascx_StrutsMappings_ManualMapping
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.strutsMappings = new O2.Core.FileViewers.Ascx.ascx_StrutsMappings();
            this.label4 = new System.Windows.Forms.Label();
            this.btCreateFindingsFromStrutsMapings = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.findingsViewer_FromStrutsMappings = new O2.Views.ASCX.O2Findings.ascx_FindingsViewer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.findingsViewer_SourceFindings = new O2.Views.ASCX.O2Findings.ascx_FindingsViewer();
            this.cbShowConsolidatedView = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btMapStrutsFindings = new System.Windows.Forms.Button();
            this.findingsViewer_MappedFindings = new O2.Views.ASCX.O2Findings.ascx_FindingsViewer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Struts Mappings";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(778, 504);
            this.splitContainer1.SplitterDistance = 251;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.strutsMappings);
            this.splitContainer3.Panel1.Controls.Add(this.label4);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.btCreateFindingsFromStrutsMapings);
            this.splitContainer3.Panel2.Controls.Add(this.label5);
            this.splitContainer3.Panel2.Controls.Add(this.findingsViewer_FromStrutsMappings);
            this.splitContainer3.Size = new System.Drawing.Size(778, 251);
            this.splitContainer3.SplitterDistance = 325;
            this.splitContainer3.TabIndex = 2;
            // 
            // strutsMappings
            // 
            this.strutsMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.strutsMappings.Location = new System.Drawing.Point(6, 31);
            this.strutsMappings.Name = "strutsMappings";
            this.strutsMappings.Size = new System.Drawing.Size(312, 213);
            this.strutsMappings.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Struts Mappings";
            // 
            // btCreateFindingsFromStrutsMapings
            // 
            this.btCreateFindingsFromStrutsMapings.Location = new System.Drawing.Point(156, 1);
            this.btCreateFindingsFromStrutsMapings.Name = "btCreateFindingsFromStrutsMapings";
            this.btCreateFindingsFromStrutsMapings.Size = new System.Drawing.Size(197, 23);
            this.btCreateFindingsFromStrutsMapings.TabIndex = 4;
            this.btCreateFindingsFromStrutsMapings.Text = "Create Findings from Struts Mappings";
            this.btCreateFindingsFromStrutsMapings.UseVisualStyleBackColor = true;
            this.btCreateFindingsFromStrutsMapings.Click += new System.EventHandler(this.btCreateFindingsFromStrutsMapings_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Struts Findings / Traces";
            // 
            // findingsViewer_FromStrutsMappings
            // 
            this.findingsViewer_FromStrutsMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findingsViewer_FromStrutsMappings.Location = new System.Drawing.Point(2, 31);
            this.findingsViewer_FromStrutsMappings.Name = "findingsViewer_FromStrutsMappings";
            this.findingsViewer_FromStrutsMappings.Size = new System.Drawing.Size(440, 216);
            this.findingsViewer_FromStrutsMappings.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.findingsViewer_SourceFindings);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.cbShowConsolidatedView);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Panel2.Controls.Add(this.btMapStrutsFindings);
            this.splitContainer2.Panel2.Controls.Add(this.findingsViewer_MappedFindings);
            this.splitContainer2.Size = new System.Drawing.Size(778, 249);
            this.splitContainer2.SplitterDistance = 327;
            this.splitContainer2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Source Findings";
            // 
            // findingsViewer_SourceFindings
            // 
            this.findingsViewer_SourceFindings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findingsViewer_SourceFindings.Location = new System.Drawing.Point(1, 33);
            this.findingsViewer_SourceFindings.Name = "findingsViewer_SourceFindings";
            this.findingsViewer_SourceFindings.Size = new System.Drawing.Size(319, 209);
            this.findingsViewer_SourceFindings.TabIndex = 0;
            // 
            // cbShowConsolidatedView
            // 
            this.cbShowConsolidatedView.AutoSize = true;
            this.cbShowConsolidatedView.Checked = true;
            this.cbShowConsolidatedView.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowConsolidatedView.Location = new System.Drawing.Point(242, 8);
            this.cbShowConsolidatedView.Name = "cbShowConsolidatedView";
            this.cbShowConsolidatedView.Size = new System.Drawing.Size(143, 17);
            this.cbShowConsolidatedView.TabIndex = 3;
            this.cbShowConsolidatedView.Text = "Show Consolidated View";
            this.cbShowConsolidatedView.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(0, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mapped Findings";
            // 
            // btMapStrutsFindings
            // 
            this.btMapStrutsFindings.Location = new System.Drawing.Point(122, 3);
            this.btMapStrutsFindings.Name = "btMapStrutsFindings";
            this.btMapStrutsFindings.Size = new System.Drawing.Size(113, 23);
            this.btMapStrutsFindings.TabIndex = 2;
            this.btMapStrutsFindings.Text = "map Struts findings";
            this.btMapStrutsFindings.UseVisualStyleBackColor = true;
            this.btMapStrutsFindings.Click += new System.EventHandler(this.btMapStrutsFindings_Click);
            // 
            // findingsViewer_MappedFindings
            // 
            this.findingsViewer_MappedFindings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.findingsViewer_MappedFindings.Location = new System.Drawing.Point(3, 33);
            this.findingsViewer_MappedFindings.Name = "findingsViewer_MappedFindings";
            this.findingsViewer_MappedFindings.Size = new System.Drawing.Size(439, 202);
            this.findingsViewer_MappedFindings.TabIndex = 1;
            // 
            // ascx_StrutsMappings_ManualMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Name = "ascx_StrutsMappings_ManualMapping";
            this.Size = new System.Drawing.Size(778, 504);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private O2.Views.ASCX.O2Findings.ascx_FindingsViewer findingsViewer_SourceFindings;
        private O2.Views.ASCX.O2Findings.ascx_FindingsViewer findingsViewer_MappedFindings;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btMapStrutsFindings;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btCreateFindingsFromStrutsMapings;
        private System.Windows.Forms.Label label5;
        private O2.Views.ASCX.O2Findings.ascx_FindingsViewer findingsViewer_FromStrutsMappings;
        private System.Windows.Forms.CheckBox cbShowConsolidatedView;
        private ascx_StrutsMappings strutsMappings;
    }
}
