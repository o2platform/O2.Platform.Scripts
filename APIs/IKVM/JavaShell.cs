// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.IO;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//O2File:JavaCompile.cs

namespace O2.XRules.Database.APIs.IKVM
{
    public class JavaShell
    {
        public string dataReceived;
        public StreamWriter inputStream;
        public Process IKVMProcess;
        public string classFileToExecute = "";
        public string classPath = "";        
        private string classToExecute = "";         
		public API_IKVM ikvm;
		
        public JavaShell(API_IKVM _ikvm)        
        {
        	ikvm = _ikvm;
        }
        
        public bool startJavaShell()
        {
            return startJavaShell(_dataReceivedCallBack,"");
        }

        public bool startJavaShell(DataReceivedEventHandler dataReceivedCallBack, string _classPath, string _classToExecute, string extraExecutionArguments)
        
        {
            return startJavaShell(dataReceivedCallBack,"");
        }

        public bool startJavaShell(DataReceivedEventHandler dataReceivedCallBack, string extraExecutionArguments)
        {
            try
            {
                classPath = getListOfCurrentJarStubsForClassPath(classPath);
                var executionArguments = string.Format(ikvm.IKVMExecution_Script,
                                                       classPath, classToExecute, extraExecutionArguments);
                if (extraExecutionArguments!="")
                    executionArguments += " " + extraExecutionArguments;
                var executionWorkingDirectory = PublicDI.config.CurrentExecutableDirectory;
                dataReceived = null;
                inputStream = null;
                IKVMProcess = null;
                IKVMProcess = Processes.startProcessAndRedirectIO(ikvm.IKVMExecutable, executionArguments, ref inputStream, dataReceivedCallBack, dataReceivedCallBack);
                if (inputStream != null)
                    return true;

                "in startJavaShell , IKVMProcess.inputStream == null".error();
            }
            catch (Exception ex)
            {
                ex.log();
            }
            return false;
        }

        private string getListOfCurrentJarStubsForClassPath(string classPath)
        {
            foreach (var jarStubFile in Files.getFilesFromDir_returnFullPath(ikvm.jarStubsCacheDir))
                classPath += string.Format(";\"{0}\"", jarStubFile);
            return classPath;
        }        
           
        public void exitfromIKVMShell()
        {            
            IKVMProcess.Kill();
            IKVMProcess.WaitForExit();
        }        

        private void _dataReceivedCallBack(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) == false)
            {
                dataReceived += e.Data;
            }
        }


/*        public static bool testIKVM()
        {
            return testIKVM("print 2+3", "5");
        }

        public static bool testIKVM(string scriptToExecute, string expectedDataReceived)
        {
            var IKVMExecution = new JavaShell();            
            var dataReceived = IKVMExecution.executeScript(scriptToExecute);
            return dataReceived == expectedDataReceived;
        }*/

        public static Process openJavaShellOnCmdExe()
        {
//            var executionArguments = string.Format(IKVMConfig.IKVMExecution_Script,IKVMConfig._IKVMRuntimeDir);            
//            return Processes.startProcess("IKVM.exe", executionArguments);            
            return null;
        }

        public void compileJavaFile(string fileToExecute)
        {
            var compiledClassFile  = new JavaCompile(ikvm).compileJavaFile(fileToExecute);
            if (compiledClassFile != "")
            {
                classToExecute = Path.GetFileNameWithoutExtension(compiledClassFile);
                classPath = Path.GetDirectoryName(compiledClassFile);
            }            
        }

        public void executeClassFile(DataReceivedEventHandler dataReceivedCallBack)
        {
            if (!string.IsNullOrEmpty(classToExecute))
            {
                startJavaShell(dataReceivedCallBack, classPath, classToExecute,"");
            }
        }

        public void executeClasFile(string _classFileToExecute, DataReceivedEventHandler dataReceivedCallBack)
        {
            classFileToExecute = _classFileToExecute;
            executeClassFile(dataReceivedCallBack);
        }

    }
}
