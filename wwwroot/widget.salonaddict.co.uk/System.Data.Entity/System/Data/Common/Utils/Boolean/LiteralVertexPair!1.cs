namespace System.Data.Common.Utils.Boolean
{
    using System;

    internal sealed class LiteralVertexPair<T_Identifier>
    {
        internal readonly Literal<T_Identifier> Literal;
        internal readonly System.Data.Common.Utils.Boolean.Vertex Vertex;

        internal LiteralVertexPair(System.Data.Common.Utils.Boolean.Vertex vertex, Literal<T_Identifier> literal)
        {
            this.Vertex = vertex;
            this.Literal = literal;
        }
    }
}

