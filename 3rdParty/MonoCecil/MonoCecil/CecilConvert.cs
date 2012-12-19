// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace O2.External.O2Mono.MonoCecil
{
    public class CecilConvert
    {
        public static MethodInfo getMethodInfoFromMethodDefinition(Type reflectionType, Mono.Cecil.MethodDefinition methodDefinition)
        {
            return DI.reflection.getMethod(reflectionType, methodDefinition.Name);    
        }

        public static MethodDefinition getMethodDefinitionFromMethodInfo(MethodInfo methodInfo, Mono.Cecil.AssemblyDefinition assemblyDefinition)
        {
            foreach (var methodDefinition in CecilUtils.getMethods(assemblyDefinition))
            {
                var functionSignature1 = new O2.DotNetWrappers.Filters.FilteredSignature(methodInfo);
                var functionSignature2 = new O2.DotNetWrappers.Filters.FilteredSignature(methodDefinition.ToString());
                if (functionSignature1.sSignature == functionSignature2.sSignature)                
                    return methodDefinition;
                if (functionSignature1.sFunctionName == functionSignature2.sFunctionName)
                { 
                }
            }
            DI.log.error("in getMethodDefinitionFromMethodInfo, could not map MethodInfo: {0}", methodInfo.ToString());
            return null;
        }
    }
}
