namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SqlNew : SqlSimpleTypeExpression
    {
        private List<MemberInfo> argMembers;
        private List<SqlExpression> args;
        private ConstructorInfo constructor;
        private List<SqlMemberAssign> members;
        private System.Data.Linq.Mapping.MetaType metaType;

        internal SqlNew(System.Data.Linq.Mapping.MetaType metaType, ProviderType sqlType, ConstructorInfo cons, IEnumerable<SqlExpression> args, IEnumerable<MemberInfo> argMembers, IEnumerable<SqlMemberAssign> members, Expression sourceExpression) : base(SqlNodeType.New, metaType.Type, sqlType, sourceExpression)
        {
            this.metaType = metaType;
            if ((cons == null) && metaType.Type.IsClass)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("cons");
            }
            this.constructor = cons;
            this.args = new List<SqlExpression>();
            this.argMembers = new List<MemberInfo>();
            this.members = new List<SqlMemberAssign>();
            if (args != null)
            {
                this.args.AddRange(args);
            }
            if (argMembers != null)
            {
                this.argMembers.AddRange(argMembers);
            }
            if (members != null)
            {
                this.members.AddRange(members);
            }
        }

        internal SqlExpression Find(MemberInfo mi)
        {
            int num = 0;
            int count = this.argMembers.Count;
            while (num < count)
            {
                MemberInfo info = this.argMembers[num];
                if (info.Name == mi.Name)
                {
                    return this.args[num];
                }
                num++;
            }
            foreach (SqlMemberAssign assign in this.Members)
            {
                if (assign.Member.Name == mi.Name)
                {
                    return assign.Expression;
                }
            }
            return null;
        }

        internal List<MemberInfo> ArgMembers =>
            this.argMembers;

        internal List<SqlExpression> Args =>
            this.args;

        internal ConstructorInfo Constructor =>
            this.constructor;

        internal List<SqlMemberAssign> Members =>
            this.members;

        internal System.Data.Linq.Mapping.MetaType MetaType =>
            this.metaType;
    }
}

