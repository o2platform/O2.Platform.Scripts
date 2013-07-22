// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Text;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Engines;
//O2Ref:itextsharp.dll 

namespace O2.XRules.Database.APIs
{	
    public class API_RSA
    {    
    	public string PathToPemKey { get;set;}
    	
    	public API_RSA()
    	{
    		PathToPemKey = @"C:\O2\_USERDATA";	// default to this location
    	}
    	
    	public API_RSA(string pathToPemKey)
    	{
    		if (pathToPemKey.valid())
    			PathToPemKey = pathToPemKey;
    	}
    	
    	public  string decryptPasswordUsingPem(string password)
    	{
			try
			{
				var passwordBytes = Convert.FromBase64String(password);  
				AsymmetricCipherKeyPair keyPair; 
				PathToPemKey =  PathToPemKey ?? "Where is the PEM private Key".askUser();
				
				if (PathToPemKey.isFolder())
					PathToPemKey = PathToPemKey.files("*.pem").first();
					
				if (PathToPemKey.valid() && PathToPemKey.fileExists())
				{
					using (var reader = File.OpenText(PathToPemKey))  
				    	keyPair = (AsymmetricCipherKeyPair) new PemReader(reader).ReadObject(); 				
					var decryptEngine = new Pkcs1Encoding(new RsaEngine());
					decryptEngine.Init(false, keyPair.Private); 				 
					var decryptedPassword = Encoding.UTF8.GetString(decryptEngine.ProcessBlock(passwordBytes, 0, passwordBytes.Length)); 				 
					return decryptedPassword; 
				}
				"in decryptPasswordUsingPem, there was no PEM file provided or found".error();
			}
			catch(Exception ex)
			{
				"[API_RSA] in decryptPasswordUsingPem: {0}".error(ex.Message);				
			}
			return null;
		}
    }        
}