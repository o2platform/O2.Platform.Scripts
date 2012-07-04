// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.H2Scripts;
using System.Runtime.InteropServices;

namespace O2.XRules.Database.Utils
{	

	public static class _Extra_ExtensionMethods_Object
	{
		public static T wait<T>(this T _object)
		{
			return _object.wait(1000, true);
		}
		public static T wait<T>(this T _object, int length, bool verbose)
		{
			_object.sleep(length, verbose);
			return _object;
		}
	}
	
	public static class _Extra_H2_ExtensionMethods
	{
		public static string scriptSourceCode(this string file)
		{
			if (file.extension(".h2"))
				return file.h2_SourceCode();
			return file.fileContents();
		}
		public static string h2_SourceCode(this string file)
		{
			if (file.extension(".h2"))
			{
				//"return source code of H2 file".info();
				return H2.load(file).SourceCode;
			}
			return null;
		}			
	}		
	
	public static class _Extra_LiveObjects_ExtensionMethods
	{
				
	}
	
	public static class _Extra_String_ExtensionMethods
	{		 

	}	
	
	
	
	
	public static class _Extra_Uri_ExtensionMethods
	{

	}
	
	
	
	
	public static class _Extra_XML_ExtensionMethods
	{		

	}
	
	public static class _Extra_RegEx_ExtensionMethods	
	{

	}
}    	