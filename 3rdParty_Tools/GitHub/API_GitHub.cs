// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:API_GuiAutomation.cs
//O2Ref:White.Core.dll 
//O2Ref:UIAutomationClient.dll 
//O2Ref:UIAutomationTypes.dll
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
    public class API_GitHub
    {    		
    	public API_GitHub()
    	{
    	}    	    	
    	
    }
    
    public static class API_GitHub_Windows_Setup
   	{
   		public static API_GitHub install_TortoiseGit(this API_GitHub gitHub)
   		{
   			var tortoiseGit_DownloadUrl = "http://tortoisegit.googlecode.com/files/Tortoisegit-1.6.5.0-32bit.msi";
			var localMsi = tortoiseGit_DownloadUrl.download();
			"tortoise msi downloaded to: {0}".info(localMsi);
			gitHub.install_TortoiseGit(localMsi);	 	
			return gitHub;
   		}
   		
   		public static API_GitHub install_TortoiseGit(this API_GitHub gitHub, string msiPath)
   		{   	
			var process = msiPath.startProcess(); 
			var guiAutomation = new API_GuiAutomation(process);  
			var tortoiseGitSetup  = guiAutomation.windows()[0]; 
			//tortoiseGitSetup.bringToFront(); 
			//step 1
			tortoiseGitSetup.button("Next >").mouse().click();  
			//step2  
			tortoiseGitSetup  = guiAutomation.windows()[0];  
			tortoiseGitSetup.radioButton("I accept the terms in the License Agreement").mouse().click() ;
			tortoiseGitSetup.button("Next >").mouse().click();  
			//step3  
			tortoiseGitSetup  = guiAutomation.windows()[0];  
			tortoiseGitSetup.button("Next >").mouse().click();   
			//step4
			tortoiseGitSetup  = guiAutomation.windows()[0];  
			tortoiseGitSetup.button("Next >").mouse().click();   
			//step5
			tortoiseGitSetup  = guiAutomation.windows()[0];  
			tortoiseGitSetup.button("Install").mouse().click();  
			//step6					
			for(int i = 0 ; i< 20; i ++) 
			{
				gitHub.sleep(2000,true); // wait 2 secs and try again 
				tortoiseGitSetup  = guiAutomation.windows()[0];  
				if (tortoiseGitSetup.button("Next >").isNull())
				{
					tortoiseGitSetup.button("Finish").mouse().click();   
					break;
				}						
			}
			return gitHub;
		}
		
		public static API_GitHub unInstall_TortoiseGit(this API_GitHub gitHub)
   		{
   			var tortoiseGit_DownloadUrl = "http://tortoisegit.googlecode.com/files/Tortoisegit-1.6.5.0-32bit.msi";
			var localMsi = tortoiseGit_DownloadUrl.download();
			"tortoise msi downloaded to: {0}".info(localMsi);
			gitHub.unInstall_TortoiseGit(localMsi);	 	
			return gitHub;
   		}

		public static API_GitHub unInstall_TortoiseGit(this API_GitHub gitHub, string msiPath)
		{
			var process = msiPath.startProcess(); 					
			var guiAutomation = new API_GuiAutomation(process);  					
			//step 1
			var tortoiseGitSetup  = guiAutomation.windows()[0];  
			tortoiseGitSetup.button("Next >").mouse().click();  
			//step 2
			tortoiseGitSetup  = guiAutomation.windows()[0];
			tortoiseGitSetup.button("Remove Installation").mouse().click();	 		  		
			//step 3
			tortoiseGitSetup  = guiAutomation.windows()[0];
			tortoiseGitSetup.button("Remove").mouse().click();	 	 	  		
			//step 4					
			for(int i = 0 ; i< 10; i ++)
			{
				gitHub.sleep(2000,true); // wait 2 secs and try again 
				tortoiseGitSetup  = guiAutomation.windows()[0];
				if (tortoiseGitSetup.button("Next >").isNull())
				{
					tortoiseGitSetup.button("Finish").mouse().click();   
					break;
				}						
			}
			return gitHub;
		}
		
		public static API_GitHub putty_generateKeys(this API_GitHub gitHub)
		{			
			var keyName = "What is Key's name".askUser();
			if (keyName.valid().isFalse())
				keyName = 4.randomLetters();
			var keyPassPhrase = "What is Key's passphrase".askUser();	
			var targetDir = "Where do you want to save the generated keys".askUser();	
			if (targetDir.valid().isFalse())
				targetDir = "_puttyKey".tempDir(false);   
			var publicKey = targetDir.pathCombine(keyName);
			var privateKey = publicKey + "_privateKey.ppk";
			var publicKeyForSSH = publicKey + "_publicKeyForSSH.ppk";
			publicKey+="_publicKey.asc";
			return gitHub.putty_generateKeys(keyPassPhrase, publicKey, publicKeyForSSH,privateKey);
		}
				
		public static API_GitHub putty_generateKeys(this API_GitHub gitHub, string keyPassPhrase, string publicKey,string publicKeyForSSH, string privateKey)
		{
			if (publicKey.inValid() || privateKey.inValid() || publicKeyForSSH.inValid())
			{
				"the  publicKey, publicKeyForSSH, privateKey paths needs to be valid".error();
				return gitHub;
			}
			var puttyGenPath = @"C:\Program Files\TortoiseGit\bin\puttygen.exe";
			if (puttyGenPath.fileExists().isFalse())
			{
				"Error: could not find puttyGen in TortoiseGit folder: {0}".error(puttyGenPath);
				return gitHub;
			}
			var process = puttyGenPath.startProcess(); 
			var puttyGen = new API_GuiAutomation(process);   
			var window = puttyGen.windows()[0];
			var generateButton = window.button("Generate").click();
			/// lets move the mouse a bit to create some randomness for PuttyGen
			window.mouse();
			generateButton.mouse();
			window.mouse();
			generateButton.mouse();
			window.mouse();
			//once the key is generated we need to put in the passphrase
			if (keyPassPhrase.inValid() )
			{
				window.textBox("Key passphrase:").set_Text(keyPassPhrase ?? "");
				window.textBox("Confirm passphrase:").set_Text(keyPassPhrase ?? "");  
			}
						
			//Saving public Key for SSH
			var keyForOpenSSH = window.textBoxes()[0].get_Text();
			keyForOpenSSH.saveAs(publicKeyForSSH);
			
			//Saving private Key
			window.button("Save private key").mouse().click(); 
			var warning = puttyGen.window("PuTTYgen Warning");  
			if (warning.notNull())
				warning.button("Yes").mouse().click();
			var saveAsWindow = puttyGen.window("Save private key as:");
			saveAsWindow.textBox("File name:").set_Text(privateKey); 
			saveAsWindow.button("Save").mouse().click(); 
			
			//Saving public Key
			window.button("Save public key").mouse().click(); 
			warning = puttyGen.window("PuTTYgen Warning");  
			if (warning.notNull())
				warning.button("Yes").mouse().click();

			saveAsWindow = puttyGen.window("Save public key as:");
			saveAsWindow.textBox("File name:").set_Text(publicKey); 
			saveAsWindow.button("Save").mouse().click();
			
			process.stop();
			
			"The keys were saved to {0} and {1}: {0}".info(publicKey, privateKey);
			
			return gitHub;
		}
		
		public static API_GitHub gitClone(this API_GitHub gitHub)
		{
			var url = "What is the git clone url?".askUser();
			var targetDir = "What is the target directory?".askUser();
			var privateKeyFile = "Where is the private Key?".askUser();
			gitHub.gitClone(url, targetDir, privateKeyFile);
			return gitHub;
		}
		public static API_GitHub gitClone(this API_GitHub gitHub, string url, string targetDir, string privateKeyFile)
		{
			if (targetDir.inValid())
				targetDir = "".tempDir();
			var guiAutomation = new API_GuiAutomation();

			if (guiAutomation.desktopWindow("Git clone").isNull())  // see if there is already an 'Git clone' window 
			{
				//Create a form window with a webbrowser that will open a local folder
				var windowName = "This is Git Clone target folder  (id:{0})".format(3.randomNumbers());
				var topPanel = O2Gui.open<Panel>(windowName,600,200 );
				var webBrowser = topPanel.add_WebBrowser_Control();
				webBrowser.open(targetDir);
				webBrowser.mouse_MoveTo_WinForm();  
				
				// get the form and right click on it
				var window = guiAutomation.desktopWindow(windowName);
				guiAutomation.mouse().click().rightClick(); 
			
				// get the context menu and click on the 'Git Clone...' menu button
				var contextMenu =  guiAutomation.getContextMenu();
				contextMenu.menu("Git Clone...").mouse().click();
			}
			// get the Git Clone window
			var gitClone = guiAutomation.desktopWindow("Git clone", 10);
			gitClone.bringToFront();
			// get a reference to the textboxes we will need to populate
			var url_TextBox = gitClone.textBox("Url: ");
			var directory_TextBox = gitClone.textBox("Directory:");
			var puttyKey_TextBox = gitClone.textBoxes().id(1571);
			if (url.valid())
				url_TextBox.set_Text(url); 
			if (privateKeyFile.valid() && privateKeyFile.fileExists())
				puttyKey_TextBox.set_Text(privateKeyFile);
			//directory_TextBox.set_Text(targetDir);   // no need to do this since it is added when we use the context menu
			
			// Click button
			gitClone.button("OK").mouse().click();			
			
			return gitHub;
		}
	}
}