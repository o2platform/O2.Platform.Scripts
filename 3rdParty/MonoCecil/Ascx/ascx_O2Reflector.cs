// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet; 
using O2.DotNetWrappers.Windows;
using O2.Kernel.CodeUtils;
using O2.Kernel.InterfacesBaseImpl;
using O2.Kernel;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:Ascx_O2Reflector.Designer.cs
//O2File:../AssemblyAnalysisImpl.cs
namespace O2.External.O2Mono.Ascx
{
	public class ascx_O2Reflector_test
	{ 
		public void test_ascx_O2Reflector()
		{
			"ascx_O2Reflector".popupWindow().add_Control<ascx_O2Reflector>();
		}
	}
    public partial class ascx_O2Reflector : UserControl
    {
        #region AvailableEngines enum

        public enum AvailableEngines
        {
            Cir,
            MonoCecil,
            Reflection
        }
 
        #endregion

        private AvailableEngines engineToUse = AvailableEngines.MonoCecil;
        public List<string> loadedAssemblies = new List<string>();
		public AssemblyAnalysisImpl assemblyAnalysis {get;set;}
		
        public ascx_O2Reflector()
        {
        	assemblyAnalysis = new AssemblyAnalysisImpl();
            InitializeComponent();
        }

        private void ascx_O2Reflector_Load(object sender, EventArgs e)
        {
            if (DesignMode == false)
            {
                if (assemblyAnalysis == null)
                {
                    PublicDI.log.error(
                        "ALERT O2Reflector Load, DependencyInjection object assemblyAnalysis is null, there will be no engines available!");
//                    if (ParentForm != null) 
//                        ParentForm.Close();
                }
                else
                {
                    loadedAssemblies.Add(PublicDI.config.ExecutingAssembly);
                    setCurrentEngineFromSelectedRadioBox();
                    assemblyAnalysis.onMethodSelectedGetILCode += methodSelectedCallback_ILCode;
                    assemblyAnalysis.onMethodSelectedGetSourceCode += methodSelectedCallback_SourceCode;
                    //loadAssembly(Config.getExecutingAssembly());                
                    //  loadThisObjectDataInMainTreeView();
                    //  tvThisObjectClassInfo.Sort();
                }
            }
        }


        public void loadAssembly(string assemblyToLoad)
        {
            if (loadedAssemblies.Contains(assemblyToLoad))
                PublicDI.log.info("Assembly {0} was already loaded", assemblyToLoad);
            else
            {
                if (assemblyAnalysis != null) // first check if the Cecil Engine is available
                {
                    if (assemblyAnalysis.canAssemblyBeLoaded(assemblyToLoad))
                        // use cecil to test to see if we can load the assesmbly (since Cecil is much more forgiving than Reflection)
                    {
                        loadedAssemblies.Add(assemblyToLoad);
                        refreshGui();
                        return;
                    }
                }
                else if (PublicDI.reflection != null) // if it not lets use the reflection engine
                {
                    if (PublicDI.reflection.loadAssembly(assemblyToLoad) != null)
                    {
                        loadedAssemblies.Add(assemblyToLoad);
                        refreshGui();
                        return;
                    }
                }
                else
                    PublicDI.log.error(
                        "in loadAssembly, There were no ENGINES (Cecil or Reflection) available, so assembly could not be loaded: {0}",
                        assemblyToLoad);

                PublicDI.log.error("in loadAssembly, could no load assessmbly: {0}", assemblyToLoad);
            }
        }

        private void refreshAssemblyBrowser()
        {
            if (assemblyAnalysis == null)
                PublicDI.log.error("in refreshAssemblyBrowser, DI variable is not set assemblyAnalysis, Aborting function");
            else
            {

                tvAssemblyBrowser.Nodes.Clear();
                tvAssemblyBrowser.Sort();
                foreach (string assemblyToLoad in loadedAssemblies)
                {
                    TreeNode node = null;
                    switch (engineToUse)
                    {
                        case AvailableEngines.Cir:
                            break;
                        case AvailableEngines.MonoCecil:
                            node = O2Forms.newTreeNode(Path.GetFileName(assemblyToLoad), assemblyToLoad, 0,
                                                       assemblyAnalysis.loadAssemblyUsingMonoCecil(assemblyToLoad));
                            assemblyAnalysis.populateTreeNodeWithObjectChilds(node);
                            break;
                        case AvailableEngines.Reflection:
                            node = O2Forms.newTreeNode(Path.GetFileName(assemblyToLoad), assemblyToLoad, 0,
                                                       assemblyAnalysis.loadAssemblyUsingReflection(assemblyToLoad));
                            assemblyAnalysis.populateTreeNodeWithObjectChilds(node);
                            break;
                    }
                    if (node != null)
                        tvAssemblyBrowser.Nodes.Add(node);
                }
            }
        }


        public void refreshLoadedAssembliesList()
        {
            tvLoadedAssemblies.Nodes.Clear();
            O2Forms.populateWindowsControlWithList(tvLoadedAssemblies, loadedAssemblies);
        }


        public void refreshGui()
        {
            refreshLoadedAssembliesList();
            refreshAssemblyBrowser();
        }

        private void btRefresh_Click(object sender, EventArgs e)
        {
            refreshGui();
        }

        private void tvLoadedAssemblies_DragDrop(object sender, DragEventArgs e)
        {
            string file = Dnd.tryToGetFileOrDirectoryFromDroppedObject(e);
            if (file != "")
                loadAssembly(file);
        }

        private void tvLoadedAssemblies_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tvAssemblyBrowser_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void tvAssemblyBrowser_DragDrop(object sender, DragEventArgs e)
        {
            string file = Dnd.tryToGetFileOrDirectoryFromDroppedObject(e);
            if (file != "")
                loadAssembly(file);
        }


        public void setCurrentEngine(AvailableEngines newEngine)
        {
            if (engineToUse != newEngine || tvAssemblyBrowser.Nodes.Count == 0)
            {
                engineToUse = newEngine;
                refreshGui();
            }
        }

        public AvailableEngines getCurrentEngine()
        {
            return engineToUse;
        }


        public void removeAllLoadedAssemblies()
        {
            loadedAssemblies = new List<string>();
            refreshGui();
        }

        public TreeView getTreeView_AssemblyBrowser()
        {
            return tvAssemblyBrowser;
        }

        public TreeView getTreeView_LoadedAssemblies()
        {
            return tvLoadedAssemblies;
        }

        private void rbEngine_DotNetReflection_CheckedChanged(object sender, EventArgs e)
        {
            setCurrentEngineFromSelectedRadioBox();
        }

        private void rbEngine_MonoCecil_CheckedChanged(object sender, EventArgs e)
        {
            setCurrentEngineFromSelectedRadioBox();
        }

        private void rbEngine_CIR_CheckedChanged(object sender, EventArgs e)
        {
            setCurrentEngineFromSelectedRadioBox();
        }

        public void setCurrentEngineFromSelectedRadioBox()
        {
            if (rbEngine_DotNetReflection.Checked)
                setCurrentEngine(AvailableEngines.Reflection);
            else if (rbEngine_MonoCecil.Checked)
                setCurrentEngine(AvailableEngines.MonoCecil);
            else if (rbEngine_CIR.Checked)
                setCurrentEngine(AvailableEngines.Cir);
        }

        /*public void populateSelectedNode_tvAssemblyBrowser()
        {
            tvAssemblyBrowser_BeforeExpand(null, null);
        }*/

        private void tvAssemblyBrowser_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            tvAssemblyBrowser.SelectedNode = e.Node;
            populateNode(tvAssemblyBrowser.SelectedNode);

            //if (selectedNode != null)
            //{
            /*selectedNode.Nodes.Clear();
                AssemblyAnalysis.populateTreeNodeWithObjectChilds(selectedNode);*/
            //}
        }


        public void populateNode(TreeNode nodeToPopulate)
        {
            if (nodeToPopulate != null)
            {
                nodeToPopulate.Nodes.Clear();
                assemblyAnalysis.populateTreeNodeWithObjectChilds(nodeToPopulate);
            }
        }

        public void methodSelectedCallback_ILCode(string code)
        {
            tbILCode.set_Text(code);
            //           DI.log.info("received: {0}", code);
        }

        public void methodSelectedCallback_SourceCode(string code)
        {
            tbSourceCode.set_Text(code);
            //           DI.log.info("received: {0}", code);
        }

        private void tvAssemblyBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            assemblyAnalysis.processTreeNodeAndRaiseCallbacks(e.Node);
            raiseO2KernelMessageWithSelectedNode(e.Node);
            //btCloneMethod.Enabled = (e.Node.Tag.GetType().Name == "MethodDefinition");
            //btCloneMethod_Click(null, null);
        }

        private void raiseO2KernelMessageWithSelectedNode(TreeNode treeNode)
        {
            string assemblyName = "Assembly";
            string _typeName = "typeName";
            string _methodName = "methodName";
            object[] _methodParameters = new object[0];
            switch (treeNode.Tag.GetType().Name)
            {
                case "Assembly":
                    break;
            }

            O2Messages.selectedTypeOrMethod(assemblyName, _typeName, _methodName, _methodParameters);
        }

        #region toRefactor

        /*

        private void ___tvClassInfo_AfterSelect(object sender, TreeViewEventArgs e)
        {
                   if (e.Node.Tag != null)
                       if (e.Node.Tag.GetType().Name == "RuntimeMethodInfo")
                       {
                           Reflection.loadMethodInfoParametersInDataGridView((MethodInfo)e.Node.Tag, dgvThisObject_SelectedMethodArguments);
                           btInvokeMethod.Enabled = Reflection.doesMethodOnlyHasSupportedParameters((MethodInfo)e.Node.Tag);
                       }
        }
        


        private void loadThisObjectDataInMainTreeView()
        {
            tvAssemblyBrowser.Nodes.Clear();

            
            foreach (Type tTypeToLoad in GlobalStaticVars.dO2ExposedMethodsForDynamicInvokation.Values)
                Reflection.loadTypeDataIntoTreeView(tTypeToLoad, tvAssemblyBrowser, cbViewAllMethods_IncludingOnesWithNotSupportedParams.Checked, cbShowArguments.Checked, cbShowReturnParameter.Checked, tbMethodFilter.Text, false);            
            if (cbAutoExpand.Checked || tvAssemblyBrowser.GetNodeCount(true) < 20)
                tvAssemblyBrowser.ExpandAll();
            else
                tvAssemblyBrowser.CollapseAll();
        }


        private void btInvokeMethod_Click(object sender, EventArgs e)
        {
            if (tvAssemblyBrowser.SelectedNode != null && tvAssemblyBrowser.SelectedNode.Tag != null && tvAssemblyBrowser.SelectedNode.Tag.GetType().Name == "RuntimeMethodInfo")
            {
                var aoMethodParameters = Reflection.getParameterObjectsFromDataGridColumn(dgvThisObject_SelectedMethodArguments, "Value");
                Reflection.executeMethodAndOutputResultInTextBoxOrDataGridView((MethodInfo)tvAssemblyBrowser.SelectedNode.Tag, aoMethodParameters, this, tbInvocationResult, dgvThisObject_InvocationResult);
            }
        }

        private void cbViewAllMethods_IncludingOnesWithNotSupportedParams_CheckedChanged(object sender, EventArgs e)
        {
            loadThisObjectDataInMainTreeView();
        }

        private void dgvInvocationResult_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
             DI.log.error("dgvInvocationResult DataError event: {0}", e.Exception.Message);
        }

        private void tbMethodFilter_TextChanged(object sender, EventArgs e)
        {
            //  if (tbMethodFilter.Text.Length > 3)
            {
                loadThisObjectDataInMainTreeView();
                tbMethodFilter.Focus();
            }
        }

        private void tbMethodFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loadThisObjectDataInMainTreeView();
                tbMethodFilter.Focus();
            } 
        } 

        private void cbAutoExpand_CheckedChanged(object sender, EventArgs e)
        {
            loadThisObjectDataInMainTreeView();
        }

        private void cbShowReturnParameter_CheckedChanged(object sender, EventArgs e)
        {
            loadThisObjectDataInMainTreeView();
        }

        private void cbShowArguments_CheckedChanged(object sender, EventArgs e)
        {
            loadThisObjectDataInMainTreeView();
        }
        */

        #endregion

        /*private void btCloneMethod_Click(object sender, EventArgs e)
        {
            TreeNode node = tvAssemblyBrowser.SelectedNode;
            if (node != null && node.Tag != null && node.Tag.GetType().Name == "MethodDefinition")
                tbILCodeCloned.Text = DI.cecilDecompiler.getILfromClonedMethod(node.Tag);
        }*/
    }
}
