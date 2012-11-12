// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.IO;
using System.Collections.Generic;
using O2.Interfaces.O2Findings;
using O2.Interfaces.XRules;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Interfaces;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
using O2.XRules.Database._Rules;
using O2.XRules.Database._Rules.J2EE.Struts;
//O2File:IAnalysisArtifacts.cs
//O2File:KAnalysisArtifacts.cs
//O2File:XUtils_Findings_v0_1.cs
//O2File:XUtils_Analysis.cs
//O2File:xUtils_Struts_v0_1.cs
//O2File:XRule_Struts.cs

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Analysis_Workflow_Phase_3 : KXRule
    {        	     
        public string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts.xml";
        //public string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\....AnalysisArtifacts";    	                
    
        public static string  workflowFolder {get; set;}
        public string  folderWithArtifacts_Phase2;
        public string  folderWithArtifacts_Phase3;

    
        public Analysis_Workflow_Phase_3()
        {
            Name = "Analysis_Workflow: Phase 3";              
        }
    	
        // PHASE 3 : TASKS
		
        // since there are lot a lot of known sinks move them all into one file
        public void task1_handleKnownSinks()
        {
            // for now save these findings in the root of folderWithArtifacts_Phase3
            var folderWithAssessmentFiles = Path.Combine(folderWithArtifacts_Phase2,"FindingsWith_Traces_KnownSinks");
            Assert.That(Directory.Exists(folderWithAssessmentFiles), "Directory folderWithAssessmentFiles does not exist: "  + folderWithAssessmentFiles);
			
            var o2Findings_WithKnownSinks = XUtils_Findings_v0_1.loadMultipleOzasmtFiles(folderWithAssessmentFiles);
			
            // save as 1 ozasmt file with all findings
            var targetFile = Path.Combine(folderWithArtifacts_Phase3,"Findings with Known Sinks.ozasmt");	
            XUtils_Findings_v0_1.saveFindings(o2Findings_WithKnownSinks, targetFile);

            // save as 1 ozasmt file per VulnType
            var targetFolder = Path.Combine(folderWithArtifacts_Phase3, "Findings with KnownSinks (by VulnType)");
            var mappedFindings = XUtils_Analysis.getDictionaryWithFindingsMappedBy_VulType(o2Findings_WithKnownSinks);
            XUtils_Analysis.saveDictionaryWithMappedFindingsToFolder(mappedFindings, targetFolder);
        }
		
        // run sequence of filters on findings (note that (if 4th param == true) saveQuery will remove the matched
        // findings from tracesToFilter
        public void task2_filterFindings(KAnalysisArtifacts analysisArtifacts, List<IO2Finding> tracesToFilter, string targetFolder, string fileName)
        {			                       
            foreach(var sourceSink in analysisArtifacts.phase_3.task2_sourceSink)
            {
                XUtils_Analysis.saveQuery(tracesToFilter, targetFolder, fileName,sourceSink.Source, sourceSink.Sink, sourceSink.RemoveMatches);	
            }            					
            // save what was left (i.e. findings that didn't match the above filters) in a separate file
            if (tracesToFilter.Count > 0)
            {
                O2Cmd.log.write("After task2 filters there were {0} findings that matched no filter", tracesToFilter.Count);
                var targetFile = Path.Combine(targetFolder,"__NO FILTER__" + " - " + fileName + ".ozasmt");
                XUtils_Findings_v0_1.saveFindings(tracesToFilter, targetFile);
            }
        }
		
        public void task3_filter_FindingsWithNoTraces(List<IO2Finding> o2FindingsWithNoTraces)
        {
            // this will create a Dictionary with a List of IO2Finding mapped to its vulnType
            var mappedFindings = XUtils_Analysis.getDictionaryWithFindingsMappedBy_VulType(o2FindingsWithNoTraces);
            // which we will save each vulnType as an separate ozasmt file
            var targetFolder = Path.Combine(folderWithArtifacts_Phase3, "Findings with No Traces (by VulnType)");
            XUtils_Analysis.saveDictionaryWithMappedFindingsToFolder(mappedFindings, targetFolder);
        }
		
		
        public void task4_CalculateStrutsFindings()
        {
            O2Cmd.log.write("TASK #4: Calculate Struts Findings");
            // check if there are Struts Mappings
            var folderWithStrutsMappings = Path.Combine(folderWithArtifacts_Phase2, "Struts Mappings");
            if (Directory.Exists(folderWithStrutsMappings))
            {
                //Assert.That(Directory.Exists(folderWithStrutsMappings), "Directory with struts mappings did not exist: " + folderWithStrutsMappings);
	
                // check if we have the filtered findings files required 
                var findingsWith_KnownSinks= Path.Combine(folderWithArtifacts_Phase3,"Findings with Known Sinks.ozasmt");
                Assert.That(File.Exists(findingsWith_KnownSinks),"Could not find findingsWith_KnownSinks: " + findingsWith_KnownSinks);
                // load findings
                var o2Findings_KnownSinks = XUtils_Findings_v0_1.loadFindingsFile(findingsWith_KnownSinks);
                // extract just the ones needed for the struts mappings
                var o2Findings = new XRule_Findings_Filter().whereSourceAndSink_ContainsRegex(o2Findings_KnownSinks,"getParameter","setAttribute");
                o2Findings.AddRange(new XRule_Findings_Filter().whereSourceAndSink_ContainsRegex(o2Findings_KnownSinks,"getAttribute","print"));			
				
                foreach(var strutsMappingsFile in Files.getFilesFromDir_returnFullPath(folderWithStrutsMappings))
                {
                    var fileName = Path.GetFileName(strutsMappingsFile);
                    var projectName = Path.GetFileNameWithoutExtension(strutsMappingsFile);
                    O2Cmd.log.write("Processing file {0} from project {1}", fileName, projectName);
					
                    // load struts mappings
                    var strutsMapping = XUtils_Struts_v0_1.loadStrutsMappingsFromFile(strutsMappingsFile);
                    Assert.That(strutsMapping!=null, "strutsMapping was null");
				
                    // execute the struts rule
                    var o2Results = XRule_Struts.strutsRule_fromGetParameterToPringViaGetSetAttributeJoins(o2Findings , strutsMapping) ;
	        	
                    // make sure we had results 
                    //Assert.That(o2Results.Count > 0 , "There were no results");
		        	
                    if (o2Results.Count==0)
                        O2Cmd.log.error("there were no results in task4_CalculateStrutsFindings");
                    else
                    {
                        // save results
                        var targetFolder = Path.Combine(folderWithArtifacts_Phase3, "Struts Mappings");
                        Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);
                        var fileWithSavedResults = Path.Combine(targetFolder,projectName + ".ozasmt");
                        XUtils_Findings_v0_1.saveFindings(o2Results,fileWithSavedResults);
			        	
                        // make sure saved file exists
                        Assert.That(File.Exists(fileWithSavedResults), "fileWithSavedResults did not exist: " + fileWithSavedResults);
			        	
                        O2Cmd.log.write("All OK. There were {0} results \r\nsaved to: {1}", o2Results.Count, fileWithSavedResults);
                    }
                }
            }
        }		              
        
        
        // PHASE 3 : XRules    	
        [XRule(Name = "Run Phase 3 (Test)")]
        public string  runPhase3()
        {            
            var testAnalysisArtifacts = KAnalysisArtifacts.load(testAnalysisArtifactsFile);
            return runPhase3(testAnalysisArtifacts);
        }
    	
        [XRule(Name = "Run Phase 3")]
        public string runPhase3(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.write("\n\n*****  PHASE 3 ***");
  			
            // setup expected target folders
            workflowFolder = analysisArtifacts.targetFolder; 
  						
            folderWithArtifacts_Phase2 = Path.Combine(workflowFolder,"Phase 2 - Artifacts");
            folderWithArtifacts_Phase3 = Path.Combine(workflowFolder,"Phase 3 - Artifacts");
            Files.checkIfDirectoryExistsAndCreateIfNot(folderWithArtifacts_Phase3);	// create Phase 2 folder (if required)
  			
            // check if  required folders exist
            Assert.That(Directory.Exists(folderWithArtifacts_Phase2), "folderWithArtifacts_forPhase2 could not be found");
            Assert.That(Directory.Exists(folderWithArtifacts_Phase3), "folderWithArtifacts_forPhase3 could not be found");

            if (analysisArtifacts.phase_3.task1_handleKnownSinks)
                task1_handleKnownSinks();

            if (analysisArtifacts.phase_3.task2_filterFindings)
            {
                var targetFolder = Path.Combine(folderWithArtifacts_Phase3, "Filtered_Findings");
                Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);	
                Files.deleteAllFilesFromDir(targetFolder);
            	
                var allTraces_KnownSinks = XUtils_Analysis.getAllTraces_KnownSinks(folderWithArtifacts_Phase2);
                var allTraces_LostSinks = XUtils_Analysis.getAllTraces_LostSinks(folderWithArtifacts_Phase2);
	  			
                task2_filterFindings((KAnalysisArtifacts)analysisArtifacts, allTraces_KnownSinks, targetFolder, "Known Sinks");
                task2_filterFindings((KAnalysisArtifacts)analysisArtifacts, allTraces_LostSinks, targetFolder, "Lost Sinks");
            }

            if (analysisArtifacts.phase_3.task3_filter_FindingsWithNoTraces)
            {
                var allTraces_NoTraces = XUtils_Analysis.getAllTraces_NoTraces(folderWithArtifacts_Phase2);
                task3_filter_FindingsWithNoTraces(allTraces_NoTraces);
            }

            if (analysisArtifacts.phase_3.task4_CalculateStrutsFindings)
                task4_CalculateStrutsFindings();
  				
            O2Cmd.log.write("\n\n*****  PHASE 3 COMPLETED ***");	
            return "Phase 3 completed";
        }
  		
    }
}