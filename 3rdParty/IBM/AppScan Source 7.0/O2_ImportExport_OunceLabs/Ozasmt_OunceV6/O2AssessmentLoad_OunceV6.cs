// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.Interfaces.O2Findings;

namespace O2.ImportExport.OunceLabs.Ozasmt_OunceV6
{
    public class O2AssessmentLoad_OunceV6 : IO2AssessmentLoad
    {
        public string engineName { get; set; }        

        public O2AssessmentLoad_OunceV6()
        {
            engineName = "O2AssessmentLoad_OunceV6";                  
        }

        public bool canLoadFile(string fileToTryToLoad)
        {
            var expectedRootElementText = "<AssessmentRun";
            var notExpectedRootElementText = "version=\"6.1.0\"";
            string rootElementText = XmlHelpers.getRootElementText(fileToTryToLoad);
            if (rootElementText.IndexOf(expectedRootElementText) > -1 && rootElementText.IndexOf(notExpectedRootElementText)== -1)
                return true;
            DI.log.error("in {0} engine, could not load file {1} since the root element value didnt match: {2}!={3}",
                         engineName, fileToTryToLoad, rootElementText ,expectedRootElementText);
            return false;
        }

        public O2AssessmentLoad_OunceV6(String fileToLoad, IO2Assessment o2Assessment)
            : this()
        {
            importFile(fileToLoad, o2Assessment);            
        }
         
        
        
        public IO2Assessment loadFile(string fileToLoad)
        {
            var o2Assessment = new O2Assessment();
            if (importFile(fileToLoad,o2Assessment))
                return o2Assessment;
            return null;
        }

        public bool importFile(string fileToLoad, IO2Assessment o2Assessment)
        {
            try
            {
                if (canLoadFile(fileToLoad))
                {
                    //o2Assessment.lastOzasmtImportWasItSucessfull = false;
                    //o2Assessment.lastOzasmtImportFile = fileToLoad;
                    //o2Assessment.lastOzasmtImportFileSize = Files.getFileSize(fileToLoad);

                    //DateTime startImportTime = DateTime.Now;
                    var timer = new O2Timer("Loaded assessment " + fileToLoad + " ").start();
                    AssessmentRun assessmentRunToImport = OzasmtUtils_OunceV6.LoadAssessmentRun(fileToLoad);
                    timer.stop();
                    /*     assessmentRun.AssessmentConfig = assessmentRunToImport.AssessmentConfig;
                 assessmentRun.AssessmentStats = assessmentRunToImport.AssessmentStats;
                 assessmentRun.Messages = assessmentRunToImport.Messages;
                 assessmentRun.name = assessmentRunToImport.name ?? OzasmtUtils_OunceV6.calculateAssessmentNameFromScans(assessmentRunToImport);*/

                    o2Assessment.name = assessmentRunToImport.name ??
                                        OzasmtUtils_OunceV6.calculateAssessmentNameFromScans(assessmentRunToImport);

                    // I don't think I need this since the O2Finding objects have the full strings
                    // map top level objects
                    /*
                 assessmentRun.FileIndeces = assessmentRunToImport.FileIndeces;                
                 assessmentRun.StringIndeces = assessmentRunToImport.StringIndeces;*/

                    // import findings
                    if (null != assessmentRunToImport.Assessment.Assessment)
                        foreach (Assessment assessment in assessmentRunToImport.Assessment.Assessment)
                            if (null != assessment.AssessmentFile)
                                foreach (AssessmentAssessmentFile assessmentFile in assessment.AssessmentFile)
                                    if (assessmentFile.Finding != null)
                                        foreach (AssessmentAssessmentFileFinding finding in assessmentFile.Finding)
                                            o2Assessment.o2Findings.Add(OzasmtUtils_OunceV6.getO2Finding(finding,
                                                                                                         assessmentFile,
                                                                                                         assessmentRunToImport));

                    // if we made it this far all went ok;
                    //o2Assessment.lastOzasmtImportTimeSpan = DateTime.Now - startImportTime;
                    //o2Assessment.lastOzasmtImportWasItSucessfull = true;
                    return true;
                }
            }
            catch
                (Exception ex)
            {
                DI.log.error("in importAssessmentRun: {0}", ex.Message);
            }
            return false;

        }


    }
}
