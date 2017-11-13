namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QueryNode
    {
        private SeekableXPathNavigator node;
        private long nodePosition;
        internal QueryNode(SeekableXPathNavigator node)
        {
            this.node = node;
            this.nodePosition = node.CurrentPosition;
        }

        internal string LocalName =>
            this.node.GetLocalName(this.nodePosition);
        internal string Name =>
            this.node.GetName(this.nodePosition);
        internal string Namespace =>
            this.node.GetNamespace(this.nodePosition);
        internal SeekableXPathNavigator Node =>
            this.node;
        internal long Position =>
            this.nodePosition;
        internal string Value =>
            this.node.GetValue(this.nodePosition);
        internal SeekableXPathNavigator MoveTo()
        {
            this.node.CurrentPosition = this.nodePosition;
            return this.node;
        }
    }
}

