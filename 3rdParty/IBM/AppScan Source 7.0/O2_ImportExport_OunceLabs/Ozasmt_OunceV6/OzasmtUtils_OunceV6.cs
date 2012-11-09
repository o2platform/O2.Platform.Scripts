// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.ImportExport.OunceLabs;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.Interfaces.O2Findings;

namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6
{
    public class OzasmtUtils_OunceV6
    {
        public static List<String> getListOfAssessmentFiles(O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            var lsAssessmentFiles = new List<string>();
            if (fadO2AssessmentDataOunceV6.dAssessmentFiles != null)
                foreach (AssessmentAssessmentFile asAssessmentFile in fadO2AssessmentDataOunceV6.dAssessmentFiles.Keys)
                {
                    lsAssessmentFiles.Add(String.Format("{0} ({1})", asAssessmentFile.filename,
                                                        asAssessmentFile.Finding.Length));
                }
            return lsAssessmentFiles;
        }

        public static List<String> getListOfFindingsPerFile(String sAssessmentFileNameToProcess,
                                                            O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            var lsFindings = new List<String>();
            if (fadO2AssessmentDataOunceV6.dAssessmentFiles != null)
                foreach (AssessmentAssessmentFile afAssessmentFile in fadO2AssessmentDataOunceV6.dAssessmentFiles.Keys)
                    if (afAssessmentFile.filename == sAssessmentFileNameToProcess)
                        // we have found the assessment file, so add the filtered findings to the List
                    {
                        foreach (AssessmentAssessmentFileFinding fFinding in afAssessmentFile.Finding)
                            lsFindings.Add(fFinding.caller_name ??
                                           getStringIndexValue(UInt32.Parse(fFinding.caller_name_id),
                                                               fadO2AssessmentDataOunceV6));
                        return lsFindings;
                    }
            return lsFindings;
        }

        public static List<String> getListOfFindingsPerVulnerabilityType(String sVulnerabilityType,
                                                                         O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            var lsFindings = new List<String>();
            if (fadO2AssessmentDataOunceV6.dVulnerabilityType != null)
                foreach (
                    AssessmentAssessmentFileFinding fFinding in
                        fadO2AssessmentDataOunceV6.dVulnerabilityType[sVulnerabilityType])

                    lsFindings.Add(fFinding.caller_name ??
                                   getStringIndexValue(UInt32.Parse(fFinding.caller_name_id), fadO2AssessmentDataOunceV6));

            return lsFindings;
        }

        public static List<String> getListOfVulnerabilityTypes(O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            var lsVulnerabilityType = new List<string>();
            if (null != fadO2AssessmentDataOunceV6)
                if (fadO2AssessmentDataOunceV6.dVulnerabilityType != null)
                    foreach (String sVulnerabilityType in fadO2AssessmentDataOunceV6.dVulnerabilityType.Keys)
                        if (sVulnerabilityType.IndexOf("Vulnerability.") > -1) // normal case
                            lsVulnerabilityType.Add(String.Format("{0} ({1})", sVulnerabilityType,
                                                                  fadO2AssessmentDataOunceV6.dVulnerabilityType[
                                                                      sVulnerabilityType].Count));
                        else // case where the vuln type has been changed (for example using the o2 custom filters
                            lsVulnerabilityType.Add(sVulnerabilityType);
            return lsVulnerabilityType;
        }

        public static List<String> getListWithUsedActionObjects(O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            var lsActionObjects = new List<String>();
            try
            {
                if (StringsAndLists.notNull(fadO2AssessmentDataOunceV6.arAssessmentRun, typeof(AssessmentRun).Name))
                    if (null != fadO2AssessmentDataOunceV6.arAssessmentRun.Assessment.Assessment)
                        foreach (Assessment aAssessment in fadO2AssessmentDataOunceV6.arAssessmentRun.Assessment.Assessment)
                        {
                            AssessmentAssessmentFile[] afAssessmentFiles = aAssessment.AssessmentFile;
                            DI.log.debug("There are {0} assessment files loaded", afAssessmentFiles.Length.ToString());
                            foreach (AssessmentAssessmentFile afAssessmentFile in afAssessmentFiles)
                                if (null != afAssessmentFile.Finding)
                                    foreach (AssessmentAssessmentFileFinding fFinding in afAssessmentFile.Finding)
                                        if (false == lsActionObjects.Contains(fFinding.actionobject_id.ToString()))
                                            lsActionObjects.Add(fFinding.actionobject_id.ToString());
                        }

                DI.log.debug("There are {0} unique ActionObjects", lsActionObjects.Count.ToString());
            }
            catch (Exception e)
            {
                DI.log.error("In getListWithUsedActionObjects: {0}", e.Message);
            }
            return lsActionObjects;
        }

        public static string getStringIndexValue(UInt32 uStringIndexId, O2AssessmentData_OunceV6 o2AssessmentDataOunceV6)
        {
            return getStringIndexValue(uStringIndexId, o2AssessmentDataOunceV6.arAssessmentRun);
        }

        public static string getStringIndexValue(UInt32 uStringIndexId, AssessmentRun assessmentRun)
        {
            if (uStringIndexId > 0 && uStringIndexId <= assessmentRun.StringIndeces.Length)
                return assessmentRun.StringIndeces[uStringIndexId - 1].value;

            return "";
        }

        public static string getFileIndexValue(UInt32 uFileIndexId, O2AssessmentData_OunceV6 assessmentDataOunceV6)
        {
            return getFileIndexValue(uFileIndexId, assessmentDataOunceV6.arAssessmentRun);
        }

        public static string getFileIndexValue(UInt32 uFileIndexId, AssessmentRun assessmentRun)
        {
            if (uFileIndexId > 0 && uFileIndexId <= assessmentRun.FileIndeces.Length)
                return assessmentRun.FileIndeces[uFileIndexId - 1].value;
            return "";
        }


        public static String getLineFromSourceCode(CallInvocation ciCallInvocation, O2AssessmentData_OunceV6 fadO2AssessmentDataOunceV6)
        {
            List<string> lsSourceCode =
                Files_WinForms.loadSourceFileIntoList(
                    fadO2AssessmentDataOunceV6.arAssessmentRun.FileIndeces[ciCallInvocation.fn_id - 1].value);
            return Files.getLineFromSourceCode(ciCallInvocation.line_number, lsSourceCode);
        }

        public static String fromAssessmentFile_get_DbId(String sAssessmentFile)
        {
            const int iMaxFileSize = 1024 * 1024 * 20;
            long fileSize = Files_WinForms.getFileSize(sAssessmentFile);
            if (fileSize > iMaxFileSize) //
            {
                DI.log.error("Skipping fromAssessmentFile_get_DbId since file size is bigger that {0}: {1}",
                             iMaxFileSize, fileSize);
                return "";
            }
            try
            {
                var xdAssessmentFile = new XmlDocument();

                xdAssessmentFile.Load(sAssessmentFile);
                XmlNodeList xnlAssessmentStats = xdAssessmentFile.GetElementsByTagName("AssessmentStats");

                foreach (XmlNode xnNode in xnlAssessmentStats)
                    if (xnNode.Attributes["language_type"] != null)
                        switch (xnNode.Attributes["language_type"].Value)
                            // hack to fix the fact that .net projects give a language_type=4
                        {
                            case "4":
                                return "3";
                            default:
                                return xnNode.Attributes["language_type"].Value; // the other ones seem to be ok
                        }
            }
            catch (Exception ex)
            {
                DI.log.error("fromAssessmentFile_get_DbId: {0}", ex.Message);
            }
            return "";
        }
        
        /// <summary>
        ///  Legacy method to add a text to string indexes (_note that is it a very inneficient process)
        /// </summary>
        /// <param name="sTextToAdd"></param>
        /// <param name="assessmentRun"></param>
        /// <returns></returns>
        public static UInt32 addTextToStringIndexes(String sTextToAdd, AssessmentRun assessmentRun)
        {
            var dStringIndexes = new Dictionary<string, uint>();
            foreach (AssessmentRunStringIndex stringIndex in assessmentRun.StringIndeces)
                dStringIndexes.Add(stringIndex.value, stringIndex.id);
            var index = addTextToStringIndexes(sTextToAdd, assessmentRun);
            assessmentRun.StringIndeces = createStringIndexArrayFromDictionary(dStringIndexes);
            return index;
        }

        public static UInt32 addTextToStringIndexes(String sTextToAdd, Dictionary<string, uint> dStringIndexes)
        {
            try
            {                
                //return 0;
                if (sTextToAdd == null)
                    return 0;                                
                // if the text already exists return it
                if (dStringIndexes.ContainsKey(sTextToAdd))
                    return dStringIndexes[sTextToAdd];
                // if not add it at the end
                UInt32 uNewId = (UInt32)dStringIndexes.Count + 1;
                dStringIndexes.Add(sTextToAdd, uNewId);                
                return uNewId;
            }
            catch (Exception ex)
            {
                DI.log.ex(ex, "in addTextToStringIndexes ");
                return 0;
            }
        }

       /* public static UInt32 addTextToStringIndexes(String sTextToAdd, AssessmentRun arAssessmentRun)
        {
            try
            {                
                //return 0;
                if (sTextToAdd == null)
                    return 0;
                var dStringIndexes = new Dictionary<string, uint>();
                foreach (AssessmentRunStringIndex siStringIndex in arAssessmentRun.StringIndeces)
                    dStringIndexes.Add(siStringIndex.value, siStringIndex.id);
                // if the text already exists return it
                if (dStringIndexes.ContainsKey(sTextToAdd))
                    return dStringIndexes[sTextToAdd];
                // if not add it at the end
                UInt32 uNewId = (UInt32)dStringIndexes.Count + 1;
                dStringIndexes.Add(sTextToAdd, uNewId);
                // and update the main string indexes
                //arAssessmentRun.StringIndeces = createStringIndexArrayFromDictionary(dStringIndexes);                
                return uNewId;
            }
            catch (Exception ex)
            {
                DI.log.ex(ex, "in addTextToStringIndexes ");
                return 0;
            }
        }*/

        /// <summary>
        ///  Legacy method to add a text to string indexes (_note that is it a very inneficient process)
        /// </summary>
        /// <param name="sTextToAdd"></param>
        /// <param name="assessmentRun"></param>
        /// <returns></returns>
        public static UInt32 addTextToFileIndexes(String sTextToAdd, AssessmentRun assessmentRun)
        {
            var dFilesIndexes = new Dictionary<string, uint>();
            foreach (AssessmentRunFileIndex siFileIndexes in assessmentRun.FileIndeces)
                dFilesIndexes.Add(siFileIndexes.value, siFileIndexes.id);
            var index = addTextToStringIndexes(sTextToAdd, assessmentRun);
            assessmentRun.FileIndeces = createFileIndexArrayFromDictionary(dFilesIndexes);            
            return index;
        }
        public static UInt32 addTextToFileIndexes(String sTextToAdd, Dictionary<string, uint> dFilesIndexes)
        {
            if (sTextToAdd == null)
                return 0;            
            // if the text already exists return it
            if (dFilesIndexes.ContainsKey(sTextToAdd))
                return dFilesIndexes[sTextToAdd];
            // if not add it at the end
            UInt32 uNewId = (UInt32)dFilesIndexes.Count + 1;
            dFilesIndexes.Add(sTextToAdd, uNewId);
            // and update the main string indexes
            
            return uNewId;
        }

        public static CallInvocation getCallInvocationObjectFromO2Trace(IO2Trace o2Trace, Dictionary<string, uint> dStringIndexes, Dictionary<string, uint> dFilesIndexes)
        {
          //  return new CallInvocation();
            var callInvocation = new CallInvocation
                                     {
                                         cn_id = addTextToStringIndexes(o2Trace.clazz, dStringIndexes),
                                         column_number = o2Trace.columnNumber,
                                         cxt_id = addTextToStringIndexes(o2Trace.context, dStringIndexes),
                                         fn_id = addTextToFileIndexes(o2Trace.file, dFilesIndexes),
                                         line_number = o2Trace.lineNumber,
                                         mn_id = addTextToStringIndexes(o2Trace.method, dStringIndexes),
                                         ordinal = o2Trace.ordinal,
                                         sig_id = addTextToStringIndexes(o2Trace.signature, dStringIndexes),
                                         taint_propagation = o2Trace.taintPropagation,
                                         Text = o2Trace.text.ToArray(),
                                         trace_type = Convert.ToUInt32(o2Trace.traceType)
                                     };

            if (o2Trace.childTraces != null) // means there are child traces
            {
                var childCallInvocation = new List<CallInvocation>();
                foreach (O2Trace childO2trace in o2Trace.childTraces)
                    childCallInvocation.Add(getCallInvocationObjectFromO2Trace(childO2trace, dStringIndexes, dFilesIndexes));
                callInvocation.CallInvocation1 = childCallInvocation.ToArray();
            }            
            return callInvocation;
        }

        public static List<IO2Trace> getO2TraceFromCallInvocation(CallInvocation[] callInvocations,
                                                                  AssessmentRun assessmentRun)
        {
            var o2Traces = new List<IO2Trace>();
            if (callInvocations != null)
            {
                foreach (CallInvocation callInvocation in callInvocations)
                {
                    var o2Trace = new O2Trace
                                      {
                                          clazz = getStringIndexValue(callInvocation.cn_id, assessmentRun),
                                          columnNumber = callInvocation.column_number,
                                          context = getStringIndexValue(callInvocation.cxt_id, assessmentRun),
                                          file = getFileIndexValue(callInvocation.fn_id, assessmentRun),
                                          lineNumber = callInvocation.line_number,
                                          method = getStringIndexValue(callInvocation.mn_id, assessmentRun),
                                          ordinal = callInvocation.ordinal,
                                          // for the signature try to use the sig_id and if that is 0 then use mn_id
                                          signature = getStringIndexValue((callInvocation.sig_id != 0) ? callInvocation.sig_id : callInvocation.mn_id, assessmentRun),
                                          taintPropagation = callInvocation.taint_propagation,
                                          traceType =
                                              (TraceType)
                                              Enum.Parse(typeof(TraceType),
                                                         callInvocation.trace_type.ToString())
                                      };
                    if (callInvocation.Text != null)
                        o2Trace.text = new List<string>(callInvocation.Text);

                    //if (callInvocation.CallInvocation1 != null) // means there are child traces
                    //{
                    o2Trace.childTraces = getO2TraceFromCallInvocation(callInvocation.CallInvocation1, assessmentRun);
                    /*new List<O2Trace>();
                    
                    foreach (CallInvocation childCallInvocation in callInvocation.CallInvocation1)
                        o2Trace.childTraces.Add(getO2TraceFromCallInvocation(childCallInvocation, assessmentRun));*/
                    //}
                    o2Traces.Add(o2Trace);
                }
            }
            return o2Traces;
        }

        public static IO2Finding getO2Finding(AssessmentAssessmentFileFinding finding,
                                              AssessmentAssessmentFile assessmentFile, AssessmentRun assessmentRun)
        {
            var o2Finding = new O2Finding
                                {
                                    actionObject = finding.actionobject_id,
                                    columnNumber = finding.column_number,
                                    confidence = finding.confidence,
                                    exclude = finding.exclude,
                                    file = assessmentFile.filename,
                                    lineNumber = finding.line_number,
                                    ordinal = finding.ordinal,
                                    propertyIds = finding.property_ids,
                                    recordId = finding.record_id,
                                    severity = finding.severity,
                                    o2Traces = getO2TraceFromCallInvocation(finding.Trace, assessmentRun),
                                };

            if (finding.cxt_id != null)
                o2Finding.context = getStringIndexValue(UInt32.Parse(finding.cxt_id), assessmentRun);

            o2Finding.callerName = finding.caller_name;
            if (o2Finding.callerName == null && finding.caller_name_id != null)
                o2Finding.callerName = getStringIndexValue(UInt32.Parse(finding.caller_name_id), assessmentRun);

            o2Finding.projectName = finding.project_name;
            if (o2Finding.projectName == null && finding.project_name_id != null)
                o2Finding.projectName = getStringIndexValue(UInt32.Parse(finding.project_name_id), assessmentRun);

            o2Finding.vulnName = finding.vuln_name;
            if (o2Finding.vulnName == null && finding.vuln_name_id != null)
                o2Finding.vulnName = getStringIndexValue(UInt32.Parse(finding.vuln_name_id), assessmentRun);

            o2Finding.vulnType = finding.vuln_type;
            if (o2Finding.vulnType == null && finding.vuln_type_id != null)
                o2Finding.vulnType = getStringIndexValue(UInt32.Parse(finding.vuln_type_id), assessmentRun);

            if (finding.Text != null)
                o2Finding.text = new List<string>(finding.Text);

            OzasmtUtils.fixExternalSourceSourceMappingProblem(o2Finding);
            return o2Finding;
        }

        public static AssessmentAssessmentFileFinding getAssessmentAssessmentFileFinding(IO2Finding o2Finding, Dictionary<string, uint> dStringIndexes, Dictionary<string, uint> dFilesIndexes)
        {
            try
            {                
                var finding = new AssessmentAssessmentFileFinding
                {
                    actionobject_id = o2Finding.actionObject,
                    caller_name_id =
                        addTextToStringIndexes(o2Finding.callerName, dStringIndexes).ToString(),
                    column_number = o2Finding.columnNumber,
                    confidence = o2Finding.confidence,
                    cxt_id = addTextToStringIndexes(o2Finding.context, dStringIndexes).ToString(),
                    exclude = o2Finding.exclude,
                    line_number = o2Finding.lineNumber,
                    ordinal = o2Finding.ordinal,
                    project_name_id =
                        addTextToStringIndexes(o2Finding.projectName, dStringIndexes).ToString(),
                    property_ids = o2Finding.propertyIds,
                    record_id = o2Finding.recordId,
                    severity = o2Finding.severity,
                    Text = (o2Finding.text!=null) ? o2Finding.text.ToArray(): null,
                    vuln_name_id = addTextToStringIndexes(o2Finding.vulnName, dStringIndexes).ToString(),
                    vuln_type_id = addTextToStringIndexes(o2Finding.vulnType, dStringIndexes).ToString()
                };

                if (o2Finding.o2Traces.Count > 0)
                {
                    var callInvocations = new List<CallInvocation>();
                    foreach (O2Trace o2trace in o2Finding.o2Traces)
                        callInvocations.Add(getCallInvocationObjectFromO2Trace(o2trace, dStringIndexes, dFilesIndexes));
                    finding.Trace = callInvocations.ToArray();
                }                
                //if (o2Finding.o2Trace != null)
                //    finding.Trace = new[] {getCallInvocationObjectFromO2Trace((o2Finding.o2Trace), assessmentRun)};
                return finding;
            }
            catch (Exception ex)
            {
                DI.log.ex(ex, "in getAssessmentAssessmentFileFinding");
            }
            return null;
        }

        public static AssessmentRunStringIndex[] createStringIndexArrayFromDictionary(
            Dictionary<String, UInt32> dNewStringIndex)
        {
            var lariAssessmentRunStringIndex = new List<AssessmentRunStringIndex>();
            foreach (String sKey in dNewStringIndex.Keys)
            {
                var ariAssessmentRunStringIndex = new AssessmentRunStringIndex
                                                      {
                                                          value = sKey,
                                                          id = dNewStringIndex[sKey]
                                                      };
                lariAssessmentRunStringIndex.Add(ariAssessmentRunStringIndex);
            }            
            return lariAssessmentRunStringIndex.ToArray();
        }

        public static AssessmentRunFileIndex[] createFileIndexArrayFromDictionary(
            Dictionary<String, UInt32> dNewFileIndex)
        {
            var lariAssessmentRunFileIndex = new List<AssessmentRunFileIndex>();
            foreach (String sKey in dNewFileIndex.Keys)
            {
                var ariAssessmentRunFileIndex = new AssessmentRunFileIndex { value = sKey, id = dNewFileIndex[sKey] };
                lariAssessmentRunFileIndex.Add(ariAssessmentRunFileIndex);
            }
            return lariAssessmentRunFileIndex.ToArray();
        }

        public static AssessmentRun getDefaultAssessmentRunObject()
        {
            // this is what we need to create a default assessment
            var arNewAssessmentRun = new AssessmentRun
                                         {
                                             name = "DefaultAssessmentRun",
                                             AssessmentStats = new AssessmentStats(),
                                             FileIndeces = new AssessmentRunFileIndex[] { },
                                             StringIndeces = new AssessmentRunStringIndex[] { }
                                         };
            var armMessage = new AssessmentRunMessage
                                 {
                                     id = 0,
                                     message =
                                         ("Custom Assessment Run File created on " +
                                          DateTime.Now)
                                 };
            arNewAssessmentRun.Messages = new[] { armMessage };
            arNewAssessmentRun.Assessment = new AssessmentRunAssessment { Assessment = new[] { new Assessment() } };
            // need to populate the date 
            arNewAssessmentRun.AssessmentStats.date =
                (uint)(DateTime.Now.Minute * 1000 + DateTime.Now.Second * 50 + DateTime.Now.Millisecond);
            // This should be enough to create unique timestamps 
            return arNewAssessmentRun;
        }

        public static bool SaveAssessmentRun(AssessmentRun arAssessmentRun, String sTargetFile)
        {
            return createSerializedXmlFileFromAssessmentRunObject(arAssessmentRun, sTargetFile);
        }

        public static AssessmentRun LoadAssessmentRun(String sFileToLoad)
        {
            return getAssessmentRunObjectFromXmlFile(sFileToLoad);
        }

        public static AssessmentRun getAssessmentRunObjectFromXmlFile(String sFileToProcess)
        {
            return (AssessmentRun)Serialize.getDeSerializedObjectFromXmlFile(sFileToProcess, typeof(AssessmentRun));
        }

        public static bool createSerializedXmlFileFromAssessmentRunObject(AssessmentRun arAssessmentRunObjectToProcess,
                                                                          String sTargetFile)
        {
            return Serialize.createSerializedXmlFileFromObject(arAssessmentRunObjectToProcess, sTargetFile, null);
        }

        internal static string calculateAssessmentNameFromScans(AssessmentRun assessmentRun)
        {
            string assessmentName = "";
            //if (null != assessmentRun != null && null != assessmentRun.Assessment != null && null != assessmentRun.Assessment.Assessment)                                
            //    foreach (Assessment assessment in assessmentRun.Assessment.Assessment)
            var results = from assessment in assessmentRun.Assessment.Assessment
                             select new {assessment.assessee_name, assessment.owner_name};
            //var assessmentNames = from AssessmentRunAssessment assessment in assessmentRun.Assessment.Assessment select assessment;            

            foreach (var result in results)            
                assessmentName += 
                    (string.IsNullOrEmpty(result.assessee_name) == false) ? result.assessee_name : (
                    (string.IsNullOrEmpty(result.owner_name) == false) ? Path.GetFileNameWithoutExtension(result.owner_name) : "") + " : ";
            
            //if (assessmentName.Length > 0)
            //    assessmentName = assessmentName.Substring(0, assessmentName.Length - 3);
            return assessmentName;
        }
    }
}
