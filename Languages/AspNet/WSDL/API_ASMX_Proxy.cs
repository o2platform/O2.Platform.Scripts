// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CSharpAST;
using FluentSharp.CoreLib;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_ASMX_Proxy
	{
		public CompilationUnit 	Proxy_CompilationUnit 			{ get; set; }
		public string 		 	WebRoot_Folder 					{ get; set; }
		public string 			AppCode_Folder					{ get; set; }
		public string 		 	Wsdl_CS_File_OriginalLocation	{ get; set; }
		public string 		 	Wsdl_CS_File_In_AppCode			{ get; set; }
		public string 		 	Proxy_Wrapper_CS_File			{ get; set; }
		public string 		 	AsmxFile						{ get; set; }
		public string 		 	WebServiceNamespace				{ get; set; }
		public TypeDeclaration 	WrapperType						{ get; set; }
		public string 		 	PropName						{ get; set; }
		public string 		 	CSharpCode						{ get; set; }
		public IParser			FileAst 						{ get; set; }
		public TypeDeclaration 	SourceClass;
		
		public API_ASMX_Proxy()
		{
			SetUp();
		}
		
		public API_ASMX_Proxy SetUp()
		{
			PropName 			  = "_web_Service";
			Proxy_CompilationUnit = new CompilationUnit();			
			Proxy_CompilationUnit.add_Using("System");
			Proxy_CompilationUnit.add_Using("System.Web.Services"); 			
			return this;
		}	
	}
		
	
	
	
	public static class API_ASMX_Proxy_ExtensionMethods
	{
		public static API_ASMX_Proxy set_Target_Values_and_Folders(this API_ASMX_Proxy asmxProxy, string webRoot_Folder, string wsdl_CS_File, string webServiceNamespace, string asmxFileName)
		{
			asmxProxy.Wsdl_CS_File_OriginalLocation = wsdl_CS_File;	
			asmxProxy.WebRoot_Folder 				= webRoot_Folder;
			asmxProxy.AppCode_Folder 				= asmxProxy.WebRoot_Folder.pathCombine("App_Code").createDir();
			asmxProxy.Wsdl_CS_File_In_AppCode 		= asmxProxy.Wsdl_CS_File_OriginalLocation.file_Copy(asmxProxy.AppCode_Folder);
			asmxProxy.Proxy_Wrapper_CS_File			= asmxProxy.AppCode_Folder.pathCombine("_WebMethod_" + wsdl_CS_File.fileName());
			asmxProxy.AsmxFile 						= asmxProxy.WebRoot_Folder.pathCombine(asmxFileName);
			asmxProxy.WebServiceNamespace			= webServiceNamespace;
			return asmxProxy;
		}
		
		public static API_ASMX_Proxy create_Proxy_Files_For_WSDL(this API_ASMX_Proxy asmxProxy, string webRoot_Folder, string wsdl_CS_File, string webServiceNamespace, string asmxFileName)
		{
			return  asmxProxy.set_Target_Values_and_Folders(webRoot_Folder, wsdl_CS_File, webServiceNamespace, asmxFileName)
							 .create_Wrapper_SourceCodeFile()
							 .createAsmxFile(asmxProxy.WrapperType.name(), asmxProxy.AsmxFile);
				  	  		 
		}
		
		public static API_ASMX_Proxy add_Property_To_Type_Constructor(this API_ASMX_Proxy asmxProxy, TypeDeclaration targetType, string propertyName, TypeDeclaration propertyType)
		{
			targetType.add_Property(propertyName,propertyType)	
					  .add_Ctor()								
					  .body()									
					  .add_Assignment(propertyName, propertyType);
			return asmxProxy;
		}
 
	 	public static MethodDeclaration create_ProxyMethod(this API_ASMX_Proxy asmxProxy, MethodDeclaration methodToProxy)
		{			
		 	var webServicesObjectName 	= asmxProxy.PropName;		 	
			var proxyMethod 			= methodToProxy.clone()
													   .remove_Attributes()
													   .add_Attribute("WebMethod");
			var body 					= proxyMethod.add_Body();					
			
			var parameters = methodToProxy.parameters().names().ast_Identifiers().ToArray();
			var invocation = webServicesObjectName.ast_Invocation_onType(methodToProxy.Name,parameters);
			
			if(proxyMethod.TypeReference.name() == "void" || proxyMethod.TypeReference.name() == "System.Void")
			{
				body.append(invocation.expressionStatement());
			}
			else
			{
				var result = body.add_Variable("result", invocation, methodToProxy.TypeReference);
				body.add_Return(result.Name.ast_Identifier());
			}
			return proxyMethod;
		 }
		
		public static API_ASMX_Proxy create_Wrapper_CSharpCode(this API_ASMX_Proxy asmxProxy)	
		{
			var wsClass 			= asmxProxy.SourceClass;
			var wsFileName 			= asmxProxy.Wsdl_CS_File_In_AppCode.fileName();
			var compilationUnit 	= asmxProxy.Proxy_CompilationUnit;
			asmxProxy.WrapperType  	= compilationUnit.add_Type(wsClass.Name + "_Wrapper");
			
			if (asmxProxy.WebServiceNamespace.valid())
				asmxProxy.WrapperType.add_Attribute("WebService", "Namespace",asmxProxy.WebServiceNamespace);
			
			asmxProxy.add_Property_To_Type_Constructor(asmxProxy.WrapperType,asmxProxy.PropName, wsClass);
			
			var wsMethods = wsClass.methods_with_Attribute("System.Web.Services.Protocols.SoapDocumentMethodAttribute");
			
			foreach(var wsMethod in wsMethods)			 
				asmxProxy.WrapperType.append(asmxProxy.create_ProxyMethod(wsMethod));  				
				
			asmxProxy.CSharpCode = compilationUnit.csharpCode()
												   .insertAt(0, @"//O2File:{0}".format(wsFileName).line());;
			return asmxProxy;
		}
		public static API_ASMX_Proxy create_Wrapper_SourceCodeFile(this API_ASMX_Proxy asmxProxy)
		{
			var sourceFile 			= asmxProxy.Wsdl_CS_File_In_AppCode;
			var targetFile 			= asmxProxy.Proxy_Wrapper_CS_File;
			asmxProxy.FileAst 		= sourceFile.csharpAst();
			asmxProxy.SourceClass 	= asmxProxy.FileAst.type_with_BaseType("System.Web.Services.Protocols.SoapHttpClientProtocol");  
			asmxProxy.create_Wrapper_CSharpCode()
					 .CSharpCode.saveAs(targetFile);
			return asmxProxy;
		}

	
		public static API_ASMX_Proxy createAsmxFile(this API_ASMX_Proxy asmxProxy, string className, string targetFile)
		{
			var asmxCodeContent = @"<%@ WebService Language=""c#"" Class=""{0}"" %>"
									   .format(className);
			asmxCodeContent.saveAs(targetFile);	
			return asmxProxy;
		}
	}
}