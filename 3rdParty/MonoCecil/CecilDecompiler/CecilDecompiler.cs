// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Text;
using  ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler;
using Mono.Cecil;
using Mono.Cecil.Cil;
using O2.DotNetWrappers.Windows;
using O2.Kernel;
using O2.External.O2Mono.MonoCecil;
using ICSharpCode.ILSpy;

//O2File:API_ILSpy.cs
//O2File:..\MonoCecil\CecilUtils.cs

//O2Ref:ILSpy\Mono.Cecil.dll
//O2Ref:ILSpy\ICSharpCode.Decompiler.dll
//O2Ref:ILSpy\ILSpy.exe
namespace O2.External.O2Mono
{
    public class CecilDecompiler
    {
        public void decompile(AssemblyDefinition assemblyDefinition, string pathToSaveDecompiledSourceCode)
        {
            try
            {
                foreach (TypeDefinition definition in CecilUtils.getTypes(assemblyDefinition))
                {
                    PublicDI.log.alsoShowInConsole = true;
                    StringBuilder builder = new StringBuilder();
                    foreach (MethodDefinition definition2 in CecilUtils.getMethods(definition))
                    {
                        PublicDI.log.info("[{0}]decompiling method: {1}", new object[] { builder.Length, definition2.ToString() });
                        builder.AppendLine(this.getSourceCode(definition2));
                    }
                    if (builder.Length > 0)
                    {
                        Files.WriteFileContent(Path.Combine(pathToSaveDecompiledSourceCode, Files.getSafeFileNameString(definition.Name) + ".cs"), builder.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                PublicDI.log.error("in decompile: {0}", new object[] { exception.Message });
            }
        }

       	public string FormatControlFlowGraph(ControlFlowGraph cfg)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("To Convert");
            /*foreach (InstructionBlock block in cfg.Blocks)
            {
                writer.WriteLine("block {0}:", block.Index);
                writer.WriteLine("\tbody:");
                foreach (Instruction instruction in block)
                {
                    writer.Write("\t\t");
                    InstructionData data = cfg.GetData(instruction);
                    writer.Write("[{0}:{1}] ", data.StackBefore, data.StackAfter);
                    Formatter.WriteInstruction(writer, instruction);
                    writer.WriteLine();
                }
                InstructionBlock[] successors = block.Successors;
                if (successors.Length > 0)
                {
                    writer.WriteLine("\tsuccessors:");
                    foreach (InstructionBlock block2 in successors)
                    {
                        writer.WriteLine("\t\tblock {0}", block2.Index);
                    }
                }
            }*/
            return writer.ToString();
        }

        public string getIL(MethodDefinition method)
        {
            return this.getIL_usingControlFLowGraph(method);
        }

        public string getIL_usingControlFLowGraph(MethodDefinition method)
        {
            try
            {
                //ControlFlowGraph cfg = ControlFlowGraph.Create(method);
                //return this.FormatControlFlowGraph(cfg);
                return "To Implment";
            }
            catch (Exception exception)
            {
                PublicDI.log.error("in CecilDecompiler.getIL :{0} \n\n{1}\n\n", new object[] { exception.Message, exception.StackTrace });
            }
            return "";
        }

        public string getIL_usingRawIlParsing(MethodDefinition methodDefinition)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < methodDefinition.Body.Instructions.Count; i++)
                {
                    Instruction instruction = methodDefinition.Body.Instructions[i];
                    string str = instruction.OpCode.ToString();
                    if (instruction.Operand != null)
                    {
                        str = str + "   ...   " + instruction.Operand;
                    }
                    builder.AppendLine(str);
                }
                return builder.ToString();
            }
            catch (Exception exception)
            {
                PublicDI.log.error("In getIL_usingRawIlParsing :{0} \n\n {1} \n\n", new object[] { exception.Message, exception.StackTrace });
            }
            return "";
        }

        public string getILfromClonedMethod(MethodDefinition methodDefinition)
        {
            return "Not working at the moment";
        }                	
        
		public string getSourceCode(TypeDefinition typeDefinition)
        {
            try
            {
	            var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();
 				decompilationOptions.FullDecompilation = true;
				csharpLanguage.DecompileType(typeDefinition, textOutput, decompilationOptions);
				return textOutput.ToString();
			}
			catch (Exception exception)
            {
                PublicDI.log.error("in getSourceCode: {0}", new object[] { exception.Message });
                return ("Error in creating source code from Type: " + exception.Message);
            }
        }		
		
        public string getSourceCode(MethodDefinition methodDefinition)
        {
            try
            {
	            var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();
 				decompilationOptions.FullDecompilation = true;
				csharpLanguage.DecompileMethod(methodDefinition, textOutput, decompilationOptions);
				return textOutput.ToString();
            	
             /*   ILanguage language = CSharp.GetLanguage(CSharpVersion.V1);
                language.GetWriter(new PlainTextFormatter(writer)).Write(method);
                MemoryStream stream = new MemoryStream();
                StreamWriter writer3 = new StreamWriter(stream);
                language.GetWriter(new PlainTextFormatter(writer3)).Write(method);
                stream.Flush();*/
                
            }
            catch (Exception exception)
            {
                PublicDI.log.error("in getSourceCode: {0}", new object[] { exception.Message });
                return ("Error in creating source code from IL: " + exception.Message);
            }
        }

        public string getSourceCode(string testExe)
        {
            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(testExe);
            if (assembly != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (MethodDefinition definition2 in CecilUtils.getMethods(assembly))
                {
                    string str = this.getSourceCode(definition2);
                    builder.Append(str);
                }
                return builder.ToString();
            }
            return "";
        }

        /* previous version of the code (before corruption and Restore via Reflection)
         *
        public string getSourceCode(string testExe)
        {
            return getSourceCode(AssemblyFactory.GetAssembly(testExe).EntryPoint);
        }

        public string getSourceCode(MethodDefinition method)
        {
            try
            {
                //var controlFlowGraph = ControlFlowGraph.Create(method);

                //var body = method.Body.Decompile(language);

                ILanguage language = CSharp.GetLanguage(CSharpVersion.V1);
                var stringWriter = new StringWriter();
                ILanguageWriter writer = language.GetWriter(new PlainTextFormatter(stringWriter));
                writer.Write(method);


                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream);

                ILanguageWriter writer2 = language.GetWriter(new PlainTextFormatter(streamWriter));

                writer2.Write(method);

                memoryStream.Flush();
                return stringWriter.ToString();
            }
            catch (Exception ex)
            {
                DI.log.error("in getSourceCode: {0}", ex.Message);
                return "Error in creating source code from IL: " + ex.Message;

            }
        }


        public string getIL(MethodDefinition method)
        {
            return getIL_usingControlFLowGraph(method);
            //return getIL_usingRawIlParsing(method);
        }

        public string getIL_usingRawIlParsing(MethodDefinition methodDefinition)
        {
            try
            {
                / * var stringWriter = new StringWriter();
                foreach (Instruction instruction in methodDefinition.Body.Instructions)
                {
                    Formatter.WriteInstruction(stringWriter, instruction);
                    stringWriter.WriteLine();
                }
                return stringWriter.ToString();
                 * * /
                var ilCode = new StringBuilder();
                for (int i = 0; i < methodDefinition.Body.Instructions.Count; i++)
                {
                    Instruction instruction = methodDefinition.Body.Instructions[i];
                    string instructionText = instruction.OpCode.ToString();
                    if (instruction.Operand != null)
                        instructionText += "   ...   " + instruction.Operand;
                    ilCode.AppendLine(instructionText);
                }
                return ilCode.ToString();
                / *
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
                        }* /

                //        return ilCode.ToString();
                //            return "raw Il Parsing";
            }
            catch (Exception ex)
            {
                DI.log.error("In getIL_usingRawIlParsing :{0} \n\n {1} \n\n", ex.Message, ex.StackTrace);
            }
            return "";
        }


        public string getIL_usingControlFLowGraph(MethodDefinition method)
        {
            try
            {
                ControlFlowGraph cfg = ControlFlowGraph.Create(method);

                string ilCode = FormatControlFlowGraph(cfg);
                return ilCode;
            }
            catch (Exception ex)
            {
                DI.log.error("in CecilDecompiler.getIL :{0} \n\n{1}\n\n", ex.Message, ex.StackTrace);
            }
            return "";
        }

        public string FormatControlFlowGraph(ControlFlowGraph cfg)
        {
            //var memoryStream = new MemoryStream();
            var stringWriter = new StringWriter();
            foreach (InstructionBlock block in cfg.Blocks)
            {
                stringWriter.WriteLine("block {0}:", block.Index);
                stringWriter.WriteLine("\tbody:");
                foreach (Instruction instruction in block)
                {
                    stringWriter.Write("\t\t");
                    InstructionData data = cfg.GetData(instruction);
                    stringWriter.Write("[{0}:{1}] ", data.StackBefore, data.StackAfter);
                    Formatter.WriteInstruction(stringWriter, instruction);
                    stringWriter.WriteLine();
                }
                InstructionBlock[] successors = block.Successors;
                if (successors.Length > 0)
                {
                    stringWriter.WriteLine("\tsuccessors:");
                    foreach (InstructionBlock successor in successors)
                    {
                        stringWriter.WriteLine("\t\tblock {0}", successor.Index);
                    }
                }
            }
            //memoryStream.Flush();
            //writer.Flush();
            return stringWriter.ToString();
            // return Encoding.ASCII.GetString(memoryStream.ToArray());
            //return writer.ToString();
        }


        public string getILfromClonedMethod(MethodDefinition methodDefinition)
        {
            return "Not working at the moment";
            //MethodDefinition clonedMethod = methodDefinition.Clone();
            //return getIL(clonedMethod);


            //return getIL_usingRawIlParsing(methodDefinition);
            //return "this is cloned IL";
        }
         */
    }
}