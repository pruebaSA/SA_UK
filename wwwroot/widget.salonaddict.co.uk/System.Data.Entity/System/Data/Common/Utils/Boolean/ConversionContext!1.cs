namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;

    internal abstract class ConversionContext<T_Identifier>
    {
        internal readonly System.Data.Common.Utils.Boolean.Solver Solver;

        protected ConversionContext()
        {
            this.Solver = new System.Data.Common.Utils.Boolean.Solver();
        }

        internal abstract IEnumerable<LiteralVertexPair<T_Identifier>> GetSuccessors(Vertex vertex);
        internal abstract Vertex TranslateTermToVertex(TermExpr<T_Identifier> term);
    }
}

