namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.EntitySql;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal sealed class ExpressionConverter
    {
        private readonly Dictionary<DbExpression, KeyValuePair<DbGroupByTemplate, string>> _aggregateDefaultTranslationToOptimizedTranslationInfoMap;
        private readonly BindingContext _bindingContext;
        private List<ClosureBinding> _closureBindings;
        private readonly HashSet<Expression> _closureCandidates = new HashSet<Expression>();
        private readonly DbCommandTree _commandTree;
        private readonly ObjectContext _context;
        private readonly Expression _expression;
        private readonly Dictionary<DbExpression, DbGroupByTemplate> _groupByDefaultToOptimizedTranslationMap;
        private Dictionary<Type, InitializerMetadata> _initializers;
        private readonly Expression _innerExpression;
        private readonly Stack<Expression> _linqExpressionStack = new Stack<Expression>();
        private MergeOption? _mergeOption;
        private ObjectParameterCollection _parameters;
        private Span _span;
        private Dictionary<DbExpression, Span> _spanMappings;
        private readonly TypeResolver _typeResolver;
        private readonly Dictionary<string, DbExpression> _variableNameToInputExpression;
        private const string BitwiseAnd = "BitwiseAnd";
        private const string BitwiseNot = "BitwiseNot";
        private const string BitwiseOr = "BitwiseOr";
        private const string BitwiseXor = "BitwiseXor";
        private const string Concat = "Concat";
        private const string CurrentDateTime = "CurrentDateTime";
        private const string CurrentDateTimeOffset = "CurrentDateTimeOffset";
        private const string CurrentUtcDateTime = "CurrentUtcDateTime";
        private const string Day = "Day";
        internal const string EdmNamespaceName = "Edm";
        internal const string EntityCollectionElementsColumnName = "Elements";
        internal const string EntityCollectionOwnerColumnName = "Owner";
        internal const string GroupColumnName = "Group";
        private const string Hour = "Hour";
        private const string IndexOf = "IndexOf";
        internal const string KeyColumnName = "Key";
        private const string Length = "Length";
        private const string LTrim = "LTrim";
        private const string Millisecond = "Millisecond";
        private const string Minute = "Minute";
        private const string Month = "Month";
        private const string Right = "Right";
        private const string RTrim = "RTrim";
        internal const string s_entityCollectionCountPropertyName = "Count";
        internal const string s_nullableHasValuePropertyName = "HasValue";
        internal const string s_nullableValuePropertyName = "Value";
        private static readonly Dictionary<ExpressionType, Translator> s_translators = InitializeTranslators();
        private const string s_visualBasicAssemblyFullName = "Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private const string Second = "Second";
        private const string Substring = "Substring";
        private const string ToLower = "ToLower";
        private const string ToUpper = "ToUpper";
        private const string Year = "Year";

        internal ExpressionConverter(ObjectContext objectContext, BindingContext bindingContext, DbCommandTree commandTree, Expression toConvert, ObjectParameterCollection sourceParams)
        {
            this._bindingContext = bindingContext;
            this._context = objectContext;
            this._commandTree = commandTree;
            if (sourceParams != null)
            {
                foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) sourceParams)
                {
                    this.AddParameter(parameter);
                }
            }
            this._expression = this.NormalizeExpression(toConvert);
            if (this._expression.NodeType == ExpressionType.Lambda)
            {
                this._innerExpression = ((LambdaExpression) this._expression).Body;
            }
            else
            {
                this._innerExpression = this._expression;
            }
            this._typeResolver = new TypeResolver(this._context.Perspective, StringComparer.Ordinal);
            this._groupByDefaultToOptimizedTranslationMap = new Dictionary<DbExpression, DbGroupByTemplate>();
            this._aggregateDefaultTranslationToOptimizedTranslationInfoMap = new Dictionary<DbExpression, KeyValuePair<DbGroupByTemplate, string>>();
            this._variableNameToInputExpression = new Dictionary<string, DbExpression>();
        }

        internal void AddClosureBinding(ClosureBinding binding)
        {
            this.AddClosureBinding(binding, true);
        }

        private void AddClosureBinding(ClosureBinding binding, bool addParam)
        {
            if (this._closureBindings == null)
            {
                this._closureBindings = new List<ClosureBinding>();
            }
            this._closureBindings.Add(binding);
            if (addParam && (binding.Parameter != null))
            {
                this.AddParameter(binding.Parameter);
            }
        }

        private void AddParameter(ObjectParameter newParam)
        {
            if (this._parameters == null)
            {
                this._parameters = new ObjectParameterCollection(this._context.Perspective);
            }
            this._parameters.Add(newParam);
        }

        private DbExpression AddSpanMapping(DbExpression expression, Span span)
        {
            if (span != null)
            {
                if (this._spanMappings == null)
                {
                    this._spanMappings = new Dictionary<DbExpression, Span>();
                }
                this._spanMappings[expression] = span;
            }
            return expression;
        }

        private DbExpression AlignTypes(DbExpression cqt, Type toClrType)
        {
            Type fromClrType = null;
            TypeUsage toType = this.GetCastTargetType(cqt.ResultType, toClrType, fromClrType, false);
            if (toType != null)
            {
                return this._commandTree.CreateCastExpression(cqt, toType);
            }
            return cqt;
        }

        private void ApplySpanMapping(DbExpression from, DbExpression to)
        {
            Span span;
            if (this.TryGetSpan(from, out span))
            {
                this.AddSpanMapping(to, span);
            }
        }

        private static bool CanOmitCast(TypeUsage fromType, TypeUsage toType, bool preserveCastForDateTime)
        {
            bool flag = TypeSemantics.IsPrimitiveType(fromType);
            if ((flag && preserveCastForDateTime) && (((PrimitiveType) fromType.EdmType).PrimitiveTypeKind == PrimitiveTypeKind.DateTime))
            {
                return false;
            }
            if (TypeUsageEquals(fromType, toType))
            {
                return true;
            }
            if (flag)
            {
                return fromType.EdmType.EdmEquals(toType.EdmType);
            }
            return TypeSemantics.IsSubTypeOf(fromType, toType);
        }

        internal static bool CanTranslatePropertyInfo(PropertyInfo propertyInfo) => 
            MemberAccessTranslator.CanTranslatePropertyInfo(propertyInfo);

        private void CheckInitializerType(Type type)
        {
            TypeUsage usage;
            if (this._context.Perspective.TryGetType(type, out usage))
            {
                BuiltInTypeKind builtInTypeKind = usage.EdmType.BuiltInTypeKind;
                if ((BuiltInTypeKind.EntityType == builtInTypeKind) || (BuiltInTypeKind.ComplexType == builtInTypeKind))
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedNominalType(usage.EdmType.FullName));
                }
            }
            if (TypeSystem.IsSequenceType(type))
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedEnumerableType(DescribeClrType(type)));
            }
        }

        internal DbExpression Convert()
        {
            DbExpression expression = this.TranslateExpression(this._expression);
            if (!this.TryGetSpan(expression, out this._span))
            {
                this._span = null;
            }
            return expression;
        }

        private DbFunctionExpression CreateCanonicalFunction(string functionName, Expression linqExpression, params DbExpression[] translatedArguments)
        {
            List<TypeUsage> argumentTypes = new List<TypeUsage>(translatedArguments.Length);
            foreach (DbExpression expression in translatedArguments)
            {
                argumentTypes.Add(expression.ResultType);
            }
            EdmFunction function = this.FindCanonicalFunction(functionName, argumentTypes, false, linqExpression);
            return this._commandTree.CreateFunctionExpression(function, translatedArguments);
        }

        private DbExpression CreateCastExpression(DbExpression source, Type toClrType, Type fromClrType)
        {
            DbExpression objB = this.NormalizeSetSource(source);
            if (!object.ReferenceEquals(source, objB) && (this.GetCastTargetType(objB.ResultType, toClrType, fromClrType, true) == null))
            {
                return source;
            }
            TypeUsage toType = this.GetCastTargetType(source.ResultType, toClrType, fromClrType, true);
            if (toType == null)
            {
                return source;
            }
            return this._commandTree.CreateCastExpression(source, toType);
        }

        private DbExpression CreateEqualsExpression(DbExpression left, DbExpression right, EqualsPattern pattern, Type leftClrType, Type rightClrType)
        {
            VerifyTypeSupportedForComparison(leftClrType, left.ResultType, null);
            VerifyTypeSupportedForComparison(rightClrType, right.ResultType, null);
            return this.RecursivelyRewriteEqualsExpression(left, right, pattern);
        }

        private DbExpression CreateIsNullExpression(DbExpression operand, Type operandClrType)
        {
            VerifyTypeSupportedForComparison(operandClrType, operand.ResultType, null);
            return this._commandTree.CreateIsNullExpression(operand);
        }

        private DbNewInstanceExpression CreateNewRowExpression(List<KeyValuePair<string, DbExpression>> columns, InitializerMetadata initializerMetadata)
        {
            List<DbExpression> args = new List<DbExpression>(columns.Count);
            List<EdmProperty> properties = new List<EdmProperty>(columns.Count);
            for (int i = 0; i < columns.Count; i++)
            {
                KeyValuePair<string, DbExpression> pair = columns[i];
                args.Add(pair.Value);
                properties.Add(new EdmProperty(pair.Key, pair.Value.ResultType));
            }
            RowType edmType = new RowType(properties, initializerMetadata);
            TypeUsage type = TypeUsage.Create(edmType);
            return this._commandTree.CreateNewInstanceExpression(type, args);
        }

        internal static string DescribeClrType(Type clrType)
        {
            string name = clrType.Name;
            if (IsCSharpGeneratedClass(name, "DisplayClass") || IsVBGeneratedClass(name, "Closure"))
            {
                return System.Data.Entity.Strings.ELinq_ClosureType;
            }
            if (IsCSharpGeneratedClass(name, "AnonymousType") || IsVBGeneratedClass(name, "AnonymousType"))
            {
                return System.Data.Entity.Strings.ELinq_AnonymousType;
            }
            string str2 = string.Empty;
            if (!string.IsNullOrEmpty(clrType.Namespace))
            {
                str2 = str2 + clrType.Namespace + ".";
            }
            return (str2 + clrType.Name);
        }

        private DbDistinctExpression Distinct(DbExpression argument)
        {
            DbDistinctExpression to = this._commandTree.CreateDistinctExpression(argument);
            this.ApplySpanMapping(argument, to);
            return to;
        }

        private DbExceptExpression Except(DbExpression left, DbExpression right)
        {
            DbExceptExpression to = this._commandTree.CreateExceptExpression(left, right);
            this.ApplySpanMapping(left, to);
            return to;
        }

        private DbFilterExpression Filter(DbExpressionBinding input, DbExpression predicate)
        {
            DbFilterExpression to = this._commandTree.CreateFilterExpression(input, predicate);
            this.ApplySpanMapping(input.Expression, to);
            return to;
        }

        private EdmFunction FindCanonicalFunction(string functionName, IList<TypeUsage> argumentTypes, bool isGroupAggregateFunction, Expression linqExpression)
        {
            IList<EdmFunction> list;
            bool flag;
            if (!this._typeResolver.TryGetFunctionFromMetadata(functionName, "Edm", true, out list))
            {
                ThrowUnresolvableCanonicalFunction(linqExpression);
            }
            EdmFunction function = TypeResolver.ResolveFunctionOverloads(list, argumentTypes, isGroupAggregateFunction, out flag);
            if (flag || (function == null))
            {
                ThrowUnresolvableStoreFunction(linqExpression);
            }
            return function;
        }

        private TypeUsage GetCastTargetType(TypeUsage fromType, Type toClrType, Type fromClrType, bool preserveCastForDateTime)
        {
            TypeUsage usage;
            if (this.TryGetValueLayerType(toClrType, out usage) && CanOmitCast(fromType, usage, preserveCastForDateTime))
            {
                return null;
            }
            return ValidateAndAdjustCastTypes(usage, fromType, toClrType, fromClrType);
        }

        private TypeUsage GetIsOrAsTargetType(TypeUsage fromType, ExpressionType operationType, Type toClrType, Type fromClrType)
        {
            TypeUsage usage;
            if (!this.TryGetValueLayerType(toClrType, out usage) || (!TypeSemantics.IsEntityType(usage) && !TypeSemantics.IsComplexType(usage)))
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedIsOrAs(operationType, DescribeClrType(fromClrType), DescribeClrType(toClrType)));
            }
            return usage;
        }

        private Expression GetLambdaExpression(Expression argument)
        {
            ClosureBinding binding;
            TypeUsage usage;
            if (ExpressionType.Lambda == argument.NodeType)
            {
                return argument;
            }
            if (ExpressionType.Quote == argument.NodeType)
            {
                return this.GetLambdaExpression(((UnaryExpression) argument).Operand);
            }
            bool allowLambda = true;
            if (!ClosureBinding.TryCreateClosureBinding(argument, this._context.Perspective, allowLambda, this._closureCandidates, out binding, out usage) || (binding.Expression == null))
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnexpectedLinqLambdaExpressionFormat);
            }
            this.AddClosureBinding(binding);
            return this.GetLambdaExpression(binding.Expression);
        }

        private LambdaExpression GetLambdaExpression(MethodCallExpression callExpression, int argumentOrdinal)
        {
            Expression argument = callExpression.Arguments[argumentOrdinal];
            return (LambdaExpression) this.GetLambdaExpression(argument);
        }

        private static IEnumerable<Translator> GetTranslators()
        {
            yield return new AndAlsoTranslator();
            yield return new OrElseTranslator();
            yield return new LessThanTranslator();
            yield return new LessThanOrEqualsTranslator();
            yield return new GreaterThanTranslator();
            yield return new GreaterThanOrEqualsTranslator();
            yield return new EqualsTranslator();
            yield return new NotEqualsTranslator();
            yield return new ConvertTranslator();
            yield return new ConstantTranslator();
            yield return new NotTranslator();
            yield return new MemberAccessTranslator();
            yield return new ParameterTranslator();
            yield return new MemberInitTranslator();
            yield return new NewTranslator();
            yield return new AddTranslator();
            yield return new ConditionalTranslator();
            yield return new DivideTranslator();
            yield return new ModuloTranslator();
            yield return new SubtractTranslator();
            yield return new MultiplyTranslator();
            yield return new NegateTranslator();
            yield return new UnaryPlusTranslator();
            yield return new MethodCallTranslator();
            yield return new CoalesceTranslator();
            yield return new AsTranslator();
            yield return new IsTranslator();
            yield return new QuoteTranslator();
            yield return new AndTranslator();
            yield return new OrTranslator();
            yield return new ExclusiveOrTranslator();
            yield return new NotSupportedTranslator(new ExpressionType[] { ExpressionType.LeftShift, ExpressionType.RightShift, ExpressionType.ArrayLength, ExpressionType.ArrayIndex, ExpressionType.Invoke, ExpressionType.Lambda, ExpressionType.ListInit, ExpressionType.NewArrayInit, ExpressionType.NewArrayBounds, ExpressionType.Power });
        }

        private TypeUsage GetValueLayerType(Type linqType)
        {
            TypeUsage usage;
            if (!this.TryGetValueLayerType(linqType, out usage))
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedType(linqType));
            }
            return usage;
        }

        private DbExpression ImplementEquality(DbExpression left, DbExpression right, EqualsPattern pattern)
        {
            DbExpressionKind expressionKind = left.ExpressionKind;
            if (expressionKind != DbExpressionKind.Constant)
            {
                if (expressionKind == DbExpressionKind.Null)
                {
                    DbExpressionKind kind3 = right.ExpressionKind;
                    if (kind3 != DbExpressionKind.Constant)
                    {
                        if (kind3 == DbExpressionKind.Null)
                        {
                            return this._commandTree.CreateTrueExpression();
                        }
                        return this._commandTree.CreateIsNullExpression(right);
                    }
                    return this._commandTree.CreateFalseExpression();
                }
                DbExpressionKind kind4 = right.ExpressionKind;
                if (kind4 != DbExpressionKind.Constant)
                {
                    if (kind4 == DbExpressionKind.Null)
                    {
                        return this._commandTree.CreateIsNullExpression(left);
                    }
                    return this.ImplementEqualityUnknownArguments(left, right, pattern);
                }
                return this.ImplementEqualityConstantAndUnknown((DbConstantExpression) right, left, pattern);
            }
            DbExpressionKind kind2 = right.ExpressionKind;
            if (kind2 != DbExpressionKind.Constant)
            {
                if (kind2 == DbExpressionKind.Null)
                {
                    return this._commandTree.CreateFalseExpression();
                }
                return this.ImplementEqualityConstantAndUnknown((DbConstantExpression) left, right, pattern);
            }
            return this._commandTree.CreateEqualsExpression(left, right);
        }

        private DbExpression ImplementEqualityConstantAndUnknown(DbConstantExpression constant, DbExpression unknown, EqualsPattern pattern)
        {
            switch (pattern)
            {
                case EqualsPattern.Store:
                case EqualsPattern.PositiveNullEquality:
                    return this._commandTree.CreateEqualsExpression(constant, unknown);
            }
            return null;
        }

        private DbExpression ImplementEqualityUnknownArguments(DbExpression left, DbExpression right, EqualsPattern pattern)
        {
            switch (pattern)
            {
                case EqualsPattern.Store:
                    return this._commandTree.CreateEqualsExpression(left, right);

                case EqualsPattern.PositiveNullEquality:
                {
                    DbExpression expression = this._commandTree.CreateIsNullExpression(left);
                    DbExpression expression2 = this._commandTree.CreateIsNullExpression(right);
                    DbExpression expression3 = this._commandTree.CreateEqualsExpression(left, right);
                    return this._commandTree.CreateOrExpression(expression3, this._commandTree.CreateAndExpression(expression, expression2));
                }
            }
            return null;
        }

        private static Dictionary<ExpressionType, Translator> InitializeTranslators()
        {
            Dictionary<ExpressionType, Translator> dictionary = new Dictionary<ExpressionType, Translator>();
            foreach (Translator translator in GetTranslators())
            {
                foreach (ExpressionType type in translator.NodeTypes)
                {
                    dictionary.Add(type, translator);
                }
            }
            return dictionary;
        }

        private DbIntersectExpression Intersect(DbExpression left, DbExpression right)
        {
            DbIntersectExpression to = this._commandTree.CreateIntersectExpression(left, right);
            this.UnifySpanMappings(left, right, to);
            return to;
        }

        private static bool IsCSharpGeneratedClass(string typeName, string pattern) => 
            ((typeName.Contains("<>") && typeName.Contains("__")) && typeName.Contains(pattern));

        private bool IsQueryRoot(Expression linqExpression)
        {
            if (!object.ReferenceEquals(this._expression, linqExpression))
            {
                return object.ReferenceEquals(this._innerExpression, linqExpression);
            }
            return true;
        }

        private static bool IsVBGeneratedClass(string typeName, string pattern) => 
            ((typeName.Contains("_") && typeName.Contains("$")) && typeName.Contains(pattern));

        private DbLimitExpression Limit(DbExpression argument, DbExpression limit)
        {
            DbLimitExpression to = this._commandTree.CreateLimitExpression(argument, limit);
            this.ApplySpanMapping(argument, to);
            return to;
        }

        private Expression NormalizeExpression(Expression expression)
        {
            HashSet<Expression> candidates = new HashSet<Expression>();
            LinqMaximalSubtreeNominator.Nominate(expression, candidates, new Func<Expression, bool>(ExpressionEvaluator.IsExpressionNodeClientEvaluatable));
            expression = LinqTreeNodeEvaluator.Evaluate(expression, candidates);
            LinqMaximalSubtreeNominator.Nominate(expression, this._closureCandidates, delegate (Expression e) {
                if (!ExpressionEvaluator.IsExpressionNodeClientEvaluatable(e))
                {
                    return ExpressionEvaluator.IsExpressionNodeAClosure(e);
                }
                return true;
            });
            expression = new LinqExpressionNormalizer().Visit(expression);
            return expression;
        }

        private DbExpression NormalizeSetSource(DbExpression input)
        {
            InitializerMetadata metadata;
            if (InitializerMetadata.TryGetInitializerMetadata(input.ResultType, out metadata))
            {
                if (metadata.Kind == InitializerMetadataKind.Grouping)
                {
                    input = this._commandTree.CreatePropertyExpression("Group", input);
                    return input;
                }
                if (metadata.Kind == InitializerMetadataKind.EntityCollection)
                {
                    input = this._commandTree.CreatePropertyExpression("Elements", input);
                }
            }
            return input;
        }

        private DbOfTypeExpression OfType(DbExpression argument, TypeUsage ofType)
        {
            DbOfTypeExpression to = this._commandTree.CreateOfTypeExpression(argument, ofType);
            this.ApplySpanMapping(argument, to);
            return to;
        }

        private DbProjectExpression Project(DbExpressionBinding input, DbExpression projection)
        {
            DbProjectExpression to = this._commandTree.CreateProjectExpression(input, projection);
            if ((projection.ExpressionKind == DbExpressionKind.VariableReference) && ((DbVariableReferenceExpression) projection).VariableName.Equals(input.VariableName, StringComparison.Ordinal))
            {
                this.ApplySpanMapping(input.Expression, to);
            }
            return to;
        }

        private DbExpression RecursivelyRewriteEqualsExpression(DbExpression left, DbExpression right, EqualsPattern pattern)
        {
            RowType edmType = left.ResultType.EdmType as RowType;
            RowType item = left.ResultType.EdmType as RowType;
            if ((edmType == null) && (item == null))
            {
                return this.ImplementEquality(left, right, pattern);
            }
            if (((edmType == null) || (item == null)) || !edmType.EdmEquals(item))
            {
                return this._commandTree.CreateFalseExpression();
            }
            DbExpression expression = null;
            foreach (EdmProperty property in edmType.Properties)
            {
                DbPropertyExpression expression2 = this._commandTree.CreatePropertyExpression(property, left);
                DbPropertyExpression expression3 = this._commandTree.CreatePropertyExpression(property, right);
                DbExpression expression4 = this.RecursivelyRewriteEqualsExpression(expression2, expression3, pattern);
                if (expression == null)
                {
                    expression = expression4;
                }
                else
                {
                    expression = this._commandTree.CreateAndExpression(expression, expression4);
                }
            }
            return expression;
        }

        private DbSkipExpression Skip(DbExpressionBinding input, IList<DbSortClause> keys, DbExpression skipCount)
        {
            DbSkipExpression to = this._commandTree.CreateSkipExpression(input, keys, skipCount);
            this.ApplySpanMapping(input.Expression, to);
            return to;
        }

        private DbSortExpression Sort(DbExpressionBinding input, IList<DbSortClause> keys)
        {
            DbSortExpression to = this._commandTree.CreateSortExpression(input, keys);
            this.ApplySpanMapping(input.Expression, to);
            return to;
        }

        private static void ThrowUnresolvableCanonicalFunction(Expression linqExpression)
        {
            if (linqExpression.NodeType == ExpressionType.Call)
            {
                MethodInfo method = ((MethodCallExpression) linqExpression).Method;
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableCanonicalFunctionForMethod(method, method.DeclaringType));
            }
            if (linqExpression.NodeType == ExpressionType.MemberAccess)
            {
                string str;
                Type type;
                MemberInfo info2 = TypeSystem.PropertyOrField(((MemberExpression) linqExpression).Member, out str, out type);
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableCanonicalFunctionForMember(info2, info2.DeclaringType));
            }
            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableCanonicalFunctionForExpression(linqExpression.NodeType));
        }

        private static void ThrowUnresolvableStoreFunction(Expression linqExpression)
        {
            if (linqExpression.NodeType == ExpressionType.Call)
            {
                MethodInfo method = ((MethodCallExpression) linqExpression).Method;
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableStoreFunctionForMethod(method, method.DeclaringType));
            }
            if (linqExpression.NodeType == ExpressionType.MemberAccess)
            {
                string str;
                Type type;
                MemberInfo info2 = TypeSystem.PropertyOrField(((MemberExpression) linqExpression).Member, out str, out type);
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableStoreFunctionForMember(info2, info2.DeclaringType));
            }
            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnresolvableStoreFunctionForExpression(linqExpression.NodeType));
        }

        private DbExpression TranslateExpression(Expression linq)
        {
            DbExpression expression3;
            if (this._linqExpressionStack.Contains(linq))
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ELinq_CycleDetected);
            }
            this._linqExpressionStack.Push(linq);
            try
            {
                ClosureBinding binding;
                TypeUsage usage;
                DbExpression expression;
                ObjectQuery query;
                Translator translator;
                bool allowLambda = false;
                if (this._bindingContext.TryGetBoundExpression(linq, out expression))
                {
                    return expression;
                }
                if (ClosureBinding.TryCreateClosureBinding(linq, this._context.Perspective, allowLambda, this._closureCandidates, out binding, out usage))
                {
                    if (!this.IsCompiledQueryMode)
                    {
                        this.AddClosureBinding(binding);
                    }
                    if (binding.Parameter != null)
                    {
                        if (this.IsCompiledQueryMode)
                        {
                            ConstantExpression expression2 = Expression.Constant(binding.Parameter.Value, binding.Parameter.ParameterType);
                            return this.TranslateExpression(expression2);
                        }
                        this._commandTree.AddParameter(binding.Parameter.Name, usage);
                        return this._commandTree.CreateParameterReferenceExpression(binding.Parameter.Name);
                    }
                    return this.TranslateInlineQueryOfT(binding.Query);
                }
                if (ExpressionEvaluator.TryEvaluateRootQuery(this._bindingContext, linq, out query))
                {
                    return this.TranslateInlineQueryOfT(query);
                }
                if (!s_translators.TryGetValue(linq.NodeType, out translator))
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnknownLinqNodeType, -1, linq.NodeType.ToString());
                }
                expression3 = translator.Translate(this, linq);
            }
            finally
            {
                this._linqExpressionStack.Pop();
            }
            return expression3;
        }

        private DbExpression TranslateInlineQueryOfT(ObjectQuery inlineQuery)
        {
            Expression expression;
            if (!object.ReferenceEquals(this._context, inlineQuery.QueryState.ObjectContext))
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedDifferentContexts);
            }
            if (!this._mergeOption.HasValue && inlineQuery.QueryState.UserSpecifiedMergeOption.HasValue)
            {
                this._mergeOption = new MergeOption?(inlineQuery.QueryState.UserSpecifiedMergeOption.Value);
            }
            if (inlineQuery.QueryState.TryGetExpression(out expression))
            {
                expression = this.NormalizeExpression(expression);
                return this.TranslateExpression(expression);
            }
            ReplacerCallback callback = null;
            EntitySqlQueryState queryState = (EntitySqlQueryState) inlineQuery.QueryState;
            DbExpression expression2 = null;
            ObjectParameterCollection objectParameters = inlineQuery.QueryState.Parameters;
            if ((!this.IsCompiledQueryMode || (objectParameters == null)) || (objectParameters.Count == 0))
            {
                if (objectParameters != null)
                {
                    foreach (ObjectParameter parameter in (IEnumerable<ObjectParameter>) inlineQuery.QueryState.Parameters)
                    {
                        this.AddParameter(parameter.ShallowCopy());
                    }
                }
                expression2 = queryState.Parse(this._commandTree);
            }
            else
            {
                DbQueryCommandTree parseTree = new DbQueryCommandTree(this._commandTree.MetadataWorkspace, this._commandTree.DataSpace);
                parseTree.Query = queryState.Parse(parseTree);
                if (callback == null)
                {
                    callback = delegate (ExpressionReplacement replacement) {
                        if (replacement.Current.ExpressionKind == DbExpressionKind.ParameterReference)
                        {
                            DbParameterReferenceExpression current = (DbParameterReferenceExpression) replacement.Current;
                            if (objectParameters.Contains(current.ParameterName))
                            {
                                ObjectParameter parameter = objectParameters[current.ParameterName];
                                if (parameter.Value == null)
                                {
                                    replacement.Replacement = replacement.Current.CommandTree.CreateNullExpression(replacement.Current.ResultType);
                                }
                                else
                                {
                                    replacement.Replacement = replacement.Current.CommandTree.CreateConstantExpression(parameter.Value, replacement.Current.ResultType);
                                }
                            }
                        }
                    };
                }
                parseTree.Replace(callback);
                expression2 = this._commandTree.Import(parseTree.Query);
            }
            return this.AddSpanMapping(expression2, inlineQuery.QueryState.Span);
        }

        private DbFunctionExpression TranslateIntoCanonicalFunction(string functionName, Expression linqExpression, params Expression[] linqArguments)
        {
            DbExpression[] translatedArguments = new DbExpression[linqArguments.Length];
            for (int i = 0; i < linqArguments.Length; i++)
            {
                translatedArguments[i] = this.TranslateExpression(linqArguments[i]);
            }
            return this.CreateCanonicalFunction(functionName, linqExpression, translatedArguments);
        }

        private DbExpression TranslateLambda(LambdaExpression lambda, DbExpression input)
        {
            Binding binding = new Binding(lambda.Parameters[0], input);
            this._bindingContext.PushBindingScope(new Binding[] { binding });
            DbExpression expression = this.TranslateExpression(lambda.Body);
            this._bindingContext.PopBindingScope();
            return expression;
        }

        private DbExpression TranslateLambda(LambdaExpression lambda, DbExpression input, out DbExpressionBinding binding)
        {
            input = this.NormalizeSetSource(input);
            binding = this._commandTree.CreateExpressionBinding(input);
            DbVariableReferenceExpression variable = binding.Variable;
            this._variableNameToInputExpression.Add(variable.VariableName, input);
            return this.TranslateLambda(lambda, variable);
        }

        private DbExpression TranslateLambda(LambdaExpression lambda, DbExpression input, out DbGroupExpressionBinding binding)
        {
            input = this.NormalizeSetSource(input);
            binding = this._commandTree.CreateGroupExpressionBinding(input);
            DbVariableReferenceExpression variable = binding.Variable;
            return this.TranslateLambda(lambda, variable);
        }

        private DbExpression TranslateSet(Expression linq) => 
            this.NormalizeSetSource(this.TranslateExpression(linq));

        private bool TryGetSpan(DbExpression expression, out Span span)
        {
            if (this._spanMappings != null)
            {
                return this._spanMappings.TryGetValue(expression, out span);
            }
            span = null;
            return false;
        }

        private bool TryGetValueLayerType(Type linqType, out TypeUsage type)
        {
            TypeUsage usage;
            this._commandTree.MetadataWorkspace.LoadAssemblyForType(linqType, Assembly.GetCallingAssembly());
            Type nonNullableType = TypeSystem.GetNonNullableType(linqType);
            Type elementType = TypeSystem.GetElementType(nonNullableType);
            if ((elementType != nonNullableType) && this.TryGetValueLayerType(elementType, out usage))
            {
                type = TypeHelpers.CreateCollectionTypeUsage(usage);
                return true;
            }
            return this._context.Perspective.TryGetTypeByName(nonNullableType.FullName, false, out type);
        }

        private bool TryRewrite(DbExpression source, DbExpressionBinding sourceBinding, DbExpression lambda, out DbExpression result)
        {
            DbGroupByTemplate template;
            result = null;
            if (this._groupByDefaultToOptimizedTranslationMap.TryGetValue(source, out template))
            {
                DbExpression expression2;
                DbGroupByExpression input = this._commandTree.CreateGroupByExpression(template.Input, template.GroupKeys, template.Aggregates);
                DbExpressionBinding binding = this._commandTree.CreateExpressionBinding(input);
                if (GroupByExpressionRewriter.TryRewrite(this, lambda, sourceBinding.VariableName, binding.Variable, template, out expression2))
                {
                    result = this.Project(binding, expression2);
                    return true;
                }
            }
            return false;
        }

        private static bool TypeUsageEquals(TypeUsage left, TypeUsage right)
        {
            if (left.EdmType.EdmEquals(right.EdmType))
            {
                return true;
            }
            if ((BuiltInTypeKind.CollectionType == left.EdmType.BuiltInTypeKind) && (BuiltInTypeKind.CollectionType == right.EdmType.BuiltInTypeKind))
            {
                return TypeUsageEquals(((CollectionType) left.EdmType).TypeUsage, ((CollectionType) right.EdmType).TypeUsage);
            }
            return (((BuiltInTypeKind.PrimitiveType == left.EdmType.BuiltInTypeKind) && (BuiltInTypeKind.PrimitiveType == right.EdmType.BuiltInTypeKind)) && ((PrimitiveType) left.EdmType).ClrEquivalentType.Equals(((PrimitiveType) right.EdmType).ClrEquivalentType));
        }

        private void UnifySpanMappings(DbExpression left, DbExpression right, DbExpression to)
        {
            Span span = null;
            Span span2 = null;
            bool flag = this.TryGetSpan(left, out span);
            bool flag2 = this.TryGetSpan(right, out span2);
            if (flag || flag2)
            {
                this.AddSpanMapping(to, Span.CopyUnion(span, span2));
            }
        }

        private DbUnionAllExpression UnionAll(DbExpression left, DbExpression right)
        {
            DbUnionAllExpression to = this._commandTree.CreateUnionAllExpression(left, right);
            this.UnifySpanMappings(left, right, to);
            return to;
        }

        private static TypeUsage ValidateAndAdjustCastTypes(TypeUsage toType, TypeUsage fromType, Type toClrType, Type fromClrType)
        {
            if (((toType == null) || !TypeSemantics.IsPrimitiveType(toType)) || !TypeSemantics.IsPrimitiveType(fromType))
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedCast(DescribeClrType(fromClrType), DescribeClrType(toClrType)));
            }
            PrimitiveTypeKind primitiveTypeKind = ((PrimitiveType) fromType.EdmType).PrimitiveTypeKind;
            if (((PrimitiveType) toType.EdmType).PrimitiveTypeKind != PrimitiveTypeKind.Decimal)
            {
                return toType;
            }
            switch (primitiveTypeKind)
            {
                case PrimitiveTypeKind.SByte:
                case PrimitiveTypeKind.Int16:
                case PrimitiveTypeKind.Int32:
                case PrimitiveTypeKind.Int64:
                case PrimitiveTypeKind.Byte:
                    toType = TypeUsage.CreateDecimalTypeUsage((PrimitiveType) toType.EdmType, 0x13, 0);
                    return toType;
            }
            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedCastToDecimal);
        }

        internal void ValidateInitializerMetadata(InitializerMetadata metadata)
        {
            InitializerMetadata metadata2;
            if ((this._initializers != null) && this._initializers.TryGetValue(metadata.ClrType, out metadata2))
            {
                if (!metadata.Equals(metadata2))
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedHeterogeneousInitializers(DescribeClrType(metadata.ClrType)));
                }
            }
            else
            {
                if (this._initializers == null)
                {
                    this._initializers = new Dictionary<Type, InitializerMetadata>();
                }
                this._initializers.Add(metadata.ClrType, metadata);
            }
        }

        private static void VerifyRowTypeSupportedForComparison(Type clrType, RowType rowType, Stack<EdmMember> memberPath)
        {
            foreach (EdmMember member in rowType.Properties)
            {
                if (memberPath == null)
                {
                    memberPath = new Stack<EdmMember>();
                }
                memberPath.Push(member);
                VerifyTypeSupportedForComparison(clrType, member.TypeUsage, memberPath);
                memberPath.Pop();
            }
        }

        private static void VerifyTypeSupportedForComparison(Type clrType, TypeUsage edmType, Stack<EdmMember> memberPath)
        {
            BuiltInTypeKind builtInTypeKind = edmType.EdmType.BuiltInTypeKind;
            if ((builtInTypeKind != BuiltInTypeKind.EntityType) && (builtInTypeKind != BuiltInTypeKind.PrimitiveType))
            {
                InitializerMetadata metadata;
                if ((builtInTypeKind == BuiltInTypeKind.RowType) && ((!InitializerMetadata.TryGetInitializerMetadata(edmType, out metadata) || (metadata.Kind == InitializerMetadataKind.ProjectionInitializer)) || (metadata.Kind == InitializerMetadataKind.ProjectionNew)))
                {
                    VerifyRowTypeSupportedForComparison(clrType, (RowType) edmType.EdmType, memberPath);
                    return;
                }
            }
            else
            {
                return;
            }
            if (memberPath == null)
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedComparison(DescribeClrType(clrType), System.Data.Entity.Strings.ELinq_PrimitiveTypesSample));
            }
            StringBuilder builder = new StringBuilder();
            foreach (EdmMember member in memberPath)
            {
                builder.Append(System.Data.Entity.Strings.ELinq_UnsupportedRowMemberComparison(member.Name));
            }
            builder.Append(System.Data.Entity.Strings.ELinq_UnsupportedRowTypeComparison(DescribeClrType(clrType)));
            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedRowComparison(builder.ToString(), System.Data.Entity.Strings.ELinq_PrimitiveTypesSample));
        }

        internal List<ClosureBinding> ClosureBindings =>
            this._closureBindings;

        private System.Data.Metadata.Edm.EdmItemCollection EdmItemCollection =>
            ((System.Data.Metadata.Edm.EdmItemCollection) this._commandTree.MetadataWorkspace.GetItemCollection(DataSpace.CSpace, true));

        private bool IsCompiledQueryMode =>
            (this._bindingContext.ObjectContext != null);

        internal ObjectParameterCollection Parameters =>
            this._parameters;

        internal MergeOption? PropagatedMergeOption =>
            this._mergeOption;

        internal Span PropagatedSpan =>
            this._span;


        private sealed class AddTranslator : ExpressionConverter.BinaryTranslator
        {
            internal AddTranslator() : base(typeArray)
            {
                ExpressionType[] typeArray = new ExpressionType[2];
                typeArray[1] = ExpressionType.AddChecked;
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq)
            {
                if (TypeSemantics.IsPrimitiveType(left.ResultType, PrimitiveTypeKind.String) && TypeSemantics.IsPrimitiveType(right.ResultType, PrimitiveTypeKind.String))
                {
                    return parent.CreateCanonicalFunction("Concat", linq, new DbExpression[] { left, right });
                }
                return parent._commandTree.CreatePlusExpression(left, right);
            }
        }

        private sealed class AndAlsoTranslator : ExpressionConverter.BinaryTranslator
        {
            internal AndAlsoTranslator() : base(new ExpressionType[] { ExpressionType.AndAlso })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateAndExpression(left, right);
        }

        private sealed class AndTranslator : ExpressionConverter.BitwiseBinaryTranslator
        {
            internal AndTranslator() : base(ExpressionType.And, "BitwiseAnd")
            {
            }

            protected override DbExpression TranslateIntoLogicExpression(ExpressionConverter parent, BinaryExpression linq, DbExpression left, DbExpression right) => 
                parent._commandTree.CreateAndExpression(left, right);
        }

        private sealed class AsTranslator : ExpressionConverter.UnaryTranslator
        {
            internal AsTranslator() : base(new ExpressionType[] { ExpressionType.TypeAs })
            {
            }

            protected override DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand)
            {
                TypeUsage resultType = operand.ResultType;
                TypeUsage treatType = parent.GetIsOrAsTargetType(resultType, ExpressionType.TypeAs, unary.Type, unary.Operand.Type);
                return parent._commandTree.CreateTreatExpression(operand, treatType);
            }
        }

        private abstract class BinaryTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
        {
            protected BinaryTranslator(params ExpressionType[] nodeTypes) : base(nodeTypes)
            {
            }

            protected abstract DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq);
            protected override DbExpression TypedTranslate(ExpressionConverter parent, BinaryExpression linq) => 
                this.TranslateBinary(parent, parent.TranslateExpression(linq.Left), parent.TranslateExpression(linq.Right), linq);
        }

        private abstract class BitwiseBinaryTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
        {
            private readonly string _canonicalFunctionName;

            protected BitwiseBinaryTranslator(ExpressionType nodeType, string canonicalFunctionName) : base(new ExpressionType[] { nodeType })
            {
                this._canonicalFunctionName = canonicalFunctionName;
            }

            protected abstract DbExpression TranslateIntoLogicExpression(ExpressionConverter parent, BinaryExpression linq, DbExpression left, DbExpression right);
            protected override DbExpression TypedTranslate(ExpressionConverter parent, BinaryExpression linq)
            {
                DbExpression left = parent.TranslateExpression(linq.Left);
                DbExpression right = parent.TranslateExpression(linq.Right);
                if (TypeSemantics.IsBooleanType(left.ResultType))
                {
                    return this.TranslateIntoLogicExpression(parent, linq, left, right);
                }
                return parent.CreateCanonicalFunction(this._canonicalFunctionName, linq, new DbExpression[] { left, right });
            }
        }

        private sealed class CoalesceTranslator : ExpressionConverter.BinaryTranslator
        {
            internal CoalesceTranslator() : base(new ExpressionType[] { ExpressionType.Coalesce })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq)
            {
                DbExpression expression = parent.CreateIsNullExpression(left, linq.Left.Type);
                List<DbExpression> whenExpressions = new List<DbExpression>(1) {
                    expression
                };
                List<DbExpression> thenExpressions = new List<DbExpression>(1) {
                    right
                };
                return parent._commandTree.CreateCaseExpression(whenExpressions, thenExpressions, left);
            }
        }

        private sealed class ConditionalTranslator : ExpressionConverter.TypedTranslator<ConditionalExpression>
        {
            internal ConditionalTranslator() : base(new ExpressionType[] { ExpressionType.Conditional })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, ConditionalExpression linq)
            {
                List<DbExpression> whenExpressions = new List<DbExpression>(1) {
                    parent.TranslateExpression(linq.Test)
                };
                List<DbExpression> thenExpressions = new List<DbExpression>(1) {
                    parent.TranslateExpression(linq.IfTrue)
                };
                DbExpression elseExpression = parent.TranslateExpression(linq.IfFalse);
                return parent._commandTree.CreateCaseExpression(whenExpressions, thenExpressions, elseExpression);
            }
        }

        private sealed class ConstantTranslator : ExpressionConverter.TypedTranslator<ConstantExpression>
        {
            internal ConstantTranslator() : base(new ExpressionType[] { ExpressionType.Constant })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, ConstantExpression linq)
            {
                TypeUsage usage;
                ObjectQuery inlineQuery = linq.Value as ObjectQuery;
                if (inlineQuery != null)
                {
                    return parent.TranslateInlineQueryOfT(inlineQuery);
                }
                bool flag = null == linq.Value;
                bool flag2 = false;
                if (parent.TryGetValueLayerType(linq.Type, out usage) && ((usage.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType) || (flag && (usage.EdmType.BuiltInTypeKind == BuiltInTypeKind.EntityType))))
                {
                    flag2 = true;
                }
                if (!flag2)
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedConstant(ExpressionConverter.DescribeClrType(linq.Type), System.Data.Entity.Strings.ELinq_PrimitiveTypesSample));
                }
                if (flag)
                {
                    return parent._commandTree.CreateNullExpression(usage);
                }
                return parent._commandTree.CreateConstantExpression(linq.Value, usage);
            }
        }

        private sealed class ConvertTranslator : ExpressionConverter.UnaryTranslator
        {
            internal ConvertTranslator() : base(new ExpressionType[] { ExpressionType.Convert, ExpressionType.ConvertChecked })
            {
            }

            protected override DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand)
            {
                Type toClrType = unary.Type;
                Type type = unary.Operand.Type;
                DbExpression source = parent.TranslateExpression(unary.Operand);
                return parent.CreateCastExpression(source, toClrType, type);
            }
        }

        private class DbGroupByTemplate
        {
            private List<KeyValuePair<string, DbAggregate>> _aggregates;
            private List<KeyValuePair<string, DbExpression>> _groupKeys;
            private DbGroupExpressionBinding _input;

            public DbGroupByTemplate(DbGroupExpressionBinding input)
            {
                this._input = input;
                this._groupKeys = new List<KeyValuePair<string, DbExpression>>();
                this._aggregates = new List<KeyValuePair<string, DbAggregate>>();
            }

            public List<KeyValuePair<string, DbAggregate>> Aggregates =>
                this._aggregates;

            public List<KeyValuePair<string, DbExpression>> GroupKeys =>
                this._groupKeys;

            public DbGroupExpressionBinding Input =>
                this._input;
        }

        private sealed class DivideTranslator : ExpressionConverter.BinaryTranslator
        {
            internal DivideTranslator() : base(new ExpressionType[] { ExpressionType.Divide })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateDivideExpression(left, right);
        }

        private enum EqualsPattern
        {
            Store,
            PositiveNullEquality
        }

        private sealed class EqualsTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
        {
            internal EqualsTranslator() : base(new ExpressionType[] { ExpressionType.Equal })
            {
            }

            private static DbExpression CreateIsNullExpression(ExpressionConverter parent, Expression input)
            {
                input = UnwrapConvert(input);
                DbExpression operand = parent.TranslateExpression(input);
                return parent.CreateIsNullExpression(operand, input.Type);
            }

            private static bool LinqExpressionIsNullConstant(Expression expression)
            {
                expression = UnwrapConvert(expression);
                if (ExpressionType.Constant != expression.NodeType)
                {
                    return false;
                }
                ConstantExpression expression2 = (ConstantExpression) expression;
                return (null == expression2.Value);
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, BinaryExpression linq)
            {
                Expression left = linq.Left;
                Expression right = linq.Right;
                bool flag = LinqExpressionIsNullConstant(left);
                bool flag2 = LinqExpressionIsNullConstant(right);
                if (flag && flag2)
                {
                    return parent._commandTree.CreateTrueExpression();
                }
                if (flag)
                {
                    return CreateIsNullExpression(parent, right);
                }
                if (flag2)
                {
                    return CreateIsNullExpression(parent, left);
                }
                DbExpression expression3 = parent.TranslateExpression(left);
                DbExpression expression4 = parent.TranslateExpression(right);
                return parent.CreateEqualsExpression(expression3, expression4, ExpressionConverter.EqualsPattern.Store, left.Type, right.Type);
            }

            private static Expression UnwrapConvert(Expression input)
            {
                while (ExpressionType.Convert == input.NodeType)
                {
                    input = ((UnaryExpression) input).Operand;
                }
                return input;
            }
        }

        private sealed class ExclusiveOrTranslator : ExpressionConverter.BitwiseBinaryTranslator
        {
            internal ExclusiveOrTranslator() : base(ExpressionType.ExclusiveOr, "BitwiseXor")
            {
            }

            protected override DbExpression TranslateIntoLogicExpression(ExpressionConverter parent, BinaryExpression linq, DbExpression left, DbExpression right)
            {
                DbExpression expression = parent._commandTree.CreateAndExpression(left, parent._commandTree.CreateNotExpression(right));
                DbExpression expression2 = parent._commandTree.CreateAndExpression(parent._commandTree.CreateNotExpression(parent.TranslateExpression(linq.Left)), parent.TranslateExpression(linq.Right));
                return parent._commandTree.CreateOrExpression(expression, expression2);
            }
        }

        private sealed class GreaterThanOrEqualsTranslator : ExpressionConverter.BinaryTranslator
        {
            internal GreaterThanOrEqualsTranslator() : base(new ExpressionType[] { ExpressionType.GreaterThanOrEqual })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateGreaterThanOrEqualsExpression(left, right);
        }

        private sealed class GreaterThanTranslator : ExpressionConverter.BinaryTranslator
        {
            internal GreaterThanTranslator() : base(new ExpressionType[] { ExpressionType.GreaterThan })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateGreaterThanExpression(left, right);
        }

        private class GroupByExpressionRewriter : DbExpressionVisitor<DbExpression>
        {
            private Dictionary<DbExpression, KeyValuePair<ExpressionConverter.DbGroupByTemplate, string>> _aggregateDefaultTranslationToOptimizedTranslationInfoMap;
            private DbCommandTree _commandTree;
            private ExpressionConverter.DbGroupByTemplate _optimizedGroupByTemplate;
            private DbVariableReferenceExpression _optimizedSourceBindingVarRef;
            private string _sourceBindingVariableName;

            private GroupByExpressionRewriter(ExpressionConverter parent, DbExpression inputExpression, string sourceBindingVariableName, DbVariableReferenceExpression optimizedSourceBindingVarRef, ExpressionConverter.DbGroupByTemplate optimizedGroupByTemplate)
            {
                this._commandTree = parent._commandTree;
                this._aggregateDefaultTranslationToOptimizedTranslationInfoMap = parent._aggregateDefaultTranslationToOptimizedTranslationInfoMap;
                this._sourceBindingVariableName = sourceBindingVariableName;
                this._optimizedSourceBindingVarRef = optimizedSourceBindingVarRef;
                this._optimizedGroupByTemplate = optimizedGroupByTemplate;
            }

            internal static bool TryRewrite(ExpressionConverter parent, DbExpression inputExpression, string sourceBindingVariableName, DbVariableReferenceExpression optimizedSourceBindingVarRef, ExpressionConverter.DbGroupByTemplate optimizedGroupByTemplate, out DbExpression outputExpression)
            {
                outputExpression = new ExpressionConverter.GroupByExpressionRewriter(parent, inputExpression, sourceBindingVariableName, optimizedSourceBindingVarRef, optimizedGroupByTemplate).VisitExpr(inputExpression);
                return (outputExpression != null);
            }

            public override DbExpression Visit(DbAndExpression e)
            {
                DbExpression left = this.VisitExpr(e.Left);
                if (left == null)
                {
                    return null;
                }
                DbExpression right = this.VisitExpr(e.Right);
                if (right == null)
                {
                    return null;
                }
                if ((e.Left == left) && (e.Right == right))
                {
                    return e;
                }
                return this._commandTree.CreateAndExpression(left, right);
            }

            public override DbExpression Visit(DbApplyExpression e) => 
                null;

            public override DbExpression Visit(DbArithmeticExpression e)
            {
                IList<DbExpression> list = this.VisitExprList(e.Arguments);
                if (list != null)
                {
                    if (list == e.Arguments)
                    {
                        return e;
                    }
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
                }
                return null;
            }

            public override DbExpression Visit(DbCaseExpression e)
            {
                IList<DbExpression> whenExpressions = this.VisitExprList(e.When);
                if (whenExpressions == null)
                {
                    return null;
                }
                IList<DbExpression> thenExpressions = this.VisitExprList(e.Then);
                if (thenExpressions == null)
                {
                    return null;
                }
                DbExpression elseExpression = this.VisitExpr(e.Else);
                if (elseExpression == null)
                {
                    return null;
                }
                if (((whenExpressions == e.When) && (thenExpressions == e.Then)) && (elseExpression == e.Else))
                {
                    return e;
                }
                return this._commandTree.CreateCaseExpression(whenExpressions, thenExpressions, elseExpression);
            }

            public override DbExpression Visit(DbCastExpression e)
            {
                DbExpression argument = this.VisitExpr(e.Argument);
                if (argument == null)
                {
                    return null;
                }
                if (argument == e.Argument)
                {
                    return e;
                }
                return this._commandTree.CreateCastExpression(argument, e.ResultType);
            }

            public override DbExpression Visit(DbComparisonExpression e)
            {
                DbExpression left = this.VisitExpr(e.Left);
                if (left != null)
                {
                    DbExpression right = this.VisitExpr(e.Right);
                    if (right == null)
                    {
                        return null;
                    }
                    if ((e.Left == left) && (e.Right == right))
                    {
                        return e;
                    }
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
                }
                return null;
            }

            public override DbExpression Visit(DbConstantExpression e) => 
                e;

            public override DbExpression Visit(DbCrossJoinExpression e) => 
                null;

            public override DbExpression Visit(DbDerefExpression e) => 
                null;

            public override DbExpression Visit(DbDistinctExpression e) => 
                null;

            public override DbExpression Visit(DbElementExpression e)
            {
                KeyValuePair<ExpressionConverter.DbGroupByTemplate, string> pair;
                if (!this._aggregateDefaultTranslationToOptimizedTranslationInfoMap.TryGetValue(e, out pair))
                {
                    return null;
                }
                if (pair.Key != this._optimizedGroupByTemplate)
                {
                    return null;
                }
                return this._commandTree.CreatePropertyExpression(pair.Value, this._optimizedSourceBindingVarRef);
            }

            public override DbExpression Visit(DbEntityRefExpression e) => 
                null;

            public override DbExpression Visit(DbExceptExpression e) => 
                null;

            public override DbExpression Visit(DbExpression e)
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.Cqt_General_UnsupportedExpression(e.GetType().FullName));
            }

            public override DbExpression Visit(DbFilterExpression e) => 
                null;

            public override DbExpression Visit(DbFunctionExpression e) => 
                null;

            public override DbExpression Visit(DbGroupByExpression e) => 
                null;

            public override DbExpression Visit(DbIntersectExpression e) => 
                null;

            public override DbExpression Visit(DbIsEmptyExpression e) => 
                null;

            public override DbExpression Visit(DbIsNullExpression e)
            {
                DbExpression argument = this.VisitExpr(e.Argument);
                if (argument == null)
                {
                    return null;
                }
                if (argument == e.Argument)
                {
                    return e;
                }
                return this._commandTree.CreateIsNullExpression(argument);
            }

            public override DbExpression Visit(DbIsOfExpression e) => 
                null;

            public override DbExpression Visit(DbJoinExpression e) => 
                null;

            public override DbExpression Visit(DbLikeExpression e) => 
                null;

            public override DbExpression Visit(DbLimitExpression e) => 
                null;

            public override DbExpression Visit(DbNewInstanceExpression e)
            {
                IList<DbExpression> args = this.VisitExprList(e.Arguments);
                if (args == null)
                {
                    return null;
                }
                if (args == e.Arguments)
                {
                    return e;
                }
                return this._commandTree.CreateNewInstanceExpression(e.ResultType, args);
            }

            public override DbExpression Visit(DbNotExpression e)
            {
                DbExpression argument = this.VisitExpr(e.Argument);
                if (argument == null)
                {
                    return null;
                }
                if (argument == e.Argument)
                {
                    return e;
                }
                return this._commandTree.CreateNotExpression(argument);
            }

            public override DbExpression Visit(DbNullExpression e) => 
                e;

            public override DbExpression Visit(DbOfTypeExpression e) => 
                null;

            public override DbExpression Visit(DbOrExpression e)
            {
                DbExpression left = this.VisitExpr(e.Left);
                if (left == null)
                {
                    return null;
                }
                DbExpression right = this.VisitExpr(e.Right);
                if (right == null)
                {
                    return null;
                }
                if ((e.Left == left) && (e.Right == right))
                {
                    return e;
                }
                return this._commandTree.CreateOrExpression(left, right);
            }

            public override DbExpression Visit(DbParameterReferenceExpression e) => 
                e;

            public override DbExpression Visit(DbProjectExpression e) => 
                null;

            public override DbExpression Visit(DbPropertyExpression e)
            {
                if (e.Instance.ExpressionKind == DbExpressionKind.VariableReference)
                {
                    DbVariableReferenceExpression expression = (DbVariableReferenceExpression) e.Instance;
                    if (expression.VariableName != this._sourceBindingVariableName)
                    {
                        return null;
                    }
                    if (e.Property.Name != "Key")
                    {
                        return null;
                    }
                    return this._commandTree.CreatePropertyExpression("Key", this._optimizedSourceBindingVarRef);
                }
                DbExpression instance = this.VisitExpr(e.Instance);
                if (instance == null)
                {
                    return null;
                }
                return this._commandTree.CreatePropertyExpression(e.Property.Name, instance);
            }

            public override DbExpression Visit(DbQuantifierExpression e) => 
                null;

            public override DbExpression Visit(DbRefExpression e) => 
                null;

            public override DbExpression Visit(DbRefKeyExpression e) => 
                null;

            public override DbExpression Visit(DbRelationshipNavigationExpression e) => 
                null;

            public override DbExpression Visit(DbScanExpression e) => 
                null;

            public override DbExpression Visit(DbSkipExpression e) => 
                null;

            public override DbExpression Visit(DbSortExpression e) => 
                null;

            public override DbExpression Visit(DbTreatExpression e) => 
                null;

            public override DbExpression Visit(DbUnionAllExpression e) => 
                null;

            public override DbExpression Visit(DbVariableReferenceExpression e) => 
                null;

            public DbExpression VisitExpr(DbExpression expr) => 
                expr.Accept<DbExpression>(this);

            public IList<DbExpression> VisitExprList(IList<DbExpression> exprList) => 
                this.VisitList<DbExpression>(exprList);

            public IList<T> VisitList<T>(IList<T> exprList) where T: DbExpression
            {
                IList<T> list = new List<T>();
                bool flag = true;
                for (int i = 0; i < exprList.Count; i++)
                {
                    DbExpression expression = this.VisitExpr(exprList[i]);
                    if (expression == null)
                    {
                        return null;
                    }
                    if (expression != exprList[i])
                    {
                        flag = false;
                    }
                    list.Add((T) expression);
                }
                if (flag)
                {
                    list = exprList;
                }
                return list;
            }
        }

        private sealed class IsTranslator : ExpressionConverter.TypedTranslator<TypeBinaryExpression>
        {
            internal IsTranslator() : base(new ExpressionType[] { ExpressionType.TypeIs })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, TypeBinaryExpression linq)
            {
                DbExpression argument = parent.TranslateExpression(linq.Expression);
                TypeUsage resultType = argument.ResultType;
                TypeUsage type = parent.GetIsOrAsTargetType(resultType, ExpressionType.TypeIs, linq.TypeOperand, linq.Expression.Type);
                return parent._commandTree.CreateIsOfExpression(argument, type);
            }
        }

        private sealed class LessThanOrEqualsTranslator : ExpressionConverter.BinaryTranslator
        {
            internal LessThanOrEqualsTranslator() : base(new ExpressionType[] { ExpressionType.LessThanOrEqual })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateLessThanOrEqualsExpression(left, right);
        }

        private sealed class LessThanTranslator : ExpressionConverter.BinaryTranslator
        {
            internal LessThanTranslator() : base(new ExpressionType[] { ExpressionType.LessThan })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateLessThanExpression(left, right);
        }

        private sealed class MemberAccessTranslator : ExpressionConverter.TypedTranslator<MemberExpression>
        {
            private static readonly Dictionary<PropertyInfo, PropertyTranslator> s_propertyTranslators = new Dictionary<PropertyInfo, PropertyTranslator>();
            private static readonly object s_vbInitializerLock = new object();
            private static bool s_vbPropertiesInitialized;

            static MemberAccessTranslator()
            {
                foreach (PropertyTranslator translator in GetPropertyTranslators())
                {
                    foreach (PropertyInfo info in translator.Properties)
                    {
                        s_propertyTranslators.Add(info, translator);
                    }
                }
            }

            internal MemberAccessTranslator() : base(new ExpressionType[] { ExpressionType.MemberAccess })
            {
            }

            internal static bool CanTranslatePropertyInfo(PropertyInfo propertyInfo)
            {
                PropertyTranslator translator;
                return TryGetTranslator(propertyInfo, out translator);
            }

            private static IEnumerable<PropertyTranslator> GetPropertyTranslators()
            {
                yield return new DefaultCanonicalFunctionPropertyTranslator();
                yield return new RenameCanonicalFunctionPropertyTranslator();
                yield return new EntityCollectionCountTranslator();
                yield return new NullableHasValueTranslator();
                yield return new NullableValueTranslator();
            }

            private static IEnumerable<PropertyTranslator> GetVisualBasicPropertyTranslators(Assembly vbAssembly)
            {
                yield return new VBDateAndTimeNowTranslator(vbAssembly);
            }

            private static void InitializeVBProperties(Assembly vbAssembly)
            {
                foreach (PropertyTranslator translator in GetVisualBasicPropertyTranslators(vbAssembly))
                {
                    foreach (PropertyInfo info in translator.Properties)
                    {
                        s_propertyTranslators.Add(info, translator);
                    }
                }
            }

            private static DbExpression NormalizeInstanceForReference(ExpressionConverter parent, DbExpression instance, EdmMember member)
            {
                EdmType edmType = instance.ResultType.EdmType;
                if (edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                {
                    EntityType type2 = (EntityType) edmType;
                    if (type2.KeyMembers.Contains(member) && (instance.ExpressionKind == DbExpressionKind.Property))
                    {
                        DbPropertyExpression expression = (DbPropertyExpression) instance;
                        if (expression.Property.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
                        {
                            NavigationProperty property = (NavigationProperty) expression.Property;
                            DbExpression from = parent._commandTree.CreateEntityRefExpression(expression.Instance);
                            DbExpression reference = parent._commandTree.CreateRelationshipNavigationExpression(property.FromEndMember, property.ToEndMember, from);
                            instance = parent._commandTree.CreateRefKeyExpression(reference);
                        }
                    }
                }
                return instance;
            }

            private static DbExpression TranslateNavigationProperty(ExpressionConverter parent, MemberInfo clrMember, DbExpression instance, NavigationProperty navProp)
            {
                DbExpression expression = parent._commandTree.CreatePropertyExpression(navProp, instance);
                if (BuiltInTypeKind.CollectionType == expression.ResultType.EdmType.BuiltInTypeKind)
                {
                    List<KeyValuePair<string, DbExpression>> columns = new List<KeyValuePair<string, DbExpression>>(2) {
                        new KeyValuePair<string, DbExpression>("Owner", instance),
                        new KeyValuePair<string, DbExpression>("Elements", expression)
                    };
                    expression = parent.CreateNewRowExpression(columns, InitializerMetadata.CreateEntityCollectionInitializer(parent.EdmItemCollection, ((PropertyInfo) clrMember).PropertyType, navProp));
                }
                return expression;
            }

            private static bool TryGetTranslator(PropertyInfo propertyInfo, out PropertyTranslator propertyTranslator)
            {
                if (propertyInfo.DeclaringType.IsGenericType)
                {
                    try
                    {
                        propertyInfo = propertyInfo.DeclaringType.GetGenericTypeDefinition().GetProperty(propertyInfo.Name, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    }
                    catch (AmbiguousMatchException)
                    {
                        propertyTranslator = null;
                        return false;
                    }
                    if (propertyInfo == null)
                    {
                        propertyTranslator = null;
                        return false;
                    }
                }
                if (s_propertyTranslators.TryGetValue(propertyInfo, out propertyTranslator))
                {
                    return true;
                }
                if ("Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" == propertyInfo.DeclaringType.Assembly.FullName)
                {
                    lock (s_vbInitializerLock)
                    {
                        if (!s_vbPropertiesInitialized)
                        {
                            InitializeVBProperties(propertyInfo.DeclaringType.Assembly);
                            s_vbPropertiesInitialized = true;
                        }
                        return s_propertyTranslators.TryGetValue(propertyInfo, out propertyTranslator);
                    }
                }
                propertyTranslator = null;
                return false;
            }

            private static bool TryResolveAsProperty(ExpressionConverter parent, MemberInfo clrMember, TypeUsage definingType, DbExpression instance, out DbExpression propertyExpression)
            {
                RowType edmType = definingType.EdmType as RowType;
                string name = clrMember.Name;
                if (edmType != null)
                {
                    EdmMember member;
                    if (edmType.Members.TryGetValue(name, false, out member))
                    {
                        propertyExpression = parent._commandTree.CreatePropertyExpression(name, instance);
                        return true;
                    }
                    propertyExpression = null;
                    return false;
                }
                StructuralType type = definingType.EdmType as StructuralType;
                if (type != null)
                {
                    EdmMember outMember = null;
                    if (parent._typeResolver.Perspective.TryGetMember(type, name, false, out outMember) && (outMember != null))
                    {
                        if (outMember.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty)
                        {
                            NavigationProperty navProp = (NavigationProperty) outMember;
                            propertyExpression = TranslateNavigationProperty(parent, clrMember, instance, navProp);
                            return true;
                        }
                        instance = NormalizeInstanceForReference(parent, instance, outMember);
                        propertyExpression = parent._commandTree.CreatePropertyExpression(name, instance);
                        return true;
                    }
                }
                if ((name == "Key") && (DbExpressionKind.Property == instance.ExpressionKind))
                {
                    InitializerMetadata metadata;
                    DbPropertyExpression expression = (DbPropertyExpression) instance;
                    if (((expression.Property.Name == "Group") && InitializerMetadata.TryGetInitializerMetadata(expression.Instance.ResultType, out metadata)) && (metadata.Kind == InitializerMetadataKind.Grouping))
                    {
                        propertyExpression = parent._commandTree.CreatePropertyExpression("Key", expression.Instance);
                        return true;
                    }
                }
                propertyExpression = null;
                return false;
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, MemberExpression linq)
            {
                string str;
                Type type;
                PropertyTranslator translator;
                MemberInfo clrMember = TypeSystem.PropertyOrField(linq.Member, out str, out type);
                if (linq.Expression != null)
                {
                    DbExpression expression;
                    DbExpression instance = parent.TranslateExpression(linq.Expression);
                    if (TryResolveAsProperty(parent, clrMember, instance.ResultType, instance, out expression))
                    {
                        return expression;
                    }
                }
                if ((clrMember.MemberType != MemberTypes.Property) || !TryGetTranslator((PropertyInfo) clrMember, out translator))
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnrecognizedMember(linq.Member.Name));
                }
                return translator.Translate(parent, linq);
            }



            private sealed class DefaultCanonicalFunctionPropertyTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                internal DefaultCanonicalFunctionPropertyTranslator() : base(GetProperties())
                {
                }

                private static IEnumerable<PropertyInfo> GetProperties()
                {
                    yield return typeof(string).GetProperty("Length", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Year", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Month", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Day", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Hour", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Minute", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Second", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTime).GetProperty("Millisecond", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Year", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Month", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Day", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Hour", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Minute", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Second", BindingFlags.Public | BindingFlags.Instance);
                    yield return typeof(DateTimeOffset).GetProperty("Millisecond", BindingFlags.Public | BindingFlags.Instance);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call) => 
                    parent.TranslateIntoCanonicalFunction(call.Member.Name, call, new Expression[] { call.Expression });

            }

            private sealed class EntityCollectionCountTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                internal EntityCollectionCountTranslator() : base(new PropertyInfo[] { GetProperty() })
                {
                }

                private static PropertyInfo GetProperty() => 
                    typeof(EntityCollection<>).GetProperty("Count", BindingFlags.Public | BindingFlags.Instance);

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call)
                {
                    MethodInfo info;
                    ReflectionUtil.TryLookupMethod(SequenceMethod.Count, out info);
                    Expression linq = Expression.Call(info.MakeGenericMethod(call.Member.DeclaringType.GetGenericArguments()), new Expression[] { call.Expression });
                    return parent.TranslateExpression(linq);
                }
            }

            private sealed class NullableHasValueTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                internal NullableHasValueTranslator() : base(new PropertyInfo[] { GetProperty() })
                {
                }

                private static PropertyInfo GetProperty() => 
                    typeof(Nullable<>).GetProperty("HasValue", BindingFlags.Public | BindingFlags.Instance);

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call)
                {
                    DbExpression operand = parent.TranslateExpression(call.Expression);
                    return parent._commandTree.CreateNotExpression(parent.CreateIsNullExpression(operand, call.Expression.Type));
                }
            }

            private sealed class NullableValueTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                internal NullableValueTranslator() : base(new PropertyInfo[] { GetProperty() })
                {
                }

                private static PropertyInfo GetProperty() => 
                    typeof(Nullable<>).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call) => 
                    parent.TranslateExpression(call.Expression);
            }

            private abstract class PropertyTranslator
            {
                private readonly IEnumerable<PropertyInfo> _properties;

                protected PropertyTranslator(IEnumerable<PropertyInfo> properties)
                {
                    this._properties = properties;
                }

                protected PropertyTranslator(params PropertyInfo[] properties)
                {
                    this._properties = properties;
                }

                public override string ToString() => 
                    base.GetType().Name;

                internal abstract DbExpression Translate(ExpressionConverter parent, MemberExpression call);

                internal IEnumerable<PropertyInfo> Properties =>
                    this._properties;
            }

            private sealed class RenameCanonicalFunctionPropertyTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                private static readonly Dictionary<PropertyInfo, string> s_propertyRenameMap = new Dictionary<PropertyInfo, string>(2);

                internal RenameCanonicalFunctionPropertyTranslator() : base(GetProperties())
                {
                }

                private static IEnumerable<PropertyInfo> GetProperties()
                {
                    yield return GetProperty(typeof(DateTime), "Now", BindingFlags.Public | BindingFlags.Static, "CurrentDateTime");
                    yield return GetProperty(typeof(DateTime), "UtcNow", BindingFlags.Public | BindingFlags.Static, "CurrentUtcDateTime");
                    yield return GetProperty(typeof(DateTimeOffset), "Now", BindingFlags.Public | BindingFlags.Static, "CurrentDateTimeOffset");
                    yield return GetProperty(typeof(TimeSpan), "Hours", BindingFlags.Public | BindingFlags.Instance, "Hour");
                    yield return GetProperty(typeof(TimeSpan), "Minutes", BindingFlags.Public | BindingFlags.Instance, "Minute");
                    yield return GetProperty(typeof(TimeSpan), "Seconds", BindingFlags.Public | BindingFlags.Instance, "Second");
                    yield return GetProperty(typeof(TimeSpan), "Milliseconds", BindingFlags.Public | BindingFlags.Instance, "Millisecond");
                }

                private static PropertyInfo GetProperty(Type declaringType, string propertyName, BindingFlags bindingFlages, string canonicalFunctionName)
                {
                    PropertyInfo property = declaringType.GetProperty(propertyName, bindingFlages);
                    s_propertyRenameMap.Add(property, canonicalFunctionName);
                    return property;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call)
                {
                    PropertyInfo member = (PropertyInfo) call.Member;
                    string functionName = s_propertyRenameMap[member];
                    if (call.Expression == null)
                    {
                        return parent.TranslateIntoCanonicalFunction(functionName, call, new Expression[0]);
                    }
                    return parent.TranslateIntoCanonicalFunction(functionName, call, new Expression[] { call.Expression });
                }

            }

            private sealed class VBDateAndTimeNowTranslator : ExpressionConverter.MemberAccessTranslator.PropertyTranslator
            {
                private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";

                internal VBDateAndTimeNowTranslator(Assembly vbAssembly) : base(new PropertyInfo[] { GetProperty(vbAssembly) })
                {
                }

                private static PropertyInfo GetProperty(Assembly vbAssembly) => 
                    vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime").GetProperty("Now", BindingFlags.Public | BindingFlags.Static);

                internal override DbExpression Translate(ExpressionConverter parent, MemberExpression call) => 
                    parent.TranslateIntoCanonicalFunction("CurrentDateTime", call, new Expression[0]);
            }
        }

        private sealed class MemberInitTranslator : ExpressionConverter.TypedTranslator<MemberInitExpression>
        {
            internal MemberInitTranslator() : base(new ExpressionType[] { ExpressionType.MemberInit })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, MemberInitExpression linq)
            {
                InitializerMetadata metadata;
                if ((linq.NewExpression.Constructor == null) || (linq.NewExpression.Constructor.GetParameters().Length != 0))
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedConstructor);
                }
                parent.CheckInitializerType(linq.Type);
                List<KeyValuePair<string, DbExpression>> columns = new List<KeyValuePair<string, DbExpression>>(linq.Bindings.Count + 1);
                MemberInfo[] members = new MemberInfo[linq.Bindings.Count];
                HashSet<string> set = new HashSet<string>(StringComparer.Ordinal);
                for (int i = 0; i < linq.Bindings.Count; i++)
                {
                    string str;
                    Type type;
                    MemberAssignment assignment = linq.Bindings[i] as MemberAssignment;
                    if (assignment == null)
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedBinding);
                    }
                    MemberInfo info = TypeSystem.PropertyOrField(assignment.Member, out str, out type);
                    DbExpression expression = parent.TranslateExpression(assignment.Expression);
                    set.Add(str);
                    members[i] = info;
                    columns.Add(new KeyValuePair<string, DbExpression>(str, expression));
                }
                if (columns.Count == 0)
                {
                    columns.Add(new KeyValuePair<string, DbExpression>("Key", parent._commandTree.CreateTrueExpression()));
                    metadata = InitializerMetadata.CreateEmptyProjectionInitializer(parent.EdmItemCollection, linq.NewExpression);
                }
                else
                {
                    metadata = InitializerMetadata.CreateProjectionInitializer(parent.EdmItemCollection, linq, members);
                }
                parent.ValidateInitializerMetadata(metadata);
                return parent.CreateNewRowExpression(columns, metadata);
            }
        }

        private sealed class MethodCallTranslator : ExpressionConverter.TypedTranslator<MethodCallExpression>
        {
            private static readonly CallTranslator s_defaultTranslator = new DefaultTranslator();
            private static readonly Dictionary<MethodInfo, CallTranslator> s_methodTranslators = InitializeMethodTranslators();
            private static readonly Dictionary<string, ObjectQueryCallTranslator> s_objectQueryTranslators = InitializeObjectQueryTranslators();
            private static readonly Dictionary<SequenceMethod, SequenceMethodTranslator> s_sequenceTranslators = InitializeSequenceMethodTranslators();
            private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";
            private static readonly object s_vbInitializerLock = new object();
            private static bool s_vbMethodsInitialized;

            internal MethodCallTranslator() : base(new ExpressionType[] { ExpressionType.Call })
            {
            }

            private static IEnumerable<CallTranslator> GetCallTranslators()
            {
                yield return new CanonicalFunctionDefaultTranslator();
                yield return new ContainsTranslator();
                yield return new StartsWithTranslator();
                yield return new EndsWithTranslator();
                yield return new IndexOfTranslator();
                yield return new SubstringTranslator();
                yield return new RemoveTranslator();
                yield return new InsertTranslator();
                yield return new IsNullOrEmptyTranslator();
                yield return new StringConcatTranslator();
                yield return new TrimStartTranslator();
                yield return new TrimEndTranslator();
            }

            private static IEnumerable<ObjectQueryCallTranslator> GetObjectQueryCallTranslators()
            {
                yield return new ObjectQueryBuilderDistinctTranslator();
                yield return new ObjectQueryBuilderExceptTranslator();
                yield return new ObjectQueryBuilderFirstTranslator();
                yield return new ObjectQueryIncludeTranslator();
                yield return new ObjectQueryBuilderIntersectTranslator();
                yield return new ObjectQueryBuilderOfTypeTranslator();
                yield return new ObjectQueryBuilderUnionTranslator();
            }

            private static IEnumerable<SequenceMethodTranslator> GetSequenceMethodTranslators()
            {
                yield return new ConcatTranslator();
                yield return new UnionTranslator();
                yield return new IntersectTranslator();
                yield return new ExceptTranslator();
                yield return new DistinctTranslator();
                yield return new WhereTranslator();
                yield return new SelectTranslator();
                yield return new OrderByTranslator();
                yield return new OrderByDescendingTranslator();
                yield return new ThenByTranslator();
                yield return new ThenByDescendingTranslator();
                yield return new SelectManyTranslator();
                yield return new AnyTranslator();
                yield return new AnyPredicateTranslator();
                yield return new AllTranslator();
                yield return new JoinTranslator();
                yield return new GroupByTranslator();
                yield return new MaxTranslator();
                yield return new MinTranslator();
                yield return new AverageTranslator();
                yield return new SumTranslator();
                yield return new CountTranslator();
                yield return new LongCountTranslator();
                yield return new CastMethodTranslator();
                yield return new GroupJoinTranslator();
                yield return new OfTypeTranslator();
                yield return new SingleTranslatorNotSupported();
                yield return new PassthroughTranslator();
                yield return new FirstTranslator();
                yield return new FirstPredicateTranslator();
                yield return new FirstOrDefaultTranslator();
                yield return new FirstOrDefaultPredicateTranslator();
                yield return new TakeTranslator();
                yield return new SkipTranslator();
            }

            private static IEnumerable<CallTranslator> GetVisualBasicCallTranslators(Assembly vbAssembly)
            {
                yield return new VBCanonicalFunctionDefaultTranslator(vbAssembly);
                yield return new VBCanonicalFunctionRenameTranslator(vbAssembly);
                yield return new VBDatePartTranslator(vbAssembly);
            }

            private static Dictionary<MethodInfo, CallTranslator> InitializeMethodTranslators()
            {
                Dictionary<MethodInfo, CallTranslator> dictionary = new Dictionary<MethodInfo, CallTranslator>();
                foreach (CallTranslator translator in GetCallTranslators())
                {
                    foreach (MethodInfo info in translator.Methods)
                    {
                        dictionary.Add(info, translator);
                    }
                }
                return dictionary;
            }

            private static Dictionary<string, ObjectQueryCallTranslator> InitializeObjectQueryTranslators()
            {
                Dictionary<string, ObjectQueryCallTranslator> dictionary = new Dictionary<string, ObjectQueryCallTranslator>(StringComparer.Ordinal);
                foreach (ObjectQueryCallTranslator translator in GetObjectQueryCallTranslators())
                {
                    dictionary[translator.MethodName] = translator;
                }
                return dictionary;
            }

            private static Dictionary<SequenceMethod, SequenceMethodTranslator> InitializeSequenceMethodTranslators()
            {
                Dictionary<SequenceMethod, SequenceMethodTranslator> dictionary = new Dictionary<SequenceMethod, SequenceMethodTranslator>();
                foreach (SequenceMethodTranslator translator in GetSequenceMethodTranslators())
                {
                    foreach (SequenceMethod method in translator.Methods)
                    {
                        dictionary.Add(method, translator);
                    }
                }
                return dictionary;
            }

            private static void InitializeVBMethods(Assembly vbAssembly)
            {
                foreach (CallTranslator translator in GetVisualBasicCallTranslators(vbAssembly))
                {
                    foreach (MethodInfo info in translator.Methods)
                    {
                        s_methodTranslators.Add(info, translator);
                    }
                }
            }

            private static bool TryGetCallTranslator(MethodInfo methodInfo, out CallTranslator callTranslator)
            {
                if (s_methodTranslators.TryGetValue(methodInfo, out callTranslator))
                {
                    return true;
                }
                if ("Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" == methodInfo.DeclaringType.Assembly.FullName)
                {
                    lock (s_vbInitializerLock)
                    {
                        if (!s_vbMethodsInitialized)
                        {
                            InitializeVBMethods(methodInfo.DeclaringType.Assembly);
                            s_vbMethodsInitialized = true;
                        }
                        return s_methodTranslators.TryGetValue(methodInfo, out callTranslator);
                    }
                }
                callTranslator = null;
                return false;
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, MethodCallExpression linq)
            {
                SequenceMethod method;
                SequenceMethodTranslator translator;
                CallTranslator translator2;
                ObjectQueryCallTranslator translator3;
                if (ReflectionUtil.TryIdentifySequenceMethod(linq.Method, out method) && s_sequenceTranslators.TryGetValue(method, out translator))
                {
                    return translator.Translate(parent, linq, method);
                }
                if (TryGetCallTranslator(linq.Method, out translator2))
                {
                    return translator2.Translate(parent, linq);
                }
                Type declaringType = linq.Method.DeclaringType;
                if (((linq.Method.IsPublic && (declaringType != null)) && (declaringType.IsGenericType && (typeof(ObjectQuery<>) == declaringType.GetGenericTypeDefinition()))) && s_objectQueryTranslators.TryGetValue(linq.Method.Name, out translator3))
                {
                    return translator3.Translate(parent, linq);
                }
                return s_defaultTranslator.Translate(parent, linq);
            }





            private abstract class AggregateTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                private readonly string _functionName;
                private readonly bool _takesPredicate;

                protected AggregateTranslator(string functionName, bool takesPredicate, params SequenceMethod[] methods) : base(methods)
                {
                    this._takesPredicate = takesPredicate;
                    this._functionName = functionName;
                }

                protected virtual EdmFunction FindFunction(ExpressionConverter parent, MethodCallExpression call, TypeUsage argumentType)
                {
                    List<TypeUsage> argumentTypes = new List<TypeUsage>(1) {
                        argumentType
                    };
                    return parent.FindCanonicalFunction(this._functionName, argumentTypes, true, call);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    bool flag = 1 == call.Arguments.Count;
                    DbExpression input = parent.TranslateSet(call.Arguments[0]);
                    TypeUsage valueLayerType = parent.GetValueLayerType(call.Type);
                    LambdaExpression lambda = null;
                    DbExpression operand = input;
                    if (!flag)
                    {
                        DbExpressionBinding binding;
                        lambda = parent.GetLambdaExpression(call, 1);
                        DbExpression predicate = parent.TranslateLambda(lambda, input, out binding);
                        if (this._takesPredicate)
                        {
                            input = parent.Filter(binding, predicate);
                        }
                        else
                        {
                            input = parent._commandTree.CreateProjectExpression(binding, predicate);
                        }
                    }
                    input = this.WrapCollectionOperand(parent, input, valueLayerType);
                    DbGroupExpressionBinding binding2 = parent._commandTree.CreateGroupExpressionBinding(input);
                    EdmFunction function = this.FindFunction(parent, call, valueLayerType);
                    List<KeyValuePair<string, DbExpression>> keys = new List<KeyValuePair<string, DbExpression>>(0);
                    List<KeyValuePair<string, DbAggregate>> aggregates = new List<KeyValuePair<string, DbAggregate>>(1) {
                        new KeyValuePair<string, DbAggregate>("Aggregate", parent._commandTree.CreateFunctionAggregate(function, binding2.GroupVariable))
                    };
                    DbGroupByExpression expression5 = parent._commandTree.CreateGroupByExpression(binding2, keys, aggregates);
                    DbExpressionBinding binding3 = parent._commandTree.CreateExpressionBinding(expression5);
                    DbPropertyExpression cqt = parent._commandTree.CreatePropertyExpression("Aggregate", binding3.Variable);
                    DbProjectExpression argument = parent._commandTree.CreateProjectExpression(binding3, parent.AlignTypes(cqt, call.Type));
                    DbElementExpression originalTranslation = parent._commandTree.CreateElementExpression(argument);
                    this.TryCreateOptimizedTranslation(parent, lambda, operand, function, originalTranslation);
                    return originalTranslation;
                }

                private void TryCreateOptimizedTranslation(ExpressionConverter parent, LambdaExpression lambda, DbExpression operand, EdmFunction function, DbExpression originalTranslation)
                {
                    if ((!this._takesPredicate || (lambda == null)) && (operand.ExpressionKind == DbExpressionKind.Property))
                    {
                        DbPropertyExpression expression = (DbPropertyExpression) operand;
                        if (expression.Instance.ExpressionKind == DbExpressionKind.VariableReference)
                        {
                            DbExpression expression3;
                            ExpressionConverter.DbGroupByTemplate template;
                            DbVariableReferenceExpression instance = (DbVariableReferenceExpression) expression.Instance;
                            if (parent._variableNameToInputExpression.TryGetValue(instance.VariableName, out expression3) && parent._groupByDefaultToOptimizedTranslationMap.TryGetValue(expression3, out template))
                            {
                                TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(function.Parameters[0].TypeUsage);
                                DbExpression groupVariable = template.Input.GroupVariable;
                                if (lambda != null)
                                {
                                    groupVariable = parent.TranslateLambda(lambda, groupVariable);
                                }
                                string key = string.Format(CultureInfo.InvariantCulture, "Aggregate{0}", new object[] { template.Aggregates.Count });
                                template.Aggregates.Add(new KeyValuePair<string, DbAggregate>(key, parent._commandTree.CreateFunctionAggregate(function, this.WrapNonCollectionOperand(parent, groupVariable, elementTypeUsage))));
                                parent._aggregateDefaultTranslationToOptimizedTranslationInfoMap.Add(originalTranslation, new KeyValuePair<ExpressionConverter.DbGroupByTemplate, string>(template, key));
                            }
                        }
                    }
                }

                protected virtual DbExpression WrapCollectionOperand(ExpressionConverter parent, DbExpression operand, TypeUsage returnType)
                {
                    if (!ExpressionConverter.TypeUsageEquals(returnType, ((CollectionType) operand.ResultType.EdmType).TypeUsage))
                    {
                        DbExpressionBinding input = parent._commandTree.CreateExpressionBinding(operand);
                        operand = parent._commandTree.CreateProjectExpression(input, parent._commandTree.CreateCastExpression(input.Variable, returnType));
                    }
                    return operand;
                }

                protected virtual DbExpression WrapNonCollectionOperand(ExpressionConverter parent, DbExpression operand, TypeUsage returnType)
                {
                    if (!ExpressionConverter.TypeUsageEquals(returnType, operand.ResultType))
                    {
                        operand = parent._commandTree.CreateCastExpression(operand, returnType);
                    }
                    return operand;
                }
            }

            private sealed class AllTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                internal AllTranslator() : base(new SequenceMethod[] { SequenceMethod.All })
                {
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda) => 
                    parent._commandTree.CreateAllExpression(sourceBinding, lambda);
            }

            private sealed class AnyPredicateTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                internal AnyPredicateTranslator() : base(new SequenceMethod[] { SequenceMethod.AnyPredicate })
                {
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda) => 
                    parent._commandTree.CreateAnyExpression(sourceBinding, lambda);
            }

            private sealed class AnyTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                internal AnyTranslator() : base(new SequenceMethod[] { SequenceMethod.Any })
                {
                }

                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call) => 
                    parent._commandTree.CreateNotExpression(parent._commandTree.CreateIsEmptyExpression(operand));
            }

            private sealed class AverageTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
            {
                internal AverageTranslator() : base("AVG", false, new SequenceMethod[] { 
                    SequenceMethod.AverageDecimal, SequenceMethod.AverageDecimalSelector, SequenceMethod.AverageDouble, SequenceMethod.AverageDoubleSelector, SequenceMethod.AverageInt, SequenceMethod.AverageIntSelector, SequenceMethod.AverageLong, SequenceMethod.AverageLongSelector, SequenceMethod.AverageSingle, SequenceMethod.AverageSingleSelector, SequenceMethod.AverageNullableDecimal, SequenceMethod.AverageNullableDecimalSelector, SequenceMethod.AverageNullableDouble, SequenceMethod.AverageNullableDoubleSelector, SequenceMethod.AverageNullableInt, SequenceMethod.AverageNullableIntSelector,
                    SequenceMethod.AverageNullableLong, SequenceMethod.AverageNullableLongSelector, SequenceMethod.AverageNullableSingle, SequenceMethod.AverageNullableSingleSelector
                })
                {
                }
            }

            private abstract class BinarySequenceMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                protected BinarySequenceMethodTranslator(params SequenceMethod[] methods) : base(methods)
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    if (call.Object != null)
                    {
                        DbExpression expression = parent.TranslateSet(call.Object);
                        DbExpression expression2 = parent.TranslateSet(call.Arguments[0]);
                        return this.TranslateBinary(parent, expression, expression2);
                    }
                    DbExpression left = parent.TranslateSet(call.Arguments[0]);
                    DbExpression right = parent.TranslateSet(call.Arguments[1]);
                    return this.TranslateBinary(parent, left, right);
                }

                protected abstract DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right);
            }

            private abstract class CallTranslator
            {
                private readonly IEnumerable<MethodInfo> _methods;

                protected CallTranslator(IEnumerable<MethodInfo> methods)
                {
                    this._methods = methods;
                }

                protected CallTranslator(params MethodInfo[] methods)
                {
                    this._methods = methods;
                }

                public override string ToString() => 
                    base.GetType().Name;

                internal abstract DbExpression Translate(ExpressionConverter parent, MethodCallExpression call);

                internal IEnumerable<MethodInfo> Methods =>
                    this._methods;
            }

            private sealed class CanonicalFunctionDefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal CanonicalFunctionDefaultTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(Math).GetMethod("Ceiling", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(Math).GetMethod("Ceiling", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(double) }, null);
                    yield return typeof(Math).GetMethod("Floor", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(Math).GetMethod("Floor", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(double) }, null);
                    yield return typeof(Math).GetMethod("Round", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(Math).GetMethod("Round", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(double) }, null);
                    yield return typeof(decimal).GetMethod("Floor", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(decimal).GetMethod("Ceiling", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(decimal).GetMethod("Round", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(decimal) }, null);
                    yield return typeof(string).GetMethod("Replace", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string) }, null);
                    yield return typeof(string).GetMethod("ToLower", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                    yield return typeof(string).GetMethod("ToUpper", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                    yield return typeof(string).GetMethod("Trim", BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    Expression[] expressionArray;
                    if (!call.Method.IsStatic)
                    {
                        List<Expression> list = new List<Expression>(call.Arguments.Count + 1) {
                            call.Object
                        };
                        list.AddRange(call.Arguments);
                        expressionArray = list.ToArray();
                    }
                    else
                    {
                        expressionArray = call.Arguments.ToArray<Expression>();
                    }
                    return parent.TranslateIntoCanonicalFunction(call.Method.Name, call, expressionArray);
                }

            }

            private sealed class CastMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal CastMethodTranslator() : base(new SequenceMethod[] { SequenceMethod.Cast })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression input = parent.TranslateSet(call.Arguments[0]);
                    Type elementType = TypeSystem.GetElementType(call.Type);
                    Type fromClrType = TypeSystem.GetElementType(call.Arguments[0].Type);
                    DbExpressionBinding binding = parent._commandTree.CreateExpressionBinding(input);
                    DbExpression projection = parent.CreateCastExpression(binding.Variable, elementType, fromClrType);
                    return parent._commandTree.CreateProjectExpression(binding, projection);
                }
            }

            private class ConcatTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
            {
                internal ConcatTranslator() : base(new SequenceMethod[] { SequenceMethod.Concat })
                {
                }

                protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right) => 
                    parent.UnionAll(left, right);
            }

            private sealed class ContainsTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal ContainsTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("Contains", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbFunctionExpression left = parent.TranslateIntoCanonicalFunction("IndexOf", call, new Expression[] { call.Arguments[0], call.Object });
                    return parent._commandTree.CreateGreaterThanExpression(left, parent._commandTree.CreateConstantExpression(0));
                }

            }

            private sealed class CountTranslator : ExpressionConverter.MethodCallTranslator.CountTranslatorBase
            {
                internal CountTranslator() : base("COUNT", new SequenceMethod[] { SequenceMethod.Count, SequenceMethod.CountPredicate })
                {
                }
            }

            private abstract class CountTranslatorBase : ExpressionConverter.MethodCallTranslator.AggregateTranslator
            {
                protected CountTranslatorBase(string functionName, params SequenceMethod[] methods) : base(functionName, true, methods)
                {
                }

                protected override EdmFunction FindFunction(ExpressionConverter parent, MethodCallExpression call, TypeUsage argumentType)
                {
                    TypeUsage resultType = parent._commandTree.CreateTrueExpression().ResultType;
                    return base.FindFunction(parent, call, resultType);
                }

                protected override DbExpression WrapCollectionOperand(ExpressionConverter parent, DbExpression operand, TypeUsage returnType)
                {
                    DbExpressionBinding input = parent._commandTree.CreateExpressionBinding(operand);
                    return parent._commandTree.CreateProjectExpression(input, parent._commandTree.CreateTrueExpression());
                }

                protected override DbExpression WrapNonCollectionOperand(ExpressionConverter parent, DbExpression operand, TypeUsage returnType)
                {
                    DbExpression argument = parent._commandTree.CreateConstantExpression(1);
                    if (!ExpressionConverter.TypeUsageEquals(argument.ResultType, returnType))
                    {
                        argument = parent._commandTree.CreateCastExpression(argument, returnType);
                    }
                    return argument;
                }
            }

            private sealed class DefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private static readonly Dictionary<MethodInfo, MethodInfo> s_alternativeMethods = InitializeAlternateMethodInfos();
                private static readonly object s_vbInitializerLock = new object();
                private static bool s_vbMethodsInitialized;

                internal DefaultTranslator() : base(new MethodInfo[0])
                {
                }

                private static Dictionary<MethodInfo, MethodInfo> InitializeAlternateMethodInfos() => 
                    new Dictionary<MethodInfo, MethodInfo>(1);

                private static void InitializeVBMethods(Assembly vbAssembly)
                {
                    Type type = vbAssembly.GetType("Microsoft.VisualBasic.Strings");
                    s_alternativeMethods.Add(type.GetMethod("Mid", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(int) }, null), type.GetMethod("Mid", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(int), typeof(int) }, null));
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    MethodInfo info;
                    if (TryGetAlternativeMethod(call.Method, out info))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedMethodSuggestedAlternative(call.Method, info));
                    }
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedMethod(call.Method));
                }

                private static bool TryGetAlternativeMethod(MethodInfo originalMethodInfo, out MethodInfo suggestedMethodInfo)
                {
                    if (s_alternativeMethods.TryGetValue(originalMethodInfo, out suggestedMethodInfo))
                    {
                        return true;
                    }
                    if ("Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" == originalMethodInfo.DeclaringType.Assembly.FullName)
                    {
                        lock (s_vbInitializerLock)
                        {
                            if (!s_vbMethodsInitialized)
                            {
                                InitializeVBMethods(originalMethodInfo.DeclaringType.Assembly);
                                s_vbMethodsInitialized = true;
                            }
                            return s_alternativeMethods.TryGetValue(originalMethodInfo, out suggestedMethodInfo);
                        }
                    }
                    suggestedMethodInfo = null;
                    return false;
                }
            }

            private sealed class DistinctTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                internal DistinctTranslator() : base(new SequenceMethod[] { SequenceMethod.Distinct })
                {
                }

                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call) => 
                    parent.Distinct(operand);
            }

            private sealed class EndsWithTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal EndsWithTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("EndsWith", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbFunctionExpression expression = parent.TranslateIntoCanonicalFunction("Length", call, new Expression[] { call.Arguments[0] });
                    DbExpression left = parent.CreateCanonicalFunction("Right", call, new DbExpression[] { parent.TranslateExpression(call.Object), expression });
                    return parent._commandTree.CreateEqualsExpression(left, parent.TranslateExpression(call.Arguments[0]));
                }

            }

            private sealed class ExceptTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
            {
                internal ExceptTranslator() : base(new SequenceMethod[] { SequenceMethod.Except })
                {
                }

                protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right) => 
                    parent.Except(left, right);
            }

            private sealed class FirstOrDefaultPredicateTranslator : ExpressionConverter.MethodCallTranslator.FirstPredicateTranslatorBase
            {
                internal FirstOrDefaultPredicateTranslator() : base(true, new SequenceMethod[] { SequenceMethod.FirstOrDefaultPredicate })
                {
                }
            }

            private sealed class FirstOrDefaultTranslator : ExpressionConverter.MethodCallTranslator.FirstTranslatorBase
            {
                internal FirstOrDefaultTranslator() : base(true, new SequenceMethod[] { SequenceMethod.FirstOrDefault })
                {
                }
            }

            private sealed class FirstPredicateTranslator : ExpressionConverter.MethodCallTranslator.FirstPredicateTranslatorBase
            {
                internal FirstPredicateTranslator() : base(false, new SequenceMethod[] { SequenceMethod.FirstPredicate })
                {
                }
            }

            private abstract class FirstPredicateTranslatorBase : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                private readonly bool _orDefault;

                protected FirstPredicateTranslatorBase(bool orDefault, params SequenceMethod[] methods) : base(methods)
                {
                    this._orDefault = orDefault;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression argument = base.Translate(parent, call);
                    if (parent.IsQueryRoot(call))
                    {
                        return parent.Limit(argument, parent._commandTree.CreateConstantExpression(1));
                    }
                    if (!this._orDefault)
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedNestedFirst);
                    }
                    DbExpression element = parent._commandTree.CreateElementExpression(argument);
                    element = ExpressionConverter.MethodCallTranslator.FirstTranslatorBase.AddDefaultCase(parent, element, call.Type);
                    Span span = null;
                    if (parent.TryGetSpan(argument, out span))
                    {
                        parent.AddSpanMapping(element, span);
                    }
                    return element;
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda) => 
                    parent.Filter(sourceBinding, lambda);
            }

            private sealed class FirstTranslator : ExpressionConverter.MethodCallTranslator.FirstTranslatorBase
            {
                internal FirstTranslator() : base(false, new SequenceMethod[] { SequenceMethod.First })
                {
                }
            }

            private abstract class FirstTranslatorBase : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                private readonly bool _orDefault;

                protected FirstTranslatorBase(bool orDefault, params SequenceMethod[] methods) : base(methods)
                {
                    this._orDefault = orDefault;
                }

                internal static DbExpression AddDefaultCase(ExpressionConverter parent, DbExpression element, Type elementType)
                {
                    object defaultValue = TypeSystem.GetDefaultValue(elementType);
                    if (defaultValue == null)
                    {
                        return element;
                    }
                    List<DbExpression> whenExpressions = new List<DbExpression>(1) {
                        parent.CreateIsNullExpression(element, elementType)
                    };
                    List<DbExpression> thenExpressions = new List<DbExpression>(1) {
                        parent._commandTree.CreateConstantExpression(defaultValue)
                    };
                    return parent._commandTree.CreateCaseExpression(whenExpressions, thenExpressions, element);
                }

                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call)
                {
                    DbExpression argument = parent.Limit(operand, parent._commandTree.CreateConstantExpression(1));
                    if (!parent.IsQueryRoot(call))
                    {
                        if (!this._orDefault)
                        {
                            throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedNestedFirst);
                        }
                        argument = parent._commandTree.CreateElementExpression(argument);
                        argument = AddDefaultCase(parent, argument, call.Type);
                    }
                    Span span = null;
                    if (parent.TryGetSpan(operand, out span))
                    {
                        parent.AddSpanMapping(argument, span);
                    }
                    return argument;
                }
            }

            private sealed class GroupByTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal GroupByTranslator() : base(new SequenceMethod[] { SequenceMethod.GroupBy, SequenceMethod.GroupByElementSelector, SequenceMethod.GroupByElementSelectorResultSelector, SequenceMethod.GroupByResultSelector })
                {
                }

                private static DbExpression ProcessResultSelector(ExpressionConverter parent, MethodCallExpression call, SequenceMethod sequenceMethod, DbExpression topLevelProject, DbExpression result)
                {
                    LambdaExpression lambdaExpression = null;
                    if (sequenceMethod == SequenceMethod.GroupByResultSelector)
                    {
                        lambdaExpression = parent.GetLambdaExpression(call, 2);
                    }
                    else if (sequenceMethod == SequenceMethod.GroupByElementSelectorResultSelector)
                    {
                        lambdaExpression = parent.GetLambdaExpression(call, 3);
                    }
                    if (lambdaExpression != null)
                    {
                        DbExpression expression5;
                        DbExpressionBinding input = parent._commandTree.CreateExpressionBinding(topLevelProject);
                        parent._variableNameToInputExpression.Add(input.VariableName, topLevelProject);
                        DbPropertyExpression cqtExpression = parent._commandTree.CreatePropertyExpression("Key", input.Variable);
                        DbPropertyExpression expression3 = parent._commandTree.CreatePropertyExpression("Group", input.Variable);
                        parent._bindingContext.PushBindingScope(new Binding[] { new Binding(lambdaExpression.Parameters[0], cqtExpression), new Binding(lambdaExpression.Parameters[1], expression3) });
                        DbExpression projection = parent.TranslateExpression(lambdaExpression.Body);
                        result = parent._commandTree.CreateProjectExpression(input, projection);
                        if (parent.TryRewrite(topLevelProject, input, projection, out expression5))
                        {
                            result = expression5;
                        }
                        parent._bindingContext.PopBindingScope();
                    }
                    return result;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call) => 
                    null;

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call, SequenceMethod sequenceMethod)
                {
                    DbExpressionBinding binding;
                    DbExpressionBinding binding2;
                    DbExpression input = parent.TranslateSet(call.Arguments[0]);
                    LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
                    DbExpression projection = parent.TranslateLambda(lambdaExpression, input, out binding);
                    DbExpression left = parent.TranslateLambda(lambdaExpression, input, out binding2);
                    if (!TypeSemantics.IsEqualComparable(projection.ResultType))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedKeySelector(call.Method.Name));
                    }
                    DbExpression expression5 = parent.Distinct(parent._commandTree.CreateProjectExpression(binding, projection));
                    DbExpressionBinding binding3 = parent._commandTree.CreateExpressionBinding(expression5);
                    DbExpression expression7 = parent.Filter(binding2, parent.CreateEqualsExpression(left, binding3.Variable, ExpressionConverter.EqualsPattern.PositiveNullEquality, lambdaExpression.Type, lambdaExpression.Type));
                    bool flag = (sequenceMethod == SequenceMethod.GroupByElementSelector) || (sequenceMethod == SequenceMethod.GroupByElementSelectorResultSelector);
                    if (flag)
                    {
                        DbExpressionBinding binding4;
                        LambdaExpression lambda = parent.GetLambdaExpression(call, 2);
                        DbExpression expression9 = parent.TranslateLambda(lambda, expression7, out binding4);
                        expression7 = parent._commandTree.CreateProjectExpression(binding4, expression9);
                    }
                    List<DbExpression> args = new List<DbExpression>(2) {
                        binding3.Variable,
                        expression7
                    };
                    List<EdmProperty> properties = new List<EdmProperty>(2) {
                        new EdmProperty("Key", args[0].ResultType),
                        new EdmProperty("Group", args[1].ResultType)
                    };
                    InitializerMetadata initializerMetadata = InitializerMetadata.CreateGroupingInitializer(parent.EdmItemCollection, TypeSystem.GetElementType(call.Type));
                    RowType edmType = new RowType(properties, initializerMetadata);
                    TypeUsage type = TypeUsage.Create(edmType);
                    DbExpression key = parent._commandTree.CreateProjectExpression(binding3, parent._commandTree.CreateNewInstanceExpression(type, args));
                    if (!flag)
                    {
                        DbGroupExpressionBinding binding5;
                        DbExpression expression11 = parent.TranslateLambda(lambdaExpression, input, out binding5);
                        ExpressionConverter.DbGroupByTemplate template = new ExpressionConverter.DbGroupByTemplate(binding5) {
                            GroupKeys = { new KeyValuePair<string, DbExpression>("Key", expression11) }
                        };
                        parent._groupByDefaultToOptimizedTranslationMap.Add(key, template);
                    }
                    DbExpression result = key;
                    return ProcessResultSelector(parent, call, sequenceMethod, key, result);
                }
            }

            private sealed class GroupJoinTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal GroupJoinTranslator() : base(new SequenceMethod[] { SequenceMethod.GroupJoin })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpressionBinding binding;
                    DbExpressionBinding binding2;
                    DbExpression input = parent.TranslateSet(call.Arguments[0]);
                    DbExpression expression2 = parent.TranslateSet(call.Arguments[1]);
                    LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 2);
                    LambdaExpression lambda = parent.GetLambdaExpression(call, 3);
                    DbExpression left = parent.TranslateLambda(lambdaExpression, input, out binding);
                    DbExpression right = parent.TranslateLambda(lambda, expression2, out binding2);
                    if (!TypeSemantics.IsEqualComparable(left.ResultType) || !TypeSemantics.IsEqualComparable(right.ResultType))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedKeySelector(call.Method.Name));
                    }
                    DbExpression expression7 = parent.Filter(binding2, parent.CreateEqualsExpression(left, right, ExpressionConverter.EqualsPattern.PositiveNullEquality, lambdaExpression.Body.Type, lambda.Body.Type));
                    List<KeyValuePair<string, DbExpression>> recordColumns = new List<KeyValuePair<string, DbExpression>>(2) {
                        new KeyValuePair<string, DbExpression>("o", binding.Variable),
                        new KeyValuePair<string, DbExpression>("i", expression7)
                    };
                    DbExpression projection = parent._commandTree.CreateNewRowExpression(recordColumns);
                    DbExpression expression9 = parent._commandTree.CreateProjectExpression(binding, projection);
                    DbExpressionBinding binding3 = parent._commandTree.CreateExpressionBinding(expression9);
                    DbExpression cqtExpression = parent._commandTree.CreatePropertyExpression("o", binding3.Variable);
                    DbExpression expression11 = parent._commandTree.CreatePropertyExpression("i", binding3.Variable);
                    LambdaExpression expression12 = parent.GetLambdaExpression(call, 4);
                    parent._bindingContext.PushBindingScope(new Binding[] { new Binding(expression12.Parameters[0], cqtExpression), new Binding(expression12.Parameters[1], expression11) });
                    DbExpression expression13 = parent.TranslateExpression(expression12.Body);
                    parent._bindingContext.PopBindingScope();
                    return parent._commandTree.CreateProjectExpression(binding3, expression13);
                }
            }

            private sealed class IndexOfTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal IndexOfTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("IndexOf", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbFunctionExpression left = parent.TranslateIntoCanonicalFunction("IndexOf", call, new Expression[] { call.Arguments[0], call.Object });
                    return parent._commandTree.CreateMinusExpression(left, parent._commandTree.CreateConstantExpression(1));
                }

            }

            private sealed class InsertTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal InsertTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("Insert", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression = parent.CreateCanonicalFunction("Substring", call, new DbExpression[] { parent.TranslateExpression(call.Object), parent._commandTree.CreateConstantExpression(1), parent.TranslateExpression(call.Arguments[0]) });
                    DbExpression expression2 = parent.CreateCanonicalFunction("Substring", call, new DbExpression[] { parent.TranslateExpression(call.Object), parent._commandTree.CreatePlusExpression(parent.TranslateExpression(call.Arguments[0]), parent._commandTree.CreateConstantExpression(1)), parent._commandTree.CreateMinusExpression(parent.TranslateIntoCanonicalFunction("Length", call, new Expression[] { call.Object }), parent.TranslateExpression(call.Arguments[0])) });
                    return parent.CreateCanonicalFunction("Concat", call, new DbExpression[] { parent.CreateCanonicalFunction("Concat", call, new DbExpression[] { expression, parent.TranslateExpression(call.Arguments[1]) }), expression2 });
                }

            }

            private sealed class IntersectTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
            {
                internal IntersectTranslator() : base(new SequenceMethod[] { SequenceMethod.Intersect })
                {
                }

                protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right) => 
                    parent.Intersect(left, right);
            }

            private sealed class IsNullOrEmptyTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal IsNullOrEmptyTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("IsNullOrEmpty", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression left = parent._commandTree.CreateIsNullExpression(parent.TranslateExpression(call.Arguments[0]));
                    DbExpression right = parent._commandTree.CreateEqualsExpression(parent.TranslateIntoCanonicalFunction("Length", call, new Expression[] { call.Arguments[0] }), parent._commandTree.CreateConstantExpression(0));
                    return parent._commandTree.CreateOrExpression(left, right);
                }

            }

            private sealed class JoinTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal JoinTranslator() : base(new SequenceMethod[] { SequenceMethod.Join })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpressionBinding binding;
                    DbExpressionBinding binding2;
                    DbExpression input = parent.TranslateSet(call.Arguments[0]);
                    DbExpression expression2 = parent.TranslateSet(call.Arguments[1]);
                    LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 2);
                    LambdaExpression lambda = parent.GetLambdaExpression(call, 3);
                    DbExpression left = parent.TranslateLambda(lambdaExpression, input, out binding);
                    DbExpression right = parent.TranslateLambda(lambda, expression2, out binding2);
                    if (!TypeSemantics.IsEqualComparable(left.ResultType) || !TypeSemantics.IsEqualComparable(right.ResultType))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedKeySelector(call.Method.Name));
                    }
                    DbJoinExpression expression7 = parent._commandTree.CreateInnerJoinExpression(binding, binding2, parent.CreateEqualsExpression(left, right, ExpressionConverter.EqualsPattern.PositiveNullEquality, lambdaExpression.Body.Type, lambda.Body.Type));
                    DbExpressionBinding binding3 = parent._commandTree.CreateExpressionBinding(expression7);
                    LambdaExpression expression8 = parent.GetLambdaExpression(call, 4);
                    DbPropertyExpression cqtExpression = parent._commandTree.CreatePropertyExpression(binding.VariableName, binding3.Variable);
                    DbPropertyExpression expression10 = parent._commandTree.CreatePropertyExpression(binding2.VariableName, binding3.Variable);
                    parent._bindingContext.PushBindingScope(new Binding[] { new Binding(expression8.Parameters[0], cqtExpression), new Binding(expression8.Parameters[1], expression10) });
                    DbExpression projection = parent.TranslateExpression(expression8.Body);
                    parent._bindingContext.PopBindingScope();
                    return parent._commandTree.CreateProjectExpression(binding3, projection);
                }
            }

            private sealed class LongCountTranslator : ExpressionConverter.MethodCallTranslator.CountTranslatorBase
            {
                internal LongCountTranslator() : base("BIGCOUNT", new SequenceMethod[] { SequenceMethod.LongCount, SequenceMethod.LongCountPredicate })
                {
                }
            }

            private sealed class MaxTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
            {
                internal MaxTranslator() : base("MAX", false, new SequenceMethod[] { 
                    SequenceMethod.Max, SequenceMethod.MaxSelector, SequenceMethod.MaxInt, SequenceMethod.MaxIntSelector, SequenceMethod.MaxDecimal, SequenceMethod.MaxDecimalSelector, SequenceMethod.MaxDouble, SequenceMethod.MaxDoubleSelector, SequenceMethod.MaxLong, SequenceMethod.MaxLongSelector, SequenceMethod.MaxSingle, SequenceMethod.MaxSingleSelector, SequenceMethod.MaxNullableDecimal, SequenceMethod.MaxNullableDecimalSelector, SequenceMethod.MaxNullableDouble, SequenceMethod.MaxNullableDoubleSelector,
                    SequenceMethod.MaxNullableInt, SequenceMethod.MaxNullableIntSelector, SequenceMethod.MaxNullableLong, SequenceMethod.MaxNullableLongSelector, SequenceMethod.MaxNullableSingle, SequenceMethod.MaxNullableSingleSelector
                })
                {
                }
            }

            private sealed class MinTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
            {
                internal MinTranslator() : base("MIN", false, new SequenceMethod[] { 
                    SequenceMethod.Min, SequenceMethod.MinSelector, SequenceMethod.MinDecimal, SequenceMethod.MinDecimalSelector, SequenceMethod.MinDouble, SequenceMethod.MinDoubleSelector, SequenceMethod.MinInt, SequenceMethod.MinIntSelector, SequenceMethod.MinLong, SequenceMethod.MinLongSelector, SequenceMethod.MinNullableDecimal, SequenceMethod.MinSingle, SequenceMethod.MinSingleSelector, SequenceMethod.MinNullableDecimalSelector, SequenceMethod.MinNullableDouble, SequenceMethod.MinNullableDoubleSelector,
                    SequenceMethod.MinNullableInt, SequenceMethod.MinNullableIntSelector, SequenceMethod.MinNullableLong, SequenceMethod.MinNullableLongSelector, SequenceMethod.MinNullableSingle, SequenceMethod.MinNullableSingleSelector
                })
                {
                }
            }

            private abstract class ObjectQueryBuilderCallTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
            {
                private readonly ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator _translator;

                protected ObjectQueryBuilderCallTranslator(string methodName, SequenceMethod sequenceEquivalent) : base(methodName)
                {
                    ExpressionConverter.MethodCallTranslator.s_sequenceTranslators.TryGetValue(sequenceEquivalent, out this._translator);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call) => 
                    this._translator.Translate(parent, call);
            }

            private sealed class ObjectQueryBuilderDistinctTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderDistinctTranslator() : base("Distinct", SequenceMethod.Distinct)
                {
                }
            }

            private sealed class ObjectQueryBuilderExceptTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderExceptTranslator() : base("Except", SequenceMethod.Except)
                {
                }
            }

            private sealed class ObjectQueryBuilderFirstTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderFirstTranslator() : base("First", SequenceMethod.First)
                {
                }
            }

            private sealed class ObjectQueryBuilderIntersectTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderIntersectTranslator() : base("Intersect", SequenceMethod.Intersect)
                {
                }
            }

            private sealed class ObjectQueryBuilderOfTypeTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderOfTypeTranslator() : base("OfType", SequenceMethod.OfType)
                {
                }
            }

            private sealed class ObjectQueryBuilderUnionTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryBuilderCallTranslator
            {
                internal ObjectQueryBuilderUnionTranslator() : base("Union", SequenceMethod.Union)
                {
                }
            }

            private abstract class ObjectQueryCallTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private readonly string _methodName;

                protected ObjectQueryCallTranslator(string methodName) : base(new MethodInfo[0])
                {
                    this._methodName = methodName;
                }

                internal string MethodName =>
                    this._methodName;
            }

            private sealed class ObjectQueryIncludeTranslator : ExpressionConverter.MethodCallTranslator.ObjectQueryCallTranslator
            {
                internal ObjectQueryIncludeTranslator() : base("Include")
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    Span span;
                    DbExpression expression = parent.TranslateExpression(call.Object);
                    if (!parent.TryGetSpan(expression, out span))
                    {
                        span = null;
                    }
                    DbExpression expression2 = parent.TranslateExpression(call.Arguments[0]);
                    string pathToInclude = null;
                    if (expression2.ExpressionKind != DbExpressionKind.Constant)
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedInclude);
                    }
                    pathToInclude = (string) ((DbConstantExpression) expression2).Value;
                    return parent.AddSpanMapping(expression, Span.IncludeIn(span, pathToInclude));
                }
            }

            private sealed class OfTypeTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                internal OfTypeTranslator() : base(new SequenceMethod[] { SequenceMethod.OfType })
                {
                }

                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call)
                {
                    TypeUsage usage;
                    Type linqType = call.Method.GetGenericArguments()[0];
                    if (!parent.TryGetValueLayerType(linqType, out usage) || (!TypeSemantics.IsEntityType(usage) && !TypeSemantics.IsComplexType(usage)))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_InvalidOfTypeResult(ExpressionConverter.DescribeClrType(linqType)));
                    }
                    return parent.OfType(operand, usage);
                }
            }

            private abstract class OneLambdaTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal OneLambdaTranslator(params SequenceMethod[] methods) : base(methods)
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression;
                    DbExpressionBinding binding;
                    DbExpression expression2;
                    return this.Translate(parent, call, out expression, out binding, out expression2);
                }

                protected DbExpression Translate(ExpressionConverter parent, MethodCallExpression call, out DbExpression source, out DbExpressionBinding sourceBinding, out DbExpression lambda)
                {
                    source = parent.TranslateExpression(call.Arguments[0]);
                    LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
                    lambda = parent.TranslateLambda(lambdaExpression, source, out sourceBinding);
                    return this.TranslateOneLambda(parent, sourceBinding, lambda);
                }

                protected abstract DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda);
            }

            private sealed class OrderByDescendingTranslator : ExpressionConverter.MethodCallTranslator.OrderByTranslatorBase
            {
                internal OrderByDescendingTranslator() : base(false, new SequenceMethod[] { SequenceMethod.OrderByDescending })
                {
                }
            }

            private sealed class OrderByTranslator : ExpressionConverter.MethodCallTranslator.OrderByTranslatorBase
            {
                internal OrderByTranslator() : base(true, new SequenceMethod[] { SequenceMethod.OrderBy })
                {
                }
            }

            private abstract class OrderByTranslatorBase : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                private readonly bool _ascending;

                protected OrderByTranslatorBase(bool ascending, params SequenceMethod[] methods) : base(methods)
                {
                    this._ascending = ascending;
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda)
                {
                    List<DbSortClause> keys = new List<DbSortClause>(1);
                    DbSortClause item = parent._commandTree.CreateSortClause(lambda, this._ascending);
                    keys.Add(item);
                    return parent.Sort(sourceBinding, keys);
                }
            }

            private abstract class PagingTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                protected PagingTranslator(params SequenceMethod[] methods) : base(methods)
                {
                }

                protected abstract DbExpression TranslatePagingOperator(ExpressionConverter parent, DbExpression operand, DbExpression count);
                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call)
                {
                    Expression linq = call.Arguments[1];
                    DbExpression count = parent.TranslateExpression(linq);
                    DbProjectExpression expression3 = null;
                    if (operand.ExpressionKind == DbExpressionKind.Project)
                    {
                        expression3 = (DbProjectExpression) operand;
                        operand = expression3.Input.Expression;
                    }
                    DbExpression expression4 = this.TranslatePagingOperator(parent, operand, count);
                    if (expression3 != null)
                    {
                        expression3.Input.Expression = expression4;
                        expression4 = expression3;
                    }
                    return expression4;
                }
            }

            private sealed class PassthroughTranslator : ExpressionConverter.MethodCallTranslator.UnarySequenceMethodTranslator
            {
                internal PassthroughTranslator() : base(new SequenceMethod[] { SequenceMethod.AsQueryableGeneric, SequenceMethod.AsQueryable, SequenceMethod.AsEnumerable })
                {
                }

                protected override DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call)
                {
                    if (!TypeSemantics.IsCollectionType(operand.ResultType))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedPassthrough(call.Method.Name, operand.ResultType.EdmType.Name));
                    }
                    return operand;
                }
            }

            private sealed class RemoveTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal RemoveTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("Remove", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
                    yield return typeof(string).GetMethod("Remove", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(int) }, null);
                }

                private static bool IsNonNegativeIntegerConstant(DbExpression argument)
                {
                    if ((argument.ExpressionKind != DbExpressionKind.Constant) || !TypeSemantics.IsPrimitiveType(argument.ResultType, PrimitiveTypeKind.Int32))
                    {
                        return false;
                    }
                    DbConstantExpression expression = (DbConstantExpression) argument;
                    int num = (int) expression.Value;
                    if (num < 0)
                    {
                        return false;
                    }
                    return true;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression = parent.CreateCanonicalFunction("Substring", call, new DbExpression[] { parent.TranslateExpression(call.Object), parent._commandTree.CreateConstantExpression(1), parent.TranslateExpression(call.Arguments[0]) });
                    if (call.Arguments.Count != 2)
                    {
                        return expression;
                    }
                    DbExpression argument = parent.TranslateExpression(call.Arguments[1]);
                    if (!IsNonNegativeIntegerConstant(argument))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedStringRemoveCase(call.Method, call.Method.GetParameters()[1].Name));
                    }
                    DbExpression expression3 = parent._commandTree.CreatePlusExpression(parent._commandTree.CreatePlusExpression(parent.TranslateExpression(call.Arguments[0]), argument), parent._commandTree.CreateConstantExpression(1));
                    DbExpression expression4 = parent._commandTree.CreateMinusExpression(parent.TranslateIntoCanonicalFunction("Length", call, new Expression[] { call.Object }), parent._commandTree.CreatePlusExpression(parent.TranslateExpression(call.Arguments[0]), parent.TranslateExpression(call.Arguments[1])));
                    DbExpression expression5 = parent.CreateCanonicalFunction("Substring", call, new DbExpression[] { parent.TranslateExpression(call.Object), expression3, expression4 });
                    return parent.CreateCanonicalFunction("Concat", call, new DbExpression[] { expression, expression5 });
                }

            }

            private sealed class SelectManyTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                internal SelectManyTranslator() : base(new SequenceMethod[] { SequenceMethod.SelectMany, SequenceMethod.SelectManyResultSelector })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression3;
                    bool flag = 3 == call.Arguments.Count;
                    DbExpression input = base.Translate(parent, call);
                    DbExpressionBinding binding = parent._commandTree.CreateExpressionBinding(input);
                    RowType edmType = (RowType) binding.Variable.ResultType.EdmType;
                    DbExpression cqtExpression = parent._commandTree.CreatePropertyExpression(edmType.Properties[1], binding.Variable);
                    if (flag)
                    {
                        DbExpression expression4 = parent._commandTree.CreatePropertyExpression(edmType.Properties[0], binding.Variable);
                        LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 2);
                        parent._bindingContext.PushBindingScope(new Binding[] { new Binding(lambdaExpression.Parameters[0], expression4), new Binding(lambdaExpression.Parameters[1], cqtExpression) });
                        expression3 = parent.TranslateSet(lambdaExpression.Body);
                        parent._bindingContext.PopBindingScope();
                    }
                    else
                    {
                        expression3 = cqtExpression;
                    }
                    return parent._commandTree.CreateProjectExpression(binding, expression3);
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda)
                {
                    lambda = parent.NormalizeSetSource(lambda);
                    DbExpressionBinding apply = parent._commandTree.CreateExpressionBinding(lambda);
                    return parent._commandTree.CreateCrossApplyExpression(sourceBinding, apply);
                }
            }

            private sealed class SelectTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                internal SelectTranslator() : base(new SequenceMethod[] { SequenceMethod.Select })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression;
                    DbExpressionBinding binding;
                    DbExpression expression2;
                    DbExpression expression4;
                    DbExpression expression3 = base.Translate(parent, call, out expression, out binding, out expression2);
                    if (parent.TryRewrite(expression, binding, expression2, out expression4))
                    {
                        expression3 = expression4;
                    }
                    return expression3;
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda) => 
                    parent.Project(sourceBinding, lambda);
            }

            private abstract class SequenceMethodTranslator
            {
                private readonly IEnumerable<SequenceMethod> _methods;

                protected SequenceMethodTranslator(params SequenceMethod[] methods)
                {
                    this._methods = methods;
                }

                public override string ToString() => 
                    base.GetType().Name;

                internal abstract DbExpression Translate(ExpressionConverter parent, MethodCallExpression call);
                internal virtual DbExpression Translate(ExpressionConverter parent, MethodCallExpression call, SequenceMethod sequenceMethod) => 
                    this.Translate(parent, call);

                internal IEnumerable<SequenceMethod> Methods =>
                    this._methods;
            }

            private sealed class SingleTranslatorNotSupported : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                internal SingleTranslatorNotSupported() : base(new SequenceMethod[] { SequenceMethod.Single, SequenceMethod.SinglePredicate, SequenceMethod.SingleOrDefault, SequenceMethod.SingleOrDefaultPredicate })
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedSingle);
                }
            }

            private sealed class SkipTranslator : ExpressionConverter.MethodCallTranslator.PagingTranslator
            {
                internal SkipTranslator() : base(new SequenceMethod[] { SequenceMethod.Skip })
                {
                }

                protected override DbExpression TranslatePagingOperator(ExpressionConverter parent, DbExpression operand, DbExpression count)
                {
                    if (operand.ExpressionKind != DbExpressionKind.Sort)
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_SkipWithoutOrder);
                    }
                    DbSortExpression expression = (DbSortExpression) operand;
                    Span span = null;
                    bool flag = parent.TryGetSpan(expression, out span);
                    DbSkipExpression expression2 = parent.Skip(expression.Input, expression.SortOrder, count);
                    if (flag)
                    {
                        parent.AddSpanMapping(expression2, span);
                    }
                    return expression2;
                }
            }

            private sealed class StartsWithTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal StartsWithTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("StartsWith", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbFunctionExpression left = parent.TranslateIntoCanonicalFunction("IndexOf", call, new Expression[] { call.Arguments[0], call.Object });
                    return parent._commandTree.CreateEqualsExpression(left, parent._commandTree.CreateConstantExpression(1));
                }

            }

            private sealed class StringConcatTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal StringConcatTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string) }, null);
                    yield return typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
                    yield return typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression = parent.TranslateExpression(call.Arguments[0]);
                    for (int i = 1; i < call.Arguments.Count; i++)
                    {
                        expression = parent.CreateCanonicalFunction("Concat", call, new DbExpression[] { expression, parent.TranslateExpression(call.Arguments[i]) });
                    }
                    return expression;
                }

            }

            private sealed class SubstringTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                internal SubstringTranslator() : base(GetMethods())
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("Substring", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int) }, null);
                    yield return typeof(string).GetMethod("Substring", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(int) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression3;
                    DbExpression expression = parent.TranslateExpression(call.Object);
                    DbExpression expression2 = parent._commandTree.CreatePlusExpression(parent.TranslateExpression(call.Arguments[0]), parent._commandTree.CreateConstantExpression(1));
                    if (call.Arguments.Count == 1)
                    {
                        expression3 = parent._commandTree.CreateMinusExpression(parent.TranslateIntoCanonicalFunction("Length", call, new Expression[] { call.Object }), parent.TranslateExpression(call.Arguments[0]));
                    }
                    else
                    {
                        expression3 = parent.TranslateExpression(call.Arguments[1]);
                    }
                    return parent.CreateCanonicalFunction("Substring", call, new DbExpression[] { expression, expression2, expression3 });
                }

            }

            private sealed class SumTranslator : ExpressionConverter.MethodCallTranslator.AggregateTranslator
            {
                internal SumTranslator() : base("SUM", false, new SequenceMethod[] { 
                    SequenceMethod.SumDecimal, SequenceMethod.SumDecimalSelector, SequenceMethod.SumDouble, SequenceMethod.SumDoubleSelector, SequenceMethod.SumInt, SequenceMethod.SumIntSelector, SequenceMethod.SumLong, SequenceMethod.SumLongSelector, SequenceMethod.SumSingle, SequenceMethod.SumSingleSelector, SequenceMethod.SumNullableDecimal, SequenceMethod.SumNullableDecimalSelector, SequenceMethod.SumNullableDouble, SequenceMethod.SumNullableDoubleSelector, SequenceMethod.SumNullableInt, SequenceMethod.SumNullableIntSelector,
                    SequenceMethod.SumNullableLong, SequenceMethod.SumNullableLongSelector, SequenceMethod.SumNullableSingle, SequenceMethod.SumNullableSingleSelector
                })
                {
                }
            }

            private sealed class TakeTranslator : ExpressionConverter.MethodCallTranslator.PagingTranslator
            {
                internal TakeTranslator() : base(new SequenceMethod[] { SequenceMethod.Take })
                {
                }

                protected override DbExpression TranslatePagingOperator(ExpressionConverter parent, DbExpression operand, DbExpression count) => 
                    parent.Limit(operand, count);
            }

            private sealed class ThenByDescendingTranslator : ExpressionConverter.MethodCallTranslator.ThenByTranslatorBase
            {
                internal ThenByDescendingTranslator() : base(false, new SequenceMethod[] { SequenceMethod.ThenByDescending })
                {
                }
            }

            private sealed class ThenByTranslator : ExpressionConverter.MethodCallTranslator.ThenByTranslatorBase
            {
                internal ThenByTranslator() : base(true, new SequenceMethod[] { SequenceMethod.ThenBy })
                {
                }
            }

            private abstract class ThenByTranslatorBase : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                private readonly bool _ascending;

                protected ThenByTranslatorBase(bool ascending, params SequenceMethod[] methods) : base(methods)
                {
                    this._ascending = ascending;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    DbExpression expression = parent.TranslateSet(call.Arguments[0]);
                    if (DbExpressionKind.Sort != expression.ExpressionKind)
                    {
                        throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ELinq_ThenByDoesNotFollowOrderBy);
                    }
                    DbSortExpression expression2 = (DbSortExpression) expression;
                    DbExpressionBinding input = expression2.Input;
                    LambdaExpression lambdaExpression = parent.GetLambdaExpression(call, 1);
                    ParameterExpression linqExpression = lambdaExpression.Parameters[0];
                    parent._bindingContext.PushBindingScope(new Binding[] { new Binding(linqExpression, input.Variable) });
                    DbExpression key = parent.TranslateExpression(lambdaExpression.Body);
                    parent._bindingContext.PopBindingScope();
                    List<DbSortClause> keys = new List<DbSortClause>(expression2.SortOrder) {
                        new DbSortClause(key, this._ascending, null)
                    };
                    return parent.Sort(input, keys);
                }
            }

            private sealed class TrimEndTranslator : ExpressionConverter.MethodCallTranslator.TrimStartTrimEndBaseTranslator
            {
                internal TrimEndTranslator() : base(GetMethods(), "RTrim")
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("TrimEnd", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(char[]) }, null);
                }

            }

            private sealed class TrimStartTranslator : ExpressionConverter.MethodCallTranslator.TrimStartTrimEndBaseTranslator
            {
                internal TrimStartTranslator() : base(GetMethods(), "LTrim")
                {
                }

                private static IEnumerable<MethodInfo> GetMethods()
                {
                    yield return typeof(string).GetMethod("TrimStart", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(char[]) }, null);
                }

            }

            private abstract class TrimStartTrimEndBaseTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private string _canonicalFunctionName;

                protected TrimStartTrimEndBaseTranslator(IEnumerable<MethodInfo> methods, string canonicalFunctionName) : base(methods)
                {
                    this._canonicalFunctionName = canonicalFunctionName;
                }

                internal static bool IsEmptyArray(Expression expression)
                {
                    if (expression.NodeType != ExpressionType.NewArrayInit)
                    {
                        return false;
                    }
                    NewArrayExpression expression2 = (NewArrayExpression) expression;
                    if (expression2.Expressions.Count != 0)
                    {
                        return false;
                    }
                    return true;
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    if (!IsEmptyArray(call.Arguments[0]))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedTrimStartTrimEndCase(call.Method));
                    }
                    return parent.TranslateIntoCanonicalFunction(this._canonicalFunctionName, call, new Expression[] { call.Object });
                }
            }

            private abstract class UnarySequenceMethodTranslator : ExpressionConverter.MethodCallTranslator.SequenceMethodTranslator
            {
                protected UnarySequenceMethodTranslator(params SequenceMethod[] methods) : base(methods)
                {
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    if (call.Object != null)
                    {
                        DbExpression expression = parent.TranslateSet(call.Object);
                        return this.TranslateUnary(parent, expression, call);
                    }
                    DbExpression operand = parent.TranslateSet(call.Arguments[0]);
                    return this.TranslateUnary(parent, operand, call);
                }

                protected abstract DbExpression TranslateUnary(ExpressionConverter parent, DbExpression operand, MethodCallExpression call);
            }

            private sealed class UnionTranslator : ExpressionConverter.MethodCallTranslator.BinarySequenceMethodTranslator
            {
                internal UnionTranslator() : base(new SequenceMethod[] { SequenceMethod.Union })
                {
                }

                protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right) => 
                    parent.Distinct(parent.UnionAll(left, right));
            }

            private sealed class VBCanonicalFunctionDefaultTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";
                private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";

                internal VBCanonicalFunctionDefaultTranslator(Assembly vbAssembly) : base(GetMethods(vbAssembly))
                {
                }

                private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
                {
                    Type type = vbAssembly.GetType("Microsoft.VisualBasic.Strings");
                    yield return type.GetMethod("Trim", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                    yield return type.GetMethod("LTrim", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                    yield return type.GetMethod("RTrim", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                    yield return type.GetMethod("Left", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(int) }, null);
                    yield return type.GetMethod("Right", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(int) }, null);
                    Type iteratorVariable1 = vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime");
                    yield return iteratorVariable1.GetMethod("Year", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                    yield return iteratorVariable1.GetMethod("Month", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                    yield return iteratorVariable1.GetMethod("Day", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                    yield return iteratorVariable1.GetMethod("Hour", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                    yield return iteratorVariable1.GetMethod("Minute", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                    yield return iteratorVariable1.GetMethod("Second", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(DateTime) }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call) => 
                    parent.TranslateIntoCanonicalFunction(call.Method.Name, call, call.Arguments.ToArray<Expression>());

            }

            private sealed class VBCanonicalFunctionRenameTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private static readonly Dictionary<MethodInfo, string> s_methodNameMap = new Dictionary<MethodInfo, string>(4);
                private const string s_stringsTypeFullName = "Microsoft.VisualBasic.Strings";

                internal VBCanonicalFunctionRenameTranslator(Assembly vbAssembly) : base(GetMethods(vbAssembly))
                {
                }

                private static MethodInfo GetMethod(Type declaringType, string methodName, string canonicalFunctionName, Type[] argumentTypes)
                {
                    MethodInfo key = declaringType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, argumentTypes, null);
                    s_methodNameMap.Add(key, canonicalFunctionName);
                    return key;
                }

                private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
                {
                    Type type = vbAssembly.GetType("Microsoft.VisualBasic.Strings");
                    yield return GetMethod(type, "Len", "Length", new Type[] { typeof(string) });
                    yield return GetMethod(type, "Mid", "Substring", new Type[] { typeof(string), typeof(int), typeof(int) });
                    yield return GetMethod(type, "UCase", "ToUpper", new Type[] { typeof(string) });
                    yield return GetMethod(type, "LCase", "ToLower", new Type[] { typeof(string) });
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call) => 
                    parent.TranslateIntoCanonicalFunction(s_methodNameMap[call.Method], call, call.Arguments.ToArray<Expression>());

            }

            private sealed class VBDatePartTranslator : ExpressionConverter.MethodCallTranslator.CallTranslator
            {
                private const string s_dateAndTimeTypeFullName = "Microsoft.VisualBasic.DateAndTime";
                private const string s_DateIntervalFullName = "Microsoft.VisualBasic.DateInterval";
                private const string s_FirstDayOfWeekFullName = "Microsoft.VisualBasic.FirstDayOfWeek";
                private const string s_FirstWeekOfYearFullName = "Microsoft.VisualBasic.FirstWeekOfYear";
                private static HashSet<string> s_supportedIntervals = new HashSet<string>();

                static VBDatePartTranslator()
                {
                    s_supportedIntervals.Add("Year");
                    s_supportedIntervals.Add("Month");
                    s_supportedIntervals.Add("Day");
                    s_supportedIntervals.Add("Hour");
                    s_supportedIntervals.Add("Minute");
                    s_supportedIntervals.Add("Second");
                }

                internal VBDatePartTranslator(Assembly vbAssembly) : base(GetMethods(vbAssembly))
                {
                }

                private static IEnumerable<MethodInfo> GetMethods(Assembly vbAssembly)
                {
                    Type type = vbAssembly.GetType("Microsoft.VisualBasic.DateAndTime");
                    Type iteratorVariable1 = vbAssembly.GetType("Microsoft.VisualBasic.DateInterval");
                    Type iteratorVariable2 = vbAssembly.GetType("Microsoft.VisualBasic.FirstDayOfWeek");
                    Type iteratorVariable3 = vbAssembly.GetType("Microsoft.VisualBasic.FirstWeekOfYear");
                    yield return type.GetMethod("DatePart", BindingFlags.Public | BindingFlags.Static, null, new Type[] { iteratorVariable1, typeof(DateTime), iteratorVariable2, iteratorVariable3 }, null);
                }

                internal override DbExpression Translate(ExpressionConverter parent, MethodCallExpression call)
                {
                    ConstantExpression expression = call.Arguments[0] as ConstantExpression;
                    if (expression == null)
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedVBDatePartNonConstantInterval(call.Method, call.Method.GetParameters()[0].Name));
                    }
                    string item = expression.Value.ToString();
                    if (!s_supportedIntervals.Contains(item))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedVBDatePartInvalidInterval(call.Method, call.Method.GetParameters()[0].Name, item));
                    }
                    return parent.TranslateIntoCanonicalFunction(item, call, new Expression[] { call.Arguments[1] });
                }

            }

            private sealed class WhereTranslator : ExpressionConverter.MethodCallTranslator.OneLambdaTranslator
            {
                internal WhereTranslator() : base(new SequenceMethod[1])
                {
                }

                protected override DbExpression TranslateOneLambda(ExpressionConverter parent, DbExpressionBinding sourceBinding, DbExpression lambda) => 
                    parent.Filter(sourceBinding, lambda);
            }
        }

        private sealed class ModuloTranslator : ExpressionConverter.BinaryTranslator
        {
            internal ModuloTranslator() : base(new ExpressionType[] { ExpressionType.Modulo })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateModuloExpression(left, right);
        }

        private sealed class MultiplyTranslator : ExpressionConverter.BinaryTranslator
        {
            internal MultiplyTranslator() : base(new ExpressionType[] { ExpressionType.Multiply, ExpressionType.MultiplyChecked })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateMultiplyExpression(left, right);
        }

        private sealed class NegateTranslator : ExpressionConverter.UnaryTranslator
        {
            internal NegateTranslator() : base(new ExpressionType[] { ExpressionType.Negate, ExpressionType.NegateChecked })
            {
            }

            protected override DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand) => 
                parent._commandTree.CreateUnaryMinusExpression(operand);
        }

        private sealed class NewTranslator : ExpressionConverter.TypedTranslator<NewExpression>
        {
            internal NewTranslator() : base(new ExpressionType[] { ExpressionType.New })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, NewExpression linq)
            {
                InitializerMetadata metadata;
                int num = (linq.Members == null) ? 0 : linq.Members.Count;
                if ((linq.Constructor == null) || (linq.Arguments.Count != num))
                {
                    throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedConstructor);
                }
                parent.CheckInitializerType(linq.Type);
                List<KeyValuePair<string, DbExpression>> columns = new List<KeyValuePair<string, DbExpression>>(num + 1);
                HashSet<string> set = new HashSet<string>(StringComparer.Ordinal);
                for (int i = 0; i < num; i++)
                {
                    string str;
                    Type type;
                    TypeSystem.PropertyOrField(linq.Members[i], out str, out type);
                    DbExpression expression = parent.TranslateExpression(linq.Arguments[i]);
                    set.Add(str);
                    columns.Add(new KeyValuePair<string, DbExpression>(str, expression));
                }
                if (num == 0)
                {
                    columns.Add(new KeyValuePair<string, DbExpression>("Key", parent._commandTree.CreateTrueExpression()));
                    metadata = InitializerMetadata.CreateEmptyProjectionInitializer(parent.EdmItemCollection, linq);
                }
                else
                {
                    metadata = InitializerMetadata.CreateProjectionInitializer(parent.EdmItemCollection, linq);
                }
                parent.ValidateInitializerMetadata(metadata);
                return parent.CreateNewRowExpression(columns, metadata);
            }
        }

        private sealed class NotEqualsTranslator : ExpressionConverter.TypedTranslator<BinaryExpression>
        {
            internal NotEqualsTranslator() : base(new ExpressionType[] { ExpressionType.NotEqual })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, BinaryExpression linq)
            {
                Expression expression = Expression.Not(Expression.Equal(linq.Left, linq.Right));
                return parent.TranslateExpression(expression);
            }
        }

        private sealed class NotSupportedTranslator : ExpressionConverter.Translator
        {
            internal NotSupportedTranslator(params ExpressionType[] nodeTypes) : base(nodeTypes)
            {
            }

            internal override DbExpression Translate(ExpressionConverter parent, Expression linq)
            {
                throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedExpressionType(linq.NodeType));
            }
        }

        private sealed class NotTranslator : ExpressionConverter.TypedTranslator<UnaryExpression>
        {
            internal NotTranslator() : base(new ExpressionType[] { ExpressionType.Not })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, UnaryExpression linq)
            {
                DbExpression argument = parent.TranslateExpression(linq.Operand);
                if (TypeSemantics.IsBooleanType(argument.ResultType))
                {
                    return parent._commandTree.CreateNotExpression(argument);
                }
                return parent.CreateCanonicalFunction("BitwiseNot", linq, new DbExpression[] { argument });
            }
        }

        private sealed class OrElseTranslator : ExpressionConverter.BinaryTranslator
        {
            internal OrElseTranslator() : base(new ExpressionType[] { ExpressionType.OrElse })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateOrExpression(left, right);
        }

        private sealed class OrTranslator : ExpressionConverter.BitwiseBinaryTranslator
        {
            internal OrTranslator() : base(ExpressionType.Or, "BitwiseOr")
            {
            }

            protected override DbExpression TranslateIntoLogicExpression(ExpressionConverter parent, BinaryExpression linq, DbExpression left, DbExpression right) => 
                parent._commandTree.CreateOrExpression(left, right);
        }

        private sealed class ParameterTranslator : ExpressionConverter.TypedTranslator<ParameterExpression>
        {
            internal ParameterTranslator() : base(new ExpressionType[] { ExpressionType.Parameter })
            {
            }

            protected override DbExpression TypedTranslate(ExpressionConverter parent, ParameterExpression linq)
            {
                if (parent._bindingContext.IsRootContextParameter(linq))
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ELinq_UnsupportedUseOfContextParameter(linq.Name));
                }
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ELinq_UnboundParameterExpression(linq.Name));
            }
        }

        private sealed class QuoteTranslator : ExpressionConverter.UnaryTranslator
        {
            internal QuoteTranslator() : base(new ExpressionType[] { ExpressionType.Quote })
            {
            }

            protected override DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand) => 
                operand;
        }

        private sealed class SubtractTranslator : ExpressionConverter.BinaryTranslator
        {
            internal SubtractTranslator() : base(new ExpressionType[] { ExpressionType.Subtract, ExpressionType.SubtractChecked })
            {
            }

            protected override DbExpression TranslateBinary(ExpressionConverter parent, DbExpression left, DbExpression right, BinaryExpression linq) => 
                parent._commandTree.CreateMinusExpression(left, right);
        }

        private abstract class Translator
        {
            private readonly ExpressionType[] _nodeTypes;

            protected Translator(params ExpressionType[] nodeTypes)
            {
                this._nodeTypes = nodeTypes;
            }

            public override string ToString() => 
                base.GetType().Name;

            internal abstract DbExpression Translate(ExpressionConverter parent, Expression linq);

            internal IEnumerable<ExpressionType> NodeTypes =>
                this._nodeTypes;
        }

        private abstract class TypedTranslator<T_Linq> : ExpressionConverter.Translator where T_Linq: Expression
        {
            protected TypedTranslator(params ExpressionType[] nodeTypes) : base(nodeTypes)
            {
            }

            internal override DbExpression Translate(ExpressionConverter parent, Expression linq) => 
                this.TypedTranslate(parent, (T_Linq) linq);

            protected abstract DbExpression TypedTranslate(ExpressionConverter parent, T_Linq linq);
        }

        private sealed class UnaryPlusTranslator : ExpressionConverter.UnaryTranslator
        {
            internal UnaryPlusTranslator() : base(new ExpressionType[] { ExpressionType.UnaryPlus })
            {
            }

            protected override DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand) => 
                operand;
        }

        private abstract class UnaryTranslator : ExpressionConverter.TypedTranslator<UnaryExpression>
        {
            protected UnaryTranslator(params ExpressionType[] nodeTypes) : base(nodeTypes)
            {
            }

            protected abstract DbExpression TranslateUnary(ExpressionConverter parent, UnaryExpression unary, DbExpression operand);
            protected override DbExpression TypedTranslate(ExpressionConverter parent, UnaryExpression linq) => 
                this.TranslateUnary(parent, linq, parent.TranslateExpression(linq.Operand));
        }
    }
}

