namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public class SqlProvider : IReaderProvider, IProvider, IDisposable, IConnectionUser
    {
        private bool checkQueries;
        private int commandTimeout;
        private SqlConnectionManager conManager;
        private string dbName;
        private bool deleted;
        private bool disposed;
        private bool enableCacheLookup;
        private TextWriter log;
        private ProviderMode mode;
        private System.Data.Linq.SqlClient.OptimizationFlags optimizationFlags;
        private int queryCount;
        private IObjectReaderCompiler readerCompiler;
        private IDataServices services;
        private const string SqlCeConnectionTypeName = "System.Data.SqlServerCe.SqlCeConnection";
        private const string SqlCeDataReaderTypeName = "System.Data.SqlServerCe.SqlCeDataReader";
        private const string SqlCeProviderInvariantName = "System.Data.SqlServerCe.3.5";
        private const string SqlCeTransactionTypeName = "System.Data.SqlServerCe.SqlCeTransaction";
        private SqlFactory sqlFactory;
        private Translator translator;
        private TypeSystemProvider typeProvider;

        public SqlProvider()
        {
            this.dbName = string.Empty;
            this.optimizationFlags = System.Data.Linq.SqlClient.OptimizationFlags.All;
            this.enableCacheLookup = true;
            this.mode = ProviderMode.NotYetDecided;
        }

        internal SqlProvider(ProviderMode mode)
        {
            this.dbName = string.Empty;
            this.optimizationFlags = System.Data.Linq.SqlClient.OptimizationFlags.All;
            this.enableCacheLookup = true;
            this.mode = mode;
        }

        private void AssignParameters(DbCommand cmd, System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parms, object[] userArguments, object lastResult)
        {
            if (parms != null)
            {
                foreach (SqlParameterInfo info in parms)
                {
                    DbParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = info.Parameter.Name;
                    parameter.Direction = info.Parameter.Direction;
                    if ((info.Parameter.Direction != ParameterDirection.Input) && (info.Parameter.Direction != ParameterDirection.InputOutput))
                    {
                        goto Label_00C4;
                    }
                    object obj2 = info.Value;
                    switch (info.Type)
                    {
                        case SqlParameterType.UserArgument:
                            try
                            {
                                obj2 = info.Accessor.DynamicInvoke(new object[] { userArguments });
                                goto Label_00AA;
                            }
                            catch (TargetInvocationException exception)
                            {
                                throw exception.InnerException;
                            }
                            break;

                        case SqlParameterType.PreviousResult:
                            break;

                        default:
                            goto Label_00AA;
                    }
                    obj2 = lastResult;
                Label_00AA:
                    this.typeProvider.InitializeParameter(info.Parameter.SqlType, parameter, obj2);
                    goto Label_00DC;
                Label_00C4:
                    this.typeProvider.InitializeParameter(info.Parameter.SqlType, parameter, null);
                Label_00DC:
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        internal QueryInfo[] BuildQuery(Expression query, SqlNodeAnnotations annotations)
        {
            this.CheckDispose();
            query = Funcletizer.Funcletize(query);
            QueryConverter converter = new QueryConverter(this.services, this.typeProvider, this.translator, this.sqlFactory);
            switch (this.Mode)
            {
                case ProviderMode.Sql2000:
                    converter.ConverterStrategy = ConverterStrategy.CanUseJoinOn | ConverterStrategy.CanUseRowStatus | ConverterStrategy.CanUseScopeIdentity;
                    break;

                case ProviderMode.Sql2005:
                case ProviderMode.Sql2008:
                    converter.ConverterStrategy = ConverterStrategy.CanOutputFromInsert | ConverterStrategy.CanUseJoinOn | ConverterStrategy.CanUseRowStatus | ConverterStrategy.CanUseOuterApply | ConverterStrategy.CanUseScopeIdentity | ConverterStrategy.SkipWithRowNumber;
                    break;

                case ProviderMode.SqlCE:
                    converter.ConverterStrategy = ConverterStrategy.CanUseOuterApply;
                    break;
            }
            SqlNode node = converter.ConvertOuter(query);
            return this.BuildQuery(this.GetResultShape(query), this.GetResultType(query), node, null, annotations);
        }

        private QueryInfo[] BuildQuery(ResultShape resultShape, Type resultType, SqlNode node, System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Linq.SqlClient.SqlParameter> parentParameters, SqlNodeAnnotations annotations)
        {
            SqlSupersetValidator validator = new SqlSupersetValidator();
            if (this.checkQueries)
            {
                validator.AddValidator(new ColumnTypeValidator());
                validator.AddValidator(new LiteralValidator());
            }
            validator.Validate(node);
            SqlColumnizer columnizer = new SqlColumnizer(new Func<SqlExpression, bool>(this.CanBeColumn));
            node = new SqlBinder(this.translator, this.sqlFactory, this.services.Model, this.services.Context.LoadOptions, columnizer) { 
                OptimizeLinkExpansions = (this.optimizationFlags & System.Data.Linq.SqlClient.OptimizationFlags.OptimizeLinkExpansions) != System.Data.Linq.SqlClient.OptimizationFlags.None,
                SimplifyCaseStatements = (this.optimizationFlags & System.Data.Linq.SqlClient.OptimizationFlags.SimplifyCaseStatements) != System.Data.Linq.SqlClient.OptimizationFlags.None,
                PreBinder = n => PreBindDotNetConverter.Convert(n, this.sqlFactory, this.services.Model)
            }.Bind(node);
            if (this.checkQueries)
            {
                validator.AddValidator(new ExpectNoAliasRefs());
                validator.AddValidator(new ExpectNoSharedExpressions());
            }
            validator.Validate(node);
            node = PostBindDotNetConverter.Convert(node, this.sqlFactory, this.Mode);
            SqlRetyper retyper = new SqlRetyper(this.typeProvider, this.services.Model);
            node = retyper.Retype(node);
            validator.Validate(node);
            node = new SqlTypeConverter(this.sqlFactory).Visit(node);
            validator.Validate(node);
            node = new SqlMethodTransformer(this.sqlFactory).Visit(node);
            validator.Validate(node);
            SqlMultiplexer.Options options = (((this.Mode == ProviderMode.Sql2008) || (this.Mode == ProviderMode.Sql2005)) || (this.Mode == ProviderMode.SqlCE)) ? SqlMultiplexer.Options.EnableBigJoin : SqlMultiplexer.Options.None;
            node = new SqlMultiplexer(options, parentParameters, this.sqlFactory).Multiplex(node);
            validator.Validate(node);
            node = new SqlFlattener(this.sqlFactory, columnizer).Flatten(node);
            validator.Validate(node);
            if (this.mode == ProviderMode.SqlCE)
            {
                node = new SqlRewriteScalarSubqueries(this.sqlFactory).Rewrite(node);
            }
            node = SqlCaseSimplifier.Simplify(node, this.sqlFactory);
            node = new SqlReorderer(this.typeProvider, this.sqlFactory).Reorder(node);
            validator.Validate(node);
            node = SqlBooleanizer.Rationalize(node, this.typeProvider, this.services.Model);
            if (this.checkQueries)
            {
                validator.AddValidator(new ExpectRationalizedBooleans());
            }
            validator.Validate(node);
            if (this.checkQueries)
            {
                validator.AddValidator(new ExpectNoFloatingColumns());
            }
            node = retyper.Retype(node);
            validator.Validate(node);
            SqlAliaser aliaser = new SqlAliaser();
            node = aliaser.AssociateColumnsWithAliases(node);
            validator.Validate(node);
            node = SqlLiftWhereClauses.Lift(node, this.typeProvider, this.services.Model);
            node = SqlLiftIndependentRowExpressions.Lift(node);
            node = SqlOuterApplyReducer.Reduce(node, this.sqlFactory, annotations);
            node = SqlTopReducer.Reduce(node, annotations, this.sqlFactory);
            node = new SqlResolver().Resolve(node);
            validator.Validate(node);
            node = aliaser.AssociateColumnsWithAliases(node);
            validator.Validate(node);
            node = SqlUnionizer.Unionize(node);
            node = SqlRemoveConstantOrderBy.Remove(node);
            node = new SqlDeflator().Deflate(node);
            validator.Validate(node);
            node = SqlCrossApplyToCrossJoin.Reduce(node, annotations);
            node = new SqlNamer().AssignNames(node);
            validator.Validate(node);
            node = new LongTypeConverter(this.sqlFactory).AddConversions(node, annotations);
            validator.AddValidator(new ExpectNoMethodCalls());
            validator.AddValidator(new ValidateNoInvalidComparison());
            validator.Validate(node);
            SqlParameterizer parameterizer = new SqlParameterizer(this.typeProvider, annotations);
            SqlFormatter formatter = new SqlFormatter();
            if (((this.mode == ProviderMode.SqlCE) || (this.mode == ProviderMode.Sql2005)) || (this.mode == ProviderMode.Sql2008))
            {
                formatter.ParenthesizeTop = true;
            }
            SqlBlock block = node as SqlBlock;
            if ((block != null) && (this.mode == ProviderMode.SqlCE))
            {
                System.Collections.ObjectModel.ReadOnlyCollection<System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo>> onlys = parameterizer.ParameterizeBlock(block);
                string[] strArray = formatter.FormatBlock(block, false);
                QueryInfo[] infoArray = new QueryInfo[strArray.Length];
                int index = 0;
                int length = strArray.Length;
                while (index < length)
                {
                    infoArray[index] = new QueryInfo(block.Statements[index], strArray[index], onlys[index], (index < (length - 1)) ? ResultShape.Return : resultShape, (index < (length - 1)) ? typeof(int) : resultType);
                    index++;
                }
                return infoArray;
            }
            System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters = parameterizer.Parameterize(node);
            string commandText = formatter.Format(node);
            return new QueryInfo[] { new QueryInfo(node, commandText, parameters, resultShape, resultType) };
        }

        private bool CanBeColumn(SqlExpression expression)
        {
            if (!expression.SqlType.CanBeColumn)
            {
                return false;
            }
            switch (expression.NodeType)
            {
                case SqlNodeType.MethodCall:
                case SqlNodeType.Member:
                case SqlNodeType.New:
                    return PostBindDotNetConverter.CanConvert(expression);
            }
            return true;
        }

        internal void CheckDispose()
        {
            if (this.disposed)
            {
                throw System.Data.Linq.SqlClient.Error.ProviderCannotBeUsedAfterDispose();
            }
        }

        private void CheckInitialized()
        {
            if (this.services == null)
            {
                throw System.Data.Linq.SqlClient.Error.ContextNotInitialized();
            }
        }

        private void CheckNotDeleted()
        {
            if (this.deleted)
            {
                throw System.Data.Linq.SqlClient.Error.DatabaseDeleteThroughContext();
            }
        }

        private void CheckSqlCompatibility(QueryInfo[] queries, SqlNodeAnnotations annotations)
        {
            if ((this.Mode == ProviderMode.Sql2000) || (this.Mode == ProviderMode.SqlCE))
            {
                int index = 0;
                int length = queries.Length;
                while (index < length)
                {
                    SqlServerCompatibilityCheck.ThrowIfUnsupported(queries[index].Query, annotations, this.Mode);
                    index++;
                }
            }
        }

        private ICompiledSubQuery[] CompileSubQueries(SqlNode query) => 
            new SubQueryCompiler(this).Compile(query);

        private ICompiledSubQuery CompileSubQuery(SqlNode query, Type elementType, System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Linq.SqlClient.SqlParameter> parameters)
        {
            query = SqlDuplicator.Copy(query);
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            QueryInfo[] queries = this.BuildQuery(ResultShape.Sequence, System.Data.Linq.SqlClient.TypeSystem.GetSequenceType(elementType), query, parameters, annotations);
            QueryInfo queryInfo = queries[0];
            ICompiledSubQuery[] subQueries = this.CompileSubQueries(queryInfo.Query);
            IObjectReaderFactory readerFactory = this.GetReaderFactory(queryInfo.Query, elementType);
            this.CheckSqlCompatibility(queries, annotations);
            return new CompiledSubQuery(queryInfo, readerFactory, parameters, subQueries);
        }

        public void Dispose()
        {
            this.disposed = true;
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.services = null;
                if (this.conManager != null)
                {
                    this.conManager.DisposeConnection();
                }
                this.conManager = null;
                this.typeProvider = null;
                this.sqlFactory = null;
                this.translator = null;
                this.readerCompiler = null;
                this.log = null;
            }
        }

        private IExecuteResult Execute(Expression query, QueryInfo queryInfo, IObjectReaderFactory factory, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries, object lastResult)
        {
            IExecuteResult result3;
            this.InitializeProviderMode();
            DbConnection connection = this.conManager.UseConnection(this);
            try
            {
                DbCommand cmd = connection.CreateCommand();
                cmd.CommandText = queryInfo.CommandText;
                cmd.Transaction = this.conManager.Transaction;
                cmd.CommandTimeout = this.commandTimeout;
                this.AssignParameters(cmd, queryInfo.Parameters, userArgs, lastResult);
                this.LogCommand(this.log, cmd);
                this.queryCount++;
                switch (queryInfo.ResultShape)
                {
                    case ResultShape.Singleton:
                    {
                        DbDataReader reader = cmd.ExecuteReader();
                        IObjectReader reader2 = factory.Create(reader, true, this, parentArgs, userArgs, subQueries);
                        this.conManager.UseConnection(reader2.Session);
                        try
                        {
                            IEnumerable sequence = (IEnumerable) Activator.CreateInstance(typeof(OneTimeEnumerable).MakeGenericType(new Type[] { queryInfo.ResultType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { reader2 }, null);
                            object obj2 = null;
                            MethodCallExpression expression = query as MethodCallExpression;
                            MethodInfo info = null;
                            if ((expression != null) && ((expression.Method.DeclaringType == typeof(Queryable)) || (expression.Method.DeclaringType == typeof(Enumerable))))
                            {
                                string name = expression.Method.Name;
                                if (name != null)
                                {
                                    if (((name == "First") || (name == "FirstOrDefault")) || (name == "SingleOrDefault"))
                                    {
                                        info = System.Data.Linq.SqlClient.TypeSystem.FindSequenceMethod(expression.Method.Name, sequence);
                                        goto Label_01DE;
                                    }
                                    else if (name == "Single")
                                    {
                                    }
                                }
                                info = System.Data.Linq.SqlClient.TypeSystem.FindSequenceMethod("Single", sequence);
                            }
                            else
                            {
                                info = System.Data.Linq.SqlClient.TypeSystem.FindSequenceMethod("SingleOrDefault", sequence);
                            }
                        Label_01DE:
                            if (info != null)
                            {
                                try
                                {
                                    obj2 = info.Invoke(null, new object[] { sequence });
                                }
                                catch (TargetInvocationException exception)
                                {
                                    if (exception.InnerException != null)
                                    {
                                        throw exception.InnerException;
                                    }
                                    throw;
                                }
                            }
                            return new ExecuteResult(cmd, queryInfo.Parameters, reader2.Session, obj2);
                        }
                        finally
                        {
                            reader2.Dispose();
                        }
                        break;
                    }
                    case ResultShape.Sequence:
                        break;

                    case ResultShape.MultipleResults:
                    {
                        DbDataReader reader5 = cmd.ExecuteReader();
                        IObjectReaderSession user = this.readerCompiler.CreateSession(reader5, this, parentArgs, userArgs, subQueries);
                        this.conManager.UseConnection(user);
                        MetaFunction function2 = this.GetFunction(query);
                        ExecuteResult executeResult = new ExecuteResult(cmd, queryInfo.Parameters, user);
                        executeResult.ReturnValue = new MultipleResults(this, function2, user, executeResult);
                        return executeResult;
                    }
                    default:
                        return new ExecuteResult(cmd, queryInfo.Parameters, null, cmd.ExecuteNonQuery(), true);
                }
                DbDataReader reader3 = cmd.ExecuteReader();
                IObjectReader reader4 = factory.Create(reader3, true, this, parentArgs, userArgs, subQueries);
                this.conManager.UseConnection(reader4.Session);
                IEnumerable source = (IEnumerable) Activator.CreateInstance(typeof(OneTimeEnumerable).MakeGenericType(new Type[] { System.Data.Linq.SqlClient.TypeSystem.GetElementType(queryInfo.ResultType) }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { reader4 }, null);
                if (typeof(IQueryable).IsAssignableFrom(queryInfo.ResultType))
                {
                    source = source.AsQueryable();
                }
                ExecuteResult result = new ExecuteResult(cmd, queryInfo.Parameters, reader4.Session);
                MetaFunction function = this.GetFunction(query);
                if ((function != null) && !function.IsComposable)
                {
                    source = (IEnumerable) Activator.CreateInstance(typeof(SingleResult).MakeGenericType(new Type[] { System.Data.Linq.SqlClient.TypeSystem.GetElementType(queryInfo.ResultType) }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { source, result, this.services.Context }, null);
                }
                result.ReturnValue = source;
                result3 = result;
            }
            finally
            {
                this.conManager.ReleaseConnection(this);
            }
            return result3;
        }

        private IExecuteResult ExecuteAll(Expression query, QueryInfo[] queryInfos, IObjectReaderFactory factory, object[] userArguments, ICompiledSubQuery[] subQueries)
        {
            IExecuteResult result = null;
            object lastResult = null;
            int index = 0;
            int length = queryInfos.Length;
            while (index < length)
            {
                if (index < (length - 1))
                {
                    result = this.Execute(query, queryInfos[index], null, null, userArguments, subQueries, lastResult);
                }
                else
                {
                    result = this.Execute(query, queryInfos[index], factory, null, userArguments, subQueries, lastResult);
                }
                if (queryInfos[index].ResultShape == ResultShape.Return)
                {
                    lastResult = result.ReturnValue;
                }
                index++;
            }
            return result;
        }

        private void ExecuteCommand(string command)
        {
            if (this.log != null)
            {
                this.log.WriteLine(command);
                this.log.WriteLine();
            }
            IDbCommand command2 = this.conManager.Connection.CreateCommand();
            command2.CommandTimeout = this.commandTimeout;
            command2.Transaction = this.conManager.Transaction;
            command2.CommandText = command;
            command2.ExecuteNonQuery();
        }

        private IExecuteResult GetCachedResult(Expression query)
        {
            object cachedObject = this.services.GetCachedObject(query);
            if (cachedObject != null)
            {
                switch (this.GetResultShape(query))
                {
                    case ResultShape.Singleton:
                        return new ExecuteResult(null, null, null, cachedObject);

                    case ResultShape.Sequence:
                        return new ExecuteResult(null, null, null, Activator.CreateInstance(typeof(SequenceOfOne).MakeGenericType(new Type[] { System.Data.Linq.SqlClient.TypeSystem.GetElementType(this.GetResultType(query)) }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { cachedObject }, null));
                }
            }
            return null;
        }

        private string GetConnectionString(string fileOrServerOrConnectionString)
        {
            if (fileOrServerOrConnectionString.IndexOf('=') >= 0)
            {
                return fileOrServerOrConnectionString;
            }
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            if (fileOrServerOrConnectionString.EndsWith(".mdf", StringComparison.OrdinalIgnoreCase))
            {
                builder.Add("AttachDBFileName", fileOrServerOrConnectionString);
                builder.Add("Server", @"localhost\sqlexpress");
                builder.Add("Integrated Security", "SSPI");
                builder.Add("User Instance", "true");
                builder.Add("MultipleActiveResultSets", "true");
            }
            else if (fileOrServerOrConnectionString.EndsWith(".sdf", StringComparison.OrdinalIgnoreCase))
            {
                builder.Add("Data Source", fileOrServerOrConnectionString);
            }
            else
            {
                builder.Add("Server", fileOrServerOrConnectionString);
                builder.Add("Database", this.services.Model.DatabaseName);
                builder.Add("Integrated Security", "SSPI");
            }
            return builder.ToString();
        }

        private string GetDatabaseName(string constr)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder {
                ConnectionString = constr
            };
            if (builder.ContainsKey("Initial Catalog"))
            {
                return (string) builder["Initial Catalog"];
            }
            if (builder.ContainsKey("Database"))
            {
                return (string) builder["Database"];
            }
            if (builder.ContainsKey("AttachDBFileName"))
            {
                return (string) builder["AttachDBFileName"];
            }
            if (builder.ContainsKey("Data Source") && ((string) builder["Data Source"]).EndsWith(".sdf", StringComparison.OrdinalIgnoreCase))
            {
                return (string) builder["Data Source"];
            }
            return this.services.Model.DatabaseName;
        }

        private IObjectReaderFactory GetDefaultFactory(System.Data.Linq.Mapping.MetaType rowType)
        {
            if (rowType == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("rowType");
            }
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            Expression source = Expression.Constant(null);
            SqlUserQuery query = new SqlUserQuery(string.Empty, null, null, source);
            if (System.Data.Linq.SqlClient.TypeSystem.IsSimpleType(rowType.Type))
            {
                SqlUserColumn item = new SqlUserColumn(rowType.Type, this.typeProvider.From(rowType.Type), query, "", false, query.SourceExpression);
                query.Columns.Add(item);
                query.Projection = item;
            }
            else
            {
                SqlUserRow row = new SqlUserRow(rowType.InheritanceRoot, this.typeProvider.GetApplicationType(0), query, source);
                query.Projection = this.translator.BuildProjection(row, rowType, true, null, source);
            }
            Type sequenceType = System.Data.Linq.SqlClient.TypeSystem.GetSequenceType(rowType.Type);
            QueryInfo[] infoArray = this.BuildQuery(ResultShape.Sequence, sequenceType, query, null, annotations);
            return this.GetReaderFactory(infoArray[infoArray.Length - 1].Query, rowType.Type);
        }

        private SqlSelect GetFinalSelect(SqlNode node)
        {
            SqlNodeType nodeType = node.NodeType;
            if (nodeType != SqlNodeType.Block)
            {
                if (nodeType == SqlNodeType.Select)
                {
                    return (SqlSelect) node;
                }
                return null;
            }
            SqlBlock block = (SqlBlock) node;
            return this.GetFinalSelect(block.Statements[block.Statements.Count - 1]);
        }

        private MetaFunction GetFunction(Expression query)
        {
            LambdaExpression expression = query as LambdaExpression;
            if (expression != null)
            {
                query = expression.Body;
            }
            MethodCallExpression expression2 = query as MethodCallExpression;
            if ((expression2 != null) && typeof(DataContext).IsAssignableFrom(expression2.Method.DeclaringType))
            {
                return this.services.Model.GetFunction(expression2.Method);
            }
            return null;
        }

        private static DbProviderFactory GetProvider(string providerName)
        {
            if ((from r in DbProviderFactories.GetFactoryClasses().Rows.OfType<DataRow>() select (string) r["InvariantName"]).Contains<string>(providerName, StringComparer.OrdinalIgnoreCase))
            {
                return DbProviderFactories.GetFactory(providerName);
            }
            return null;
        }

        private IObjectReaderFactory GetReaderFactory(SqlNode node, Type elemType)
        {
            SqlSelect finalSelect = node as SqlSelect;
            SqlExpression selection = null;
            if ((finalSelect == null) && (node.NodeType == SqlNodeType.Block))
            {
                finalSelect = this.GetFinalSelect(node);
            }
            if (finalSelect != null)
            {
                selection = finalSelect.Selection;
            }
            else
            {
                SqlUserQuery query = node as SqlUserQuery;
                if ((query != null) && (query.Projection != null))
                {
                    selection = query.Projection;
                }
            }
            if (selection != null)
            {
                return this.readerCompiler.Compile(selection, elemType);
            }
            return this.GetDefaultFactory(this.services.Model.GetMetaType(elemType));
        }

        private ResultShape GetResultShape(Expression query)
        {
            LambdaExpression expression = query as LambdaExpression;
            if (expression != null)
            {
                query = expression.Body;
            }
            if (query.Type == typeof(void))
            {
                return ResultShape.Return;
            }
            if (query.Type == typeof(System.Data.Linq.IMultipleResults))
            {
                return ResultShape.MultipleResults;
            }
            bool flag = typeof(IEnumerable).IsAssignableFrom(query.Type);
            ProviderType type = this.typeProvider.From(query.Type);
            bool flag2 = !type.IsRuntimeOnlyType && !type.IsApplicationType;
            bool flag3 = flag2 || !flag;
            MethodCallExpression expression2 = query as MethodCallExpression;
            if (expression2 != null)
            {
                if ((expression2.Method.DeclaringType == typeof(Queryable)) || (expression2.Method.DeclaringType == typeof(Enumerable)))
                {
                    string str;
                    if (((str = expression2.Method.Name) != null) && (((str == "First") || (str == "FirstOrDefault")) || ((str == "Single") || (str == "SingleOrDefault"))))
                    {
                        flag3 = true;
                    }
                }
                else if (expression2.Method.DeclaringType == typeof(DataContext))
                {
                    if (expression2.Method.Name == "ExecuteCommand")
                    {
                        return ResultShape.Return;
                    }
                }
                else if (expression2.Method.DeclaringType.IsSubclassOf(typeof(DataContext)))
                {
                    MetaFunction function = this.GetFunction(query);
                    if (function != null)
                    {
                        if (!function.IsComposable)
                        {
                            flag3 = false;
                        }
                        else if (flag2)
                        {
                            flag3 = true;
                        }
                    }
                }
                else if ((expression2.Method.DeclaringType == typeof(DataManipulation)) && (expression2.Method.ReturnType == typeof(int)))
                {
                    return ResultShape.Return;
                }
            }
            if (flag3)
            {
                return ResultShape.Singleton;
            }
            if (flag2)
            {
                return ResultShape.Return;
            }
            return ResultShape.Sequence;
        }

        private Type GetResultType(Expression query)
        {
            LambdaExpression expression = query as LambdaExpression;
            if (expression != null)
            {
                query = expression.Body;
            }
            return query.Type;
        }

        private void InitializeProviderMode()
        {
            if (this.mode == ProviderMode.NotYetDecided)
            {
                if (this.IsSqlCe)
                {
                    this.mode = ProviderMode.SqlCE;
                }
                else if (this.IsServer2KOrEarlier)
                {
                    this.mode = ProviderMode.Sql2000;
                }
                else if (this.IsServer2005)
                {
                    this.mode = ProviderMode.Sql2005;
                }
                else
                {
                    this.mode = ProviderMode.Sql2008;
                }
            }
            if (this.typeProvider == null)
            {
                switch (this.mode)
                {
                    case ProviderMode.Sql2000:
                        this.typeProvider = SqlTypeSystem.Create2000Provider();
                        break;

                    case ProviderMode.Sql2005:
                        this.typeProvider = SqlTypeSystem.Create2005Provider();
                        break;

                    case ProviderMode.Sql2008:
                        this.typeProvider = SqlTypeSystem.Create2008Provider();
                        break;

                    case ProviderMode.SqlCE:
                        this.typeProvider = SqlTypeSystem.CreateCEProvider();
                        break;
                }
            }
            if (this.sqlFactory == null)
            {
                this.sqlFactory = new SqlFactory(this.typeProvider, this.services.Model);
                this.translator = new Translator(this.services, this.sqlFactory, this.typeProvider);
            }
        }

        private void LogCommand(TextWriter writer, DbCommand cmd)
        {
            if (writer != null)
            {
                writer.WriteLine(cmd.CommandText);
                foreach (DbParameter parameter in cmd.Parameters)
                {
                    int num = 0;
                    int num2 = 0;
                    PropertyInfo property = parameter.GetType().GetProperty("Precision");
                    if (property != null)
                    {
                        num = (int) Convert.ChangeType(property.GetValue(parameter, null), typeof(int), CultureInfo.InvariantCulture);
                    }
                    PropertyInfo info2 = parameter.GetType().GetProperty("Scale");
                    if (info2 != null)
                    {
                        num2 = (int) Convert.ChangeType(info2.GetValue(parameter, null), typeof(int), CultureInfo.InvariantCulture);
                    }
                    System.Data.SqlClient.SqlParameter parameter2 = parameter as System.Data.SqlClient.SqlParameter;
                    writer.WriteLine("-- {0}: {1} {2} (Size = {3}; Prec = {4}; Scale = {5}) [{6}]", new object[] { parameter.ParameterName, parameter.Direction, (parameter2 == null) ? parameter.DbType.ToString() : parameter2.SqlDbType.ToString(), parameter.Size.ToString(CultureInfo.CurrentCulture), num, num2, (parameter2 == null) ? parameter.Value : parameter2.SqlValue });
                }
                writer.WriteLine("-- Context: {0}({1}) Model: {2} Build: {3}", new object[] { base.GetType().Name, this.Mode, this.services.Model.GetType().Name, "3.5.30729.5455" });
                writer.WriteLine();
            }
        }

        void IProvider.ClearConnection()
        {
            this.CheckDispose();
            this.CheckInitialized();
            this.conManager.ClearConnection();
        }

        ICompiledQuery IProvider.Compile(Expression query)
        {
            this.CheckDispose();
            this.CheckInitialized();
            if (query == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("query");
            }
            this.InitializeProviderMode();
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            QueryInfo[] queries = this.BuildQuery(query, annotations);
            this.CheckSqlCompatibility(queries, annotations);
            LambdaExpression expression = query as LambdaExpression;
            if (expression != null)
            {
                query = expression.Body;
            }
            IObjectReaderFactory readerFactory = null;
            ICompiledSubQuery[] subQueries = null;
            QueryInfo info = queries[queries.Length - 1];
            if (info.ResultShape == ResultShape.Singleton)
            {
                subQueries = this.CompileSubQueries(info.Query);
                readerFactory = this.GetReaderFactory(info.Query, info.ResultType);
            }
            else if (info.ResultShape == ResultShape.Sequence)
            {
                subQueries = this.CompileSubQueries(info.Query);
                readerFactory = this.GetReaderFactory(info.Query, System.Data.Linq.SqlClient.TypeSystem.GetElementType(info.ResultType));
            }
            return new CompiledQuery(this, query, queries, readerFactory, subQueries);
        }

        void IProvider.CreateDatabase()
        {
            object obj3;
            this.CheckDispose();
            this.CheckInitialized();
            string dbName = null;
            string str2 = null;
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder {
                ConnectionString = this.conManager.Connection.ConnectionString
            };
            if (this.conManager.Connection.State != ConnectionState.Closed)
            {
                object obj4;
                if ((this.mode == ProviderMode.SqlCE) && File.Exists(this.dbName))
                {
                    throw System.Data.Linq.SqlClient.Error.CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists(this.dbName);
                }
                if (builder.TryGetValue("Initial Catalog", out obj4))
                {
                    dbName = obj4.ToString();
                }
                if (builder.TryGetValue("Database", out obj4))
                {
                    dbName = obj4.ToString();
                }
                if (builder.TryGetValue("AttachDBFileName", out obj4))
                {
                    str2 = obj4.ToString();
                }
                goto Label_01D2;
            }
            if (this.mode == ProviderMode.SqlCE)
            {
                if (!File.Exists(this.dbName))
                {
                    Type type = this.conManager.Connection.GetType().Module.GetType("System.Data.SqlServerCe.SqlCeEngine");
                    using (object obj2 = Activator.CreateInstance(type, new object[] { builder.ToString() }))
                    {
                        try
                        {
                            type.InvokeMember("CreateDatabase", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj2, new object[0], CultureInfo.InvariantCulture);
                        }
                        catch (TargetInvocationException exception)
                        {
                            throw exception.InnerException;
                        }
                        goto Label_0153;
                    }
                }
                throw System.Data.Linq.SqlClient.Error.CreateDatabaseFailedBecauseSqlCEDatabaseAlreadyExists(this.dbName);
            }
            if (builder.TryGetValue("Initial Catalog", out obj3))
            {
                dbName = obj3.ToString();
                builder.Remove("Initial Catalog");
            }
            if (builder.TryGetValue("Database", out obj3))
            {
                dbName = obj3.ToString();
                builder.Remove("Database");
            }
            if (builder.TryGetValue("AttachDBFileName", out obj3))
            {
                str2 = obj3.ToString();
                builder.Remove("AttachDBFileName");
            }
        Label_0153:
            this.conManager.Connection.ConnectionString = builder.ToString();
        Label_01D2:
            if (string.IsNullOrEmpty(dbName))
            {
                if (string.IsNullOrEmpty(str2))
                {
                    if (string.IsNullOrEmpty(this.dbName))
                    {
                        throw System.Data.Linq.SqlClient.Error.CouldNotDetermineCatalogName();
                    }
                    dbName = this.dbName;
                }
                else
                {
                    dbName = Path.GetFullPath(str2);
                }
            }
            this.conManager.UseConnection(this);
            this.conManager.AutoClose = false;
            try
            {
                if (this.services.Model.GetTables().FirstOrDefault<MetaTable>() == null)
                {
                    throw System.Data.Linq.SqlClient.Error.CreateDatabaseFailedBecauseOfContextWithNoTables(this.services.Model.DatabaseName);
                }
                this.deleted = false;
                if (this.mode == ProviderMode.SqlCE)
                {
                    foreach (MetaTable table in this.services.Model.GetTables())
                    {
                        string createTableCommand = SqlBuilder.GetCreateTableCommand(table);
                        if (!string.IsNullOrEmpty(createTableCommand))
                        {
                            this.ExecuteCommand(createTableCommand);
                        }
                    }
                    foreach (MetaTable table2 in this.services.Model.GetTables())
                    {
                        foreach (string str4 in SqlBuilder.GetCreateForeignKeyCommands(table2))
                        {
                            if (!string.IsNullOrEmpty(str4))
                            {
                                this.ExecuteCommand(str4);
                            }
                        }
                    }
                }
                else
                {
                    string command = SqlBuilder.GetCreateDatabaseCommand(dbName, str2, Path.ChangeExtension(str2, ".ldf"));
                    this.ExecuteCommand(command);
                    this.conManager.Connection.ChangeDatabase(dbName);
                    if ((this.mode == ProviderMode.Sql2005) || (this.mode == ProviderMode.Sql2008))
                    {
                        HashSet<string> set = new HashSet<string>();
                        foreach (MetaTable table3 in this.services.Model.GetTables())
                        {
                            string createSchemaForTableCommand = SqlBuilder.GetCreateSchemaForTableCommand(table3);
                            if (!string.IsNullOrEmpty(createSchemaForTableCommand))
                            {
                                set.Add(createSchemaForTableCommand);
                            }
                        }
                        foreach (string str7 in set)
                        {
                            this.ExecuteCommand(str7);
                        }
                    }
                    StringBuilder builder2 = new StringBuilder();
                    foreach (MetaTable table4 in this.services.Model.GetTables())
                    {
                        string str8 = SqlBuilder.GetCreateTableCommand(table4);
                        if (!string.IsNullOrEmpty(str8))
                        {
                            builder2.AppendLine(str8);
                        }
                    }
                    foreach (MetaTable table5 in this.services.Model.GetTables())
                    {
                        foreach (string str9 in SqlBuilder.GetCreateForeignKeyCommands(table5))
                        {
                            if (!string.IsNullOrEmpty(str9))
                            {
                                builder2.AppendLine(str9);
                            }
                        }
                    }
                    if (builder2.Length > 0)
                    {
                        builder2.Insert(0, "SET ARITHABORT ON" + Environment.NewLine);
                        this.ExecuteCommand(builder2.ToString());
                    }
                }
            }
            finally
            {
                this.conManager.ReleaseConnection(this);
                if (this.conManager.Connection is SqlConnection)
                {
                    SqlConnection.ClearAllPools();
                }
            }
        }

        bool IProvider.DatabaseExists()
        {
            this.CheckDispose();
            this.CheckInitialized();
            if (this.deleted)
            {
                return false;
            }
            bool flag = false;
            if (this.mode == ProviderMode.SqlCE)
            {
                return File.Exists(this.dbName);
            }
            string connectionString = this.conManager.Connection.ConnectionString;
            try
            {
                this.conManager.UseConnection(this);
                this.conManager.Connection.ChangeDatabase(this.dbName);
                this.conManager.ReleaseConnection(this);
                flag = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if ((this.conManager.Connection.State == ConnectionState.Closed) && (string.Compare(this.conManager.Connection.ConnectionString, connectionString, StringComparison.Ordinal) != 0))
                {
                    this.conManager.Connection.ConnectionString = connectionString;
                }
            }
            return flag;
        }

        void IProvider.DeleteDatabase()
        {
            this.CheckDispose();
            this.CheckInitialized();
            if (!this.deleted)
            {
                if (this.mode == ProviderMode.SqlCE)
                {
                    ((IProvider) this).ClearConnection();
                    File.Delete(this.dbName);
                    this.deleted = true;
                }
                else
                {
                    string connectionString = this.conManager.Connection.ConnectionString;
                    DbConnection connection = this.conManager.UseConnection(this);
                    try
                    {
                        connection.ChangeDatabase("MASTER");
                        if (connection is SqlConnection)
                        {
                            SqlConnection.ClearAllPools();
                        }
                        if (this.log != null)
                        {
                            this.log.WriteLine(System.Data.Linq.SqlClient.Strings.LogAttemptingToDeleteDatabase(this.dbName));
                        }
                        this.ExecuteCommand(SqlBuilder.GetDropDatabaseCommand(this.dbName));
                        this.deleted = true;
                    }
                    finally
                    {
                        this.conManager.ReleaseConnection(this);
                        if ((this.conManager.Connection.State == ConnectionState.Closed) && (string.Compare(this.conManager.Connection.ConnectionString, connectionString, StringComparison.Ordinal) != 0))
                        {
                            this.conManager.Connection.ConnectionString = connectionString;
                        }
                    }
                }
            }
        }

        IExecuteResult IProvider.Execute(Expression query)
        {
            this.CheckDispose();
            this.CheckInitialized();
            this.CheckNotDeleted();
            if (query == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("query");
            }
            this.InitializeProviderMode();
            query = Funcletizer.Funcletize(query);
            if (this.EnableCacheLookup)
            {
                IExecuteResult cachedResult = this.GetCachedResult(query);
                if (cachedResult != null)
                {
                    return cachedResult;
                }
            }
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            QueryInfo[] queries = this.BuildQuery(query, annotations);
            this.CheckSqlCompatibility(queries, annotations);
            LambdaExpression expression = query as LambdaExpression;
            if (expression != null)
            {
                query = expression.Body;
            }
            IObjectReaderFactory readerFactory = null;
            ICompiledSubQuery[] subQueries = null;
            QueryInfo info = queries[queries.Length - 1];
            if (info.ResultShape == ResultShape.Singleton)
            {
                subQueries = this.CompileSubQueries(info.Query);
                readerFactory = this.GetReaderFactory(info.Query, info.ResultType);
            }
            else if (info.ResultShape == ResultShape.Sequence)
            {
                subQueries = this.CompileSubQueries(info.Query);
                readerFactory = this.GetReaderFactory(info.Query, System.Data.Linq.SqlClient.TypeSystem.GetElementType(info.ResultType));
            }
            return this.ExecuteAll(query, queries, readerFactory, null, subQueries);
        }

        DbCommand IProvider.GetCommand(Expression query)
        {
            this.CheckDispose();
            this.CheckInitialized();
            if (query == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("query");
            }
            this.InitializeProviderMode();
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            QueryInfo[] infoArray = this.BuildQuery(query, annotations);
            QueryInfo info = infoArray[infoArray.Length - 1];
            DbCommand cmd = this.conManager.Connection.CreateCommand();
            cmd.CommandText = info.CommandText;
            cmd.Transaction = (SqlTransaction) this.conManager.Transaction;
            cmd.CommandTimeout = this.commandTimeout;
            this.AssignParameters(cmd, info.Parameters, null, null);
            return cmd;
        }

        string IProvider.GetQueryText(Expression query)
        {
            this.CheckDispose();
            this.CheckInitialized();
            if (query == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("query");
            }
            this.InitializeProviderMode();
            SqlNodeAnnotations annotations = new SqlNodeAnnotations();
            QueryInfo[] infoArray = this.BuildQuery(query, annotations);
            StringBuilder builder = new StringBuilder();
            int index = 0;
            int length = infoArray.Length;
            while (index < length)
            {
                QueryInfo info = infoArray[index];
                builder.Append(info.CommandText);
                builder.AppendLine();
                index++;
            }
            return builder.ToString();
        }

        void IProvider.Initialize(IDataServices dataServices, object connection)
        {
            DbConnection connection2;
            Type type;
            if (dataServices == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("dataServices");
            }
            this.services = dataServices;
            DbTransaction transaction = null;
            string fileOrServerOrConnectionString = connection as string;
            if (fileOrServerOrConnectionString != null)
            {
                string connectionString = this.GetConnectionString(fileOrServerOrConnectionString);
                this.dbName = this.GetDatabaseName(connectionString);
                if (this.dbName.EndsWith(".sdf", StringComparison.OrdinalIgnoreCase))
                {
                    this.mode = ProviderMode.SqlCE;
                }
                if (this.mode == ProviderMode.SqlCE)
                {
                    DbProviderFactory provider = GetProvider("System.Data.SqlServerCe.3.5");
                    if (provider == null)
                    {
                        throw System.Data.Linq.SqlClient.Error.ProviderNotInstalled(this.dbName, "System.Data.SqlServerCe.3.5");
                    }
                    connection2 = provider.CreateConnection();
                }
                else
                {
                    connection2 = new SqlConnection();
                }
                connection2.ConnectionString = connectionString;
            }
            else
            {
                transaction = connection as SqlTransaction;
                if ((transaction == null) && (connection.GetType().FullName == "System.Data.SqlServerCe.SqlCeTransaction"))
                {
                    transaction = connection as DbTransaction;
                }
                if (transaction != null)
                {
                    connection = transaction.Connection;
                }
                connection2 = connection as DbConnection;
                if (connection2 == null)
                {
                    throw System.Data.Linq.SqlClient.Error.InvalidConnectionArgument("connection");
                }
                if (connection2.GetType().FullName == "System.Data.SqlServerCe.SqlCeConnection")
                {
                    this.mode = ProviderMode.SqlCE;
                }
                this.dbName = this.GetDatabaseName(connection2.ConnectionString);
            }
            using (DbCommand command = connection2.CreateCommand())
            {
                this.commandTimeout = command.CommandTimeout;
            }
            int maxUsers = 1;
            if (connection2.ConnectionString.Contains("MultipleActiveResultSets"))
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder {
                    ConnectionString = connection2.ConnectionString
                };
                if (string.Compare((string) builder["MultipleActiveResultSets"], "true", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    maxUsers = 10;
                }
            }
            this.conManager = new SqlConnectionManager(this, connection2, maxUsers);
            if (transaction != null)
            {
                this.conManager.Transaction = transaction;
            }
            if (this.mode == ProviderMode.SqlCE)
            {
                type = connection2.GetType().Module.GetType("System.Data.SqlServerCe.SqlCeDataReader");
            }
            else if (connection2 is SqlConnection)
            {
                type = typeof(SqlDataReader);
            }
            else
            {
                type = typeof(DbDataReader);
            }
            this.readerCompiler = new ObjectReaderCompiler(type, this.services);
        }

        System.Data.Linq.IMultipleResults IProvider.Translate(DbDataReader reader)
        {
            this.CheckDispose();
            this.CheckInitialized();
            this.InitializeProviderMode();
            if (reader == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("reader");
            }
            return new MultipleResults(this, null, this.readerCompiler.CreateSession(reader, this, null, null, null), null);
        }

        IEnumerable IProvider.Translate(Type elementType, DbDataReader reader)
        {
            this.CheckDispose();
            this.CheckInitialized();
            this.InitializeProviderMode();
            if (elementType == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("elementType");
            }
            if (reader == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("reader");
            }
            System.Data.Linq.Mapping.MetaType metaType = this.services.Model.GetMetaType(elementType);
            IEnumerator enumerator = this.GetDefaultFactory(metaType).Create(reader, true, this, null, null, null);
            return (IEnumerable) Activator.CreateInstance(typeof(OneTimeEnumerable).MakeGenericType(new Type[] { elementType }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { enumerator }, null);
        }

        void IConnectionUser.CompleteUse()
        {
        }

        internal bool CheckQueries
        {
            get
            {
                this.CheckDispose();
                return this.checkQueries;
            }
            set
            {
                this.CheckDispose();
                this.checkQueries = value;
            }
        }

        internal bool EnableCacheLookup
        {
            get
            {
                this.CheckDispose();
                return this.enableCacheLookup;
            }
            set
            {
                this.CheckDispose();
                this.enableCacheLookup = value;
            }
        }

        private bool IsServer2005
        {
            get
            {
                bool flag;
                DbConnection connection = this.conManager.UseConnection(this);
                try
                {
                    if (connection.ServerVersion.StartsWith("09.00.", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    flag = false;
                }
                finally
                {
                    this.conManager.ReleaseConnection(this);
                }
                return flag;
            }
        }

        private bool IsServer2KOrEarlier
        {
            get
            {
                bool flag;
                DbConnection connection = this.conManager.UseConnection(this);
                try
                {
                    string serverVersion = connection.ServerVersion;
                    if (serverVersion.StartsWith("06.00.", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    if (serverVersion.StartsWith("06.50.", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    if (serverVersion.StartsWith("07.00.", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    if (serverVersion.StartsWith("08.00.", StringComparison.Ordinal))
                    {
                        return true;
                    }
                    flag = false;
                }
                finally
                {
                    this.conManager.ReleaseConnection(this);
                }
                return flag;
            }
        }

        private bool IsSqlCe
        {
            get
            {
                DbConnection connection = this.conManager.UseConnection(this);
                try
                {
                    if (string.CompareOrdinal(connection.GetType().FullName, "System.Data.SqlServerCe.SqlCeConnection") == 0)
                    {
                        return true;
                    }
                }
                finally
                {
                    this.conManager.ReleaseConnection(this);
                }
                return false;
            }
        }

        internal int MaxUsers
        {
            get
            {
                this.CheckDispose();
                return this.conManager.MaxUsers;
            }
        }

        internal ProviderMode Mode
        {
            get
            {
                this.CheckDispose();
                this.CheckInitialized();
                this.InitializeProviderMode();
                return this.mode;
            }
        }

        internal System.Data.Linq.SqlClient.OptimizationFlags OptimizationFlags
        {
            get
            {
                this.CheckDispose();
                return this.optimizationFlags;
            }
            set
            {
                this.CheckDispose();
                this.optimizationFlags = value;
            }
        }

        internal int QueryCount
        {
            get
            {
                this.CheckDispose();
                return this.queryCount;
            }
        }

        int IProvider.CommandTimeout
        {
            get
            {
                this.CheckDispose();
                return this.commandTimeout;
            }
            set
            {
                this.CheckDispose();
                this.commandTimeout = value;
            }
        }

        DbConnection IProvider.Connection
        {
            get
            {
                this.CheckDispose();
                this.CheckInitialized();
                return this.conManager.Connection;
            }
        }

        TextWriter IProvider.Log
        {
            get
            {
                this.CheckDispose();
                this.CheckInitialized();
                return this.log;
            }
            set
            {
                this.CheckDispose();
                this.CheckInitialized();
                this.log = value;
            }
        }

        DbTransaction IProvider.Transaction
        {
            get
            {
                this.CheckDispose();
                this.CheckInitialized();
                return this.conManager.Transaction;
            }
            set
            {
                this.CheckDispose();
                this.CheckInitialized();
                this.conManager.Transaction = value;
            }
        }

        IConnectionManager IReaderProvider.ConnectionManager =>
            this.conManager;

        IDataServices IReaderProvider.Services =>
            this.services;

        private class CompiledQuery : ICompiledQuery
        {
            private IObjectReaderFactory factory;
            private DataLoadOptions originalShape;
            private Expression query;
            private SqlProvider.QueryInfo[] queryInfos;
            private ICompiledSubQuery[] subQueries;

            internal CompiledQuery(SqlProvider provider, Expression query, SqlProvider.QueryInfo[] queryInfos, IObjectReaderFactory factory, ICompiledSubQuery[] subQueries)
            {
                this.originalShape = provider.services.Context.LoadOptions;
                this.query = query;
                this.queryInfos = queryInfos;
                this.factory = factory;
                this.subQueries = subQueries;
            }

            private static bool AreEquivalentShapes(DataLoadOptions shape1, DataLoadOptions shape2)
            {
                if (shape1 == shape2)
                {
                    return true;
                }
                if (shape1 == null)
                {
                    return shape2.IsEmpty;
                }
                if (shape2 == null)
                {
                    return shape1.IsEmpty;
                }
                return (shape1.IsEmpty && shape2.IsEmpty);
            }

            public IExecuteResult Execute(IProvider provider, object[] arguments)
            {
                if (provider == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("provider");
                }
                SqlProvider provider2 = provider as SqlProvider;
                if (provider2 == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentTypeMismatch("provider");
                }
                if (!AreEquivalentShapes(this.originalShape, provider2.services.Context.LoadOptions))
                {
                    throw System.Data.Linq.SqlClient.Error.CompiledQueryAgainstMultipleShapesNotSupported();
                }
                return provider2.ExecuteAll(this.query, this.queryInfos, this.factory, arguments, this.subQueries);
            }
        }

        private class CompiledSubQuery : ICompiledSubQuery
        {
            private IObjectReaderFactory factory;
            private System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Linq.SqlClient.SqlParameter> parameters;
            private SqlProvider.QueryInfo queryInfo;
            private ICompiledSubQuery[] subQueries;

            internal CompiledSubQuery(SqlProvider.QueryInfo queryInfo, IObjectReaderFactory factory, System.Collections.ObjectModel.ReadOnlyCollection<System.Data.Linq.SqlClient.SqlParameter> parameters, ICompiledSubQuery[] subQueries)
            {
                this.queryInfo = queryInfo;
                this.factory = factory;
                this.parameters = parameters;
                this.subQueries = subQueries;
            }

            public IExecuteResult Execute(IProvider provider, object[] parentArgs, object[] userArgs)
            {
                if (((parentArgs == null) && (this.parameters != null)) && (this.parameters.Count != 0))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("arguments");
                }
                SqlProvider provider2 = provider as SqlProvider;
                if (provider2 == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentTypeMismatch("provider");
                }
                List<SqlParameterInfo> list = new List<SqlParameterInfo>(this.queryInfo.Parameters);
                int index = 0;
                int count = this.parameters.Count;
                while (index < count)
                {
                    list.Add(new SqlParameterInfo(this.parameters[index], parentArgs[index]));
                    index++;
                }
                SqlProvider.QueryInfo queryInfo = new SqlProvider.QueryInfo(this.queryInfo.Query, this.queryInfo.CommandText, list.AsReadOnly(), this.queryInfo.ResultShape, this.queryInfo.ResultType);
                return provider2.Execute(null, queryInfo, this.factory, parentArgs, userArgs, this.subQueries, null);
            }
        }

        private class ExecuteResult : IExecuteResult, IDisposable
        {
            private DbCommand command;
            private int iReturnParameter;
            private bool isDisposed;
            private System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters;
            private IObjectReaderSession session;
            private bool useReturnValue;
            private object value;

            internal ExecuteResult(DbCommand command, System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters, IObjectReaderSession session)
            {
                this.iReturnParameter = -1;
                this.command = command;
                this.parameters = parameters;
                this.session = session;
            }

            internal ExecuteResult(DbCommand command, System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters, IObjectReaderSession session, object value) : this(command, parameters, session, value, false)
            {
            }

            internal ExecuteResult(DbCommand command, System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters, IObjectReaderSession session, object value, bool useReturnValue) : this(command, parameters, session)
            {
                this.value = value;
                this.useReturnValue = useReturnValue;
                if (((this.command != null) && (this.parameters != null)) && useReturnValue)
                {
                    this.iReturnParameter = this.GetParameterIndex("@RETURN_VALUE");
                }
            }

            public void Dispose()
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    if (this.session != null)
                    {
                        this.session.Dispose();
                    }
                }
            }

            private int GetParameterIndex(string paramName)
            {
                int num2 = 0;
                int count = this.parameters.Count;
                while (num2 < count)
                {
                    if (string.Compare(this.parameters[num2].Parameter.Name, paramName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return num2;
                    }
                    num2++;
                }
                return -1;
            }

            public object GetParameterValue(int parameterIndex)
            {
                if (((this.parameters == null) || (parameterIndex < 0)) || (parameterIndex > this.parameters.Count))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentOutOfRange("parameterIndex");
                }
                if ((this.session != null) && !this.session.IsBuffered)
                {
                    this.session.Buffer();
                }
                SqlParameterInfo info = this.parameters[parameterIndex];
                object obj2 = this.command.Parameters[parameterIndex].Value;
                if (obj2 == DBNull.Value)
                {
                    obj2 = null;
                }
                if ((obj2 != null) && (obj2.GetType() != info.Parameter.ClrType))
                {
                    return DBConvert.ChangeType(obj2, info.Parameter.ClrType);
                }
                return obj2;
            }

            internal object GetParameterValue(string paramName)
            {
                int parameterIndex = this.GetParameterIndex(paramName);
                if (parameterIndex >= 0)
                {
                    return this.GetParameterValue(parameterIndex);
                }
                return null;
            }

            public object ReturnValue
            {
                get
                {
                    if (this.iReturnParameter >= 0)
                    {
                        return this.GetParameterValue(this.iReturnParameter);
                    }
                    return this.value;
                }
                internal set
                {
                    this.value = value;
                }
            }
        }

        private class MultipleResults : System.Data.Linq.IMultipleResults, IFunctionResult, IDisposable
        {
            private SqlProvider.ExecuteResult executeResult;
            private MetaFunction function;
            private bool isDisposed;
            private SqlProvider provider;
            private IObjectReaderSession session;

            internal MultipleResults(SqlProvider provider, MetaFunction function, IObjectReaderSession session, SqlProvider.ExecuteResult executeResult)
            {
                this.provider = provider;
                this.function = function;
                this.session = session;
                this.executeResult = executeResult;
            }

            public void Dispose()
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    if (this.executeResult != null)
                    {
                        this.executeResult.Dispose();
                    }
                    else
                    {
                        this.session.Dispose();
                    }
                }
            }

            public IEnumerable<T> GetResult<T>()
            {
                System.Data.Linq.Mapping.MetaType rowType = null;
                Func<System.Data.Linq.Mapping.MetaType, bool> predicate = null;
                if (this.function != null)
                {
                    foreach (System.Data.Linq.Mapping.MetaType type2 in this.function.ResultRowTypes)
                    {
                        if (predicate == null)
                        {
                            predicate = it => it.Type == typeof(T);
                        }
                        rowType = type2.InheritanceTypes.SingleOrDefault<System.Data.Linq.Mapping.MetaType>(predicate);
                        if (rowType != null)
                        {
                            break;
                        }
                    }
                }
                if (rowType == null)
                {
                    rowType = this.provider.services.Model.GetMetaType(typeof(T));
                }
                IObjectReader nextResult = this.provider.GetDefaultFactory(rowType).GetNextResult(this.session, false);
                if (nextResult == null)
                {
                    this.Dispose();
                    return null;
                }
                return new SqlProvider.SingleResult<T>(new SqlProvider.OneTimeEnumerable<T>((IEnumerator<T>) nextResult), this.executeResult, this.provider.services.Context);
            }

            public object ReturnValue
            {
                get
                {
                    if (this.executeResult != null)
                    {
                        return this.executeResult.GetParameterValue("@RETURN_VALUE");
                    }
                    return null;
                }
            }
        }

        private class OneTimeEnumerable<T> : IEnumerable<T>, IEnumerable
        {
            private IEnumerator<T> enumerator;

            internal OneTimeEnumerable(IEnumerator<T> enumerator)
            {
                this.enumerator = enumerator;
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (this.enumerator == null)
                {
                    throw System.Data.Linq.SqlClient.Error.CannotEnumerateResultsMoreThanOnce();
                }
                IEnumerator<T> enumerator = this.enumerator;
                this.enumerator = null;
                return enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();
        }

        internal enum ProviderMode
        {
            NotYetDecided,
            Sql2000,
            Sql2005,
            Sql2008,
            SqlCE
        }

        internal class QueryInfo
        {
            private string commandText;
            private System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters;
            private SqlNode query;
            private System.Data.Linq.SqlClient.SqlProvider.ResultShape resultShape;
            private Type resultType;

            internal QueryInfo(SqlNode query, string commandText, System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> parameters, System.Data.Linq.SqlClient.SqlProvider.ResultShape resultShape, Type resultType)
            {
                this.query = query;
                this.commandText = commandText;
                this.parameters = parameters;
                this.resultShape = resultShape;
                this.resultType = resultType;
            }

            internal string CommandText =>
                this.commandText;

            internal System.Collections.ObjectModel.ReadOnlyCollection<SqlParameterInfo> Parameters =>
                this.parameters;

            internal SqlNode Query =>
                this.query;

            internal System.Data.Linq.SqlClient.SqlProvider.ResultShape ResultShape =>
                this.resultShape;

            internal Type ResultType =>
                this.resultType;
        }

        internal enum ResultShape
        {
            Return,
            Singleton,
            Sequence,
            MultipleResults
        }

        private class SequenceOfOne<T> : IEnumerable<T>, IEnumerable
        {
            private T[] sequence;

            internal SequenceOfOne(T value)
            {
                this.sequence = new T[] { value };
            }

            public IEnumerator<T> GetEnumerator() => 
                ((IEnumerable<T>) this.sequence).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();
        }

        private class SingleResult<T> : ISingleResult<T>, IEnumerable<T>, IEnumerable, IFunctionResult, IDisposable, IListSource
        {
            private IBindingList cachedList;
            private DataContext context;
            private IEnumerable<T> enumerable;
            private SqlProvider.ExecuteResult executeResult;

            internal SingleResult(IEnumerable<T> enumerable, SqlProvider.ExecuteResult executeResult, DataContext context)
            {
                this.enumerable = enumerable;
                this.executeResult = executeResult;
                this.context = context;
            }

            public void Dispose()
            {
                this.executeResult.Dispose();
            }

            public IEnumerator<T> GetEnumerator() => 
                this.enumerable.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            IList IListSource.GetList()
            {
                if (this.cachedList == null)
                {
                    this.cachedList = BindingList.Create<T>(this.context, this);
                }
                return this.cachedList;
            }

            public object ReturnValue =>
                this.executeResult.GetParameterValue("@RETURN_VALUE");

            bool IListSource.ContainsListCollection =>
                false;
        }

        private class SubQueryCompiler : SqlVisitor
        {
            private SqlProvider provider;
            private List<ICompiledSubQuery> subQueries;

            internal SubQueryCompiler(SqlProvider provider)
            {
                this.provider = provider;
            }

            internal ICompiledSubQuery[] Compile(SqlNode node)
            {
                this.subQueries = new List<ICompiledSubQuery>();
                this.Visit(node);
                return this.subQueries.ToArray();
            }

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq)
            {
                Type elementType = (cq.Query.NodeType == SqlNodeType.Multiset) ? System.Data.Linq.SqlClient.TypeSystem.GetElementType(cq.ClrType) : cq.ClrType;
                ICompiledSubQuery item = this.provider.CompileSubQuery(cq.Query.Select, elementType, cq.Parameters.AsReadOnly());
                cq.Ordinal = this.subQueries.Count;
                this.subQueries.Add(item);
                return cq;
            }

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                this.Visit(select.Selection);
                return select;
            }

            internal override SqlExpression VisitSubSelect(SqlSubSelect ss) => 
                ss;
        }
    }
}

