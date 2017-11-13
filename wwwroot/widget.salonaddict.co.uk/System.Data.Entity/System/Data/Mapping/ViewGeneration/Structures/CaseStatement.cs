namespace System.Data.Mapping.ViewGeneration.Structures
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.CqlGeneration;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class CaseStatement : InternalBase
    {
        private List<WhenThen> m_clauses;
        private ProjectedSlot m_elseValue;
        private System.Data.Mapping.ViewGeneration.Structures.MemberPath m_memberPath;

        internal CaseStatement(System.Data.Mapping.ViewGeneration.Structures.MemberPath memberPath)
        {
            this.m_memberPath = memberPath;
            this.m_clauses = new List<WhenThen>();
        }

        internal void AddWhenThen(BoolExpression condition, ProjectedSlot value)
        {
            condition.ExpensiveSimplify();
            this.m_clauses.Add(new WhenThen(condition, value));
        }

        private void AppendWithBlock(StringBuilder builder, IEnumerable<WithStatement> withStatements, EdmType fromType, string blockAlias, int indentLevel)
        {
            bool flag = true;
            foreach (WithStatement statement in withStatements)
            {
                if (statement.EntityTypeForFromEnd.IsAssignableFrom(fromType))
                {
                    if (flag)
                    {
                        flag = false;
                        builder.Append(" WITH ");
                    }
                    statement.AsCql(builder, blockAlias, indentLevel);
                }
            }
        }

        private void AppendWithBlock(StringBuilder builder, IEnumerable<WithStatement> withStatements, string blockAlias, int indentLevel, ProjectedSlot projectedSlot)
        {
            if ((withStatements != null) && (withStatements.Count<WithStatement>() > 0))
            {
                ConstantSlot slot = projectedSlot as ConstantSlot;
                if (slot != null)
                {
                    TypeConstant cellConstant = slot.CellConstant as TypeConstant;
                    if (cellConstant != null)
                    {
                        this.AppendWithBlock(builder, withStatements, cellConstant.CdmType, blockAlias, indentLevel);
                    }
                }
            }
        }

        internal StringBuilder AsCql(StringBuilder builder, IEnumerable<WithStatement> withStatements, string blockAlias, int indentLevel)
        {
            StringUtil.IndentNewLine(builder, indentLevel + 1);
            if (this.Clauses.Count == 0)
            {
                CaseSlotValueAsCql(builder, this.ElseValue, this.MemberPath, blockAlias);
                this.AppendWithBlock(builder, withStatements, blockAlias, indentLevel, this.ElseValue);
                return builder;
            }
            builder.Append("CASE");
            foreach (WhenThen then in this.Clauses)
            {
                StringUtil.IndentNewLine(builder, indentLevel + 2);
                builder.Append("WHEN ");
                then.Condition.AsCql(builder, blockAlias);
                builder.Append(" THEN ");
                CaseSlotValueAsCql(builder, then.Value, this.MemberPath, blockAlias);
                this.AppendWithBlock(builder, withStatements, blockAlias, indentLevel + 2, then.Value);
            }
            if (this.ElseValue != null)
            {
                StringUtil.IndentNewLine(builder, indentLevel + 2);
                builder.Append("ELSE ");
                CaseSlotValueAsCql(builder, this.ElseValue, this.MemberPath, blockAlias);
                this.AppendWithBlock(builder, withStatements, blockAlias, indentLevel + 2, this.ElseValue);
            }
            StringUtil.IndentNewLine(builder, indentLevel + 1);
            builder.Append("END");
            return builder;
        }

        private static StringBuilder CaseSlotValueAsCql(StringBuilder builder, ProjectedSlot slot, System.Data.Mapping.ViewGeneration.Structures.MemberPath outputMember, string blockAlias)
        {
            slot.AsCql(builder, outputMember, blockAlias, 1);
            return builder;
        }

        internal CaseStatement MakeCaseWithAliasedSlots(CqlBlock block, System.Data.Mapping.ViewGeneration.Structures.MemberPath outputPath, int slotNum)
        {
            CaseStatement statement = new CaseStatement(this.m_memberPath);
            foreach (WhenThen then in this.m_clauses)
            {
                WhenThen item = then.ReplaceWithAliasedSlot(block, outputPath, slotNum);
                statement.m_clauses.Add(item);
            }
            if (this.m_elseValue != null)
            {
                statement.m_elseValue = this.m_elseValue.MakeAliasedSlot(block, outputPath, slotNum);
            }
            return statement;
        }

        internal void Simplify()
        {
            List<WhenThen> list = new List<WhenThen>();
            bool flag = false;
            foreach (WhenThen then in this.m_clauses)
            {
                ConstantSlot slot = then.Value as ConstantSlot;
                if ((slot != null) && (slot.CellConstant.IsNull() || slot.CellConstant.IsUndefined()))
                {
                    flag = true;
                }
                else
                {
                    list.Add(then);
                    if (then.Condition.IsTrue)
                    {
                        break;
                    }
                }
            }
            if (flag && (list.Count == 0))
            {
                this.m_elseValue = new ConstantSlot(CellConstant.Null);
            }
            if ((list.Count > 0) && !flag)
            {
                int index = list.Count - 1;
                this.m_elseValue = list[index].Value;
                list.RemoveAt(index);
            }
            this.m_clauses = list;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.AppendLine("CASE");
            foreach (WhenThen then in this.m_clauses)
            {
                builder.Append("    WHEN ");
                then.Condition.ToCompactString(builder);
                builder.Append(" THEN ");
                then.Value.ToCompactString(builder);
                builder.AppendLine();
            }
            if (this.m_elseValue != null)
            {
                builder.Append("    ELSE ");
                this.m_elseValue.ToCompactString(builder);
                builder.AppendLine();
            }
            builder.Append("END AS ");
            this.m_memberPath.ToCompactString(builder);
        }

        private bool TryGetInstantiatedType(ProjectedSlot slot, out EdmType type)
        {
            type = null;
            ConstantSlot slot2 = slot as ConstantSlot;
            if (slot2 != null)
            {
                TypeConstant cellConstant = slot2.CellConstant as TypeConstant;
                if (cellConstant != null)
                {
                    type = cellConstant.CdmType;
                    return true;
                }
            }
            return false;
        }

        internal List<WhenThen> Clauses =>
            this.m_clauses;

        internal bool DependsOnMemberValue
        {
            get
            {
                if (this.m_elseValue is JoinTreeSlot)
                {
                    return true;
                }
                foreach (WhenThen then in this.m_clauses)
                {
                    if (then.Value is JoinTreeSlot)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal ProjectedSlot ElseValue =>
            this.m_elseValue;

        internal IEnumerable<EdmType> InstantiatedTypes
        {
            get
            {
                EdmType iteratorVariable2;
                foreach (WhenThen iteratorVariable0 in this.m_clauses)
                {
                    EdmType iteratorVariable1;
                    if (this.TryGetInstantiatedType(iteratorVariable0.Value, out iteratorVariable1))
                    {
                        yield return iteratorVariable1;
                    }
                }
                if (this.TryGetInstantiatedType(this.m_elseValue, out iteratorVariable2))
                {
                    yield return iteratorVariable2;
                }
            }
        }

        internal System.Data.Mapping.ViewGeneration.Structures.MemberPath MemberPath =>
            this.m_memberPath;


        internal class WhenThen : InternalBase
        {
            private BoolExpression m_condition;
            private ProjectedSlot m_value;

            internal WhenThen(BoolExpression condition, ProjectedSlot value)
            {
                this.m_condition = condition;
                this.m_value = value;
            }

            internal CaseStatement.WhenThen ReplaceWithAliasedSlot(CqlBlock block, MemberPath outputPath, int slotNum) => 
                new CaseStatement.WhenThen(this.m_condition, this.m_value.MakeAliasedSlot(block, outputPath, slotNum));

            internal override void ToCompactString(StringBuilder builder)
            {
                builder.Append("WHEN ");
                this.m_condition.ToCompactString(builder);
                builder.Append("THEN ");
                this.m_value.ToCompactString(builder);
            }

            internal BoolExpression Condition =>
                this.m_condition;

            internal ProjectedSlot Value =>
                this.m_value;
        }
    }
}

