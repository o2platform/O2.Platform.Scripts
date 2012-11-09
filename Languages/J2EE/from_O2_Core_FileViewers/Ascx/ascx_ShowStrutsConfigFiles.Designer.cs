// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_ShowStrutsConfigFiles
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.lbLoadedFiles = new System.Windows.Forms.ListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.j2EE_web_xml = new O2.Core.FileViewers.Ascx.ascx_J2EE_web_xml();
            this.validator_xml1 = new O2.Core.FileViewers.Ascx.ascx_Validation_xml();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.struts_config_xml1 = new O2.Core.FileViewers.Ascx.ascx_Struts_config_xml();
            this.tilesDefinition_xml1 = new O2.Core.FileViewers.Ascx.ascx_TilesDefinition_xml();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1101, 588);
            this.splitContainer1.SplitterDistance = 466;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "drop config files below";
            // 
            // lbLoadedFiles
            // 
            this.lbLoadedFiles.AllowDrop = true;
            this.lbLoadedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLoadedFiles.FormattingEnabled = true;
            this.lbLoadedFiles.Location = new System.Drawing.Point(3, 24);
            this.lbLoadedFiles.Name = "lbLoadedFiles";
            this.lbLoadedFiles.Size = new System.Drawing.Size(456, 173);
            this.lbLoadedFiles.TabIndex = 0;
            this.lbLoadedFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbLoadedFiles_DragDrop);
            this.lbLoadedFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbLoadedFiles_DragEnter);
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
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
            this.splitContainer2.Size = new System.Drawing.Size(631, 588);
            this.splitContainer2.SplitterDistance = 304;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.j2EE_web_xml);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.validator_xml1);
            this.splitContainer3.Size = new System.Drawing.Size(304, 588);
            this.splitContainer3.SplitterDistance = 275;
            this.splitContainer3.TabIndex = 0;
            // 
            // j2EE_web_xml
            // 
            this.j2EE_web_xml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.j2EE_web_xml.Location = new System.Drawing.Point(0, 0);
            this.j2EE_web_xml.Name = "j2EE_web_xml";
            this.j2EE_web_xml.Size = new System.Drawing.Size(300, 271);
            this.j2EE_web_xml.TabIndex = 0;
            // 
            // validator_xml1
            // 
            this.validator_xml1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.validator_xml1.Location = new System.Drawing.Point(0, 0);
            this.validator_xml1.Name = "validator_xml1";
            this.validator_xml1.Size = new System.Drawing.Size(300, 305);
            this.validator_xml1.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.struts_config_xml1);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.tilesDefinition_xml1);
            this.splitContainer4.Size = new System.Drawing.Size(323, 588);
            this.splitContainer4.SplitterDistance = 275;
            this.splitContainer4.TabIndex = 0;
            // 
            // struts_config_xml1
            // 
            this.struts_config_xml1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.struts_config_xml1.Location = new System.Drawing.Point(0, 0);
            this.struts_config_xml1.Name = "struts_config_xml1";
            this.struts_config_xml1.Size = new System.Drawing.Size(319, 271);
            this.struts_config_xml1.TabIndex = 0;
            // 
            // tilesDefinition_xml1
            // 
            this.tilesDefinition_xml1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilesDefinition_xml1.Location = new System.Drawing.Point(0, 0);
            this.tilesDefinition_xml1.Name = "tilesDefinition_xml1";
            this.tilesDefinition_xml1.Size = new System.Drawing.Size(319, 305);
            this.tilesDefinition_xml1.TabIndex = 0;
            // 
            // splitContainer5
            // 
            this.splitContainer5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.lbLoadedFiles);
            this.splitContainer5.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.treeView1);
            this.splitContainer5.Panel2.Controls.Add(this.label2);
            this.splitContainer5.Size = new System.Drawing.Size(466, 588);
            this.splitContainer5.SplitterDistance = 209;
            this.splitContainer5.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mapped data";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(6, 27);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(453, 266);
            this.treeView1.TabIndex = 3;
            // 
            // ascx_ShowStrutsConfigFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ascx_ShowStrutsConfigFiles";
            this.Size = new System.Drawing.Size(1101, 588);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel1.PerformLayout();
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            this.splitContainer5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbLoadedFiles;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private ascx_J2EE_web_xml j2EE_web_xml;
        private ascx_Validation_xml validator_xml1;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private ascx_Struts_config_xml struts_config_xml1;
        private ascx_TilesDefinition_xml tilesDefinition_xml1;
        private System.Windows.Forms.SplitContainer splitContainer5;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label2;
    }
}
