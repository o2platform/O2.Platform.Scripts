using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.ImportExport.Misc;
using O2.Interfaces.O2Findings;
using O2.XRules.Database.APIs;
//O2File:API_WebScarab.cs

namespace O2.ImportExport.Misc.WebScarab
{

    public class O2AssesmentLoad_WebScarab : IO2AssessmentLoad
    {
    	public string engineName { get; set;}                

        public O2AssesmentLoad_WebScarab()
        {
            this.engineName = "O2AssesmentLoad_WebScarab";
        }

        public static void addTrace(IO2Finding o2Finding, List<string> values, string key)
        {
            foreach (string value in values)
            {
                o2Finding.o2Traces.Add(new O2Trace(key + " = " + value));
            }
        }

        public static void addTrace(IO2Finding o2Finding, string value, string key)
        {
            if (value != null)
            {
                o2Finding.o2Traces.Add(new O2Trace(key + " = " + value));
            }
        }

        public bool canLoadFile(string fileToTryToLoad)
        {
            return (Path.GetFileName(fileToTryToLoad) == "conversationlog");
        }

        public static List<IO2Finding> createFindingsFromConversation(List<IWebscarabConversation> webScarabConversations)
        {
            List<IO2Finding> o2Findings = new List<IO2Finding>();
            foreach (IWebscarabConversation conversation in webScarabConversations)
            {
                O2Finding o2Finding = new O2Finding();
                if ((conversation.TAG != null) && (conversation.TAG != ""))
                {
                    o2Finding.vulnType = conversation.TAG;
                }
                else
                {
                    o2Finding.vulnType = "Tag not defined";
                }
                o2Finding.vulnName = conversation.METHOD + ": " + conversation.URL;
                addTrace(o2Finding, conversation.COOKIE, "COOKIE");
                addTrace(o2Finding, conversation.STATUS, "STATUS");
                addTrace(o2Finding, conversation.ORIGIN, "ORIGIN");
                addTrace(o2Finding, conversation.URL, "URL");
                addTrace(o2Finding, conversation.XSS_GET, "XSS_GET");
                addTrace(o2Finding, conversation.CRLF_GET, "CRLF_GET");
                addTrace(o2Finding, conversation.SET_COOKIE, "SET_COOKIE");
                addTrace(o2Finding, conversation.XSS_POST, "XSS_POST");
                var traceRequest = new O2Trace("request: " + conversation.request);
                traceRequest.file = conversation.request;                
                var traceResponse = new O2Trace("response: " + conversation.response);
                traceResponse.file = conversation.response;
                o2Finding.file =  conversation.response;
                o2Finding.o2Traces.Add(traceRequest);
                o2Finding.o2Traces.Add(traceResponse);
                o2Findings.Add(o2Finding);
            }
            return o2Findings;
        }

        public static IO2Assessment createO2AssessmentFromWebScarabFile(string conversationFile)
        {
            O2Assessment o2Assessment = new O2Assessment();
            try
            {
                o2Assessment.name = "Webscarab Import of: " + conversationFile;
                var webScarabConversations = new API_WebScarab().loadConversationsFile(conversationFile);                
                List<IO2Finding> o2Findings = createFindingsFromConversation(webScarabConversations);
                o2Assessment.o2Findings = o2Findings;
            }
            catch (Exception ex)
            {
                ex.log("in createO2AssessmentFromWebScarabFile");
            }
            return o2Assessment;
        }       

        public bool importFile(string fileToLoad, IO2Assessment o2Assessment)
        {
            IO2Assessment loadedO2Assessment = this.loadFile(fileToLoad);
            if (loadedO2Assessment != null)
            {
                o2Assessment.o2Findings = loadedO2Assessment.o2Findings;
                return true;
            }
            return false;
        }

        public IO2Assessment loadFile(string fileToLoad)
        {
            return createO2AssessmentFromWebScarabFile(fileToLoad);
        }
    }
}

