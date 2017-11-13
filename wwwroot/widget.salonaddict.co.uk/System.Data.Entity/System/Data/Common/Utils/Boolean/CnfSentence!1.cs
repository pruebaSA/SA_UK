namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Data.Common.Utils;

    internal sealed class CnfSentence<T_Identifier> : Sentence<T_Identifier, CnfClause<T_Identifier>>
    {
        internal CnfSentence(Set<CnfClause<T_Identifier>> clauses) : base(clauses, ExprType.And)
        {
        }
    }
}

