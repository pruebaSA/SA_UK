namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class Translator
    {
        private IDataServices services;
        private SqlFactory sql;
        private TypeSystemProvider typeProvider;

        internal Translator(IDataServices services, SqlFactory sqlFactory, TypeSystemProvider typeProvider)
        {
            this.services = services;
            this.sql = sqlFactory;
            this.typeProvider = typeProvider;
        }

        private static SqlExpression BestIdentityNode(SqlTypeCase tc)
        {
            foreach (SqlTypeCaseWhen when in tc.Whens)
            {
                if (when.TypeBinding.NodeType == SqlNodeType.New)
                {
                    return when.TypeBinding;
                }
            }
            return tc.Whens[0].TypeBinding;
        }

        internal SqlSelect BuildDefaultQuery(MetaType rowType, bool allowDeferred, SqlLink link, Expression source)
        {
            if (rowType.HasInheritance && (rowType.InheritanceRoot != rowType))
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentWrongValue("rowType");
            }
            SqlTable node = this.sql.Table(rowType.Table, rowType, source);
            SqlAlias alias = new SqlAlias(node);
            SqlAliasRef item = new SqlAliasRef(alias);
            return new SqlSelect(this.BuildProjection(item, node.RowType, allowDeferred, link, source), alias, source);
        }

        private SqlLink BuildLink(SqlExpression item, MetaDataMember member, Expression source)
        {
            if (member.IsAssociation)
            {
                SqlExpression[] expressionArray = new SqlExpression[member.Association.ThisKey.Count];
                int index = 0;
                int length = expressionArray.Length;
                while (index < length)
                {
                    MetaDataMember member2 = member.Association.ThisKey[index];
                    expressionArray[index] = this.sql.Member(item, member2.Member);
                    index++;
                }
                return new SqlLink(new object(), member.Association.OtherType, member.Type, this.typeProvider.From(member.Type), item, member, expressionArray, null, source);
            }
            MetaType declaringType = member.DeclaringType;
            List<SqlExpression> keyExpressions = new List<SqlExpression>();
            foreach (MetaDataMember member3 in declaringType.IdentityMembers)
            {
                keyExpressions.Add(this.sql.Member(item, member3.Member));
            }
            return new SqlLink(new object(), declaringType, member.Type, this.typeProvider.From(member.Type), item, member, keyExpressions, this.sql.Member(item, member.Member), source);
        }

        internal SqlExpression BuildProjection(SqlExpression item, MetaType rowType, bool allowDeferred, SqlLink link, Expression source)
        {
            if (!rowType.HasInheritance)
            {
                return this.BuildProjectionInternal(item, rowType, (rowType.Table != null) ? rowType.PersistentDataMembers : rowType.DataMembers, allowDeferred, link, source);
            }
            List<MetaType> list = new List<MetaType>(rowType.InheritanceTypes);
            List<SqlTypeCaseWhen> list2 = new List<SqlTypeCaseWhen>();
            SqlTypeCaseWhen when = null;
            MetaType inheritanceRoot = rowType.InheritanceRoot;
            MetaDataMember discriminator = inheritanceRoot.Discriminator;
            Type type = discriminator.Type;
            SqlMember member2 = this.sql.Member(item, discriminator.Member);
            foreach (MetaType type3 in list)
            {
                if (type3.HasInheritanceCode)
                {
                    SqlNew typeBinding = this.BuildProjectionInternal(item, type3, type3.PersistentDataMembers, allowDeferred, link, source);
                    if (type3.IsInheritanceDefault)
                    {
                        when = new SqlTypeCaseWhen(null, typeBinding);
                    }
                    object obj2 = InheritanceRules.InheritanceCodeForClientCompare(type3.InheritanceCode, member2.SqlType);
                    SqlExpression match = this.sql.Value(type, this.sql.Default(discriminator), obj2, true, source);
                    list2.Add(new SqlTypeCaseWhen(match, typeBinding));
                }
            }
            if (when == null)
            {
                throw System.Data.Linq.SqlClient.Error.EmptyCaseNotSupported();
            }
            list2.Add(when);
            return this.sql.TypeCase(inheritanceRoot.Type, inheritanceRoot, member2, list2.ToArray(), source);
        }

        private SqlNew BuildProjectionInternal(SqlExpression item, MetaType rowType, IEnumerable<MetaDataMember> members, bool allowDeferred, SqlLink link, Expression source)
        {
            List<SqlMemberAssign> bindings = new List<SqlMemberAssign>();
            foreach (MetaDataMember member in members)
            {
                if (allowDeferred && (member.IsAssociation || member.IsDeferred))
                {
                    if ((((link != null) && (member != link.Member)) && (member.IsAssociation && (member.MappedName == link.Member.MappedName))) && (!member.Association.IsMany && !this.IsPreloaded(link.Member.Member)))
                    {
                        SqlLink expr = this.BuildLink(item, member, source);
                        expr.Expansion = link.Expression;
                        bindings.Add(new SqlMemberAssign(member.Member, expr));
                    }
                    else
                    {
                        bindings.Add(new SqlMemberAssign(member.Member, this.BuildLink(item, member, source)));
                    }
                }
                else if (!member.IsAssociation)
                {
                    bindings.Add(new SqlMemberAssign(member.Member, this.sql.Member(item, member)));
                }
            }
            ConstructorInfo cons = rowType.Type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (cons == null)
            {
                throw System.Data.Linq.SqlClient.Error.MappedTypeMustHaveDefaultConstructor(rowType.Type);
            }
            return this.sql.New(rowType, cons, null, null, bindings, source);
        }

        private List<SqlExpression> GetIdentityExpressions(MetaType type, SqlExpression expr)
        {
            List<MetaDataMember> list = this.GetIdentityMembers(type).ToList<MetaDataMember>();
            List<SqlExpression> list2 = new List<SqlExpression>(list.Count);
            foreach (MetaDataMember member in list)
            {
                list2.Add(this.sql.Member((SqlExpression) SqlDuplicator.Copy(expr), member));
            }
            return list2;
        }

        private IEnumerable<MetaDataMember> GetIdentityMembers(MetaType type)
        {
            if (type.IsEntity)
            {
                return type.IdentityMembers;
            }
            return (from m in type.DataMembers
                where IsPublic(m.Member)
                select m);
        }

        private bool IsPreloaded(MemberInfo member) => 
            this.services.Context.LoadOptions?.IsPreloaded(member);

        private static bool IsPublic(MemberInfo mi)
        {
            FieldInfo info = mi as FieldInfo;
            if (info != null)
            {
                return info.IsPublic;
            }
            PropertyInfo info2 = mi as PropertyInfo;
            if ((info2 != null) && info2.CanRead)
            {
                MethodInfo getMethod = info2.GetGetMethod();
                if (getMethod != null)
                {
                    return getMethod.IsPublic;
                }
            }
            return false;
        }

        internal static Expression TranslateAssociation(DataContext context, MetaAssociation association, Expression otherSource, Expression[] keyValues, Expression thisInstance)
        {
            if (association == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("association");
            }
            if (keyValues == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("keyValues");
            }
            if (context.LoadOptions != null)
            {
                LambdaExpression associationSubquery = context.LoadOptions.GetAssociationSubquery(association.ThisMember.Member);
                if (associationSubquery != null)
                {
                    RelationComposer composer = new RelationComposer(associationSubquery.Parameters[0], association, otherSource, thisInstance);
                    return composer.Visit(associationSubquery.Body);
                }
            }
            return WhereClauseFromSourceAndKeys(otherSource, association.OtherKey.ToArray<MetaDataMember>(), keyValues);
        }

        internal SqlExpression TranslateEquals(SqlBinary expr)
        {
            List<SqlExpression> keyExpressions;
            List<SqlExpression> identityExpressions;
            SqlExpression left = expr.Left;
            SqlExpression right = expr.Right;
            if (right.NodeType == SqlNodeType.Element)
            {
                SqlSubSelect select = (SqlSubSelect) right;
                SqlAlias alias = new SqlAlias(select.Select);
                SqlAliasRef selection = new SqlAliasRef(alias);
                SqlSelect select2 = new SqlSelect(selection, alias, expr.SourceExpression) {
                    Where = this.sql.Binary(expr.NodeType, this.sql.DoNotVisitExpression(left), selection)
                };
                return this.sql.SubSelect(SqlNodeType.Exists, select2);
            }
            if (left.NodeType == SqlNodeType.Element)
            {
                SqlSubSelect select3 = (SqlSubSelect) left;
                SqlAlias alias2 = new SqlAlias(select3.Select);
                SqlAliasRef ref3 = new SqlAliasRef(alias2);
                SqlSelect select4 = new SqlSelect(ref3, alias2, expr.SourceExpression) {
                    Where = this.sql.Binary(expr.NodeType, this.sql.DoNotVisitExpression(right), ref3)
                };
                return this.sql.SubSelect(SqlNodeType.Exists, select4);
            }
            MetaType sourceMetaType = TypeSource.GetSourceMetaType(left, this.services.Model);
            MetaType type = TypeSource.GetSourceMetaType(right, this.services.Model);
            if (left.NodeType == SqlNodeType.TypeCase)
            {
                left = BestIdentityNode((SqlTypeCase) left);
            }
            if (right.NodeType == SqlNodeType.TypeCase)
            {
                right = BestIdentityNode((SqlTypeCase) right);
            }
            if ((sourceMetaType.IsEntity && type.IsEntity) && (sourceMetaType.Table != type.Table))
            {
                throw System.Data.Linq.SqlClient.Error.CannotCompareItemsAssociatedWithDifferentTable();
            }
            if (((!sourceMetaType.IsEntity && !type.IsEntity) && ((left.NodeType != SqlNodeType.New) || left.SqlType.CanBeColumn)) && ((right.NodeType != SqlNodeType.New) || right.SqlType.CanBeColumn))
            {
                if ((expr.NodeType == SqlNodeType.EQ2V) || (expr.NodeType == SqlNodeType.NE2V))
                {
                    return this.TranslateEqualsOp(expr.NodeType, this.sql.DoNotVisitExpression(expr.Left), this.sql.DoNotVisitExpression(expr.Right), false);
                }
                return expr;
            }
            if ((sourceMetaType != type) && (sourceMetaType.InheritanceRoot != type.InheritanceRoot))
            {
                return this.sql.Binary(SqlNodeType.EQ, this.sql.ValueFromObject(0, expr.SourceExpression), this.sql.ValueFromObject(1, expr.SourceExpression));
            }
            SqlLink link = left as SqlLink;
            if (((link != null) && link.Member.IsAssociation) && link.Member.Association.IsForeignKey)
            {
                keyExpressions = link.KeyExpressions;
            }
            else
            {
                keyExpressions = this.GetIdentityExpressions(sourceMetaType, this.sql.DoNotVisitExpression(left));
            }
            SqlLink link2 = right as SqlLink;
            if (((link2 != null) && link2.Member.IsAssociation) && link2.Member.Association.IsForeignKey)
            {
                identityExpressions = link2.KeyExpressions;
            }
            else
            {
                identityExpressions = this.GetIdentityExpressions(type, this.sql.DoNotVisitExpression(right));
            }
            SqlExpression expression3 = null;
            SqlNodeType op = ((expr.NodeType == SqlNodeType.EQ2V) || (expr.NodeType == SqlNodeType.NE2V)) ? SqlNodeType.EQ2V : SqlNodeType.EQ;
            int num = 0;
            int count = keyExpressions.Count;
            while (num < count)
            {
                SqlExpression expression4 = this.TranslateEqualsOp(op, keyExpressions[num], identityExpressions[num], !sourceMetaType.IsEntity);
                if (expression3 == null)
                {
                    expression3 = expression4;
                }
                else
                {
                    expression3 = this.sql.Binary(SqlNodeType.And, expression3, expression4);
                }
                num++;
            }
            if ((expr.NodeType != SqlNodeType.NE) && (expr.NodeType != SqlNodeType.NE2V))
            {
                return expression3;
            }
            return this.sql.Unary(SqlNodeType.Not, expression3, expression3.SourceExpression);
        }

        private SqlExpression TranslateEqualsOp(SqlNodeType op, SqlExpression left, SqlExpression right, bool allowExpand)
        {
            switch (op)
            {
                case SqlNodeType.EQ:
                case SqlNodeType.NE:
                    return this.sql.Binary(op, left, right);

                case SqlNodeType.EQ2V:
                {
                    if ((SqlExpressionNullability.CanBeNull(left) != false) && (SqlExpressionNullability.CanBeNull(right) != false))
                    {
                        SqlNodeType type = allowExpand ? SqlNodeType.EQ2V : SqlNodeType.EQ;
                        return this.sql.Binary(SqlNodeType.Or, this.sql.Binary(SqlNodeType.And, this.sql.Unary(SqlNodeType.IsNull, (SqlExpression) SqlDuplicator.Copy(left)), this.sql.Unary(SqlNodeType.IsNull, (SqlExpression) SqlDuplicator.Copy(right))), this.sql.Binary(SqlNodeType.And, this.sql.Binary(SqlNodeType.And, this.sql.Unary(SqlNodeType.IsNotNull, (SqlExpression) SqlDuplicator.Copy(left)), this.sql.Unary(SqlNodeType.IsNotNull, (SqlExpression) SqlDuplicator.Copy(right))), this.sql.Binary(type, left, right)));
                    }
                    SqlNodeType nodeType = allowExpand ? SqlNodeType.EQ2V : SqlNodeType.EQ;
                    return this.sql.Binary(nodeType, left, right);
                }
                case SqlNodeType.NE2V:
                {
                    if ((SqlExpressionNullability.CanBeNull(left) == false) || (SqlExpressionNullability.CanBeNull(right) == false))
                    {
                        SqlNodeType type4 = allowExpand ? SqlNodeType.NE2V : SqlNodeType.NE;
                        return this.sql.Binary(type4, left, right);
                    }
                    SqlNodeType type3 = allowExpand ? SqlNodeType.EQ2V : SqlNodeType.EQ;
                    return this.sql.Unary(SqlNodeType.Not, this.sql.Binary(SqlNodeType.Or, this.sql.Binary(SqlNodeType.And, this.sql.Unary(SqlNodeType.IsNull, (SqlExpression) SqlDuplicator.Copy(left)), this.sql.Unary(SqlNodeType.IsNull, (SqlExpression) SqlDuplicator.Copy(right))), this.sql.Binary(SqlNodeType.And, this.sql.Binary(SqlNodeType.And, this.sql.Unary(SqlNodeType.IsNotNull, (SqlExpression) SqlDuplicator.Copy(left)), this.sql.Unary(SqlNodeType.IsNotNull, (SqlExpression) SqlDuplicator.Copy(right))), this.sql.Binary(type3, left, right))));
                }
            }
            throw System.Data.Linq.SqlClient.Error.UnexpectedNode(op);
        }

        internal SqlNode TranslateLink(SqlLink link, bool asExpression) => 
            this.TranslateLink(link, link.KeyExpressions, asExpression);

        internal SqlNode TranslateLink(SqlLink link, List<SqlExpression> keyExpressions, bool asExpression)
        {
            MetaDataMember member = link.Member;
            if (!member.IsAssociation)
            {
                return link.Expansion;
            }
            MetaType otherType = member.Association.OtherType;
            Type type = otherType.InheritanceRoot.Type;
            ITable table = this.services.Context.GetTable(type);
            Expression otherSource = new LinkedTableExpression(link, table, typeof(IQueryable<>).MakeGenericType(new Type[] { otherType.Type }));
            Expression[] keyValues = new Expression[keyExpressions.Count];
            for (int i = 0; i < keyExpressions.Count; i++)
            {
                MetaDataMember member2 = member.Association.OtherKey[i];
                Type memberType = TypeSystem.GetMemberType(member2.Member);
                keyValues[i] = InternalExpression.Known(keyExpressions[i], memberType);
            }
            Expression thisInstance = (link.Expression != null) ? ((Expression) InternalExpression.Known(link.Expression)) : ((Expression) Expression.Constant(null, link.Member.Member.DeclaringType));
            Expression expression3 = TranslateAssociation(this.services.Context, member.Association, otherSource, keyValues, thisInstance);
            QueryConverter converter = new QueryConverter(this.services, this.typeProvider, this, this.sql);
            SqlSelect select = (SqlSelect) converter.ConvertInner(expression3, link.SourceExpression);
            SqlNode node = select;
            if (!asExpression)
            {
                return node;
            }
            if (member.Association.IsMany)
            {
                return new SqlSubSelect(SqlNodeType.Multiset, link.ClrType, link.SqlType, select);
            }
            return new SqlSubSelect(SqlNodeType.Element, link.ClrType, link.SqlType, select);
        }

        internal SqlExpression TranslateLinkEquals(SqlBinary bo)
        {
            SqlLink left = bo.Left as SqlLink;
            SqlLink right = bo.Right as SqlLink;
            if ((((left == null) || !left.Member.IsAssociation) || !left.Member.Association.IsForeignKey) && (((right == null) || !right.Member.IsAssociation) || !right.Member.Association.IsForeignKey))
            {
                return bo;
            }
            return this.TranslateEquals(bo);
        }

        internal SqlExpression TranslateLinkIsNull(SqlUnary expr)
        {
            SqlLink operand = expr.Operand as SqlLink;
            if (((operand == null) || !operand.Member.IsAssociation) || !operand.Member.Association.IsForeignKey)
            {
                return expr;
            }
            List<SqlExpression> keyExpressions = operand.KeyExpressions;
            SqlExpression left = null;
            SqlNodeType nodeType = (expr.NodeType == SqlNodeType.IsNull) ? SqlNodeType.Or : SqlNodeType.And;
            int num = 0;
            int count = keyExpressions.Count;
            while (num < count)
            {
                SqlExpression right = this.sql.Unary(expr.NodeType, this.sql.DoNotVisitExpression(keyExpressions[num]), expr.SourceExpression);
                if (left == null)
                {
                    left = right;
                }
                else
                {
                    left = this.sql.Binary(nodeType, left, right);
                }
                num++;
            }
            return left;
        }

        internal static Expression WhereClauseFromSourceAndKeys(Expression source, MetaDataMember[] keyMembers, Expression[] keyValues)
        {
            Type elementType = TypeSystem.GetElementType(source.Type);
            ParameterExpression expression = Expression.Parameter(elementType, "p");
            Expression left = null;
            for (int i = 0; i < keyMembers.Length; i++)
            {
                MetaDataMember member = keyMembers[i];
                Expression expression3 = (elementType == member.Member.DeclaringType) ? ((Expression) expression) : ((Expression) Expression.Convert(expression, member.Member.DeclaringType));
                Expression expression4 = (member.Member is FieldInfo) ? Expression.Field(expression3, (FieldInfo) member.Member) : Expression.Property(expression3, (PropertyInfo) member.Member);
                Expression expression5 = keyValues[i];
                if (expression5.Type != expression4.Type)
                {
                    expression5 = Expression.Convert(expression5, expression4.Type);
                }
                Expression right = Expression.Equal(expression4, expression5);
                left = (left != null) ? Expression.And(left, right) : right;
            }
            return Expression.Call(typeof(Enumerable), "Where", new Type[] { expression.Type }, new Expression[] { source, Expression.Lambda(left, new ParameterExpression[] { expression }) });
        }

        private class RelationComposer : System.Data.Linq.SqlClient.ExpressionVisitor
        {
            private MetaAssociation association;
            private Expression otherSouce;
            private ParameterExpression parameter;
            private Expression parameterReplacement;

            internal RelationComposer(ParameterExpression parameter, MetaAssociation association, Expression otherSouce, Expression parameterReplacement)
            {
                if (parameter == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("parameter");
                }
                if (association == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("association");
                }
                if (otherSouce == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("otherSouce");
                }
                if (parameterReplacement == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("parameterReplacement");
                }
                this.parameter = parameter;
                this.association = association;
                this.otherSouce = otherSouce;
                this.parameterReplacement = parameterReplacement;
            }

            private static Expression[] GetKeyValues(Expression expr, ReadOnlyCollection<MetaDataMember> keys)
            {
                List<Expression> list = new List<Expression>();
                foreach (MetaDataMember member in keys)
                {
                    list.Add(Expression.PropertyOrField(expr, member.Name));
                }
                return list.ToArray();
            }

            internal override Expression VisitMemberAccess(MemberExpression m)
            {
                if (MetaPosition.AreSameMember(m.Member, this.association.ThisMember.Member))
                {
                    Expression[] keyValues = GetKeyValues(this.Visit(m.Expression), this.association.ThisKey);
                    return Translator.WhereClauseFromSourceAndKeys(this.otherSouce, this.association.OtherKey.ToArray<MetaDataMember>(), keyValues);
                }
                Expression expression = this.Visit(m.Expression);
                if (expression == m.Expression)
                {
                    return m;
                }
                if (((expression.Type != m.Expression.Type) && (m.Member.Name == "Count")) && TypeSystem.IsSequenceType(expression.Type))
                {
                    return Expression.Call(typeof(Enumerable), "Count", new Type[] { TypeSystem.GetElementType(expression.Type) }, new Expression[] { expression });
                }
                return Expression.MakeMemberAccess(expression, m.Member);
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                if (p == this.parameter)
                {
                    return this.parameterReplacement;
                }
                return base.VisitParameter(p);
            }
        }
    }
}

