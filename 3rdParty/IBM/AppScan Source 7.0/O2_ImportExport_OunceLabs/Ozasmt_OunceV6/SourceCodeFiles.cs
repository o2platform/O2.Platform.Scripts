// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Misc;
using O2.DotNetWrappers.Xsd;
using O2.ImportExport.OunceLabs;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;

namespace O2.Legacy.OunceV6.SavedAssessmentFile.classes
{
    public class SourceCodeFiles
    {
        public static bool areAllSourceCodeReferencesInAssessmentFileValid(O2AssessmentData_OunceV6 oadO2AssessmentDataOunceV6)
        {
            DI.log.debug("Checking to see if all source code references are valid");
            if (oadO2AssessmentDataOunceV6.arAssessmentRun == null)
            {
                DI.log.error(
                    "in areAllSourceCodeReferencesInAssessmentFileValid: oadO2AssessmentDataOunceV6.arAssessmentRun == null  (aborting)");
                return true;
            }
            try
            {
                foreach (string sFile in getListOfUniqueFiles(oadO2AssessmentDataOunceV6))
                    if (false == File.Exists(sFile))
                        return false;
                return true;
            }
            catch (Exception ex)
            {
                DI.log.error("in areAllSourceCodeReferencesInAssessmentFileValid: {0}", ex.Message);
                return false;
            }
        }

        public static void tryToFixSourceCodeReferences(O2AssessmentData_OunceV6 oadO2AssessmentDataOunceV6)
        {
            DI.log.debug("Trying To Fix Source Code References");
            SourceCodeMappings scmSourceCodeMappings = SourceCodeMappingsUtils.getSourceCodeMappings();
            foreach (SourceCodeMappingsMapping mMapping in scmSourceCodeMappings.Mapping)
            {
                fixAllFileReferencesOnAssessmentDataObject(oadO2AssessmentDataOunceV6, mMapping.replaceThisString,
                                                           mMapping.withThisString);
            }
        }

        public static List<String> getListOfUniqueFiles(O2AssessmentData_OunceV6 oadO2AssessmentDataOunceV6)
        {
            var lsUniqueFiles = new List<string>();
            // search in FileIndexes            
            foreach (AssessmentRunFileIndex fiFileIndex in oadO2AssessmentDataOunceV6.arAssessmentRun.FileIndeces)
                if (false == lsUniqueFiles.Contains(fiFileIndex.value))
                    lsUniqueFiles.Add(fiFileIndex.value);
            foreach (AssessmentAssessmentFile fFile in oadO2AssessmentDataOunceV6.dAssessmentFiles.Keys)
                if (false == lsUniqueFiles.Contains(fFile.filename))
                    lsUniqueFiles.Add(fFile.filename);
            // search in FileFindings
            // {to do}
            return lsUniqueFiles;
        }


  

        public static void fixAllFileReferencesOnAssessmentDataObject(O2AssessmentData_OunceV6 oadO2AssessmentDataOunceV6,
                                                                      String sFix_PathToFind, String sFix_PathToReplace)
        {
            var rfmresolvedFileMapping = new SourceCodeMappingsUtils.resolvedFileMapping("")
                                             {
                                                 sFix_PathToFind = sFix_PathToFind,
                                                 sFix_PathToReplace = sFix_PathToReplace
                                             };
            fixAllFileReferencesOnAssessmentDataObject(oadO2AssessmentDataOunceV6, rfmresolvedFileMapping);
        }

        public static void fixAllFileReferencesOnAssessmentDataObject(O2AssessmentData_OunceV6 oadO2AssessmentDataOunceV6,
                                                                      SourceCodeMappingsUtils.resolvedFileMapping rfmResolvedFileMapping)
        {
            if (oadO2AssessmentDataOunceV6 != null && rfmResolvedFileMapping != null)
            {
                foreach (AssessmentRunFileIndex fiFileIndex in oadO2AssessmentDataOunceV6.arAssessmentRun.FileIndeces)
                    fiFileIndex.value = fiFileIndex.value.Replace(rfmResolvedFileMapping.sFix_PathToFind,
                                                                  rfmResolvedFileMapping.sFix_PathToReplace);
                foreach (AssessmentAssessmentFile fFile in oadO2AssessmentDataOunceV6.dAssessmentFiles.Keys)
                    if (rfmResolvedFileMapping.sFix_PathToFind != "")
                        fFile.filename = fFile.filename.Replace(rfmResolvedFileMapping.sFix_PathToFind,
                                                            rfmResolvedFileMapping.sFix_PathToReplace);
                    else
                        fFile.filename = Path.Combine(rfmResolvedFileMapping.sFix_PathToReplace, fFile.filename);
            }
        }

       
    }
}
