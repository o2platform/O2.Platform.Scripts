using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:System.Web.Extensions.dll

namespace SecurityInnovation.TeamMentor.WebClient
{	
	public class TM_GUI_Objects
	{		
		public List<string> GuidanceItemsMappings 	{ get; set;}
		public List<string> UniqueStrings			{ get; set;}
		
		public TM_GUI_Objects()
		{
			GuidanceItemsMappings = new List<string>();						
			UniqueStrings = new List<string>();
		}				
		
		/*public Dictionary<string,int> dictionary()
		{
			return _uniqueStrings;
		}*/
		
		public int add_UniqueString(string value)
		{				
			if (UniqueStrings.Contains(value).isFalse())
				UniqueStrings.Add(value);
				
			return get_UniqueString(value);
						
		}				
		
		public int get_StringIndex(string value)
		{
			return UniqueStrings.IndexOf(value);
		}
		
		public string get_UniqueString(int index)
		{
			return UniqueStrings[index];
		}
		
		public int get_UniqueString(string value)
		{
			return get_StringIndex(value);
		}				
		
		public int get_Index(string value)
		{
			return get_StringIndex(value);
		}
		
		public string get_String(int index)
		{
			return get_UniqueString(index);
		}
		
	}		
	
	public class Library_V3	
	{
		public Guid libraryId 				{ get; set; }
		public String name		 			{ get; set; }
		public List<Folder_V3> subFolders 	{ get; set; }
		public List<View_V3> views 			{ get; set; }
		public List<Guid> guidanceItems		{ get; set; }
		
		public Library_V3()
		{
			subFolders = new List<Folder_V3> ();
			views = new List<View_V3>();
			guidanceItems = new List<Guid>();
		}
		
		public override string ToString()
		{
			return "library: {0}".format(name);
		}
	}
	
	public class Folder_V3	
	{
		public Guid libraryId { get; set; }
		public Guid folderId  { get; set; }
		public string name { get; set; }
		public List<View_V3> views { get; set; }
		public List<Folder_V3> subFolders { get; set; }
		public List<Guid> guidanceItems		{ get; set; }
		
		public Folder_V3()
		{
			views = new List<View_V3>();
			subFolders = new List<Folder_V3>();
			guidanceItems = new List<Guid>();
		}
		
		public override string ToString()
		{
			return "folder: {0}".format(name);
		}
	}
	
	public class View_V3
	{
		public Guid libraryId { get; set; }
		public Guid folderId { get; set; }
		public Guid viewId { get; set; }
		public string caption { get; set; }
		public string author { get; set; }
		public string guidanceItems_Indexes { get; set; }
		public List<Guid> guidanceItems {get;set;}
		
		public View_V3()
		{
			guidanceItems= new List<Guid>();
		}
		
		public override string ToString()
		{
			return "view: {0}".format(caption);
		}
	}
	
	public class NewUser
    {
    	public string username { get; set; }
    	public string passwordHash {get;set;}
    	public string email { get; set; }
    	public string firstname { get; set; }
    	public string lastname { get; set; }
    	public string note { get; set; }
    	public int groupId { get; set; }
    }
	
	public class TreeNodeItem
	{
		public string type { get; set; }
		public string libraryId { get; set; }		
		public string itemId { get; set; }
	}
}
