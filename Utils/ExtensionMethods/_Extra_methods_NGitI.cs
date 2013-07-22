// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

// ReSharper disable RedundantUsingDirective
// ReSharper restore RedundantUsingDirective
using FluentSharp.Git;
using FluentSharp.Git.APIs;

//O2Ref:FluentSharp.NGit.dll

namespace O2.XRules.Database.Utils
{		
	public static class _Extra_methods_NGit
	{	
		public static API_NGit git_Clone_or_Pull(this string localFolder, string targetRepo)
		{
			if(localFolder.isGitRepository())
			{
				var nGit = localFolder.git_Open();				
				nGit.pull();
                return nGit;
			}
			return targetRepo.git_Clone(localFolder);			
		}
	}
}