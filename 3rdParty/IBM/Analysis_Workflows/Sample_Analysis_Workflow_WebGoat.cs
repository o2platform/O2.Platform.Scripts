// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Interfaces.XRules;
using O2.Kernel;
using O2.XRules.Database.Interfaces;
//O2Ref:nunit.framework.dll
using NUnit.Framework;
//O2File:IAnalysisArtifacts.cs
//O2File:Analysis_Workflow.cs
//O2File:XUtils_AnalysisWorkflow.cs

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class Sample_Analysis_Workflow_WebGoat : KXRule
    {
        private static IO2Log log = PublicDI.log;

        public Sample_Analysis_Workflow_WebGoat()
        {
            log.alsoShowInConsole = true;
            Name = "Sample_Analysis_Workflow_WebGoat";
        }
		
        static string demoDataFolder = @"E:\O2\Demodata\_AnalysisWorkflow\";
        static string webGoatAnalysisArtifactsFile = demoDataFolder + "WebGoat.AnalysisArtifacts.xml";
        static string webGoatAssessmentFile = @"F:\_AppsToScan\webgoat\OSA (Out of the box) - Java and jsp Files  10-20-09 115PM.ozasmt";
        
        [XRule(Name = "WebGoat_AllPhases")]
        public string webgoat_AllPhases()
        {			
            createWebgoatArtifactsFile();
            setWebGoatPhaseSettings_example1();
            return startAnalysis(webGoatAnalysisArtifactsFile);
        }
        
        [XRule(Name = "WebGoat_Just_Phase3_Task2")]
        public string webgoat_Just_Phase3_Task2()
        {			
            createWebgoatArtifactsFile();
            setWebGoatPhaseSettings_example2();
            return startAnalysis(webGoatAnalysisArtifactsFile);
        }
        
        
        [XRule(Name = "Start Analysis")]
        public string startAnalysis(string artifactsFile)
        {
        	
            var analysisArtifacts = KAnalysisArtifacts.load(artifactsFile);        	
        	        	
            var analysisWorkflow = new Analysis_Workflow();
            return analysisWorkflow.start(analysisArtifacts);        	                                
        }
		
        
        [Test]
        public string createWebgoatArtifactsFile()
        {
            File.Delete(webGoatAnalysisArtifactsFile);
            Assert.That(false == File.Exists(webGoatAnalysisArtifactsFile), "webGoatAnalysisArtifactsFile should not exists at this stage: " + webGoatAnalysisArtifactsFile);
            string workflowName = "webgoat (from O2 Unit test)";
            string assessmentFile= webGoatAssessmentFile;
            string targetFolder = Path.Combine(demoDataFolder, workflowName);
            string targetAnalysisArtifactsFile  = webGoatAnalysisArtifactsFile;        	
            // create it
            var analysisArtifacts = (KAnalysisArtifacts)XUtils_AnalysisWorkflow.createAnalysisArtifact(workflowName ,assessmentFile ,targetFolder);        	
            // save it        	
            KAnalysisArtifacts.save(analysisArtifacts, targetAnalysisArtifactsFile);
            // make sure it exists
            Assert.That(File.Exists(webGoatAnalysisArtifactsFile), "webGoatAnalysisArtifactsFile was not created: " + webGoatAnalysisArtifactsFile);
            return webGoatAnalysisArtifactsFile;
        }

        [Test]
        public string setWebGoatPhaseSettings_example1()
        {
            var analysisArtifacts = KAnalysisArtifacts.load(webGoatAnalysisArtifactsFile);        				
            analysisArtifacts.phase_1.run = true;
            analysisArtifacts.phase_1.task1_copyAssessmentFiles = true;
            analysisArtifacts.phase_1.task2_copyProjectConfigFiles = true;
            analysisArtifacts.phase_2.run = true;
            analysisArtifacts.phase_2.task1_SplitFindingsOnTrace = true;
            analysisArtifacts.phase_2.task2_createStrutsMappings = true;			
            analysisArtifacts.phase_3.run = true;
            analysisArtifacts.phase_3.task1_handleKnownSinks = true;
            analysisArtifacts.phase_3.task2_filterFindings = true;
            analysisArtifacts.phase_3.task3_filter_FindingsWithNoTraces = true;				
            analysisArtifacts.phase_3.task4_CalculateStrutsFindings = true;			
            analysisArtifacts.phase_4.run = true;
            analysisArtifacts.phase_4.task1_analyseFindingsWithKnownSinks = true;
            analysisArtifacts.phase_4.task2_AdjustsStrutsFindings = true;			
            analysisArtifacts.phase_5.run = true;
            analysisArtifacts.phase_5.task1_createFinalAssessmentFile = true;		

            // save the results in the end
            KAnalysisArtifacts.save((KAnalysisArtifacts)analysisArtifacts,webGoatAnalysisArtifactsFile);	
            return webGoatAnalysisArtifactsFile;
        }
		
        [Test]
        public string setWebGoatPhaseSettings_example2()
        {
            var analysisArtifacts = KAnalysisArtifacts.load(webGoatAnalysisArtifactsFile);  		 // loads AnalysisArtifact xml file      				
            XUtils_AnalysisWorkflow.setAllPhasesAndTasksValue(analysisArtifacts,false);				 // disables all phases and tasks 
            analysisArtifacts.phase_3.run	= true;													 // enable phase #3
            analysisArtifacts.phase_3.task2_filterFindings = true;									 // enable phase #3's tasks #2
            analysisArtifacts.phase_3.task2_sourceSink.Clear();										 // remove previous entries
			
            // note: the SourceSink object should be created with 3 parameters: 
            //             - Source
            //             - Sink
            //             - RemoveMatches   : when set will remove the findings that matched the Source/Sink pair from the next queries
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("getParameter","",false)); // add new mappings
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","org.apache",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("getAttribute","",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","setAttribute",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","setProperty",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","sql",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","print",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","io",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","Cookie",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","exec",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","log",true));
            //analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("get","set",true));
            analysisArtifacts.phase_3.task2_sourceSink.Add(new SourceSink("","external_caller",true));
		
            KAnalysisArtifacts.save((KAnalysisArtifacts)analysisArtifacts,webGoatAnalysisArtifactsFile);	
            return webGoatAnalysisArtifactsFile;
        }        	      
    }
}