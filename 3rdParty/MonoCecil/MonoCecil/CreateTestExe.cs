// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using Mono.Cecil;
using O2.External.O2Mono;
using O2.Kernel;
//O2File:CecilUtils.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class CreateTestExe
    {
        public static string exeName = "BasicHelloWorld";
        public CecilAssemblyBuilder cecilAssemblyBuilder;

        public CreateTestExe createBasicHelloWorldExe()
        {
            return createBasicHelloWorldExe(false);
        }

        public CreateTestExe createBasicHelloWorldExe(bool bWithPressEnter)
        {
            cecilAssemblyBuilder = new CecilAssemblyBuilder(exeName, ModuleKind.Console);
            TypeDefinition tdType = cecilAssemblyBuilder.addType("BasicTest", "Program");
            MethodDefinition mdMain = cecilAssemblyBuilder.addMainMethod(tdType);
            cecilAssemblyBuilder.codeBlock_ConsoleWriteLine(mdMain,
                                                            String.Format(
                                                                "Hello World " + Environment.NewLine +
                                                                Environment.NewLine +
                                                                "(Created at {0})" + Environment.NewLine +
                                                                Environment.NewLine +
                                                                "(by {1})", DateTime.Now, Environment.UserName));
            if (bWithPressEnter)
                cecilAssemblyBuilder.codeBlock_PressEnter(mdMain);
            return this;
        }

        public string save()
        {
            return save(PublicDI.config.O2TempDir);
        }

        public string save(string targetFolder)
        {
            return cecilAssemblyBuilder.Save(targetFolder);
        }

        public string save(string targetFolder, string fileName)
        {
            return cecilAssemblyBuilder.Save(targetFolder, fileName);
        }
    }
}
