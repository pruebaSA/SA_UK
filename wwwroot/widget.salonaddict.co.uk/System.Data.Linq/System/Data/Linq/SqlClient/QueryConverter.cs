namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class QueryConverter
    {
        private bool allowDeferred;
        private System.Data.Linq.SqlClient.ConverterStrategy converterStrategy;
        private Expression dominatingExpression;
        private Dictionary<ParameterExpression, SqlNode> dupMap;
        private Dictionary<ParameterExpression, Expression> exprMap;
        private Dictionary<SqlNode, GroupInfo> gmap;
        private Dictionary<ParameterExpression, SqlExpression> map;
        private bool outerNode;
        private IDataServices services;
        private SqlFactory sql;
        private Translator translator;
        private TypeSystemProvider typeProvider;

        internal QueryConverter(IDataServices services, TypeSystemProvider typeProvider, Translator translator, SqlFactory sql)
        {
            if (services == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("services");
            }
            if (sql == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("sql");
            }
            if (translator == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("translator");
            }
            if (typeProvider == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("typeProvider");
            }
            this.services = services;
            this.translator = translator;
            this.sql = sql;
            this.typeProvider = typeProvider;
            this.map = new Dictionary<ParameterExpression, SqlExpression>();
            this.exprMap = new Dictionary<ParameterExpression, Expression>();
            this.dupMap = new Dictionary<ParameterExpression, SqlNode>();
            this.gmap = new Dictionary<SqlNode, GroupInfo>();
            this.allowDeferred = true;
        }

        private bool CanSkipOnSelection(SqlExpression selection)
        {
            if (!this.IsGrouping(selection.ClrType))
            {
                if (this.services.Model.GetTable(selection.ClrType) != null)
                {
                    return true;
                }
                if (TypeSystem.IsSequenceType(selection.ClrType) && !selection.SqlType.CanBeColumn)
                {
                    return false;
                }
                switch (selection.NodeType)
                {
                    case SqlNodeType.AliasRef:
                    {
                        SqlNode node = ((SqlAliasRef) selection).Alias.Node;
                        SqlSelect select = node as SqlSelect;
                        if (select != null)
                        {
                            return this.CanSkipOnSelection(select.Selection);
                        }
                        SqlUnion union = node as SqlUnion;
                        if (union != null)
                        {
                            bool flag = false;
                            bool flag2 = false;
                            SqlSelect left = union.Left as SqlSelect;
                            if (left != null)
                            {
                                flag = this.CanSkipOnSelection(left.Selection);
                            }
                            SqlSelect right = union.Right as SqlSelect;
                            if (right != null)
                            {
                                flag2 = this.CanSkipOnSelection(right.Selection);
                            }
                            return (flag && flag2);
                        }
                        SqlExpression expression = (SqlExpression) node;
                        return this.CanSkipOnSelection(expression);
                    }
                    case SqlNodeType.New:
                    {
                        SqlNew new2 = (SqlNew) selection;
                        foreach (SqlMemberAssign assign in new2.Members)
                        {
                            if (!this.CanSkipOnSelection(assign.Expression))
                            {
                                return false;
                            }
                        }
                        if (new2.ArgMembers != null)
                        {
                            int num = 0;
                            int count = new2.ArgMembers.Count;
                            while (num < count)
                            {
                                if (!this.CanSkipOnSelection(new2.Args[num]))
                                {
                                    return false;
                                }
                                num++;
                            }
                        }
                        break;
                    }
                }
            }
            return true;
        }

        private SqlNode ChangeType(SqlExpression expr, Type type)
        {
            if (type == typeof(object))
            {
                return expr;
            }
            if ((expr.NodeType == SqlNodeType.Value) && (((SqlValue) expr).Value == null))
            {
                return this.sql.TypedLiteralNull(type, expr.SourceExpression);
            }
            if (expr.NodeType == SqlNodeType.ClientParameter)
            {
                SqlClientParameter parameter = (SqlClientParameter) expr;
                return new SqlClientParameter(type, this.sql.TypeProvider.From(type), Expression.Lambda(Expression.Convert(parameter.Accessor.Body, type), parameter.Accessor.Parameters.ToArray<ParameterExpression>()), parameter.SourceExpression);
            }
            ConversionMethod method = this.ChooseConversionMethod(expr.ClrType, type);
            switch (method)
            {
                case ConversionMethod.Treat:
                    return new SqlUnary(SqlNodeType.Treat, type, this.typeProvider.From(type), expr, expr.SourceExpression);

                case ConversionMethod.Ignore:
                    return expr;

                case ConversionMethod.Convert:
                    return this.sql.UnaryConvert(type, this.typeProvider.From(type), expr, expr.SourceExpression);

                case ConversionMethod.Lift:
                    return new SqlLift(type, expr, this.dominatingExpression);
            }
            throw System.Data.Linq.SqlClient.Error.UnhandledExpressionType(method);
        }

        private void CheckContext(SqlExpression expr)
        {
            SqlValue value2 = expr as SqlValue;
            if (value2 != null)
            {
                DataContext context = value2.Value as DataContext;
                if ((context != null) && (context != this.services.Context))
                {
                    throw System.Data.Linq.SqlClient.Error.WrongDataContext();
                }
            }
        }

        private static Expression ChooseBestDominatingExpression(Expression last, Expression next)
        {
            if (last != null)
            {
                if (next == null)
                {
                    return last;
                }
                if (next is MethodCallExpression)
                {
                    return next;
                }
                if (last is MethodCallExpression)
                {
                    return last;
                }
            }
            return next;
        }

        private ConversionMethod ChooseConversionMethod(Type fromType, Type toType)
        {
            Type nonNullableType = TypeSystem.GetNonNullableType(fromType);
            Type seqType = TypeSystem.GetNonNullableType(toType);
            if ((fromType != toType) && (nonNullableType == seqType))
            {
                return ConversionMethod.Lift;
            }
            if (!TypeSystem.IsSequenceType(nonNullableType) && !TypeSystem.IsSequenceType(seqType))
            {
                ProviderType type3 = this.typeProvider.From(nonNullableType);
                ProviderType type4 = this.typeProvider.From(seqType);
                bool isRuntimeOnlyType = type3.IsRuntimeOnlyType;
                bool flag2 = type4.IsRuntimeOnlyType;
                if (isRuntimeOnlyType || flag2)
                {
                    return ConversionMethod.Treat;
                }
                if (((nonNullableType != seqType) && (!type3.IsString || !type3.Equals(type4))) && (!nonNullableType.IsEnum && !seqType.IsEnum))
                {
                    return ConversionMethod.Convert;
                }
            }
            return ConversionMethod.Ignore;
        }

        private SqlSelect CoerceToSequence(SqlNode node)
        {
            SqlSelect select = node as SqlSelect;
            if (select != null)
            {
                return select;
            }
            if (node.NodeType == SqlNodeType.Value)
            {
                SqlValue value2 = (SqlValue) node;
                ITable table = value2.Value as ITable;
                if (table != null)
                {
                    return this.CoerceToSequence(this.TranslateConstantTable(table, null));
                }
                IQueryable queryable = value2.Value as IQueryable;
                if (queryable == null)
                {
                    throw System.Data.Linq.SqlClient.Error.CapturedValuesCannotBeSequences();
                }
                Expression exp = Funcletizer.Funcletize(queryable.Expression);
                return this.VisitSequence(exp);
            }
            if ((node.NodeType == SqlNodeType.Multiset) || (node.NodeType == SqlNodeType.Element))
            {
                return ((SqlSubSelect) node).Select;
            }
            if (node.NodeType == SqlNodeType.ClientArray)
            {
                throw System.Data.Linq.SqlClient.Error.ConstructedArraysNotSupported();
            }
            if (node.NodeType == SqlNodeType.ClientParameter)
            {
                throw System.Data.Linq.SqlClient.Error.ParametersCannotBeSequences();
            }
            SqlExpression expression2 = (SqlExpression) node;
            SqlAlias from = new SqlAlias(expression2);
            return new SqlSelect(new SqlAliasRef(from), from, this.dominatingExpression);
        }

        private void CompensateForLowerPrecedenceOfDateType(ref SqlExpression left, ref SqlExpression right)
        {
            SqlExpression expression;
            if (this.sql.IsSqlDateType(left) && this.sql.IsSqlDateTimeType(right))
            {
                expression = new SqlVariable(right.ClrType, right.SqlType, "DATETIME2", right.SourceExpression);
                right = this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression, right }, right.SourceExpression);
            }
            else if (this.sql.IsSqlDateType(right) && this.sql.IsSqlDateTimeType(left))
            {
                expression = new SqlVariable(left.ClrType, left.SqlType, "DATETIME2", left.SourceExpression);
                left = this.sql.FunctionCall(typeof(DateTime), "CONVERT", new SqlExpression[] { expression, left }, left.SourceExpression);
            }
        }

        internal SqlNode ConvertInner(Expression node, Expression dominantExpression)
        {
            this.dominatingExpression = dominantExpression;
            bool outerNode = this.outerNode;
            this.outerNode = false;
            SqlNode node2 = this.VisitInner(node);
            this.outerNode = outerNode;
            return node2;
        }

        internal SqlNode ConvertOuter(Expression node)
        {
            SqlNode node2;
            this.dominatingExpression = node;
            this.outerNode = true;
            if (typeof(ITable).IsAssignableFrom(node.Type))
            {
                node2 = this.VisitSequence(node);
            }
            else
            {
                node2 = this.VisitInner(node);
            }
            if (node2.NodeType == SqlNodeType.MethodCall)
            {
                throw System.Data.Linq.SqlClient.Error.InvalidMethodExecution(((SqlMethodCall) node2).Method.Name);
            }
            SqlExpression selection = node2 as SqlExpression;
            if (selection != null)
            {
                node2 = new SqlSelect(selection, null, this.dominatingExpression);
            }
            return new SqlIncludeScope(node2, this.dominatingExpression);
        }

        private GroupInfo FindGroupInfo(SqlNode source)
        {
            GroupInfo info = null;
            this.gmap.TryGetValue(source, out info);
            if (info == null)
            {
                SqlAlias alias = source as SqlAlias;
                if (alias != null)
                {
                    SqlSelect node = alias.Node as SqlSelect;
                    if (node != null)
                    {
                        return this.FindGroupInfo(node.Selection);
                    }
                    source = alias.Node;
                }
                SqlExpression key = source as SqlExpression;
                if (key == null)
                {
                    return null;
                }
                switch (key.NodeType)
                {
                    case SqlNodeType.AliasRef:
                        return this.FindGroupInfo(((SqlAliasRef) key).Alias);

                    case SqlNodeType.Member:
                        return this.FindGroupInfo(((SqlMember) key).Expression);
                }
                this.gmap.TryGetValue(key, out info);
            }
            return info;
        }

        private SqlExpression GenerateInExpression(SqlExpression expr, List<SqlExpression> list)
        {
            if (list.Count == 0)
            {
                return this.sql.ValueFromObject(false, this.dominatingExpression);
            }
            if (list[0].SqlType.CanBeColumn)
            {
                return this.sql.In(expr, list, this.dominatingExpression);
            }
            SqlExpression left = this.sql.Binary(SqlNodeType.EQ, expr, list[0]);
            int num = 1;
            int count = list.Count;
            while (num < count)
            {
                left = this.sql.Binary(SqlNodeType.Or, left, this.sql.Binary(SqlNodeType.EQ, (SqlExpression) SqlDuplicator.Copy(expr), list[num]));
                num++;
            }
            return left;
        }

        private SqlExpression GenerateQuantifier(SqlAlias alias, SqlExpression cond, bool isAny)
        {
            SqlAliasRef selection = new SqlAliasRef(alias);
            if (isAny)
            {
                SqlSelect select = new SqlSelect(selection, alias, this.dominatingExpression) {
                    Where = cond,
                    OrderingType = SqlOrderingType.Never
                };
                return this.sql.SubSelect(SqlNodeType.Exists, select);
            }
            SqlSelect select3 = new SqlSelect(selection, alias, this.dominatingExpression);
            SqlSubSelect expression = this.sql.SubSelect(SqlNodeType.Exists, select3);
            select3.Where = this.sql.Unary(SqlNodeType.Not2V, cond, this.dominatingExpression);
            return this.sql.Unary(SqlNodeType.Not, expression, this.dominatingExpression);
        }

        private SqlSelect GenerateSkipTake(SqlSelect sequence, SqlExpression skipExp, SqlExpression takeExp)
        {
            SqlSelect node = this.LockSelect(sequence);
            SqlValue value2 = skipExp as SqlValue;
            if ((skipExp == null) || ((value2 != null) && (((int) value2.Value) <= 0)))
            {
                if (takeExp != null)
                {
                    node.Top = takeExp;
                }
                return node;
            }
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef selection = new SqlAliasRef(alias);
            if (this.UseConverterStrategy(System.Data.Linq.SqlClient.ConverterStrategy.SkipWithRowNumber))
            {
                SqlColumn col = new SqlColumn("ROW_NUMBER", this.sql.RowNumber(new List<SqlOrderExpression>(), this.dominatingExpression));
                SqlColumnRef expr = new SqlColumnRef(col);
                node.Row.Columns.Add(col);
                SqlSelect select2 = new SqlSelect(selection, alias, this.dominatingExpression);
                if (takeExp != null)
                {
                    select2.Where = this.sql.Between(expr, this.sql.Add(skipExp, 1), this.sql.Binary(SqlNodeType.Add, (SqlExpression) SqlDuplicator.Copy(skipExp), takeExp), this.dominatingExpression);
                    return select2;
                }
                select2.Where = this.sql.Binary(SqlNodeType.GT, expr, skipExp);
                return select2;
            }
            if (!this.CanSkipOnSelection(node.Selection))
            {
                throw System.Data.Linq.SqlClient.Error.SkipNotSupportedForSequenceTypes();
            }
            SingleTableQueryVisitor visitor = new SingleTableQueryVisitor();
            visitor.Visit(node);
            if (!visitor.IsValid)
            {
                throw System.Data.Linq.SqlClient.Error.SkipRequiresSingleTableQueryWithPKs();
            }
            SqlSelect select3 = (SqlSelect) SqlDuplicator.Copy(node);
            select3.Top = skipExp;
            SqlAlias alias2 = new SqlAlias(select3);
            SqlAliasRef ref4 = new SqlAliasRef(alias2);
            SqlSelect select = new SqlSelect(ref4, alias2, this.dominatingExpression) {
                Where = this.sql.Binary(SqlNodeType.EQ2V, selection, ref4)
            };
            SqlSubSelect expression = this.sql.SubSelect(SqlNodeType.Exists, select);
            return new SqlSelect(selection, alias, this.dominatingExpression) { 
                Where = this.sql.Unary(SqlNodeType.Not, expression, this.dominatingExpression),
                Top = takeExp
            };
        }

        private SqlExpression GetAggregate(SqlNodeType aggType, Type clrType, SqlExpression exp) => 
            new SqlUnary(aggType, clrType, this.typeProvider.From(clrType), exp, this.dominatingExpression);

        private static Expression[] GetArray(Expression array)
        {
            NewArrayExpression expression = array as NewArrayExpression;
            if (expression != null)
            {
                return expression.Expressions.ToArray<Expression>();
            }
            ConstantExpression expression2 = array as ConstantExpression;
            if (expression2 != null)
            {
                object[] objArray = expression2.Value as object[];
                if (objArray != null)
                {
                    Type elemType = TypeSystem.GetElementType(expression2.Type);
                    return (from o in objArray select Expression.Constant(o, elemType)).ToArray<ConstantExpression>();
                }
            }
            return new Expression[0];
        }

        private List<SqlExpression> GetFunctionParameters(MethodCallExpression mce, MetaFunction function)
        {
            List<SqlExpression> list = new List<SqlExpression>(mce.Arguments.Count);
            int num = 0;
            int count = mce.Arguments.Count;
            while (num < count)
            {
                SqlExpression item = this.VisitExpression(mce.Arguments[num]);
                MetaParameter parameter = function.Parameters[num];
                if (!string.IsNullOrEmpty(parameter.DbType))
                {
                    SqlSimpleTypeExpression expression2 = item as SqlSimpleTypeExpression;
                    if (expression2 != null)
                    {
                        ProviderType type = this.typeProvider.Parse(parameter.DbType);
                        expression2.SetSqlType(type);
                    }
                }
                list.Add(item);
                num++;
            }
            return list;
        }

        private SqlExpression GetIdentityExpression(MetaDataMember id, bool isOutputFromInsert)
        {
            if (isOutputFromInsert)
            {
                return new SqlVariable(id.Type, this.sql.Default(id), "@id", this.dominatingExpression);
            }
            ProviderType type = this.sql.Default(id);
            if (!IsLegalIdentityType(type.GetClosestRuntimeType()))
            {
                throw System.Data.Linq.SqlClient.Error.InvalidDbGeneratedType(type.ToQueryString());
            }
            if ((this.converterStrategy & System.Data.Linq.SqlClient.ConverterStrategy.CanUseScopeIdentity) != System.Data.Linq.SqlClient.ConverterStrategy.Default)
            {
                return new SqlVariable(typeof(decimal), this.typeProvider.From(typeof(decimal)), "SCOPE_IDENTITY()", this.dominatingExpression);
            }
            return new SqlVariable(typeof(decimal), this.typeProvider.From(typeof(decimal)), "@@IDENTITY", this.dominatingExpression);
        }

        private LambdaExpression GetLambda(Expression expression) => 
            (this.RemoveQuotes(expression) as LambdaExpression);

        private SqlExpression GetRowCountExpression()
        {
            if ((this.converterStrategy & System.Data.Linq.SqlClient.ConverterStrategy.CanUseRowStatus) != System.Data.Linq.SqlClient.ConverterStrategy.Default)
            {
                return new SqlVariable(typeof(decimal), this.typeProvider.From(typeof(decimal)), "@@ROWCOUNT", this.dominatingExpression);
            }
            return new SqlVariable(typeof(decimal), this.typeProvider.From(typeof(decimal)), "@ROWCOUNT", this.dominatingExpression);
        }

        private object GetValue(Expression expression, string operation)
        {
            SqlExpression expression2 = this.VisitExpression(expression);
            if (expression2.NodeType != SqlNodeType.Value)
            {
                throw System.Data.Linq.SqlClient.Error.NonConstantExpressionsNotSupportedFor(operation);
            }
            return ((SqlValue) expression2).Value;
        }

        private static bool IsDataManipulationCall(MethodCallExpression mc) => 
            (mc.Method.IsStatic && (mc.Method.DeclaringType == typeof(DataManipulation)));

        private bool IsDbGeneratedKeyProjectionOnly(Expression projection, MetaDataMember keyMember)
        {
            NewArrayExpression expression = projection as NewArrayExpression;
            if ((expression != null) && (expression.Expressions.Count == 1))
            {
                Expression operand = expression.Expressions[0];
                while ((operand.NodeType == ExpressionType.Convert) || (operand.NodeType == ExpressionType.ConvertChecked))
                {
                    operand = ((UnaryExpression) operand).Operand;
                }
                MemberExpression expression3 = operand as MemberExpression;
                if ((expression3 != null) && (expression3.Member == keyMember.Member))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsGrouping(Type t) => 
            (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IGrouping<,>)));

        private bool IsLambda(Expression expression) => 
            (this.RemoveQuotes(expression).NodeType == ExpressionType.Lambda);

        private static bool IsLegalIdentityType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                    return true;
            }
            return false;
        }

        private bool IsMappedFunctionCall(MethodCallExpression mc) => 
            (this.services.Model.GetFunction(mc.Method) != null);

        private bool IsSequenceOperatorCall(MethodCallExpression mc)
        {
            Type declaringType = mc.Method.DeclaringType;
            if ((declaringType != typeof(Enumerable)) && (declaringType != typeof(Queryable)))
            {
                return false;
            }
            return true;
        }

        private bool IsSequenceOperatorCall(MethodCallExpression mc, string methodName) => 
            (this.IsSequenceOperatorCall(mc) && (mc.Method.Name == methodName));

        private SqlSelect LockSelect(SqlSelect sel)
        {
            if ((((sel.Selection.NodeType == SqlNodeType.AliasRef) && (sel.Where == null)) && ((sel.OrderBy.Count <= 0) && (sel.GroupBy.Count <= 0))) && (((sel.Having == null) && (sel.Top == null)) && ((sel.OrderingType == SqlOrderingType.Default) && !sel.IsDistinct)))
            {
                return sel;
            }
            SqlAlias from = new SqlAlias(sel);
            return new SqlSelect(new SqlAliasRef(from), from, this.dominatingExpression);
        }

        private SqlExpression MakeCoalesce(SqlExpression left, SqlExpression right, Type resultType)
        {
            this.CompensateForLowerPrecedenceOfDateType(ref left, ref right);
            if (TypeSystem.IsSimpleType(resultType))
            {
                return this.sql.Binary(SqlNodeType.Coalesce, left, right, resultType);
            }
            List<SqlWhen> list = new List<SqlWhen>(1) {
                new SqlWhen(this.sql.Unary(SqlNodeType.IsNull, left, left.SourceExpression), right)
            };
            SqlDuplicator duplicator = new SqlDuplicator(true);
            return this.sql.SearchedCase(list.ToArray(), (SqlExpression) duplicator.Duplicate(left), this.dominatingExpression);
        }

        private static IEnumerable<MemberInfo> PropertyOrFieldOf(IEnumerable<MemberInfo> members)
        {
            if (members == null)
            {
                return null;
            }
            List<MemberInfo> list = new List<MemberInfo>();
            foreach (MemberInfo info in members)
            {
                MemberTypes memberType = info.MemberType;
                if (memberType != MemberTypes.Field)
                {
                    if (memberType != MemberTypes.Method)
                    {
                        if (memberType != MemberTypes.Property)
                        {
                            throw System.Data.Linq.SqlClient.Error.CouldNotConvertToPropertyOrField(info);
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo info2 in info.DeclaringType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (info2.CanRead && (info2.GetGetMethod() == info))
                            {
                                list.Add(info2);
                                break;
                            }
                        }
                        continue;
                    }
                }
                list.Add(info);
            }
            return list;
        }

        private Expression RemoveQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression) expression).Operand;
            }
            return expression;
        }

        private SqlNode TranslateConstantTable(ITable table, SqlLink link)
        {
            if (table.Context != this.services.Context)
            {
                throw System.Data.Linq.SqlClient.Error.WrongDataContext();
            }
            MetaTable table2 = this.services.Model.GetTable(table.ElementType);
            return this.translator.BuildDefaultQuery(table2.RowType, this.allowDeferred, link, this.dominatingExpression);
        }

        private SqlNode TranslateStoredProcedureCall(MethodCallExpression mce, MetaFunction function)
        {
            if (!this.outerNode)
            {
                throw System.Data.Linq.SqlClient.Error.SprocsCannotBeComposed();
            }
            List<SqlExpression> functionParameters = this.GetFunctionParameters(mce, function);
            SqlStoredProcedureCall query = new SqlStoredProcedureCall(function, null, functionParameters, mce);
            Type returnType = mce.Method.ReturnType;
            if (returnType.IsGenericType && ((returnType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || (returnType.GetGenericTypeDefinition() == typeof(ISingleResult<>))))
            {
                MetaType inheritanceRoot = function.ResultRowTypes[0].InheritanceRoot;
                SqlUserRow item = new SqlUserRow(inheritanceRoot, this.typeProvider.GetApplicationType(0), query, mce);
                query.Projection = this.translator.BuildProjection(item, inheritanceRoot, this.allowDeferred, null, mce);
                return query;
            }
            if ((!typeof(IMultipleResults).IsAssignableFrom(returnType) && (returnType != typeof(int))) && (returnType != typeof(int?)))
            {
                throw System.Data.Linq.SqlClient.Error.InvalidReturnFromSproc(returnType);
            }
            return query;
        }

        private SqlNode TranslateTableValuedFunction(MethodCallExpression mce, MetaFunction function)
        {
            List<SqlExpression> functionParameters = this.GetFunctionParameters(mce, function);
            SqlAlias alias = new SqlAlias(this.sql.TableValuedFunctionCall(function.ResultRowTypes[0].InheritanceRoot, mce.Method.ReturnType, function.MappedName, functionParameters, mce));
            SqlAliasRef item = new SqlAliasRef(alias);
            return new SqlSelect(this.translator.BuildProjection(item, function.ResultRowTypes[0].InheritanceRoot, this.allowDeferred, null, mce), alias, mce);
        }

        private bool UseConverterStrategy(System.Data.Linq.SqlClient.ConverterStrategy strategy) => 
            ((this.converterStrategy & strategy) == strategy);

        internal SqlNode Visit(Expression node)
        {
            bool outerNode = this.outerNode;
            this.outerNode = false;
            SqlNode node2 = this.VisitInner(node);
            this.outerNode = outerNode;
            return node2;
        }

        private SqlNode VisitAggregate(Expression sequence, LambdaExpression lambda, SqlNodeType aggType, Type returnType)
        {
            bool flag = (aggType == SqlNodeType.Count) || (aggType == SqlNodeType.LongCount);
            SqlNode node = this.Visit(sequence);
            SqlSelect select = this.CoerceToSequence(node);
            SqlAlias from = new SqlAlias(select);
            SqlAliasRef ref2 = new SqlAliasRef(from);
            MethodCallExpression mc = sequence as MethodCallExpression;
            if (((!this.outerNode && !flag) && ((lambda == null) || ((lambda.Parameters.Count == 1) && (lambda.Parameters[0] == lambda.Body)))) && (((mc != null) && this.IsSequenceOperatorCall(mc, "Select")) && (select.From is SqlAlias)))
            {
                LambdaExpression expression2 = this.GetLambda(mc.Arguments[1]);
                lambda = Expression.Lambda(expression2.Type, expression2.Body, expression2.Parameters);
                from = (SqlAlias) select.From;
                ref2 = new SqlAliasRef(from);
            }
            if ((lambda != null) && !TypeSystem.IsSimpleType(lambda.Body.Type))
            {
                throw System.Data.Linq.SqlClient.Error.CannotAggregateType(lambda.Body.Type);
            }
            if ((select.Selection.SqlType.IsRuntimeOnlyType && !this.IsGrouping(sequence.Type)) && (!flag && (lambda == null)))
            {
                throw System.Data.Linq.SqlClient.Error.NonCountAggregateFunctionsAreNotValidOnProjections(aggType);
            }
            if (lambda != null)
            {
                this.map[lambda.Parameters[0]] = ref2;
            }
            if (this.outerNode)
            {
                SqlExpression expression3 = (lambda != null) ? this.VisitExpression(lambda.Body) : null;
                SqlExpression expression4 = null;
                if (flag && (expression3 != null))
                {
                    expression4 = expression3;
                    expression3 = null;
                }
                else if ((expression3 == null) && !flag)
                {
                    expression3 = ref2;
                }
                if (expression3 != null)
                {
                    expression3 = new SqlSimpleExpression(expression3);
                }
                return new SqlSelect(this.GetAggregate(aggType, returnType, expression3), from, this.dominatingExpression) { 
                    Where = expression4,
                    OrderingType = SqlOrderingType.Never
                };
            }
            if (!flag || (lambda == null))
            {
                GroupInfo info = this.FindGroupInfo(node);
                if (info != null)
                {
                    SqlExpression elementOnGroupSource = null;
                    if (lambda != null)
                    {
                        this.map[lambda.Parameters[0]] = (SqlExpression) SqlDuplicator.Copy(info.ElementOnGroupSource);
                        elementOnGroupSource = this.VisitExpression(lambda.Body);
                    }
                    else if (!flag)
                    {
                        elementOnGroupSource = info.ElementOnGroupSource;
                    }
                    if (elementOnGroupSource != null)
                    {
                        elementOnGroupSource = new SqlSimpleExpression(elementOnGroupSource);
                    }
                    SqlExpression expression6 = this.GetAggregate(aggType, returnType, elementOnGroupSource);
                    SqlColumn item = new SqlColumn(expression6.ClrType, expression6.SqlType, null, null, expression6, this.dominatingExpression);
                    info.SelectWithGroup.Row.Columns.Add(item);
                    return new SqlColumnRef(item);
                }
            }
            SqlExpression expr = (lambda != null) ? this.VisitExpression(lambda.Body) : null;
            if (expr != null)
            {
                expr = new SqlSimpleExpression(expr);
            }
            SqlSelect select3 = new SqlSelect(this.GetAggregate(aggType, returnType, flag ? null : ((lambda == null) ? ref2 : expr)), from, this.dominatingExpression) {
                Where = flag ? expr : null
            };
            return this.sql.SubSelect(SqlNodeType.ScalarSubSelect, select3);
        }

        private SqlNode VisitArrayIndex(BinaryExpression b)
        {
            SqlExpression expression = this.VisitExpression(b.Left);
            SqlExpression expression2 = this.VisitExpression(b.Right);
            if ((expression.NodeType != SqlNodeType.ClientParameter) || (expression2.NodeType != SqlNodeType.Value))
            {
                throw System.Data.Linq.SqlClient.Error.UnrecognizedExpressionNode(b.NodeType);
            }
            SqlClientParameter parameter = (SqlClientParameter) expression;
            SqlValue value2 = (SqlValue) expression2;
            return new SqlClientParameter(b.Type, this.sql.TypeProvider.From(b.Type), Expression.Lambda(Expression.ArrayIndex(parameter.Accessor.Body, Expression.Constant(value2.Value, value2.ClrType)), parameter.Accessor.Parameters.ToArray<ParameterExpression>()), this.dominatingExpression);
        }

        private SqlNode VisitArrayLength(UnaryExpression c)
        {
            SqlExpression expr = this.VisitExpression(c.Operand);
            if (!expr.SqlType.IsString && !expr.SqlType.IsChar)
            {
                return this.sql.DATALENGTH(expr);
            }
            return this.sql.CLRLENGTH(expr);
        }

        private SqlNode VisitAs(UnaryExpression a)
        {
            SqlNode node = this.Visit(a.Operand);
            SqlExpression expr = node as SqlExpression;
            if (expr != null)
            {
                return new SqlUnary(SqlNodeType.Treat, a.Type, this.typeProvider.From(a.Type), expr, a);
            }
            SqlSelect select = node as SqlSelect;
            if (select == null)
            {
                throw System.Data.Linq.SqlClient.Error.DidNotExpectAs(a);
            }
            return new SqlUnary(SqlNodeType.Treat, a.Type, this.typeProvider.From(a.Type), this.sql.SubSelect(SqlNodeType.Multiset, select), a);
        }

        private SqlNode VisitBinary(BinaryExpression b)
        {
            SqlExpression left = this.VisitExpression(b.Left);
            SqlExpression right = this.VisitExpression(b.Right);
            if (b.Method != null)
            {
                return this.sql.MethodCall(b.Type, b.Method, null, new SqlExpression[] { left, right }, this.dominatingExpression);
            }
            switch (b.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return this.sql.Binary(SqlNodeType.Add, left, right, b.Type);

                case ExpressionType.And:
                    if ((b.Left.Type != typeof(bool)) && (b.Left.Type != typeof(bool?)))
                    {
                        return this.sql.Binary(SqlNodeType.BitAnd, left, right, b.Type);
                    }
                    return this.sql.Binary(SqlNodeType.And, left, right, b.Type);

                case ExpressionType.AndAlso:
                    return this.sql.Binary(SqlNodeType.And, left, right, b.Type);

                case ExpressionType.Coalesce:
                    return this.MakeCoalesce(left, right, b.Type);

                case ExpressionType.Divide:
                    return this.sql.Binary(SqlNodeType.Div, left, right, b.Type);

                case ExpressionType.Equal:
                    return this.sql.Binary(SqlNodeType.EQ, left, right, b.Type);

                case ExpressionType.ExclusiveOr:
                    return this.sql.Binary(SqlNodeType.BitXor, left, right, b.Type);

                case ExpressionType.GreaterThan:
                    return this.sql.Binary(SqlNodeType.GT, left, right, b.Type);

                case ExpressionType.GreaterThanOrEqual:
                    return this.sql.Binary(SqlNodeType.GE, left, right, b.Type);

                case ExpressionType.LessThan:
                    return this.sql.Binary(SqlNodeType.LT, left, right, b.Type);

                case ExpressionType.LessThanOrEqual:
                    return this.sql.Binary(SqlNodeType.LE, left, right, b.Type);

                case ExpressionType.Modulo:
                    return this.sql.Binary(SqlNodeType.Mod, left, right, b.Type);

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return this.sql.Binary(SqlNodeType.Mul, left, right, b.Type);

                case ExpressionType.NotEqual:
                    return this.sql.Binary(SqlNodeType.NE, left, right, b.Type);

                case ExpressionType.Or:
                    if ((b.Left.Type != typeof(bool)) && (b.Left.Type != typeof(bool?)))
                    {
                        return this.sql.Binary(SqlNodeType.BitOr, left, right, b.Type);
                    }
                    return this.sql.Binary(SqlNodeType.Or, left, right, b.Type);

                case ExpressionType.OrElse:
                    return this.sql.Binary(SqlNodeType.Or, left, right, b.Type);

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.sql.Binary(SqlNodeType.Sub, left, right, b.Type);
            }
            throw System.Data.Linq.SqlClient.Error.BinaryOperatorNotRecognized(b.NodeType);
        }

        private SqlNode VisitCast(UnaryExpression c)
        {
            if (c.Method != null)
            {
                SqlExpression expression = this.VisitExpression(c.Operand);
                return this.sql.MethodCall(c.Type, c.Method, null, new SqlExpression[] { expression }, this.dominatingExpression);
            }
            return this.VisitChangeType(c.Operand, c.Type);
        }

        private SqlNode VisitChangeType(Expression expression, Type type)
        {
            SqlExpression expr = this.VisitExpression(expression);
            return this.ChangeType(expr, type);
        }

        private SqlNode VisitConcat(Expression source1, Expression source2)
        {
            SqlSelect left = this.VisitSequence(source1);
            SqlSelect right = this.VisitSequence(source2);
            SqlUnion node = new SqlUnion(left, right, true);
            SqlAlias from = new SqlAlias(node);
            return new SqlSelect(new SqlAliasRef(from), from, this.dominatingExpression) { OrderingType = SqlOrderingType.Blocked };
        }

        private SqlExpression VisitConditional(ConditionalExpression cond)
        {
            List<SqlWhen> list = new List<SqlWhen>(1) {
                new SqlWhen(this.VisitExpression(cond.Test), this.VisitExpression(cond.IfTrue))
            };
            SqlExpression @else = this.VisitExpression(cond.IfFalse);
            while (@else.NodeType == SqlNodeType.SearchedCase)
            {
                SqlSearchedCase @case = (SqlSearchedCase) @else;
                list.AddRange(@case.Whens);
                @else = @case.Else;
            }
            return this.sql.SearchedCase(list.ToArray(), @else, this.dominatingExpression);
        }

        private SqlNode VisitConstant(ConstantExpression cons)
        {
            Type type = cons.Type;
            if (cons.Value == null)
            {
                return this.sql.TypedLiteralNull(type, this.dominatingExpression);
            }
            if (type == typeof(object))
            {
                type = cons.Value.GetType();
            }
            return this.sql.ValueFromObject(cons.Value, type, true, this.dominatingExpression);
        }

        private SqlNode VisitContains(Expression sequence, Expression value)
        {
            Func<Expression, SqlExpression> selector = null;
            Func<object, SqlExpression> func2 = null;
            Type elemType = TypeSystem.GetElementType(sequence.Type);
            if (sequence.NodeType == ExpressionType.NewArrayInit)
            {
                NewArrayExpression expression = (NewArrayExpression) sequence;
                if (selector == null)
                {
                    selector = v => this.VisitExpression(v);
                }
                List<SqlExpression> list = expression.Expressions.Select<Expression, SqlExpression>(selector).ToList<SqlExpression>();
                SqlExpression expr = this.VisitExpression(value);
                return this.GenerateInExpression(expr, list);
            }
            SqlNode node = this.Visit(sequence);
            if (node.NodeType == SqlNodeType.Value)
            {
                IEnumerable source = ((SqlValue) node).Value as IEnumerable;
                IQueryable queryable = source as IQueryable;
                if (queryable == null)
                {
                    SqlExpression expression3 = this.VisitExpression(value);
                    if (func2 == null)
                    {
                        func2 = v => this.sql.ValueFromObject(v, elemType, true, this.dominatingExpression);
                    }
                    List<SqlExpression> list2 = source.OfType<object>().Select<object, SqlExpression>(func2).ToList<SqlExpression>();
                    return this.GenerateInExpression(expression3, list2);
                }
                node = this.Visit(queryable.Expression);
            }
            ParameterExpression left = Expression.Parameter(value.Type, "p");
            LambdaExpression lambda = Expression.Lambda(Expression.Equal(left, value), new ParameterExpression[] { left });
            return this.VisitQuantifier(this.CoerceToSequence(node), lambda, true);
        }

        private SqlNode VisitDataManipulationCall(MethodCallExpression mc)
        {
            if (!IsDataManipulationCall(mc))
            {
                throw System.Data.Linq.SqlClient.Error.InvalidSequenceOperatorCall(mc.Method.Name);
            }
            bool flag = false;
            string name = mc.Method.Name;
            if (name != null)
            {
                if (name == "Insert")
                {
                    flag = true;
                    if (mc.Arguments.Count == 2)
                    {
                        return this.VisitInsert(mc.Arguments[0], this.GetLambda(mc.Arguments[1]));
                    }
                    if (mc.Arguments.Count == 1)
                    {
                        return this.VisitInsert(mc.Arguments[0], null);
                    }
                }
                else if (name == "Update")
                {
                    flag = true;
                    if (mc.Arguments.Count == 3)
                    {
                        return this.VisitUpdate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), this.GetLambda(mc.Arguments[2]));
                    }
                    if (mc.Arguments.Count == 2)
                    {
                        if (mc.Method.GetGenericArguments().Length == 1)
                        {
                            return this.VisitUpdate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), null);
                        }
                        return this.VisitUpdate(mc.Arguments[0], null, this.GetLambda(mc.Arguments[1]));
                    }
                    if (mc.Arguments.Count == 1)
                    {
                        return this.VisitUpdate(mc.Arguments[0], null, null);
                    }
                }
                else if (name == "Delete")
                {
                    flag = true;
                    if (mc.Arguments.Count == 2)
                    {
                        return this.VisitDelete(mc.Arguments[0], this.GetLambda(mc.Arguments[1]));
                    }
                    if (mc.Arguments.Count == 1)
                    {
                        return this.VisitDelete(mc.Arguments[0], null);
                    }
                }
            }
            if (flag)
            {
                throw System.Data.Linq.SqlClient.Error.QueryOperatorOverloadNotSupported(mc.Method.Name);
            }
            throw System.Data.Linq.SqlClient.Error.QueryOperatorNotSupported(mc.Method.Name);
        }

        private SqlSelect VisitDefaultIfEmpty(Expression sequence)
        {
            SqlAlias alias = new SqlAlias(this.VisitSequence(sequence));
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            SqlExpression selection = new SqlOptionalValue(new SqlColumn("test", this.sql.Unary(SqlNodeType.OuterJoinedValue, this.sql.Value(typeof(int?), this.typeProvider.From(typeof(int)), 1, false, this.dominatingExpression))), this.sql.Unary(SqlNodeType.OuterJoinedValue, ref2));
            SqlSelect node = new SqlSelect(selection, alias, this.dominatingExpression);
            alias = new SqlAlias(node);
            ref2 = new SqlAliasRef(alias);
            SqlSelect select3 = new SqlSelect(this.sql.TypedLiteralNull(typeof(string), this.dominatingExpression), null, this.dominatingExpression);
            SqlAlias left = new SqlAlias(select3);
            return new SqlSelect(ref2, new SqlJoin(SqlJoinType.OuterApply, left, alias, null, this.dominatingExpression), this.dominatingExpression);
        }

        private SqlStatement VisitDelete(Expression item, LambdaExpression check)
        {
            SqlStatement statement;
            if (item == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("item");
            }
            bool allowDeferred = this.allowDeferred;
            this.allowDeferred = false;
            try
            {
                MetaTable table = this.services.Model.GetTable(item.Type);
                Expression sourceExpression = this.services.Context.GetTable(table.RowType.Type).Expression;
                Type type = table.RowType.Type;
                ParameterExpression left = Expression.Parameter(type, "p");
                LambdaExpression expression = Expression.Lambda(Expression.Equal(left, item), new ParameterExpression[] { left });
                if (check != null)
                {
                    expression = Expression.Lambda(Expression.And(Expression.Invoke(expression, new Expression[] { left }), Expression.Invoke(check, new Expression[] { left })), new ParameterExpression[] { left });
                }
                Expression exp = Expression.Call(typeof(Enumerable), "Where", new Type[] { type }, new Expression[] { sourceExpression, expression });
                SqlSelect select = new RetypeCheckClause().VisitSelect(this.VisitSequence(exp));
                this.allowDeferred = allowDeferred;
                SqlDelete delete = new SqlDelete(select, sourceExpression);
                statement = delete;
            }
            finally
            {
                this.allowDeferred = allowDeferred;
            }
            return statement;
        }

        private SqlSelect VisitDistinct(Expression sequence)
        {
            SqlSelect select = this.LockSelect(this.VisitSequence(sequence));
            select.IsDistinct = true;
            select.OrderingType = SqlOrderingType.Blocked;
            return select;
        }

        private SqlNode VisitExcept(Expression source1, Expression source2)
        {
            Type elementType = TypeSystem.GetElementType(source1.Type);
            if (this.IsGrouping(elementType))
            {
                throw System.Data.Linq.SqlClient.Error.ExceptNotSupportedForHierarchicalTypes();
            }
            SqlSelect node = this.LockSelect(this.VisitSequence(source1));
            SqlSelect select2 = this.VisitSequence(source2);
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef left = new SqlAliasRef(alias);
            SqlAlias alias2 = new SqlAlias(select2);
            SqlAliasRef right = new SqlAliasRef(alias2);
            SqlExpression expression = this.GenerateQuantifier(alias2, this.sql.Binary(SqlNodeType.EQ2V, left, right), true);
            return new SqlSelect(left, alias, node.SourceExpression) { 
                Where = this.sql.Unary(SqlNodeType.Not, expression),
                IsDistinct = true,
                OrderingType = SqlOrderingType.Blocked
            };
        }

        private SqlExpression VisitExpression(Expression exp)
        {
            SqlNode node = this.Visit(exp);
            if (node == null)
            {
                return null;
            }
            SqlExpression expression = node as SqlExpression;
            if (expression != null)
            {
                return expression;
            }
            SqlSelect select = node as SqlSelect;
            if (select == null)
            {
                throw System.Data.Linq.SqlClient.Error.UnrecognizedExpressionNode(node);
            }
            return this.sql.SubSelect(SqlNodeType.Multiset, select, exp.Type);
        }

        private SqlNode VisitFirst(Expression sequence, LambdaExpression lambda, bool isFirst)
        {
            SqlSelect select = this.LockSelect(this.VisitSequence(sequence));
            if (lambda != null)
            {
                this.map[lambda.Parameters[0]] = (SqlAliasRef) select.Selection;
                select.Where = this.VisitExpression(lambda.Body);
            }
            if (isFirst)
            {
                select.Top = this.sql.ValueFromObject(1, false, this.dominatingExpression);
            }
            if (this.outerNode)
            {
                return select;
            }
            SqlNodeType nt = this.typeProvider.From(select.Selection.ClrType).CanBeColumn ? SqlNodeType.ScalarSubSelect : SqlNodeType.Element;
            return this.sql.SubSelect(nt, select, sequence.Type);
        }

        private SqlNode VisitGroupBy(Expression sequence, LambdaExpression keyLambda, LambdaExpression elemLambda, LambdaExpression resultSelector)
        {
            SqlSelect sel = this.VisitSequence(sequence);
            sel = this.LockSelect(sel);
            SqlAlias alias = new SqlAlias(sel);
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            this.map[keyLambda.Parameters[0]] = ref2;
            SqlExpression expr = this.VisitExpression(keyLambda.Body);
            SqlDuplicator duplicator = new SqlDuplicator();
            SqlSelect node = (SqlSelect) duplicator.Duplicate(sel);
            SqlAlias alias2 = new SqlAlias(node);
            SqlAliasRef ref3 = new SqlAliasRef(alias2);
            this.map[keyLambda.Parameters[0]] = ref3;
            SqlExpression right = this.VisitExpression(keyLambda.Body);
            SqlExpression selection = null;
            SqlExpression expression4 = null;
            if (elemLambda != null)
            {
                this.map[elemLambda.Parameters[0]] = ref3;
                selection = this.VisitExpression(elemLambda.Body);
                this.map[elemLambda.Parameters[0]] = ref2;
                expression4 = this.VisitExpression(elemLambda.Body);
            }
            else
            {
                selection = ref3;
                expression4 = ref2;
            }
            SqlSharedExpression expression5 = new SqlSharedExpression(expr);
            expr = new SqlSharedExpressionRef(expression5);
            SqlSelect select = new SqlSelect(selection, alias2, this.dominatingExpression) {
                Where = this.sql.Binary(SqlNodeType.EQ2V, expr, right)
            };
            SqlSubSelect group = this.sql.SubSelect(SqlNodeType.Multiset, select);
            SqlSelect select5 = new SqlSelect(new SqlSharedExpressionRef(expression5), alias, this.dominatingExpression) {
                GroupBy = { expression5 }
            };
            SqlAlias from = new SqlAlias(select5);
            SqlSelect select6 = null;
            if (resultSelector != null)
            {
                Type type = typeof(IGrouping<,>).MakeGenericType(new Type[] { expr.ClrType, selection.ClrType });
                SqlExpression expression6 = new SqlGrouping(type, this.typeProvider.From(type), expr, group, this.dominatingExpression);
                SqlSelect select7 = new SqlSelect(expression6, from, this.dominatingExpression);
                SqlAlias alias4 = new SqlAlias(select7);
                SqlAliasRef ref4 = new SqlAliasRef(alias4);
                this.map[resultSelector.Parameters[0]] = this.sql.Member(ref4, type.GetProperty("Key"));
                this.map[resultSelector.Parameters[1]] = ref4;
                GroupInfo info = new GroupInfo {
                    SelectWithGroup = select5,
                    ElementOnGroupSource = expression4
                };
                this.gmap[ref4] = info;
                SqlExpression expression7 = this.VisitExpression(resultSelector.Body);
                select6 = new SqlSelect(expression7, alias4, this.dominatingExpression);
                GroupInfo info2 = new GroupInfo {
                    SelectWithGroup = select5,
                    ElementOnGroupSource = expression4
                };
                this.gmap[expression7] = info2;
                return select6;
            }
            Type clrType = typeof(IGrouping<,>).MakeGenericType(new Type[] { expr.ClrType, selection.ClrType });
            SqlExpression expression8 = new SqlGrouping(clrType, this.typeProvider.From(clrType), expr, group, this.dominatingExpression);
            select6 = new SqlSelect(expression8, from, this.dominatingExpression);
            GroupInfo info3 = new GroupInfo {
                SelectWithGroup = select5,
                ElementOnGroupSource = expression4
            };
            this.gmap[expression8] = info3;
            return select6;
        }

        private SqlSelect VisitGroupJoin(Expression outerSequence, Expression innerSequence, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector, LambdaExpression resultSelector)
        {
            SqlSelect node = this.VisitSequence(outerSequence);
            SqlSelect select2 = this.VisitSequence(innerSequence);
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            SqlAlias alias2 = new SqlAlias(select2);
            SqlAliasRef selection = new SqlAliasRef(alias2);
            this.map[outerKeySelector.Parameters[0]] = ref2;
            SqlExpression left = this.VisitExpression(outerKeySelector.Body);
            this.map[innerKeySelector.Parameters[0]] = selection;
            SqlExpression right = this.VisitExpression(innerKeySelector.Body);
            SqlExpression expression3 = this.sql.Binary(SqlNodeType.EQ, left, right);
            SqlSelect select = new SqlSelect(selection, alias2, this.dominatingExpression) {
                Where = expression3
            };
            SqlSubSelect select4 = this.sql.SubSelect(SqlNodeType.Multiset, select);
            this.map[resultSelector.Parameters[0]] = ref2;
            this.dupMap[resultSelector.Parameters[1]] = select4;
            return new SqlSelect(this.VisitExpression(resultSelector.Body), alias, this.dominatingExpression);
        }

        private SqlNode VisitInner(Expression node)
        {
            SqlNode node2;
            if (node == null)
            {
                return null;
            }
            Expression dominatingExpression = this.dominatingExpression;
            this.dominatingExpression = ChooseBestDominatingExpression(this.dominatingExpression, node);
            try
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Coalesce:
                    case ExpressionType.Divide:
                    case ExpressionType.Equal:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Modulo:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                    case ExpressionType.Power:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        return this.VisitBinary((BinaryExpression) node);

                    case ExpressionType.ArrayLength:
                        return this.VisitArrayLength((UnaryExpression) node);

                    case ExpressionType.ArrayIndex:
                        return this.VisitArrayIndex((BinaryExpression) node);

                    case ExpressionType.Call:
                        return this.VisitMethodCall((MethodCallExpression) node);

                    case ExpressionType.Conditional:
                        return this.VisitConditional((ConditionalExpression) node);

                    case ExpressionType.Constant:
                        return this.VisitConstant((ConstantExpression) node);

                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        return this.VisitCast((UnaryExpression) node);

                    case ExpressionType.Invoke:
                        return this.VisitInvocation((InvocationExpression) node);

                    case ExpressionType.Lambda:
                        return this.VisitLambda((LambdaExpression) node);

                    case ExpressionType.LeftShift:
                    case ExpressionType.RightShift:
                        throw System.Data.Linq.SqlClient.Error.UnsupportedNodeType(node.NodeType);

                    case ExpressionType.MemberAccess:
                        return this.VisitMemberAccess((MemberExpression) node);

                    case ExpressionType.MemberInit:
                        return this.VisitMemberInit((MemberInitExpression) node);

                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                        return this.VisitUnary((UnaryExpression) node);

                    case ExpressionType.UnaryPlus:
                        if (node.Type != typeof(TimeSpan))
                        {
                            throw System.Data.Linq.SqlClient.Error.UnrecognizedExpressionNode(node.NodeType);
                        }
                        return this.VisitUnary((UnaryExpression) node);

                    case ExpressionType.New:
                        return this.VisitNew((NewExpression) node);

                    case ExpressionType.NewArrayInit:
                        return this.VisitNewArrayInit((NewArrayExpression) node);

                    case ExpressionType.Parameter:
                        return this.VisitParameter((ParameterExpression) node);

                    case ExpressionType.Quote:
                        return this.Visit(((UnaryExpression) node).Operand);

                    case ExpressionType.TypeAs:
                        return this.VisitAs((UnaryExpression) node);

                    case ExpressionType.TypeIs:
                        return this.VisitTypeBinary((TypeBinaryExpression) node);

                    case ((ExpressionType) 0x7d0):
                        return ((KnownExpression) node).Node;

                    case ((ExpressionType) 0x7d1):
                        return this.VisitLinkedTable((LinkedTableExpression) node);
                }
                throw System.Data.Linq.SqlClient.Error.UnrecognizedExpressionNode(node.NodeType);
            }
            finally
            {
                this.dominatingExpression = dominatingExpression;
            }
            return node2;
        }

        private SqlStatement VisitInsert(Expression item, LambdaExpression resultSelector)
        {
            if (item == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("item");
            }
            this.dominatingExpression = item;
            MetaTable table = this.services.Model.GetTable(item.Type);
            Expression sourceExpression = this.services.Context.GetTable(table.RowType.Type).Expression;
            MetaType inheritanceType = null;
            SqlNew expr = null;
            ConstantExpression expression2 = item as ConstantExpression;
            if (expression2 == null)
            {
                throw System.Data.Linq.SqlClient.Error.InsertItemMustBeConstant();
            }
            if (expression2.Value == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("item");
            }
            List<SqlMemberAssign> bindings = new List<SqlMemberAssign>();
            inheritanceType = table.RowType.GetInheritanceType(expression2.Value.GetType());
            SqlExpression expression3 = this.sql.ValueFromObject(expression2.Value, true, sourceExpression);
            foreach (MetaDataMember member in inheritanceType.PersistentDataMembers)
            {
                if ((!member.IsAssociation && !member.IsDbGenerated) && !member.IsVersion)
                {
                    bindings.Add(new SqlMemberAssign(member.Member, this.sql.Member(expression3, member.Member)));
                }
            }
            ConstructorInfo constructor = inheritanceType.Type.GetConstructor(Type.EmptyTypes);
            expr = this.sql.New(inheritanceType, constructor, null, null, bindings, item);
            SqlTable table2 = this.sql.Table(table, table.RowType, this.dominatingExpression);
            SqlInsert insert = new SqlInsert(table2, expr, item);
            if (resultSelector == null)
            {
                return insert;
            }
            MetaDataMember dBGeneratedIdentityMember = inheritanceType.DBGeneratedIdentityMember;
            bool flag = false;
            if (dBGeneratedIdentityMember != null)
            {
                flag = this.IsDbGeneratedKeyProjectionOnly(resultSelector.Body, dBGeneratedIdentityMember);
                if ((dBGeneratedIdentityMember.Type == typeof(Guid)) && ((this.converterStrategy & System.Data.Linq.SqlClient.ConverterStrategy.CanOutputFromInsert) != System.Data.Linq.SqlClient.ConverterStrategy.Default))
                {
                    insert.OutputKey = new SqlColumn(dBGeneratedIdentityMember.Type, this.sql.Default(dBGeneratedIdentityMember), dBGeneratedIdentityMember.Name, dBGeneratedIdentityMember, null, this.dominatingExpression);
                    if (!flag)
                    {
                        insert.OutputToLocal = true;
                    }
                }
            }
            SqlSelect select = null;
            SqlSelect select2 = null;
            SqlAlias alias = new SqlAlias(table2);
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            this.map.Add(resultSelector.Parameters[0], ref2);
            SqlExpression selection = this.VisitExpression(resultSelector.Body);
            SqlExpression expression5 = null;
            if (dBGeneratedIdentityMember != null)
            {
                expression5 = this.sql.Binary(SqlNodeType.EQ, this.sql.Member(ref2, dBGeneratedIdentityMember.Member), this.GetIdentityExpression(dBGeneratedIdentityMember, insert.OutputKey != null));
            }
            else
            {
                SqlExpression right = this.VisitExpression(item);
                expression5 = this.sql.Binary(SqlNodeType.EQ2V, ref2, right);
            }
            select = new SqlSelect(selection, alias, resultSelector) {
                Where = expression5
            };
            if ((dBGeneratedIdentityMember != null) && flag)
            {
                if (insert.OutputKey == null)
                {
                    SqlExpression identityExpression = this.GetIdentityExpression(dBGeneratedIdentityMember, false);
                    if (identityExpression.ClrType != dBGeneratedIdentityMember.Type)
                    {
                        ProviderType sqlType = this.sql.Default(dBGeneratedIdentityMember);
                        identityExpression = this.sql.ConvertTo(dBGeneratedIdentityMember.Type, sqlType, identityExpression);
                    }
                    ParameterExpression expression = Expression.Parameter(dBGeneratedIdentityMember.Type, "p");
                    Expression[] initializers = new Expression[] { Expression.Convert(expression, typeof(object)) };
                    LambdaExpression expression10 = Expression.Lambda(Expression.NewArrayInit(typeof(object), initializers), new ParameterExpression[] { expression });
                    this.map.Add(expression, identityExpression);
                    select2 = new SqlSelect(this.VisitExpression(expression10.Body), null, expression10);
                }
                select.DoNotOutput = true;
            }
            SqlBlock block = new SqlBlock(this.dominatingExpression) {
                Statements = { insert }
            };
            if (select2 != null)
            {
                block.Statements.Add(select2);
            }
            block.Statements.Add(select);
            return block;
        }

        private SqlNode VisitIntersect(Expression source1, Expression source2)
        {
            Type elementType = TypeSystem.GetElementType(source1.Type);
            if (this.IsGrouping(elementType))
            {
                throw System.Data.Linq.SqlClient.Error.IntersectNotSupportedForHierarchicalTypes();
            }
            SqlSelect node = this.LockSelect(this.VisitSequence(source1));
            SqlSelect select2 = this.VisitSequence(source2);
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef left = new SqlAliasRef(alias);
            SqlAlias alias2 = new SqlAlias(select2);
            SqlAliasRef right = new SqlAliasRef(alias2);
            SqlExpression expression = this.GenerateQuantifier(alias2, this.sql.Binary(SqlNodeType.EQ2V, left, right), true);
            return new SqlSelect(left, alias, node.SourceExpression) { 
                Where = expression,
                IsDistinct = true,
                OrderingType = SqlOrderingType.Blocked
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private SqlNode VisitInvocation(InvocationExpression invoke)
        {
            LambdaExpression expression = (invoke.Expression.NodeType == ExpressionType.Quote) ? ((LambdaExpression) ((UnaryExpression) invoke.Expression).Operand) : (invoke.Expression as LambdaExpression);
            if (expression != null)
            {
                int num = 0;
                int count = invoke.Arguments.Count;
                while (num < count)
                {
                    this.exprMap[expression.Parameters[num]] = invoke.Arguments[num];
                    num++;
                }
                return this.VisitInner(expression.Body);
            }
            SqlExpression expression2 = this.VisitExpression(invoke.Expression);
            if (expression2.NodeType == SqlNodeType.Value)
            {
                SqlValue value2 = (SqlValue) expression2;
                Delegate delegate2 = value2.Value as Delegate;
                if (delegate2 != null)
                {
                    System.Data.Linq.CompiledQuery target = delegate2.Target as System.Data.Linq.CompiledQuery;
                    if (target != null)
                    {
                        return this.VisitInvocation(Expression.Invoke(target.Expression, invoke.Arguments));
                    }
                    if (invoke.Arguments.Count == 0)
                    {
                        object obj2;
                        try
                        {
                            obj2 = delegate2.DynamicInvoke(null);
                        }
                        catch (TargetInvocationException exception)
                        {
                            throw exception.InnerException;
                        }
                        return this.sql.ValueFromObject(obj2, invoke.Type, true, this.dominatingExpression);
                    }
                }
            }
            SqlExpression[] exprs = new SqlExpression[invoke.Arguments.Count];
            for (int i = 0; i < exprs.Length; i++)
            {
                exprs[i] = (SqlExpression) this.Visit(invoke.Arguments[i]);
            }
            SqlClientArray array = new SqlClientArray(typeof(object[]), this.typeProvider.From(typeof(object[])), exprs, this.dominatingExpression);
            return this.sql.MethodCall(invoke.Type, typeof(Delegate).GetMethod("DynamicInvoke"), expression2, new SqlExpression[] { array }, this.dominatingExpression);
        }

        private SqlSelect VisitJoin(Expression outerSequence, Expression innerSequence, LambdaExpression outerKeySelector, LambdaExpression innerKeySelector, LambdaExpression resultSelector)
        {
            SqlSelect node = this.VisitSequence(outerSequence);
            SqlSelect select2 = this.VisitSequence(innerSequence);
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            SqlAlias alias2 = new SqlAlias(select2);
            SqlAliasRef ref3 = new SqlAliasRef(alias2);
            this.map[outerKeySelector.Parameters[0]] = ref2;
            SqlExpression left = this.VisitExpression(outerKeySelector.Body);
            this.map[innerKeySelector.Parameters[0]] = ref3;
            SqlExpression right = this.VisitExpression(innerKeySelector.Body);
            this.map[resultSelector.Parameters[0]] = ref2;
            this.map[resultSelector.Parameters[1]] = ref3;
            SqlExpression selection = this.VisitExpression(resultSelector.Body);
            SqlExpression cond = this.sql.Binary(SqlNodeType.EQ, left, right);
            if ((this.converterStrategy & System.Data.Linq.SqlClient.ConverterStrategy.CanUseJoinOn) != System.Data.Linq.SqlClient.ConverterStrategy.Default)
            {
                return new SqlSelect(selection, new SqlJoin(SqlJoinType.Inner, alias, alias2, cond, this.dominatingExpression), this.dominatingExpression);
            }
            return new SqlSelect(selection, new SqlJoin(SqlJoinType.Cross, alias, alias2, null, this.dominatingExpression), this.dominatingExpression) { Where = cond };
        }

        private SqlNode VisitLambda(LambdaExpression lambda)
        {
            int num = 0;
            int count = lambda.Parameters.Count;
            while (num < count)
            {
                ParameterExpression expression2;
                ParameterExpression expression = lambda.Parameters[num];
                if (expression.Type == typeof(Type))
                {
                    throw System.Data.Linq.SqlClient.Error.BadParameterType(expression.Type);
                }
                LambdaExpression accessor = Expression.Lambda(typeof(Func<,>).MakeGenericType(new Type[] { typeof(object[]), expression.Type }), Expression.Convert(Expression.ArrayIndex(expression2 = Expression.Parameter(typeof(object[]), "args"), Expression.Constant(num)), expression.Type), new ParameterExpression[] { expression2 });
                SqlClientParameter parameter = new SqlClientParameter(expression.Type, this.typeProvider.From(expression.Type), accessor, this.dominatingExpression);
                this.dupMap[expression] = parameter;
                num++;
            }
            return this.VisitInner(lambda.Body);
        }

        private SqlNode VisitLinkedTable(LinkedTableExpression linkedTable) => 
            this.TranslateConstantTable(linkedTable.Table, linkedTable.Link);

        private SqlNode VisitMappedFunctionCall(MethodCallExpression mc)
        {
            MetaFunction function = this.services.Model.GetFunction(mc.Method);
            this.CheckContext(this.VisitExpression(mc.Object));
            if (!function.IsComposable)
            {
                return this.TranslateStoredProcedureCall(mc, function);
            }
            if (function.ResultRowTypes.Count > 0)
            {
                return this.TranslateTableValuedFunction(mc, function);
            }
            ProviderType sqlType = ((function.ReturnParameter != null) && !string.IsNullOrEmpty(function.ReturnParameter.DbType)) ? this.typeProvider.Parse(function.ReturnParameter.DbType) : this.typeProvider.From(mc.Method.ReturnType);
            List<SqlExpression> functionParameters = this.GetFunctionParameters(mc, function);
            return this.sql.FunctionCall(mc.Method.ReturnType, sqlType, function.MappedName, functionParameters, mc);
        }

        private SqlNode VisitMemberAccess(MemberExpression ma)
        {
            Type memberType = TypeSystem.GetMemberType(ma.Member);
            if (memberType.IsGenericType && (memberType.GetGenericTypeDefinition() == typeof(Table<>)))
            {
                Type type = memberType.GetGenericArguments()[0];
                this.CheckContext(this.VisitExpression(ma.Expression));
                ITable table = this.services.Context.GetTable(type);
                if (table != null)
                {
                    return this.Visit(Expression.Constant(table));
                }
            }
            if ((ma.Member.Name == "Count") && TypeSystem.IsSequenceType(ma.Expression.Type))
            {
                return this.VisitAggregate(ma.Expression, null, SqlNodeType.Count, typeof(int));
            }
            return this.sql.Member(this.VisitExpression(ma.Expression), ma.Member);
        }

        private SqlExpression VisitMemberInit(MemberInitExpression init)
        {
            MetaType metaType = this.services.Model.GetMetaType(init.Type);
            if (metaType.IsEntity)
            {
                throw System.Data.Linq.SqlClient.Error.CannotMaterializeEntityType(init.Type);
            }
            SqlExpression[] args = null;
            NewExpression newExpression = init.NewExpression;
            if ((newExpression.Type == typeof(decimal)) && (newExpression.Arguments.Count == 1))
            {
                return (this.VisitCast(Expression.Convert(newExpression.Arguments[0], typeof(decimal))) as SqlExpression);
            }
            if (newExpression.Arguments.Count > 0)
            {
                args = new SqlExpression[newExpression.Arguments.Count];
                int index = 0;
                int length = args.Length;
                while (index < length)
                {
                    args[index] = this.VisitExpression(newExpression.Arguments[index]);
                    index++;
                }
            }
            int count = init.Bindings.Count;
            SqlMemberAssign[] items = new SqlMemberAssign[count];
            int[] keys = new int[items.Length];
            for (int i = 0; i < count; i++)
            {
                MemberAssignment assignment = init.Bindings[i] as MemberAssignment;
                if (assignment == null)
                {
                    throw System.Data.Linq.SqlClient.Error.UnhandledBindingType(init.Bindings[i].BindingType);
                }
                SqlExpression expr = this.VisitExpression(assignment.Expression);
                items[i] = new SqlMemberAssign(assignment.Member, expr);
                keys[i] = metaType.GetDataMember(assignment.Member).Ordinal;
            }
            Array.Sort<int, SqlMemberAssign>(keys, items, 0, items.Length);
            return this.sql.New(metaType, newExpression.Constructor, args, PropertyOrFieldOf(newExpression.Members), items, this.dominatingExpression);
        }

        private SqlNode VisitMethodCall(MethodCallExpression mc)
        {
            Type declaringType = mc.Method.DeclaringType;
            if (mc.Method.IsStatic)
            {
                if (this.IsSequenceOperatorCall(mc))
                {
                    return this.VisitSequenceOperatorCall(mc);
                }
                if (IsDataManipulationCall(mc))
                {
                    return this.VisitDataManipulationCall(mc);
                }
                if (((declaringType == typeof(DBConvert)) || (declaringType == typeof(Convert))) && (mc.Method.Name == "ChangeType"))
                {
                    SqlNode node = null;
                    if (mc.Arguments.Count == 2)
                    {
                        object obj2 = this.GetValue(mc.Arguments[1], "ChangeType");
                        if ((obj2 != null) && typeof(Type).IsAssignableFrom(obj2.GetType()))
                        {
                            node = this.VisitChangeType(mc.Arguments[0], (Type) obj2);
                        }
                    }
                    if (node == null)
                    {
                        throw System.Data.Linq.SqlClient.Error.MethodFormHasNoSupportConversionToSql(mc.Method.Name, mc.Method);
                    }
                    return node;
                }
            }
            else if (typeof(DataContext).IsAssignableFrom(mc.Method.DeclaringType))
            {
                string name = mc.Method.Name;
                if (name != null)
                {
                    if (name == "GetTable")
                    {
                        if (mc.Method.IsGenericMethod)
                        {
                            Type[] genericArguments = mc.Method.GetGenericArguments();
                            if ((genericArguments.Length == 1) && (mc.Method.GetParameters().Length == 0))
                            {
                                this.CheckContext(this.VisitExpression(mc.Object));
                                ITable table = this.services.Context.GetTable(genericArguments[0]);
                                if (table != null)
                                {
                                    return this.Visit(Expression.Constant(table));
                                }
                            }
                        }
                    }
                    else if ((name == "ExecuteCommand") || (name == "ExecuteQuery"))
                    {
                        return this.VisitUserQuery((string) this.GetValue(mc.Arguments[0], mc.Method.Name), GetArray(mc.Arguments[1]), mc.Type);
                    }
                }
                if (this.IsMappedFunctionCall(mc))
                {
                    return this.VisitMappedFunctionCall(mc);
                }
            }
            else if ((((mc.Method.DeclaringType != typeof(string)) && (mc.Method.Name == "Contains")) && (!mc.Method.IsStatic && typeof(IList).IsAssignableFrom(mc.Method.DeclaringType))) && (((mc.Type == typeof(bool)) && (mc.Arguments.Count == 1)) && TypeSystem.GetElementType(mc.Method.DeclaringType).IsAssignableFrom(mc.Arguments[0].Type)))
            {
                return this.VisitContains(mc.Object, mc.Arguments[0]);
            }
            SqlExpression expression = this.VisitExpression(mc.Object);
            SqlExpression[] args = new SqlExpression[mc.Arguments.Count];
            int index = 0;
            int length = args.Length;
            while (index < length)
            {
                args[index] = this.VisitExpression(mc.Arguments[index]);
                index++;
            }
            return this.sql.MethodCall(mc.Method, expression, args, this.dominatingExpression);
        }

        private SqlExpression VisitNew(NewExpression qn)
        {
            if ((TypeSystem.IsNullableType(qn.Type) && (qn.Arguments.Count == 1)) && (TypeSystem.GetNonNullableType(qn.Type) == qn.Arguments[0].Type))
            {
                return (this.VisitCast(Expression.Convert(qn.Arguments[0], qn.Type)) as SqlExpression);
            }
            if ((qn.Type == typeof(decimal)) && (qn.Arguments.Count == 1))
            {
                return (this.VisitCast(Expression.Convert(qn.Arguments[0], typeof(decimal))) as SqlExpression);
            }
            MetaType metaType = this.services.Model.GetMetaType(qn.Type);
            if (metaType.IsEntity)
            {
                throw System.Data.Linq.SqlClient.Error.CannotMaterializeEntityType(qn.Type);
            }
            SqlExpression[] args = null;
            if (qn.Arguments.Count > 0)
            {
                args = new SqlExpression[qn.Arguments.Count];
                int index = 0;
                int count = qn.Arguments.Count;
                while (index < count)
                {
                    args[index] = this.VisitExpression(qn.Arguments[index]);
                    index++;
                }
            }
            return this.sql.New(metaType, qn.Constructor, args, PropertyOrFieldOf(qn.Members), null, this.dominatingExpression);
        }

        private SqlExpression VisitNewArrayInit(NewArrayExpression arr)
        {
            SqlExpression[] exprs = new SqlExpression[arr.Expressions.Count];
            int index = 0;
            int length = exprs.Length;
            while (index < length)
            {
                exprs[index] = this.VisitExpression(arr.Expressions[index]);
                index++;
            }
            return new SqlClientArray(arr.Type, this.typeProvider.From(arr.Type), exprs, this.dominatingExpression);
        }

        private SqlSelect VisitOfType(Expression sequence, Type ofType)
        {
            SqlSelect sel = this.LockSelect(this.VisitSequence(sequence));
            SqlAliasRef selection = (SqlAliasRef) sel.Selection;
            sel.Selection = new SqlUnary(SqlNodeType.Treat, ofType, this.typeProvider.From(ofType), selection, this.dominatingExpression);
            sel = this.LockSelect(sel);
            selection = (SqlAliasRef) sel.Selection;
            sel.Where = this.sql.AndAccumulate(sel.Where, this.sql.Unary(SqlNodeType.IsNotNull, selection, this.dominatingExpression));
            return sel;
        }

        private SqlSelect VisitOrderBy(Expression sequence, LambdaExpression expression, SqlOrderType orderType)
        {
            if (this.IsGrouping(expression.Body.Type))
            {
                throw System.Data.Linq.SqlClient.Error.GroupingNotSupportedAsOrderCriterion();
            }
            if (!this.typeProvider.From(expression.Body.Type).IsOrderable)
            {
                throw System.Data.Linq.SqlClient.Error.TypeCannotBeOrdered(expression.Body.Type);
            }
            SqlSelect node = this.LockSelect(this.VisitSequence(sequence));
            if ((node.Selection.NodeType != SqlNodeType.AliasRef) || (node.OrderBy.Count > 0))
            {
                SqlAlias alias = new SqlAlias(node);
                SqlAliasRef selection = new SqlAliasRef(alias);
                node = new SqlSelect(selection, alias, this.dominatingExpression);
            }
            this.map[expression.Parameters[0]] = (SqlAliasRef) node.Selection;
            SqlExpression expr = this.VisitExpression(expression.Body);
            node.OrderBy.Add(new SqlOrderExpression(orderType, expr));
            return node;
        }

        private SqlNode VisitParameter(ParameterExpression p)
        {
            SqlExpression expression;
            Expression expression2;
            SqlNode node;
            if (this.map.TryGetValue(p, out expression))
            {
                return expression;
            }
            if (this.exprMap.TryGetValue(p, out expression2))
            {
                return this.Visit(expression2);
            }
            if (!this.dupMap.TryGetValue(p, out node))
            {
                throw System.Data.Linq.SqlClient.Error.ParameterNotInScope(p.Name);
            }
            SqlDuplicator duplicator = new SqlDuplicator(true);
            return duplicator.Duplicate(node);
        }

        private SqlNode VisitQuantifier(SqlSelect select, LambdaExpression lambda, bool isAny)
        {
            SqlAlias alias = new SqlAlias(select);
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            if (lambda != null)
            {
                this.map[lambda.Parameters[0]] = ref2;
            }
            SqlExpression cond = (lambda != null) ? this.VisitExpression(lambda.Body) : null;
            return this.GenerateQuantifier(alias, cond, isAny);
        }

        private SqlNode VisitQuantifier(Expression sequence, LambdaExpression lambda, bool isAny) => 
            this.VisitQuantifier(this.VisitSequence(sequence), lambda, isAny);

        private SqlSelect VisitSelect(Expression sequence, LambdaExpression selector)
        {
            SqlAlias alias = new SqlAlias(this.VisitSequence(sequence));
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            this.map[selector.Parameters[0]] = ref2;
            SqlNode node = this.Visit(selector.Body);
            SqlSelect select = node as SqlSelect;
            if (select != null)
            {
                return new SqlSelect(this.sql.SubSelect(SqlNodeType.Multiset, select, selector.Body.Type), alias, this.dominatingExpression);
            }
            if (((node.NodeType == SqlNodeType.Element) || (node.NodeType == SqlNodeType.ScalarSubSelect)) && ((this.converterStrategy & System.Data.Linq.SqlClient.ConverterStrategy.CanUseOuterApply) != System.Data.Linq.SqlClient.ConverterStrategy.Default))
            {
                SqlSubSelect select3 = (SqlSubSelect) node;
                SqlSelect select4 = select3.Select;
                SqlAlias alias2 = new SqlAlias(select4);
                SqlAliasRef ref3 = new SqlAliasRef(alias2);
                if (node.NodeType == SqlNodeType.Element)
                {
                    select4.Selection = new SqlOptionalValue(new SqlColumn("test", this.sql.Unary(SqlNodeType.OuterJoinedValue, this.sql.Value(typeof(int?), this.typeProvider.From(typeof(int)), 1, false, this.dominatingExpression))), this.sql.Unary(SqlNodeType.OuterJoinedValue, select4.Selection));
                }
                else
                {
                    select4.Selection = this.sql.Unary(SqlNodeType.OuterJoinedValue, select4.Selection);
                }
                return new SqlSelect(ref3, new SqlJoin(SqlJoinType.OuterApply, alias, alias2, null, this.dominatingExpression), this.dominatingExpression);
            }
            SqlExpression selection = node as SqlExpression;
            if (selection == null)
            {
                throw System.Data.Linq.SqlClient.Error.BadProjectionInSelect();
            }
            return new SqlSelect(selection, alias, this.dominatingExpression);
        }

        private SqlSelect VisitSelectMany(Expression sequence, LambdaExpression colSelector, LambdaExpression resultSelector)
        {
            SqlAlias alias = new SqlAlias(this.VisitSequence(sequence));
            SqlAliasRef ref2 = new SqlAliasRef(alias);
            this.map[colSelector.Parameters[0]] = ref2;
            SqlAlias alias2 = new SqlAlias(this.VisitSequence(colSelector.Body));
            SqlAliasRef ref3 = new SqlAliasRef(alias2);
            SqlJoin from = new SqlJoin(SqlJoinType.CrossApply, alias, alias2, null, this.dominatingExpression);
            SqlExpression selection = ref3;
            if (resultSelector != null)
            {
                this.map[resultSelector.Parameters[0]] = ref2;
                this.map[resultSelector.Parameters[1]] = ref3;
                selection = this.VisitExpression(resultSelector.Body);
            }
            return new SqlSelect(selection, from, this.dominatingExpression);
        }

        private SqlSelect VisitSequence(Expression exp) => 
            this.CoerceToSequence(this.Visit(exp));

        private SqlNode VisitSequenceCast(Expression sequence, Type type)
        {
            Type elementType = TypeSystem.GetElementType(sequence.Type);
            ParameterExpression expression = Expression.Parameter(elementType, "pc");
            return this.Visit(Expression.Call(typeof(Enumerable), "Select", new Type[] { elementType, type }, new Expression[] { sequence, Expression.Lambda(Expression.Convert(expression, type), new ParameterExpression[] { expression }) }));
        }

        private SqlNode VisitSequenceOperatorCall(MethodCallExpression mc)
        {
            Type declaringType = mc.Method.DeclaringType;
            bool flag = false;
            if (!this.IsSequenceOperatorCall(mc))
            {
                throw System.Data.Linq.SqlClient.Error.InvalidSequenceOperatorCall(declaringType);
            }
            switch (mc.Method.Name)
            {
                case "Select":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitSelect(mc.Arguments[0], this.GetLambda(mc.Arguments[1]));

                case "SelectMany":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        if ((((mc.Arguments.Count != 3) || !this.IsLambda(mc.Arguments[1])) || ((this.GetLambda(mc.Arguments[1]).Parameters.Count != 1) || !this.IsLambda(mc.Arguments[2]))) || (this.GetLambda(mc.Arguments[2]).Parameters.Count != 2))
                        {
                            break;
                        }
                        return this.VisitSelectMany(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), this.GetLambda(mc.Arguments[2]));
                    }
                    return this.VisitSelectMany(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), null);

                case "Join":
                    flag = true;
                    if ((((mc.Arguments.Count != 5) || !this.IsLambda(mc.Arguments[2])) || ((this.GetLambda(mc.Arguments[2]).Parameters.Count != 1) || !this.IsLambda(mc.Arguments[3]))) || (((this.GetLambda(mc.Arguments[3]).Parameters.Count != 1) || !this.IsLambda(mc.Arguments[4])) || (this.GetLambda(mc.Arguments[4]).Parameters.Count != 2)))
                    {
                        break;
                    }
                    return this.VisitJoin(mc.Arguments[0], mc.Arguments[1], this.GetLambda(mc.Arguments[2]), this.GetLambda(mc.Arguments[3]), this.GetLambda(mc.Arguments[4]));

                case "GroupJoin":
                    flag = true;
                    if ((((mc.Arguments.Count != 5) || !this.IsLambda(mc.Arguments[2])) || ((this.GetLambda(mc.Arguments[2]).Parameters.Count != 1) || !this.IsLambda(mc.Arguments[3]))) || (((this.GetLambda(mc.Arguments[3]).Parameters.Count != 1) || !this.IsLambda(mc.Arguments[4])) || (this.GetLambda(mc.Arguments[4]).Parameters.Count != 2)))
                    {
                        break;
                    }
                    return this.VisitGroupJoin(mc.Arguments[0], mc.Arguments[1], this.GetLambda(mc.Arguments[2]), this.GetLambda(mc.Arguments[3]), this.GetLambda(mc.Arguments[4]));

                case "DefaultIfEmpty":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        break;
                    }
                    return this.VisitDefaultIfEmpty(mc.Arguments[0]);

                case "OfType":
                {
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        break;
                    }
                    Type ofType = mc.Method.GetGenericArguments()[0];
                    return this.VisitOfType(mc.Arguments[0], ofType);
                }
                case "Cast":
                {
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        break;
                    }
                    Type type = mc.Method.GetGenericArguments()[0];
                    return this.VisitSequenceCast(mc.Arguments[0], type);
                }
                case "Where":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitWhere(mc.Arguments[0], this.GetLambda(mc.Arguments[1]));

                case "First":
                case "FirstOrDefault":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitFirst(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), true);
                    }
                    return this.VisitFirst(mc.Arguments[0], null, true);

                case "Single":
                case "SingleOrDefault":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitFirst(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), false);
                    }
                    return this.VisitFirst(mc.Arguments[0], null, false);

                case "Distinct":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        break;
                    }
                    return this.VisitDistinct(mc.Arguments[0]);

                case "Concat":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitConcat(mc.Arguments[0], mc.Arguments[1]);

                case "Union":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitUnion(mc.Arguments[0], mc.Arguments[1]);

                case "Intersect":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitIntersect(mc.Arguments[0], mc.Arguments[1]);

                case "Except":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitExcept(mc.Arguments[0], mc.Arguments[1]);

                case "Any":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitQuantifier(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), true);
                    }
                    return this.VisitQuantifier(mc.Arguments[0], null, true);

                case "All":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitQuantifier(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), false);

                case "Count":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.Count, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.Count, mc.Type);

                case "LongCount":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.LongCount, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.LongCount, mc.Type);

                case "Sum":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.Sum, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.Sum, mc.Type);

                case "Min":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.Min, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.Min, mc.Type);

                case "Max":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.Max, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.Max, mc.Type);

                case "Average":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                        {
                            break;
                        }
                        return this.VisitAggregate(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlNodeType.Avg, mc.Type);
                    }
                    return this.VisitAggregate(mc.Arguments[0], null, SqlNodeType.Avg, mc.Type);

                case "GroupBy":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        if ((((mc.Arguments.Count == 3) && this.IsLambda(mc.Arguments[1])) && ((this.GetLambda(mc.Arguments[1]).Parameters.Count == 1) && this.IsLambda(mc.Arguments[2]))) && (this.GetLambda(mc.Arguments[2]).Parameters.Count == 1))
                        {
                            return this.VisitGroupBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), this.GetLambda(mc.Arguments[2]), null);
                        }
                        if ((((mc.Arguments.Count == 3) && this.IsLambda(mc.Arguments[1])) && ((this.GetLambda(mc.Arguments[1]).Parameters.Count == 1) && this.IsLambda(mc.Arguments[2]))) && (this.GetLambda(mc.Arguments[2]).Parameters.Count == 2))
                        {
                            return this.VisitGroupBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), null, this.GetLambda(mc.Arguments[2]));
                        }
                        if ((((mc.Arguments.Count == 4) && this.IsLambda(mc.Arguments[1])) && ((this.GetLambda(mc.Arguments[1]).Parameters.Count == 1) && this.IsLambda(mc.Arguments[2]))) && (((this.GetLambda(mc.Arguments[2]).Parameters.Count == 1) && this.IsLambda(mc.Arguments[3])) && (this.GetLambda(mc.Arguments[3]).Parameters.Count == 2)))
                        {
                            return this.VisitGroupBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), this.GetLambda(mc.Arguments[2]), this.GetLambda(mc.Arguments[3]));
                        }
                        break;
                    }
                    return this.VisitGroupBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), null, null);

                case "OrderBy":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitOrderBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlOrderType.Ascending);

                case "OrderByDescending":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitOrderBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlOrderType.Descending);

                case "ThenBy":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitThenBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlOrderType.Ascending);

                case "ThenByDescending":
                    flag = true;
                    if (((mc.Arguments.Count != 2) || !this.IsLambda(mc.Arguments[1])) || (this.GetLambda(mc.Arguments[1]).Parameters.Count != 1))
                    {
                        break;
                    }
                    return this.VisitThenBy(mc.Arguments[0], this.GetLambda(mc.Arguments[1]), SqlOrderType.Descending);

                case "Take":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitTake(mc.Arguments[0], mc.Arguments[1]);

                case "Skip":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitSkip(mc.Arguments[0], mc.Arguments[1]);

                case "Contains":
                    flag = true;
                    if (mc.Arguments.Count != 2)
                    {
                        break;
                    }
                    return this.VisitContains(mc.Arguments[0], mc.Arguments[1]);

                case "ToList":
                case "AsEnumerable":
                case "ToArray":
                    flag = true;
                    if (mc.Arguments.Count != 1)
                    {
                        break;
                    }
                    return this.Visit(mc.Arguments[0]);
            }
            if (flag)
            {
                throw System.Data.Linq.SqlClient.Error.QueryOperatorOverloadNotSupported(mc.Method.Name);
            }
            throw System.Data.Linq.SqlClient.Error.QueryOperatorNotSupported(mc.Method.Name);
        }

        private SqlSelect VisitSkip(Expression sequence, Expression skipCount)
        {
            SqlExpression skipExp = this.VisitExpression(skipCount);
            if (skipExp.NodeType == SqlNodeType.Value)
            {
                SqlValue value2 = (SqlValue) skipExp;
                if (typeof(int).IsAssignableFrom(value2.Value.GetType()) && (((int) value2.Value) < 0))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("skipCount");
                }
            }
            SqlSelect select = this.VisitSequence(sequence);
            return this.GenerateSkipTake(select, skipExp, null);
        }

        private SqlSelect VisitTake(Expression sequence, Expression count)
        {
            SqlExpression takeExp = this.VisitExpression(count);
            if (takeExp.NodeType == SqlNodeType.Value)
            {
                SqlValue value2 = (SqlValue) takeExp;
                if (typeof(int).IsAssignableFrom(value2.Value.GetType()) && (((int) value2.Value) < 0))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("takeCount");
                }
            }
            MethodCallExpression mc = sequence as MethodCallExpression;
            if (((mc != null) && this.IsSequenceOperatorCall(mc)) && ((mc.Method.Name == "Skip") && (mc.Arguments.Count == 2)))
            {
                SqlExpression skipExp = this.VisitExpression(mc.Arguments[1]);
                if (skipExp.NodeType == SqlNodeType.Value)
                {
                    SqlValue value3 = (SqlValue) skipExp;
                    if (typeof(int).IsAssignableFrom(value3.Value.GetType()) && (((int) value3.Value) < 0))
                    {
                        throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("skipCount");
                    }
                }
                SqlSelect select = this.VisitSequence(mc.Arguments[0]);
                return this.GenerateSkipTake(select, skipExp, takeExp);
            }
            SqlSelect select2 = this.VisitSequence(sequence);
            return this.GenerateSkipTake(select2, null, takeExp);
        }

        private SqlSelect VisitThenBy(Expression sequence, LambdaExpression expression, SqlOrderType orderType)
        {
            if (this.IsGrouping(expression.Body.Type))
            {
                throw System.Data.Linq.SqlClient.Error.GroupingNotSupportedAsOrderCriterion();
            }
            if (!this.typeProvider.From(expression.Body.Type).IsOrderable)
            {
                throw System.Data.Linq.SqlClient.Error.TypeCannotBeOrdered(expression.Body.Type);
            }
            SqlSelect select = this.VisitSequence(sequence);
            this.map[expression.Parameters[0]] = (SqlAliasRef) select.Selection;
            SqlExpression expr = this.VisitExpression(expression.Body);
            select.OrderBy.Add(new SqlOrderExpression(orderType, expr));
            return select;
        }

        private SqlNode VisitTypeBinary(TypeBinaryExpression b)
        {
            SqlExpression expr = this.VisitExpression(b.Expression);
            if (b.NodeType != ExpressionType.TypeIs)
            {
                throw System.Data.Linq.SqlClient.Error.TypeBinaryOperatorNotRecognized();
            }
            Type typeOperand = b.TypeOperand;
            return this.sql.Unary(SqlNodeType.IsNotNull, new SqlUnary(SqlNodeType.Treat, typeOperand, this.typeProvider.From(typeOperand), expr, this.dominatingExpression), this.dominatingExpression);
        }

        private SqlNode VisitUnary(UnaryExpression u)
        {
            SqlExpression expression = this.VisitExpression(u.Operand);
            if (u.Method != null)
            {
                return this.sql.MethodCall(u.Type, u.Method, null, new SqlExpression[] { expression }, this.dominatingExpression);
            }
            SqlExpression expression2 = null;
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return this.sql.Unary(SqlNodeType.Negate, expression, this.dominatingExpression);

                case ExpressionType.UnaryPlus:
                    return expression2;

                case ExpressionType.Not:
                    if ((u.Operand.Type != typeof(bool)) && (u.Operand.Type != typeof(bool?)))
                    {
                        return this.sql.Unary(SqlNodeType.BitNot, expression, this.dominatingExpression);
                    }
                    return this.sql.Unary(SqlNodeType.Not, expression, this.dominatingExpression);

                case ExpressionType.TypeAs:
                    return this.sql.Unary(SqlNodeType.Treat, expression, this.dominatingExpression);
            }
            return expression2;
        }

        private SqlNode VisitUnion(Expression source1, Expression source2)
        {
            SqlSelect left = this.VisitSequence(source1);
            SqlSelect right = this.VisitSequence(source2);
            SqlUnion node = new SqlUnion(left, right, false);
            SqlAlias from = new SqlAlias(node);
            return new SqlSelect(new SqlAliasRef(from), from, this.dominatingExpression) { OrderingType = SqlOrderingType.Blocked };
        }

        private SqlStatement VisitUpdate(Expression item, LambdaExpression check, LambdaExpression resultSelector)
        {
            SqlStatement statement;
            if (item == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("item");
            }
            MetaTable table = this.services.Model.GetTable(item.Type);
            Expression sourceExpression = this.services.Context.GetTable(table.RowType.Type).Expression;
            Type type = table.RowType.Type;
            bool allowDeferred = this.allowDeferred;
            this.allowDeferred = false;
            try
            {
                Expression exp = sourceExpression;
                ParameterExpression left = Expression.Parameter(type, "p");
                LambdaExpression expression4 = Expression.Lambda(Expression.Equal(left, item), new ParameterExpression[] { left });
                LambdaExpression expression = expression4;
                if (check != null)
                {
                    expression = Expression.Lambda(Expression.And(Expression.Invoke(expression, new Expression[] { left }), Expression.Invoke(check, new Expression[] { left })), new ParameterExpression[] { left });
                }
                exp = Expression.Call(typeof(Enumerable), "Where", new Type[] { type }, new Expression[] { exp, expression });
                SqlSelect select = new RetypeCheckClause().VisitSelect(this.VisitSequence(exp));
                List<SqlAssign> assignments = new List<SqlAssign>();
                ConstantExpression expression6 = item as ConstantExpression;
                if (expression6 == null)
                {
                    throw System.Data.Linq.SqlClient.Error.UpdateItemMustBeConstant();
                }
                if (expression6.Value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("item");
                }
                Type type2 = expression6.Value.GetType();
                MetaType metaType = this.services.Model.GetMetaType(type2);
                foreach (ModifiedMemberInfo info in this.services.Context.GetTable(metaType.InheritanceRoot.Type).GetModifiedMembers(expression6.Value))
                {
                    MetaDataMember dataMember = metaType.GetDataMember(info.Member);
                    assignments.Add(new SqlAssign(this.sql.Member(select.Selection, info.Member), new SqlValue(dataMember.Type, this.typeProvider.From(dataMember.Type), info.CurrentValue, true, sourceExpression), sourceExpression));
                }
                SqlUpdate update = new SqlUpdate(select, assignments, sourceExpression);
                if (resultSelector == null)
                {
                    return update;
                }
                SqlSelect select2 = null;
                exp = sourceExpression;
                exp = Expression.Call(typeof(Enumerable), "Where", new Type[] { type }, new Expression[] { exp, expression4 });
                exp = Expression.Call(typeof(Enumerable), "Select", new Type[] { type, resultSelector.Body.Type }, new Expression[] { exp, resultSelector });
                select2 = this.VisitSequence(exp);
                select2.Where = this.sql.AndAccumulate(this.sql.Binary(SqlNodeType.GT, this.GetRowCountExpression(), this.sql.ValueFromObject(0, false, this.dominatingExpression)), select2.Where);
                SqlBlock block = new SqlBlock(sourceExpression) {
                    Statements = { 
                        update,
                        select2
                    }
                };
                statement = block;
            }
            finally
            {
                this.allowDeferred = allowDeferred;
            }
            return statement;
        }

        private SqlUserQuery VisitUserQuery(string query, Expression[] arguments, Type resultType)
        {
            SqlExpression[] args = new SqlExpression[arguments.Length];
            int index = 0;
            int length = args.Length;
            while (index < length)
            {
                args[index] = this.VisitExpression(arguments[index]);
                index++;
            }
            SqlUserQuery query2 = new SqlUserQuery(query, null, args, this.dominatingExpression);
            if (resultType != typeof(void))
            {
                Type elementType = TypeSystem.GetElementType(resultType);
                MetaType metaType = this.services.Model.GetMetaType(elementType);
                if (TypeSystem.IsSimpleType(elementType))
                {
                    SqlUserColumn column = new SqlUserColumn(elementType, this.typeProvider.From(elementType), query2, "", false, this.dominatingExpression);
                    query2.Columns.Add(column);
                    query2.Projection = column;
                    return query2;
                }
                SqlUserRow item = new SqlUserRow(metaType.InheritanceRoot, this.typeProvider.GetApplicationType(0), query2, this.dominatingExpression);
                query2.Projection = this.translator.BuildProjection(item, metaType, this.allowDeferred, null, this.dominatingExpression);
            }
            return query2;
        }

        private SqlSelect VisitWhere(Expression sequence, LambdaExpression predicate)
        {
            SqlSelect select = this.LockSelect(this.VisitSequence(sequence));
            this.map[predicate.Parameters[0]] = (SqlAliasRef) select.Selection;
            select.Where = this.VisitExpression(predicate.Body);
            return select;
        }

        internal System.Data.Linq.SqlClient.ConverterStrategy ConverterStrategy
        {
            get => 
                this.converterStrategy;
            set
            {
                this.converterStrategy = value;
            }
        }

        private enum ConversionMethod
        {
            Treat,
            Ignore,
            Convert,
            Lift
        }

        private class GroupInfo
        {
            internal SqlExpression ElementOnGroupSource;
            internal SqlSelect SelectWithGroup;
        }

        private class RetypeCheckClause : SqlVisitor
        {
            internal override SqlExpression VisitMethodCall(SqlMethodCall mc)
            {
                if ((mc.Arguments.Count == 2) && (mc.Method.Name == "op_Equality"))
                {
                    SqlExpression expression = mc.Arguments[1];
                    if (expression.NodeType == SqlNodeType.Value)
                    {
                        ((SqlValue) expression).SetSqlType(mc.Arguments[0].SqlType);
                    }
                }
                return base.VisitMethodCall(mc);
            }
        }
    }
}

