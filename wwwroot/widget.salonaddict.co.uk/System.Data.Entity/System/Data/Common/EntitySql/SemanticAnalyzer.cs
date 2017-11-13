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
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal sealed class SemanticAnalyzer
    {
        private static readonly Dictionary<Type, AstExprConverter> _astExprConverters = CreateAstExprConverters();
        private static readonly Dictionary<BuiltInKind, BuiltInExprConverter> _builtInExprConverter = CreateBuiltInExprConverter();
        private SemanticResolver _sr;
        private static CommandConverter[] commandConverters = new CommandConverter[] { new CommandConverter(SemanticAnalyzer.ConvertGeneralExpression), new CommandConverter(SemanticAnalyzer.ConvertGeneralExpression) };

        internal SemanticAnalyzer(SemanticResolver sr)
        {
            EntityUtil.CheckArgumentNull<SemanticResolver>(sr, "sr");
            this._sr = sr;
        }

        internal DbCommandTree Analyze(Expr astExpr) => 
            ConvertCommand(this.Initialize(astExpr, null), this._sr);

        internal DbExpression Analyze(Expr astExpr, DbCommandTree commandTree) => 
            ConvertRootExpression(this.Initialize(astExpr, commandTree).QueryExpr, this._sr);

        private static DbExpression Convert(Expr astExpr, SemanticResolver sr)
        {
            if (astExpr == null)
            {
                return null;
            }
            AstExprConverter converter = _astExprConverters[astExpr.GetType()];
            return converter?.Invoke(astExpr, sr);
        }

        private static DbExpression ConvertAggregateFunctionInGroupScope(MethodExpr methodExpr, IList<EdmFunction> functionTypes, SemanticResolver sr)
        {
            DbExpression converted = null;
            if (!TryConvertAsResolvedGroupAggregate(methodExpr, sr, out converted))
            {
                if (TryConvertAsOrdinaryFunctionInGroup(methodExpr, functionTypes, sr, out converted))
                {
                    return converted;
                }
                if (!TryConvertAsGroupAggregateFunction(methodExpr, functionTypes, sr, out converted))
                {
                    throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.FailedToResolveAggregateFunction(methodExpr.MethodPrefixExpr.FullName));
                }
            }
            return converted;
        }

        private static Pair<DbExpression, DbExpression> ConvertArithmeticArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            DbExpression expression = Convert(astBuiltInExpr.Arg1, sr);
            if (!TypeSemantics.IsNumericType(expression.ResultType) && !SemanticResolver.IsNullExpression(expression))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.ExpressionMustBeNumericType);
            }
            DbExpression expression2 = null;
            if (astBuiltInExpr.Arg2 != null)
            {
                expression2 = Convert(astBuiltInExpr.Arg2, sr);
                if (!TypeSemantics.IsNumericType(expression2.ResultType) && !SemanticResolver.IsNullExpression(expression2))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.ExpressionMustBeNumericType);
                }
                if (TypeHelpers.GetCommonTypeUsage(expression.ResultType, expression2.ResultType) == null)
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.ErrCtx, Strings.ArgumentTypesAreIncompatible(expression.ResultType.EdmType.FullName, expression2.ResultType.EdmType.FullName));
                }
            }
            return sr.EnsureTypedNulls(expression, expression2, astBuiltInExpr.ErrCtx, () => Strings.InvalidNullArithmetic);
        }

        private static DbExpression ConvertBuiltIn(Expr astExpr, SemanticResolver sr)
        {
            if (astExpr == null)
            {
                return null;
            }
            BuiltInExpr astBltInExpr = (BuiltInExpr) astExpr;
            BuiltInExprConverter converter = _builtInExprConverter[astBltInExpr.Kind];
            return converter?.Invoke(astBltInExpr, sr);
        }

        private static DbExpression ConvertCaseExpr(Expr expr, SemanticResolver sr)
        {
            CaseExpr expr2 = (CaseExpr) expr;
            List<DbExpression> whenExpressions = new List<DbExpression>(expr2.WhenThenExprList.Count);
            PairOfLists<DbExpression, TypeUsage> lists = new PairOfLists<DbExpression, TypeUsage>();
            for (int i = 0; i < expr2.WhenThenExprList.Count; i++)
            {
                WhenThenExpr expr3 = expr2.WhenThenExprList[i];
                DbExpression item = Convert(expr3.WhenExpr, sr);
                if (!TypeResolver.IsBooleanType(item.ResultType))
                {
                    throw EntityUtil.EntitySqlError(expr3.WhenExpr.ErrCtx, Strings.ExpressionTypeMustBeBoolean);
                }
                whenExpressions.Add(item);
                DbExpression left = Convert(expr3.ThenExpr, sr);
                lists.Add(left, left.ResultType);
            }
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(lists.Right);
            if (commonTypeUsage == null)
            {
                throw EntityUtil.EntitySqlError(expr2.WhenThenExprList.Expressions[0].ThenExpr.ErrCtx, Strings.InvalidCaseThenTypes);
            }
            if ((expr2.ElseExpr == null) && TypeSemantics.IsNullType(commonTypeUsage))
            {
                throw EntityUtil.EntitySqlError(expr2.WhenThenExprList.Expressions[0].ThenExpr.ErrCtx, Strings.InvalidCaseThenNullType);
            }
            DbExpression expression3 = null;
            if (expr2.ElseExpr != null)
            {
                expression3 = Convert(expr2.ElseExpr, sr);
                commonTypeUsage = TypeHelpers.GetCommonTypeUsage(commonTypeUsage, expression3.ResultType);
                if (commonTypeUsage == null)
                {
                    throw EntityUtil.EntitySqlError(expr2.ElseExpr.ErrCtx, Strings.InvalidCaseElseType);
                }
                if (TypeSemantics.IsNullType(commonTypeUsage))
                {
                    throw EntityUtil.EntitySqlError(expr2.ElseExpr.ErrCtx, Strings.InvalidCaseWhenThenNullType);
                }
                if (SemanticResolver.IsNullExpression(expression3))
                {
                    SemanticResolver.EnsureValidTypeForNullExpression(commonTypeUsage, expr2.ElseExpr.ErrCtx);
                    expression3 = sr.CmdTree.CreateNullExpression(commonTypeUsage);
                }
            }
            else if (TypeSemantics.IsCollectionType(commonTypeUsage))
            {
                expression3 = sr.CmdTree.CreateNewEmptyCollectionExpression(commonTypeUsage);
            }
            else
            {
                SemanticResolver.EnsureValidTypeForNullExpression(commonTypeUsage, expr2.ErrCtx);
                expression3 = sr.CmdTree.CreateNullExpression(commonTypeUsage);
            }
            for (int j = 0; j < lists.Count; j++)
            {
                if (SemanticResolver.IsNullExpression(lists.Left[j]))
                {
                    SemanticResolver.EnsureValidTypeForNullExpression(commonTypeUsage, expr2.WhenThenExprList[j].ThenExpr.ErrCtx);
                    lists[j] = new Pair<DbExpression, TypeUsage>(sr.CmdTree.CreateNullExpression(commonTypeUsage), commonTypeUsage);
                }
            }
            return sr.CmdTree.CreateCaseExpression(whenExpressions, lists.Left, expression3);
        }

        private static DbCommandTree ConvertCommand(CommandExpr astCommandExpr, SemanticResolver sr)
        {
            EntityUtil.CheckArgumentNull<CommandExpr>(astCommandExpr, "astCommandExpr");
            Expr queryExpr = astCommandExpr.QueryExpr;
            return commandConverters[(int) queryExpr.ExprKind](queryExpr, sr);
        }

        private static DbExpression ConvertCreateRefExpr(Expr astExpr, SemanticResolver sr)
        {
            CreateRefExpr expr = (CreateRefExpr) astExpr;
            DbScanExpression expression2 = Convert(expr.EntitySet, sr) as DbScanExpression;
            if (expression2 == null)
            {
                throw EntityUtil.EntitySqlError(expr.EntitySet.ErrCtx, Strings.ExprIsNotValidEntitySetForCreateRef);
            }
            EntitySet target = expression2.Target as EntitySet;
            if (target == null)
            {
                throw EntityUtil.EntitySqlError(expr.EntitySet.ErrCtx, Strings.ExprIsNotValidEntitySetForCreateRef);
            }
            DbExpression expression = Convert(expr.Keys, sr);
            SemanticResolver.EnsureIsNotUntypedNull(expression, expr.Keys.ErrCtx);
            RowType edmType = expression.ResultType.EdmType as RowType;
            if (edmType == null)
            {
                throw EntityUtil.EntitySqlError(expr.Keys.ErrCtx, Strings.InvalidCreateRefKeyType);
            }
            RowType type2 = TypeHelpers.CreateKeyRowType(target.ElementType, sr.CmdTree.MetadataWorkspace);
            if (type2.Members.Count != edmType.Members.Count)
            {
                throw EntityUtil.EntitySqlError(expr.Keys.ErrCtx, Strings.ImcompatibleCreateRefKeyType);
            }
            if (!TypeSemantics.IsEquivalentOrPromotableTo(expression.ResultType, TypeUsage.Create(type2)))
            {
                throw EntityUtil.EntitySqlError(expr.Keys.ErrCtx, Strings.ImcompatibleCreateRefKeyElementType);
            }
            if (expr.TypeIdentifier != null)
            {
                TypeUsage type = ConvertTypeIdentifier(expr.TypeIdentifier, sr);
                if (!TypeSemantics.IsEntityType(type))
                {
                    throw EntityUtil.EntitySqlError(expr.TypeIdentifier.ErrCtx, Strings.CreateRefTypeIdentifierMustSpecifyAnEntityType(type.EdmType.Identity, type.EdmType.BuiltInTypeKind.ToString()));
                }
                if (!TypeSemantics.IsValidPolymorphicCast(target.ElementType, type.EdmType))
                {
                    throw EntityUtil.EntitySqlError(expr.TypeIdentifier.ErrCtx, Strings.CreateRefTypeIdentifierMustBeASubOrSuperType(target.ElementType.Identity, type.EdmType.FullName));
                }
                return sr.CmdTree.CreateRefExpression(target, expression, (EntityType) type.EdmType);
            }
            return sr.CmdTree.CreateRefExpression(target, expression);
        }

        private static DbExpression ConvertDeRefExpr(Expr astExpr, SemanticResolver sr)
        {
            DerefExpr expr = (DerefExpr) astExpr;
            DbExpression reference = null;
            reference = Convert(expr.RefExpr, sr);
            if (!TypeSemantics.IsReferenceType(reference.ResultType))
            {
                throw EntityUtil.EntitySqlError(expr.RefExpr.ErrCtx, Strings.DeRefArgIsNotOfRefType(reference.ResultType.EdmType.FullName));
            }
            return sr.CmdTree.CreateDerefExpression(reference);
        }

        private static DbExpression ConvertDotExpr(Expr expr, SemanticResolver sr)
        {
            DotExpr dotExpr = (DotExpr) expr;
            DbExpression expression = null;
            if (dotExpr.IsDottedIdentifier)
            {
                expression = sr.ResolveIdentifier(dotExpr.Names, dotExpr.ErrCtx);
                if (expression == null)
                {
                    CqlErrorHelper.ReportIdentifierError(expr, sr);
                }
                return expression;
            }
            return ConvertDotExpressionProcess(dotExpr, sr);
        }

        private static DbExpression ConvertDotExpressionProcess(DotExpr dotExpr, SemanticResolver sr) => 
            sr.ResolveIdentifierChain(dotExpr.Names, 0, Convert(dotExpr.LeftMostExpression, sr), dotExpr.ErrCtx);

        private static Pair<DbExpression, DbExpression> ConvertEqualCompArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            Pair<DbExpression, DbExpression> pair = sr.EnsureTypedNulls(Convert(astBuiltInExpr.Arg1, sr), Convert(astBuiltInExpr.Arg2, sr), astBuiltInExpr.ErrCtx, () => Strings.InvalidNullComparison);
            if (!TypeSemantics.IsEqualComparableTo(pair.Left.ResultType, pair.Right.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.ErrCtx, Strings.ArgumentTypesAreIncompatible(pair.Left.ResultType.EdmType.FullName, pair.Right.ResultType.EdmType.FullName));
            }
            return pair;
        }

        private static List<DbExpression> ConvertFunctionArguments(ExprList<Expr> astExprList, SemanticResolver sr)
        {
            List<DbExpression> list = new List<DbExpression>();
            if (astExprList != null)
            {
                for (int i = 0; i < astExprList.Count; i++)
                {
                    list.Add(Convert(astExprList[i], sr));
                }
            }
            return list;
        }

        private static DbCommandTree ConvertGeneralExpression(Expr astExpr, SemanticResolver sr)
        {
            DbExpression expression = ConvertRootExpression(astExpr, sr);
            DbQueryCommandTree cmdTree = (DbQueryCommandTree) sr.CmdTree;
            cmdTree.Query = expression;
            return cmdTree;
        }

        private static DbExpression ConvertIdentifier(Expr expr, SemanticResolver sr)
        {
            Identifier identifier = (Identifier) expr;
            DbExpression expression = sr.ResolveIdentifier(new string[] { identifier.Name }, expr.ErrCtx);
            if (expression == null)
            {
                CqlErrorHelper.ReportIdentifierError(expr, sr);
            }
            return expression;
        }

        private static Pair<DbExpression, DbExpression> ConvertInExprArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            DbExpression expression = Convert(astBuiltInExpr.Arg1, sr);
            if (TypeSemantics.IsCollectionType(expression.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustNotBeCollection);
            }
            DbExpression right = Convert(astBuiltInExpr.Arg2, sr);
            if (!TypeSemantics.IsCollectionType(right.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.RightSetExpressionArgsMustBeCollection);
            }
            if (SemanticResolver.IsNullExpression(expression))
            {
                TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(right.ResultType);
                SemanticResolver.EnsureValidTypeForNullExpression(elementTypeUsage, astBuiltInExpr.Arg1.ErrCtx);
                expression = sr.CmdTree.CreateNullExpression(elementTypeUsage);
            }
            else
            {
                TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(expression.ResultType, TypeHelpers.GetElementTypeUsage(right.ResultType));
                if ((commonTypeUsage == null) || !TypeHelpers.IsValidInOpType(commonTypeUsage))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.ErrCtx, Strings.InvalidInExprArgs(expression.ResultType.EdmType.FullName, right.ResultType.EdmType.FullName));
                }
            }
            return new Pair<DbExpression, DbExpression>(expression, right);
        }

        private static DbExpression ConvertKeyExpr(Expr astExpr, SemanticResolver sr)
        {
            KeyExpr expr = (KeyExpr) astExpr;
            DbExpression expression = Convert(expr.RefExpr, sr);
            SemanticResolver.EnsureIsNotUntypedNull(expression, expr.RefExpr.ErrCtx);
            if (TypeSemantics.IsEntityType(expression.ResultType))
            {
                expression = sr.CmdTree.CreateEntityRefExpression(expression);
            }
            else if (!TypeSemantics.IsReferenceType(expression.ResultType))
            {
                throw EntityUtil.EntitySqlError(expr.RefExpr.ErrCtx, Strings.InvalidKeyArgument(TypeHelpers.GetFullName(expression.ResultType)));
            }
            return sr.CmdTree.CreateRefKeyExpression(expression);
        }

        private static DbExpression ConvertLiteral(Expr expr, SemanticResolver sr)
        {
            Literal literal = (Literal) expr;
            if (literal.IsNullLiteral)
            {
                return new UntypedNullExpression(sr.CmdTree);
            }
            return sr.CmdTree.CreateConstantExpression(literal.Value, sr.TypeResolver.GetLiteralTypeUsage(literal));
        }

        private static Pair<DbExpression, DbExpression> ConvertLogicalArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            DbExpression left = Convert(astBuiltInExpr.Arg1, sr);
            if (left is UntypedNullExpression)
            {
                left = sr.CmdTree.CreateNullExpression(sr.TypeResolver.BooleanType);
            }
            DbExpression right = Convert(astBuiltInExpr.Arg2, sr);
            if (right is UntypedNullExpression)
            {
                right = sr.CmdTree.CreateNullExpression(sr.TypeResolver.BooleanType);
            }
            if (!TypeResolver.IsBooleanType(left.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeBoolean);
            }
            if ((right != null) && !TypeResolver.IsBooleanType(right.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.ExpressionTypeMustBeBoolean);
            }
            return new Pair<DbExpression, DbExpression>(left, right);
        }

        private static DbExpression ConvertMethodExpr(Expr expr, SemanticResolver sr)
        {
            MethodExpr methodExpr = (MethodExpr) expr;
            DotExpr methodPrefixExpr = methodExpr.MethodPrefixExpr;
            DbExpression baseExpr = (methodPrefixExpr.LeftMostExpression != null) ? Convert(methodPrefixExpr.LeftMostExpression, sr) : null;
            int prefixIndex = 0;
            if (((baseExpr == null) && methodPrefixExpr.IsDottedIdentifier) && (methodPrefixExpr.Names.Length > 0))
            {
                ScopeEntry entry;
                if (sr.TryScopeLookup(methodPrefixExpr.Names[0], out entry))
                {
                    if (!TypeResolver.IsValidTypeForMethodCall(entry.Expression.ResultType))
                    {
                        throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.DefiningTypeDoesNotSupportMethodCalls);
                    }
                    baseExpr = entry.Expression;
                    prefixIndex = 1;
                }
                else
                {
                    KeyValuePair<string, TypeUsage> pair;
                    if (sr.Variables.TryGetValue(methodPrefixExpr.Names[0], out pair))
                    {
                        baseExpr = sr.CmdTree.CreateVariableReferenceExpression(pair.Key, pair.Value);
                        prefixIndex = 1;
                    }
                }
            }
            if (baseExpr is UntypedNullExpression)
            {
                throw EntityUtil.EntitySqlError(methodPrefixExpr.LeftMostExpression.ErrCtx, Strings.ExpressionCannotBeNull);
            }
            if (baseExpr == null)
            {
                return ConvertStaticMethodOrFunction(methodExpr, sr);
            }
            return ConvertMethodInstance(baseExpr, methodExpr, prefixIndex, sr);
        }

        private static DbExpression ConvertMethodInstance(DbExpression baseExpr, MethodExpr methodExpr, int prefixIndex, SemanticResolver sr)
        {
            DotExpr methodPrefixExpr = methodExpr.MethodPrefixExpr;
            if (TypeSemantics.IsPrimitiveType(baseExpr.ResultType))
            {
                throw EntityUtil.EntitySqlError(methodPrefixExpr.LeftMostExpression.ErrCtx, Strings.MethodNotAllowedOnScalars);
            }
            DbExpression innerExpr = baseExpr;
            for (int i = prefixIndex; i < (methodPrefixExpr.Length - 1); i++)
            {
                innerExpr = sr.ResolveIdentifierElement(innerExpr.ResultType, innerExpr, methodPrefixExpr.Names[i], methodPrefixExpr.ErrCtx);
                if (innerExpr == null)
                {
                    throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.InvalidMethodPathElement(methodPrefixExpr.Names[i], innerExpr.ResultType.EdmType.FullName));
                }
            }
            List<DbExpression> args = ConvertFunctionArguments(methodExpr.Args, sr);
            return SemanticResolver.CreateInstanceMethod(innerExpr, args, methodExpr);
        }

        private static DbExpression ConvertMultisetConstructor(Expr expr, SemanticResolver sr)
        {
            MultisetConstructorExpr expr2 = (MultisetConstructorExpr) expr;
            if (expr2.ExprList == null)
            {
                throw EntityUtil.EntitySqlError(expr.ErrCtx, Strings.CannotCreateEmptyMultiset);
            }
            PairOfLists<TypeUsage, DbExpression> lists = ProcessExprList(expr2.ExprList, sr);
            TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(lists.Left);
            if (commonTypeUsage == null)
            {
                throw EntityUtil.EntitySqlError(expr.ErrCtx, Strings.MultisetElemsAreNotTypeCompatible);
            }
            if (TypeSemantics.IsNullType(commonTypeUsage))
            {
                throw EntityUtil.EntitySqlError(expr.ErrCtx, Strings.CannotCreateMultisetofNulls);
            }
            commonTypeUsage = TypeHelpers.GetReadOnlyType(commonTypeUsage);
            for (int i = 0; i < lists.Count; i++)
            {
                if (SemanticResolver.IsNullExpression(lists.Right[i]))
                {
                    SemanticResolver.EnsureValidTypeForNullExpression(commonTypeUsage, expr2.ExprList[i].ErrCtx);
                    lists.Right[i] = sr.CmdTree.CreateNullExpression(commonTypeUsage);
                }
            }
            return sr.CmdTree.CreateNewInstanceExpression(TypeHelpers.CreateCollectionTypeUsage(commonTypeUsage, true), lists.Right);
        }

        private static Pair<DbExpression, DbExpression> ConvertOrderCompArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            Pair<DbExpression, DbExpression> pair = sr.EnsureTypedNulls(Convert(astBuiltInExpr.Arg1, sr), Convert(astBuiltInExpr.Arg2, sr), astBuiltInExpr.ErrCtx, () => Strings.InvalidNullComparison);
            if (!TypeSemantics.IsOrderComparableTo(pair.Left.ResultType, pair.Right.ResultType))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.ErrCtx, Strings.ArgumentTypesAreIncompatible(pair.Left.ResultType.EdmType.FullName, pair.Right.ResultType.EdmType.FullName));
            }
            return pair;
        }

        private static DbExpression ConvertParameter(Expr expr, SemanticResolver sr)
        {
            KeyValuePair<string, TypeUsage> pair;
            Parameter parameter = (Parameter) expr;
            TypeUsage usage = null;
            if ((sr.Variables != null) && sr.Variables.TryGetValue(parameter.Name, out pair))
            {
                return sr.CmdTree.CreateVariableReferenceExpression(pair.Key, pair.Value);
            }
            if ((sr.Parameters == null) || !sr.Parameters.TryGetValue(parameter.Name, out usage))
            {
                throw EntityUtil.EntitySqlError(parameter.ErrCtx, Strings.ParameterWasNotDefined(parameter.Name));
            }
            sr.CmdTree.AddParameter(parameter.Name, TypeHelpers.GetReadOnlyType(usage));
            return sr.CmdTree.CreateParameterReferenceExpression(parameter.Name);
        }

        private static Pair<DbExpression, DbExpression> ConvertPlusOperands(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            DbExpression expression = Convert(astBuiltInExpr.Arg1, sr);
            if ((!TypeSemantics.IsNumericType(expression.ResultType) && !TypeSemantics.IsPrimitiveType(expression.ResultType, PrimitiveTypeKind.String)) && !SemanticResolver.IsNullExpression(expression))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.PlusLeftExpressionInvalidType);
            }
            DbExpression expression2 = Convert(astBuiltInExpr.Arg2, sr);
            if ((!TypeSemantics.IsNumericType(expression2.ResultType) && !TypeSemantics.IsPrimitiveType(expression2.ResultType, PrimitiveTypeKind.String)) && !SemanticResolver.IsNullExpression(expression2))
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.PlusRightExpressionInvalidType);
            }
            if (TypeHelpers.GetCommonTypeUsage(expression.ResultType, expression2.ResultType) == null)
            {
                throw EntityUtil.EntitySqlError(astBuiltInExpr.ErrCtx, Strings.ArgumentTypesAreIncompatible(expression.ResultType.EdmType.FullName, expression2.ResultType.EdmType.FullName));
            }
            return sr.EnsureTypedNulls(expression, expression2, astBuiltInExpr.ErrCtx, () => Strings.InvalidNullArithmetic);
        }

        private static DbExpression ConvertQuery(Expr expr, SemanticResolver sr)
        {
            QueryExpr queryExpr = (QueryExpr) expr;
            bool flag = ParserOptions.CompilationMode.RestrictedViewGenerationMode == sr.ParserOptions.ParserCompilationMode;
            ValidateAndCompensateQuery(queryExpr);
            using (sr.EnterScopeRegion())
            {
                DbExpressionBinding source = ProcessWhereClause(ProcessFromClause(queryExpr.FromClause, sr), queryExpr.WhereClause, sr);
                if (!flag)
                {
                    source = ProcessOrderByClause(ProcessHavingClause(ProcessGroupByClause(source, queryExpr, sr), queryExpr.HavingClause, sr), queryExpr, sr);
                }
                return ProcessSelectClause(source, queryExpr, sr);
            }
        }

        private static DbExpression ConvertRefExpr(Expr astExpr, SemanticResolver sr)
        {
            RefExpr expr = (RefExpr) astExpr;
            DbExpression entity = Convert(expr.RefArgExpr, sr);
            if (!TypeSemantics.IsEntityType(entity.ResultType))
            {
                throw EntityUtil.EntitySqlError(expr.RefArgExpr.ErrCtx, Strings.RefArgIsNotOfEntityType(entity.ResultType.EdmType.FullName));
            }
            return sr.CmdTree.CreateEntityRefExpression(entity);
        }

        private static DbRelatedEntityRef ConvertRelatedEntityRef(RelshipNavigationExpr relshipExpr, SemanticResolver sr)
        {
            DbExpression expression;
            RelationshipEndMember member;
            RelationshipEndMember member2;
            RelationshipType type;
            ValidateRelationshipTraversal(relshipExpr, true, sr, out expression, out type, out member, out member2);
            if ((RelationshipMultiplicity.One != member.RelationshipMultiplicity) && (member.RelationshipMultiplicity != RelationshipMultiplicity.ZeroOrOne))
            {
                throw EntityUtil.EntitySqlError(relshipExpr.ErrCtx, Strings.InvalidWithRelationshipTargetEndMultiplicity(member.Identity, member.RelationshipMultiplicity.ToString()));
            }
            return sr.CmdTree.CreateRelatedEntityRef(member2, member, expression);
        }

        private static DbExpression ConvertRelshipNavigationExpr(Expr astExpr, SemanticResolver sr)
        {
            DbExpression expression;
            RelationshipEndMember member;
            RelationshipEndMember member2;
            RelationshipType type;
            RelshipNavigationExpr relshipExpr = (RelshipNavigationExpr) astExpr;
            ValidateRelationshipTraversal(relshipExpr, false, sr, out expression, out type, out member, out member2);
            return sr.CmdTree.CreateRelationshipNavigationExpression(member, member2, expression);
        }

        private static DbExpression ConvertRootExpression(Expr astExpr, SemanticResolver sr)
        {
            DbExpression input = Convert(astExpr, sr);
            if (TypeSemantics.IsNullType(input.ResultType))
            {
                throw EntityUtil.EntitySqlError(astExpr.ErrCtx, Strings.ResultingExpressionTypeCannotBeNull);
            }
            if (input is DbScanExpression)
            {
                DbExpressionBinding binding = sr.CmdTree.CreateExpressionBinding(input, sr.GenerateInternalName("extent"));
                input = sr.CmdTree.CreateProjectExpression(binding, binding.Variable);
            }
            if (sr.ParserOptions.ParserCompilationMode == ParserOptions.CompilationMode.NormalMode)
            {
                SemanticResolver.ValidateQueryResultType(input.ResultType, astExpr.ErrCtx);
            }
            return input;
        }

        private static DbExpression ConvertRowConstructor(Expr expr, SemanticResolver sr)
        {
            RowConstructorExpr expr2 = (RowConstructorExpr) expr;
            Dictionary<string, TypeUsage> columns = new Dictionary<string, TypeUsage>(sr.ScopeStringComparer);
            List<DbExpression> args = new List<DbExpression>(expr2.AliasExprList.Count);
            for (int i = 0; i < expr2.AliasExprList.Count; i++)
            {
                AliasExpr aliasExpr = expr2.AliasExprList[i];
                DbExpression convertedExpression = Convert(aliasExpr.Expr, sr);
                string key = sr.InferAliasName(aliasExpr, convertedExpression);
                if (columns.ContainsKey(key))
                {
                    if (aliasExpr.HasAlias)
                    {
                        CqlErrorHelper.ReportAliasAlreadyUsedError(key, aliasExpr.AliasIdentifier.ErrCtx, Strings.InRowConstructor);
                    }
                    else
                    {
                        key = sr.GenerateInternalName("autoRowCol");
                    }
                }
                if (SemanticResolver.IsNullExpression(convertedExpression))
                {
                    throw EntityUtil.EntitySqlError(aliasExpr.Expr.ErrCtx, Strings.RowCtorElementCannotBeNull);
                }
                columns.Add(key, convertedExpression.ResultType);
                args.Add(convertedExpression);
            }
            return sr.CmdTree.CreateNewInstanceExpression(TypeHelpers.CreateRowTypeUsage(columns, true), args);
        }

        private static Pair<DbExpression, DbExpression> ConvertSetArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr)
        {
            DbExpression left = Convert(astBuiltInExpr.Arg1, sr);
            DbExpression right = null;
            if (astBuiltInExpr.Arg2 != null)
            {
                TypeUsage usage;
                if (!TypeSemantics.IsCollectionType(left.ResultType))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.LeftSetExpressionArgsMustBeCollection);
                }
                right = Convert(astBuiltInExpr.Arg2, sr);
                if (!TypeSemantics.IsCollectionType(right.ResultType))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.RightSetExpressionArgsMustBeCollection);
                }
                TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(left.ResultType);
                TypeUsage usage3 = TypeHelpers.GetElementTypeUsage(right.ResultType);
                if (!TypeSemantics.TryGetCommonType(elementTypeUsage, usage3, out usage))
                {
                    CqlErrorHelper.ReportIncompatibleCommonType(astBuiltInExpr.ErrCtx, elementTypeUsage, usage3);
                }
                if (astBuiltInExpr.Kind == BuiltInKind.UnionAll)
                {
                    if (Helper.IsAssociationType(elementTypeUsage.EdmType))
                    {
                        throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.InvalidAssociationTypeForUnion(elementTypeUsage.Identity));
                    }
                    if (Helper.IsAssociationType(usage3.EdmType))
                    {
                        throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.InvalidAssociationTypeForUnion(usage3.Identity));
                    }
                }
                else
                {
                    if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(left.ResultType)))
                    {
                        throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.PlaceholderSetArgTypeIsNotEqualComparable(astBuiltInExpr.Kind.ToString().ToUpperInvariant(), Strings.LocalizedLeft, TypeHelpers.GetElementTypeUsage(left.ResultType).EdmType.FullName));
                    }
                    if (!TypeHelpers.IsSetComparableOpType(TypeHelpers.GetElementTypeUsage(right.ResultType)))
                    {
                        throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg2.ErrCtx, Strings.PlaceholderSetArgTypeIsNotEqualComparable(astBuiltInExpr.Kind.ToString().ToUpperInvariant(), Strings.LocalizedRight, TypeHelpers.GetElementTypeUsage(right.ResultType).EdmType.FullName));
                    }
                }
            }
            else
            {
                if (!TypeSemantics.IsCollectionType(left.ResultType))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.InvalidUnarySetOpArgument(astBuiltInExpr.Name));
                }
                if ((astBuiltInExpr.Kind == BuiltInKind.Distinct) && !TypeHelpers.IsValidDistinctOpType(TypeHelpers.GetElementTypeUsage(left.ResultType)))
                {
                    throw EntityUtil.EntitySqlError(astBuiltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeEqualComparable);
                }
            }
            return new Pair<DbExpression, DbExpression>(left, right);
        }

        private static DbExpression ConvertSimpleInExpression(SemanticResolver sr, DbExpression left, DbExpression right)
        {
            DbNewInstanceExpression expression = (DbNewInstanceExpression) right;
            if (expression.Arguments.Count == 0)
            {
                return sr.CmdTree.CreateConstantExpression(false);
            }
            DbExpression expression2 = null;
            foreach (DbExpression expression3 in expression.Arguments)
            {
                DbExpression expression4 = left.Clone();
                DbExpression expression5 = sr.CmdTree.CreateEqualsExpression(expression4, expression3);
                if (expression2 == null)
                {
                    expression2 = expression5;
                }
                else
                {
                    expression2 = sr.CmdTree.CreateOrExpression(expression2, expression5);
                }
            }
            return expression2;
        }

        private static DbExpression ConvertStaticMethodOrFunction(MethodExpr methodExpr, SemanticResolver sr)
        {
            TypeUsage usage;
            TypeUsage usage2;
            IList<EdmFunction> list;
            sr.ResolveNameAsStaticMethodOrFunction(methodExpr, out usage, out usage2, out list);
            if (usage != null)
            {
                List<DbRelatedEntityRef> relshipExprList = null;
                if (methodExpr.HasRelationships)
                {
                    if (sr.ParserOptions.ParserCompilationMode == ParserOptions.CompilationMode.NormalMode)
                    {
                        throw EntityUtil.EntitySqlError(methodExpr.Relationships.ErrCtx, Strings.InvalidModeForWithRelationshipClause);
                    }
                    HashSet<string> set = new HashSet<string>();
                    relshipExprList = new List<DbRelatedEntityRef>(methodExpr.Relationships.Count);
                    for (int i = 0; i < methodExpr.Relationships.Count; i++)
                    {
                        RelshipNavigationExpr relshipExpr = methodExpr.Relationships[i];
                        DbRelatedEntityRef item = ConvertRelatedEntityRef(relshipExpr, sr);
                        string str = string.Join(":", new string[] { item.TargetEnd.DeclaringType.Identity, item.TargetEnd.Identity });
                        if (set.Contains(str))
                        {
                            throw EntityUtil.EntitySqlError(relshipExpr.ErrCtx, Strings.RelationshipTargetMustBeUnique(item.TargetEntityReference.ResultType.EdmType.Identity));
                        }
                        set.Add(str);
                        relshipExprList.Add(item);
                    }
                }
                return sr.CreateInstanceOfType(usage, ConvertFunctionArguments(methodExpr.Args, sr), relshipExprList, methodExpr);
            }
            if (usage2 != null)
            {
                return SemanticResolver.CreateStaticMethod(usage2, ConvertFunctionArguments(methodExpr.Args, sr), methodExpr);
            }
            if ((list == null) || (0 >= list.Count))
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.CannotResolveNameToFunction(methodExpr.MethodPrefixExpr.FullName));
            }
            if (TypeSemantics.IsAggregateFunction(list[0]) && sr.IsInAnyGroupScope())
            {
                return ConvertAggregateFunctionInGroupScope(methodExpr, list, sr);
            }
            return sr.CreateFunction(list, ConvertFunctionArguments(methodExpr.Args, sr), methodExpr);
        }

        private static Pair<DbExpression, TypeUsage> ConvertTypeExprArgs(BuiltInExpr astBuiltInExpr, SemanticResolver sr) => 
            new Pair<DbExpression, TypeUsage>(Convert(astBuiltInExpr.Arg1, sr), ConvertTypeIdentifier(astBuiltInExpr.Arg2, sr));

        private static TypeUsage ConvertTypeIdentifier(Expr typeIdentifierExpr, SemanticResolver sr)
        {
            MethodExpr expr = null;
            string[] names;
            DotExpr expr2 = typeIdentifierExpr as DotExpr;
            if ((expr2 != null) && expr2.IsDottedIdentifier)
            {
                names = expr2.Names;
            }
            else
            {
                expr = typeIdentifierExpr as MethodExpr;
                if (expr != null)
                {
                    names = expr.MethodPrefixExpr.Names;
                }
                else
                {
                    Identifier identifier = typeIdentifierExpr as Identifier;
                    if (identifier == null)
                    {
                        throw EntityUtil.EntitySqlError(typeIdentifierExpr.ErrCtx, Strings.InvalidTypeNameExpression);
                    }
                    names = new string[] { identifier.Name };
                }
            }
            return sr.ResolveNameAsType(names, typeIdentifierExpr);
        }

        private static Dictionary<Type, AstExprConverter> CreateAstExprConverters() => 
            new Dictionary<Type, AstExprConverter>(15) { 
                { 
                    typeof(Literal),
                    new AstExprConverter(SemanticAnalyzer.ConvertLiteral)
                },
                { 
                    typeof(Parameter),
                    new AstExprConverter(SemanticAnalyzer.ConvertParameter)
                },
                { 
                    typeof(Identifier),
                    new AstExprConverter(SemanticAnalyzer.ConvertIdentifier)
                },
                { 
                    typeof(DotExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertDotExpr)
                },
                { 
                    typeof(BuiltInExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertBuiltIn)
                },
                { 
                    typeof(QueryExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertQuery)
                },
                { 
                    typeof(RowConstructorExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertRowConstructor)
                },
                { 
                    typeof(MultisetConstructorExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertMultisetConstructor)
                },
                { 
                    typeof(CaseExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertCaseExpr)
                },
                { 
                    typeof(RelshipNavigationExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertRelshipNavigationExpr)
                },
                { 
                    typeof(RefExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertRefExpr)
                },
                { 
                    typeof(DerefExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertDeRefExpr)
                },
                { 
                    typeof(MethodExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertMethodExpr)
                },
                { 
                    typeof(CreateRefExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertCreateRefExpr)
                },
                { 
                    typeof(KeyExpr),
                    new AstExprConverter(SemanticAnalyzer.ConvertKeyExpr)
                }
            };

        private static Dictionary<BuiltInKind, BuiltInExprConverter> CreateBuiltInExprConverter() => 
            new Dictionary<BuiltInKind, BuiltInExprConverter>(4) { 
                { 
                    BuiltInKind.Plus,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        IList<EdmFunction> list;
                        Pair<DbExpression, DbExpression> pair = ConvertPlusOperands(bltInExpr, sr);
                        if (TypeSemantics.IsNumericType(pair.Left.ResultType))
                        {
                            return sr.CmdTree.CreatePlusExpression(pair.Left, pair.Right);
                        }
                        if (!sr.TypeResolver.TryGetFunctionFromMetadata("Concat", "Edm", true, out list))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ErrCtx, Strings.ConcatBuiltinNotSupported);
                        }
                        List<TypeUsage> argTypes = new List<TypeUsage>(2) {
                            pair.Left.ResultType,
                            pair.Right.ResultType
                        };
                        bool isAmbiguous = false;
                        EdmFunction function = TypeResolver.ResolveFunctionOverloads(list, argTypes, false, out isAmbiguous);
                        if ((function == null) || isAmbiguous)
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ErrCtx, Strings.ConcatBuiltinNotSupported);
                        }
                        return sr.CmdTree.CreateFunctionExpression(function, new DbExpression[] { 
                            pair.Left,
                            pair.Right
                        });
                    }
                },
                { 
                    BuiltInKind.Minus,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertArithmeticArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateMinusExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Multiply,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertArithmeticArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateMultiplyExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Divide,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertArithmeticArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateDivideExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Modulus,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertArithmeticArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateModuloExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.UnaryMinus,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        DbExpression argument = sr.CmdTree.CreateUnaryMinusExpression(ConvertArithmeticArgs(bltInExpr, sr).Left);
                        if (!TypeSemantics.IsUnsignedNumericType(argument.ResultType))
                        {
                            return argument;
                        }
                        TypeUsage promotableType = null;
                        if (!TypeHelpers.TryGetClosestPromotableType(argument.ResultType, out promotableType))
                        {
                            throw EntityUtil.EntitySqlError(Strings.InvalidUnsignedTypeForUnaryMinusOperation(argument.ResultType.EdmType.FullName));
                        }
                        return sr.CmdTree.CreateCastExpression(argument, promotableType);
                    }
                },
                { 
                    BuiltInKind.UnaryPlus,
                    (bltInExpr, sr) => ConvertArithmeticArgs(bltInExpr, sr).Left
                },
                { 
                    BuiltInKind.And,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertLogicalArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateAndExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Or,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertLogicalArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateOrExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Not,
                    ((BuiltInExprConverter) ((bltInExpr, sr) => sr.CmdTree.CreateNotExpression(ConvertLogicalArgs(bltInExpr, sr).Left)))
                },
                { 
                    BuiltInKind.Equal,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertEqualCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateEqualsExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.NotEqual,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertEqualCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateNotExpression(sr.CmdTree.CreateEqualsExpression(pair.Left, pair.Right));
                    }
                },
                { 
                    BuiltInKind.GreaterEqual,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertOrderCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateGreaterThanOrEqualsExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.GreaterThan,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertOrderCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateGreaterThanExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.LessEqual,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertOrderCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateLessThanOrEqualsExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.LessThan,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertOrderCompArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateLessThanExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Union,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertSetArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateDistinctExpression(sr.CmdTree.CreateUnionAllExpression(pair.Left, pair.Right));
                    }
                },
                { 
                    BuiltInKind.UnionAll,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertSetArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateUnionAllExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Intersect,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertSetArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateIntersectExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Overlaps,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertSetArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateNotExpression(sr.CmdTree.CreateIsEmptyExpression(sr.CmdTree.CreateIntersectExpression(pair.Left, pair.Right)));
                    }
                },
                { 
                    BuiltInKind.AnyElement,
                    ((BuiltInExprConverter) ((bltInExpr, sr) => sr.CmdTree.CreateElementExpression(ConvertSetArgs(bltInExpr, sr).Left)))
                },
                { 
                    BuiltInKind.Element,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        throw EntityUtil.NotSupported(Strings.ElementOperatorIsNotSupported);
                    }
                },
                { 
                    BuiltInKind.Except,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertSetArgs(bltInExpr, sr);
                        return sr.CmdTree.CreateExceptExpression(pair.Left, pair.Right);
                    }
                },
                { 
                    BuiltInKind.Exists,
                    ((BuiltInExprConverter) ((bltInExpr, sr) => sr.CmdTree.CreateNotExpression(sr.CmdTree.CreateIsEmptyExpression(ConvertSetArgs(bltInExpr, sr).Left))))
                },
                { 
                    BuiltInKind.Flatten,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        DbExpression input = Convert(bltInExpr.Arg1, sr);
                        if (!TypeSemantics.IsCollectionType(input.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.InvalidFlattenArgument);
                        }
                        if (!TypeSemantics.IsCollectionType(TypeHelpers.GetElementTypeUsage(input.ResultType)))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.InvalidFlattenArgument);
                        }
                        DbExpressionBinding binding = sr.CmdTree.CreateExpressionBinding(input, sr.GenerateInternalName("l_flatten"));
                        DbExpressionBinding apply = sr.CmdTree.CreateExpressionBinding(binding.Variable, sr.GenerateInternalName("r_flatten"));
                        DbExpressionBinding binding3 = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateCrossApplyExpression(binding, apply), sr.GenerateInternalName("flatten"));
                        return sr.CmdTree.CreateProjectExpression(binding3, sr.CmdTree.CreatePropertyExpression(apply.VariableName, binding3.Variable));
                    }
                },
                { 
                    BuiltInKind.In,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertInExprArgs(bltInExpr, sr);
                        if (pair.Right.ExpressionKind == DbExpressionKind.NewInstance)
                        {
                            return ConvertSimpleInExpression(sr, pair.Left, pair.Right);
                        }
                        DbExpressionBinding input = sr.CmdTree.CreateExpressionBinding(pair.Right, sr.GenerateInternalName("in-filter"));
                        DbExpression left = pair.Left;
                        DbExpression variable = input.Variable;
                        DbExpression right = sr.CmdTree.CreateNotExpression(sr.CmdTree.CreateIsEmptyExpression(sr.CmdTree.CreateFilterExpression(input, sr.CmdTree.CreateEqualsExpression(left, variable))));
                        List<DbExpression> whenExpressions = new List<DbExpression>(1) {
                            sr.CmdTree.CreateIsNullExpression(left)
                        };
                        List<DbExpression> thenExpressions = new List<DbExpression>(1) {
                            sr.CmdTree.CreateNullExpression(sr.TypeResolver.BooleanType)
                        };
                        DbExpression expression4 = sr.CmdTree.CreateCaseExpression(whenExpressions, thenExpressions, sr.CmdTree.CreateFalseExpression());
                        return sr.CmdTree.CreateOrExpression(expression4, right);
                    }
                },
                { 
                    BuiltInKind.NotIn,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = ConvertInExprArgs(bltInExpr, sr);
                        if (pair.Right.ExpressionKind == DbExpressionKind.NewInstance)
                        {
                            return sr.CmdTree.CreateNotExpression(ConvertSimpleInExpression(sr, pair.Left, pair.Right));
                        }
                        DbExpressionBinding input = sr.CmdTree.CreateExpressionBinding(pair.Right, sr.GenerateInternalName("in-filter"));
                        DbExpression left = pair.Left;
                        DbExpression variable = input.Variable;
                        DbExpression right = sr.CmdTree.CreateIsEmptyExpression(sr.CmdTree.CreateFilterExpression(input, sr.CmdTree.CreateEqualsExpression(left, variable)));
                        List<DbExpression> whenExpressions = new List<DbExpression>(1) {
                            sr.CmdTree.CreateIsNullExpression(left)
                        };
                        List<DbExpression> thenExpressions = new List<DbExpression>(1) {
                            sr.CmdTree.CreateNullExpression(sr.TypeResolver.BooleanType)
                        };
                        DbExpression expression4 = sr.CmdTree.CreateCaseExpression(whenExpressions, thenExpressions, sr.CmdTree.CreateTrueExpression());
                        return sr.CmdTree.CreateAndExpression(expression4, right);
                    }
                },
                { 
                    BuiltInKind.Distinct,
                    ((BuiltInExprConverter) ((bltInExpr, sr) => sr.CmdTree.CreateDistinctExpression(ConvertSetArgs(bltInExpr, sr).Left)))
                },
                { 
                    BuiltInKind.IsNull,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        DbExpression expression = Convert(bltInExpr.Arg1, sr);
                        if (!TypeHelpers.IsValidIsNullOpType(expression.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.IsNullInvalidType);
                        }
                        if (!SemanticResolver.IsNullExpression(expression))
                        {
                            return sr.CmdTree.CreateIsNullExpression(expression);
                        }
                        return sr.CmdTree.CreateTrueExpression();
                    }
                },
                { 
                    BuiltInKind.IsNotNull,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        DbExpression expression = Convert(bltInExpr.Arg1, sr);
                        if (!TypeHelpers.IsValidIsNullOpType(expression.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.IsNullInvalidType);
                        }
                        expression = SemanticResolver.IsNullExpression(expression) ? ((DbExpression) sr.CmdTree.CreateTrueExpression()) : ((DbExpression) sr.CmdTree.CreateIsNullExpression(expression));
                        return sr.CmdTree.CreateNotExpression(expression);
                    }
                },
                { 
                    BuiltInKind.IsOf,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, TypeUsage> pair = ConvertTypeExprArgs(bltInExpr, sr);
                        bool flag = (bool) ((Literal) bltInExpr.ArgList[2]).Value;
                        bool flag2 = (bool) ((Literal) bltInExpr.ArgList[3]).Value;
                        bool flag3 = sr.ParserOptions.ParserCompilationMode == ParserOptions.CompilationMode.RestrictedViewGenerationMode;
                        if (TypeSemantics.IsNullType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionCannotBeNull);
                        }
                        if (!flag3 && !TypeSemantics.IsEntityType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeEntityType(Strings.CtxIsOf, pair.Left.ResultType.EdmType.BuiltInTypeKind.ToString(), pair.Left.ResultType.EdmType.FullName));
                        }
                        if (flag3 && !TypeSemantics.IsNominalType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeNominalType(Strings.CtxIsOf, pair.Left.ResultType.EdmType.BuiltInTypeKind.ToString(), pair.Left.ResultType.EdmType.FullName));
                        }
                        if (!flag3 && !TypeSemantics.IsEntityType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeEntityType(Strings.CtxIsOf, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (flag3 && !TypeSemantics.IsNominalType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeNominalType(Strings.CtxIsOf, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (!TypeSemantics.IsPolymorphicType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.TypeMustBeInheritableType);
                        }
                        if (!TypeSemantics.IsPolymorphicType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeInheritableType);
                        }
                        if (!TypeResolver.IsSubOrSuperType(pair.Left.ResultType, pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ErrCtx, Strings.NotASuperOrSubType(pair.Left.ResultType.EdmType.FullName, pair.Right.EdmType.FullName));
                        }
                        pair.Right = TypeHelpers.GetReadOnlyType(pair.Right);
                        DbExpression argument = null;
                        if (flag)
                        {
                            argument = sr.CmdTree.CreateIsOfOnlyExpression(pair.Left, pair.Right);
                        }
                        else
                        {
                            argument = sr.CmdTree.CreateIsOfExpression(pair.Left, pair.Right);
                        }
                        if (flag2)
                        {
                            argument = sr.CmdTree.CreateNotExpression(argument);
                        }
                        return argument;
                    }
                },
                { 
                    BuiltInKind.Treat,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, TypeUsage> pair = ConvertTypeExprArgs(bltInExpr, sr);
                        bool flag = sr.ParserOptions.ParserCompilationMode == ParserOptions.CompilationMode.RestrictedViewGenerationMode;
                        if (!flag && !TypeSemantics.IsEntityType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeEntityType(Strings.CtxTreat, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (flag && !TypeSemantics.IsNominalType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeNominalType(Strings.CtxTreat, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (TypeSemantics.IsNullType(pair.Left.ResultType))
                        {
                            pair.Left = sr.CmdTree.CreateNullExpression(pair.Right);
                        }
                        else
                        {
                            if (!flag && !TypeSemantics.IsEntityType(pair.Left.ResultType))
                            {
                                throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeEntityType(Strings.CtxTreat, pair.Left.ResultType.EdmType.BuiltInTypeKind.ToString(), pair.Left.ResultType.EdmType.FullName));
                            }
                            if (flag && !TypeSemantics.IsNominalType(pair.Left.ResultType))
                            {
                                throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionTypeMustBeNominalType(Strings.CtxTreat, pair.Left.ResultType.EdmType.BuiltInTypeKind.ToString(), pair.Left.ResultType.EdmType.FullName));
                            }
                        }
                        if (!TypeSemantics.IsPolymorphicType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.TypeMustBeInheritableType);
                        }
                        if (!TypeSemantics.IsPolymorphicType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeInheritableType);
                        }
                        if (!TypeResolver.IsSubOrSuperType(pair.Left.ResultType, pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.NotASuperOrSubType(pair.Left.ResultType.EdmType.FullName, pair.Right.EdmType.FullName));
                        }
                        return sr.CmdTree.CreateTreatExpression(pair.Left, TypeHelpers.GetReadOnlyType(pair.Right));
                    }
                },
                { 
                    BuiltInKind.Cast,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, TypeUsage> pair = ConvertTypeExprArgs(bltInExpr, sr);
                        if (!TypeSemantics.IsPrimitiveType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.InvalidCastType);
                        }
                        if (SemanticResolver.IsNullExpression(pair.Left))
                        {
                            return sr.CmdTree.CreateCastExpression(sr.CmdTree.CreateNullExpression(pair.Right), pair.Right);
                        }
                        if (pair.Left.ResultType.BuiltInTypeKind != BuiltInTypeKind.EnumType)
                        {
                            if (!TypeSemantics.IsPrimitiveType(pair.Left.ResultType))
                            {
                                throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.InvalidCastExpressionType);
                            }
                            if (!TypeSemantics.IsCastAllowed(pair.Left.ResultType, pair.Right))
                            {
                                throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.InvalidCast(pair.Left.ResultType.EdmType, pair.Right.EdmType.FullName));
                            }
                        }
                        return sr.CmdTree.CreateCastExpression(pair.Left, TypeHelpers.GetReadOnlyType(pair.Right));
                    }
                },
                { 
                    BuiltInKind.OfType,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, TypeUsage> pair = ConvertTypeExprArgs(bltInExpr, sr);
                        bool flag = (bool) ((Literal) bltInExpr.ArgList[2]).Value;
                        bool flag2 = sr.ParserOptions.ParserCompilationMode == ParserOptions.CompilationMode.RestrictedViewGenerationMode;
                        if (!TypeSemantics.IsCollectionType(pair.Left.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.ExpressionMustBeCollection);
                        }
                        TypeUsage type = TypeHelpers.GetElementTypeUsage(pair.Left.ResultType);
                        if (!flag2 && !TypeSemantics.IsEntityType(type))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.OfTypeExpressionElementTypeMustBeEntityType(type.EdmType.BuiltInTypeKind.ToString(), type));
                        }
                        if (flag2 && !TypeSemantics.IsNominalType(type))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.OfTypeExpressionElementTypeMustBeNominalType(type.EdmType.BuiltInTypeKind.ToString(), type));
                        }
                        if (!flag2 && !TypeSemantics.IsEntityType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeEntityType(Strings.CtxOfType, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (flag2 && !TypeSemantics.IsNominalType(pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.TypeMustBeNominalType(Strings.CtxOfType, pair.Right.EdmType.BuiltInTypeKind.ToString(), pair.Right.EdmType.FullName));
                        }
                        if (flag && pair.Right.EdmType.Abstract)
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.OfTypeOnlyTypeArgumentCannotBeAbstract(pair.Right.EdmType.FullName));
                        }
                        if (!TypeResolver.IsSubOrSuperType(type, pair.Right))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.NotASuperOrSubType(type.EdmType.FullName, pair.Right.EdmType.FullName));
                        }
                        if (flag)
                        {
                            return sr.CmdTree.CreateOfTypeOnlyExpression(pair.Left, TypeHelpers.GetReadOnlyType(pair.Right));
                        }
                        return sr.CmdTree.CreateOfTypeExpression(pair.Left, TypeHelpers.GetReadOnlyType(pair.Right));
                    }
                },
                { 
                    BuiltInKind.Like,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        DbExpression input = Convert(bltInExpr.Arg1, sr);
                        if (TypeSemantics.IsNullType(input.ResultType))
                        {
                            input = sr.CmdTree.CreateNullExpression(sr.TypeResolver.StringType);
                        }
                        else if (!TypeResolver.IsStringType(input.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg1.ErrCtx, Strings.LikeArgMustBeStringType);
                        }
                        DbExpression pattern = Convert(bltInExpr.Arg2, sr);
                        if (pattern is UntypedNullExpression)
                        {
                            pattern = sr.CmdTree.CreateNullExpression(sr.TypeResolver.StringType);
                        }
                        else if (!TypeResolver.IsStringType(pattern.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.Arg2.ErrCtx, Strings.LikeArgMustBeStringType);
                        }
                        if (3 == bltInExpr.ArgCount)
                        {
                            DbExpression escape = Convert(bltInExpr.ArgList[2], sr);
                            if (escape is UntypedNullExpression)
                            {
                                escape = sr.CmdTree.CreateNullExpression(sr.TypeResolver.StringType);
                            }
                            else if (!TypeResolver.IsStringType(escape.ResultType))
                            {
                                throw EntityUtil.EntitySqlError(bltInExpr.ArgList[2].ErrCtx, Strings.LikeArgMustBeStringType);
                            }
                            return sr.CmdTree.CreateLikeExpression(input, pattern, escape);
                        }
                        return sr.CmdTree.CreateLikeExpression(input, pattern);
                    }
                },
                { 
                    BuiltInKind.Between,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        Pair<DbExpression, DbExpression> pair = sr.EnsureTypedNulls(Convert(bltInExpr.ArgList[1], sr), Convert(bltInExpr.ArgList[2], sr), bltInExpr.ArgList[0].ErrCtx, () => Strings.BetweenLimitsCannotBeUntypedNulls);
                        TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(pair.Left.ResultType, pair.Right.ResultType);
                        if (commonTypeUsage == null)
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ArgList[0].ErrCtx, Strings.BetweenLimitsTypesAreNotCompatible(pair.Left.ResultType.EdmType.FullName, pair.Right.ResultType.EdmType.FullName));
                        }
                        if (!TypeSemantics.IsOrderComparableTo(pair.Left.ResultType, pair.Right.ResultType))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ArgList[0].ErrCtx, Strings.BetweenLimitsTypesAreNotOrderComparable(pair.Left.ResultType.EdmType.FullName, pair.Right.ResultType.EdmType.FullName));
                        }
                        DbExpression left = Convert(bltInExpr.ArgList[0], sr);
                        if (TypeSemantics.IsNullType(left.ResultType))
                        {
                            left = sr.CmdTree.CreateNullExpression(commonTypeUsage);
                        }
                        if (!TypeSemantics.IsOrderComparableTo(left.ResultType, commonTypeUsage))
                        {
                            throw EntityUtil.EntitySqlError(bltInExpr.ArgList[0].ErrCtx, Strings.BetweenValueIsNotOrderComparable(left.ResultType.EdmType.FullName, commonTypeUsage.EdmType.FullName));
                        }
                        return sr.CmdTree.CreateAndExpression(sr.CmdTree.CreateGreaterThanOrEqualsExpression(left, pair.Left), sr.CmdTree.CreateLessThanOrEqualsExpression(left, pair.Right));
                    }
                },
                { 
                    BuiltInKind.NotBetween,
                    delegate (BuiltInExpr bltInExpr, SemanticResolver sr) {
                        bltInExpr.Kind = BuiltInKind.Between;
                        DbExpression expression = sr.CmdTree.CreateNotExpression(ConvertBuiltIn(bltInExpr, sr));
                        bltInExpr.Kind = BuiltInKind.NotBetween;
                        return expression;
                    }
                }
            };

        private CommandExpr Initialize(Expr astExpr, DbCommandTree tree)
        {
            CommandExpr astCommandExpr = astExpr as CommandExpr;
            if (astCommandExpr == null)
            {
                throw EntityUtil.Argument(Strings.UnknownAstCommandExpression);
            }
            if (tree == null)
            {
                this._sr.SetCommandTreeFactory(astCommandExpr);
            }
            else
            {
                this._sr.SetCommandTreeFactory(astCommandExpr, tree);
            }
            this._sr.DeclareCanonicalNamespace();
            this._sr.DeclareNamespaces(astCommandExpr.NamespaceDeclList);
            return astCommandExpr;
        }

        private static DbExpressionBinding ProcessAliasedFromClauseItem(AliasExpr aliasedExpr, SemanticResolver sr)
        {
            DbExpressionBinding sourceBinding = null;
            DbExpression expression = Convert(aliasedExpr.Expr, sr);
            SemanticResolver.EnsureIsNotUntypedNull(expression, aliasedExpr.Expr.ErrCtx);
            if (!TypeSemantics.IsCollectionType(expression.ResultType))
            {
                throw EntityUtil.EntitySqlError(aliasedExpr.Expr.ErrCtx, Strings.ExpressionMustBeCollection);
            }
            string key = sr.InferAliasName(aliasedExpr, expression);
            if (sr.IsInCurrentScope(key))
            {
                if (aliasedExpr.HasAlias)
                {
                    CqlErrorHelper.ReportAliasAlreadyUsedError(key, aliasedExpr.AliasIdentifier.ErrCtx, Strings.InFromClause);
                }
                else
                {
                    key = sr.GenerateInternalName("autoFrom");
                }
            }
            sourceBinding = sr.CmdTree.CreateExpressionBinding(expression, key);
            sr.AddSourceBinding(sourceBinding);
            return sourceBinding;
        }

        private static DbExpressionBinding ProcessApplyClauseItem(ApplyClauseItem applyClause, SemanticResolver sr)
        {
            DbExpressionBinding binding = null;
            sr.CurrentScopeRegionFlags.PathTagger.VisitLeftNode();
            DbExpressionBinding input = ProcessFromClauseItem(applyClause.LeftExpr, sr);
            sr.CurrentScopeRegionFlags.PathTagger.LeaveNode();
            sr.CurrentScopeRegionFlags.PathTagger.VisitRightNode();
            DbExpressionBinding apply = ProcessFromClauseItem(applyClause.RightExpr, sr);
            sr.CurrentScopeRegionFlags.PathTagger.LeaveNode();
            binding = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateApplyExpressionByKind(SemanticResolver.MapApplyKind(applyClause.ApplyKind), input, apply), sr.GenerateInternalName("apply"));
            sr.FixupNamedSourceVarBindings(binding.Variable);
            return binding;
        }

        private static PairOfLists<TypeUsage, DbExpression> ProcessExprList(ExprList<Expr> astExprList, SemanticResolver sr)
        {
            List<TypeUsage> leftValues = new List<TypeUsage>(astExprList.Count);
            List<DbExpression> rightValues = new List<DbExpression>(astExprList.Count);
            for (int i = 0; i < astExprList.Count; i++)
            {
                DbExpression item = Convert(astExprList[i], sr);
                leftValues.Add(item.ResultType);
                rightValues.Add(item);
            }
            return new PairOfLists<TypeUsage, DbExpression>(leftValues, rightValues);
        }

        private static DbExpressionBinding ProcessFromClause(FromClause fromClause, SemanticResolver sr)
        {
            DbExpressionBinding input = null;
            DbExpressionBinding apply = null;
            for (int i = 0; i < fromClause.FromClauseItems.Count; i++)
            {
                sr.SetCurrentScopeVarKind(fromClause.FromClauseItems[i].FromClauseItemKind);
                apply = ProcessFromClauseItem(fromClause.FromClauseItems[i], sr);
                sr.ResetCurrentScopeVarKind();
                if (input == null)
                {
                    input = apply;
                }
                else
                {
                    input = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateCrossApplyExpression(input, apply), sr.GenerateInternalName("lcapply"));
                    sr.FixupNamedSourceVarBindings(input.Variable);
                }
            }
            return input;
        }

        private static DbExpressionBinding ProcessFromClauseItem(FromClauseItem fromClauseItem, SemanticResolver sr)
        {
            AliasExpr fromExpr = fromClauseItem.FromExpr as AliasExpr;
            if (fromExpr != null)
            {
                return ProcessAliasedFromClauseItem(fromExpr, sr);
            }
            JoinClauseItem joinClause = fromClauseItem.FromExpr as JoinClauseItem;
            if (joinClause != null)
            {
                return ProcessJoinClauseItem(joinClause, sr);
            }
            return ProcessApplyClauseItem((ApplyClauseItem) fromClauseItem.FromExpr, sr);
        }

        private static DbExpressionBinding ProcessGroupByClause(DbExpressionBinding source, QueryExpr queryExpr, SemanticResolver sr)
        {
            SemanticResolver.ScopeViewKind scopeView = sr.GetScopeView();
            GroupByClause groupByClause = queryExpr.GroupByClause;
            if (queryExpr.GroupByClause == null)
            {
                if (!queryExpr.HasMethodCall)
                {
                    return source;
                }
                sr.CurrentScopeRegionFlags.IsImplicitGroup = true;
            }
            else
            {
                sr.CurrentScopeRegionFlags.IsImplicitGroup = false;
            }
            DbExpressionBinding binding = null;
            DbGroupExpressionBinding input = sr.CmdTree.CreateGroupExpressionBinding(source.Expression, sr.GenerateInternalName("geb"), sr.GenerateInternalName("group"));
            sr.FixupGroupSourceVarBindings(input.Variable, input.GroupVariable);
            int capacity = (groupByClause != null) ? groupByClause.GroupItems.Count : 0;
            List<KeyValuePair<string, DbExpression>> keys = new List<KeyValuePair<string, DbExpression>>(capacity);
            HashSet<string> set = new HashSet<string>(sr.ScopeStringComparer);
            List<DbExpression> list2 = new List<DbExpression>(8);
            if (!sr.CurrentScopeRegionFlags.IsImplicitGroup)
            {
                for (int m = 0; m < capacity; m++)
                {
                    AliasExpr aliasExpr = groupByClause.GroupItems[m];
                    sr.ResetScopeRegionCorrelationFlag();
                    DbExpression expression = Convert(aliasExpr.Expr, sr);
                    SemanticResolver.EnsureIsNotUntypedNull(expression, aliasExpr.Expr.ErrCtx);
                    if (!sr.CurrentScopeRegionFlags.WasResolutionCorrelated)
                    {
                        throw EntityUtil.EntitySqlError(aliasExpr.Expr.ErrCtx, Strings.KeyMustBeCorrelated("GROUP BY"));
                    }
                    sr.CurrentScopeRegionFlags.IsInsideGroupAggregate = true;
                    DbExpression expression2 = Convert(aliasExpr.Expr, sr);
                    list2.Add(expression2);
                    sr.CurrentScopeRegionFlags.IsInsideGroupAggregate = false;
                    if (!TypeHelpers.IsValidGroupKeyType(expression.ResultType))
                    {
                        throw EntityUtil.EntitySqlError(aliasExpr.Expr.ErrCtx, Strings.GroupingKeysMustBeEqualComparable);
                    }
                    string fullName = sr.InferAliasName(aliasExpr, expression);
                    if (set.Contains(fullName))
                    {
                        if (aliasExpr.HasAlias)
                        {
                            CqlErrorHelper.ReportAliasAlreadyUsedError(fullName, aliasExpr.AliasIdentifier.ErrCtx, Strings.InGroupClause);
                        }
                        else
                        {
                            fullName = sr.GenerateInternalName("autoGroup");
                        }
                    }
                    set.Add(fullName);
                    keys.Add(new KeyValuePair<string, DbExpression>(fullName, expression));
                    if (!aliasExpr.HasAlias)
                    {
                        DotExpr expr = aliasExpr.Expr as DotExpr;
                        if ((expr != null) && expr.IsDottedIdentifier)
                        {
                            fullName = expr.FullName;
                            if (set.Contains(fullName))
                            {
                                CqlErrorHelper.ReportAliasAlreadyUsedError(fullName, expr.ErrCtx, Strings.InGroupClause);
                            }
                            set.Add(fullName);
                            keys.Add(new KeyValuePair<string, DbExpression>(fullName, expression));
                            list2.Add(expression2);
                        }
                    }
                }
            }
            SavePoint sp = sr.CreateSavePoint();
            sr.EnterScope();
            for (int i = 0; i < keys.Count; i++)
            {
                KeyValuePair<string, DbExpression> pair3 = keys[i];
                KeyValuePair<string, DbExpression> pair4 = keys[i];
                sr.AddDummyGroupKeyToScope(pair3.Key, pair4.Value, list2[i]);
            }
            sr.CurrentScopeRegionFlags.IsInGroupScope = true;
            if ((queryExpr.HavingClause != null) && queryExpr.HavingClause.HasMethodCall)
            {
                sr.CurrentScopeRegionFlags.ResetGroupAggregateNestingCount();
                SemanticResolver.EnsureIsNotUntypedNull(Convert(queryExpr.HavingClause.HavingPredicate, sr), queryExpr.HavingClause.ErrCtx);
            }
            Dictionary<string, DbExpression> dictionary = null;
            if ((queryExpr.OrderByClause != null) || queryExpr.SelectClause.HasMethodCall)
            {
                dictionary = new Dictionary<string, DbExpression>(queryExpr.SelectClause.Items.Count, sr.ScopeStringComparer);
                for (int n = 0; n < queryExpr.SelectClause.Items.Count; n++)
                {
                    AliasExpr expr3 = queryExpr.SelectClause.Items[n];
                    sr.CurrentScopeRegionFlags.ResetGroupAggregateNestingCount();
                    DbExpression expression4 = Convert(expr3.Expr, sr);
                    SemanticResolver.EnsureIsNotUntypedNull(expression4, expr3.Expr.ErrCtx);
                    expression4 = sr.CmdTree.CreateNullExpression(expression4.ResultType);
                    string key = sr.InferAliasName(expr3, expression4);
                    if (dictionary.ContainsKey(key))
                    {
                        if (expr3.HasAlias)
                        {
                            CqlErrorHelper.ReportAliasAlreadyUsedError(key, expr3.AliasIdentifier.ErrCtx, Strings.InSelectProjectionList);
                        }
                        else
                        {
                            key = sr.GenerateInternalName("autoProject");
                        }
                    }
                    dictionary.Add(key, expression4);
                }
            }
            if ((queryExpr.OrderByClause != null) && queryExpr.OrderByClause.HasMethodCall)
            {
                sr.EnterScope();
                foreach (KeyValuePair<string, DbExpression> pair in dictionary)
                {
                    sr.AddToScope(pair.Key, new ProjectionScopeEntry(pair.Key, pair.Value));
                }
                for (int num5 = 0; num5 < queryExpr.OrderByClause.OrderByClauseItem.Count; num5++)
                {
                    OrderByClauseItem item = queryExpr.OrderByClause.OrderByClauseItem[num5];
                    sr.CurrentScopeRegionFlags.ResetGroupAggregateNestingCount();
                    sr.ResetScopeRegionCorrelationFlag();
                    SemanticResolver.EnsureIsNotUntypedNull(Convert(item.OrderExpr, sr), item.OrderExpr.ErrCtx);
                    if (!sr.CurrentScopeRegionFlags.WasResolutionCorrelated)
                    {
                        throw EntityUtil.EntitySqlError(item.ErrCtx, Strings.KeyMustBeCorrelated("ORDER BY"));
                    }
                }
                sr.LeaveScope();
            }
            if (sr.CurrentScopeRegionFlags.IsImplicitGroup)
            {
                if (sr.CurrentScopeRegionFlags.GroupAggregatesInfo.Count == 0)
                {
                    sr.RollbackToSavepoint(sp);
                    sr.UndoFixupGroupSourceVarBindings(source.Variable);
                    sr.CurrentScopeRegionFlags.IsInGroupScope = false;
                    sr.CurrentScopeRegionFlags.IsImplicitGroup = false;
                    sr.SetScopeView(scopeView);
                    return source;
                }
                sr.CurrentScopeRegionFlags.IsImplicitGroup = false;
            }
            List<KeyValuePair<string, DbAggregate>> aggregates = new List<KeyValuePair<string, DbAggregate>>(sr.CurrentScopeRegionFlags.GroupAggregatesInfo.Count);
            foreach (KeyValuePair<MethodExpr, GroupAggregateInfo> pair2 in sr.CurrentScopeRegionFlags.GroupAggregatesInfo)
            {
                aggregates.Add(new KeyValuePair<string, DbAggregate>(pair2.Value.AggregateName, pair2.Value.AggregateExpression));
                pair2.Key.ResetDummyExpression();
            }
            binding = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateGroupByExpression(input, keys, aggregates), sr.GenerateInternalName("group"));
            for (int j = 0; j < keys.Count; j++)
            {
                KeyValuePair<string, DbExpression> pair5 = keys[j];
                sr.ReplaceGroupVarInScope(pair5.Key, binding.Variable);
            }
            for (int k = 0; k < aggregates.Count; k++)
            {
                KeyValuePair<string, DbAggregate> pair6 = aggregates[k];
                sr.CurrentScopeRegionFlags.AddGroupAggregateToScopeFlags(pair6.Key);
                KeyValuePair<string, DbAggregate> pair7 = aggregates[k];
                sr.AddGroupAggregateToScope(pair7.Key, binding.Variable);
            }
            sr.SetScopeView(SemanticResolver.ScopeViewKind.GroupScope);
            sr.FixupNamedSourceVarBindings(binding.Variable);
            sr.MarkGroupInputVars();
            return binding;
        }

        private static DbExpressionBinding ProcessHavingClause(DbExpressionBinding source, HavingClause havingClause, SemanticResolver sr)
        {
            if (havingClause == null)
            {
                return source;
            }
            DbExpressionBinding binding = null;
            DbExpression expression = Convert(havingClause.HavingPredicate, sr);
            SemanticResolver.EnsureIsNotUntypedNull(expression, havingClause.ErrCtx);
            if (!TypeResolver.IsBooleanType(expression.ResultType))
            {
                throw EntityUtil.EntitySqlError(havingClause.ErrCtx, Strings.ExpressionTypeMustBeBoolean);
            }
            binding = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateFilterExpression(source, expression), sr.GenerateInternalName("having"));
            sr.FixupSourceVarBindings(binding.Variable);
            return binding;
        }

        private static DbExpressionBinding ProcessJoinClauseItem(JoinClauseItem joinClause, SemanticResolver sr)
        {
            DbExpressionBinding binding = null;
            if (joinClause.OnExpr == null)
            {
                if (JoinKind.Inner == joinClause.JoinKind)
                {
                    throw EntityUtil.EntitySqlError(joinClause.ErrCtx, Strings.InnerJoinMustHaveOnPredicate);
                }
            }
            else if (joinClause.JoinKind == JoinKind.Cross)
            {
                throw EntityUtil.EntitySqlError(joinClause.OnExpr.ErrCtx, Strings.InvalidPredicateForCrossJoin);
            }
            sr.CurrentScopeRegionFlags.PathTagger.VisitLeftNode();
            DbExpressionBinding binding2 = ProcessFromClauseItem(joinClause.LeftExpr, sr);
            sr.CurrentScopeRegionFlags.PathTagger.LeaveNode();
            sr.CurrentScopeRegionFlags.IsInsideJoinOnPredicate = false;
            sr.CurrentScopeRegionFlags.PathTagger.VisitRightNode();
            DbExpressionBinding binding3 = ProcessFromClauseItem(joinClause.RightExpr, sr);
            sr.CurrentScopeRegionFlags.PathTagger.LeaveNode();
            if (joinClause.JoinKind == JoinKind.RightOuter)
            {
                joinClause.JoinKind = JoinKind.LeftOuter;
                DbExpressionBinding binding4 = binding2;
                binding2 = binding3;
                binding3 = binding4;
            }
            DbExpressionKind joinKind = SemanticResolver.MapJoinKind(joinClause.JoinKind);
            sr.CurrentScopeRegionFlags.IsInsideJoinOnPredicate = true;
            DbExpression expression = null;
            if (joinClause.OnExpr == null)
            {
                if (DbExpressionKind.CrossJoin != joinKind)
                {
                    expression = sr.CmdTree.CreateTrueExpression();
                }
            }
            else
            {
                expression = Convert(joinClause.OnExpr, sr);
                SemanticResolver.EnsureIsNotUntypedNull(expression, joinClause.OnExpr.ErrCtx);
            }
            sr.CurrentScopeRegionFlags.IsInsideJoinOnPredicate = false;
            binding = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateJoinExpressionByKind(joinKind, expression, binding2, binding3), sr.GenerateInternalName("join"));
            sr.FixupNamedSourceVarBindings(binding.Variable);
            return binding;
        }

        private static DbExpressionBinding ProcessOrderByClause(DbExpressionBinding source, QueryExpr queryExpr, SemanticResolver sr)
        {
            if (queryExpr.OrderByClause == null)
            {
                return source;
            }
            DbExpressionBinding binding = null;
            OrderByClause orderByClause = queryExpr.OrderByClause;
            SelectClause selectClause = queryExpr.SelectClause;
            SavePoint sp = sr.CreateSavePoint();
            Dictionary<string, DbExpression> collection = new Dictionary<string, DbExpression>(selectClause.Items.Count, sr.ScopeStringComparer);
            for (int i = 0; i < selectClause.Items.Count; i++)
            {
                AliasExpr aliasExpr = selectClause.Items[i];
                DbExpression expression = Convert(aliasExpr.Expr, sr);
                SemanticResolver.EnsureIsNotUntypedNull(expression, aliasExpr.Expr.ErrCtx);
                string key = sr.InferAliasName(aliasExpr, expression);
                if (collection.ContainsKey(key))
                {
                    if (aliasExpr.HasAlias)
                    {
                        CqlErrorHelper.ReportAliasAlreadyUsedError(key, aliasExpr.AliasIdentifier.ErrCtx, Strings.InSelectProjectionList);
                    }
                    else
                    {
                        key = sr.GenerateInternalName("autoProject");
                    }
                }
                collection.Add(key, expression);
            }
            DbExpression expression2 = null;
            if (orderByClause.HasSkipSubClause)
            {
                expression2 = Convert(orderByClause.SkipSubClause, sr);
                SemanticResolver.EnsureIsNotUntypedNull(expression2, orderByClause.SkipSubClause.ErrCtx);
                DbConstantExpression expression3 = expression2 as DbConstantExpression;
                if (!TypeSemantics.IsPromotableTo(expression2.ResultType, sr.TypeResolver.Int64Type))
                {
                    throw EntityUtil.EntitySqlError(orderByClause.SkipSubClause.ErrCtx, Strings.PlaceholderExpressionMustBeCompatibleWithEdm64("SKIP", expression2.ResultType.EdmType.FullName));
                }
                if ((expression3 != null) && (System.Convert.ToInt64(expression3.Value, CultureInfo.InvariantCulture) < 0L))
                {
                    throw EntityUtil.EntitySqlError(orderByClause.SkipSubClause.ErrCtx, Strings.PlaceholderExpressionMustBeGreaterThanOrEqualToZero("SKIP"));
                }
            }
            sr.EnterScope();
            foreach (KeyValuePair<string, DbExpression> pair in collection)
            {
                if (!sr.CurrentScopeRegionFlags.ContainsGroupAggregate(pair.Key))
                {
                    sr.AddToScope(pair.Key, new ProjectionScopeEntry(pair.Key, pair.Value));
                }
            }
            SemanticResolver.ScopeViewKind scopeView = sr.GetScopeView();
            if (selectClause.DistinctKind == DistinctKind.Distinct)
            {
                DbExpression expression4;
                sr.SetScopeView(SemanticResolver.ScopeViewKind.CurrentScope);
                List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>(collection);
                if (selectClause.SelectKind == SelectKind.SelectRow)
                {
                    expression4 = sr.CmdTree.CreateNewRowExpression(recordColumns);
                }
                else
                {
                    KeyValuePair<string, DbExpression> pair2 = recordColumns[0];
                    expression4 = pair2.Value;
                }
                expression4 = sr.CmdTree.CreateProjectExpression(source, expression4);
                SemanticResolver.ValidateDistinctProjection(selectClause, expression4.ResultType);
                source = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateDistinctExpression(expression4), sr.GenerateInternalName("distinct"));
                for (int k = 0; k < recordColumns.Count; k++)
                {
                    KeyValuePair<string, DbExpression> pair3 = recordColumns[k];
                    if (!sr.CurrentScopeRegionFlags.ContainsGroupAggregate(pair3.Key))
                    {
                        KeyValuePair<string, DbExpression> pair4 = recordColumns[k];
                        sr.RemoveFromScope(pair4.Key);
                        KeyValuePair<string, DbExpression> pair5 = recordColumns[k];
                        SourceScopeEntry scopeEntry = new SourceScopeEntry(ScopeEntryKind.SourceVar, pair5.Key, source.Variable);
                        if (selectClause.SelectKind == SelectKind.SelectRow)
                        {
                            KeyValuePair<string, DbExpression> pair6 = recordColumns[k];
                            scopeEntry.AddBindingPrefix(pair6.Key);
                        }
                        KeyValuePair<string, DbExpression> pair7 = recordColumns[k];
                        sr.AddToScope(pair7.Key, scopeEntry);
                    }
                }
            }
            else if (sr.CurrentScopeRegionFlags.IsInGroupScope)
            {
                sr.SetScopeView(SemanticResolver.ScopeViewKind.CurrentAndPreviousScope);
            }
            List<DbSortClause> sortOrder = new List<DbSortClause>(orderByClause.OrderByClauseItem.Expressions.Count);
            for (int j = 0; j < orderByClause.OrderByClauseItem.Expressions.Count; j++)
            {
                OrderByClauseItem item = orderByClause.OrderByClauseItem.Expressions[j];
                sr.CurrentScopeRegionFlags.ResetGroupAggregateNestingCount();
                sr.ResetScopeRegionCorrelationFlag();
                DbExpression expression5 = Convert(item.OrderExpr, sr);
                SemanticResolver.EnsureIsNotUntypedNull(expression5, item.OrderExpr.ErrCtx);
                if (!sr.CurrentScopeRegionFlags.WasResolutionCorrelated)
                {
                    throw EntityUtil.EntitySqlError(item.ErrCtx, Strings.KeyMustBeCorrelated("ORDER BY"));
                }
                if (!TypeHelpers.IsValidSortOpKeyType(expression5.ResultType))
                {
                    throw EntityUtil.EntitySqlError(item.OrderExpr.ErrCtx, Strings.OrderByKeyIsNotOrderComparable);
                }
                bool ascending = (item.OrderKind == OrderKind.None) || (item.OrderKind == OrderKind.Asc);
                string name = null;
                if (item.IsCollated)
                {
                    if (!TypeResolver.IsKeyValidForCollation(expression5.ResultType))
                    {
                        throw EntityUtil.EntitySqlError(item.OrderExpr.ErrCtx, Strings.InvalidKeyTypeForCollation(expression5.ResultType.EdmType.FullName));
                    }
                    name = item.CollateIdentifier.Name;
                }
                else if ((sr.ParserOptions.DefaultOrderByCollation.Length > 0) && TypeResolver.IsKeyValidForCollation(expression5.ResultType))
                {
                    name = sr.ParserOptions.DefaultOrderByCollation;
                }
                if (string.IsNullOrEmpty(name))
                {
                    sortOrder.Add(sr.CmdTree.CreateSortClause(expression5, ascending));
                }
                else
                {
                    sortOrder.Add(sr.CmdTree.CreateSortClause(expression5, ascending, name));
                }
            }
            DbExpression input = null;
            if (orderByClause.HasSkipSubClause)
            {
                input = sr.CmdTree.CreateSkipExpression(source, sortOrder, expression2);
            }
            else
            {
                input = sr.CmdTree.CreateSortExpression(source, sortOrder);
            }
            binding = sr.CmdTree.CreateExpressionBinding(input, sr.GenerateInternalName("sort"));
            sr.FixupSourceVarBindings(binding.Variable);
            sr.RollbackToSavepoint(sp);
            sr.SetScopeView(scopeView);
            return binding;
        }

        private static DbExpression ProcessSelectClause(DbExpressionBinding source, QueryExpr queryExpr, SemanticResolver sr)
        {
            DbExpression argument = null;
            SelectClause selectClause = queryExpr.SelectClause;
            HashSet<string> set = new HashSet<string>(sr.ScopeStringComparer);
            List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>(selectClause.Items.Count);
            if ((queryExpr.OrderByClause != null) && (selectClause.DistinctKind == DistinctKind.Distinct))
            {
                argument = source.Expression;
            }
            else
            {
                for (int i = 0; i < selectClause.Items.Count; i++)
                {
                    AliasExpr aliasExpr = selectClause.Items[i];
                    DbExpression expression = Convert(aliasExpr.Expr, sr);
                    SemanticResolver.EnsureIsNotUntypedNull(expression, aliasExpr.Expr.ErrCtx);
                    string item = sr.InferAliasName(aliasExpr, expression);
                    if (set.Contains(item))
                    {
                        if (aliasExpr.HasAlias)
                        {
                            CqlErrorHelper.ReportAliasAlreadyUsedError(item, aliasExpr.AliasIdentifier.ErrCtx, Strings.InSelectProjectionList);
                        }
                        else
                        {
                            item = sr.GenerateInternalName("autoProject");
                        }
                    }
                    set.Add(item);
                    recordColumns.Add(new KeyValuePair<string, DbExpression>(item, expression));
                }
                if (selectClause.SelectKind == SelectKind.SelectValue)
                {
                    if (recordColumns.Count > 1)
                    {
                        throw EntityUtil.EntitySqlError(selectClause.ErrCtx, Strings.InvalidSelectItem);
                    }
                    KeyValuePair<string, DbExpression> pair = recordColumns[0];
                    argument = sr.CmdTree.CreateProjectExpression(source, pair.Value);
                }
                else
                {
                    argument = sr.CmdTree.CreateProjectExpression(source, sr.CmdTree.CreateNewRowExpression(recordColumns));
                }
                if (selectClause.DistinctKind == DistinctKind.Distinct)
                {
                    SemanticResolver.ValidateDistinctProjection(selectClause, argument.ResultType);
                    argument = sr.CmdTree.CreateDistinctExpression(argument);
                }
            }
            if (selectClause.HasTopClause || ((queryExpr.OrderByClause != null) && queryExpr.OrderByClause.HasLimitSubClause))
            {
                ErrorContext errCtx = selectClause.HasTopClause ? selectClause.TopExpr.ErrCtx : queryExpr.OrderByClause.LimitSubClause.ErrCtx;
                DbExpression expression3 = Convert(selectClause.HasTopClause ? selectClause.TopExpr : queryExpr.OrderByClause.LimitSubClause, sr);
                SemanticResolver.EnsureIsNotUntypedNull(expression3, errCtx);
                sr.EnsureValidLimitExpression(errCtx, expression3, selectClause.HasTopClause ? "TOP" : "LIMIT");
                argument = sr.CmdTree.CreateLimitExpression(argument, expression3);
            }
            return argument;
        }

        private static DbExpressionBinding ProcessWhereClause(DbExpressionBinding source, Expr whereClause, SemanticResolver sr)
        {
            if (whereClause == null)
            {
                return source;
            }
            DbExpressionBinding binding = null;
            DbExpression expression = Convert(whereClause, sr);
            SemanticResolver.EnsureIsNotUntypedNull(expression, whereClause.ErrCtx);
            if (!TypeResolver.IsBooleanType(expression.ResultType))
            {
                throw EntityUtil.EntitySqlError(whereClause.ErrCtx, Strings.ExpressionTypeMustBeBoolean);
            }
            binding = sr.CmdTree.CreateExpressionBinding(sr.CmdTree.CreateFilterExpression(source, expression), sr.GenerateInternalName("where"));
            sr.FixupSourceVarBindings(binding.Variable);
            return binding;
        }

        private static bool TryConvertAsGroupAggregateFunction(MethodExpr methodExpr, IList<EdmFunction> functionTypeList, SemanticResolver sr, out DbExpression converted)
        {
            DbFunctionAggregate aggregate;
            converted = null;
            SemanticResolver.ScopeViewKind scopeView = sr.GetScopeView();
            sr.SetScopeView(SemanticResolver.ScopeViewKind.All);
            sr.CurrentScopeRegionFlags.IsInsideGroupAggregate = true;
            sr.CurrentScopeRegionFlags.WasNestedGroupAggregateReferredByInnerExpressions = false;
            sr.PushAggregateAstNode(methodExpr);
            sr.CurrentScopeRegionFlags.DecrementGroupAggregateNestingCount();
            List<DbExpression> list = ConvertFunctionArguments(methodExpr.Args, sr);
            sr.CurrentScopeRegionFlags.IsInsideGroupAggregate = false;
            List<TypeUsage> argTypes = new List<TypeUsage>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                argTypes.Add(list[i].ResultType);
            }
            bool isAmbiguous = false;
            EdmFunction function = TypeResolver.ResolveFunctionOverloads(functionTypeList, argTypes, true, out isAmbiguous);
            if (isAmbiguous)
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.AmbiguousFunctionArguments);
            }
            if (function == null)
            {
                CqlErrorHelper.ReportFunctionOverloadError(methodExpr, functionTypeList[0], argTypes);
            }
            if (sr.CurrentScopeRegionFlags.WasNestedGroupAggregateReferredByInnerExpressions)
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.NestedAggregatesCannotBeUsedInAggregateFunctions);
            }
            if (sr.CurrentScopeRegionFlags.GroupAggregateNestingCount < -1)
            {
                throw EntityUtil.EntitySqlError(methodExpr.MethodIdentifier.ErrCtx, Strings.InvalidNestedGroupAggregateCall);
            }
            TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(function.Parameters[0].TypeUsage);
            if (TypeSemantics.IsNullType(list[0].ResultType))
            {
                list[0] = sr.CmdTree.CreateNullExpression(elementTypeUsage);
            }
            if (methodExpr.DistinctKind == DistinctKind.Distinct)
            {
                aggregate = sr.CmdTree.CreateDistinctFunctionAggregate(function, list[0]);
            }
            else
            {
                aggregate = sr.CmdTree.CreateFunctionAggregate(function, list[0]);
            }
            string aggregateName = sr.GenerateInternalName("groupAgg" + function.Name);
            AggregateAstNodeInfo info = sr.PopAggregateAstNode();
            info.AssertMethodExprEquivalent(methodExpr);
            sr.AddGroupAggregateInfoToScopeRegion(methodExpr, aggregateName, aggregate, info.ScopeIndex);
            converted = sr.CmdTree.CreateNullExpression(function.ReturnParameter.TypeUsage);
            methodExpr.SetAggregateInfo(aggregateName, converted);
            sr.SetScopeView(scopeView);
            sr.CurrentScopeRegionFlags.IncrementGroupAggregateNestingCount();
            return true;
        }

        private static bool TryConvertAsOrdinaryFunctionInGroup(MethodExpr methodExpr, IList<EdmFunction> functionTypes, SemanticResolver sr, out DbExpression converted)
        {
            converted = null;
            SemanticResolver.ScopeViewKind scopeView = sr.GetScopeView();
            List<DbExpression> args = ConvertFunctionArguments(methodExpr.Args, sr);
            List<TypeUsage> argTypes = new List<TypeUsage>(args.Count);
            for (int i = 0; i < args.Count; i++)
            {
                argTypes.Add(args[i].ResultType);
            }
            bool isAmbiguous = false;
            EdmFunction function = TypeResolver.ResolveFunctionOverloads(functionTypes, argTypes, false, out isAmbiguous);
            if (isAmbiguous)
            {
                throw EntityUtil.EntitySqlError(methodExpr.ErrCtx, Strings.AmbiguousFunctionArguments);
            }
            if (function != null)
            {
                if (!sr.CurrentScopeRegionFlags.IsImplicitGroup)
                {
                    sr.SetScopeView(SemanticResolver.ScopeViewKind.CurrentScope);
                }
                args = ConvertFunctionArguments(methodExpr.Args, sr);
                sr.SetScopeView(scopeView);
                converted = sr.CmdTree.CreateFunctionExpression(function, args);
            }
            return (null != function);
        }

        private static bool TryConvertAsResolvedGroupAggregate(MethodExpr methodExpr, SemanticResolver sr, out DbExpression converted)
        {
            ScopeEntry entry;
            int num;
            converted = null;
            if (!methodExpr.WasResolved)
            {
                return false;
            }
            sr.CurrentScopeRegionFlags.DecrementGroupAggregateNestingCount();
            SemanticResolver.ScopeViewKind scopeView = sr.GetScopeView();
            sr.SetScopeView(SemanticResolver.ScopeViewKind.All);
            if (!sr.TryScopeLookup(methodExpr.InternalAggregateName, out entry, out num))
            {
                converted = methodExpr.DummyExpression;
                return true;
            }
            sr.SetScopeRegionCorrelationFlag(num);
            sr.SetScopeView(scopeView);
            converted = entry.Expression;
            return true;
        }

        private static void ValidateAndCompensateQuery(QueryExpr queryExpr)
        {
            if ((queryExpr.HavingClause != null) && (queryExpr.GroupByClause == null))
            {
                throw EntityUtil.EntitySqlError(queryExpr.ErrCtx, Strings.HavingRequiresGroupClause);
            }
            if (queryExpr.SelectClause.HasTopClause)
            {
                if ((queryExpr.OrderByClause != null) && queryExpr.OrderByClause.HasLimitSubClause)
                {
                    throw EntityUtil.EntitySqlError(queryExpr.SelectClause.TopExpr.ErrCtx, Strings.TopAndLimitCannotCoexist);
                }
                if ((queryExpr.OrderByClause != null) && queryExpr.OrderByClause.HasSkipSubClause)
                {
                    throw EntityUtil.EntitySqlError(queryExpr.SelectClause.TopExpr.ErrCtx, Strings.TopAndSkipCannotCoexist);
                }
            }
        }

        private static void ValidateRelationshipTraversal(RelshipNavigationExpr relshipExpr, bool isTargetEnd, SemanticResolver sr, out DbExpression refExpr, out RelationshipType relationshipType, out RelationshipEndMember refEnd, out RelationshipEndMember otherEnd)
        {
            relationshipType = null;
            refEnd = null;
            otherEnd = null;
            refExpr = null;
            relationshipType = sr.ResolveNameAsType(relshipExpr.RelationTypeNames, relshipExpr.RelationTypeNameIdentifier).EdmType as RelationshipType;
            if (relationshipType == null)
            {
                throw EntityUtil.EntitySqlError(relshipExpr.RelationTypeNameIdentifier.ErrCtx, Strings.InvalidRelationshipTypeName(relshipExpr.RelationTypeFullName));
            }
            refExpr = Convert(relshipExpr.RelationshipSource, sr);
            if (!isTargetEnd && TypeSemantics.IsEntityType(refExpr.ResultType))
            {
                refExpr = sr.CmdTree.CreateEntityRefExpression(refExpr);
            }
            if (!TypeSemantics.IsReferenceType(refExpr.ResultType))
            {
                throw EntityUtil.EntitySqlError(relshipExpr.RelationshipSource.ErrCtx, Strings.InvalidRelationshipSourceType);
            }
            if (!TypeSemantics.IsTypeValidForRelationship(TypeHelpers.GetElementTypeUsage(refExpr.ResultType), relationshipType))
            {
                throw EntityUtil.EntitySqlError(relshipExpr.RelationTypeNameIdentifier.ErrCtx, Strings.RelationshipTypeIsNotCompatibleWithEntity(TypeHelpers.GetFullName(TypeHelpers.GetElementTypeUsage(refExpr.ResultType)), TypeHelpers.GetFullName(relationshipType)));
            }
            TypeUsage type = null;
            int num = 0;
            TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(refExpr.ResultType);
            for (int i = 0; i < relationshipType.Members.Count; i++)
            {
                if (relationshipType.Members[i].Name.Equals(relshipExpr.ToEndIdentifierName, StringComparison.OrdinalIgnoreCase))
                {
                    otherEnd = (RelationshipEndMember) relationshipType.Members[i];
                }
                else if (((relshipExpr.FromEndIdentifier != null) && relationshipType.Members[i].Name.Equals(relshipExpr.FromEndIdentifierName, StringComparison.OrdinalIgnoreCase)) || ((relshipExpr.FromEndIdentifier == null) && TypeSemantics.IsEquivalentOrPromotableTo(elementTypeUsage, TypeHelpers.GetElementTypeUsage(relationshipType.Members[i].TypeUsage))))
                {
                    num++;
                    if (num > 1)
                    {
                        ErrorContext errCtx = (relshipExpr.FromEndIdentifier == null) ? relshipExpr.ErrCtx : relshipExpr.FromEndIdentifier.ErrCtx;
                        throw EntityUtil.EntitySqlError(errCtx, Strings.RelationshipFromEndIsAmbiguos);
                    }
                    refEnd = (RelationshipEndMember) relationshipType.Members[i];
                    type = relationshipType.Members[i].TypeUsage;
                }
            }
            if (otherEnd == null)
            {
                if (relshipExpr.ToEndIdentifier != null)
                {
                    throw EntityUtil.EntitySqlError(relshipExpr.ToEndIdentifier.ErrCtx, Strings.InvalidRelationshipMember(relshipExpr.ToEndIdentifierName, relshipExpr.RelationTypeFullName));
                }
                if (2 != relationshipType.Members.Count)
                {
                    throw EntityUtil.EntitySqlError(relshipExpr.ErrCtx, Strings.InvalidImplicitRelationshipToEnd(relshipExpr.RelationTypeFullName));
                }
                otherEnd = refEnd.Name.Equals(relationshipType.Members[0].Name, StringComparison.OrdinalIgnoreCase) ? ((RelationshipEndMember) relationshipType.Members[1]) : ((RelationshipEndMember) relationshipType.Members[0]);
            }
            if ((refEnd == null) || (type == null))
            {
                ErrorContext context2 = (relshipExpr.FromEndIdentifier == null) ? relshipExpr.ErrCtx : relshipExpr.FromEndIdentifier.ErrCtx;
                if (relshipExpr.FromEndIdentifier == null)
                {
                    throw EntityUtil.EntitySqlError(context2, Strings.InvalidImplicitRelationshipFromEnd(relshipExpr.RelationTypeFullName));
                }
                throw EntityUtil.EntitySqlError(context2, Strings.InvalidRelationshipMember(relshipExpr.FromEndIdentifierName, relshipExpr.RelationTypeFullName));
            }
            if (!TypeSemantics.IsValidPolymorphicCast(TypeHelpers.GetElementTypeUsage(refExpr.ResultType), TypeHelpers.GetElementTypeUsage(refEnd.TypeUsage)))
            {
                ErrorContext context3 = (relshipExpr.FromEndIdentifier == null) ? relshipExpr.ErrCtx : relshipExpr.FromEndIdentifier.ErrCtx;
                throw EntityUtil.EntitySqlError(context3, Strings.SourceTypeMustBePromotoableToFromEndRelationType(TypeHelpers.GetElementTypeUsage(refExpr.ResultType).EdmType.FullName, TypeHelpers.GetElementTypeUsage(type).EdmType.FullName));
            }
        }

        private delegate DbExpression AstExprConverter(Expr astExpr, SemanticResolver sr);

        private delegate DbExpression BuiltInExprConverter(BuiltInExpr astBltInExpr, SemanticResolver sr);

        private delegate DbCommandTree CommandConverter(Expr astExpr, SemanticResolver sr);
    }
}

