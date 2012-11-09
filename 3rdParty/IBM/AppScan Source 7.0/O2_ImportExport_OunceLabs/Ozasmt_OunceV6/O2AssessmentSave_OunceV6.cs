// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.DotNetWrappers.DotNet;
using O2.Interfaces.O2Findings;

namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6
{
    public class O2AssessmentSave_OunceV6 : IO2AssessmentSave
    {        
        public AssessmentRun assessmentRun {get;set;}                

        public O2AssessmentSave_OunceV6()
        {
            engineName = "O2AssessmentSave_OunceV6";
            assessmentRun = OzasmtUtils_OunceV6.getDefaultAssessmentRunObject();
        }

        public string engineName {get; set;}
        

        public string save(List<IO2Finding> o2Findings)
        {
            string tempOzasmtFile = DI.config.getTempFileInTempDirectory("ozasmt");

            return (save(o2Findings,tempOzasmtFile)) ? tempOzasmtFile : "";
        }

        public bool save(List<IO2Finding> o2Findings, string sPathToSaveAssessment)
        {
            return save(assessmentRun.name, o2Findings, sPathToSaveAssessment);
        }

        public bool save(string assessmentName, IEnumerable<IO2Finding> o2Findings, string sPathToSaveAssessment)
        {
            createAssessmentRunObject(assessmentName, o2Findings);
            return OzasmtUtils_OunceV6.SaveAssessmentRun(assessmentRun, sPathToSaveAssessment);
        }

        private void createAssessmentRunObject(string assessmentName, IEnumerable<IO2Finding> o2Findings)
        {
            assessmentRun.name = assessmentName ?? "";
            assessmentRun.Assessment.owner_name = assessmentRun.name;
            assessmentRun.Assessment.assessee_name = assessmentRun.name;
            assessmentRun.Assessment.owner_type = "Application";
            addO2FindingsToAssessmentRunObject(o2Findings);
        }

        private AssessmentRun createAssessmentRunObject(IO2Assessment o2Assessment)
        {
            createAssessmentRunObject(o2Assessment.name, o2Assessment.o2Findings);
            return assessmentRun;
        }

        public void addO2FindingsToAssessmentRunObject(IEnumerable<IO2Finding> o2Findings)
        {
            Dictionary<string, List<AssessmentAssessmentFileFinding>> filesMappedToO2Findings =
                getFilesToO2FindingMappings(o2Findings);
            var assessmentFiles = new List<AssessmentAssessmentFile>();
            foreach (string file in filesMappedToO2Findings.Keys)
            {
                var assessmentFile = new AssessmentAssessmentFile
                                         {
                                             filename = file,
                                             Finding = filesMappedToO2Findings[file].ToArray()
                                         };
                assessmentFiles.Add(assessmentFile);
            }
            assessmentRun.Assessment.Assessment[0].AssessmentFile = assessmentFiles.ToArray();
        }

        public Dictionary<String, List<AssessmentAssessmentFileFinding>> getFilesToO2FindingMappings(IEnumerable<IO2Finding> o2Findings)
        {
            var filesMappedToO2Findings = new Dictionary<string, List<AssessmentAssessmentFileFinding>>();
            // create var to hold string and file Indexes and populate it with the current string indexes
            var dStringIndexes = new Dictionary<string, uint>();
            var dFilesIndexes = new Dictionary<string, uint>();
            foreach (AssessmentRunStringIndex stringIndex in assessmentRun.StringIndeces)
                dStringIndexes.Add(stringIndex.value, stringIndex.id);
            foreach (AssessmentRunFileIndex siFileIndexes in assessmentRun.FileIndeces)
                dFilesIndexes.Add(siFileIndexes.value, siFileIndexes.id);

            foreach (IO2Finding o2Finding in o2Findings)
            {
                if (o2Finding.file == null)
                    o2Finding.file = "[Findings with NO file mappped]";

                if (false == filesMappedToO2Findings.ContainsKey(o2Finding.file))
                    filesMappedToO2Findings.Add(o2Finding.file, new List<AssessmentAssessmentFileFinding>());

                filesMappedToO2Findings[o2Finding.file].Add(OzasmtUtils_OunceV6.getAssessmentAssessmentFileFinding(
                                                                o2Finding, dStringIndexes, dFilesIndexes));
            }
            // finaly update the main string and file indexes
            assessmentRun.StringIndeces = OzasmtUtils_OunceV6.createStringIndexArrayFromDictionary(dStringIndexes);
            assessmentRun.FileIndeces = OzasmtUtils_OunceV6.createFileIndexArrayFromDictionary(dFilesIndexes);
            return filesMappedToO2Findings;
        }

        /// <summary>
        /// This function loads up the ozasmtSource file and adds its stats to a new finding called savedCreatedOzasmtAs 
        /// which will have the fingdings in o2AssessmentTarget
        /// </summary>
        /// <param name="ozasmtSource"></param>
        /// <param name="o2AssessmentTarget"></param>
        /// <param name="savedCreatedOzasmtAs"></param>
        public bool addAssessmentStatsFromSourceToO2AssessmentAndSaveIt(string ozasmtSource, IO2Assessment o2AssessmentTarget, string savedCreatedOzasmtAs)
        {
            AssessmentRun assessmentRunToImport = OzasmtUtils_OunceV6.LoadAssessmentRun(ozasmtSource);            
            var targetAssessmentRun = createAssessmentRunObject(o2AssessmentTarget);
            // map assessmentRunToImport to targetAssessmentRun

            // add targetAssessmentRun top level data
            targetAssessmentRun.AssessmentStats = assessmentRunToImport.AssessmentStats;
            targetAssessmentRun.AssessmentConfig = assessmentRunToImport.AssessmentConfig;
            targetAssessmentRun.Messages = assessmentRunToImport.Messages;
            // add Assessment data            
            targetAssessmentRun.Assessment.assessee_name = assessmentRunToImport.Assessment.assessee_name;
            targetAssessmentRun.Assessment.AssessmentStats = assessmentRunToImport.AssessmentStats;
            targetAssessmentRun.Assessment.owner_name = assessmentRunToImport.Assessment.owner_name;
            targetAssessmentRun.Assessment.owner_type = assessmentRunToImport.Assessment.owner_type;

            // add project and file data   

            //create backup of current findings 
            var currentAssessmentDataBackup = targetAssessmentRun.Assessment.Assessment[0];   // there should only be one
            // assign current Assessment array to assessmentRunToImport.Assessment.Assessment
            targetAssessmentRun.Assessment.Assessment = assessmentRunToImport.Assessment.Assessment;
            // remove all findings references (since what we want is the stats            
            foreach (var assessment in targetAssessmentRun.Assessment.Assessment)
                if (assessment.AssessmentFile != null)
                    foreach (var assessmentFile in assessment.AssessmentFile)
                        assessmentFile.Finding = null;
            // apppend the currentAssessmentDataBackup to the current Assessment Array
            var assessments = new List<Assessment>(targetAssessmentRun.Assessment.Assessment);
            assessments.Add(currentAssessmentDataBackup);
            targetAssessmentRun.Assessment.Assessment = assessments.ToArray();
            
            //targetAssessmentRun.name = "AAAA";
            // save it 
            return OzasmtUtils_OunceV6.SaveAssessmentRun(assessmentRun, savedCreatedOzasmtAs);            
        }
    }
}
