// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using O2.Interfaces.O2Findings;
using O2.Interfaces.XRules;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Interfaces;
using NUnit.Framework;

//O2Ref:nunit.framework.dll
//O2File:IAnalysisArtifacts.cs
//O2File:xUtils_Findings_v0_1.cs
//O2File:XUtils_Analysis.cs
//O2File:xUtils_Struts_v0_1.cs




namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Analysis_Workflow_Phase_2 : KXRule
    {    
        //string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts";
        string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\....AnalysisArtifacts";
    	
        public bool onlyRunTasksOnOneFile = false;
	                    	
        public string  workflowFolder { get; set;}
        public string  folderWithArtifacts_Phase1 { get; set;}
        public string  folderWithArtifacts_Phase2 { get; set;}
    
        public Analysis_Workflow_Phase_2()
        {
            Name = "Analysis_Workflow: Phase 2";              
        }
    	    	
    	    	
        //  PHASE 2 - TASKS
		
        public void task1_FilterFindings(List<IO2Finding> o2FindingsInFile, string fileName)
        {
            O2Cmd.log.write("TASK #1: Filter Findings");
            XUtils_Analysis.copyFindings(o2FindingsInFile, fileName, folderWithArtifacts_Phase2, "FindingsWith_NoTraces", findingWith_NoTraces);
            //copyFindings(o2FindingsInFile, fileName, "FindingsWith_Traces", findingWith_Traces);
            XUtils_Analysis.copyFindings(o2FindingsInFile, fileName, folderWithArtifacts_Phase2, "FindingsWith_Traces_KnownSinks", findingWith_Traces_KnownSinks);
            XUtils_Analysis.copyFindings(o2FindingsInFile, fileName,folderWithArtifacts_Phase2,  "FindingsWith_Traces_LostSinks", findingWith_Traces_LostSinks); 			  
        }
    	
        public static bool findingWith_NoTraces(IO2Finding o2Finding)
        {
            return o2Finding.o2Traces.Count == 0;
        }
    	
        public static bool findingWith_Traces(IO2Finding o2Finding)
        {
            return o2Finding.o2Traces.Count > 0;
        }
    	
        public static bool findingWith_Traces_KnownSinks(IO2Finding o2Finding)
        {
            return ((O2Finding)o2Finding).KnownSink != "";
        }
    	
        public static bool findingWith_Traces_LostSinks(IO2Finding o2Finding)
        {
            return ((O2Finding)o2Finding).LostSink != "";
        }
    
        public void task2_createStrutsMappings()
        {
            O2Cmd.log.write("TASK #2: Create Struts Mappings");
            foreach(var folder in Files.getListOfAllDirectoriesFromDirectory(folderWithArtifacts_Phase1,false))
            {
                var splittedName = Path.GetFileName(folder).Split(new [] {" (-) "},StringSplitOptions.None);
                if (splittedName.Length==2)
                {
                    var folderType = splittedName[0].Trim();
                    var value = splittedName[1].Trim();
                    O2Cmd.log.write("folderType: " + folderType);
                    switch(folderType)
                    {
                        case "Config files":
                            var targetFolder = Path.Combine(folderWithArtifacts_Phase2, "Struts Mappings");
                            Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);
                            Assert.That(Directory.Exists(targetFolder));
                            var targetFile = Path.Combine(targetFolder, value + ".O2StrutsMappings");
                            XUtils_Analysis.createStrutsMappingsFromFilesIn(folder, targetFile);
                            break;
                    }
                }
            }
        }    	    	
    	
    	
        // helper method (will be moved to XUtils_Findings
    	    
 
        // PHASE 2 - XRules
        [XRule(Name = "Run Phase 2 (Test)")]
        public string  runPhase2()
        {            
            var testAnalysisArtifacts = KAnalysisArtifacts.load(testAnalysisArtifactsFile);
            return runPhase2(testAnalysisArtifacts);
        }
    	
        [XRule(Name = "Run Phase 2")]
        public string runPhase2(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.write("\n\n*****  PHASE 2 ***");
  			
            // setup expected target folders
            workflowFolder = analysisArtifacts.targetFolder; 
  			
            folderWithArtifacts_Phase1 = Path.Combine(workflowFolder,"Phase 1 - Artifacts");
            folderWithArtifacts_Phase2 = Path.Combine(workflowFolder,"Phase 2 - Artifacts");
            Files.checkIfDirectoryExistsAndCreateIfNot(folderWithArtifacts_Phase2);	// create Phase 2 folder (if required)
  			
            // check if  required folders exist
            Assert.That(Directory.Exists(folderWithArtifacts_Phase1), "folderWithArtifacts_forPhase1 could not be found");
            Assert.That(Directory.Exists(folderWithArtifacts_Phase2), "folderWithArtifacts_forPhase2 could not be found");

            if (analysisArtifacts.phase_2.task1_SplitFindingsOnTrace)  			
                foreach(var file in Files.getFilesFromDir_returnFullPath(folderWithArtifacts_Phase1))
                {
                    var fileName = Path.GetFileName(file);
                    // load findings
                    var o2FindingsInFile = XUtils_Findings_v0_1.loadFindingsFile(file);
                    // check if file was loaded ok
                    Assert.That(o2FindingsInFile != null ,"o2FindingsInFile was null. file loaded :"+ file );
                    // Assert.That(o2FindingsInFile.Count > 0, "There were no findings loaded from file: " + file);
                    O2Cmd.log.write("Loaded {0} findings from file {1}",o2FindingsInFile.Count  , fileName);
                    task1_FilterFindings(o2FindingsInFile, fileName);  		
                    if (onlyRunTasksOnOneFile)
                        break;  // during development just use first file
                }


            if (analysisArtifacts.phase_2.task2_createStrutsMappings)   
                task2_createStrutsMappings();
   			    
   			    
            O2Cmd.log.write("\n\n**** : PHASE 2 Completed\n\n");
  			
            return "Phase 1 completed";
        }
    	 
    	 
    	
    }
}