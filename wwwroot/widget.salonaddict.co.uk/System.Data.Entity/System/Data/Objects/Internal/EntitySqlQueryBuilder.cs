namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Text;

    internal static class EntitySqlQueryBuilder
    {
        private const string _asOp = "\r\n) AS ";
        private const string _distinctEpilog = "\r\n)";
        private const string _distinctProlog = "SET(\r\n";
        private const string _exceptOp = "\r\n) EXCEPT (\r\n";
        private const string _fromOp = "\r\nFROM (\r\n";
        private const string _groupByOp = "\r\nGROUP BY\r\n";
        private const string _intersectOp = "\r\n) INTERSECT (\r\n";
        private const string _limitOp = "\r\nLIMIT\r\n";
        private const string _ofTypeEpilog = "]\r\n)";
        private const string _ofTypeInfix = "\r\n),\r\n[";
        private const string _ofTypeInfix2 = "].[";
        private const string _ofTypeProlog = "OFTYPE(\r\n(\r\n";
        private const string _orderByOp = "\r\nORDER BY\r\n";
        private const string _selectOp = "SELECT ";
        private const string _selectValueOp = "SELECT VALUE ";
        private const string _setOpEpilog = "\r\n)";
        private const string _setOpProlog = "(\r\n";
        private const string _skipOp = "\r\nSKIP\r\n";
        private const string _topInfix = "\r\n) ";
        private const string _topOp = "SELECT VALUE TOP(\r\n";
        private const string _unionAllOp = "\r\n) UNION ALL (\r\n";
        private const string _unionOp = "\r\n) UNION (\r\n";
        private const string _whereOp = "\r\nWHERE\r\n";

        private static ObjectQueryState BuildOrderByOrWhere(ObjectQueryState query, string alias, string predicateOrKeys, ObjectParameter[] parameters, string op, string skipCount, bool allowsLimit)
        {
            string commandText = GetCommandText(query);
            int capacity = (((((("SELECT VALUE ".Length + alias.Length) + "\r\nFROM (\r\n".Length) + commandText.Length) + "\r\n) AS ".Length) + alias.Length) + op.Length) + predicateOrKeys.Length;
            if (skipCount != null)
            {
                capacity += "\r\nSKIP\r\n".Length + skipCount.Length;
            }
            StringBuilder queryText = new StringBuilder(capacity);
            queryText.Append("SELECT VALUE ");
            queryText.Append(alias);
            queryText.Append("\r\nFROM (\r\n");
            queryText.Append(commandText);
            queryText.Append("\r\n) AS ");
            queryText.Append(alias);
            queryText.Append(op);
            queryText.Append(predicateOrKeys);
            if (skipCount != null)
            {
                queryText.Append("\r\nSKIP\r\n");
                queryText.Append(skipCount);
            }
            return NewBuilderQuery(query, query.ElementType, queryText, allowsLimit, query.Span, MergeParameters(query.ObjectContext, query.Parameters, parameters));
        }

        private static ObjectQueryState BuildSelectOrSelectValue(ObjectQueryState query, string alias, string projection, ObjectParameter[] parameters, string projectOp, Type elementType)
        {
            string commandText = GetCommandText(query);
            int capacity = ((((projectOp.Length + projection.Length) + "\r\nFROM (\r\n".Length) + commandText.Length) + "\r\n) AS ".Length) + alias.Length;
            StringBuilder queryText = new StringBuilder(capacity);
            queryText.Append(projectOp);
            queryText.Append(projection);
            queryText.Append("\r\nFROM (\r\n");
            queryText.Append(commandText);
            queryText.Append("\r\n) AS ");
            queryText.Append(alias);
            return NewBuilderQuery(query, elementType, queryText, null, MergeParameters(query.ObjectContext, query.Parameters, parameters));
        }

        private static ObjectQueryState BuildSetOp(ObjectQueryState leftQuery, ObjectQueryState rightQuery, Span newSpan, string setOp)
        {
            string commandText = GetCommandText(leftQuery);
            string str2 = GetCommandText(rightQuery);
            if (!object.ReferenceEquals(leftQuery.ObjectContext, rightQuery.ObjectContext))
            {
                throw EntityUtil.Argument(Strings.ObjectQuery_QueryBuilder_InvalidQueryArgument, "query");
            }
            int capacity = ((("(\r\n".Length + commandText.Length) + setOp.Length) + str2.Length) + "\r\n)".Length;
            StringBuilder queryText = new StringBuilder(capacity);
            queryText.Append("(\r\n");
            queryText.Append(commandText);
            queryText.Append(setOp);
            queryText.Append(str2);
            queryText.Append("\r\n)");
            return NewBuilderQuery(leftQuery, leftQuery.ElementType, queryText, newSpan, MergeParameters(leftQuery.Parameters, rightQuery.Parameters));
        }

        internal static ObjectQueryState Distinct(ObjectQueryState query)
        {
            string commandText = GetCommandText(query);
            StringBuilder queryText = new StringBuilder(("SET(\r\n".Length + commandText.Length) + "\r\n)".Length);
            queryText.Append("SET(\r\n");
            queryText.Append(commandText);
            queryText.Append("\r\n)");
            return NewBuilderQuery(query, query.ElementType, queryText, query.Span, ObjectParameterCollection.DeepCopy(query.Parameters));
        }

        internal static ObjectQueryState Except(ObjectQueryState leftQuery, ObjectQueryState rightQuery) => 
            BuildSetOp(leftQuery, rightQuery, leftQuery.Span, "\r\n) EXCEPT (\r\n");

        private static string GetCommandText(ObjectQueryState query)
        {
            string commandText = null;
            if (!query.TryGetCommandText(out commandText))
            {
                throw EntityUtil.NotSupported(Strings.ObjectQuery_QueryBuilder_NotSupportedLinqSource);
            }
            return commandText;
        }

        internal static ObjectQueryState GroupBy(ObjectQueryState query, string alias, string keys, string projection, ObjectParameter[] parameters)
        {
            string commandText = GetCommandText(query);
            int capacity = (((((("SELECT ".Length + projection.Length) + "\r\nFROM (\r\n".Length) + commandText.Length) + "\r\n) AS ".Length) + alias.Length) + "\r\nGROUP BY\r\n".Length) + keys.Length;
            StringBuilder queryText = new StringBuilder(capacity);
            queryText.Append("SELECT ");
            queryText.Append(projection);
            queryText.Append("\r\nFROM (\r\n");
            queryText.Append(commandText);
            queryText.Append("\r\n) AS ");
            queryText.Append(alias);
            queryText.Append("\r\nGROUP BY\r\n");
            queryText.Append(keys);
            return NewBuilderQuery(query, typeof(DbDataRecord), queryText, null, MergeParameters(query.ObjectContext, query.Parameters, parameters));
        }

        internal static ObjectQueryState Intersect(ObjectQueryState leftQuery, ObjectQueryState rightQuery)
        {
            Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
            return BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) INTERSECT (\r\n");
        }

        private static ObjectParameterCollection MergeParameters(ObjectParameterCollection query1Params, ObjectParameterCollection query2Params)
        {
            ObjectParameterCollection parameters;
            ObjectParameterCollection parameters2;
            if ((query1Params == null) && (query2Params == null))
            {
                return null;
            }
            if (query1Params != null)
            {
                parameters = ObjectParameterCollection.DeepCopy(query1Params);
                parameters2 = query2Params;
            }
            else
            {
                parameters = ObjectParameterCollection.DeepCopy(query2Params);
                parameters2 = query1Params;
            }
            if (parameters2 != null)
            {
                foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) parameters2)
                {
                    parameters.Add(parameter.ShallowCopy());
                }
            }
            return parameters;
        }

        private static ObjectParameterCollection MergeParameters(ObjectContext context, ObjectParameterCollection sourceQueryParams, ObjectParameter[] builderMethodParams)
        {
            if ((sourceQueryParams == null) && (builderMethodParams.Length == 0))
            {
                return null;
            }
            ObjectParameterCollection parameters = ObjectParameterCollection.DeepCopy(sourceQueryParams);
            if (parameters == null)
            {
                parameters = new ObjectParameterCollection(context.Perspective);
            }
            foreach (ObjectParameter parameter in builderMethodParams)
            {
                parameters.Add(parameter);
            }
            return parameters;
        }

        private static ObjectQueryState NewBuilderQuery(ObjectQueryState sourceQuery, Type elementType, StringBuilder queryText, Span newSpan, IEnumerable<ObjectParameter> enumerableParams) => 
            NewBuilderQuery(sourceQuery, elementType, queryText, false, newSpan, enumerableParams);

        private static ObjectQueryState NewBuilderQuery(ObjectQueryState sourceQuery, Type elementType, StringBuilder queryText, bool allowsLimit, Span newSpan, IEnumerable<ObjectParameter> enumerableParams)
        {
            ObjectParameterCollection parameters = enumerableParams as ObjectParameterCollection;
            if ((parameters == null) && (enumerableParams != null))
            {
                parameters = new ObjectParameterCollection(sourceQuery.ObjectContext.Perspective);
                foreach (ObjectParameter parameter in enumerableParams)
                {
                    parameters.Add(parameter);
                }
            }
            EntitySqlQueryState other = new EntitySqlQueryState(elementType, queryText.ToString(), allowsLimit, sourceQuery.ObjectContext, parameters, newSpan);
            sourceQuery.ApplySettingsTo(other);
            return other;
        }

        internal static ObjectQueryState OfType(ObjectQueryState query, EdmType newType, Type clrOfType)
        {
            string commandText = GetCommandText(query);
            int capacity = ((((("OFTYPE(\r\n(\r\n".Length + commandText.Length) + "\r\n),\r\n[".Length) + newType.NamespaceName.Length) + ((newType.NamespaceName != string.Empty) ? "].[".Length : 0)) + newType.Name.Length) + "]\r\n)".Length;
            StringBuilder queryText = new StringBuilder(capacity);
            queryText.Append("OFTYPE(\r\n(\r\n");
            queryText.Append(commandText);
            queryText.Append("\r\n),\r\n[");
            if (newType.NamespaceName != string.Empty)
            {
                queryText.Append(newType.NamespaceName);
                queryText.Append("].[");
            }
            queryText.Append(newType.Name);
            queryText.Append("]\r\n)");
            return NewBuilderQuery(query, clrOfType, queryText, query.Span, ObjectParameterCollection.DeepCopy(query.Parameters));
        }

        internal static ObjectQueryState OrderBy(ObjectQueryState query, string alias, string keys, ObjectParameter[] parameters) => 
            BuildOrderByOrWhere(query, alias, keys, parameters, "\r\nORDER BY\r\n", null, true);

        internal static ObjectQueryState Select(ObjectQueryState query, string alias, string projection, ObjectParameter[] parameters) => 
            BuildSelectOrSelectValue(query, alias, projection, parameters, "SELECT ", typeof(DbDataRecord));

        internal static ObjectQueryState SelectValue(ObjectQueryState query, string alias, string projection, ObjectParameter[] parameters, Type projectedType) => 
            BuildSelectOrSelectValue(query, alias, projection, parameters, "SELECT VALUE ", projectedType);

        internal static ObjectQueryState Skip(ObjectQueryState query, string alias, string keys, string count, ObjectParameter[] parameters) => 
            BuildOrderByOrWhere(query, alias, keys, parameters, "\r\nORDER BY\r\n", count, true);

        internal static ObjectQueryState Top(ObjectQueryState query, string alias, string count, ObjectParameter[] parameters)
        {
            int length = count.Length;
            string commandText = GetCommandText(query);
            bool allowsLimitSubclause = ((EntitySqlQueryState) query).AllowsLimitSubclause;
            if (allowsLimitSubclause)
            {
                length += commandText.Length + "\r\nLIMIT\r\n".Length;
            }
            else
            {
                length += ((((("SELECT VALUE TOP(\r\n".Length + "\r\n) ".Length) + alias.Length) + "\r\nFROM (\r\n".Length) + commandText.Length) + "\r\n) AS ".Length) + alias.Length;
            }
            StringBuilder queryText = new StringBuilder(length);
            if (allowsLimitSubclause)
            {
                queryText.Append(commandText);
                queryText.Append("\r\nLIMIT\r\n");
                queryText.Append(count);
            }
            else
            {
                queryText.Append("SELECT VALUE TOP(\r\n");
                queryText.Append(count);
                queryText.Append("\r\n) ");
                queryText.Append(alias);
                queryText.Append("\r\nFROM (\r\n");
                queryText.Append(commandText);
                queryText.Append("\r\n) AS ");
                queryText.Append(alias);
            }
            return NewBuilderQuery(query, query.ElementType, queryText, query.Span, MergeParameters(query.ObjectContext, query.Parameters, parameters));
        }

        internal static ObjectQueryState Union(ObjectQueryState leftQuery, ObjectQueryState rightQuery)
        {
            Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
            return BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) UNION (\r\n");
        }

        internal static ObjectQueryState UnionAll(ObjectQueryState leftQuery, ObjectQueryState rightQuery)
        {
            Span newSpan = Span.CopyUnion(leftQuery.Span, rightQuery.Span);
            return BuildSetOp(leftQuery, rightQuery, newSpan, "\r\n) UNION ALL (\r\n");
        }

        internal static ObjectQueryState Where(ObjectQueryState query, string alias, string predicate, ObjectParameter[] parameters) => 
            BuildOrderByOrWhere(query, alias, predicate, parameters, "\r\nWHERE\r\n", null, false);
    }
}

