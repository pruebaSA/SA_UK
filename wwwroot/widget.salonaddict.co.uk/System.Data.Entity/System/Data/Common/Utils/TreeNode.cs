namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class TreeNode
    {
        private List<TreeNode> _children;
        private int _position;
        private StringBuilder _text;

        internal TreeNode()
        {
            this._children = new List<TreeNode>();
            this._text = new StringBuilder();
        }

        internal TreeNode(string text, List<TreeNode> children) : this(text, new TreeNode[0])
        {
            if (children != null)
            {
                this._children.AddRange(children);
            }
        }

        internal TreeNode(string text, params TreeNode[] children)
        {
            this._children = new List<TreeNode>();
            if (string.IsNullOrEmpty(text))
            {
                this._text = new StringBuilder();
            }
            else
            {
                this._text = new StringBuilder(text);
            }
            if (children != null)
            {
                this._children.AddRange(children);
            }
        }

        internal IList<TreeNode> Children =>
            this._children;

        internal int Position
        {
            get => 
                this._position;
            set
            {
                this._position = value;
            }
        }

        internal StringBuilder Text =>
            this._text;
    }
}

