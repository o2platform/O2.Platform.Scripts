// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using O2.Core.FileViewers.J2EE;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.Struts_1_5
{
    public class StrutsMappingsHelpers
    {
        public static string strutsMappingExtension = ".O2StrutsMapping";

        public static IStrutsMappings calculateStrutsMapping(string webXmlFile, string strutsConfigFile, string tilesDefinitionsFile, string validationXmlFile)
        {
            var webXml = J2eeConfigFiles.getWebXml(webXmlFile);
            if (webXml == null)
                return null;

            var tilesDefinitions = J2eeConfigFiles.getTilesDefinitionXml(tilesDefinitionsFile);

            var validation = J2eeConfigFiles.getValidationXml(validationXmlFile);

            var strutsConfig = J2eeConfigFiles.getStrutsConfig(strutsConfigFile, tilesDefinitions, validation);
            if (strutsConfig == null)
                return null;

            
            return calculateStrutsMapping(webXml, strutsConfig);
        }

        public static IStrutsMappings calculateStrutsMapping(IWebXml webXml, IStrutsConfigXml strutsConfigXml)
        {
            var strutsMappings = new KStrutsMappings();

            var servletsDictionary = ((KWebXml) webXml).getServletsDictionary();

            foreach (var servletMapping in webXml.servletMappings)
            {
                if (servletsDictionary.ContainsKey(servletMapping.servletName))
                {                    
                    var servlet = servletsDictionary[servletMapping.servletName];
                    if ("org.apache.struts.action.ActionServlet" == servlet.servletClass)
                    {
                        strutsMappings.actionServlets.Add(calculateActionServlet(webXml, servletMapping, servlet, strutsConfigXml));
                    }
                    else
                        strutsMappings.otherServlets.Add(
                            string.Format("{0}   {1}  {2}",
                                      servletMapping.servletName, servletMapping.urlPattern,
                                      servlet.servletClass));
                }
                else
                    DI.log.error("in calculateStrutsMapping, could not find servlet: {0}", servletMapping.servletName);
                    
            }            
            return strutsMappings;
        }
        

        private static IStrutsMappings_ActionServlet calculateActionServlet(IWebXml webXml, IWebXml_Servlet_Mapping servletMapping,IWebXml_Servlet servlet, IStrutsConfigXml strutsConfigXml)
        {
            var actionServlet = new KStrutsMappings_ActionServlet
                                    {
                                        name = servlet.servletName,
                                        loadOnStartUp = servlet.loadOnStartUp,
                                        urlPattern =  servletMapping.urlPattern
                                    };

            try
            {
                foreach (var param in servlet.initParam)
                    switch (param.Key)
                    {
                        case "config":
                            var configFiles = param.Value.Split(',');
                            foreach (var configFile in configFiles)
                                actionServlet.configFiles.Add(configFile.Trim());
                            break;
                        case "debug":
                            actionServlet.debug = param.Value;
                            break;
                        case "detail":
                            actionServlet.detail = param.Value;
                            break;
                        case "rulesets":
                            actionServlet.ruleSets = param.Value;
                            break;
                        case "validate":
                            actionServlet.validate = param.Value;
                            break;
                        case "application":
                            actionServlet.application = param.Value;
                            break;
                        case "chainConfig":
                            var chainConfigFiles = param.Value.Split(',');
                            foreach (var chainConfigFile in chainConfigFiles)
                                actionServlet.chainConfigFiles.Add(chainConfigFile.Trim());
                            break;
                        default:
                            DI.log.error("in calculateActionServlet, unsupported servlet.initParam key value: {0}", param.Key);
                            break;
                    }

                // map filters
                foreach (var filter in ((KWebXml)webXml).getFiltersDictionary())
                {
                    var strutsFilter = new KStrutsMappings_Filter
                                           {
                                               name = filter.Key.filterName,
                                               urlPattern = filter.Key.urlPattern,
                                               dispatcher = filter.Key.dispatcher,
                                               @class = filter.Value.filterClass
                                           };
                    actionServlet.filters.Add(strutsFilter);
                }


                // map paths and mapped view

                foreach (var actionMapping in strutsConfigXml.actionmappings)
                {
                    var actionType = actionMapping.type ?? "[no type defined]";

                    // populate controller dictionary
                    if (false == actionServlet.controllers.ContainsKey(actionType))
                        actionServlet.controllers.Add(actionType, new KStrutsMappings_Controller
                                                                        {
                                                                            name = actionMapping.name,
                                                                            type = actionMapping.type
                                                                        });
                    var controler = actionServlet.controllers[actionType];

                    // calculate paths
                    var path = new KStrutsMappings_Controller_Path
                                   {
                                       path = actionMapping.path,
                                       input = actionMapping.input,
                                       validate = actionMapping.validate,
                                       forwards = actionMapping.forwards
                                   };

                    foreach (var forward in actionMapping.forwards)
                        addMappedView(path, forward.path, strutsConfigXml.resolvedViews);

                    //path.resolvedViews.Add(forward.path);
                    //if (false == string.IsNullOrEmpty(path.input))
                    //    path.resolvedViews.Add(path.input);
                    addMappedView(path, path.input, strutsConfigXml.resolvedViews);

                    controler.paths.Add(path);
                }

                // map beans
                foreach (var formBean in strutsConfigXml.formBeans)
                    actionServlet.formsBeans.Add(formBean.name, formBean);

                // finally map the beans to the controllers
                foreach (var controller in actionServlet.controllers)
                    if (controller.Value.name!= null)
                        if (actionServlet.formsBeans.ContainsKey(controller.Value.name))
                            controller.Value.formBean = actionServlet.formsBeans[controller.Value.name];

            }
            catch (Exception ex)
            {
                DI.log.error("in calculateActionServlet: {0}", ex.Message);
            }
            return actionServlet;
        }

        private static void addMappedView(IStrutsMappings_Controller_Path path, string viewToAdd, Dictionary<string, List<string>> resolvedViewsDictionary)
        {
            if (false == string.IsNullOrEmpty(viewToAdd))
            {
                if (viewToAdd.IndexOf(".jsp") > -1 || viewToAdd.IndexOf(".do") > -1)
                    path.resolvedViews.Add(viewToAdd);
                else if (resolvedViewsDictionary.ContainsKey(viewToAdd))
                {
                    foreach (var resolvedView in resolvedViewsDictionary[viewToAdd])
                        if (resolvedView.IndexOf(".jsp") > -1 || viewToAdd.IndexOf(".do") > -1)
                            path.resolvedViews.Add(resolvedView);
                }
                else
                    path.notResolvedViews.Add(viewToAdd);
            }
        }

        //public static void addMappedView()

        public static string saveStrutsMappings(IStrutsMappings strutsMappings, string targetFileOrFolder)
        {
            if (strutsMappings == null)
                return "";
            if (Directory.Exists(targetFileOrFolder))
                targetFileOrFolder = Path.Combine(targetFileOrFolder, Files.getTempFileName() + strutsMappingExtension);
            else if (false == Directory.Exists(Path.GetDirectoryName(targetFileOrFolder)))
            {
                DI.log.error("Invalid filename supplied since that directly doesnt exist: {0}", targetFileOrFolder);
                return "";
            }
            if (Serialize.createSerializedBinaryFileFromObject(strutsMappings, targetFileOrFolder))
                DI.log.info("Serialized Struts Mapping object saved to: {0}", targetFileOrFolder);
            else
                DI.log.error("There was a problem serializing Struts Mapping object saved to: {0}", targetFileOrFolder);
            return targetFileOrFolder;
        }        
    }
}
