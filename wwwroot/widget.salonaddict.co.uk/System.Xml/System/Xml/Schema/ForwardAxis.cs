namespace System.Xml.Schema
{
    using System;

    internal class ForwardAxis
    {
        private bool isAttribute;
        private bool isDss;
        private bool isSelfAxis;
        private DoubleLinkAxis rootNode;
        private DoubleLinkAxis topNode;

        public ForwardAxis(DoubleLinkAxis axis, bool isdesorself)
        {
            this.isDss = isdesorself;
            this.isAttribute = Asttree.IsAttribute(axis);
            this.topNode = axis;
            this.rootNode = axis;
            while (this.rootNode.Input != null)
            {
                this.rootNode = (DoubleLinkAxis) this.rootNode.Input;
            }
            this.isSelfAxis = Asttree.IsSelf(this.topNode);
        }

        internal bool IsAttribute =>
            this.isAttribute;

        internal bool IsDss =>
            this.isDss;

        internal bool IsSelfAxis =>
            this.isSelfAxis;

        internal DoubleLinkAxis RootNode =>
            this.rootNode;

        internal DoubleLinkAxis TopNode =>
            this.topNode;
    }
}

