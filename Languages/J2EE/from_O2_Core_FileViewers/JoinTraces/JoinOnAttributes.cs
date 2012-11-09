// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.DotNetWrappers.O2Findings;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.Interfaces.O2Findings;
using O2.Views.ASCX.O2Findings;

namespace O2.Core.FileViewers.JoinTraces
{
    public class JoinOnAttributes
    {
        /*public static void joinTraces()
        {
            var sinkFindings = new List<IO2Finding>();
            var sourceFindings = new List<IO2Finding>();

            findTracesToJoin(sinkFindings, sourceFindings);

            fixSinkVulnNamesBasedOnSinkContextHashMapKey("Findings_With_HashMap_To_Join_", sinkFindings);

            fixSourceVulnNamesBasedOnSinkContextHashMapKey("Findings_With_HashMap_To_Join_", sourceFindings);

            var results = joinTracesWhereSinkMatchesSource(sinkFindings, sourceFindings);

            var newAssessmentFile = new O2Assessment(results);
            var savedFile = newAssessmentFile.save(new O2AssessmentSave_OunceV6());
            DI.log.info("Filtered results saved to: {0}", savedFile);


            ascx_FindingsViewer.openInFloatWindow(results);
        }*/

        public static void findTracesToJoin(string ozasmtFileToLoad)
        {
            var sinkFindings = new List<IO2Finding>();
            var sourceFindings = new List<IO2Finding>();

            findTracesToJoin(ozasmtFileToLoad,sinkFindings, sourceFindings);

            var results = new List<IO2Finding>();
            results.AddRange(sinkFindings);
            results.AddRange(sourceFindings);

            ascx_FindingsViewer.openInFloatWindow(results.ToList());
        }

        public static void findTracesToJoin(string ozasmtFileToLoad, List<IO2Finding> sinkFindings, List<IO2Finding> sourceFindings)
        {
            findTracesToJoin(ozasmtFileToLoad, "setAttribute", "getAttribute", sinkFindings, sourceFindings);

            //var results = new List<IO2Finding>();

        }

        public static void findTracesToJoin(string ozasmtFileToLoad, string sinkMethodToFind, string sourceMethodToFind,
                                                 List<IO2Finding> sinkFindings, List<IO2Finding> sourceFindings)
        {
            var o2Assessment = new O2Assessment(new O2AssessmentLoad_OunceV6(), ozasmtFileToLoad);

            foreach (O2Finding o2Finding in o2Assessment.o2Findings)
                if (o2Finding.Sink.IndexOf(sinkMethodToFind) > -1)
                    sinkFindings.Add(o2Finding);
                else if (o2Finding.SourceContext.IndexOf(sourceMethodToFind) > -1)
                    sourceFindings.Add(o2Finding);
            DI.log.info("There are {0} sinkFindings ( sink ~= {1} )", sinkFindings.Count, sinkMethodToFind);
            DI.log.info("There are {0} sourceFindings ( source ~= {1})", sourceFindings.Count, sourceMethodToFind);

            //ascx_FindingsViewer.openInFloatWindow(results.ToList());
        }

        public static void fixSinkVulnNamesBasedOnSinkContextHashMapKey(string vulnNamePrefix, List<IO2Finding> sinkFindings)
        {
            //foreach(O2Finding o2Finding in sinkFindings)
            //	log.info("{0} -> {1}",  o2Finding.vulnName, o2Finding.SinkContext);
            foreach (O2Finding o2Finding in sinkFindings)
            {
                var hashTagName = extractNameFromContext(o2Finding.SinkContext, "\"", "\"");
                if (hashTagName != "")
                {
                    o2Finding.vulnName = vulnNamePrefix + "_" + hashTagName;
                    //o2Finding.vulnName = o2Finding.vulnName.Replace("addAttribute", "attribute");
                }
            }

            //ascx_FindingsViewer.openInFloatWindow(sinkFindings);
        }

        public static void fixSourceVulnNamesBasedOnSinkContextHashMapKey(string vulnNamePrefix, List<IO2Finding> sourceFindings)
        {
            foreach (O2Finding o2Finding in sourceFindings)
            {
                var hashTagName = extractNameFromContext(o2Finding.SourceContext, "\"", "\"");

                if (hashTagName != "")
                {
                    //log.info(hashTagName);
                    o2Finding.vulnName = vulnNamePrefix + "_" + hashTagName;
                    //o2Finding.vulnName = o2Finding.vulnName.Replace("addAttribute", "attribute");
                }
            }
            //ascx_FindingsViewer.openInFloatWindow(sourceFindings);
        }

        public static string extractNameFromContext(string textToProcess, string leftKeyword, string rigthKeyword)
        {
            var indexOfFirstQuote = textToProcess.IndexOf(leftKeyword);
            if (indexOfFirstQuote > -1)
            {
                var subString = textToProcess.Substring(indexOfFirstQuote + leftKeyword.Length);
                var indexOf2ndQuote = subString.IndexOf(rigthKeyword);
                if (indexOf2ndQuote > -1)
                    return subString.Substring(0, indexOf2ndQuote);
            }

            return "";
        }

        public static List<IO2Finding> joinTracesWhereSinkMatchesSource(List<IO2Finding> sinkFindings, List<IO2Finding> sourceFindings)
        {
            var results = new List<IO2Finding>();
            foreach (var o2SinkFinding in sinkFindings)
                foreach (var o2SourcesFinding in sourceFindings)
                    if (o2SourcesFinding.vulnName.IndexOf(o2SinkFinding.vulnName) > -1)
                    {
                        var o2NewFinding = (O2Finding)OzasmtCopy.createCopy(o2SinkFinding);

                        results.Add(o2NewFinding);
                        var sink = o2NewFinding.getSink();

                        var joinPointTrace = new O2Trace(
                        string.Format("O2 Auto Join Point::: {0}   ->  {1}", o2SinkFinding.vulnName, o2SourcesFinding.vulnName));

                        sink.traceType = TraceType.Type_4;

                        joinPointTrace.traceType = TraceType.Type_4;

                        sink.childTraces.Add(joinPointTrace);

                        var jspSignature = o2SourcesFinding.o2Traces[0].signature;
                        var indexofJsp = jspSignature.IndexOf("._jsp");
                        if (indexofJsp > -1)
                            jspSignature = jspSignature.Substring(0, indexofJsp) + ".jsp";
                        jspSignature = jspSignature.Replace("jsp_servlet", "").Replace("_45_", @"-").Replace(".__", @"/").Replace("._", @"/");


                        var jspTrace = new O2Trace("JSP: " + jspSignature);

                        joinPointTrace.childTraces.Add(jspTrace);
                        jspTrace.childTraces.AddRange(o2SourcesFinding.o2Traces);

                        //var copyOfSourcesFinding = (O2Finding)OzasmtCopy.createCopy(o2SourcesFinding);


                        //copyOfSourcesFinding.o2Traces[0].signature = modifiedSignature;
                        //newTrace.childTraces.AddRange(copyOfSourcesFinding.o2Traces);

                        //newTrace.childTraces.AddRange(o2SourcesFinding.o2Traces);


                        //log.info("we have a match: {0}        ->        {1}", o2SinkFinding.vulnName, o2SourcesFinding.vulnName);
                        //return results;
                    }
            return results;
        }

    }
}
