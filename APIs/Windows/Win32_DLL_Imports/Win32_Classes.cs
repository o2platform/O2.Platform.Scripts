using System;
using System.Drawing;

namespace O2.XRules.Database.APIs
{
	public partial class Win32
	{
		public class WINDOWCMD
		{
			public const int GW_HWNDFIRST = 0;
			public const int GW_HWNDLAST = 1;
			public const int GW_HWNDNEXT = 2;
			public const int GW_HWNDPREV = 3;
			public const int GW_OWNER = 4;
			public const int GW_CHILD = 5;
		}
		public class PROCESSACCESS
		{
			public const int PROCESS_QUERY_INFORMATION = 1024;
		}
		public class WM
		{
			public const int WM_CREATE = 1;
		}
		public class RDW
		{
			public const uint RDW_INVALIDATE = 1u;
			public const uint RDW_INTERNALPAINT = 2u;
			public const uint RDW_ERASE = 4u;
			public const uint RDW_VALIDATE = 8u;
			public const uint RDW_NOINTERNALPAINT = 16u;
			public const uint RDW_NOERASE = 32u;
			public const uint RDW_NOCHILDREN = 64u;
			public const uint RDW_ALLCHILDREN = 128u;
			public const uint RDW_UPDATENOW = 256u;
			public const uint RDW_ERASENOW = 512u;
			public const uint RDW_FRAME = 1024u;
			public const uint RDW_NOFRAME = 2048u;
		}
	}
	
	
	//see http://www.pinvoke.net/default.aspx/Enums/ShowWindowCommand.html
	public enum ShowWindowCommands : int
	{	    
	    Hide = 0,
	    Normal = 1,
	    ShowMinimized = 2,
	    Maximize = 3, // is this the right value?
	    ShowMaximized = 3,
	    ShowNoActivate = 4,
	    Show = 5,
	    Minimize = 6,
	    ShowMinNoActive = 7,
	    ShowNA = 8,
	    Restore = 9,
	    ShowDefault = 10,
	    ForceMinimize = 11
	}
}