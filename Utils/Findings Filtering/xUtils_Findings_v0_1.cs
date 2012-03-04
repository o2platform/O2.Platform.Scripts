// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
//O2Tag_OnlyAddReferencedAssemblies
//O2Ref:System.dll
using System.IO;
using System.Threading;
using System.Collections.Generic;
//O2Ref:System.Windows.Forms.dll
using System.Windows.Forms;
//O2Ref:O2_Core_FileViewers.dll
using O2.Core.FileViewers.JoinTraces;
//O2Ref:O2_DotNetWrappers.dll
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
//O2Ref:O2_ImportExport_OunceLabs.dll
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
//O2Ref:O2_Kernel.dll
using O2.Interfaces.O2Core;
using O2.Interfaces.O2Findings;
using O2.Kernel;
//O2Ref:O2_Views_ASCX.dll
//O2Ref:O2_Interfaces.dll

using O2.Views.ASCX.O2Findings;

namespace O2.XRules.Database._Rules
{
    public class XUtils_Findings_v0_1
    {
        private static readonly IO2Log log = PublicDI.log;

        public static Thread openFindingsInNewWindow(List<IO2Finding> o2Findings)
        {
            return ascx_FindingsViewer.openInFloatWindow(o2Findings);
        }

        public static Thread openFindingsInNewWindow(List<IO2Finding> o2Findings, string windowTitle)
        {
            return ascx_FindingsViewer.openInFloatWindow(o2Findings, windowTitle);
        }


        public static List<IO2Finding> loadFindingsFile(string fileToLoad)
        {
            var o2Assessment = new O2Assessment(new O2AssessmentLoad_OunceV6(), fileToLoad);
            log.info("there are {0} findings loaded in this file", o2Assessment.o2Findings.Count);
            return o2Assessment.o2Findings;
        }
        
        public static string saveFindings(List<IO2Finding> o2Findings)
        {
            var savedFile = new O2Assessment(o2Findings).save(new O2AssessmentSave_OunceV6()); 
            log.info("Assessemnt File saved with {0} findings: {1}", o2Findings.Count, savedFile);
            return savedFile;
        }
        public static void saveFindings(List<IO2Finding> o2Findings, string pathToSaveFindings)
        {
        	new O2Assessment(o2Findings).save(new O2AssessmentSave_OunceV6(), pathToSaveFindings); 
        	log.info("Assessemnt File saved with {0} findings: {1}", o2Findings.Count, pathToSaveFindings);
        }

        public static List<IO2Finding> loadFindingsFiles(List<string> filesToLoad)
        {
            var loadedFindings = new List<IO2Finding>();
            foreach (var fileToLoad in filesToLoad)
            {
                var o2Findings = loadFindingsFile(fileToLoad);
                loadedFindings.AddRange(o2Findings);
            }
            log.info("Total # of findings loaded: {0}", loadedFindings);
            return loadedFindings;
        }

        public static List<IO2Finding> calculateFindings(List<IO2Finding> findingsToFilter, string sourceSignatures, string sinkSignatures)
        {
            return O2FindingsHelpers.calculateFindings(findingsToFilter, sourceSignatures, sinkSignatures);
        }

        // Description: this function loads multiple Ozasmt Files (not recursively)
        public static List<IO2Finding> loadMultipleOzasmtFiles(string pathToOzastmFilesToLoad)
        {
            return loadMultipleOzasmtFiles(pathToOzastmFilesToLoad,"*.ozasmt",  false);
        }

        // Description: this function loads multiple Ozasmt Files (recursively or not, based on filter)
        public static List<IO2Finding> loadMultipleOzasmtFiles(string pathToOzastmFilesToLoad, string filter, bool searchRecursively)
        {
            var o2Findings = new List<IO2Finding>();
            if (Directory.Exists(pathToOzastmFilesToLoad))
                foreach (var fileToLoad in Files.getFilesFromDir_returnFullPath(pathToOzastmFilesToLoad, filter, searchRecursively))
                {
                    log.info("loading findings from file: {0}", fileToLoad);
                    o2Findings.AddRange(loadFindingsFile(fileToLoad));
                }
            return o2Findings;
        }
        
        public static List<IO2Finding> mapJoinPoints_HashTagsOn_Sinks(List<IO2Finding> o2findings)
        {
            var results = new List<IO2Finding>();
            foreach (O2Finding o2Finding in o2findings)
            {
                var hashTagName = JoinOnAttributes.extractNameFromContext(o2Finding.SinkContext, "\"", "\"");
                // make this the last trace
                if (hashTagName != "")
                {
                    var copyOfO2Finding = (O2Finding)OzasmtCopy.createCopy(o2Finding);
                    var joinLocation = copyOfO2Finding.o2Traces[0].file;

                    // insert JoinSink
                    copyOfO2Finding.addTrace(copyOfO2Finding.getSink(), hashTagName, TraceType.O2JoinSink);

                    // insert Location
                    copyOfO2Finding.insertTrace(joinLocation, TraceType.O2JoinLocation);

                    results.Add(copyOfO2Finding);
                }
            }
            return results;
        }

        public static List<IO2Finding> mapJoinPoints_HashTagsOn_Sources(List<IO2Finding> o2findings)
        {
            var results = new List<IO2Finding>();
            foreach (O2Finding o2Finding in o2findings)
            {
                var hashTagName = JoinOnAttributes.extractNameFromContext(o2Finding.SourceContext, "\"", "\"");
                // make this the first trace
                if (hashTagName != "")
                {
                    //var newO2Trace = new O2Trace(hashTagName, TraceType.O2JoinSource);                    

                    var copyOfO2Finding = (O2Finding)OzasmtCopy.createCopy(o2Finding);
                    var joinLocation = copyOfO2Finding.o2Traces[0].file;

                    // insert JoinSource
                    copyOfO2Finding.insertTrace(hashTagName, TraceType.O2JoinSource);

                    // insert Location                    
                    copyOfO2Finding.insertTrace(joinLocation, TraceType.O2JoinLocation);

                    //newO2Trace.childTraces.AddRange(o2Finding.o2Traces);
                    //copyOfO2Finding.o2Traces = new List<IO2Trace> { newO2Trace };
                    results.Add(copyOfO2Finding);
                }
            }
            return results;
        }



    }
}
