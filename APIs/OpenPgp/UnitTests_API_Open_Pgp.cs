// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;


//O2File:API_OpenPgp.cs
//O2Ref:NUnit.Framework.dll



namespace O2.XRules.Database.APIs
{
	[TestFixture]
    public class UnitTests_API_Open_Pgp
    {    
    	private static IO2Log log = PublicDI.log;

		[Test]
        public void createKey()
        {
        	var openPgp = new API_OpenPgp();
			openPgp.createKey(); 
			//openPgp.moveFilesToFolder(@"C:\O2\_USERDATA\Pgp");
			var file = @"C:\O2\_USERDATA\Pgp\PgpDetails for {0}.xml".format(openPgp.Identity);
			openPgp.save(file); 
			//show.file(file); 
			Assert.That(openPgp.PrivateKey.fileExists(), "PrivateKey file did not exist");
			Assert.That(openPgp.PublicKey.fileExists(), "PublicKey  file did not exist");
			Assert.That(file.fileExists(), "Saved API_OpenPgp file did not exist");			
        }
        
        //TODo: make this generic
        public void encryptAndDecrypt()
        {
        	var testPgpAccount = @"C:\O2\_USERDATA\Pgp\PgpDetails for fgRcFCSvXtGq.xml";
			var testFile = @"C:\O2\_USERDATA\Pgp\TestContentFile"; 
			var testFile_2 = @"C:\O2\_USERDATA\Pgp\TestContentFile_2.txt";
			
			var openPgp = API_OpenPgp.load(testPgpAccount);
			var encryptedFile = openPgp.encryptFile(testFile); 
			Assert.That(encryptedFile.fileExists(), "Encrypted File didn't exist"); 
			
			var decryptedFile = openPgp.decryptFile(encryptedFile);
			
			Assert.That(decryptedFile.fileExists(), "Decrypted File didn't exist"); 			
        }
    	    	    	    	    
    }
}
