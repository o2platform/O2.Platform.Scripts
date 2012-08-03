// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using O2.Interfaces.FrameworkSupport.J2EE;
using O2.Interfaces.O2Findings;
using O2.Interfaces.Views;
using O2.Kernel;
using O2.Core.FileViewers.Ascx;
using O2.Core.FileViewers.Ascx.O2Rules;
using O2.Core.FileViewers.JoinTraces;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.External.WinFormsUI.Forms;
using O2.Views.ASCX.O2Findings;
//O2Ref:O2_External_WinFormsUI.dll
//O2Ref:O2_Core_FileViewers.dll
//O2File:XUtils_Findings_v0_1.cs


namespace O2.XRules.Database._Rules
{
    public class XUtils_Struts_Joins_V0_1_Helpers
    {

        public static List<IO2Finding> executeRule_AndReturn_ResultFindings(
            string baseO2Findings, string strutsMappingsFile, string taintSources_SourceRegEx, string taintSources_SinkRegEx,
            string finalSinks_SourceRegEx, string finalSinks_SinkRegEx, Func<string, string> joinPointFilter)
        {
            var xRuleStuts = executeRule_AndReturn_XRuleStrutsObject(
                baseO2Findings, strutsMappingsFile, taintSources_SourceRegEx, taintSources_SinkRegEx,
                finalSinks_SourceRegEx, finalSinks_SinkRegEx, joinPointFilter);
            return xRuleStuts.getResults();
        }

        public static XUtils_Struts_Joins_V0_1 executeRule_AndReturn_XRuleStrutsObject(
            string baseO2Findings, string strutsMappingsFile, string taintSources_SourceRegEx, string taintSources_SinkRegEx,
            string finalSinks_SourceRegEx, string finalSinks_SinkRegEx, Func<string, string> joinPointFilter)
        {
            var xRuleStuts = new XUtils_Struts_Joins_V0_1()
            {
                BaseO2Findings = baseO2Findings,
                StrutsMappingsFile = strutsMappingsFile,
                TaintSources_SourceRegEx = taintSources_SourceRegEx,
                TaintSources_SinkRegEx = taintSources_SinkRegEx,
                FinalSinks_SourceRegEx = finalSinks_SourceRegEx,
                FinalSinks_SinkRegEx = finalSinks_SinkRegEx,
                JoinPointFilter = joinPointFilter
            };

            xRuleStuts.calculateFindings();
            return xRuleStuts;
        }

        public static XUtils_Struts_Joins_V0_1 executeRule_AndReturn_XRuleStrutsObject(
            List<IO2Finding> baseO2Findings, IStrutsMappings strutsMappings, string taintSources_SourceRegEx, string taintSources_SinkRegEx,
            string finalSinks_SourceRegEx, string finalSinks_SinkRegEx, Func<string, string> joinPointFilter)
        {
            var xRuleStuts = new XUtils_Struts_Joins_V0_1()
            {
                findingsWith_BaseO2Findings = baseO2Findings,
                StrutsMappings = strutsMappings,
                TaintSources_SourceRegEx = taintSources_SourceRegEx,
                TaintSources_SinkRegEx = taintSources_SinkRegEx,
                FinalSinks_SourceRegEx = finalSinks_SourceRegEx,
                FinalSinks_SinkRegEx = finalSinks_SinkRegEx,
                JoinPointFilter = joinPointFilter
            };

            xRuleStuts.calculateFindings();
            return xRuleStuts;
        }
    }

    public class XUtils_Struts_Joins_V0_1
    {    
        public string BaseO2Findings { get; set; }
        public string StrutsMappingsFile { get; set; }
        public string TaintSources_SourceRegEx { get; set; }
        public string TaintSources_SinkRegEx { get; set; }
        public string FinalSinks_SourceRegEx { get; set; }
        public string FinalSinks_SinkRegEx { get; set; }
        public Func<string, string> JoinPointFilter { get; set; }

        public List<IO2Finding> findingsWith_BaseO2Findings { get; set; }
        public List<IO2Finding> findingsWith_StrutsMappings { get; set; }
        public List<IO2Finding> findingsWith_Results { get; set; }
        public List<IO2Finding> findingsWith_FindingsFromTaintSources { get; set; }
        public List<IO2Finding> findingsWith_FindingsToFinalSinks { get; set; }

        public IStrutsMappings StrutsMappings { get; set; }                

        public void calculateFindings()
        {
            // set variables
            // open GUI


            //strutsRuleGUI.loadBaseO2Findings(BaseO2Findings).Join();
            //strutsRuleGUI.loadO2MappingsFile(StrutsMappingsFile);

            // execute rule
            calculateFinalResults(TaintSources_SourceRegEx, TaintSources_SinkRegEx, FinalSinks_SourceRegEx,
                FinalSinks_SinkRegEx);
        }

        public List<IO2Finding> getResults()
        {
            return findingsWith_Results;
        }

        public void showFinalResultsIn_fidingsViewer()
        {
            ascx_FindingsViewer.openInFloatWindow(getResults(), "O2 Rule - Struts");
        }

        public void showFinalResultsIn_O2RulesStrutsGUI()
        {
            var strutsRuleGUI =
                (ascx_O2Rules_Struts)
                O2AscxGUI.openAscx(typeof(ascx_O2Rules_Struts), O2DockState.Float, "Struts Mapping File");
            showFinalResults(strutsRuleGUI.findingsViewer_BaseFindings,
                             strutsRuleGUI.strutsMappingsControl,
                             strutsRuleGUI.findingsViewer_FromStrutsMappings,
                             strutsRuleGUI.filteredFindings_TaintSources,
                             strutsRuleGUI.filteredFindings_FinalSinks,
                             strutsRuleGUI.findingsViewer_FinalFindings);
        }

        public void showFinalResults(ascx_FindingsViewer findingsViewer_BaseFindings,
            ascx_StrutsMappings strutsMappingsControl,
            ascx_FindingsViewer findingsViewer_FromStrutsMappings,
            ascx_FilteredFindings filteredFindings_TaintSources,
            ascx_FilteredFindings filteredFindings_FinalSinks,
            ascx_FindingsViewer findingsViewer_FinalFindings)
        {
            // basefindings and strutsmappings
            findingsViewer_BaseFindings.loadO2Findings(findingsWith_BaseO2Findings);
            strutsMappingsControl.showStrutsMappings(StrutsMappings);
            findingsViewer_FromStrutsMappings.loadO2Findings(findingsWith_StrutsMappings);

            //filteredFindings_TaintSources
            filteredFindings_TaintSources.setSourceSignatureRegEx(TaintSources_SourceRegEx);
            filteredFindings_TaintSources.setSinkSignatureRegEx(TaintSources_SinkRegEx);
            filteredFindings_TaintSources.setFindingsToFilter(findingsWith_BaseO2Findings);
            filteredFindings_TaintSources.setMapJointPointsCallback(XUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sinks);
            filteredFindings_TaintSources.setFindingsViewerFilters("_JoinSink", "");
            filteredFindings_TaintSources.setFindingsResult(findingsWith_FindingsFromTaintSources);



            //filteredFindings_FinalSinks
            filteredFindings_FinalSinks.setSourceSignatureRegEx(FinalSinks_SourceRegEx);
            filteredFindings_FinalSinks.setSinkSignatureRegEx(FinalSinks_SinkRegEx);
            filteredFindings_FinalSinks.setFindingsToFilter(findingsWith_BaseO2Findings);
            filteredFindings_FinalSinks.setMapJointPointsCallback(XUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sources);
            filteredFindings_FinalSinks.setFindingsViewerFilters("_JoinSource", "");
            filteredFindings_FinalSinks.setFindingsResult(findingsWith_FindingsToFinalSinks);


            // results (i.e. final findings)
            //findingsViewer_FinalFindings.loadO2Findings(findingsWith_Results);
            findingsViewer_FinalFindings.loadO2Findings(findingsWith_Results, true);
        }

        public void calculateFinalResults(
            string taintSources_SourceRegEx, string taintSources_SinkRegEx, string finalSinks_SourceRegEx, string finalSinks_SinkRegEx)
        {
            if (findingsWith_BaseO2Findings==null)
                findingsWith_BaseO2Findings = XUtils_Findings_v0_1.loadFindingsFile(BaseO2Findings);

            // calculate TaintSources           
            findingsWith_FindingsFromTaintSources = O2FindingsHelpers.calculateFindings(
                                                            findingsWith_BaseO2Findings,
                                                            taintSources_SourceRegEx,
                                                            taintSources_SinkRegEx,
                                                            XUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sinks);

            // calculate FinalSinks
            findingsWith_FindingsToFinalSinks = O2FindingsHelpers.calculateFindings(
                                                            findingsWith_BaseO2Findings,
                                                            FinalSinks_SourceRegEx,
                                                            finalSinks_SinkRegEx,
                                                            XUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sources);

            // calculate strutsMapping object and findings
            if (StrutsMappings == null)
                StrutsMappings = (IStrutsMappings)Serialize.getDeSerializedObjectFromBinaryFile(StrutsMappingsFile, typeof(KStrutsMappings));
            findingsWith_StrutsMappings = StrutsMappingHelpers.createFindingsFromStrutsMappings(StrutsMappings);

            calculateResults();

            //            results = xUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sinks(results);


            /*runFilterFor_TaintSources(
                taintSources_SourceRegEx, taintSources_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings,
                xUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sinks, filteredFindings_TaintSources);
            runFilterFor_FinalSinks(
                finalSinks_SourceRegEx, finalSinks_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings,
                xUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sources, filteredFindings_FinalSinks);


/*            runFilterFor_TaintSources(
                taintSources_SourceRegEx, taintSources_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings,
                xUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sinks, filteredFindings_TaintSources);
            runFilterFor_FinalSinks(
                finalSinks_SourceRegEx, finalSinks_SinkRegEx, findingsViewer_BaseFindings.currentO2Findings,
                xUtils_Findings_v0_1.mapJoinPoints_HashTagsOn_Sources, filteredFindings_FinalSinks);
            */

            /*
            createFindingsFromStrutsMappings(strutsMappingsControl, findingsViewer_FromStrutsMappings);

            calculateResults(strutsMappingsControl,filteredFindings_TaintSources, filteredFindings_FinalSinks, findingsViewer_FinalFindings);
             */
        }

        /*public void runFilterFor_TaintSources(
            string sourcesRegEx, string sinksRegEx, List<IO2Finding> findingsToFilter,
            Func<List<IO2Finding>, List<IO2Finding>> mapJointPointsCallback, ascx_FilteredFindings filteredFindings_TaintSources)
        {
            filteredFindings_TaintSources.setFindingsViewerFilters("_JoinSink", "");
            filteredFindings_TaintSources.calculateFindings(sourcesRegEx, sinksRegEx, findingsToFilter, mapJointPointsCallback);
        }*/
        /*
        public void runFilterFor_FinalSinks(
            string sourcesRegEx, string sinksRegEx, List<IO2Finding> findingsToFilter,
            Func<List<IO2Finding>, List<IO2Finding>> mapJointPointsCallback, ascx_FilteredFindings filteredFindings_FinalSinks)
        {
            filteredFindings_FinalSinks.setFindingsViewerFilters("_JoinSource", "");
            filteredFindings_FinalSinks.calculateFindings(sourcesRegEx, sinksRegEx, findingsToFilter, mapJointPointsCallback);
        }*/

        public static void runFilterOn_SourceFindings(List<IO2Finding> sourceFindings, string sourceSignatures, ascx_FindingsViewer findingsViewer_ToLoadResults)
        {
            var results = new List<IO2Finding>();
            foreach (O2Finding o2Finding in sourceFindings)
                if (RegEx.findStringInString(o2Finding.Source, sourceSignatures))
                    results.Add(o2Finding);

            findingsViewer_ToLoadResults.setFilter1Value("Source");
            findingsViewer_ToLoadResults.setFilter2Value("Sink");
            findingsViewer_ToLoadResults.loadO2Findings(results, true);
        }

        public static void runFilterOn_FinalSinksFindings(List<IO2Finding> sourceFindings, string sinkSignatures, ascx_FindingsViewer findingsViewer_ToLoadResults)
        {
            var results = new List<IO2Finding>();
            foreach (O2Finding o2Finding in sourceFindings)
                if (RegEx.findStringInString(o2Finding.Sink, sinkSignatures))
                    results.Add(o2Finding);

            findingsViewer_ToLoadResults.setFilter1Value("Sink");
            findingsViewer_ToLoadResults.setFilter2Value("Source");
            findingsViewer_ToLoadResults.loadO2Findings(results, true);
        }

        public static void createFindingsFromStrutsMappings(ascx_StrutsMappings strutsMappingsControl, ascx_FindingsViewer findingsViewer_FromStrutsMappings)
        {
            createFindingsFromStrutsMappings(strutsMappingsControl.getStrutsMappingObject(), findingsViewer_FromStrutsMappings);
        }

        public static void createFindingsFromStrutsMappings(IStrutsMappings strutsMappings, ascx_FindingsViewer findingsViewer_ToLoadResults)
        {
            var createdFindings = StrutsMappingHelpers.createFindingsFromStrutsMappings(strutsMappings);
            findingsViewer_ToLoadResults.setTraceTreeViewVisibleStatus(true);
            findingsViewer_ToLoadResults.setFilter2Value("(no filter)");
            findingsViewer_ToLoadResults.loadO2Findings(createdFindings, true);
        }

        public void calculateResults()
        /*ascx_StrutsMappings strutsMappingsControl,
        ascx_FilteredFindings filteredFindings_TaintSources,
        ascx_FilteredFindings filteredFindings_FinalSinks,
        ascx_FindingsViewer findingsViewer_FinalFindings
        )*/
        {
            //const string controlTypePrefixString = "Controller Type: ";
            //const string viewPrefixString = "view: ";

            var results = new List<IO2Finding>();

            // need to build 4 dictionaries
            PublicDI.log.debug("building 4 dictionaries with sources,sinks and Join points");
            // JoinSink in taintSources
            var taintSourcesJoinSinks = new Dictionary<string, List<IO2Finding>>();
            //foreach (O2Finding o2Finding in filteredFindings_TaintSources.getResults())
            foreach (O2Finding o2Finding in findingsWith_FindingsFromTaintSources)
            {
                var joinSinks = o2Finding.JoinSinks();
                if (joinSinks.Count == 1)
                {
                    var joinSink = joinSinks[0]; // we currenty only support the case where there is one JoinSink
                    if (false == taintSourcesJoinSinks.ContainsKey(joinSink))
                        taintSourcesJoinSinks.Add(joinSink, new List<IO2Finding>());
                    taintSourcesJoinSinks[joinSink].Add(o2Finding);
                }
            }

            // var taintSourcesJoinLocations
            var taintSourcesJoinLocations = new Dictionary<string, List<IO2Finding>>();
            //foreach (O2Finding o2Finding in filteredFindings_TaintSources.getResults())
            foreach (O2Finding o2Finding in findingsWith_FindingsFromTaintSources)
            {
                var joinLocations = o2Finding.getJoinLocations();
                if (joinLocations.Count == 1)
                {
                    var joinLocation = joinLocations[0].signature; // we currenty only support the case where there is one JoinLocation
                    if (false == taintSourcesJoinLocations.ContainsKey(joinLocation))
                        taintSourcesJoinLocations.Add(joinLocation, new List<IO2Finding>());
                    taintSourcesJoinLocations[joinLocation].Add(o2Finding);
                }
            }

            // JoinSources in final Sinks
            var finalSinksTaintSources = new Dictionary<string, List<IO2Finding>>();
            //foreach (O2Finding o2Finding in filteredFindings_FinalSinks.getResults())
            foreach (O2Finding o2Finding in findingsWith_FindingsToFinalSinks)
            {
                var joinSources = o2Finding.JoinSources();
                if (joinSources.Count == 1)
                {
                    var joinSource = joinSources[0]; // we currenty only support the case where there is one JoinSource
                    if (false == finalSinksTaintSources.ContainsKey(joinSource))
                        finalSinksTaintSources.Add(joinSource, new List<IO2Finding>());
                    finalSinksTaintSources[joinSource].Add(o2Finding);
                }
            }

            // var finalSinksJoinLocations
            var finalSinksJoinLocations = new Dictionary<string, List<IO2Finding>>();
            //foreach (O2Finding o2Finding in filteredFindings_FinalSinks.getResults())
            foreach (O2Finding o2Finding in findingsWith_FindingsToFinalSinks)
            {
                var joinLocations = o2Finding.getJoinLocations();
                if (joinLocations.Count == 1)
                {
                    var joinLocation = joinLocations[0].signature; // we currenty only support the case where there is one JoinLocation
                    if (false == finalSinksJoinLocations.ContainsKey(joinLocation))
                        finalSinksJoinLocations.Add(joinLocation, new List<IO2Finding>());
                    finalSinksJoinLocations[joinLocation].Add(o2Finding);
                }
            }

            PublicDI.log.debug("mapping all data");
            //foreach (var actionServlet in strutsMappingsControl.getStrutsMappingObject().actionServlets)            
            foreach (var actionServlet in StrutsMappings.actionServlets)
            {
                var controllersToAdd = actionServlet.controllers.Values.Count;
                var controllersAdded = 0;
                foreach (var controler in actionServlet.controllers.Values)
                {
                    if (controllersAdded++ % 10 == 0)
                        PublicDI.log.debug("   Added [{0}/{1}] controllers ({2} findings so far)", controllersAdded, controllersToAdd , results.Count );
                    foreach (var path in controler.paths)
                        foreach (var view in path.resolvedViews)
                        {
                            //DI.log.info("{0} - {1}", controler.type, view);
                            // now search on the Join Locations
                            foreach (var taintSourcesJoinLocation in taintSourcesJoinLocations)
                                foreach (var finalSinksJoinLocation in finalSinksJoinLocations)
                                {
                                    var filteredController = controler.type ?? "";
                                    var filteredView = JoinPointFilter(view); //.Replace("\\", ".").Replace('/', '.');
                                    var filteredTaintSource = JoinPointFilter(taintSourcesJoinLocation.Key);
                                        //.Replace('\\', '.').Replace('/', '.');
                                    var filteredFinalSink = JoinPointFilter(finalSinksJoinLocation.Key);
                                        //.Replace('\\', '.').Replace('/', '.'); ;

                                    if (filteredTaintSource.Contains(filteredController) &&
                                        filteredFinalSink.Contains(filteredView))
                                    {
                                        foreach (O2Finding taintSourceFinding in taintSourcesJoinLocation.Value)
                                            foreach (O2Finding finalSinkFinding in finalSinksJoinLocation.Value)
                                            {
                                                if (taintSourceFinding.JoinSinks().Count == 1 &&
                                                    finalSinkFinding.JoinSources().Count == 1)
                                                {
                                                    if (taintSourceFinding.JoinSinks()[0] ==
                                                        finalSinkFinding.JoinSources()[0])
                                                    {

                                                        // if we have a match , create the finding
                                                        var o2Finding = new O2Finding
                                                                            {
                                                                                vulnType =
                                                                                    "Struts.Finding : " + controler.type,
                                                                                vulnName = controler.type ?? ""
                                                                            };


                                                        var o2RootTrace =
                                                            (O2Trace)
                                                            o2Finding.addTrace("Struts Mapping", TraceType.Root_Call);
                                                        var controllerTrace =
                                                            o2RootTrace.addTrace("Controller: " + controler.type,
                                                                                 TraceType.O2JoinSink);                                                        
                                                        if (controler.formBean != null)
                                                        {
                                                            var beanTrace =
                                                                (O2Trace)
                                                                o2RootTrace.addTrace(
                                                                    "Form Bean : " + controler.formBean.name,
                                                                    TraceType.O2Info);
                                                            beanTrace.addTrace("has validation mapping" +
                                                                               controler.formBean.hasValidationMapping);

                                                            // only add the field that matches the current join
                                                            var currentJoinPoint = JoinOnAttributes.extractNameFromContext(taintSourceFinding.SourceContext, "\"", "\"");                                                                
                                                            foreach (var field in controler.formBean.fields)
                                                                if (field.Key == currentJoinPoint)
                                                                {

                                                                    var joinPoint =
                                                                        (O2Trace) beanTrace.addTrace(field.Value.name);
                                                                    joinPoint.addTrace("hasValidationMapping: " +
                                                                                       field.Value.hasValidationMapping);
                                                                    joinPoint.addTrace_IfNotEmpty("depends: ",
                                                                                                  field.Value.depends);
                                                                    joinPoint.addTrace_IfNotEmpty("initial",
                                                                                                  field.Value.initial);
                                                                    joinPoint.addTrace_IfNotEmpty("type",
                                                                                                  field.Value.type);
                                                                    foreach (var validator in field.Value.validators)
                                                                        joinPoint.addTrace(
                                                                            string.Format("validator: {0}={1}",
                                                                                          validator.Key, validator.Value));
                                                                }

                                                            //var formBeanTrace = o2RootTrace.addTrace( "Form Bean: " + controler.formBean.type ?? "",TraceType.O2Info);
                                                        }
                                                        controllerTrace.childTraces.AddRange(taintSourceFinding.o2Traces);

                                                        var pathsTrace =
                                                            (O2Trace) o2RootTrace.addTrace("paths:", TraceType.O2Info);
                                                        var pathTrace =
                                                            (O2Trace) pathsTrace.addTrace("url: " + path.path);
                                                        pathTrace.addTrace("controller: " + controler.type + " <- ");
                                                        var viewTrace = pathTrace.addTrace("view: " + filteredView,
                                                                                           TraceType.O2JoinSink);

                                                        viewTrace.childTraces.AddRange(finalSinkFinding.o2Traces);
                                                        results.Add(o2Finding);
                                                    }
                                                }

                                            }


                                        /* o2Finding.addTraces(new[]
                                                                {
                                                                    filteredController, filteredView, filteredTaintSource,
                                                                    filteredFinalSink
                                                                });*/

                                    }
                                    //DI.log.info(" {0} = {1}", filteredController, filteredTaintSource);
                                    //DI.log.info("   {0} = {1}", filteredView, filteredFinalSink);
                                }
                        }
                }
            }
            findingsWith_Results = results;
            PublicDI.log.debug("mapping complete");
            //findingsViewer_FinalFindings.loadO2Findings(results, true);

        }
    }
}
