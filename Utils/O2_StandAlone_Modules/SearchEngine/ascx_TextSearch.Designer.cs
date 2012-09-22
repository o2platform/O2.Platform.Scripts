// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using O2.External.SharpDevelop.Ascx;
using O2.Kernel.CodeUtils;
using O2.Views.ASCX.CoreControls;
using O2.Views.ASCX.SourceCodeEdit;

//O2File:ascx_TextSearch.cs

namespace O2.Tool.SearchEngine.Ascx
{
    partial class ascx_TextSearch
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTextToSearch2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbTextToSearch1 = new System.Windows.Forms.TextBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.asceSourceCodeEditor = new ascx_SourceCodeEditor();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(856, 405);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.TabIndex = 0;
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
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.label2);
            this.splitContainer2.Panel2.Controls.Add(this.tbTextToSearch2);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2.Controls.Add(this.tbTextToSearch1);
            this.splitContainer2.Size = new System.Drawing.Size(258, 405);
            this.splitContainer2.SplitterDistance = 223;
            this.splitContainer2.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Loaded Files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Search for:";
            // 
            // tbTextToSearch2
            // 
            this.tbTextToSearch2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTextToSearch2.Location = new System.Drawing.Point(75, 57);
            this.tbTextToSearch2.Name = "tbTextToSearch2";
            this.tbTextToSearch2.Size = new System.Drawing.Size(176, 20);
            this.tbTextToSearch2.TabIndex = 2;
            this.tbTextToSearch2.TextChanged += new System.EventHandler(this.tbTextToSearch2_TextChanged);
            this.tbTextToSearch2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTextToSearch2_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search for:";
            // 
            // tbTextToSearch1
            // 
            this.tbTextToSearch1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTextToSearch1.Location = new System.Drawing.Point(75, 31);
            this.tbTextToSearch1.Name = "tbTextToSearch1";
            this.tbTextToSearch1.Size = new System.Drawing.Size(176, 20);
            this.tbTextToSearch1.TabIndex = 0;
            //this.tbTextToSearch1.TextChanged += new System.EventHandler(this.tbTextToSearch1_TextChanged);
            //this.tbTextToSearch1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTextToSearch1_KeyPress);
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.asceSourceCodeEditor);
            this.splitContainer3.Size = new System.Drawing.Size(594, 405);
            this.splitContainer3.SplitterDistance = 203;
            this.splitContainer3.TabIndex = 0;
            // 
            // asceSourceCodeEditor
            // 
            this.asceSourceCodeEditor.AllowDrop = true;
            this.asceSourceCodeEditor.BackColor = System.Drawing.SystemColors.Control;
            this.asceSourceCodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.asceSourceCodeEditor.ForeColor = System.Drawing.Color.Black;
            this.asceSourceCodeEditor.Location = new System.Drawing.Point(0, 0);
            this.asceSourceCodeEditor.Name = "asceSourceCodeEditor";
            this.asceSourceCodeEditor.Size = new System.Drawing.Size(590, 194);
            this.asceSourceCodeEditor.TabIndex = 0;
            // 
            // ascx_TextSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ascx_TextSearch";
            this.Size = new System.Drawing.Size(856, 405);
            this.Load += new System.EventHandler(this.ascx_TextSearch_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTextToSearch2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbTextToSearch1;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label3;
        private ascx_SourceCodeEditor asceSourceCodeEditor;
    }
}
