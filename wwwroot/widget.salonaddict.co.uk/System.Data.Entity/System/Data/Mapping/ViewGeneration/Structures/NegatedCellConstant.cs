namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Text;

    internal class NegatedCellConstant : CellConstant
    {
        private Set<CellConstant> m_negatedDomain;

        private NegatedCellConstant()
        {
            this.m_negatedDomain = new Set<CellConstant>(CellConstant.EqualityComparer);
        }

        internal NegatedCellConstant(IEnumerable<CellConstant> values)
        {
            this.m_negatedDomain = new Set<CellConstant>(values, CellConstant.EqualityComparer);
        }

        internal StringBuilder AsCql(StringBuilder builder, string blockAlias, Set<CellConstant> constants, MemberPath path, bool canSkipIsNotNull) => 
            this.ToStringHelper(builder, blockAlias, constants, path, canSkipIsNotNull, false);

        internal StringBuilder AsUserString(StringBuilder builder, string blockAlias, Set<CellConstant> constants, MemberPath path, bool canSkipIsNotNull) => 
            this.ToStringHelper(builder, blockAlias, constants, path, canSkipIsNotNull, true);

        internal override bool CheckRepInvariant() => 
            this.CheckRepInvariantLocal();

        internal bool CheckRepInvariantLocal()
        {
            using (IEnumerator<CellConstant> enumerator = this.Elements.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    CellConstant current = enumerator.Current;
                }
            }
            return true;
        }

        internal bool Contains(CellConstant constant) => 
            this.m_negatedDomain.Contains(constant);

        internal static NegatedCellConstant CreateNotNull()
        {
            NegatedCellConstant constant = new NegatedCellConstant();
            constant.m_negatedDomain.Add(CellConstant.Null);
            return constant;
        }

        private static void GenerateAndsForSlot(StringBuilder builder, string blockAlias, Set<CellConstant> constants, MemberPath path, bool anyAdded, bool userString)
        {
            foreach (CellConstant constant in constants)
            {
                if (anyAdded)
                {
                    builder.Append(" AND ");
                }
                anyAdded = true;
                if (userString)
                {
                    path.ToCompactString(builder, blockAlias);
                    builder.Append(" <>");
                    constant.ToCompactString(builder);
                }
                else
                {
                    path.AsCql(builder, blockAlias);
                    builder.Append(" <>");
                    constant.AsCql(builder, path, blockAlias);
                }
            }
        }

        protected override int GetHash()
        {
            int num = 0;
            foreach (CellConstant constant in this.m_negatedDomain)
            {
                num ^= CellConstant.EqualityComparer.GetHashCode(constant);
            }
            return num;
        }

        internal override bool HasNotNull() => 
            this.m_negatedDomain.Contains(CellConstant.Null);

        protected override bool IsEqualTo(CellConstant right)
        {
            NegatedCellConstant constant = right as NegatedCellConstant;
            if (constant == null)
            {
                return false;
            }
            return this.m_negatedDomain.SetEquals(constant.m_negatedDomain);
        }

        internal override bool IsNotNull() => 
            ((this.m_negatedDomain.Count == 1) && this.m_negatedDomain.Contains(CellConstant.Null));

        internal override void ToCompactString(StringBuilder builder)
        {
            if (this.IsNotNull())
            {
                builder.Append("NOT_NULL");
            }
            else
            {
                builder.Append("NOT(");
                StringUtil.ToCommaSeparatedStringSorted(builder, this.m_negatedDomain);
                builder.Append(")");
            }
        }

        private StringBuilder ToStringHelper(StringBuilder builder, string blockAlias, Set<CellConstant> constants, MemberPath path, bool canSkipIsNotNull, bool userString)
        {
            bool isNullable = path.IsNullable;
            Set<CellConstant> set = new Set<CellConstant>(this.Elements, CellConstant.EqualityComparer);
            foreach (CellConstant constant in constants)
            {
                if (!constant.Equals(this))
                {
                    set.Remove(constant);
                }
            }
            if (set.Count == 0)
            {
                builder.Append("true");
                return builder;
            }
            bool flag2 = set.Contains(CellConstant.Null);
            set.Remove(CellConstant.Null);
            bool anyAdded = false;
            if (flag2 || (isNullable && !canSkipIsNotNull))
            {
                if (userString)
                {
                    path.ToCompactString(builder, blockAlias);
                    builder.Append(" is not NULL");
                }
                else
                {
                    path.AsCql(builder, blockAlias);
                    builder.Append(" IS NOT NULL");
                }
                anyAdded = true;
            }
            GenerateAndsForSlot(builder, blockAlias, set, path, anyAdded, userString);
            return builder;
        }

        internal override string ToUserString()
        {
            if (this.IsNotNull())
            {
                return Strings.ViewGen_NotNull;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (CellConstant constant in this.m_negatedDomain)
            {
                if ((this.m_negatedDomain.Count <= 1) || !constant.IsNull())
                {
                    if (!flag)
                    {
                        builder.Append(Strings.ViewGen_CommaBlank);
                    }
                    flag = false;
                    builder.Append(constant.ToUserString());
                }
            }
            StringBuilder builder2 = new StringBuilder();
            builder2.Append(Strings.ViewGen_NegatedCellConstant_0(builder.ToString()));
            return builder2.ToString();
        }

        internal IEnumerable<CellConstant> Elements =>
            this.m_negatedDomain;
    }
}

