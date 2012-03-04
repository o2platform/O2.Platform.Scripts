// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Ref:System.dll
using System;
using System.IO;
using System.Collections.Generic;
//O2Ref:System.Core.dll
using System.Linq;
using System.Reflection;
using System.Text;
//O2Ref:O2_Interfaces.dll
using O2.Interfaces.O2Core;
//O2Ref:O2_Kernel.dll
using O2.Kernel; 
using O2.Kernel.Interfaces.XRules;
//O2Ref:O2_DotNetWrappers.dll
using O2.DotNetWrappers.O2Findings;
//O2File:IAnalysisArtifacts.cs
using O2.XRules.Database.Interfaces;  
//O2File:KAnalysisArtifacts.cs

namespace O2.XRules.Database._Rules.IBM.Analysis_Workflows
{
    public class XUtils_AnalysisWorkflow
    {    
        private static IO2Log log = PublicDI.log;    	    	    	    			
    	
        // creates a new Artifacts Object file
        public static IAnalysisArtifacts createAnalysisArtifact(string workflowName,string assessmentFile, string targetFolder)
        {    		
            var analysisArtifacts = new KAnalysisArtifacts(workflowName);
            analysisArtifacts.assessmentFilesOrFolderToLoad.Add(assessmentFile);            
            analysisArtifacts.targetFolder = targetFolder;        	
            return analysisArtifacts;
                        
        }
    	
        // creates a new Artifacts Object file and saves it
        public static bool createAnalysisArtifactFile(string workflowName,string assessmentFile, string targetFolder, string targetAnalysisArtifactsFile)
        {
            var analysisArtifacts = (KAnalysisArtifacts)createAnalysisArtifact(workflowName, assessmentFile, targetFolder);
            setAllPropertiesValue(analysisArtifacts,true);
            KAnalysisArtifacts.save(analysisArtifacts, targetAnalysisArtifactsFile);    		
            return File.Exists(targetAnalysisArtifactsFile);
        }
    	
        public static void setAllPhasesAndTasksValue(IAnalysisArtifacts analysisArtifacts,  bool value)
        {
            log.info("in setAllPhasesAndTasksValue");
            analysisArtifacts.runAllPhases = true;
            setAllPropertiesValue(analysisArtifacts.phase_1,value);
            setAllPropertiesValue(analysisArtifacts.phase_2,value);
            setAllPropertiesValue(analysisArtifacts.phase_3,value);
            setAllPropertiesValue(analysisArtifacts.phase_4,value);
            setAllPropertiesValue(analysisArtifacts.phase_5,value);
        }
    	
        public static void setAllPropertiesValue(object objectToProcess, bool value)
        {
            if (objectToProcess != null)
            {
                foreach(var property in PublicDI.reflection.getProperties(objectToProcess))    			
                {    				
                    if (property.PropertyType ==typeof(bool))
                    {
                        PublicDI.reflection.setProperty(property, objectToProcess,value);    				    			    					
                    }
                }
            }
            else
                log.error("object was null");
        }
    	
    }
}