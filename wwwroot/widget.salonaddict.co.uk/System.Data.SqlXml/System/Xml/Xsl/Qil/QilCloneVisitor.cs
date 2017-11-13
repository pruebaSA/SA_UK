namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilCloneVisitor : QilScopedVisitor
    {
        private QilFactory fac;
        private SubstitutionList subs;

        public QilCloneVisitor(QilFactory fac) : this(fac, new SubstitutionList())
        {
        }

        public QilCloneVisitor(QilFactory fac, SubstitutionList subs)
        {
            this.fac = fac;
            this.subs = subs;
        }

        protected override void BeginScope(QilNode node)
        {
            this.subs.AddSubstitutionPair(node, node.ShallowClone(this.fac));
        }

        public QilNode Clone(QilNode node)
        {
            QilDepthChecker.Check(node);
            return this.VisitAssumeReference(node);
        }

        protected override void EndScope(QilNode node)
        {
            this.subs.RemoveLastSubstitutionPair();
        }

        protected QilNode FindClonedReference(QilNode node) => 
            this.subs.FindReplacement(node);

        protected override QilNode Visit(QilNode oldNode)
        {
            QilNode n = null;
            if (oldNode == null)
            {
                return null;
            }
            if (oldNode is QilReference)
            {
                n = this.FindClonedReference(oldNode);
            }
            if (n == null)
            {
                n = oldNode.ShallowClone(this.fac);
            }
            return base.Visit(n);
        }

        protected override QilNode VisitChildren(QilNode parent)
        {
            for (int i = 0; i < parent.Count; i++)
            {
                QilNode n = parent[i];
                if (this.IsReference(parent, i))
                {
                    parent[i] = this.VisitReference(n);
                    if (parent[i] == null)
                    {
                        parent[i] = n;
                    }
                }
                else
                {
                    parent[i] = this.Visit(n);
                }
            }
            return parent;
        }

        protected override QilNode VisitReference(QilNode oldNode)
        {
            QilNode node = this.FindClonedReference(oldNode);
            return base.VisitReference((node == null) ? oldNode : node);
        }
    }
}

