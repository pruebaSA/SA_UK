namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal abstract class TreePrinter
    {
        private char _horizontals = '_';
        private List<TreeNode> _scopes = new List<TreeNode>();
        private bool _showLines = true;
        private char _verticals = '|';

        internal TreePrinter()
        {
        }

        internal virtual void AfterAppend(TreeNode node, StringBuilder text)
        {
        }

        internal virtual void BeforeAppend(TreeNode node, StringBuilder text)
        {
        }

        private void IndentLine(StringBuilder text)
        {
            int num = 0;
            for (int i = 0; i < this._scopes.Count; i++)
            {
                TreeNode node = this._scopes[i];
                if (!this._showLines || ((node.Position == node.Children.Count) && (i != (this._scopes.Count - 1))))
                {
                    text.Append(' ');
                }
                else
                {
                    text.Append(this._verticals);
                }
                num++;
                if ((this._scopes.Count == num) && this._showLines)
                {
                    text.Append(this._horizontals);
                }
                else
                {
                    text.Append(' ');
                }
            }
        }

        internal virtual void PreProcess(TreeNode node)
        {
        }

        internal virtual string Print(TreeNode node)
        {
            this.PreProcess(node);
            StringBuilder text = new StringBuilder();
            this.PrintNode(text, node);
            return text.ToString();
        }

        internal virtual void PrintChildren(StringBuilder text, TreeNode node)
        {
            this._scopes.Add(node);
            node.Position = 0;
            foreach (TreeNode node2 in node.Children)
            {
                text.AppendLine();
                node.Position++;
                this.PrintNode(text, node2);
            }
            this._scopes.RemoveAt(this._scopes.Count - 1);
        }

        internal virtual void PrintNode(StringBuilder text, TreeNode node)
        {
            this.IndentLine(text);
            this.BeforeAppend(node, text);
            text.Append(node.Text.ToString());
            this.AfterAppend(node, text);
            this.PrintChildren(text, node);
        }
    }
}

