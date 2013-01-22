using System;
using System.Drawing;

namespace O2.XRules.Database.APIs
{
	public partial class WinAPI
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
		
		
			
		public enum ShowWindowCommands : int			//see http://www.pinvoke.net/default.aspx/Enums/ShowWindowCommand.html
		{	    
		    Hide 			= 0,
		    Normal 			= 1,
		    ShowMinimized 	= 2,
		    //Maximize		= 3, // is this the right value?
		    ShowMaximized 	= 3,
		    ShowNoActivate 	= 4,
		    Show 			= 5,
		    Minimize 		= 6,
		    ShowMinNoActive = 7,
		    ShowNA 			= 8,
		    Restore 		= 9,
		    ShowDefault 	= 10,
		    ForceMinimize 	= 11
		}
		
		public  enum SetWindowPosFlags : uint			// see http://www.pinvoke.net/default.aspx/Enums/SetWindowPosFlags.html
		{	    
		    AsynchronousWindowPosition 	= 0x4000,
		    DeferErase 					= 0x2000,
		    DrawFrame		 			= 0x0020,
		    FrameChanged 				= 0x0020,
		    HideWindow 					= 0x0080,
		    DoNotActivate 				= 0x0010,
		    DoNotCopyBits 				= 0x0100,
		    IgnoreMove 					= 0x0002,
		    DoNotChangeOwnerZOrder 		= 0x0200,
		    DoNotRedraw 				= 0x0008,
		    DoNotReposition 			= 0x0200,
		    DoNotSendChangingEvent 		= 0x0400,
		    IgnoreResize 				= 0x0001,
		    IgnoreZOrder 				= 0x0004,
		    ShowWindow 					= 0x0040,
		    
		    
		    IgnoreMoveAndResize			= IgnoreMove | IgnoreResize
		}
	}
}