namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal sealed class SemanticResolver
    {
        private List<AggregateAstNodeInfo> _aggregateAstNodes = new List<AggregateAstNodeInfo>();
        private DbCommandTree _commandTree;
        private uint _namegenCounter;
        private Dictionary<string, TypeUsage> _parameters;
        private System.Data.Common.EntitySql.ParserOptions _parserOptions;
        private int _scopeIndexHighMark;
        private List<ScopeRegionFlags> _scopeRegionFlags = new List<ScopeRegionFlags>();
        private List<SavePoint> _scopeRegionSavepoints = new List<SavePoint>();
        private StaticContext _staticContext;
        private StringComparer _stringComparer;
        private System.Data.Common.EntitySql.TypeResolver _typeResolver;
        private Dictionary<string, KeyValuePair<string, TypeUsage>> _variables;
        private static DbExpressionKind[] applyMap = new DbExpressionKind[] { DbExpressionKind.CrossApply, DbExpressionKind.OuterApply };
        private static DbExpressionKind[] joinMap = new DbExpressionKind[] { DbExpressionKind.CrossJoin, DbExpressionKind.InnerJoin, DbExpressionKind.LeftOuterJoin, DbExpressionKind.FullOuterJoin };

        internal SemanticResolver(Perspective perspective, System.Data.Common.EntitySql.ParserOptions parserOptions, Dictionary<string, TypeUsage> eSqlParameters, Dictionary<string, TypeUsage> variableParameters)
        {
            EntityUtil.CheckArgumentNull<Perspective>(perspective, "perspective");
            EntityUtil.CheckArgumentNull<System.Data.Common.EntitySql.ParserOptions>(parserOptions, "parserOptions");
            this._parserOptions = parserOptions;
            if (this._parserOptions.IdentifierCaseSensitiveness == System.Data.Common.EntitySql.ParserOptions.CaseSensitiveness.CaseSensitive)
            {
                this._stringComparer = StringComparer.Ordinal;
            }
            else
            {
                this._stringComparer = StringComparer.OrdinalIgnoreCase;
            }
            this._typeResolver = new System.Data.Common.EntitySql.TypeResolver(perspective, this._stringComparer);
            this._staticContext = new StaticContext(this._stringComparer);
            this._parameters = this.ValidateParameters(eSqlParameters);
            this._variables = this.ValidateVariables(variableParameters);
            this.EnterScopeRegion();
            this.CurrentScopeRegionFlags.ScopeViewKind = ScopeViewKind.All;
        }

        internal void AddDummyGroupKeyToScope(string groupKey, DbExpression varBasedExpression, DbExpression groupVarBasedExpression)
        {
            this._staticContext.AddGroupDummyVar(groupKey, varBasedExpression, groupVarBasedExpression);
        }

        internal void AddGroupAggregateInfoToScopeRegion(MethodExpr astNode, string aggregateName, DbAggregate aggregateExpression, int groupVarScopeIndex)
        {
            int num = 0;
            bool flag = false;
            if (groupVarScopeIndex == -2147483648)
            {
                flag = true;
                num = 0;
            }
            else
            {
                num = 0;
                while (num < this._scopeRegionSavepoints.Count)
                {
                    if (this._scopeRegionSavepoints[num].ContainsScope(groupVarScopeIndex))
                    {
                        flag = true;
                        break;
                    }
                    num++;
                }
            }
            if (!flag)
            {
                throw EntityUtil.EntitySqlError(Strings.GroupVarNotFoundInScope);
            }
            this._scopeRegionFlags[num].AddGroupAggregateInfo(astNode, new GroupAggregateInfo(aggregateName, aggregateExpression));
        }

        internal void AddGroupAggregateToScope(string aggregateName, DbVariableReferenceExpression sourceVar)
        {
            this._staticContext.AddAggregateToScope(aggregateName, sourceVar);
        }

        internal void AddSourceBinding(DbExpressionBinding sourceBinding)
        {
            this._staticContext.AddSourceBinding(sourceBinding, this.CurrentScopeRegionFlags.ScopeEntryKind, this.CurrentScopeRegionFlags.PathTagger.Tag);
        }

        internal void AddToScope(string key, ScopeEntry scopeEntry)
        {
            this._staticContext.Add(key, scopeEntry);
        }

        internal DbExpression CreateFunction(IList<EdmFunction> functionTypeList, List<DbExpression> args, MethodExpr methodExpr)
        {
            methodExpr.ErrCtx.ErrorContextInfo = "CtxGenericFunctionCall";
            bool isAmbiguous = false;
            if (methodExpr.DistinctKind != DistinctKind.None)
            {
                methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxFunction(functionTypeList[0].Name);
                methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.InvalidDistinctArgumentInNonAggFunction);
            }
            List<TypeUsage> argTypes = new List<TypeUsage>(args.Count);
            for (int i = 0; i < args.Count; i++)
            {
                argTypes.Add(args[i].ResultType);
            }
            EdmFunction function = System.Data.Common.EntitySql.TypeResolver.ResolveFunctionOverloads(functionTypeList, argTypes, false, out isAmbiguous);
            if (isAmbiguous)
            {
                methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxFunction(functionTypeList[0].Name);
                methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.AmbiguousFunctionArguments);
            }
            if (function == null)
            {
                CqlErrorHelper.ReportFunctionOverloadError(methodExpr, functionTypeList[0], argTypes);
            }
            for (int j = 0; j < args.Count; j++)
            {
                if (TypeSemantics.IsNullType(args[j].ResultType))
                {
                    args[j] = this.CmdTree.CreateNullExpression(function.Parameters[j].TypeUsage);
                }
            }
            return this.CmdTree.CreateFunctionExpression(function, args);
        }

        internal static DbExpression CreateInstanceMethod(DbExpression instance, List<DbExpression> args, MethodExpr methodExpr) => 
            CreateMethod(instance.ResultType, args, methodExpr, instance, false);

        internal DbExpression CreateInstanceOfType(TypeUsage type, List<DbExpression> args, List<DbRelatedEntityRef> relshipExprList, MethodExpr methodExpr)
        {
            int num = 0;
            int count = args.Count;
            methodExpr.ErrCtx.ErrorContextInfo = "CtxGenericTypeCtor";
            if (type.EdmType.Abstract)
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.CannotInstantiateAbstractType(type.EdmType.FullName));
            }
            if (methodExpr.DistinctKind != DistinctKind.None)
            {
                methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxTypeCtorWithType(TypeHelpers.GetFullName(type));
                methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.InvalidDistinctArgumentInCtor);
            }
            if ((TypeSemantics.IsComplexType(type) || TypeSemantics.IsEntityType(type)) || TypeSemantics.IsRelationshipType(type))
            {
                StructuralType edmType = (StructuralType) type.EdmType;
                foreach (EdmMember member in TypeHelpers.GetAllStructuralMembers(edmType))
                {
                    TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(member);
                    if (count <= num)
                    {
                        methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxTypeCtorWithType(TypeHelpers.GetFullName(type));
                        methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                        throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.NumberOfTypeCtorIsLessThenFormalSpec(member.Name));
                    }
                    if (TypeSemantics.IsNullType(args[num].ResultType))
                    {
                        if (Helper.IsEdmProperty(member) && !((EdmProperty) member).Nullable)
                        {
                            throw EntityUtil.EntitySqlError(methodExpr.Args[num].ErrCtx, Strings.InvalidNullLiteralForNonNullableMember(member.Name, TypeHelpers.GetFullName(edmType)));
                        }
                        args[num] = this.CmdTree.CreateNullExpression(modelTypeUsage);
                    }
                    bool flag = TypeSemantics.IsPromotableTo(args[num].ResultType, modelTypeUsage);
                    if ((System.Data.Common.EntitySql.ParserOptions.CompilationMode.RestrictedViewGenerationMode == this._parserOptions.ParserCompilationMode) || (System.Data.Common.EntitySql.ParserOptions.CompilationMode.UserViewGenerationMode == this._parserOptions.ParserCompilationMode))
                    {
                        if (!flag && !TypeSemantics.IsPromotableTo(modelTypeUsage, args[num].ResultType))
                        {
                            throw EntityUtil.EntitySqlError(methodExpr.Args[num].ErrCtx, Strings.InvalidCtorArgumentType(args[num].ResultType.Identity, member.Name, modelTypeUsage.Identity));
                        }
                        if (Helper.IsPrimitiveType(modelTypeUsage.EdmType) && !TypeSemantics.IsSubTypeOf(args[num].ResultType, modelTypeUsage))
                        {
                            args[num] = this._commandTree.CreateCastExpression(args[num], modelTypeUsage);
                        }
                    }
                    else if (!flag)
                    {
                        throw EntityUtil.EntitySqlError(methodExpr.Args[num].ErrCtx, Strings.InvalidCtorArgumentType(args[num].ResultType.Identity, member.Name, modelTypeUsage.Identity));
                    }
                    num++;
                }
            }
            else
            {
                methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxTypeCtorWithType(TypeHelpers.GetFullName(type));
                methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.InvalidCtorUseOnType(TypeHelpers.GetFullName(type)));
            }
            if (num != count)
            {
                methodExpr.ErrCtx.ErrorContextInfo = Strings.CtxTypeCtorWithType(TypeHelpers.GetFullName(type));
                methodExpr.ErrCtx.UseContextInfoAsResourceIdentifier = false;
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.NumberOfTypeCtorIsMoreThenFormalSpec(TypeHelpers.GetFullName(type)));
            }
            if ((relshipExprList != null) && (relshipExprList.Count > 0))
            {
                EntityType instanceType = (EntityType) type.EdmType;
                return this.CmdTree.CreateNewEntityWithRelationshipsExpression(instanceType, args, relshipExprList);
            }
            return this.CmdTree.CreateNewInstanceExpression(TypeHelpers.GetReadOnlyType(type), args);
        }

        private static DbExpression CreateMethod(TypeUsage definingType, List<DbExpression> args, MethodExpr methodExpr, DbExpression instance, bool isStatic)
        {
            throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.MethodInvocationNotSupported);
        }

        private string CreateNewAlias(DbExpression expr)
        {
            DbScanExpression expression = expr as DbScanExpression;
            if (expression != null)
            {
                return expression.Target.Name;
            }
            DbPropertyExpression expression2 = expr as DbPropertyExpression;
            if (expression2 != null)
            {
                return expression2.Property.Name;
            }
            DbVariableReferenceExpression expression3 = expr as DbVariableReferenceExpression;
            if (expression3 != null)
            {
                return expression3.VariableName;
            }
            return this.GenerateInternalName(string.Empty);
        }

        internal SavePoint CreateSavePoint() => 
            new SavePoint(this.CurrentScopeIndex);

        internal static DbExpression CreateStaticMethod(TypeUsage definingType, List<DbExpression> args, MethodExpr methodExpr) => 
            CreateMethod(definingType, args, methodExpr, null, true);

        internal void DeclareCanonicalNamespace()
        {
            if (!this.TypeResolver.TryAddNamespace("Edm"))
            {
                throw EntityUtil.EntitySqlError(Strings.FailedToDeclareCanonicalNamespace);
            }
        }

        internal void DeclareNamespaces(ExprList<NamespaceExpr> nsExprList)
        {
            if (nsExprList != null)
            {
                for (int i = 0; i < nsExprList.Count; i++)
                {
                    NamespaceExpr expr = nsExprList[i];
                    string fullName = expr.NamespaceName.FullName;
                    if (fullName.Equals("Edm", StringComparison.OrdinalIgnoreCase))
                    {
                        throw EntityUtil.EntitySqlError(expr.NamespaceName.ErrCtx, Strings.CannotUseCanonicalNamespace(fullName));
                    }
                    if (expr.IsAliased)
                    {
                        string name = expr.AliasIdentifier.Name;
                        if (!this.TypeResolver.TryAddAliasedNamespace(name, fullName))
                        {
                            throw EntityUtil.EntitySqlError(expr.AliasIdentifier.ErrCtx, Strings.NamespaceAliasAlreadyUsed(name));
                        }
                    }
                    else if (!this.TypeResolver.TryAddNamespace(fullName))
                    {
                        throw EntityUtil.EntitySqlError(expr.NamespaceName.ErrCtx, Strings.NamespaceNameAlreadyDeclared(fullName));
                    }
                }
            }
        }

        internal static void EnsureIsNotUntypedNull(DbExpression expression, ErrorContext errCtx)
        {
            if (expression is UntypedNullExpression)
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.ExpressionCannotBeNull);
            }
        }

        internal Pair<DbExpression, DbExpression> EnsureTypedNulls(DbExpression leftExpr, DbExpression rightExpr, ErrorContext errCtx, Func<string> formatMessage)
        {
            DbExpression left = leftExpr;
            DbExpression right = rightExpr;
            UntypedNullExpression expression3 = leftExpr as UntypedNullExpression;
            UntypedNullExpression expression4 = rightExpr as UntypedNullExpression;
            if (expression3 != null)
            {
                if ((expression4 != null) || (rightExpr == null))
                {
                    throw EntityUtil.EntitySqlError(errCtx, formatMessage());
                }
                left = this.CmdTree.CreateNullExpression(rightExpr.ResultType);
            }
            else if (expression4 != null)
            {
                right = this.CmdTree.CreateNullExpression(leftExpr.ResultType);
            }
            return new Pair<DbExpression, DbExpression>(left, right);
        }

        internal void EnsureValidLimitExpression(ErrorContext errCtx, DbExpression expr, string subclauseName)
        {
            if (!TypeSemantics.IsPromotableTo(expr.ResultType, this.TypeResolver.Int64Type))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.PlaceholderExpressionMustBeCompatibleWithEdm64(subclauseName, expr.ResultType.EdmType.FullName));
            }
            DbConstantExpression expression = expr as DbConstantExpression;
            if ((expression != null) && (Convert.ToInt64(expression.Value, CultureInfo.InvariantCulture) < 0L))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.PlaceholderExpressionMustBeGreaterThanOrEqualToZero(subclauseName));
            }
        }

        internal static void EnsureValidTypeForNullExpression(TypeUsage type, ErrorContext errCtx)
        {
            if (TypeSemantics.IsCollectionType(type))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.NullLiteralCannotBePromotedToCollectionOfNulls);
            }
        }

        internal ScopeDisposer EnterScope()
        {
            this._staticContext.EnterScope();
            return new ScopeDisposer(this);
        }

        internal ScopeRegionDisposer EnterScopeRegion()
        {
            this._scopeRegionSavepoints.Insert(0, this.CreateSavePoint());
            this._staticContext.EnterScope();
            this._scopeRegionFlags.Insert(0, new ScopeRegionFlags());
            return new ScopeRegionDisposer(this);
        }

        internal void FixupGroupSourceVarBindings(DbVariableReferenceExpression newSourceVar, DbVariableReferenceExpression newGroupVar)
        {
            this.InternalFixupGroupSourceVarBindings(newSourceVar, newGroupVar);
        }

        internal void FixupNamedSourceVarBindings(DbVariableReferenceExpression newSourceVar)
        {
            for (int i = this.CurrentScopeRegionSavePoint.ScopeIndex + 1; i <= this.CurrentScopeIndex; i++)
            {
                foreach (KeyValuePair<string, ScopeEntry> pair in this._staticContext.GetScopeByIndex(i))
                {
                    if ((pair.Value.Kind == this.CurrentScopeKind) && !pair.Value.IsHidden)
                    {
                        SourceScopeEntry entry = pair.Value as SourceScopeEntry;
                        if ((entry != null) && TreePathTagger.IsChildNode(this.CurrentScopeRegionFlags.PathTagger.Tag, entry.VarTag))
                        {
                            entry.AddBindingPrefix(newSourceVar.VariableName);
                            entry.SetNewSourceBinding(newSourceVar);
                        }
                    }
                }
            }
        }

        internal void FixupSourceVarBindings(DbVariableReferenceExpression newSourceVar)
        {
            for (int i = this.CurrentScopeRegionSavePoint.ScopeIndex + 1; i <= this.CurrentScopeIndex; i++)
            {
                foreach (KeyValuePair<string, ScopeEntry> pair in this._staticContext.GetScopeByIndex(i))
                {
                    SourceScopeEntry entry = pair.Value as SourceScopeEntry;
                    if (entry != null)
                    {
                        entry.SetNewSourceBinding(newSourceVar);
                    }
                }
            }
        }

        internal string GenerateInternalName(string hint)
        {
            uint num2 = this._namegenCounter++;
            return ("_##" + hint + num2.ToString(CultureInfo.InvariantCulture));
        }

        private DbExpression GetExpressionFromScopeEntry(ScopeEntry scopeEntry, int scopeIndex)
        {
            DbExpression expression = null;
            expression = scopeEntry.Expression;
            if (this.CurrentScopeRegionFlags.IsInsideGroupAggregate)
            {
                if (this._aggregateAstNodes.Count > 0)
                {
                    this._aggregateAstNodes[0].ScopeIndex = Math.Max(this._aggregateAstNodes[0].ScopeIndex, scopeIndex);
                }
                SourceScopeEntry entry = scopeEntry as SourceScopeEntry;
                if (entry != null)
                {
                    if (entry.GroupVarExpression != null)
                    {
                        return entry.AggregateExpression;
                    }
                    return expression;
                }
                DummyGroupVarScopeEntry entry2 = scopeEntry as DummyGroupVarScopeEntry;
                if (entry2 != null)
                {
                    return entry2.AggregateExpression;
                }
            }
            return expression;
        }

        internal ScopeViewKind GetScopeView() => 
            this.CurrentScopeRegionFlags.ScopeViewKind;

        private byte GetValidDecimalFacetValue(PrimitiveType primitiveType, Literal typeArg, string FacetName)
        {
            FacetDescription facet = Helper.GetFacet(primitiveType.ProviderManifest.GetFacetDescriptions(primitiveType), FacetName);
            if (facet == null)
            {
                throw EntityUtil.EntitySqlError(typeArg.ErrCtx, Strings.TypeDoesNotSupportPrecisionOrScale(primitiveType.FullName, FacetName));
            }
            byte result = 0;
            if (!byte.TryParse(typeArg.OriginalValue, out result))
            {
                throw EntityUtil.EntitySqlError(typeArg.ErrCtx, Strings.TypeSpecIsNotValid);
            }
            if (result > facet.MaxValue)
            {
                throw EntityUtil.EntitySqlError(typeArg.ErrCtx, Strings.TypeSpecExceedsMax(FacetName));
            }
            if (result < facet.MinValue)
            {
                throw EntityUtil.EntitySqlError(typeArg.ErrCtx, Strings.TypeSpecBellowMin(FacetName));
            }
            return result;
        }

        private TypeUsage HandleParametrizedType(TypeUsage typeUsage, MethodExpr methodExpr)
        {
            PrimitiveType edmType = typeUsage.EdmType as PrimitiveType;
            if ((edmType == null) || (edmType.PrimitiveTypeKind != PrimitiveTypeKind.Decimal))
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.TypeDoesNotSupportParameters(edmType.FullName));
            }
            byte precision = this.GetValidDecimalFacetValue(edmType, (Literal) methodExpr.Args[0], "Precision");
            byte scale = 0;
            if (2 == methodExpr.Args.Count)
            {
                scale = this.GetValidDecimalFacetValue(edmType, (Literal) methodExpr.Args[1], "Scale");
            }
            if (precision < scale)
            {
                throw EntityUtil.EntitySqlError(methodExpr.Args[1].ErrCtx, Strings.DecimalPrecisionMustBeGreaterThanScale(edmType.FullName));
            }
            return TypeUsage.CreateDecimalTypeUsage(edmType, precision, scale);
        }

        internal string InferAliasName(AliasExpr aliasExpr, DbExpression convertedExpression)
        {
            if (aliasExpr.HasAlias)
            {
                return aliasExpr.AliasIdentifier.Name;
            }
            Identifier identifier = aliasExpr.Expr as Identifier;
            if (identifier != null)
            {
                return identifier.Name;
            }
            DotExpr expr = aliasExpr.Expr as DotExpr;
            if ((expr != null) && expr.IsDottedIdentifier)
            {
                return expr.Names[expr.Length - 1];
            }
            return this.CreateNewAlias(convertedExpression);
        }

        private void InternalFixupGroupSourceVarBindings(DbVariableReferenceExpression newSourceVar, DbVariableReferenceExpression newGroupVar)
        {
            for (int i = this.CurrentScopeRegionSavePoint.ScopeIndex + 1; i <= this.CurrentScopeIndex; i++)
            {
                foreach (KeyValuePair<string, ScopeEntry> pair in this._staticContext.GetScopeByIndex(i))
                {
                    if ((pair.Value.Kind == this.CurrentScopeKind) && !pair.Value.IsHidden)
                    {
                        SourceScopeEntry entry = pair.Value as SourceScopeEntry;
                        if (entry != null)
                        {
                            entry.SetNewSourceBinding(newSourceVar);
                            entry.GroupVarExpression = newGroupVar;
                        }
                    }
                }
            }
        }

        internal bool IsInAnyGroupScope()
        {
            for (int i = 0; i < this._scopeRegionFlags.Count; i++)
            {
                if (this._scopeRegionFlags[i].IsInGroupScope)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsInCurrentScope(string key) => 
            this._staticContext.IsInCurrentScope(key);

        internal static bool IsNullExpression(DbExpression expression) => 
            (expression is UntypedNullExpression);

        internal void LeaveScope()
        {
            this._staticContext.LeaveScope();
        }

        internal void LeaveScopeRegion()
        {
            this.RollbackToSavepoint(this.CurrentScopeRegionSavePoint);
            this._scopeRegionSavepoints.RemoveAt(0);
            foreach (MethodExpr expr in this.CurrentScopeRegionFlags.GroupAggregatesInfo.Keys)
            {
                expr.ResetAggregateInfo();
            }
            this._scopeRegionFlags.RemoveAt(0);
            this.SetScopeView(this.CurrentScopeRegionFlags.ScopeViewKind);
        }

        internal static DbExpressionKind MapApplyKind(ApplyKind applyKind) => 
            applyMap[(int) applyKind];

        internal static DbExpressionKind MapJoinKind(JoinKind joinKind) => 
            joinMap[(int) joinKind];

        internal void MarkGroupInputVars()
        {
            for (int i = this.CurrentScopeRegionSavePoint.ScopeIndex + 1; i <= this.CurrentScopeIndex; i++)
            {
                foreach (KeyValuePair<string, ScopeEntry> pair in this._staticContext.GetScopeByIndex(i))
                {
                    if ((pair.Value.VarKind != SourceVarKind.GroupAggregate) && (pair.Value.VarKind != SourceVarKind.GroupKey))
                    {
                        pair.Value.VarKind = SourceVarKind.GroupInput;
                    }
                }
            }
        }

        internal AggregateAstNodeInfo PopAggregateAstNode()
        {
            AggregateAstNodeInfo info = this._aggregateAstNodes[0];
            this._aggregateAstNodes.RemoveAt(0);
            return info;
        }

        internal void PushAggregateAstNode(MethodExpr astNode)
        {
            this._aggregateAstNodes.Insert(0, new AggregateAstNodeInfo(astNode));
        }

        internal void RemoveFromScope(string key)
        {
            this._staticContext.RemoveFromScope(key);
        }

        internal void ReplaceGroupVarInScope(string groupVarName, DbVariableReferenceExpression groupSourceBinding)
        {
            this._staticContext.ReplaceGroupVarInScope(groupVarName, groupSourceBinding);
        }

        internal void ResetCurrentScopeVarKind()
        {
            for (int i = this.CurrentScopeRegionSavePoint.ScopeIndex + 1; i <= this.CurrentScopeIndex; i++)
            {
                foreach (KeyValuePair<string, ScopeEntry> pair in this._staticContext.GetScopeByIndex(i))
                {
                    pair.Value.Kind = ScopeEntryKind.SourceVar;
                }
            }
            this.SetCurrentScopeKind(ScopeEntryKind.SourceVar);
        }

        internal void ResetScopeRegionCorrelationFlag()
        {
            this.CurrentScopeRegionFlags.WasResolutionCorrelated = false;
        }

        internal DbExpression ResolveIdentifier(string[] names, ErrorContext errCtx)
        {
            TypeUsage definingType = null;
            DbExpression convExpr = null;
            ScopeEntry entry;
            int scopeIndex = -1;
            int suffixIndex = 0;
            if ((names.Length > 1) && this.TryScopeLookup(System.Data.Common.EntitySql.TypeResolver.GetFullName(names), out entry, out scopeIndex))
            {
                this.SetScopeRegionCorrelationFlag(scopeIndex);
                return this.GetExpressionFromScopeEntry(entry, scopeIndex);
            }
            if (this.TryScopeLookup(names[0], out entry, out scopeIndex))
            {
                if ((entry.Kind == ScopeEntryKind.JoinSourceVar) && !this.CurrentScopeRegionFlags.IsInsideJoinOnPredicate)
                {
                    throw EntityUtil.EntitySqlError(errCtx, Strings.InvalidJoinLeftCorrelation);
                }
                this.SetScopeRegionCorrelationFlag(scopeIndex);
                names = System.Data.Common.EntitySql.TypeResolver.TrimNamesPrefix(names, 1);
                convExpr = this.GetExpressionFromScopeEntry(entry, scopeIndex);
                definingType = convExpr.ResultType;
            }
            else
            {
                KeyValuePair<string, TypeUsage> pair;
                if (this._variables.TryGetValue(names[0], out pair))
                {
                    names = System.Data.Common.EntitySql.TypeResolver.TrimNamesPrefix(names, 1);
                    convExpr = this.CmdTree.CreateVariableReferenceExpression(pair.Key, pair.Value);
                    definingType = convExpr.ResultType;
                }
                else if (this.TryResolveAsEntitySet(names, errCtx, out suffixIndex, out convExpr))
                {
                    definingType = convExpr.ResultType;
                }
                else
                {
                    int matchCount = 0;
                    definingType = this.TypeResolver.ResolveBaseType(names, out suffixIndex, out matchCount);
                    if (matchCount > 1)
                    {
                        throw EntityUtil.EntitySqlError(errCtx, Strings.AmbiguousName(System.Data.Common.EntitySql.TypeResolver.GetFullName(names)));
                    }
                }
            }
            if (definingType != null)
            {
                return this.ResolveIdentifierChain(names, suffixIndex, definingType, convExpr, errCtx);
            }
            return null;
        }

        internal DbExpression ResolveIdentifierChain(string[] names, int suffixIndex, DbExpression innerExpr, ErrorContext errCtx) => 
            this.ResolveIdentifierChain(names, suffixIndex, innerExpr.ResultType, innerExpr, errCtx);

        private DbExpression ResolveIdentifierChain(string[] names, int suffixIndex, TypeUsage definingType, DbExpression innerExpr, ErrorContext errCtx)
        {
            DbExpression expression = innerExpr;
            if (names.Length > 0)
            {
                TypeUsage resultType = definingType;
                for (int i = suffixIndex; i < names.Length; i++)
                {
                    expression = this.ResolveIdentifierElement(resultType, expression, names[i], errCtx);
                    resultType = expression.ResultType;
                }
            }
            return expression;
        }

        internal DbExpression ResolveIdentifierElement(TypeUsage definingType, DbExpression innerExpr, string name, ErrorContext errCtx)
        {
            DbExpression convExpr = null;
            if (!this.TryResolveAsProperty(name, (innerExpr == null) ? definingType : innerExpr.ResultType, innerExpr, errCtx, out convExpr) && !this.TryResolveAsRef(name, (innerExpr == null) ? definingType : innerExpr.ResultType, innerExpr, errCtx, out convExpr))
            {
                throw CqlErrorHelper.ReportIdentifierElementError(errCtx, name, definingType);
            }
            return convExpr;
        }

        internal void ResolveNameAsStaticMethodOrFunction(MethodExpr methodExpr, out TypeUsage constructorType, out TypeUsage staticMethodType, out IList<EdmFunction> functionType)
        {
            DotExpr methodPrefixExpr = methodExpr.MethodPrefixExpr;
            int matchCount = 0;
            int num2 = 0;
            constructorType = null;
            staticMethodType = null;
            functionType = null;
            constructorType = this.TypeResolver.ResolveNameAsType(methodPrefixExpr.Names, methodPrefixExpr.Names.Length, out matchCount);
            if (0 < matchCount)
            {
                num2++;
                if (!TypeSemantics.IsStructuralType(constructorType))
                {
                    throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.InvalidCtorUseOnType(TypeHelpers.GetFullName(constructorType)));
                }
                if (1 < matchCount)
                {
                    methodPrefixExpr.ErrCtx.ErrorContextInfo = "CtxGenericTypeCtor";
                    throw EntityUtil.EntitySqlError(methodPrefixExpr.ErrCtx, Strings.MultipleMatchesForName(Strings.LocalizedType, methodPrefixExpr.FullName));
                }
            }
            List<string> foundNamespaces = null;
            functionType = this.TypeResolver.ResolveNameAsFunction(methodPrefixExpr.Names, true, out matchCount, out foundNamespaces);
            if (0 < matchCount)
            {
                num2++;
                if (1 < matchCount)
                {
                    methodExpr.ErrCtx.ErrorContextInfo = "CtxGenericFunctionCall";
                    CqlErrorHelper.ReportAmbiguousFunctionError(methodExpr, foundNamespaces);
                }
            }
            if (1 < num2)
            {
                throw EntityUtil.EntitySqlError(methodPrefixExpr.Identifier.ErrCtx, Strings.AmbiguousFunctionMethodCtorName(methodPrefixExpr.FullName));
            }
            if (num2 == 0)
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.CannotResolveNameToFunction(methodPrefixExpr.FullName));
            }
        }

        internal TypeUsage ResolveNameAsType(string[] names, Expr astTypeExpression)
        {
            int matchCount = 0;
            ErrorContext errCtx = astTypeExpression.ErrCtx;
            TypeUsage typeUsage = this.TypeResolver.ResolveNameAsType(names, names.Length, out matchCount);
            if (typeUsage == null)
            {
                CqlErrorHelper.ReportTypeResolutionError(names, astTypeExpression, this);
            }
            if (matchCount > 1)
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.AmbiguousTypeName(System.Data.Common.EntitySql.TypeResolver.GetFullName(names)));
            }
            MethodExpr methodExpr = astTypeExpression as MethodExpr;
            if (methodExpr != null)
            {
                typeUsage = this.HandleParametrizedType(typeUsage, methodExpr);
            }
            return typeUsage;
        }

        internal void RollbackToSavepoint(SavePoint sp)
        {
            this._staticContext.RollbackToSavepoint(sp);
        }

        internal void SetCommandTreeFactory(CommandExpr astCommandExpr)
        {
            EntityUtil.CheckArgumentNull<CommandExpr>(astCommandExpr, "astCommandExpr");
            if (this._commandTree != null)
            {
                throw EntityUtil.EntitySqlError(Strings.CommandTreeCanOnlyBeSetOnce);
            }
            switch (astCommandExpr.QueryExpr.ExprKind)
            {
                case AstExprKind.Generic:
                case AstExprKind.Query:
                    this._commandTree = new DbQueryCommandTree(this.TypeResolver.Perspective.MetadataWorkspace, this.TypeResolver.Perspective.TargetDataspace);
                    return;
            }
            throw EntityUtil.Argument(Strings.UnknownAstExpressionType);
        }

        internal void SetCommandTreeFactory(CommandExpr astCommandExpr, DbCommandTree commandTree)
        {
            EntityUtil.CheckArgumentNull<CommandExpr>(astCommandExpr, "astCommandExpr");
            EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
            if (this._commandTree != null)
            {
                throw EntityUtil.EntitySqlError(Strings.CommandTreeCanOnlyBeSetOnce);
            }
            if (!(commandTree is DbQueryCommandTree) || ((astCommandExpr.ExprKind != AstExprKind.Query) && (astCommandExpr.ExprKind != AstExprKind.Generic)))
            {
                throw EntityUtil.Argument(Strings.UnknownAstExpressionType);
            }
            this._commandTree = commandTree;
        }

        internal void SetCurrentScopeKind(ScopeEntryKind scopeKind)
        {
            this.CurrentScopeRegionFlags.ScopeEntryKind = scopeKind;
        }

        internal void SetCurrentScopeVarKind(FromClauseItemKind fromClauseItemKind)
        {
            if (fromClauseItemKind == FromClauseItemKind.JoinFromClause)
            {
                this.SetCurrentScopeKind(ScopeEntryKind.JoinSourceVar);
            }
            else if (fromClauseItemKind == FromClauseItemKind.ApplyFromClause)
            {
                this.SetCurrentScopeKind(ScopeEntryKind.ApplySourceVar);
            }
            else
            {
                this.SetCurrentScopeKind(ScopeEntryKind.SourceVar);
            }
        }

        internal void SetScopeRegionCorrelationFlag(int scopeIndex)
        {
            for (int i = 0; i < this._scopeRegionFlags.Count; i++)
            {
                if (scopeIndex > this._scopeRegionSavepoints[i].ScopeIndex)
                {
                    this._scopeRegionFlags[i].WasResolutionCorrelated = true;
                    return;
                }
            }
        }

        internal void SetScopeView(ScopeViewKind viewKind)
        {
            this.CurrentScopeRegionFlags.ScopeViewKind = viewKind;
            switch (this.CurrentScopeRegionFlags.ScopeViewKind)
            {
                case ScopeViewKind.All:
                case ScopeViewKind.CurrentContext:
                case ScopeViewKind.GroupScope:
                    this._scopeIndexHighMark = 0;
                    return;

                case ScopeViewKind.CurrentScopeRegion:
                    this._scopeIndexHighMark = this.CurrentScopeRegionSavePoint.ScopeIndex;
                    return;

                case ScopeViewKind.CurrentAndPreviousScope:
                    this._scopeIndexHighMark = this.CurrentScopeIndex - 1;
                    return;

                case ScopeViewKind.CurrentScope:
                    this._scopeIndexHighMark = this.CurrentScopeIndex;
                    return;
            }
        }

        private bool TryResolveAsEntitySet(string[] names, ErrorContext errCtx, out int suffixIndex, out DbExpression convExpr)
        {
            convExpr = null;
            suffixIndex = 0;
            EntityContainer entityContainer = null;
            if (names.Length == 1)
            {
                entityContainer = this.TypeResolver.Perspective.GetDefaultContainer();
                suffixIndex = 0;
            }
            else if (this.TypeResolver.Perspective.TryGetEntityContainer(names[0], true, out entityContainer))
            {
                suffixIndex = 1;
            }
            else
            {
                return false;
            }
            if (entityContainer != null)
            {
                EntitySetBase outSet = null;
                if (this.TypeResolver.Perspective.TryGetExtent(entityContainer, names[suffixIndex], true, out outSet))
                {
                    convExpr = this.CmdTree.CreateScanExpression(outSet);
                    suffixIndex++;
                    return true;
                }
                if (names.Length > 1)
                {
                    throw EntityUtil.EntitySqlError(errCtx, Strings.EntitySetIsDoesNotBelongToEntityContainer(names[1], names[0]));
                }
            }
            if ((names.Length == 1) && this.TypeResolver.Perspective.TryGetEntityContainer(names[0], true, out entityContainer))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.MissingEntitySetName(names[0]));
            }
            return false;
        }

        private bool TryResolveAsProperty(string name, TypeUsage definingType, DbExpression innerExpr, ErrorContext errCtx, out DbExpression convExpr)
        {
            convExpr = null;
            if (Helper.IsStructuralType(definingType.EdmType))
            {
                EdmMember outMember = null;
                if (this.TypeResolver.Perspective.TryGetMember((StructuralType) definingType.EdmType, name, true, out outMember) && (outMember != null))
                {
                    if (innerExpr == null)
                    {
                        throw EntityUtil.EntitySqlError(errCtx, Strings.StaticMembersAreNotSupported(TypeHelpers.GetFullName(definingType.EdmType), name));
                    }
                    convExpr = this.CmdTree.CreatePropertyExpression(name, true, innerExpr);
                    return true;
                }
            }
            return false;
        }

        private bool TryResolveAsRef(string name, TypeUsage definingType, DbExpression innerExpr, ErrorContext errCtx, out DbExpression convExpr)
        {
            convExpr = null;
            if (!TypeSemantics.IsReferenceType(definingType))
            {
                return false;
            }
            convExpr = this.CmdTree.CreateDerefExpression(innerExpr);
            TypeUsage resultType = convExpr.ResultType;
            if (!this.TryResolveAsProperty(name, convExpr.ResultType, convExpr, errCtx, out convExpr))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.InvalidDeRefProperty(name, TypeHelpers.GetFullName(resultType), TypeHelpers.GetFullName(definingType)));
            }
            return true;
        }

        internal bool TryScopeLookup(string key, out ScopeEntry scopeEntry)
        {
            int num;
            return this.TryScopeLookup(key, out scopeEntry, out num);
        }

        internal bool TryScopeLookup(string key, out ScopeEntry scopeEntry, out int scopeIndex) => 
            this.TryScopeLookup(key, false, out scopeEntry, out scopeIndex);

        internal bool TryScopeLookup(string key, bool ignoreGroupScope, out ScopeEntry scopeEntry, out int scopeIndex)
        {
            scopeEntry = null;
            scopeIndex = -1;
            int num = 0;
            for (num = this.CurrentScopeIndex; num >= this._scopeIndexHighMark; num--)
            {
                if (this._staticContext.GetScopeByIndex(num).TryLookup(key, out scopeEntry))
                {
                    if ((!ignoreGroupScope && this.CurrentScopeRegionFlags.IsInGroupScope) && (scopeEntry.VarKind == SourceVarKind.GroupInput))
                    {
                        return false;
                    }
                    scopeIndex = num;
                    return true;
                }
            }
            return false;
        }

        internal void UndoFixupGroupSourceVarBindings(DbVariableReferenceExpression originalSourceVar)
        {
            this.InternalFixupGroupSourceVarBindings(originalSourceVar, null);
        }

        internal static void ValidateDistinctProjection(SelectClause selectClause, TypeUsage projectionType)
        {
            TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(projectionType);
            if (!TypeHelpers.IsValidDistinctOpType(elementTypeUsage))
            {
                ErrorContext errCtx = selectClause.Items[0].Expr.ErrCtx;
                if (TypeSemantics.IsRowType(elementTypeUsage))
                {
                    RowType edmType = elementTypeUsage.EdmType as RowType;
                    for (int i = 0; i < edmType.Members.Count; i++)
                    {
                        if (!TypeSemantics.IsEqualComparable(edmType.Members[i].TypeUsage))
                        {
                            errCtx = selectClause.Items[i].Expr.ErrCtx;
                            break;
                        }
                    }
                }
                throw EntityUtil.EntitySqlError(errCtx, Strings.SelectDistinctMustBeEqualComparable);
            }
        }

        private Dictionary<string, TypeUsage> ValidateParameters(Dictionary<string, TypeUsage> paramDefs)
        {
            Dictionary<string, TypeUsage> dictionary = new Dictionary<string, TypeUsage>(this._stringComparer);
            if (paramDefs != null)
            {
                foreach (KeyValuePair<string, TypeUsage> pair in paramDefs)
                {
                    if (dictionary.ContainsKey(pair.Key))
                    {
                        throw EntityUtil.EntitySqlError(Strings.MultipleDefinitionsOfParameter(pair.Key));
                    }
                    if (!ValidateParameterType(pair.Value))
                    {
                        throw EntityUtil.EntitySqlError(Strings.InvalidParameterType(pair.Key));
                    }
                    dictionary.Add(pair.Key, pair.Value);
                }
            }
            return dictionary;
        }

        private static bool ValidateParameterType(TypeUsage paramType)
        {
            if ((paramType == null) || (paramType.EdmType == null))
            {
                return false;
            }
            if (!TypeSemantics.IsPrimitiveType(paramType))
            {
                return (paramType.EdmType is EnumType);
            }
            return true;
        }

        internal static void ValidateQueryResultType(TypeUsage resultType, ErrorContext errCtx)
        {
            if (Helper.IsCollectionType(resultType.EdmType))
            {
                ValidateQueryResultType(((CollectionType) resultType.EdmType).TypeUsage, errCtx);
            }
            else if (Helper.IsRowType(resultType.EdmType))
            {
                foreach (EdmProperty property in ((RowType) resultType.EdmType).Properties)
                {
                    ValidateQueryResultType(property.TypeUsage, errCtx);
                }
            }
            else if (Helper.IsAssociationType(resultType.EdmType))
            {
                throw EntityUtil.EntitySqlError(errCtx, Strings.InvalidQueryResultType(resultType.Identity));
            }
        }

        private Dictionary<string, KeyValuePair<string, TypeUsage>> ValidateVariables(Dictionary<string, TypeUsage> varDefs)
        {
            Dictionary<string, KeyValuePair<string, TypeUsage>> dictionary = new Dictionary<string, KeyValuePair<string, TypeUsage>>(this._stringComparer);
            if (varDefs != null)
            {
                foreach (KeyValuePair<string, TypeUsage> pair in varDefs)
                {
                    if (dictionary.ContainsKey(pair.Key))
                    {
                        throw EntityUtil.EntitySqlError(Strings.MultipleDefinitionsOfVariable(pair.Key));
                    }
                    dictionary.Add(pair.Key, new KeyValuePair<string, TypeUsage>(pair.Key, pair.Value));
                }
            }
            return dictionary;
        }

        internal DbCommandTree CmdTree =>
            this._commandTree;

        internal int CurrentScopeIndex =>
            this._staticContext.CurrentScopeIndex;

        internal ScopeEntryKind CurrentScopeKind =>
            this.CurrentScopeRegionFlags.ScopeEntryKind;

        internal ScopeRegionFlags CurrentScopeRegionFlags =>
            this._scopeRegionFlags[0];

        internal SavePoint CurrentScopeRegionSavePoint =>
            this._scopeRegionSavepoints[0];

        internal Dictionary<string, TypeUsage> Parameters =>
            this._parameters;

        internal System.Data.Common.EntitySql.ParserOptions ParserOptions =>
            this._parserOptions;

        internal StringComparer ScopeStringComparer =>
            this._stringComparer;

        internal System.Data.Common.EntitySql.TypeResolver TypeResolver =>
            this._typeResolver;

        internal Dictionary<string, KeyValuePair<string, TypeUsage>> Variables =>
            this._variables;

        internal sealed class ScopeDisposer : IDisposable
        {
            private SemanticResolver _semanticResolver;

            internal ScopeDisposer(SemanticResolver semanticResolver)
            {
                this._semanticResolver = semanticResolver;
            }

            public void Dispose()
            {
                this._semanticResolver.LeaveScope();
            }
        }

        internal sealed class ScopeRegionDisposer : IDisposable
        {
            private SemanticResolver _semanticResolver;

            internal ScopeRegionDisposer(SemanticResolver semanticResolver)
            {
                this._semanticResolver = semanticResolver;
            }

            public void Dispose()
            {
                this._semanticResolver.LeaveScopeRegion();
            }
        }

        internal enum ScopeViewKind
        {
            All,
            CurrentContext,
            CurrentScopeRegion,
            CurrentAndPreviousScope,
            CurrentScope,
            GroupScope
        }
    }
}

