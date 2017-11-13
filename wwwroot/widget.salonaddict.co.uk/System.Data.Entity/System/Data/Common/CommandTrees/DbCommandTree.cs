namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public abstract class DbCommandTree
    {
        private AliasGenerator _bindingAliases;
        private List<EntityContainer> _containers = new List<EntityContainer>();
        private int _currentVersion = 1;
        private System.Data.Metadata.Edm.DataSpace _dataSpace;
        private DbConstantExpression _falseExpr;
        private System.Data.Metadata.Edm.MetadataWorkspace _metadata;
        private Dictionary<string, KeyValuePair<string, TypeUsage>> _paramMappings = new Dictionary<string, KeyValuePair<string, TypeUsage>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Regex _paramNameRegex = new Regex("^([A-Za-z])([A-Za-z0-9_])*$");
        private DbConstantExpression _trueExpr;
        private CommandTreeTypeHelper _typeHelp;
        private int _validatedVersion;
        internal readonly int ObjectId = Interlocked.Increment(ref s_instanceCount);
        private static int s_instanceCount;

        internal DbCommandTree(System.Data.Metadata.Edm.MetadataWorkspace metadata, System.Data.Metadata.Edm.DataSpace dataSpace)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.ctor|API> %d#", this.ObjectId))
            {
                ItemCollection items;
                if (metadata == null)
                {
                    throw EntityUtil.ArgumentNull("metadata");
                }
                if (!CommandTreeUtils.IsValidDataSpace(dataSpace))
                {
                    throw EntityUtil.Argument(Strings.Cqt_CommandTree_InvalidDataSpace, "dataSpace");
                }
                this._metadata = new System.Data.Metadata.Edm.MetadataWorkspace();
                if (metadata.TryGetItemCollection(System.Data.Metadata.Edm.DataSpace.OSpace, out items))
                {
                    this._metadata.RegisterItemCollection(items);
                }
                this._metadata.RegisterItemCollection(metadata.GetItemCollection(System.Data.Metadata.Edm.DataSpace.CSpace));
                this._metadata.RegisterItemCollection(metadata.GetItemCollection(System.Data.Metadata.Edm.DataSpace.CSSpace));
                this._metadata.RegisterItemCollection(metadata.GetItemCollection(System.Data.Metadata.Edm.DataSpace.SSpace));
                this._dataSpace = dataSpace;
                this._typeHelp = new CommandTreeTypeHelper(this);
            }
        }

        internal void AddParameter(string name, TypeUsage type)
        {
            EntityUtil.CheckArgumentNull<string>(name, "name");
            this.TypeHelper.CheckType(type);
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.AddParameter|API> %d#", this.ObjectId))
            {
                KeyValuePair<string, TypeUsage> pair;
                EntityBid.Trace("<cqt.DbCommandTree.AddParameter|INFO> %d#, name='%ls'\n", this.ObjectId, name);
                EntityBid.Trace("<cqt.DbCommandTree.AddParameter|INFO> %d#, type='%ls'\n", this.ObjectId, TypeHelpers.GetStrongName(type));
                if (!IsValidParameterName(name))
                {
                    throw EntityUtil.Argument(Strings.Cqt_CommandTree_InvalidParameterName(name), "name");
                }
                if (this._paramMappings.TryGetValue(name, out pair))
                {
                    if (!TypeSemantics.IsStructurallyEqualTo(type, pair.Value))
                    {
                        throw EntityUtil.Argument(Strings.Cqt_CommandTree_ParameterExists, "type");
                    }
                }
                else
                {
                    this._paramMappings.Add(name, new KeyValuePair<string, TypeUsage>(name, type));
                }
            }
        }

        internal void ClearParameters()
        {
            EntityBid.Trace("<cqt.DbCommandTree.ClearParameters|API> %d#\n", this.ObjectId);
            this._paramMappings.Clear();
        }

        internal void CopyParametersTo(DbCommandTree tree)
        {
            foreach (KeyValuePair<string, TypeUsage> pair in this._paramMappings.Values)
            {
                tree.AddParameter(pair.Key, pair.Value);
            }
        }

        internal DbQuantifierExpression CreateAllExpression(DbExpressionBinding input, DbExpression predicate) => 
            new DbQuantifierExpression(this, DbExpressionKind.All, input, predicate);

        internal DbAndExpression CreateAndExpression(DbExpression left, DbExpression right) => 
            new DbAndExpression(this, left, right);

        internal DbQuantifierExpression CreateAnyExpression(DbExpressionBinding input, DbExpression predicate) => 
            new DbQuantifierExpression(this, DbExpressionKind.Any, input, predicate);

        internal DbApplyExpression CreateApplyExpressionByKind(DbExpressionKind applyKind, DbExpressionBinding input, DbExpressionBinding apply)
        {
            DbExpressionKind kind = applyKind;
            if (kind != DbExpressionKind.CrossApply)
            {
                if (kind != DbExpressionKind.OuterApply)
                {
                    throw EntityUtil.InvalidEnumerationValue(typeof(DbExpressionKind), (int) applyKind);
                }
                return this.CreateOuterApplyExpression(input, apply);
            }
            return this.CreateCrossApplyExpression(input, apply);
        }

        internal DbCaseExpression CreateCaseExpression(IList<DbExpression> whenExpressions, IList<DbExpression> thenExpressions, DbExpression elseExpression) => 
            new DbCaseExpression(this, whenExpressions, thenExpressions, elseExpression);

        internal DbCastExpression CreateCastExpression(DbExpression argument, TypeUsage toType) => 
            new DbCastExpression(this, toType, argument);

        internal DbConstantExpression CreateConstantExpression(object value) => 
            new DbConstantExpression(this, value);

        internal DbConstantExpression CreateConstantExpression(object value, TypeUsage constantType) => 
            new DbConstantExpression(this, value, constantType);

        internal DbApplyExpression CreateCrossApplyExpression(DbExpressionBinding input, DbExpressionBinding apply) => 
            new DbApplyExpression(this, input, apply, DbExpressionKind.CrossApply);

        internal DbCrossJoinExpression CreateCrossJoinExpression(IList<DbExpressionBinding> inputs) => 
            new DbCrossJoinExpression(this, inputs);

        internal DbDerefExpression CreateDerefExpression(DbExpression reference) => 
            new DbDerefExpression(this, reference);

        internal DbDistinctExpression CreateDistinctExpression(DbExpression argument) => 
            new DbDistinctExpression(this, argument);

        internal DbFunctionAggregate CreateDistinctFunctionAggregate(EdmFunction function, DbExpression argument) => 
            new DbFunctionAggregate(this, function, argument, true);

        internal DbArithmeticExpression CreateDivideExpression(DbExpression left, DbExpression right) => 
            new DbArithmeticExpression(this, DbExpressionKind.Divide, CommandTreeUtils.CreateList<DbExpression>(left, right));

        internal DbElementExpression CreateElementExpression(DbExpression argument) => 
            new DbElementExpression(this, argument, false);

        internal DbElementExpression CreateElementExpressionUnwrapSingleProperty(DbExpression argument) => 
            new DbElementExpression(this, argument, true);

        internal DbEntityRefExpression CreateEntityRefExpression(DbExpression entity) => 
            new DbEntityRefExpression(this, entity);

        internal DbComparisonExpression CreateEqualsExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.Equals, left, right);

        internal DbExceptExpression CreateExceptExpression(DbExpression left, DbExpression right) => 
            new DbExceptExpression(this, left, right);

        internal DbExpressionBinding CreateExpressionBinding(DbExpression input) => 
            new DbExpressionBinding(this, input, this.BindingAliases.Next());

        internal DbExpressionBinding CreateExpressionBinding(DbExpression input, string varName) => 
            new DbExpressionBinding(this, input, varName);

        internal DbConstantExpression CreateFalseExpression()
        {
            if (this._falseExpr == null)
            {
                this._falseExpr = this.CreateConstantExpression(false, this.TypeHelper.CreateBooleanResultType());
            }
            return this._falseExpr;
        }

        internal DbFilterExpression CreateFilterExpression(DbExpressionBinding input, DbExpression predicate) => 
            new DbFilterExpression(this, input, predicate);

        internal DbJoinExpression CreateFullOuterJoinExpression(DbExpressionBinding left, DbExpressionBinding right, DbExpression joinCondition) => 
            new DbJoinExpression(this, DbExpressionKind.FullOuterJoin, left, right, joinCondition);

        internal DbFunctionAggregate CreateFunctionAggregate(EdmFunction function, DbExpression argument) => 
            new DbFunctionAggregate(this, function, argument, false);

        internal DbFunctionExpression CreateFunctionExpression(EdmFunction function, IList<DbExpression> args) => 
            new DbFunctionExpression(this, function, null, args);

        internal DbComparisonExpression CreateGreaterThanExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.GreaterThan, left, right);

        internal DbComparisonExpression CreateGreaterThanOrEqualsExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.GreaterThanOrEquals, left, right);

        internal DbGroupByExpression CreateGroupByExpression(DbGroupExpressionBinding input, IList<KeyValuePair<string, DbExpression>> keys, IList<KeyValuePair<string, DbAggregate>> aggregates) => 
            new DbGroupByExpression(this, input, keys, aggregates);

        internal DbGroupExpressionBinding CreateGroupExpressionBinding(DbExpression input)
        {
            string varName = this.BindingAliases.Next();
            return new DbGroupExpressionBinding(this, input, varName, string.Format(CultureInfo.InvariantCulture, "Group{0}", new object[] { varName }));
        }

        internal DbGroupExpressionBinding CreateGroupExpressionBinding(DbExpression input, string varName, string groupVarName) => 
            new DbGroupExpressionBinding(this, input, varName, groupVarName);

        internal DbJoinExpression CreateInnerJoinExpression(DbExpressionBinding left, DbExpressionBinding right, DbExpression joinCondition) => 
            new DbJoinExpression(this, DbExpressionKind.InnerJoin, left, right, joinCondition);

        internal DbIntersectExpression CreateIntersectExpression(DbExpression left, DbExpression right) => 
            new DbIntersectExpression(this, left, right);

        internal DbIsEmptyExpression CreateIsEmptyExpression(DbExpression argument) => 
            new DbIsEmptyExpression(this, argument);

        internal DbIsNullExpression CreateIsNullExpression(DbExpression argument) => 
            new DbIsNullExpression(this, argument, false);

        internal DbIsNullExpression CreateIsNullExpressionAllowingRowTypeArgument(DbExpression argument) => 
            new DbIsNullExpression(this, argument, true);

        internal DbIsOfExpression CreateIsOfExpression(DbExpression argument, TypeUsage type) => 
            new DbIsOfExpression(this, DbExpressionKind.IsOf, type, argument);

        internal DbIsOfExpression CreateIsOfOnlyExpression(DbExpression argument, TypeUsage type) => 
            new DbIsOfExpression(this, DbExpressionKind.IsOfOnly, type, argument);

        internal DbExpression CreateJoinExpressionByKind(DbExpressionKind joinKind, DbExpression joinCondition, DbExpressionBinding input1, DbExpressionBinding input2)
        {
            if (DbExpressionKind.CrossJoin == joinKind)
            {
                return this.CreateCrossJoinExpression(new DbExpressionBinding[] { input1, input2 });
            }
            DbExpressionKind kind = joinKind;
            if (kind == DbExpressionKind.FullOuterJoin)
            {
                return this.CreateFullOuterJoinExpression(input1, input2, joinCondition);
            }
            if (kind != DbExpressionKind.InnerJoin)
            {
                if (kind != DbExpressionKind.LeftOuterJoin)
                {
                    throw EntityUtil.InvalidEnumerationValue(typeof(DbExpressionKind), (int) joinKind);
                }
                return this.CreateLeftOuterJoinExpression(input1, input2, joinCondition);
            }
            return this.CreateInnerJoinExpression(input1, input2, joinCondition);
        }

        internal DbFunctionExpression CreateLambdaFunctionExpression(IEnumerable<KeyValuePair<string, TypeUsage>> formals, DbExpression body, IList<DbExpression> args)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<KeyValuePair<string, TypeUsage>>>(formals, "formals");
            EntityUtil.CheckArgumentNull<DbExpression>(body, "body");
            List<FunctionParameter> funcParams = new List<FunctionParameter>();
            List<KeyValuePair<string, TypeUsage>> list = new List<KeyValuePair<string, TypeUsage>>(formals);
            int idx = 0;
            CommandTreeUtils.CheckNamedList<TypeUsage>("formals", list, true, delegate (KeyValuePair<string, TypeUsage> formal, int index) {
                this.TypeHelper.CheckType(formal.Value, CommandTreeUtils.FormatIndex("formals", idx));
                idx++;
                funcParams.Add(new FunctionParameter(formal.Key, formal.Value, ParameterMode.In));
            });
            FunctionParameter parameter = new FunctionParameter("Return", body.ResultType, ParameterMode.ReturnValue);
            EdmFunctionPayload payload = new EdmFunctionPayload {
                ReturnParameter = parameter,
                Parameters = funcParams.ToArray()
            };
            EdmFunction function = new EdmFunction(DbFunctionExpression.LambdaFunctionName, DbFunctionExpression.LambdaFunctionNameSpace, parameter.TypeUsage.EdmType.DataSpace, payload);
            function.SetReadOnly();
            return new DbFunctionExpression(this, function, body, args);
        }

        internal DbJoinExpression CreateLeftOuterJoinExpression(DbExpressionBinding left, DbExpressionBinding right, DbExpression joinCondition) => 
            new DbJoinExpression(this, DbExpressionKind.LeftOuterJoin, left, right, joinCondition);

        internal DbComparisonExpression CreateLessThanExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.LessThan, left, right);

        internal DbComparisonExpression CreateLessThanOrEqualsExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.LessThanOrEquals, left, right);

        internal DbLikeExpression CreateLikeExpression(DbExpression input, DbExpression pattern) => 
            new DbLikeExpression(this, input, pattern, null);

        internal DbLikeExpression CreateLikeExpression(DbExpression input, DbExpression pattern, DbExpression escape)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(escape, "escape");
            return new DbLikeExpression(this, input, pattern, escape);
        }

        internal DbLimitExpression CreateLimitExpression(DbExpression argument, DbExpression limit) => 
            new DbLimitExpression(this, argument, limit, false);

        internal DbLimitExpression CreateLimitWithTiesExpression(DbExpression argument, DbExpression limit) => 
            new DbLimitExpression(this, argument, limit, true);

        internal DbArithmeticExpression CreateMinusExpression(DbExpression left, DbExpression right) => 
            new DbArithmeticExpression(this, DbExpressionKind.Minus, CommandTreeUtils.CreateList<DbExpression>(left, right));

        internal DbArithmeticExpression CreateModuloExpression(DbExpression left, DbExpression right) => 
            new DbArithmeticExpression(this, DbExpressionKind.Modulo, CommandTreeUtils.CreateList<DbExpression>(left, right));

        internal DbArithmeticExpression CreateMultiplyExpression(DbExpression left, DbExpression right) => 
            new DbArithmeticExpression(this, DbExpressionKind.Multiply, CommandTreeUtils.CreateList<DbExpression>(left, right));

        internal DbNewInstanceExpression CreateNewCollectionExpression(IList<DbExpression> collectionElements)
        {
            if (collectionElements == null)
            {
                throw EntityUtil.ArgumentNull("collectionElements");
            }
            if (collectionElements.Count < 1)
            {
                throw EntityUtil.Argument(Strings.Cqt_Factory_NewCollectionElementsRequired, "collectionElements");
            }
            ExpressionList args = new ExpressionList("collectionElements", this, collectionElements.Count);
            args.SetElements(collectionElements);
            TypeUsage commonElementType = args.GetCommonElementType();
            if (TypeSemantics.IsNullOrNullType(commonElementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Factory_NewCollectionInvalidCommonType, "collectionElements");
            }
            return new DbNewInstanceExpression(this, CommandTreeTypeHelper.CreateCollectionResultType(commonElementType), args);
        }

        internal DbNewInstanceExpression CreateNewEmptyCollectionExpression(TypeUsage collectionType)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(collectionType, "collectionType");
            if (!TypeSemantics.IsCollectionType(collectionType))
            {
                throw EntityUtil.Argument(Strings.Cqt_NewInstance_CollectionTypeRequired, "collectionType");
            }
            return new DbNewInstanceExpression(this, collectionType, null);
        }

        internal DbNewInstanceExpression CreateNewEntityWithRelationshipsExpression(EntityType instanceType, IList<DbExpression> attributeValues, IList<DbRelatedEntityRef> relationships) => 
            new DbNewInstanceExpression(this, instanceType, attributeValues, relationships);

        internal DbNewInstanceExpression CreateNewInstanceExpression(TypeUsage type, IList<DbExpression> args) => 
            new DbNewInstanceExpression(this, type, args);

        internal DbNewInstanceExpression CreateNewRowExpression(IList<KeyValuePair<string, DbExpression>> recordColumns)
        {
            List<DbExpression> args = new List<DbExpression>();
            List<KeyValuePair<string, TypeUsage>> colTypes = new List<KeyValuePair<string, TypeUsage>>();
            CommandTreeUtils.CheckNamedList<DbExpression>("recordColumns", recordColumns, false, delegate (KeyValuePair<string, DbExpression> exprInfo, int index) {
                new ExpressionLink(CommandTreeUtils.FormatIndex("recordColumns", index), this, exprInfo.Value);
                args.Add(exprInfo.Value);
                colTypes.Add(new KeyValuePair<string, TypeUsage>(exprInfo.Key, exprInfo.Value.ResultType));
            });
            return new DbNewInstanceExpression(this, CommandTreeTypeHelper.CreateResultType(TypeHelpers.CreateRowType(colTypes)), args);
        }

        internal DbComparisonExpression CreateNotEqualsExpression(DbExpression left, DbExpression right) => 
            new DbComparisonExpression(this, DbExpressionKind.NotEquals, left, right);

        internal DbNotExpression CreateNotExpression(DbExpression argument) => 
            new DbNotExpression(this, argument);

        internal DbNullExpression CreateNullExpression(TypeUsage type) => 
            new DbNullExpression(this, type);

        internal DbOfTypeExpression CreateOfTypeExpression(DbExpression argument, TypeUsage ofType) => 
            new DbOfTypeExpression(this, DbExpressionKind.OfType, ofType, argument);

        internal DbOfTypeExpression CreateOfTypeOnlyExpression(DbExpression argument, TypeUsage ofType) => 
            new DbOfTypeExpression(this, DbExpressionKind.OfTypeOnly, ofType, argument);

        internal DbOrExpression CreateOrExpression(DbExpression left, DbExpression right) => 
            new DbOrExpression(this, left, right);

        internal DbApplyExpression CreateOuterApplyExpression(DbExpressionBinding input, DbExpressionBinding apply) => 
            new DbApplyExpression(this, input, apply, DbExpressionKind.OuterApply);

        internal DbParameterReferenceExpression CreateParameterReferenceExpression(string name)
        {
            KeyValuePair<string, TypeUsage> pair;
            if (name == null)
            {
                throw EntityUtil.ArgumentNull("name");
            }
            if (!IsValidParameterName(name) || !this._paramMappings.TryGetValue(name, out pair))
            {
                throw EntityUtil.ArgumentOutOfRange(Strings.Cqt_CommandTree_NoParameterExists, "name");
            }
            return new DbParameterReferenceExpression(this, pair.Value, pair.Key);
        }

        internal DbArithmeticExpression CreatePlusExpression(DbExpression left, DbExpression right) => 
            new DbArithmeticExpression(this, DbExpressionKind.Plus, CommandTreeUtils.CreateList<DbExpression>(left, right));

        internal DbProjectExpression CreateProjectExpression(DbExpressionBinding input, DbExpression projection) => 
            new DbProjectExpression(this, input, projection);

        internal DbPropertyExpression CreatePropertyExpression(EdmProperty propertyInfo, DbExpression instance) => 
            new DbPropertyExpression(this, propertyInfo, instance);

        internal DbPropertyExpression CreatePropertyExpression(NavigationProperty propertyInfo, DbExpression instance) => 
            new DbPropertyExpression(this, propertyInfo, instance);

        internal DbPropertyExpression CreatePropertyExpression(RelationshipEndMember memberInfo, DbExpression instance) => 
            new DbPropertyExpression(this, memberInfo, instance);

        internal DbPropertyExpression CreatePropertyExpression(string propertyName, DbExpression instance) => 
            this.CreatePropertyExpression(propertyName, false, instance);

        internal DbPropertyExpression CreatePropertyExpression(string propertyName, bool ignoreCase, DbExpression instance)
        {
            StructuralType type;
            EdmMember member;
            EntityUtil.CheckArgumentNull<string>(propertyName, "propertyName");
            EntityUtil.CheckArgumentNull<DbExpression>(instance, "instance");
            if (TypeHelpers.TryGetEdmType<StructuralType>(instance.ResultType, out type) && type.Members.TryGetValue(propertyName, ignoreCase, out member))
            {
                if (Helper.IsRelationshipEndMember(member))
                {
                    return this.CreatePropertyExpression((RelationshipEndMember) member, instance);
                }
                if (Helper.IsEdmProperty(member))
                {
                    return this.CreatePropertyExpression((EdmProperty) member, instance);
                }
                if (Helper.IsNavigationProperty(member))
                {
                    return this.CreatePropertyExpression((NavigationProperty) member, instance);
                }
            }
            throw EntityUtil.ArgumentOutOfRange(Strings.Cqt_Factory_NoSuchProperty, "propertyName");
        }

        internal DbRefExpression CreateRefExpression(EntitySet entitySet, DbExpression refKeys)
        {
            EntityUtil.CheckArgumentNull<EntitySet>(entitySet, "entitySet");
            return new DbRefExpression(this, entitySet, refKeys, entitySet.ElementType);
        }

        internal DbRefExpression CreateRefExpression(EntitySet entitySet, DbExpression refKeys, EntityType entityType) => 
            new DbRefExpression(this, entitySet, refKeys, entityType);

        internal DbRefKeyExpression CreateRefKeyExpression(DbExpression reference) => 
            new DbRefKeyExpression(this, reference);

        internal DbRelatedEntityRef CreateRelatedEntityRef(RelationshipEndMember sourceEnd, RelationshipEndMember targetEnd, DbExpression targetEntity) => 
            new DbRelatedEntityRef(this, sourceEnd, targetEnd, targetEntity);

        internal DbRelationshipNavigationExpression CreateRelationshipNavigationExpression(RelationshipEndMember fromEnd, RelationshipEndMember toEnd, DbExpression from) => 
            new DbRelationshipNavigationExpression(this, fromEnd, toEnd, from);

        internal DbRelationshipNavigationExpression CreateRelationshipNavigationExpression(RelationshipType type, string fromEndName, string toEndName, DbExpression from) => 
            new DbRelationshipNavigationExpression(this, type, fromEndName, toEndName, from);

        internal DbScanExpression CreateScanExpression(EntitySetBase target) => 
            new DbScanExpression(this, target);

        internal DbSkipExpression CreateSkipExpression(DbExpressionBinding input, IList<DbSortClause> sortOrder, DbExpression count) => 
            new DbSkipExpression(this, input, sortOrder, count);

        internal DbSortClause CreateSortClause(DbExpression key, bool ascending) => 
            new DbSortClause(key, ascending, string.Empty);

        internal DbSortClause CreateSortClause(DbExpression key, bool ascending, string collation)
        {
            EntityUtil.CheckArgumentNull<string>(collation, "collation");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(collation))
            {
                throw EntityUtil.ArgumentOutOfRange(Strings.Cqt_Sort_EmptyCollationInvalid, "collation");
            }
            return new DbSortClause(key, ascending, collation);
        }

        internal DbSortExpression CreateSortExpression(DbExpressionBinding input, IList<DbSortClause> sortOrder) => 
            new DbSortExpression(this, input, sortOrder);

        internal DbTreatExpression CreateTreatExpression(DbExpression argument, TypeUsage treatType) => 
            new DbTreatExpression(this, treatType, argument);

        internal DbConstantExpression CreateTrueExpression()
        {
            if (this._trueExpr == null)
            {
                this._trueExpr = this.CreateConstantExpression(true, this.TypeHelper.CreateBooleanResultType());
            }
            return this._trueExpr;
        }

        internal DbArithmeticExpression CreateUnaryMinusExpression(DbExpression argument) => 
            new DbArithmeticExpression(this, DbExpressionKind.UnaryMinus, CommandTreeUtils.CreateList<DbExpression>(argument));

        internal DbUnionAllExpression CreateUnionAllExpression(DbExpression left, DbExpression right) => 
            new DbUnionAllExpression(this, left, right);

        internal DbVariableReferenceExpression CreateVariableReferenceExpression(string varName, TypeUsage varType) => 
            new DbVariableReferenceExpression(this, varType, varName);

        internal void Dump(ExpressionDumper dumper)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object> {
                { 
                    "DataSpace",
                    this.DataSpace
                }
            };
            dumper.Begin(base.GetType().Name, attrs);
            dumper.Begin("Parameters", (Dictionary<string, object>) null);
            foreach (KeyValuePair<string, TypeUsage> pair in this.Parameters)
            {
                Dictionary<string, object> dictionary2 = new Dictionary<string, object> {
                    { 
                        "Name",
                        pair.Key
                    }
                };
                dumper.Begin("Parameter", dictionary2);
                dumper.Dump(pair.Value, "ParameterType");
                dumper.End("Parameter");
            }
            dumper.End("Parameters");
            this.DumpStructure(dumper);
            dumper.End(base.GetType().Name);
        }

        internal abstract void DumpStructure(ExpressionDumper dumper);
        internal string DumpXml()
        {
            MemoryStream stream = new MemoryStream();
            XmlExpressionDumper dumper = new XmlExpressionDumper(stream);
            this.Dump(dumper);
            dumper.Close();
            return XmlExpressionDumper.DefaultEncoding.GetString(stream.ToArray());
        }

        internal bool HasParameter(string parameterName, TypeUsage parameterType)
        {
            KeyValuePair<string, TypeUsage> pair;
            if (!this._paramMappings.TryGetValue(parameterName, out pair))
            {
                return false;
            }
            return (pair.Key.Equals(parameterName, StringComparison.Ordinal) && TypeSemantics.IsStructurallyEqualTo(pair.Value, parameterType));
        }

        internal DbExpression Import(DbExpression source)
        {
            if (source == null)
            {
                throw EntityUtil.ArgumentNull("source");
            }
            if (source.CommandTree == null)
            {
                throw EntityUtil.Argument(Strings.Cqt_CommandTree_Import_NullCommandTreeInvalid, "source");
            }
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.Import|API> %d#", this.ObjectId))
            {
                EntityBid.Trace("<cqt.DbCommandTree.Import|INFO> %d#, source=%d#, %d{cqt.DbExpressionKind}\n", this.ObjectId, DbExpression.GetObjectId(source), DbExpression.GetExpressionKind(source));
                if (this == source.CommandTree)
                {
                    return source;
                }
                return ExpressionCopier.Copy(this, source);
            }
        }

        internal static bool IsValidParameterName(string name) => 
            (!StringUtil.IsNullOrEmptyOrWhiteSpace(name) && _paramNameRegex.IsMatch(name));

        internal string Print() => 
            this.PrintTree(new ExpressionPrinter());

        internal abstract string PrintTree(ExpressionPrinter printer);
        internal abstract void Replace(ExpressionReplacer replacer);
        internal void Replace(ReplacerCallback callback)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.Replace(ReplacerCallback)|API> %d#", this.ObjectId))
            {
                ExpressionReplacer replacer = new ExpressionReplacer(callback);
                this.Replace(replacer);
            }
        }

        internal void SetModified()
        {
            int num = this._currentVersion;
            if (0x7fffffff == this._currentVersion)
            {
                this._currentVersion = 1;
                this._validatedVersion = 0;
            }
            else
            {
                this._currentVersion++;
            }
            EntityBid.Trace("<cqt.DbCommandTree.SetModified|INFO> %d# previous version=%d, new version=%d\n", this.ObjectId, num, this._currentVersion);
        }

        internal void SetValid()
        {
            EntityBid.Trace("<cqt.DbCommandTree.SetValid|API> %d#\n", this.ObjectId);
            this._validatedVersion = this._currentVersion;
        }

        internal void Trace()
        {
            if (EntityBid.AdvancedOn)
            {
                EntityBid.PutStrChunked(this.DumpXml());
                EntityBid.Trace("\n");
            }
            else if (EntityBid.TraceOn)
            {
                EntityBid.PutStrChunked(this.Print());
                EntityBid.Trace("\n");
            }
        }

        internal void TrackContainer(EntityContainer container)
        {
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.TrackContainer|API> %d#", this.ObjectId))
            {
                if (!this._containers.Contains(container))
                {
                    EntityBid.Trace("<cqt.DbCommandTree.TrackContainer|INFO> %d#, Adding container to tracked containers list: '%ls'\n", this.ObjectId, container.Name);
                    this._containers.Add(container);
                }
            }
        }

        internal void Validate()
        {
            using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.Validate|API> %d#", this.ObjectId))
            {
                if (this.ValidationRequired)
                {
                    this.Validate(new Validator());
                    this.SetValid();
                }
            }
        }

        internal virtual void Validate(Validator v)
        {
        }

        internal AliasGenerator BindingAliases
        {
            get
            {
                if (this._bindingAliases == null)
                {
                    this._bindingAliases = new AliasGenerator(string.Format(CultureInfo.InvariantCulture, "Var_{0}_", new object[] { this.ObjectId }), 0);
                }
                return this._bindingAliases;
            }
        }

        internal abstract DbCommandTreeKind CommandTreeKind { get; }

        internal System.Data.Metadata.Edm.DataSpace DataSpace =>
            this._dataSpace;

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this._metadata;

        public IEnumerable<KeyValuePair<string, TypeUsage>> Parameters
        {
            get
            {
                using (new EntityBid.ScopeAuto("<cqt.DbCommandTree.get_Parameters|API> %d#", this.ObjectId))
                {
                    Dictionary<string, TypeUsage> dictionary = new Dictionary<string, TypeUsage>();
                    foreach (KeyValuePair<string, TypeUsage> pair in this._paramMappings.Values)
                    {
                        EntityBid.Trace("<cqt.DbCommandTree.get_Parameters|INFO> %d#, name='%ls'\n", this.ObjectId, pair.Key);
                        EntityBid.Trace("<cqt.DbCommandTree.get_Parameters|INFO> %d#, type='%ls'\n", this.ObjectId, TypeHelpers.GetStrongName(pair.Value));
                        dictionary.Add(pair.Key, pair.Value);
                    }
                    return dictionary;
                }
            }
        }

        internal CommandTreeTypeHelper TypeHelper =>
            this._typeHelp;

        internal bool ValidationRequired
        {
            get
            {
                bool flag = this._validatedVersion < this._currentVersion;
                EntityBid.Trace("<cqt.DbCommandTree.get_ValidationRequired|API> %d#, %d{bool}\n", this.ObjectId, flag);
                return flag;
            }
        }
    }
}

