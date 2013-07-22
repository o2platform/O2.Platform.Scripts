// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentSharp.CoreLib.API;
using Mono.Cecil;
using Mono.Cecil.Cil;

//O2File:CecilUtils.cs
//O2File:CecilAssemblyDependencies.cs
namespace O2.External.O2Mono.MonoCecil
{
    public class CecilCodeSearch
    {
        public static bool findInAssembly_CustomAttribute(String sAssemblyToLoad, String sAttributeToLoad)
        {
            try
            {
                Debug.WriteLine("Processing assembly: " + sAssemblyToLoad);
                AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(sAssemblyToLoad);
                foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                        foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                            foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
                                if (customAttribute.Constructor.DeclaringType.Name == sAttributeToLoad)
                                    return true;
                                //else
                                //    DI.log.info(customAttribute.Constructor.DeclaringType.Name);
            }
            catch (Exception ex)
            {
                PublicDI.log.error("in findInAssembly_CustomAttribute: ", ex.Message);
            }
            return false;
        }

        public static bool findInAssembly_Dependency(String sAssemblyToLoad, String dependencyToFind, bool recursiveSearch)
        {
            try
            {
                var dependencies = CecilAssemblyDependencies.getDictionaryOfDependenciesForAssembly_WithNoRecursiveSearch(sAssemblyToLoad);
            }
            catch (Exception ex)
            {
                PublicDI.log.error("in findInAssembly_CustomAttribute: ", ex.Message);
            }
            return false;
        }

        public static List<string> findInAssemblyVariable(string assemblyToLoad)
        {
            string methodToFind = "System.Void System.Web.UI.WebControls.Button::add_Click(System.EventHandler)";
            int parameterOffset = 2;
            return findInAssembly_OnCallX_ParameterType(assemblyToLoad, methodToFind, parameterOffset);
        }

        public static List<string> findInAssembly_OnCallX_ParameterType(string assemblyToLoad, string methodToFind,
                                                                        int parameterOffset)
        {
            var findings = new List<string>();
            foreach (MethodDefinition methodDefinition in CecilUtils.getMethods(assemblyToLoad))
                for (int i = 0; i < methodDefinition.Body.Instructions.Count; i++)
                {
                    Instruction instruction = methodDefinition.Body.Instructions[i];
                    if (instruction.Operand != null)
                    {
                        if (instruction.Operand.ToString() == methodToFind)
                        {
                            var sourceMethod = (MethodDefinition) instruction.Operand;
                            // DI.log.debug("[{0}] {1} ", instruction.OpCode.Name,

                            var sinkMethod =
                                (MethodDefinition) methodDefinition.Body.Instructions[i - parameterOffset].Operand;
                            // DI.log.debug("-->[{0}] {1} ", instructionWithParameter.OpCode.Name,
                            //               instructionWithParameter.Operand.ToString());
                            // DI.log.debug("{0} -- > {1}", sourceMethod.Name, sinkMethod.Name);
                            //MethodDefinition property = (MethodDefinition)method.Body.Instructions[i - parameterOffset].Operand;
                            findings.Add(String.Format("{0} -- > {1}", sourceMethod.Name, sinkMethod.Name));
                        }
                    }
                }

            return findings;
        }

        public static List<TypeDefinition> getTypesWithAttribute(string assemblyToLoad, string attributeToFinding)
        {
            var typesWithAttribute = new List<TypeDefinition>();
            foreach (TypeDefinition typeDefinition in CecilUtils.getTypes(assemblyToLoad))
            {
                foreach (CustomAttribute customAttribute in typeDefinition.CustomAttributes)
                    if (customAttribute.Constructor.DeclaringType.Name == attributeToFinding)
                        typesWithAttribute.Add(typeDefinition);
            }
            return typesWithAttribute;
        }

        public static List<MethodDefinition> getMethodsWithAttribute(TypeDefinition testFixture, string attributeToFinding)
        {
            var tests = new List<MethodDefinition>();            
            foreach (MethodDefinition methodDefinition in CecilUtils.getMethods(testFixture))
                foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
                    if (customAttribute.Constructor.DeclaringType.Name == attributeToFinding)
                        tests.Add(methodDefinition);
            return tests;
        }
    }
}
