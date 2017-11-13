namespace System.Data.SqlClient.SqlGen
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal sealed class SqlGenerator : DbExpressionVisitor<ISqlFragment>
    {
        private static readonly Dictionary<string, FunctionHandler> _canonicalFunctionHandlers = InitializeCanonicalFunctionHandlers();
        private static readonly Set<string> _datepartKeywords = new Set<string>(new string[] { 
            "year", "yy", "yyyy", "quarter", "qq", "q", "month", "mm", "m", "dayofyear", "dy", "y", "day", "dd", "d", "week",
            "wk", "ww", "weekday", "dw", "w", "hour", "hh", "minute", "mi", "n", "second", "ss", "s", "millisecond", "ms", "microsecond",
            "mcs", "nanosecond", "ns", "tzoffset", "tz", "iso_week", "isoww", "isowk"
        }, StringComparer.OrdinalIgnoreCase).MakeReadOnly();
        private static readonly Dictionary<string, string> _functionNameToOperatorDictionary = InitializeFunctionNameToOperatorDictionary();
        private static readonly Set<string> _functionRequiresReturnTypeCast = new Set<string>(new string[] { "SqlServer.LEN", "SqlServer.PATINDEX", "SqlServer.CHARINDEX", "SqlServer.DATALENGTH", "Edm.IndexOf", "Edm.Length" }, StringComparer.Ordinal).MakeReadOnly();
        private static readonly Set<string> _maxTypeNames = new Set<string>(new string[] { "varchar(max)", "nvarchar(max)", "text", "ntext", "varbinary(max)", "image", "xml" }, StringComparer.Ordinal).MakeReadOnly();
        private static readonly Dictionary<string, FunctionHandler> _storeFunctionHandlers = InitializeStoreFunctionHandlers();
        private StoreItemCollection _storeItemCollection;
        private Dictionary<string, int> allColumnNames;
        private Dictionary<string, int> allExtentNames;
        private const byte defaultDecimalPrecision = 0x12;
        private static readonly char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        private TypeUsage integerType;
        private Stack<bool> isParentAJoinStack;
        private bool isVarRefSingle;
        private MetadataWorkspace metadataWorkspace;
        private Stack<SqlSelectStatement> selectStatementStack;
        private System.Data.SqlClient.SqlVersion sqlVersion;
        private SymbolTable symbolTable = new SymbolTable();

        private SqlGenerator(System.Data.SqlClient.SqlVersion sqlVersion)
        {
            this.sqlVersion = sqlVersion;
        }

        private void AddColumn(SqlSelectStatement selectStatement, Symbol symbol, List<Symbol> columnList, Dictionary<string, Symbol> columnDictionary, ref string separator, string columnName)
        {
            Symbol symbol2;
            this.allColumnNames[columnName] = 0;
            if (!symbol.Columns.TryGetValue(columnName, out symbol2))
            {
                symbol2 = new Symbol(columnName, null);
                symbol.Columns.Add(columnName, symbol2);
            }
            selectStatement.Select.Append(separator);
            selectStatement.Select.Append(symbol);
            selectStatement.Select.Append(".");
            if (symbol.OutputColumnsRenamed)
            {
                selectStatement.Select.Append(symbol2);
                selectStatement.OutputColumns.Add(symbol2.Name, symbol2);
            }
            else
            {
                selectStatement.Select.Append(QuoteIdentifier(columnName));
            }
            selectStatement.Select.Append(" AS ");
            selectStatement.Select.Append(symbol2);
            if (columnDictionary.ContainsKey(columnName))
            {
                columnDictionary[columnName].NeedsRenaming = true;
                symbol2.NeedsRenaming = true;
            }
            else
            {
                columnDictionary[columnName] = symbol.Columns[columnName];
            }
            columnList.Add(symbol2);
            separator = ", ";
        }

        private void AddColumns(SqlSelectStatement selectStatement, Symbol symbol, List<Symbol> columnList, Dictionary<string, Symbol> columnDictionary, ref string separator)
        {
            JoinSymbol symbol2 = symbol as JoinSymbol;
            if (symbol2 != null)
            {
                if (!symbol2.IsNestedJoin)
                {
                    foreach (Symbol symbol3 in symbol2.ExtentList)
                    {
                        if ((symbol3.Type != null) && !TypeSemantics.IsPrimitiveType(symbol3.Type))
                        {
                            this.AddColumns(selectStatement, symbol3, columnList, columnDictionary, ref separator);
                        }
                    }
                }
                else
                {
                    foreach (Symbol symbol4 in symbol2.ColumnList)
                    {
                        selectStatement.Select.Append(separator);
                        selectStatement.Select.Append(symbol);
                        selectStatement.Select.Append(".");
                        selectStatement.Select.Append(symbol4);
                        if (columnDictionary.ContainsKey(symbol4.Name))
                        {
                            columnDictionary[symbol4.Name].NeedsRenaming = true;
                            symbol4.NeedsRenaming = true;
                        }
                        else
                        {
                            columnDictionary[symbol4.Name] = symbol4;
                        }
                        columnList.Add(symbol4);
                        separator = ", ";
                    }
                }
            }
            else
            {
                if (symbol.OutputColumnsRenamed)
                {
                    selectStatement.OutputColumnsRenamed = true;
                    selectStatement.OutputColumns = new Dictionary<string, Symbol>();
                }
                if ((symbol.Type == null) || TypeSemantics.IsPrimitiveType(symbol.Type))
                {
                    this.AddColumn(selectStatement, symbol, columnList, columnDictionary, ref separator, "X");
                }
                else
                {
                    foreach (EdmProperty property in TypeHelpers.GetProperties(symbol.Type))
                    {
                        this.AddColumn(selectStatement, symbol, columnList, columnDictionary, ref separator, property.Name);
                    }
                }
            }
        }

        private List<Symbol> AddDefaultColumns(SqlSelectStatement selectStatement)
        {
            List<Symbol> columnList = new List<Symbol>();
            Dictionary<string, Symbol> columnDictionary = new Dictionary<string, Symbol>(StringComparer.OrdinalIgnoreCase);
            string separator = "";
            if (!selectStatement.Select.IsEmpty)
            {
                separator = ", ";
            }
            foreach (Symbol symbol in selectStatement.FromExtents)
            {
                this.AddColumns(selectStatement, symbol, columnList, columnDictionary, ref separator);
            }
            return columnList;
        }

        private void AddFromSymbol(SqlSelectStatement selectStatement, string inputVarName, Symbol fromSymbol)
        {
            this.AddFromSymbol(selectStatement, inputVarName, fromSymbol, true);
        }

        private void AddFromSymbol(SqlSelectStatement selectStatement, string inputVarName, Symbol fromSymbol, bool addToSymbolTable)
        {
            if ((selectStatement.FromExtents.Count == 0) || (fromSymbol != selectStatement.FromExtents[0]))
            {
                selectStatement.FromExtents.Add(fromSymbol);
                selectStatement.From.Append(" AS ");
                selectStatement.From.Append(fromSymbol);
                this.allExtentNames[fromSymbol.Name] = 0;
            }
            if (addToSymbolTable)
            {
                this.symbolTable.Add(inputVarName, fromSymbol);
            }
        }

        private void AddSortKeys(SqlBuilder orderByClause, IList<DbSortClause> sortKeys)
        {
            string s = "";
            foreach (DbSortClause clause in sortKeys)
            {
                orderByClause.Append(s);
                orderByClause.Append(clause.Expression.Accept<ISqlFragment>(this));
                if (!string.IsNullOrEmpty(clause.Collation))
                {
                    orderByClause.Append(" COLLATE ");
                    orderByClause.Append(clause.Collation);
                }
                orderByClause.Append(clause.Ascending ? " ASC" : " DESC");
                s = ", ";
            }
        }

        private void AssertKatmaiOrNewer(DbFunctionExpression e)
        {
            if (this.IsPreKatmai)
            {
                throw EntityUtil.NotSupported(Strings.CanonicalFunctionNotSupportedPriorSql10(e.Function.Name));
            }
        }

        private void AssertKatmaiOrNewer(PrimitiveTypeKind primitiveTypeKind)
        {
            if (this.IsPreKatmai)
            {
                throw EntityUtil.NotSupported(Strings.PrimitiveTypeNotSupportedPriorSql10(primitiveTypeKind));
            }
        }

        private static string ByteArrayToBinaryString(byte[] binaryArray)
        {
            StringBuilder builder = new StringBuilder(binaryArray.Length * 2);
            for (int i = 0; i < binaryArray.Length; i++)
            {
                builder.Append(hexDigits[(binaryArray[i] & 240) >> 4]).Append(hexDigits[binaryArray[i] & 15]);
            }
            return builder.ToString();
        }

        private bool CastReturnTypeToInt32(DbFunctionExpression e)
        {
            if (_functionRequiresReturnTypeCast.Contains(e.Function.FullName))
            {
                for (int i = 0; i < e.Arguments.Count; i++)
                {
                    TypeUsage storeType = this._storeItemCollection.StoreProviderManifest.GetStoreType(e.Arguments[i].ResultType);
                    if (_maxTypeNames.Contains(storeType.EdmType.Name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private SqlSelectStatement CreateNewSelectStatement(SqlSelectStatement oldStatement, string inputVarName, TypeUsage inputVarType, out Symbol fromSymbol) => 
            this.CreateNewSelectStatement(oldStatement, inputVarName, inputVarType, true, out fromSymbol);

        private SqlSelectStatement CreateNewSelectStatement(SqlSelectStatement oldStatement, string inputVarName, TypeUsage inputVarType, bool finalizeOldStatement, out Symbol fromSymbol)
        {
            fromSymbol = null;
            if (finalizeOldStatement && oldStatement.Select.IsEmpty)
            {
                List<Symbol> list = this.AddDefaultColumns(oldStatement);
                JoinSymbol symbol = oldStatement.FromExtents[0] as JoinSymbol;
                if (symbol != null)
                {
                    JoinSymbol symbol2 = new JoinSymbol(inputVarName, inputVarType, symbol.ExtentList) {
                        IsNestedJoin = true,
                        ColumnList = list,
                        FlattenedExtentList = symbol.FlattenedExtentList
                    };
                    fromSymbol = symbol2;
                }
            }
            if (fromSymbol == null)
            {
                if (oldStatement.OutputColumnsRenamed)
                {
                    fromSymbol = new Symbol(inputVarName, inputVarType, oldStatement.OutputColumns);
                }
                else
                {
                    fromSymbol = new Symbol(inputVarName, inputVarType);
                }
            }
            SqlSelectStatement statement = new SqlSelectStatement();
            statement.From.Append("( ");
            statement.From.Append(oldStatement);
            statement.From.AppendLine();
            statement.From.Append(") ");
            return statement;
        }

        private static string EscapeSingleQuote(string s, bool isUnicode) => 
            ((isUnicode ? "N'" : "'") + s.Replace("'", "''") + "'");

        private static string GenerateFunctionSql(DbFunctionCommandTree tree, out CommandType commandType)
        {
            tree.Validate();
            EdmFunction edmFunction = tree.EdmFunction;
            if (string.IsNullOrEmpty(edmFunction.CommandTextAttribute))
            {
                commandType = CommandType.StoredProcedure;
                string name = string.IsNullOrEmpty(edmFunction.Schema) ? edmFunction.NamespaceName : edmFunction.Schema;
                string str2 = string.IsNullOrEmpty(edmFunction.StoreFunctionNameAttribute) ? edmFunction.Name : edmFunction.StoreFunctionNameAttribute;
                string str3 = QuoteIdentifier(name);
                string str4 = QuoteIdentifier(str2);
                return (str3 + "." + str4);
            }
            commandType = CommandType.Text;
            return edmFunction.CommandTextAttribute;
        }

        private string GenerateSql(DbQueryCommandTree tree)
        {
            ISqlFragment fragment;
            tree.Validate();
            DbQueryCommandTree tree2 = tree;
            if ((this.SqlVersion == System.Data.SqlClient.SqlVersion.Sql8) && Sql8ConformanceChecker.NeedsRewrite(tree.Query))
            {
                tree2 = Sql8ExpressionRewriter.Rewrite(tree);
            }
            this.metadataWorkspace = tree2.MetadataWorkspace;
            this._storeItemCollection = (StoreItemCollection) this.Workspace.GetItemCollection(DataSpace.SSpace);
            this.selectStatementStack = new Stack<SqlSelectStatement>();
            this.isParentAJoinStack = new Stack<bool>();
            this.allExtentNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            this.allColumnNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (TypeSemantics.IsCollectionType(tree2.Query.ResultType))
            {
                SqlSelectStatement statement = this.VisitExpressionEnsureSqlStatement(tree2.Query);
                statement.IsTopMost = true;
                fragment = statement;
            }
            else
            {
                SqlBuilder builder = new SqlBuilder();
                builder.Append("SELECT ");
                builder.Append(tree2.Query.Accept<ISqlFragment>(this));
                fragment = builder;
            }
            if (this.isVarRefSingle)
            {
                throw EntityUtil.NotSupported();
            }
            return this.WriteSql(fragment);
        }

        internal static string GenerateSql(DbCommandTree tree, System.Data.SqlClient.SqlVersion sqlVersion, out List<SqlParameter> parameters, out CommandType commandType)
        {
            SqlGenerator generator;
            commandType = CommandType.Text;
            parameters = null;
            switch (tree.CommandTreeKind)
            {
                case DbCommandTreeKind.Query:
                    generator = new SqlGenerator(sqlVersion);
                    return generator.GenerateSql((DbQueryCommandTree) tree);

                case DbCommandTreeKind.Update:
                    return DmlSqlGenerator.GenerateUpdateSql((DbUpdateCommandTree) tree, sqlVersion, out parameters);

                case DbCommandTreeKind.Insert:
                    return DmlSqlGenerator.GenerateInsertSql((DbInsertCommandTree) tree, sqlVersion, out parameters);

                case DbCommandTreeKind.Delete:
                    return DmlSqlGenerator.GenerateDeleteSql((DbDeleteCommandTree) tree, sqlVersion, out parameters);

                case DbCommandTreeKind.Function:
                    generator = new SqlGenerator(sqlVersion);
                    return GenerateFunctionSql((DbFunctionCommandTree) tree, out commandType);
            }
            parameters = null;
            return null;
        }

        private TypeUsage GetPrimitiveType(PrimitiveTypeKind modelType) => 
            TypeUsage.CreateDefaultTypeUsage(this._storeItemCollection.GetMappedPrimitiveType(modelType));

        private string GetSqlPrimitiveType(TypeUsage type)
        {
            TypeUsage storeType = this._storeItemCollection.StoreProviderManifest.GetStoreType(type);
            string name = storeType.EdmType.Name;
            int maxLength = 0;
            byte precision = 0;
            byte scale = 0;
            PrimitiveTypeKind primitiveTypeKind = ((PrimitiveType) storeType.EdmType).PrimitiveTypeKind;
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    if (!TypeHelpers.IsFacetValueConstant(storeType, "MaxLength"))
                    {
                        TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                        name = name + "(" + maxLength.ToString(CultureInfo.InvariantCulture) + ")";
                    }
                    return name;

                case PrimitiveTypeKind.Boolean:
                case PrimitiveTypeKind.Byte:
                    return name;

                case PrimitiveTypeKind.DateTime:
                    return (this.IsPreKatmai ? "datetime" : "datetime2");

                case PrimitiveTypeKind.Decimal:
                    if (!TypeHelpers.IsFacetValueConstant(storeType, "Precision"))
                    {
                        TypeHelpers.TryGetPrecision(storeType, out precision);
                        TypeHelpers.TryGetScale(storeType, out scale);
                        name = string.Concat(new object[] { name, "(", precision, ",", scale, ")" });
                    }
                    return name;

                case PrimitiveTypeKind.String:
                    if (!TypeHelpers.IsFacetValueConstant(storeType, "MaxLength"))
                    {
                        TypeHelpers.TryGetMaxLength(storeType, out maxLength);
                        name = name + "(" + maxLength.ToString(CultureInfo.InvariantCulture) + ")";
                    }
                    return name;

                case PrimitiveTypeKind.Time:
                    this.AssertKatmaiOrNewer(primitiveTypeKind);
                    return "time";

                case PrimitiveTypeKind.DateTimeOffset:
                    this.AssertKatmaiOrNewer(primitiveTypeKind);
                    return "datetimeoffset";
            }
            return name;
        }

        internal static string GetTargetTSql(EntitySetBase entitySetBase)
        {
            if (entitySetBase.CachedProviderSql == null)
            {
                if (entitySetBase.DefiningQuery == null)
                {
                    StringBuilder builder = new StringBuilder(50);
                    if (!string.IsNullOrEmpty(entitySetBase.Schema))
                    {
                        builder.Append(QuoteIdentifier(entitySetBase.Schema));
                        builder.Append(".");
                    }
                    else
                    {
                        builder.Append(QuoteIdentifier(entitySetBase.EntityContainer.Name));
                        builder.Append(".");
                    }
                    if (!string.IsNullOrEmpty(entitySetBase.Table))
                    {
                        builder.Append(QuoteIdentifier(entitySetBase.Table));
                    }
                    else
                    {
                        builder.Append(QuoteIdentifier(entitySetBase.Name));
                    }
                    entitySetBase.CachedProviderSql = builder.ToString();
                }
                else
                {
                    entitySetBase.CachedProviderSql = "(" + entitySetBase.DefiningQuery + ")";
                }
            }
            return entitySetBase.CachedProviderSql;
        }

        private static bool GroupByAggregateNeedsInnerQuery(DbExpression expression, string inputVarRefName) => 
            GroupByExpressionNeedsInnerQuery(expression, inputVarRefName, true);

        private static bool GroupByAggregatesNeedInnerQuery(IList<DbAggregate> aggregates, string inputVarRefName)
        {
            foreach (DbAggregate aggregate in aggregates)
            {
                if (GroupByAggregateNeedsInnerQuery(aggregate.Arguments[0], inputVarRefName))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool GroupByExpressionNeedsInnerQuery(DbExpression expression, string inputVarRefName, bool allowConstants)
        {
            if (allowConstants && (expression.ExpressionKind == DbExpressionKind.Constant))
            {
                return false;
            }
            if (expression.ExpressionKind == DbExpressionKind.Cast)
            {
                DbCastExpression expression2 = (DbCastExpression) expression;
                return GroupByExpressionNeedsInnerQuery(expression2.Argument, inputVarRefName, allowConstants);
            }
            if (expression.ExpressionKind == DbExpressionKind.Property)
            {
                DbPropertyExpression expression3 = (DbPropertyExpression) expression;
                return GroupByExpressionNeedsInnerQuery(expression3.Instance, inputVarRefName, allowConstants);
            }
            if (expression.ExpressionKind == DbExpressionKind.VariableReference)
            {
                DbVariableReferenceExpression expression4 = expression as DbVariableReferenceExpression;
                return !expression4.VariableName.Equals(inputVarRefName);
            }
            return true;
        }

        private static bool GroupByKeyNeedsInnerQuery(DbExpression expression, string inputVarRefName) => 
            GroupByExpressionNeedsInnerQuery(expression, inputVarRefName, false);

        private static bool GroupByKeysNeedInnerQuery(IList<DbExpression> keys, string inputVarRefName)
        {
            foreach (DbExpression expression in keys)
            {
                if (GroupByKeyNeedsInnerQuery(expression, inputVarRefName))
                {
                    return true;
                }
            }
            return false;
        }

        private static ISqlFragment HandleCanonicalFunctionBitwise(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleSpecialFunctionToOperator(e, true);

        private static ISqlFragment HandleCanonicalFunctionCurrentDateTime(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionGivenNameBasedOnVersion(e, "GetDate", "SysDateTime");

        private static ISqlFragment HandleCanonicalFunctionCurrentDateTimeOffset(SqlGenerator sqlgen, DbFunctionExpression e)
        {
            sqlgen.AssertKatmaiOrNewer(e);
            return sqlgen.HandleFunctionDefaultGivenName(e, "SysDateTimeOffset");
        }

        private static ISqlFragment HandleCanonicalFunctionCurrentUtcDateTime(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionGivenNameBasedOnVersion(e, "GetUtcDate", "SysUtcDateTime");

        private static ISqlFragment HandleCanonicalFunctionDatepart(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleCanonicalFunctionDatepart(e.Function.Name.ToLowerInvariant(), e);

        private ISqlFragment HandleCanonicalFunctionDatepart(string datepart, DbFunctionExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("DATEPART (");
            builder.Append(datepart);
            builder.Append(", ");
            builder.Append(e.Arguments[0].Accept<ISqlFragment>(this));
            builder.Append(")");
            return builder;
        }

        private static ISqlFragment HandleCanonicalFunctionGetTotalOffsetMinutes(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleCanonicalFunctionDatepart("tzoffset", e);

        private static ISqlFragment HandleCanonicalFunctionIndexOf(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionDefaultGivenName(e, "CHARINDEX");

        private static ISqlFragment HandleCanonicalFunctionLength(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionDefaultGivenName(e, "LEN");

        private static ISqlFragment HandleCanonicalFunctionNewGuid(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionDefaultGivenName(e, "NEWID");

        private static ISqlFragment HandleCanonicalFunctionRound(SqlGenerator sqlgen, DbFunctionExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("ROUND(");
            builder.Append(e.Arguments[0].Accept<ISqlFragment>(sqlgen));
            builder.Append(", 0)");
            return builder;
        }

        private static ISqlFragment HandleCanonicalFunctionToLower(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionDefaultGivenName(e, "LOWER");

        private static ISqlFragment HandleCanonicalFunctionToUpper(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleFunctionDefaultGivenName(e, "UPPER");

        private static ISqlFragment HandleCanonicalFunctionTrim(SqlGenerator sqlgen, DbFunctionExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("LTRIM(RTRIM(");
            builder.Append(e.Arguments[0].Accept<ISqlFragment>(sqlgen));
            builder.Append("))");
            return builder;
        }

        private static ISqlFragment HandleConcatFunction(SqlGenerator sqlgen, DbFunctionExpression e) => 
            sqlgen.HandleSpecialFunctionToOperator(e, false);

        private ISqlFragment HandleCountExpression(DbExpression e)
        {
            if (e.ExpressionKind == DbExpressionKind.Constant)
            {
                SqlBuilder builder = new SqlBuilder();
                builder.Append(((DbConstantExpression) e).Value.ToString());
                return builder;
            }
            return e.Accept<ISqlFragment>(this);
        }

        private static ISqlFragment HandleDatepartDateFunction(SqlGenerator sqlgen, DbFunctionExpression e)
        {
            DbConstantExpression expression = e.Arguments[0] as DbConstantExpression;
            if (expression == null)
            {
                throw EntityUtil.InvalidOperation(Strings.InvalidDatePartArgumentExpression(e.Function.NamespaceName, e.Function.Name));
            }
            string element = expression.Value as string;
            if (element == null)
            {
                throw EntityUtil.InvalidOperation(Strings.InvalidDatePartArgumentExpression(e.Function.NamespaceName, e.Function.Name));
            }
            SqlBuilder result = new SqlBuilder();
            if (!_datepartKeywords.Contains(element))
            {
                throw EntityUtil.InvalidOperation(Strings.InvalidDatePartArgumentValue(element, e.Function.NamespaceName, e.Function.Name));
            }
            WriteFunctionName(result, e.Function);
            result.Append("(");
            result.Append(element);
            string s = ", ";
            for (int i = 1; i < e.Arguments.Count; i++)
            {
                result.Append(s);
                result.Append(e.Arguments[i].Accept<ISqlFragment>(sqlgen));
            }
            result.Append(")");
            return result;
        }

        private void HandleFunctionArgumentsDefault(DbFunctionExpression e, SqlBuilder result)
        {
            bool niladicFunctionAttribute = e.Function.NiladicFunctionAttribute;
            if (niladicFunctionAttribute && (e.Arguments.Count > 0))
            {
                EntityUtil.Metadata(Strings.NiladicFunctionsCannotHaveParameters);
            }
            if (!niladicFunctionAttribute)
            {
                result.Append("(");
                string s = "";
                foreach (DbExpression expression in e.Arguments)
                {
                    result.Append(s);
                    result.Append(expression.Accept<ISqlFragment>(this));
                    s = ", ";
                }
                result.Append(")");
            }
        }

        private ISqlFragment HandleFunctionDefault(DbFunctionExpression e)
        {
            SqlBuilder result = new SqlBuilder();
            bool flag = this.CastReturnTypeToInt32(e);
            if (flag)
            {
                result.Append(" CAST(");
            }
            WriteFunctionName(result, e.Function);
            this.HandleFunctionArgumentsDefault(e, result);
            if (flag)
            {
                result.Append(" AS int)");
            }
            return result;
        }

        private ISqlFragment HandleFunctionDefaultGivenName(DbFunctionExpression e, string functionName)
        {
            SqlBuilder result = new SqlBuilder();
            bool flag = this.CastReturnTypeToInt32(e);
            if (flag)
            {
                result.Append("CAST(");
            }
            result.Append(functionName);
            this.HandleFunctionArgumentsDefault(e, result);
            if (flag)
            {
                result.Append(" AS int)");
            }
            return result;
        }

        private ISqlFragment HandleFunctionGivenNameBasedOnVersion(DbFunctionExpression e, string preKatmaiName, string katmaiName)
        {
            if (this.IsPreKatmai)
            {
                return this.HandleFunctionDefaultGivenName(e, preKatmaiName);
            }
            return this.HandleFunctionDefaultGivenName(e, katmaiName);
        }

        private ISqlFragment HandleSpecialCanonicalFunction(DbFunctionExpression e) => 
            this.HandleSpecialFunction(_canonicalFunctionHandlers, e);

        private ISqlFragment HandleSpecialFunction(Dictionary<string, FunctionHandler> handlers, DbFunctionExpression e) => 
            handlers[e.Function.Name](this, e);

        private ISqlFragment HandleSpecialFunctionToOperator(DbFunctionExpression e, bool parenthesiseArguments)
        {
            SqlBuilder builder = new SqlBuilder();
            if (e.Arguments.Count > 1)
            {
                if (parenthesiseArguments)
                {
                    builder.Append("(");
                }
                builder.Append(e.Arguments[0].Accept<ISqlFragment>(this));
                if (parenthesiseArguments)
                {
                    builder.Append(")");
                }
            }
            builder.Append(" ");
            builder.Append(_functionNameToOperatorDictionary[e.Function.Name]);
            builder.Append(" ");
            if (parenthesiseArguments)
            {
                builder.Append("(");
            }
            builder.Append(e.Arguments[e.Arguments.Count - 1].Accept<ISqlFragment>(this));
            if (parenthesiseArguments)
            {
                builder.Append(")");
            }
            return builder;
        }

        private ISqlFragment HandleSpecialStoreFunction(DbFunctionExpression e) => 
            this.HandleSpecialFunction(_storeFunctionHandlers, e);

        private static Dictionary<string, FunctionHandler> InitializeCanonicalFunctionHandlers() => 
            new Dictionary<string, FunctionHandler>(0x10, StringComparer.Ordinal) { 
                { 
                    "IndexOf",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionIndexOf)
                },
                { 
                    "Length",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionLength)
                },
                { 
                    "NewGuid",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionNewGuid)
                },
                { 
                    "Round",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionRound)
                },
                { 
                    "ToLower",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionToLower)
                },
                { 
                    "ToUpper",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionToUpper)
                },
                { 
                    "Trim",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionTrim)
                },
                { 
                    "Year",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Month",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Day",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Hour",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Minute",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Second",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "Millisecond",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
                },
                { 
                    "CurrentDateTime",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
                },
                { 
                    "CurrentUtcDateTime",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentUtcDateTime)
                },
                { 
                    "CurrentDateTimeOffset",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTimeOffset)
                },
                { 
                    "GetTotalOffsetMinutes",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionGetTotalOffsetMinutes)
                },
                { 
                    "Concat",
                    new FunctionHandler(SqlGenerator.HandleConcatFunction)
                },
                { 
                    "BitwiseAnd",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
                },
                { 
                    "BitwiseNot",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
                },
                { 
                    "BitwiseOr",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
                },
                { 
                    "BitwiseXor",
                    new FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
                }
            };

        private static Dictionary<string, string> InitializeFunctionNameToOperatorDictionary() => 
            new Dictionary<string, string>(5, StringComparer.Ordinal) { 
                { 
                    "Concat",
                    "+"
                },
                { 
                    "CONCAT",
                    "+"
                },
                { 
                    "BitwiseAnd",
                    "&"
                },
                { 
                    "BitwiseNot",
                    "~"
                },
                { 
                    "BitwiseOr",
                    "|"
                },
                { 
                    "BitwiseXor",
                    "^"
                }
            };

        private static Dictionary<string, FunctionHandler> InitializeStoreFunctionHandlers() => 
            new Dictionary<string, FunctionHandler>(5, StringComparer.Ordinal) { 
                { 
                    "CONCAT",
                    new FunctionHandler(SqlGenerator.HandleConcatFunction)
                },
                { 
                    "DATEADD",
                    new FunctionHandler(SqlGenerator.HandleDatepartDateFunction)
                },
                { 
                    "DATEDIFF",
                    new FunctionHandler(SqlGenerator.HandleDatepartDateFunction)
                },
                { 
                    "DATENAME",
                    new FunctionHandler(SqlGenerator.HandleDatepartDateFunction)
                },
                { 
                    "DATEPART",
                    new FunctionHandler(SqlGenerator.HandleDatepartDateFunction)
                }
            };

        private static bool IsApplyExpression(DbExpression e)
        {
            if (DbExpressionKind.CrossApply != e.ExpressionKind)
            {
                return (DbExpressionKind.OuterApply == e.ExpressionKind);
            }
            return true;
        }

        private static bool IsCompatible(SqlSelectStatement result, DbExpressionKind expressionKind)
        {
            switch (expressionKind)
            {
                case DbExpressionKind.Skip:
                    if ((!result.Select.IsEmpty || !result.GroupBy.IsEmpty) || !result.OrderBy.IsEmpty)
                    {
                        return false;
                    }
                    return !result.IsDistinct;

                case DbExpressionKind.Sort:
                    if ((!result.Select.IsEmpty || !result.GroupBy.IsEmpty) || !result.OrderBy.IsEmpty)
                    {
                        return false;
                    }
                    return !result.IsDistinct;

                case DbExpressionKind.Project:
                    return ((result.Select.IsEmpty && result.GroupBy.IsEmpty) && !result.IsDistinct);

                case DbExpressionKind.Limit:
                case DbExpressionKind.Element:
                    return (result.Top == null);

                case DbExpressionKind.Distinct:
                    if (result.Top != null)
                    {
                        return false;
                    }
                    return result.OrderBy.IsEmpty;

                case DbExpressionKind.Filter:
                    return (((result.Select.IsEmpty && result.Where.IsEmpty) && result.GroupBy.IsEmpty) && (result.Top == null));

                case DbExpressionKind.GroupBy:
                    return (((result.Select.IsEmpty && result.GroupBy.IsEmpty) && result.OrderBy.IsEmpty) && (result.Top == null));
            }
            throw EntityUtil.InvalidOperation(string.Empty);
        }

        private static bool IsComplexExpression(DbExpression e)
        {
            DbExpressionKind expressionKind = e.ExpressionKind;
            return (((expressionKind != DbExpressionKind.Constant) && (expressionKind != DbExpressionKind.ParameterReference)) && (expressionKind != DbExpressionKind.Property));
        }

        private static bool IsJoinExpression(DbExpression e)
        {
            if (((DbExpressionKind.CrossJoin != e.ExpressionKind) && (DbExpressionKind.FullOuterJoin != e.ExpressionKind)) && (DbExpressionKind.InnerJoin != e.ExpressionKind))
            {
                return (DbExpressionKind.LeftOuterJoin == e.ExpressionKind);
            }
            return true;
        }

        private static bool IsSpecialCanonicalFunction(DbFunctionExpression e) => 
            (TypeHelpers.IsCanonicalFunction(e.Function) && _canonicalFunctionHandlers.ContainsKey(e.Function.Name));

        private static bool IsSpecialStoreFunction(DbFunctionExpression e) => 
            (IsStoreFunction(e.Function) && _storeFunctionHandlers.ContainsKey(e.Function.Name));

        private static bool IsStoreFunction(EdmFunction function) => 
            (function.BuiltInAttribute && !TypeHelpers.IsCanonicalFunction(function));

        private void ParanthesizeExpressionIfNeeded(DbExpression e, SqlBuilder result)
        {
            if (IsComplexExpression(e))
            {
                result.Append("(");
                result.Append(e.Accept<ISqlFragment>(this));
                result.Append(")");
            }
            else
            {
                result.Append(e.Accept<ISqlFragment>(this));
            }
        }

        private void ProcessJoinInputResult(ISqlFragment fromExtentFragment, SqlSelectStatement result, DbExpressionBinding input, int fromSymbolStart)
        {
            Symbol fromSymbol = null;
            if (result != fromExtentFragment)
            {
                SqlSelectStatement selectStatement = fromExtentFragment as SqlSelectStatement;
                if (selectStatement != null)
                {
                    if (selectStatement.Select.IsEmpty)
                    {
                        List<Symbol> list = this.AddDefaultColumns(selectStatement);
                        if (IsJoinExpression(input.Expression) || IsApplyExpression(input.Expression))
                        {
                            List<Symbol> fromExtents = selectStatement.FromExtents;
                            JoinSymbol symbol2 = new JoinSymbol(input.VariableName, input.VariableType, fromExtents) {
                                IsNestedJoin = true,
                                ColumnList = list
                            };
                            fromSymbol = symbol2;
                        }
                        else
                        {
                            JoinSymbol symbol3 = selectStatement.FromExtents[0] as JoinSymbol;
                            if (symbol3 != null)
                            {
                                JoinSymbol symbol4 = new JoinSymbol(input.VariableName, input.VariableType, symbol3.ExtentList) {
                                    IsNestedJoin = true,
                                    ColumnList = list,
                                    FlattenedExtentList = symbol3.FlattenedExtentList
                                };
                                fromSymbol = symbol4;
                            }
                            else if (selectStatement.FromExtents[0].OutputColumnsRenamed)
                            {
                                fromSymbol = new Symbol(input.VariableName, input.VariableType, selectStatement.FromExtents[0].Columns);
                            }
                        }
                    }
                    else if (selectStatement.OutputColumnsRenamed)
                    {
                        fromSymbol = new Symbol(input.VariableName, input.VariableType, selectStatement.OutputColumns);
                    }
                    result.From.Append(" (");
                    result.From.Append(selectStatement);
                    result.From.Append(" )");
                }
                else if (input.Expression is DbScanExpression)
                {
                    result.From.Append(fromExtentFragment);
                }
                else
                {
                    WrapNonQueryExtent(result, fromExtentFragment, input.Expression.ExpressionKind);
                }
                if (fromSymbol == null)
                {
                    fromSymbol = new Symbol(input.VariableName, input.VariableType);
                }
                this.AddFromSymbol(result, input.VariableName, fromSymbol);
                result.AllJoinExtents.Add(fromSymbol);
            }
            else
            {
                List<Symbol> extents = new List<Symbol>();
                for (int i = fromSymbolStart; i < result.FromExtents.Count; i++)
                {
                    extents.Add(result.FromExtents[i]);
                }
                result.FromExtents.RemoveRange(fromSymbolStart, result.FromExtents.Count - fromSymbolStart);
                fromSymbol = new JoinSymbol(input.VariableName, input.VariableType, extents);
                result.FromExtents.Add(fromSymbol);
                this.symbolTable.Add(input.VariableName, fromSymbol);
            }
        }

        internal static string QuoteIdentifier(string name) => 
            ("[" + name.Replace("]", "]]") + "]");

        public override ISqlFragment Visit(DbAndExpression e) => 
            this.VisitBinaryExpression(" AND ", DbExpressionKind.And, e.Left, e.Right);

        public override ISqlFragment Visit(DbApplyExpression e)
        {
            string str;
            List<DbExpressionBinding> inputs = new List<DbExpressionBinding> {
                e.Input,
                e.Apply
            };
            DbExpressionKind expressionKind = e.ExpressionKind;
            if (expressionKind != DbExpressionKind.CrossApply)
            {
                if (expressionKind != DbExpressionKind.OuterApply)
                {
                    throw EntityUtil.InvalidOperation(string.Empty);
                }
            }
            else
            {
                str = "CROSS APPLY";
                goto Label_004B;
            }
            str = "OUTER APPLY";
        Label_004B:
            return this.VisitJoinExpression(inputs, DbExpressionKind.CrossJoin, str, null);
        }

        public override ISqlFragment Visit(DbArithmeticExpression e)
        {
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.Plus:
                    return this.VisitBinaryExpression(" + ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);

                case DbExpressionKind.UnaryMinus:
                {
                    SqlBuilder builder = new SqlBuilder();
                    builder.Append(" -(");
                    builder.Append(e.Arguments[0].Accept<ISqlFragment>(this));
                    builder.Append(")");
                    return builder;
                }
                case DbExpressionKind.Minus:
                    return this.VisitBinaryExpression(" - ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);

                case DbExpressionKind.Modulo:
                    return this.VisitBinaryExpression(" % ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);

                case DbExpressionKind.Multiply:
                    return this.VisitBinaryExpression(" * ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);

                case DbExpressionKind.Divide:
                    return this.VisitBinaryExpression(" / ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);
            }
            throw EntityUtil.InvalidOperation(string.Empty);
        }

        public override ISqlFragment Visit(DbCaseExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("CASE");
            for (int i = 0; i < e.When.Count; i++)
            {
                builder.Append(" WHEN (");
                builder.Append(e.When[i].Accept<ISqlFragment>(this));
                builder.Append(") THEN ");
                builder.Append(e.Then[i].Accept<ISqlFragment>(this));
            }
            if ((e.Else != null) && !(e.Else is DbNullExpression))
            {
                builder.Append(" ELSE ");
                builder.Append(e.Else.Accept<ISqlFragment>(this));
            }
            builder.Append(" END");
            return builder;
        }

        public override ISqlFragment Visit(DbCastExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append(" CAST( ");
            builder.Append(e.Argument.Accept<ISqlFragment>(this));
            builder.Append(" AS ");
            builder.Append(this.GetSqlPrimitiveType(e.ResultType));
            builder.Append(")");
            return builder;
        }

        public override ISqlFragment Visit(DbComparisonExpression e)
        {
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.LessThan:
                    return this.VisitComparisonExpression(" < ", e.Left, e.Right);

                case DbExpressionKind.LessThanOrEquals:
                    return this.VisitComparisonExpression(" <= ", e.Left, e.Right);

                case DbExpressionKind.NotEquals:
                    return this.VisitComparisonExpression(" <> ", e.Left, e.Right);

                case DbExpressionKind.GreaterThan:
                    return this.VisitComparisonExpression(" > ", e.Left, e.Right);

                case DbExpressionKind.GreaterThanOrEquals:
                    return this.VisitComparisonExpression(" >= ", e.Left, e.Right);

                case DbExpressionKind.Equals:
                    return this.VisitComparisonExpression(" = ", e.Left, e.Right);
            }
            throw EntityUtil.InvalidOperation(string.Empty);
        }

        public override ISqlFragment Visit(DbConstantExpression e) => 
            this.VisitConstant(e, false);

        public override ISqlFragment Visit(DbCrossJoinExpression e) => 
            this.VisitJoinExpression(e.Inputs, e.ExpressionKind, "CROSS JOIN", null);

        public override ISqlFragment Visit(DbDerefExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbDistinctExpression e)
        {
            SqlSelectStatement result = this.VisitExpressionEnsureSqlStatement(e.Argument);
            if (!IsCompatible(result, e.ExpressionKind))
            {
                Symbol symbol;
                TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(e.Argument.ResultType);
                result = this.CreateNewSelectStatement(result, "distinct", elementTypeUsage, out symbol);
                this.AddFromSymbol(result, "distinct", symbol, false);
            }
            result.IsDistinct = true;
            return result;
        }

        public override ISqlFragment Visit(DbElementExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("(");
            builder.Append(this.VisitExpressionEnsureSqlStatement(e.Argument));
            builder.Append(")");
            return builder;
        }

        public override ISqlFragment Visit(DbEntityRefExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbExceptExpression e) => 
            this.VisitSetOpExpression(e.Left, e.Right, "EXCEPT");

        public override ISqlFragment Visit(DbExpression e)
        {
            throw EntityUtil.InvalidOperation(string.Empty);
        }

        public override ISqlFragment Visit(DbFilterExpression e) => 
            this.VisitFilterExpression(e.Input, e.Predicate, false);

        public override ISqlFragment Visit(DbFunctionExpression e)
        {
            if (e.IsLambda)
            {
                throw EntityUtil.NotSupported();
            }
            if (IsSpecialCanonicalFunction(e))
            {
                return this.HandleSpecialCanonicalFunction(e);
            }
            if (IsSpecialStoreFunction(e))
            {
                return this.HandleSpecialStoreFunction(e);
            }
            return this.HandleFunctionDefault(e);
        }

        public override ISqlFragment Visit(DbGroupByExpression e)
        {
            Symbol symbol;
            SqlSelectStatement statement2;
            SqlSelectStatement result = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out symbol);
            if (!IsCompatible(result, e.ExpressionKind))
            {
                result = this.CreateNewSelectStatement(result, e.Input.VariableName, e.Input.VariableType, out symbol);
            }
            this.selectStatementStack.Push(result);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(result, e.Input.VariableName, symbol);
            this.symbolTable.Add(e.Input.GroupVariableName, symbol);
            RowType edmType = TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);
            bool flag = GroupByAggregatesNeedInnerQuery(e.Aggregates, e.Input.GroupVariableName) || GroupByKeysNeedInnerQuery(e.Keys, e.Input.VariableName);
            if (flag)
            {
                statement2 = this.CreateNewSelectStatement(result, e.Input.VariableName, e.Input.VariableType, false, out symbol);
                this.AddFromSymbol(statement2, e.Input.VariableName, symbol, false);
            }
            else
            {
                statement2 = result;
            }
            using (IEnumerator<EdmProperty> enumerator = edmType.Properties.GetEnumerator())
            {
                enumerator.MoveNext();
                string s = "";
                foreach (DbExpression expression in e.Keys)
                {
                    EdmProperty current = enumerator.Current;
                    string str2 = QuoteIdentifier(current.Name);
                    statement2.GroupBy.Append(s);
                    ISqlFragment fragment = expression.Accept<ISqlFragment>(this);
                    if (!flag)
                    {
                        statement2.Select.Append(s);
                        statement2.Select.AppendLine();
                        statement2.Select.Append(fragment);
                        statement2.Select.Append(" AS ");
                        statement2.Select.Append(str2);
                        statement2.GroupBy.Append(fragment);
                    }
                    else
                    {
                        result.Select.Append(s);
                        result.Select.AppendLine();
                        result.Select.Append(fragment);
                        result.Select.Append(" AS ");
                        result.Select.Append(str2);
                        statement2.Select.Append(s);
                        statement2.Select.AppendLine();
                        statement2.Select.Append(symbol);
                        statement2.Select.Append(".");
                        statement2.Select.Append(str2);
                        statement2.Select.Append(" AS ");
                        statement2.Select.Append(str2);
                        statement2.GroupBy.Append(str2);
                    }
                    s = ", ";
                    enumerator.MoveNext();
                }
                foreach (DbAggregate aggregate in e.Aggregates)
                {
                    object obj2;
                    EdmProperty property2 = enumerator.Current;
                    string str3 = QuoteIdentifier(property2.Name);
                    ISqlFragment fragment2 = aggregate.Arguments[0].Accept<ISqlFragment>(this);
                    if (flag)
                    {
                        SqlBuilder builder = new SqlBuilder();
                        builder.Append(symbol);
                        builder.Append(".");
                        builder.Append(str3);
                        obj2 = builder;
                        result.Select.Append(s);
                        result.Select.AppendLine();
                        result.Select.Append(fragment2);
                        result.Select.Append(" AS ");
                        result.Select.Append(str3);
                    }
                    else
                    {
                        obj2 = fragment2;
                    }
                    ISqlFragment fragment3 = VisitAggregate(aggregate, obj2);
                    statement2.Select.Append(s);
                    statement2.Select.AppendLine();
                    statement2.Select.Append(fragment3);
                    statement2.Select.Append(" AS ");
                    statement2.Select.Append(str3);
                    s = ", ";
                    enumerator.MoveNext();
                }
            }
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            return statement2;
        }

        public override ISqlFragment Visit(DbIntersectExpression e) => 
            this.VisitSetOpExpression(e.Left, e.Right, "INTERSECT");

        public override ISqlFragment Visit(DbIsEmptyExpression e) => 
            this.VisitIsEmptyExpression(e, false);

        public override ISqlFragment Visit(DbIsNullExpression e) => 
            this.VisitIsNullExpression(e, false);

        public override ISqlFragment Visit(DbIsOfExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbJoinExpression e)
        {
            string str;
            DbExpressionKind expressionKind = e.ExpressionKind;
            if (expressionKind == DbExpressionKind.FullOuterJoin)
            {
                str = "FULL OUTER JOIN";
            }
            else if (expressionKind == DbExpressionKind.InnerJoin)
            {
                str = "INNER JOIN";
            }
            else if (expressionKind == DbExpressionKind.LeftOuterJoin)
            {
                str = "LEFT OUTER JOIN";
            }
            else
            {
                str = null;
            }
            List<DbExpressionBinding> inputs = new List<DbExpressionBinding>(2) {
                e.Left,
                e.Right
            };
            return this.VisitJoinExpression(inputs, e.ExpressionKind, str, e.JoinCondition);
        }

        public override ISqlFragment Visit(DbLikeExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append(e.Argument.Accept<ISqlFragment>(this));
            builder.Append(" LIKE ");
            builder.Append(e.Pattern.Accept<ISqlFragment>(this));
            if (e.Escape.ExpressionKind != DbExpressionKind.Null)
            {
                builder.Append(" ESCAPE ");
                builder.Append(e.Escape.Accept<ISqlFragment>(this));
            }
            return builder;
        }

        public override ISqlFragment Visit(DbLimitExpression e)
        {
            SqlSelectStatement result = this.VisitExpressionEnsureSqlStatement(e.Argument, false);
            if (!IsCompatible(result, e.ExpressionKind))
            {
                Symbol symbol;
                TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(e.Argument.ResultType);
                result = this.CreateNewSelectStatement(result, "top", elementTypeUsage, out symbol);
                this.AddFromSymbol(result, "top", symbol, false);
            }
            ISqlFragment topCount = this.HandleCountExpression(e.Limit);
            result.Top = new TopClause(topCount, e.WithTies);
            return result;
        }

        public override ISqlFragment Visit(DbNewInstanceExpression e)
        {
            if (!TypeSemantics.IsCollectionType(e.ResultType))
            {
                throw EntityUtil.NotSupported();
            }
            return this.VisitCollectionConstructor(e);
        }

        public override ISqlFragment Visit(DbNotExpression e)
        {
            DbNotExpression argument = e.Argument as DbNotExpression;
            if (argument != null)
            {
                return argument.Argument.Accept<ISqlFragment>(this);
            }
            DbIsEmptyExpression expression2 = e.Argument as DbIsEmptyExpression;
            if (expression2 != null)
            {
                return this.VisitIsEmptyExpression(expression2, true);
            }
            DbIsNullExpression expression3 = e.Argument as DbIsNullExpression;
            if (expression3 != null)
            {
                return this.VisitIsNullExpression(expression3, true);
            }
            DbComparisonExpression expression4 = e.Argument as DbComparisonExpression;
            if ((expression4 != null) && (expression4.ExpressionKind == DbExpressionKind.Equals))
            {
                return this.VisitBinaryExpression(" <> ", DbExpressionKind.NotEquals, expression4.Left, expression4.Right);
            }
            SqlBuilder builder = new SqlBuilder();
            builder.Append(" NOT (");
            builder.Append(e.Argument.Accept<ISqlFragment>(this));
            builder.Append(")");
            return builder;
        }

        public override ISqlFragment Visit(DbNullExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("CAST(NULL AS ");
            TypeUsage resultType = e.ResultType;
            PrimitiveType edmType = resultType.EdmType as PrimitiveType;
            PrimitiveTypeKind primitiveTypeKind = edmType.PrimitiveTypeKind;
            if (primitiveTypeKind != PrimitiveTypeKind.Binary)
            {
                if (primitiveTypeKind != PrimitiveTypeKind.String)
                {
                    builder.Append(this.GetSqlPrimitiveType(resultType));
                }
                else
                {
                    builder.Append("varchar(1)");
                }
            }
            else
            {
                builder.Append("varbinary(1)");
            }
            builder.Append(")");
            return builder;
        }

        public override ISqlFragment Visit(DbOfTypeExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbOrExpression e) => 
            this.VisitBinaryExpression(" OR ", e.ExpressionKind, e.Left, e.Right);

        public override ISqlFragment Visit(DbParameterReferenceExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append("@" + e.ParameterName);
            return builder;
        }

        public override ISqlFragment Visit(DbProjectExpression e)
        {
            Symbol symbol;
            SqlSelectStatement result = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out symbol);
            bool aliasesNeedRenaming = false;
            if (!IsCompatible(result, e.ExpressionKind))
            {
                result = this.CreateNewSelectStatement(result, e.Input.VariableName, e.Input.VariableType, out symbol);
            }
            else if ((this.SqlVersion == System.Data.SqlClient.SqlVersion.Sql8) && !result.OrderBy.IsEmpty)
            {
                aliasesNeedRenaming = true;
            }
            this.selectStatementStack.Push(result);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(result, e.Input.VariableName, symbol);
            DbNewInstanceExpression projection = e.Projection as DbNewInstanceExpression;
            if (projection != null)
            {
                Dictionary<string, Symbol> dictionary;
                result.Select.Append(this.VisitNewInstanceExpression(projection, aliasesNeedRenaming, out dictionary));
                if (aliasesNeedRenaming)
                {
                    result.OutputColumnsRenamed = true;
                    result.OutputColumns = dictionary;
                }
            }
            else
            {
                result.Select.Append(e.Projection.Accept<ISqlFragment>(this));
            }
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            return result;
        }

        public override ISqlFragment Visit(DbPropertyExpression e)
        {
            SqlBuilder builder;
            ISqlFragment s = e.Instance.Accept<ISqlFragment>(this);
            if (e.Instance is DbVariableReferenceExpression)
            {
                this.isVarRefSingle = false;
            }
            JoinSymbol source = s as JoinSymbol;
            if (source != null)
            {
                if (source.IsNestedJoin)
                {
                    return new SymbolPair(source, source.NameToExtent[e.Property.Name]);
                }
                return source.NameToExtent[e.Property.Name];
            }
            SymbolPair pair = s as SymbolPair;
            if (pair != null)
            {
                JoinSymbol column = pair.Column as JoinSymbol;
                if (column != null)
                {
                    pair.Column = column.NameToExtent[e.Property.Name];
                    return pair;
                }
                if (pair.Column.Columns.ContainsKey(e.Property.Name))
                {
                    builder = new SqlBuilder();
                    builder.Append(pair.Source);
                    builder.Append(".");
                    builder.Append(pair.Column.Columns[e.Property.Name]);
                    return builder;
                }
            }
            builder = new SqlBuilder();
            builder.Append(s);
            builder.Append(".");
            Symbol symbol3 = s as Symbol;
            if ((symbol3 != null) && symbol3.OutputColumnsRenamed)
            {
                builder.Append(symbol3.Columns[e.Property.Name]);
                return builder;
            }
            builder.Append(QuoteIdentifier(e.Property.Name));
            return builder;
        }

        public override ISqlFragment Visit(DbQuantifierExpression e)
        {
            SqlBuilder builder = new SqlBuilder();
            bool negatePredicate = e.ExpressionKind == DbExpressionKind.All;
            if (e.ExpressionKind == DbExpressionKind.Any)
            {
                builder.Append("EXISTS (");
            }
            else
            {
                builder.Append("NOT EXISTS (");
            }
            SqlSelectStatement selectStatement = this.VisitFilterExpression(e.Input, e.Predicate, negatePredicate);
            if (selectStatement.Select.IsEmpty)
            {
                this.AddDefaultColumns(selectStatement);
            }
            builder.Append(selectStatement);
            builder.Append(")");
            return builder;
        }

        public override ISqlFragment Visit(DbRefExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbRefKeyExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbRelationshipNavigationExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbScanExpression e)
        {
            EntitySetBase target = e.Target;
            if (this.IsParentAJoin)
            {
                SqlBuilder builder = new SqlBuilder();
                builder.Append(GetTargetTSql(target));
                return builder;
            }
            SqlSelectStatement statement = new SqlSelectStatement();
            statement.From.Append(GetTargetTSql(target));
            return statement;
        }

        public override ISqlFragment Visit(DbSkipExpression e)
        {
            Symbol symbol;
            SqlSelectStatement result = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out symbol);
            if (!IsCompatible(result, e.ExpressionKind))
            {
                result = this.CreateNewSelectStatement(result, e.Input.VariableName, e.Input.VariableType, out symbol);
            }
            this.selectStatementStack.Push(result);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(result, e.Input.VariableName, symbol);
            List<Symbol> list = this.AddDefaultColumns(result);
            result.Select.Append(", row_number() OVER (ORDER BY ");
            this.AddSortKeys(result.Select, e.SortOrder);
            result.Select.Append(") AS ");
            Symbol s = new Symbol("row_number", this.IntegerType);
            result.Select.Append(s);
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            SqlSelectStatement item = new SqlSelectStatement();
            item.From.Append("( ");
            item.From.Append(result);
            item.From.AppendLine();
            item.From.Append(") ");
            Symbol fromSymbol = null;
            if (result.FromExtents.Count == 1)
            {
                JoinSymbol symbol4 = result.FromExtents[0] as JoinSymbol;
                if (symbol4 != null)
                {
                    JoinSymbol symbol5 = new JoinSymbol(e.Input.VariableName, e.Input.VariableType, symbol4.ExtentList) {
                        IsNestedJoin = true,
                        ColumnList = list,
                        FlattenedExtentList = symbol4.FlattenedExtentList
                    };
                    fromSymbol = symbol5;
                }
            }
            if (fromSymbol == null)
            {
                fromSymbol = new Symbol(e.Input.VariableName, e.Input.VariableType);
            }
            this.selectStatementStack.Push(item);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(item, e.Input.VariableName, fromSymbol);
            item.Where.Append(fromSymbol);
            item.Where.Append(".");
            item.Where.Append(s);
            item.Where.Append(" > ");
            item.Where.Append(this.HandleCountExpression(e.Count));
            this.AddSortKeys(item.OrderBy, e.SortOrder);
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            return item;
        }

        public override ISqlFragment Visit(DbSortExpression e)
        {
            Symbol symbol;
            SqlSelectStatement result = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out symbol);
            if (!IsCompatible(result, e.ExpressionKind))
            {
                result = this.CreateNewSelectStatement(result, e.Input.VariableName, e.Input.VariableType, out symbol);
            }
            this.selectStatementStack.Push(result);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(result, e.Input.VariableName, symbol);
            this.AddSortKeys(result.OrderBy, e.SortOrder);
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            return result;
        }

        public override ISqlFragment Visit(DbTreatExpression e)
        {
            throw EntityUtil.NotSupported();
        }

        public override ISqlFragment Visit(DbUnionAllExpression e) => 
            this.VisitSetOpExpression(e.Left, e.Right, "UNION ALL");

        public override ISqlFragment Visit(DbVariableReferenceExpression e)
        {
            if (this.isVarRefSingle)
            {
                throw EntityUtil.NotSupported();
            }
            this.isVarRefSingle = true;
            Symbol item = this.symbolTable.Lookup(e.VariableName);
            if (!this.CurrentSelectStatement.FromExtents.Contains(item))
            {
                this.CurrentSelectStatement.OuterExtents[item] = true;
            }
            return item;
        }

        private static SqlBuilder VisitAggregate(DbAggregate aggregate, object aggregateArgument)
        {
            SqlBuilder result = new SqlBuilder();
            DbFunctionAggregate aggregate2 = aggregate as DbFunctionAggregate;
            if (aggregate2 == null)
            {
                throw EntityUtil.NotSupported();
            }
            if (TypeHelpers.IsCanonicalFunction(aggregate2.Function) && string.Equals(aggregate2.Function.Name, "BigCount", StringComparison.Ordinal))
            {
                result.Append("COUNT_BIG");
            }
            else
            {
                WriteFunctionName(result, aggregate2.Function);
            }
            result.Append("(");
            DbFunctionAggregate aggregate3 = aggregate2;
            if ((aggregate3 != null) && aggregate3.Distinct)
            {
                result.Append("DISTINCT ");
            }
            result.Append(aggregateArgument);
            result.Append(")");
            return result;
        }

        private SqlBuilder VisitBinaryExpression(string op, DbExpressionKind expressionKind, DbExpression left, DbExpression right)
        {
            SqlBuilder result = new SqlBuilder();
            bool flag = true;
            foreach (DbExpression expression in CommandTreeUtils.FlattenAssociativeExpression(expressionKind, new DbExpression[] { left, right }))
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    result.Append(op);
                }
                this.ParanthesizeExpressionIfNeeded(expression, result);
            }
            return result;
        }

        private ISqlFragment VisitCollectionConstructor(DbNewInstanceExpression e)
        {
            if ((e.Arguments.Count == 1) && (e.Arguments[0].ExpressionKind == DbExpressionKind.Element))
            {
                DbElementExpression expression = e.Arguments[0] as DbElementExpression;
                SqlSelectStatement result = this.VisitExpressionEnsureSqlStatement(expression.Argument);
                if (!IsCompatible(result, DbExpressionKind.Element))
                {
                    Symbol symbol;
                    TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(expression.Argument.ResultType);
                    result = this.CreateNewSelectStatement(result, "element", elementTypeUsage, out symbol);
                    this.AddFromSymbol(result, "element", symbol, false);
                }
                result.Top = new TopClause(1, false);
                return result;
            }
            CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(e.ResultType);
            bool flag = TypeSemantics.IsPrimitiveType(edmType.TypeUsage);
            SqlBuilder builder = new SqlBuilder();
            string s = "";
            if (e.Arguments.Count == 0)
            {
                builder.Append(" SELECT CAST(null as ");
                builder.Append(this.GetSqlPrimitiveType(edmType.TypeUsage));
                builder.Append(") AS X FROM (SELECT 1) AS Y WHERE 1=0");
            }
            foreach (DbExpression expression2 in e.Arguments)
            {
                builder.Append(s);
                builder.Append(" SELECT ");
                builder.Append(expression2.Accept<ISqlFragment>(this));
                if (flag)
                {
                    builder.Append(" AS X ");
                }
                s = " UNION ALL ";
            }
            return builder;
        }

        private SqlBuilder VisitComparisonExpression(string op, DbExpression left, DbExpression right)
        {
            SqlBuilder result = new SqlBuilder();
            bool isCastOptional = left.ResultType.EdmType == right.ResultType.EdmType;
            if (left.ExpressionKind == DbExpressionKind.Constant)
            {
                result.Append(this.VisitConstant((DbConstantExpression) left, isCastOptional));
            }
            else
            {
                this.ParanthesizeExpressionIfNeeded(left, result);
            }
            result.Append(op);
            if (right.ExpressionKind == DbExpressionKind.Constant)
            {
                result.Append(this.VisitConstant((DbConstantExpression) right, isCastOptional));
                return result;
            }
            this.ParanthesizeExpressionIfNeeded(right, result);
            return result;
        }

        private ISqlFragment VisitConstant(DbConstantExpression e, bool isCastOptional)
        {
            PrimitiveTypeKind kind;
            SqlBuilder result = new SqlBuilder();
            if (!TypeHelpers.TryGetPrimitiveTypeKind(e.ResultType, out kind))
            {
                throw EntityUtil.NotSupported();
            }
            switch (kind)
            {
                case PrimitiveTypeKind.Binary:
                    result.Append(" 0x");
                    result.Append(ByteArrayToBinaryString((byte[]) e.Value));
                    result.Append(" ");
                    return result;

                case PrimitiveTypeKind.Boolean:
                    WrapWithCastIfNeeded(!isCastOptional, ((bool) e.Value) ? "1" : "0", "bit", result);
                    return result;

                case PrimitiveTypeKind.Byte:
                    WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "tinyint", result);
                    return result;

                case PrimitiveTypeKind.DateTime:
                {
                    result.Append("convert(");
                    result.Append(this.IsPreKatmai ? "datetime" : "datetime2");
                    result.Append(", ");
                    DateTime time = (DateTime) e.Value;
                    result.Append(EscapeSingleQuote(time.ToString(this.IsPreKatmai ? "yyyy-MM-dd HH:mm:ss.fff" : "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture), false));
                    result.Append(", 121)");
                    return result;
                }
                case PrimitiveTypeKind.Decimal:
                {
                    string str = ((decimal) e.Value).ToString(CultureInfo.InvariantCulture);
                    bool cast = (-1 == str.IndexOf('.')) && (str.TrimStart(new char[] { '-' }).Length < 20);
                    string typeName = "decimal(" + Math.Max((byte) str.Length, 0x12).ToString(CultureInfo.InvariantCulture) + ")";
                    WrapWithCastIfNeeded(cast, str, typeName, result);
                    return result;
                }
                case PrimitiveTypeKind.Double:
                    WrapWithCastIfNeeded(true, ((double) e.Value).ToString("R", CultureInfo.InvariantCulture), "float(53)", result);
                    return result;

                case PrimitiveTypeKind.Guid:
                    WrapWithCastIfNeeded(true, EscapeSingleQuote(e.Value.ToString(), false), "uniqueidentifier", result);
                    return result;

                case PrimitiveTypeKind.Single:
                    WrapWithCastIfNeeded(true, ((float) e.Value).ToString("R", CultureInfo.InvariantCulture), "real", result);
                    return result;

                case PrimitiveTypeKind.Int16:
                    WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "smallint", result);
                    return result;

                case PrimitiveTypeKind.Int32:
                    result.Append(e.Value.ToString());
                    return result;

                case PrimitiveTypeKind.Int64:
                    WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "bigint", result);
                    return result;

                case PrimitiveTypeKind.String:
                    bool flag2;
                    if (!TypeHelpers.TryGetIsUnicode(e.ResultType, out flag2))
                    {
                        flag2 = true;
                    }
                    result.Append(EscapeSingleQuote(e.Value as string, flag2));
                    return result;

                case PrimitiveTypeKind.Time:
                    this.AssertKatmaiOrNewer(kind);
                    result.Append("convert(");
                    result.Append(e.ResultType.EdmType.Name);
                    result.Append(", ");
                    result.Append(EscapeSingleQuote(e.Value.ToString(), false));
                    result.Append(", 121)");
                    return result;

                case PrimitiveTypeKind.DateTimeOffset:
                {
                    this.AssertKatmaiOrNewer(kind);
                    result.Append("convert(");
                    result.Append(e.ResultType.EdmType.Name);
                    result.Append(", ");
                    DateTimeOffset offset = (DateTimeOffset) e.Value;
                    result.Append(EscapeSingleQuote(offset.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture), false));
                    result.Append(", 121)");
                    return result;
                }
            }
            throw EntityUtil.NotSupported();
        }

        private SqlSelectStatement VisitExpressionEnsureSqlStatement(DbExpression e) => 
            this.VisitExpressionEnsureSqlStatement(e, true);

        private SqlSelectStatement VisitExpressionEnsureSqlStatement(DbExpression e, bool addDefaultColumns)
        {
            SqlSelectStatement statement;
            Symbol symbol;
            string str;
            DbExpressionKind expressionKind = e.ExpressionKind;
            if (expressionKind <= DbExpressionKind.GroupBy)
            {
                switch (expressionKind)
                {
                    case DbExpressionKind.Filter:
                    case DbExpressionKind.GroupBy:
                        goto Label_0028;
                }
                goto Label_003A;
            }
            if ((expressionKind != DbExpressionKind.Project) && (expressionKind != DbExpressionKind.Sort))
            {
                goto Label_003A;
            }
        Label_0028:
            statement = e.Accept<ISqlFragment>(this) as SqlSelectStatement;
            goto Label_00CB;
        Label_003A:
            str = "c";
            this.symbolTable.EnterScope();
            TypeUsage inputVarType = null;
            DbExpressionKind kind2 = e.ExpressionKind;
            if (kind2 <= DbExpressionKind.InnerJoin)
            {
                switch (kind2)
                {
                    case DbExpressionKind.CrossApply:
                    case DbExpressionKind.CrossJoin:
                    case DbExpressionKind.FullOuterJoin:
                    case DbExpressionKind.InnerJoin:
                        goto Label_008C;
                }
                goto Label_009A;
            }
            if (((kind2 != DbExpressionKind.LeftOuterJoin) && (kind2 != DbExpressionKind.OuterApply)) && (kind2 != DbExpressionKind.Scan))
            {
                goto Label_009A;
            }
        Label_008C:
            inputVarType = TypeHelpers.GetElementTypeUsage(e.ResultType);
            goto Label_00AB;
        Label_009A:
            inputVarType = TypeHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage;
        Label_00AB:
            statement = this.VisitInputExpression(e, str, inputVarType, out symbol);
            this.AddFromSymbol(statement, str, symbol);
            this.symbolTable.ExitScope();
        Label_00CB:
            if (addDefaultColumns && statement.Select.IsEmpty)
            {
                this.AddDefaultColumns(statement);
            }
            return statement;
        }

        private SqlSelectStatement VisitFilterExpression(DbExpressionBinding input, DbExpression predicate, bool negatePredicate)
        {
            Symbol symbol;
            SqlSelectStatement result = this.VisitInputExpression(input.Expression, input.VariableName, input.VariableType, out symbol);
            if (!IsCompatible(result, DbExpressionKind.Filter))
            {
                result = this.CreateNewSelectStatement(result, input.VariableName, input.VariableType, out symbol);
            }
            this.selectStatementStack.Push(result);
            this.symbolTable.EnterScope();
            this.AddFromSymbol(result, input.VariableName, symbol);
            if (negatePredicate)
            {
                result.Where.Append("NOT (");
            }
            result.Where.Append(predicate.Accept<ISqlFragment>(this));
            if (negatePredicate)
            {
                result.Where.Append(")");
            }
            this.symbolTable.ExitScope();
            this.selectStatementStack.Pop();
            return result;
        }

        private SqlSelectStatement VisitInputExpression(DbExpression inputExpression, string inputVarName, TypeUsage inputVarType, out Symbol fromSymbol)
        {
            ISqlFragment sqlFragment = inputExpression.Accept<ISqlFragment>(this);
            SqlSelectStatement result = sqlFragment as SqlSelectStatement;
            if (result == null)
            {
                result = new SqlSelectStatement();
                WrapNonQueryExtent(result, sqlFragment, inputExpression.ExpressionKind);
            }
            if (result.FromExtents.Count == 0)
            {
                fromSymbol = new Symbol(inputVarName, inputVarType);
                return result;
            }
            if (result.FromExtents.Count == 1)
            {
                fromSymbol = result.FromExtents[0];
                return result;
            }
            JoinSymbol symbol = new JoinSymbol(inputVarName, inputVarType, result.FromExtents) {
                FlattenedExtentList = result.AllJoinExtents
            };
            fromSymbol = symbol;
            result.FromExtents.Clear();
            result.FromExtents.Add(fromSymbol);
            return result;
        }

        private SqlBuilder VisitIsEmptyExpression(DbIsEmptyExpression e, bool negate)
        {
            SqlBuilder builder = new SqlBuilder();
            if (!negate)
            {
                builder.Append(" NOT");
            }
            builder.Append(" EXISTS (");
            builder.Append(this.VisitExpressionEnsureSqlStatement(e.Argument));
            builder.AppendLine();
            builder.Append(")");
            return builder;
        }

        private SqlBuilder VisitIsNullExpression(DbIsNullExpression e, bool negate)
        {
            SqlBuilder builder = new SqlBuilder();
            builder.Append(e.Argument.Accept<ISqlFragment>(this));
            if (!negate)
            {
                builder.Append(" IS NULL");
                return builder;
            }
            builder.Append(" IS NOT NULL");
            return builder;
        }

        private ISqlFragment VisitJoinExpression(IList<DbExpressionBinding> inputs, DbExpressionKind joinKind, string joinString, DbExpression joinCondition)
        {
            SqlSelectStatement currentSelectStatement;
            if (!this.IsParentAJoin)
            {
                currentSelectStatement = new SqlSelectStatement {
                    AllJoinExtents = new List<Symbol>()
                };
                this.selectStatementStack.Push(currentSelectStatement);
            }
            else
            {
                currentSelectStatement = this.CurrentSelectStatement;
            }
            this.symbolTable.EnterScope();
            string str = "";
            bool flag = true;
            int count = inputs.Count;
            for (int i = 0; i < count; i++)
            {
                DbExpressionBinding input = inputs[i];
                if (str.Length != 0)
                {
                    currentSelectStatement.From.AppendLine();
                }
                currentSelectStatement.From.Append(str + " ");
                bool item = (input.Expression.ExpressionKind == DbExpressionKind.Scan) || (flag && (IsJoinExpression(input.Expression) || IsApplyExpression(input.Expression)));
                this.isParentAJoinStack.Push(item);
                int fromSymbolStart = currentSelectStatement.FromExtents.Count;
                ISqlFragment fromExtentFragment = input.Expression.Accept<ISqlFragment>(this);
                this.isParentAJoinStack.Pop();
                this.ProcessJoinInputResult(fromExtentFragment, currentSelectStatement, input, fromSymbolStart);
                str = joinString;
                flag = false;
            }
            switch (joinKind)
            {
                case DbExpressionKind.FullOuterJoin:
                case DbExpressionKind.InnerJoin:
                case DbExpressionKind.LeftOuterJoin:
                    currentSelectStatement.From.Append(" ON ");
                    this.isParentAJoinStack.Push(false);
                    currentSelectStatement.From.Append(joinCondition.Accept<ISqlFragment>(this));
                    this.isParentAJoinStack.Pop();
                    break;
            }
            this.symbolTable.ExitScope();
            if (!this.IsParentAJoin)
            {
                this.selectStatementStack.Pop();
            }
            return currentSelectStatement;
        }

        private ISqlFragment VisitNewInstanceExpression(DbNewInstanceExpression e, bool aliasesNeedRenaming, out Dictionary<string, Symbol> newColumns)
        {
            SqlBuilder builder = new SqlBuilder();
            RowType edmType = e.ResultType.EdmType as RowType;
            if (edmType == null)
            {
                throw EntityUtil.NotSupported();
            }
            if (aliasesNeedRenaming)
            {
                newColumns = new Dictionary<string, Symbol>(e.Arguments.Count);
            }
            else
            {
                newColumns = null;
            }
            ReadOnlyMetadataCollection<EdmProperty> properties = edmType.Properties;
            string s = "";
            for (int i = 0; i < e.Arguments.Count; i++)
            {
                DbExpression expression = e.Arguments[i];
                if (TypeSemantics.IsRowType(expression.ResultType))
                {
                    throw EntityUtil.NotSupported();
                }
                EdmProperty property = properties[i];
                builder.Append(s);
                builder.AppendLine();
                builder.Append(expression.Accept<ISqlFragment>(this));
                builder.Append(" AS ");
                if (aliasesNeedRenaming)
                {
                    Symbol symbol = new Symbol(property.Name, property.TypeUsage) {
                        NeedsRenaming = true,
                        NewName = "Internal_" + property.Name
                    };
                    builder.Append(symbol);
                    newColumns.Add(property.Name, symbol);
                }
                else
                {
                    builder.Append(QuoteIdentifier(property.Name));
                }
                s = ", ";
            }
            return builder;
        }

        private ISqlFragment VisitSetOpExpression(DbExpression left, DbExpression right, string separator)
        {
            SqlSelectStatement s = this.VisitExpressionEnsureSqlStatement(left);
            SqlSelectStatement statement2 = this.VisitExpressionEnsureSqlStatement(right);
            SqlBuilder builder = new SqlBuilder();
            builder.Append(s);
            builder.AppendLine();
            builder.Append(separator);
            builder.AppendLine();
            builder.Append(statement2);
            if (!s.OutputColumnsRenamed)
            {
                return builder;
            }
            SqlSelectStatement selectStatement = new SqlSelectStatement();
            selectStatement.From.Append("( ");
            selectStatement.From.Append(builder);
            selectStatement.From.AppendLine();
            selectStatement.From.Append(") ");
            Symbol fromSymbol = new Symbol("X", left.ResultType, s.OutputColumns);
            this.AddFromSymbol(selectStatement, null, fromSymbol, false);
            return selectStatement;
        }

        private static void WrapNonQueryExtent(SqlSelectStatement result, ISqlFragment sqlFragment, DbExpressionKind expressionKind)
        {
            if (expressionKind == DbExpressionKind.Function)
            {
                result.From.Append(sqlFragment);
            }
            else
            {
                result.From.Append(" (");
                result.From.Append(sqlFragment);
                result.From.Append(")");
            }
        }

        private static void WrapWithCastIfNeeded(bool cast, string value, string typeName, SqlBuilder result)
        {
            if (!cast)
            {
                result.Append(value);
            }
            else
            {
                result.Append("cast(");
                result.Append(value);
                result.Append(" as ");
                result.Append(typeName);
                result.Append(")");
            }
        }

        private static void WriteFunctionName(SqlBuilder result, EdmFunction function)
        {
            string storeFunctionNameAttribute;
            if (function.StoreFunctionNameAttribute != null)
            {
                storeFunctionNameAttribute = function.StoreFunctionNameAttribute;
            }
            else
            {
                storeFunctionNameAttribute = function.Name;
            }
            if (TypeHelpers.IsCanonicalFunction(function))
            {
                result.Append(storeFunctionNameAttribute.ToUpperInvariant());
            }
            else if (IsStoreFunction(function))
            {
                result.Append(storeFunctionNameAttribute);
            }
            else
            {
                if (string.IsNullOrEmpty(function.Schema))
                {
                    result.Append(QuoteIdentifier(function.NamespaceName));
                }
                else
                {
                    result.Append(QuoteIdentifier(function.Schema));
                }
                result.Append(".");
                result.Append(QuoteIdentifier(storeFunctionNameAttribute));
            }
        }

        private string WriteSql(ISqlFragment sqlStatement)
        {
            StringBuilder b = new StringBuilder(0x400);
            using (SqlWriter writer = new SqlWriter(b))
            {
                sqlStatement.WriteSql(writer, this);
            }
            return b.ToString();
        }

        internal Dictionary<string, int> AllColumnNames =>
            this.allColumnNames;

        internal Dictionary<string, int> AllExtentNames =>
            this.allExtentNames;

        private SqlSelectStatement CurrentSelectStatement =>
            this.selectStatementStack.Peek();

        internal TypeUsage IntegerType
        {
            get
            {
                if (this.integerType == null)
                {
                    this.integerType = this.GetPrimitiveType(PrimitiveTypeKind.Int64);
                }
                return this.integerType;
            }
        }

        private bool IsParentAJoin
        {
            get
            {
                if (this.isParentAJoinStack.Count != 0)
                {
                    return this.isParentAJoinStack.Peek();
                }
                return false;
            }
        }

        internal bool IsPreKatmai =>
            SqlVersionUtils.IsPreKatmai(this.SqlVersion);

        internal System.Data.SqlClient.SqlVersion SqlVersion =>
            this.sqlVersion;

        internal MetadataWorkspace Workspace =>
            this.metadataWorkspace;

        private delegate ISqlFragment FunctionHandler(SqlGenerator sqlgen, DbFunctionExpression functionExpr);
    }
}

