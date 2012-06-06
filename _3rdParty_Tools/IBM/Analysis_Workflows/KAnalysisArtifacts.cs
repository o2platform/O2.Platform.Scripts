// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
//O2Tag_OnlyAddReferencedAssemblies
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//O2Ref:System.Xml.dll
//O2Ref:System.Core.dll
using System.Xml.Serialization;
//O2Ref:O2_DotNetWrappers.Dll
using O2.DotNetWrappers.DotNet;
//O2Ref:O2_Kernel.dll
using O2.Kernel;
//O2File:IAnalysisArtifacts.cs

namespace O2.XRules.Database.Interfaces
{
    [Serializable]
    public class KAnalysisArtifacts : IAnalysisArtifacts
    {
        public string AnalysisID { get; set; }
        public List<string> assessmentFilesOrFolderToLoad { get; set; }
        public List<string> projectFilesOrFolder { get; set; }
        public List<string> projectWebRoots { get; set; }
        public string targetFolder { get; set; }
        public bool runAllPhases { get; set; }
        public KPhase_1 phase_1 { get; set; }           // can't use IPhase_1 because it will not be serializeable
        public KPhase_2 phase_2 { get; set; }
        public KPhase_3 phase_3 { get; set; }
        public KPhase_4 phase_4 { get; set; }
        public KPhase_5 phase_5 { get; set; }


 		public KAnalysisArtifacts(string analysisID)
        {
        	phase_1 = new KPhase_1();
        	phase_2 = new KPhase_2();
        	phase_3 = new KPhase_3();
        	phase_4 = new KPhase_4();
        	phase_5 = new KPhase_5();
            AnalysisID = analysisID ?? "O2Analysis";
            assessmentFilesOrFolderToLoad = new List<string>();
            projectFilesOrFolder = new List<string>();
            projectWebRoots = new List<string>();
            targetFolder = Path.Combine(PublicDI.config.O2TempDir, "_Analysis_Workflow_" + AnalysisID);
            runAllPhases = true;            
        }
        
        public string getAnalysisDetails()
        {
            var analysisDetails = new StringBuilder();
            viewHelper(assessmentFilesOrFolderToLoad, ref analysisDetails, "assessmentFilesOrFolderToLoad");
            viewHelper(projectFilesOrFolder, ref analysisDetails, "projectFilesOrFolder");
            viewHelper(projectWebRoots, ref analysisDetails, "projectWebRoots");
            analysisDetails.AppendFormat(": targetfolder = {0}\n", targetFolder);
            analysisDetails.AppendFormat(": runAllPhases = {0}\n", runAllPhases);
            return analysisDetails.ToString();
        }

        public void viewHelper(List<string> listToView, ref StringBuilder stringBuilder, string typeOfList)
        {
            if (listToView.Count > 0)
            {
                stringBuilder.AppendFormat(": {0} ({1})\n", typeOfList, listToView.Count);
                foreach (var fileOrFolder in listToView)
                    stringBuilder.AppendFormat("   - {0} \n", fileOrFolder);
                stringBuilder.AppendLine();
            }
        }

        public KAnalysisArtifacts() : this("")
        {
            phase_1 = new KPhase_1();
            phase_2 = new KPhase_2();
            phase_3 = new KPhase_3();
            phase_4 = new KPhase_4();
            phase_5 = new KPhase_5();
        }      

        public static IAnalysisArtifacts load(string analysisArtifactsFile)
        {
            try
            {
                return (IAnalysisArtifacts)Serialize.getDeSerializedObjectFromXmlFile(analysisArtifactsFile, typeof(KAnalysisArtifacts));                
            }
            catch (Exception ex)
            {
                PublicDI.log.error("KAnalysisArtifacts.load :{0}", ex.Message);
                return null;
            }
        }

        public static bool save(KAnalysisArtifacts analysisArtifacts, string targetFile)
        {
            try
            {
                Serialize.createSerializedXmlFileFromObject(analysisArtifacts, targetFile);
                return true;
            }
            catch (Exception ex)
            {
                PublicDI.log.error("KAnalysisArtifacts.save :{0}", ex.Message);
                return false;
            }
        }
    }

    [Serializable]
    public class KPhase_1 : IPhase_1
    {
        public bool run { get; set; }
        public bool task1_copyAssessmentFiles { get; set; }
        public bool task2_copyProjectConfigFiles { get; set; }

        public KPhase_1()
        {
            run = true;
            task1_copyAssessmentFiles = true;
            task2_copyProjectConfigFiles = true;
        }
    }

    [Serializable]
    public class KPhase_2 : IPhase_2
    {
        public bool run { get; set; }
        public bool task1_SplitFindingsOnTrace { get; set; }
        public bool task2_createStrutsMappings { get; set; }

        public KPhase_2()
        {
            run = true;
            task1_SplitFindingsOnTrace = true;
            task2_createStrutsMappings = true;
        }
    }

    [Serializable]
    public class KPhase_3 : IPhase_3
    {
        public bool run { get; set; }
        public bool task1_handleKnownSinks { get; set; }
        public bool task2_filterFindings { get; set; }
        public List<SourceSink>  task2_sourceSink { get; set; }
        public List<FindingsFilter> task2_findingsFilter { get; set; }
        public bool task3_filter_FindingsWithNoTraces { get; set; }
        public bool task4_CalculateStrutsFindings { get; set; }
        
        public KPhase_3()
        {
            run = true;
            task1_handleKnownSinks = true;
            task2_filterFindings = true;
            task3_filter_FindingsWithNoTraces = true;
            task4_CalculateStrutsFindings = true;
            task2_sourceSink = new List<SourceSink>();
            task2_findingsFilter = new List<FindingsFilter>();
        }
    }
    
    [Serializable]
    public class SourceSink
    {
    	[XmlAttribute]
    	public string Source { get; set; }
    	[XmlAttribute]
    	public string Sink { get; set; }    	
    	[XmlAttribute]
    	public bool RemoveMatches { get; set; }    	
    	
    	public SourceSink()
    	{
    		Source = "";
    		Sink = "";
    		RemoveMatches = false;
    	}
    	public SourceSink(string source, string sink, bool removeMatches)
    	{
    		Source = source;
    		Sink = sink;
    		RemoveMatches = removeMatches;
    	}
    	public SourceSink(string source, string sink) : this(source,sink,false)
    	{
    		
    	}
    }
    
    [Serializable]
    public class FindingsFilter
    {
    	[XmlAttribute]
    	public string Type { get; set; }
    	[XmlAttribute]
    	public string Value { get; set; }    	
    }

    [Serializable]
    public class KPhase_4 : IPhase_4
    {
        public bool run { get; set; }
        public bool task1_analyseFindingsWithKnownSinks { get; set; }
        public bool task2_AdjustsStrutsFindings { get; set; }

        public KPhase_4()
        {
            run = true;
            task1_analyseFindingsWithKnownSinks = true;
            task2_AdjustsStrutsFindings = true;
        }
    }

    [Serializable]
    public class KPhase_5 : IPhase_5
    {
        public bool run { get; set; }
        public bool task1_createFinalAssessmentFile { get; set; }

        public KPhase_5()
        {
            run = true;            
            task1_createFinalAssessmentFile = true;
        }
    }
}
