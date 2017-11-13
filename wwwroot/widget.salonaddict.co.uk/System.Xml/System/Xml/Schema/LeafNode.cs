﻿namespace System.Xml.Schema
{
    using System;

    internal class LeafNode : SyntaxTreeNode
    {
        private int pos;

        public LeafNode(int pos)
        {
            this.pos = pos;
        }

        public override SyntaxTreeNode Clone(Positions positions) => 
            new LeafNode(positions.Add(positions[this.pos].symbol, positions[this.pos].particle));

        public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
        {
            firstpos.Set(this.pos);
            lastpos.Set(this.pos);
        }

        public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
        {
        }

        public override bool IsNullable =>
            false;

        public int Pos
        {
            get => 
                this.pos;
            set
            {
                this.pos = value;
            }
        }
    }
}

