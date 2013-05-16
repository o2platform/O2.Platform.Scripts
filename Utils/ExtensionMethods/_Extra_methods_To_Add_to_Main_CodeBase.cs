// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;   
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Forms; 
using System.Collections;   
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Linq;
using System.Xml.Linq;  
using System.Reflection; 
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;
using O2.Interfaces.O2Core;
using O2.Interfaces.O2Findings;
using O2.Kernel;
using O2.Kernel.Objects;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.H2Scripts;
using O2.DotNetWrappers.Zip;
using O2.Views.ASCX;
using O2.Views.ASCX.CoreControls;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.Platform.BCL.O2_Views_ASCX;
using O2.External.SharpDevelop.AST;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.XRules.Database.Utils;
using O2.API.AST.ExtensionMethods.CSharp;
using ICSharpCode.TextEditor;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast; 
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using System.CodeDom;

using O2.Views.ASCX.O2Findings;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.Forms;
using System.Security.Cryptography;

using Ionic.Zip;
//O2Ref:Ionic.Zip.dll


namespace O2.XRules.Database.APIs
{	

	// O2.API.AST.ExtensionMethods.CSharp.MethodDeclaration_ExtensionMethods
	public static class Extra_Objects_ExtensionMethods
	{
		public static T clone<T>(this T objectToClone)
		{
			try
			{
				if (objectToClone.isNull())
					"[object<T>.clone] provided object was null (type = {0})".error(typeof(T));
				else
					return (T)objectToClone.invoke("MemberwiseClone");
			}
			catch(Exception ex)
			{
				"[object<T>.clone]Faild to clone object {0} of type {1}".error(objectToClone.str(), typeof(T));
			}
			return default(T);
		}	
	}
	
	public static class Extra_PrimitiveExpression_ExtensionMethods
	{
		public static PrimitiveExpression ast_Primitive(this string primitiveName)
		{
			return new PrimitiveExpression(primitiveName);
		}
		public static List<PrimitiveExpression> ast_Primitive(this List<string> primitiveNames)
		{
			return primitiveNames.select((name)=> name.ast_Primitive());			
		}
	}
	
	
	public static class Extra_IdentifierExpression_ExtensionMethods
	{
		public static IdentifierExpression ast_Identifier(this string identifierName)
		{
			return new IdentifierExpression(identifierName);
		}
		public static List<IdentifierExpression> ast_Identifiers(this List<string> identifierNames)
		{
			return identifierNames.select((name)=> name.ast_Identifier());			
		}
	}
	
	public static class Extra_AttributedNode_ExtensionMethods
	{		
		public static T remove_Attributes<T>(this T attributedNode) where T : AttributedNode
		{
			attributedNode.Attributes =null;
			return attributedNode;
		}
		public static T add_Attribute<T>(this T attributedNode , string attributeName, string parameterName, string parameterValue)  where T : AttributedNode
		{
			var namedArgument = new NamedArgumentExpression();
			namedArgument.Name = parameterName;
			namedArgument.Expression = parameterValue.ast_Primitive();
			var attribute = new ICSharpCode.NRefactory.Ast.Attribute();
			attribute.Name = attributeName;
			attribute.NamedArguments.Add(namedArgument);
			attributedNode.add_Attribute(attribute);
			return attributedNode;
		}
	}
	
	public static class Extra_MethodDeclaration_ExtensionMethods
	{		
		public static MethodDeclaration ast_Method(this string methodName)
		{
			return new MethodDeclaration()
							.name(methodName)
							.public_Instance()
							.returnType_Void()
							.empty_Body();
		}
			
		public static MethodDeclaration name(this MethodDeclaration methodDeclaration, string methodName)
		{
			methodDeclaration.Name = methodName;
			return methodDeclaration;
		}
		
		public static MethodDeclaration name_Add(this MethodDeclaration methodDeclaration, string textToAdd)
		{
			methodDeclaration.Name += textToAdd;
			return methodDeclaration;
		}
		
		public static MethodDeclaration empty_Body(this MethodDeclaration methodDeclaration)
		{
			return methodDeclaration.body(new BlockStatement());
		}
		public static MethodDeclaration body(this MethodDeclaration methodDeclaration, BlockStatement newBody)
		{
			methodDeclaration.Body = newBody;
			return methodDeclaration;
		}	
	
		public static MethodDeclaration returnType_Void(this MethodDeclaration methodDeclaration)
		{
			return methodDeclaration.setReturnType("void");
		}
		public static MethodDeclaration setReturnType(this MethodDeclaration methodDeclaration, string returnType)
		{
			return methodDeclaration.returnType_using_Keyword(returnType);
		}
		public static MethodDeclaration returnType_using_Keyword(this MethodDeclaration methodDeclaration, string returnType)
		{
			methodDeclaration.TypeReference = new TypeReference(returnType, true);
			return methodDeclaration;
		}
		
		public static MethodDeclaration public_Static(this MethodDeclaration methodDeclaration)
		{
			methodDeclaration.Modifier  = Modifiers.Static | Modifiers.Public;
			return methodDeclaration;
		}
		
		public static MethodDeclaration public_Instance(this MethodDeclaration methodDeclaration)
		{
			methodDeclaration.Modifier  =  Modifiers.Public;
			return methodDeclaration;
		}
		
		public static MethodDeclaration private_Static(this MethodDeclaration methodDeclaration)
		{
			methodDeclaration.Modifier  = Modifiers.Static | Modifiers.Private;
			return methodDeclaration;
		}						
		
		public static List<IdentifierExpression> parameters_Names_as_Indentifiers(this MethodDeclaration methodDeclaration)
		{
			return methodDeclaration.parameters().names().ast_Identifiers();
		}
	}
	public static class Extra_BlockStatement_ExtensionMethods
	{
		public static BlockStatement append_AsStatement(this BlockStatement blockExpression, Expression expression)
		{
			return blockExpression.append(expression.expressionStatement());
		}
				
		public static VariableDeclaration add_Variable_with_NewObject(this BlockStatement blockStatement, string variableName, string typeName)
		{
			return blockStatement.add_Variable_with_NewObject(variableName, typeName.ast_TypeReference());
		}
		
		public static VariableDeclaration add_Variable_with_NewObject(this BlockStatement blockStatement, string variableName, TypeDeclaration typeDeclaration)
		{
			return blockStatement.add_Variable_with_NewObject(variableName, typeDeclaration.Name.ast_TypeReference());
		}
		public static VariableDeclaration add_Variable_with_NewObject(this BlockStatement blockStatement, string variableName, TypeReference typeReference)
		{
			return blockStatement.add_Variable(variableName, typeReference.ast_ObjectCreate(), typeReference);
		}	
	}
	
	public static class Extra_Invocation_ExtensionMethods
	{		
		public static InvocationExpression ast_Invocation_onType(this string typeName, string methodName, params object[] parameters)
		{
			return new BlockStatement().add_Invocation(typeName, methodName, parameters);
		}
		
		public static InvocationExpression ast_Invocation(this  string methodName, params object[] parameters)
		{
			return new BlockStatement().add_Invocation("", methodName, parameters);
		}				
		
		public static InvocationExpression add_Invocation(this BlockStatement blockStatement, VariableDeclaration variableDeclaration, string methodName, params Expression[] arguments)
		{
			return blockStatement.add_Invocation(variableDeclaration.Name, methodName, arguments);
		}
		
		public static InvocationExpression add_Invocation(this InvocationExpression parentInvocation, string methodName, params Expression[] arguments)
		{
			return parentInvocation.toMemberReference(methodName)
					     		   .toInvocation(arguments);  
		}
		public static InvocationExpression toInvocation(this Expression targetObject, params Expression[] arguments)
		{
			return new InvocationExpression(targetObject, arguments.toList());
		}
	}
	public static class Extra_MemberReferenceExpression_ExtensionMethods
	{
		public static MemberReferenceExpression toMemberReference(this Expression targetObject, string memberName)
		{
			return new MemberReferenceExpression(targetObject, memberName);
		}
	}
	 
	public static class Extra_PropertyDeclaration_ExtensionMethods
	{
		public static PropertyDeclaration ast_Property(this string propertyName, string propertyType)
		{
			var modifier = Modifiers.Public;
			var attributes = new List<AttributeSection>();			
			var parameters = new List<ParameterDeclarationExpression>() {};
			
			var propertyDeclaration = new PropertyDeclaration(modifier, attributes, propertyName, parameters);
			propertyDeclaration.TypeReference = propertyType.ast_TypeReference(); 
			propertyDeclaration.GetRegion = new PropertyGetRegion(null, null);
			propertyDeclaration.SetRegion = new PropertySetRegion(null, null);
			return propertyDeclaration;
		}
		public static TypeDeclaration add_Property(this TypeDeclaration targetType, string propertyName, TypeDeclaration propertyType)
		{
			targetType.add_Property(propertyName.ast_Property(propertyType.name()));	
			return targetType;
		}
	}
	public static class Extra_TypeDeclaration_ExtensionMethods
	{
		public static TypeDeclaration type_with_BaseType(this IParser iParser, string baseType)
		{
			return iParser.types().type_with_BaseType(baseType);
		}
	
		public static TypeDeclaration type_with_BaseType(this List<TypeDeclaration> typeDeclarations, string baseType)
		{
			foreach(var typeDeclaration in typeDeclarations)			
				if(typeDeclaration.baseTypes().contains(baseType))
					return typeDeclaration;				
			return null;
		}
		
		public static TypeReference typeReference(this TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.name().ast_TypeReference();
		}
		public static List<string> baseTypes(this TypeDeclaration typeDeclaration)
		{
			return (from baseType in typeDeclaration.BaseTypes
				    select baseType.str()).toList();			
		}
		
		public static MethodDeclaration method(this TypeDeclaration typeReference, string name)
		{
			return typeReference.methods().method(name);
		}
		public static List<MethodDeclaration> methods_with_Attribute(this TypeDeclaration typeDeclaration , string attributeName)
		{
			return typeDeclaration.methods().methods_with_Attribute(attributeName);
		}
		
		public static List<MethodDeclaration> methods_with_Attribute(this List<MethodDeclaration> methods , string attributeName)
		{
			return (from method in methods
					where method.attributes().names().contains(attributeName)
					select method).toList();
		}
	}
	
	public static class Extra_AssignmentExpression_ExtensionMethods
	{
		public static AssignmentExpression assign_To(this Expression left, Expression right)
		{
			return new AssignmentExpression(left, AssignmentOperatorType.Assign, right);
		}
		public static AssignmentExpression assign_To(this Expression left,AssignmentOperatorType assignmentOperator, Expression right)
		{
			return new AssignmentExpression(left, assignmentOperator, right);
		}		
		public static BlockStatement add_Assignment(this BlockStatement blockStatement, string Identifier_Left, TypeDeclaration typeDeclaration_Right)
		{
			return blockStatement.add_Assignment(Identifier_Left.ast_Identifier(), typeDeclaration_Right.ast_ObjectCreate());
		}
		public static BlockStatement add_Assignment(this BlockStatement blockStatement, PropertyDeclaration propertyDeclaration_Left, TypeDeclaration typeDeclaration_Right)
		{
			return blockStatement.add_Assignment(propertyDeclaration_Left, typeDeclaration_Right.ast_ObjectCreate());
		}		
		public static BlockStatement add_Assignment(this BlockStatement blockStatement, PropertyDeclaration propertyDeclaration_Left, Expression right)
		{
			return blockStatement.add_Assignment(propertyDeclaration_Left.Name.ast_Identifier(), right);
		}
		public static BlockStatement add_Assignment(this BlockStatement blockStatement, Expression left, Expression right)
		{
			var assignment = left.assign_To(right);
			return blockStatement.append(assignment.expressionStatement());
		}
	}	
	
	public static class Extra_TypeReference_ExtensionMethods
	{	
		public static ObjectCreateExpression ast_ObjectCreate(this TypeDeclaration typeDeclaration)
		{
			return typeDeclaration.typeReference().ast_ObjectCreate();
		}
	
		public static ObjectCreateExpression ast_ObjectCreate(this TypeReference typeReference)
		{
			return new ObjectCreateExpression(typeReference, null);
		}
		
		public static TypeReference ast_TypeReference(this string type)
		{
			return new TypeReference(type, true);	
		}
	
		public static ConstructorDeclaration add_Ctor(this TypeDeclaration typeDeclaration)
		{
			var name = "";
			var modifier = Modifiers.Public;
			var _parameters = new List<ParameterDeclarationExpression>();
			var _attributes = new List<AttributeSection>();
			
			var ctorDeclaration = new ConstructorDeclaration(name, modifier, _parameters, _attributes);
			ctorDeclaration.Body = new BlockStatement();
			typeDeclaration.add_Ctor(ctorDeclaration);
			return ctorDeclaration;
		}
		
		public static BlockStatement body(this ConstructorDeclaration constructorDeclaration)
		{
			if (constructorDeclaration.Body.isNull())
				constructorDeclaration.Body = new BlockStatement();
			return constructorDeclaration.Body;
		}
	}
	public static class Extra_ascx_SourceCodeEditor
	{
		public static ascx_Simple_Script_Editor add_Script_Me(this Panel panel, object targetObject, string varName)
		{
			return targetObject.script_Me(varName,panel);
		}
		
		public static ascx_SourceCodeEditor csharp_Colors(this ascx_SourceCodeEditor codeEditor)
		{
			return codeEditor.set_ColorsForCSharp();
		}
		public static ascx_Simple_Script_Editor executeOnCompile(this ascx_Simple_Script_Editor simpleEditor)
		{
			simpleEditor.ExecuteOnCompile = true;
			return simpleEditor;
		}
	}

	public static class Extra_WinForm_Controls_TreeView
	{
		public static TreeView add_Nodes(this TreeView treeView, params string[] nodes)
		{
			return treeView.add_Nodes(nodes.toList());
		}
	}
	
	public static class Extra_WinForm_Controls_Control
	{
		public static T autoSize<T>(this T control, bool value) where T : Control
		{
			return control.invokeOnThread(
				()=>{
						control.AutoSize = value;
						return control;
					});
		}
	}
	public static class Extra_CryptoMethods
	{
		//based on code sample from: http://msdn.microsoft.com/en-us/library/system.security.cryptography.aes(v=vs.100).aspx
		public static byte[] encrypt_AES(this string plainText, string key, string iv)
		{
			return plainText.encrypt_AES(key.hexStringToByteArray(), iv.hexStringToByteArray());
		}
		public static byte[] encrypt_AES(this string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the streams used
            // to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;
            CryptoStream csEncrypt = null;
            StreamWriter swEncrypt = null;

            // Declare the Aes object
            // used to encrypt the data.
            Aes aesAlg = null;

            // Declare the bytes used to hold the
            // encrypted data.
            byte[] encrypted = null;

            try
            {
                // Create an Aes object
                // with the specified key and IV.
                aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                swEncrypt = new StreamWriter(csEncrypt);

                //Write all data to the stream.
                swEncrypt.Write(plainText);

            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (swEncrypt != null)
                    swEncrypt.Close();
                if (csEncrypt != null)
                    csEncrypt.Close();
                if (msEncrypt != null)
                    msEncrypt.Close();

                // Clear the Aes object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }
        public static string decrypt_AES(this byte[] cipherText, string key, string iv)
		{
			return cipherText.decrypt_AES(key.hexStringToByteArray(), iv.hexStringToByteArray());
		}
		public static string decrypt_AES(this byte[] cipherText, byte[] Key,  byte[] IV)	 
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // TDeclare the streams used
            // to decrypt to an in memory
            // array of bytes.
            MemoryStream msDecrypt = null;
            CryptoStream csDecrypt = null;
            StreamReader srDecrypt = null;

            // Declare the Aes object
            // used to decrypt the data.
            Aes aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create an Aes object
                // with the specified key and IV.
                aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                msDecrypt = new MemoryStream(cipherText);
                csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                srDecrypt = new StreamReader(csDecrypt);

                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            finally
            {
                // Clean things up.

                // Close the streams.
                if (srDecrypt != null)
                    srDecrypt.Close();
                if (csDecrypt != null)
                    csDecrypt.Close();
                if (msDecrypt != null)
                    msDecrypt.Close();

                // Clear the Aes object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
      		
		public static byte[] hexStringToByteArray(this string hex)
		{
			try
			{
				return Enumerable.Range(0, hex.Length)
	            	             .Where(x => x % 2 == 0)
	                	         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
	                    	     .ToArray();
			}
			catch(Exception ex)
			{
				ex.log();
				return new byte[] {};
			}
	    }
	
	}

	public static class Extra_Assembly
	{
		
	}
	
	public static class Extra_compile_Collections
	{
		
	}

	public static class Extra_compile_Control
	{
	
		
		 
    	
    	
		//not working (more work is needed to add drag and drop support to Images (and other WinForm controls)
		/*public static T  onDrag<T, T1>(this T control, Func<T, T1> getDragData) where T : UserControl
        {
            control.ItemDrag += (sender, e) =>
            {
                var dataToDrag = getDragData(tag);                
                    if (dataToDrag != null)
                        control.DoDragDrop(dataToDrag, DragDropEffects.Copy);                    
            };
            return control;
        }*/		
	}
	
	public static class Extra_WinForm_Controls_Menu
	{
		public static MenuStrip add_MenuStrip(this Control control)
		{
			return control.add_Control<MenuStrip>();
		}
		
		public static MenuStrip insert_MenuStrip(this Control control)
		{
			return control.insert_Above(30).splitContainerFixed().add_MenuStrip();
		}
		
		public static ToolStripMenuItem add_Menu(this MenuStrip menuStrip, string name)
		{
			return menuStrip.add_MenuItem(name);
		}
		public static ToolStripMenuItem add_Menu(this ToolStripMenuItem toolStripMenuItem, string name)
		{
			if (toolStripMenuItem.notNull() && toolStripMenuItem.Owner is MenuStrip)
				return (toolStripMenuItem.Owner as MenuStrip).add_Menu(name);
			"[in add_Menu] toolStripMenuItem.Owner was not a MenuStrip, it was: {0}".error(toolStripMenuItem.typeName());
			return null;
		}
	}
	public static class Extra_WinForm_Controls_Drawing
	{
		    	
			
		/*public static Font regular(this Font font)			// this requires a different approach
		{
			return font.style_Add(FontStyle.Regular);
		}*/

	}
	
	public static class Extra_WinForm_Controls_ImageList
	{
		
		/* we can't do this because of cross thread error, but I can't see how we can get the thread value from the Image list (even though it is already connected to a Treeview)
		public static ImageList add(this ImageList imageList, params string[] keys)
		{
			foreach(var key in keys)
				imageList.add(key, key.formImage());
			return imageList;
		}
		public static ImageList add(this ImageList imageList, string key, Image image)
		{
			if (key.notNull() && image.notNull())
				imageList.Images.Add(key, image);
			return imageList;
		}*/
		
		
		
	}
	
	
	
	public static class Extra_WinForm_Controls_TreeNode
    {
    	
    	
    	
		
    	
    	
    }
	
	public static class Extra_WinForm_Controls_Label
	{
		
	}
	
	public static class Extra_ZIp
	{
		
	}

	public static class Extra_Misc
	{		
	
		
		
		// do this propely when adding to main O2 code base since this will not work as expected when there are other textboxes and buttons on the same 'control' object
		public static T add_LabelAndTextAndButton<T>(this T control, string labelText, string textBoxText, string buttonText,ref TextBox textBox, ref Button button, Action<string> onButtonClick)            where T : Control
		{
			control.add_LabelAndTextAndButton(labelText,textBoxText,buttonText, onButtonClick);
			textBox = control.control<TextBox>();
			button = control.control<Button>();
			return control;
		}
	}

	public static class Extra_compile_Label
	{
		
	}
	public static class Extra_compile_PictureBox
	{
		
		
		
		
	}
		
	public static class Extra_Controls_ToolStip
	{
		
	 
		
	}	
	public static class _Extra_WinForms_Controls_MainMenu
	{
		
	}
	
	public static class Extra_Misc_Icons
	{
		
	}
    
	public static class Extra_Processes
	{			
		[DllImport( "kernel32.dll" )]
    	public static extern bool IsWow64Process( System.IntPtr aProcessHandle, out bool lpSystemInfo );
		
		//this is not working correctly
		public static bool is64BitProcess(this Process process )
	    {
	    	if (process.isNull())
	    	{
	    		"in process.is64BitProcess provided process value was null!".error();
	    		return false;
	    	}
	        bool lIs64BitProcess = false;
	        //if ( System.Environment.Is64BitOperatingSystem ) 
	        //{
	            IsWow64Process( process.Handle, out lIs64BitProcess );
	            return ! lIs64BitProcess; // weirdly the value is reversed
	        //}
	        //"[Is Target Process 64Bit = {0}]".debug(lIs64BitProcess);
	        //return lIs64BitProcess;	        	        
	    }
	   
	}	
}
    	