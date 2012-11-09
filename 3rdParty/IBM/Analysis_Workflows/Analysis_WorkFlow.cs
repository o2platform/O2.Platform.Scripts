// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.IO;
using O2.Interfaces.O2Core;
using O2.Interfaces.XRules;
using O2.Kernel;
using O2.DotNetWrappers.O2CmdShell;
using O2.DotNetWrappers.Windows; 
//O2File:IAnalysisArtifacts.cs
using O2.XRules.Database.Interfaces;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
//O2File:Analysis_WorkFlow_Phase_1.cs
//O2File:Analysis_WorkFlow_Phase_2.cs
//O2File:Analysis_WorkFlow_Phase_3.cs
//O2File:Analysis_WorkFlow_Phase_4.cs
//O2File:Analysis_WorkFlow_Phase_5.cs
//O2Ref:O2_Core_FileViewers.dll

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    [TestFixture]
    public class Analysis_Workflow : KXRule
    {    
        string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\WebGoat.AnalysisArtifacts";
        //string testAnalysisArtifactsFile = @"E:\O2\Demodata\_AnalysisWorkflow\XPlanner.AnalysisArtifacts";
    	
        private static IO2Log log = PublicDI.log;

        public Analysis_Workflow()
        {
            Name = "Analysis_Workflow";
        }

        [XRule(Name = "Start from GUI")]
        public string start()
        {
            return start(testAnalysisArtifactsFile);
        }
        [XRule(Name = "Start with artifacts file")]
        public string start(string analysisArtifactsFile)
        {
            var analysisArtifacts = KAnalysisArtifacts.load(analysisArtifactsFile);
            start(analysisArtifacts);
            return "execution completed";
        }

        [XRule(Name = "Start with assessments")]
        public string start(string folderWithAssessments, string targetFolder)
        {
            return start(folderWithAssessments, "", targetFolder);
        }

        [XRule(Name = "Start with assessments and projects")]
        public string start(string folderWithAssessments, string folderWithProjectFiles, string targetFolder)
        {
            var projectName = Path.GetFileName(folderWithAssessments);  // get the project name from the name of the folderWithAssessments
            var analysisArtifacts = new KAnalysisArtifacts(projectName);

            analysisArtifacts.assessmentFilesOrFolderToLoad.Add(folderWithAssessments);
            if (false == string.IsNullOrEmpty(folderWithProjectFiles))
                analysisArtifacts.projectFilesOrFolder.Add(folderWithProjectFiles);
            analysisArtifacts.targetFolder = targetFolder;            
            return start(analysisArtifacts);            
        }
     

        [XRule(Name = "Start")]
        public string start(IAnalysisArtifacts analysisArtifacts)
        {
            O2Cmd.log.write("\n\n*********   O2 Analysis Workflow **********\n\n");

            O2Cmd.log.write(analysisArtifacts.getAnalysisDetails());

            if (analysisArtifacts.runAllPhases || analysisArtifacts.phase_1.run)
                new Analysis_Workflow_Phase_1().runPhase1(analysisArtifacts);

            if (analysisArtifacts.runAllPhases || analysisArtifacts.phase_2.run)
                new Analysis_Workflow_Phase_2().runPhase2(analysisArtifacts);

            if (analysisArtifacts.runAllPhases || analysisArtifacts.phase_3.run)
                new Analysis_Workflow_Phase_3().runPhase3(analysisArtifacts);

            if (analysisArtifacts.runAllPhases || analysisArtifacts.phase_4.run)
                new Analysis_Workflow_Phase_4().runPhase4(analysisArtifacts);

            if (analysisArtifacts.runAllPhases || analysisArtifacts.phase_5.run)
                new Analysis_Workflow_Phase_5().runPhase5(analysisArtifacts);
                
            return "execution completed";
        }  
        
        [Test]
        [XRule(Name = "test Manual Phases")]
        public string testManualPhases()
        {        	
            return manual_phases(testAnalysisArtifactsFile,"1234");        	
        }
        
        [XRule(Name = "Manual_Phases")]
        public string manual_phases(string analysisArtifactsFile, string phase)
        {
            O2Cmd.log.write("\n\n*********   O2 Analysis Workflow : Manual Phase execution **********\n\n");
            O2Cmd.log.write("\n: analysisArtifactsFile = {0}", analysisArtifactsFile);
            O2Cmd.log.write("\n: phase = {0}", phase);

            var analysisArtifacts = KAnalysisArtifacts.load(analysisArtifactsFile);

            O2Cmd.log.write(analysisArtifacts.getAnalysisDetails());

            if (phase.IndexOf("1") > -1)
                new Analysis_Workflow_Phase_1().runPhase1(analysisArtifacts);
                
            if (phase.IndexOf("2") > -1)
                new Analysis_Workflow_Phase_2().runPhase2(analysisArtifacts);
                
            if (phase.IndexOf("3") > -1)
                new Analysis_Workflow_Phase_3().runPhase3(analysisArtifacts);
                
            if (phase.IndexOf("4") > -1)
                new Analysis_Workflow_Phase_4().runPhase4(analysisArtifacts);
                
            if (phase.IndexOf("5") > -1)
                new Analysis_Workflow_Phase_5().runPhase5(analysisArtifacts);            
                        
            return "manual phase  execution completed";
        }
  		      	  		         
    }
}