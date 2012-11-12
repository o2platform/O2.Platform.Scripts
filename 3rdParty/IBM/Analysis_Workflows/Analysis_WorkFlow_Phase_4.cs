// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.IO;
using System.Collections.Generic;
using System.Linq;
using O2.Interfaces.O2Findings;
using O2.Interfaces.XRules;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database._Rules;
using O2.XRules.Database.Interfaces;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
//O2File:IAnalysisArtifacts.cs
//O2File:XUtils_Findings_v0_1.cs
//O2File:XUtils_Analysis.cs


namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Analysis_Workflow_Phase_4 : KXRule
    {        	 
        //string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts";
        string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\....AnalysisArtifacts";
    	
        public bool deleteAllFiles = true;           
    	
        public static string  workflowFolder { get; set; }
        public string  folderWithArtifacts_Phase3;
        public string  folderWithArtifacts_Phase4;

    
        public Analysis_Workflow_Phase_4()
        {
            Name = "Analysis_Workflow: Phase 4";
        }
    	
        // PHASE 4 : TASKS
		
        // Handle the Findings With Knonw Sinks
        public void task1_analyseFindingsWithKnownSinks()
        {
            // check if source findings file is there 
            var fileWith_Findings_WithKnownSinks = Path.Combine(folderWithArtifacts_Phase3, "Findings with Known Sinks.ozasmt");
            Assert.That(File.Exists(fileWith_Findings_WithKnownSinks), "fileWith_Findings_WithKnownSinks file did not exist: " + fileWith_Findings_WithKnownSinks);
			
            // load findings
            var o2Findings_WithKnownSinks = XUtils_Findings_v0_1.loadFindingsFile(fileWith_Findings_WithKnownSinks);
            Assert.That(o2Findings_WithKnownSinks.Count > 0 , "There were no findings in o2Findings_WithKnownSinks object");		



            // show findingds (while in analysis mode)
            //XUtils_Findings_v0_1.openFindingsInNewWindow(o2Findings_WithKnownSinks);
			
            analyzeFindingsOfVulnType_SqlInjection(o2Findings_WithKnownSinks,true); 
            makeKnownFindingsTypeII(o2Findings_WithKnownSinks);
        }
		
		        
        public void analyzeFindingsOfVulnType_SqlInjection(List<IO2Finding> o2Findings, bool removeFindingsFromSourceList)
        {
            // extract the Sql Injection ones
            var sqlInjectionFindings = XUtils_Analysis.getFindingsWithVulnType(o2Findings, "Vulnerability.Injection.SQL", removeFindingsFromSourceList);
		    	
            if (sqlInjectionFindings.Count == 0)
                return;
            // var fileWithSqlInjections = Path.Combine(folderWithArtifacts_Phase4, "Findings_with_SQL_Injection");
            //XUtils_Findings_v0_1.saveFindings(sqlInjectionFindings, fileWithSqlInjections);
		   
            //Assert.That(File.Exists(fileWithSqlInjections), "fileWithSqlInjections was not created");
            var sqlInjectionValidators = new List<string> {
                                                              "java.lang.Integer.<init>(int):void",
                                                              "java.lang.Integer.valueOf(int):java.lang.Integer",
                                                              "java.lang.String.valueOf(int):java.lang.String",
                                                              ":java.util.DateTime"};
            var nonExploitable = new List<IO2Finding>();
            var maybeExploitable = new List<IO2Finding>();
		    
            foreach(O2Finding o2Finding in sqlInjectionFindings)
            {
                var validatorFound = "";
                foreach(var validator in sqlInjectionValidators)
                    if (XUtils_Analysis.doesFindingHasTraceSignature(o2Finding, validator))
                    {
                        validatorFound = validator;
                        break;
                    }
                // modify finding
                if (validatorFound != "")
                {
                    o2Finding.context = string.Format("found validator: {0}   ,   {1}", validatorFound, o2Finding.context);
                    nonExploitable.Add(o2Finding);
                    o2Finding.vulnType+=".NotExploitable";
                    o2Finding.severity = 3;
                    o2Finding.confidence = 1;
                }
                else
                {
                    maybeExploitable.Add(o2Finding);
                    o2Finding.vulnType+=".MaybeExploitable.InternalMethod";
                    o2Finding.severity = 0;
                    o2Finding.confidence = 2;
                }
            }
		    
            var fileWith_NonExploitable  = Path.Combine(folderWithArtifacts_Phase4, "NonExploitable_Findings_with_SQL_Injection.ozasmt");
            XUtils_Findings_v0_1.saveFindings(nonExploitable,fileWith_NonExploitable);
	 		
            var fileWith_MaybeExploitable  = Path.Combine(folderWithArtifacts_Phase4, "MaybeExploitable_Findings_with_SQL_Injection.ozasmt");
	 	
            XUtils_Findings_v0_1.saveFindings(maybeExploitable,fileWith_MaybeExploitable);

            //XUtils_Findings_v0_1.openFindingsInNewWindow(nonExploitable).Join();
            //XUtils_Findings_v0_1.openFindingsInNewWindow(maybeExploitable).Join();
        }
		
        // note this should be the last one to run
        public void makeKnownFindingsTypeII(List<IO2Finding> o2Findings)
        {
            foreach(var o2Finding in o2Findings)
                o2Finding.confidence = 2;
            var saveAsTypeII  = Path.Combine(folderWithArtifacts_Phase4, "all_Non_Processed_KnownSink_Findings_as_Type_II.ozasmt");
	 	
            XUtils_Findings_v0_1.saveFindings(o2Findings,saveAsTypeII);
        }
		
        public void task2_AdjustsStrutsFindings()
        {
            var validatorPatternIDText = "validator: patternid=";
            var validatorValidContentText = "validator: valid-content";
	
            O2Cmd.log.write("TASK 2: AdjustsStrutsFindings");	
            var strutsFindingsFolder = Path.Combine(folderWithArtifacts_Phase3,"Struts Mappings");
			
            if (false == Directory.Exists(strutsFindingsFolder))
                return;
            //Assert.That(Directory.Exists(strutsFindingsFolder), "strutsFindingsFolder did not exists: " + strutsFindingsFolder);
			
            foreach(var strutsFindingFile in Files.getFilesFromDir_returnFullPath(strutsFindingsFolder))
            {
                var o2Findings = XUtils_Findings_v0_1.loadFindingsFile(strutsFindingFile);
                foreach(O2Finding o2Finding in o2Findings)
                {
                    var allTraces = OzasmtUtils.getListWithAllTraces(o2Finding);
                    foreach(var o2Trace in allTraces)
                    {
                        if (o2Trace.signature.StartsWith(validatorPatternIDText))
                        {
                            var pattern = o2Trace.signature.Replace(validatorPatternIDText,"");
                            if(pattern == "FREE_TEXT")
                            {
                                o2Finding.vulnType = "Struts.CrossSiteScripting.NOT.Validated";
                                o2Finding.confidence = 1;
                                o2Finding.severity = 0;
                            }
                            else
                            {
                                o2Finding.vulnType = "Struts.CrossSiteScripting.Validated";
                                o2Finding.confidence = 1;
                                o2Finding.severity = 2;
                            }
                            o2Finding.vulnType += " : " + pattern;
                            break;
                        }				
                        else if (o2Trace.signature.StartsWith(validatorValidContentText))
                        {
                            var pattern = o2Trace.signature.Replace(validatorValidContentText,"");
                            o2Finding.vulnType = "Struts.CrossSiteScripting.Validated.ValidContent";
                            o2Finding.confidence = 2;
                            o2Finding.severity = 2;
                        }
                    }
//					validator: patternid=
					
					
                }
                //XUtils_Findings_v0_1.openFindingsInNewWindow(o2Findings);
                var targetFile = Path.Combine(folderWithArtifacts_Phase4,"Struts Mappings - " + Path.GetFileName(strutsFindingFile));
                XUtils_Findings_v0_1.saveFindings(o2Findings,targetFile);
                O2Cmd.log.write("Struts Mappings saved to: {0}", targetFile);
            }
			
			
            //foreach(var
		
        }
        		
		
        // PHASE 4 : XRules    	
        [XRule(Name = "Run Phase 4 (Test)")]
        public string  runPhase4()
        {            
            var testAnalysisArtifacts = KAnalysisArtifacts.load(testAnalysisArtifactsFile);
            return runPhase4(testAnalysisArtifacts);
        }
    	
        [XRule(Name = "Run Phase 4")]
        public string runPhase4(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.write("\n\n*****  PHASE 4 ***");
  			
            // setup expected target folders
            workflowFolder = analysisArtifacts.targetFolder;   		
  			
            folderWithArtifacts_Phase3 = Path.Combine(workflowFolder,"Phase 3 - Artifacts");
            folderWithArtifacts_Phase4 = Path.Combine(workflowFolder,"Phase 4 - Artifacts");
            Files.checkIfDirectoryExistsAndCreateIfNot(folderWithArtifacts_Phase4);	// create Phase 2 folder (if required)
  			
            // check if  required folders exist
            Assert.That(Directory.Exists(folderWithArtifacts_Phase3), "folderWithArtifacts_forPhase3 could not be found");
            Assert.That(Directory.Exists(folderWithArtifacts_Phase4), "folderWithArtifacts_forPhase4 could not be found");
  			
            // delete all files from folderWithArtifacts_Phase4 dir
            if (deleteAllFiles)
                Files.deleteAllFilesFromDir(folderWithArtifacts_Phase4);

            if (analysisArtifacts.phase_4.task1_analyseFindingsWithKnownSinks)
                task1_analyseFindingsWithKnownSinks();

            if (analysisArtifacts.phase_4.task2_AdjustsStrutsFindings)
                task2_AdjustsStrutsFindings();

            O2Cmd.log.write("\n\n*****  PHASE 4 COMPLETED ***");	
            return "Phase 4 completed";
        }
  		
    }
}