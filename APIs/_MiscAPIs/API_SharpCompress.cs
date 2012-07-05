// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Reader.Rar;
using SharpCompress.Reader.Zip;
using SharpCompress.Archive;
using SharpCompress.Archive.Zip;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2Ref:SharpCompress.3.5.dll


namespace O2.XRules.Database.APIs
{
	public class API_SharpCompress
	{
		
	}
	
	public static class API_SharpCompress_ExtensionMethods_Rar
	{
		public static bool unRar(this string rarFile, string targetDir)
		{
			"UnRar Files will be created in folder: {0}".debug(targetDir);
			try
			{
				using (var file = File.OpenRead(rarFile))
					using (var reader = RarReader.Open(file, Options.None))
						while (reader.MoveToNextEntry())	
							if (!reader.Entry.IsDirectory)
							{
								"writing file '{0}'".info(reader.Entry.FilePath); 
								reader.WriteEntryToDirectory(targetDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
							}		
				return true;							
			}
			catch(Exception ex)
			{
				"[rarFile]: {0}".error(ex.Message);	
				return false;
			}						
		}
		
		public static bool download_and_Unrar(this string url, string targetDir)
		{
			if (url.isUri())
			{
				var tmpFile = url.download();
				if (tmpFile.fileExists())
					return tmpFile.unRar(targetDir);
			}
			return false;
		}
		
		public static string download_and_Unrar(this string url)
		{
			var tempDir = url.fileName_WithoutExtension().tempDir();
			if (url.download_and_Unrar(tempDir))
				return tempDir;
			return null;
		}
	}	
	
	public static class API_SharpCompress_ExtensionMethods_Zip
	{
		public static string zip(this string sourceFolder)
		{
			var zipFile = "{0}.zip".format(sourceFolder);
			if (sourceFolder.zip(zipFile))
				return zipFile;
			return null;
		}
		
		public static bool zip(this string sourceFolder, string zipFile)
		{
			var searchPattern = "*.*";
			return sourceFolder.zip(searchPattern, zipFile);
		}
		
		public static bool zip(this string sourceFolder, string searchPattern, string zipFile)
		{
			try
			{				
				zipFile.deleteIfExists();
				
				using (var archive = ZipArchive.Create())
				{
				    archive.AddAllFromDirectory(sourceFolder, searchPattern, SearchOption.AllDirectories);
				    archive.SaveTo(zipFile, CompressionType.Deflate);				    
				}
				return zipFile.fileExists();
			}
			catch(Exception ex)
			{
				"[zip] : {0}".error(ex.Message);
				return false;
			}
		}
		
		public static bool unZip(this string zipFile)
		{			
			var targetFolder = zipFile.parentFolder().pathCombine(zipFile.fileName_WithoutExtension());
			if (targetFolder.dirExists())
			{
				"[unZip] default unzip directory already existed, so using a randon one: {0}".error(targetFolder);
				targetFolder = targetFolder.append("_{0}".format(5.randomLetters()));
			}
			return zipFile.unZip(targetFolder);
		}
		
		public static bool unZip(this string zipFile, string targetDir)
		{			
			"UnZip Files will be created in folder: {0}".debug(targetDir);
			try
			{
				using (var file = File.OpenRead(zipFile))
				{
					using (var reader = ZipReader.Open(file,null, Options.None))
						while (reader.MoveToNextEntry())	
							if (!reader.Entry.IsDirectory)
							{
								"writing file '{0}'".info(reader.Entry.FilePath); 
								reader.WriteEntryToDirectory(targetDir, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
							}		
					file.Close();		
				}
				return true;							
			}
			catch(Exception ex)
			{
				"[zipFile]: {0}".error(ex.Message);	
				return false;
			}						
		}
	}
}