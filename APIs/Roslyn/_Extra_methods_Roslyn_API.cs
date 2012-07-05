// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq; 
using System.Reflection;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using Roslyn.Services;

//O2Ref:O2_FluentSharp_Roslyn.dll

//O2Ref:Roslyn.Services.dll
//O2Ref:Roslyn.Services.CSharp.dll
//O2Ref:Roslyn.Compilers.dll
//O2Ref:Roslyn.Compilers.CSharp.dll
//O2Ref:Roslyn.Utilities.dll

namespace O2.XRules.Database.APIs
{	
	
	public static class _Extra_methods_Roslyn_API_StatementSyntax
	{
		public static StatementSyntax statement(this string code)
		{
			return Syntax.ParseStatement(code);
		}
	}		
	
	  
	public static class _Extra_methods_Roslyn_API_ClassDeclaration
	{			
		public static ClassDeclarationSyntax classDeclaration(this string className, bool addEmptyBody = true)
		{
			var classDeclaration = Syntax.ClassDeclaration(className);
	//		if (addEmptyBody)
	//			return classDeclaration.add_Body();
			return classDeclaration;
		}
	
		public static List<ClassDeclarationSyntax> classes(this CompilationUnitSyntax compilationUnit)
		{
			return compilationUnit.ChildNodes()
							      .OfType<ClassDeclarationSyntax>().toList();
		}
		
		public static CompilationUnitSyntax replace(this CompilationUnitSyntax compilationUnit, ClassDeclarationSyntax classA, ClassDeclarationSyntax classB)
		{
			return compilationUnit.ReplaceNode(classA, classB);
		}
		
		public static CompilationUnitSyntax replace(this ClassDeclarationSyntax classA, ClassDeclarationSyntax classB)
		{
			return classA.compilationUnit().ReplaceNode(classA, classB);
		}
		
		public static CompilationUnitSyntax replaceWith(this ClassDeclarationSyntax classA, ClassDeclarationSyntax classB)
		{
			return classA.replace(classB);
		}
		
		public static ClassDeclarationSyntax add(this ClassDeclarationSyntax classDeclaration, MethodDeclarationSyntax methodDeclaration)
		{
			return classDeclaration.AddMembers(methodDeclaration);
		}
		
		public static ClassDeclarationSyntax add_Method(this ClassDeclarationSyntax classDeclaration, string methodName, string returnType = null)
		{			
			return classDeclaration.AddMembers(methodName.methodDeclaration(returnType));			
		}

	}
	
	public static class _Extra_methods_Roslyn_API_MethodDeclaration
	{
		public static List<MethodDeclarationSyntax> methods(this ClassDeclarationSyntax classDeclarationSyntax)
		{
			return classDeclarationSyntax.ChildNodes()
							      		 .OfType<MethodDeclarationSyntax>().toList();
		}
		
		public static MethodDeclarationSyntax methodDeclaration(this string methodName, string returnType = null, bool addEmptyBody = true)
		{
			return methodName.methodDeclaration(returnType.parse_TypeName(), addEmptyBody);
		}
		
		public static MethodDeclarationSyntax methodDeclaration(this string methodName, TypeSyntax returnType, bool addEmptyBody = true)
		{
			var methodDeclaration = Syntax.MethodDeclaration(returnType, methodName);
			if (addEmptyBody)
				return methodDeclaration.add_Body();
			return methodDeclaration;
		}		
		
		public static MethodDeclarationSyntax add_Body(this MethodDeclarationSyntax methodDeclaration)
		{
			return methodDeclaration.WithBody(Syntax.Block());        
		}
		
		public static MethodDeclarationSyntax set_Body(this MethodDeclarationSyntax methodDeclaration, string code)
		{
			return methodDeclaration.WithBody(code.parse_Block());
		}		

        public static MethodDeclarationSyntax add_Statement(this MethodDeclarationSyntax methodDeclaration, string code)
		{
			return methodDeclaration.add_Statement(code.statement());
		}
		
		public static MethodDeclarationSyntax add_Statement(this MethodDeclarationSyntax methodDeclaration, StatementSyntax statement)
		{
			return methodDeclaration.AddBodyStatements(statement);				 
		}
		

	}
	
	public static class _Extra_methods_Roslyn_API_SyntaxTree
	{
		
		public static SyntaxTree parse_Script(this string code)
		{
			var parserOptions =  ParseOptions.Default.WithKind(SourceCodeKind.Script);
			return SyntaxTree.ParseCompilationUnit(code, "",parserOptions);
		}
		
		public static SyntaxTree ast(this string code)
		{
			return code.tree();
		}
		
		public static SyntaxTree astTree(this string code)
		{
			return code.tree();
		}
		
		public static SyntaxNode root(this SyntaxTree tree)
		{
			return tree.GetRoot();
		}
		 
		
		public static bool hasErrors(this SyntaxTree tree)
		{
			return tree.errors().size() > 0;
		}
		
		public static Assembly create_Assembly(this SyntaxTree tree)
		{
			return tree.compiler().create_Assembly();
		}
	}
	
	
	
	public static class _Extra_methods_Roslyn_API_CompilationUnit
	{
	
		public static CompilationUnitSyntax add_Class(this CompilationUnitSyntax compilationUnit, ClassDeclarationSyntax classDeclaration)
		{			
			return compilationUnit.AddMembers(classDeclaration);
		}
		
		public static CompilationUnitSyntax compilationUnit(this SyntaxTree tree)
		{
			return (CompilationUnitSyntax)tree.GetRoot();
		}
		
		public static CompilationUnitSyntax compilationUnit(this SyntaxNode syntaxNode)
		{
			return syntaxNode.parent<CompilationUnitSyntax>();
		}
		
		public static CompilationUnitSyntax add_Using(this CompilationUnitSyntax compilationUnit, string @using)
		{
			return compilationUnit.AddUsings(Syntax.UsingDirective(@using.parse_Name()));
		}
	}
	
	public static class _Extra_methods_Roslyn_API_Compiler
	{
		public static List<Assembly> compileSolution(this string solutionFile, bool createUniqueAssemblies = true)
		{			
			var workspace = Workspace.LoadSolution(solutionFile);
			var solution = workspace.CurrentSolution;
			return solution.compileSolution(createUniqueAssemblies);
		}
		
		public static List<Assembly> compileSolution(this ISolution solution,  bool createUniqueAssemblies = true)
		{
			var compiledDlls = new List<Assembly>();
			foreach(var project in solution.Projects)
			{
				var assembly = project.compile_And_ReturnAssembly(createUniqueAssemblies);
				
				if (assembly.notNull())
					compiledDlls.Add(assembly);
			}			
			return compiledDlls;
		}
		
		public static Assembly compile_And_ReturnUniqueAssembly(this IProject project)
		{
			return project.compile_And_ReturnAssembly(true);
		}
		
		public static Assembly compile_And_ReturnUniqueAssembly(this IProject project, Action<List<CommonDiagnostic>> onErrors)
		{
			return project.compile_And_ReturnAssembly(onErrors,true);
		}
		
		
		public static Assembly compile_And_ReturnAssembly(this IProject project, bool createUniqueAssemblies = true)
		{
			Action<List<CommonDiagnostic>>  onErrors = 
				(errors)=> "{0}".error(errors.asString());
			
			return project.compile_And_ReturnAssembly(onErrors, createUniqueAssemblies);
		}
		
		public static Assembly compile_And_ReturnAssembly(this IProject project, Action<List<CommonDiagnostic>> onErrors , bool createUniqueAssemblies = true)
		{
			var compilation = project.GetCompilation();		
			if (compilation.hasErrors())
			{				
				var errors = compilation.errors();						
				"There are {0} errors".info(errors.size());					
				if (onErrors.notNull())
					onErrors(errors);
				return null;
			}		
			if (createUniqueAssemblies)
				compilation.makeAssemblyNameUnique();
			return compilation.create_Assembly_IntoDisk();			
		}
		
		public static CommonCompilation makeAssemblyNameUnique(this CommonCompilation compilation)
		{
				//make the assembly unique (or we wont be able to load this assembly more than one per process
			compilation.Assembly.field("baseNameNoExtension", compilation.Assembly.field("baseNameNoExtension").str().add_RandomLetters(5));
			return compilation;
		}
		
		public static string createExe(this string code, bool force32bit = false)
		{
			var targetFile = ".exe".tempFile();
			return code.createExe(targetFile, force32bit);
		}
		
		public static string createExe(this string code, string targetFile, bool force32bit = false)
		{	
			if (code.ast().hasErrors())
			{
				code.ast().errors_Details().error();
				return null;
			}
							
			var compiler =  code.ast().compiler().as_ConsoleApp();
						
			if (compiler.hasErrors())		
			{
				compiler.errors_Details().error();
				return null;
			}			
			if (force32bit)
				compiler.set_Platform_x86();
			var assembly = compiler.create_Assembly(targetFile);
			if (assembly.notNull())
				return assembly.Location;			
			return null;
		}
		public static string compile_Script_in_Separate_Folder(this string h2Script)
		{
			var assembly = h2Script.compile_H2Script(); 

			var exe = h2Script.compile_Script_into_Exe(); 
			if (exe.notNull())
			{
				var exeLocation = exe.Location; 
				var fileName = h2Script.fileName_WithoutExtension(); 
				var targetDir = exeLocation.fileName_WithoutExtension().tempDir(false); 
				var fixedName = targetDir.pathCombine("{0}.exe".format(fileName));
			  
			
				var dependencies = exe.referencedAssemblies(true);
				//return dependencies;
				dependencies.locations().files_Copy(targetDir);
				
				var exeInTargetDir = exeLocation.file_Copy(fixedName);
				return fixedName;
			}
			return null;
		}
						
		public static Assembly compile_Script_into_Exe(this string scriptFile, bool withRandomName = true)
		{
			//var compiledH2Script = (script.extension("H2.compile_H2Script(); 
			var compiledScript = scriptFile.compileScriptFile();
			if (compiledScript.isNull())
			{
				"[compile_H2Script_into_Exe] failed to compile H2 Script:{0}".error(scriptFile); 
				return null;
			}
			var code =
			@"using System;
			
			public class mainInvoker
			{
				public static void Main(string[] args)
				{					
					new  DynamicType().dynamicMethod();					
				}
			}";
			
			if (code.ast().hasErrors())
			{
				code.ast().errors_Details().error();
				return null;
			}
			var assemblyName = scriptFile.fileName_WithoutExtension();
			if (withRandomName)			//note: with this the created assembly can only be loaded once in this app domain
				assemblyName = "{0} [{1}]".format(assemblyName, 5.randomNumbers());
							
			var tempExe = "".tempDir().pathCombine("{0}.exe".format(assemblyName));
			var compiler = code.ast().compiler(assemblyName,false)
									 .add_Reference(compiledScript.Location);		
			//if (withRandomName.isFalse())
			//	compiler = (Compilation)compiler.makeAssemblyNameUnique();	//not working
			if (compiler.hasErrors())		
			{
				compiler.errors_Details().error();
				return null;
			}			
			var compilationOptions = new CompilationOptions(OutputKind.WindowsApplication);						
			compiler.set_Options(compilationOptions);
			if (clr.x86())
				compiler.set_Platform_x86();
			var assembly = compiler.create_Assembly(tempExe);
			if (assembly.isNull())			 
				"[compile_H2Script_into_Exe] Failed to compile assembly".error();
			return assembly;
		}
		
		public static CommonCompilation set_Options(this CommonCompilation compiler, CompilationOptions compilationOptions)
		{
			compiler.field("options", compilationOptions);
			return compiler;
		}

		public static CommonCompilation set_Platform_x86(this CommonCompilation compiler)
		{			
			var options = (compiler.Options as CompilationOptions).WithPlatform(Roslyn.Compilers.Common.Platform.x86);
			return compiler.set_Options(options);
		}
		
		public static bool hasErrors(this CommonCompilation compilation)
		{
			return compilation.errors().size() > 0;	
		}
		
		
		public static T as_ConsoleApp<T>(this T compiler) where T : CommonCompilation
		{
			return compiler.outputKind(OutputKind.ConsoleApplication);
		}
		
		public static T as_WindowsApp<T>(this T compiler) where T : CommonCompilation
		{
			return compiler.outputKind(OutputKind.WindowsApplication);
		}
		
		public static T as_Dll<T>(this T compiler) where T : CommonCompilation
		{
			return compiler.outputKind(OutputKind.DynamicallyLinkedLibrary);
		}
		
		public static T outputKind<T>(this T compiler, OutputKind outputKind) where T : CommonCompilation
		{
			var compilationOptions = new CompilationOptions(outputKind);
			//compiler.Options = compilationOptions;
			compiler.field("options", compilationOptions);
			return compiler;
		}
		
		
		public static Compilation compiler(this SyntaxTree tree)
		{
			return tree.compiler(7.randomLetters());
		}				
		
		public static object invokeFirstMethod(this string code)
		{
			var tree = code.tree();
			if (tree.hasErrors())
			{
				"[invokeFirstMethod] ast errors: {0}".error(tree.errors_Details());
				return null;
			}
			var compiler = 	tree.compiler();
			return compiler.invokeFirstMethod();
		}
		
		public static object invokeFirstMethod(this CommonCompilation compiler)
		{
			if (compiler.hasErrors())
			{
				"[invokeFirstMethod] compiler errors: {0}".error(compiler.errors_Details());
				return null;
			}
			
			var assembly = compiler.create_Assembly();
			if (assembly.isNull())
			{
				"[invokeFirstMethod] failed to create assembly".error();
				return null;
			}
			return assembly.types()
						   .methods()
						   .first()
						   .invoke();
		}					  
	}
	
	
	public static class _Extra_methods_Roslyn_API_other
	{		
				
		public static BlockSyntax parse_Block(this string code)
		{
			var wrapInBrackets = "{".line() + code + "}";
			return (BlockSyntax)wrapInBrackets.statement();
		}
		
		
		public static T parent<T>(this SyntaxNode syntaxNode)
			where T : SyntaxNode
		{
			if (syntaxNode.notNull())
			{
				if (syntaxNode.Parent is T)
					return (T)syntaxNode.Parent;
				return syntaxNode.Parent.parent<T>();
			}
			return default(T);
		}		
		
		
		public static NameSyntax parse_Name(this string name)
		{		
			return Syntax.ParseName(name);
		}		
		
		public static TypeSyntax parse_TypeName(this string name)
		{		
			return Syntax.ParseTypeName(name ?? "void");
		}
		
					
		public static string code(this SyntaxNode syntaxNode)
		{
			return syntaxNode.formatedCode();
		}
		
	}
	
}
    	