// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.DotNetWrappers.Windows;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.Struts_1_5
{
    public class StrutsMappingsViewHelpers
    {
        public static void populateTreeNodeWith_ActionServlet(TreeNode treeNode, KStrutsMappings_ActionServlet strutsActionServet)
        {
            addNodeIfNonEmpty(treeNode, "url pattern", strutsActionServet.urlPattern);
            addNodeIfNonEmpty(treeNode, "load on startup", strutsActionServet.loadOnStartUp);
            addNodeIfNonEmpty(treeNode, "debug", strutsActionServet.debug);
            addNodeIfNonEmpty(treeNode, "validate", strutsActionServet.validate);
            addNodeIfNonEmpty(treeNode, "detail", strutsActionServet.detail);
            addNodeIfNonEmpty(treeNode, "rule sets", strutsActionServet.ruleSets);
            addNodeIfNonEmpty(treeNode, "application", strutsActionServet.application);

            var currentUrlPattern =
                (strutsActionServet.urlPattern[strutsActionServet.urlPattern.Length - 1] == '*')
                    ? strutsActionServet.urlPattern.Substring(0, strutsActionServet.urlPattern.Length - 1)
                    : strutsActionServet.urlPattern;


            foreach (var filter in strutsActionServet.filters)
            {
                if (filter.urlPattern == "/*" || filter.urlPattern.StartsWith(currentUrlPattern))
                {
                    var nodeText =
                        string.Format("Filter: {0}   -->   {1}   -->   {2}",
                                       filter.name, filter.urlPattern, filter.@class);
                    treeNode.Nodes.Add(nodeText);
                }
            }
            /*treeNode.Nodes.Add("debug: " + strutsActionServet.debug);
                    
            treeNode.Nodes.Add("detail: " + strutsActionServet.detail);
            treeNode.Nodes.Add("rule sets: " + strutsActionServet.ruleSets);
            treeNode.Nodes.Add("application: " + strutsActionServet.ruleSets);*/
            foreach (var configFile in strutsActionServet.configFiles)
                treeNode.Nodes.Add("config file: " + configFile);

            foreach (var chainConfigFile in strutsActionServet.chainConfigFiles)
                treeNode.Nodes.Add("chainConfig file: " + chainConfigFile);


            if (strutsActionServet.controllers.Count > 0)
            {
                var controllersNode = O2Forms.newTreeNode(treeNode.Nodes, "controllers", 0,
                                                          strutsActionServet.controllers);
                controllersNode.Nodes.Add("DummyNode");

            }

            if (strutsActionServet.formsBeans.Count > 0)
            {
                var controllersNode = O2Forms.newTreeNode(treeNode.Nodes, "form beans", 0,
                                                          strutsActionServet.formsBeans);
                controllersNode.Nodes.Add("DummyNode");

            }

        }
        public static void addNodeIfNonEmpty(TreeNode treeNode, string nodeName, string nodeValue)
        {
            if (false == String.IsNullOrEmpty(nodeValue))
                treeNode.Nodes.Add(String.Format("{0} : {1}", nodeName, nodeValue));
        }

        internal static void populateTreeNodeWith_Controllers(TreeNode treeNode, Dictionary<string, IStrutsMappings_Controller> controllers)
        {
            var mappedControllers = new Dictionary<string, TreeNode>();
            var sortedDictionary = new SortedDictionary<string, IStrutsMappings_Controller>(controllers);            
            foreach (var controller in sortedDictionary.Values)
            {
                var treeNodeWithController = new TreeNode(controller.type);
                addNodeIfNonEmpty(treeNodeWithController, "name", controller.name);
                // add form bean
                if (controller.formBean != null)
                    O2Forms.newTreeNode(treeNodeWithController.Nodes, "form bean:" + controller.formBean.name, 0, controller.formBean, true);
                //treeNodeWithController.Nodes.Add("name: " + controller.name);
                var treeNodeWithControllerPaths = new TreeNode("Paths");
                foreach (var path in controller.paths)
                    O2Forms.newTreeNode(treeNodeWithControllerPaths.Nodes, path.path, 0, path, true);

                treeNodeWithController.Nodes.Add(treeNodeWithControllerPaths);
                treeNode.Nodes.Add(treeNodeWithController);
            }
            /*
            foreach(var controller in controllers)
            {
                var controllerType = controller.type ?? "[no type defined]";
                if (false == mappedControllers.ContainsKey(controllerType))
                    mappedControllers.Add(controllerType, new TreeNode(controller.type));
                var controllerNode = mappedControllers[controllerType];
                var childNode = new TreeNode(controller.name);

                controllerNode.Nodes.Add(childNode);                                                
            }
            foreach (var controllerNode in mappedControllers.Values)
                treeNode.Nodes.Add(controllerNode);*/
        }

        public static void populateTreeNodeWith_Controller_Path(TreeNode treeNode, KStrutsMappings_Controller_Path path)
        {
            addListIfNotEmpty(treeNode, path.resolvedViews, "resolved views:",false);
            addListIfNotEmpty(treeNode, path.notResolvedViews, "NOT resolved views:", false);
            addNodeIfNonEmpty(treeNode, "input", path.input);
            addNodeIfNonEmpty(treeNode, "validate", path.validate);
            addListIfNotEmpty(treeNode, path.forwards, "forward:", true);
            /*if (path.forwards.Count > 0)
                foreach (var forward in path.forwards)
                    addNodeIfNonEmpty(treeNode, "forward", forward.ToString());            */
            
        }

        public static void addListIfNotEmpty<T>(TreeNode treeNode, List<T> itemsToAdd, string name, bool addAsFlatList)
        {
            if (itemsToAdd.Count >0)
            {
                if (addAsFlatList)
                    foreach(var item in itemsToAdd)
                        addNodeIfNonEmpty(treeNode, name, item.ToString());
                else
                {
                    var itemTreeNode = new TreeNode(name);
                    foreach (var item in itemsToAdd)
                        itemTreeNode.Nodes.Add(item.ToString());
                        //addNodeIfNonEmpty(itemTreeNode, name, item.ToString());
                    treeNode.Nodes.Add(itemTreeNode);
                }
            }
        }

        private static void addDictionaryIfNotEmpty(TreeNode treeNode, Dictionary<string, string> dictionary, string name, bool addAsFlatList)
        {
            if(dictionary.Keys.Count  > 0)
            {
                if (addAsFlatList)
                {
                    foreach (var item in dictionary)
                        treeNode.Nodes.Add(string.Format("{0}: {1} = {2}",name, item.Key, item.Value));
                }
                else
                {
                    var rootNode = new TreeNode(name);
                    foreach (var item in dictionary)
                        rootNode.Nodes.Add(string.Format("{0} = {1}", item.Key, item.Value));
                    treeNode.Nodes.Add(rootNode);
                }
            }
        }


        public static void populateTreeNodeWith_FormBeans(TreeNode treeNode, Dictionary<string, IStrutsConfig_FormBean> formBeans)
        {
            //treeNode.Nodes.Add("ASDASDA");
            foreach (var formBean in formBeans)
            {
                O2Forms.newTreeNode(treeNode.Nodes, formBean.Key, 0, formBean.Value,true);
            }
        }

        internal static void populateTreeNodeWith_FormBean(TreeNode treeNode, IStrutsConfig_FormBean formBean)
        {
            addNodeIfNonEmpty(treeNode, "type", formBean.type);
            addNodeIfNonEmpty(treeNode, "extends", formBean.extends);
            addNodeIfNonEmpty(treeNode, "hasValidationMapping", formBean.hasValidationMapping.ToString());
            if (formBean.fields.Count >0)
            {
                var fieldsTreeNode = new TreeNode("fields");
                foreach(var field in formBean.fields)
                    O2Forms.newTreeNode(fieldsTreeNode.Nodes, field.Key, 0, field.Value, true);
                treeNode.Nodes.Add(fieldsTreeNode);
            }
                        
            
            //addDictionaryIfNotEmpty(treeNode, formBean.properties, "fields",true);
        }

        internal static void populateTreeNodeWith_FormBean_Field(TreeNode treeNode, KStrutsConfig_FormBean_Field formBeanField)
        {
            addNodeIfNonEmpty(treeNode, "type", formBeanField.type);
            addNodeIfNonEmpty(treeNode, "initial", formBeanField.initial);
            addNodeIfNonEmpty(treeNode, "has validation mapping", formBeanField.hasValidationMapping.ToString());
            addNodeIfNonEmpty(treeNode, "depends", formBeanField.depends);
            addDictionaryIfNotEmpty(treeNode, formBeanField.validators, "validator", true);            
        }
    }
}
