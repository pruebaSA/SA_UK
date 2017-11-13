namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class OneOfTypeConst : OneOfConst
    {
        internal OneOfTypeConst(JoinTreeNode node, IEnumerable<EdmType> values) : base(new JoinTreeSlot(node), CreateTypeConstants(values))
        {
        }

        internal OneOfTypeConst(JoinTreeNode node, CellConstant value) : base(new JoinTreeSlot(node), value)
        {
        }

        internal OneOfTypeConst(JoinTreeSlot slot, CellConstantDomain domain) : base(slot, domain)
        {
        }

        internal OneOfTypeConst(JoinTreeNode node, IEnumerable<CellConstant> values, IEnumerable<CellConstant> possibleValues) : base(new JoinTreeSlot(node), values, possibleValues)
        {
        }

        internal OneOfTypeConst(JoinTreeNode node, IEnumerable<EdmType> values, IEnumerable<EdmType> possibleValues) : base(new JoinTreeSlot(node), CreateTypeConstants(values), CreateTypeConstants(possibleValues))
        {
        }

        internal OneOfTypeConst(JoinTreeNode node, CellConstant value, IEnumerable<CellConstant> possibleValues) : base(new JoinTreeSlot(node), value, possibleValues)
        {
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            if (base.Values.Count > 1)
            {
                builder.Append('(');
            }
            bool flag = true;
            foreach (CellConstant constant in base.Values.Values)
            {
                TypeConstant constant2 = constant as TypeConstant;
                if (!flag)
                {
                    builder.Append(" OR ");
                }
                flag = false;
                if (base.Slot.MemberPath.EdmType is RefType)
                {
                    builder.Append("Deref(");
                    base.Slot.MemberPath.AsCql(builder, blockAlias);
                    builder.Append(')');
                }
                else
                {
                    base.Slot.MemberPath.AsCql(builder, blockAlias);
                }
                if (constant.IsNull())
                {
                    builder.Append(" IS NULL");
                }
                else
                {
                    builder.Append(" IS OF (ONLY ");
                    CqlWriter.AppendEscapedTypeName(builder, constant2.CdmType);
                    builder.Append(')');
                }
            }
            if (base.Values.Count > 1)
            {
                builder.Append(')');
            }
            return builder;
        }

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull)
        {
            if (base.Slot.MemberPath.EdmType is RefType)
            {
                builder.Append("Deref(");
                base.Slot.MemberPath.AsCql(builder, blockAlias);
                builder.Append(')');
            }
            else
            {
                base.Slot.MemberPath.AsCql(builder, blockAlias);
            }
            if (base.Values.Count > 1)
            {
                builder.Append(" is a (");
            }
            else
            {
                builder.Append(" is type ");
            }
            bool flag = true;
            foreach (CellConstant constant in base.Values.Values)
            {
                TypeConstant constant2 = constant as TypeConstant;
                if (!flag)
                {
                    builder.Append(" OR ");
                }
                if (constant.IsNull())
                {
                    builder.Append(" NULL");
                }
                else
                {
                    CqlWriter.AppendEscapedTypeName(builder, constant2.CdmType);
                }
                flag = false;
            }
            if (base.Values.Count > 1)
            {
                builder.Append(')');
            }
            return builder;
        }

        private static IEnumerable<CellConstant> CreateTypeConstants(IEnumerable<EdmType> types)
        {
            foreach (EdmType iteratorVariable0 in types)
            {
                if (iteratorVariable0 == null)
                {
                    yield return CellConstant.Null;
                }
                else
                {
                    yield return new TypeConstant(iteratorVariable0);
                }
            }
        }

        internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> FixRange(Set<CellConstant> range, MemberDomainMap memberDomainMap)
        {
            IEnumerable<CellConstant> domain = memberDomainMap.GetDomain(base.Slot.MemberPath);
            BoolLiteral literal = new OneOfTypeConst(base.Slot, new CellConstantDomain(range, domain));
            return literal.GetDomainBoolExpression(memberDomainMap);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("type(");
            base.Slot.ToCompactString(builder);
            builder.Append(") IN (");
            StringUtil.ToCommaSeparatedStringSorted(builder, base.Values.Values);
            builder.Append(")");
        }

    }
}

