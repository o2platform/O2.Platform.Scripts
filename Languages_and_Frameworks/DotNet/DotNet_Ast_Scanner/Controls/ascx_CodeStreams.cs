// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.XRules.Database.Utils;
using O2.XRules.Database.Languages_and_Frameworks.DotNet;

//O2File:Saved_MethodStream.cs


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	public class test_ascx_CodeStreams
	{		
		public void launchGui()
		{			
			var ascxCodeStreams = "testing ascx_CodeStreams".showAsForm<ascx_CodeStreams>(1000,600);			
			ascxCodeStreams.buildGuiToViewFolderContents();						
		}
	}
	

	public class ascx_CodeStreams : Control
	{
	
		public Saved_MethodStream savedMethodStream;
		public ascx_SourceCodeViewer codeViewer;
		public TreeView codeStreams;
		public TreeView codeStreamViewer;
		
		public List<Saved_MethodStream> AllSavedMethodStreams { get; set; }
		public Dictionary<string,List<Saved_MethodStream>> AllSavedMethodStreams_MappedByFileName { get; set; }
		
		public ascx_CodeStreams()
		{
			this.Width = 700;
			this.Height = 500;
			buildGui();
		}
		
		public ascx_CodeStreams buildGui() 
		{
			codeViewer = this.add_SourceCodeViewer();
			codeStreams = codeViewer.insert_Right().add_GroupBox("All Code Streams").add_TreeView();
			codeStreamViewer = codeStreams.parent().insert_Below().add_GroupBox("Selected Code Stream").add_TreeView(); 
			//var codeStreamViewer = topPanel.insert_Right().add_TreeView();
			
			Action<TreeNode, CodeStreamPath> add_CodeStreamPath = null;
			
			add_CodeStreamPath = 
				(treeNode, codeStreamPath)=>{
												var newNode = treeNode.add_Node(codeStreamPath);
												foreach(var childPath in codeStreamPath.CodeStreamPaths)
													add_CodeStreamPath(newNode, childPath);
											};
											
			Action<TreeView, CodeStreamPath> showCodeStreamPath= 
				(treeView, codeStreamPath)=>{																		
												treeView.clear(); 
												add_CodeStreamPath(treeView.rootNode(), codeStreamPath);
												treeView.expandAll();
												treeView.selectFirst();
											};
			
			Action<ascx_SourceCodeEditor, CodeStreamPath, bool> colorCodePath = 
				(codeEditor, codeStreamPath, clearMarkers)=>
					{												
						if (codeEditor.getSourceCode().inValid() || codeStreamPath.Line == 0 && codeStreamPath.Column ==0)
							return;						
						try
						{
							if (clearMarkers)
							{
								codeEditor.clearMarkers(); 								
								codeEditor.caret(codeStreamPath.Line,codeStreamPath.Column);
							}
							codeEditor.selectTextWithColor( codeStreamPath.Line,
															codeStreamPath.Column,
															codeStreamPath.Line_End,
															codeStreamPath.Column_End);
							codeEditor.refresh(); 										
						}
						catch(Exception ex)
						{
							ex.log();
						}					
				  	};
			
			Action<ascx_SourceCodeEditor, List<CodeStreamPath>> colorCodePaths = 
				(codeEditor, codeStreamPaths)=> {
													foreach(var codeStreamPath in codeStreamPaths)
														colorCodePath(codeEditor, codeStreamPath,false); 
											    };
				
			Action<TreeView,ascx_SourceCodeEditor> set_AfterSelect_SyncWithCodeEditor = 
				(treeView, codeEditor)=>{
								treeView.afterSelect<CodeStreamPath>(
										(codeStreamPath)=> colorCodePath(codeEditor, codeStreamPath,true ) );
			
							};
			
			
			set_AfterSelect_SyncWithCodeEditor(codeStreams, codeViewer.editor()); 
			set_AfterSelect_SyncWithCodeEditor(codeStreamViewer, codeViewer.editor());
			
			codeStreams.afterSelect<CodeStreamPath>(
				(codeStreamPath)=> showCodeStreamPath(codeStreamViewer, codeStreamPath));											
					
			
			codeStreams.beforeExpand<CodeStreamPath>(
				(treeNode, codeStreamPath)=>{
												treeNode.add_Nodes(codeStreamPath.CodeStreamPaths, (codeStream) => codeStream.CodeStreamPaths.size() > 0 );
											});
			
			
			codeViewer.onClick(
				()=>{ 
						if (savedMethodStream.notNull())
						{	
							codeViewer.editor().clearMarkers();  
							codeStreamViewer.clear();
							codeStreams.clear();
							var line = codeViewer.caret().Line + 1; 				
							var column = codeViewer.caret().Column + 1;  							
							CodeStreamPath lastMatch = null;
							foreach(var codeStreamPath in savedMethodStream.CodeStreams)
							{
								if (codeStreamPath.Line <= line && codeStreamPath.Line_End >= line &&
								    codeStreamPath.Column <= column && codeStreamPath.Column_End >= column)
								    {
								    	codeStreams.add_Node(codeStreamPath);
										lastMatch = codeStreamPath;																									
									}
							}
							if (lastMatch.notNull())
							{								
								showCodeStreamPath(codeStreamViewer, lastMatch);
								var codeStreamPaths = (from node in codeStreamViewer.allNodes() 
													   select (CodeStreamPath)node.get_Tag()).toList();
								colorCodePaths(codeViewer.editor(), codeStreamPaths);   
							}
							else
								refresh_AllCodeStreams_TreeView();
								
								
							
						}
					});
				return this;
		}
		
		public void refresh_AllCodeStreams_TreeView()
		{
			codeStreams.clear();
			codeStreams.add_Nodes(savedMethodStream.CodeStreams, (codeStream) => codeStream.CodeStreamPaths.size() > 0 );  
			codeViewer.editor().clearMarkers(); 	
			O2.DotNetWrappers.DotNet.O2Thread.mtaThread(
				()=>{						
						codeStreams.selectFirst(); 						
					});
		}
		
		public void loadMethodStreamFile(string pathToFile)
		{
			 loadMethodStream(pathToFile.load<Saved_MethodStream>());
		}
		
		public void loadMethodStream(Saved_MethodStream _savedMethodStream)
		{
			if (_savedMethodStream.isNull())
				"provided savedMethodStream object is null".error();
			else
			{
				savedMethodStream = _savedMethodStream;
				codeViewer.set_Text(savedMethodStream.MethodStream );
				//codeViewer.open(savedMethodStream.MethodStream_FileCache );
				refresh_AllCodeStreams_TreeView();				
				//codeStreams.selectFirst();					    				
			}
		}
	}
	
	public static class ascx_CodeStreams_ExtensionMethods
	{
		public static ascx_CodeStreams load_SavedMethodStreams_fromFolder(this ascx_CodeStreams ascxCodeStreams, string folder)
		{
			"Loading all savedMethodStreams from folder: {0}".info(folder);
			return ascxCodeStreams.load_SavedMethodStreams(folder.files());
		}
		
		public static ascx_CodeStreams load_SavedMethodStreams(this ascx_CodeStreams ascxCodeStreams, List<string>files)
		{
			ascxCodeStreams.AllSavedMethodStreams = new List<Saved_MethodStream>();
			"Loading {0} savedMethodStreams: {0}".info(files.size());
			foreach(var file in files)
			{
				var savedMethodStream = file.load<Saved_MethodStream>();
				if (savedMethodStream.notNull())
					ascxCodeStreams.AllSavedMethodStreams.add(savedMethodStream);
				else
					"Could not load savedMethodStream from file: {0}".error(file);
			}
			ascxCodeStreams.calculate_XRefs_AllSavedMethodStreams();
			return ascxCodeStreams;
		}
		
		public static ascx_CodeStreams calculate_XRefs_AllSavedMethodStreams(this ascx_CodeStreams ascxCodeStreams)
		{
			var savedMethodStreams = ascxCodeStreams.AllSavedMethodStreams;
			if (savedMethodStreams.notNull())
			{
				ascxCodeStreams.AllSavedMethodStreams_MappedByFileName = new Dictionary<string,List<Saved_MethodStream>>();
				foreach(var savedMethodStream in savedMethodStreams)
					ascxCodeStreams.AllSavedMethodStreams_MappedByFileName.add(savedMethodStream.RootMethod.Location.File, savedMethodStream);	
			}
			return ascxCodeStreams;
		}
		
		public static ascx_CodeStreams load_by_File_and_MethodName(this ascx_CodeStreams ascxCodeStreams, string file, string methodName)
		{
			if (ascxCodeStreams.AllSavedMethodStreams.isNull())			
				"There are no SavedMethodStreams loaded".error();							
			else
				if (ascxCodeStreams.AllSavedMethodStreams_MappedByFileName.hasKey(file).isFalse())
				{
					"There is no MethodStream loaded for file: {0}".error(file);
				}
				else
				{
					foreach(var savedMethodStream in 	ascxCodeStreams.AllSavedMethodStreams_MappedByFileName[file])
						if (savedMethodStream.RootMethod.Name == methodName)
						{
							"Found a match, loading data for method: {0}".info(savedMethodStream.RootMethod.Signature);
							ascxCodeStreams.loadMethodStream(savedMethodStream);
							return ascxCodeStreams;
						}
						
					"could not find a match for method {0} inside file {1}".error(methodName, file);
				}				
			return ascxCodeStreams;
			
		}
	}
	
	public static class ascx_CodeStreams_GUIs
	{
		public static ascx_CodeStreams buildGuiToViewFolderContents(this ascx_CodeStreams ascxCodeStreams)
		{
			return ascxCodeStreams.buildGuiToViewFolderContents(null);
		}
		
		public static ascx_CodeStreams buildGuiToViewFolderContents(this ascx_CodeStreams ascxCodeStreams, string initialFolder)
		{
			"in buildGuiToViewFolderContents, with initialFolder set to: {0}".info(initialFolder);
			var availableStreamsPanel = ascxCodeStreams.insert_Left(200,"Available Streams");
			
			var sourceFolder = availableStreamsPanel.insert_Above(20)
													.add_Label("Source Folder:")
													.top(2)
												  	.append_TextBox("")
												  	.align_Right(availableStreamsPanel);			
												  	
			Action<string> loadFilesFromFolder =
				(folder)=> {
								var availableStreams = availableStreamsPanel.clear().add_TreeViewWithFilter(folder.files());//, (filePath)=> filePath.fileName());
					
								availableStreams.afterSelect<string>(
									(file)=>{
												var savedMethodStream = file.load<Saved_MethodStream>();
												ascxCodeStreams.loadMethodStream(savedMethodStream);
											});
								availableStreams.selectFirst();			
							};
					
			
											  
			sourceFolder.onDrop(
				(folder) => {
								sourceFolder.set_Text(folder);
								loadFilesFromFolder(folder);
						    });
			
			sourceFolder.onEnter(loadFilesFromFolder);		
			
			if (initialFolder.valid() && initialFolder.dirExists())
				loadFilesFromFolder(initialFolder);
			return ascxCodeStreams;
		}
	}

}
