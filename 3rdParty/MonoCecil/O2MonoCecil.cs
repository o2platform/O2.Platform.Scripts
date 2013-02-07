// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//NOTE: this is the code that was on the O2_External_O2Mono.dll assembly (not needed anymore)
//      this needs a LOT of refactoring since this code was created before the use of C# extension methods

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Mono.Cecil;
using O2.DotNetWrappers.Windows;
using O2.External.O2Mono;
using O2.External.O2Mono.MonoCecil;
using O2.Interfaces.ExternalDlls;
using O2.Kernel;
//Installer:ILSpy_Installer.cs!ILSpy/ILSpy.exe
//O2File:CecilFilteredSignature.cs
//O2File:MonoCecil/CecilUtils.cs
//O2File:IO2MonoCecil.cs
//O2File:CecilDecompiler.cs

namespace O2.External.O2Mono
{
    public class O2MonoCecil : IO2MonoCecil
    {
    	public static O2MonoCecil Current {get;set;}
    	
    	static O2MonoCecil()
    	{
    		Current = new O2MonoCecil();
    		
    	}
    	
        #region using core MonoCecil lib

        public object loadAssembly(string assemblyToLoad)
        {
            return CecilUtils.getAssembly(assemblyToLoad);
        }
        public bool canAssemblyBeLoaded(string assemblyToLoad)
        {
            return (CecilUtils.getAssembly(assemblyToLoad) != null) ? true : false;
        }
        public object getAssemblyEntryPoint(string assemblyToLoad)
        {
            return CecilUtils.getAssemblyEntryPoint(assemblyToLoad);            
        }
        /*public string getMethodDefinitionDeclaringTypeModuleName(object methodDefinition)
        {
            return CecilUtils.getMethodDefinitionDeclaringTypeModuleName(methodDefinition);            
        }

        public string getMethodDefinitionDeclaringTypeName(object methodDefinition)
        {
            throw new System.NotImplementedException();
        }
        public string getMethodDefinitionName(object methodDefinition)
        {
            throw new System.NotImplementedException();
        }*/

        #endregion

        #region using CecilDecompiler 

        public string getSourceCode(MethodDefinition methodDefinition)
        {
        	return new CecilDecompiler().getSourceCode(methodDefinition);
            //return (methodDefinition is MethodDefinition) ? getSourceCode(methodDefinition) : "";
        }
        public string getIL(MethodDefinition methodDefinition)
        {
        	return new CecilDecompiler().getIL(methodDefinition);
            //return (methodDefinition is MethodDefinition) ? getIL(methodDefinition) : "";
        }
        public string getILfromClonedMethod(MethodDefinition methodDefinition)
        {
        	return new CecilDecompiler().getILfromClonedMethod(methodDefinition);
            //return (methodDefinition is MethodDefinition)
            //           ? getILfromClonedMethod(methodDefinition)
            //           : "";
        }
        #endregion

        #region support to windows FORM/ASCX

        private const int iconIndex_module = 1;
        private const int iconIndex_namespace = 2;
        private const int iconIndex_type = 3;
        private const int iconIndex_method = 4;

        public List<TreeNode> populateTreeNodeWithObjectChilds(TreeNode node, object tag, bool populateFirstChildNodes)
        {
            var newNodes = new List<TreeNode>();
            try
            {
                //  DI.log.debug("populating :{0} - {1}", node, tag);                
                if (node != null && tag != null)
                    switch (tag.GetType().Name)
                    {
                        case "AssemblyDefinition":
                            foreach (ModuleDefinition module in CecilUtils.getModules((AssemblyDefinition)tag))
                                newNodes.Add(O2Forms.newTreeNode(node, module.Name, iconIndex_module, module));
                            break;
                        case "ModuleDefinition":
                            Dictionary<string, List<TypeDefinition>> cecilTypesMappedToNamespaces =
                                CecilUtils.getDictionaryWithTypesMappedToNamespaces((ModuleDefinition)tag);

                            foreach (string typeNamespace in cecilTypesMappedToNamespaces.Keys)
                            {
                                TreeNode namespaceNode = O2Forms.newTreeNode(node, typeNamespace, iconIndex_namespace,
                                                                             cecilTypesMappedToNamespaces[
                                                                                 typeNamespace]);
                                foreach (TypeDefinition type in cecilTypesMappedToNamespaces[typeNamespace])
                                    O2Forms.newTreeNode(namespaceNode, type.Name, iconIndex_type, type);
                                newNodes.Add(namespaceNode);
                            }
                            /*foreach (var type in MonoCecil.CecilUtils.getTypes((ModuleDefinition)tag)) 
                                newNodes.Add(O2Forms.newTreeNode(node, type.FullName, iconIndex_module, type));*/
                            break;
                        case "TypeDefinition":
                            // add nested types in Type
                            foreach (TypeDefinition type in CecilUtils.getTypes((TypeDefinition)tag))
                                newNodes.Add(O2Forms.newTreeNode(node, type.Name, iconIndex_type, type));
                            // add methods in Type
                            foreach (MethodDefinition method in CecilUtils.getMethods((TypeDefinition)tag))
                                newNodes.Add(O2Forms.newTreeNode(node,
                                                                 new CecilFilteredSignature(method).getReflectorView(),
                                                                 iconIndex_method, method));
                            break;
                        case "MethodDefinition":
                            break;
                        case "List`1":
                            foreach (object item in (IEnumerable)tag)
                            {
                                switch (item.GetType().Name)
                                {
                                    case "TypeDefinition":
                                        newNodes.Add(O2Forms.newTreeNode(node, ((TypeDefinition)item).Name,
                                                                         iconIndex_type, item));
                                        break;
                                }
                            }
                            break;
                    }
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in populateTreeNodeWithObjectChilds");
            }
            return newNodes;
        }

        #endregion
    }
}
