<%@ Page Title="" Language="C#" %>
<% @Import Namespace="O2.Kernel.ExtensionMethods" %>
<% @Import Namespace="O2.DotNetWrappers.ExtensionMethods" %>
<% @Import Namespace="O2.Views.ASCX.Ascx.MainGUI" %>
<% @Import Namespace="O2.Views.ASCX.classes.MainGUI" %>
<% @Import Namespace="O2.External.SharpDevelop.ExtensionMethods" %>


<html>
<head runat="server" />
<script runAt="server">
	void sendMessage(string message)
	{
		Response.Write("<h3>{0}</h3>".format(message));
	}
	
	protected void Page_Load(object sender, EventArgs e)
	{		
		var action = Request["action"];
		if (action.valid())
		{			
			switch(action)
			{
				case "LogViewer":
					sendMessage("Opening up Log Viewer");
					O2Gui.open<ascx_LogViewer>("Asp.Net O2 LogViewer", 400,400);
					//.popupWindow().add_Control<>();
					break;
				case "ScriptEditor":	
					sendMessage("Opening up Script Editor");
					"ascx_Simple_Script_Editor.cs.o2".local().compile().executeFirstMethod();   
					break;
				case "QuickDevelopmentGui":	
					sendMessage("Opening up Quick Development Environment");
					"ascx_Quick_Development_GUI.cs.o2".local().compile().executeFirstMethod();   
					break;					
				case "O2DevelopmentEnviroment":	
					sendMessage("Opening up O2 Development Environment");
					"Util - O2 Development Environment.h2".local().compile_H2Script().executeFirstMethod();   
					break;			
				case "AspNetPageRender":	
					sendMessage("Opening up AspNet Page Render Tool");
					//"Tool - AspNet Page Render.h2".local().compile_H2Script().executeFirstMethod();   
					@"C:\O2\O2Scripts_Database\_Scripts\Languages_and_Frameworks\AspNet\Tool - AspNet Page Render.h2".compile_H2Script().executeFirstMethod();   
					break;				
					
					
			}
		}
	}
</script>
<body>
	<hr>
	
	<h2>O2 Server-Side Actions</h2>
	
		<ul> 
			<li><a href="?action=LogViewer">Log Viewer</a></li>
			<li><a href="?action=ScriptEditor">Script Editor</a></li>
			<li><a href="?action=QuickDevelopmentGui">Quick Development Gui</a></li>
			<li><a href="?action=O2DevelopmentEnviroment">O2 Development Environment</a></li>
			<li><a href="?action=AspNetPageRender">AspNet Page Render</a></li>						
			<li><a href="Create_o2_AspNet_Page_Objects.aspx">Create o2 AspNet Page Objects.aspx</a></li>
			
		</ul>			

</body>
</html>		
	
