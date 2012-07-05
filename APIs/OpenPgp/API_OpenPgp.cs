// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO;

using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2Ref:itextsharp.dll 


namespace O2.XRules.Database.APIs
{
	
// parts of these CreateKey methods are based on the examples from http://downloads.bouncycastle.org/csharp/bccrypto-net-1.6.1-src.zip		
		
    public class API_OpenPgp
    {    
    	public string PublicKey { get; set; }
    	public string PrivateKey { get; set; } 
    	public string Identity { get; set; }
    	public string PassPhrase { get; set; }        	
    	
    	public API_OpenPgp()
    	{
    	}
    	    	
    	
    	public static API_OpenPgp load(string file)
    	{
    		return file.deserialize<API_OpenPgp>();
    	}
    }
    
    public static class API_OpenPgp_ExtensionMethods
    {    
    	public static API_OpenPgp openPgp(this string pathToOpenPgpFile)
    	{
    		return pathToOpenPgpFile.load<API_OpenPgp>();
    	}
    	
    	public static string save(this API_OpenPgp openPgp)
    	{
    		return openPgp.save("_API_OpenPgp.xml".tempFile());
    	}
    	
    	public static string save(this API_OpenPgp openPgp, string pathToSave)
    	{
    		if (openPgp.serialize(pathToSave))
    			return pathToSave;
    		return "";
    	}
    	
    	public static API_OpenPgp moveFilesToFolder(this API_OpenPgp openPgp, string targetFolder)
    	{
    		targetFolder.createDir(); // make sure it exists
    		openPgp.PrivateKey = Files.Copy(openPgp.PrivateKey,targetFolder);
    		openPgp.PublicKey = Files.Copy(openPgp.PublicKey,targetFolder);
    		return openPgp;
    	}
    	
    	public static API_OpenPgp fixOpenPgpKeysPaths(this API_OpenPgp openPgp, string file)
    	{    	
			// see if we need to resolve the paths
			if (openPgp.PrivateKey.fileExists().isFalse())
			{
				var localPrivateKey = file.directoryName().pathCombine(openPgp.PrivateKey);
				if (localPrivateKey.fileExists())
					openPgp.PrivateKey = localPrivateKey;
			}
			
			if (openPgp.PublicKey.fileExists().isFalse())
			{
				var localPublicKey = file.directoryName().pathCombine(openPgp.PublicKey);
				if (localPublicKey.fileExists())
					openPgp.PublicKey = localPublicKey;
			}					
			return openPgp;
		}

    }
    
    
    public static class API_OpenPgp_ExtensionMethods_CreateKey
    {	    		
    	public static string defaultSufix_PrivateKey = "_privateKey.asc";
    	public static string defaultSufix_PublicKey = "_publicKey.asc";
    	
    	public static string createKey(this API_OpenPgp openPgp)
    	{    	    
    		string random_Identity = 12.randomLetters();
    		string random_passPhrase = 64.randomLetters();    							
    		return openPgp.createKey(random_Identity, random_passPhrase);
    	}
    	
    	public static string createKey(this API_OpenPgp openPgp, string identity)
    	{
    		string random_passPhrase = 64.randomLetters();
    		return openPgp.createKey(identity, random_passPhrase);
    	}
    	
    	public static string createKey(this API_OpenPgp openPgp, string identity, string passPhrase)
    	{
    		return openPgp.createKey(identity, passPhrase, "".tempDir());
    	}
    	
    	public static string createKey(this API_OpenPgp openPgp, string identity, string passPhrase, string targetFolder)
    	{
    		targetFolder.createDir(); 	// make sure it is created
			string pathTo_PrivateKey = targetFolder.pathCombine("{0}_{1}".format(identity, defaultSufix_PrivateKey));
    		string pathTo_PublicKey = targetFolder.pathCombine("{0}_{1}".format(identity, defaultSufix_PublicKey));
    		return openPgp.createKey(identity,passPhrase, pathTo_PrivateKey, pathTo_PublicKey);
    	}
    	
    	public static string createKey(this API_OpenPgp openPgp, string identity, string passPhrase, string pathTo_PrivateKey, string pathTo_PublicKey)
    	{    		
    		if (identity.valid().isFalse())
    		{
    			"[API_OpenPgp] there was no value provided for the mandatory field: PGP Identity".error();
    			return null;
    		}
    		IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator("RSA");

            kpg.Init(new RsaKeyGenerationParameters(
				//BigInteger.ValueOf(0x10001), new SecureRandom(), 1024, 25));
				BigInteger.ValueOf(0x10001), new SecureRandom(), 2048, 25));

            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();
            
			Stream privateKey, publicKey;			
			privateKey = File.Create(pathTo_PrivateKey);
            publicKey = File.Create(pathTo_PublicKey);

			new RsaKeyRingGenerator().ExportKeyPair(privateKey, publicKey, kp.Public, kp.Private, identity, passPhrase.ToCharArray(), true);
            
			privateKey.Close();
			publicKey.Close();
			
			openPgp.PrivateKey = pathTo_PrivateKey;
			openPgp.PublicKey = pathTo_PublicKey;				
			openPgp.Identity =  identity;
			openPgp.PassPhrase = passPhrase;
			
			var openPgpFile = pathTo_PublicKey.directoryName().pathCombine("{0}_OpenPgp.xml".format(identity));
			openPgp.save(openPgpFile);
			"[API_OpenPgp] Pgp mappings for identity '{0}' saved to: {0}".debug(openPgpFile);
			return openPgpFile;
    	}    	    	    
    
	    public sealed class RsaKeyRingGenerator
	    {
	        public RsaKeyRingGenerator()
	        {
	        }
	
			public void ExportKeyPair(
	            Stream                   secretOut,
	            Stream                   publicOut,
	            AsymmetricKeyParameter   publicKey,
	            AsymmetricKeyParameter   privateKey,
	            string                   identity,
	            char[]                   passPhrase,
	            bool                     armor)
	        {
				if (armor)
				{
					secretOut = new ArmoredOutputStream(secretOut);
				}
	
				PgpSecretKey secretKey = new PgpSecretKey(
	                PgpSignature.DefaultCertification,
	                PublicKeyAlgorithmTag.RsaGeneral,
	                publicKey,
	                privateKey,
	                DateTime.UtcNow,
	                identity,
	                SymmetricKeyAlgorithmTag.Cast5,
	                passPhrase,
	                null,
	                null,
	                new SecureRandom()
	                );
	
	            secretKey.Encode(secretOut);
	
				if (armor)
	            {
					secretOut.Close();
					publicOut = new ArmoredOutputStream(publicOut);
	            }
	
	            PgpPublicKey key = secretKey.PublicKey;
	
	            key.Encode(publicOut);
	
				if (armor)
				{
					publicOut.Close();
				}
	        }
		}
	}
		
	
	public static class API_OpenPgp_ExtensionMethods_EncryptText
	{
	
		public static string pgp_EncryptText(this string textToEncrypt, string pathToOpenPgpFile)
		{
			if (pathToOpenPgpFile.fileExists())
			{
				var openPgp = pathToOpenPgpFile.openPgp();					
				openPgp.fixOpenPgpKeysPaths(pathToOpenPgpFile);
				return openPgp.encryptText(textToEncrypt);
			}
			return null;
		}
		
		public static string encryptText(this API_OpenPgp openPgp, string textToEncrypt)
		{
			try
			{ 
				if (openPgp.notNull())
				{
					var tempFile = textToEncrypt.save(); 
					var resultTempFile = openPgp.encryptFile(tempFile); 
					var result = resultTempFile.fileContents();											
					Files.deleteFile(tempFile);	
					Files.deleteFile(resultTempFile);
					return result;
				}
			} 
			catch(Exception ex)
			{
				ex.log("[API_OpenPgp] in EncryptText");				
			}	
			return null;
		}
		
		public static string decryptText(this API_OpenPgp openPgp, string textToEncrypt)
		{
			try
			{ 
				if (openPgp.notNull()) 
				{ 
					var tempFile = textToEncrypt.saveWithExtension(".asc");
					"tempFile: {0}".info(tempFile);
					var resultTempFile = openPgp.decryptFile(tempFile); 
					var result = resultTempFile.fileContents();											
					Files.deleteFile(tempFile);	
					Files.deleteFile(resultTempFile);
					return result;
				}																						
			} 
			catch(Exception ex)
			{
				ex.log("[API_OpenPgp] in DecryptText");				
			}
			return null;

		}
	}
	
	public static class API_OpenPgp_ExtensionMethods_EncryptFile
	{
		public static string encryptFile(this API_OpenPgp openPgp, string fileToEncrypt)
		{
			try
			{				
				var publicKey = openPgp.PublicKey;
				if (publicKey.fileExists().isFalse())
					publicKey = PublicDI.CurrentScript.directoryName().pathCombine(publicKey);
				if (fileToEncrypt.fileExists().isFalse())
				{
					"[API_OpenPgp] in API_OpenPgp signFile, the provided file to encrypt doesn't exist: {0}".error(fileToEncrypt);
					return "";
				}
				var keyIn = File.OpenRead(publicKey);
				var pathToEncryptedFile = fileToEncrypt + ".asc";
	            var fos = File.Create(pathToEncryptedFile);
				EncryptFile(fos, fileToEncrypt, OpenPgp_HelperMethods.ReadPublicKey(keyIn), true, true);			
				fos.Close();			
				return pathToEncryptedFile;			
			}
			catch(Exception ex)
			{
				ex.log("[API_OpenPgp]  in encryptFile");
				return null;
			}						
			
		}
		
		private static void EncryptFile(
            Stream			outputStream,
            string			fileName,
            PgpPublicKey	encKey,
            bool			armor,
            bool			withIntegrityCheck)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            try
            {
                MemoryStream bOut = new MemoryStream();

				PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(
                    CompressionAlgorithmTag.Zip);

				PgpUtilities.WriteFileToLiteralData(
					comData.Open(bOut),
					PgpLiteralData.Binary,
					new FileInfo(fileName));

				comData.Close();

				PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(
					SymmetricKeyAlgorithmTag.Cast5, withIntegrityCheck, new SecureRandom());

				cPk.AddMethod(encKey);

				byte[] bytes = bOut.ToArray();

				Stream cOut = cPk.Open(outputStream, bytes.Length);

				cOut.Write(bytes, 0, bytes.Length);

				cOut.Close();

				if (armor)
				{
					outputStream.Close();
				}
            }
            catch (PgpException e)
            {            
            	e.log("[API_OpenPgp] in EncryptFile");
            /*    Console.Error.WriteLine(e);

                Exception underlyingException = e.InnerException;
                if (underlyingException != null)
                {
                    Console.Error.WriteLine(underlyingException.Message);
                    Console.Error.WriteLine(underlyingException.StackTrace);
                }*/
            }
        }
	}
	
	
	public static class API_OpenPgp_ExtensionMethods_DecryptFile
	{
	
		public static string decryptFile(this API_OpenPgp openPgp, string fileToDecrypt)
		{
			"In Decrypt step".info();
			var privateKey = openPgp.PrivateKey;
			if (privateKey.fileExists().isFalse())
				privateKey = PublicDI.CurrentScript.directoryName().pathCombine(privateKey);
			var passPhrase = openPgp.PassPhrase;
			if (passPhrase.valid().isFalse())
				passPhrase = "What is the passphase to unlock the Private Key?".askUser();
				
			if (fileToDecrypt.fileExists().isFalse())
			{
				"in API_OpenPgp signFile, the provided file to decrypt doesn't exist: {0}".error(fileToDecrypt);
				return "";
			}
			var keyIn = File.OpenRead(privateKey);			
			var fin = File.OpenRead(fileToDecrypt);		
			var originalExtension = Path.GetFileNameWithoutExtension(fileToDecrypt).extension();
			if (originalExtension.valid().isFalse())
				originalExtension = ".out";
			var pathToDecryptedFile = fileToDecrypt + originalExtension; // new FileInfo(args[1]).Name + ".out"
			DecryptFile(fin, keyIn, passPhrase.ToCharArray(), pathToDecryptedFile);
			fin.Close();
			keyIn.Close();	
			//var pathToEncryptedFile = fileToEncrypt + ".asc";
            //var fos = File.Create(pathToEncryptedFile);
			//EncryptFile(fos, fileToEncrypt, OpenPgp_HelperMethods.ReadPublicKey(keyIn), true, true);			
			//fos.Close();			
			return pathToDecryptedFile;
		}
		
		        /**
        * decrypt the passed in message stream
        */
        private static void DecryptFile(
            Stream	inputStream,
            Stream	keyIn,
			char[]	passwd,
			string	pathToDecryptedFile)		//DC 
		{
			try
			{
	            inputStream = PgpUtilities.GetDecoderStream(inputStream);
	
	            try
	            {
	                PgpObjectFactory pgpF = new PgpObjectFactory(inputStream);
	                PgpEncryptedDataList enc;
	
	                PgpObject o = pgpF.NextPgpObject();
	                //
	                // the first object might be a PGP marker packet.
	                //
	                
	                if (o is PgpEncryptedDataList)
	                {
	                    enc = (PgpEncryptedDataList)o;
	                }
	                else
	                {
	                    enc = (PgpEncryptedDataList)pgpF.NextPgpObject();
	                }
	
	                //
	                // find the secret key
	                //
	                PgpPrivateKey sKey = null;
	                PgpPublicKeyEncryptedData pbe = null;
					PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(
						PgpUtilities.GetDecoderStream(keyIn));
	
					foreach (PgpPublicKeyEncryptedData pked in enc.GetEncryptedDataObjects())
	                {
	                    sKey = OpenPgp_HelperMethods.FindSecretKey(pgpSec, pked.KeyId, passwd);
	
	                    if (sKey != null)
	                    {
	                        pbe = pked;
	                        break;
	                    }
	                }
	
					if (sKey == null)
	                {
	                    throw new ArgumentException("secret key for message not found.");
	                }
	
	                Stream clear = pbe.GetDataStream(sKey);
	
	                PgpObjectFactory plainFact = new PgpObjectFactory(clear);
	
	                PgpObject message = plainFact.NextPgpObject();
					
					PgpObjectFactory pgpFact = null;
					
	                if (message is PgpCompressedData)
	                {
	                    PgpCompressedData cData = (PgpCompressedData)message;
	                    pgpFact = new PgpObjectFactory(cData.GetDataStream());
	
	                    message = pgpFact.NextPgpObject();
	                }
					
					if (message is PgpOnePassSignatureList)		// DC
					{											// DC
						message = pgpFact.NextPgpObject();		// DC
					}											// DC
					
	                if (message is PgpLiteralData)
	                {	                	
	                    PgpLiteralData ld = (PgpLiteralData)message;
							
						Stream fOut = File.Create(pathToDecryptedFile);		//DC (modified to use the name provided in pathToDecryptedFile
						Stream unc = ld.GetInputStream();
						Streams.PipeAll(unc, fOut);						
						fOut.Close();
	                }
	                else if (message is PgpOnePassSignatureList)
	                {
	                    "[API_OpenPgp][DecryptFile] encrypted message contains a signed message - not literal data.".error();	                  
	                    return ;	                    
	                }
	                else
	                {
	                    "[API_OpenPgp][DecryptFile] message is not a simple encrypted file - type unknown.".error();
	                    return;
	                }
	
	                if (pbe.IsIntegrityProtected())
	                {
	                    if (!pbe.Verify())
	                    {
	                        "[API_OpenPgp][DecryptFile] message failed integrity check".error();
	                    }
	                    else
	                    {
	                        "[API_OpenPgp][DecryptFile] message integrity check passed".debug();
	                    }
	                }
	                else
	                {
	                    "[API_OpenPgp][DecryptFile] no message integrity check".error();
	                }
	            }
	            catch (PgpException e)
	            {
	            	e.log("[API_OpenPgp] in DecryptFile: " + e.StackTrace );
	            	
	                /*Console.Error.WriteLine(e);
	
	                Exception underlyingException = e.InnerException;
	                if (underlyingException != null)
	                {
	                    Console.Error.WriteLine(underlyingException.Message);
	                    Console.Error.WriteLine(underlyingException.StackTrace);
	                }*/
	            }
			}
			catch (Exception ex)
			{
				ex.log("[API_OpenPgp] in DecryptFile  : " + ex.StackTrace );
			}
        }

	}
	
	//need to check this code out to make sure it is working as expected
	/*
	public static class API_OpenPgp_ExtensionMethods_SignFile
	{
	
		public static string signFile(this API_OpenPgp openPgp, string fileToSign)
		{
			if (fileToSign.fileExists().isFalse())
			{
				"in API_OpenPgp signFile, the provided file to sign doesn't exist: {0}".error(fileToSign);
				return "";
			}
			var keyIn = File.OpenRead(openPgp.PrivateKey);
			var pathToSignedFile = fileToSign + ".asc";
			var fos = File.Create(pathToSignedFile);
			SignFile(fileToSign, keyIn, fos, openPgp.PassPhrase.ToCharArray(), true, true);
			fos.Close();
			return pathToSignedFile;
		}
		
		//TODO: this part is not working with the file created by the SignFile
		public static API_OpenPgp verifyFile(this API_OpenPgp openPgp, string fileToVerify)
		{
			if (fileToVerify.fileExists().isFalse())			
				"in API_OpenPgp verifyFile, the provided file to verify doesn't exist: {0}".error(fileToVerify);
			else
			{			
				var keyIn = File.OpenRead(openPgp.PublicKey);			
				var fos = File.Create(fileToVerify);
				if (VerifyFile(fos, keyIn))			
					"[API_OpenPgp] signature verified.".debug();
				else
	            	"[API_OpenPgp] signature verification failed.".error();
	            	
	            fos.Close();
	        }
			return openPgp;
		}
		
		
		private static bool VerifyFile(Stream inputStream,Stream keyIn)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpObjectFactory			pgpFact = new PgpObjectFactory(inputStream);
            PgpCompressedData			c1 = (PgpCompressedData) pgpFact.NextPgpObject();
            pgpFact = new PgpObjectFactory(c1.GetDataStream());

            PgpOnePassSignatureList		p1 = (PgpOnePassSignatureList) pgpFact.NextPgpObject();
            PgpOnePassSignature			ops = p1[0];
 
            PgpLiteralData				p2 = (PgpLiteralData) pgpFact.NextPgpObject();
            Stream						dIn = p2.GetInputStream();
            PgpPublicKeyRingBundle		pgpRing = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(keyIn));
            PgpPublicKey				key = pgpRing.GetPublicKey(ops.KeyId);
            Stream						fos = File.Create(p2.FileName);

			ops.InitVerify(key);

			int ch;
			while ((ch = dIn.ReadByte()) >= 0)
            {
                ops.Update((byte)ch);
                fos.WriteByte((byte) ch);
            }
            fos.Close();

            PgpSignatureList	p3 = (PgpSignatureList)pgpFact.NextPgpObject();
			PgpSignature		firstSig = p3[0];
            if (ops.Verify(firstSig))                       	
                return true;            
            else            
                return false;            
        }
        
		private static void SignFile(
            string	fileName,
            Stream	keyIn,
            Stream	outputStream,
            char[]	pass,
            bool	armor,
			bool	compress)
        {
            if (armor)
            {
                outputStream = new ArmoredOutputStream(outputStream);
            }

            PgpSecretKey pgpSec = OpenPgp_HelperMethods.ReadSecretKey(keyIn);
            PgpPrivateKey pgpPrivKey = pgpSec.ExtractPrivateKey(pass);
            PgpSignatureGenerator sGen = new PgpSignatureGenerator(pgpSec.PublicKey.Algorithm, HashAlgorithmTag.Sha1);

            sGen.InitSign(PgpSignature.BinaryDocument, pgpPrivKey);
            foreach (string userId in pgpSec.PublicKey.GetUserIds())
            {
                PgpSignatureSubpacketGenerator spGen = new PgpSignatureSubpacketGenerator();
                spGen.SetSignerUserId(false, userId);
                sGen.SetHashedSubpackets(spGen.Generate());
                // Just the first one!
                break;
            }

            Stream cOut = outputStream;
			PgpCompressedDataGenerator cGen = null;
			if (compress)
			{
				cGen = new PgpCompressedDataGenerator(CompressionAlgorithmTag.ZLib);

				cOut = cGen.Open(cOut);
			}

			BcpgOutputStream bOut = new BcpgOutputStream(cOut);

            sGen.GenerateOnePassVersion(false).Encode(bOut);

            FileInfo					file = new FileInfo(fileName);
            PgpLiteralDataGenerator     lGen = new PgpLiteralDataGenerator();
            Stream						lOut = lGen.Open(bOut, PgpLiteralData.Binary, file);
            FileStream					fIn = file.OpenRead();
            int                         ch = 0;

			while ((ch = fIn.ReadByte()) >= 0)
            {
                lOut.WriteByte((byte) ch);
                sGen.Update((byte)ch);
            }

			fIn.Close();
			lGen.Close();

			sGen.Generate().Encode(bOut);

			if (cGen != null)
			{
				cGen.Close();
			}

			if (armor)
			{
				outputStream.Close();
			}
        }               
	}*/


	public class OpenPgp_HelperMethods
	{
		public static PgpSecretKey ReadSecretKey(
            Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

            PgpSecretKeyRingBundle pgpSec = new PgpSecretKeyRingBundle(inputStream);

			//
            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            //

			foreach (PgpSecretKeyRing kRing in pgpSec.GetKeyRings())
            {
                foreach (PgpSecretKey k in kRing.GetSecretKeys())
                {
                    if (k.IsSigningKey)
                    {
                        return k;
                    }
                }
            }

			throw new ArgumentException("Can't find signing key in key ring.");
        }
        
		public static PgpPublicKey ReadPublicKey(
            Stream inputStream)
        {
            inputStream = PgpUtilities.GetDecoderStream(inputStream);

			PgpPublicKeyRingBundle pgpPub = new PgpPublicKeyRingBundle(inputStream);

			//
            // we just loop through the collection till we find a key suitable for encryption, in the real
            // world you would probably want to be a bit smarter about this.
            //

            //
            // iterate through the key rings.
            //

            foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey k in kRing.GetPublicKeys())
                {
                    if (k.IsEncryptionKey)
                    {
                        return k;
                    }
                }
            }

			throw new ArgumentException("Can't find encryption key in key ring.");
        }

		/**
        * Search a secret key ring collection for a secret key corresponding to
        * keyId if it exists.
        *
        * @param pgpSec a secret key ring collection.
        * @param keyId keyId we want.
        * @param pass passphrase to decrypt secret key with.
        * @return
        */
        public static PgpPrivateKey FindSecretKey(
			PgpSecretKeyRingBundle	pgpSec,
            long					keyId,
            char[]					pass)
        {
            PgpSecretKey pgpSecKey = pgpSec.GetSecretKey(keyId);

			if (pgpSecKey == null)
            {
                return null;
            }

			return pgpSecKey.ExtractPrivateKey(pass);
        }

	}
}
