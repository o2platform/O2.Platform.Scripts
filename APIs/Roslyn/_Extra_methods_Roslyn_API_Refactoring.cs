// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq; 
using System.Reflection;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using Roslyn.Services;

//O2File:_Extra_methods_Roslyn_API.cs

//O2Ref:O2_FluentSharp_Roslyn.dll

//O2Ref:Roslyn.Services.dll
//O2Ref:Roslyn.Services.CSharp.dll
//O2Ref:Roslyn.Compilers.dll
//O2Ref:Roslyn.Compilers.CSharp.dll
//O2Ref:Roslyn.Utilities.dll

namespace O2.XRules.Database.APIs
{	
	
	public static class _Extra_methods_Roslyn_API_Refactoring
	{
		public static string refactor_InitializerExpressions(this string originalCode)
		{
			try
			{
				var ast = originalCode.ast_Script(); 
				var root = ast.GetRoot();   
				
				var node = root.node<InitializerExpressionSyntax>();
				
				var original_StatementSyntax = node.parent<StatementSyntax>().str();
				
				var new_StatementSyntax = node.parent<StatementSyntax>().ReplaceNode(node,null).code().trim();
				var variableName = node.parent<VariableDeclarationSyntax>().Variables.first().Identifier.ValueText;
								
				var parentStatement = node.parent<StatementSyntax>();
				var parentBlock = node.parent_Block();
				var statements = parentBlock.Statements;
				var indexOf = statements.IndexOf(parentStatement);
				
				var assignExpressions = node.nodes(SyntaxKind.AssignExpression);
				if (assignExpressions.size() > 0 )
				{							
					foreach(var assignExpression in assignExpressions)
					{ 
						var newStatement = "{0}.{1};".line().format(variableName,assignExpression.str().trim()).statement();
						statements =  statements.Insert(indexOf+1, newStatement);
					}
					//var newStatement = "var a = 122;".line().statement();
										
				}
				else // assume that they are colection innitializers
				{
					foreach(var collectionInitializer in node.nodes())
					{ 
						var newStatement = "{0}.Add({1});".line().format(variableName,collectionInitializer.str().trim()).statement();
						statements =  statements.Insert(indexOf+1, newStatement);
					}
				}
				parentBlock = parentBlock.Update(parentBlock.OpenBraceToken, statements, parentBlock.CloseBraceToken);
										
				var refactoredCode = parentBlock.str()
												.replace(original_StatementSyntax,new_StatementSyntax.line())
												.replace("".line().add(";"),";");
					
				return refactoredCode;

				//var collectionInitializer = node.nodes(SyntaxKind.LiteralExpreInitializerExpression);
				//return "got {0}".format(collectionInitializer.size());
				return originalCode;
			}
			catch(Exception ex)
			{
				ex.log();
				return null;
			}
		}
	}
}