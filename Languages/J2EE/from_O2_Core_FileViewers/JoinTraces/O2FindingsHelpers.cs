// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.Interfaces.O2Findings;

namespace O2.Core.FileViewers.JoinTraces
{
    public class O2FindingsHelpers
    {        

        public static DataTable getDataTableWithFindingsDetails(List<IO2Finding> findingsToFilter)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Value");

            if (findingsToFilter!= null)
            {
                var withTraces = 0;
                var noTraces = 0;
                var lostSinks = 0;
                var knownSinks = 0;
                foreach (O2Finding o2Finding in findingsToFilter)
                {
                    if (o2Finding.o2Traces.Count == 0)
                        noTraces++;
                    else
                    {
                        withTraces++;
                        if (o2Finding.LostSink != "")
                            lostSinks++;
                        else if (o2Finding.KnownSink != "")
                            knownSinks++;
                    }

                }
                dataTable.Rows.Add("# Findings Loaded", findingsToFilter.Count);
                dataTable.Rows.Add("# Findings with Traces ", withTraces);
                dataTable.Rows.Add("# Findings with NO Traces ", noTraces);
                dataTable.Rows.Add("# Sinks ", lostSinks);
                dataTable.Rows.Add("# Sources ", knownSinks);
            }
            return dataTable;
        }

        public static List<IO2Finding> calculateFindings(List<IO2Finding> findingsToFilter, string sourceSignatures, string sinkSignatures)
        {
            return calculateFindings(findingsToFilter, sourceSignatures, sinkSignatures, null);
        }

        public static List<IO2Finding> calculateFindings(List<IO2Finding> findingsToFilter, string sourceSignatures, string sinkSignatures, Func<List<IO2Finding>, List<IO2Finding>> mapJointPointsCallback)
        {
            // add suport for one regex per line
            sourceSignatures = RegEx.convertLinesIntoRegExes(sourceSignatures);
            sinkSignatures = RegEx.convertLinesIntoRegExes(sinkSignatures);

            var results = new List<IO2Finding>();
            foreach (O2Finding o2Finding in findingsToFilter)
                if (RegEx.findStringInString(o2Finding.Source, sourceSignatures) && RegEx.findStringInString(o2Finding.Sink, sinkSignatures))
                    results.Add(o2Finding);
            if (mapJointPointsCallback == null)
                return results;

            return mapJointPointsCallback(results);
        }
    }
}
