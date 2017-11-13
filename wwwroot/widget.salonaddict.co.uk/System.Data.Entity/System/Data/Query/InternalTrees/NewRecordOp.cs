namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    internal sealed class NewRecordOp : ScalarOp
    {
        private List<EdmProperty> m_fields;
        internal static readonly NewRecordOp Pattern = new NewRecordOp();

        private NewRecordOp() : base(OpType.NewRecord)
        {
        }

        internal NewRecordOp(TypeUsage type) : base(OpType.NewRecord, type)
        {
            this.m_fields = new List<EdmProperty>(TypeHelpers.GetEdmType<RowType>(type).Properties);
        }

        internal NewRecordOp(TypeUsage type, List<EdmProperty> fields) : base(OpType.NewRecord, type)
        {
            this.m_fields = fields;
        }

        [DebuggerNonUserCode]
        internal override void Accept(BasicOpVisitor v, Node n)
        {
            v.Visit(this, n);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType>(BasicOpVisitorOfT<TResultType> v, Node n) => 
            v.Visit(this, n);

        internal bool GetFieldPosition(EdmProperty field, out int fieldPosition)
        {
            fieldPosition = 0;
            for (int i = 0; i < this.m_fields.Count; i++)
            {
                if (this.m_fields[i] == field)
                {
                    fieldPosition = i;
                    return true;
                }
            }
            return false;
        }

        internal List<EdmProperty> Properties =>
            this.m_fields;
    }
}

