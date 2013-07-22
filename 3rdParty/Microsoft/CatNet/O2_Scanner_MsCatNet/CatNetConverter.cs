// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 1/19/2009
 * Time: 10:26 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Xml;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.WinForms.O2Findings;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;

//O2File:O2AssessmentData_OunceV6.cs
//O2File:O2AssessmentSave_OunceV6.cs


namespace O2.Scanner.MsCatNet.Converter
{
    /// <summary>
    /// Description of CatNetConverter.
    /// </summary>
    public class CatNetConverter : IXmlConverter
    {
        //String sPathToOzasmtFile = "";
        //String sConvertedOzasmtFile = "";        
        //   List<String> lsErrors = new List<String>();

        public XmlDocument catNetXml = new XmlDocument();
        private String fileToConvert;

        public CatNetConverter(String fileToConvert)
        {
            //this.sFileToConvert = sFileToConvert;
            //sPathToOzasmtFile = Path.GetDirectoryName(sFileToConvert);
            loadFileToConvert(fileToConvert);
        }

        #region IXmlConverter Members

        public bool loadFileToConvert(String _fileToConvert)
        {
            try
            {
                fileToConvert = _fileToConvert;
                catNetXml.LoadXml(Files.getFileContents(fileToConvert));
                // there was a race condition here (maybe caused by the use of FileSystemWatch to trigger this), which was causing the xml file not to be loaded all times
                if (catNetXml.InnerXml == "")
                {
                    PublicDI.log.error("There was a problem loading CatXml file since catNetXml.InnerXml == \"\" for {0}",
                                 _fileToConvert);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                return false;
            }
        }

        public bool convert()
        {
            return convert(fileToConvert + ".ozasmt");
        }

        public bool convert(String sTargetOzasmtFile)
        {
            try
            {
                if (catNetXml == null || catNetXml.InnerXml == "")
                    return false;
                var o2Assessment = new O2Assessment();
                addCatNetResultsAsFindings(o2Assessment, catNetXml);

                if (o2Assessment.o2Findings.Count > 0)
                {
                    o2Assessment.save(new O2AssessmentSave_OunceV6(),sTargetOzasmtFile);
                    PublicDI.log.info("Converted ozasmt file (with {0} findings) saved to {0}", sTargetOzasmtFile);
                    return true;
                }
                PublicDI.log.info("There were no findings in converted file (from: {0})", sTargetOzasmtFile);
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CatNetConverted.convert");
            }
            return false;
        }

        #endregion

        public static void addCatNetResultsAsFindings(O2Assessment o2Assessment, XmlDocument catNetXml)
        {
            //var results = catNetXml.GetElementsByTagName("Resultsss");
            PublicDI.log.info(" -------------------- ");

            foreach (XmlElement rule in catNetXml.GetElementsByTagName("Rule"))
            {
                try
                {
                    XmlElement ruleNameXmlElement = rule["Name"];
                    string ruleName = (ruleNameXmlElement == null) ? "Unknown Rule Name" : ruleNameXmlElement.InnerText;

                    foreach (XmlNode result in rule.GetElementsByTagName("Result"))
                    {
                        // ReSharper disable PossibleNullReferenceException
                        string signature = getSignatureFromEntryPoint(result["EntryPoint"].InnerText);

                        var o2Finding = new O2Finding();

                        o2Finding.context = (result["EntryPoint"] == null) ? "" : result["EntryPoint"].InnerText;
                        o2Finding.confidence = (result["ConfidenceLevel"] == null)
                                                   ? (byte) 0
                                                   : getConfidence(result["ConfidenceLevel"].InnerText);
                        o2Finding.callerName = getMethodNameFromSignature(signature);
                        o2Finding.lineNumber = (result["Transformations"] == null &&
                                                result["Transformations"]["Origin"] != null)
                                                   ? 0
                                                   : uint.Parse(
                                                         result["Transformations"]["Origin"].GetAttribute("line"));


                        o2Finding.file = (result["Transformations"] == null &&
                                          result["Transformations"]["Origin"] != null)
                                             ? ""
                                             : result["Transformations"]["Origin"].GetAttribute("file");
                        o2Finding.severity = 2;
                        o2Finding.vulnName = signature;
                        o2Finding.vulnType = ruleName;

                        //                        };

                        o2Finding.text.Add(result["Resolution"].InnerText);
                        o2Finding.text.Add(result["ProblemDescription"].InnerText);


                        addCatNetTransformationsAsO2Traces(o2Finding, result["Transformations"]);

                        // ReSharper restore PossibleNullReferenceException
                        o2Assessment.o2Findings.Add(o2Finding);
                    }
                }
                catch (Exception ex)
                {
                    PublicDI.log.ex(ex, "in addCatNetResultsAsFindings, while processing rule: " + rule.InnerXml);
                }
            }
        }

        public static byte getConfidence(String confidenceLevel)
        {
            switch (confidenceLevel)
            {
                default:
                    return 0;
            }
        }

        public static string getSignatureFromEntryPoint(string sEntryPoint)
        {
            int indexOfSemiColon = sEntryPoint.IndexOf(';');
            if (indexOfSemiColon > -1)
            {
                string parseName = sEntryPoint.Substring(0, indexOfSemiColon);
                parseName = parseName.Replace("MethodHeader (", "");

                return parseName.Substring(0, parseName.Length - 1);
            }
            return sEntryPoint;
        }

        public static string getMethodNameFromSignature(string signature)
        {
            int indexOfLeftBracket = signature.IndexOf('(');
            if (indexOfLeftBracket > 0)
                return signature.Substring(0, indexOfLeftBracket);
            return signature;
        }

        public static void addCatNetTransformationsAsO2Traces(O2Finding o2Finding, XmlElement transformations)
        {
            // ReSharper disable PossibleNullReferenceException
            //var o2Trace = new O2Trace();
            //o2Finding.o2Traces.Add(o2Trace);
            foreach (XmlNode transformation in transformations.ChildNodes)
            {
                var o2Trace = new O2Trace
                                  {
                                      context =
                                          (transformation["Statement"] == null)
                                              ? ""
                                              : transformation["Statement"].InnerText,
                                      lineNumber =
                                          (transformation.Attributes["line"] == null)
                                              ? 0
                                              : uint.Parse(transformation.Attributes["line"].Value),
                                      file =
                                          (transformation.Attributes["file"] == null)
                                              ? ""
                                              : transformation.Attributes["file"].Value
                                  };

                switch (transformation.Name)
                {
                    case "Origin":
                        o2Finding.o2Traces.Add(new O2Trace
                                                   {
                                                       clazz = "Origin",
                                                       traceType = TraceType.Source,
                                                       method = transformation["StatementMethod"].InnerText,
                                                       signature = transformation["StatementMethod"].InnerText,
                                                       context = o2Trace.context,
                                                       lineNumber = o2Trace.lineNumber,
                                                       file = o2Trace.file
                                                   });
                        o2Finding.o2Traces.Add(o2Trace);
                        break;
                    case "MethodBoundary":
                    case "CallResult":
                        o2Trace.clazz = transformation.Name;
                        o2Trace.method = transformation["Method"].InnerText;
                        o2Trace.signature = o2Trace.method;
                        if (o2Finding.o2Traces.Count > 0)
                        {
                            o2Finding.o2Traces[0].childTraces.Add(o2Trace);
                            o2Trace.traceType = o2Finding.o2Traces[0].childTraces.Count ==
                                                (transformations.ChildNodes.Count - 1)
                                                    ? TraceType.Known_Sink
                                                    : TraceType.Root_Call;
                        }
                        else
                            o2Finding.o2Traces.Add(o2Trace);
                        break;
                    default:
                        break;
                }
            }
            // ReSharper restore PossibleNullReferenceException
        }

        public void Log(string sLogMessage)
        {
            Debug.WriteLine(sLogMessage);
        }
    }
}
