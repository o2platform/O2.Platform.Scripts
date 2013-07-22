// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;
using GraphSharp.Controls;

//O2File:GraphSharp_ExtensionMethods.cs

//O2Ref:GraphSharp.Controls.dll 
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll
//O2Ref:WindowsBase.dll
//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll


namespace O2.XRules.Database.Utils
{
    public static class VerticesAndEdges_ExtensionMethods
    {    
    
    	public static List<object> nodes(this GraphLayout graphLayout)
    	{    	 	        
            return graphLayout.vertices();
    	}
    	
		public static object node(this GraphLayout graphLayout, int nodeId)
    	{    	 	        
            var vertices = graphLayout.vertices();
            if (vertices.size() > nodeId)
            	return vertices[nodeId];
            return null;
    	}    	
    	public static List<object> vertices(this GraphLayout graphLayout)
    	{    	 	        
            return graphLayout.get_Graph().Vertices.ToList();
    	}
    	
    	public static GraphLayout edgeToAll(this GraphLayout graphLayout, object vertexToLink)
    	{
    		foreach(var vertex in graphLayout.vertices())
				graphLayout.edge(vertexToLink,vertex);
			return graphLayout;
    	}
    	
    	public static GraphLayout  edgeFromAll(this GraphLayout graphLayout, object vertexToLink)
    	{
    		foreach(var vertex in graphLayout.vertices())
				graphLayout.edge(vertex,vertexToLink);
			return graphLayout;
    	}
    	
    	public static GraphLayout  edgeToFirst(this GraphLayout graphLayout, object vertexToLink)
    	{
    		var vertices = graphLayout.vertices();
    		graphLayout.edge(vertexToLink, vertices[0]);
    		return graphLayout;    		
    	}
    	
    	public static GraphLayout  edgeFromLast(this GraphLayout graphLayout, object vertexToLink)
    	{
    		var vertices = graphLayout.vertices();
    		graphLayout.edge(vertices[vertices.size()-1],vertexToLink);
    		return graphLayout;    		
    	}
    	    	    		    
    }
}
