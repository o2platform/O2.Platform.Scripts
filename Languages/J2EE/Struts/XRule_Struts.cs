// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Collections.Generic;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.CoreLib.Interfaces.J2EE;

//O2File:XUtils_Findings_v0_1.cs
//O2File:XUtils_Struts_v0_1.cs
//O2File:XUtils_Struts_Joins_V0_1.cs

namespace O2.XRules.Database._Rules.J2EE.Struts
{
    public class XRule_Struts : KXRule
    {

        static string strutsMappingsFile = @"...\_OunceApplication\O2Data\....O2StrutsMapping";
        static string baseO2FindingsFile = @"...\_OunceApplication\O2Data\OSA - ...  11-3-09 807PM.ozasmt";    	

        
        public static bool showResultsInO2RulesStrutsGUI = false;
        public static bool showResultsInNewFindingsViewer = false;

        public XRule_Struts()
        {
            Name = "_WORKING Rule fo Struts";
        }

        [XRule(Name="Test with local files")]
        public static void Test()
        {
            var baseO2Findings = XUtils_Findings_v0_1.loadFindingsFile(baseO2FindingsFile);
            var strutsMapping = (KStrutsMappings)Serialize.getDeSerializedObjectFromBinaryFile(strutsMappingsFile, typeof(KStrutsMappings));

            var o2Findings = strutsRule_fromGetParameterToPringViaGetSetAttributeJoins(baseO2Findings, strutsMapping);
            XUtils_Findings_v0_1.openFindingsInNewWindow(o2Findings);
        }	   

        [XRule(Name = "_StrutsRule.from.GetParameter.to.Print.via.SetGetAttributeJoins")]
        public static List<IO2Finding> strutsRule_fromGetParameterToPringViaGetSetAttributeJoins(List<IO2Finding> baseO2Findings, IStrutsMappings strutsMappings)
        {        
            PublicDI.log.info("executing rule: StrutsRule.from.GetParameter.to.Print.via.SetGetAttributeJoins with {0} fingings and {1} action servlets", 
                              baseO2Findings.Count, strutsMappings.actionServlets.Count);

            var taintSources_SourceRegEx = @"getParameter\(java.lang.String\)";
            var taintSources_SinkRegEx = @"setAttribute\(java.lang.String";

            var finalSinks_SourceRegEx = @"getAttribute\(java.lang.String\)";
            var finalSinks_SinkRegEx = @"print";

            var results = executeStrutsRule(baseO2Findings, strutsMappings, taintSources_SourceRegEx, taintSources_SinkRegEx, finalSinks_SourceRegEx, finalSinks_SinkRegEx);
            return results;
        }

        [XRule(Name = "StrutsRule.AnyToAny.via.SetGetAttributeJoins")]
        public static List<IO2Finding> test2(List<IO2Finding> baseO2Findings, KStrutsMappings strutsMappings)
        {
            var taintSources_SourceRegEx = @"";
            var taintSources_SinkRegEx = @"setAttribute\(java.lang.String";

            var finalSinks_SourceRegEx = @"getAttribute\(java.lang.String\)";
            var finalSinks_SinkRegEx = @"";
            var results = executeStrutsRule(baseO2Findings, strutsMappings, taintSources_SourceRegEx, taintSources_SinkRegEx, finalSinks_SourceRegEx, finalSinks_SinkRegEx);
            return results;
        }

        [XRule(Name = "StrutsRule.AnyToAny.via.Any")]
        public static List<IO2Finding> test3(List<IO2Finding> baseO2Findings, KStrutsMappings strutsMappings)
        {
            var taintSources_SourceRegEx = @"";
            var taintSources_SinkRegEx = @"";

            var finalSinks_SourceRegEx = @"";
            var finalSinks_SinkRegEx = @"";
            var results = executeStrutsRule(baseO2Findings, strutsMappings, taintSources_SourceRegEx, taintSources_SinkRegEx, finalSinks_SourceRegEx, finalSinks_SinkRegEx);
            return results;
        }

        public static List<IO2Finding> executeStrutsRule(List<IO2Finding> baseO2Findings, IStrutsMappings strutsMappings,string taintSources_SourceRegEx, string taintSources_SinkRegEx, string finalSinks_SourceRegEx, string finalSinks_SinkRegEx)
        {
            var xRulesObject = XUtils_Struts_Joins_V0_1_Helpers.executeRule_AndReturn_XRuleStrutsObject(
                baseO2Findings, strutsMappings, taintSources_SourceRegEx, taintSources_SinkRegEx,
                finalSinks_SourceRegEx, finalSinks_SinkRegEx, joinPointFilter);

            if (showResultsInO2RulesStrutsGUI)
                xRulesObject.showFinalResultsIn_O2RulesStrutsGUI();
            if (showResultsInNewFindingsViewer)
                xRulesObject.showFinalResultsIn_fidingsViewer();
            return xRulesObject.getResults();
        }

        public static string joinPointFilter(string dataToFilter)
        {
            return dataToFilter
                .Replace("\\__",".")
                .Replace("\\_", ".")
                .Replace("_45_","-")
                .Replace(".java", "")
                .Replace(".jsp", "")
                .Replace("\\", ".").Replace('/', '.');
        }
               
        /*public static void test()
		{
			new XRule_Struts().calculateFindings(baseO2Findings,strutsMappingFile);
		}*/
    }
}