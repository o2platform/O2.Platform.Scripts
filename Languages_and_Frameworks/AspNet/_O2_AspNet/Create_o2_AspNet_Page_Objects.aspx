<%@ Page Language="C#"%>
<% @Import Namespace="O2.Kernel" %>
<% @Import Namespace="O2.Kernel.ExtensionMethods" %>
<% @Import Namespace="O2.DotNetWrappers.ExtensionMethods" %>
<% @Import Namespace="O2.External.SharpDevelop.ExtensionMethods" %>


<html>
	<head runat="server">
		<title>Create O2 AspNet_Page objects</title>
	</head>
	<body>
		<h1> Creating AspNet_Page objects</h1>
		for current website<br/>
		<%		
	
			var script = @"C:\O2\O2Scripts_Database\_Scripts\Languages_and_Frameworks\AspNet\_O2_AspNet\Create_o2_AspNet_Page_Objects.h2";
			Response.Write(script.compile_H2Script().executeFirstMethod());
		
		%>
	<hr/>
	</body>
</body>