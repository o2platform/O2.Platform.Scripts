// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Interfaces;
using FluentSharp.REPL.Utils;
using O2.Views.ASCX.MerlinWizard;
using NUnit.Framework; 
using Merlin;
using O2.Views.ASCX.MerlinWizard.O2Wizard_ExtensionMethods;

//O2File:O2Wizard.cs

//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:merlin.dll
//O2Ref:nunit.framework.dll
//O2Ref:O2_Core_XRules.dll
//O2Ref:O2_External_SharpDevelop.dll

namespace O2.XRules.Database.Utils
{	
	[TestFixture]
	[O2Wizard]
	public class Workflow_BackupFolder
	{
		private static IO2Log log = PublicDI.log;

        [Test]
        public string runWizard_BackupFolder()
        {
            //return runWizard_BackupFolder(PublicDI.config.CurrentExecutableDirectory);
            //return runWizard_BackupFolder(XRules_Config.PathTo_XRulesDatabase_fromO2);
            return runWizard_BackupFolder(PublicDI.config.LocalScriptsFolder);
            
            //
        }

        public string runWizard_BackupFolder(string startFolder)
        {
            var targetFolder = Path.Combine(PublicDI.config.O2TempDir, "..\\_o2_Backups");
            Files.checkIfDirectoryExistsAndCreateIfNot(targetFolder);
            return runWizard_BackupFolder(startFolder, targetFolder);
        }

        public string runWizard_BackupFolder(string startFolder, string targetFolder)
        {
            var o2Wizard = new O2Wizard("Backup folder: " + startFolder);            
            o2Wizard.Steps.add_Directory("Choose Directory To Backup", startFolder);
            o2Wizard.Steps.add_Directory("Choose Directory To Store Zip file", targetFolder);
            o2Wizard.Steps.add_Action("Confirm backup action", confirmBackupAction);
            o2Wizard.Steps.add_Action("Backing up files", executeTask);
            o2Wizard.start();            
            return "ok";
        }
		
		public void confirmBackupAction(IStep step)
		{			
			O2Thread.mtaThread(
			()=> {
				step.set_Text("");				
				var sourceDirectory = step.getPathFromStep(0);
				var targetDirectory = step.getPathFromStep(1);
				var targetFile = calculateTargetFileName(sourceDirectory, targetDirectory);
				step.append_Text("You are about to create a backup of the folder: {1}{1}\t{0} {1}{1} ", sourceDirectory, Environment.NewLine);
                step.append_Text("into the file: {1}{1}\t{0}{1}{1}", targetFile, Environment.NewLine);
                step.append_Text("Do you want to processed");
			});
		}
		
		
		public void executeTask(IStep step)
		{			
			O2Thread.mtaThread(
			()=> {
			
				var sourceDirectory = step.getPathFromStep(0);
				var targetDirectory = step.getPathFromStep(1);
				var targetFile = calculateTargetFileName(sourceDirectory, targetDirectory);
								
				step.allowNext(false);
				step.allowBack(false);						
				step.append_Line(" .... creating zip file ....");
				
				new zipUtils().zipFolder(sourceDirectory, targetFile);
				if (File.Exists(targetFile))
					step.append_Line("File Created: {0}", targetFile);	
				else
					step.append_Line("There was a problem creating the file: {0}", targetFile);	
				step.append_Line("all done");
				step.allowNext(true);
				step.allowBack(true);									
			});		
		}
		
		public string calculateTargetFileName(string sourceDirectory, string targetDirectory)
		{
			var filename = string.Format("O2 Backup for ({0}) done on ({1}).zip",sourceDirectory, DateTime.Now.ToString());
			filename  = Files.getSafeFileNameString(filename);
			return Path.Combine(targetDirectory, filename + ".zip");
		}						
	}		
}
