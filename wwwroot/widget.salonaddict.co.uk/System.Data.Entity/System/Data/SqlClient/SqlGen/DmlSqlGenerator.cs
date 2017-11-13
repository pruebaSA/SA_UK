namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class DmlSqlGenerator
    {
        private const int s_commandTextBuilderInitialCapacity = 0x100;

        internal static string GenerateDeleteSql(DbDeleteCommandTree tree, SqlVersion sqlVersion, out List<SqlParameter> parameters)
        {
            StringBuilder commandText = new StringBuilder(0x100);
            ExpressionTranslator visitor = new ExpressionTranslator(commandText, tree, false, sqlVersion);
            commandText.Append("delete ");
            tree.Target.Expression.Accept(visitor);
            commandText.AppendLine();
            commandText.Append("where ");
            tree.Predicate.Accept(visitor);
            parameters = visitor.Parameters;
            return commandText.ToString();
        }

        internal static string GenerateInsertSql(DbInsertCommandTree tree, SqlVersion sqlVersion, out List<SqlParameter> parameters)
        {
            StringBuilder commandText = new StringBuilder(0x100);
            ExpressionTranslator visitor = new ExpressionTranslator(commandText, tree, null != tree.Returning, sqlVersion);
            commandText.Append("insert ");
            tree.Target.Expression.Accept(visitor);
            if (0 < tree.SetClauses.Count)
            {
                commandText.Append("(");
                bool flag = true;
                foreach (DbSetClause clause in tree.SetClauses)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        commandText.Append(", ");
                    }
                    clause.Property.Accept(visitor);
                }
                commandText.AppendLine(")");
                flag = true;
                commandText.Append("values (");
                foreach (DbSetClause clause2 in tree.SetClauses)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        commandText.Append(", ");
                    }
                    clause2.Value.Accept(visitor);
                    visitor.RegisterMemberValue(clause2.Property, clause2.Value);
                }
                commandText.AppendLine(")");
            }
            else
            {
                commandText.AppendLine().AppendLine("default values");
            }
            GenerateReturningSql(commandText, tree, visitor, tree.Returning);
            parameters = visitor.Parameters;
            return commandText.ToString();
        }

        private static string GenerateMemberTSql(EdmMember member)
        {
            string str;
            EntityType declaringType = (EntityType) member.DeclaringType;
            if (!declaringType.TryGetMemberSql(member, out str))
            {
                str = SqlGenerator.QuoteIdentifier(member.Name);
                declaringType.SetMemberSql(member, str);
            }
            return str;
        }

        private static void GenerateReturningSql(StringBuilder commandText, DbModificationCommandTree tree, ExpressionTranslator translator, DbExpression returning)
        {
            if (returning != null)
            {
                commandText.Append("select ");
                returning.Accept(translator);
                commandText.AppendLine();
                commandText.Append("from ");
                tree.Target.Expression.Accept(translator);
                commandText.AppendLine();
                commandText.Append("where @@ROWCOUNT > 0");
                EntitySetBase target = ((DbScanExpression) tree.Target.Expression).Target;
                bool flag = false;
                foreach (EdmMember member in target.ElementType.KeyMembers)
                {
                    SqlParameter parameter;
                    commandText.Append(" and ");
                    commandText.Append(GenerateMemberTSql(member));
                    commandText.Append(" = ");
                    if (translator.MemberValues.TryGetValue(member, out parameter))
                    {
                        commandText.Append(parameter.ParameterName);
                    }
                    else
                    {
                        if (flag)
                        {
                            throw EntityUtil.NotSupported(Strings.Update_NotSupportedServerGenKey(target.Name));
                        }
                        if (!IsValidIdentityColumnType(member.TypeUsage))
                        {
                            throw EntityUtil.InvalidOperation(Strings.Update_NotSupportedIdentityType(member.Name, member.TypeUsage.ToString()));
                        }
                        commandText.Append("scope_identity()");
                        flag = true;
                    }
                }
            }
        }

        internal static string GenerateUpdateSql(DbUpdateCommandTree tree, SqlVersion sqlVersion, out List<SqlParameter> parameters)
        {
            StringBuilder commandText = new StringBuilder(0x100);
            ExpressionTranslator visitor = new ExpressionTranslator(commandText, tree, null != tree.Returning, sqlVersion);
            if (tree.SetClauses.Count == 0)
            {
                commandText.AppendLine("declare @p int");
            }
            commandText.Append("update ");
            tree.Target.Expression.Accept(visitor);
            commandText.AppendLine();
            bool flag = true;
            commandText.Append("set ");
            foreach (DbSetClause clause in tree.SetClauses)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    commandText.Append(", ");
                }
                clause.Property.Accept(visitor);
                commandText.Append(" = ");
                clause.Value.Accept(visitor);
            }
            if (flag)
            {
                commandText.Append("@p = 0");
            }
            commandText.AppendLine();
            commandText.Append("where ");
            tree.Predicate.Accept(visitor);
            commandText.AppendLine();
            GenerateReturningSql(commandText, tree, visitor, tree.Returning);
            parameters = visitor.Parameters;
            return commandText.ToString();
        }

        private static bool IsValidIdentityColumnType(TypeUsage typeUsage)
        {
            Facet facet;
            if (typeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
            {
                return false;
            }
            string name = typeUsage.EdmType.Name;
            switch (name)
            {
                case "tinyint":
                case "smallint":
                case "int":
                case "bigint":
                    return true;
            }
            if ((name != "decimal") && (name != "numeric"))
            {
                return false;
            }
            return (typeUsage.Facets.TryGetValue("Scale", false, out facet) && (Convert.ToInt32(facet.Value, CultureInfo.InvariantCulture) == 0));
        }

        private class ExpressionTranslator : BasicExpressionVisitor
        {
            private readonly StringBuilder _commandText;
            private readonly DbModificationCommandTree _commandTree;
            private readonly Dictionary<EdmMember, SqlParameter> _memberValues;
            private readonly List<SqlParameter> _parameters;
            private readonly SqlVersion _version;
            private static readonly AliasGenerator s_parameterNames = new AliasGenerator("@", 0x3e8);

            internal ExpressionTranslator(StringBuilder commandText, DbModificationCommandTree commandTree, bool preserveMemberValues, SqlVersion version)
            {
                this._commandText = commandText;
                this._commandTree = commandTree;
                this._version = version;
                this._parameters = new List<SqlParameter>();
                this._memberValues = preserveMemberValues ? new Dictionary<EdmMember, SqlParameter>() : null;
            }

            internal SqlParameter CreateParameter(object value, TypeUsage type)
            {
                SqlParameter item = SqlProviderServices.CreateSqlParameter(s_parameterNames.GetName(this._parameters.Count), type, ParameterMode.In, value, true, this._version);
                this._parameters.Add(item);
                return item;
            }

            internal void RegisterMemberValue(DbExpression propertyExpression, DbExpression value)
            {
                if (this._memberValues != null)
                {
                    EdmMember property = ((DbPropertyExpression) propertyExpression).Property;
                    if (value.ExpressionKind != DbExpressionKind.Null)
                    {
                        this._memberValues[property] = this._parameters[this._parameters.Count - 1];
                    }
                }
            }

            public override void Visit(DbAndExpression expression)
            {
                this.VisitBinary(expression, " and ");
            }

            public override void Visit(DbComparisonExpression expression)
            {
                this.VisitBinary(expression, " = ");
                this.RegisterMemberValue(expression.Left, expression.Right);
            }

            public override void Visit(DbConstantExpression expression)
            {
                SqlParameter parameter = this.CreateParameter(expression.Value, expression.ResultType);
                this._commandText.Append(parameter.ParameterName);
            }

            public override void Visit(DbIsNullExpression expression)
            {
                expression.Argument.Accept(this);
                this._commandText.Append(" is null");
            }

            public override void Visit(DbNewInstanceExpression expression)
            {
                bool flag = true;
                foreach (DbExpression expression2 in expression.Arguments)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        this._commandText.Append(", ");
                    }
                    expression2.Accept(this);
                }
            }

            public override void Visit(DbNotExpression expression)
            {
                this._commandText.Append("not (");
                expression.Accept(this);
                this._commandText.Append(")");
            }

            public override void Visit(DbNullExpression expression)
            {
                this._commandText.Append("null");
            }

            public override void Visit(DbOrExpression expression)
            {
                this.VisitBinary(expression, " or ");
            }

            public override void Visit(DbPropertyExpression expression)
            {
                this._commandText.Append(DmlSqlGenerator.GenerateMemberTSql(expression.Property));
            }

            public override void Visit(DbScanExpression expression)
            {
                if (expression.Target.DefiningQuery != null)
                {
                    string str;
                    if (this._commandTree.CommandTreeKind == DbCommandTreeKind.Delete)
                    {
                        str = "DeleteFunction";
                    }
                    else if (this._commandTree.CommandTreeKind == DbCommandTreeKind.Insert)
                    {
                        str = "InsertFunction";
                    }
                    else
                    {
                        str = "UpdateFunction";
                    }
                    throw EntityUtil.Update(Strings.Update_SqlEntitySetWithoutDmlFunctions(expression.Target.Name, str, "ModificationFunctionMapping"), null, new IEntityStateEntry[0]);
                }
                this._commandText.Append(SqlGenerator.GetTargetTSql(expression.Target));
            }

            private void VisitBinary(DbBinaryExpression expression, string separator)
            {
                this._commandText.Append("(");
                expression.Left.Accept(this);
                this._commandText.Append(separator);
                expression.Right.Accept(this);
                this._commandText.Append(")");
            }

            internal Dictionary<EdmMember, SqlParameter> MemberValues =>
                this._memberValues;

            internal List<SqlParameter> Parameters =>
                this._parameters;
        }
    }
}

