namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class Positions
    {
        private ArrayList positions = new ArrayList();

        public int Add(int symbol, object particle) => 
            this.positions.Add(new Position(symbol, particle));

        public int Count =>
            this.positions.Count;

        public Position this[int pos] =>
            ((Position) this.positions[pos]);
    }
}

