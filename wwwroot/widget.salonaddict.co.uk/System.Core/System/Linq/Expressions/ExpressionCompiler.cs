namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class ExpressionCompiler
    {
        private List<object> globals = new List<object>();
        private List<LambdaInfo> lambdas = new List<LambdaInfo>();
        private CompileScope scope;

        internal ExpressionCompiler()
        {
        }

        private int AddGlobal(Type type, object value)
        {
            int count = this.globals.Count;
            this.globals.Add(Activator.CreateInstance(MakeStrongBoxType(type), new object[] { value }));
            return count;
        }

        public D Compile<D>(Expression<D> lambda)
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(D)))
            {
                throw Error.TypeParameterIsNotDelegate(typeof(D));
            }
            return (D) this.Compile(lambda);
        }

        public Delegate Compile(LambdaExpression lambda) => 
            this.CompileDynamicLambda(lambda);

        private Delegate CompileDynamicLambda(LambdaExpression lambda)
        {
            this.lambdas = new List<LambdaInfo>();
            this.globals = new List<object>();
            int num = this.GenerateLambda(lambda);
            LambdaInfo info = this.lambdas[num];
            ExecutionScope target = new ExecutionScope(null, info, this.globals.ToArray(), null);
            return ((DynamicMethod) info.Method).CreateDelegate(lambda.Type, target);
        }

        private StackType Generate(ILGenerator gen, Expression node, StackType ask)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.GenerateBinary(gen, (BinaryExpression) node, ask);

                case ExpressionType.ArrayLength:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.TypeAs:
                    return this.GenerateUnary(gen, (UnaryExpression) node, ask);

                case ExpressionType.Call:
                    return this.GenerateMethodCall(gen, (MethodCallExpression) node, ask);

                case ExpressionType.Conditional:
                    return this.GenerateConditional(gen, (ConditionalExpression) node);

                case ExpressionType.Constant:
                    return this.GenerateConstant(gen, (ConstantExpression) node, ask);

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    this.GenerateConvert(gen, (UnaryExpression) node);
                    return StackType.Value;

                case ExpressionType.Invoke:
                    return this.GenerateInvoke(gen, (InvocationExpression) node, ask);

                case ExpressionType.Lambda:
                    this.GenerateCreateDelegate(gen, (LambdaExpression) node);
                    return StackType.Value;

                case ExpressionType.ListInit:
                    return this.GenerateListInit(gen, (ListInitExpression) node);

                case ExpressionType.MemberAccess:
                    return this.GenerateMemberAccess(gen, (MemberExpression) node, ask);

                case ExpressionType.MemberInit:
                    return this.GenerateMemberInit(gen, (MemberInitExpression) node);

                case ExpressionType.New:
                    return this.GenerateNew(gen, (NewExpression) node, ask);

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    this.GenerateNewArray(gen, (NewArrayExpression) node);
                    return StackType.Value;

                case ExpressionType.Parameter:
                    return this.GenerateParameterAccess(gen, (ParameterExpression) node, ask);

                case ExpressionType.Quote:
                    this.GenerateQuote(gen, (UnaryExpression) node);
                    return StackType.Value;

                case ExpressionType.TypeIs:
                    this.GenerateTypeIs(gen, (TypeBinaryExpression) node);
                    return StackType.Value;
            }
            throw Error.UnhandledExpressionType(node.NodeType);
        }

        private StackType GenerateAndAlso(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            if ((b.Method != null) && !IsLiftedLogicalBinaryOperator(b.Left.Type, b.Right.Type, b.Method))
            {
                this.GenerateMethodAndAlso(gen, b);
            }
            else
            {
                if (b.Left.Type == typeof(bool?))
                {
                    return this.GenerateLiftedAndAlso(gen, b, ask);
                }
                if (IsLiftedLogicalBinaryOperator(b.Left.Type, b.Right.Type, b.Method))
                {
                    return this.GenerateUserdefinedLiftedAndAlso(gen, b, ask);
                }
                this.GenerateUnliftedAndAlso(gen, b);
            }
            return StackType.Value;
        }

        private StackType GenerateArgAccess(ILGenerator gen, int iArg, StackType ask)
        {
            if (ask == StackType.Value)
            {
                switch (iArg)
                {
                    case 0:
                        gen.Emit(OpCodes.Ldarg_0);
                        return ask;

                    case 1:
                        gen.Emit(OpCodes.Ldarg_1);
                        return ask;

                    case 2:
                        gen.Emit(OpCodes.Ldarg_2);
                        return ask;

                    case 3:
                        gen.Emit(OpCodes.Ldarg_3);
                        return ask;
                }
                if (iArg < 0x80)
                {
                    gen.Emit(OpCodes.Ldarg_S, (byte) iArg);
                    return ask;
                }
                gen.Emit(OpCodes.Ldarg, iArg);
                return ask;
            }
            if (iArg < 0x80)
            {
                gen.Emit(OpCodes.Ldarga_S, (byte) iArg);
                return ask;
            }
            gen.Emit(OpCodes.Ldarga, iArg);
            return ask;
        }

        private List<WriteBack> GenerateArgs(ILGenerator gen, ParameterInfo[] pis, ReadOnlyCollection<Expression> args)
        {
            List<WriteBack> list = new List<WriteBack>();
            int index = 0;
            int length = pis.Length;
            while (index < length)
            {
                ParameterInfo info = pis[index];
                Expression node = args[index];
                StackType ask = info.ParameterType.IsByRef ? StackType.Address : StackType.Value;
                StackType type2 = this.Generate(gen, node, ask);
                if ((ask == StackType.Address) && (type2 != StackType.Address))
                {
                    LocalBuilder local = gen.DeclareLocal(node.Type);
                    gen.Emit(OpCodes.Stloc, local);
                    gen.Emit(OpCodes.Ldloca, local);
                    if (args[index] is MemberExpression)
                    {
                        list.Add(new WriteBack(local, args[index]));
                    }
                }
                index++;
            }
            return list;
        }

        private StackType GenerateArrayAccess(ILGenerator gen, Type type, StackType ask)
        {
            if (ask == StackType.Address)
            {
                gen.Emit(OpCodes.Ldelema, type);
                return ask;
            }
            if (!type.IsValueType)
            {
                gen.Emit(OpCodes.Ldelem_Ref);
                return ask;
            }
            if (type.IsEnum)
            {
                gen.Emit(OpCodes.Ldelem, type);
                return ask;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                    gen.Emit(OpCodes.Ldelem_I1);
                    return ask;

                case TypeCode.Int16:
                    gen.Emit(OpCodes.Ldelem_I2);
                    return ask;

                case TypeCode.Int32:
                    gen.Emit(OpCodes.Ldelem_I4);
                    return ask;

                case TypeCode.Int64:
                    gen.Emit(OpCodes.Ldelem_I8);
                    return ask;

                case TypeCode.Single:
                    gen.Emit(OpCodes.Ldelem_R4);
                    return ask;

                case TypeCode.Double:
                    gen.Emit(OpCodes.Ldelem_R8);
                    return ask;
            }
            gen.Emit(OpCodes.Ldelem, type);
            return ask;
        }

        private void GenerateArrayAssign(ILGenerator gen, Type type)
        {
            if (type.IsEnum)
            {
                gen.Emit(OpCodes.Stelem, type);
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        gen.Emit(OpCodes.Stelem_I1);
                        return;

                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        gen.Emit(OpCodes.Stelem_I2);
                        return;

                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        gen.Emit(OpCodes.Stelem_I4);
                        return;

                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        gen.Emit(OpCodes.Stelem_I8);
                        return;

                    case TypeCode.Single:
                        gen.Emit(OpCodes.Stelem_R4);
                        return;

                    case TypeCode.Double:
                        gen.Emit(OpCodes.Stelem_R8);
                        return;
                }
                if (type.IsValueType)
                {
                    gen.Emit(OpCodes.Stelem, type);
                }
                else
                {
                    gen.Emit(OpCodes.Stelem_Ref);
                }
            }
        }

        private StackType GenerateBinary(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                    return this.GenerateAndAlso(gen, b, ask);

                case ExpressionType.Coalesce:
                    this.GenerateCoalesce(gen, b);
                    return StackType.Value;

                case ExpressionType.OrElse:
                    return this.GenerateOrElse(gen, b, ask);
            }
            if (b.Method != null)
            {
                return this.GenerateBinaryMethod(gen, b, ask);
            }
            if (((b.NodeType == ExpressionType.Equal) || (b.NodeType == ExpressionType.NotEqual)) && ((b.Type == typeof(bool)) || (b.Type == typeof(bool?))))
            {
                if ((IsNullConstant(b.Left) && !IsNullConstant(b.Right)) && IsNullable(b.Right.Type))
                {
                    return this.GenerateNullEquality(gen, b.NodeType, b.Right, b.IsLiftedToNull);
                }
                if ((IsNullConstant(b.Right) && !IsNullConstant(b.Left)) && IsNullable(b.Left.Type))
                {
                    return this.GenerateNullEquality(gen, b.NodeType, b.Left, b.IsLiftedToNull);
                }
            }
            this.Generate(gen, b.Left, StackType.Value);
            this.Generate(gen, b.Right, StackType.Value);
            return this.GenerateBinaryOp(gen, b.NodeType, b.Left.Type, b.Right.Type, b.Type, b.IsLiftedToNull, ask);
        }

        private StackType GenerateBinaryMethod(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            if (!b.IsLifted)
            {
                MethodCallExpression node = Expression.Call(null, b.Method, new Expression[] { b.Left, b.Right });
                return this.Generate(gen, node, ask);
            }
            ParameterExpression expression = Expression.Parameter(Expression.GetNonNullableType(b.Left.Type), null);
            ParameterExpression expression2 = Expression.Parameter(Expression.GetNonNullableType(b.Right.Type), null);
            MethodCallExpression mc = Expression.Call(null, b.Method, new Expression[] { expression, expression2 });
            Type resultType = null;
            if (b.IsLiftedToNull)
            {
                resultType = Expression.GetNullableType(mc.Type);
            }
            else
            {
                switch (b.NodeType)
                {
                    case ExpressionType.Equal:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.NotEqual:
                        if (mc.Type != typeof(bool))
                        {
                            throw Error.ArgumentMustBeBoolean();
                        }
                        resultType = typeof(bool);
                        goto Label_00DF;
                }
                resultType = Expression.GetNullableType(mc.Type);
            }
        Label_00DF:;
            IEnumerable<ParameterExpression> parameters = new ParameterExpression[] { expression, expression2 };
            IEnumerable<Expression> arguments = new Expression[] { b.Left, b.Right };
            Expression.ValidateLift(parameters, arguments);
            return this.GenerateLift(gen, b.NodeType, resultType, mc, parameters, arguments);
        }

        private StackType GenerateBinaryOp(ILGenerator gen, ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull, StackType ask)
        {
            bool flag = IsNullable(leftType);
            bool flag2 = IsNullable(rightType);
            switch (op)
            {
                case ExpressionType.ArrayIndex:
                {
                    if (flag2)
                    {
                        LocalBuilder local = gen.DeclareLocal(rightType);
                        gen.Emit(OpCodes.Stloc, local);
                        gen.Emit(OpCodes.Ldloca, local);
                        this.GenerateGetValue(gen, rightType);
                    }
                    Type nonNullableType = GetNonNullableType(rightType);
                    if (nonNullableType != typeof(int))
                    {
                        this.GenerateConvertToType(gen, nonNullableType, typeof(int), true);
                    }
                    return this.GenerateArrayAccess(gen, leftType.GetElementType(), ask);
                }
                case ExpressionType.Coalesce:
                    throw Error.UnexpectedCoalesceOperator();
            }
            if (flag)
            {
                return this.GenerateLiftedBinaryOp(gen, op, leftType, rightType, resultType, liftedToNull, ask);
            }
            return this.GenerateUnliftedBinaryOp(gen, op, leftType, rightType);
        }

        private void GenerateBinding(ILGenerator gen, MemberBinding binding, Type objectType)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    this.GenerateMemberAssignment(gen, (MemberAssignment) binding, objectType);
                    return;

                case MemberBindingType.MemberBinding:
                    this.GenerateMemberMemberBinding(gen, (MemberMemberBinding) binding);
                    return;

                case MemberBindingType.ListBinding:
                    this.GenerateMemberListBinding(gen, (MemberListBinding) binding);
                    return;
            }
            throw Error.UnknownBindingType();
        }

        private void GenerateCastToType(ILGenerator gen, Type typeFrom, Type typeTo)
        {
            if (!typeFrom.IsValueType && typeTo.IsValueType)
            {
                gen.Emit(OpCodes.Unbox_Any, typeTo);
            }
            else if (typeFrom.IsValueType && !typeTo.IsValueType)
            {
                gen.Emit(OpCodes.Box, typeFrom);
                if (typeTo != typeof(object))
                {
                    gen.Emit(OpCodes.Castclass, typeTo);
                }
            }
            else
            {
                if (typeFrom.IsValueType || typeTo.IsValueType)
                {
                    throw Error.InvalidCast(typeFrom, typeTo);
                }
                gen.Emit(OpCodes.Castclass, typeTo);
            }
        }

        private void GenerateCoalesce(ILGenerator gen, BinaryExpression b)
        {
            if (IsNullable(b.Left.Type))
            {
                this.GenerateNullableCoalesce(gen, b);
            }
            else
            {
                if (b.Left.Type.IsValueType)
                {
                    throw Error.CoalesceUsedOnNonNullType();
                }
                if (b.Conversion != null)
                {
                    this.GenerateLambdaReferenceCoalesce(gen, b);
                }
                else if (b.Method != null)
                {
                    this.GenerateUserDefinedReferenceCoalesce(gen, b);
                }
                else
                {
                    this.GenerateReferenceCoalesceWithoutConversion(gen, b);
                }
            }
        }

        private StackType GenerateConditional(ILGenerator gen, ConditionalExpression b)
        {
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            this.Generate(gen, b.Test, StackType.Value);
            gen.Emit(OpCodes.Brfalse, label);
            this.Generate(gen, b.IfTrue, StackType.Value);
            gen.Emit(OpCodes.Br, label2);
            gen.MarkLabel(label);
            this.Generate(gen, b.IfFalse, StackType.Value);
            gen.MarkLabel(label2);
            return StackType.Value;
        }

        private StackType GenerateConstant(ILGenerator gen, ConstantExpression c, StackType ask) => 
            this.GenerateConstant(gen, c.Type, c.Value, ask);

        private StackType GenerateConstant(ILGenerator gen, Type type, object value, StackType ask)
        {
            if (value == null)
            {
                if (type.IsValueType)
                {
                    LocalBuilder local = gen.DeclareLocal(type);
                    gen.Emit(OpCodes.Ldloca, local);
                    gen.Emit(OpCodes.Initobj, type);
                    gen.Emit(OpCodes.Ldloc, local);
                }
                else
                {
                    gen.Emit(OpCodes.Ldnull);
                }
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        this.GenerateConstInt(gen, ((bool) value) ? 1 : 0);
                        goto Label_013B;

                    case TypeCode.SByte:
                        this.GenerateConstInt(gen, (sbyte) value);
                        gen.Emit(OpCodes.Conv_I1);
                        goto Label_013B;

                    case TypeCode.Int16:
                        this.GenerateConstInt(gen, (short) value);
                        gen.Emit(OpCodes.Conv_I2);
                        goto Label_013B;

                    case TypeCode.Int32:
                        this.GenerateConstInt(gen, (int) value);
                        goto Label_013B;

                    case TypeCode.Int64:
                        gen.Emit(OpCodes.Ldc_I8, (long) value);
                        goto Label_013B;

                    case TypeCode.Single:
                        gen.Emit(OpCodes.Ldc_R4, (float) value);
                        goto Label_013B;

                    case TypeCode.Double:
                        gen.Emit(OpCodes.Ldc_R8, (double) value);
                        goto Label_013B;
                }
                int iGlobal = this.AddGlobal(type, value);
                return this.GenerateGlobalAccess(gen, iGlobal, type, ask);
            }
        Label_013B:
            return StackType.Value;
        }

        private void GenerateConstInt(ILGenerator gen, int value)
        {
            switch (value)
            {
                case 0:
                    gen.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    gen.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    gen.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    gen.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    gen.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    gen.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    gen.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    gen.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    gen.Emit(OpCodes.Ldc_I4_8);
                    return;

                case -1:
                    gen.Emit(OpCodes.Ldc_I4_M1);
                    return;
            }
            if ((value >= -127) && (value < 0x80))
            {
                gen.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
            }
            else
            {
                gen.Emit(OpCodes.Ldc_I4, value);
            }
        }

        private void GenerateConvert(ILGenerator gen, UnaryExpression u)
        {
            if (u.Method != null)
            {
                if (u.IsLifted && (!u.Type.IsValueType || !u.Operand.Type.IsValueType))
                {
                    ParameterInfo[] parameters = u.Method.GetParameters();
                    Type parameterType = parameters[0].ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                    }
                    Expression node = Expression.Convert(Expression.Call(null, u.Method, new Expression[] { Expression.Convert(u.Operand, parameters[0].ParameterType) }), u.Type);
                    this.Generate(gen, node, StackType.Value);
                }
                else
                {
                    this.GenerateUnaryMethod(gen, u, StackType.Value);
                }
            }
            else
            {
                this.Generate(gen, u.Operand, StackType.Value);
                this.GenerateConvertToType(gen, u.Operand.Type, u.Type, u.NodeType == ExpressionType.ConvertChecked);
            }
        }

        private void GenerateConvertToType(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            if (typeFrom != typeTo)
            {
                bool flag = IsNullable(typeFrom);
                bool flag2 = IsNullable(typeTo);
                Type nonNullableType = GetNonNullableType(typeFrom);
                Type c = GetNonNullableType(typeTo);
                if ((typeFrom.IsInterface || typeTo.IsInterface) || ((typeFrom == typeof(object)) || (typeTo == typeof(object))))
                {
                    this.GenerateCastToType(gen, typeFrom, typeTo);
                }
                else if (flag || flag2)
                {
                    this.GenerateNullableConversion(gen, typeFrom, typeTo, isChecked);
                }
                else if ((!IsConvertible(typeFrom) || !IsConvertible(typeTo)) && (nonNullableType.IsAssignableFrom(c) || c.IsAssignableFrom(nonNullableType)))
                {
                    this.GenerateCastToType(gen, typeFrom, typeTo);
                }
                else if (typeFrom.IsArray && typeTo.IsArray)
                {
                    this.GenerateCastToType(gen, typeFrom, typeTo);
                }
                else
                {
                    this.GenerateNumericConversion(gen, typeFrom, typeTo, isChecked);
                }
            }
        }

        private void GenerateCreateDelegate(ILGenerator gen, LambdaExpression lambda)
        {
            int num = this.GenerateLambda(lambda);
            GenerateLoadExecutionScope(gen);
            this.GenerateConstInt(gen, num);
            if (this.scope.HoistedLocalsVar != null)
            {
                this.GenerateLoadHoistedLocals(gen);
            }
            else
            {
                gen.Emit(OpCodes.Ldnull);
            }
            gen.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateDelegate", BindingFlags.Public | BindingFlags.Instance));
            gen.Emit(OpCodes.Castclass, lambda.Type);
        }

        private void GenerateFieldAccess(ILGenerator gen, FieldInfo fi, StackType ask)
        {
            StackType address;
            if (fi.IsLiteral)
            {
                address = this.GenerateConstant(gen, fi.FieldType, fi.GetRawConstantValue(), ask);
            }
            else
            {
                OpCode code;
                if ((ask == StackType.Value) || fi.IsInitOnly)
                {
                    code = fi.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
                    address = StackType.Value;
                }
                else
                {
                    code = fi.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda;
                    address = StackType.Address;
                }
                gen.Emit(code, fi);
            }
            if ((ask == StackType.Address) && (address == StackType.Value))
            {
                LocalBuilder local = gen.DeclareLocal(fi.FieldType);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
            }
        }

        private void GenerateGetValue(ILGenerator gen, Type nullableType)
        {
            MethodInfo method = nullableType.GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);
            gen.Emit(OpCodes.Call, method);
        }

        private void GenerateGetValueOrDefault(ILGenerator gen, Type nullableType)
        {
            MethodInfo method = nullableType.GetMethod("GetValueOrDefault", Type.EmptyTypes);
            gen.Emit(OpCodes.Call, method);
        }

        private StackType GenerateGlobalAccess(ILGenerator gen, int iGlobal, Type type, StackType ask)
        {
            GenerateLoadExecutionScope(gen);
            gen.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Globals", BindingFlags.Public | BindingFlags.Instance));
            this.GenerateConstInt(gen, iGlobal);
            gen.Emit(OpCodes.Ldelem_Ref);
            Type cls = MakeStrongBoxType(type);
            gen.Emit(OpCodes.Castclass, cls);
            FieldInfo field = cls.GetField("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (ask == StackType.Value)
            {
                gen.Emit(OpCodes.Ldfld, field);
                return ask;
            }
            gen.Emit(OpCodes.Ldflda, field);
            return ask;
        }

        private void GenerateHasValue(ILGenerator gen, Type nullableType)
        {
            MethodInfo method = nullableType.GetMethod("get_HasValue", BindingFlags.Public | BindingFlags.Instance);
            gen.Emit(OpCodes.Call, method);
        }

        private StackType GenerateHoistedLocalAccess(ILGenerator gen, int hoistIndex, Type type, StackType ask)
        {
            this.GenerateConstInt(gen, hoistIndex);
            gen.Emit(OpCodes.Ldelem_Ref);
            Type cls = MakeStrongBoxType(type);
            gen.Emit(OpCodes.Castclass, cls);
            FieldInfo field = cls.GetField("Value", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (ask == StackType.Value)
            {
                gen.Emit(OpCodes.Ldfld, field);
                return ask;
            }
            gen.Emit(OpCodes.Ldflda, field);
            return ask;
        }

        private void GenerateInitHoistedLocals(ILGenerator gen)
        {
            if (this.scope.HoistedLocals.Count != 0)
            {
                this.scope.HoistedLocalsVar = gen.DeclareLocal(typeof(object[]));
                GenerateLoadExecutionScope(gen);
                gen.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateHoistedLocals", BindingFlags.Public | BindingFlags.Instance));
                gen.Emit(OpCodes.Stloc, this.scope.HoistedLocalsVar);
                int count = this.scope.Lambda.Parameters.Count;
                for (int i = 0; i < count; i++)
                {
                    ParameterExpression p = this.scope.Lambda.Parameters[i];
                    if (this.IsHoisted(p))
                    {
                        this.PrepareInitLocal(gen, p);
                        this.GenerateArgAccess(gen, i + 1, StackType.Value);
                        this.GenerateInitLocal(gen, p);
                    }
                }
            }
        }

        private void GenerateInitLocal(ILGenerator gen, ParameterExpression p)
        {
            int num;
            if (this.scope.HoistedLocals.TryGetValue(p, out num))
            {
                ConstructorInfo constructor = MakeStrongBoxType(p.Type).GetConstructor(new Type[] { p.Type });
                gen.Emit(OpCodes.Newobj, constructor);
                gen.Emit(OpCodes.Stelem_Ref);
            }
            else
            {
                LocalBuilder builder;
                if (!this.scope.Locals.TryGetValue(p, out builder))
                {
                    throw Error.NotSupported();
                }
                gen.Emit(OpCodes.Stloc, builder);
            }
        }

        private StackType GenerateInvoke(ILGenerator gen, InvocationExpression invoke, StackType ask)
        {
            LambdaExpression expression = (invoke.Expression.NodeType == ExpressionType.Quote) ? ((LambdaExpression) ((UnaryExpression) invoke.Expression).Operand) : (invoke.Expression as LambdaExpression);
            if (expression != null)
            {
                int num = 0;
                int count = invoke.Arguments.Count;
                while (num < count)
                {
                    ParameterExpression p = expression.Parameters[num];
                    this.PrepareInitLocal(gen, p);
                    this.Generate(gen, invoke.Arguments[num], StackType.Value);
                    this.GenerateInitLocal(gen, p);
                    num++;
                }
                return this.Generate(gen, expression.Body, ask);
            }
            Expression instance = invoke.Expression;
            if (typeof(LambdaExpression).IsAssignableFrom(instance.Type))
            {
                instance = Expression.Call(instance, instance.Type.GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance));
            }
            instance = Expression.Call(instance, instance.Type.GetMethod("Invoke"), invoke.Arguments);
            return this.Generate(gen, instance, ask);
        }

        private int GenerateLambda(LambdaExpression lambda)
        {
            this.scope = new CompileScope(this.scope, lambda);
            MethodInfo mi = lambda.Type.GetMethod("Invoke");
            new Hoister().Hoist(this.scope);
            DynamicMethod method = new DynamicMethod("lambda_method", mi.ReturnType, this.GetParameterTypes(mi), true);
            ILGenerator iLGenerator = method.GetILGenerator();
            MethodInfo info2 = method;
            this.GenerateInitHoistedLocals(iLGenerator);
            this.Generate(iLGenerator, lambda.Body, StackType.Value);
            if ((mi.ReturnType == typeof(void)) && (lambda.Body.Type != typeof(void)))
            {
                iLGenerator.Emit(OpCodes.Pop);
            }
            iLGenerator.Emit(OpCodes.Ret);
            int count = this.lambdas.Count;
            this.lambdas.Add(new LambdaInfo(lambda, info2, this.scope.HoistedLocals, this.lambdas));
            this.scope = this.scope.Parent;
            return count;
        }

        private void GenerateLambdaReferenceCoalesce(ILGenerator gen, BinaryExpression b)
        {
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Dup);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.Emit(OpCodes.Pop);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Br, label);
            gen.MarkLabel(label2);
            ParameterExpression p = b.Conversion.Parameters[0];
            this.PrepareInitLocal(gen, p);
            this.GenerateInitLocal(gen, p);
            this.Generate(gen, b.Conversion.Body, StackType.Value);
            gen.MarkLabel(label);
        }

        private StackType GenerateLift(ILGenerator gen, ExpressionType nodeType, Type resultType, MethodCallExpression mc, IEnumerable<ParameterExpression> parameters, IEnumerable<Expression> arguments)
        {
            ReadOnlyCollection<ParameterExpression> onlys = parameters.ToReadOnlyCollection<ParameterExpression>();
            ReadOnlyCollection<Expression> onlys2 = arguments.ToReadOnlyCollection<Expression>();
            switch (nodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                {
                    if (resultType == Expression.GetNullableType(mc.Type))
                    {
                        break;
                    }
                    Label label3 = gen.DefineLabel();
                    Label label4 = gen.DefineLabel();
                    Label label5 = gen.DefineLabel();
                    LocalBuilder builder4 = gen.DeclareLocal(typeof(bool));
                    LocalBuilder builder5 = gen.DeclareLocal(typeof(bool));
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Stloc, builder4);
                    gen.Emit(OpCodes.Ldc_I4_1);
                    gen.Emit(OpCodes.Stloc, builder5);
                    int num3 = 0;
                    int num4 = onlys.Count;
                    while (num3 < num4)
                    {
                        ParameterExpression p = onlys[num3];
                        Expression node = onlys2[num3];
                        this.PrepareInitLocal(gen, p);
                        if (IsNullable(node.Type))
                        {
                            if (this.Generate(gen, node, StackType.Address) == StackType.Value)
                            {
                                LocalBuilder builder6 = gen.DeclareLocal(node.Type);
                                gen.Emit(OpCodes.Stloc, builder6);
                                gen.Emit(OpCodes.Ldloca, builder6);
                            }
                            gen.Emit(OpCodes.Dup);
                            this.GenerateHasValue(gen, node.Type);
                            gen.Emit(OpCodes.Ldc_I4_0);
                            gen.Emit(OpCodes.Ceq);
                            gen.Emit(OpCodes.Dup);
                            gen.Emit(OpCodes.Ldloc, builder4);
                            gen.Emit(OpCodes.Or);
                            gen.Emit(OpCodes.Stloc, builder4);
                            gen.Emit(OpCodes.Ldloc, builder5);
                            gen.Emit(OpCodes.And);
                            gen.Emit(OpCodes.Stloc, builder5);
                            this.GenerateGetValueOrDefault(gen, node.Type);
                        }
                        else
                        {
                            this.Generate(gen, node, StackType.Value);
                            if (!node.Type.IsValueType)
                            {
                                gen.Emit(OpCodes.Dup);
                                gen.Emit(OpCodes.Ldnull);
                                gen.Emit(OpCodes.Ceq);
                                gen.Emit(OpCodes.Dup);
                                gen.Emit(OpCodes.Ldloc, builder4);
                                gen.Emit(OpCodes.Or);
                                gen.Emit(OpCodes.Stloc, builder4);
                                gen.Emit(OpCodes.Ldloc, builder5);
                                gen.Emit(OpCodes.And);
                                gen.Emit(OpCodes.Stloc, builder5);
                            }
                            else
                            {
                                gen.Emit(OpCodes.Ldc_I4_0);
                                gen.Emit(OpCodes.Stloc, builder5);
                            }
                        }
                        this.GenerateInitLocal(gen, p);
                        num3++;
                    }
                    gen.Emit(OpCodes.Ldloc, builder5);
                    gen.Emit(OpCodes.Brtrue, label4);
                    gen.Emit(OpCodes.Ldloc, builder4);
                    gen.Emit(OpCodes.Brtrue, label5);
                    this.Generate(gen, mc, StackType.Value);
                    if (IsNullable(resultType) && (resultType != mc.Type))
                    {
                        ConstructorInfo constructor = resultType.GetConstructor(new Type[] { mc.Type });
                        gen.Emit(OpCodes.Newobj, constructor);
                    }
                    gen.Emit(OpCodes.Br_S, label3);
                    gen.MarkLabel(label4);
                    bool flag = nodeType == ExpressionType.Equal;
                    this.GenerateConstant(gen, Expression.Constant(flag), StackType.Value);
                    gen.Emit(OpCodes.Br_S, label3);
                    gen.MarkLabel(label5);
                    flag = nodeType == ExpressionType.NotEqual;
                    this.GenerateConstant(gen, Expression.Constant(flag), StackType.Value);
                    gen.MarkLabel(label3);
                    return StackType.Value;
                }
            }
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(typeof(bool));
            int num = 0;
            int count = onlys.Count;
            while (num < count)
            {
                ParameterExpression expression = onlys[num];
                Expression expression2 = onlys2[num];
                if (IsNullable(expression2.Type))
                {
                    this.PrepareInitLocal(gen, expression);
                    if (this.Generate(gen, expression2, StackType.Address) == StackType.Value)
                    {
                        LocalBuilder builder2 = gen.DeclareLocal(expression2.Type);
                        gen.Emit(OpCodes.Stloc, builder2);
                        gen.Emit(OpCodes.Ldloca, builder2);
                    }
                    gen.Emit(OpCodes.Dup);
                    this.GenerateHasValue(gen, expression2.Type);
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ceq);
                    gen.Emit(OpCodes.Stloc, local);
                    this.GenerateGetValueOrDefault(gen, expression2.Type);
                    this.GenerateInitLocal(gen, expression);
                }
                else
                {
                    this.PrepareInitLocal(gen, expression);
                    this.Generate(gen, expression2, StackType.Value);
                    if (!expression2.Type.IsValueType)
                    {
                        gen.Emit(OpCodes.Dup);
                        gen.Emit(OpCodes.Ldnull);
                        gen.Emit(OpCodes.Ceq);
                        gen.Emit(OpCodes.Stloc, local);
                    }
                    this.GenerateInitLocal(gen, expression);
                }
                gen.Emit(OpCodes.Ldloc, local);
                gen.Emit(OpCodes.Brtrue, label2);
                num++;
            }
            this.Generate(gen, mc, StackType.Value);
            if (IsNullable(resultType) && (resultType != mc.Type))
            {
                ConstructorInfo con = resultType.GetConstructor(new Type[] { mc.Type });
                gen.Emit(OpCodes.Newobj, con);
            }
            gen.Emit(OpCodes.Br_S, label);
            gen.MarkLabel(label2);
            if (resultType == Expression.GetNullableType(mc.Type))
            {
                if (resultType.IsValueType)
                {
                    LocalBuilder builder3 = gen.DeclareLocal(resultType);
                    gen.Emit(OpCodes.Ldloca, builder3);
                    gen.Emit(OpCodes.Initobj, resultType);
                    gen.Emit(OpCodes.Ldloc, builder3);
                }
                else
                {
                    gen.Emit(OpCodes.Ldnull);
                }
            }
            else
            {
                switch (nodeType)
                {
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                        gen.Emit(OpCodes.Ldc_I4_0);
                        break;
                }
            }
            gen.MarkLabel(label);
            return StackType.Value;
        }

        private StackType GenerateLiftedAndAlso(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            Type localType = typeof(bool?);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            Label label4 = gen.DefineLabel();
            Label label5 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(localType);
            LocalBuilder builder2 = gen.DeclareLocal(localType);
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue, label2);
            gen.MarkLabel(label);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse_S, label3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label3);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label4);
            ConstructorInfo constructor = localType.GetConstructor(new Type[] { typeof(bool) });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label5);
            gen.MarkLabel(label3);
            gen.Emit(OpCodes.Ldloca, local);
            gen.Emit(OpCodes.Initobj, localType);
            gen.MarkLabel(label5);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private StackType GenerateLiftedBinaryArithmetic(ILGenerator gen, ExpressionType op, Type leftType, Type rightType, Type resultType, StackType ask)
        {
            bool flag = IsNullable(leftType);
            bool flag2 = IsNullable(rightType);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(leftType);
            LocalBuilder builder2 = gen.DeclareLocal(rightType);
            LocalBuilder builder3 = gen.DeclareLocal(resultType);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            if (flag && flag2)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.And);
                gen.Emit(OpCodes.Brfalse_S, label);
            }
            else if (flag)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Brfalse_S, label);
            }
            else if (flag2)
            {
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.Brfalse_S, label);
            }
            if (flag)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateGetValueOrDefault(gen, leftType);
            }
            else
            {
                gen.Emit(OpCodes.Ldloc, local);
            }
            if (flag2)
            {
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateGetValueOrDefault(gen, rightType);
            }
            else
            {
                gen.Emit(OpCodes.Ldloc, builder2);
            }
            this.GenerateBinaryOp(gen, op, GetNonNullableType(leftType), GetNonNullableType(rightType), GetNonNullableType(resultType), false, StackType.Value);
            ConstructorInfo constructor = resultType.GetConstructor(new Type[] { GetNonNullableType(resultType) });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, builder3);
            gen.Emit(OpCodes.Br_S, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloca, builder3);
            gen.Emit(OpCodes.Initobj, resultType);
            gen.MarkLabel(label2);
            return this.ReturnFromLocal(gen, ask, builder3);
        }

        private StackType GenerateLiftedBinaryOp(ILGenerator gen, ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull, StackType ask)
        {
            switch (op)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return this.GenerateLiftedBinaryArithmetic(gen, op, leftType, rightType, resultType, ask);

                case ExpressionType.And:
                    if (leftType != typeof(bool?))
                    {
                        return this.GenerateLiftedBinaryArithmetic(gen, op, leftType, rightType, resultType, ask);
                    }
                    return this.GenerateLiftedBooleanAnd(gen, ask);

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    return this.GenerateLiftedRelational(gen, op, leftType, rightType, resultType, liftedToNull, ask);

                case ExpressionType.Or:
                    if (leftType != typeof(bool?))
                    {
                        return this.GenerateLiftedBinaryArithmetic(gen, op, leftType, rightType, resultType, ask);
                    }
                    return this.GenerateLiftedBooleanOr(gen, ask);
            }
            return StackType.Value;
        }

        private StackType GenerateLiftedBooleanAnd(ILGenerator gen, StackType ask)
        {
            Type localType = typeof(bool?);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            Label label4 = gen.DefineLabel();
            Label label5 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(localType);
            LocalBuilder builder2 = gen.DeclareLocal(localType);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse_S, label3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brtrue_S, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label3);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label4);
            ConstructorInfo constructor = localType.GetConstructor(new Type[] { typeof(bool) });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label5);
            gen.MarkLabel(label3);
            gen.Emit(OpCodes.Ldloca, local);
            gen.Emit(OpCodes.Initobj, localType);
            gen.MarkLabel(label5);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private StackType GenerateLiftedBooleanOr(ILGenerator gen, StackType ask)
        {
            Type localType = typeof(bool?);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            Label label4 = gen.DefineLabel();
            Label label5 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(localType);
            LocalBuilder builder2 = gen.DeclareLocal(localType);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse_S, label3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse_S, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label3);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label4);
            ConstructorInfo constructor = localType.GetConstructor(new Type[] { typeof(bool) });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label5);
            gen.MarkLabel(label3);
            gen.Emit(OpCodes.Ldloca, local);
            gen.Emit(OpCodes.Initobj, localType);
            gen.MarkLabel(label5);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private StackType GenerateLiftedOrElse(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            Type localType = typeof(bool?);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            Label label4 = gen.DefineLabel();
            Label label5 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(localType);
            LocalBuilder builder2 = gen.DeclareLocal(localType);
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.MarkLabel(label);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse_S, label3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, localType);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse_S, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, localType);
            gen.Emit(OpCodes.Brfalse, label3);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Br_S, label4);
            gen.MarkLabel(label4);
            ConstructorInfo constructor = localType.GetConstructor(new Type[] { typeof(bool) });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label5);
            gen.MarkLabel(label3);
            gen.Emit(OpCodes.Ldloca, local);
            gen.Emit(OpCodes.Initobj, localType);
            gen.MarkLabel(label5);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private StackType GenerateLiftedRelational(ILGenerator gen, ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull, StackType ask)
        {
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(leftType);
            LocalBuilder builder2 = gen.DeclareLocal(rightType);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            if (op == ExpressionType.Equal)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.And);
                gen.Emit(OpCodes.Dup);
                if (liftedToNull)
                {
                    gen.Emit(OpCodes.Brtrue_S, label);
                }
                else
                {
                    gen.Emit(OpCodes.Brtrue_S, label2);
                }
                gen.Emit(OpCodes.Pop);
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.And);
                gen.Emit(OpCodes.Dup);
                if (liftedToNull)
                {
                    gen.Emit(OpCodes.Brfalse_S, label);
                }
                else
                {
                    gen.Emit(OpCodes.Brfalse_S, label2);
                }
                gen.Emit(OpCodes.Pop);
            }
            else if (op == ExpressionType.NotEqual)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.Or);
                gen.Emit(OpCodes.Dup);
                if (liftedToNull)
                {
                    gen.Emit(OpCodes.Brfalse_S, label);
                }
                else
                {
                    gen.Emit(OpCodes.Brfalse_S, label2);
                }
                gen.Emit(OpCodes.Pop);
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
                gen.Emit(OpCodes.Or);
                gen.Emit(OpCodes.Dup);
                if (liftedToNull)
                {
                    gen.Emit(OpCodes.Brtrue_S, label);
                }
                else
                {
                    gen.Emit(OpCodes.Brtrue_S, label2);
                }
                gen.Emit(OpCodes.Pop);
            }
            else
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, leftType);
                gen.Emit(OpCodes.Ldloca, builder2);
                this.GenerateHasValue(gen, rightType);
                gen.Emit(OpCodes.And);
                gen.Emit(OpCodes.Dup);
                if (liftedToNull)
                {
                    gen.Emit(OpCodes.Brfalse_S, label);
                }
                else
                {
                    gen.Emit(OpCodes.Brfalse_S, label2);
                }
                gen.Emit(OpCodes.Pop);
            }
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, leftType);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, rightType);
            StackType type = this.GenerateBinaryOp(gen, op, GetNonNullableType(leftType), GetNonNullableType(rightType), GetNonNullableType(resultType), false, ask);
            gen.MarkLabel(label2);
            if (resultType != GetNonNullableType(resultType))
            {
                this.GenerateConvertToType(gen, GetNonNullableType(resultType), resultType, true);
            }
            gen.Emit(OpCodes.Br, label3);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Unbox_Any, resultType);
            gen.MarkLabel(label3);
            return type;
        }

        private StackType GenerateListInit(ILGenerator gen, ListInitExpression init)
        {
            this.Generate(gen, init.NewExpression, StackType.Value);
            LocalBuilder local = null;
            if (init.NewExpression.Type.IsValueType)
            {
                local = gen.DeclareLocal(init.NewExpression.Type);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
            }
            this.GenerateListInit(gen, init.Initializers, local == null, init.NewExpression.Type);
            if (local != null)
            {
                gen.Emit(OpCodes.Ldloc, local);
            }
            return StackType.Value;
        }

        private void GenerateListInit(ILGenerator gen, ReadOnlyCollection<ElementInit> initializers, bool keepOnStack, Type objectType)
        {
            int num = 0;
            int count = initializers.Count;
            while (num < count)
            {
                if (keepOnStack || (num < (count - 1)))
                {
                    gen.Emit(OpCodes.Dup);
                }
                this.GenerateMethodCall(gen, initializers[num].AddMethod, initializers[num].Arguments, objectType);
                if (initializers[num].AddMethod.ReturnType != typeof(void))
                {
                    gen.Emit(OpCodes.Pop);
                }
                num++;
            }
        }

        private static void GenerateLoadExecutionScope(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);
        }

        private void GenerateLoadHoistedLocals(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldloc, this.scope.HoistedLocalsVar);
        }

        private StackType GenerateMemberAccess(ILGenerator gen, MemberExpression m, StackType ask) => 
            this.GenerateMemberAccess(gen, m.Expression, m.Member, ask);

        private StackType GenerateMemberAccess(ILGenerator gen, MemberInfo member, StackType ask)
        {
            FieldInfo fi = member as FieldInfo;
            if (fi != null)
            {
                this.GenerateFieldAccess(gen, fi, ask);
                return ask;
            }
            PropertyInfo info2 = member as PropertyInfo;
            if (info2 == null)
            {
                throw Error.UnhandledMemberAccess(member);
            }
            MethodInfo getMethod = info2.GetGetMethod(true);
            gen.Emit(this.UseVirtual(getMethod) ? OpCodes.Callvirt : OpCodes.Call, getMethod);
            return StackType.Value;
        }

        private StackType GenerateMemberAccess(ILGenerator gen, Expression expression, MemberInfo member, StackType ask)
        {
            FieldInfo info = member as FieldInfo;
            if (info != null)
            {
                if (!info.IsStatic)
                {
                    StackType type = expression.Type.IsValueType ? StackType.Address : StackType.Value;
                    if (this.Generate(gen, expression, type) != type)
                    {
                        LocalBuilder local = gen.DeclareLocal(expression.Type);
                        gen.Emit(OpCodes.Stloc, local);
                        gen.Emit(OpCodes.Ldloca, local);
                    }
                }
                return this.GenerateMemberAccess(gen, member, ask);
            }
            PropertyInfo info2 = member as PropertyInfo;
            if (info2 == null)
            {
                throw Error.UnhandledMemberAccess(member);
            }
            if (!info2.GetGetMethod(true).IsStatic)
            {
                StackType type3 = expression.Type.IsValueType ? StackType.Address : StackType.Value;
                if (this.Generate(gen, expression, type3) != type3)
                {
                    LocalBuilder builder2 = gen.DeclareLocal(expression.Type);
                    gen.Emit(OpCodes.Stloc, builder2);
                    gen.Emit(OpCodes.Ldloca, builder2);
                }
            }
            return this.GenerateMemberAccess(gen, member, ask);
        }

        private void GenerateMemberAssignment(ILGenerator gen, MemberAssignment binding, Type objectType)
        {
            this.Generate(gen, binding.Expression, StackType.Value);
            FieldInfo member = binding.Member as FieldInfo;
            if (member != null)
            {
                gen.Emit(OpCodes.Stfld, member);
            }
            else
            {
                PropertyInfo info2 = binding.Member as PropertyInfo;
                MethodInfo setMethod = info2.GetSetMethod(true);
                if (info2 == null)
                {
                    throw Error.UnhandledBinding();
                }
                if (this.UseVirtual(setMethod))
                {
                    if (objectType.IsValueType)
                    {
                        gen.Emit(OpCodes.Constrained, objectType);
                    }
                    gen.Emit(OpCodes.Callvirt, setMethod);
                }
                else
                {
                    gen.Emit(OpCodes.Call, setMethod);
                }
            }
        }

        private StackType GenerateMemberInit(ILGenerator gen, MemberInitExpression init)
        {
            this.Generate(gen, init.NewExpression, StackType.Value);
            LocalBuilder local = null;
            if (init.NewExpression.Type.IsValueType && (init.Bindings.Count > 0))
            {
                local = gen.DeclareLocal(init.NewExpression.Type);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
            }
            this.GenerateMemberInit(gen, init.Bindings, local == null, init.NewExpression.Type);
            if (local != null)
            {
                gen.Emit(OpCodes.Ldloc, local);
            }
            return StackType.Value;
        }

        private void GenerateMemberInit(ILGenerator gen, ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack, Type objectType)
        {
            int num = 0;
            int count = bindings.Count;
            while (num < count)
            {
                if (keepOnStack || (num < (count - 1)))
                {
                    gen.Emit(OpCodes.Dup);
                }
                this.GenerateBinding(gen, bindings[num], objectType);
                num++;
            }
        }

        private void GenerateMemberListBinding(ILGenerator gen, MemberListBinding binding)
        {
            Type memberType = this.GetMemberType(binding.Member);
            if ((binding.Member is PropertyInfo) && memberType.IsValueType)
            {
                throw Error.CannotAutoInitializeValueTypeElementThroughProperty(binding.Member);
            }
            StackType ask = memberType.IsValueType ? StackType.Address : StackType.Value;
            if ((this.GenerateMemberAccess(gen, binding.Member, ask) != StackType.Address) && memberType.IsValueType)
            {
                LocalBuilder local = gen.DeclareLocal(memberType);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
            }
            this.GenerateListInit(gen, binding.Initializers, false, memberType);
        }

        private void GenerateMemberMemberBinding(ILGenerator gen, MemberMemberBinding binding)
        {
            Type memberType = this.GetMemberType(binding.Member);
            if ((binding.Member is PropertyInfo) && memberType.IsValueType)
            {
                throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(binding.Member);
            }
            StackType ask = memberType.IsValueType ? StackType.Address : StackType.Value;
            if ((this.GenerateMemberAccess(gen, binding.Member, ask) != ask) && memberType.IsValueType)
            {
                LocalBuilder local = gen.DeclareLocal(memberType);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
            }
            if (binding.Bindings.Count == 0)
            {
                gen.Emit(OpCodes.Pop);
            }
            else
            {
                this.GenerateMemberInit(gen, binding.Bindings, false, memberType);
            }
        }

        private void GenerateMemberWriteBack(ILGenerator gen, Expression expression, MemberInfo member, LocalBuilder loc)
        {
            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                if (!field.IsStatic)
                {
                    StackType ask = expression.Type.IsValueType ? StackType.Address : StackType.Value;
                    this.Generate(gen, expression, ask);
                    gen.Emit(OpCodes.Ldloc, loc);
                    gen.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    gen.Emit(OpCodes.Ldloc, loc);
                    gen.Emit(OpCodes.Stsfld, field);
                }
            }
            else
            {
                PropertyInfo info2 = member as PropertyInfo;
                if (info2 == null)
                {
                    throw Error.UnhandledMemberAccess(member);
                }
                MethodInfo setMethod = info2.GetSetMethod(true);
                if (setMethod != null)
                {
                    if (!setMethod.IsStatic)
                    {
                        StackType type2 = expression.Type.IsValueType ? StackType.Address : StackType.Value;
                        this.Generate(gen, expression, type2);
                    }
                    gen.Emit(OpCodes.Ldloc, loc);
                    gen.Emit(this.UseVirtual(setMethod) ? OpCodes.Callvirt : OpCodes.Call, setMethod);
                }
            }
        }

        private void GenerateMethodAndAlso(ILGenerator gen, BinaryExpression b)
        {
            Label label = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Dup);
            Type parameterType = b.Method.GetParameters()[0].ParameterType;
            Type[] types = new Type[] { parameterType };
            MethodInfo meth = parameterType.GetMethod("op_False", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Call, meth);
            gen.Emit(OpCodes.Brtrue, label);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Call, b.Method);
            gen.MarkLabel(label);
        }

        private StackType GenerateMethodCall(ILGenerator gen, MethodCallExpression mc, StackType ask)
        {
            StackType address = StackType.Value;
            MethodInfo method = mc.Method;
            if (!mc.Method.IsStatic)
            {
                StackType type2 = mc.Object.Type.IsValueType ? StackType.Address : StackType.Value;
                if (this.Generate(gen, mc.Object, type2) != type2)
                {
                    LocalBuilder local = gen.DeclareLocal(mc.Object.Type);
                    gen.Emit(OpCodes.Stloc, local);
                    gen.Emit(OpCodes.Ldloca, local);
                }
                if (((ask == StackType.Address) && mc.Object.Type.IsArray) && (method == mc.Object.Type.GetMethod("Get", BindingFlags.Public | BindingFlags.Instance)))
                {
                    method = mc.Object.Type.GetMethod("Address", BindingFlags.Public | BindingFlags.Instance);
                    address = StackType.Address;
                }
            }
            this.GenerateMethodCall(gen, method, mc.Arguments, mc.Object?.Type);
            return address;
        }

        private void GenerateMethodCall(ILGenerator gen, MethodInfo mi, ReadOnlyCollection<Expression> args, Type objectType)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            List<WriteBack> list = this.GenerateArgs(gen, parameters, args);
            OpCode opcode = this.UseVirtual(mi) ? OpCodes.Callvirt : OpCodes.Call;
            if ((opcode == OpCodes.Callvirt) && objectType.IsValueType)
            {
                gen.Emit(OpCodes.Constrained, objectType);
            }
            if (mi.CallingConvention == CallingConventions.VarArgs)
            {
                Type[] optionalParameterTypes = new Type[args.Count];
                int index = 0;
                int length = optionalParameterTypes.Length;
                while (index < length)
                {
                    optionalParameterTypes[index] = args[index].Type;
                    index++;
                }
                gen.EmitCall(opcode, mi, optionalParameterTypes);
            }
            else
            {
                gen.Emit(opcode, mi);
            }
            foreach (WriteBack back in list)
            {
                this.GenerateWriteBack(gen, back);
            }
        }

        private void GenerateMethodOrElse(ILGenerator gen, BinaryExpression b)
        {
            Label label = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Dup);
            Type parameterType = b.Method.GetParameters()[0].ParameterType;
            Type[] types = new Type[] { parameterType };
            MethodInfo meth = parameterType.GetMethod("op_True", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Call, meth);
            gen.Emit(OpCodes.Brtrue, label);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Call, b.Method);
            gen.MarkLabel(label);
        }

        private StackType GenerateNew(ILGenerator gen, NewExpression nex, StackType ask)
        {
            LocalBuilder local = null;
            if (nex.Type.IsValueType)
            {
                local = gen.DeclareLocal(nex.Type);
            }
            if (nex.Constructor != null)
            {
                ParameterInfo[] parameters = nex.Constructor.GetParameters();
                this.GenerateArgs(gen, parameters, nex.Arguments);
                gen.Emit(OpCodes.Newobj, nex.Constructor);
                if (nex.Type.IsValueType)
                {
                    gen.Emit(OpCodes.Stloc, local);
                }
            }
            else if (nex.Type.IsValueType)
            {
                gen.Emit(OpCodes.Ldloca, local);
                gen.Emit(OpCodes.Initobj, nex.Type);
            }
            else
            {
                ConstructorInfo constructor = nex.Type.GetConstructor(Type.EmptyTypes);
                gen.Emit(OpCodes.Newobj, constructor);
            }
            if (nex.Type.IsValueType)
            {
                return this.ReturnFromLocal(gen, ask, local);
            }
            return StackType.Value;
        }

        private void GenerateNewArray(ILGenerator gen, NewArrayExpression nex)
        {
            Type elementType = nex.Type.GetElementType();
            if (nex.NodeType == ExpressionType.NewArrayInit)
            {
                this.GenerateConstInt(gen, nex.Expressions.Count);
                gen.Emit(OpCodes.Newarr, elementType);
                int num = 0;
                int count = nex.Expressions.Count;
                while (num < count)
                {
                    gen.Emit(OpCodes.Dup);
                    this.GenerateConstInt(gen, num);
                    this.Generate(gen, nex.Expressions[num], StackType.Value);
                    this.GenerateArrayAssign(gen, elementType);
                    num++;
                }
            }
            else
            {
                Type[] types = new Type[nex.Expressions.Count];
                int index = 0;
                int length = types.Length;
                while (index < length)
                {
                    types[index] = typeof(int);
                    index++;
                }
                int num5 = 0;
                int num6 = nex.Expressions.Count;
                while (num5 < num6)
                {
                    Expression node = nex.Expressions[num5];
                    this.Generate(gen, node, StackType.Value);
                    if (node.Type != typeof(int))
                    {
                        this.GenerateConvertToType(gen, node.Type, typeof(int), true);
                    }
                    num5++;
                }
                if (nex.Expressions.Count > 1)
                {
                    int[] lengths = new int[nex.Expressions.Count];
                    ConstructorInfo constructor = Array.CreateInstance(elementType, lengths).GetType().GetConstructor(types);
                    gen.Emit(OpCodes.Newobj, constructor);
                }
                else
                {
                    gen.Emit(OpCodes.Newarr, elementType);
                }
            }
        }

        private void GenerateNonNullableToNullableConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            LocalBuilder local = null;
            local = gen.DeclareLocal(typeTo);
            Type nonNullableType = GetNonNullableType(typeTo);
            this.GenerateConvertToType(gen, typeFrom, nonNullableType, isChecked);
            ConstructorInfo constructor = typeTo.GetConstructor(new Type[] { nonNullableType });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloc, local);
        }

        private void GenerateNullableCoalesce(ILGenerator gen, BinaryExpression b)
        {
            LocalBuilder local = gen.DeclareLocal(b.Left.Type);
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, b.Left.Type);
            gen.Emit(OpCodes.Brfalse, label);
            Type nonNullableType = GetNonNullableType(b.Left.Type);
            if (b.Method != null)
            {
                if (!b.Method.GetParameters()[0].ParameterType.IsAssignableFrom(b.Left.Type))
                {
                    gen.Emit(OpCodes.Ldloca, local);
                    this.GenerateGetValueOrDefault(gen, b.Left.Type);
                }
                else
                {
                    gen.Emit(OpCodes.Ldloc, local);
                }
                gen.Emit(OpCodes.Call, b.Method);
            }
            else if (b.Conversion != null)
            {
                ParameterExpression p = b.Conversion.Parameters[0];
                this.PrepareInitLocal(gen, p);
                if (!p.Type.IsAssignableFrom(b.Left.Type))
                {
                    gen.Emit(OpCodes.Ldloca, local);
                    this.GenerateGetValueOrDefault(gen, b.Left.Type);
                }
                else
                {
                    gen.Emit(OpCodes.Ldloc, local);
                }
                this.GenerateInitLocal(gen, p);
                this.Generate(gen, b.Conversion.Body, StackType.Value);
            }
            else if (b.Type != nonNullableType)
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateGetValueOrDefault(gen, b.Left.Type);
                this.GenerateConvertToType(gen, nonNullableType, b.Type, true);
            }
            else
            {
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateGetValueOrDefault(gen, b.Left.Type);
            }
            gen.Emit(OpCodes.Br, label2);
            gen.MarkLabel(label);
            this.Generate(gen, b.Right, StackType.Value);
            if (b.Right.Type != b.Type)
            {
                this.GenerateConvertToType(gen, b.Right.Type, b.Type, true);
            }
            gen.MarkLabel(label2);
        }

        private void GenerateNullableConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool flag = IsNullable(typeFrom);
            bool flag2 = IsNullable(typeTo);
            if (flag && flag2)
            {
                this.GenerateNullableToNullableConversion(gen, typeFrom, typeTo, isChecked);
            }
            else if (flag)
            {
                this.GenerateNullableToNonNullableConversion(gen, typeFrom, typeTo, isChecked);
            }
            else
            {
                this.GenerateNonNullableToNullableConversion(gen, typeFrom, typeTo, isChecked);
            }
        }

        private void GenerateNullableToNonNullableConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            if (typeTo.IsValueType)
            {
                this.GenerateNullableToNonNullableStructConversion(gen, typeFrom, typeTo, isChecked);
            }
            else
            {
                this.GenerateNullableToReferenceConversion(gen, typeFrom);
            }
        }

        private void GenerateNullableToNonNullableStructConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            LocalBuilder local = null;
            local = gen.DeclareLocal(typeFrom);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValue(gen, typeFrom);
            Type nonNullableType = GetNonNullableType(typeFrom);
            this.GenerateConvertToType(gen, nonNullableType, typeTo, isChecked);
        }

        private void GenerateNullableToNullableConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            Label label = new Label();
            Label label2 = new Label();
            LocalBuilder local = null;
            LocalBuilder builder2 = null;
            local = gen.DeclareLocal(typeFrom);
            gen.Emit(OpCodes.Stloc, local);
            builder2 = gen.DeclareLocal(typeTo);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, typeFrom);
            label = gen.DefineLabel();
            gen.Emit(OpCodes.Brfalse_S, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, typeFrom);
            Type nonNullableType = GetNonNullableType(typeFrom);
            Type type2 = GetNonNullableType(typeTo);
            this.GenerateConvertToType(gen, nonNullableType, type2, isChecked);
            ConstructorInfo constructor = typeTo.GetConstructor(new Type[] { type2 });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, builder2);
            label2 = gen.DefineLabel();
            gen.Emit(OpCodes.Br_S, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloca, builder2);
            gen.Emit(OpCodes.Initobj, typeTo);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldloc, builder2);
        }

        private void GenerateNullableToReferenceConversion(ILGenerator gen, Type typeFrom)
        {
            gen.Emit(OpCodes.Box, typeFrom);
        }

        private StackType GenerateNullEquality(ILGenerator gen, ExpressionType op, Expression e, bool isLiftedToNull)
        {
            this.Generate(gen, e, StackType.Value);
            if (isLiftedToNull)
            {
                gen.Emit(OpCodes.Pop);
                this.GenerateConstant(gen, Expression.Constant(null, typeof(bool?)), StackType.Value);
            }
            else
            {
                LocalBuilder local = gen.DeclareLocal(e.Type);
                gen.Emit(OpCodes.Stloc, local);
                gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(gen, e.Type);
                if (op == ExpressionType.Equal)
                {
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Ceq);
                }
            }
            return StackType.Value;
        }

        private void GenerateNumericConversion(ILGenerator gen, Type typeFrom, Type typeTo, bool isChecked)
        {
            bool flag = IsUnsigned(typeFrom);
            IsFloatingPoint(typeFrom);
            if (typeTo == typeof(float))
            {
                if (flag)
                {
                    gen.Emit(OpCodes.Conv_R_Un);
                }
                gen.Emit(OpCodes.Conv_R4);
            }
            else if (typeTo == typeof(double))
            {
                if (flag)
                {
                    gen.Emit(OpCodes.Conv_R_Un);
                }
                gen.Emit(OpCodes.Conv_R8);
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(typeTo);
                if (isChecked)
                {
                    if (flag)
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Char:
                            case TypeCode.UInt16:
                                gen.Emit(OpCodes.Conv_Ovf_U2_Un);
                                return;

                            case TypeCode.SByte:
                                gen.Emit(OpCodes.Conv_Ovf_I1_Un);
                                return;

                            case TypeCode.Byte:
                                gen.Emit(OpCodes.Conv_Ovf_U1_Un);
                                return;

                            case TypeCode.Int16:
                                gen.Emit(OpCodes.Conv_Ovf_I2_Un);
                                return;

                            case TypeCode.Int32:
                                gen.Emit(OpCodes.Conv_Ovf_I4_Un);
                                return;

                            case TypeCode.UInt32:
                                gen.Emit(OpCodes.Conv_Ovf_U4_Un);
                                return;

                            case TypeCode.Int64:
                                gen.Emit(OpCodes.Conv_Ovf_I8_Un);
                                return;

                            case TypeCode.UInt64:
                                gen.Emit(OpCodes.Conv_Ovf_U8_Un);
                                return;
                        }
                        throw Error.UnhandledConvert(typeTo);
                    }
                    switch (typeCode)
                    {
                        case TypeCode.Char:
                        case TypeCode.UInt16:
                            gen.Emit(OpCodes.Conv_Ovf_U2);
                            return;

                        case TypeCode.SByte:
                            gen.Emit(OpCodes.Conv_Ovf_I1);
                            return;

                        case TypeCode.Byte:
                            gen.Emit(OpCodes.Conv_Ovf_U1);
                            return;

                        case TypeCode.Int16:
                            gen.Emit(OpCodes.Conv_Ovf_I2);
                            return;

                        case TypeCode.Int32:
                            gen.Emit(OpCodes.Conv_Ovf_I4);
                            return;

                        case TypeCode.UInt32:
                            gen.Emit(OpCodes.Conv_Ovf_U4);
                            return;

                        case TypeCode.Int64:
                            gen.Emit(OpCodes.Conv_Ovf_I8);
                            return;

                        case TypeCode.UInt64:
                            gen.Emit(OpCodes.Conv_Ovf_U8);
                            return;
                    }
                    throw Error.UnhandledConvert(typeTo);
                }
                if (flag)
                {
                    switch (typeCode)
                    {
                        case TypeCode.Char:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                            gen.Emit(OpCodes.Conv_U2);
                            return;

                        case TypeCode.SByte:
                        case TypeCode.Byte:
                            gen.Emit(OpCodes.Conv_U1);
                            return;

                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                            gen.Emit(OpCodes.Conv_U4);
                            return;

                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            gen.Emit(OpCodes.Conv_U8);
                            return;
                    }
                    throw Error.UnhandledConvert(typeTo);
                }
                switch (typeCode)
                {
                    case TypeCode.Char:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        gen.Emit(OpCodes.Conv_I2);
                        return;

                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        gen.Emit(OpCodes.Conv_I1);
                        return;

                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        gen.Emit(OpCodes.Conv_I4);
                        return;

                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        gen.Emit(OpCodes.Conv_I8);
                        return;
                }
                throw Error.UnhandledConvert(typeTo);
            }
        }

        private StackType GenerateOrElse(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            if ((b.Method != null) && !IsLiftedLogicalBinaryOperator(b.Left.Type, b.Right.Type, b.Method))
            {
                this.GenerateMethodOrElse(gen, b);
            }
            else
            {
                if (b.Left.Type == typeof(bool?))
                {
                    return this.GenerateLiftedOrElse(gen, b, ask);
                }
                if (IsLiftedLogicalBinaryOperator(b.Left.Type, b.Right.Type, b.Method))
                {
                    return this.GenerateUserdefinedLiftedOrElse(gen, b, ask);
                }
                this.GenerateUnliftedOrElse(gen, b);
            }
            return StackType.Value;
        }

        private StackType GenerateParameterAccess(ILGenerator gen, ParameterExpression p, StackType ask)
        {
            LocalBuilder builder;
            int num;
            if (this.scope.Locals.TryGetValue(p, out builder))
            {
                if (ask == StackType.Value)
                {
                    gen.Emit(OpCodes.Ldloc, builder);
                    return ask;
                }
                gen.Emit(OpCodes.Ldloca, builder);
                return ask;
            }
            if (this.scope.HoistedLocals.TryGetValue(p, out num))
            {
                this.GenerateLoadHoistedLocals(gen);
                return this.GenerateHoistedLocalAccess(gen, num, p.Type, ask);
            }
            int num2 = 0;
            int count = this.scope.Lambda.Parameters.Count;
            while (num2 < count)
            {
                if (this.scope.Lambda.Parameters[num2] == p)
                {
                    return this.GenerateArgAccess(gen, num2 + 1, ask);
                }
                num2++;
            }
            GenerateLoadExecutionScope(gen);
            for (CompileScope scope = this.scope.Parent; scope != null; scope = scope.Parent)
            {
                if (scope.HoistedLocals.TryGetValue(p, out num))
                {
                    gen.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Locals", BindingFlags.Public | BindingFlags.Instance));
                    return this.GenerateHoistedLocalAccess(gen, num, p.Type, ask);
                }
                gen.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Parent", BindingFlags.Public | BindingFlags.Instance));
            }
            throw Error.LambdaParameterNotInScope();
        }

        private void GenerateQuote(ILGenerator gen, UnaryExpression quote)
        {
            GenerateLoadExecutionScope(gen);
            int iGlobal = this.AddGlobal(typeof(Expression), quote.Operand);
            this.GenerateGlobalAccess(gen, iGlobal, typeof(Expression), StackType.Value);
            if (this.scope.HoistedLocalsVar != null)
            {
                this.GenerateLoadHoistedLocals(gen);
            }
            else
            {
                gen.Emit(OpCodes.Ldnull);
            }
            MethodInfo method = typeof(ExecutionScope).GetMethod("IsolateExpression", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            gen.Emit(OpCodes.Callvirt, method);
            Type cls = quote.Operand.GetType();
            if (cls != typeof(Expression))
            {
                gen.Emit(OpCodes.Castclass, cls);
            }
        }

        private void GenerateReferenceCoalesceWithoutConversion(ILGenerator gen, BinaryExpression b)
        {
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Dup);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.Emit(OpCodes.Pop);
            this.Generate(gen, b.Right, StackType.Value);
            if (b.Right.Type != b.Type)
            {
                gen.Emit(OpCodes.Castclass, b.Type);
            }
            gen.Emit(OpCodes.Br_S, label);
            gen.MarkLabel(label2);
            if (b.Left.Type != b.Type)
            {
                gen.Emit(OpCodes.Castclass, b.Type);
            }
            gen.MarkLabel(label);
        }

        private void GenerateTypeIs(ILGenerator gen, TypeBinaryExpression b)
        {
            this.Generate(gen, b.Expression, StackType.Value);
            if (b.Expression.Type == typeof(void))
            {
                gen.Emit(OpCodes.Ldc_I4_0);
            }
            else
            {
                if (b.Expression.Type.IsValueType)
                {
                    gen.Emit(OpCodes.Box, b.Expression.Type);
                }
                gen.Emit(OpCodes.Isinst, b.TypeOperand);
                gen.Emit(OpCodes.Ldnull);
                gen.Emit(OpCodes.Cgt_Un);
            }
        }

        private StackType GenerateUnary(ILGenerator gen, UnaryExpression u, StackType ask)
        {
            if (u.Method != null)
            {
                return this.GenerateUnaryMethod(gen, u, ask);
            }
            if ((u.NodeType == ExpressionType.NegateChecked) && IsInteger(u.Operand.Type))
            {
                this.GenerateConstInt(gen, 0);
                this.GenerateConvertToType(gen, typeof(int), u.Operand.Type, false);
                this.Generate(gen, u.Operand, StackType.Value);
                return this.GenerateBinaryOp(gen, ExpressionType.SubtractChecked, u.Operand.Type, u.Operand.Type, u.Type, false, ask);
            }
            this.Generate(gen, u.Operand, StackType.Value);
            return this.GenerateUnaryOp(gen, u.NodeType, u.Operand.Type, u.Type, ask);
        }

        private StackType GenerateUnaryMethod(ILGenerator gen, UnaryExpression u, StackType ask)
        {
            if (u.IsLifted)
            {
                ParameterExpression expression = Expression.Parameter(Expression.GetNonNullableType(u.Operand.Type), null);
                MethodCallExpression mc = Expression.Call(null, u.Method, new Expression[] { expression });
                Type nullableType = Expression.GetNullableType(mc.Type);
                this.GenerateLift(gen, u.NodeType, nullableType, mc, new ParameterExpression[] { expression }, new Expression[] { u.Operand });
                this.GenerateConvertToType(gen, nullableType, u.Type, false);
                return StackType.Value;
            }
            MethodCallExpression node = Expression.Call(null, u.Method, new Expression[] { u.Operand });
            return this.Generate(gen, node, ask);
        }

        private StackType GenerateUnaryOp(ILGenerator gen, ExpressionType op, Type operandType, Type resultType, StackType ask)
        {
            bool flag = IsNullable(operandType);
            if (op == ExpressionType.ArrayLength)
            {
                gen.Emit(OpCodes.Ldlen);
                return StackType.Value;
            }
            if (!flag)
            {
                switch (op)
                {
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                        gen.Emit(OpCodes.Neg);
                        goto Label_030F;

                    case ExpressionType.UnaryPlus:
                        gen.Emit(OpCodes.Nop);
                        goto Label_030F;

                    case ExpressionType.Not:
                        if (operandType != typeof(bool))
                        {
                            gen.Emit(OpCodes.Not);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Ldc_I4_0);
                            gen.Emit(OpCodes.Ceq);
                        }
                        goto Label_030F;

                    case ExpressionType.TypeAs:
                        if (operandType.IsValueType)
                        {
                            gen.Emit(OpCodes.Box, operandType);
                        }
                        gen.Emit(OpCodes.Isinst, resultType);
                        if (IsNullable(resultType))
                        {
                            gen.Emit(OpCodes.Unbox_Any, resultType);
                        }
                        goto Label_030F;
                }
                throw Error.UnhandledUnary(op);
            }
            switch (op)
            {
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                    break;

                case ExpressionType.Not:
                {
                    if (operandType != typeof(bool?))
                    {
                        break;
                    }
                    gen.DefineLabel();
                    Label label = gen.DefineLabel();
                    LocalBuilder builder = gen.DeclareLocal(operandType);
                    gen.Emit(OpCodes.Stloc, builder);
                    gen.Emit(OpCodes.Ldloca, builder);
                    this.GenerateHasValue(gen, operandType);
                    gen.Emit(OpCodes.Brfalse_S, label);
                    gen.Emit(OpCodes.Ldloca, builder);
                    this.GenerateGetValueOrDefault(gen, operandType);
                    Type type = GetNonNullableType(operandType);
                    this.GenerateUnaryOp(gen, op, type, typeof(bool), StackType.Value);
                    ConstructorInfo con = resultType.GetConstructor(new Type[] { typeof(bool) });
                    gen.Emit(OpCodes.Newobj, con);
                    gen.Emit(OpCodes.Stloc, builder);
                    gen.MarkLabel(label);
                    return this.ReturnFromLocal(gen, ask, builder);
                }
                case ExpressionType.TypeAs:
                    gen.Emit(OpCodes.Box, operandType);
                    gen.Emit(OpCodes.Isinst, resultType);
                    if (IsNullable(resultType))
                    {
                        gen.Emit(OpCodes.Unbox_Any, resultType);
                    }
                    return StackType.Value;

                default:
                    throw Error.UnhandledUnary(op);
            }
            Label label2 = gen.DefineLabel();
            Label label3 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(operandType);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, operandType);
            gen.Emit(OpCodes.Brfalse_S, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, operandType);
            Type nonNullableType = GetNonNullableType(resultType);
            this.GenerateUnaryOp(gen, op, nonNullableType, nonNullableType, StackType.Value);
            ConstructorInfo constructor = resultType.GetConstructor(new Type[] { nonNullableType });
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br_S, label3);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Ldloca, local);
            gen.Emit(OpCodes.Initobj, resultType);
            gen.MarkLabel(label3);
            return this.ReturnFromLocal(gen, ask, local);
        Label_030F:
            return StackType.Value;
        }

        private void GenerateUnliftedAndAlso(ILGenerator gen, BinaryExpression b)
        {
            this.Generate(gen, b.Left, StackType.Value);
            Label label = gen.DefineLabel();
            gen.Emit(OpCodes.Dup);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Pop);
            this.Generate(gen, b.Right, StackType.Value);
            gen.MarkLabel(label);
        }

        private StackType GenerateUnliftedBinaryOp(ILGenerator gen, ExpressionType op, Type leftType, Type rightType)
        {
            Label label;
            Label label2;
            if ((op == ExpressionType.Equal) || (op == ExpressionType.NotEqual))
            {
                GenerateUnliftedEquality(gen, op, leftType);
                return StackType.Value;
            }
            if (!leftType.IsPrimitive)
            {
                throw Error.OperatorNotImplementedForType(op, leftType);
            }
            switch (op)
            {
                case ExpressionType.Add:
                    gen.Emit(OpCodes.Add);
                    goto Label_04AE;

                case ExpressionType.AddChecked:
                {
                    LocalBuilder local = gen.DeclareLocal(leftType);
                    LocalBuilder builder2 = gen.DeclareLocal(rightType);
                    gen.Emit(OpCodes.Stloc, builder2);
                    gen.Emit(OpCodes.Stloc, local);
                    gen.Emit(OpCodes.Ldloc, local);
                    gen.Emit(OpCodes.Ldloc, builder2);
                    if (!IsFloatingPoint(leftType))
                    {
                        if (IsUnsigned(leftType))
                        {
                            gen.Emit(OpCodes.Add_Ovf_Un);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Add_Ovf);
                        }
                    }
                    else
                    {
                        gen.Emit(OpCodes.Add);
                    }
                    goto Label_04AE;
                }
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    gen.Emit(OpCodes.And);
                    goto Label_04AE;

                case ExpressionType.Divide:
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Div);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Div_Un);
                    }
                    goto Label_04AE;

                case ExpressionType.ExclusiveOr:
                    gen.Emit(OpCodes.Xor);
                    goto Label_04AE;

                case ExpressionType.GreaterThan:
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Cgt);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Cgt_Un);
                    }
                    goto Label_04AE;

                case ExpressionType.GreaterThanOrEqual:
                {
                    Label label3 = gen.DefineLabel();
                    Label label4 = gen.DefineLabel();
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Bge_S, label3);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Bge_Un_S, label3);
                    }
                    gen.Emit(OpCodes.Ldc_I4_0);
                    gen.Emit(OpCodes.Br_S, label4);
                    gen.MarkLabel(label3);
                    gen.Emit(OpCodes.Ldc_I4_1);
                    gen.MarkLabel(label4);
                    goto Label_04AE;
                }
                case ExpressionType.LeftShift:
                {
                    Type nonNullableType = GetNonNullableType(rightType);
                    if (nonNullableType != typeof(int))
                    {
                        this.GenerateConvertToType(gen, nonNullableType, typeof(int), true);
                    }
                    gen.Emit(OpCodes.Shl);
                    goto Label_04AE;
                }
                case ExpressionType.LessThan:
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Clt);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Clt_Un);
                    }
                    goto Label_04AE;

                case ExpressionType.LessThanOrEqual:
                    label = gen.DefineLabel();
                    label2 = gen.DefineLabel();
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Ble_S, label);
                        break;
                    }
                    gen.Emit(OpCodes.Ble_Un_S, label);
                    break;

                case ExpressionType.Modulo:
                    if (!IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Rem);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Rem_Un);
                    }
                    goto Label_04AE;

                case ExpressionType.Multiply:
                    gen.Emit(OpCodes.Mul);
                    goto Label_04AE;

                case ExpressionType.MultiplyChecked:
                {
                    LocalBuilder builder5 = gen.DeclareLocal(leftType);
                    LocalBuilder builder6 = gen.DeclareLocal(rightType);
                    gen.Emit(OpCodes.Stloc, builder6);
                    gen.Emit(OpCodes.Stloc, builder5);
                    gen.Emit(OpCodes.Ldloc, builder5);
                    gen.Emit(OpCodes.Ldloc, builder6);
                    if (!IsFloatingPoint(leftType))
                    {
                        if (IsUnsigned(leftType))
                        {
                            gen.Emit(OpCodes.Mul_Ovf_Un);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Mul_Ovf);
                        }
                    }
                    else
                    {
                        gen.Emit(OpCodes.Mul);
                    }
                    goto Label_04AE;
                }
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    gen.Emit(OpCodes.Or);
                    goto Label_04AE;

                case ExpressionType.RightShift:
                {
                    Type typeFrom = GetNonNullableType(rightType);
                    if (typeFrom != typeof(int))
                    {
                        this.GenerateConvertToType(gen, typeFrom, typeof(int), true);
                    }
                    if (IsUnsigned(leftType))
                    {
                        gen.Emit(OpCodes.Shr_Un);
                    }
                    else
                    {
                        gen.Emit(OpCodes.Shr);
                    }
                    goto Label_04AE;
                }
                case ExpressionType.Subtract:
                    gen.Emit(OpCodes.Sub);
                    goto Label_04AE;

                case ExpressionType.SubtractChecked:
                {
                    LocalBuilder builder3 = gen.DeclareLocal(leftType);
                    LocalBuilder builder4 = gen.DeclareLocal(rightType);
                    gen.Emit(OpCodes.Stloc, builder4);
                    gen.Emit(OpCodes.Stloc, builder3);
                    gen.Emit(OpCodes.Ldloc, builder3);
                    gen.Emit(OpCodes.Ldloc, builder4);
                    if (!IsFloatingPoint(leftType))
                    {
                        if (IsUnsigned(leftType))
                        {
                            gen.Emit(OpCodes.Sub_Ovf_Un);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Sub_Ovf);
                        }
                    }
                    else
                    {
                        gen.Emit(OpCodes.Sub);
                    }
                    goto Label_04AE;
                }
                default:
                    throw Error.UnhandledBinary(op);
            }
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Br_S, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.MarkLabel(label2);
        Label_04AE:
            return StackType.Value;
        }

        private static void GenerateUnliftedEquality(ILGenerator gen, ExpressionType op, Type type)
        {
            if ((!type.IsPrimitive && type.IsValueType) && !type.IsEnum)
            {
                throw Error.OperatorNotImplementedForType(op, type);
            }
            gen.Emit(OpCodes.Ceq);
            if (op == ExpressionType.NotEqual)
            {
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ceq);
            }
        }

        private void GenerateUnliftedOrElse(ILGenerator gen, BinaryExpression b)
        {
            this.Generate(gen, b.Left, StackType.Value);
            Label label = gen.DefineLabel();
            gen.Emit(OpCodes.Dup);
            gen.Emit(OpCodes.Brtrue, label);
            gen.Emit(OpCodes.Pop);
            this.Generate(gen, b.Right, StackType.Value);
            gen.MarkLabel(label);
        }

        private StackType GenerateUserdefinedLiftedAndAlso(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            Type type = b.Left.Type;
            Type nonNullableType = GetNonNullableType(type);
            gen.DefineLabel();
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(type);
            LocalBuilder builder2 = gen.DeclareLocal(type);
            LocalBuilder builder3 = gen.DeclareLocal(nonNullableType);
            LocalBuilder builder4 = gen.DeclareLocal(nonNullableType);
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Stloc, local);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, type);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, type);
            Type[] types = new Type[] { nonNullableType };
            MethodInfo meth = nonNullableType.GetMethod("op_False", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Call, meth);
            gen.Emit(OpCodes.Brtrue, label2);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, type);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, type);
            gen.Emit(OpCodes.Stloc, builder3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, type);
            gen.Emit(OpCodes.Stloc, builder4);
            types = new Type[] { nonNullableType, nonNullableType };
            MethodInfo info2 = nonNullableType.GetMethod("op_BitwiseAnd", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Ldloc, builder3);
            gen.Emit(OpCodes.Ldloc, builder4);
            gen.Emit(OpCodes.Call, info2);
            if (info2.ReturnType != type)
            {
                this.GenerateConvertToType(gen, info2.ReturnType, type, true);
            }
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            gen.MarkLabel(label2);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private StackType GenerateUserdefinedLiftedOrElse(ILGenerator gen, BinaryExpression b, StackType ask)
        {
            Type type = b.Left.Type;
            Type nonNullableType = GetNonNullableType(type);
            gen.DefineLabel();
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            LocalBuilder local = gen.DeclareLocal(type);
            LocalBuilder builder2 = gen.DeclareLocal(type);
            LocalBuilder builder3 = gen.DeclareLocal(nonNullableType);
            LocalBuilder builder4 = gen.DeclareLocal(nonNullableType);
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Stloc, local);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Stloc, builder2);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateHasValue(gen, type);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, type);
            Type[] types = new Type[] { nonNullableType };
            MethodInfo meth = nonNullableType.GetMethod("op_True", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Call, meth);
            gen.Emit(OpCodes.Brtrue, label2);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateHasValue(gen, type);
            gen.Emit(OpCodes.Brfalse, label);
            gen.Emit(OpCodes.Ldloca, local);
            this.GenerateGetValueOrDefault(gen, type);
            gen.Emit(OpCodes.Stloc, builder3);
            gen.Emit(OpCodes.Ldloca, builder2);
            this.GenerateGetValueOrDefault(gen, type);
            gen.Emit(OpCodes.Stloc, builder4);
            types = new Type[] { nonNullableType, nonNullableType };
            MethodInfo info2 = nonNullableType.GetMethod("op_BitwiseOr", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, types, null);
            gen.Emit(OpCodes.Ldloc, builder3);
            gen.Emit(OpCodes.Ldloc, builder4);
            gen.Emit(OpCodes.Call, info2);
            if (info2.ReturnType != type)
            {
                this.GenerateConvertToType(gen, info2.ReturnType, type, true);
            }
            gen.Emit(OpCodes.Stloc, local);
            gen.Emit(OpCodes.Br, label2);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloc, builder2);
            gen.Emit(OpCodes.Stloc, local);
            gen.MarkLabel(label2);
            return this.ReturnFromLocal(gen, ask, local);
        }

        private void GenerateUserDefinedReferenceCoalesce(ILGenerator gen, BinaryExpression b)
        {
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            this.Generate(gen, b.Left, StackType.Value);
            gen.Emit(OpCodes.Dup);
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Brfalse, label2);
            gen.Emit(OpCodes.Pop);
            this.Generate(gen, b.Right, StackType.Value);
            gen.Emit(OpCodes.Br_S, label);
            gen.MarkLabel(label2);
            gen.Emit(OpCodes.Call, b.Method);
            gen.MarkLabel(label);
        }

        private void GenerateWriteBack(ILGenerator gen, WriteBack writeback)
        {
            MemberExpression arg = writeback.arg as MemberExpression;
            if (arg != null)
            {
                this.GenerateMemberWriteBack(gen, arg.Expression, arg.Member, writeback.loc);
            }
        }

        private Type GetMemberType(MemberInfo member)
        {
            FieldInfo info = member as FieldInfo;
            if (info != null)
            {
                return info.FieldType;
            }
            PropertyInfo info2 = member as PropertyInfo;
            return info2?.PropertyType;
        }

        private static Type GetNonNullableType(Type type)
        {
            if (IsNullable(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        private Type[] GetParameterTypes(MethodInfo mi)
        {
            ParameterInfo[] parameters = mi.GetParameters();
            Type[] typeArray = new Type[parameters.Length + 1];
            int index = 0;
            int length = parameters.Length;
            while (index < length)
            {
                typeArray[index + 1] = parameters[index].ParameterType;
                index++;
            }
            typeArray[0] = typeof(ExecutionScope);
            return typeArray;
        }

        private static bool IsConvertible(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
            {
                return true;
            }
            switch (Type.GetTypeCode(type))
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

        private static bool IsFloatingPoint(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
            }
            return false;
        }

        private bool IsHoisted(ParameterExpression p) => 
            this.scope.HoistedLocals.ContainsKey(p);

        private static bool IsInteger(Type type)
        {
            type = GetNonNullableType(type);
            if (!type.IsEnum)
            {
                switch (Type.GetTypeCode(type))
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

        private static bool IsLiftedLogicalBinaryOperator(Type left, Type right, MethodInfo method) => 
            ((((right == left) && IsNullable(left)) && (method != null)) && (method.ReturnType == GetNonNullableType(left)));

        private static bool IsNullable(Type type) => 
            (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        private static bool IsNullConstant(Expression e) => 
            ((e.NodeType == ExpressionType.Constant) && (((ConstantExpression) e).Value == null));

        private static bool IsUnsigned(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }

        private static Type MakeStrongBoxType(Type type) => 
            typeof(StrongBox<>).MakeGenericType(new Type[] { type });

        private void PrepareInitLocal(ILGenerator gen, ParameterExpression p)
        {
            int num;
            if (this.scope.HoistedLocals.TryGetValue(p, out num))
            {
                this.GenerateLoadHoistedLocals(gen);
                this.GenerateConstInt(gen, num);
            }
            else
            {
                LocalBuilder builder = gen.DeclareLocal(p.Type);
                this.scope.Locals.Add(p, builder);
            }
        }

        private StackType ReturnFromLocal(ILGenerator gen, StackType ask, LocalBuilder local)
        {
            if (ask == StackType.Address)
            {
                gen.Emit(OpCodes.Ldloca, local);
                return ask;
            }
            gen.Emit(OpCodes.Ldloc, local);
            return ask;
        }

        private bool UseVirtual(MethodInfo mi)
        {
            if (mi.IsStatic)
            {
                return false;
            }
            if (mi.DeclaringType.IsValueType)
            {
                return false;
            }
            return true;
        }

        private class CompileScope
        {
            internal Dictionary<ParameterExpression, int> HoistedLocals;
            internal LocalBuilder HoistedLocalsVar;
            internal LambdaExpression Lambda;
            internal Dictionary<ParameterExpression, LocalBuilder> Locals;
            internal ExpressionCompiler.CompileScope Parent;

            internal CompileScope(ExpressionCompiler.CompileScope parent, LambdaExpression lambda)
            {
                this.Parent = parent;
                this.Lambda = lambda;
                this.Locals = new Dictionary<ParameterExpression, LocalBuilder>();
                this.HoistedLocals = new Dictionary<ParameterExpression, int>();
            }
        }

        private class Hoister : ExpressionVisitor
        {
            private LambdaExpression current;
            private ExpressionCompiler.CompileScope expressionScope;
            private List<ParameterExpression> locals;

            internal Hoister()
            {
            }

            internal void Hoist(ExpressionCompiler.CompileScope scope)
            {
                this.expressionScope = scope;
                this.current = scope.Lambda;
                this.locals = new List<ParameterExpression>(scope.Lambda.Parameters);
                this.Visit(scope.Lambda.Body);
            }

            internal override Expression VisitInvocation(InvocationExpression iv)
            {
                if (this.expressionScope.Lambda == this.current)
                {
                    if (iv.Expression.NodeType == ExpressionType.Lambda)
                    {
                        LambdaExpression expression = (LambdaExpression) iv.Expression;
                        this.locals.AddRange(expression.Parameters);
                    }
                    else if ((iv.Expression.NodeType == ExpressionType.Quote) && iv.Expression.Type.IsSubclassOf(typeof(LambdaExpression)))
                    {
                        LambdaExpression operand = (LambdaExpression) ((UnaryExpression) iv.Expression).Operand;
                        this.locals.AddRange(operand.Parameters);
                    }
                }
                return base.VisitInvocation(iv);
            }

            internal override Expression VisitLambda(LambdaExpression l)
            {
                LambdaExpression current = this.current;
                this.current = l;
                this.Visit(l.Body);
                this.current = current;
                return l;
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                if ((this.locals.Contains(p) && (this.expressionScope.Lambda != this.current)) && !this.expressionScope.HoistedLocals.ContainsKey(p))
                {
                    this.expressionScope.HoistedLocals.Add(p, this.expressionScope.HoistedLocals.Count);
                }
                return p;
            }
        }

        internal class LambdaInfo
        {
            internal Dictionary<ParameterExpression, int> HoistedLocals;
            internal LambdaExpression Lambda;
            internal List<ExpressionCompiler.LambdaInfo> Lambdas;
            internal MethodInfo Method;

            internal LambdaInfo(LambdaExpression lambda, MethodInfo method, Dictionary<ParameterExpression, int> hoistedLocals, List<ExpressionCompiler.LambdaInfo> lambdas)
            {
                this.Lambda = lambda;
                this.Method = method;
                this.HoistedLocals = hoistedLocals;
                this.Lambdas = lambdas;
            }
        }

        private enum StackType
        {
            Value,
            Address
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WriteBack
        {
            public LocalBuilder loc;
            public Expression arg;
            public WriteBack(LocalBuilder loc, Expression arg)
            {
                this.loc = loc;
                this.arg = arg;
            }
        }
    }
}

