// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2File:ascx_HostLocalWebsite.cs

using FluentSharp.CoreLib.API;
using FluentSharp.WinForms.Controls;

namespace O2.Tool.HostLocalWebsite.ascx
{
    partial class ascx_HostLocalWebsite
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
            onDispose();
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btStartWebService = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbSettings_Path = new System.Windows.Forms.TextBox();
            this.tbSettings_Exe = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbSettings_VPath = new System.Windows.Forms.TextBox();
            this.tbSettings_Port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.wbWebServices = new System.Windows.Forms.WebBrowser();
            this.btSetTestEnvironment = new System.Windows.Forms.Button();
            this.tbUrlToLoad = new System.Windows.Forms.TextBox();
            this.btReloadUrl = new System.Windows.Forms.Button();
            this.ascx_DropObject1 = new DropObject();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btStartWebService
            // 
            this.btStartWebService.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btStartWebService.Location = new System.Drawing.Point(189, 4);
            this.btStartWebService.Name = "btStartWebService";
            this.btStartWebService.Size = new System.Drawing.Size(181, 78);
            this.btStartWebService.TabIndex = 1;
            this.btStartWebService.Text = "Step 2: Start or Restart Web Site";
            this.btStartWebService.UseVisualStyleBackColor = true;
            this.btStartWebService.Click += new System.EventHandler(this.btStartWebService_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbSettings_Path);
            this.groupBox1.Controls.Add(this.tbSettings_Exe);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbSettings_VPath);
            this.groupBox1.Controls.Add(this.tbSettings_Port);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(423, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(495, 78);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Config Settings";
            // 
            // tbSettings_Path
            // 
            this.tbSettings_Path.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSettings_Path.Location = new System.Drawing.Point(181, 43);
            this.tbSettings_Path.Name = "tbSettings_Path";
            this.tbSettings_Path.Size = new System.Drawing.Size(308, 20);
            this.tbSettings_Path.TabIndex = 7;
            this.tbSettings_Path.TextChanged += new System.EventHandler(this.tbSettings_TextChanged);
            // 
            // tbSettings_Exe
            // 
            this.tbSettings_Exe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSettings_Exe.Location = new System.Drawing.Point(181, 23);
            this.tbSettings_Exe.Name = "tbSettings_Exe";
            this.tbSettings_Exe.Size = new System.Drawing.Size(308, 20);
            this.tbSettings_Exe.TabIndex = 6;
            this.tbSettings_Exe.TextChanged += new System.EventHandler(this.tbSettings_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(149, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Path";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(150, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Exe";
            // 
            // tbSettings_VPath
            // 
            this.tbSettings_VPath.Location = new System.Drawing.Point(42, 43);
            this.tbSettings_VPath.Name = "tbSettings_VPath";
            this.tbSettings_VPath.Size = new System.Drawing.Size(107, 20);
            this.tbSettings_VPath.TabIndex = 3;
            this.tbSettings_VPath.TextChanged += new System.EventHandler(this.tbSettings_TextChanged);
            // 
            // tbSettings_Port
            // 
            this.tbSettings_Port.Location = new System.Drawing.Point(42, 23);
            this.tbSettings_Port.Name = "tbSettings_Port";
            this.tbSettings_Port.Size = new System.Drawing.Size(40, 20);
            this.tbSettings_Port.TabIndex = 2;
            this.tbSettings_Port.TextChanged += new System.EventHandler(this.tbSettings_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "VPath";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // wbWebServices
            // 
            this.wbWebServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wbWebServices.Location = new System.Drawing.Point(4, 118);
            this.wbWebServices.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbWebServices.Name = "wbWebServices";
            this.wbWebServices.Size = new System.Drawing.Size(1052, 395);
            this.wbWebServices.TabIndex = 3;
            // 
            // btSetTestEnvironment
            // 
            this.btSetTestEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSetTestEnvironment.Location = new System.Drawing.Point(924, 8);
            this.btSetTestEnvironment.Name = "btSetTestEnvironment";
            this.btSetTestEnvironment.Size = new System.Drawing.Size(132, 23);
            this.btSetTestEnvironment.TabIndex = 4;
            this.btSetTestEnvironment.Text = "Load Test Environment";
            this.btSetTestEnvironment.UseVisualStyleBackColor = true;
            this.btSetTestEnvironment.Visible = false;
            this.btSetTestEnvironment.Click += new System.EventHandler(this.btSetTestEnvironment_Click);
            // 
            // tbUrlToLoad
            // 
            this.tbUrlToLoad.Location = new System.Drawing.Point(104, 95);
            this.tbUrlToLoad.Name = "tbUrlToLoad";
            this.tbUrlToLoad.Size = new System.Drawing.Size(845, 20);
            this.tbUrlToLoad.TabIndex = 5;
            this.tbUrlToLoad.TextChanged += new System.EventHandler(this.tbUrlToLoad_TextChanged);
            // 
            // btReloadUrl
            // 
            this.btReloadUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btReloadUrl.Location = new System.Drawing.Point(955, 95);
            this.btReloadUrl.Name = "btReloadUrl";
            this.btReloadUrl.Size = new System.Drawing.Size(101, 20);
            this.btReloadUrl.TabIndex = 6;
            this.btReloadUrl.Text = "Reload";
            this.btReloadUrl.UseVisualStyleBackColor = true;
            this.btReloadUrl.Click += new System.EventHandler(this.btReloadUrl_Click);
            // 
            // ascx_DropObject1
            // 
            this.ascx_DropObject1.AllowDrop = true;
            this.ascx_DropObject1.BackColor = System.Drawing.Color.Maroon;
            this.ascx_DropObject1.ForeColor = System.Drawing.Color.White;
            this.ascx_DropObject1.Location = new System.Drawing.Point(6, 4);
            this.ascx_DropObject1.Name = "ascx_DropObject1";
            this.ascx_DropObject1.Size = new System.Drawing.Size(177, 78);
            this.ascx_DropObject1.TabIndex = 7;
            this.ascx_DropObject1.Text = "Step 1: Drop Here Folder With Website to Host ";
            this.ascx_DropObject1.eDnDAction_ObjectDataReceived_Event += new Callbacks.dMethod_Object(this.ascx_DropObject1_eDnDAction_ObjectDataReceived_Event);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Url (loaded below):";
            // 
            // ascx_HostLocalWebsite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ascx_DropObject1);
            this.Controls.Add(this.btReloadUrl);
            this.Controls.Add(this.tbUrlToLoad);
            this.Controls.Add(this.btSetTestEnvironment);
            this.Controls.Add(this.wbWebServices);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btStartWebService);
            this.Name = "ascx_HostLocalWebsite";
            this.Size = new System.Drawing.Size(1059, 516);
            this.Load += new System.EventHandler(this.ascx_WebServiceScan_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btStartWebService;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbSettings_Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbSettings_Path;
        private System.Windows.Forms.TextBox tbSettings_Exe;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbSettings_VPath;
        private System.Windows.Forms.WebBrowser wbWebServices;
        private System.Windows.Forms.Button btSetTestEnvironment;
        private System.Windows.Forms.TextBox tbUrlToLoad;
        private System.Windows.Forms.Button btReloadUrl;
        private DropObject ascx_DropObject1;
        private System.Windows.Forms.Label label5;
    }
}
