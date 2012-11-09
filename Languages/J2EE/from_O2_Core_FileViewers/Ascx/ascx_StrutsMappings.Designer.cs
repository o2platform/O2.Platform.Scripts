// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_StrutsMappings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ascx_StrutsMappings));
            this.tvStrutsMappings = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.btOpenStrutsMappingsFile = new System.Windows.Forms.ToolStripButton();
            this.tbTargetDirectoryOrFolder = new System.Windows.Forms.ToolStripTextBox();
            this.btSaveCurrentMappings = new System.Windows.Forms.ToolStripButton();
            this.btRemoveLoadedMappings = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvStrutsMappings
            // 
            this.tvStrutsMappings.AllowDrop = true;
            this.tvStrutsMappings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tvStrutsMappings.Location = new System.Drawing.Point(0, 28);
            this.tvStrutsMappings.Name = "tvStrutsMappings";
            this.tvStrutsMappings.Size = new System.Drawing.Size(448, 404);
            this.tvStrutsMappings.TabIndex = 2;
            this.tvStrutsMappings.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvStrutsMappings_BeforeExpand);
            this.tvStrutsMappings.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvStrutsMappings_DragDrop);
            this.tvStrutsMappings.DragEnter += new System.Windows.Forms.DragEventHandler(this.tvStrutsMappings_DragEnter);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.btOpenStrutsMappingsFile,
            this.btSaveCurrentMappings,
            this.tbTargetDirectoryOrFolder,
            this.btRemoveLoadedMappings});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(451, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(99, 22);
            this.toolStripLabel1.Text = "struts mappings";
            // 
            // btOpenStrutsMappingsFile
            // 
            this.btOpenStrutsMappingsFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btOpenStrutsMappingsFile.Image = ((System.Drawing.Image)(resources.GetObject("btOpenStrutsMappingsFile.Image")));
            this.btOpenStrutsMappingsFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btOpenStrutsMappingsFile.Name = "btOpenStrutsMappingsFile";
            this.btOpenStrutsMappingsFile.Size = new System.Drawing.Size(23, 22);
            this.btOpenStrutsMappingsFile.Text = "toolStripButton1";
            this.btOpenStrutsMappingsFile.Click += new System.EventHandler(this.btOpenStrutsMappingsFile_Click);
            // 
            // tbTargetDirectoryOrFolder
            // 
            this.tbTargetDirectoryOrFolder.Name = "tbTargetDirectoryOrFolder";
            this.tbTargetDirectoryOrFolder.Size = new System.Drawing.Size(100, 25);
            // 
            // btSaveCurrentMappings
            // 
            this.btSaveCurrentMappings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btSaveCurrentMappings.Image = ((System.Drawing.Image)(resources.GetObject("btSaveCurrentMappings.Image")));
            this.btSaveCurrentMappings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btSaveCurrentMappings.Name = "btSaveCurrentMappings";
            this.btSaveCurrentMappings.Size = new System.Drawing.Size(23, 22);
            this.btSaveCurrentMappings.Text = "Save Current Mappings";
            this.btSaveCurrentMappings.Click += new System.EventHandler(this.btSaveCurrentMappings_Click);
            // 
            // btRemoveLoadedMappings
            // 
            this.btRemoveLoadedMappings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btRemoveLoadedMappings.Image = ((System.Drawing.Image)(resources.GetObject("btRemoveLoadedMappings.Image")));
            this.btRemoveLoadedMappings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btRemoveLoadedMappings.Name = "btRemoveLoadedMappings";
            this.btRemoveLoadedMappings.Size = new System.Drawing.Size(23, 22);
            this.btRemoveLoadedMappings.Text = "Remove loaded mappings";
            this.btRemoveLoadedMappings.Click += new System.EventHandler(this.btRemoveLoadedMappings_Click);
            // 
            // ascx_StrutsMappings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tvStrutsMappings);
            this.Name = "ascx_StrutsMappings";
            this.Size = new System.Drawing.Size(451, 435);
            this.Load += new System.EventHandler(this.ascx_StrutsMappings_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvStrutsMappings;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btSaveCurrentMappings;
        private System.Windows.Forms.ToolStripButton btOpenStrutsMappingsFile;
        private System.Windows.Forms.ToolStripTextBox tbTargetDirectoryOrFolder;
        private System.Windows.Forms.ToolStripButton btRemoveLoadedMappings;
    }
}
