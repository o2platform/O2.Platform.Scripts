// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.Utils
{	
	
	[Serializable]
	public  class NameValueItems : Items
	{
		
	}
	
	[Serializable]
	public  class Items : List<Item> 
	{
		public bool overrideIfExists = true;
		public bool alertOnOverriding = true;
		
		public string this[string key] 
		{
			get
			{
				foreach(var item in this)
					if (item.Key == key)
						return item.Value;
				return null;
					//return new Item(value,value);
			}	
			set
			{
				if (overrideIfExists)				
					foreach(var item in this)
						if (item.Key == key)
						{	
							if(alertOnOverriding)
								"Item Override: on key value '{0}', overriding the original value '{1}' with '{2}'".debug(item.Key,item.Value, value);					
							item.Value = value;
							return;
						}
				this.Add(new Item(key, value));
			}
		}
				
	}
	
	[Serializable]
	public  class Item : NameValuePair<string,string>
	{
		public Item()
		{}
		
		public Item(string key, string value) : base(key,value)
		{
			
		}
	}
	
	public static class Items_ExtensionMethods
	{
		public static Item item(this Items items, string key)
		{
			foreach(var item in items)
				if (item.Key == key)
					return item;
			return null;				
		}
		
		public static string value(this Item item)
		{
			if(item.notNull())
				return item.Value;
			return null;
		}
		
		public static Item value(this Item item, string value)
		{
			if(item.notNull())
				item.Value = value;
			return item;
		}
		
		public static bool hasKey(this Items items, string key)
		{
			foreach(var item in items)
				if (item.Key == key)
					return true;
			return false;		
					
		}			
		
		public static Items add(this Items items, string key, string value)
		{
			items[key] = value;
			return items;
		}		
		
		public static Dictionary<string,string> toDictionary(this Items items)
		{
			var dictionary = new Dictionary<string,string>();
			foreach(var item in items)	
			{
				if(dictionary.hasKey(item.Key))
					"alert: in Items.toDictionary there was a duplicate key value for {0}, the original value ('{1}') will be overritten with '{2}')".debug(item.Key, dictionary[item.Key], item.Value);					
				dictionary.add(item.Key, item.Value);
			}
			return dictionary;
		}
		
		public static Items toItems(this Dictionary<string,string> dictionary)
		{
			var items = new Items();
			foreach(var keyValuePair in dictionary)
				items.add(keyValuePair.toItem());
			return items;
		}
		
		public static Item toItem(this KeyValuePair<string,string> keyValuePair)
		{
			return new Item(keyValuePair.Key,keyValuePair.Value);
		}
		
		public static Items remove(this Items items, string key)
		{
			var itemToRemove = items.item(key);
			if(itemToRemove.isNull())
				"in Items.remove, could not find item with key: '{0}'".error(key);
			else
				items.Remove(itemToRemove);
			return items;
		}
		
		public static Items set(this Items items, string key, string value)
		{
			return items.add(key,value);
		}
		
		public static string get(this Items items, string key)
		{
			return items[key];
		}
		
		public static List<string> keys(this Items items)
		{
			return (from item in items
					select item.Key).toList();
		}
		
		public static List<string> values(this Items items)
		{
			return (from item in items
					select item.Value).toList();
		}
	}
	
	[Serializable]
	public  class NameValuePair<T,K>
	{
		[XmlAttribute]
		public T Key {get;set;}
		[XmlAttribute]
		public K Value {get;set;}
		
		public NameValuePair()
		{}
		
		public NameValuePair(T key, K value)
		{
			Key = key;
			Value = value;
		}
		
		public override string ToString()
		{
			return Key.str();
		}
	}

	public static class NameValuePair_ExtensionMethods
	{									
		public static List<NameValuePair<T,K>> add<T,K>(this List<NameValuePair<T,K>> list, T key, K value)
		{
			list.Add(new NameValuePair<T,K>(key,value));
			return list;
		}
	}
	
	/*public static class NameValueItems_ExtensionMethods
	{
		public static NameValueItems add(this NameValueItems items, string key, string value)
		{
			Items_ExtensionMethods.add((Items)items,key,value);
			return items;
		}
	}*/
	
	
		#region tuples

    public class Tuple<T>
    {
        public Tuple(T first)
        {
            First = first;
        }

        public T First { get; set; }
        
        public T Item1 { get { return First; } set { First = value;} } // to make it compatible with .NET 4.0 ones
    }

    public class Tuple<T, T2> : Tuple<T>
    {
        public Tuple(T first, T2 second)
            : base(first)
        {
            Second = second;
        }

        public T2 Second { get; set; }
        
        public T2 Item2  { get { return Second; } set { Second = value;} }
    }

    public class Tuple<T, T2, T3> : Tuple<T, T2>
    {
        public Tuple(T first, T2 second, T3 third)
            : base(first, second)
        {
            Third = third;
        }

        public T3 Third { get; set; }
    }

    public class Tuple<T, T2, T3, T4> : Tuple<T, T2, T3>
    {
        public Tuple(T first, T2 second, T3 third, T4 fourth)
            : base(first, second, third)
        {
            Fourth = fourth;
        }

        public T4 Fourth { get; set; }
    }

    #endregion	        
}
    	