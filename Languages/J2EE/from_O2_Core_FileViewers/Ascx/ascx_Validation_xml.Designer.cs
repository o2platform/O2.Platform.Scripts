// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_Validation_xml
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
            this.lbValidatatorForms = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tableList_FormFields = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Location = new System.Drawing.Point(7, 33);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbValidatatorForms);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableList_FormFields);
            this.splitContainer1.Size = new System.Drawing.Size(551, 401);
            this.splitContainer1.SplitterDistance = 247;
            this.splitContainer1.TabIndex = 10;
            // 
            // lbValidatatorForms
            // 
            this.lbValidatatorForms.AllowDrop = true;
            this.lbValidatatorForms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbValidatatorForms.FormattingEnabled = true;
            this.lbValidatatorForms.Location = new System.Drawing.Point(4, 27);
            this.lbValidatatorForms.Name = "lbValidatatorForms";
            this.lbValidatatorForms.Size = new System.Drawing.Size(236, 368);
            this.lbValidatatorForms.TabIndex = 1;
            this.lbValidatatorForms.SelectedIndexChanged += new System.EventHandler(this.lbValidatatorForms_SelectedIndexChanged);
            this.lbValidatatorForms.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbValidatatorForms_DragDrop);
            this.lbValidatatorForms.DragEnter += new System.Windows.Forms.DragEventHandler(this.lbValidatatorForms_DragEnter);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(178, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Validator Forms (click to view)";
            // 
            // tableList_FormFields
            // 
            this.tableList_FormFields._Title = "Form Fields";
            this.tableList_FormFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableList_FormFields.Location = new System.Drawing.Point(0, 0);
            this.tableList_FormFields.Name = "tableList_FormFields";
            this.tableList_FormFields.Size = new System.Drawing.Size(296, 397);
            this.tableList_FormFields.TabIndex = 8;
            this.tableList_FormFields._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Validator.xml file";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(401, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "(drop file to load on any control)";
            // 
            // ascx_Validation_xml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Name = "ascx_Validation_xml";
            this.Size = new System.Drawing.Size(561, 437);
            this.Load += new System.EventHandler(this.ascx_J2EE_web_xml_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_FormFields;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lbValidatatorForms;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
    }
}
