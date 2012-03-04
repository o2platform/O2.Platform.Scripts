// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using Jint;
using Jint.Native;
using Jint.Debugger;
using Jint.Expressions;

//O2Ref:Jint.dll

namespace O2.XRules.Database.Languages_and_Frameworks.Javascript
{
	//Jint Visitors
	
	public class CalculateMethodSignature : Jint_Visitor
	{
		public string Name {get;set;}
		public string Class {get;set;}
		public List<Statement> Arguments {get;set;}
		
		public CalculateMethodSignature(IStatementVisitor Jint_Visitor)
		{
		
		}
	}
	
	public class Jint_Visitor : IStatementVisitor
	{
		public Statement RootStatement { get; set; }
		public bool LogVisit { get; set; }
		public List<Statement> Statements { get; set; }		
		public Dictionary<string, List<Statement>> Statements_byType { get; set; }		
		public Dictionary<Statement,Statement> PreviousMappings {get;set;}
		
		private Statement PreviousObject {get;set;}
		
		public Jint_Visitor()
		{
			LogVisit = false;
			Statements = new List<Statement> ();	
			Statements_byType = new Dictionary<string, List<Statement>>();
			PreviousMappings = new Dictionary<Statement,Statement>();
		}
		
		public void logVisit(Statement statement)
		{	
			if (LogVisit)			
				"[DefaultVisitor][Statement] {0}".info(statement.typeName());
			Statements.Add(statement);			
			Statements_byType.add(statement.typeName(), statement);				
			PreviousMappings.Add(statement, PreviousObject);					
			PreviousObject = statement;
		}
		
		public virtual void Visit(Program program) 
		{
			logVisit(program);
			foreach (var current in program.ReorderStatements())			
				current.Accept(this);			
		}		
		public virtual void Visit(AssignmentExpression expression) 
		{
			logVisit(expression);
		}
		public virtual void Visit(BlockStatement expression) { logVisit(expression); }
		public virtual void Visit(BreakStatement expression) { logVisit(expression); }
		public virtual void Visit(ContinueStatement expression) { logVisit(expression); }
		public virtual void Visit(DoWhileStatement expression) { logVisit(expression); }
		public virtual void Visit(EmptyStatement expression) { logVisit(expression); }
		public virtual void Visit(ExpressionStatement expression) 
		{
			logVisit(expression); 
			expression.Expression.Accept(this);
		}
		public virtual void Visit(ForEachInStatement expression) { logVisit(expression); }
		public virtual void Visit(ForStatement expression) { logVisit(expression); }
		public virtual void Visit(FunctionDeclarationStatement expression){ logVisit(expression); }
		public virtual void Visit(IfStatement expression) { logVisit(expression); }
		public virtual void Visit(ReturnStatement expression) { logVisit(expression); }
		public virtual void Visit(SwitchStatement expression) { logVisit(expression); }
		public virtual void Visit(WithStatement expression) { logVisit(expression); }
		public virtual void Visit(ThrowStatement expression) { logVisit(expression); }
		public virtual void Visit(TryStatement expression) { logVisit(expression); }
		public virtual void Visit(VariableDeclarationStatement expression) { logVisit(expression); }
		public virtual void Visit(WhileStatement expression) { logVisit(expression); }
		public virtual void Visit(ArrayDeclaration expression) { logVisit(expression); }
		public virtual void Visit(CommaOperatorStatement expression) { logVisit(expression); }
		public virtual void Visit(FunctionExpression expression) { logVisit(expression); }
		public virtual void Visit(MemberExpression expression) 		
		{
			logVisit(expression);
			if (expression.Previous != null)				
				expression.Previous.Accept(this);			
			expression.Member.Accept(this);
		}
		public virtual void Visit(MethodCall expression) 		
		{
			logVisit(expression);
			foreach(var argument in expression.Arguments)
				argument.Accept(this);
		}
		public virtual void Visit(Indexer expression) { logVisit(expression); }
		public virtual void Visit(PropertyExpression expression) { logVisit(expression); }
		public virtual void Visit(PropertyDeclarationExpression expression) { logVisit(expression); }
		public virtual void Visit(Identifier expression) { logVisit(expression); }
		public virtual void Visit(JsonExpression expression) 
		{ 
			logVisit(expression); 
			foreach(var item in expression.Values)
				item.Value.Accept(this); 
		}
		public virtual void Visit(NewExpression expression) { logVisit(expression); }
		public virtual void Visit(BinaryExpression expression) { logVisit(expression); }
		public virtual void Visit(TernaryExpression expression) { logVisit(expression); }
		public virtual void Visit(UnaryExpression expression) { logVisit(expression); }
		public virtual void Visit(ValueExpression expression) { logVisit(expression); }
		public virtual void Visit(RegexpExpression expression) { logVisit(expression); }
		public virtual void Visit(Statement expression) { logVisit(expression); }
	}
	
	
	public static class DefaultVisitor_ExtensionMethods
	{
		public static Jint_Visitor map<T>(this Jint_Visitor jintVisitor, T statement)
			where T : Statement
		{
			jintVisitor.RootStatement = statement;
			statement.Accept(jintVisitor);
			return jintVisitor;
		}
		
		public static Statement previous(this Jint_Visitor jintVisitor, Statement startStatement)
		{
			var match = jintVisitor.previous(startStatement, false);
			if (match.notNull() && match.size()>0)
				return match[0];
			return null;
		}
		public static List<Statement> previous(this Jint_Visitor jintVisitor, Statement startStatement, bool recursiveSearch)
		{
			if (recursiveSearch)
				return jintVisitor.previous_All(startStatement);
				
			if (jintVisitor.PreviousMappings.hasKey(startStatement))
				return jintVisitor.PreviousMappings[startStatement].wrapOnList();;
			return null;
		}
		
		public static List<T> previous<T>(this Jint_Visitor jintVisitor, Statement startStatement)
			where T : Statement	
		{
			return jintVisitor.previous_All<T>(startStatement);
		}
		
		public static List<Statement> previous_All(this Jint_Visitor jintVisitor, Statement startStatement)
		{
			return jintVisitor.previous_All<Statement>(startStatement);
		}
		
		public static List<T> previous_All<T>(this Jint_Visitor jintVisitor, Statement startStatement)
			where T : Statement
		{
			return jintVisitor.previous_All<T>(startStatement, null);
		}
		
		public static List<Statement> previous_All_Until<T1>(this Jint_Visitor jintVisitor, Statement startStatement)		
			where T1 : Statement
		{
			return jintVisitor.previous_All<Statement>(startStatement, typeof(T1));
		}
		
		public static List<T> previous_All_Until<T, T1>(this Jint_Visitor jintVisitor, Statement startStatement)
			where T : Statement
			where T1 : Statement
		{
			return jintVisitor.previous_All<T>(startStatement, typeof(T1));
		}
		
		public static List<T> previous_All<T>(this Jint_Visitor jintVisitor, Statement startStatement, Type untilStatement)
			where T : Statement
		{
			var allPrevious = new List<T>();
			var previous = jintVisitor.previous(startStatement);
			while (previous.notNull())
			{
				if (untilStatement.notNull() && previous.type() == untilStatement)
					break;
				if (previous is T)
					allPrevious.add((T)previous);
				previous = jintVisitor.previous(previous);
			}
			return allPrevious;			
		}
		
		public static List<MethodCall> methods(this Jint_Visitor jintVisitor)
		{
			if (jintVisitor.Statements_byType.hasKey("MethodCall"))
				return jintVisitor.Statements_byType["MethodCall"].Cast<MethodCall>().toList();
			return new List<MethodCall>();
		}
		
		public static string name(this Jint_Visitor jintVisitor, MethodCall methodCall)
		{
			var previous = jintVisitor.previous(methodCall);
			return previous.notNull()
					? previous.str()
					: null;			
		}
	}
}
	