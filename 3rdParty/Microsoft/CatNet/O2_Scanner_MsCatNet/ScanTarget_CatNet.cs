// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using O2.Kernel.InterfacesBaseImpl;

namespace O2.Scanner.MsCatNet.Scan
{
    public class ScanTarget_CatNet : KScanTarget
    {
        /*
                public bool scanDll(IScanTarget scanTarget, Callbacks.dMethod_String _logCallback, Callbacks.dMethod_Object dProcessCompletionCallback)
                {
                    String sSaveAssessmentTo = Path.Combine(scanTarget.WorkDirectory, Path.GetFileNameWithoutExtension(scanTarget.ApplicationFile) + "_MSCatNet.xml");
                    return scanDll(scanTarget.Target, sSaveAssessmentTo, _logCallback, dProcessCompletionCallback);
                }

                public bool scanDll(String sDllToScan)
                {
                    return scanDll(sDllToScan, DI.config.TempFileNameInTempDirectory + ".xml", null, null);
                }

                public bool scanDll(String sDllToScan, String pathToSaveResultsFile, Callbacks.dMethod_String logCallback, Callbacks.dMethod_Object dProcessCompletionCallback)
                {
                    invokeOnScanCompletion = dProcessCompletionCallback;
                    scanResults = pathToSaveResultsFile;
                    //            logCallback = _logCallback;  
                    //var sExecArguments = String.Format("/file:\"{0}\" /report:\"{1}\"", sDllToScan, scanResults);

                    return executeCatNetCommandWithArguments(sDllToScan, scanResults, logCallback, invokeOnScanCompletion);

                }

                

                private void internalOnScanCompleteCallback(Object oObject)
                {
                    try
                    {
                        if (File.Exists(scanResults))
                            new CatNetConverter(scanResults).convert();
                        Events.raiseRegistedCallbacks(invokeOnScanCompletion, new[] { oObject });
                    }
                    catch (Exception ex)
                    {
                         DI.log.ex(ex, "internalOnScanCompleteCallback");
                    }
                }

               
            }
         * */
    }
}
