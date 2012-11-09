// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Interfaces.FrameworkSupport.J2EE;

namespace O2.Core.FileViewers.Struts_1_5
{
   [Serializable]
   public class KStrutsConfigXml : IStrutsConfigXml
    {
        public List<IStrutsConfig_FormBean> formBeans { get; set; }
        public Dictionary<string, string> globalForwards { get; set; }
        public List<IStrutsConfig_Action> actionmappings { get; set; }
        public List<IStrutsConfig_PlugIn> plugIns { get; set; }
        public Dictionary<string, List<string>> resolvedViews { get; set; }    

        public KStrutsConfigXml()
        {
            formBeans = new List<IStrutsConfig_FormBean>();
            globalForwards = new Dictionary<string, string>();
            actionmappings = new List<IStrutsConfig_Action>();
            plugIns = new List<IStrutsConfig_PlugIn>();
            resolvedViews = new Dictionary<string, List<string>>();
        }
    }
    [Serializable]
    public class KStrutsConfig_FormBean : IStrutsConfig_FormBean
    {
        public string name { get; set; }
        public string type { get; set; }
        public string extends { get; set; }
        public bool hasValidationMapping { get; set; }
        public Dictionary<string, IStrutsConfig_FormBean_Field> fields { get; set; }

        //public Dictionary<string, string> properties { get; set; }        

        public KStrutsConfig_FormBean()
        {
            //properties = new Dictionary<string, string>();  
            fields = new Dictionary<string, IStrutsConfig_FormBean_Field>();
        }
    }
    [Serializable]
    public class KStrutsConfig_FormBean_Field: IStrutsConfig_FormBean_Field
    {
        public bool hasValidationMapping { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string initial { get; set; }
        public string depends { get; set; }    
        public Dictionary<string, string> validators { get; set; }

        public KStrutsConfig_FormBean_Field()
        {
            validators = new Dictionary<string, string>();
        }
    }
    [Serializable]
    class KStrutsConfig_Action : IStrutsConfig_Action
    {
        public string path { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string scope { get; set; }
        public string input { get; set; }
        public string unknown { get; set; }
        public string validate { get; set; }
        public string extends { get; set; }
        public string parameter { get; set; }

        public List<IStrutsConfig_Action_Forward> forwards { get; set; }
        public KStrutsConfig_Action()
        {
            forwards = new List<IStrutsConfig_Action_Forward>();
        }
    }
    [Serializable]
    class KStrutsConfig_Action_Forward : IStrutsConfig_Action_Forward
    {
        public string name { get; set; }
        public string path { get; set; }
        public string redirect { get; set; }

        public override string ToString()
        {
            return string.Format("[{0} = {1} {2}] ", name, path,(string.IsNullOrEmpty(redirect)) ? "" : "redirect=" + redirect);
        }
    }
    [Serializable]
    class KStrutsConfig_PlugIn : IStrutsConfig_PlugIn
    {
        public string className { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public KStrutsConfig_PlugIn()
        {
            properties = new Dictionary<string, string>();
        }
    }
}
