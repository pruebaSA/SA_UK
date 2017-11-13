namespace System.Data.Query.InternalTrees
{
    using System;

    internal sealed class PatternMatchRule : Rule
    {
        private Node m_pattern;

        internal PatternMatchRule(Node pattern, Rule.ProcessNodeDelegate processDelegate) : base(pattern.Op.OpType, processDelegate)
        {
            this.m_pattern = pattern;
        }

        internal override bool Match(Node node) => 
            this.Match(this.m_pattern, node);

        private bool Match(Node pattern, Node original)
        {
            if (pattern.Op.OpType != OpType.Leaf)
            {
                if (pattern.Op.OpType != original.Op.OpType)
                {
                    return false;
                }
                if (pattern.Children.Count != original.Children.Count)
                {
                    return false;
                }
                for (int i = 0; i < pattern.Children.Count; i++)
                {
                    if (!this.Match(pattern.Children[i], original.Children[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

