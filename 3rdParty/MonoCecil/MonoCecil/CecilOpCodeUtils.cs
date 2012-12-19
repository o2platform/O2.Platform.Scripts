// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

//O2File:CecilUtils.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class CecilOpCodeUtils
    {
        public static Instruction lastInstructionFromMethod(MethodDefinition mdTargetMethod)
        {
            return mdTargetMethod.Body.Instructions[mdTargetMethod.Body.Instructions.Count - 1];
        }

        public static void addInstructionsToMethod_InsertAtEnd(MethodDefinition mdTargetMethod,
                                                               List<Instruction> lsInstructionsToAdd)
        {
            var cwCliWorker = mdTargetMethod.Body.GetILProcessor();
            //for (int i = 0; i < lsInstructionsToAdd.Count; i++)  
            foreach (Instruction iInstruction in lsInstructionsToAdd)
                cwCliWorker.InsertBefore(lastInstructionFromMethod(mdTargetMethod), iInstruction);
        }
    }
}
