// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Ref:QuickGraph.dll
using QuickGraph;

namespace O2.API.Visualization
{    
	public class GraphFactory
    {
    	public static BidirectionalGraph<object, IEdge<object>> emptyGraph()
    	{
    		return new BidirectionalGraph<object, IEdge<object>>();
    	}
    	
    	public static BidirectionalGraph<object, IEdge<object>> testGraph()
    	{
    	    var bidirectionalGraph = new BidirectionalGraph<object, IEdge<object>>();
            var vertices = new object[] { "A", "B", "C", "D", "E", "F" };
            var edges = new IEdge<object>[] {
                    new Edge<object>(vertices[0], vertices[1]),
                    new Edge<object>(vertices[1], vertices[2]),
                    new Edge<object>(vertices[1], vertices[3]),
                    new Edge<object>(vertices[3], vertices[4]),
                    new Edge<object>(vertices[0], vertices[4]),
                    new Edge<object>(vertices[4], vertices[5])
                };
            bidirectionalGraph.AddVerticesAndEdgeRange(edges);
            return bidirectionalGraph;
    	}
    	
    }    	    	    	    	    

}
