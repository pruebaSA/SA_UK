namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class ResourceBinder : DataServiceExpressionVisitor
    {
        private static void AddConjuncts(Expression e, List<Expression> conjuncts)
        {
            if (PatternRules.MatchAnd(e))
            {
                BinaryExpression expression = (BinaryExpression) e;
                AddConjuncts(expression.Left, conjuncts);
                AddConjuncts(expression.Right, conjuncts);
            }
            else
            {
                conjuncts.Add(e);
            }
        }

        private static void AddSequenceQueryOption(ResourceExpression target, QueryOptionExpression qoe)
        {
            ValidationRules.RequireNonSingleton(target);
            ResourceSetExpression expression = (ResourceSetExpression) target;
            switch (qoe.NodeType)
            {
                case ((ExpressionType) 0x2714):
                    if (expression.Take != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("skip", "top"));
                    }
                    break;

                case ((ExpressionType) 0x2715):
                    if (expression.Skip != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("orderby", "skip"));
                    }
                    if (expression.Take != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("orderby", "top"));
                    }
                    if (expression.Projection != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("orderby", "select"));
                    }
                    break;

                case ((ExpressionType) 0x2716):
                    if (expression.Skip != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("filter", "skip"));
                    }
                    if (expression.Take != null)
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("filter", "top"));
                    }
                    if (expression.Projection == null)
                    {
                        break;
                    }
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionOutOfOrder("filter", "select"));
            }
            expression.AddSequenceQueryOption(qoe);
        }

        private static Expression AnalyzeAddCountOption(MethodCallExpression mce, CountOption countOption)
        {
            ResourceExpression e = StripConvert(mce.Object) as ResourceExpression;
            if (e == null)
            {
                return mce;
            }
            ValidationRules.RequireCanAddCount(e);
            ValidationRules.RequireNonSingleton(e);
            e.CountOption = countOption;
            return e;
        }

        private static Expression AnalyzeAddCustomQueryOption(MethodCallExpression mce)
        {
            ResourceExpression e = StripConvert(mce.Object) as ResourceExpression;
            if (e == null)
            {
                return mce;
            }
            ValidationRules.RequireCanAddCustomQueryOption(e);
            ConstantExpression key = StripTo<ConstantExpression>(mce.Arguments[0]);
            ConstantExpression expression4 = StripTo<ConstantExpression>(mce.Arguments[1]);
            if (((string) key.Value).Trim() == ('$' + "expand"))
            {
                ValidationRules.RequireCanExpand(e);
                e.ExpandPaths = e.ExpandPaths.Union<string>(new string[] { ((string) expression4.Value) }, StringComparer.Ordinal).ToList<string>();
                return e;
            }
            ValidationRules.RequireLegalCustomQueryOption(mce.Arguments[0], e);
            e.CustomQueryOptions.Add(key, expression4);
            return e;
        }

        private static Expression AnalyzeCast(MethodCallExpression mce)
        {
            ResourceExpression expression = mce.Arguments[0] as ResourceExpression;
            if (expression != null)
            {
                return expression.CreateCloneWithNewType(mce.Method.ReturnType);
            }
            return mce;
        }

        private static Expression AnalyzeCountMethod(MethodCallExpression mce)
        {
            ResourceExpression e = (ResourceExpression) mce.Arguments[0];
            if (e == null)
            {
                return mce;
            }
            ValidationRules.RequireCanAddCount(e);
            ValidationRules.RequireNonSingleton(e);
            e.CountOption = CountOption.ValueOnly;
            return e;
        }

        private static Expression AnalyzeExpand(MethodCallExpression mce)
        {
            ResourceExpression e = StripConvert(mce.Object) as ResourceExpression;
            if (e == null)
            {
                return mce;
            }
            ValidationRules.RequireCanExpand(e);
            string item = (string) StripTo<ConstantExpression>(mce.Arguments[0]).Value;
            if (!e.ExpandPaths.Contains(item))
            {
                e.ExpandPaths.Add(item);
            }
            return e;
        }

        internal static Expression AnalyzeNavigation(MethodCallExpression mce)
        {
            LambdaExpression expression2;
            Expression input = mce.Arguments[0];
            if (PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out expression2))
            {
                ResourceExpression expression3;
                Expression expression4;
                MemberExpression expression5;
                if (PatternRules.MatchIdentitySelector(expression2))
                {
                    return input;
                }
                if (PatternRules.MatchTransparentIdentitySelector(input, expression2))
                {
                    return RemoveTransparentScope(mce.Method.ReturnType, (ResourceSetExpression) input);
                }
                if ((IsValidNavigationSource(input, out expression3) && TryBindToInput(expression3, expression2, out expression4)) && PatternRules.MatchPropertyProjectionSingleton(expression3, expression4, out expression5))
                {
                    expression4 = expression5;
                    return CreateNavigationPropertySingletonExpression(mce.Method.ReturnType, expression3, expression4);
                }
            }
            return mce;
        }

        private static Expression AnalyzePredicate(MethodCallExpression mce)
        {
            ResourceSetExpression expression;
            LambdaExpression expression2;
            List<Expression> list4;
            if (!TryGetResourceSetMethodArguments(mce, out expression, out expression2))
            {
                ValidationRules.RequireNonSingleton(mce.Arguments[0]);
                return mce;
            }
            List<Expression> conjuncts = new List<Expression>();
            AddConjuncts(expression2.Body, conjuncts);
            Dictionary<ResourceSetExpression, List<Expression>> dictionary = new Dictionary<ResourceSetExpression, List<Expression>>(ReferenceEqualityComparer<ResourceSetExpression>.Instance);
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            foreach (Expression expression3 in conjuncts)
            {
                Expression item = InputBinder.Bind(expression3, expression, expression2.Parameters[0], referencedInputs);
                if (referencedInputs.Count > 1)
                {
                    return mce;
                }
                ResourceSetExpression key = (referencedInputs.Count == 0) ? expression : (referencedInputs[0] as ResourceSetExpression);
                if (key == null)
                {
                    return mce;
                }
                List<Expression> list3 = null;
                if (!dictionary.TryGetValue(key, out list3))
                {
                    list3 = new List<Expression>();
                    dictionary[key] = list3;
                }
                list3.Add(item);
                referencedInputs.Clear();
            }
            conjuncts = null;
            if (dictionary.TryGetValue(expression, out list4))
            {
                dictionary.Remove(expression);
            }
            else
            {
                list4 = null;
            }
            foreach (KeyValuePair<ResourceSetExpression, List<Expression>> pair in dictionary)
            {
                Dictionary<PropertyInfo, ConstantExpression> dictionary2;
                ResourceSetExpression target = pair.Key;
                List<Expression> predicates = pair.Value;
                if (!ExtractKeyPredicate(target, predicates, out dictionary2) || (predicates.Count > 0))
                {
                    return mce;
                }
                SetKeyPredicate(target, dictionary2);
            }
            if (list4 != null)
            {
                Dictionary<PropertyInfo, ConstantExpression> dictionary3;
                int num;
                Expression predicate;
                if (ExtractKeyPredicate(expression, list4, out dictionary3))
                {
                    if (expression.HasSequenceQueryOptions)
                    {
                        Expression expression7 = BuildKeyPredicateFilter(expression.CreateReference(), dictionary3);
                        list4.Add(expression7);
                    }
                    else
                    {
                        SetKeyPredicate(expression, dictionary3);
                    }
                }
                if (list4.Count <= 0)
                {
                    return expression;
                }
                if (expression.KeyPredicate != null)
                {
                    Expression expression8 = BuildKeyPredicateFilter(expression.CreateReference(), expression.KeyPredicate);
                    list4.Add(expression8);
                    expression.KeyPredicate = null;
                }
                if (expression.Filter != null)
                {
                    num = 0;
                    predicate = expression.Filter.Predicate;
                }
                else
                {
                    num = 1;
                    predicate = list4[0];
                }
                for (int i = num; i < list4.Count; i++)
                {
                    predicate = Expression.And(predicate, list4[i]);
                }
                AddSequenceQueryOption(expression, new FilterQueryOptionExpression(mce.Method.ReturnType, predicate));
            }
            return expression;
        }

        internal bool AnalyzeProjection(MethodCallExpression mce, SequenceMethod sequenceMethod, out Expression e)
        {
            e = mce;
            bool matchMembers = sequenceMethod == SequenceMethod.SelectManyResultSelector;
            ResourceExpression currentInput = this.Visit(mce.Arguments[0]) as ResourceExpression;
            if (currentInput == null)
            {
                return false;
            }
            if (sequenceMethod == SequenceMethod.SelectManyResultSelector)
            {
                LambdaExpression expression4;
                MemberExpression expression7;
                bool flag2;
                Expression expression = mce.Arguments[1];
                if (!PatternRules.MatchParameterMemberAccess(expression))
                {
                    return false;
                }
                Expression expression3 = mce.Arguments[2];
                if (!PatternRules.MatchDoubleArgumentLambda(expression3, out expression4))
                {
                    return false;
                }
                if (ExpressionPresenceVisitor.IsExpressionPresent(expression4.Parameters[0], expression4.Body))
                {
                    return false;
                }
                List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                LambdaExpression expression5 = StripTo<LambdaExpression>(expression);
                Expression potentialPropertyRef = StripCastMethodCalls(InputBinder.Bind(expression5.Body, currentInput, expression5.Parameters[0], referencedInputs));
                if (!PatternRules.MatchPropertyProjectionSet(currentInput, potentialPropertyRef, out expression7))
                {
                    return false;
                }
                potentialPropertyRef = expression7;
                ResourceExpression source = CreateResourceSetExpression(mce.Method.ReturnType, currentInput, potentialPropertyRef, TypeSystem.GetElementType(potentialPropertyRef.Type));
                if (!PatternRules.MatchMemberInitExpressionWithDefaultConstructor(source, expression4) && !PatternRules.MatchNewExpression(source, expression4))
                {
                    return false;
                }
                expression4 = Expression.Lambda(expression4.Body, new ParameterExpression[] { expression4.Parameters[1] });
                ResourceExpression re = source.CreateCloneWithNewType(mce.Type);
                try
                {
                    flag2 = ProjectionAnalyzer.Analyze(expression4, re, false);
                }
                catch (NotSupportedException)
                {
                    flag2 = false;
                }
                if (!flag2)
                {
                    return false;
                }
                e = re;
                ValidationRules.RequireCanProject(source);
            }
            else
            {
                LambdaExpression expression10;
                if (!PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out expression10))
                {
                    return false;
                }
                expression10 = ProjectionRewriter.TryToRewrite(expression10, currentInput.ResourceType);
                ResourceExpression expression11 = currentInput.CreateCloneWithNewType(mce.Type);
                if (!ProjectionAnalyzer.Analyze(expression10, expression11, matchMembers))
                {
                    return false;
                }
                ValidationRules.RequireCanProject(currentInput);
                e = expression11;
            }
            return true;
        }

        private static Expression AnalyzeResourceSetConstantMethod(MethodCallExpression mce, Func<MethodCallExpression, ResourceExpression, ConstantExpression, Expression> constantMethodAnalyzer)
        {
            ResourceExpression expression = (ResourceExpression) mce.Arguments[0];
            ConstantExpression expression2 = StripTo<ConstantExpression>(mce.Arguments[1]);
            if (expression2 == null)
            {
                return mce;
            }
            return constantMethodAnalyzer(mce, expression, expression2);
        }

        private static Expression AnalyzeResourceSetMethod(MethodCallExpression mce, Func<MethodCallExpression, ResourceSetExpression, Expression, Expression> sequenceMethodAnalyzer)
        {
            ResourceSetExpression expression;
            LambdaExpression expression2;
            Expression expression3;
            if (!TryGetResourceSetMethodArguments(mce, out expression, out expression2))
            {
                return mce;
            }
            if (!TryBindToInput(expression, expression2, out expression3))
            {
                return mce;
            }
            return sequenceMethodAnalyzer(mce, expression, expression3);
        }

        internal static Expression AnalyzeSelectMany(MethodCallExpression mce)
        {
            if ((mce.Arguments.Count == 2) || (mce.Arguments.Count == 3))
            {
                ResourceExpression expression;
                LambdaExpression expression2;
                MemberExpression expression5;
                if (!IsValidNavigationSource(mce.Arguments[0], out expression))
                {
                    return mce;
                }
                if (!PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out expression2))
                {
                    return mce;
                }
                List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                Expression expression3 = InputBinder.Bind(expression2.Body, expression, expression2.Parameters[0], referencedInputs);
                Type elementType = TypeSystem.GetElementType(expression3.Type);
                expression3 = StripCastMethodCalls(expression3);
                ResourceSetExpression sourceResourceSet = null;
                if (PatternRules.MatchPropertyProjectionSet(expression, expression3, out expression5))
                {
                    expression3 = expression5;
                    sourceResourceSet = CreateResourceSetExpression(mce.Method.ReturnType, expression, expression3, elementType);
                    if (mce.Arguments.Count == 3)
                    {
                        return AnalyzeSelectManySelector(mce, sourceResourceSet);
                    }
                    return sourceResourceSet;
                }
            }
            return mce;
        }

        private static Expression AnalyzeSelectManySelector(MethodCallExpression selectManyCall, ResourceSetExpression sourceResourceSet)
        {
            ResourceSetExpression.TransparentAccessors accessors;
            LambdaExpression resultSelector = StripTo<LambdaExpression>(selectManyCall.Arguments[2]);
            if (PatternRules.MatchTransparentScopeSelector(sourceResourceSet, resultSelector, out accessors))
            {
                sourceResourceSet.TransparentScope = accessors;
                return sourceResourceSet;
            }
            if (PatternRules.MatchIdentityProjectionResultSelector(resultSelector))
            {
                return sourceResourceSet;
            }
            if (PatternRules.MatchMemberInitExpressionWithDefaultConstructor(sourceResourceSet, resultSelector) || PatternRules.MatchNewExpression(sourceResourceSet, resultSelector))
            {
                if (!ProjectionAnalyzer.Analyze(Expression.Lambda(resultSelector.Body, new ParameterExpression[] { resultSelector.Parameters[1] }), sourceResourceSet, false))
                {
                    return selectManyCall;
                }
                return sourceResourceSet;
            }
            return selectManyCall;
        }

        internal static Expression ApplyOrdering(MethodCallExpression mce, ResourceSetExpression input, Expression selector, bool descending, bool thenBy)
        {
            List<OrderByQueryOptionExpression.Selector> selectors;
            if (!thenBy)
            {
                selectors = new List<OrderByQueryOptionExpression.Selector>();
                AddSequenceQueryOption(input, new OrderByQueryOptionExpression(mce.Type, selectors));
            }
            else
            {
                selectors = input.OrderBy.Selectors;
            }
            selectors.Add(new OrderByQueryOptionExpression.Selector(selector, descending));
            return input;
        }

        internal static Expression Bind(Expression e)
        {
            Expression expression = new ResourceBinder().Visit(e);
            VerifyKeyPredicates(expression);
            VerifyNotSelectManyProjection(expression);
            return expression;
        }

        private static Expression BuildKeyPredicateFilter(InputReferenceExpression input, Dictionary<PropertyInfo, ConstantExpression> keyValuesDictionary)
        {
            Expression left = null;
            foreach (KeyValuePair<PropertyInfo, ConstantExpression> pair in keyValuesDictionary)
            {
                MemberExpression introduced4 = Expression.Property(input, pair.Key);
                Expression right = Expression.Equal(introduced4, pair.Value);
                if (left == null)
                {
                    left = right;
                }
                else
                {
                    left = Expression.And(left, right);
                }
            }
            return left;
        }

        private static bool CollectionContentsEqual<T>(ICollection<T> left, ICollection<T> right, IEqualityComparer<T> comparer) where T: class
        {
            if (left.Count != right.Count)
            {
                return false;
            }
            if (left.Count == 1)
            {
                return comparer.Equals(left.First<T>(), right.First<T>());
            }
            HashSet<T> set = new HashSet<T>(left, comparer);
            foreach (T local in right)
            {
                if (!set.Contains(local))
                {
                    return false;
                }
            }
            return true;
        }

        private static NavigationPropertySingletonExpression CreateNavigationPropertySingletonExpression(Type type, ResourceExpression source, Expression memberExpression)
        {
            NavigationPropertySingletonExpression expression = new NavigationPropertySingletonExpression(type, source, memberExpression, memberExpression.Type, source.ExpandPaths.ToList<string>(), source.CountOption, source.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>(kvp => kvp.Key, kvp => kvp.Value), null);
            source.ExpandPaths.Clear();
            source.CountOption = CountOption.None;
            source.CustomQueryOptions.Clear();
            return expression;
        }

        private static ResourceSetExpression CreateResourceSetExpression(Type type, ResourceExpression source, Expression memberExpression, Type resourceType)
        {
            Type elementType = TypeSystem.GetElementType(type);
            ResourceSetExpression expression = new ResourceSetExpression(typeof(IOrderedQueryable<>).MakeGenericType(new Type[] { elementType }), source, memberExpression, resourceType, source.ExpandPaths.ToList<string>(), source.CountOption, source.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>(kvp => kvp.Key, kvp => kvp.Value), null);
            source.ExpandPaths.Clear();
            source.CountOption = CountOption.None;
            source.CustomQueryOptions.Clear();
            return expression;
        }

        private static bool ExtractKeyPredicate(ResourceSetExpression target, List<Expression> predicates, out Dictionary<PropertyInfo, ConstantExpression> keyValues)
        {
            keyValues = null;
            List<Expression> collection = null;
            foreach (Expression expression in predicates)
            {
                PropertyInfo info;
                ConstantExpression expression2;
                if (PatternRules.MatchKeyComparison(expression, out info, out expression2))
                {
                    if (keyValues == null)
                    {
                        keyValues = new Dictionary<PropertyInfo, ConstantExpression>(EqualityComparer<PropertyInfo>.Default);
                    }
                    else if (keyValues.ContainsKey(info))
                    {
                        throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.ALinq_CanOnlyApplyOneKeyPredicate);
                    }
                    keyValues.Add(info, expression2);
                }
                else
                {
                    if (collection == null)
                    {
                        collection = new List<Expression>();
                    }
                    collection.Add(expression);
                }
            }
            if ((keyValues != null) && !CollectionContentsEqual<PropertyInfo>(PatternRules.GetKeyProperties(target.CreateReference().Type), keyValues.Keys, PropertyInfoEqualityComparer.Instance))
            {
                keyValues = null;
                return false;
            }
            if (keyValues != null)
            {
                predicates.Clear();
                if (collection != null)
                {
                    predicates.AddRange(collection);
                }
            }
            return (keyValues != null);
        }

        internal static bool IsMissingKeyPredicates(Expression expression)
        {
            ResourceExpression expression2 = expression as ResourceExpression;
            if (expression2 != null)
            {
                if (IsMissingKeyPredicates(expression2.Source))
                {
                    return true;
                }
                if (expression2.Source != null)
                {
                    ResourceSetExpression source = expression2.Source as ResourceSetExpression;
                    if ((source != null) && !source.HasKeyPredicate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsValidNavigationSource(Expression input, out ResourceExpression sourceExpression)
        {
            ValidationRules.RequireCanNavigate(input);
            sourceExpression = input as ResourceExpression;
            return (sourceExpression != null);
        }

        private static Expression LimitCardinality(MethodCallExpression mce, int maxCardinality)
        {
            if (mce.Arguments.Count == 1)
            {
                ResourceSetExpression target = mce.Arguments[0] as ResourceSetExpression;
                if (target != null)
                {
                    if ((!target.HasKeyPredicate && (target.NodeType != ((ExpressionType) 0x2711))) && ((target.Take == null) || (((int) target.Take.TakeAmount.Value) > maxCardinality)))
                    {
                        AddSequenceQueryOption(target, new TakeQueryOptionExpression(mce.Type, Expression.Constant(maxCardinality)));
                    }
                    return mce.Arguments[0];
                }
                if (mce.Arguments[0] is NavigationPropertySingletonExpression)
                {
                    return mce.Arguments[0];
                }
            }
            return mce;
        }

        private static ResourceSetExpression RemoveTransparentScope(Type expectedResultType, ResourceSetExpression input)
        {
            ResourceSetExpression expression = new ResourceSetExpression(expectedResultType, input.Source, input.MemberExpression, input.ResourceType, input.ExpandPaths, input.CountOption, input.CustomQueryOptions, input.Projection) {
                KeyPredicate = input.KeyPredicate
            };
            foreach (QueryOptionExpression expression2 in input.SequenceQueryOptions)
            {
                expression.AddSequenceQueryOption(expression2);
            }
            expression.OverrideInputReference(input);
            return expression;
        }

        private static void SetKeyPredicate(ResourceSetExpression rse, Dictionary<PropertyInfo, ConstantExpression> keyValues)
        {
            if (rse.KeyPredicate == null)
            {
                rse.KeyPredicate = new Dictionary<PropertyInfo, ConstantExpression>(EqualityComparer<PropertyInfo>.Default);
            }
            foreach (KeyValuePair<PropertyInfo, ConstantExpression> pair in keyValues)
            {
                if (rse.KeyPredicate.Keys.Contains<PropertyInfo>(pair.Key))
                {
                    throw System.Data.Services.Client.Error.NotSupported(System.Data.Services.Client.Strings.ALinq_CanOnlyApplyOneKeyPredicate);
                }
                rse.KeyPredicate.Add(pair.Key, pair.Value);
            }
        }

        private static Expression StripCastMethodCalls(Expression expression)
        {
            for (MethodCallExpression expression2 = StripTo<MethodCallExpression>(expression); (expression2 != null) && ReflectionUtil.IsSequenceMethod(expression2.Method, SequenceMethod.Cast); expression2 = StripTo<MethodCallExpression>(expression))
            {
                expression = expression2.Arguments[0];
            }
            return expression;
        }

        private static Expression StripConvert(Expression e)
        {
            UnaryExpression expression = e as UnaryExpression;
            if ((((expression != null) && (expression.NodeType == ExpressionType.Convert)) && expression.Type.IsGenericType) && ((expression.Type.GetGenericTypeDefinition() == typeof(DataServiceQuery<>)) || (expression.Type.GetGenericTypeDefinition() == typeof(DataServiceQuery<>.DataServiceOrderedQuery))))
            {
                e = expression.Operand;
                ResourceExpression expression2 = e as ResourceExpression;
                if (expression2 != null)
                {
                    e = expression2.CreateCloneWithNewType(expression.Type);
                }
            }
            return e;
        }

        internal static Expression StripConvertToAssignable(Expression e)
        {
            UnaryExpression expression = e as UnaryExpression;
            if ((expression != null) && PatternRules.MatchConvertToAssignable(expression))
            {
                return expression.Operand;
            }
            return e;
        }

        internal static T StripTo<T>(Expression expression) where T: Expression
        {
            Expression expression2;
            do
            {
                expression2 = expression;
                expression = (expression.NodeType == ExpressionType.Quote) ? ((UnaryExpression) expression).Operand : expression;
                expression = StripConvertToAssignable(expression);
            }
            while (expression2 != expression);
            return (expression2 as T);
        }

        private static bool TryBindToInput(ResourceExpression input, LambdaExpression le, out Expression bound)
        {
            List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
            bound = InputBinder.Bind(le.Body, input, le.Parameters[0], referencedInputs);
            if ((referencedInputs.Count > 1) || ((referencedInputs.Count == 1) && (referencedInputs[0] != input)))
            {
                bound = null;
            }
            return (bound != null);
        }

        private static bool TryGetResourceSetMethodArguments(MethodCallExpression mce, out ResourceSetExpression input, out LambdaExpression lambda)
        {
            input = null;
            lambda = null;
            input = mce.Arguments[0] as ResourceSetExpression;
            return ((input != null) && PatternRules.MatchSingleArgumentLambda(mce.Arguments[1], out lambda));
        }

        internal static void VerifyKeyPredicates(Expression e)
        {
            if (IsMissingKeyPredicates(e))
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantNavigateWithoutKeyPredicate);
            }
        }

        internal static void VerifyNotSelectManyProjection(Expression expression)
        {
            ResourceSetExpression expression2 = expression as ResourceSetExpression;
            if (expression2 != null)
            {
                ProjectionQueryOptionExpression projection = expression2.Projection;
                if (projection != null)
                {
                    MethodCallExpression expression4 = StripTo<MethodCallExpression>(projection.Selector.Body);
                    if ((expression4 != null) && (expression4.Method.Name == "SelectMany"))
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_UnsupportedExpression(expression4));
                    }
                }
                else if (expression2.HasTransparentScope)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_UnsupportedExpression(expression2));
                }
            }
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            Expression e = base.VisitBinary(b);
            if (PatternRules.MatchStringAddition(e))
            {
                BinaryExpression expression2 = StripTo<BinaryExpression>(e);
                return Expression.Call(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }), new Expression[] { expression2.Left, expression2.Right });
            }
            return e;
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            PropertyInfo info;
            MethodInfo info2;
            Expression expression = base.VisitMemberAccess(m);
            MemberExpression me = StripTo<MemberExpression>(expression);
            if (((me != null) && PatternRules.MatchNonPrivateReadableProperty(me, out info)) && TypeSystem.TryGetPropertyAsMethod(info, out info2))
            {
                return Expression.Call(me.Expression, info2);
            }
            return expression;
        }

        internal override Expression VisitMethodCall(MethodCallExpression mce)
        {
            Expression expression;
            SequenceMethod method;
            if ((ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out method) && ((method == SequenceMethod.Select) || (method == SequenceMethod.SelectManyResultSelector))) && this.AnalyzeProjection(mce, method, out expression))
            {
                return expression;
            }
            expression = base.VisitMethodCall(mce);
            mce = expression as MethodCallExpression;
            if (mce == null)
            {
                return expression;
            }
            if (!ReflectionUtil.TryIdentifySequenceMethod(mce.Method, out method))
            {
                if (!mce.Method.DeclaringType.IsGenericType || (mce.Method.DeclaringType.GetGenericTypeDefinition() != typeof(DataServiceQuery<>)))
                {
                    return mce;
                }
                Type type = typeof(DataServiceQuery<>).MakeGenericType(new Type[] { mce.Method.DeclaringType.GetGenericArguments()[0] });
                if (mce.Method == type.GetMethod("Expand", new Type[] { typeof(string) }))
                {
                    return AnalyzeExpand(mce);
                }
                if (mce.Method == type.GetMethod("AddQueryOption", new Type[] { typeof(string), typeof(object) }))
                {
                    return AnalyzeAddCustomQueryOption(mce);
                }
                if (mce.Method != type.GetMethod("IncludeTotalCount"))
                {
                    throw System.Data.Services.Client.Error.MethodNotSupported(mce);
                }
                return AnalyzeAddCountOption(mce, CountOption.InlineAll);
            }
            switch (method)
            {
                case SequenceMethod.Where:
                    return AnalyzePredicate(mce);

                case SequenceMethod.Cast:
                    return AnalyzeCast(mce);

                case SequenceMethod.Select:
                    return AnalyzeNavigation(mce);

                case SequenceMethod.SelectMany:
                case SequenceMethod.SelectManyResultSelector:
                    return AnalyzeSelectMany(mce);

                case SequenceMethod.OrderBy:
                    return AnalyzeResourceSetMethod(mce, (callExp, resource, selector) => ApplyOrdering(callExp, resource, selector, false, false));

                case SequenceMethod.OrderByDescending:
                    return AnalyzeResourceSetMethod(mce, (callExp, resource, selector) => ApplyOrdering(callExp, resource, selector, true, false));

                case SequenceMethod.ThenBy:
                    return AnalyzeResourceSetMethod(mce, (callExp, resource, selector) => ApplyOrdering(callExp, resource, selector, false, true));

                case SequenceMethod.ThenByDescending:
                    return AnalyzeResourceSetMethod(mce, (callExp, resource, selector) => ApplyOrdering(callExp, resource, selector, true, true));

                case SequenceMethod.Take:
                    return AnalyzeResourceSetConstantMethod(mce, delegate (MethodCallExpression callExp, ResourceExpression resource, ConstantExpression takeCount) {
                        AddSequenceQueryOption(resource, new TakeQueryOptionExpression(callExp.Type, takeCount));
                        return resource;
                    });

                case SequenceMethod.Skip:
                    return AnalyzeResourceSetConstantMethod(mce, delegate (MethodCallExpression callExp, ResourceExpression resource, ConstantExpression skipCount) {
                        AddSequenceQueryOption(resource, new SkipQueryOptionExpression(callExp.Type, skipCount));
                        return resource;
                    });

                case SequenceMethod.First:
                case SequenceMethod.FirstOrDefault:
                    return LimitCardinality(mce, 1);

                case SequenceMethod.Single:
                case SequenceMethod.SingleOrDefault:
                    return LimitCardinality(mce, 2);

                case SequenceMethod.Count:
                case SequenceMethod.LongCount:
                    return AnalyzeCountMethod(mce);
            }
            throw System.Data.Services.Client.Error.MethodNotSupported(mce);
        }

        internal override Expression VisitResourceSetExpression(ResourceSetExpression rse)
        {
            if (rse.NodeType == ((ExpressionType) 0x2710))
            {
                return new ResourceSetExpression(rse.Type, rse.Source, rse.MemberExpression, rse.ResourceType, null, CountOption.None, null, null);
            }
            return rse;
        }

        private sealed class ExpressionPresenceVisitor : DataServiceExpressionVisitor
        {
            private bool found;
            private readonly Expression target;

            private ExpressionPresenceVisitor(Expression target)
            {
                this.target = target;
            }

            internal static bool IsExpressionPresent(Expression target, Expression tree)
            {
                ResourceBinder.ExpressionPresenceVisitor visitor = new ResourceBinder.ExpressionPresenceVisitor(target);
                visitor.Visit(tree);
                return visitor.found;
            }

            internal override Expression Visit(Expression exp)
            {
                if (this.found || object.ReferenceEquals(this.target, exp))
                {
                    this.found = true;
                    return exp;
                }
                return base.Visit(exp);
            }
        }

        internal static class PatternRules
        {
            private static bool ExpressionIsSimpleAccess(Expression argument, ReadOnlyCollection<ParameterExpression> expressions)
            {
                MemberExpression expression2;
                Expression expression = argument;
                do
                {
                    expression2 = expression as MemberExpression;
                    if (expression2 != null)
                    {
                        expression = expression2.Expression;
                    }
                }
                while (expression2 != null);
                ParameterExpression expression3 = expression as ParameterExpression;
                if (expression3 == null)
                {
                    return false;
                }
                return expressions.Contains(expression3);
            }

            internal static List<PropertyInfo> GetKeyProperties(Type type)
            {
                ClientType type2 = ClientType.Create(type, false);
                List<PropertyInfo> list = new List<PropertyInfo>();
                foreach (ClientType.ClientProperty property in type2.Properties)
                {
                    if (property.KeyProperty)
                    {
                        list.Add(property.DeclaringType.GetProperty(property.PropertyName));
                    }
                }
                return list;
            }

            internal static bool MatchAnd(Expression e)
            {
                BinaryExpression expression = e as BinaryExpression;
                if (expression == null)
                {
                    return false;
                }
                if (expression.NodeType != ExpressionType.And)
                {
                    return (expression.NodeType == ExpressionType.AndAlso);
                }
                return true;
            }

            internal static bool MatchBinaryEquality(Expression e) => 
                (MatchBinaryExpression(e) && (((BinaryExpression) e).NodeType == ExpressionType.Equal));

            internal static bool MatchBinaryExpression(Expression e) => 
                (e is BinaryExpression);

            internal static bool MatchConstant(Expression e, out ConstantExpression constExpr)
            {
                constExpr = e as ConstantExpression;
                return (constExpr != null);
            }

            internal static bool MatchConvertToAssignable(UnaryExpression expression)
            {
                if (((expression.NodeType != ExpressionType.Convert) && (expression.NodeType != ExpressionType.ConvertChecked)) && (expression.NodeType != ExpressionType.TypeAs))
                {
                    return false;
                }
                return expression.Type.IsAssignableFrom(expression.Operand.Type);
            }

            internal static bool MatchDoubleArgumentLambda(Expression expression, out LambdaExpression lambda) => 
                MatchNaryLambda(expression, 2, out lambda);

            internal static MatchEqualityCheckResult MatchEquality(Expression expression)
            {
                MatchEqualityCheckResult result = new MatchEqualityCheckResult {
                    Match = false,
                    EqualityYieldsTrue = true
                };
            Label_0018:
                if (MatchReferenceEquals(expression))
                {
                    MethodCallExpression expression2 = (MethodCallExpression) expression;
                    result.Match = true;
                    result.TestLeft = expression2.Arguments[0];
                    result.TestRight = expression2.Arguments[1];
                    return result;
                }
                if (MatchNot(expression))
                {
                    result.EqualityYieldsTrue = !result.EqualityYieldsTrue;
                    expression = ((UnaryExpression) expression).Operand;
                    goto Label_0018;
                }
                BinaryExpression expression3 = expression as BinaryExpression;
                if (expression3 != null)
                {
                    if (expression3.NodeType == ExpressionType.NotEqual)
                    {
                        result.EqualityYieldsTrue = !result.EqualityYieldsTrue;
                    }
                    else if (expression3.NodeType != ExpressionType.Equal)
                    {
                        return result;
                    }
                    result.TestLeft = expression3.Left;
                    result.TestRight = expression3.Right;
                    result.Match = true;
                }
                return result;
            }

            internal static bool MatchIdentityProjectionResultSelector(Expression e)
            {
                LambdaExpression expression = (LambdaExpression) e;
                return (expression.Body == expression.Parameters[1]);
            }

            internal static bool MatchIdentitySelector(LambdaExpression lambda)
            {
                ParameterExpression expression = lambda.Parameters[0];
                return (expression == ResourceBinder.StripTo<ParameterExpression>(lambda.Body));
            }

            internal static bool MatchKeyComparison(Expression e, out PropertyInfo keyProperty, out ConstantExpression keyValue)
            {
                if (MatchBinaryEquality(e))
                {
                    BinaryExpression expression = (BinaryExpression) e;
                    if ((MatchKeyProperty(expression.Left, out keyProperty) && MatchConstant(expression.Right, out keyValue)) || (MatchKeyProperty(expression.Right, out keyProperty) && MatchConstant(expression.Left, out keyValue)))
                    {
                        return (keyValue.Value != null);
                    }
                }
                keyProperty = null;
                keyValue = null;
                return false;
            }

            internal static bool MatchKeyProperty(Expression expression, out PropertyInfo property)
            {
                PropertyInfo info;
                property = null;
                if (MatchNonPrivateReadableProperty(expression, out info) && GetKeyProperties(info.ReflectedType).Contains<PropertyInfo>(info, ResourceBinder.PropertyInfoEqualityComparer.Instance))
                {
                    property = info;
                    return true;
                }
                return false;
            }

            internal static bool MatchMemberInitExpressionWithDefaultConstructor(Expression source, LambdaExpression e)
            {
                ResourceExpression expression2;
                MemberInitExpression expression = ResourceBinder.StripTo<MemberInitExpression>(e.Body);
                return ((MatchResource(source, out expression2) && (expression != null)) && (expression.NewExpression.Arguments.Count == 0));
            }

            private static bool MatchNaryLambda(Expression expression, int parameterCount, out LambdaExpression lambda)
            {
                lambda = null;
                LambdaExpression expression2 = ResourceBinder.StripTo<LambdaExpression>(expression);
                if ((expression2 != null) && (expression2.Parameters.Count == parameterCount))
                {
                    lambda = expression2;
                }
                return (lambda != null);
            }

            private static bool MatchNavigationPropertyProjection(ResourceExpression input, Expression potentialPropertyRef, bool requireSet, out MemberExpression navigationMember)
            {
                Expression expression;
                List<string> list;
                if (((MatchNonSingletonProperty(potentialPropertyRef) == requireSet) && MatchPropertyAccess(potentialPropertyRef, out navigationMember, out expression, out list)) && (expression == input.CreateReference()))
                {
                    return true;
                }
                navigationMember = null;
                return false;
            }

            internal static bool MatchNewDataServiceCollectionOfT(NewExpression nex) => 
                (nex.Type.IsGenericType && WebUtil.IsDataServiceCollectionType(nex.Type.GetGenericTypeDefinition()));

            internal static bool MatchNewExpression(Expression source, LambdaExpression e)
            {
                ResourceExpression expression;
                return (MatchResource(source, out expression) && (e.Body is NewExpression));
            }

            internal static bool MatchNonPrivateReadableProperty(Expression e, out PropertyInfo propInfo)
            {
                MemberExpression me = e as MemberExpression;
                if (me == null)
                {
                    propInfo = null;
                    return false;
                }
                return MatchNonPrivateReadableProperty(me, out propInfo);
            }

            internal static bool MatchNonPrivateReadableProperty(MemberExpression me, out PropertyInfo propInfo)
            {
                propInfo = null;
                if (me.Member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo member = (PropertyInfo) me.Member;
                    if (member.CanRead && !TypeSystem.IsPrivate(member))
                    {
                        propInfo = member;
                        return true;
                    }
                }
                return false;
            }

            internal static bool MatchNonSingletonProperty(Expression e) => 
                (((TypeSystem.FindIEnumerable(e.Type) != null) && (e.Type != typeof(char[]))) && (e.Type != typeof(byte[])));

            internal static bool MatchNot(Expression expression) => 
                (expression.NodeType == ExpressionType.Not);

            internal static MatchNullCheckResult MatchNullCheck(Expression entityInScope, ConditionalExpression conditional)
            {
                MatchNullCheckResult result = new MatchNullCheckResult();
                MatchEqualityCheckResult result2 = MatchEquality(conditional.Test);
                if (result2.Match)
                {
                    Expression ifFalse;
                    Expression testRight;
                    if (result2.EqualityYieldsTrue)
                    {
                        if (!MatchNullConstant(conditional.IfTrue))
                        {
                            return result;
                        }
                        ifFalse = conditional.IfFalse;
                    }
                    else
                    {
                        if (!MatchNullConstant(conditional.IfFalse))
                        {
                            return result;
                        }
                        ifFalse = conditional.IfTrue;
                    }
                    if (MatchNullConstant(result2.TestLeft))
                    {
                        testRight = result2.TestRight;
                    }
                    else if (MatchNullConstant(result2.TestRight))
                    {
                        testRight = result2.TestLeft;
                    }
                    else
                    {
                        return result;
                    }
                    MemberAssignmentAnalysis analysis = MemberAssignmentAnalysis.Analyze(entityInScope, ifFalse);
                    if (!analysis.MultiplePathsFound)
                    {
                        MemberAssignmentAnalysis analysis2 = MemberAssignmentAnalysis.Analyze(entityInScope, testRight);
                        if (analysis2.MultiplePathsFound)
                        {
                            return result;
                        }
                        Expression[] expressionsToTargetEntity = analysis.GetExpressionsToTargetEntity();
                        Expression[] expressionArray2 = analysis2.GetExpressionsToTargetEntity();
                        if (expressionArray2.Length > expressionsToTargetEntity.Length)
                        {
                            return result;
                        }
                        for (int i = 0; i < expressionArray2.Length; i++)
                        {
                            Expression expression3 = expressionsToTargetEntity[i];
                            Expression expression4 = expressionArray2[i];
                            if (expression3 != expression4)
                            {
                                if ((expression3.NodeType != expression4.NodeType) || (expression3.NodeType != ExpressionType.MemberAccess))
                                {
                                    return result;
                                }
                                if (((MemberExpression) expression3).Member != ((MemberExpression) expression4).Member)
                                {
                                    return result;
                                }
                            }
                        }
                        result.AssignExpression = ifFalse;
                        result.Match = true;
                        result.TestToNullExpression = testRight;
                    }
                }
                return result;
            }

            internal static bool MatchNullConstant(Expression expression)
            {
                ConstantExpression expression2 = expression as ConstantExpression;
                return ((expression2 != null) && (expression2.Value == null));
            }

            internal static bool MatchParameterMemberAccess(Expression expression)
            {
                LambdaExpression expression2 = ResourceBinder.StripTo<LambdaExpression>(expression);
                if ((expression2 != null) && (expression2.Parameters.Count == 1))
                {
                    ParameterExpression expression3 = expression2.Parameters[0];
                    for (MemberExpression expression5 = ResourceBinder.StripTo<MemberExpression>(ResourceBinder.StripCastMethodCalls(expression2.Body)); expression5 != null; expression5 = ResourceBinder.StripTo<MemberExpression>(expression5.Expression))
                    {
                        if (expression5.Expression == expression3)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            internal static bool MatchPropertyAccess(Expression e, out MemberExpression member, out Expression instance, out List<string> propertyPath)
            {
                instance = null;
                propertyPath = null;
                MemberExpression me = ResourceBinder.StripTo<MemberExpression>(e);
                member = me;
                while (me != null)
                {
                    PropertyInfo info;
                    if (MatchNonPrivateReadableProperty(me, out info))
                    {
                        if (propertyPath == null)
                        {
                            propertyPath = new List<string>();
                        }
                        propertyPath.Insert(0, info.Name);
                        e = me.Expression;
                        me = ResourceBinder.StripTo<MemberExpression>(e);
                    }
                    else
                    {
                        me = null;
                    }
                }
                if (propertyPath != null)
                {
                    instance = e;
                    return true;
                }
                return false;
            }

            internal static bool MatchPropertyProjectionSet(ResourceExpression input, Expression potentialPropertyRef, out MemberExpression navigationMember) => 
                MatchNavigationPropertyProjection(input, potentialPropertyRef, true, out navigationMember);

            internal static bool MatchPropertyProjectionSingleton(ResourceExpression input, Expression potentialPropertyRef, out MemberExpression navigationMember) => 
                MatchNavigationPropertyProjection(input, potentialPropertyRef, false, out navigationMember);

            internal static bool MatchReferenceEquals(Expression expression)
            {
                MethodCallExpression expression2 = expression as MethodCallExpression;
                return (expression2?.Method == typeof(object).GetMethod("ReferenceEquals"));
            }

            internal static bool MatchResource(Expression expression, out ResourceExpression resource)
            {
                resource = expression as ResourceExpression;
                return (resource != null);
            }

            internal static bool MatchSingleArgumentLambda(Expression expression, out LambdaExpression lambda) => 
                MatchNaryLambda(expression, 1, out lambda);

            internal static bool MatchStringAddition(Expression e)
            {
                if (e.NodeType != ExpressionType.Add)
                {
                    return false;
                }
                BinaryExpression expression = e as BinaryExpression;
                return (((expression != null) && (expression.Left.Type == typeof(string))) && (expression.Right.Type == typeof(string)));
            }

            internal static bool MatchTransparentIdentitySelector(Expression input, LambdaExpression selector)
            {
                MemberExpression expression4;
                Expression expression5;
                List<string> list;
                if (selector.Parameters.Count != 1)
                {
                    return false;
                }
                ResourceSetExpression expression = input as ResourceSetExpression;
                if ((expression == null) || (expression.TransparentScope == null))
                {
                    return false;
                }
                Expression body = selector.Body;
                ParameterExpression expression3 = selector.Parameters[0];
                if (!MatchPropertyAccess(body, out expression4, out expression5, out list))
                {
                    return false;
                }
                return (((expression5 == expression3) && (list.Count == 1)) && (list[0] == expression.TransparentScope.Accessor));
            }

            internal static bool MatchTransparentScopeSelector(ResourceSetExpression input, LambdaExpression resultSelector, out ResourceSetExpression.TransparentAccessors transparentScope)
            {
                transparentScope = null;
                if (resultSelector.Body.NodeType != ExpressionType.New)
                {
                    return false;
                }
                NewExpression body = (NewExpression) resultSelector.Body;
                if (body.Arguments.Count < 2)
                {
                    return false;
                }
                if (body.Type.BaseType != typeof(object))
                {
                    return false;
                }
                ParameterInfo[] parameters = body.Constructor.GetParameters();
                if (body.Members.Count != parameters.Length)
                {
                    return false;
                }
                ResourceSetExpression source = input.Source as ResourceSetExpression;
                int index = -1;
                ParameterExpression expression3 = resultSelector.Parameters[0];
                ParameterExpression expression4 = resultSelector.Parameters[1];
                MemberInfo[] infoArray2 = new MemberInfo[body.Members.Count];
                PropertyInfo[] properties = body.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                Dictionary<string, Expression> sourceAccesors = new Dictionary<string, Expression>(parameters.Length - 1, StringComparer.Ordinal);
                for (int i = 0; i < body.Arguments.Count; i++)
                {
                    Func<PropertyInfo, bool> predicate = null;
                    Expression argument = body.Arguments[i];
                    MemberInfo member = body.Members[i];
                    if (!ExpressionIsSimpleAccess(argument, resultSelector.Parameters))
                    {
                        return false;
                    }
                    if (member.MemberType == MemberTypes.Method)
                    {
                        if (predicate == null)
                        {
                            predicate = property => property.GetGetMethod() == member;
                        }
                        member = properties.Where<PropertyInfo>(predicate).FirstOrDefault<PropertyInfo>();
                        if (member == null)
                        {
                            return false;
                        }
                    }
                    if (member.Name != parameters[i].Name)
                    {
                        return false;
                    }
                    infoArray2[i] = member;
                    ParameterExpression expression6 = ResourceBinder.StripTo<ParameterExpression>(argument);
                    if (expression4 == expression6)
                    {
                        if (index != -1)
                        {
                            return false;
                        }
                        index = i;
                    }
                    else if (expression3 == expression6)
                    {
                        sourceAccesors[member.Name] = source.CreateReference();
                    }
                    else
                    {
                        List<ResourceExpression> referencedInputs = new List<ResourceExpression>();
                        InputBinder.Bind(argument, source, resultSelector.Parameters[0], referencedInputs);
                        if (referencedInputs.Count != 1)
                        {
                            return false;
                        }
                        sourceAccesors[member.Name] = referencedInputs[0].CreateReference();
                    }
                }
                if (index == -1)
                {
                    return false;
                }
                string name = infoArray2[index].Name;
                transparentScope = new ResourceSetExpression.TransparentAccessors(name, sourceAccesors);
                return true;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct MatchEqualityCheckResult
            {
                internal bool EqualityYieldsTrue;
                internal bool Match;
                internal Expression TestLeft;
                internal Expression TestRight;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct MatchNullCheckResult
            {
                internal Expression AssignExpression;
                internal bool Match;
                internal Expression TestToNullExpression;
            }
        }

        private sealed class PropertyInfoEqualityComparer : IEqualityComparer<PropertyInfo>
        {
            internal static readonly ResourceBinder.PropertyInfoEqualityComparer Instance = new ResourceBinder.PropertyInfoEqualityComparer();

            private PropertyInfoEqualityComparer()
            {
            }

            public bool Equals(PropertyInfo left, PropertyInfo right)
            {
                if (object.ReferenceEquals(left, right))
                {
                    return true;
                }
                if ((left == null) || (right == null))
                {
                    return false;
                }
                return (object.ReferenceEquals(left.DeclaringType, right.DeclaringType) && left.Name.Equals(right.Name));
            }

            public int GetHashCode(PropertyInfo obj) => 
                obj.GetHashCode();
        }

        private static class ValidationRules
        {
            internal static void RequireCanAddCount(Expression e)
            {
                ResourceExpression resource = (ResourceExpression) e;
                if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotAddCountOption);
                }
                if (resource.CountOption != CountOption.None)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotAddCountOptionConflict);
                }
            }

            internal static void RequireCanAddCustomQueryOption(Expression e)
            {
                ResourceExpression resource = (ResourceExpression) e;
                if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddQueryOption);
                }
            }

            internal static void RequireCanExpand(Expression e)
            {
                ResourceExpression resource = (ResourceExpression) e;
                if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantExpand);
                }
                if (resource.Projection != null)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotProjectWithExplicitExpansion);
                }
            }

            internal static void RequireCanNavigate(Expression e)
            {
                ResourceExpression expression2;
                ResourceSetExpression expression = e as ResourceSetExpression;
                if ((expression != null) && expression.HasSequenceQueryOptions)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionsOnlyAllowedOnLeafNodes);
                }
                if (ResourceBinder.PatternRules.MatchResource(e, out expression2) && (expression2.Projection != null))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ProjectionOnlyAllowedOnLeafNodes);
                }
            }

            internal static void RequireCanProject(Expression e)
            {
                ResourceExpression resource = (ResourceExpression) e;
                if (!ResourceBinder.PatternRules.MatchResource(e, out resource))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CanOnlyProjectTheLeaf);
                }
                if (resource.Projection != null)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_ProjectionCanOnlyHaveOneProjection);
                }
                if (resource.ExpandPaths.Count > 0)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CannotProjectWithExplicitExpansion);
                }
            }

            internal static void RequireLegalCustomQueryOption(Expression e, ResourceExpression target)
            {
                Func<KeyValuePair<ConstantExpression, ConstantExpression>, bool> predicate = null;
                string name = ((string) (e as ConstantExpression).Value).Trim();
                if (name[0] == '$')
                {
                    if (predicate == null)
                    {
                        predicate = c => ((string) c.Key.Value) == name;
                    }
                    if (target.CustomQueryOptions.Any<KeyValuePair<ConstantExpression, ConstantExpression>>(predicate))
                    {
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddDuplicateQueryOption(name));
                    }
                    ResourceSetExpression expression = target as ResourceSetExpression;
                    if (expression != null)
                    {
                        switch (name.Substring(1))
                        {
                            case "filter":
                                if (expression.Filter != null)
                                {
                                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                return;

                            case "orderby":
                                if (expression.OrderBy != null)
                                {
                                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                return;

                            case "expand":
                                return;

                            case "skip":
                                if (expression.Skip != null)
                                {
                                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                return;

                            case "top":
                                if (expression.Take != null)
                                {
                                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                return;

                            case "inlinecount":
                                if (expression.CountOption != CountOption.None)
                                {
                                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddAstoriaQueryOption(name));
                                }
                                return;
                        }
                        throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantAddQueryOptionStartingWithDollarSign(name));
                    }
                }
            }

            internal static void RequireNonSingleton(Expression e)
            {
                ResourceExpression expression = e as ResourceExpression;
                if ((expression != null) && expression.IsSingleton)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_QueryOptionsOnlyAllowedOnSingletons);
                }
            }
        }
    }
}

