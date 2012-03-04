<%@ Page Language="C#" %>
<% @Import Namespace="O2.Kernel" %>
<% @Import Namespace="O2.Kernel.ExtensionMethods" %>
<% @Import Namespace="O2.DotNetWrappers.ExtensionMethods" %>
<% @Import Namespace="O2.External.SharpDevelop.ExtensionMethods" %>
<html><body>

<%
	Response.Write("<h1>Opening up: {0}</h1>".format("Util - Show AspNet Context object details.h2"));
	@"C:\O2\O2Scripts_Database\_Scripts\Languages_and_Frameworks\AspNet\ScriptsToRunWith_AspxContext\Util - Show AspNet Context object details.h2".compile_H2Script().executeFirstMethod();   	
%>
					
</body></html>