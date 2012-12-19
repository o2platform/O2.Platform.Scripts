// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.Kernel.Objects;
using O2.Kernel;

//O2File:CecilUtils.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class CecilAssemblyDependencies
    {
        //private string targetDllDirectory;
        public List<string> processedAssemblies;                // for the recursive loop
        public Dictionary<string, string> assemblyDependencies; //  <fullname,path>
        public List<string> TargetDlls { get; set; }
        public List<string> pathsToFindReferencedDlls = new List<string>();

        public CecilAssemblyDependencies(string targetDll)
            : this(targetDll, new List<string>{Path.GetDirectoryName(targetDll)})
        {            
            
        }
        
        public CecilAssemblyDependencies(string targetDll, List<string> _pathsToFindReferencedDlls)
            :this (new List<string> {targetDll}, _pathsToFindReferencedDlls)
        {
            
        }

        public CecilAssemblyDependencies(List<string> targetDlls, List<string> _pathsToFindReferencedDlls)
        {
            TargetDlls = targetDlls;
            pathsToFindReferencedDlls = _pathsToFindReferencedDlls;
        }
        
        
        public Dictionary<string, string> calculateDependencies()
        {
            assemblyDependencies = new Dictionary<string, string>();
            processedAssemblies = new List<string>();
            foreach (var targetDll in TargetDlls)
            {
                PublicDI.log.info("Calculating DotNet dependencies for " + targetDll);                                
                calculateDependenciesForAssembly(targetDll); // start recursive search
            }
            return assemblyDependencies;
        }

        public void calculateDependenciesForAssembly(string dllToProcess) //getDependenciesForAssembly()
        {
            if (false == processedAssemblies.Contains(dllToProcess)) // only handle each dll once
            {
                if (CecilUtils.isDotNetAssembly(dllToProcess,false))
                {
                    processedAssemblies.add(dllToProcess);
                    var assemblyDefinition = CecilUtils.getAssembly(dllToProcess);
                    addAssemblyToDependenciesList(assemblyDefinition.Name.FullName, dllToProcess);
                    var modulesInDll = CecilUtils.getModules(assemblyDefinition);
                    if (modulesInDll != null)
                        foreach (ModuleDefinition module in modulesInDll)
                            foreach (AssemblyNameReference assemblyNameReference in module.AssemblyReferences)
                            {
                                //DI.log.info("{0}  -   {1} ", dllToProcess, assemblyNameReference.Name);
                                string mappedNameToTargetDllDirectory =
                                    tryToFindReferencedDllInProvidedReferenceSearchPaths(assemblyNameReference.Name);

                                addAssemblyToDependenciesList(assemblyNameReference.FullName, mappedNameToTargetDllDirectory);

                                // recursively search on each of these dlls since we need to find all dependent dlls
                                calculateDependenciesForAssembly(mappedNameToTargetDllDirectory);
                            }
                }
            }
            // log.info("There were {0} assemblyDependencies discovered", assemblyDependencies.Count);            
            //return assemblyDependencies;
        }

        private string tryToFindReferencedDllInProvidedReferenceSearchPaths(string assemblyToResolve)
        {
            if (false == File.Exists(assemblyToResolve))
                
            foreach (var referenceDirectory in pathsToFindReferencedDlls)
            {
                var fileAndDirectory = Path.Combine(referenceDirectory, assemblyToResolve);
                if (File.Exists(fileAndDirectory + ".dll"))
                    return fileAndDirectory + ".dll";
                if (File.Exists(fileAndDirectory + ".exe"))
                    return fileAndDirectory + ".exe";
            }
            return assemblyToResolve;
            
            /*Path.Combine(targetDllDirectory,
                                                                             assemblyNameReference.Name);
            // if the assemblyNameReference is in the  targerDllDirectory use it path in the assemblyDependencies Key's value
            if (File.Exists(mappedNameToTargetDllDirectory + ".exe"))
                mappedNameToTargetDllDirectory = addAssemblyToDependenciesList(assemblyNameReference.FullName,
                                              mappedNameToTargetDllDirectory + ".exe");
            else if (File.Exists(mappedNameToTargetDllDirectory + ".dll"))
                mappedNameToTargetDllDirectory = addAssemblyToDependenciesList(assemblyNameReference.FullName,
                                              mappedNameToTargetDllDirectory + ".dll");
            else
                mappedNameToTargetDllDirectory = addAssemblyToDependenciesList(assemblyNameReference.FullName, assemblyNameReference.Name);*/
        }

        public string addAssemblyToDependenciesList(string assemblyToAdd, string fullPathToAssembly)
        {
            if (false == assemblyDependencies.ContainsKey(assemblyToAdd))
                assemblyDependencies.Add(assemblyToAdd, fullPathToAssembly);
            else
            {

            }
            return fullPathToAssembly;
        }


        public static O2AppDomainFactory getO2AppDomainFactoryOnTempDirWithAllDependenciesResolved(
            string fullPathToDllToProcess)
        {
            var o2AppDomainFactory = new O2AppDomainFactory();
            if (false ==
                o2AppDomainFactory.load(Path.GetFileNameWithoutExtension(fullPathToDllToProcess), fullPathToDllToProcess,
                                        true))
                return null;
            Dictionary<string, string> assemblyDependencies =
                new CecilAssemblyDependencies(fullPathToDllToProcess).calculateDependencies();
     
            if (o2AppDomainFactory.load(assemblyDependencies).Count == 0)
                return o2AppDomainFactory;
            PublicDI.log.error(
                "in getO2AppDomainFactoryOnTempDirWithAllDependenciesResolved, there were assemblyDependencies that were not loaded, for: " +
                fullPathToDllToProcess);
            return null;
        }

        public static void copyAssemblyDependenciesToAssemblyDirectory(string targetAssembly)
        {
            var pathsToFindReferencedDll = new List<string>();
            // if no paths were provided add the current one
            pathsToFindReferencedDll.add(Path.GetDirectoryName(targetAssembly));
            // and if exists the hardcoded path            
            pathsToFindReferencedDll.add(PublicDI.config.CurrentExecutableDirectory);
            pathsToFindReferencedDll.add(PublicDI.config.ReferencesDownloadLocation);
            copyAssemblyDependenciesToAssemblyDirectory(targetAssembly, pathsToFindReferencedDll);
        }
        
        public static void copyAssemblyDependenciesToAssemblyDirectory(string pathToAssemblyToAnalyze, List<string> pathsToFindReferencedDlls)
        {
            var assemblyDependencies =
                new CecilAssemblyDependencies(pathToAssemblyToAnalyze, pathsToFindReferencedDlls).calculateDependencies();
            var targetDirectory = Path.GetDirectoryName(pathToAssemblyToAnalyze);
            foreach (var assemblyToCopy in assemblyDependencies.Values)
                if (File.Exists(assemblyToCopy))
                {
                    var targetFile = Path.Combine(targetDirectory, Path.GetFileName(assemblyToCopy));
                    if (false == File.Exists(targetFile))
                    {
                        File.Copy(assemblyToCopy, targetFile);
                        var pdbFile = assemblyToCopy.Replace(Path.GetExtension(assemblyToCopy), ".pdb");
                        if (File.Exists(pdbFile))
                            Files.copy(pdbFile, targetDirectory);
                    }
                }

        }

        public static List<string> getListOfDependenciesForType(Type type)
        {
            return getListOfDependenciesForTypes(new List<Type> {type});
        }

        public static List<string> getListOfDependenciesForTypes(List<Type> types)
        {
            var assemblies = new List<String>();
            foreach(var type in types)
                assemblies.Add(type.Assembly.Location);
            return getListOfDependenciesForAssemblies(assemblies);
        }


        public static Dictionary<string,string> getDictionaryOfDependenciesForAssembly_WithNoRecursiveSearch(string assembly)
        {
            var results = new Dictionary<string,string>();
            try
            {

                var assemblyDefinition = CecilUtils.getAssembly(assembly);
                if (assemblyDefinition != null)
                {
                    var modulesInDll = CecilUtils.getModules(assemblyDefinition);
                    if (modulesInDll != null)
                        foreach (ModuleDefinition module in modulesInDll)
                            foreach (AssemblyNameReference assemblyNameReference in module.AssemblyReferences)
                                results.Add(assemblyNameReference.FullName, assemblyNameReference.Name);
                }
            }
            catch (Exception ex)
            {
                PublicDI.log.error("in getListOfDependenciesForAssembly_WithNoRecursiveSearch: {0}", ex.Message);
            }
            return results;
        }
        
        public static List<string> getListOfDependenciesForAssembly(string assembly)
        {
            return getListOfDependenciesForAssemblies(new List<string> {assembly});
        }

        public static List<string> getListOfDependenciesForAssemblies(List<String> assemblies)
        {
            List<string> directoriesWithSourceAssemblies = getDefaultDirectoriesWithSourceAssemblies(assemblies);
            return getListOfDependenciesForAssemblies(assemblies, directoriesWithSourceAssemblies);
        }

        public static List<string> getDefaultDirectoriesWithSourceAssemblies(List<string> assemblies)
        {
            var directoriesWithSourceAssemblies = new List<String>();
            foreach(var assembly in assemblies)
            {
                var assemblyDirectory = Path.GetDirectoryName(assembly);
                if (false == directoriesWithSourceAssemblies.Contains(assemblyDirectory)) 
                    directoriesWithSourceAssemblies.Add(assemblyDirectory);
            }
            //if (Directory.Exists(DI.config.hardCodedO2LocalBuildDir))   // if this exists add it (solves the problem that on the UnitTests executions the dlls are placed on unique directories
            //    directoriesWithSourceAssemblies.Add(DI.config.hardCodedO2LocalBuildDir);
            var executingDirectory = Path.GetDirectoryName(PublicDI.config.ExecutingAssembly);
            if (false == directoriesWithSourceAssemblies.Contains(executingDirectory))      // also add the current executing directory (needed when running from standalone O2 module installs)
                directoriesWithSourceAssemblies.Add(executingDirectory);   
            return directoriesWithSourceAssemblies;
            //var directoryWithSourceAssemblies = DI.hardCodedO2DeploymentDir;
        }


        public static List<string> getListOfDependenciesForAssemblies(List<String> assemblies, List<String> directoriesWithSourceAssemblies)
        {
            var calculatedAssembliesDictionary = new CecilAssemblyDependencies(assemblies, directoriesWithSourceAssemblies).calculateDependencies();
            return new List<string>(calculatedAssembliesDictionary.Values);
        }

        

        public static string populateDirectoryWithAllDependenciesOfAssembly(string assemblyToAnalyze)
        {
            return populateDirectoryWithAllDependenciesOfAssembly(assemblyToAnalyze, "");
        }

        public static string populateDirectoryWithAllDependenciesOfAssembly(string assemblyToAnalyze, string loadDllsFrom)
        {
            var targetDirectory = PublicDI.config.TempFolderInTempDirectory;
            var directoriesWithSourceAssesmblies = getDefaultDirectoriesWithSourceAssemblies(new List<string> {assemblyToAnalyze});
            if (loadDllsFrom!= null)
                directoriesWithSourceAssesmblies.Add(loadDllsFrom);
            populateDirectoryWithAllDependenciesOfAssembly(targetDirectory, assemblyToAnalyze, directoriesWithSourceAssesmblies);
            return targetDirectory;
        }

        public static void populateDirectoryWithAllDependenciesOfAssembly(string targetDirectory, string assemblyToAnalyze, List<string> pathsToFindReferencedDlls)
        {
            if (pathsToFindReferencedDlls == null)
                pathsToFindReferencedDlls = getDefaultDirectoriesWithSourceAssemblies(new List<string> { assemblyToAnalyze });
            Files.checkIfDirectoryExistsAndCreateIfNot(targetDirectory);
            var fileInTargetDirectory = Files.copy(assemblyToAnalyze, targetDirectory);
            copyAssemblyDependenciesToAssemblyDirectory(fileInTargetDirectory, pathsToFindReferencedDlls);
        }
    }
}
