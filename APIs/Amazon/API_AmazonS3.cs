// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Utils;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using O2.XRules.Database.Utils;

using Amazon.S3.Model;
using Amazon.S3;
using Amazon;

//O2File:ISecretData.cs
//O2File:SecretData_ExtensionMethods.cs
//O2File:ascx_AskUserForLoginDetails.cs

//O2Ref:AWSSDK.dll

namespace O2.XRules.Database.APIs
{	
	public class API_AmazonS3
	{			
		public Credential Credential { get; set; }
		public AmazonS3 S3Client { get; set; }
		public bool LoggedIn { get; set; }
		public string OwnerName { get; set; }
		public string OwnerId { get; set; }
		public bool DebugMode { get; set; }
		
		public string defaultCredentialsFile = @"C:\O2\_USERDATA\AmazonS3.xml";
		public string defaultS3Server = "http://s3.amazonaws.com";
		
		public API_AmazonS3()
		{
			DebugMode = true;
		}
		
		public API_AmazonS3 login()
		{
			if (this.Credential!= null)
				return login(this.Credential);
			return login(defaultCredentialsFile);
		}
		
		public API_AmazonS3 login(string credentialsFile)
		{
			loadCredentialsFromFile(credentialsFile,true);
    		return login(this.Credential);    		
    	}
 
    	public API_AmazonS3 login(string username, string password)
    	{
    		return login(new Credential(username, password));
    	}
 
    	public API_AmazonS3 login(Credential credential)
    	{
    		try
    		{
    			debugMsg("[API_AmazonS3]: logging in");
				LoggedIn = false;
    			if (credential == null)
    				"[API_AmazonS3]: no credentials provided, aborting login process".error();
    			else
    			{
    				S3Client = AWSClientFactory.CreateAmazonS3Client(credential.username(), credential.password());
    				return checkLoginStatus();    				
    			}
    		}
    		catch(Exception ex)
    		{
    			ex.log("[API_AmazonS3]: login failed");    		
    		}
    		
    		return this;
    	}
    	
    	public API_AmazonS3 loadCredentialsFromFile(string credentialsFile, bool askUserForCredentialsIfFileDoesntExist)
    	{
    		if (credentialsFile.fileExists())
    			this.Credential = credentialsFile.credential("AmazonS3");    		
    		else if(askUserForCredentialsIfFileDoesntExist)
    			this.Credential = ascx_AskUserForLoginDetails.ask();    		
    		return this;
    	}
    	
    	public API_AmazonS3 checkLoginStatus()
    	{
    		try
			{
				if (this.S3Client.notNull())
				{
					var listBucketResponse = this.S3Client.ListBuckets();
					if (listBucketResponse.notNull())
					{
						OwnerName = listBucketResponse.Owner.DisplayName;
						OwnerId = listBucketResponse.Owner.Id;
						LoggedIn = true;
					}
				}
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in checkLoginStatus()");    		
			}
			return this;
    	
    	}
    	
    	public API_AmazonS3 debugMsg(string debugMessageFormat,params string[] parameters)
    	{
    		DebugMode.ifDebug(debugMessageFormat,parameters);
    		return this;
    	}
    	
    	public API_AmazonS3 debugMsg(string debugMessage)
    	{
    		DebugMode.ifDebug(debugMessage);
    		return this;
    	}
	}
	
	public static class API_AmazonS3_ExtensionMethods
	{
		public static List<String> folders(this API_AmazonS3 amazonS3)
		{
			return amazonS3.s3_Buckets();
		}
		
		public static List<String> s3_Buckets(this API_AmazonS3 amazonS3)
		{
			return (from bucket in amazonS3.s3_BucketsRaw()
					select bucket.BucketName).toList();
		}
		
		public static List<S3Bucket> s3_BucketsRaw(this API_AmazonS3 amazonS3)
		{
			try
			{
				amazonS3.debugMsg("[API_AmazonS3]: retriving buckets list");
				var listBucketsResponse = amazonS3.S3Client.ListBuckets();
				if (listBucketsResponse.notNull())
					return listBucketsResponse.Buckets;				
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in bunckets()");    		
			}
			return new List<S3Bucket>();
		}

		public static List<String> files(this API_AmazonS3 amazonS3, string bucketName)
		{
			return amazonS3.s3_Objects(bucketName);
		}
		public static List<String> s3_Objects(this API_AmazonS3 amazonS3, string bucketName)
		{
			return (from s3_Object in amazonS3.s3_ObjectsRaw(bucketName)
					select s3_Object.Key).toList();
		}
		
		public static List<S3Object> s3_ObjectsRaw(this API_AmazonS3 amazonS3, string bucketName)
		{
			try
			{
				amazonS3.debugMsg("[API_AmazonS3]: retriving objects list for bucket:{0}", bucketName);
				
				var listObjectsRequest = new ListObjectsRequest();
  				listObjectsRequest.WithBucketName(bucketName);
  				return amazonS3.S3Client.ListObjects(listObjectsRequest).S3Objects;   				  								
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in s3_ObjectsRaw()");    		
			}
			return new List<S3Object>();
		}
		
		// note that if the folder already exists and is owned by the current user
		// this will also return the folder name (which could be interpreted has if the folder has been created
		// which is false since the folder already existed)
		public static string  create_Folder(this API_AmazonS3 amazonS3, string newFolderName)
		{
			amazonS3.debugMsg("[API_AmazonS3]: creating folder :{0}", newFolderName);			
			var putBucketResponse = amazonS3.create_S3_Bucket(newFolderName);
			return (putBucketResponse.notNull())
						? newFolderName
						: "";
		}
		
		public static PutBucketResponse  create_S3_Bucket(this API_AmazonS3 amazonS3, string newBucketName)
		{
			try
			{
				var putBucketRequest = new PutBucketRequest();
				putBucketRequest.WithBucketName(newBucketName);
				return amazonS3.S3Client.PutBucket(putBucketRequest);
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in create_S3_Bucket()");    		
			}
			return null;
			
		}	
		
		public static string  delete_Folder(this API_AmazonS3 amazonS3, string folderToDelete)
		{
			amazonS3.debugMsg("[API_AmazonS3]: deleting folder :{0}", folderToDelete);			
			var deleteBucketResponse = amazonS3.delete_S3_Bucket(folderToDelete);
			return (deleteBucketResponse.notNull())
						? folderToDelete
						: "";
		}
		
		public static DeleteBucketResponse delete_S3_Bucket(this API_AmazonS3 amazonS3, string folderToDelete)
		{
			try
			{
				var deleteBucketRequest = new DeleteBucketRequest();
				deleteBucketRequest.WithBucketName(folderToDelete);
				return amazonS3.S3Client.DeleteBucket(deleteBucketRequest);
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in create_S3_Bucket()");    		
			}
			return null;
			
		}	
		
		public static string  delete_File(this API_AmazonS3 amazonS3, string bucket, string fileToDelete)
		{
			amazonS3.debugMsg("[API_AmazonS3]: deleting file :{0}/{1}", bucket, fileToDelete);			
			var deleteObjectResponse = amazonS3.delete_S3_Object(bucket, fileToDelete);
			return (deleteObjectResponse.notNull())
						? fileToDelete
						: "";
		}
		
		public static DeleteObjectResponse delete_S3_Object(this API_AmazonS3 amazonS3,string bucket, string keyToDelete)
		{
			try
			{
				var deleteObjectRequest = new DeleteObjectRequest();
				deleteObjectRequest.WithBucketName(bucket);
				deleteObjectRequest.WithKey(keyToDelete);
				return amazonS3.S3Client.DeleteObject(deleteObjectRequest);
			}
			catch(Exception ex)
			{
				ex.log("[API_AmazonS3]: in delete_S3_Object()");    		
			}
			return null;
			
		}	

		
		public static PutObjectResponse add_S3_Object(this API_AmazonS3 amazonS3, string targetBucket, string fileToAdd)
		{
			var putObjectRequest  = new PutObjectRequest();
			putObjectRequest.WithBucketName(targetBucket);
			putObjectRequest.WithCannedACL(S3CannedACL.PublicRead); // make it publicly available buy default
			putObjectRequest.WithFilePath(fileToAdd);
			return amazonS3.S3Client.PutObject(putObjectRequest);
		}
	}


	public static class API_AmazonS3_ExtensionMethods_WinForms
	{
		public static Thread showFilesInTreeView(this API_AmazonS3 amazonS3, TreeView treeView, string folderName)
		{
			treeView.backColor("LightPink");  
			// we need to run the next code in a separate thread or the treeView.backColor update doesn't show
			if (folderName.valid())
				return O2Thread.mtaThread(
					()=>{
							treeView.clear();
							var files = amazonS3.files(folderName);
							foreach(var file in files)
							{
								var url = "{0}/{1}/{2}".format(amazonS3.defaultS3Server, folderName, file);						
								treeView.add_Node(file, url.uri());
							}
							treeView.backColor("White");						
						});
			return null;
		}
		
		public static T showFileInControl<T>(this API_AmazonS3 amazonS3, T control, Uri uri)
			where T : Control
		{			
			if (uri.isNull())
				return control;			
			control.clear();				
			switch(uri.str().extension())
			{
				case ".gif":
					control.add_PictureBox().loadFromUri(uri);
					break; 
				default:
					var fileContents = uri.getUrlContents();
					control.add_TextArea().set_Text(fileContents);
					break;
			}				
			return control;
		}
		
	}
}