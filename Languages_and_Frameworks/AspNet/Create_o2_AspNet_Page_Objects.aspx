<%@ Page Language="C#"%>
<% @Import Namespace="O2.XRules.Database.Languages_and_Frameworks.DotNet" %>
<% @Import Namespace="O2.Kernel" %>
<% @Import Namespace="O2.Kernel.ExtensionMethods" %>
<% @Import Namespace="O2.DotNetWrappers.ExtensionMethods" %>

<% @Import Namespace="O2.External.SharpDevelop.ExtensionMethods" %>
<% @Import Namespace="O2.Views.ASCX.ExtensionMethods" %>


<html>
	<head>
		<title>Create O2 AspNet_Page objects</title>
	</head>
	<body>
		<h1> Creating AspNnet_Page objects</h1>
		for current website
	<%		

		Response.Flush();		
		
		//O2Gui.open<System.Windows.Forms.Panel>("Util - LogViewer", 400,140).add_LogViewer().bringToFront();
		
		var binFolder = AppDomain.CurrentDomain.RelativeSearchPath;
		var targetFolder = binFolder.pathCombine("_Saved_AspNetPage_Objects").createDir();
		Response.Write("<h3>Files will be saved at: {0}</h3>".format(targetFolder));
		
		//show.info(Request);
		var rootFolder = AppDomain.CurrentDomain.BaseDirectory;
		var server = "localhost";

		Action<string> processPage =
			(page)=>{									
						"Processing page: {0}".info(page);
						try
						{
							var aspNetPage  = new AspNet_Page(rootFolder, "/", server);		
							aspNetPage.Store_AspNet_SourceCode = true;
							aspNetPage.Store_Compiled_AspNet_SourceCode = true;
							aspNetPage.parseAspNetPage(page);     						
							var targetPage = targetFolder.pathCombine(page.safeFileName()+".xml");
							if (aspNetPage.saveAs(targetPage))
								Response.Write("processed: {0}<br/>".format(page));						
						}
						catch(Exception ex)
						{
							ex.log();
							Response.Write("ERROR processing: {0} : {1} <br/>".format(page, ex.Message));						
						}
					};							
		
		
		foreach(var file in rootFolder.files(true, "*.aspx", "*.ascx","*.asmx"))
		{
			var virtualPath = "/" + file.remove(rootFolder).replace("\\","/");			
			processPage(virtualPath);			
			Response.Flush();		
		}
		var pageName = "/a.aspx";
		//processPage(pageName);
		//PublicDI.log.showMessageBox("click 2");
		//Response.Write(AppDomain.CurrentDomain.RelativeSearchPath);
	%>
	<hr/>
	</body>
</body>