namespace O2.API.AST.ExtensionMethods
{
    public static class CodeStreamGraphNode_ExtensionMethods
    {
        /*public static O2GraphAstNode newO2GraphNode(this O2CodeStream codeStream);
        {
            return new O2GraphAstNode(codeStream, originalObject);
        }*/

        /*public static O2GraphAstNode newO2GraphNode(this CompilationUnit compilationUnit, object originalObject)
        {
            return new O2GraphAstNode(originalObject, compilationUnit);
        }

        public static O2GraphAstNode newO2GraphNode(this CompilationUnit compilationUnit, object originalObject, string nodeText)
        {
            return new O2GraphAstNode(originalObject, nodeText, compilationUnit);
        }*/

        // this needs to be rewitten using the new AstVistors
        /*
        public static void expandNode(this GraphAstVisitor graphAstVisitor, O2GraphNode nodeToExpand)
        {
            object resolved = null;
            if (nodeToExpand.OriginalObject is MethodGroupResolveResult)
                resolved = nodeToExpand.OriginalObject;
            else if (nodeToExpand.OriginalObject is Expression)
                resolved = graphAstVisitor.O2AstResolver.resolve(nodeToExpand);

            if (resolved == null)
                return;

            //graphAstVisitor.Graph.edge(nodeToExpand, new O2GraphNode(resolved));
            IMethod methodToFollow = null;
            if (resolved is MethodGroupResolveResult)
            {
                var resolvedNames = new List<string>();
                foreach (var groupResult in (resolved as MethodGroupResolveResult).Methods)
                    foreach (var method in groupResult)
                    {
                        methodToFollow = method;
                        break;
                    }
            }
            if (methodToFollow is DefaultMethod)
            {
                var defaultMethod = (DefaultMethod)methodToFollow;
                var classA = graphAstVisitor.O2AstResolver.myProjectContent.Classes.ToList()[0];

                var methodFinder = new AstMethodFinder(graphAstVisitor.O2AstResolver);
                var foundMethods = methodFinder.find(defaultMethod.DotNetName);

                if (foundMethods.Count > 0)
                {
                    foreach (var methodfound in foundMethods)
                    {
                        methodfound.AcceptVisitor(graphAstVisitor, nodeToExpand);
                    }
                }
            }


            "in expand node for: {0}".format(nodeToExpand).info();
        } */       

    }
}
