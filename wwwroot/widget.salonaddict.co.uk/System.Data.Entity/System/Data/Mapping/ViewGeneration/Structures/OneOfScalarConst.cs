namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class OneOfScalarConst : OneOfConst
    {
        internal OneOfScalarConst(JoinTreeNode node, CellConstant value) : base(new JoinTreeSlot(node), value)
        {
        }

        internal OneOfScalarConst(JoinTreeSlot slot, CellConstantDomain domain) : base(slot, domain)
        {
        }

        internal OneOfScalarConst(JoinTreeNode node, IEnumerable<CellConstant> values, IEnumerable<CellConstant> possibleValues) : base(new JoinTreeSlot(node), values, possibleValues)
        {
        }

        internal OneOfScalarConst(JoinTreeNode node, object value, TypeUsage memberType) : this(node, new ScalarConstant(value))
        {
        }

        internal override StringBuilder AsCql(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            this.ToStringHelper(builder, blockAlias, canSkipIsNotNull, false);

        internal override StringBuilder AsUserString(StringBuilder builder, string blockAlias, bool canSkipIsNotNull) => 
            this.ToStringHelper(builder, blockAlias, canSkipIsNotNull, true);

        internal override BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> FixRange(System.Data.Common.Utils.Set<CellConstant> range, MemberDomainMap memberDomainMap)
        {
            IEnumerable<CellConstant> domain = memberDomainMap.GetDomain(base.Slot.MemberPath);
            BoolLiteral literal = new OneOfScalarConst(base.Slot, new CellConstantDomain(range, domain));
            return literal.GetDomainBoolExpression(memberDomainMap);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            base.Slot.ToCompactString(builder);
            builder.Append(" IN (");
            StringUtil.ToCommaSeparatedStringSorted(builder, base.Values.Values);
            builder.Append(")");
        }

        private StringBuilder ToStringHelper(StringBuilder builder, string blockAlias, bool canSkipIsNotNull, bool userString)
        {
            System.Data.Common.Utils.Set<CellConstant> constants = new System.Data.Common.Utils.Set<CellConstant>(base.Values.Values, CellConstant.EqualityComparer);
            NegatedCellConstant constant = null;
            foreach (CellConstant constant2 in constants)
            {
                NegatedCellConstant constant3 = constant2 as NegatedCellConstant;
                if (constant3 != null)
                {
                    constant = constant3;
                }
            }
            if (constant != null)
            {
                if (userString)
                {
                    return constant.AsUserString(builder, blockAlias, constants, base.Slot.MemberPath, canSkipIsNotNull);
                }
                return constant.AsCql(builder, blockAlias, constants, base.Slot.MemberPath, canSkipIsNotNull);
            }
            bool flag = constants.Contains(CellConstant.Null);
            constants.Remove(CellConstant.Null);
            if (constants.Contains(CellConstant.Undefined))
            {
                constants.Remove(CellConstant.Undefined);
                flag = true;
            }
            if (flag)
            {
                if (constants.Count > 0)
                {
                    builder.Append('(');
                }
                if (userString)
                {
                    base.Slot.MemberPath.ToCompactString(builder, blockAlias);
                    builder.Append(" is NULL");
                }
                else
                {
                    base.Slot.MemberPath.AsCql(builder, blockAlias);
                    builder.Append(" IS NULL");
                }
                if (constants.Count > 0)
                {
                    builder.Append(" OR ");
                }
            }
            if (constants.Count != 0)
            {
                bool isNullable = base.Slot.MemberPath.IsNullable;
                if (isNullable && !canSkipIsNotNull)
                {
                    builder.Append("(");
                }
                if (userString)
                {
                    base.Slot.MemberPath.ToCompactString(builder, blockAlias);
                }
                else
                {
                    base.Slot.MemberPath.AsCql(builder, blockAlias);
                }
                if (constants.Count > 1)
                {
                    builder.Append(" IN {");
                    bool flag3 = true;
                    foreach (CellConstant constant4 in constants)
                    {
                        if (!flag3)
                        {
                            builder.Append(", ");
                        }
                        flag3 = false;
                        if (userString)
                        {
                            constant4.ToCompactString(builder);
                        }
                        else
                        {
                            constant4.AsCql(builder, base.Slot.MemberPath, blockAlias);
                        }
                    }
                    builder.Append("}");
                }
                else
                {
                    builder.Append(" = ");
                    if (userString)
                    {
                        constants.Single<CellConstant>().ToCompactString(builder);
                    }
                    else
                    {
                        constants.Single<CellConstant>().AsCql(builder, base.Slot.MemberPath, blockAlias);
                    }
                }
                if (isNullable && !canSkipIsNotNull)
                {
                    builder.Append(" AND ");
                    if (userString)
                    {
                        base.Slot.MemberPath.ToCompactString(builder, System.Data.Entity.Strings.ViewGen_EntityInstanceToken);
                        builder.Append(" is not NULL)");
                    }
                    else
                    {
                        base.Slot.MemberPath.AsCql(builder, blockAlias);
                        builder.Append(" IS NOT NULL)");
                    }
                }
                if (flag)
                {
                    builder.Append(')');
                }
            }
            return builder;
        }
    }
}

