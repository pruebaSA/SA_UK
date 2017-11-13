namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal sealed class NewEntityOp : NewEntityBaseOp
    {
        internal static readonly NewEntityOp Pattern = new NewEntityOp();

        private NewEntityOp() : base(OpType.NewEntity)
        {
        }

        internal NewEntityOp(TypeUsage type, List<RelProperty> relProperties, EntitySetBase entitySet) : base(OpType.NewEntity, type, entitySet, relProperties)
        {
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);
    }
}

