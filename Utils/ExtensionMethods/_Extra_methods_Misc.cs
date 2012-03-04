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
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using System.Runtime.InteropServices;

namespace O2.XRules.Database.Utils
{		
	public static class _Extra_LiveObjects_ExtensionMethods
	{
		public static T castIfType<T>(this object _object)
		{
			if (_object is T)
				return (T)_object;
			return default(T);
		}
		
		public static object liveObject(this string key, object value)
		{
			O2LiveObjects.set(key,value);
			return value;
		}
		
		public static object liveObject(this string key)
		{
			return O2LiveObjects.get(key);	
		}
		
		
		public static object o2Cache(this string key)
		{
			return key.liveObject();
		}
		
		public static object o2Cache(this string key, object value)
		{
			return key.liveObject(value);
		}
		
		public static T o2Cache<T>(this string key, T value)
		{
			O2LiveObjects.set(key,value);
			return value;
		}
		
		public static T o2Cache<T>(this string key)
		{
			var value =  O2LiveObjects.get(key);
			if (value is T)
				return (T)value;
			return default(T);
		}
		
		public static T o2Cache<T>(this string key, Func<T> ctor)
		{
			if (key.o2Cache<T>().isNull())
			{
				"there was no o2Chache object for type '{0}' so invoking the contructor callback".info(typeof(T));
				key.o2Cache<T>(ctor());
			}
			return key.o2Cache<T>();			
		}		
	}
	
	public static class _Extra_String_ExtensionMethods
	{		 
		public static Exception logStackTrace(this Exception ex)
		{
			"Error strackTrace: \n\n {0}".error(ex.StackTrace);
			return ex;
		}
		
		public static string ifEmptyUse(this string _string , string textToUse)
		{
			return (_string.empty() )
						?  textToUse
						: _string;
		}
		
		public static string upperCaseFirstLetter(this string _string)
		{
			if (_string.valid())
			{
				return _string[0].str().upper() + _string.subString(1); 
			}
			return _string;
		}								
		
		public static string append(this string _string, string stringToAppend)
		{
			return _string + stringToAppend;
		}
		
		public static string insertAt(this string _string,  int index, string stringToInsert)
		{
			return _string.Insert(index,stringToInsert);
		}
		
		public static string subString(this string _string, int startPosition)
		{
			if (_string.size() < startPosition)
				return "";
			return _string.Substring(startPosition);
		}
		
		public static string subString(this string _string,int startPosition, int size)
		{
			var subString = _string.subString(startPosition);
			if (subString.size() < size)
				return subString;
			return subString.Substring(0,size);
		}
		
		public static string subString_After(this string _string, string _stringToFind)
		{
			var index = _string.IndexOf(_stringToFind);
			if (index >0)
			{
				return _string.subString(index + _stringToFind.size());
			}
			return "";
		}
		
		public static string lastChar(this string _string)
		{
			if (_string.size() > 0)
				return _string[_string.size()-1].str();
			return "";			
		}
		
		public static bool lastChar(this string _string, char lastChar)
		{
			return _string.lastChar(lastChar.str());
		}
		
		public static bool lastChar(this string _string, string lastChar)
		{
			return _string.lastChar() == lastChar;
		}
		
		public static string firstChar(this string _string)
		{
			if (_string.size() > 0)
				return _string[0].str();
			return "";			
		}
		
		public static bool firstChar(this string _string, char lastChar)
		{
			return _string.firstChar(lastChar.str());
		}
		
		public static bool firstChar(this string _string, string lastChar)
		{
			return _string.firstChar() == lastChar;
		}
		
		public static string add_RandomLetters(this string _string)
		{
			return _string.add_RandomLetters(10);
		}
		
		public static string add_RandomLetters(this string _string, int count)
		{
			return "{0}_{1}".format(_string,count.randomLetters());
		}
		
		public static int randomNumber(this int max)
		{
			return max.random();
		}
				
		public static string ascii(this int _int)
		{
			try
			{				
				return ((char)_int).str();					
			}
			catch(Exception ex)
			{
				ex.log();
				return "";
			}
		}
		
		public static Guid next(this Guid guid)
		{
			return guid.next(1);
		}
		
		public static Guid next(this Guid guid, int incrementValue)
		{			
			var guidParts = guid.str().split("-");
			var lastPartNextNumber = Int64.Parse(guidParts[4], System.Globalization.NumberStyles.AllowHexSpecifier);
			lastPartNextNumber+= incrementValue;
			var lastPartAsString = lastPartNextNumber.ToString("X12");
			var newGuidString = "{0}-{1}-{2}-{3}-{4}".format(guidParts[0],guidParts[1],guidParts[2],guidParts[3],lastPartAsString);
			return new Guid(newGuidString); 					
		}
		
		public static Guid emptyGuid(this Guid guid)
		{
			return Guid.Empty;
		}
		
		public static Guid newGuid(this string guidValue)
		{
			return Guid.NewGuid();
		}
		
		public static Guid guid(this string guidValue)
		{
			if (guidValue.inValid())
				return Guid.Empty;			
			return new Guid(guidValue);		
		}
		
		public static bool isGuid(this String guidValue)
		{
			try
			{
				new Guid(guidValue);
				return true;
			}
			catch
			{
				return false;
			}
		}
		
		public static bool toBool(this string _string)
		{
			try
			{
				if (_string.valid())
					return bool.Parse(_string);				
			}
			catch(Exception ex)
			{
				"in toBool, failed to convert provided value ('{0}') into a boolean: {2}".format(_string, ex.Message);				
			}
			return false;
		}
		
		public static double toDouble(this string _string)
		{
			try
			{
				if (_string.valid())
					return Double.Parse(_string);				
			}
			catch(Exception ex)
			{
				"in toDouble, failed to convert provided value ('{0}') into a double: {2}".format(_string, ex.Message);				
			}
			return default(double);
		}
		
		public static IPAddress toIPAddress(this string _string)
		{
			try
			{
				var ipAddress = IPAddress.Loopback;
				IPAddress.TryParse(_string, out ipAddress);
				return ipAddress;
			}
			catch(Exception ex)
			{
				"Error in toIPAddress: {0}".error(ex.Message);
				return null;
			}
		}
		
		public static byte hexToByte(this string hexNumber)
		{
			try
			{
				return Byte.Parse(hexNumber,System.Globalization.NumberStyles.HexNumber);
			}
			catch(Exception ex)
			{
				"[hexToByte]	Failed to convert {0} : {1}".error(hexNumber, ex.Message);
				return default(byte);
			}
		}
		
		public static byte[] hexToBytes(this List<string> hexNumbers)
		{
			var bytes = new List<byte>();
			foreach(var hexNumber in hexNumbers)
				bytes.add(hexNumber.hexToByte());
			return bytes.ToArray();
		}
		
		public static int hexToInt(this string hexNumber)
		{
			try
			{
				return Int32.Parse(hexNumber,System.Globalization.NumberStyles.HexNumber);
			}
			catch(Exception ex)
			{
				"[hexToInt]	Failed to convert {0} : {1}".error(hexNumber, ex.Message);
				return -1;
			}
		}
		
		public static long hexToLong(this string hexNumber)
		{
			try
			{
				return long.Parse(hexNumber,System.Globalization.NumberStyles.HexNumber);
			}
			catch(Exception ex)
			{
				"[hexToLong]	Failed to convert {0} : {1}".error(hexNumber, ex.Message);
				return -1;
			}
		}
		
		public static string hexToAscii(this string hexNumber)
		{
			var value = hexNumber.hexToInt();
			if (value > 0)
				return value.ascii();
			return "";
		}
		
		public static string hexToAscii(this List<string> hexNumbers)
		{
			var asciiString = new StringBuilder();
			foreach(var hexNumber in hexNumbers)
				asciiString.Append(hexNumber.hexToAscii());
			return asciiString.str();
		}
	}	
	
	public static class _Extra_Long_ExtensionMethods
	{		
		public static long toLong(this double _double)
		{
			return (long)_double;
		}
		
		public static long add(this long _long, long value)
		{
			return _long + value;
		}				
	}
	
	public static class _Extra_Int_ExtensionMethods
	{		
		public static int toInt(this double _double)
		{
			return (int)_double;
		}
		
		public static int mod( this int num1, int num2)
		{
			return num1 % num2;
		}
		public static bool mod0( this int num1, int num2)
		{
			return num1.mod(num2) ==0;
		}
		
		public static string intToBinaryString(this int number)
		{
			return Convert.ToString(number,2);
		}			
	}	
	
	public static class _Extra_UInt_ExtensionMethods
	{
		public static uint toUInt(this string value)
		{
			return UInt32.Parse(value);
		}
	}
	
	
	public static class _Extra_Uri_ExtensionMethods
	{
		public static Uri append(this Uri uri, string virtualPath)
		{
			try
			{
				return new Uri(uri, virtualPath);
			}
			catch
			{
				return null;
			}
		}
	}
	
	public static class _Extra_DateTime_ExtensionMethods
	{
		public static long unixTime(this DateTime dateTime)
		{
			return (dateTime - new DateTime(1970, 1, 1)).TotalSeconds.toLong();
		}
		
		public static long unixTime_Now(this int secondsToAdd)
		{
			return DateTime.UtcNow.unixTime().add(secondsToAdd);
		}
		
		public static long unixTimeStamp_InSeconds(this int secondsToAdd)
		{
			return secondsToAdd.unixTime_Now();
		}
		
		public static bool isDate(this string date)
		{
			try
			{
				DateTime.Parse(date);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
	
	
	public static class _Extra_XML_ExtensionMethods
	{		
		public static List<XmlAttribute> add_XmlAttribute(this List<XmlAttribute> xmlAttributes, string name, string value)
		{
			var xmlDocument = (xmlAttributes.size() > 0) 
									?  xmlAttributes[0].OwnerDocument
									: new XmlDocument();						
			var newAttribute = xmlDocument.CreateAttribute(name);
			newAttribute.Value = value;
			xmlAttributes.add(newAttribute);
			return xmlAttributes;
		}		
	}
}    	