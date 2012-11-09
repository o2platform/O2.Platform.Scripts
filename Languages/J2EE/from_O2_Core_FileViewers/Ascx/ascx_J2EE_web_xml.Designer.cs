// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_J2EE_web_xml
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
            this.btMapLoadedFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbListenerClass = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbDisplayName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbDescription = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableList_Filter = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.tableList_FilterMappings = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tableList_Servlets = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.tableList_ServletMappings = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "J2EE Web.XML file";
            // 
            // btMapLoadedFile
            // 
            this.btMapLoadedFile.Location = new System.Drawing.Point(304, 11);
            this.btMapLoadedFile.Name = "btMapLoadedFile";
            this.btMapLoadedFile.Size = new System.Drawing.Size(103, 23);
            this.btMapLoadedFile.TabIndex = 2;
            this.btMapLoadedFile.Text = "Map Loaded File";
            this.btMapLoadedFile.UseVisualStyleBackColor = true;
            this.btMapLoadedFile.Click += new System.EventHandler(this.btMapLoadedFile_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "description:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbListenerClass);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lbDisplayName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lbDescription);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btMapLoadedFile);
            this.groupBox1.Location = new System.Drawing.Point(7, 31);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(413, 78);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // lbListenerClass
            // 
            this.lbListenerClass.AutoSize = true;
            this.lbListenerClass.Location = new System.Drawing.Point(79, 54);
            this.lbListenerClass.Name = "lbListenerClass";
            this.lbListenerClass.Size = new System.Drawing.Size(16, 13);
            this.lbListenerClass.TabIndex = 8;
            this.lbListenerClass.Text = "...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "listener class:";
            // 
            // lbDisplayName
            // 
            this.lbDisplayName.AutoSize = true;
            this.lbDisplayName.Location = new System.Drawing.Point(79, 35);
            this.lbDisplayName.Name = "lbDisplayName";
            this.lbDisplayName.Size = new System.Drawing.Size(16, 13);
            this.lbDisplayName.TabIndex = 6;
            this.lbDisplayName.Text = "...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "display name:";
            // 
            // lbDescription
            // 
            this.lbDescription.AutoSize = true;
            this.lbDescription.Location = new System.Drawing.Point(79, 16);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(16, 13);
            this.lbDescription.TabIndex = 4;
            this.lbDescription.Text = "...";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(7, 115);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(551, 319);
            this.splitContainer1.SplitterDistance = 247;
            this.splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tableList_Filter);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableList_FilterMappings);
            this.splitContainer2.Size = new System.Drawing.Size(247, 319);
            this.splitContainer2.SplitterDistance = 193;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableList_Filter
            // 
            this.tableList_Filter._Title = "Filter";
            this.tableList_Filter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_Filter.Location = new System.Drawing.Point(0, 0);
            this.tableList_Filter.Name = "tableList_Filter";
            this.tableList_Filter.Size = new System.Drawing.Size(243, 189);
            this.tableList_Filter.TabIndex = 5;
            this.tableList_Filter._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // tableList_FilterMappings
            // 
            this.tableList_FilterMappings._Title = "Filter Mappings";
            this.tableList_FilterMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_FilterMappings.Location = new System.Drawing.Point(0, 0);
            this.tableList_FilterMappings.Name = "tableList_FilterMappings";
            this.tableList_FilterMappings.Size = new System.Drawing.Size(243, 118);
            this.tableList_FilterMappings.TabIndex = 6;
            this.tableList_FilterMappings._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
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
            this.splitContainer3.Panel1.Controls.Add(this.tableList_Servlets);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tableList_ServletMappings);
            this.splitContainer3.Size = new System.Drawing.Size(300, 319);
            this.splitContainer3.SplitterDistance = 191;
            this.splitContainer3.TabIndex = 0;
            // 
            // tableList_Servlets
            // 
            this.tableList_Servlets._Title = "Servlets";
            this.tableList_Servlets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_Servlets.Location = new System.Drawing.Point(0, 0);
            this.tableList_Servlets.Name = "tableList_Servlets";
            this.tableList_Servlets.Size = new System.Drawing.Size(296, 187);
            this.tableList_Servlets.TabIndex = 8;
            this.tableList_Servlets._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // tableList_ServletMappings
            // 
            this.tableList_ServletMappings._Title = "Servlet Mappings";
            this.tableList_ServletMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_ServletMappings.Location = new System.Drawing.Point(0, 0);
            this.tableList_ServletMappings.Name = "tableList_ServletMappings";
            this.tableList_ServletMappings.Size = new System.Drawing.Size(296, 120);
            this.tableList_ServletMappings.TabIndex = 9;
            this.tableList_ServletMappings._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(401, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "(drop file to load on any control)";
            // 
            // ascx_J2EE_web_xml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "ascx_J2EE_web_xml";
            this.Size = new System.Drawing.Size(561, 437);
            this.Load += new System.EventHandler(this.ascx_J2EE_web_xml_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btMapLoadedFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Label lbDisplayName;
        private System.Windows.Forms.Label label4;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_Filter;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_FilterMappings;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_Servlets;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_ServletMappings;
        private System.Windows.Forms.Label lbListenerClass;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label3;
    }
}
