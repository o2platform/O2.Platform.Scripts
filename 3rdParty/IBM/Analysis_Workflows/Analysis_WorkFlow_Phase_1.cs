// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
//O2Tag_OnlyAddReferencedAssemblies
using System.IO;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Interfaces.XRules;
using O2.Kernel;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.O2CmdShell;
using O2.XRules.Database.Interfaces;
using NUnit.Framework;

//O2File:IAnalysisArtifacts.cs
//O2Ref:nunit.framework.dll

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Analysis_Workflow_Phase_1 : KXRule
    {          	       
        //string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts";
        string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\XPlanner.AnalysisArtifacts";    	        
    
        //public static IO2Log log = PublicDI.log;
        public bool deletePhase1FolderBeforeCopying = false;
        public bool dontCopyIfTargetFileAlreadyExists = true;
    	
        public string  workflowFolder { get; set;}
        public string folderWithArtifacts_Phase1 { get; set;} 
    	       
        public IO2Log log = PublicDI.log;

        public Analysis_Workflow_Phase_1()
        {
            Name = "Analysis_Workflow: Phase 1";              
        }
    	    	    	    	    	    	    
        // PHASE 1 - TASKS
    	        
        public void task1_copyAssessmentFiles(IAnalysisArtifacts analysisArtifacts)
        {        
            if (deletePhase1FolderBeforeCopying)
                Files.deleteAllFilesFromDir(folderWithArtifacts_Phase1);
                
            var filesCopied = new List<string>();
            foreach(var fileOrFolder in analysisArtifacts.assessmentFilesOrFolderToLoad)
            {        		
                if (File.Exists(fileOrFolder))
                    filesCopied.Add(Files.copyVerbose(fileOrFolder, folderWithArtifacts_Phase1,dontCopyIfTargetFileAlreadyExists));
                else 
                    if (Directory.Exists(fileOrFolder))        		        			
                        foreach(var assessmentFile in Files.getFilesFromDir_returnFullPath(fileOrFolder, "*.ozasmt", true))
                            filesCopied.Add(Files.copyVerbose(assessmentFile, folderWithArtifacts_Phase1,dontCopyIfTargetFileAlreadyExists));
            }

  			            
            //check to see if files were copied  ok
            foreach(var file in  filesCopied)
            {
                var fileName = Path.GetFileName(file);
                var targetFolder = folderWithArtifacts_Phase1;
                var expectedFile = Path.Combine(targetFolder, fileName);
                Assert.That(File.Exists(expectedFile),"Expected file did not exist " + expectedFile);
            }
        }
    	       
        public void task2_copyProjectConfigFiles(IAnalysisArtifacts analysisArtifacts)
        {
            if (analysisArtifacts.projectWebRoots.Count ==0)
                return;
            O2Cmd.log.write("Copying {0} Project Config Files to : {1} ", analysisArtifacts.projectWebRoots.Count, folderWithArtifacts_Phase1);
    		
            //Assert.That(virtualPathsTo_ProjectWebRoot != null, "virtualPathsTo_ProjectWebRoot was null");
  			
            foreach(var projectWebRoot in  analysisArtifacts.projectWebRoots)
            {            	
                // check if we can resolve the project web root
                //var projectWebRoot = Path.Combine(folderWithArtifacts_forPhase1,project.Value);
                Assert.That(Directory.Exists(projectWebRoot), "projectWebRoot did not exist: " + projectWebRoot);
                
                // use the name of the top-level directory of projectWebRoot as the ProjectKey
                var projectKey = Path.GetFileName(projectWebRoot);
                
                // check if we can resolve the WEB-INF web root
                var projectWebInf = Path.Combine(projectWebRoot,"WEB-INF");
                Assert.That(Directory.Exists(projectWebInf), "projectWebInf did not exist: " + projectWebInf);
                // folder to copy config files to
                var targetFolder = Path.Combine(folderWithArtifacts_Phase1, "Config files (-) " + projectKey);
                Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder); 
                Assert.That(Directory.Exists(targetFolder), "targetFolder did not exist: " + targetFolder);		
                // copy config *.xml files
                foreach(var configFile in Files.getFilesFromDir_returnFullPath(projectWebInf,"*.xml", false))
                    Files.copy(configFile, targetFolder);
				
                // make sure target folder has at least 1 file
                Assert.That(Files.getFilesFromDir_returnFullPath(targetFolder).Count > 0, "There wer no config files copied to: " + targetFolder);               
            }
        }
    	
    	
        // PHASE 1 - XRUles
    	
    	
        [XRule(Name = "Run Phase 1 (Test)")]
        public string  runPhase1()
        {            
            var testAnalysisArtifacts = KAnalysisArtifacts.load(testAnalysisArtifactsFile);
            return runPhase1(testAnalysisArtifacts);
        }
    	
        [XRule(Name = "Run Phase 1")]
        public string runPhase1(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.info("\n\n*****  PHASE 1 \n");            
            O2Cmd.log.write("this  phase will copy all artifacts (i.e. scans, config files, etc..)  into a unique location");


            Files.checkIfDirectoryExistsAndCreateIfNot(analysisArtifacts.targetFolder);            
            Assert.That(Directory.Exists(analysisArtifacts.targetFolder), "could not find analysisArtifacts.targetFolder: " + analysisArtifacts.targetFolder);

            workflowFolder = analysisArtifacts.targetFolder;
            folderWithArtifacts_Phase1 = Path.Combine(workflowFolder, "Phase 1 - Artifacts");

            Files.checkIfDirectoryExistsAndCreateIfNot(folderWithArtifacts_Phase1);   
            Assert.That(Directory.Exists(folderWithArtifacts_Phase1), "folderWithArtifacts_forPhase1 could not be found");

            if (analysisArtifacts.phase_1.task1_copyAssessmentFiles)
                task1_copyAssessmentFiles(analysisArtifacts);

            if (analysisArtifacts.phase_1.task2_copyProjectConfigFiles)
                task2_copyProjectConfigFiles(analysisArtifacts);
  				
  			
            O2Cmd.log.write("\n\n**** : PHASE 1 Completed");
  			
            return "Phase 1 completed";
        }
    }
}