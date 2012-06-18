// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.ExtensionMethods;
using Amazon.EC2;
using Amazon.EC2.Model;

//O2Ref:AWSSDK.dll
//O2File:ascx_AskUserForLoginDetails.cs	
//O2File:API_RSA.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{	
	public class API_AmazonEC2
	{			
		public ICredential ApiKey { get; set; }
		public string DefaultRegion { get; set; }
		public API_RSA ApiRsa { get; set; }
		
		public int TimerCount = 60;
		public int TimerSleep  = 60 * 1000;
		public string CachedImageListRequest { get; set; }
		
		public API_AmazonEC2() : this(null)
		{
			
		}		
		
		public API_AmazonEC2(ICredential apiKey) 
		{
			DefaultRegion = "us-west-1";//"eu-west-1";
			if (apiKey.isNull())
				apiKey = ascx_AskUserForLoginDetails.ask();
			ApiKey = apiKey;
		}
	}
	
	public static class API_AmazonEC2_ExtensionMethods
	{
		public static List<string> getEC2Regions(this API_AmazonEC2 amazonEC2)
		{
			var ec2Client = new AmazonEC2Client(amazonEC2.ApiKey.UserName, amazonEC2.ApiKey.Password); 
			return (from region in  ec2Client.DescribeRegions(new DescribeRegionsRequest())
		 		      			 			.DescribeRegionsResult.Region
					select region.RegionName).toList();
		}
		
		public static AmazonEC2Client getEC2Client(this API_AmazonEC2 amazonEC2)
		{
			return amazonEC2.getEC2Client(amazonEC2.DefaultRegion);
		}
		
		public static AmazonEC2Client getEC2Client(this API_AmazonEC2 amazonEC2, string region)
		{
			return new AmazonEC2Client(amazonEC2.ApiKey.UserName,
								 	   amazonEC2.ApiKey.Password, new AmazonEC2Config() 
								 	   							{ServiceURL = "http://{0}.ec2.amazonaws.com".format(region)});
		}
		
		
		public static List<Reservation> getReservationsInRegion(this API_AmazonEC2 amazonEC2, string region)
		{
		        "Gettting Reservations in region: {0}".info(region);
				var ec2ClientInRegion = amazonEC2.getEC2Client(region);
				var describesInstance = new DescribeInstancesRequest(); 				
				var reservations = ec2ClientInRegion.DescribeInstances(describesInstance)
													.DescribeInstancesResult
													.Reservation; 				
				return reservations;									  
		}
		
		public static Dictionary<string,List<RunningInstance>> getEC2Instances(this API_AmazonEC2 amazonEC2)
		{
			return amazonEC2.getEC2Instances(true);
		}
		
		public static Dictionary<string,List<RunningInstance>> getEC2Instances(this API_AmazonEC2 amazonEC2,bool onlyShowDefaultRegion)
		{		
			var instances = new Dictionary<string,List<RunningInstance>>();		
			
			var reservations = new List<Reservation>();
			if (onlyShowDefaultRegion)
				reservations.add(amazonEC2.getReservationsInRegion(amazonEC2.DefaultRegion));
			else
				foreach(var region in amazonEC2.getEC2Regions()) 
					reservations.add(amazonEC2.getReservationsInRegion(region));				        
						
			foreach(var reservation in reservations)
					foreach(var runningInstance in reservation.RunningInstance)
						instances.add(reservation.GroupName.Aggregate((a, b) => a + ',' + b),
									  runningInstance); 			
			return instances;			
		}
	}
	
	public static class API_AmazonEC2_ExtensionMethods_RunningInstance
	{
		public static RunningInstance startInstance(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)
	    {							
			"Starting instance with ID: {0}".info(runningInstance.InstanceId);							
			var ec2Client = amazonEC2.getEC2Client(runningInstance.Placement.AvailabilityZone.removeLastChar());
			var result = ec2Client.StartInstances(new StartInstancesRequest()
													.WithInstanceId(runningInstance.InstanceId));
			return runningInstance;
		}

		public static RunningInstance stopInstance(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)
	    {													
			"Stopping instance with ID: {0}".info(runningInstance.InstanceId);							
			var ec2Client = amazonEC2.getEC2Client(runningInstance.Placement.AvailabilityZone.removeLastChar());
			var result = ec2Client.StopInstances(new StopInstancesRequest() 
													.WithInstanceId(runningInstance.InstanceId)); 
	    	return runningInstance;
		}
		
		public static RunningInstance rebootInstance(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)
	    {													
			"Rebooting instance with ID: {0}".info(runningInstance.InstanceId);							
			var ec2Client = amazonEC2.getEC2Client(runningInstance.Placement.AvailabilityZone.removeLastChar());
			var result = ec2Client.RebootInstances(new RebootInstancesRequest() 
													.WithInstanceId(runningInstance.InstanceId)); 
	    	return runningInstance;
		}
	   
		public static RunningInstance showConsoleOut(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)
	    {							
			"Getting Console out instance with ID: {0}".info(runningInstance.InstanceId);			
			var ec2Client = amazonEC2.getEC2Client(runningInstance.Placement.AvailabilityZone.removeLastChar());
			var consoleOutResult = ec2Client.GetConsoleOutput(new GetConsoleOutputRequest()
																    .WithInstanceId(runningInstance.InstanceId));
			var consoleOut = consoleOutResult.GetConsoleOutputResult.ConsoleOutput.Output.base64Decode();	
			consoleOut.showInCodeViewer(".bat");
			return runningInstance;
		}		
	
		public static string getPassword(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)	
		{
			return amazonEC2.getPassword(runningInstance,null);
		}
		public static string getPassword(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance, string pathToPemFile)	
		{	 
			"Tests on  instance with ID: {0}".info(runningInstance.InstanceId);										
			var ec2Client = amazonEC2.getEC2Client(runningInstance.Placement.AvailabilityZone.removeLastChar());
			var passwordResponse = ec2Client.GetPasswordData(new GetPasswordDataRequest().WithInstanceId(runningInstance.InstanceId));
			"raw password data : {0}".info(passwordResponse.GetPasswordDataResult.ToXML());						
			if (amazonEC2.ApiRsa.isNull())
				amazonEC2.ApiRsa = new API_RSA(pathToPemFile);
			var decryptedPassword = amazonEC2.ApiRsa.decryptPasswordUsingPem(passwordResponse.GetPasswordDataResult.PasswordData.Data); 				
			"decrypted password: {0}".info(decryptedPassword);
			return decryptedPassword;			
	   	}
	   		   	
	   	
	   	public static string getRunningInstanceSignature(this API_AmazonEC2 amazonEC2, RunningInstance runningInstance)	
	   	{
			var signature = "{0}  -  {1}  -  {2}  -  {3}  -  {4} ".format(
		   						runningInstance.InstanceId, 
		   						runningInstance.InstanceType, 
		   						runningInstance.IpAddress,
		   						runningInstance.Placement.AvailabilityZone,
		   						runningInstance.InstanceState.Name);
			foreach(var tag in runningInstance.Tag)
				//signature = "{0}= {1}  -  {2}".format(tag.Key, tag.Value, signature); 
				signature = "{1}  -  {2}".format(tag.Key, tag.Value, signature); 
		   	return signature;
		}
	}

	public static class API_AmazonEC2_ExtensionMethods_LaunchImage
	{
		public static RunningInstance launchInstance_Micro(this AmazonEC2Client ec2Client, string imageId, string securityGroup, string keyname)
		{
			return ec2Client.launchInstance(imageId, "t1.micro", securityGroup, keyname);
		}
		public static RunningInstance launchInstance(this AmazonEC2Client ec2Client, string imageId, string instanceType, string securityGroup, string keyname)
		{
			"Launching Amazon EC2 Instance".info();
			var runInstancesRequest = new RunInstancesRequest();

			runInstancesRequest.ImageId = imageId;
			runInstancesRequest.MinCount = 1;
			runInstancesRequest.MaxCount = 1;
			runInstancesRequest.InstanceType = instanceType;
			runInstancesRequest.SecurityGroup.Add(securityGroup);
			runInstancesRequest.KeyName = keyname;
			var runningInstance =  ec2Client.RunInstances(runInstancesRequest)
											.RunInstancesResult
											.Reservation
											.RunningInstance.first();
			"Launched Image with ID: {0}".info(runningInstance.ImageId);
			return runningInstance;
		}
	}
	
	public static class API_AmazonEC2_ExtensionMethods_Images
	{	
		//Visibility
		public static List<Image> @public(this List<Image> images)
		{  
			return images.where(image=>image.Visibility == "Public"); 
		}
		
		public static List<Image> @private(this List<Image> images)
		{  
			return images.where(image=>image.Visibility == "Private"); 
		}
		
		//Architecture
		public static List<Image> _32Bit(this List<Image> images)
		{  
			return images.where(image=>image.Architecture == "i386"); 
		}
		
		public static List<Image> _64Bit(this List<Image> images)
		{  
			return images.where(image=>image.Architecture == "x86_64"); 
		}
		
		//ImageOwnerAlias
		public static List<Image> amazon(this List<Image> images)
		{  
			return images.where(image=>image.ImageOwnerAlias == "amazon"); 
		}
		
		public static List<Image> microsoft(this List<Image> images)
		{  
			return images.where(image=>image.ImageOwnerAlias == "microsoft");  
		}
		
		//Platform
		public static List<Image> windows(this List<Image> images)
		{  
			return images.where(image=>image.Platform == "windows");  
		}		
		
		//ImageId
		public static Image imageId(this List<Image> images, string imageId)
		{  
			return images.where(image=>image.ImageId == imageId).first();  
		}		
		
		//Name
		
		public static Image name(this List<Image> images, string name)
		{  
			return images.where(image=>image.Name == name).first();
		}		
		
		public static List<Image> name_Contains(this List<Image> images, string name)
		{  
			return images.where(image=>image.Name.contains(name));
		}		
		
		public static List<Image> name_RegEx(this List<Image> images, string name)
		{  
			return images.where(image=>image.Name.regEx(name));
		}		
		
		//Description
		public static Image description(this List<Image> images, string description)
		{  
			return images.where(image=>image.Description == description).first();  
		}		
		
		public static List<Image> description_Contains(this List<Image> images, string description)
		{  
			return images.where(image=>image.Description.contains(description));
		}		
		
		public static List<Image> description_RegEx(this List<Image> images, string description)
		{  
			return images.where(image=>image.Description.regEx(description));
		}		
		
		//this quite an expensive operation (3M of data retrieved) - so I added caching support
		public static List<Image> getImagesList(this API_AmazonEC2 amazonEC2, AmazonEC2Client ec2Client)
		{										
			if (amazonEC2.CachedImageListRequest.fileExists())
				return amazonEC2.CachedImageListRequest.load<List<Amazon.EC2.Model.Image>>(); 
				
			var describeImagesRequest = new DescribeImagesRequest(); 				
			"Retrieving ImagesList from Amazon..".info();	
			var images = ec2Client.DescribeImages(describeImagesRequest)
					  .DescribeImagesResult.Image;
			if (images.isNull() || images.size()==0)
			{
				"in getImagesList, there was an error retrieving list (are we online?)".error();				
			}
			else
			{
				amazonEC2.CachedImageListRequest = images.save();
				"The Image List was saved to : {0}".info(amazonEC2.CachedImageListRequest);  
			}
			return images;
		}				
	}
	
	
	public static class API_AmazonEC2_ExtensionMethods_GUIs
	{
		public static API_AmazonEC2 addStopInstanceGui(this API_AmazonEC2 amazonEC2, Panel targetPanel, TreeView treeViewWithInstances)
		{						
			Action startTimer = null;
			Action stopTimer = null;
			var instancesToStop = targetPanel.add_GroupBox("Stop Instance in {0} minutes".format((amazonEC2.TimerCount * amazonEC2.TimerCount / 60))) 
									         .add_TreeView();						
			var timerBar = instancesToStop.insert_Below(20).add_ProgressBar();
			instancesToStop.add_ContextMenu().add_MenuItem("Stop now",true, 
													()=>{
															"Stopping {0} instances now".debug(instancesToStop.nodes().size()); 
															foreach(var node in instancesToStop.nodes())
																amazonEC2.stopInstance((RunningInstance)node.get_Tag());
														})
											 .add_MenuItem("Clear list", ()=>instancesToStop.clear());
			var startTimerLink = instancesToStop.insert_Above(15).add_Link("Add instance to list",0,0, 
													()=>{
															var selectedNode = treeViewWithInstances.selected();
															if (selectedNode.notNull())
															{
																var tag = selectedNode.get_Tag();  
																if (tag is RunningInstance)
																{
																	var selectedInstance = (RunningInstance)tag;
																	var nodeText = "{0} - {1}".format(selectedInstance.InstanceId, selectedInstance.IpAddress);
																	instancesToStop.add_Node(nodeText, selectedInstance);
																}
															}
															//treeViewWithInstances.nodes().showInfo();
														})
											.append_Link("Start timer", ()=>startTimer());  
			var timerEnabled = false;								
			var	stopTimerLink = startTimerLink.append_Link("Stop timer", ()=>stopTimer()).enabled(false);  							
			startTimer = ()=>{											
									"starting timer".info();
									timerEnabled = true;												
									timerBar.maximum(amazonEC2.TimerCount);
									timerBar.value(0);
									startTimerLink.enabled(false);
									stopTimerLink.enabled(true);
									while(timerEnabled && timerBar.Value < amazonEC2.TimerCount)
									{
										"In StopInstances Timer [{0}/{1}], sleeping for {2} seconds".info(timerBar.Value, amazonEC2.TimerCount, amazonEC2.TimerSleep/1000);
										timerBar.sleep(amazonEC2.TimerSleep, false);
										timerBar.increment(1);																										
									}
									if (timerEnabled)
									{													
										"Timer is completed stopping {0} instances now".debug(instancesToStop.nodes().size());
										foreach(var node in instancesToStop.nodes())
											amazonEC2.stopInstance((RunningInstance)node.get_Tag());
									}
									else
										"Timer was stopped so nothing to do".debug();			
									startTimerLink.enabled(true);
									stopTimerLink.enabled(false);

							 };
			stopTimer = ()=>{
								
									"stopping timer".info();
									timerEnabled = false; 
									
									startTimerLink.enabled(true);
									stopTimerLink.enabled(false);
							 };
			targetPanel.onClosed(()=> 	timerEnabled=false);					 
			
			return amazonEC2;
		}						

		public static List<Image> show_ImagesList_In_TreeView(this API_AmazonEC2 amazonEC2)
		{
			return amazonEC2.show_ImagesList_In_TreeView(amazonEC2.getEC2Client());
		}
		
		public static List<Image> show_ImagesList_In_TreeView(this API_AmazonEC2 amazonEC2, AmazonEC2Client ec2Client)
		{
			return amazonEC2.show_ImagesList_In_TreeView(ec2Client, "Amazon EC2 Images List".popupWindow());
		}				
		
		public static List<Image> show_ImagesList_In_TreeView(this API_AmazonEC2 amazonEC2, AmazonEC2Client ec2Client, Control control)
		{				
			var treeView = control.clear().add_TreeView_with_PropertyGrid(false).sort();  	 
			treeView.parent().backColor(System.Drawing.Color.Azure);
			treeView.visible(false);
			Application.DoEvents();
			var imagesList = amazonEC2.getImagesList(ec2Client); 
			
			Func<Amazon.EC2.Model.Image,string> imageName = 
				(image)=> (image.Description.valid())
									? "{0} - {1}".format(image.Description, image.Name)
									: "{0}".format(image.Name).trim();
									
			Action<string> mapByProperty  = 
				(propertyName)=>{				
									var byPropertyNode = treeView.add_Node("by {0}".format(propertyName),"");
									foreach(var distinctPropertyValue in imagesList.Select((image)=>image.property(propertyName).str()).Distinct())
									{										
										var byDistinctPropertyValue = byPropertyNode.add_Node(distinctPropertyValue,"");

										var mappedByImageName = new Dictionary<string, List<Image>>();
										foreach(var imageInProperty in imagesList.Where((image) => image.property(propertyName).str() == distinctPropertyValue))
											mappedByImageName.add(imageName(imageInProperty),imageInProperty);																				
											
										foreach(var mappedData in mappedByImageName)
										{
											if (mappedData.Value.size() > 1)
												byDistinctPropertyValue.add_Node("{0}".format(mappedData.Key,mappedData.Value.size()))
														      		   .add_Nodes(mappedData.Value, imageName);
											else
												byDistinctPropertyValue.add_Node(imageName(mappedData.Value.first()),mappedData.Value.first());
										}			     
									}													  			
								};
			mapByProperty("Visibility");
			mapByProperty("ImageOwnerAlias");
			mapByProperty("Platform"); 
			mapByProperty("Architecture"); 
			"Completed processing show_ImagesList_In_TreeView".info();
			if (treeView.nodes().size()>0)
				treeView.backColor(System.Drawing.Color.White); 
			treeView.visible(true);				
			return imagesList;
		}
	}
	
}