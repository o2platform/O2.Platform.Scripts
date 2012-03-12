// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
 
namespace O2.XRules.Database.Utils
{	
	public static class List_ExtensionMethods
	{
		public static List<string> removeEmpty(this List<string> list)
		{
			return (from item in list
					where item.valid()
					select item).toList();
		}
				
		public static List<string> add_If_Not_There(this List<string> list, string item)
		{
			if (item.notNull())
				if (list.contains(item).isFalse())
					list.add(item);
			return list;
		}
		public static string join(this List<string> list)
		{
			return list.join(",");
		}
		
		public static string join(this List<string> list, string separator)
		{
			if (list.size()==1)
				return list[0];
			if (list.size() > 1)
				return list.Aggregate((a,b)=> "{0} {1} {2}".format(a,separator,b));
			return "";
		}
		
		public static T item<T>(this List<T> list, int index)
		{
			return list.value(index);
		}
		
		public static T value<T>(this List<T> list, int index)
		{
			if (list.size() > index)
				return list[index];
			return default(T);
		}
		
		public static List<T> where<T>(this List<T> list, Func<T,bool> query)
		{
			return list.Where<T>(query).toList();
		}
		
		public static T first<T>(this List<T> list, Func<T,bool> query)
		{
			var results = list.Where<T>(query).toList();
			if (results.size()>0)
				return results.First();
			return default(T);
		}
		
		public static T second<T>(this List<T> list)
		{			
			if (list.notNull() && list.size()>1)
				return list[1];		
			return default(T);
		}
		
		public static List<T> removeRange<T>(this List<T> list, int start, int end)
		{
			list.RemoveRange(start,end);
			return list;
		}
	
		public static List<T> list<T>(this T item)
		{
			return item.wrapOnList<T>();
		}
		
		public static List<T> push<T>(this List<T> list, T value)
		{
			if (list.notNull())			
				list.add(value);
			return list;
		}

		public static T pop<T>(this List<T> list)
		{			
			if (list.notNull() && list.Count > 0)
			{
				int valuePosition = list.Count - 1;
				var value = list[valuePosition];
				list.RemoveAt(valuePosition);
				return value;
			}
			return  default(T);
		}
		
		public static T shift<T>(this List<T> list)
		{			
			if (list.notNull() && list.Count > 0)
			{
				T value = list[0];
				list.RemoveAt(0);
				return value;
			}
			return default(T);			
		}
	
		public static List<T> unshift<T>(this List<T> list, T value)
		{
			if (list.notNull())			
				list.Insert(0, value);
			return list;
		}
		
	}
	
	public static class IEnumerable_ExtensionMethods
	{
		public static bool isIEnumerable(this object list)
		{
			return list.notNull() && list is IEnumerable;
		}
		
		public static int count(this object list)
		{
			if (list.isIEnumerable())
				return (list as IEnumerable).count();
			return -1;
		}
		
		public static int size(this IEnumerable list)
		{
			return list.count();
		}
		public static int count(this IEnumerable list)
		{			
			var count = 0;
			if (list.notNull())
				foreach(var item in list)
					count++;
			return count;
		}
		
		public static object first(this IEnumerable list)
		{
			if(list.notNull())
				foreach(var item in list)
					return item;
			return null;
		}
		
		public static T first<T>(this IEnumerable<T> list)
		{
			try
			{
				if (list.notNull())
					return list.First();
			}
			catch(Exception ex)
			{	
				if (ex.Message != "Sequence contains no elements")
					"[IEnumerable.first] {0}".error(ex.Message);
			}
			return default(T);
		}						
		
		public static T last<T>(this IEnumerable<T> list)
		{
			try
			{
				if (list.notNull())
					return list.Last();
			}
			catch(Exception ex)
			{	
				if (ex.Message != "Sequence contains no elements")
					"[IEnumerable.first] {0}".error(ex.Message);
			}
			return default(T);
		}
		
		public static object last(this IEnumerable list)
		{
			object lastItem = null;
			if(list.notNull())
				foreach(var item in list)
					lastItem= item;
			return lastItem;
		}
		
		
		public static List<T> insert<T>(this List<T> list, T value)
		{
			return list.insert(0, value);
		}
		
		public static List<T> insert<T>(this List<T> list, int position, T value)
		{
			list.Insert(position, value);
			return list;
		}		
				
	}
	
	public static class Dictionary_ExtensionMethods
	{
		public static Dictionary<string,string> toStringDictionary(this string targetString, string rowSeparator, string keySeparator)
		{
			var stringDictionary = new Dictionary<string,string>();
			try
			{
				foreach(var row in targetString.split(rowSeparator))
				{
					if(row.valid())
					{
						var splittedRow = row.split(keySeparator);
						if (splittedRow.size()!=2)
							"[toStringDictionary] splittedRow was not 2: {0}".error(row);
						else
						{
							if (stringDictionary.hasKey(splittedRow[0]))
								"[toStringDictionary] key already existed in the collection: {0}".error(splittedRow[0]);		
							else
								stringDictionary.Add(splittedRow[0], splittedRow[1]);
						}
					}
				}
			}
			catch(Exception ex)
			{
				"[toStringDictionary] {0}".error(ex.Message);
			}
			return stringDictionary;
		}
		
		public static Dictionary<string,string> remove(this Dictionary<string,string> dictionary, Func<KeyValuePair<string,string>, bool> filter)
		{
			var itemsToRemove = dictionary.Where(filter).toList();
			//var itemsToRemove = CompileEngine.CachedCompiledAssemblies.Where((item)=>item.Value.str().contains("O2_TeamMentor_AspNet"));  
			foreach(var itemToRemove in itemsToRemove)
				dictionary.Remove(itemToRemove.Key);    
			return dictionary;
		}
		
		public static T value<T>(this Dictionary<string,object> dictionary, string name)
		{
			var value = dictionary.value(name);
			if (value.notNull() && value is T)
				return (T)value;
			return default(T);
		}
	}
	
	public static class Loop_ExtensionMethods
	{
		public static Action loop(this int count , Action action)
		{
			return count.loop(0,action);
		}
		
		public static Action loop(this int count , int delay,  Action action)
		{
			"Executing provided action for {0} times with a delay of {1} milliseconds".info(count, delay);
			for(var i=0 ; i < count ; i ++)
			{
				action();
				if (delay > 0)
					count.sleep(delay);
			}
			return action;
		}
		
		public static Action<int> loop(this int count , Action<int> action)
		{
			return count.loop(0, action);
		}
		
		public static Action<int> loop(this int count , int start, Action<int> action)
		{
			return count.loop(start,1, action);
		}
		
		public static Action<int> loop(this int count, int start , int step, Action<int> action)
		{
			for(var i=start ; i < count ; i+=step)			
				action(i);							
			return action;
		}
		
		public static Func<int,bool> loop(this int count, Func<int,bool> action)
		{
			return count.loop(0, action);
		}
		
		public static Func<int,bool> loop(this int count, int start , Func<int,bool> action)
		{
			for(var i=start ; i < count ; i++)			
				if (action(i).isFalse())
					break;
			return action;
		}
		
		
		public static List<T> loopIntoList<T>(this int count , Func<int,T> action)
		{
			return count.loopIntoList(0, action);
		}
		
		public static List<T> loopIntoList<T>(this int count , int start, Func<int,T> action)
		{
			return count.loopIntoList(start,1, action);
		}
		
		public static List<T> loopIntoList<T>(this int count, int start , int step, Func<int,T> action)
		{
			var results = new List<T>();
			for(var i=start ; i < count ; i+=step)			
				results.Add(action(i));
			return results;
		}		
		
	}
	
	public static class _Extra_Collections_ExtensionMethods_Uri
	{
		public static List<Uri> uris(this List<string> urls)
		{
			return (from url in urls
					where url.isUri()
					select url.uri()).toList();
		}	
		
		public static List<string> queryParameters_Names(this List<Uri> uris)
		{			
			return (from uri in uris
					from name in uri.queryParameters_Indexed_ByName().Keys					
					select name).Distinct().toList();
					
/*			var names = new List<string>();
			foreach(var uri in uris)
			{
				var mappedQueryParameters = uri.queryParameters_Indexed_ByName();
				foreach(var mappedItems in mappedQueryParameters)
					if (!mappedItems.hasKey
					names.add_If_Not_There(mappedItems.Key);
					return names;
			}*/
			
		}
		
		public static List<string> queryParameters_Values(this List<Uri> uris, string parameterName)
		{
			return (from uri in uris
					let parameters = uri.queryParameters_Indexed_ByName()										
					where parameters.hasKey(parameterName)					
					select parameters[parameterName]).toList();					
		}
		
		public static List<string> queryParameters_Values(this List<Uri> uris)
		{
			var values = new List<string>();
			foreach(var uri in uris)
				values.AddRange(uri.queryParameters_Indexed_ByName().Values);								
			return values;				
		}
		
		public static Dictionary<string,string> queryParameters_Indexed_ByName(this Uri uri)
		{		
			var queryParameters = new Dictionary<string,string>();
			if (uri.notNull())
			{
				var query = uri.Query;
				if (query.starts("?"))
					query = query.removeFirstChar();
				foreach(var parameter in query.split("&"))				
					if (parameter.valid())
					{
						var splitParameter = parameter.split("=");
						if (splitParameter.size()==2)
							if (queryParameters.hasKey(splitParameter[0]))
							{	
								"duplicate parameter key in property '{0}', adding extra parameter in a new line".info(splitParameter[0]);
								queryParameters.add(splitParameter[0], queryParameters[splitParameter[0]].line() + splitParameter[1]);
							}
							else
								queryParameters.add(splitParameter[0], splitParameter[1]);
						else						
							"Something's wrong with the parameter value, there should only be one = in there: '{0}' ".info(parameter);
					}					
			} 
		return queryParameters;
		}
	}
		
}