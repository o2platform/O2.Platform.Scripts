// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.IO;
using O2.Interfaces.XRules;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Interfaces;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
//O2File:IAnalysisArtifacts.cs
//O2File:XUtils_Findings_v0_1.cs

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Analysis_Workflow_Phase_5 : KXRule
    {        	 

        //string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts";
        string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\XPlanner.AnalysisArtifacts";
    	
        public static string  workflowFolder {get;set;}
        public string  folderWithArtifacts_Phase4 {get;set;}
        public string  folderWithArtifacts_Phase5 {get;set;}
        
        public string finalAssessmentFile {get;set;}

    
        public Analysis_Workflow_Phase_5()
        {
            Name = "Analysis_Workflow: Phase 5";
        }
    	
        // PHASE 5 : TASKS
		
                
        public void task1_createFinalAssessmentFile()
        {
            var o2Findings = XUtils_Findings_v0_1.loadMultipleOzasmtFiles(folderWithArtifacts_Phase4);
            O2Cmd.log.write("There are {0} findings for final assessment file", o2Findings.Count);
            
            // make them compatible with OSA
            OzasmtCompatibility.makeCompatibleWithOunceV6(o2Findings);                        
            finalAssessmentFile = Path.Combine(folderWithArtifacts_Phase5, "Final Set of Findings.ozasmt");
            XUtils_Findings_v0_1.saveFindings(o2Findings,finalAssessmentFile);            
            
            O2Cmd.log.write("Final assessment file created: {0}", finalAssessmentFile);   
            
            copyFinalAssessmentFileToWorkflowFolder();
        }
		
		
        public void copyFinalAssessmentFileToWorkflowFolder()
        {
            Files.Copy(finalAssessmentFile,workflowFolder);
        }
	
        
		
       
        // PHASE 4 : XRules    	
        [XRule(Name = "Run Phase 5 (Test)")]
        public string  runPhase5()
        {            
            var testAnalysisArtifacts = KAnalysisArtifacts.load(testAnalysisArtifactsFile);
            var result = runPhase5(testAnalysisArtifacts);
            var finalO2Findings = XUtils_Findings_v0_1.loadFindingsFile(finalAssessmentFile);
            XUtils_Findings_v0_1.openFindingsInNewWindow(finalO2Findings);
            return result;
            
        }
    	
        [XRule(Name = "Run Phase 5")]
        public string runPhase5(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.write("\n\n*****  PHASE 5 ***");
  			
            // setup expected target folders
            workflowFolder = analysisArtifacts.targetFolder;   	
  			
            folderWithArtifacts_Phase4 = Path.Combine(workflowFolder,"Phase 4 - Artifacts");
            folderWithArtifacts_Phase5 = Path.Combine(workflowFolder,"Phase 5 - Artifacts");
            Files.checkIfDirectoryExistsAndCreateIfNot(folderWithArtifacts_Phase5);	// create Phase 2 folder (if required)
  			
            // check if  required folders exist
            Assert.That(Directory.Exists(folderWithArtifacts_Phase4), "folderWithArtifacts_forPhase4 could not be found");
            Assert.That(Directory.Exists(folderWithArtifacts_Phase5), "folderWithArtifacts_forPhase5 could not be found");
  			
            if (analysisArtifacts.phase_5.task1_createFinalAssessmentFile)
                task1_createFinalAssessmentFile();
  		
            O2Cmd.log.write("\n\n*****  PHASE 5 COMPLETED ***");	
            return "Phase 4 completed";
        }
  		
    }
}