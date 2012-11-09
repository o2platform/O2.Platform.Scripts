// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;

namespace O2.Core.FileViewers.Ascx
{
    public partial class ascx_ShowStrutsConfigFiles : UserControl
    {
        public ascx_ShowStrutsConfigFiles()
        {
            InitializeComponent();
        }

        private void lbLoadedFiles_DragEnter(object sender, DragEventArgs e)
        {
            Dnd.setEffect(e);
        }

        private void lbLoadedFiles_DragDrop(object sender, DragEventArgs e)
        {
            loadFileorFolder(Dnd.tryToGetFileOrDirectoryFromDroppedObject(e));
        }

        private void loadFileorFolder(string fileOrDirectoryToLoad)
        {
            if (Directory.Exists(fileOrDirectoryToLoad))
            {
                foreach (var file in Files.getFilesFromDir_returnFullPath(fileOrDirectoryToLoad))
                    loadFileorFolder(file);
            }
            else
            {
                var fileToLoad = fileOrDirectoryToLoad; 
                var filename = Path.GetFileName(fileToLoad);
                switch (filename)
                {
                    case "web.xml":
                        j2EE_web_xml.mapFile(fileToLoad);
                        break;
                    case "validation.xml":
                        validator_xml1.mapFile(fileToLoad);
                        break;
                    case "struts-config.xml":
                        struts_config_xml1.mapFile(fileToLoad);
                        break;
                    case "web.xml__":
                        j2EE_web_xml.mapFile(fileToLoad);
                        break;
                    default:
                        if (filename.IndexOf("tiles-definition") > -1)
                            tilesDefinition_xml1.mapFile(fileToLoad, false);
                        else
                            return;
                        break;
                }
                lbLoadedFiles.Items.Add(fileToLoad);
            }
        }
    }
}
