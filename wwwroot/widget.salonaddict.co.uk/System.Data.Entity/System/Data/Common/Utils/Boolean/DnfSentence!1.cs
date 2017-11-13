namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class DnfSentence<T_Identifier> : Sentence<T_Identifier, DnfClause<T_Identifier>>
    {
        internal DnfSentence(Set<DnfClause<T_Identifier>> clauses) : base(clauses, ExprType.Or)
        {
        }
    }
}

