// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using System.Runtime.InteropServices;
using mscoree;

//O2Ref:mscoree_v4.0.dll

namespace O2.XRules.Database.APIs
{
	public class API_Mscoree
	{
		public API_Mscoree()
		{
			"mscoree loaded as: {0}".info("mscoree_v4.0.dll".assembly());
		}
	}
	
	public static class API_Mscoree_ExtensionMethods
	{
		// based on the code from http://blogs.msdn.com/b/jackg/archive/2007/06/11/enumerating-appdomains.aspx
		public static List<AppDomain> get_AppDomains_in_CurrentProcess(this API_Mscoree mscoree)
		{
			var corRuntimeHostClass = new mscoree.CorRuntimeHostClass();
			var  _IList = new List<AppDomain>();
			IntPtr enumHandle = IntPtr.Zero;
			CorRuntimeHostClass host = new mscoree.CorRuntimeHostClass();
			try
			{
			    host.EnumDomains(out enumHandle);
			    object domain = null;
			    while (true)
			    {
			
			        host.NextDomain(enumHandle, out domain);
			        if (domain == null) break;
			        AppDomain appDomain = (AppDomain)domain;
			        _IList.Add(appDomain);
			    }
			    return _IList;
			}
			catch (Exception e)
			{
			    Console.WriteLine(e.ToString());
			    return null;
			}
			finally
			{
			    host.CloseEnum(enumHandle);
			    Marshal.ReleaseComObject(host);
			}			
		}
	}
	
}
