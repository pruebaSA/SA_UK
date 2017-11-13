namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Linq.Mapping;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SqlFactory
    {
        private MetaModel model;
        private TypeSystemProvider typeProvider;

        internal SqlFactory(TypeSystemProvider typeProvider, MetaModel model)
        {
            this.typeProvider = typeProvider;
            this.model = model;
        }

        internal SqlExpression Add(params SqlExpression[] expressions)
        {
            SqlExpression right = expressions[expressions.Length - 1];
            for (int i = expressions.Length - 2; i >= 0; i--)
            {
                right = this.Binary(SqlNodeType.Add, expressions[i], right);
            }
            return right;
        }

        internal SqlExpression Add(SqlExpression expr, int second) => 
            this.Binary(SqlNodeType.Add, expr, this.ValueFromObject(second, false, expr.SourceExpression));

        internal SqlExpression AddTimeSpan(SqlExpression dateTime, SqlExpression timeSpan) => 
            this.AddTimeSpan(dateTime, timeSpan, false);

        internal SqlExpression AddTimeSpan(SqlExpression dateTime, SqlExpression timeSpan, bool asNullable)
        {
            SqlExpression expression = this.DATEPART("NANOSECOND", timeSpan);
            SqlExpression expression2 = this.DATEPART("MILLISECOND", timeSpan);
            SqlExpression expression3 = this.DATEPART("SECOND", timeSpan);
            SqlExpression expression4 = this.DATEPART("MINUTE", timeSpan);
            SqlExpression expression5 = this.DATEPART("HOUR", timeSpan);
            SqlExpression expr = dateTime;
            if (this.IsSqlHighPrecisionDateTimeType(dateTime))
            {
                expr = this.DATEADD("NANOSECOND", expression, expr, dateTime.SourceExpression, asNullable);
            }
            else
            {
                expr = this.DATEADD("MILLISECOND", expression2, expr, dateTime.SourceExpression, asNullable);
            }
            expr = this.DATEADD("SECOND", expression3, expr, dateTime.SourceExpression, asNullable);
            expr = this.DATEADD("MINUTE", expression4, expr, dateTime.SourceExpression, asNullable);
            expr = this.DATEADD("HOUR", expression5, expr, dateTime.SourceExpression, asNullable);
            if (this.IsSqlDateTimeOffsetType(dateTime))
            {
                return this.ConvertTo(typeof(DateTimeOffset), expr);
            }
            return expr;
        }

        internal SqlExpression AndAccumulate(SqlExpression left, SqlExpression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return this.Binary(SqlNodeType.And, left, right);
        }

        internal SqlBetween Between(SqlExpression expr, SqlExpression start, SqlExpression end, Expression source) => 
            new SqlBetween(typeof(bool), this.typeProvider.From(typeof(bool)), expr, start, end, source);

        internal SqlBinary Binary(SqlNodeType nodeType, SqlExpression left, SqlExpression right) => 
            this.Binary(nodeType, left, right, null, null);

        internal SqlBinary Binary(SqlNodeType nodeType, SqlExpression left, SqlExpression right, MethodInfo method) => 
            this.Binary(nodeType, left, right, method, null);

        internal SqlBinary Binary(SqlNodeType nodeType, SqlExpression left, SqlExpression right, Type clrType) => 
            this.Binary(nodeType, left, right, null, clrType);

        internal SqlBinary Binary(SqlNodeType nodeType, SqlExpression left, SqlExpression right, MethodInfo method, Type clrType)
        {
            ProviderType sqlType = null;
            if (nodeType.IsPredicateBinaryOperator())
            {
                if (clrType == null)
                {
                    clrType = typeof(bool);
                }
                sqlType = this.typeProvider.From(clrType);
            }
            else
            {
                ProviderType type2 = this.typeProvider.PredictTypeForBinary(nodeType, left.SqlType, right.SqlType);
                if (type2 == right.SqlType)
                {
                    if (clrType == null)
                    {
                        clrType = right.ClrType;
                    }
                    sqlType = right.SqlType;
                }
                else if (type2 == left.SqlType)
                {
                    if (clrType == null)
                    {
                        clrType = left.ClrType;
                    }
                    sqlType = left.SqlType;
                }
                else
                {
                    sqlType = type2;
                    if (clrType == null)
                    {
                        clrType = type2.GetClosestRuntimeType();
                    }
                }
            }
            return new SqlBinary(nodeType, clrType, sqlType, left, right, method);
        }

        internal SqlExpression Case(Type clrType, SqlExpression discriminator, List<SqlExpression> matches, List<SqlExpression> values, Expression sourceExpression)
        {
            if (values.Count == 0)
            {
                throw System.Data.Linq.SqlClient.Error.EmptyCaseNotSupported();
            }
            bool flag = false;
            foreach (SqlExpression expression in values)
            {
                flag |= expression.IsClientAidedExpression();
            }
            if (flag)
            {
                List<SqlClientWhen> list = new List<SqlClientWhen>();
                int num = 0;
                int num2 = matches.Count;
                while (num < num2)
                {
                    list.Add(new SqlClientWhen(matches[num], values[num]));
                    num++;
                }
                return new SqlClientCase(clrType, discriminator, list, sourceExpression);
            }
            List<SqlWhen> whens = new List<SqlWhen>();
            int num3 = 0;
            int count = matches.Count;
            while (num3 < count)
            {
                whens.Add(new SqlWhen(matches[num3], values[num3]));
                num3++;
            }
            return new SqlSimpleCase(clrType, discriminator, whens, sourceExpression);
        }

        internal SqlExpression CastTo(Type clrType, SqlExpression expr) => 
            this.UnaryCast(clrType, this.typeProvider.From(clrType), expr, expr.SourceExpression);

        internal SqlExpression CLRLENGTH(SqlExpression expr) => 
            this.Unary(SqlNodeType.ClrLength, expr);

        internal SqlExpression Concat(params SqlExpression[] expressions)
        {
            SqlExpression right = expressions[expressions.Length - 1];
            for (int i = expressions.Length - 2; i >= 0; i--)
            {
                right = this.Binary(SqlNodeType.Concat, expressions[i], right);
            }
            return right;
        }

        internal SqlExpression ConvertTimeToDouble(SqlExpression exp)
        {
            if (!this.IsSqlTimeType(exp))
            {
                return exp;
            }
            return this.ConvertToDouble(exp);
        }

        internal SqlExpression ConvertTo(Type clrType, SqlExpression expr)
        {
            if (clrType.IsGenericType && (clrType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                clrType = clrType.GetGenericArguments()[0];
            }
            bool flag = clrType == typeof(TimeSpan);
            if (this.IsSqlTimeType(expr))
            {
                if (flag)
                {
                    return expr;
                }
                expr = this.ConvertToDateTime(expr);
            }
            return this.UnaryConvert(clrType, this.typeProvider.From(clrType), expr, expr.SourceExpression);
        }

        internal SqlExpression ConvertTo(Type clrType, ProviderType sqlType, SqlExpression expr) => 
            this.UnaryConvert(clrType, sqlType, expr, expr.SourceExpression);

        internal SqlExpression ConvertToBigint(SqlExpression expr) => 
            this.ConvertTo(typeof(long), expr);

        internal SqlExpression ConvertToBool(SqlExpression expr) => 
            this.ConvertTo(typeof(bool), expr);

        internal SqlExpression ConvertToDateTime(SqlExpression expr) => 
            this.UnaryConvert(typeof(DateTime), this.typeProvider.From(typeof(DateTime)), expr, expr.SourceExpression);

        internal SqlExpression ConvertToDouble(SqlExpression expr) => 
            this.ConvertTo(typeof(double), expr);

        internal SqlExpression ConvertToInt(SqlExpression expr) => 
            this.ConvertTo(typeof(int), expr);

        internal SqlExpression DATALENGTH(SqlExpression expr) => 
            this.FunctionCall(typeof(int), "DATALENGTH", new SqlExpression[] { expr }, expr.SourceExpression);

        internal SqlExpression DATEADD(string partName, SqlExpression value, SqlExpression expr) => 
            this.DATEADD(partName, value, expr, expr.SourceExpression, false);

        internal SqlExpression DATEADD(string partName, SqlExpression value, SqlExpression expr, Expression sourceExpression, bool asNullable)
        {
            Type clrType = asNullable ? typeof(DateTime?) : typeof(DateTime);
            return this.FunctionCall(clrType, "DATEADD", new SqlExpression[] { new SqlVariable(typeof(void), null, partName, sourceExpression), value, expr }, sourceExpression);
        }

        internal SqlExpression DATEPART(string partName, SqlExpression expr) => 
            this.FunctionCall(typeof(int), "DATEPART", new SqlExpression[] { new SqlVariable(typeof(void), null, partName, expr.SourceExpression), expr }, expr.SourceExpression);

        internal SqlExpression DATETIMEOFFSETADD(string partName, SqlExpression value, SqlExpression expr) => 
            this.DATETIMEOFFSETADD(partName, value, expr, expr.SourceExpression, false);

        internal SqlExpression DATETIMEOFFSETADD(string partName, SqlExpression value, SqlExpression expr, Expression sourceExpression, bool asNullable)
        {
            Type clrType = asNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
            return this.FunctionCall(clrType, "DATEADD", new SqlExpression[] { new SqlVariable(typeof(void), null, partName, sourceExpression), value, expr }, sourceExpression);
        }

        internal ProviderType Default(MetaDataMember member)
        {
            if (member == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("member");
            }
            if (member.DbType != null)
            {
                return this.typeProvider.Parse(member.DbType);
            }
            return this.typeProvider.From(member.Type);
        }

        internal ProviderType Default(Type clrType) => 
            this.typeProvider.From(clrType);

        internal SqlExpression DiscriminatedType(SqlExpression discriminator, MetaType targetType) => 
            new SqlDiscriminatedType(this.typeProvider.From(typeof(Type)), discriminator, targetType, discriminator.SourceExpression);

        internal SqlExpression Divide(SqlExpression first, SqlExpression second) => 
            this.Binary(SqlNodeType.Div, first, second);

        internal SqlExpression Divide(SqlExpression expr, long second) => 
            this.Binary(SqlNodeType.Div, expr, this.ValueFromObject(second, false, expr.SourceExpression));

        internal SqlDoNotVisitExpression DoNotVisitExpression(SqlExpression expr) => 
            new SqlDoNotVisitExpression(expr);

        internal SqlExprSet ExprSet(SqlExpression[] exprs, Expression sourceExpression) => 
            new SqlExprSet(exprs[0].ClrType, exprs, sourceExpression);

        internal SqlFunctionCall FunctionCall(Type clrType, string name, IEnumerable<SqlExpression> args, Expression source) => 
            new SqlFunctionCall(clrType, this.Default(clrType), name, args, source);

        internal SqlFunctionCall FunctionCall(Type clrType, ProviderType sqlType, string name, IEnumerable<SqlExpression> args, Expression source) => 
            new SqlFunctionCall(clrType, sqlType, name, args, source);

        internal SqlIn In(SqlExpression expr, IEnumerable<SqlExpression> values, Expression source) => 
            new SqlIn(typeof(bool), this.typeProvider.From(typeof(bool)), expr, values, source);

        internal bool IsSqlDateTimeOffsetType(SqlExpression exp) => 
            (((SqlTypeSystem.SqlType) exp.SqlType).SqlDbType == SqlDbType.DateTimeOffset);

        internal bool IsSqlDateTimeType(SqlExpression exp)
        {
            SqlDbType sqlDbType = ((SqlTypeSystem.SqlType) exp.SqlType).SqlDbType;
            if (sqlDbType != SqlDbType.DateTime)
            {
                return (sqlDbType == SqlDbType.SmallDateTime);
            }
            return true;
        }

        internal bool IsSqlDateType(SqlExpression exp) => 
            (((SqlTypeSystem.SqlType) exp.SqlType).SqlDbType == SqlDbType.Date);

        internal bool IsSqlHighPrecisionDateTimeType(SqlExpression exp)
        {
            SqlDbType sqlDbType = ((SqlTypeSystem.SqlType) exp.SqlType).SqlDbType;
            if ((sqlDbType != SqlDbType.Time) && (sqlDbType != SqlDbType.DateTime2))
            {
                return (sqlDbType == SqlDbType.DateTimeOffset);
            }
            return true;
        }

        internal bool IsSqlTimeType(SqlExpression exp) => 
            (((SqlTypeSystem.SqlType) exp.SqlType).SqlDbType == SqlDbType.Time);

        internal SqlExpression LEN(SqlExpression expr) => 
            this.FunctionCall(typeof(int), "LEN", new SqlExpression[] { expr }, expr.SourceExpression);

        internal SqlLike Like(SqlExpression expr, SqlExpression pattern, SqlExpression escape, Expression source) => 
            new SqlLike(typeof(bool), this.typeProvider.From(typeof(bool)), expr, pattern, escape, source);

        internal SqlMember Member(SqlExpression expr, MetaDataMember member) => 
            new SqlMember(member.Type, this.Default(member), expr, member.Member);

        internal SqlMember Member(SqlExpression expr, MemberInfo member)
        {
            Type memberType = TypeSystem.GetMemberType(member);
            MetaType metaType = this.model.GetMetaType(member.DeclaringType);
            MetaDataMember dataMember = metaType.GetDataMember(member);
            if ((metaType != null) && (dataMember != null))
            {
                return new SqlMember(memberType, this.Default(dataMember), expr, member);
            }
            return new SqlMember(memberType, this.Default(memberType), expr, member);
        }

        internal SqlMethodCall MethodCall(MethodInfo method, SqlExpression obj, SqlExpression[] args, Expression sourceExpression) => 
            new SqlMethodCall(method.ReturnType, this.Default(method.ReturnType), method, obj, args, sourceExpression);

        internal SqlMethodCall MethodCall(Type returnType, MethodInfo method, SqlExpression obj, SqlExpression[] args, Expression sourceExpression) => 
            new SqlMethodCall(returnType, this.Default(returnType), method, obj, args, sourceExpression);

        internal SqlExpression Mod(SqlExpression expr, long second) => 
            this.Binary(SqlNodeType.Mod, expr, this.ValueFromObject(second, false, expr.SourceExpression));

        internal SqlExpression Multiply(params SqlExpression[] expressions)
        {
            SqlExpression right = expressions[expressions.Length - 1];
            for (int i = expressions.Length - 2; i >= 0; i--)
            {
                right = this.Binary(SqlNodeType.Mul, expressions[i], right);
            }
            return right;
        }

        internal SqlExpression Multiply(SqlExpression expr, long second) => 
            this.Binary(SqlNodeType.Mul, expr, this.ValueFromObject(second, false, expr.SourceExpression));

        internal SqlNew New(MetaType type, ConstructorInfo cons, IEnumerable<SqlExpression> args, IEnumerable<MemberInfo> argMembers, IEnumerable<SqlMemberAssign> bindings, Expression sourceExpression) => 
            new SqlNew(type, this.typeProvider.From(type.Type), cons, args, argMembers, bindings, sourceExpression);

        internal SqlExpression ObjectType(SqlExpression obj, Expression sourceExpression) => 
            new SqlObjectType(obj, this.typeProvider.From(typeof(Type)), sourceExpression);

        internal SqlExpression OrAccumulate(SqlExpression left, SqlExpression right)
        {
            if (left == null)
            {
                return right;
            }
            if (right == null)
            {
                return left;
            }
            return this.Binary(SqlNodeType.Or, left, right);
        }

        internal SqlExpression Parameter(object value, Expression source)
        {
            Type clrType = value.GetType();
            return this.Value(clrType, this.typeProvider.From(value), value, true, source);
        }

        internal SqlRowNumber RowNumber(List<SqlOrderExpression> orderBy, Expression sourceExpression) => 
            new SqlRowNumber(typeof(long), this.typeProvider.From(typeof(long)), orderBy, sourceExpression);

        internal SqlSearchedCase SearchedCase(SqlWhen[] whens, SqlExpression @else, Expression sourceExpression) => 
            new SqlSearchedCase(whens[0].Value.ClrType, whens, @else, sourceExpression);

        internal SqlExpression StaticType(MetaType typeOf, Expression sourceExpression)
        {
            if (typeOf == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("typeOf");
            }
            if (typeOf.InheritanceCode == null)
            {
                return new SqlValue(typeof(Type), this.typeProvider.From(typeof(Type)), typeOf.Type, false, sourceExpression);
            }
            Type clrType = typeOf.InheritanceCode.GetType();
            SqlValue discriminator = new SqlValue(clrType, this.typeProvider.From(clrType), typeOf.InheritanceCode, true, sourceExpression);
            return this.DiscriminatedType(discriminator, typeOf);
        }

        internal SqlSubSelect SubSelect(SqlNodeType nt, SqlSelect select) => 
            this.SubSelect(nt, select, null);

        internal SqlSubSelect SubSelect(SqlNodeType nt, SqlSelect select, Type clrType)
        {
            ProviderType sqlType = null;
            SqlNodeType type2 = nt;
            if (type2 <= SqlNodeType.Exists)
            {
                switch (type2)
                {
                    case SqlNodeType.Element:
                        goto Label_0022;

                    case SqlNodeType.Exists:
                        clrType = typeof(bool);
                        sqlType = this.typeProvider.From(typeof(bool));
                        break;
                }
                goto Label_0098;
            }
            if (type2 == SqlNodeType.Multiset)
            {
                if (clrType == null)
                {
                    clrType = typeof(List<>).MakeGenericType(new Type[] { select.Selection.ClrType });
                }
                sqlType = this.typeProvider.GetApplicationType(1);
                goto Label_0098;
            }
            if (type2 != SqlNodeType.ScalarSubSelect)
            {
                goto Label_0098;
            }
        Label_0022:
            clrType = select.Selection.ClrType;
            sqlType = select.Selection.SqlType;
        Label_0098:
            return new SqlSubSelect(nt, clrType, sqlType, select);
        }

        internal SqlExpression Subtract(SqlExpression first, SqlExpression second) => 
            this.Binary(SqlNodeType.Sub, first, second);

        internal SqlExpression Subtract(SqlExpression expr, int second) => 
            this.Binary(SqlNodeType.Sub, expr, this.ValueFromObject(second, false, expr.SourceExpression));

        internal SqlTable Table(MetaTable table, MetaType rowType, Expression sourceExpression) => 
            new SqlTable(table, rowType, this.typeProvider.GetApplicationType(0), sourceExpression);

        internal SqlTableValuedFunctionCall TableValuedFunctionCall(MetaType rowType, Type clrType, string name, IEnumerable<SqlExpression> args, Expression source) => 
            new SqlTableValuedFunctionCall(rowType, clrType, this.Default(clrType), name, args, source);

        internal SqlExpression TypeCase(Type clrType, MetaType rowType, SqlExpression discriminator, IEnumerable<SqlTypeCaseWhen> whens, Expression sourceExpression) => 
            new SqlTypeCase(clrType, this.typeProvider.From(clrType), rowType, discriminator, whens, sourceExpression);

        public SqlExpression TypedLiteralNull(Type type, Expression sourceExpression) => 
            this.ValueFromObject(null, type, false, sourceExpression);

        internal SqlUnary Unary(SqlNodeType nodeType, SqlExpression expression) => 
            this.Unary(nodeType, expression, expression.SourceExpression);

        internal SqlUnary Unary(SqlNodeType nodeType, SqlExpression expression, Expression sourceExpression) => 
            this.Unary(nodeType, expression, null, sourceExpression);

        internal SqlUnary Unary(SqlNodeType nodeType, SqlExpression expression, MethodInfo method, Expression sourceExpression)
        {
            Type clrType = null;
            ProviderType sqlType = null;
            if (nodeType == SqlNodeType.Count)
            {
                clrType = typeof(int);
                sqlType = this.typeProvider.From(typeof(int));
            }
            else if (nodeType == SqlNodeType.LongCount)
            {
                clrType = typeof(long);
                sqlType = this.typeProvider.From(typeof(long));
            }
            else if (nodeType == SqlNodeType.ClrLength)
            {
                clrType = typeof(int);
                sqlType = this.typeProvider.From(typeof(int));
            }
            else
            {
                if (nodeType.IsPredicateUnaryOperator())
                {
                    clrType = typeof(bool);
                }
                else
                {
                    clrType = expression.ClrType;
                }
                sqlType = this.typeProvider.PredictTypeForUnary(nodeType, expression.SqlType);
            }
            return new SqlUnary(nodeType, clrType, sqlType, expression, method, sourceExpression);
        }

        internal SqlUnary UnaryCast(Type targetClrType, ProviderType targetSqlType, SqlExpression expression, Expression sourceExpression) => 
            new SqlUnary(SqlNodeType.Cast, targetClrType, targetSqlType, expression, null, sourceExpression);

        internal SqlUnary UnaryConvert(Type targetClrType, ProviderType targetSqlType, SqlExpression expression, Expression sourceExpression) => 
            new SqlUnary(SqlNodeType.Convert, targetClrType, targetSqlType, expression, null, sourceExpression);

        internal SqlUnary UnaryValueOf(SqlExpression expression, Expression sourceExpression) => 
            new SqlUnary(SqlNodeType.ValueOf, TypeSystem.GetNonNullableType(expression.ClrType), expression.SqlType, expression, null, sourceExpression);

        internal SqlExpression Value(Type clrType, ProviderType sqlType, object value, bool isClientSpecified, Expression sourceExpression)
        {
            if (typeof(Type).IsAssignableFrom(clrType))
            {
                MetaType metaType = this.model.GetMetaType((Type) value);
                return this.StaticType(metaType, sourceExpression);
            }
            return new SqlValue(clrType, sqlType, value, isClientSpecified, sourceExpression);
        }

        internal SqlExpression ValueFromObject(object value, Expression sourceExpression) => 
            this.ValueFromObject(value, false, sourceExpression);

        internal SqlExpression ValueFromObject(object value, bool isClientSpecified, Expression sourceExpression)
        {
            if (value == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
            }
            Type clrType = value.GetType();
            return this.ValueFromObject(value, clrType, isClientSpecified, sourceExpression);
        }

        internal SqlExpression ValueFromObject(object value, Type clrType, bool isClientSpecified, Expression sourceExpression)
        {
            if (clrType == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("clrType");
            }
            ProviderType sqlType = (value == null) ? this.typeProvider.From(clrType) : this.typeProvider.From(value);
            return this.Value(clrType, sqlType, value, isClientSpecified, sourceExpression);
        }

        internal TypeSystemProvider TypeProvider =>
            this.typeProvider;
    }
}

