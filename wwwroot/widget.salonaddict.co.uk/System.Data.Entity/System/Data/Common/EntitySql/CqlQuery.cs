namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal static class CqlQuery
    {
        internal static DbCommandTree AnalyzeSemantics(Expr astExpr, Perspective perspective, ParserOptions parserOptions, Dictionary<string, TypeUsage> parameters, Dictionary<string, TypeUsage> variables)
        {
            IntPtr zero = IntPtr.Zero;
            EntityBid.ScopeEnter(out zero, "<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|API>");
            DbCommandTree tree = null;
            try
            {
                EntityUtil.CheckArgumentNull<Expr>(astExpr, "astExpr");
                EntityUtil.CheckArgumentNull<Perspective>(perspective, "perspective");
                if (EntityBid.TraceOn)
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> astExpr=%d#\n", astExpr.GetHashCode());
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> perspective=%d, perspective Type='%ls'\n", perspective.GetHashCode(), perspective.GetType().Name);
                    if (parserOptions != null)
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.AllowQuotedIdentifiers=%d{bool}\n", parserOptions.AllowQuotedIdentifiers);
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.DefaultOrderByCollation='%ls'\n", parserOptions.DefaultOrderByCollation);
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.IdentifierCaseSensitiveness=%d{System.Data.Common.ParserOptions.CaseSensitiveness}\n", (int) parserOptions.IdentifierCaseSensitiveness);
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions is NULL\n");
                    }
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, TypeUsage> pair in parameters)
                        {
                            EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parameter='%ls', Type='%ls'\n", pair.Key, pair.Value.EdmType.Name);
                        }
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parameters=null\n");
                    }
                    if (variables != null)
                    {
                        foreach (KeyValuePair<string, TypeUsage> pair2 in variables)
                        {
                            EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> variable='%ls', Type='%ls'\n", pair2.Key, pair2.Value.EdmType.Name);
                        }
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> variables=null\n");
                    }
                }
                tree = new SemanticAnalyzer(new SemanticResolver(perspective, parserOptions, parameters, variables)).Analyze(astExpr);
                if (EntityBid.TraceOn && (tree != null))
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|RET> DbCommandTree DUMP BEGIN\n");
                    tree.Trace();
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|RET> DbCommandTree DUMP END\n");
                }
            }
            catch (MetadataException exception)
            {
                throw EntityUtil.EntitySqlError(Strings.GeneralExceptionAsQueryInnerException("Metadata"), exception);
            }
            catch (MappingException exception2)
            {
                throw EntityUtil.EntitySqlError(Strings.GeneralExceptionAsQueryInnerException("Mapping"), exception2);
            }
            finally
            {
                EntityBid.ScopeLeave(ref zero);
            }
            return tree;
        }

        internal static DbExpression AnalyzeSemantics(DbCommandTree builderTree, Expr astExpr, Perspective perspective, ParserOptions parserOptions, Dictionary<string, TypeUsage> parameters, Dictionary<string, TypeUsage> variables)
        {
            IntPtr zero = IntPtr.Zero;
            EntityBid.ScopeEnter(out zero, "<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|API>");
            DbExpression expression = null;
            try
            {
                EntityUtil.CheckArgumentNull<DbCommandTree>(builderTree, "builderTree");
                EntityUtil.CheckArgumentNull<Expr>(astExpr, "astExpr");
                EntityUtil.CheckArgumentNull<Perspective>(perspective, "perspective");
                if (EntityBid.TraceOn)
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> astExpr=%d#\n", astExpr.GetHashCode());
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> perspective=%d, perspective Type='%ls'\n", perspective.GetHashCode(), perspective.GetType().Name);
                    if (parserOptions != null)
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.AllowQuotedIdentifiers=%d{bool}\n", parserOptions.AllowQuotedIdentifiers);
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.DefaultOrderByCollation='%ls'\n", parserOptions.DefaultOrderByCollation);
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions.IdentifierCaseSensitiveness=%d{System.Data.Common.ParserOptions.CaseSensitiveness}\n", (int) parserOptions.IdentifierCaseSensitiveness);
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parserOptions is NULL\n");
                    }
                    if (parameters != null)
                    {
                        foreach (KeyValuePair<string, TypeUsage> pair in parameters)
                        {
                            EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parameter='%ls', Type='%ls'\n", pair.Key, pair.Value.EdmType.Name);
                        }
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> parameters=null\n");
                    }
                    if (variables != null)
                    {
                        foreach (KeyValuePair<string, TypeUsage> pair2 in variables)
                        {
                            EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> variable='%ls', Type='%ls'\n", pair2.Key, pair2.Value.EdmType.Name);
                        }
                    }
                    else
                    {
                        EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|INFO> variables=null\n");
                    }
                }
                expression = new SemanticAnalyzer(new SemanticResolver(perspective, parserOptions, parameters, variables)).Analyze(astExpr, builderTree);
                if (EntityBid.TraceOn && (expression != null))
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|RET> DbExpression DUMP BEGIN\n");
                    DbExpression.TraceInfo(expression);
                    EntityBid.Trace("<System.Data.Common.CqlQuery.AnalyzeSemantics|QUERY|RET> DbExpression DUMP END\n");
                }
            }
            catch (MetadataException exception)
            {
                throw EntityUtil.EntitySqlError(Strings.GeneralExceptionAsQueryInnerException("Metadata"), exception);
            }
            catch (MappingException exception2)
            {
                throw EntityUtil.EntitySqlError(Strings.GeneralExceptionAsQueryInnerException("Mapping"), exception2);
            }
            finally
            {
                EntityBid.ScopeLeave(ref zero);
            }
            return expression;
        }

        internal static DbCommandTree Compile(string queryText, Perspective perspective, ParserOptions parserOptions, Dictionary<string, TypeUsage> parameters) => 
            Compile(queryText, perspective, parserOptions, parameters, null, true);

        internal static DbExpression Compile(DbCommandTree builderTree, string queryText, Perspective perspective, ParserOptions parserOptions, Dictionary<string, TypeUsage> parameters, Dictionary<string, TypeUsage> variables)
        {
            IntPtr zero = IntPtr.Zero;
            EntityBid.ScopeEnter(out zero, "<System.Data.Common.CqlQuery.Compile|QUERY|API> perspective.TargetDataspace: %d{DataSpace}", (int) perspective.TargetDataspace);
            DbExpression expression = null;
            try
            {
                EntityUtil.CheckArgumentNull<DbCommandTree>(builderTree, "builderTree");
                EntityUtil.CheckArgumentNull<string>(queryText, "queryText");
                EntityUtil.CheckArgumentNull<Perspective>(perspective, "perspective");
                ParserOptions options = parserOptions ?? new ParserOptions();
                Expr astExpr = Parse(queryText, options);
                expression = AnalyzeSemantics(builderTree, astExpr, perspective, options, parameters, variables);
            }
            finally
            {
                EntityBid.ScopeLeave(ref zero);
            }
            return expression;
        }

        internal static DbCommandTree Compile(string query, Perspective perspective, ParserOptions parserOptions, Dictionary<string, TypeUsage> parameters, Dictionary<string, TypeUsage> variables, bool validateTree)
        {
            IntPtr zero = IntPtr.Zero;
            EntityBid.ScopeEnter(out zero, "<System.Data.Common.CqlQuery.Compile|QUERY|API> perspective.TargetDataspace: %d{DataSpace}, validateTree: %d{bool}", (int) perspective.TargetDataspace, validateTree);
            DbCommandTree tree = null;
            try
            {
                EntityUtil.CheckArgumentNull<string>(query, "query");
                EntityUtil.CheckArgumentNull<Perspective>(perspective, "perspective");
                ParserOptions options = parserOptions ?? new ParserOptions();
                tree = AnalyzeSemantics(Parse(query, options), perspective, options, parameters, variables);
                if (validateTree)
                {
                    tree.Validate();
                }
            }
            finally
            {
                EntityBid.ScopeLeave(ref zero);
            }
            return tree;
        }

        internal static Expr Parse(string query, ParserOptions parserOptions)
        {
            IntPtr zero = IntPtr.Zero;
            EntityBid.ScopeEnter(out zero, "<System.Data.Common.CqlQuery.Parse|QUERY|API>");
            Expr expr = null;
            try
            {
                EntityUtil.CheckArgumentNull<string>(query, "query");
                if (query.Trim().Length == 0)
                {
                    throw EntityUtil.Argument(Strings.InvalidEmptyQueryTextArgument);
                }
                if (EntityBid.TraceOn)
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|INFO> parserOptions.AllowQuotedIdentifiers=%d{bool}\n", parserOptions.AllowQuotedIdentifiers);
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|INFO> parserOptions.DefaultOrderByCollation='%ls'\n", parserOptions.DefaultOrderByCollation);
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|INFO> parserOptions.IdentifierCaseSensitiveness=%d{System.Data.Common.ParserOptions.CaseSensitiveness}\n", (int) parserOptions.IdentifierCaseSensitiveness);
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|INFO> query BEGIN\n");
                    EntityBid.PutStr(query);
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|INFO> query END\n");
                }
                expr = new CqlParser(parserOptions, true).Parse(query);
                if (EntityBid.TraceOn)
                {
                    EntityBid.Trace("<System.Data.Common.CqlQuery.Parse|QUERY|API|RET> astExpr=%d#\n", (expr != null) ? expr.GetHashCode() : 0);
                }
            }
            finally
            {
                EntityBid.ScopeLeave(ref zero);
            }
            if (expr == null)
            {
                throw EntityUtil.EntitySqlError(query, Strings.InvalidEmptyQuery, 0);
            }
            return expr;
        }
    }
}

