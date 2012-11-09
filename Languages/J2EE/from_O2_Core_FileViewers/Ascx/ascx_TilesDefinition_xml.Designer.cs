// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_TilesDefinition_xml
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
            this.tableList_TilesDefinitions = new O2.Views.ASCX.DataViewers.ascx_TableList();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
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
            // tableList_TilesDefinitions
            // 
            this.tableList_TilesDefinitions._Title = "TilesDefinitions";
            this.tableList_TilesDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableList_TilesDefinitions.Location = new System.Drawing.Point(3, 37);
            this.tableList_TilesDefinitions.Name = "tableList_TilesDefinitions";
            this.tableList_TilesDefinitions.Size = new System.Drawing.Size(555, 397);
            this.tableList_TilesDefinitions.TabIndex = 8;
            this.tableList_TilesDefinitions._onTableListDrop += new O2.DotNetWrappers.DotNet.O2Thread.FuncVoidT1<System.Windows.Forms.DragEventArgs>(this._onTableListDrop);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Tiles definition xml files";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(403, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "(drop file to load on any control)";
            // 
            // ascx_TilesDefinition_xml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableList_TilesDefinitions);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Name = "ascx_TilesDefinition_xml";
            this.Size = new System.Drawing.Size(561, 437);
            this.Load += new System.EventHandler(this.ascx_J2EE_web_xml_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private O2.Views.ASCX.DataViewers.ascx_TableList tableList_TilesDefinitions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}
