namespace System.Data.Objects.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class ObjectSpanRewriter
    {
        private MetadataWorkspace _metadata;
        private Stack<NavigationInfo> _navSources = new Stack<NavigationInfo>();
        private bool _relationshipSpan;
        private int _spanCount;
        private System.Data.Objects.Internal.SpanIndex _spanIndex;
        private DbExpression _toRewrite;

        internal ObjectSpanRewriter(DbExpression toRewrite)
        {
            this._toRewrite = toRewrite;
            this._metadata = toRewrite.CommandTree.MetadataWorkspace;
        }

        private void AddSpanMap(RowType rowType, Dictionary<int, AssociationEndMember> columnMap)
        {
            if (this._spanIndex == null)
            {
                this._spanIndex = new System.Data.Objects.Internal.SpanIndex();
            }
            this._spanIndex.AddSpanMap(rowType, columnMap);
        }

        private void AddSpannedRowType(RowType spannedType, TypeUsage originalType)
        {
            if (this._spanIndex == null)
            {
                this._spanIndex = new System.Data.Objects.Internal.SpanIndex();
            }
            this._spanIndex.AddSpannedRowType(spannedType, originalType);
        }

        internal virtual SpanTrackingInfo CreateEntitySpanTrackingInfo(DbExpression expression, EntityType entityType) => 
            new SpanTrackingInfo();

        private void EnterCollection()
        {
            this._navSources.Push(null);
        }

        private NavigationInfo EnterNavigationCollection(AssociationEndMember sourceEnd, DbExpression navSource)
        {
            NavigationInfo item = new NavigationInfo(sourceEnd, navSource);
            this._navSources.Push(item);
            return item;
        }

        internal static bool EntityTypeEquals(EntityTypeBase entityType1, EntityTypeBase entityType2) => 
            object.ReferenceEquals(entityType1, entityType2);

        private void ExitCollection()
        {
            this._navSources.Pop();
        }

        private List<KeyValuePair<AssociationEndMember, AssociationEndMember>> GetRelationshipSpanEnds(EntityType entityType)
        {
            List<KeyValuePair<AssociationEndMember, AssociationEndMember>> list = null;
            if (this._relationshipSpan)
            {
                foreach (AssociationType type in this._metadata.GetItems<AssociationType>(DataSpace.CSpace))
                {
                    if (2 == type.AssociationEndMembers.Count)
                    {
                        AssociationEndMember fromEnd = type.AssociationEndMembers[0];
                        AssociationEndMember toEnd = type.AssociationEndMembers[1];
                        if (IsValidRelationshipSpan(entityType, fromEnd, toEnd))
                        {
                            if (list == null)
                            {
                                list = new List<KeyValuePair<AssociationEndMember, AssociationEndMember>>();
                            }
                            list.Add(new KeyValuePair<AssociationEndMember, AssociationEndMember>(fromEnd, toEnd));
                        }
                        if (IsValidRelationshipSpan(entityType, toEnd, fromEnd))
                        {
                            if (list == null)
                            {
                                list = new List<KeyValuePair<AssociationEndMember, AssociationEndMember>>();
                            }
                            list.Add(new KeyValuePair<AssociationEndMember, AssociationEndMember>(toEnd, fromEnd));
                        }
                    }
                }
            }
            return list;
        }

        internal SpanTrackingInfo InitializeTrackingInfo(bool createAssociationEndTrackingInfo)
        {
            SpanTrackingInfo info = new SpanTrackingInfo {
                ColumnDefinitions = new List<KeyValuePair<string, DbExpression>>(),
                ColumnNames = new AliasGenerator(string.Format(CultureInfo.InvariantCulture, "Span{0}_Column", new object[] { this._spanCount })),
                SpannedColumns = new Dictionary<int, AssociationEndMember>()
            };
            if (createAssociationEndTrackingInfo)
            {
                info.FullSpannedEnds = new Dictionary<AssociationEndMember, bool>();
            }
            return info;
        }

        private static bool IsValidRelationshipSpan(EntityType compareType, AssociationEndMember fromEnd, AssociationEndMember toEnd)
        {
            if ((RelationshipMultiplicity.One != toEnd.RelationshipMultiplicity) && (toEnd.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne))
            {
                return false;
            }
            EntityType elementType = (EntityType) ((RefType) fromEnd.TypeUsage.EdmType).ElementType;
            if (!EntityTypeEquals(compareType, elementType) && !TypeSemantics.IsSubTypeOf(compareType, elementType))
            {
                return TypeSemantics.IsSubTypeOf(elementType, compareType);
            }
            return true;
        }

        protected DbExpression Rewrite(DbExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Element:
                    return this.RewriteElementExpression((DbElementExpression) expression);

                case DbExpressionKind.Limit:
                    return this.RewriteLimitExpression((DbLimitExpression) expression);
            }
            BuiltInTypeKind builtInTypeKind = expression.ResultType.EdmType.BuiltInTypeKind;
            if (builtInTypeKind == BuiltInTypeKind.CollectionType)
            {
                return this.RewriteCollection(expression, (CollectionType) expression.ResultType.EdmType);
            }
            if (builtInTypeKind != BuiltInTypeKind.EntityType)
            {
                if (builtInTypeKind == BuiltInTypeKind.RowType)
                {
                    return this.RewriteRow(expression, (RowType) expression.ResultType.EdmType);
                }
                return expression;
            }
            return this.RewriteEntity(expression, (EntityType) expression.ResultType.EdmType);
        }

        private DbExpression RewriteCollection(DbExpression expression, CollectionType collectionType)
        {
            DbProjectExpression expression2 = null;
            if (DbExpressionKind.Project == expression.ExpressionKind)
            {
                expression2 = (DbProjectExpression) expression;
            }
            DbRelationshipNavigationExpression expression3 = null;
            NavigationInfo info = null;
            if (this.RelationshipSpan)
            {
                if (expression2 != null)
                {
                    expression3 = RelationshipNavigationVisitor.FindNavigationExpression(expression2.Input.Expression);
                }
                else
                {
                    expression3 = RelationshipNavigationVisitor.FindNavigationExpression(expression);
                }
            }
            if (expression3 != null)
            {
                info = this.EnterNavigationCollection((AssociationEndMember) expression3.NavigateFrom, expression3.NavigationSource);
            }
            else
            {
                this.EnterCollection();
            }
            if (expression2 != null)
            {
                DbExpression objB = this.Rewrite(expression2.Projection);
                if (!object.ReferenceEquals(expression2.Projection, objB))
                {
                    expression = expression.CommandTree.CreateProjectExpression(expression2.Input, objB);
                }
            }
            else
            {
                DbExpressionBinding input = expression.CommandTree.CreateExpressionBinding(expression);
                DbExpression variable = input.Variable;
                DbExpression expression6 = this.Rewrite(variable);
                if (!object.ReferenceEquals(variable, expression6))
                {
                    expression = expression.CommandTree.CreateProjectExpression(input, expression6);
                }
            }
            this.ExitCollection();
            if ((info != null) && info.InUse)
            {
                expression3.NavigationSource = info.CreateSourceReference();
                List<KeyValuePair<string, TypeUsage>> formals = new List<KeyValuePair<string, TypeUsage>>(1) {
                    new KeyValuePair<string, TypeUsage>(info.SourceVariableName, info.Source.ResultType)
                };
                List<DbExpression> args = new List<DbExpression>(1) {
                    info.Source
                };
                expression = expression.CommandTree.CreateLambdaFunctionExpression(formals, expression, args);
            }
            return expression;
        }

        private DbExpression RewriteElementExpression(DbElementExpression expression)
        {
            DbExpression objB = this.Rewrite(expression.Argument);
            if (!object.ReferenceEquals(expression.Argument, objB))
            {
                expression = expression.CommandTree.CreateElementExpression(objB);
            }
            return expression;
        }

        private DbExpression RewriteEntity(DbExpression expression, EntityType entityType)
        {
            if (DbExpressionKind.NewInstance == expression.ExpressionKind)
            {
                return expression;
            }
            this._spanCount++;
            int num = this._spanCount;
            SpanTrackingInfo info = this.CreateEntitySpanTrackingInfo(expression, entityType);
            List<KeyValuePair<AssociationEndMember, AssociationEndMember>> relationshipSpanEnds = null;
            relationshipSpanEnds = this.GetRelationshipSpanEnds(entityType);
            if (relationshipSpanEnds != null)
            {
                if (info.ColumnDefinitions == null)
                {
                    info = this.InitializeTrackingInfo(false);
                }
                int num2 = info.ColumnDefinitions.Count + 1;
                foreach (KeyValuePair<AssociationEndMember, AssociationEndMember> pair in relationshipSpanEnds)
                {
                    if ((info.FullSpannedEnds == null) || !info.FullSpannedEnds.ContainsKey(pair.Value))
                    {
                        DbExpression source = null;
                        if (!this.TryGetNavigationSource(pair.Value, out source))
                        {
                            DbExpression from = expression.CommandTree.CreateEntityRefExpression(expression.Clone());
                            source = expression.CommandTree.CreateRelationshipNavigationExpression(pair.Key, pair.Value, from);
                        }
                        info.ColumnDefinitions.Add(new KeyValuePair<string, DbExpression>(info.ColumnNames.Next(), source));
                        info.SpannedColumns[num2] = pair.Value;
                        num2++;
                    }
                }
            }
            if (info.ColumnDefinitions == null)
            {
                this._spanCount--;
                return expression;
            }
            info.ColumnDefinitions.Insert(0, new KeyValuePair<string, DbExpression>(string.Format(CultureInfo.InvariantCulture, "Span{0}_SpanRoot", new object[] { num }), expression));
            DbExpression expression4 = expression.CommandTree.CreateNewRowExpression(info.ColumnDefinitions);
            RowType edmType = (RowType) expression4.ResultType.EdmType;
            this.AddSpanMap(edmType, info.SpannedColumns);
            return expression4;
        }

        private DbExpression RewriteLimitExpression(DbLimitExpression expression)
        {
            DbExpression objB = this.Rewrite(expression.Argument);
            if (!object.ReferenceEquals(expression.Argument, objB))
            {
                expression = expression.CommandTree.CreateLimitExpression(objB, expression.Limit);
            }
            return expression;
        }

        internal DbExpression RewriteQuery()
        {
            DbExpression objB = this.Rewrite(this._toRewrite);
            if (object.ReferenceEquals(this._toRewrite, objB))
            {
                return null;
            }
            return objB;
        }

        private DbExpression RewriteRow(DbExpression expression, RowType rowType)
        {
            DbNewInstanceExpression expression2 = expression as DbNewInstanceExpression;
            bool flag = false;
            Dictionary<int, DbExpression> dictionary = null;
            Dictionary<int, DbExpression> dictionary2 = null;
            for (int i = 0; i < rowType.Properties.Count; i++)
            {
                EdmProperty property = rowType.Properties[i];
                DbExpression expression3 = null;
                if (expression2 != null)
                {
                    expression3 = expression2.Arguments[i];
                }
                else
                {
                    DbExpression instance = null;
                    if (flag)
                    {
                        instance = expression.Clone();
                    }
                    else
                    {
                        instance = expression;
                        flag = true;
                    }
                    expression3 = instance.CommandTree.CreatePropertyExpression(property.Name, instance);
                }
                DbExpression objA = this.Rewrite(expression3);
                if (!object.ReferenceEquals(objA, expression3))
                {
                    if (dictionary2 == null)
                    {
                        dictionary2 = new Dictionary<int, DbExpression>();
                    }
                    dictionary2[i] = objA;
                }
                else
                {
                    if (dictionary == null)
                    {
                        dictionary = new Dictionary<int, DbExpression>();
                    }
                    dictionary[i] = expression3;
                }
            }
            if (dictionary2 == null)
            {
                return expression;
            }
            List<DbExpression> args = new List<DbExpression>(rowType.Properties.Count);
            List<EdmProperty> properties = new List<EdmProperty>(rowType.Properties.Count);
            for (int j = 0; j < rowType.Properties.Count; j++)
            {
                EdmProperty property2 = rowType.Properties[j];
                DbExpression expression6 = null;
                if (!dictionary2.TryGetValue(j, out expression6))
                {
                    expression6 = dictionary[j];
                }
                args.Add(expression6);
                properties.Add(new EdmProperty(property2.Name, expression6.ResultType));
            }
            RowType edmType = new RowType(properties, rowType.InitializerMetadata);
            TypeUsage type = TypeUsage.Create(edmType);
            DbExpression elseExpression = expression.CommandTree.CreateNewInstanceExpression(type, args);
            if (expression2 == null)
            {
                DbExpression expression8 = expression.CommandTree.CreateIsNullExpressionAllowingRowTypeArgument(expression.Clone());
                DbExpression expression9 = expression.CommandTree.CreateNullExpression(type);
                elseExpression = expression.CommandTree.CreateCaseExpression(new List<DbExpression>(new DbExpression[] { expression8 }), new List<DbExpression>(new DbExpression[] { expression9 }), elseExpression);
            }
            this.AddSpannedRowType(edmType, expression.ResultType);
            return elseExpression;
        }

        private bool TryGetNavigationSource(AssociationEndMember wasSourceNowTargetEnd, out DbExpression source)
        {
            source = null;
            NavigationInfo info = null;
            if (this._navSources.Count > 0)
            {
                info = this._navSources.Peek();
                if ((info != null) && !object.ReferenceEquals(wasSourceNowTargetEnd, info.SourceEnd))
                {
                    info = null;
                }
            }
            if (info != null)
            {
                source = info.CreateSourceReference();
                info.InUse = true;
                return true;
            }
            return false;
        }

        internal static bool TryRewrite(DbExpression query, Span span, MergeOption mergeOption, out DbExpression newQuery, out System.Data.Objects.Internal.SpanIndex spanInfo)
        {
            newQuery = null;
            spanInfo = null;
            ObjectSpanRewriter rewriter = null;
            bool flag = Span.RequiresRelationshipSpan(mergeOption);
            if ((span != null) && (span.SpanList.Count > 0))
            {
                rewriter = new ObjectFullSpanRewriter(query, span);
            }
            else if (flag)
            {
                rewriter = new ObjectSpanRewriter(query);
            }
            if (rewriter != null)
            {
                rewriter.RelationshipSpan = flag;
                newQuery = rewriter.RewriteQuery();
                if (newQuery != null)
                {
                    spanInfo = rewriter.SpanIndex;
                }
            }
            return (spanInfo != null);
        }

        internal MetadataWorkspace Metadata =>
            this._metadata;

        internal DbExpression Query =>
            this._toRewrite;

        internal bool RelationshipSpan
        {
            get => 
                this._relationshipSpan;
            set
            {
                this._relationshipSpan = value;
            }
        }

        internal System.Data.Objects.Internal.SpanIndex SpanIndex =>
            this._spanIndex;

        private class NavigationInfo
        {
            private DbExpression _source;
            private AssociationEndMember _sourceEnd;
            private string _sourceVarName;
            public bool InUse;

            public NavigationInfo(AssociationEndMember sourceEnd, DbExpression source)
            {
                this._sourceVarName = source.CommandTree.BindingAliases.Next();
                this._sourceEnd = sourceEnd;
                this._source = source;
            }

            public DbExpression CreateSourceReference() => 
                this.Source.CommandTree.CreateVariableReferenceExpression(this._sourceVarName, this.Source.ResultType);

            public DbExpression Source =>
                this._source;

            public AssociationEndMember SourceEnd =>
                this._sourceEnd;

            public string SourceVariableName =>
                this._sourceVarName;
        }

        private class RelationshipNavigationVisitor : DbExpressionVisitor<DbRelationshipNavigationExpression>
        {
            private DbRelationshipNavigationExpression Find(DbExpression expression) => 
                expression.Accept<DbRelationshipNavigationExpression>(this);

            internal static DbRelationshipNavigationExpression FindNavigationExpression(DbExpression expression)
            {
                TypeUsage typeUsage = ((CollectionType) expression.ResultType.EdmType).TypeUsage;
                if (!TypeSemantics.IsEntityType(typeUsage) && !TypeSemantics.IsReferenceType(typeUsage))
                {
                    return null;
                }
                ObjectSpanRewriter.RelationshipNavigationVisitor visitor = new ObjectSpanRewriter.RelationshipNavigationVisitor();
                return visitor.Find(expression);
            }

            public override DbRelationshipNavigationExpression Visit(DbAndExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbApplyExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbArithmeticExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbCaseExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbCastExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbComparisonExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbConstantExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbCrossJoinExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbDerefExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbDistinctExpression expression) => 
                this.Find(expression.Argument);

            public override DbRelationshipNavigationExpression Visit(DbElementExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbEntityRefExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbExceptExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbExpression expression)
            {
                throw EntityUtil.NotSupported();
            }

            public override DbRelationshipNavigationExpression Visit(DbFilterExpression expression) => 
                this.Find(expression.Input.Expression);

            public override DbRelationshipNavigationExpression Visit(DbFunctionExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbGroupByExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbIntersectExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbIsEmptyExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbIsNullExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbIsOfExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbJoinExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbLikeExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbLimitExpression expression) => 
                this.Find(expression.Argument);

            public override DbRelationshipNavigationExpression Visit(DbNewInstanceExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbNotExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbNullExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbOfTypeExpression expression) => 
                this.Find(expression.Argument);

            public override DbRelationshipNavigationExpression Visit(DbOrExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbParameterReferenceExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbProjectExpression expression)
            {
                DbExpression projection = expression.Projection;
                if (DbExpressionKind.Deref == projection.ExpressionKind)
                {
                    projection = ((DbDerefExpression) projection).Argument;
                }
                if (DbExpressionKind.VariableReference == projection.ExpressionKind)
                {
                    DbVariableReferenceExpression expression3 = (DbVariableReferenceExpression) projection;
                    if (expression3.VariableName.Equals(expression.Input.VariableName, StringComparison.Ordinal))
                    {
                        return this.Find(expression.Input.Expression);
                    }
                }
                return null;
            }

            public override DbRelationshipNavigationExpression Visit(DbPropertyExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbQuantifierExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbRefExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbRefKeyExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbRelationshipNavigationExpression expression) => 
                expression;

            public override DbRelationshipNavigationExpression Visit(DbScanExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbSkipExpression expression) => 
                this.Find(expression.Input.Expression);

            public override DbRelationshipNavigationExpression Visit(DbSortExpression expression) => 
                this.Find(expression.Input.Expression);

            public override DbRelationshipNavigationExpression Visit(DbTreatExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbUnionAllExpression expression) => 
                null;

            public override DbRelationshipNavigationExpression Visit(DbVariableReferenceExpression expression) => 
                null;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SpanTrackingInfo
        {
            public List<KeyValuePair<string, DbExpression>> ColumnDefinitions;
            public AliasGenerator ColumnNames;
            public Dictionary<int, AssociationEndMember> SpannedColumns;
            public Dictionary<AssociationEndMember, bool> FullSpannedEnds;
        }
    }
}

