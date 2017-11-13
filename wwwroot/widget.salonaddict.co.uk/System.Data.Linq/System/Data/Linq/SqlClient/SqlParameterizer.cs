namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Linq.Mapping;

    internal class SqlParameterizer
    {
        private SqlNodeAnnotations annotations;
        private int index;
        private TypeSystemProvider typeProvider;

        internal SqlParameterizer(TypeSystemProvider typeProvider, SqlNodeAnnotations annotations)
        {
            this.typeProvider = typeProvider;
            this.annotations = annotations;
        }

        internal virtual string CreateParameterName() => 
            ("@p" + this.index++);

        internal ReadOnlyCollection<SqlParameterInfo> Parameterize(SqlNode node) => 
            this.ParameterizeInternal(node).AsReadOnly();

        internal ReadOnlyCollection<ReadOnlyCollection<SqlParameterInfo>> ParameterizeBlock(SqlBlock block)
        {
            SqlParameterInfo item = new SqlParameterInfo(new SqlParameter(typeof(int), this.typeProvider.From(typeof(int)), "@ROWCOUNT", block.SourceExpression));
            List<ReadOnlyCollection<SqlParameterInfo>> list = new List<ReadOnlyCollection<SqlParameterInfo>>();
            int num = 0;
            int count = block.Statements.Count;
            while (num < count)
            {
                SqlNode node = block.Statements[num];
                List<SqlParameterInfo> list2 = this.ParameterizeInternal(node);
                if (num > 0)
                {
                    list2.Add(item);
                }
                list.Add(list2.AsReadOnly());
                num++;
            }
            return list.AsReadOnly();
        }

        private List<SqlParameterInfo> ParameterizeInternal(SqlNode node)
        {
            Visitor visitor = new Visitor(this);
            visitor.Visit(node);
            return new List<SqlParameterInfo>(visitor.currentParams);
        }

        private class Visitor : SqlVisitor
        {
            internal List<SqlParameterInfo> currentParams;
            internal Dictionary<object, SqlParameterInfo> map;
            private SqlParameterizer parameterizer;
            private ProviderType timeProviderType;
            private bool topLevel;

            internal Visitor(SqlParameterizer parameterizer)
            {
                this.parameterizer = parameterizer;
                this.topLevel = true;
                this.map = new Dictionary<object, SqlParameterInfo>();
                this.currentParams = new List<SqlParameterInfo>();
            }

            private ParameterDirection GetParameterDirection(MetaParameter p)
            {
                if (p.Parameter.IsRetval)
                {
                    return ParameterDirection.ReturnValue;
                }
                if (p.Parameter.IsOut)
                {
                    return ParameterDirection.Output;
                }
                if (p.Parameter.ParameterType.IsByRef)
                {
                    return ParameterDirection.InputOutput;
                }
                return ParameterDirection.Input;
            }

            private SqlParameter InsertLookup(SqlValue cp)
            {
                SqlParameterInfo info = null;
                if (!this.map.TryGetValue(cp, out info))
                {
                    SqlParameter parameter;
                    if (this.timeProviderType == null)
                    {
                        parameter = new SqlParameter(cp.ClrType, cp.SqlType, this.parameterizer.CreateParameterName(), cp.SourceExpression);
                        info = new SqlParameterInfo(parameter, cp.Value);
                    }
                    else
                    {
                        parameter = new SqlParameter(cp.ClrType, this.timeProviderType, this.parameterizer.CreateParameterName(), cp.SourceExpression);
                        DateTime time = (DateTime) cp.Value;
                        info = new SqlParameterInfo(parameter, time.TimeOfDay);
                    }
                    this.map.Add(cp, info);
                    this.currentParams.Add(info);
                }
                return info.Parameter;
            }

            private bool RetypeOutParameter(SqlParameter node)
            {
                if (node.SqlType.IsLargeType)
                {
                    ProviderType bestLargeType = this.parameterizer.typeProvider.GetBestLargeType(node.SqlType);
                    if (node.SqlType != bestLargeType)
                    {
                        node.SetSqlType(bestLargeType);
                        return true;
                    }
                    this.parameterizer.annotations.Add(node, new SqlServerCompatibilityAnnotation(System.Data.Linq.SqlClient.Strings.MaxSizeNotSupported(node.SourceExpression), new SqlProvider.ProviderMode[] { SqlProvider.ProviderMode.Sql2000 }));
                }
                return false;
            }

            internal override SqlExpression VisitBinaryOperator(SqlBinary bo)
            {
                switch (bo.NodeType)
                {
                    case SqlNodeType.EQ:
                    case SqlNodeType.EQ2V:
                    case SqlNodeType.NE:
                    case SqlNodeType.NE2V:
                    {
                        SqlDbType sqlDbType = ((SqlTypeSystem.SqlType) bo.Left.SqlType).SqlDbType;
                        SqlDbType type2 = ((SqlTypeSystem.SqlType) bo.Right.SqlType).SqlDbType;
                        if (sqlDbType != type2)
                        {
                            bool flag = bo.Left is SqlColumnRef;
                            bool flag2 = bo.Right is SqlColumnRef;
                            if (flag != flag2)
                            {
                                if ((flag && (sqlDbType == SqlDbType.Time)) && (bo.Right.ClrType == typeof(DateTime)))
                                {
                                    this.timeProviderType = bo.Left.SqlType;
                                }
                                else if ((flag2 && (type2 == SqlDbType.Time)) && (bo.Left.ClrType == typeof(DateTime)))
                                {
                                    this.timeProviderType = bo.Left.SqlType;
                                }
                            }
                        }
                        break;
                    }
                }
                base.VisitBinaryOperator(bo);
                return bo;
            }

            internal override SqlExpression VisitClientParameter(SqlClientParameter cp)
            {
                if (cp.SqlType.CanBeParameter)
                {
                    SqlParameter parameter = new SqlParameter(cp.ClrType, cp.SqlType, this.parameterizer.CreateParameterName(), cp.SourceExpression);
                    this.currentParams.Add(new SqlParameterInfo(parameter, cp.Accessor.Compile()));
                    return parameter;
                }
                return cp;
            }

            internal override SqlStatement VisitDelete(SqlDelete sd)
            {
                bool topLevel = this.topLevel;
                this.topLevel = false;
                base.VisitDelete(sd);
                this.topLevel = topLevel;
                return sd;
            }

            internal override SqlStatement VisitInsert(SqlInsert sin)
            {
                bool topLevel = this.topLevel;
                this.topLevel = false;
                base.VisitInsert(sin);
                this.topLevel = topLevel;
                return sin;
            }

            internal SqlExpression VisitParameter(SqlExpression expr)
            {
                SqlExpression expression = this.VisitExpression(expr);
                SqlNodeType nodeType = expression.NodeType;
                if (nodeType != SqlNodeType.Parameter)
                {
                    if (nodeType == SqlNodeType.Value)
                    {
                        return this.InsertLookup((SqlValue) expression);
                    }
                    return expression;
                }
                return (SqlParameter) expression;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                bool topLevel = this.topLevel;
                this.topLevel = false;
                select.From = (SqlSource) this.Visit(select.From);
                select.Where = this.VisitExpression(select.Where);
                int num = 0;
                int count = select.GroupBy.Count;
                while (num < count)
                {
                    select.GroupBy[num] = this.VisitExpression(select.GroupBy[num]);
                    num++;
                }
                select.Having = this.VisitExpression(select.Having);
                int num3 = 0;
                int num4 = select.OrderBy.Count;
                while (num3 < num4)
                {
                    select.OrderBy[num3].Expression = this.VisitExpression(select.OrderBy[num3].Expression);
                    num3++;
                }
                select.Top = this.VisitExpression(select.Top);
                select.Row = (SqlRow) this.Visit(select.Row);
                this.topLevel = topLevel;
                select.Selection = this.VisitExpression(select.Selection);
                return select;
            }

            internal override SqlStoredProcedureCall VisitStoredProcedureCall(SqlStoredProcedureCall spc)
            {
                this.VisitUserQuery(spc);
                int num = 0;
                int count = spc.Function.Parameters.Count;
                while (num < count)
                {
                    MetaParameter p = spc.Function.Parameters[num];
                    SqlParameter node = spc.Arguments[num] as SqlParameter;
                    if (node != null)
                    {
                        node.Direction = this.GetParameterDirection(p);
                        if ((node.Direction == ParameterDirection.InputOutput) || (node.Direction == ParameterDirection.Output))
                        {
                            this.RetypeOutParameter(node);
                        }
                    }
                    num++;
                }
                SqlParameter parameter3 = new SqlParameter(typeof(int?), this.parameterizer.typeProvider.From(typeof(int)), "@RETURN_VALUE", spc.SourceExpression) {
                    Direction = ParameterDirection.Output
                };
                this.currentParams.Add(new SqlParameterInfo(parameter3));
                return spc;
            }

            internal override SqlStatement VisitUpdate(SqlUpdate sup)
            {
                bool topLevel = this.topLevel;
                this.topLevel = false;
                base.VisitUpdate(sup);
                this.topLevel = topLevel;
                return sup;
            }

            internal override SqlUserQuery VisitUserQuery(SqlUserQuery suq)
            {
                bool topLevel = this.topLevel;
                this.topLevel = false;
                int num = 0;
                int count = suq.Arguments.Count;
                while (num < count)
                {
                    suq.Arguments[num] = this.VisitParameter(suq.Arguments[num]);
                    num++;
                }
                this.topLevel = topLevel;
                suq.Projection = this.VisitExpression(suq.Projection);
                return suq;
            }

            internal override SqlExpression VisitValue(SqlValue value)
            {
                if ((!this.topLevel && value.IsClientSpecified) && value.SqlType.CanBeParameter)
                {
                    return this.InsertLookup(value);
                }
                return value;
            }
        }
    }
}

