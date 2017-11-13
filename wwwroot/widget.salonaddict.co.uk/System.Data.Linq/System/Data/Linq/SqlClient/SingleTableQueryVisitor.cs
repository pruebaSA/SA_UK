namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Reflection;

    internal class SingleTableQueryVisitor : SqlVisitor
    {
        private List<MemberInfo> IdentityMembers;
        private bool IsDistinct;
        public bool IsValid = true;

        internal SingleTableQueryVisitor()
        {
        }

        private void AddIdentityMembers(IEnumerable<MemberInfo> members)
        {
            this.IdentityMembers = new List<MemberInfo>(members);
        }

        private static bool IsColumnMatch(MemberInfo column, SqlExpression expr)
        {
            MemberInfo member = null;
            switch (expr.NodeType)
            {
                case SqlNodeType.Column:
                    member = ((SqlColumn) expr).MetaMember.Member;
                    break;

                case SqlNodeType.ColumnRef:
                    member = ((SqlColumnRef) expr).Column.MetaMember.Member;
                    break;

                case SqlNodeType.Member:
                    member = ((SqlMember) expr).Member;
                    break;
            }
            return ((member != null) && (member == column));
        }

        internal override SqlNode Visit(SqlNode node)
        {
            if (this.IsValid && (node != null))
            {
                return base.Visit(node);
            }
            return node;
        }

        internal override SqlExpression VisitNew(SqlNew sox)
        {
            foreach (MemberInfo info in this.IdentityMembers)
            {
                bool flag = false;
                foreach (SqlExpression expression in sox.Args)
                {
                    flag = IsColumnMatch(info, expression);
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    foreach (SqlMemberAssign assign in sox.Members)
                    {
                        SqlExpression expr = assign.Expression;
                        flag = IsColumnMatch(info, expr);
                        if (flag)
                        {
                            break;
                        }
                    }
                }
                this.IsValid &= flag;
                if (!this.IsValid)
                {
                    return sox;
                }
            }
            return sox;
        }

        internal override SqlSelect VisitSelect(SqlSelect select)
        {
            if (select.IsDistinct)
            {
                this.IsDistinct = true;
                this.AddIdentityMembers(select.Selection.ClrType.GetProperties());
                return select;
            }
            select.From = (SqlSource) base.Visit(select.From);
            if ((this.IdentityMembers == null) || (this.IdentityMembers.Count == 0))
            {
                throw System.Data.Linq.SqlClient.Error.SkipRequiresSingleTableQueryWithPKs();
            }
            SqlNodeType nodeType = select.Selection.NodeType;
            if (nodeType <= SqlNodeType.ColumnRef)
            {
                switch (nodeType)
                {
                    case SqlNodeType.Column:
                    case SqlNodeType.ColumnRef:
                        goto Label_009E;

                    case SqlNodeType.AliasRef:
                        goto Label_00DD;
                }
                goto Label_00F1;
            }
            if (nodeType != SqlNodeType.Member)
            {
                switch (nodeType)
                {
                    case SqlNodeType.Treat:
                    case SqlNodeType.TypeCase:
                        return select;

                    case SqlNodeType.New:
                        goto Label_00DD;
                }
                goto Label_00F1;
            }
        Label_009E:
            if (this.IdentityMembers.Count == 1)
            {
                MemberInfo column = this.IdentityMembers[0];
                this.IsValid &= IsColumnMatch(column, select.Selection);
                return select;
            }
            this.IsValid = false;
            return select;
        Label_00DD:
            select.Selection = this.VisitExpression(select.Selection);
            return select;
        Label_00F1:
            this.IsValid = false;
            return select;
        }

        internal override SqlSource VisitSource(SqlSource source) => 
            base.VisitSource(source);

        internal override SqlTable VisitTable(SqlTable tab)
        {
            if (!this.IsDistinct)
            {
                if (this.IdentityMembers != null)
                {
                    this.IsValid = false;
                    return tab;
                }
                this.AddIdentityMembers(from m in tab.MetaTable.RowType.IdentityMembers select m.Member);
            }
            return tab;
        }

        internal override SqlNode VisitUnion(SqlUnion su)
        {
            if (su.All)
            {
                this.IsValid = false;
            }
            this.IsDistinct = true;
            this.AddIdentityMembers(su.GetClrType().GetProperties());
            return su;
        }
    }
}

