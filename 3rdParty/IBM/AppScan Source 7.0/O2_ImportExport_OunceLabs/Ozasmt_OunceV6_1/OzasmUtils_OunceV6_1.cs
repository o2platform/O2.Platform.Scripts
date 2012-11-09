// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.Interfaces.O2Findings;

namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6_1
{
    public class OzasmtUtils_OunceV6_1
    {
        public static bool importOzasmtAssessmentIntoO2Assessment(string fileToLoad, IO2Assessment o2Assessment)
        {
            try
            {
                AssessmentRun assessmentRunToImport = getAssessmentRunObjectFromXmlFile(fileToLoad);
                o2Assessment.name = assessmentRunToImport.name;
                if (null != assessmentRunToImport.Assessment.Assessment)
                    foreach (Assessment assessment in assessmentRunToImport.Assessment.Assessment)
                        if (null != assessment.AsmntFile)
                            foreach (AssessmentAsmntFile asmntFile in assessment.AsmntFile)
                                if (asmntFile.Finding != null)
                                    foreach (AssessmentAsmntFileFinding finding in asmntFile.Finding)
                                        o2Assessment.o2Findings.Add(getO2Finding(finding, assessmentRunToImport));
                return true;
            }
            catch (Exception ex)
            {
                DI.log.ex(ex, "in OzasmtUtils_OunceV6_1.importOzasmtAssessmentIntoO2Assessment");
            }
            return false;
        }

        public static AssessmentRun getAssessmentRunObjectFromXmlFile(String sFileToProcess)
        {
            return (AssessmentRun)Serialize.getDeSerializedObjectFromXmlFile(sFileToProcess, typeof(AssessmentRun));
        }

        private static IO2Finding getO2Finding(AssessmentAsmntFileFinding finding, AssessmentRun assessmentRunToImport)
        {
            var o2Finding = new O2Finding();
            addFindingDataToO2Finding(finding, o2Finding, assessmentRunToImport);
            addTraceToO2Finding(finding.trace, o2Finding, assessmentRunToImport);
            OzasmtUtils.fixExternalSourceSourceMappingProblem(o2Finding);           // fix the 'ExternalSource Source' problem
            return o2Finding;
        }

        private static void addFindingDataToO2Finding(AssessmentAsmntFileFinding finding, IO2Finding o2Finding, AssessmentRun assessmentRun)
        {
            AssessmentRunFindingData findingData = assessmentRun.FindingDataPool[finding.data_id-1];
            AssessmentRunSite siteData = assessmentRun.SitePool[findingData.site_id - 1];
            if (findingData.id != finding.data_id || siteData.id != findingData.site_id)
                DI.log.error("in addFindingDataToO2Finding findingData.id != (finding.data_id-1) or siteData.id != (findingData.site_id - 1)");
            else
            {
                o2Finding.actionObject = findingData.ao_id;
                o2Finding.callerName = getStringIndexValue(siteData.caller, assessmentRun);
                o2Finding.columnNumber = siteData.cn;
                o2Finding.confidence = (byte) findingData.conf;
                o2Finding.context = getStringIndexValue(siteData.cxt, assessmentRun);                
                o2Finding.exclude = finding.excluded;
                o2Finding.file = getFileIndexValue(siteData.file_id, assessmentRun);
                o2Finding.lineNumber = siteData.ln;
                o2Finding.method = getStringIndexValue(siteData.method, assessmentRun);
                o2Finding.ordinal = siteData.ord; 
                o2Finding.projectName = getStringIndexValue(findingData.project_name, assessmentRun);
                o2Finding.propertyIds = findingData.prop_ids; /**/
                o2Finding.recordId = findingData.rec_id;
                o2Finding.severity = (byte) findingData.sev;
             //   o2Finding.signature = getStringIndexValue(siteData.sig, assessmentRun);
                o2Finding.text = null; /**/
                o2Finding.vulnName = getStringIndexValue(siteData.sig, assessmentRun); /*making the sig the vuln name*/
                o2Finding.vulnType = getStringIndexValue(findingData.vtype, assessmentRun);                

            }                                                                                                            
        }

        private static void addTraceToO2Finding(string traces, IO2Finding o2Finding, AssessmentRun assessmentRun)
        {
            if (false == string.IsNullOrEmpty(traces))
            {
                var splittedTraces = traces.Split(',');
                var traceStack = new Stack<List<IO2Trace>>(); // use to keep track of where we add the trace
                traceStack.Push(o2Finding.o2Traces);          // the first one is the main o2Findings.o2Traces 
                foreach(var traceItem in splittedTraces)
                {                    
                    var splittedTrace = traceItem.Split('.');   // in this version the dots mean how many nodes we have to go up
                    int traceIndex;
                    if (Int32.TryParse(splittedTrace[0], out traceIndex))
                    {
                        AssessmentRunTaint taint = assessmentRun.TaintPool[traceIndex - 1];
                        AssessmentRunSite siteData = assessmentRun.SitePool[taint.site_id - 1];
                        var o2Trace = new O2Trace
                                          {
                                              caller = getStringIndexValue(siteData.caller, assessmentRun),
                                              columnNumber = siteData.cn,
                                              context = getStringIndexValue(siteData.cxt, assessmentRun),
                                              file = getFileIndexValue(siteData.file_id, assessmentRun),
                                              lineNumber = siteData.ln,
                                              method = getStringIndexValue(siteData.method, assessmentRun),
                                              ordinal = siteData.ord,
                                              signature = getStringIndexValue(siteData.sig, assessmentRun),
                                              argument = taint.arg,
                                              direction = taint.dir,
                                              traceType =((TraceType) Enum.Parse(typeof (TraceType), taint.trace_type.ToString()))                                              
                                          };                        
                        //o2Trace.clazz = getStringIndexValue(,assessmentRun);  // check if siteData.caller is a good match for clazz
                        //o2Trace.taintPropagation = ;
                        //o2Trace.text = ;
                        traceStack.Peek().Add(o2Trace); // add the current trace as a child of the the item on the top of traceStack
                        traceStack.Push(o2Trace.childTraces);   // and make the current trace the item on the top of traceStack (which will be changed if there were dots in the traceItem (handled below))                        
                    }
                    else
                    {
                        DI.log.error("in addTraceToO2Finding , could not parse into int {0} from {1}", splittedTrace[0], traceItem);
                    }
                    if (splittedTrace.Length > 1) // means there were dots in the traceitem
                        for (var i = 1; i < splittedTrace.Length; i++)
                            traceStack.Pop();
                }
                o2Finding.o2Traces[0].signature += traces;
            }
        }
        

        public static string getStringIndexValue(UInt32 uStringIndexId, AssessmentRun assessmentRun)
        {
            if (uStringIndexId > 0 && uStringIndexId <= assessmentRun.StringPool.Length)
                return assessmentRun.StringPool[uStringIndexId - 1].value;
            return "";
        }

        public static string getFileIndexValue(UInt32 uFileIndexId, AssessmentRun assessmentRun)
        {
            if (uFileIndexId > 0 && uFileIndexId <= assessmentRun.FilePool.Length)
                return assessmentRun.FilePool[uFileIndexId - 1].value;
            return "";
        }
    }
}
