// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//O2File:ascx_JavaExecution.Designer.cs
//O2File:ascx_JavaExecution.Controllers.cs
//O2File:JavaCompile.cs
//O2File:API_IKVM.cs

namespace O2.XRules.Database.APIs.IKVM 
{
    public partial class ascx_JavaExecution : UserControl
    {
        public ascx_JavaExecution()
        {
            InitializeComponent();
        }

        private void ascx_JavaExecution_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        private void llLoadDefaultSetOfFilesToConvert_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            loadDefaultSetOfFilesToConvert();
        }

        private void btCreateJarStubFiles_Click(object sender, EventArgs e)
        {
            createJarStubFiles();
        }

        private void llDeleteJarStubs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            deleteJarStubs();
        }

        private void llDeleteEmptyJarStubs_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            deleteEmptyJarStubs();
        }

        private void directoryToDropJarsToConvertIntoDotNetAssemblies__onTreeViewDrop(string droppedFileOrFolder)
        {
            convertJarToDotNetAssembly(droppedFileOrFolder, directoryToDropJarsToConvertIntoDotNetAssemblies.getCurrentDirectory());
        }                                
    }
}
