namespace System.Data.Common.EntitySql
{
    using System;
    using System.Text;

    internal class TreePathTagger
    {
        private StringBuilder _sb = new StringBuilder(8);

        internal TreePathTagger()
        {
        }

        internal static bool IsChildNode(string parentNodePrefix, string childNodePrefix) => 
            (string.IsNullOrEmpty(childNodePrefix) || ((childNodePrefix.Length > parentNodePrefix.Length) && childNodePrefix.StartsWith(parentNodePrefix, StringComparison.Ordinal)));

        internal void LeaveNode()
        {
            this._sb.Remove(this._sb.Length - 1, 1);
        }

        internal void VisitLeftNode()
        {
            this._sb.Append('-');
        }

        internal void VisitRightNode()
        {
            this._sb.Append('+');
        }

        internal string Tag =>
            this._sb.ToString();
    }
}

