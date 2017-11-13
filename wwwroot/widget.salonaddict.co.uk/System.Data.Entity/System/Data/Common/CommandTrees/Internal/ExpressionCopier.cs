namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal class ExpressionCopier : DbExpressionVisitor<DbExpression>
    {
        private DbCommandTree _commandTree;
        private MetadataMapper _mapper;

        protected ExpressionCopier(DbCommandTree commandTree, MetadataMapper mapper)
        {
            this._commandTree = commandTree;
            this._mapper = mapper;
        }

        internal static DbExpression Copy(DbCommandTree destination, DbExpression expression)
        {
            using (new EntityBid.ScopeAuto("<cqti.ExpressionCopier.Copy|API> destination=%d#", destination.ObjectId))
            {
                EntityBid.Trace("<cqti.ExpressionCopier.Copy|INFO> expression=%d#, %d{cqt.DbExpressionKind}\n", DbExpression.GetObjectId(expression), DbExpression.GetExpressionKind(expression));
                ExpressionCopier copier = new ExpressionCopier(destination, GetMapper(expression.CommandTree.MetadataWorkspace, destination.MetadataWorkspace));
                return copier.VisitExpr(expression);
            }
        }

        private static MetadataMapper GetMapper(MetadataWorkspace source, MetadataWorkspace destination)
        {
            ItemCollection itemCollection = source.GetItemCollection(DataSpace.CSSpace);
            ItemCollection objB = destination.GetItemCollection(DataSpace.CSSpace);
            if (!object.ReferenceEquals(itemCollection, objB))
            {
                EntityBid.Trace("<cqti.ExpressionCopier.Copy|INFO> Using cross-workspace metadata mapper\n");
                return new WorkspaceMapper(destination);
            }
            EntityBid.Trace("<cqti.ExpressionCopier.Copy|INFO> Using identity metadata mapper\n");
            return MetadataMapper.IdentityMapper;
        }

        public override DbExpression Visit(DbAndExpression e) => 
            this._commandTree.CreateAndExpression(this.VisitExpr(e.Left), this.VisitExpr(e.Right));

        public override DbExpression Visit(DbApplyExpression e) => 
            this._commandTree.CreateApplyExpressionByKind(e.ExpressionKind, this.VisitBinding(e.Input), this.VisitBinding(e.Apply));

        public override DbExpression Visit(DbArithmeticExpression e)
        {
            List<DbExpression> list = this.VisitExprList(e.Arguments);
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.Minus:
                    return this._commandTree.CreateMinusExpression(list[0], list[1]);

                case DbExpressionKind.Modulo:
                    return this._commandTree.CreateModuloExpression(list[0], list[1]);

                case DbExpressionKind.Multiply:
                    return this._commandTree.CreateMultiplyExpression(list[0], list[1]);

                case DbExpressionKind.Divide:
                    return this._commandTree.CreateDivideExpression(list[0], list[1]);

                case DbExpressionKind.Plus:
                    return this._commandTree.CreatePlusExpression(list[0], list[1]);

                case DbExpressionKind.UnaryMinus:
                    return this._commandTree.CreateUnaryMinusExpression(list[0]);
            }
            return null;
        }

        public override DbExpression Visit(DbCaseExpression e) => 
            this._commandTree.CreateCaseExpression(this.VisitExprList(e.When), this.VisitExprList(e.Then), this.VisitExpr(e.Else));

        public override DbExpression Visit(DbCastExpression e) => 
            this._commandTree.CreateCastExpression(this.VisitExpr(e.Argument), this._mapper.Map(e.ResultType));

        public override DbExpression Visit(DbComparisonExpression e)
        {
            DbExpression left = this.VisitExpr(e.Left);
            DbExpression right = this.VisitExpr(e.Right);
            switch (e.ExpressionKind)
            {
                case DbExpressionKind.GreaterThan:
                    return this._commandTree.CreateGreaterThanExpression(left, right);

                case DbExpressionKind.GreaterThanOrEquals:
                    return this._commandTree.CreateGreaterThanOrEqualsExpression(left, right);

                case DbExpressionKind.Equals:
                    return this._commandTree.CreateEqualsExpression(left, right);

                case DbExpressionKind.LessThan:
                    return this._commandTree.CreateLessThanExpression(left, right);

                case DbExpressionKind.LessThanOrEquals:
                    return this._commandTree.CreateLessThanOrEqualsExpression(left, right);

                case DbExpressionKind.NotEquals:
                    return this._commandTree.CreateNotEqualsExpression(left, right);
            }
            return null;
        }

        public override DbExpression Visit(DbConstantExpression e) => 
            this._commandTree.CreateConstantExpression(e.Value, this._mapper.Map(e.ResultType));

        public override DbExpression Visit(DbCrossJoinExpression e) => 
            this._commandTree.CreateCrossJoinExpression(this.VisitBindingList(e.Inputs));

        public override DbExpression Visit(DbDerefExpression e) => 
            this._commandTree.CreateDerefExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbDistinctExpression e) => 
            this._commandTree.CreateDistinctExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbElementExpression e)
        {
            DbExpression argument = this.VisitExpr(e.Argument);
            if (e.IsSinglePropertyUnwrapped)
            {
                return this._commandTree.CreateElementExpressionUnwrapSingleProperty(argument);
            }
            return this._commandTree.CreateElementExpression(argument);
        }

        public override DbExpression Visit(DbEntityRefExpression e) => 
            this._commandTree.CreateEntityRefExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbExceptExpression e) => 
            this._commandTree.CreateExceptExpression(this.VisitExpr(e.Left), this.VisitExpr(e.Right));

        public override DbExpression Visit(DbExpression e)
        {
            throw EntityUtil.NotSupported(Strings.Cqt_General_UnsupportedExpression(e.GetType().FullName));
        }

        public override DbExpression Visit(DbFilterExpression e) => 
            this._commandTree.CreateFilterExpression(this.VisitBinding(e.Input), this.VisitExpr(e.Predicate));

        public override DbExpression Visit(DbFunctionExpression e)
        {
            if (!e.IsLambda)
            {
                return this._commandTree.CreateFunctionExpression(this._mapper.Map(e.Function), this.VisitExprList(e.Arguments));
            }
            List<KeyValuePair<string, TypeUsage>> formals = new List<KeyValuePair<string, TypeUsage>>();
            for (int i = 0; i < e.Function.Parameters.Count; i++)
            {
                FunctionParameter parameter = e.Function.Parameters[i];
                formals.Add(new KeyValuePair<string, TypeUsage>(parameter.Name, this._mapper.Map(parameter.TypeUsage)));
            }
            return this._commandTree.CreateLambdaFunctionExpression(formals, this.VisitExpr(e.LambdaBody), this.VisitExprList(e.Arguments));
        }

        public override DbExpression Visit(DbGroupByExpression e)
        {
            List<KeyValuePair<string, DbExpression>> keys = new List<KeyValuePair<string, DbExpression>>();
            List<KeyValuePair<string, DbAggregate>> aggregates = new List<KeyValuePair<string, DbAggregate>>();
            ReadOnlyMetadataCollection<EdmProperty> properties = TypeHelpers.GetEdmType<RowType>(TypeHelpers.GetEdmType<CollectionType>(this._mapper.Map(e.ResultType)).TypeUsage).Properties;
            int num = 0;
            for (int i = 0; i < e.Keys.Count; i++)
            {
                keys.Add(new KeyValuePair<string, DbExpression>(properties[num].Name, this.VisitExpr(e.Keys[i])));
                num++;
            }
            for (int j = 0; j < e.Aggregates.Count; j++)
            {
                aggregates.Add(new KeyValuePair<string, DbAggregate>(properties[num].Name, this.VisitAggregate(e.Aggregates[j])));
                num++;
            }
            return this._commandTree.CreateGroupByExpression(this.VisitGroupExpressionBinding(e.Input), keys, aggregates);
        }

        public override DbExpression Visit(DbIntersectExpression e) => 
            this._commandTree.CreateIntersectExpression(this.VisitExpr(e.Left), this.VisitExpr(e.Right));

        public override DbExpression Visit(DbIsEmptyExpression e) => 
            this._commandTree.CreateIsEmptyExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbIsNullExpression e)
        {
            DbExpression argument = this.VisitExpr(e.Argument);
            return this._commandTree.CreateIsNullExpression(argument);
        }

        public override DbExpression Visit(DbIsOfExpression e)
        {
            if (DbExpressionKind.IsOfOnly == e.ExpressionKind)
            {
                return this._commandTree.CreateIsOfOnlyExpression(this.VisitExpr(e.Argument), this._mapper.Map(e.OfType));
            }
            return this._commandTree.CreateIsOfExpression(this.VisitExpr(e.Argument), this._mapper.Map(e.OfType));
        }

        public override DbExpression Visit(DbJoinExpression e) => 
            this._commandTree.CreateJoinExpressionByKind(e.ExpressionKind, this.VisitExpr(e.JoinCondition), this.VisitBinding(e.Left), this.VisitBinding(e.Right));

        public override DbExpression Visit(DbLikeExpression e) => 
            this._commandTree.CreateLikeExpression(this.VisitExpr(e.Argument), this.VisitExpr(e.Pattern), this.VisitExpr(e.Escape));

        public override DbExpression Visit(DbLimitExpression e)
        {
            DbExpression argument = this.VisitExpr(e.Argument);
            DbExpression limit = this.VisitExpr(e.Limit);
            if (e.WithTies)
            {
                return this._commandTree.CreateLimitWithTiesExpression(argument, limit);
            }
            return this._commandTree.CreateLimitExpression(argument, limit);
        }

        public override DbExpression Visit(DbNewInstanceExpression e)
        {
            TypeUsage type = this._mapper.Map(e.ResultType);
            List<DbExpression> args = this.VisitExprList(e.Arguments);
            if ((type.EdmType.BuiltInTypeKind != BuiltInTypeKind.EntityType) || !e.HasRelatedEntityReferences)
            {
                return this._commandTree.CreateNewInstanceExpression(type, args);
            }
            List<DbRelatedEntityRef> relationships = new List<DbRelatedEntityRef>(e.RelatedEntityReferences.Count);
            for (int i = 0; i < e.RelatedEntityReferences.Count; i++)
            {
                DbRelatedEntityRef ref2 = e.RelatedEntityReferences[i];
                RelationshipEndMember sourceEnd = this._mapper.Map(ref2.SourceEnd);
                RelationshipEndMember targetEnd = this._mapper.Map(ref2.TargetEnd);
                DbExpression targetEntity = this.VisitExpr(ref2.TargetEntityReference);
                relationships.Add(this._commandTree.CreateRelatedEntityRef(sourceEnd, targetEnd, targetEntity));
            }
            return this._commandTree.CreateNewEntityWithRelationshipsExpression((EntityType) type.EdmType, args, relationships);
        }

        public override DbExpression Visit(DbNotExpression e) => 
            this._commandTree.CreateNotExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbNullExpression e) => 
            this._commandTree.CreateNullExpression(this._mapper.Map(e.ResultType));

        public override DbExpression Visit(DbOfTypeExpression e)
        {
            DbExpression argument = this.VisitExpr(e.Argument);
            TypeUsage ofType = this._mapper.Map(e.OfType);
            if (DbExpressionKind.OfTypeOnly == e.ExpressionKind)
            {
                return this._commandTree.CreateOfTypeOnlyExpression(argument, ofType);
            }
            return this._commandTree.CreateOfTypeExpression(argument, ofType);
        }

        public override DbExpression Visit(DbOrExpression e) => 
            this._commandTree.CreateOrExpression(this.VisitExpr(e.Left), this.VisitExpr(e.Right));

        public override DbExpression Visit(DbParameterReferenceExpression e) => 
            this._commandTree.CreateParameterReferenceExpression(e.ParameterName);

        public override DbExpression Visit(DbProjectExpression e) => 
            this._commandTree.CreateProjectExpression(this.VisitBinding(e.Input), this.VisitExpr(e.Projection));

        public override DbExpression Visit(DbPropertyExpression e)
        {
            DbExpression instance = this.VisitExpr(e.Instance);
            if (Helper.IsNavigationProperty(e.Property))
            {
                return this._commandTree.CreatePropertyExpression(this._mapper.Map((NavigationProperty) e.Property), instance);
            }
            if (Helper.IsEdmProperty(e.Property))
            {
                return this._commandTree.CreatePropertyExpression(this._mapper.Map((EdmProperty) e.Property), instance);
            }
            return this._commandTree.CreatePropertyExpression(this._mapper.Map((RelationshipEndMember) e.Property), instance);
        }

        public override DbExpression Visit(DbQuantifierExpression e)
        {
            DbExpressionBinding input = this.VisitBinding(e.Input);
            DbExpression predicate = this.VisitExpr(e.Predicate);
            if (e.ExpressionKind == DbExpressionKind.Any)
            {
                return this._commandTree.CreateAnyExpression(input, predicate);
            }
            return this._commandTree.CreateAllExpression(input, predicate);
        }

        public override DbExpression Visit(DbRefExpression e)
        {
            EntityTypeBase elementType = TypeHelpers.GetEdmType<RefType>(e.ResultType).ElementType;
            return this._commandTree.CreateRefExpression(this._mapper.Map(e.EntitySet), this.VisitExpr(e.Argument), (EntityType) this._mapper.Map(elementType));
        }

        public override DbExpression Visit(DbRefKeyExpression e) => 
            this._commandTree.CreateRefKeyExpression(this.VisitExpr(e.Argument));

        public override DbExpression Visit(DbRelationshipNavigationExpression e)
        {
            RelationshipEndMember fromEnd = this._mapper.Map(e.NavigateFrom);
            RelationshipEndMember toEnd = this._mapper.Map(e.NavigateTo);
            return this._commandTree.CreateRelationshipNavigationExpression(fromEnd, toEnd, this.VisitExpr(e.NavigationSource));
        }

        public override DbExpression Visit(DbScanExpression e) => 
            this._commandTree.CreateScanExpression(this._mapper.Map(e.Target));

        public override DbExpression Visit(DbSkipExpression e) => 
            this._commandTree.CreateSkipExpression(this.VisitBinding(e.Input), this.VisitSortOrder(e.SortOrder), this.VisitExpr(e.Count));

        public override DbExpression Visit(DbSortExpression e) => 
            this._commandTree.CreateSortExpression(this.VisitBinding(e.Input), this.VisitSortOrder(e.SortOrder));

        public override DbExpression Visit(DbTreatExpression e) => 
            this._commandTree.CreateTreatExpression(this.VisitExpr(e.Argument), this._mapper.Map(e.ResultType));

        public override DbExpression Visit(DbUnionAllExpression e) => 
            this._commandTree.CreateUnionAllExpression(this.VisitExpr(e.Left), this.VisitExpr(e.Right));

        public override DbExpression Visit(DbVariableReferenceExpression e) => 
            this._commandTree.CreateVariableReferenceExpression(e.VariableName, this._mapper.Map(e.ResultType));

        public DbAggregate VisitAggregate(DbAggregate a)
        {
            DbFunctionAggregate aggregate = a as DbFunctionAggregate;
            if (aggregate != null)
            {
                return this.VisitFunctionAggregate(aggregate);
            }
            return null;
        }

        public DbExpressionBinding VisitBinding(DbExpressionBinding b)
        {
            DbExpression input = this.VisitExpr(b.Expression);
            return this._commandTree.CreateExpressionBinding(input, b.VariableName);
        }

        public List<DbExpressionBinding> VisitBindingList(IList<DbExpressionBinding> bindingList)
        {
            List<DbExpressionBinding> list = null;
            if (bindingList != null)
            {
                list = new List<DbExpressionBinding>();
                for (int i = 0; i < bindingList.Count; i++)
                {
                    list.Add(this.VisitBinding(bindingList[i]));
                }
            }
            return list;
        }

        public DbExpression VisitExpr(DbExpression expr) => 
            expr?.Accept<DbExpression>(this);

        public List<DbExpression> VisitExprList(IList<DbExpression> exprList) => 
            this.VisitList<DbExpression>(exprList);

        public virtual DbFunctionAggregate VisitFunctionAggregate(DbFunctionAggregate a)
        {
            if (a.Distinct)
            {
                return this._commandTree.CreateDistinctFunctionAggregate(this._mapper.Map(a.Function), this.VisitExpr(a.Arguments[0]));
            }
            return this._commandTree.CreateFunctionAggregate(this._mapper.Map(a.Function), this.VisitExpr(a.Arguments[0]));
        }

        public DbGroupExpressionBinding VisitGroupExpressionBinding(DbGroupExpressionBinding gb)
        {
            DbExpression input = this.VisitExpr(gb.Expression);
            return this._commandTree.CreateGroupExpressionBinding(input, gb.VariableName, gb.GroupVariableName);
        }

        public List<T> VisitList<T>(IList<T> exprList) where T: DbExpression
        {
            List<T> list = null;
            if (exprList != null)
            {
                list = new List<T>();
                for (int i = 0; i < exprList.Count; i++)
                {
                    list.Add((T) this.VisitExpr(exprList[i]));
                }
            }
            return list;
        }

        public List<DbSortClause> VisitSortOrder(IList<DbSortClause> sortOrder)
        {
            List<DbSortClause> list = new List<DbSortClause>();
            for (int i = 0; i < sortOrder.Count; i++)
            {
                DbSortClause clause = sortOrder[i];
                DbExpression key = this.VisitExpr(clause.Expression);
                bool ascending = clause.Ascending;
                if (!string.IsNullOrEmpty(clause.Collation))
                {
                    list.Add(this._commandTree.CreateSortClause(key, ascending, clause.Collation));
                }
                else
                {
                    list.Add(this._commandTree.CreateSortClause(key, ascending));
                }
            }
            return list;
        }

        protected DbCommandTree CommandTree =>
            this._commandTree;

        protected class MetadataMapper
        {
            internal static readonly ExpressionCopier.MetadataMapper IdentityMapper = new ExpressionCopier.MetadataMapper();

            internal MetadataMapper()
            {
            }

            internal virtual EdmFunction Map(EdmFunction f) => 
                f;

            internal virtual EdmProperty Map(EdmProperty p) => 
                p;

            internal virtual EdmType Map(EdmType s) => 
                s;

            internal virtual EntitySet Map(EntitySet e) => 
                e;

            internal virtual EntitySetBase Map(EntitySetBase e) => 
                e;

            internal virtual NavigationProperty Map(NavigationProperty p) => 
                p;

            internal virtual RelationshipEndMember Map(RelationshipEndMember m) => 
                m;

            internal virtual TypeUsage Map(TypeUsage t) => 
                t;
        }

        private class WorkspaceMapper : ExpressionCopier.MetadataMapper
        {
            private MetadataWorkspace _metadata;
            private Dictionary<string, TypeUsage> _transientTypes = new Dictionary<string, TypeUsage>();

            internal WorkspaceMapper(MetadataWorkspace destinationWorkspace)
            {
                this._metadata = destinationWorkspace;
            }

            internal override EdmFunction Map(EdmFunction f)
            {
                TypeUsage[] parameterTypes = new TypeUsage[f.Parameters.Count];
                int index = 0;
                foreach (FunctionParameter parameter in f.Parameters)
                {
                    parameterTypes[index] = this.Map(parameter.TypeUsage);
                    index++;
                }
                EdmFunction function = null;
                if (!this._metadata.TryGetFunction(f.Name, f.NamespaceName, parameterTypes, false, f.DataSpace, out function) || (function == null))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_FunctionNotFound(TypeHelpers.GetFullName(f)));
                }
                return function;
            }

            internal override EdmProperty Map(EdmProperty prop)
            {
                EdmProperty property;
                if (!this.TryMapMember<EdmProperty>(prop, out property))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_PropertyNotFound(prop.Name, TypeHelpers.GetFullName(prop.DeclaringType)));
                }
                return property;
            }

            internal override EdmType Map(EdmType type)
            {
                EdmType type2 = null;
                if (BuiltInTypeKind.RefType == type.BuiltInTypeKind)
                {
                    RefType type3 = (RefType) type;
                    return new RefType((EntityType) this.Map(type3.ElementType));
                }
                if (BuiltInTypeKind.CollectionType == type.BuiltInTypeKind)
                {
                    CollectionType type4 = (CollectionType) type;
                    return new CollectionType(this.Map(type4.TypeUsage));
                }
                if (BuiltInTypeKind.RowType == type.BuiltInTypeKind)
                {
                    RowType type5 = (RowType) type;
                    List<EdmProperty> properties = new List<EdmProperty>();
                    foreach (EdmProperty property in type5.Properties)
                    {
                        properties.Add(new EdmProperty(property.Name, this.Map(property.TypeUsage)));
                    }
                    return new RowType(properties, type5.InitializerMetadata);
                }
                if (!this._metadata.TryGetType(type.Name, type.NamespaceName, type.DataSpace, out type2) || (type2 == null))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_TypeNotFound(TypeHelpers.GetFullName(type)));
                }
                return type2;
            }

            internal override EntitySet Map(EntitySet e)
            {
                EntityContainer container;
                if (!this._metadata.TryGetEntityContainer(e.EntityContainer.Name, e.EntityContainer.DataSpace, out container))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_EntityContainerNotFound(e.EntityContainer.Name));
                }
                EntitySetBase item = null;
                if (container.BaseEntitySets.TryGetValue(e.Name, false, out item))
                {
                    EntitySet set = item as EntitySet;
                    if (set != null)
                    {
                        return set;
                    }
                }
                throw EntityUtil.Argument(Strings.Cqt_Copier_EntitySetNotFound(e.EntityContainer.Name, e.Name));
            }

            internal override EntitySetBase Map(EntitySetBase e)
            {
                EntityContainer container;
                if (!this._metadata.TryGetEntityContainer(e.EntityContainer.Name, e.EntityContainer.DataSpace, out container))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_EntityContainerNotFound(e.EntityContainer.Name));
                }
                EntitySetBase item = null;
                if (!container.BaseEntitySets.TryGetValue(e.Name, false, out item) || (item == null))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_EntitySetNotFound(e.EntityContainer.Name, e.Name));
                }
                return item;
            }

            internal override NavigationProperty Map(NavigationProperty prop)
            {
                NavigationProperty property;
                if (!this.TryMapMember<NavigationProperty>(prop, out property))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_PropertyNotFound(prop.Name, TypeHelpers.GetFullName(prop.DeclaringType)));
                }
                return property;
            }

            internal override RelationshipEndMember Map(RelationshipEndMember end)
            {
                RelationshipEndMember member;
                if (!this.TryMapMember<RelationshipEndMember>(end, out member))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Copier_EndNotFound(end.Name, TypeHelpers.GetFullName(end.DeclaringType)));
                }
                return member;
            }

            internal override TypeUsage Map(TypeUsage type)
            {
                EdmType objA = this.Map(type.EdmType);
                if (object.ReferenceEquals(objA, type.EdmType))
                {
                    return type;
                }
                Facet[] facets = new Facet[type.Facets.Count];
                int index = 0;
                foreach (Facet facet in type.Facets)
                {
                    facets[index] = facet;
                    index++;
                }
                return TypeUsage.Create(objA, facets);
            }

            private bool TryMapMember<TMemberType>(TMemberType prop, out TMemberType foundProp) where TMemberType: EdmMember
            {
                foundProp = default(TMemberType);
                StructuralType type = this.Map(prop.DeclaringType) as StructuralType;
                if (type != null)
                {
                    EdmMember item = null;
                    if (type.Members.TryGetValue(prop.Name, false, out item))
                    {
                        foundProp = item as TMemberType;
                    }
                }
                return (((TMemberType) foundProp) != null);
            }
        }
    }
}

