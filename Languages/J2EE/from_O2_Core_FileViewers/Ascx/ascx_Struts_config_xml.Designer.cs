// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_Struts_config_xml
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableList_FormBeans = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.tableList_GlobalForwards = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.tableList_ActionMappings = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.tableList_PlugIns = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
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
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(7, 32);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(551, 402);
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
            this.splitContainer2.Panel1.Controls.Add(this.tableList_FormBeans);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableList_GlobalForwards);
            this.splitContainer2.Size = new System.Drawing.Size(247, 402);
            this.splitContainer2.SplitterDistance = 279;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableList_FormBeans
            // 
            this.tableList_FormBeans._Title = "Form Beans";
            this.tableList_FormBeans.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_FormBeans.Location = new System.Drawing.Point(0, 0);
            this.tableList_FormBeans.Name = "tableList_FormBeans";
            this.tableList_FormBeans.Size = new System.Drawing.Size(243, 275);
            this.tableList_FormBeans.TabIndex = 5;
            this.tableList_FormBeans._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // tableList_GlobalForwards
            // 
            this.tableList_GlobalForwards._Title = "Global Forwards";
            this.tableList_GlobalForwards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_GlobalForwards.Location = new System.Drawing.Point(0, 0);
            this.tableList_GlobalForwards.Name = "tableList_GlobalForwards";
            this.tableList_GlobalForwards.Size = new System.Drawing.Size(243, 115);
            this.tableList_GlobalForwards.TabIndex = 6;
            this.tableList_GlobalForwards._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
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
            this.splitContainer3.Panel1.Controls.Add(this.tableList_ActionMappings);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.tableList_PlugIns);
            this.splitContainer3.Size = new System.Drawing.Size(300, 402);
            this.splitContainer3.SplitterDistance = 279;
            this.splitContainer3.TabIndex = 9;
            // 
            // tableList_ActionMappings
            // 
            this.tableList_ActionMappings._Title = "Action Mappings";
            this.tableList_ActionMappings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_ActionMappings.Location = new System.Drawing.Point(0, 0);
            this.tableList_ActionMappings.Name = "tableList_ActionMappings";
            this.tableList_ActionMappings.Size = new System.Drawing.Size(296, 275);
            this.tableList_ActionMappings.TabIndex = 8;
            this.tableList_ActionMappings._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // tableList_PlugIns
            // 
            this.tableList_PlugIns._Title = "Plug-Ins";
            this.tableList_PlugIns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_PlugIns.Location = new System.Drawing.Point(0, 0);
            this.tableList_PlugIns.Name = "tableList_PlugIns";
            this.tableList_PlugIns.Size = new System.Drawing.Size(296, 115);
            this.tableList_PlugIns.TabIndex = 9;
            this.tableList_PlugIns._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Struts-config.xml file";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(401, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "(drop file to load on any control)";
            // 
            // ascx_Struts_config_xml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ascx_Struts_config_xml";
            this.Size = new System.Drawing.Size(561, 437);
            this.Load += new System.EventHandler(this.ascx_J2EE_web_xml_Load);
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

        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_FormBeans;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_GlobalForwards;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_ActionMappings;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_PlugIns;
    }
}
