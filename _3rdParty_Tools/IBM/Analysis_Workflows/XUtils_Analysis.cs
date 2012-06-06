// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Ref:System.dll
using System;
using System.Collections.Generic;
using System.IO;
//O2Ref:System.Core.dll
using System.Linq;
using System.Text;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.Interfaces.O2Findings;
using O2.Interfaces.XRules;
//O2File:XRule_Findings_Filter.cs
//O2File:XUtils_Struts_v0_1.cs
//O2File:XUtils_Findings_v0_1.cs

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class XUtils_Analysis
    {
        public static string createStrutsMappingsFromFilesIn(string folderWithConfigFiles, string targetFile)
        {
            string webXml = Path.Combine(folderWithConfigFiles, @"web.xml");
            string strutsConfigXml = Path.Combine(folderWithConfigFiles, @"struts-config.xml");
            string tilesDefinitionXml = Path.Combine(folderWithConfigFiles, @"tiles-definition.xml");
            string validationXml = Path.Combine(folderWithConfigFiles, @"validation.xml");

            // Need to change this to be retrieved from the web.xml -> struts-config data
            if (false == File.Exists(tilesDefinitionXml))
                tilesDefinitionXml = Path.Combine(folderWithConfigFiles, @"tiles-definitions.xml");
            // and save it
            var strutsMappingsFile = XUtils_Struts_v0_1.calculateAndSaveStrutsMappings(targetFile, webXml, strutsConfigXml, tilesDefinitionXml, validationXml);
            Assert.That(File.Exists(strutsMappingsFile), "strutsMappingsFile was not created");
            return strutsMappingsFile;
        }

        public static void copyFindings(List<IO2Finding> o2Findings, string sourceFileName, string targetFolder, string targetFolderName, Func<IO2Finding, bool> condition)
        {
            targetFolder = Path.Combine(targetFolder, targetFolderName);
            Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);
            Assert.That(Directory.Exists(targetFolder), "copyFindings: targetFolder did not exist " + targetFolder);
            var findingsWithNoTraces = new List<IO2Finding>();
            foreach (var o2Finding in o2Findings)
                //if (o2Finding.o2Traces.Count == 0)
                if (condition(o2Finding))
                    findingsWithNoTraces.Add(o2Finding);

            O2Cmd.log.write("copyFindings: There were {0} findings that matched the criteria ({1}) ", findingsWithNoTraces.Count, targetFolderName);
            if (findingsWithNoTraces.Count > 0)
            {
                var targetFile = Path.Combine(targetFolder, sourceFileName);
                XUtils_Findings_v0_1.saveFindings(findingsWithNoTraces, targetFile);
                Assert.That(File.Exists(targetFile), "Task 1: targetFile was not saved : " + targetFile);
                O2Cmd.log.write("copyFindings: Findings saved to " + targetFile);
            }
        }

        public static void saveQuery(List<IO2Finding> o2FindingsInFile, string targetFolder, string fileName, string sourceRegEx, string sinkRegex, bool removeFindingsFromSourceList)
        {
            saveQuery(o2FindingsInFile, targetFolder, fileName, sourceRegEx, sinkRegex, "", removeFindingsFromSourceList);
        }
        public static void saveQuery(List<IO2Finding> o2FindingsInFile, string targetFolder, string fileName, string sourceRegEx, string sinkRegex, string fileNamePrefix, bool removeFindingsFromSourceList)
        {
            var fileNamePostfix = string.Format("Source ({0})  Sink ({1})", sourceRegEx, sinkRegex);
            saveQuery(o2FindingsInFile, targetFolder, fileName, sourceRegEx, sinkRegex, fileNamePrefix, fileNamePostfix, removeFindingsFromSourceList);
        }

        public static List<IO2Finding> saveQuery(List<IO2Finding> o2FindingsInFile, string targetFolder, string fileName, string sourceRegEx, string sinkRegex, string fileNamePrefix, string fileNamePostfix, bool removeFindingsFromSourceList)
        {
            O2Cmd.log.write("Executing query: Source = {0}  Sink = {1}", sourceRegEx, sinkRegex);
            var results = new XRule_Findings_Filter().whereSourceAndSink_ContainsRegex(o2FindingsInFile, sourceRegEx, sinkRegex);
            O2Cmd.log.write("   Query returned {0} results", results.Count);
            var targetFile = Path.Combine(targetFolder, fileNamePrefix + fileName + " - " + fileNamePostfix);
            if (false == targetFile.EndsWith(".ozasmt"))
                targetFile += ".ozasmt";
            if (results.Count > 0)
            {
                XUtils_Findings_v0_1.saveFindings(results, targetFile);
                if (removeFindingsFromSourceList)
                    removeFindingsFromList(o2FindingsInFile, results);
            }
            return results;
        }

        public static Dictionary<string, List<IO2Finding>> getDictionaryWithFindingsMappedBy_VulType(List<IO2Finding> o2Findings)
        {
            O2Cmd.log.write("Calculating Dictionary for {0} findings", o2Findings.Count);
            var mappedFindings = new Dictionary<string, List<IO2Finding>>();
            foreach (var o2Finding in o2Findings)
            {
                if (false == mappedFindings.ContainsKey(o2Finding.vulnType))
                    mappedFindings.Add(o2Finding.vulnType, new List<IO2Finding>());
                mappedFindings[o2Finding.vulnType].Add(o2Finding);
            }
            //Assert.That(mappedFindings.Keys.Count >0, "There were no Keys in mappedFindings");
            return mappedFindings;
        }

        public static void saveDictionaryWithMappedFindingsToFolder(Dictionary<string, List<IO2Finding>> mappedFindings, string targetFolder)
        {
            Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);
            Assert.That(Directory.Exists(targetFolder), "Directory targetFolder did not exist: " + targetFolder);
            foreach (var mappedEntry in mappedFindings)
            {
                var findingsToSave = mappedEntry.Value;
                //var fileName = string.Format("{0} ({1} Findings).ozasmt", mappedEntry.Key , findingsToSave.Count); // can't do this if we wanto to easily consume these findings from the next phase
                var fileName = string.Format("{0}.ozasmt", mappedEntry.Key);
                O2Cmd.log.write("Creating file {0} with {0} findings", fileName, findingsToSave.Count);
                var targetFile = Path.Combine(targetFolder, fileName);
                XUtils_Findings_v0_1.saveFindings(findingsToSave, targetFile);
            }
        }

        public static void removeFindingsFromList(List<IO2Finding> o2Findings, List<IO2Finding> o2FindingsToRemove)
        {
            //O2Cmd.log.write("removing {0} findings from list with {1} findings", o2FindingsToRemove.Count, o2Findings.Count);
            foreach (var o2Finding in o2FindingsToRemove)
                o2Findings.Remove(o2Finding);
        }
        
        public static List<IO2Finding> getAllTraces_KnownSinks(string targetFolder)
        {
            var folderWith_KnownSinks = Path.Combine(targetFolder, "FindingsWith_Traces_KnownSinks");
            Assert.That(Directory.Exists(folderWith_KnownSinks), "Directory folderWith_KnownSinks did not exist: " + folderWith_KnownSinks);
            var o2Findings = XUtils_Findings_v0_1.loadMultipleOzasmtFiles(folderWith_KnownSinks);
            O2Cmd.log.write("Findings with Known Sinks: {0}", o2Findings.Count);
            return o2Findings;
        }

        public static List<IO2Finding> getAllTraces_LostSinks(string targetFolder)
        {
            var folderWith_LostSinks = Path.Combine(targetFolder, "FindingsWith_Traces_LostSinks");
            Assert.That(Directory.Exists(folderWith_LostSinks), "directory folderWith_LostSinks did not exist: " + folderWith_LostSinks);
            var o2Findings = XUtils_Findings_v0_1.loadMultipleOzasmtFiles(folderWith_LostSinks);
            O2Cmd.log.write("Findings with Lost Sinks: {0}", o2Findings.Count);
            return o2Findings;
        }

        public static List<IO2Finding> getAllTraces_NoTraces(string targetFolder)
        {
            var noTraces = new List<IO2Finding>();
            var folderWith_NoTraces = Path.Combine(targetFolder, "FindingsWith_NoTraces");
            Assert.That(Directory.Exists(folderWith_NoTraces), "directory folderWith_NoTraces did not exist: " + folderWith_NoTraces);
            var o2Findings = XUtils_Findings_v0_1.loadMultipleOzasmtFiles(folderWith_NoTraces);
            O2Cmd.log.write("Findings with No Traces: {0}", o2Findings.Count);
            return o2Findings;
        }

        public static List<IO2Finding> getFindingsWithVulnType(List<IO2Finding> o2Findings, string vulnType, bool removeFindingsFromSourceList)
        {
            var results = whereVulnType_Is(o2Findings, vulnType);
            // make sure we had result
            //Assert.That(results.Count > 0, "There were no findings of vulnType: " + vulnType);
            if (removeFindingsFromSourceList && results.Count > 0)
                removeFindingsFromList(o2Findings, results);
            return results;
        }


        [XRule(Name = "Only.findings.where.vulnName.IS")]
        public static List<IO2Finding> whereVulnType_Is(List<IO2Finding> o2Findings, string text)
        {
            return
                (from IO2Finding o2Finding in o2Findings
                 where o2Finding.vulnType == text
                 select o2Finding).ToList();
        }      

        public static bool doesFindingHasTraceSignature(IO2Finding o2Finding, string signatureRegEx)
        {
            var allTraces = OzasmtUtils.getListWithAllTraces((O2Finding)o2Finding);
            foreach (var o2Trace in allTraces)
                if (o2Trace.signature.IndexOf(signatureRegEx) > -1)// |
                    //RegEx.findStringInString(o2Trace.signature,signatureRegEx))
                    return true;
            return false;
        }
        
    }
}