// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.Core.FileViewers.JoinTraces;
using O2.Core.FileViewers.Struts_1_5;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.Ascx
{
    partial class ascx_StrutsMappings
    {
        public bool runOnLoad = true;        

        private string webXmlToMap = "";
        private string strutsConfigToMap = "";
        private string tilesDefinitionsToMap = "";
        private string validationXmlToMap = "";

        public void onLoad()
        {
            if (DesignMode == false && runOnLoad)
            {
                tbTargetDirectoryOrFolder.Text = DI.config.O2TempDir;
                runOnLoad = false;
            }
        }                
        public void showStrutsMappings(IStrutsMappings strutsMappings)
        {
            if (strutsMappings == null)
            {
                DI.log.error("in showStrutsMappings, strutsMappings == null");
                return;
            }

            tvStrutsMappings.Tag = strutsMappings;
            tvStrutsMappings.invokeOnThread(() => refreshTreeView());
        }

        private void refreshTreeView(IStrutsMappings _strutsMappings)
        {
            tvStrutsMappings.Tag = _strutsMappings;
            refreshTreeView();
        }

        private void refreshTreeView()
        {
            this.invokeOnThread(
                () =>
                    {
                        tvStrutsMappings.Nodes.Clear();
                        if (tvStrutsMappings.Tag != null && tvStrutsMappings.Tag is IStrutsMappings)
                        {
                            var strutsMappings = ((IStrutsMappings) tvStrutsMappings.Tag);

                            foreach (var actionServlet in strutsMappings.actionServlets)
                            {
                                var newNode = O2Forms.newTreeNode(tvStrutsMappings.Nodes, actionServlet.ToString(), 0,
                                                                  actionServlet);
                                newNode.Nodes.Add("DummyNode");
                            }
                            foreach (var otherServlet in strutsMappings.otherServlets)
                                O2Forms.newTreeNode(tvStrutsMappings.Nodes, otherServlet);
                        }
                    });
        }

        public void showStrutsMappings(string webXmlFile, string strutsConfigFile, string tilesDefinitionsFile, string validationXmlFile)
        {
            webXmlToMap = webXmlFile;
            strutsConfigToMap = strutsConfigFile;
            tilesDefinitionsToMap = tilesDefinitionsFile;
            validationXmlToMap = validationXmlFile;
            showStrutsMappings();
        }

        public void showStrutsMappings()
        {
            var strutsMappings = StrutsMappingsHelpers.calculateStrutsMapping(webXmlToMap, strutsConfigToMap, tilesDefinitionsToMap, validationXmlToMap);
            showStrutsMappings(strutsMappings);
        }

        private void loadFileOrFolder(string fileOrFolderToLoad)
        {
            if (Directory.Exists(fileOrFolderToLoad))
            {
                foreach (var file in Files.getFilesFromDir_returnFullPath(fileOrFolderToLoad, "*.xml"))
                    loadFileOrFolder(file);
            }
            else
                if (File.Exists(fileOrFolderToLoad))
                {
                    switch(Path.GetExtension(fileOrFolderToLoad).ToLower())
                    {
                        case ".o2strutsmapping":
                            loadO2StrutsMappingFile(fileOrFolderToLoad);
                            break;
                        case ".xml":
                            var fileName = Path.GetFileName(fileOrFolderToLoad);
                            if (fileName == "web.xml")
                                webXmlToMap = fileOrFolderToLoad;
                            else if (fileName == "struts-config.xml")
                                strutsConfigToMap = fileOrFolderToLoad;
                            else if (fileName == "validation.xml")
                                validationXmlToMap = fileOrFolderToLoad;
                            else if (RegEx.findStringInString(fileName, ".*tiles.*xml"))
                                tilesDefinitionsToMap = fileOrFolderToLoad;
                            else
                                return;  // don't do anything unless it is a file that has a mapping
                            showStrutsMappings();
                            break;
                    }                    
                }
        }

        private void onTreeViewBeforeExpand(TreeNode treeNode, object tagObject)
        {
            try
            {

                treeNode.Nodes.Clear();
                var tagObjectType = tagObject.GetType().FullName;
                switch (tagObjectType)
                {
                    case "O2.Kernel.Interfaces.FrameworkSupport.J2EE.KStrutsMappings_ActionServlet":
                        var strutsActionServet = (KStrutsMappings_ActionServlet)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_ActionServlet(treeNode, strutsActionServet);

                        break;
                    //case "System.Collections.Generic.List`1[[O2.Core.FileViewers.Interfaces.IStrutsMappings_Controller, O2_Core_FileViewers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]":
                    //case "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[O2.Core.FileViewers.Interfaces.IStrutsMappings_Controller, O2_Core_FileViewers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]":
                    case "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[O2.Kernel.Interfaces.FrameworkSupport.J2EE.IStrutsMappings_Controller, O2_Kernel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]":
                        var controllers = (Dictionary<string, IStrutsMappings_Controller>)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_Controllers(treeNode, controllers);
                        break;
                    case "O2.Kernel.Interfaces.FrameworkSupport.J2EE.KStrutsMappings_Controller_Path":
                    //case "O2.Core.FileViewers.Interfaces.KStrutsMappings_Controller_Path":
                        var path = (KStrutsMappings_Controller_Path)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_Controller_Path(treeNode, path);
                        break;
                    //case "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[O2.Core.FileViewers.Interfaces.IStrutsConfig_FormBean, O2_Core_FileViewers, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]":
                    case "System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[O2.Kernel.Interfaces.FrameworkSupport.J2EE.IStrutsConfig_FormBean, O2_Kernel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]":
                        var formBeans = (Dictionary<string, IStrutsConfig_FormBean>)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_FormBeans(treeNode, formBeans);
                        break;
                    case "O2.Core.FileViewers.Struts_1_5.KStrutsConfig_FormBean":
                        var formBean = (IStrutsConfig_FormBean)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_FormBean(treeNode, formBean);
                        break;
                    case "O2.Core.FileViewers.Struts_1_5.KStrutsConfig_FormBean_Field":
                        var formBeanField = (KStrutsConfig_FormBean_Field)tagObject;
                        StrutsMappingsViewHelpers.populateTreeNodeWith_FormBean_Field(treeNode, formBeanField);
                        break;
                    default:
                        DI.log.error("in onTreeViewBeforeExpand, tag type not supported: {0}", tagObjectType);
                        break;
                }
            }
            catch (Exception ex)
            {
                DI.log.error("on onTreeViewBeforeExpand: {0}", ex.Message);
            }
        }

        public IStrutsMappings getStrutsMappingObject()
        {
            return (tvStrutsMappings.Tag != null && tvStrutsMappings.Tag is IStrutsMappings)
                       ? (IStrutsMappings)tvStrutsMappings.Tag
                       : null;
        }
        
        
        public void openMappingsFile()
        {
            DI.log.info("Select file to open");
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                DI.log.info("Loading file: {0}", openFileDialog.FileName);
                loadO2StrutsMappingFile(openFileDialog.FileName);
            }
        }

        public void loadStrutsMappings(IStrutsMappings strutsMappings)
        {
            refreshTreeView(strutsMappings);
        }

        public void loadO2StrutsMappingFile(string fileToProcess)
        {
            var strutsMappings = StrutsMappingHelpers.loadO2StrutsMappingFile(fileToProcess);
            if (strutsMappings != null)
            {
                DI.log.info("Sucessfuly create struts mapping object from file: {0}", fileToProcess);
                refreshTreeView(strutsMappings);
            }
            else
                DI.log.error("There was a problem serializing Struts Mapping object saved to: {0}", fileToProcess);
        }

        public void saveCurrentMappings(string targetFileOrFolder)
        {
            var currentMappings = getStrutsMappingObject();
            StrutsMappingsHelpers.saveStrutsMappings(currentMappings, targetFileOrFolder);
        }           
    }
}
