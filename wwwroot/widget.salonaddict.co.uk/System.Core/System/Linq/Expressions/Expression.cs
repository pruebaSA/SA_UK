namespace System.Linq.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    public abstract class Expression
    {
        private static readonly System.Type[] actionTypes = new System.Type[] { typeof(Action), typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>) };
        private static readonly System.Type[] funcTypes = new System.Type[] { typeof(Func<>), typeof(Func<,>), typeof(Func<,,>), typeof(Func<,,,>), typeof(Func<,,,,>) };
        private static System.Type[] lambdaTypes = new System.Type[] { typeof(Expression), typeof(IEnumerable<ParameterExpression>) };
        private ExpressionType nodeType;
        private System.Type type;

        protected Expression(ExpressionType nodeType, System.Type type)
        {
            this.nodeType = nodeType;
            this.type = type;
        }

        public static BinaryExpression Add(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.Add, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Add, "op_Addition", left, right, true);
        }

        public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Add(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Add, left, right, method, true);
        }

        public static BinaryExpression AddChecked(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.AddChecked, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.AddChecked, "op_Addition", left, right, false);
        }

        public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return AddChecked(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.AddChecked, left, right, method, true);
        }

        public static BinaryExpression And(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsIntegerOrBool(left.Type))
            {
                return new BinaryExpression(ExpressionType.And, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.And, "op_BitwiseAnd", left, right, true);
        }

        public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return And(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.And, left, right, method, true);
        }

        public static BinaryExpression AndAlso(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsBool(left.Type))
            {
                return new BinaryExpression(ExpressionType.AndAlso, left, right, left.Type);
            }
            MethodInfo method = GetUserDefinedBinaryOperator(ExpressionType.AndAlso, left.Type, right.Type, "op_BitwiseAnd");
            if (method == null)
            {
                throw System.Linq.Expressions.Error.BinaryOperatorNotDefined(ExpressionType.AndAlso, left.Type, right.Type);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
            return new BinaryExpression(ExpressionType.AndAlso, left, right, method, (IsNullableType(left.Type) && (method.ReturnType == GetNonNullableType(left.Type))) ? left.Type : method.ReturnType);
        }

        public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return AndAlso(left, right);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.AndAlso, left.Type, right.Type, method);
            return new BinaryExpression(ExpressionType.AndAlso, left, right, method, (IsNullableType(left.Type) && (method.ReturnType == GetNonNullableType(left.Type))) ? left.Type : method.ReturnType);
        }

        private static MethodInfo ApplyTypeArgs(MethodInfo m, System.Type[] typeArgs)
        {
            if ((typeArgs == null) || (typeArgs.Length == 0))
            {
                if (!m.IsGenericMethodDefinition)
                {
                    return m;
                }
            }
            else if (m.IsGenericMethodDefinition && (m.GetGenericArguments().Length == typeArgs.Length))
            {
                return m.MakeGenericMethod(typeArgs);
            }
            return null;
        }

        private static bool AreAssignable(System.Type dest, System.Type src) => 
            ((dest == src) || (dest.IsAssignableFrom(src) || (((dest.IsArray && src.IsArray) && ((dest.GetArrayRank() == src.GetArrayRank()) && AreReferenceAssignable(dest.GetElementType(), src.GetElementType()))) || (((src.IsArray && dest.IsGenericType) && (((dest.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || (dest.GetGenericTypeDefinition() == typeof(IList<>))) || (dest.GetGenericTypeDefinition() == typeof(ICollection<>)))) && (dest.GetGenericArguments()[0] == src.GetElementType())))));

        private static bool AreReferenceAssignable(System.Type dest, System.Type src) => 
            ((dest == src) || ((!dest.IsValueType && !src.IsValueType) && AreAssignable(dest, src)));

        public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
        {
            if (array == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("array");
            }
            if (indexes == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("indexes");
            }
            if (!array.Type.IsArray)
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeArray();
            }
            ReadOnlyCollection<Expression> arguments = indexes.ToReadOnlyCollection<Expression>();
            if (array.Type.GetArrayRank() != arguments.Count)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
            }
            foreach (Expression expression in arguments)
            {
                if (expression.Type != typeof(int))
                {
                    throw System.Linq.Expressions.Error.ArgumentMustBeArrayIndexType();
                }
            }
            MethodInfo method = array.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance);
            return Call(array, method, arguments);
        }

        public static BinaryExpression ArrayIndex(Expression array, Expression index)
        {
            if (array == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("array");
            }
            if (index == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("index");
            }
            if (index.Type != typeof(int))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeArrayIndexType();
            }
            if (!array.Type.IsArray)
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeArray();
            }
            if (array.Type.GetArrayRank() != 1)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
            }
            return new BinaryExpression(ExpressionType.ArrayIndex, array, index, array.Type.GetElementType());
        }

        public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes) => 
            ArrayIndex(array, (IEnumerable<Expression>) indexes);

        public static UnaryExpression ArrayLength(Expression array)
        {
            if (array == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("array");
            }
            if (!array.Type.IsArray || !AreAssignable(typeof(Array), array.Type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeArray();
            }
            if (array.Type.GetArrayRank() != 1)
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeSingleDimensionalArrayType();
            }
            return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int));
        }

        public static MemberAssignment Bind(MemberInfo member, Expression expression)
        {
            System.Type type;
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            ValidateSettableFieldOrPropertyMember(member, out type);
            if (!AreAssignable(type, expression.Type))
            {
                throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
            }
            return new MemberAssignment(member, expression);
        }

        public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            ValidateMethodInfo(propertyAccessor);
            return Bind(GetProperty(propertyAccessor), expression);
        }

        internal virtual void BuildString(StringBuilder builder)
        {
            if (builder == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("builder");
            }
            builder.Append("[");
            builder.Append(this.nodeType.ToString());
            builder.Append("]");
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method) => 
            Call(instance, method, (Expression[]) null);

        public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments) => 
            Call(null, method, arguments.ToReadOnlyCollection<Expression>());

        public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
        {
            ReadOnlyCollection<Expression> onlys = arguments.ToReadOnlyCollection<Expression>();
            ValidateCallArgs(instance, method, ref onlys);
            return new MethodCallExpression(ExpressionType.Call, method, instance, onlys);
        }

        public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments) => 
            Call(instance, method, arguments.ToReadOnlyCollection<Expression>());

        public static MethodCallExpression Call(Expression instance, string methodName, System.Type[] typeArguments, params Expression[] arguments)
        {
            if (instance == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("instance");
            }
            if (methodName == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("methodName");
            }
            if (arguments == null)
            {
                arguments = new Expression[0];
            }
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return Call(instance, FindMethod(instance.Type, methodName, typeArguments, arguments, flags), arguments);
        }

        public static MethodCallExpression Call(System.Type type, string methodName, System.Type[] typeArguments, params Expression[] arguments)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (methodName == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("methodName");
            }
            if (arguments == null)
            {
                arguments = new Expression[0];
            }
            BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            return Call(null, FindMethod(type, methodName, typeArguments, arguments, flags), arguments);
        }

        private static bool CheckMethod(MethodInfo method, MethodInfo propertyMethod)
        {
            if (method == propertyMethod)
            {
                return true;
            }
            System.Type declaringType = method.DeclaringType;
            return ((declaringType.IsInterface && (method.Name == propertyMethod.Name)) && (declaringType.GetMethod(method.Name) == propertyMethod));
        }

        public static BinaryExpression Coalesce(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            return new BinaryExpression(ExpressionType.Coalesce, left, right, ValidateCoalesceArgTypes(left.Type, right.Type));
        }

        public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (conversion == null)
            {
                return Coalesce(left, right);
            }
            if (left.Type.IsValueType && !IsNullableType(left.Type))
            {
                throw System.Linq.Expressions.Error.CoalesceUsedOnNonNullType();
            }
            MethodInfo method = conversion.Type.GetMethod("Invoke");
            if (method.ReturnType == typeof(void))
            {
                throw System.Linq.Expressions.Error.UserDefinedOperatorMustNotBeVoid(conversion);
            }
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 1)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(conversion);
            }
            if (method.ReturnType != right.Type)
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
            }
            if (!ParameterIsAssignable(parameters[0], GetNonNullableType(left.Type)) && !ParameterIsAssignable(parameters[0], left.Type))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(ExpressionType.Coalesce, conversion.ToString());
            }
            return new BinaryExpression(ExpressionType.Coalesce, left, right, conversion, right.Type);
        }

        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (test == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("test");
            }
            if (ifTrue == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("ifTrue");
            }
            if (ifFalse == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("ifFalse");
            }
            if (test.Type != typeof(bool))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeBoolean();
            }
            ValidateSameArgTypes(ifTrue.Type, ifFalse.Type);
            return new ConditionalExpression(test, ifTrue, ifFalse, ifTrue.Type);
        }

        public static ConstantExpression Constant(object value)
        {
            System.Type type = (value != null) ? value.GetType() : typeof(object);
            return Constant(value, type);
        }

        public static ConstantExpression Constant(object value, System.Type type)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (((value == null) && type.IsValueType) && !IsNullableType(type))
            {
                throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
            }
            if ((value != null) && !AreAssignable(type, value.GetType()))
            {
                throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
            }
            return new ConstantExpression(value, type);
        }

        public static UnaryExpression Convert(Expression expression, System.Type type)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (!HasIdentityPrimitiveOrNullableConversion(expression.Type, type) && !HasReferenceConversion(expression.Type, type))
            {
                return GetUserDefinedCoercionOrThrow(ExpressionType.Convert, expression, type);
            }
            return new UnaryExpression(ExpressionType.Convert, expression, type);
        }

        public static UnaryExpression Convert(Expression expression, System.Type type, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return Convert(expression, type);
            }
            return GetMethodBasedCoercionOperator(ExpressionType.Convert, expression, type, method);
        }

        public static UnaryExpression ConvertChecked(Expression expression, System.Type type)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (HasIdentityPrimitiveOrNullableConversion(expression.Type, type))
            {
                return new UnaryExpression(ExpressionType.ConvertChecked, expression, type);
            }
            if (HasReferenceConversion(expression.Type, type))
            {
                return new UnaryExpression(ExpressionType.Convert, expression, type);
            }
            return GetUserDefinedCoercionOrThrow(ExpressionType.ConvertChecked, expression, type);
        }

        public static UnaryExpression ConvertChecked(Expression expression, System.Type type, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return ConvertChecked(expression, type);
            }
            return GetMethodBasedCoercionOperator(ExpressionType.ConvertChecked, expression, type, method);
        }

        public static BinaryExpression Divide(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.Divide, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Divide, "op_Division", left, right, true);
        }

        public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Divide(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Divide, left, right, method, true);
        }

        public static System.Linq.Expressions.ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            if (addMethod == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("addMethod");
            }
            if (arguments == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("arguments");
            }
            ValidateElementInitAddMethodInfo(addMethod);
            ReadOnlyCollection<Expression> onlys = arguments.ToReadOnlyCollection<Expression>();
            ValidateArgumentTypes(addMethod, ref onlys);
            return new System.Linq.Expressions.ElementInit(addMethod, onlys);
        }

        public static System.Linq.Expressions.ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments) => 
            ElementInit(addMethod, (IEnumerable<Expression>) arguments);

        public static BinaryExpression Equal(Expression left, Expression right) => 
            Equal(left, right, false, null);

        public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetEqualityComparisonOperator(ExpressionType.Equal, "op_Equality", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Equal, left, right, method, liftToNull);
        }

        public static BinaryExpression ExclusiveOr(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsIntegerOrBool(left.Type))
            {
                return new BinaryExpression(ExpressionType.ExclusiveOr, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.ExclusiveOr, "op_ExclusiveOr", left, right, true);
        }

        public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return ExclusiveOr(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.ExclusiveOr, left, right, method, true);
        }

        public static MemberExpression Field(Expression expression, FieldInfo field)
        {
            if (field == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("field");
            }
            if (!field.IsStatic)
            {
                if (expression == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("expression");
                }
                if (!AreReferenceAssignable(field.DeclaringType, expression.Type))
                {
                    throw System.Linq.Expressions.Error.FieldNotDefinedForType(field, expression.Type);
                }
            }
            return new MemberExpression(expression, field, field.FieldType);
        }

        public static MemberExpression Field(Expression expression, string fieldName)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            FieldInfo field = expression.Type.GetField(fieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field == null)
            {
                field = expression.Type.GetField(fieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            if (field == null)
            {
                throw System.Linq.Expressions.Error.FieldNotDefinedForType(fieldName, expression.Type);
            }
            return Field(expression, field);
        }

        private static int FindBestMethod(IEnumerable<MethodInfo> methods, System.Type[] typeArgs, Expression[] args, out MethodInfo method)
        {
            int num = 0;
            method = null;
            foreach (MethodInfo info in methods)
            {
                MethodInfo m = ApplyTypeArgs(info, typeArgs);
                if ((m != null) && IsCompatible(m, args))
                {
                    if ((method == null) || (!method.IsPublic && m.IsPublic))
                    {
                        method = m;
                        num = 1;
                    }
                    else if (method.IsPublic == m.IsPublic)
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        private static MethodInfo FindConversionOperator(MethodInfo[] methods, System.Type typeFrom, System.Type typeTo)
        {
            foreach (MethodInfo info in methods)
            {
                if (((info.Name == "op_Implicit") || (info.Name == "op_Explicit")) && ((info.ReturnType == typeTo) && (info.GetParameters()[0].ParameterType == typeFrom)))
                {
                    return info;
                }
            }
            return null;
        }

        private static MethodInfo FindMethod(System.Type type, string methodName, System.Type[] typeArgs, Expression[] args, BindingFlags flags)
        {
            MethodInfo info;
            MemberInfo[] source = type.FindMembers(MemberTypes.Method, flags, System.Type.FilterNameIgnoreCase, methodName);
            if ((source == null) || (source.Length == 0))
            {
                throw System.Linq.Expressions.Error.MethodDoesNotExistOnType(methodName, type);
            }
            int num = FindBestMethod(source.Cast<MethodInfo>(), typeArgs, args, out info);
            if (num == 0)
            {
                throw System.Linq.Expressions.Error.MethodWithArgsDoesNotExistOnType(methodName, type);
            }
            if (num > 1)
            {
                throw System.Linq.Expressions.Error.MethodWithMoreThanOneMatch(methodName, type);
            }
            return info;
        }

        public static System.Type GetActionType(params System.Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("typeArgs");
            }
            if (typeArgs.Length >= actionTypes.Length)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfTypeArgsForAction();
            }
            if (typeArgs.Length == 0)
            {
                return actionTypes[typeArgs.Length];
            }
            return actionTypes[typeArgs.Length].MakeGenericType(typeArgs);
        }

        private static BinaryExpression GetComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
        {
            if ((left.Type != right.Type) || !IsNumeric(left.Type))
            {
                return GetUserDefinedBinaryOperatorOrThrow(binaryType, opName, left, right, liftToNull);
            }
            if (IsNullableType(left.Type) && liftToNull)
            {
                return new BinaryExpression(binaryType, left, right, typeof(bool?));
            }
            return new BinaryExpression(binaryType, left, right, typeof(bool));
        }

        private static BinaryExpression GetEqualityComparisonOperator(ExpressionType binaryType, string opName, Expression left, Expression right, bool liftToNull)
        {
            if ((left.Type == right.Type) && (IsNumeric(left.Type) || (left.Type == typeof(object))))
            {
                if (IsNullableType(left.Type) && liftToNull)
                {
                    return new BinaryExpression(binaryType, left, right, typeof(bool?));
                }
                return new BinaryExpression(binaryType, left, right, typeof(bool));
            }
            BinaryExpression expression = GetUserDefinedBinaryOperator(binaryType, opName, left, right, liftToNull);
            if (expression != null)
            {
                return expression;
            }
            if (!HasBuiltInEqualityOperator(left.Type, right.Type) && !IsNullComparison(left, right))
            {
                throw System.Linq.Expressions.Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
            }
            if (IsNullableType(left.Type) && liftToNull)
            {
                return new BinaryExpression(binaryType, left, right, typeof(bool?));
            }
            return new BinaryExpression(binaryType, left, right, typeof(bool));
        }

        public static System.Type GetFuncType(params System.Type[] typeArgs)
        {
            if (typeArgs == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("typeArgs");
            }
            if ((typeArgs.Length < 1) || (typeArgs.Length > 5))
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfTypeArgsForFunc();
            }
            return funcTypes[typeArgs.Length - 1].MakeGenericType(typeArgs);
        }

        private static BinaryExpression GetMethodBasedBinaryOperator(ExpressionType binaryType, Expression left, Expression right, MethodInfo method, bool liftToNull)
        {
            ValidateOperator(method);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 2)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
            }
            if (ParameterIsAssignable(parameters[0], left.Type) && ParameterIsAssignable(parameters[1], right.Type))
            {
                ValidateParamswithOperandsOrThrow(parameters[0].ParameterType, left.Type, binaryType, method.Name);
                ValidateParamswithOperandsOrThrow(parameters[1].ParameterType, right.Type, binaryType, method.Name);
                return new BinaryExpression(binaryType, left, right, method, method.ReturnType);
            }
            if (((!IsNullableType(left.Type) || !IsNullableType(right.Type)) || (!ParameterIsAssignable(parameters[0], GetNonNullableType(left.Type)) || !ParameterIsAssignable(parameters[1], GetNonNullableType(right.Type)))) || (!method.ReturnType.IsValueType || IsNullableType(method.ReturnType)))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(binaryType, method.Name);
            }
            if ((method.ReturnType == typeof(bool)) && !liftToNull)
            {
                return new BinaryExpression(binaryType, left, right, method, typeof(bool));
            }
            return new BinaryExpression(binaryType, left, right, method, GetNullableType(method.ReturnType));
        }

        private static UnaryExpression GetMethodBasedCoercionOperator(ExpressionType unaryType, Expression operand, System.Type convertToType, MethodInfo method)
        {
            ValidateOperator(method);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 1)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
            }
            if (ParameterIsAssignable(parameters[0], operand.Type) && (method.ReturnType == convertToType))
            {
                return new UnaryExpression(unaryType, operand, method, method.ReturnType);
            }
            if ((!IsNullableType(operand.Type) && !IsNullableType(convertToType)) || (!ParameterIsAssignable(parameters[0], GetNonNullableType(operand.Type)) || (method.ReturnType != GetNonNullableType(convertToType))))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(unaryType, method.Name);
            }
            return new UnaryExpression(unaryType, operand, method, convertToType);
        }

        private static UnaryExpression GetMethodBasedUnaryOperator(ExpressionType unaryType, Expression operand, MethodInfo method)
        {
            ValidateOperator(method);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 1)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
            }
            if (ParameterIsAssignable(parameters[0], operand.Type))
            {
                ValidateParamswithOperandsOrThrow(parameters[0].ParameterType, operand.Type, unaryType, method.Name);
                return new UnaryExpression(unaryType, operand, method, method.ReturnType);
            }
            if ((!IsNullableType(operand.Type) || !ParameterIsAssignable(parameters[0], GetNonNullableType(operand.Type))) || (!method.ReturnType.IsValueType || IsNullableType(method.ReturnType)))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(unaryType, method.Name);
            }
            return new UnaryExpression(unaryType, operand, method, GetNullableType(method.ReturnType));
        }

        internal static System.Type GetNonNullableType(System.Type type)
        {
            if (IsNullableType(type))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static System.Type GetNullableType(System.Type type)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (type.IsValueType && !IsNullableType(type))
            {
                return typeof(Nullable<>).MakeGenericType(new System.Type[] { type });
            }
            return type;
        }

        private static PropertyInfo GetProperty(MethodInfo mi)
        {
            System.Type declaringType = mi.DeclaringType;
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public;
            bindingAttr |= mi.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            foreach (PropertyInfo info in declaringType.GetProperties(bindingAttr))
            {
                if (info.CanRead && CheckMethod(mi, info.GetGetMethod(true)))
                {
                    return info;
                }
                if (info.CanWrite && CheckMethod(mi, info.GetSetMethod(true)))
                {
                    return info;
                }
            }
            throw System.Linq.Expressions.Error.MethodNotPropertyAccessor(mi.DeclaringType, mi.Name);
        }

        private static MethodInfo GetUserDefinedBinaryOperator(ExpressionType binaryType, System.Type leftType, System.Type rightType, string name)
        {
            System.Type[] types = new System.Type[] { leftType, rightType };
            System.Type nonNullableType = GetNonNullableType(leftType);
            System.Type type2 = GetNonNullableType(rightType);
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            MethodInfo method = nonNullableType.GetMethod(name, bindingAttr, null, types, null);
            if (method == null)
            {
                method = type2.GetMethod(name, bindingAttr, null, types, null);
            }
            if (IsLiftingConditionalLogicalOperator(leftType, rightType, method, binaryType))
            {
                method = GetUserDefinedBinaryOperator(binaryType, nonNullableType, type2, name);
            }
            return method;
        }

        private static BinaryExpression GetUserDefinedBinaryOperator(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
        {
            MethodInfo method = GetUserDefinedBinaryOperator(binaryType, left.Type, right.Type, name);
            if (method != null)
            {
                return new BinaryExpression(binaryType, left, right, method, method.ReturnType);
            }
            if (IsNullableType(left.Type) && IsNullableType(right.Type))
            {
                System.Type nonNullableType = GetNonNullableType(left.Type);
                System.Type rightType = GetNonNullableType(right.Type);
                method = GetUserDefinedBinaryOperator(binaryType, nonNullableType, rightType, name);
                if (((method != null) && method.ReturnType.IsValueType) && !IsNullableType(method.ReturnType))
                {
                    if ((method.ReturnType == typeof(bool)) && !liftToNull)
                    {
                        return new BinaryExpression(binaryType, left, right, method, typeof(bool));
                    }
                    return new BinaryExpression(binaryType, left, right, method, GetNullableType(method.ReturnType));
                }
            }
            return null;
        }

        private static BinaryExpression GetUserDefinedBinaryOperatorOrThrow(ExpressionType binaryType, string name, Expression left, Expression right, bool liftToNull)
        {
            BinaryExpression expression = GetUserDefinedBinaryOperator(binaryType, name, left, right, liftToNull);
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.BinaryOperatorNotDefined(binaryType, left.Type, right.Type);
            }
            ValidateParamswithOperandsOrThrow(expression.Method.GetParameters()[0].ParameterType, left.Type, binaryType, name);
            ValidateParamswithOperandsOrThrow(expression.Method.GetParameters()[1].ParameterType, right.Type, binaryType, name);
            return expression;
        }

        private static UnaryExpression GetUserDefinedCoercion(ExpressionType coercionType, Expression expression, System.Type convertToType)
        {
            System.Type nonNullableType = GetNonNullableType(expression.Type);
            System.Type typeTo = GetNonNullableType(convertToType);
            MethodInfo[] methods = nonNullableType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo method = FindConversionOperator(methods, expression.Type, convertToType);
            if (method != null)
            {
                return new UnaryExpression(coercionType, expression, method, convertToType);
            }
            MethodInfo[] infoArray2 = typeTo.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            method = FindConversionOperator(infoArray2, expression.Type, convertToType);
            if (method != null)
            {
                return new UnaryExpression(coercionType, expression, method, convertToType);
            }
            if ((nonNullableType != expression.Type) || (typeTo != convertToType))
            {
                method = FindConversionOperator(methods, nonNullableType, typeTo);
                if (method == null)
                {
                    method = FindConversionOperator(infoArray2, nonNullableType, typeTo);
                }
                if (method != null)
                {
                    return new UnaryExpression(coercionType, expression, method, convertToType);
                }
            }
            return null;
        }

        private static UnaryExpression GetUserDefinedCoercionOrThrow(ExpressionType coercionType, Expression expression, System.Type convertToType)
        {
            UnaryExpression expression2 = GetUserDefinedCoercion(coercionType, expression, convertToType);
            if (expression2 == null)
            {
                throw System.Linq.Expressions.Error.CoercionOperatorNotDefined(expression.Type, convertToType);
            }
            return expression2;
        }

        private static UnaryExpression GetUserDefinedUnaryOperator(ExpressionType unaryType, string name, Expression operand)
        {
            System.Type type = operand.Type;
            System.Type[] types = new System.Type[] { type };
            System.Type nonNullableType = GetNonNullableType(type);
            MethodInfo method = nonNullableType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            if (method != null)
            {
                return new UnaryExpression(unaryType, operand, method, method.ReturnType);
            }
            if (IsNullableType(type))
            {
                types[0] = nonNullableType;
                method = nonNullableType.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
                if (((method != null) && method.ReturnType.IsValueType) && !IsNullableType(method.ReturnType))
                {
                    return new UnaryExpression(unaryType, operand, method, GetNullableType(method.ReturnType));
                }
            }
            return null;
        }

        private static UnaryExpression GetUserDefinedUnaryOperatorOrThrow(ExpressionType unaryType, string name, Expression operand)
        {
            UnaryExpression expression = GetUserDefinedUnaryOperator(unaryType, name, operand);
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.UnaryOperatorNotDefined(unaryType, operand.Type);
            }
            ValidateParamswithOperandsOrThrow(expression.Method.GetParameters()[0].ParameterType, operand.Type, unaryType, name);
            return expression;
        }

        public static BinaryExpression GreaterThan(Expression left, Expression right) => 
            GreaterThan(left, right, false, null);

        public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.GreaterThan, "op_GreaterThan", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.GreaterThan, left, right, method, liftToNull);
        }

        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right) => 
            GreaterThanOrEqual(left, right, false, null);

        public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.GreaterThanOrEqual, "op_GreaterThanOrEqual", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.GreaterThanOrEqual, left, right, method, liftToNull);
        }

        private static bool HasBuiltInEqualityOperator(System.Type left, System.Type right)
        {
            if (!left.IsInterface || right.IsValueType)
            {
                if (right.IsInterface && !left.IsValueType)
                {
                    return true;
                }
                if ((!left.IsValueType && !right.IsValueType) && (AreReferenceAssignable(left, right) || AreReferenceAssignable(right, left)))
                {
                    return true;
                }
                if (left != right)
                {
                    return false;
                }
                System.Type nonNullableType = GetNonNullableType(left);
                if (((nonNullableType != typeof(bool)) && !IsNumeric(nonNullableType)) && !nonNullableType.IsEnum)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HasIdentityPrimitiveOrNullableConversion(System.Type source, System.Type dest) => 
            ((source == dest) || ((IsNullableType(source) && (dest == GetNonNullableType(source))) || ((IsNullableType(dest) && (source == GetNonNullableType(dest))) || ((IsConvertible(source) && IsConvertible(dest)) && (GetNonNullableType(dest) != typeof(bool))))));

        private static bool HasReferenceConversion(System.Type source, System.Type dest)
        {
            System.Type nonNullableType = GetNonNullableType(source);
            System.Type src = GetNonNullableType(dest);
            if (!AreAssignable(nonNullableType, src))
            {
                if (AreAssignable(src, nonNullableType))
                {
                    return true;
                }
                if (source.IsInterface || dest.IsInterface)
                {
                    return true;
                }
                if ((source != typeof(object)) && (dest != typeof(object)))
                {
                    return false;
                }
            }
            return true;
        }

        public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            System.Type type = expression.Type;
            if (type == typeof(Delegate))
            {
                throw System.Linq.Expressions.Error.ExpressionTypeNotInvocable(type);
            }
            if (!AreAssignable(typeof(Delegate), expression.Type))
            {
                System.Type type2 = TypeHelper.FindGenericType(typeof(Expression<>), expression.Type);
                if (type2 == null)
                {
                    throw System.Linq.Expressions.Error.ExpressionTypeNotInvocable(expression.Type);
                }
                type = type2.GetGenericArguments()[0];
            }
            MethodInfo method = type.GetMethod("Invoke");
            ParameterInfo[] parameters = method.GetParameters();
            ReadOnlyCollection<Expression> onlys = arguments.ToReadOnlyCollection<Expression>();
            if (parameters.Length > 0)
            {
                if (onlys.Count != parameters.Length)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaArguments();
                }
                List<Expression> sequence = null;
                int index = 0;
                int count = onlys.Count;
                while (index < count)
                {
                    Expression expression2 = onlys[index];
                    ParameterInfo info2 = parameters[index];
                    if (expression2 == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("arguments");
                    }
                    System.Type parameterType = info2.ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    if (!AreReferenceAssignable(parameterType, expression2.Type))
                    {
                        if (!IsSameOrSubclass(typeof(Expression), parameterType) || !AreAssignable(parameterType, expression2.GetType()))
                        {
                            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchParameter(expression2.Type, parameterType);
                        }
                        expression2 = Quote(expression2);
                    }
                    if ((sequence == null) && (expression2 != onlys[index]))
                    {
                        sequence = new List<Expression>(onlys.Count);
                        for (int i = 0; i < index; i++)
                        {
                            sequence.Add(onlys[i]);
                        }
                    }
                    if (sequence != null)
                    {
                        sequence.Add(expression2);
                    }
                    index++;
                }
                if (sequence != null)
                {
                    onlys = sequence.ToReadOnlyCollection<Expression>();
                }
            }
            else if (onlys.Count > 0)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaArguments();
            }
            return new InvocationExpression(expression, method.ReturnType, onlys);
        }

        public static InvocationExpression Invoke(Expression expression, params Expression[] arguments) => 
            Invoke(expression, arguments.ToReadOnlyCollection<Expression>());

        private static bool IsArithmetic(System.Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
            }
            return false;
        }

        private static bool IsBool(System.Type type)
        {
            type = GetNonNullableType(type);
            return (type == typeof(bool));
        }

        private static bool IsCompatible(MethodInfo m, Expression[] args)
        {
            ParameterInfo[] parameters = m.GetParameters();
            if (parameters.Length != args.Length)
            {
                return false;
            }
            for (int i = 0; i < args.Length; i++)
            {
                Expression expression = args[i];
                if (expression == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("argument");
                }
                System.Type src = expression.Type;
                System.Type parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }
                if (!AreReferenceAssignable(parameterType, src) && (!IsSameOrSubclass(typeof(Expression), parameterType) || !AreAssignable(parameterType, expression.GetType())))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsConvertible(System.Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return true;
            }
            switch (System.Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
            }
            return false;
        }

        private static bool IsIdentityConversion(System.Type source, System.Type destination) => 
            (source == destination);

        private static bool IsImplicitBoxingConversion(System.Type source, System.Type destination) => 
            ((source.IsValueType && ((destination == typeof(object)) || (destination == typeof(ValueType)))) || (source.IsEnum && (destination == typeof(Enum))));

        private static bool IsImplicitlyConvertible(System.Type source, System.Type destination)
        {
            if ((!IsIdentityConversion(source, destination) && !IsImplicitNumericConversion(source, destination)) && (!IsImplicitReferenceConversion(source, destination) && !IsImplicitBoxingConversion(source, destination)))
            {
                return IsImplicitNullableConversion(source, destination);
            }
            return true;
        }

        private static bool IsImplicitNullableConversion(System.Type source, System.Type destination) => 
            (IsNullableType(destination) && IsImplicitlyConvertible(GetNonNullableType(source), GetNonNullableType(destination)));

        private static bool IsImplicitNumericConversion(System.Type source, System.Type destination)
        {
            TypeCode typeCode = System.Type.GetTypeCode(source);
            TypeCode code2 = System.Type.GetTypeCode(destination);
            switch (typeCode)
            {
                case TypeCode.Char:
                    switch (code2)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.SByte:
                    switch (code2)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;

                case TypeCode.Byte:
                    switch (code2)
                    {
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int16:
                    switch (code2)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.UInt16:
                    switch (code2)
                    {
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int32:
                    switch (code2)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.UInt32:
                    switch (code2)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Int64:
                case TypeCode.UInt64:
                    switch (code2)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    return false;

                case TypeCode.Single:
                    return (code2 == TypeCode.Double);

                default:
                    return false;
            }
            return false;
        }

        private static bool IsImplicitReferenceConversion(System.Type source, System.Type destination) => 
            AreAssignable(destination, source);

        private static bool IsInteger(System.Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        private static bool IsIntegerOrBool(System.Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        private static bool IsLiftingConditionalLogicalOperator(System.Type left, System.Type right, MethodInfo method, ExpressionType binaryType)
        {
            if ((!IsNullableType(right) || !IsNullableType(left)) || (method != null))
            {
                return false;
            }
            if (binaryType != ExpressionType.AndAlso)
            {
                return (binaryType == ExpressionType.OrElse);
            }
            return true;
        }

        internal static bool IsNullableType(System.Type type) => 
            (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        private static bool IsNullComparison(Expression left, Expression right) => 
            (((IsNullConstant(left) && !IsNullConstant(right)) && IsNullableType(right.Type)) || ((IsNullConstant(right) && !IsNullConstant(left)) && IsNullableType(left.Type)));

        private static bool IsNullConstant(Expression expr)
        {
            ConstantExpression expression = expr as ConstantExpression;
            return (expression?.Value == null);
        }

        private static bool IsNumeric(System.Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
            }
            return false;
        }

        private static bool IsSameOrSubclass(System.Type type, System.Type subType)
        {
            if (type != subType)
            {
                return subType.IsSubclassOf(type);
            }
            return true;
        }

        private static bool IsUnSigned(System.Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        return true;
                }
            }
            return false;
        }

        private static bool IsValidLiftedConditionalLogicalOperator(System.Type left, System.Type right, ParameterInfo[] pms) => 
            (((left == right) && IsNullableType(right)) && (pms[1].ParameterType == GetNonNullableType(right)));

        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (body == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("body");
            }
            ReadOnlyCollection<ParameterExpression> onlys = parameters.ToReadOnlyCollection<ParameterExpression>();
            ValidateLambdaArgs(typeof(TDelegate), ref body, onlys);
            return new Expression<TDelegate>(body, onlys);
        }

        public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters) => 
            Lambda<TDelegate>(body, parameters.ToReadOnlyCollection<ParameterExpression>());

        public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
        {
            System.Type actionType;
            if (body == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("body");
            }
            bool flag = body.Type == typeof(void);
            int index = (parameters == null) ? 0 : parameters.Length;
            System.Type[] typeArgs = new System.Type[index + (flag ? 0 : 1)];
            for (int i = 0; i < index; i++)
            {
                if (parameters[i] == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("parameter");
                }
                typeArgs[i] = parameters[i].Type;
            }
            if (flag)
            {
                actionType = GetActionType(typeArgs);
            }
            else
            {
                typeArgs[index] = body.Type;
                actionType = GetFuncType(typeArgs);
            }
            return Lambda(actionType, body, parameters);
        }

        public static LambdaExpression Lambda(System.Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if (delegateType == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("delegateType");
            }
            if (body == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("body");
            }
            ReadOnlyCollection<ParameterExpression> onlys = parameters.ToReadOnlyCollection<ParameterExpression>();
            ValidateLambdaArgs(delegateType, ref body, onlys);
            return (LambdaExpression) typeof(Expression).GetMethod("Lambda", BindingFlags.Public | BindingFlags.Static, null, lambdaTypes, null).MakeGenericMethod(new System.Type[] { delegateType }).Invoke(null, new object[] { body, onlys });
        }

        public static LambdaExpression Lambda(System.Type delegateType, Expression body, params ParameterExpression[] parameters) => 
            Lambda(delegateType, body, parameters.ToReadOnlyCollection<ParameterExpression>());

        public static BinaryExpression LeftShift(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (IsInteger(left.Type) && (GetNonNullableType(right.Type) == typeof(int)))
            {
                return new BinaryExpression(ExpressionType.LeftShift, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.LeftShift, "op_LeftShift", left, right, true);
        }

        public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return LeftShift(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LeftShift, left, right, method, true);
        }

        public static BinaryExpression LessThan(Expression left, Expression right) => 
            LessThan(left, right, false, null);

        public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.LessThan, "op_LessThan", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LessThan, left, right, method, liftToNull);
        }

        public static BinaryExpression LessThanOrEqual(Expression left, Expression right) => 
            LessThanOrEqual(left, right, false, null);

        public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetComparisonOperator(ExpressionType.LessThanOrEqual, "op_LessThanOrEqual", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.LessThanOrEqual, left, right, method, liftToNull);
        }

        public static MemberListBinding ListBind(MemberInfo member, IEnumerable<System.Linq.Expressions.ElementInit> initializers)
        {
            System.Type type;
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            ValidateGettableFieldOrPropertyMember(member, out type);
            ReadOnlyCollection<System.Linq.Expressions.ElementInit> onlys = initializers.ToReadOnlyCollection<System.Linq.Expressions.ElementInit>();
            ValidateListInitArgs(type, onlys);
            return new MemberListBinding(member, onlys);
        }

        public static MemberListBinding ListBind(MemberInfo member, params System.Linq.Expressions.ElementInit[] initializers)
        {
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            return ListBind(member, initializers.ToReadOnlyCollection<System.Linq.Expressions.ElementInit>());
        }

        public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<System.Linq.Expressions.ElementInit> initializers)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            return ListBind(GetProperty(propertyAccessor), initializers);
        }

        public static MemberListBinding ListBind(MethodInfo propertyAccessor, params System.Linq.Expressions.ElementInit[] initializers)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            return ListBind(propertyAccessor, initializers.ToReadOnlyCollection<System.Linq.Expressions.ElementInit>());
        }

        public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<System.Linq.Expressions.ElementInit> initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (!initializers.Any<System.Linq.Expressions.ElementInit>())
            {
                throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
            }
            ReadOnlyCollection<System.Linq.Expressions.ElementInit> onlys = initializers.ToReadOnlyCollection<System.Linq.Expressions.ElementInit>();
            ValidateListInitArgs(newExpression.Type, onlys);
            return new ListInitExpression(newExpression, onlys);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (!initializers.Any<Expression>())
            {
                throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
            }
            MethodInfo addMethod = FindMethod(newExpression.Type, "Add", null, new Expression[] { initializers.First<Expression>() }, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return ListInit(newExpression, addMethod, initializers);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, params System.Linq.Expressions.ElementInit[] initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            return ListInit(newExpression, initializers.ToReadOnlyCollection<System.Linq.Expressions.ElementInit>());
        }

        public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            return ListInit(newExpression, (IEnumerable<Expression>) initializers);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (!initializers.Any<Expression>())
            {
                throw System.Linq.Expressions.Error.ListInitializerWithZeroMembers();
            }
            if (addMethod == null)
            {
                return ListInit(newExpression, initializers);
            }
            List<System.Linq.Expressions.ElementInit> list = new List<System.Linq.Expressions.ElementInit>();
            foreach (Expression expression in initializers)
            {
                list.Add(ElementInit(addMethod, new Expression[] { expression }));
            }
            return ListInit(newExpression, list);
        }

        public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (addMethod == null)
            {
                return ListInit(newExpression, (IEnumerable<Expression>) initializers);
            }
            return ListInit(newExpression, addMethod, (IEnumerable<Expression>) initializers);
        }

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right) => 
            MakeBinary(binaryType, left, right, false, null);

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method) => 
            MakeBinary(binaryType, left, right, liftToNull, method, null);

        public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
        {
            switch (binaryType)
            {
                case ExpressionType.Add:
                    return Add(left, right, method);

                case ExpressionType.AddChecked:
                    return AddChecked(left, right, method);

                case ExpressionType.And:
                    return And(left, right, method);

                case ExpressionType.AndAlso:
                    return AndAlso(left, right);

                case ExpressionType.ArrayIndex:
                    return ArrayIndex(left, right);

                case ExpressionType.Coalesce:
                    return Coalesce(left, right, conversion);

                case ExpressionType.Divide:
                    return Divide(left, right, method);

                case ExpressionType.Equal:
                    return Equal(left, right, liftToNull, method);

                case ExpressionType.ExclusiveOr:
                    return ExclusiveOr(left, right, method);

                case ExpressionType.GreaterThan:
                    return GreaterThan(left, right, liftToNull, method);

                case ExpressionType.GreaterThanOrEqual:
                    return GreaterThanOrEqual(left, right, liftToNull, method);

                case ExpressionType.LeftShift:
                    return LeftShift(left, right, method);

                case ExpressionType.LessThan:
                    return LessThan(left, right, liftToNull, method);

                case ExpressionType.LessThanOrEqual:
                    return LessThanOrEqual(left, right, liftToNull, method);

                case ExpressionType.Modulo:
                    return Modulo(left, right, method);

                case ExpressionType.Multiply:
                    return Multiply(left, right, method);

                case ExpressionType.MultiplyChecked:
                    return MultiplyChecked(left, right, method);

                case ExpressionType.NotEqual:
                    return NotEqual(left, right, liftToNull, method);

                case ExpressionType.Or:
                    return Or(left, right, method);

                case ExpressionType.OrElse:
                    return OrElse(left, right);

                case ExpressionType.Power:
                    return Power(left, right, method);

                case ExpressionType.RightShift:
                    return RightShift(left, right, method);

                case ExpressionType.Subtract:
                    return Subtract(left, right, method);

                case ExpressionType.SubtractChecked:
                    return SubtractChecked(left, right, method);
            }
            throw System.Linq.Expressions.Error.UnhandledBinary(binaryType);
        }

        public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
        {
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                return Field(expression, field);
            }
            PropertyInfo property = member as PropertyInfo;
            if (property == null)
            {
                throw System.Linq.Expressions.Error.MemberNotFieldOrProperty(member);
            }
            return Property(expression, property);
        }

        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, System.Type type) => 
            MakeUnary(unaryType, operand, type, null);

        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, System.Type type, MethodInfo method)
        {
            switch (unaryType)
            {
                case ExpressionType.Convert:
                    return Convert(operand, type, method);

                case ExpressionType.ConvertChecked:
                    return ConvertChecked(operand, type, method);

                case ExpressionType.ArrayLength:
                    return ArrayLength(operand);

                case ExpressionType.Negate:
                    return Negate(operand, method);

                case ExpressionType.NegateChecked:
                    return NegateChecked(operand, method);

                case ExpressionType.Not:
                    return Not(operand, method);

                case ExpressionType.Quote:
                    return Quote(operand);

                case ExpressionType.TypeAs:
                    return TypeAs(operand, type);
            }
            throw System.Linq.Expressions.Error.UnhandledUnary(unaryType);
        }

        public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
        {
            System.Type type;
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            if (bindings == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bindings");
            }
            ReadOnlyCollection<MemberBinding> onlys = bindings.ToReadOnlyCollection<MemberBinding>();
            ValidateGettableFieldOrPropertyMember(member, out type);
            ValidateMemberInitArgs(type, onlys);
            return new MemberMemberBinding(member, onlys);
        }

        public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
        {
            if (member == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("member");
            }
            if (bindings == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bindings");
            }
            return MemberBind(member, bindings.ToReadOnlyCollection<MemberBinding>());
        }

        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            return MemberBind(GetProperty(propertyAccessor), bindings);
        }

        public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            return MemberBind(GetProperty(propertyAccessor), bindings);
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (bindings == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bindings");
            }
            ReadOnlyCollection<MemberBinding> onlys = bindings.ToReadOnlyCollection<MemberBinding>();
            ValidateMemberInitArgs(newExpression.Type, onlys);
            return new MemberInitExpression(newExpression, onlys);
        }

        public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
        {
            if (newExpression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("newExpression");
            }
            if (bindings == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bindings");
            }
            return MemberInit(newExpression, bindings.ToReadOnlyCollection<MemberBinding>());
        }

        public static BinaryExpression Modulo(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.Modulo, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Modulo, "op_Modulus", left, right, true);
        }

        public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Modulo(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Modulo, left, right, method, true);
        }

        public static BinaryExpression Multiply(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.Multiply, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Multiply, "op_Multiply", left, right, true);
        }

        public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Multiply(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Multiply, left, right, method, true);
        }

        public static BinaryExpression MultiplyChecked(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.MultiplyChecked, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.MultiplyChecked, "op_Multiply", left, right, true);
        }

        public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return MultiplyChecked(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.MultiplyChecked, left, right, method, true);
        }

        public static UnaryExpression Negate(Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (IsArithmetic(expression.Type) && !IsUnSigned(expression.Type))
            {
                return new UnaryExpression(ExpressionType.Negate, expression, expression.Type);
            }
            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Negate, "op_UnaryNegation", expression);
        }

        public static UnaryExpression Negate(Expression expression, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return Negate(expression);
            }
            return GetMethodBasedUnaryOperator(ExpressionType.Negate, expression, method);
        }

        public static UnaryExpression NegateChecked(Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (IsArithmetic(expression.Type) && !IsUnSigned(expression.Type))
            {
                return new UnaryExpression(ExpressionType.NegateChecked, expression, expression.Type);
            }
            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.NegateChecked, "op_UnaryNegation", expression);
        }

        public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return NegateChecked(expression);
            }
            return GetMethodBasedUnaryOperator(ExpressionType.NegateChecked, expression, method);
        }

        public static NewExpression New(ConstructorInfo constructor) => 
            New(constructor, null.ToReadOnlyCollection<Expression>());

        public static NewExpression New(System.Type type)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (type == typeof(void))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            ConstructorInfo constructor = null;
            if (!type.IsValueType)
            {
                constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, System.Type.EmptyTypes, null);
                if (constructor == null)
                {
                    throw System.Linq.Expressions.Error.TypeMissingDefaultConstructor(type);
                }
                return New(constructor);
            }
            return new NewExpression(type, null, null.ToReadOnlyCollection<Expression>());
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
        {
            if (constructor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("constructor");
            }
            ReadOnlyCollection<Expression> onlys = arguments.ToReadOnlyCollection<Expression>();
            ValidateNewArgs(constructor.DeclaringType, constructor, ref onlys);
            return new NewExpression(constructor.DeclaringType, constructor, onlys);
        }

        public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments) => 
            New(constructor, arguments.ToReadOnlyCollection<Expression>());

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
        {
            if (constructor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("constructor");
            }
            ReadOnlyCollection<MemberInfo> onlys = members.ToReadOnlyCollection<MemberInfo>();
            ReadOnlyCollection<Expression> onlys2 = arguments.ToReadOnlyCollection<Expression>();
            ValidateNewArgs(constructor, ref onlys2, onlys);
            return new NewExpression(constructor.DeclaringType, constructor, onlys2, onlys);
        }

        public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members) => 
            New(constructor, arguments, members.ToReadOnlyCollection<MemberInfo>());

        public static NewArrayExpression NewArrayBounds(System.Type type, IEnumerable<Expression> bounds)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (bounds == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bounds");
            }
            if (type.Equals(typeof(void)))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            ReadOnlyCollection<Expression> expressions = bounds.ToReadOnlyCollection<Expression>();
            int num = 0;
            int count = expressions.Count;
            while (num < count)
            {
                Expression expression = expressions[num];
                if (expression == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("bounds");
                }
                ValidateIntegerArg(expression.Type);
                num++;
            }
            return new NewArrayExpression(ExpressionType.NewArrayBounds, type.MakeArrayType(expressions.Count), expressions);
        }

        public static NewArrayExpression NewArrayBounds(System.Type type, params Expression[] bounds)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (bounds == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("bounds");
            }
            if (type.Equals(typeof(void)))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            return NewArrayBounds(type, bounds.ToReadOnlyCollection<Expression>());
        }

        public static NewArrayExpression NewArrayInit(System.Type type, IEnumerable<Expression> initializers)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (type.Equals(typeof(void)))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            ReadOnlyCollection<Expression> expressions = initializers.ToReadOnlyCollection<Expression>();
            List<Expression> sequence = null;
            int num = 0;
            int count = expressions.Count;
            while (num < count)
            {
                Expression expression = expressions[num];
                if (expression == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("initializers");
                }
                if (!AreReferenceAssignable(type, expression.Type))
                {
                    if (!IsSameOrSubclass(typeof(Expression), type) || !AreAssignable(type, expression.GetType()))
                    {
                        throw System.Linq.Expressions.Error.ExpressionTypeCannotInitializeArrayType(expression.Type, type);
                    }
                    expression = Quote(expression);
                }
                if ((sequence == null) && (expression != expressions[num]))
                {
                    sequence = new List<Expression>(expressions.Count);
                    for (int i = 0; i < num; i++)
                    {
                        sequence.Add(expressions[i]);
                    }
                }
                if (sequence != null)
                {
                    sequence.Add(expression);
                }
                num++;
            }
            if (sequence != null)
            {
                expressions = sequence.ToReadOnlyCollection<Expression>();
            }
            return new NewArrayExpression(ExpressionType.NewArrayInit, type.MakeArrayType(), expressions);
        }

        public static NewArrayExpression NewArrayInit(System.Type type, params Expression[] initializers)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (initializers == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("initializers");
            }
            if (type.Equals(typeof(void)))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            return NewArrayInit(type, initializers.ToReadOnlyCollection<Expression>());
        }

        public static UnaryExpression Not(Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (IsIntegerOrBool(expression.Type))
            {
                return new UnaryExpression(ExpressionType.Not, expression, expression.Type);
            }
            UnaryExpression expression2 = GetUserDefinedUnaryOperator(ExpressionType.Not, "op_LogicalNot", expression);
            if (expression2 != null)
            {
                return expression2;
            }
            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Not, "op_OnesComplement", expression);
        }

        public static UnaryExpression Not(Expression expression, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return Not(expression);
            }
            return GetMethodBasedUnaryOperator(ExpressionType.Not, expression, method);
        }

        public static BinaryExpression NotEqual(Expression left, Expression right) => 
            NotEqual(left, right, false, null);

        public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return GetEqualityComparisonOperator(ExpressionType.NotEqual, "op_Inequality", left, right, liftToNull);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.NotEqual, left, right, method, liftToNull);
        }

        public static BinaryExpression Or(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsIntegerOrBool(left.Type))
            {
                return new BinaryExpression(ExpressionType.Or, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Or, "op_BitwiseOr", left, right, true);
        }

        public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Or(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Or, left, right, method, true);
        }

        public static BinaryExpression OrElse(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsBool(left.Type))
            {
                return new BinaryExpression(ExpressionType.OrElse, left, right, left.Type);
            }
            MethodInfo method = GetUserDefinedBinaryOperator(ExpressionType.OrElse, left.Type, right.Type, "op_BitwiseOr");
            if (method == null)
            {
                throw System.Linq.Expressions.Error.BinaryOperatorNotDefined(ExpressionType.OrElse, left.Type, right.Type);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
            return new BinaryExpression(ExpressionType.OrElse, left, right, method, (IsNullableType(left.Type) && (method.ReturnType == GetNonNullableType(left.Type))) ? left.Type : method.ReturnType);
        }

        public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return OrElse(left, right);
            }
            ValidateUserDefinedConditionalLogicOperator(ExpressionType.OrElse, left.Type, right.Type, method);
            return new BinaryExpression(ExpressionType.OrElse, left, right, method, (IsNullableType(left.Type) && (method.ReturnType == GetNonNullableType(left.Type))) ? left.Type : method.ReturnType);
        }

        public static ParameterExpression Parameter(System.Type type, string name)
        {
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (type == typeof(void))
            {
                throw System.Linq.Expressions.Error.ArgumentCannotBeOfTypeVoid();
            }
            return new ParameterExpression(type, name);
        }

        private static bool ParameterIsAssignable(ParameterInfo pi, System.Type argType)
        {
            System.Type parameterType = pi.ParameterType;
            if (parameterType.IsByRef)
            {
                parameterType = parameterType.GetElementType();
            }
            return AreReferenceAssignable(parameterType, argType);
        }

        public static BinaryExpression Power(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            MethodInfo method = typeof(Math).GetMethod("Pow", BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                throw System.Linq.Expressions.Error.BinaryOperatorNotDefined(ExpressionType.Power, left.Type, right.Type);
            }
            return Power(left, right, method);
        }

        public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Power(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Power, left, right, method, true);
        }

        public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
        {
            if (propertyAccessor == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("propertyAccessor");
            }
            ValidateMethodInfo(propertyAccessor);
            return Property(expression, GetProperty(propertyAccessor));
        }

        public static MemberExpression Property(Expression expression, PropertyInfo property)
        {
            if (property == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("property");
            }
            if (!property.CanRead)
            {
                throw System.Linq.Expressions.Error.PropertyDoesNotHaveGetter(property);
            }
            if (!property.GetGetMethod(true).IsStatic)
            {
                if (expression == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("expression");
                }
                if (!AreReferenceAssignable(property.DeclaringType, expression.Type))
                {
                    throw System.Linq.Expressions.Error.PropertyNotDefinedForType(property, expression.Type);
                }
            }
            return new MemberExpression(expression, property, property.PropertyType);
        }

        public static MemberExpression Property(Expression expression, string propertyName)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            PropertyInfo property = expression.Type.GetProperty(propertyName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
            {
                property = expression.Type.GetProperty(propertyName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            if (property == null)
            {
                throw System.Linq.Expressions.Error.PropertyNotDefinedForType(propertyName, expression.Type);
            }
            return Property(expression, property);
        }

        public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            PropertyInfo property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property != null)
            {
                return Property(expression, property);
            }
            FieldInfo field = expression.Type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field == null)
            {
                property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return Property(expression, property);
                }
                field = expression.Type.GetField(propertyOrFieldName, BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (field == null)
                {
                    throw System.Linq.Expressions.Error.NotAMemberOfType(propertyOrFieldName, expression.Type);
                }
            }
            return Field(expression, field);
        }

        public static UnaryExpression Quote(Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            return new UnaryExpression(ExpressionType.Quote, expression, expression.GetType());
        }

        public static BinaryExpression RightShift(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (IsInteger(left.Type) && (GetNonNullableType(right.Type) == typeof(int)))
            {
                return new BinaryExpression(ExpressionType.RightShift, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.RightShift, "op_RightShift", left, right, true);
        }

        public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return RightShift(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.RightShift, left, right, method, true);
        }

        public static BinaryExpression Subtract(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.Subtract, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.Subtract, "op_Subtraction", left, right, true);
        }

        public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return Subtract(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.Subtract, left, right, method, true);
        }

        public static BinaryExpression SubtractChecked(Expression left, Expression right)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if ((left.Type == right.Type) && IsArithmetic(left.Type))
            {
                return new BinaryExpression(ExpressionType.SubtractChecked, left, right, left.Type);
            }
            return GetUserDefinedBinaryOperatorOrThrow(ExpressionType.SubtractChecked, "op_Subtraction", left, right, true);
        }

        public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
        {
            if (left == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("left");
            }
            if (right == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("right");
            }
            if (method == null)
            {
                return SubtractChecked(left, right);
            }
            return GetMethodBasedBinaryOperator(ExpressionType.SubtractChecked, left, right, method, true);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this.BuildString(builder);
            return builder.ToString();
        }

        public static UnaryExpression TypeAs(Expression expression, System.Type type)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (type.IsValueType && !IsNullableType(type))
            {
                throw System.Linq.Expressions.Error.IncorrectTypeForTypeAs(type);
            }
            return new UnaryExpression(ExpressionType.TypeAs, expression, type);
        }

        public static TypeBinaryExpression TypeIs(Expression expression, System.Type type)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            return new TypeBinaryExpression(ExpressionType.TypeIs, expression, type, typeof(bool));
        }

        public static UnaryExpression UnaryPlus(Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (IsArithmetic(expression.Type))
            {
                return new UnaryExpression(ExpressionType.UnaryPlus, expression, expression.Type);
            }
            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.UnaryPlus, "op_UnaryPlus", expression);
        }

        public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
        {
            if (expression == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("expression");
            }
            if (method == null)
            {
                return UnaryPlus(expression);
            }
            return GetMethodBasedUnaryOperator(ExpressionType.UnaryPlus, expression, method);
        }

        private static void ValidateAnonymousTypeMember(MemberInfo member, out System.Type memberType)
        {
            MemberTypes types = member.MemberType;
            if (types != MemberTypes.Field)
            {
                if (types != MemberTypes.Method)
                {
                    if (types != MemberTypes.Property)
                    {
                        throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfoOrMethod();
                    }
                    PropertyInfo info2 = member as PropertyInfo;
                    if (!info2.CanRead)
                    {
                        throw System.Linq.Expressions.Error.PropertyDoesNotHaveGetter(info2);
                    }
                    if (info2.GetGetMethod().IsStatic)
                    {
                        throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
                    }
                    memberType = info2.PropertyType;
                }
                else
                {
                    MethodInfo info3 = member as MethodInfo;
                    if (info3.IsStatic)
                    {
                        throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
                    }
                    memberType = info3.ReturnType;
                }
            }
            else
            {
                FieldInfo info = member as FieldInfo;
                if (info.IsStatic)
                {
                    throw System.Linq.Expressions.Error.ArgumentMustBeInstanceMember();
                }
                memberType = info.FieldType;
            }
        }

        private static void ValidateArgumentTypes(MethodInfo method, ref ReadOnlyCollection<Expression> arguments)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                if (parameters.Length != arguments.Count)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
                }
                List<Expression> sequence = null;
                int index = 0;
                int length = parameters.Length;
                while (index < length)
                {
                    Expression expression = arguments[index];
                    ParameterInfo info = parameters[index];
                    if (expression == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("arguments");
                    }
                    System.Type parameterType = info.ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    ValidateType(parameterType);
                    if (!AreReferenceAssignable(parameterType, expression.Type))
                    {
                        if (!IsSameOrSubclass(typeof(Expression), parameterType) || !AreAssignable(parameterType, expression.GetType()))
                        {
                            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchMethodParameter(expression.Type, parameterType, method);
                        }
                        expression = Quote(expression);
                    }
                    if ((sequence == null) && (expression != arguments[index]))
                    {
                        sequence = new List<Expression>(arguments.Count);
                        for (int i = 0; i < index; i++)
                        {
                            sequence.Add(arguments[i]);
                        }
                    }
                    if (sequence != null)
                    {
                        sequence.Add(expression);
                    }
                    index++;
                }
                if (sequence != null)
                {
                    arguments = sequence.ToReadOnlyCollection<Expression>();
                }
            }
            else if (arguments.Count > 0)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
            }
        }

        private static void ValidateBoolArg(System.Type type)
        {
            if (!IsBool(type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeBoolean();
            }
        }

        private static void ValidateCallArgs(Expression instance, MethodInfo method, ref ReadOnlyCollection<Expression> arguments)
        {
            if (method == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("method");
            }
            if (arguments == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("arguments");
            }
            ValidateMethodInfo(method);
            if (!method.IsStatic)
            {
                if (instance == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("instance");
                }
                ValidateCallInstanceType(instance.Type, method);
            }
            ValidateArgumentTypes(method, ref arguments);
        }

        private static void ValidateCallInstanceType(System.Type instanceType, MethodInfo method)
        {
            if (!AreReferenceAssignable(method.DeclaringType, instanceType))
            {
                if (instanceType.IsValueType)
                {
                    if (AreReferenceAssignable(method.DeclaringType, typeof(object)))
                    {
                        return;
                    }
                    if (AreReferenceAssignable(method.DeclaringType, typeof(ValueType)))
                    {
                        return;
                    }
                    if (instanceType.IsEnum && AreReferenceAssignable(method.DeclaringType, typeof(Enum)))
                    {
                        return;
                    }
                    if (method.DeclaringType.IsInterface)
                    {
                        foreach (System.Type type in instanceType.GetInterfaces())
                        {
                            if (AreReferenceAssignable(method.DeclaringType, type))
                            {
                                return;
                            }
                        }
                    }
                }
                throw System.Linq.Expressions.Error.MethodNotDefinedForType(method, instanceType);
            }
        }

        private static System.Type ValidateCoalesceArgTypes(System.Type left, System.Type right)
        {
            System.Type nonNullableType = GetNonNullableType(left);
            if (left.IsValueType && !IsNullableType(left))
            {
                throw System.Linq.Expressions.Error.CoalesceUsedOnNonNullType();
            }
            if (IsNullableType(left) && IsImplicitlyConvertible(right, nonNullableType))
            {
                return nonNullableType;
            }
            if (IsImplicitlyConvertible(right, left))
            {
                return left;
            }
            if (!IsImplicitlyConvertible(nonNullableType, right))
            {
                throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
            }
            return right;
        }

        private static void ValidateConvertibleArg(System.Type type)
        {
            if (!IsConvertible(type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeConvertible();
            }
        }

        private static void ValidateElementInitAddMethodInfo(MethodInfo addMethod)
        {
            ValidateMethodInfo(addMethod);
            if (addMethod.GetParameters().Length == 0)
            {
                throw System.Linq.Expressions.Error.ElementInitializerMethodWithZeroArgs();
            }
            if (!addMethod.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                throw System.Linq.Expressions.Error.ElementInitializerMethodNotAdd();
            }
            if (addMethod.IsStatic)
            {
                throw System.Linq.Expressions.Error.ElementInitializerMethodStatic();
            }
            foreach (ParameterInfo info in addMethod.GetParameters())
            {
                if (info.ParameterType.IsByRef)
                {
                    throw System.Linq.Expressions.Error.ElementInitializerMethodNoRefOutParam(info.Name, addMethod.Name);
                }
            }
        }

        private static void ValidateGettableFieldOrPropertyMember(MemberInfo member, out System.Type memberType)
        {
            FieldInfo info = member as FieldInfo;
            if (info == null)
            {
                PropertyInfo info2 = member as PropertyInfo;
                if (info2 == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfo();
                }
                if (!info2.CanRead)
                {
                    throw System.Linq.Expressions.Error.PropertyDoesNotHaveGetter(info2);
                }
                memberType = info2.PropertyType;
            }
            else
            {
                memberType = info.FieldType;
            }
        }

        private static void ValidateIntegerArg(System.Type type)
        {
            if (!IsInteger(type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeInteger();
            }
        }

        private static void ValidateIntegerOrBoolArg(System.Type type)
        {
            if (!IsIntegerOrBool(type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeIntegerOrBoolean();
            }
        }

        private static void ValidateLambdaArgs(System.Type delegateType, ref Expression body, ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (delegateType == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("delegateType");
            }
            if (body == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("body");
            }
            if (!AreAssignable(typeof(Delegate), delegateType) || (delegateType == typeof(Delegate)))
            {
                throw System.Linq.Expressions.Error.LambdaTypeMustBeDerivedFromSystemDelegate();
            }
            MethodInfo method = delegateType.GetMethod("Invoke");
            ParameterInfo[] infoArray = method.GetParameters();
            if (infoArray.Length > 0)
            {
                if (infoArray.Length != parameters.Count)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaDeclarationParameters();
                }
                int index = 0;
                int length = infoArray.Length;
                while (index < length)
                {
                    Expression expression = parameters[index];
                    ParameterInfo info2 = infoArray[index];
                    if (expression == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("parameters");
                    }
                    System.Type parameterType = info2.ParameterType;
                    if (parameterType.IsByRef || expression.Type.IsByRef)
                    {
                        throw System.Linq.Expressions.Error.ExpressionMayNotContainByrefParameters();
                    }
                    if (!AreReferenceAssignable(expression.Type, parameterType))
                    {
                        throw System.Linq.Expressions.Error.ParameterExpressionNotValidAsDelegate(expression.Type, parameterType);
                    }
                    index++;
                }
            }
            else if (parameters.Count > 0)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfLambdaDeclarationParameters();
            }
            if ((method.ReturnType != typeof(void)) && !AreReferenceAssignable(method.ReturnType, body.Type))
            {
                if (!IsSameOrSubclass(typeof(Expression), method.ReturnType) || !AreAssignable(method.ReturnType, body.GetType()))
                {
                    throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchReturn(body.Type, method.ReturnType);
                }
                body = Quote(body);
            }
        }

        internal static void ValidateLift(IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> arguments)
        {
            ReadOnlyCollection<ParameterExpression> onlys = parameters.ToReadOnlyCollection<ParameterExpression>();
            ReadOnlyCollection<Expression> onlys2 = arguments.ToReadOnlyCollection<Expression>();
            if (onlys.Count != onlys2.Count)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfIndexes();
            }
            int num = 0;
            int count = onlys.Count;
            while (num < count)
            {
                if (!AreReferenceAssignable(onlys[num].Type, GetNonNullableType(onlys2[num].Type)))
                {
                    throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
                }
                num++;
            }
        }

        private static void ValidateListInitArgs(System.Type listType, ReadOnlyCollection<System.Linq.Expressions.ElementInit> initializers)
        {
            if (!AreAssignable(typeof(IEnumerable), listType))
            {
                throw System.Linq.Expressions.Error.TypeNotIEnumerable(listType);
            }
            int num = 0;
            int count = initializers.Count;
            while (num < count)
            {
                System.Linq.Expressions.ElementInit init = initializers[num];
                if (init == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentNull("initializers");
                }
                ValidateCallInstanceType(listType, init.AddMethod);
                num++;
            }
        }

        private static void ValidateMemberInitArgs(System.Type type, ReadOnlyCollection<MemberBinding> bindings)
        {
            int num = 0;
            int count = bindings.Count;
            while (num < count)
            {
                MemberBinding binding = bindings[num];
                if (!AreAssignable(binding.Member.DeclaringType, type))
                {
                    throw System.Linq.Expressions.Error.NotAMemberOfType(binding.Member.Name, type);
                }
                num++;
            }
        }

        private static void ValidateMethodInfo(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
            {
                throw System.Linq.Expressions.Error.MethodIsGeneric(method);
            }
            if (method.ContainsGenericParameters)
            {
                throw System.Linq.Expressions.Error.MethodContainsGenericParameters(method);
            }
        }

        private static void ValidateNewArgs(ConstructorInfo constructor, ref ReadOnlyCollection<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
        {
            ParameterInfo[] infoArray;
            if ((infoArray = constructor.GetParameters()).Length > 0)
            {
                if (arguments.Count != infoArray.Length)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
                }
                if (arguments.Count != members.Count)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfArgumentsForMembers();
                }
                List<Expression> sequence = null;
                int index = 0;
                int count = arguments.Count;
                while (index < count)
                {
                    System.Type type;
                    Expression expression = arguments[index];
                    if (expression == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("argument");
                    }
                    MemberInfo member = members[index];
                    if (member == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("member");
                    }
                    if (member.DeclaringType != constructor.DeclaringType)
                    {
                        throw System.Linq.Expressions.Error.ArgumentMemberNotDeclOnType(member.Name, constructor.DeclaringType.Name);
                    }
                    ValidateAnonymousTypeMember(member, out type);
                    if (!AreReferenceAssignable(expression.Type, type))
                    {
                        if (!IsSameOrSubclass(typeof(Expression), type) || !AreAssignable(type, expression.GetType()))
                        {
                            throw System.Linq.Expressions.Error.ArgumentTypeDoesNotMatchMember(expression.Type, type);
                        }
                        expression = Quote(expression);
                    }
                    ParameterInfo info2 = infoArray[index];
                    System.Type parameterType = info2.ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    if (!AreReferenceAssignable(parameterType, expression.Type))
                    {
                        if (!IsSameOrSubclass(typeof(Expression), parameterType) || !AreAssignable(parameterType, expression.Type))
                        {
                            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchConstructorParameter(expression.Type, parameterType);
                        }
                        expression = Quote(expression);
                    }
                    if ((sequence == null) && (expression != arguments[index]))
                    {
                        sequence = new List<Expression>(arguments.Count);
                        for (int i = 0; i < index; i++)
                        {
                            sequence.Add(arguments[i]);
                        }
                    }
                    if (sequence != null)
                    {
                        sequence.Add(expression);
                    }
                    index++;
                }
                if (sequence != null)
                {
                    arguments = sequence.ToReadOnlyCollection<Expression>();
                }
            }
            else
            {
                if ((arguments != null) && (arguments.Count > 0))
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
                }
                if ((members != null) && (members.Count > 0))
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfMembersForGivenConstructor();
                }
            }
        }

        private static void ValidateNewArgs(System.Type type, ConstructorInfo constructor, ref ReadOnlyCollection<Expression> arguments)
        {
            ParameterInfo[] infoArray;
            if (type == null)
            {
                throw System.Linq.Expressions.Error.ArgumentNull("type");
            }
            if (!type.IsValueType && (constructor == null))
            {
                throw System.Linq.Expressions.Error.ArgumentNull("constructor");
            }
            if ((constructor != null) && ((infoArray = constructor.GetParameters()).Length > 0))
            {
                if (arguments.Count != infoArray.Length)
                {
                    throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
                }
                List<Expression> sequence = null;
                int index = 0;
                int count = arguments.Count;
                while (index < count)
                {
                    Expression expression = arguments[index];
                    ParameterInfo info = infoArray[index];
                    if (expression == null)
                    {
                        throw System.Linq.Expressions.Error.ArgumentNull("arguments");
                    }
                    System.Type parameterType = info.ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    if (!AreReferenceAssignable(parameterType, expression.Type))
                    {
                        if (!IsSameOrSubclass(typeof(Expression), parameterType) || !AreAssignable(parameterType, expression.GetType()))
                        {
                            throw System.Linq.Expressions.Error.ExpressionTypeDoesNotMatchConstructorParameter(expression.Type, parameterType);
                        }
                        expression = Quote(expression);
                    }
                    if ((sequence == null) && (expression != arguments[index]))
                    {
                        sequence = new List<Expression>(arguments.Count);
                        for (int i = 0; i < index; i++)
                        {
                            sequence.Add(arguments[i]);
                        }
                    }
                    if (sequence != null)
                    {
                        sequence.Add(expression);
                    }
                    index++;
                }
                if (sequence != null)
                {
                    arguments = sequence.ToReadOnlyCollection<Expression>();
                }
            }
            else if ((arguments != null) && (arguments.Count > 0))
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfConstructorArguments();
            }
        }

        private static void ValidateNumericArg(System.Type type)
        {
            if (!IsNumeric(type))
            {
                throw System.Linq.Expressions.Error.ArgumentMustBeNumeric();
            }
        }

        private static void ValidateOperator(MethodInfo method)
        {
            ValidateMethodInfo(method);
            if (!method.IsStatic)
            {
                throw System.Linq.Expressions.Error.UserDefinedOperatorMustBeStatic(method);
            }
            if (method.ReturnType == typeof(void))
            {
                throw System.Linq.Expressions.Error.UserDefinedOperatorMustNotBeVoid(method);
            }
        }

        private static void ValidateParamswithOperandsOrThrow(System.Type paramType, System.Type operandType, ExpressionType exprType, string name)
        {
            if (IsNullableType(paramType) && !IsNullableType(operandType))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(exprType, name);
            }
        }

        private static void ValidateSameArgTypes(System.Type left, System.Type right)
        {
            if (left != right)
            {
                throw System.Linq.Expressions.Error.ArgumentTypesMustMatch();
            }
        }

        private static void ValidateSettableFieldOrPropertyMember(MemberInfo member, out System.Type memberType)
        {
            FieldInfo info = member as FieldInfo;
            if (info == null)
            {
                PropertyInfo info2 = member as PropertyInfo;
                if (info2 == null)
                {
                    throw System.Linq.Expressions.Error.ArgumentMustBeFieldInfoOrPropertInfo();
                }
                if (!info2.CanWrite)
                {
                    throw System.Linq.Expressions.Error.PropertyDoesNotHaveSetter(info2);
                }
                memberType = info2.PropertyType;
            }
            else
            {
                memberType = info.FieldType;
            }
        }

        private static void ValidateType(System.Type type)
        {
            if (type.IsGenericTypeDefinition)
            {
                throw System.Linq.Expressions.Error.TypeIsGeneric(type);
            }
            if (type.ContainsGenericParameters)
            {
                throw System.Linq.Expressions.Error.TypeContainsGenericParameters(type);
            }
        }

        private static void ValidateUserDefinedConditionalLogicOperator(ExpressionType nodeType, System.Type left, System.Type right, MethodInfo method)
        {
            ValidateOperator(method);
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 2)
            {
                throw System.Linq.Expressions.Error.IncorrectNumberOfMethodCallArguments(method);
            }
            if (!ParameterIsAssignable(parameters[0], left) && (!IsNullableType(left) || !ParameterIsAssignable(parameters[0], GetNonNullableType(left))))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
            }
            if (!ParameterIsAssignable(parameters[1], right) && (!IsNullableType(right) || !ParameterIsAssignable(parameters[1], GetNonNullableType(right))))
            {
                throw System.Linq.Expressions.Error.OperandTypesDoNotMatchParameters(nodeType, method.Name);
            }
            if (parameters[0].ParameterType != parameters[1].ParameterType)
            {
                throw System.Linq.Expressions.Error.LogicalOperatorMustHaveConsistentTypes(nodeType, method.Name);
            }
            if (method.ReturnType != parameters[0].ParameterType)
            {
                throw System.Linq.Expressions.Error.LogicalOperatorMustHaveConsistentTypes(nodeType, method.Name);
            }
            if (IsValidLiftedConditionalLogicalOperator(left, right, parameters))
            {
                left = GetNonNullableType(left);
                right = GetNonNullableType(left);
            }
            System.Type[] types = new System.Type[] { parameters[0].ParameterType };
            MethodInfo info = method.DeclaringType.GetMethod("op_True", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            MethodInfo info2 = method.DeclaringType.GetMethod("op_False", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            if ((info == null) || (info2 == null))
            {
                throw System.Linq.Expressions.Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
            }
            if (info.ReturnType != typeof(bool))
            {
                throw System.Linq.Expressions.Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
            }
            if (info2.ReturnType != typeof(bool))
            {
                throw System.Linq.Expressions.Error.LogicalOperatorMustHaveBooleanOperators(nodeType, method.Name);
            }
        }

        public ExpressionType NodeType =>
            this.nodeType;

        public System.Type Type =>
            this.type;
    }
}

