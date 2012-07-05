// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using O2.API.AST.CSharp;
using ICSharpCode.NRefactory.Ast;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using ICSharpCode.SharpDevelop.Dom;
using O2.API.AST.Visitors;
using O2.API.AST.ExtensionMethods;
using O2.API.AST.ExtensionMethods.CSharp;

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class Ast_Engine_Cache
    {    
    	public static Dictionary<string, O2MappedAstData> Cached_O2MappedAstData { get; set;}
    	public static bool CacheEnabled { get; set; }	
 		static Ast_Engine_Cache()
 		{ 			
 			Cached_O2MappedAstData = new Dictionary<string, O2MappedAstData>();
 			CacheEnabled = true;
 		}
 		
 		public static Dictionary<string, O2MappedAstData> cache_AstData()
 		{
 			return Cached_O2MappedAstData;
 		}
 		
 		public static Dictionary<string, O2MappedAstData> clear()
 		{
 			"[Ast_Engine_Cache] clearing cache".info();
 			Cached_O2MappedAstData.Clear();
 			return Cached_O2MappedAstData;
 		}
 		
 		public static O2MappedAstData get(string file)
 		{
 			if (CacheEnabled && Cached_O2MappedAstData.hasKey(file))
 			{
// 				"[Ast_Engine_Cache]  using O2MappedAstData cached version of file: {0}".debug(file);
 				return Cached_O2MappedAstData[file];
 			}
// 			"[Ast_Engine_Cache]  creating O2MappedAstData for file: {0}".debug(file);
			var astData = new O2MappedAstData();
			astData.loadFile(file);
			if (CacheEnabled)
				Cached_O2MappedAstData.add(file, astData);
			return astData;	
 		}
    }
}
