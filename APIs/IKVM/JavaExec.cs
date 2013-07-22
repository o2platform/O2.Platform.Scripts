// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Diagnostics;
using System.IO;
using FluentSharp.CoreLib.API;

//O2File:JavaShell.cs

namespace O2.XRules.Database.APIs.IKVM
{
    public class JavaExec
    {
    	public API_IKVM ikvm;
		
        public JavaExec(API_IKVM _ikvm)        
        {
        	ikvm = _ikvm;
            ikvm.checkIKVMInstallation();
        }

        public string executeJavaFile(string fileToExecute)
        {
                return executeJavaFile(fileToExecute, "");
        }

        public string executeJavaFile(string fileToExecute, string arguments)
        {            
            var classToExecute = Path.GetFileNameWithoutExtension(fileToExecute);
            var classPath = Path.GetDirectoryName(fileToExecute);
            var executionArguments = string.Format(ikvm.IKVMExecution_Script, classPath, classToExecute, arguments);
            return Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(ikvm.IKVMExecutable, executionArguments);
        }        

        // if we pass a callback for logging we need to start a IKVM shell
        public Process executeJavaFile(string fileToExecute, DataReceivedEventHandler dataReceivedCallBack)
        {            
            var IKVMShell = new JavaShell(ikvm);
            IKVMShell.compileJavaFile(fileToExecute);
            IKVMShell.executeClassFile(dataReceivedCallBack);
            //IKVMShell.startJavaShell(dataReceivedCallBack, fileToExecute);
            return IKVMShell.IKVMProcess;            
        }
                        

        public Process startIKVMShell(DataReceivedEventHandler dataReceivedCallBack)
        {
            var IKVMShell = new JavaShell(ikvm);
            IKVMShell.startJavaShell(dataReceivedCallBack,"");
            return IKVMShell.IKVMProcess;            
        }


    }
}
