// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Core.FileViewers.Struts_1_5;
using O2.Core.FileViewers.XSD;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.J2EE
{
    public class J2eeConfigFiles
    {
        public static IWebXml getWebXml(string pathToWebXmlFile)
        {

            // hack to handle prob with XSD schema (which doesn't load when ...)
            var fileContents = Files.getFileContents(pathToWebXmlFile);
            if (fileContents.IndexOf("<web-app>") > -1)
            {
                fileContents = fileContents.Replace("<web-app>",
                                                    "<web-app version=\"2.4\" xmlns=\"http://java.sun.com/xml/ns/j2ee\">");
                pathToWebXmlFile = DI.config.getTempFileInTempDirectory("web.xml");
                Files.WriteFileContent(pathToWebXmlFile, fileContents);            
            }
            var webXml = new KWebXml();
            var webAppType = ((webappType)Serialize.getDeSerializedObjectFromXmlFile(pathToWebXmlFile, typeof(webappType)));
            if (webAppType != null)
            {
                foreach (var item in webAppType.Items)
                {
                    switch (item.GetType().Name)
                    {
                        case "displaynameType":
                            var displayName = (displaynameType) item;
                            webXml.displayName = displayName.Value;
                            break;
                        case "descriptionType":
                            var description = (descriptionType) item;
                            webXml.description = description.Value;
                            break;
                        case "paramvalueType":
                            var paramvalue = (paramvalueType)item;
                            DI.log.debug("paramvalueType:  {0} = {1}", paramvalue.paramname.Value, paramvalue.paramvalue.Value);
                            //webXml.displayName = displayName.Value;
                            break;
                        case "filterType":
                            var filter = (filterType) item;
                            webXml.filters.Add(
                                new KWebXml_Filter
                                    {
                                        filterClass =
                                            (filter.filterclass != null ? filter.filterclass.Value : ""),
                                        filterName =
                                            filter.filtername != null ? filter.filtername.Value : ""
                                    });
                            break;
                        case "filtermappingType":
                            var filterMapping = (filtermappingType) item;
                            var newFilterMapping =
                                new kWebXml_Filter_Mapping()
                                    {
                                        filterName =
                                            filterMapping.filtername != null
                                                ? filterMapping.filtername.Value
                                                : "",
                                        dispatcher =
                                            filterMapping.filtername != null
                                                ? filterMapping.filtername.Value
                                                : ""
                                    };
                            if (filterMapping.Item != null)
                                if (filterMapping.Item is urlpatternType)
                                    newFilterMapping.urlPattern = ((urlpatternType) filterMapping.Item).Value;
                            webXml.filterMappings.Add(newFilterMapping);
                            break;

                        case "listenerType":
                            var listenerType = (listenerType)item;
                            webXml.listenerClass = listenerType.listenerclass.Value;
                            break;
                        case "servletType":
                            var servletType = (servletType)item;
                            var newServlet = new KWebXml_Servlet
                                              {
                                                  servletName = (servletType.servletname != null)  ? servletType.servletname.Value : "",
                                                  loadOnStartUp = (servletType.loadonstartup != null) ? servletType.loadonstartup.Value : ""
                                              };
                            if (servletType.Item is fullyqualifiedclassType)
                                newServlet.servletClass = servletType.Item.Value;
                            if (servletType.initparam != null)
                                foreach (var _initParam in servletType.initparam)                                
                                    newServlet.initParam.Add(_initParam.paramname.Value, _initParam.paramvalue.Value);                                    

                            webXml.servlets.Add(newServlet);
                            break;
                        case "servletmappingType":
                            var servletMapping = (servletmappingType)item;
                            var newServletMapping =
                                new KWebXml_Servlet_Mapping()
                                    {
                                        servletName = servletMapping.servletname.Value,
                                        urlPattern = servletMapping.urlpattern.Value
                                    };
                            webXml.servletMappings.Add(newServletMapping);
                            break;
                        default:
                            DI.log.info("no mapping for :" + item.GetType().Name);
                            break;
                    }
                }
            }
            return webXml;
        }

        public static IStrutsConfigXml getStrutsConfig(string strutsConfigFile)        
        {
            return getStrutsConfig(strutsConfigFile, null,null);
        }

        internal static IStrutsConfigXml getStrutsConfig(string strutsConfigFile, ITilesDefinitions tilesDefinitions, IValidationXml validation)
        {
            var strutsConfigXml = new KStrutsConfigXml();
            try
            {
                var strutsConfig =
                    ((strutsconfig) Serialize.getDeSerializedObjectFromXmlFile(strutsConfigFile, typeof (strutsconfig)));

                if (strutsConfig != null)
                {
                    if (strutsConfig.formbeans != null)
                    {
                        // first add the forms
                        foreach (var formBean in strutsConfig.formbeans)
                        {
                            var newFormBean = new KStrutsConfig_FormBean()
                                                  {
                                                      name = formBean.name,
                                                      type = formBean.type,
                                                      extends = formBean.extends,
                                                  };
                            if (formBean.formproperty != null)
                                foreach (var formProperty in formBean.formproperty)
                                {
                                    var field = new KStrutsConfig_FormBean_Field()
                                                    {
                                                        name = formProperty.name,
                                                        type = formProperty.type,
                                                        initial = formProperty.initial
                                                    };
                                    newFormBean.fields.Add(formProperty.name, field);

                                    //newFormBean.properties.Add(formProperty.name, formProperty.type);
                                }
                            strutsConfigXml.formBeans.Add(newFormBean);
                        }
                        // now map the validation
                        if (validation != null)
                        {
                            var validationForms = new Dictionary<string, IValidation_Form>();
                            foreach (var validationForm in validation.forms)
                                if (false == validationForms.ContainsKey(validationForm.name))
                                    validationForms.Add(validationForm.name, validationForm);
                                else
                                {
                                    DI.log.error("Duplicate form validator: {0}", validationForm.name);
                                    //var asd = validationForms[validationForm.name].fields;
                                }

                            foreach (var formBean in strutsConfigXml.formBeans)
                            {
                                if (false == validationForms.ContainsKey(formBean.name))
                                    formBean.hasValidationMapping = false;
                                else
                                {
                                    var validatorForm = validationForms[formBean.name];
                                    foreach (var field in formBean.fields)
                                    {
                                        if (validatorForm.fields.ContainsKey(field.Key))
                                        {
                                            field.Value.hasValidationMapping = true;
                                            field.Value.depends = validatorForm.fields[field.Key].depends;
                                            foreach (var var in validatorForm.fields[field.Key].vars)
                                                field.Value.validators.Add(var.Key, var.Value);
                                        }
                                        else
                                            field.Value.hasValidationMapping = false;
                                    }
                                    //formBean.properties
                                    //foreach(var )
                                    formBean.hasValidationMapping = true;
                                }
                            }
                        }
                    }
                    if (strutsConfig.globalforwards != null)
                        foreach (var globalForward in strutsConfig.globalforwards)
                            strutsConfigXml.globalForwards.Add(globalForward.name, globalForward.path);


                    if (strutsConfig.actionmappings != null)
                        foreach (var action in strutsConfig.actionmappings)
                        {
                            var newActionMapping = new KStrutsConfig_Action()
                                                       {
                                                           name = action.name,
                                                           path = action.path,
                                                           input = action.input,
                                                           scope = action.scope,
                                                           type = action.type,
                                                           validate = action.validate.ToString(),
                                                           unknown = action.unknown,
                                                           extends = action.extends,
                                                           parameter = action.parameter
                                                       };
                            if (action.forward != null)
                                foreach (var forward in action.forward)
                                {
                                    var newForward = new KStrutsConfig_Action_Forward()
                                                         {
                                                             name = forward.name,
                                                             path = forward.path,
                                                             redirect = forward.redirect
                                                         };
                                    newActionMapping.forwards.Add(newForward);
                                }
                            strutsConfigXml.actionmappings.Add(newActionMapping);
                        }
                    if (strutsConfig.plugin != null)
                        foreach (var plugin in strutsConfig.plugin)
                        {
                            var newPlugIn = new KStrutsConfig_PlugIn()
                                                {
                                                    className = plugin.className
                                                };
                            if (plugin.setproperty != null)
                                foreach (var property in plugin.setproperty)
                                    newPlugIn.properties.Add(property.property, property.value);

                            strutsConfigXml.plugIns.Add(newPlugIn);
                        }

                    if (tilesDefinitions != null)
                        foreach (var titleDefinition in tilesDefinitions.definitions)
                        {
                            //var value = titleDefinition.page ?? titleDefinition.path ?? "";
                            var value = titleDefinition.page ?? titleDefinition.path;

                            addTileDefinition(strutsConfigXml.resolvedViews, titleDefinition.name, value);

                            foreach (var put in titleDefinition.puts)
                                addTileDefinition(strutsConfigXml.resolvedViews, titleDefinition.name, put.Value);
                            /*if (value != null)
                                if (false == strutsConfigXml.resolvedViews.ContainsKey(titleDefinition.name))
                                    strutsConfigXml.resolvedViews.Add(titleDefinition.name, value);
                                else
                                    DI.log.error("in mapping TilesDefinition, there was a duplicate key: {0} with value {1}",
                                                titleDefinition.name, value);
                            */


                        }

                }
            }
            catch (Exception ex)
            {
                DI.log.error("in getStrutsConfig: {0}", ex.Message);

            }
            return strutsConfigXml;
        }

        public static void addTileDefinition(Dictionary<string, List<string>> resolvedViews, string key, string value)
        {
            if (key != null && value != null)
            {
                if (false == resolvedViews.ContainsKey(key))
                    resolvedViews[key] = new List<string>();
                resolvedViews[key].Add(value);
            }
        }

        public static IValidationXml getValidationXml(string fileToMap)
        {
            var validatorXml = new KValidationXml();
            try
            {

                var formvalidation =
                    ((formvalidation) Serialize.getDeSerializedObjectFromXmlFile(fileToMap, typeof (formvalidation)));

                if (formvalidation != null && formvalidation.formset != null)
                {
                    foreach (var formset in formvalidation.formset)
                        foreach (var form in formset.form)
                        {
                            var validationForm = new KValidation_Form()
                                                     {
                                                         name = form.name
                                                     };
                            if (form.field != null)
                                foreach (var field in form.field)
                                {
                                    var formField = new KValidation_Form_Field()
                                                        {
                                                            depends = field.depends,
                                                            property = field.property
                                                        };
                                    if (field.var != null)
                                        foreach (var _var in field.var)
                                            formField.vars.Add(_var.varname, _var.varvalue);

                                    if (validationForm.fields.ContainsKey(field.property))
                                    { 
                                        //DI.log.error("Duplicate form field validation entry: form:{0} field:{1}", form.name, field.property);
                                        // already exists so lets update the existing one:

                                        validationForm.fields[field.property].depends += ", " + formField.depends;
                                        foreach (var var in formField.vars)
                                            if (validationForm.fields[field.property].vars.ContainsKey(var.Key))
                                            {
                                                if (var.Value != "") 
                                                    validationForm.fields[field.property].vars[var.Key] += ", " + var.Value;
                                            }
                                            else
                                                validationForm.fields[field.property].vars.Add(var.Key, var.Value);
                                    }
                                    else
                                        validationForm.fields.Add(field.property, formField);
                                }
                            validatorXml.forms.Add(validationForm);
                        }

                }
            }
            catch (Exception ex)
            {
                DI.log.error("in getValidationXml: {0}", ex.Message);                
            }
            return validatorXml;
        }

        public static ITilesDefinitions getTilesDefinitionXml(string fileToMap)
        {
            var tilesDefinitions = new KTilesDefinitions();
            var tilesDefinitionXml = ((tilesdefinitions)Serialize.getDeSerializedObjectFromXmlFile(fileToMap, typeof(tilesdefinitions)));
            if (tilesDefinitionXml != null)
            {
                foreach (var definition in tilesDefinitionXml.definition)
                {
                    var newTilesDefinition = new KTilesDefinition()
                                            {
                                                name = definition.name,
                                                extends = definition.extends,
                                                path = definition.path,
                                                page = definition.page
                                            };
                    if (definition.put != null)
                        foreach(var put in definition.put)
                            newTilesDefinition.puts.Add(put.name, put.value);
                    tilesDefinitions.definitions.Add(newTilesDefinition);
                }               
        }
            return tilesDefinitions;
        }        
    }
}
