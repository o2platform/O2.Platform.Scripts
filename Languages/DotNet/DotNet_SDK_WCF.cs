using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//O2Ref:MS_SDK_SvcUtil.exe

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class DotNet_SDK_WCF
    {
    	public string SvnUtil_Exe { get; set;}
    	public string TargetDirectory { get; set; }
    	public string OutputTarget { get; set; }    		// this should be an Enum with the 3 available options: code, metadata or xmlSerializer.
    	public string MetadataDocumentPath { get; set; }

		//modified
		
		public DotNet_SDK_WCF()
		{
			SvnUtil_Exe = PublicDI.config.CurrentExecutableDirectory.pathCombine("MS_SDK_SvcUtil.exe");
			metadata();
		}
		
		public DotNet_SDK_WCF metadata()
		{
			OutputTarget = "metadata";
			return this;
		}
		
		public DotNet_SDK_WCF code()
		{
			OutputTarget = "code";
			return this;
		}
		
		public DotNet_SDK_WCF xmlSerializer()
		{
			OutputTarget = "xmlSerializer";
			return this;
		}				
		
		public string svnUtil_Execute(string target)
		{
			var parameters = "";
			if (target.valid())
			{				
				if (TargetDirectory.valid())
					parameters = " /directory:\"{0}\" {1}".format(TargetDirectory, parameters);
				if (OutputTarget.valid())	
					parameters = " /target:{0} {1}".format(OutputTarget, parameters);
					
				parameters += "\"" + target + "\"";
			}			
			"Executing .NET SDK's SvcUtil.exe tool with parameters: {0}".info(parameters);
			return Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(SvnUtil_Exe, parameters);
		}			
       
    }
    
    public static class DotNet_SDK_WCF_SvnUtil
    {
    	
    	public static string svnUtil_Help(this DotNet_SDK_WCF sdkWCF)
		{
			return sdkWCF.svnUtil_Execute("");
		}
    }
}
