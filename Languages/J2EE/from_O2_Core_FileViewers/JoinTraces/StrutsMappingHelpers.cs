// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.Interfaces.FrameworkSupport.J2EE;
using O2.Interfaces.O2Findings;

namespace O2.Core.FileViewers.JoinTraces
{
    public class StrutsMappingHelpers
    {
        public static List<IO2Finding> createFindingsFromStrutsMappings(IStrutsMappings iStrutsMappings)
        {
            var newO2Findings = new List<IO2Finding>();


            foreach (var actionServlet in iStrutsMappings.actionServlets)
                foreach (var controler in actionServlet.controllers.Values)
                {
                    var o2Finding = new O2Finding
                    {
                        vulnType = "Struts.Finding : " + controler.type,
                        vulnName = controler.type ?? ""
                    };


                    var o2RootTrace = (O2Trace)o2Finding.addTrace("Struts Mapping", TraceType.O2JoinSource);
                    o2RootTrace.addTrace("Controller Type: " + controler.type, TraceType.O2JoinSink);

                    // add formbean
                    if (controler.formBean != null)
                    {
                        var beanTrace = (O2Trace)o2RootTrace.addTrace("Form Bean : " + controler.formBean.name, TraceType.O2Info);
                        beanTrace.addTrace("has validation mapping" + controler.formBean.hasValidationMapping);
                        foreach (var field in controler.formBean.fields)
                            beanTrace.addTrace(field.Value.name);
                    }


                    var pathsTrace = (O2Trace)o2RootTrace.addTrace("paths:", TraceType.O2Info);

                    foreach (var path in controler.paths)
                    {
                        var pathTrace = (O2Trace)pathsTrace.addTrace("url: " + path.path);
                        pathTrace.addTrace("controller: " + controler.type + " <- ");
                        pathTrace.addTraces("view: ", TraceType.O2JoinSink, path.resolvedViews.ToArray());
                    }

                    //o2Finding.o2Traces.Add(o2RootTrace);
                    newO2Findings.Add(o2Finding);
                }
            return newO2Findings;
        }

        public static List<IO2Finding> joinTracesUsingConsolidatedView(Dictionary<string, List<O2Finding>> joinSinksDictionary, Dictionary<string, List<IO2Finding>> rootFunctions)
        {
            var mappedFindings = new List<IO2Finding>();
            foreach (var joinSinkSignature in joinSinksDictionary.Keys)
            {
                const string controlTypePrefixString = "Controller Type: ";
                const string viewPrefixString = "view: ";

                var className = joinSinkSignature.Replace(controlTypePrefixString, "").Replace(viewPrefixString, "");
                if (rootFunctions.ContainsKey(className))
                {
                    foreach (var o2FindingWithJoinSink in joinSinksDictionary[joinSinkSignature])
                    {

                        //    bool addedController = false;
                        //    bool addedView = false;

                        //var modifiedFinding = (O2Finding)OzasmtCopy.createCopy(o2FindingWithJoinSink);
                        var joinSinkToAppendTraces = o2FindingWithJoinSink.getJoinSink(joinSinkSignature);
                        if (joinSinkToAppendTraces != null)
                        {
                            // add the traces from rootFunctions[className] to the joinSinkToAppendTraces
                            foreach (O2Finding o2FindingInRootFunctions in rootFunctions[className])
                            {
                                if (o2FindingInRootFunctions.Sink.IndexOf("setAttribute") > -1 ||
                                    o2FindingInRootFunctions.Source.IndexOf("getAttribute") > -1)
                                {
                                    joinSinkToAppendTraces.childTraces.AddRange(o2FindingInRootFunctions.o2Traces);
                                }
                                //var joinPoint = new O2Trace(o2FindingInRootFunctions.vulnName);
                                //joinPoint.childTraces.AddRange(o2FindingInRootFunctions.o2Traces);
                                //joinSinkToAppendTraces.childTraces.Add(joinPoint);


                            }

                            if (false == mappedFindings.Contains(o2FindingWithJoinSink))
                                mappedFindings.Add(o2FindingWithJoinSink);
                            else
                            {
                            }
                        }
                        else
                        {
                            DI.log.error("Something is wrong since we could not find the Join sink trace: {0}",
                                         joinSinkSignature);
                        }
                    }
                    //      if (addedController && addedView)


                    //mappedFindings.Add(rootFunctions[className][0]);
                }
            }
            return mappedFindings;
        }

        public static IStrutsMappings loadO2StrutsMappingFile(string pathToO2StrutsMapping)
        {
            return (IStrutsMappings)Serialize.getDeSerializedObjectFromBinaryFile(pathToO2StrutsMapping, typeof(KStrutsMappings));
        }
    }
}
