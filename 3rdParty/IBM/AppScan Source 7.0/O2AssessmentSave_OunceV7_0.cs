// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6_1;
using O2.XRules.Database.Findings;
//O2File:xsd_Ozasmt_OunceV7_0.cs
//O2File:Findings_ExtensionMethods.cs 

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.ThirdPary.IBM
{
	public class O2AssessmentSave_OunceV7_test
	{
		public static void test()
		{
			var testFile = @"E:\_Work\IBM\8.6_files\8.6_files\AltoroJ_2.5 _Callbacks.ozasmt";
	
			var findings = "cachedFindings".o2Cache(()=>testFile.loadO2Findings());
			var assessmentSave = new O2AssessmentSave_OunceV7();
			    
			var savedFile = assessmentSave.save(findings);
		}

	}
    public class O2AssessmentSave_OunceV7 : IO2AssessmentSave
    {        
        public AssessmentRun assessmentRun {get;set;}                

        public O2AssessmentSave_OunceV7()
        {
            engineName = "O2AssessmentSave_OunceV7";
            assessmentRun = O2Assessment_OunceV7_Utils.getDefaultAssessmentRunObject();
        }

        public string engineName {get; set;}
        

        public string save(List<IO2Finding> o2Findings)
        {
            string tempOzasmtFile = PublicDI.config.getTempFileInTempDirectory("ozasmt");

            return (save(o2Findings,tempOzasmtFile)) ? tempOzasmtFile : "";
        }

        public bool save(List<IO2Finding> o2Findings, string sPathToSaveAssessment)
        {
            return save(assessmentRun.name, o2Findings, sPathToSaveAssessment);            
        }

        public bool save(string assessmentName, IEnumerable<IO2Finding> o2Findings, string sPathToSaveAssessment)
        {
            createAssessmentRunObject(assessmentName, o2Findings.toList());
            return assessmentRun.saveAs(sPathToSaveAssessment);            
        } 
		public void createAssessmentRunObject(List<IO2Finding> o2Findings)
		{
			createAssessmentRunObject(assessmentRun.name, o2Findings);
		}
        public void createAssessmentRunObject(string assessmentName, List<IO2Finding> o2Findings)
        {
            assessmentRun.name 						= assessmentName ?? "";
            assessmentRun.Assessment.assessee_file 	= "";            
            assessmentRun.Assessment.assessee_name 	= assessmentRun.name;
            assessmentRun.Assessment.assessee_type 	= "Application";
            addO2FindingsToAssessmentRunObject(o2Findings);
        }

        public AssessmentRun createAssessmentRunObject(IO2Assessment o2Assessment)
        {
            createAssessmentRunObject(o2Assessment.name, o2Assessment.o2Findings);
            return assessmentRun;
        }

        public void addO2FindingsToAssessmentRunObject(List<IO2Finding> o2Findings)
        {
        	assessmentRun.FilePool 		= getFilePool(o2Findings);
        	assessmentRun.StringPool	= getStringPool(o2Findings);
        	
            //Dictionary<string, List<AssessmentRunFile>> filesMappedToO2Findings = getFilePool(o2Findings);
            
            /*
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
            */
        }
        
        public AssessmentRunFile[] getFilePool(List<IO2Finding> o2Findings)
        {            
            var uniqueFiles = (	from o2Finding in o2Findings
            					where o2Finding.file.notNull()
            					select o2Finding.file).distinct();
            					
			var filesFromTraces = (	from trace in o2Findings.withTraces().allTraces()				
									where trace.file.notNull()
								 	select trace.file).distinct();
						
			uniqueFiles.add_If_Not_There(filesFromTraces);		   

			var filePool = new List<AssessmentRunFile>();
			
			filePool.add(new AssessmentRunFile() { id=1});
			UInt32 id = 2;
			foreach(var uniqueFile in uniqueFiles)
				filePool.add(new AssessmentRunFile() { id = id++ , value = uniqueFile});
				
		
		
			return filePool.ToArray();            
        }
        
        public AssessmentRunString[] getStringPool(List<IO2Finding> o2Findings)
        {        	
			var strings = new List<string>();
			foreach(var finding in o2Findings)
			{ 
				strings.addRange(finding.callerName, finding.context, finding.method,
								 finding.projectName,finding.vulnName,finding.vulnType);	
				foreach(var trace in finding.allTraces())	
					strings.addRange( trace.caller, trace.context ,trace.method ,trace.signature);				 
			}
						var filePool = new List<AssessmentRunFile>();
			
			var uniqueStrings = strings.distinct();
			
			var stringPool = new List<AssessmentRunString>();
			//stringPool.add(new AssessmentRunString() { id=1});
			UInt32 id = 1;
			foreach(var uniqueString in uniqueStrings)
				stringPool.add(new AssessmentRunString() { id = id++ , value = uniqueString});

			return stringPool.ToArray();
        }
/*
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
        }*/
    }
       
    public class O2Assessment_OunceV7_Utils
    {
    	public static AssessmentRun getVersionFromDirectLoad(string ozasmtFile)
		{
			return OzasmtUtils_OunceV7_0.getAssessmentRunObjectFromXmlFile(ozasmtFile);
		}
		
		public static AssessmentRun getVersionFromSaveEngine(string ozasmtFile)
		{
			var findings = ozasmtFile.loadO2Findings();
			var assessmentSave = new O2AssessmentSave_OunceV7(); 
			assessmentSave.createAssessmentRunObject(findings);
			return assessmentSave.assessmentRun;
		}		
    
        public static AssessmentRun getDefaultAssessmentRunObject()
        {
            // this is what we need to create a default assessment
            var defaultName = "DefaultAssessmentRun_v8";
            var defaultVersion  = "8.6.0.0";            				 
            
            var arNewAssessmentRun = new AssessmentRun
                                         	{
                                            	AssessmentStats = new AssessmentRunAssessmentStats(),         
									         	AssessmentConfig = new AssessmentRunAssessmentConfig(),
												SharedDataStats = new AssessmentRunSharedDataStats(),
												StringPool = new AssessmentRunString[] {},
												FilePool = new AssessmentRunFile[] {},
												SitePool = new AssessmentRunSite[] {},
												TaintPool = new AssessmentRunTaint[] {},
												FindingDataPool = new AssessmentRunFindingData[] {},
//												Assessment = new AssessmentRunAssessment(),
												Messages = new AssessmentRunMessage[] {},
												name = defaultName,		 			
												version = defaultVersion
                                         	};
//not sure if this is needed                                         	
/*            var armMessage = new AssessmentRunMessage
                                 {
                                     id = 0,
                                     message =
                                         ("Custom Assessment Run File created on " +
                                          DateTime.Now)
                                 };
            arNewAssessmentRun.Messages = new[] { armMessage };*/
            arNewAssessmentRun.Assessment = new AssessmentRunAssessment { Assessment = new[] { new Assessment() } };
            // need to populate the date 
            arNewAssessmentRun.AssessmentStats.date =
                (uint)(DateTime.Now.Minute * 1000 + DateTime.Now.Second * 50 + DateTime.Now.Millisecond);
            // This should be enough to create unique timestamps 
            return arNewAssessmentRun;
        }
    }
    
    
    public static class O2Assessment_OunceV7_ExtensionMethods
    {
    	public static List<string> files (this AssessmentRunFile[] assessmentRunFiles)
    	{
    		return assessmentRunFiles.Select((assessmentRunFile)=> assessmentRunFile.value).toList();
    	}
    	
    	public static List<string> strings (this AssessmentRunString[] assessmentRunStrings)
    	{
    		return assessmentRunStrings.Select((assessmentRunString)=> assessmentRunString.value).toList();
    	}
    }
}
