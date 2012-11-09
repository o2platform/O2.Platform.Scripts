// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.J2EE
{
    [Serializable]
    public class KWebXml : IWebXml
    {
        public string description { get; set;}        
        public string displayName { get; set;}
        public string listenerClass { get; set; }
        public List<IWebXml_Filter> filters  { get; set;}        
        public List<IWebXml_Filter_Mapping> filterMappings { get; set; }
        public List<IWebXml_Servlet> servlets { get; set; }
        public List<IWebXml_Servlet_Mapping> servletMappings { get; set; }

        public KWebXml()
        {
            description = "";
            displayName = "";
            listenerClass = "";
            filters = new List<IWebXml_Filter>();
            filterMappings = new List<IWebXml_Filter_Mapping>();
            servlets = new List<IWebXml_Servlet>();
            servletMappings = new List<IWebXml_Servlet_Mapping>();
        }

        public Dictionary<string, IWebXml_Servlet> getServletsDictionary()
        {
            var servletsDictionary = new Dictionary<string, IWebXml_Servlet>();
            foreach(var servlet in servlets)
                servletsDictionary.Add(servlet.servletName, servlet);
            return servletsDictionary;
        }

        public Dictionary<IWebXml_Filter_Mapping, IWebXml_Filter> getFiltersDictionary()
        {
            var filtersDictionary = new Dictionary<IWebXml_Filter_Mapping, IWebXml_Filter>();

            var currentFilters = new Dictionary<string, IWebXml_Filter>();
            foreach (var filter in filters)
                currentFilters.Add(filter.filterName, filter);

            foreach (var filterMapping in filterMappings)
                if (currentFilters.ContainsKey(filterMapping.filterName))                
                    filtersDictionary.Add(filterMapping, currentFilters[filterMapping.filterName]);                
                else
                    DI.log.error("in getFiltersDictionary, could not map filter: {0}", filterMapping.filterName);
            return filtersDictionary;
        }
    }

    [Serializable]
    public class KWebXml_Filter : IWebXml_Filter
    {
        public string filterClass { get; set; }
        public string filterName { get; set; }        
    }

    [Serializable]
    public class kWebXml_Filter_Mapping : IWebXml_Filter_Mapping
    {
        public string filterName { get; set; }
        public string urlPattern { get; set; }
        public string dispatcher { get; set; }
    }

    [Serializable]
    public class KWebXml_Servlet : IWebXml_Servlet
    {
        public string servletName { get; set; }
        public string servletClass { get; set; }
        public string loadOnStartUp { get; set; }
        public Dictionary<string, string> initParam { get; set; }
        public KWebXml_Servlet()
        {
            initParam = new Dictionary<string, string>();
        }
    }

    [Serializable]
    public class KWebXml_Servlet_Mapping : IWebXml_Servlet_Mapping
    {
        public string servletName { get; set; }
        public string urlPattern { get; set; }
    }
    
}
