// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;

namespace O2.XRules.Database.APIs
{
	public class API_ProcessHanles_test
	{
		public void test()
		{			
			var handles = API_ProcessHandles.returnArrayListWithCurrentHandles_usingBruteForceMethod(0xFFFF);			
			var tableList = "Current Process Handles".popupWindow()
													 .add_TableList()
													 .show(handles);													 
			tableList.makeColumnWidthMatchCellWidth();
			var currentProcess = Processes.getCurrentProcess();    
			tableList.title("{0} of {1} handles".info(handles.size(), currentProcess.HandleCount));
		}
		
		
	}
	public class API_ProcessHandles
	{	
		public static  List<handleItemInfo> returnArrayListWithCurrentHandles_usingBruteForceMethod(int numberOfHandlesToTry)
		{
			var listOfHandlesNames = new List<handleItemInfo>();			
			for (int i=0; i<numberOfHandlesToTry;i++)
			{				
				var handleItemInfo = getHandleItemInfo(i*4);
				if (handleItemInfo.notNull())
					listOfHandlesNames.add(handleItemInfo);
			}				
			return listOfHandlesNames;
		}
		
		public static handleItemInfo getHandleItemInfo(int handle)
		{
			IntPtr ObjectInformation = Marshal.AllocHGlobal(512);						
			ulong Length = 512;
			ulong ResultLength = 0;
			long callReturnValue = NtQueryObject(handle,OBJECT_INFORMATION_CLASS.ObjectNameInformation,ObjectInformation ,Length,ref ResultLength);				
			if (callReturnValue !=0 && callReturnValue != 0xc0000008)
			{
				//listOfHandlesNames.Add(":::::ERROR::::: on Item " + Convert.ToString(i*4,16).ToString() + " the error " + Convert.ToString(callReturnValue,16).ToString() + " occured");
				(":::::ERROR::::: on Item " + Convert.ToString(handle,16).ToString() + " the error " + Convert.ToString(callReturnValue,16).ToString() + " occured").error();
			}
			if (callReturnValue ==0)
			{								
				NAME_QUERY objectName = new NAME_QUERY();
				objectName = (NAME_QUERY)Marshal.PtrToStructure(ObjectInformation,objectName.GetType());					
				if (objectName.noIdeaWhatThisIs != "")
				{													
					handleItemInfo tempHandleItemInfo = new handleItemInfo( handle, objectName.Name, objectName.noIdeaWhatThisIs);
					return tempHandleItemInfo;					
				}
/*					else
				{
					handleItemInfo tempHandleItemInfo = new handleItemInfo( 0, objectName.Name, objectName.noIdeaWhatThisIs);
					listOfHandlesNames.Add(tempHandleItemInfo);						
				}*/
			}				
			return null;
		}

		public class handleItemInfo
		{
			public int 		HandleNumber 	{ get; set; }
			public string 	HandleName 		{ get; set; }
			public string 	ExtraInfo ;
			
			public handleItemInfo(int handleNumber, string handleName, string extraInfo)
			{
				HandleNumber = handleNumber;
				HandleName = handleName;
				ExtraInfo = extraInfo;
			}
		}

		const uint SystemHandleInformation = 16;

		[DllImport("kernel32.dll")]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("ntdll.dll", CharSet=CharSet.Auto)]
		public static extern uint NtQuerySystemInformation(	uint SystemInformationClass,
															IntPtr SystemInformation,
															long SystemInformationLength,
															uint ReturnLength );
		[DllImport("ntdll.dll", CharSet=CharSet.Auto)]
		public static extern uint NtQueryObject(int ObjectHandle,
												OBJECT_INFORMATION_CLASS ObjectInformationClass,
												IntPtr ObjectInformation,
												ulong Length,
												ref ulong ResultLength);

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto), BestFitMapping(false)]
			public struct NAME_QUERY
		{
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=4)] 
			public string noIdeaWhatThisIs;
			[MarshalAs(UnmanagedType.ByValTStr,SizeConst=512)] 
			public string Name;			
		} ;

		public enum OBJECT_INFORMATION_CLASS
		{
			ObjectBasicInformation,			// Result is OBJECT_BASIC_INFORMATION structure
			ObjectNameInformation,			// Result is OBJECT_NAME_INFORMATION structure
			ObjectTypeInformation,			// Result is OBJECT_TYPE_INFORMATION structure
			ObjectAllInformation,			// Result is OBJECT_ALL_INFORMATION structure
			ObjectDataInformation			// Result is OBJECT_DATA_INFORMATION structure
		
		}
		
		
	}			
}