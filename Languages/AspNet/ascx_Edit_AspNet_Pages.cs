// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

//O2File:WatiN_IE_ExtensionMethods.cs

//O2Ref:WatiN.Core.1x.dll
//O2Ref:Interop.SHDocVw.dll

namespace O2.XRules.Database.Languages_and_Frameworks.AspNet
{

	public class ascx_Edit_AspNet_Pages_Test
	{
		public void test()
		{
			"test ascx_Edit_AspNet_Pages".showAsForm<ascx_Edit_AspNet_Pages>(1000,600)
										 .buildGui();
		}
	}
	
    public class ascx_Edit_AspNet_Pages : Control
    {   
    	public string File_Path { get; set;}
		public string Virtual_Path { get; set; }				
		public string CSProj_File {get; set;}
		
		public bool ShowFilesWithExtension_CS {get;set;}
		public bool ShowFilesWithExtension_AscxAsaxConfig {get;set;}	
		public bool OpenSourceCodeFiles {get;set;}
		
		public string Web_Address { get; set; }
			
		public Panel ActionsPanel { get; set; }
		public TreeView AspNetFiles { get; set; }
		public ascx_SourceCodeViewer Aspx_CodeViewer { get; set; }
		public ascx_SourceCodeEditor CSharp_CodeViewer { get; set; }
		
		public WatiN_IE ie { get; set; }
		
    	public ascx_Edit_AspNet_Pages()
    	{
    		this.Virtual_Path = "/";
    		/*this.width(1000);
    		this.height(600);
    		buildGui();*/
    	}
    	
    	public ascx_Edit_AspNet_Pages buildGui()
    	{    		
    		var topPanel = this.add_Panel();
    		topPanel.insert_LogViewer();

			ActionsPanel = topPanel.insert_Above<Panel>(70).add_GroupBox("Actions").add_Panel() ;
			AspNetFiles = topPanel.insert_Left<Panel>(300).add_GroupBox("ASP.NET Files")
													   .add_TreeView()
													   .sort();
			
			 
			ActionsPanel.add_Label("WebRoot",30,0).autoSize()
						.append_TextBox(this.File_Path)
							.width(200)
							.onEnter(
								(text)=>{	
											this.File_Path = text;
											this.loadFiles_FromLocalFolder();
										})
						.append_Label("Url").top(30).autoSize()
						.append_TextBox(this.Web_Address)
							.width(200)
							.onTextChange((text)=> this.Web_Address = text)
						.append_Link("refresh IE", ()=>ie.open_ASync(ie.url()));
						
			Aspx_CodeViewer = topPanel.add_GroupBox("Aspx page").add_SourceCodeViewer();
			CSharp_CodeViewer = Aspx_CodeViewer.parent().insert_Below().add_GroupBox("CSharp page").add_SourceCodeEditor();
			
			this.ie = topPanel.add_IE_SideView();
			ActionsPanel.add_Link("Compile project",5,0,
							()=>{
									if (this.CSProj_File.fileExists())
									{
										var msBuild = @"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"; 
										var parameters = "\"{0}\"".format(CSProj_File);
										msBuild.startProcess(parameters,										
													(log)=> {
																if (log.isNull())
																	return;
																if (log.lower().contains("error "))
																	log.error();
																else
																	if (log.lower().contains("warning "))	
																		log.debug();
																	else
																		log.info();
															});
									}
									else
										"CSProj_File variable is not set of file does not exist: {0}".error(CSProj_File);
								});
						
			
			this.ActionsPanel.add_CheckBox("Show *.cs files", 5,200,
									(value) => { 	
													this.ShowFilesWithExtension_CS = value;
													this.loadFiles_FromLocalFolder();
												}).autoSize();
			
			this.ActionsPanel.add_CheckBox("Show *.ascx, *.asax, *.config files", 5,320,
									(value) => { 	
													this.ShowFilesWithExtension_AscxAsaxConfig = value;
													this.loadFiles_FromLocalFolder();
												}).autoSize();
												
			var splitContainers = this.controls<SplitContainer>(true);												
			this.ActionsPanel.add_CheckBox("show/open source code files", 5,550,
									(value) =>  {
													this.OpenSourceCodeFiles = value;								
													splitContainers[2].panel1Collapsed(value.isFalse());
												}).autoSize().@check();

			setPageViewers();
			
			
			AspNetFiles.add_ContextMenu().add_MenuItem("refresh", ()=> this.loadFiles_FromLocalFolder());
			return this;
    	}
    	
    	public ascx_Edit_AspNet_Pages setPageViewers()
    	{
    		Action<string> showFile =
    				(file)=>{	
							if (OpenSourceCodeFiles)
							{
								Aspx_CodeViewer.open(file);
								var csharpFile = "{0}.cs".format(file);
								if (csharpFile.fileExists())
									CSharp_CodeViewer.open(csharpFile);
								else
								{					
									var folder = file.directoryName();
									var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
									csharpFile = folder.pathCombine("App_Code").pathCombine("{0}.cs".format(fileName));
									if (csharpFile.fileExists())
										CSharp_CodeViewer.open(csharpFile); 
								}
							}							
							var pageUrl = new Uri(this.Web_Address.uri(), file.remove(this.File_Path));					
							this.ie.open_ASync(pageUrl.str()); 
						
						}	;	
    		AspNetFiles.afterSelect<string>(showFile);
    		AspNetFiles.onDoubleClick<string>(showFile);
			
			return this;
    	}
	}
	
	public static class ascx_Edit_AspNet_Pages_ExtensionMethods
	{
		public static ascx_Edit_AspNet_Pages loadFiles_FromLocalFolder(this ascx_Edit_AspNet_Pages editAspNet)
		{
			return editAspNet.loadFiles_FromLocalFolder(editAspNet.File_Path);
		}
		
		public static ascx_Edit_AspNet_Pages loadFiles_FromLocalFolder(this ascx_Edit_AspNet_Pages editAspNet, string folder)
		{
			if (folder.notNull())
			{
				editAspNet.File_Path = folder;
				
				var files = folder.files(true, "*.aspx", "*.asmx","*.ashx","*.svc");
				if (editAspNet.ShowFilesWithExtension_CS)
					files.add(folder.files(true,"*.cs"));
				if (editAspNet.ShowFilesWithExtension_AscxAsaxConfig)
					files.add(folder.files(true,"*.ascx","*.asax", "*.config"));
				editAspNet.AspNetFiles.clear();
				editAspNet.AspNetFiles.add_Nodes(files, (file)=> file.remove(folder));  
			}
			return editAspNet;
		}
	}
	public static class IE_ExtensionMethods
	{
		public static WatiN_IE add_IE_SideView<T>(this T control)
			where T : System.Windows.Forms.Control
		{
			
			var internetView = control.insert_Right().add_GroupBox("Internet Explorer View");
			var addressBar = internetView.parent().insert_Above(20).add_TextBox("Current page:","");
			var htmlCode = internetView.parent().insert_Below(200).add_GroupBox("Html Code").add_SourceCodeEditor(); 			
			var ie = internetView.add_IE();			
			addressBar.onEnter((text)=> ie.open_ASync(text));
			ie.onNavigate(
				(url)=> {
							addressBar.set_Text(url);				
							htmlCode.set_Text(ie.html(), "a.html");
						});			
			return ie;
		}
		
	}
	
	
}
