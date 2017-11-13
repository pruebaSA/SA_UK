namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class BasicOpVisitorOfNode : BasicOpVisitorOfT<Node>
    {
        protected BasicOpVisitorOfNode()
        {
        }

        protected override Node VisitAncillaryOpDefault(AncillaryOp op, Node n) => 
            this.VisitDefault(n);

        protected override void VisitChildren(Node n)
        {
            for (int i = 0; i < n.Children.Count; i++)
            {
                n.Children[i] = base.VisitNode(n.Children[i]);
            }
        }

        protected override Node VisitDefault(Node n)
        {
            this.VisitChildren(n);
            return n;
        }

        protected override Node VisitPhysicalOpDefault(PhysicalOp op, Node n) => 
            this.VisitDefault(n);

        protected override Node VisitRelOpDefault(RelOp op, Node n) => 
            this.VisitDefault(n);

        protected override Node VisitScalarOpDefault(ScalarOp op, Node n) => 
            this.VisitDefault(n);
    }
}

