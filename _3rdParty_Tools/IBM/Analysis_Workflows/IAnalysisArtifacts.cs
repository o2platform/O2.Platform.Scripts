// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
//O2Tag_OnlyAddReferencedAssemblies
using System;
using System.Collections;
using System.Collections.Generic;
//O2Ref:System.Core.dll
using System.Linq;
//O2File:KAnalysisArtifacts.cs
//O2Ref:O2_Interfaces.dll
//O2Ref:NUnit.Framework.dll

namespace O2.XRules.Database.Interfaces
{
    public interface IAnalysisArtifacts
    {
        string AnalysisID { get; set; }
        List<string> assessmentFilesOrFolderToLoad { get; set; }
        List<string> projectFilesOrFolder { get; set; }
        List<string> projectWebRoots { get; set; }
        String targetFolder { get; set; }
        bool runAllPhases { get; set; }
        KPhase_1 phase_1 { get; set; }           // can't use IPhase_1 because it will not be serializeable
        KPhase_2 phase_2 { get; set; }
        KPhase_3 phase_3 { get; set; }
        KPhase_4 phase_4 { get; set; }
        KPhase_5 phase_5 { get; set; }
        string getAnalysisDetails();
    }

    public interface IPhase_1
    {
        bool run { get; set; }
        bool task1_copyAssessmentFiles { get; set; }
        bool task2_copyProjectConfigFiles { get; set; }
    }   

    public interface IPhase_2
    {
        bool run { get; set; }
        bool task1_SplitFindingsOnTrace { get; set; }
        bool task2_createStrutsMappings { get; set; }        
    }

    public interface IPhase_3
    {
        bool run { get; set; }
        bool task1_handleKnownSinks { get; set; }
        bool task2_filterFindings { get; set; }
        bool task3_filter_FindingsWithNoTraces { get; set; }
        bool task4_CalculateStrutsFindings { get; set; }
    }

    public interface IPhase_4
    {
        bool run { get; set; }
        bool task1_analyseFindingsWithKnownSinks { get; set; }
        bool task2_AdjustsStrutsFindings { get; set; }
    }

    public interface IPhase_5
    {
        bool run { get; set; }
        bool task1_createFinalAssessmentFile { get; set; }
    }
}
