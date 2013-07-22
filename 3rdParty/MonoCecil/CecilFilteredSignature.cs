// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib.API;
using Mono.Cecil;

//O2File:MonoCecil/CecilUtils.cs

namespace O2.External.O2Mono
{
    public class CecilFilteredSignature : FilteredSignature
    {
        public CecilFilteredSignature(MethodDefinition methodInfo)
        {
            populateSignatureObjectsFromMethodInfo(methodInfo);
        }

        private void populateSignatureObjectsFromMethodInfo(MethodDefinition methodDefinition)
        {
            //   DI.log.info(" --   :{0}", methodInfo.Name);
            sFunctionName = methodDefinition.Name;
            foreach (ParameterDefinition parameter in methodDefinition.Parameters)
            {
                // sParameters += (parameter.ParameterType.IsGenericType)
                //                   ? getGenericSignature(parameter.ParameterType)
                //                   : parameter.ParameterType.Name;
                sParameters += parameter.ParameterType.Name;
                sParameters += ", ";
            }
            if (sParameters != "")
                sParameters = sParameters.Substring(0, sParameters.Length - 2);

            sReturnClass = getGenericSignature(methodDefinition.ReturnType);
        }

        public static string getGenericSignature(TypeReference genericType)
        {
            return genericType.Name;
            ///Todo: check MonoCecil support for generics in TypeDefinition
            /*
            if (!genericType.IsGenericType || genericType.Name != "List`1")
                return genericType.Name;
            string genericArguments = "";
            foreach (Type type in genericType.GetGenericArguments())
                genericArguments += type.Name + ",";
            if (genericArguments != "")
                genericArguments = genericArguments.Substring(0, genericArguments.Length - 1);
            return string.Format("List<{0}>", genericArguments);*/
        }
    }
}
