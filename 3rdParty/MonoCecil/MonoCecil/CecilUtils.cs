// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ICSharpCode.NRefactory.TypeSystem;
using O2.DotNetWrappers.Windows;
using O2.Kernel;

//O2File:API_ILSpy.cs
//O2Ref:ILSpy\Mono.Cecil.dll
//O2Ref:ILSpy\ICSharpCode.Decompiler.dll
//O2Ref:ILSpy\ICSharpCode.NRefactory.dll

//O2File:MethodCalled.cs
//O2File:CecilAssemblyBuilder.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class CecilUtils
    {    	
        // see mono Reflector Add-on since it contains this feature (so figure out how they do it)
        public void tryToFixPublicKey(String sAssemblyToFix)
        {
            AssemblyDefinition adAssembly = AssemblyDefinition.ReadAssembly(sAssemblyToFix);
            adAssembly.Name.PublicKey = null;            
            adAssembly.Write(sAssemblyToFix + ".fix.dll");
            //AssemblyFactory.SaveAssembly(adAssembly, sAssemblyToFix + ".fix.dll");
        }



        public static AssemblyDefinition getAssembly(String assemblyToLoad)
        {
            try
            {                
           //     return AssemblyFactory.GetAssembly(Files.getFileContentsAsByteArray(assemblyToLoad));

                return  AssemblyDefinition.ReadAssembly(assemblyToLoad);
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getAssembly");
                return null;
            }
        }

        public static MethodDefinition getAssemblyEntryPoint(string assemblyToLoad)
        {
            try
            {
                return getAssembly(assemblyToLoad).EntryPoint;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getAssemblyEntryPoint");
                return null;
            }
        }

        public static List<ModuleDefinition> getModules(String assemblyToLoad)
        {
            try
            {
                if (File.Exists(assemblyToLoad))
                {
                    AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyToLoad);
                    return getModules(assemblyDefinition);
                }
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getModules");
                
            }
            return null;
        }

        public static List<ModuleDefinition> getModules(AssemblyDefinition assemblyDefinition)
        {
            try
            {
                var modules = new List<ModuleDefinition>();
                foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    modules.Add(moduleDefinition);
                return modules;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getModules");
                return null;
            }


        }

        public static List<TypeDefinition> getTypes(String assemblyToLoad)
        {
            try
            {
                var types = new List<TypeDefinition>();
                foreach (ModuleDefinition moduleDefinition in getModules(assemblyToLoad))
                    types.AddRange(getTypes(moduleDefinition));
                return types;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getTypes");
                return null;
            }
        }

        public static List<TypeDefinition> getTypes(AssemblyDefinition assemblyDefinition)
        {
            try
            {
                var types = new List<TypeDefinition>();
                foreach (ModuleDefinition module in assemblyDefinition.Modules)
                    foreach (TypeDefinition type in getTypes(module))
                        types.Add(type);
                return types;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getTypes");
                return null;
            }
        }

        public static List<TypeDefinition> getTypes(ModuleDefinition moduleDefinition)
        {
            try
            {
                var types = new List<TypeDefinition>();
                foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                    types.Add(typeDefinition);
                return types;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static List<TypeDefinition> getTypes(TypeDefinition typeDefinition)
        {
            try
            {
                var types = new List<TypeDefinition>();
                foreach (TypeDefinition type in typeDefinition.NestedTypes)
                    types.Add(type);
                return types;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getTypes");
                return null;
            }
        }

   /*     public static List<MethodReference> getConstructors(TypeDefinition tType)
        {
            try
            {
                var constructors = new List<MethodReference>();
                foreach(var constructor in tType.Constructors)
                {
                    var type = constructor.GetType();
                    var typename = constructor.GetType().Name;
                }
                return constructors;
            }
            catch (Exception ex)
            {
                DI.log.ex(ex, "in CecilUtils.getConstructors");
                return null;
            }
        }*/

        public static List<MethodDefinition> getMethods(AssemblyDefinition assemblyToLoad)
        {
            try
            {
                var methods = new List<MethodDefinition>();
                foreach (TypeDefinition typeDefinition in getTypes(assemblyToLoad))
                    foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                        methods.Add(methodDefinition);
                return methods;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethods");
                return null;
            }
        }


        public static List<MethodDefinition> getMethods(String assemblyToLoad)
        {
            try
            {
                return getMethods(getAssembly(assemblyToLoad));
          /*      var methods = new List<MethodDefinition>();
                foreach (TypeDefinition typeDefinition in getTypes(assemblyToLoad))
                    foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                        methods.Add(methodDefinition);
                return methods;*/
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethods");
                return null;
            }
        }

        public static List<MethodDefinition> getMethods(TypeDefinition typeDefinition)
        {
            try
            {
                var methods = new List<MethodDefinition>();
                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                    methods.Add(methodDefinition);
                return methods;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethods");
                return null;
            }
        }

        public static List<MethodDefinition> getMethodsStatic(TypeDefinition tType)
        {
            try
            {
                var methods = new List<MethodDefinition>();
                foreach (MethodDefinition methodDefinition in getMethods(tType))
                    if (methodDefinition.IsStatic)
                        methods.Add(methodDefinition);
                return methods;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethodsStatic");
                return null;
            }
        }

        public static MethodDefinition getMethod(TypeDefinition type, string methodToFind, object[] methodParameters)
        {
            var methodParameterTypes = new List<Type>();
            if (methodParameters!=null && methodParameters.Length >0)
                foreach(var methodParameter in methodParameters)
                    methodParameterTypes.Add(methodParameter.GetType());
            return (getMethod(type, methodToFind, methodParameterTypes.ToArray()));
        }

        public static MethodDefinition getMethod(TypeDefinition type, string methodToFind, Type[] methodParameters)
        {
            if (methodParameters == null)
                methodParameters = new Type[0];
            try
            {
                foreach (MethodDefinition method in getMethods(type))
                {
                    if (method.Name == methodToFind)
                    {
                        var parameters = method.Parameters;
                        if (parameters.Count == 0 && (methodParameters.Length == 0))
                            return method;
                        if (parameters.Count == methodParameters.Length)
                        {
                            bool allParamsMatched = false;
                            for (int i = 0; i < parameters.Count; i++)
                            {                                   
                                if (parameters[i].ParameterType.FullName == methodParameters[i].FullName)
                                    allParamsMatched = true;
                                else
                                {
                                    allParamsMatched = false;
                                    break;
                                }
                            }
                            if (allParamsMatched)
                                return method;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethod");                
            }
            return null;
        }

        public static List<MethodCalled> getMethodsCalledInsideAssembly(String assemblyToLoad)
        {
            try
            {
                var methodsCalled = new List<MethodCalled>();
                foreach (MethodDefinition methodDefinition in getMethods(assemblyToLoad))
                    methodsCalled.AddRange(getMethodsCalledInsideMethod(methodDefinition));
                return methodsCalled;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethodsCalledInsideAssembly");
                return null;
            }
        }

        public static List<MethodCalled> getMethodsCalledInsideMethod(MethodDefinition methodDefinition)
        {
            try
            {
                var methodsCalled = new List<MethodCalled>();
                if (methodDefinition.Body != null)
                {
                    SequencePoint currentSequencePoint = null;
                    foreach (Instruction instruction in methodDefinition.Body.Instructions)
                    {
                        currentSequencePoint = instruction.SequencePoint ?? currentSequencePoint;
                        if (instruction.Operand != null)
                        {
                            switch (instruction.Operand.GetType().Name)
                            {
                                case "MethodReference":
                                case "MethodDefinition":
                                    methodsCalled.Add(new MethodCalled((IMemberReference)instruction.Operand, currentSequencePoint));
                                    break;
                                default:
                                    break;
                            }
                            //DI.log.info(instruction.Operand.GetType().Name);
                            //if (instruction.Operand.GetType().Name == "MethodReference")
                            // need to check if I also need to hook into MethodDefinition                         
                        }
                    }
                }
                if (methodsCalled.Count ==0)
                {
                    var tokenMethod = methodDefinition.MetadataToken;
                    var tokenType = methodDefinition.DeclaringType.MetadataToken;
                    if (methodDefinition.HasBody)
                    {
                    }
                }
                return methodsCalled;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getMethodsCalledInsideMethod");
                return null;
            }
        }

		//these two need to be converted to the new version Mono.Cecil
		/*
        public static Object getAttributeValueFromAssembly(String assemblyToLoad, String sAttributeToFetch,
                                                           int iParameterValueIndex)
        {
            try
            {
                PublicDI.log.info("getAttributeValueFromAssembly -  assemblyToLoad: {0}  sAttributeToFetch: {1}  iParameterValueIndex: {2}  ", assemblyToLoad, sAttributeToFetch, iParameterValueIndex);
                AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyToLoad);
                foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                        foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                            foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
                                if (customAttribute.Constructor.DeclaringType.Name == sAttributeToFetch)
                                    return customAttribute.ConstructorParameters[iParameterValueIndex];
                return "";
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getAttributeValueFromAssembly");
                return null;
            }
        }

        public static bool setAttributeValueFromAssembly(String assemblyToLoad, String sAttributeToFetch,
                                                         int iParameterValueIndex, Object oValue)
        {
            try
            {
               	PublicDI.log.info("getAttributeValueFromAssembly -  assemblyToLoad: {0}  sAttributeToFetch: {1}  iParameterValueIndex: {2}   oValue: {3}", assemblyToLoad, sAttributeToFetch, iParameterValueIndex, oValue);
                AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyToLoad);
                foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
                    foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
                        foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
                            foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
                                if (customAttribute.Constructor.DeclaringType.Name == sAttributeToFetch)
                                {
                                    customAttribute.ConstructorParameters[iParameterValueIndex] = oValue;
                                    assemblyDefinition.Write(assemblyToLoad);
                                    return true;
                                }
                return false;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.setAttributeValueFromAssembly");
                return false;
            }
        }*/


        public static Dictionary<string, List<TypeDefinition>> getDictionaryWithTypesMappedToNamespaces(
            ModuleDefinition moduleDefinition)
        {
            try
            {
                var typesMappedToNamespaces = new Dictionary<string, List<TypeDefinition>>();
                foreach (TypeDefinition type in getTypes(moduleDefinition))
                {
                    string typeNamespace = type.Namespace;
                    if (typeNamespace == null)
                    {
                        if (!typesMappedToNamespaces.ContainsKey(""))
                            typesMappedToNamespaces.Add("", new List<TypeDefinition>());
                        typesMappedToNamespaces[""].Add(type);
                    }
                    else
                    {
                        if (!typesMappedToNamespaces.ContainsKey(typeNamespace))
                            typesMappedToNamespaces.Add(typeNamespace, new List<TypeDefinition>());
                        typesMappedToNamespaces[typeNamespace].Add(type);
                    }
                }
                return typesMappedToNamespaces;
            }
            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getDictionaryWithTypesMappedToNamespaces");
                return null;
            }
        }

        public static TypeDefinition getType(AssemblyDefinition assemblyDefinition, string name)
        {
            try
            {
                List<TypeDefinition> types = getTypes(assemblyDefinition);
                foreach (TypeDefinition type in types)
                    if (type.Name == name || type.FullName == name)
                        return type;
                return null;
            }

            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.getType");
                return null;
            }
        }

        public static string CreateAssemblyFromType(TypeDefinition typeToExtract, string targetFolder)
        {
            try
            {
                string fileName = typeToExtract.Name.Replace('<', '_').Replace('>', '_');
                var cecilNewAssembly = new CecilAssemblyBuilder(fileName);


                cecilNewAssembly.addDummyType();
                // todo: checkout why It looks like I need this in order for the type cloning to work 
                cecilNewAssembly.addType(typeToExtract);
                return cecilNewAssembly.Save(targetFolder);
            }

            catch (Exception ex)
            {
                PublicDI.log.ex(ex, "in CecilUtils.CreateAssemblyFromType");
                return null;
            }
        }

        public static bool isDotNetAssembly(string assemblyToCheck)
        {
            return isDotNetAssembly(assemblyToCheck, true);
        }

        public static bool isDotNetAssembly(string assemblyToCheck, bool verbose)
        {
            try
            {
                //AssemblyDefinition assemblyManifest = AssemblyFactory.GetAssemblyManifest(assemblyToCheck);
                AssemblyDefinition assemblyManifest = AssemblyDefinition.ReadAssembly(assemblyToCheck);
                //var assembly = loadAssembly(assemblyToCheck);
                if (assemblyManifest == null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                if (verbose)
                    PublicDI.log.error("in CecilUtils.isDotNetAssembly: while processing {0} this error occured: {1}", assemblyToCheck, ex.Message);
                return false;
            }
        }

        public static string getMethodTypeNameAndParameters(MethodReference methodReference, string splitStringBetweenTypeAndMethod)
        {
            //return string.Format("{0}{1}{2}", methodReference.ReturnType.ReturnType.FullName, splitStringBetweenTypeAndMethod, getMethodParametersSiganature(methodReference));
            return string.Format("{0}{1}{2}", methodReference.MethodReturnType, splitStringBetweenTypeAndMethod, getMethodParametersSiganature(methodReference));
        }

        public static string getMethodNameAndParameters(MethodReference methodReference)
        {
            return string.Format("{0}{1}", methodReference.Name, getMethodParametersSiganature(methodReference));
        }
    
        

        //based on the code from Mono.Cecil.MethodReference.ToString()
        public static string getMethodParametersSiganature(MethodReference methodReference)
        {
        	return methodReference.FullName;
        	
        	//the GetSentinel was not working in the new version
        	/*
            int sentinel = methodReference.GetSentinel();
            var builder = new StringBuilder();            
            builder.Append("(");
            for (int i = 0; i < methodReference.Parameters.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                if (i == sentinel)
                {
                    builder.Append("...,");
                }
                builder.Append(methodReference.Parameters[i].ParameterType.FullName);
            }
            builder.Append(")");
            return builder.ToString();*/
        }
    }
}
