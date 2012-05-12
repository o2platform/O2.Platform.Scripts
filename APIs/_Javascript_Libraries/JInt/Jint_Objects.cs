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

//O2File:Jint_Visitors.cs
//O2Ref:Jint.dll

//Jint Objects
namespace O2.XRules.Database.Languages_and_Frameworks.Javascript
{	
	
	public class Jint_Function
	{
		public string Name { get; set; }
		public string Class { get; set; }
		public List<Expression> Arguments { get; set;}
		
		public Jint_Function()
		{
			Name = "";
			Class = "";
			//Arguments = new List<string>();
		}
	}
			
	public static class Jint_Function_ExtensionMethods
	{
		public static Jint_Function jintFunction(this Jint_Visitor jintVisitor, MethodCall methodCall)
		{
			var jintFunction = new Jint_Function();			
			var previous = jintVisitor.previous_All<Identifier>(methodCall);     
			if (previous.size()>0)
			{
				jintFunction.Name = previous.first().Text;
				previous.Reverse();  
				var className = "";				
				foreach(var item in previous)
					className= "{0}.{1}".format(className, item.Text);
				className = className.removeFirstChar(); 				 				
				jintFunction.Class = className;
			}
			else
				jintFunction.Name = "[anonymous]";
			jintFunction.Arguments = methodCall.Arguments;
			//foreach(var argument in methodCall.Arguments)
			//	jintFunction.Arguments.Add(argument.str()); 
			return jintFunction;
		}
	}
}