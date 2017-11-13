namespace System.Xml.Schema
{
    using System;

    internal sealed class LeafRangeNode : LeafNode
    {
        private decimal max;
        private decimal min;
        private BitSet nextIteration;

        public LeafRangeNode(decimal min, decimal max) : this(-1, min, max)
        {
        }

        public LeafRangeNode(int pos, decimal min, decimal max) : base(pos)
        {
            this.min = min;
            this.max = max;
        }

        public override SyntaxTreeNode Clone(Positions positions) => 
            new LeafRangeNode(base.Pos, this.min, this.max);

        public override bool IsRangeNode =>
            true;

        public decimal Max =>
            this.max;

        public decimal Min =>
            this.min;

        public BitSet NextIteration
        {
            get => 
                this.nextIteration;
            set
            {
                this.nextIteration = value;
            }
        }
    }
}

