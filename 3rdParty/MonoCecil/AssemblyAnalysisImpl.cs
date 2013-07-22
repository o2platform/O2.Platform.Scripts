// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms.Interfaces;
using FluentSharp.WinForms.Utils;
using Mono.Cecil;

//O2File:MonoCecil/CecilUtils.cs
//O2File:O2MonoCecil.cs
namespace O2.External.O2Mono
{
    public class AssemblyAnalysisImpl : IAssemblyAnalysis
    {
    	O2FormsReflectionASCX reflectionEngine;
    
        public bool CecilEngineAvailble
        {
            get { return O2MonoCecil.Current != null; }
        }

        public bool ReflectionEngineAvailble
        {
            get { return reflectionEngine != null; }
        }

		
		
        public AssemblyAnalysisImpl()
        {
        	reflectionEngine = new O2FormsReflectionASCX();
        
            //cecilEngineAvailble = (DI.monoCecil != null);
            if (false == CecilEngineAvailble)
                PublicDI.log.error(
                    "Not all required DI (Dependency Injections) are available for the CECIL Engine (which will not be available)");

            //reflectionEngineAvailble = (DI.reflectionASCX != null);
            if (false == ReflectionEngineAvailble)
                PublicDI.log.error(
                    "Not all required DI (Dependency Injections) are available for the REFLECTION Engine (which will not be available)");            
        }

        #region IAssemblyAnalysis Members

        public event Action<string> onMethodSelectedGetILCode;
        public event Action<string> onMethodSelectedGetSourceCode;

        public bool canAssemblyBeLoaded(string assemblyToLoad)
        {
            return O2MonoCecil.Current.canAssemblyBeLoaded(assemblyToLoad);
        }

        public void processTreeNodeAndRaiseCallbacks(TreeNode node)
        {
            if (node != null && node.Tag != null)
            {
                switch (node.Tag.GetType().Name)
                {
                    case "MethodDefinition":
                        //     var message = "Showing IL for " + ((MethodDefinition)node.Tag).Name;
                        string ilCode = O2MonoCecil.Current.getIL(node.Tag as MethodDefinition);
                        string sourceCode = O2MonoCecil.Current.getSourceCode(node.Tag  as MethodDefinition);
                        Callbacks.raiseRegistedCallbacks(onMethodSelectedGetILCode, new[] {ilCode});
                        Callbacks.raiseRegistedCallbacks(onMethodSelectedGetSourceCode, new[] {sourceCode});
                        break;
                }
            }
        }

        public void populateTreeNodeWithObjectChilds(TreeNode node)
        {
            populateTreeNodeWithObjectChilds(node, node.Tag, true /*populateFirstChildNodes*/);
        }

        public void populateTreeNodeWithObjectChilds(TreeNode node, object tag, bool populateFirstChildNodes)
        {
            try
            {
                //  DI.log.debug("populating :{0} - {1}", node, tag);
                var newNodes = new List<TreeNode>();
                if (node != null && tag != null)
                {
                    if (CecilEngineAvailble)
                        newNodes.AddRange(O2MonoCecil.Current.populateTreeNodeWithObjectChilds(node, tag,
                                                                                        populateFirstChildNodes));
                    if (ReflectionEngineAvailble)
                        newNodes.AddRange(reflectionEngine.populateTreeNodeWithObjectChilds(node, tag,populateFirstChildNodes));
                }
                // use this for debugging since under normal circunstances the method's nodes will have no child nodes
                /*if (newNodes.Count == 0)
                    DI.log.error("in populateTreeNodeWithObjectChilds, no nodes were added by the available engines, so this most likely is an supported node type: {0}  , {1}", 
                        (tag!=null) ? tag.GetType().Name : "{tag==null}",
                        (tag!=null) ? tag.GetType().FullName : "{tag==null}");*/

                if (populateFirstChildNodes)
                    foreach (TreeNode newNode in newNodes)
                        populateTreeNodeWithObjectChilds(newNode, newNode.Tag, false);
            }
            catch (Exception ex)
            {
                PublicDI.log.error("in populateTreeNodeWithObjectChilds: {0}", ex.Message);
            }
        }

        public object loadAssemblyUsingMonoCecil(string assemblyToLoad)
        {
            if (CecilEngineAvailble)
                return O2MonoCecil.Current.loadAssembly(assemblyToLoad);
            return null;
        }

        public object loadAssemblyUsingReflection(string assemblyToLoad)
        {
            if (ReflectionEngineAvailble)
                return reflectionEngine.loadAssembly(assemblyToLoad);

            return null;
        }

        #endregion
    }
}
