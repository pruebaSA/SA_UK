namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Data.Linq.SqlClient.Implementation;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class ObjectReaderCompiler : IObjectReaderCompiler
    {
        private FieldInfo argsField;
        private FieldInfo bufferReaderField;
        private static LocalDataStoreSlot cacheSlot = Thread.AllocateDataSlot();
        private Type dataReaderType;
        private FieldInfo globalsField;
        private static int maxReaderCacheSize = 10;
        private MethodInfo miBRisDBNull;
        private MethodInfo miDRisDBNull;
        private FieldInfo ordinalsField;
        private FieldInfo readerField;
        private IDataServices services;

        internal ObjectReaderCompiler(Type dataReaderType, IDataServices services)
        {
            this.dataReaderType = dataReaderType;
            this.services = services;
            this.miDRisDBNull = dataReaderType.GetMethod("IsDBNull", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            this.miBRisDBNull = typeof(DbDataReader).GetMethod("IsDBNull", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            Type type = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.dataReaderType });
            this.ordinalsField = type.GetField("Ordinals", BindingFlags.Public | BindingFlags.Instance);
            this.globalsField = type.GetField("Globals", BindingFlags.Public | BindingFlags.Instance);
            this.argsField = type.GetField("Arguments", BindingFlags.Public | BindingFlags.Instance);
            this.readerField = type.GetField("DataReader", BindingFlags.Public | BindingFlags.Instance);
            this.bufferReaderField = type.GetField("BufferReader", BindingFlags.Public | BindingFlags.Instance);
        }

        public IObjectReaderFactory Compile(SqlExpression expression, Type elementType)
        {
            object identity = this.services.Context.Mapping.Identity;
            DataLoadOptions loadOptions = this.services.Context.LoadOptions;
            IObjectReaderFactory factory = null;
            ReaderFactoryCache data = null;
            bool flag = SqlProjectionComparer.CanBeCompared(expression);
            if (flag)
            {
                data = (ReaderFactoryCache) Thread.GetData(cacheSlot);
                if (data == null)
                {
                    data = new ReaderFactoryCache(maxReaderCacheSize);
                    Thread.SetData(cacheSlot, data);
                }
                factory = data.GetFactory(elementType, this.dataReaderType, identity, loadOptions, expression);
            }
            if (factory == null)
            {
                Generator gen = new Generator(this, elementType);
                DynamicMethod method = this.CompileDynamicMethod(gen, expression, elementType);
                Type delegateType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.dataReaderType }), elementType });
                Delegate delegate2 = method.CreateDelegate(delegateType);
                factory = (IObjectReaderFactory) Activator.CreateInstance(typeof(ObjectReaderFactory).MakeGenericType(new Type[] { this.dataReaderType, elementType }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { delegate2, gen.NamedColumns, gen.Globals, gen.Locals }, null);
                if (flag)
                {
                    expression = new SourceExpressionRemover().VisitExpression(expression);
                    data.AddFactory(elementType, this.dataReaderType, identity, loadOptions, expression, factory);
                }
            }
            return factory;
        }

        private DynamicMethod CompileDynamicMethod(Generator gen, SqlExpression expression, Type elementType)
        {
            Type type = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.dataReaderType });
            DynamicMethod method = new DynamicMethod("Read_" + elementType.Name, elementType, new Type[] { type }, true);
            gen.GenerateBody(method.GetILGenerator(), expression);
            return method;
        }

        public IObjectReaderSession CreateSession(DbDataReader reader, IReaderProvider provider, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries) => 
            ((IObjectReaderSession) Activator.CreateInstance(typeof(ObjectReaderSession).MakeGenericType(new Type[] { this.dataReaderType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { reader, provider, parentArgs, userArgs, subQueries }, null));

        private class Generator
        {
            private Dictionary<MetaAssociation, int> associationSubQueries;
            private ObjectReaderCompiler compiler;
            private Type elementType;
            private ILGenerator gen;
            private List<object> globals;
            private LocalBuilder locDataReader;
            private List<ObjectReaderCompiler.NamedColumn> namedColumns;
            private int nLocals;
            private static Type[] readMethodSignature = new Type[] { typeof(int) };
            private ObjectReaderCompiler.SideEffectChecker sideEffectChecker = new ObjectReaderCompiler.SideEffectChecker();

            internal Generator(ObjectReaderCompiler compiler, Type elementType)
            {
                this.compiler = compiler;
                this.elementType = elementType;
                this.associationSubQueries = new Dictionary<MetaAssociation, int>();
            }

            private int AddGlobal(Type type, object value)
            {
                int count = this.globals.Count;
                if (type.IsValueType)
                {
                    this.globals.Add(Activator.CreateInstance(typeof(StrongBox<>).MakeGenericType(new Type[] { type }), new object[] { value }));
                    return count;
                }
                this.globals.Add(value);
                return count;
            }

            private int AllocateLocal() => 
                this.nLocals++;

            private Type Generate(SqlNode node) => 
                this.Generate(node, null);

            private Type Generate(SqlNode node, LocalBuilder locInstance)
            {
                switch (node.NodeType)
                {
                    case SqlNodeType.ClientArray:
                        return this.GenerateClientArray((SqlClientArray) node);

                    case SqlNodeType.ClientCase:
                        return this.GenerateClientCase((SqlClientCase) node, false, locInstance);

                    case SqlNodeType.ClientParameter:
                        return this.GenerateClientParameter((SqlClientParameter) node);

                    case SqlNodeType.ClientQuery:
                        return this.GenerateClientQuery((SqlClientQuery) node, locInstance);

                    case SqlNodeType.ColumnRef:
                        return this.GenerateColumnReference((SqlColumnRef) node);

                    case SqlNodeType.DiscriminatedType:
                        return this.GenerateDiscriminatedType((SqlDiscriminatedType) node);

                    case SqlNodeType.Lift:
                        return this.GenerateLift((SqlLift) node);

                    case SqlNodeType.Link:
                        return this.GenerateLink((SqlLink) node, locInstance);

                    case SqlNodeType.Grouping:
                        return this.GenerateGrouping((SqlGrouping) node);

                    case SqlNodeType.JoinedCollection:
                        return this.GenerateJoinedCollection((SqlJoinedCollection) node);

                    case SqlNodeType.MethodCall:
                        return this.GenerateMethodCall((SqlMethodCall) node);

                    case SqlNodeType.Member:
                        return this.GenerateMember((SqlMember) node);

                    case SqlNodeType.OptionalValue:
                        return this.GenerateOptionalValue((SqlOptionalValue) node);

                    case SqlNodeType.OuterJoinedValue:
                        return this.Generate(((SqlUnary) node).Operand);

                    case SqlNodeType.SearchedCase:
                        return this.GenerateSearchedCase((SqlSearchedCase) node);

                    case SqlNodeType.New:
                        return this.GenerateNew((SqlNew) node);

                    case SqlNodeType.Value:
                        return this.GenerateValue((SqlValue) node);

                    case SqlNodeType.ValueOf:
                        return this.GenerateValueOf((SqlUnary) node);

                    case SqlNodeType.UserColumn:
                        return this.GenerateUserColumn((SqlUserColumn) node);

                    case SqlNodeType.TypeCase:
                        return this.GenerateTypeCase((SqlTypeCase) node);
                }
                throw System.Data.Linq.SqlClient.Error.CouldNotTranslateExpressionForReading(node.SourceExpression);
            }

            private void GenerateAccessArguments()
            {
                this.gen.Emit(OpCodes.Ldarg_0);
                this.gen.Emit(OpCodes.Ldfld, this.compiler.argsField);
            }

            private void GenerateAccessBufferReader()
            {
                this.gen.Emit(OpCodes.Ldarg_0);
                this.gen.Emit(OpCodes.Ldfld, this.compiler.bufferReaderField);
            }

            private void GenerateAccessDataReader()
            {
                this.gen.Emit(OpCodes.Ldloc, this.locDataReader);
            }

            private void GenerateAccessGlobals()
            {
                this.gen.Emit(OpCodes.Ldarg_0);
                this.gen.Emit(OpCodes.Ldfld, this.compiler.globalsField);
            }

            private void GenerateAccessOrdinals()
            {
                this.gen.Emit(OpCodes.Ldarg_0);
                this.gen.Emit(OpCodes.Ldfld, this.compiler.ordinalsField);
            }

            private Type GenerateArrayAccess(Type type, bool address)
            {
                if (!type.IsEnum && (Type.GetTypeCode(type) == TypeCode.Int32))
                {
                    this.gen.Emit(OpCodes.Ldelem_I4);
                }
                return type;
            }

            private void GenerateArrayAssign(Type type)
            {
                if (!type.IsEnum)
                {
                    TypeCode typeCode = Type.GetTypeCode(type);
                    if (type.IsValueType)
                    {
                        this.gen.Emit(OpCodes.Stelem, type);
                    }
                    else
                    {
                        this.gen.Emit(OpCodes.Stelem_Ref);
                    }
                }
            }

            private void GenerateAssignDeferredEntitySet(MetaDataMember mm, LocalBuilder locInstance, SqlExpression expr, LocalBuilder locStoreInMember)
            {
                MemberInfo mi = (mm.StorageMember != null) ? mm.StorageMember : mm.Member;
                Type memberType = TypeSystem.GetMemberType(mi);
                Label label = this.gen.DefineLabel();
                Type type2 = typeof(IEnumerable<>).MakeGenericType(memberType.GetGenericArguments());
                bool flag = this.HasSideEffect(expr);
                if ((locStoreInMember != null) && !flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                Type localType = this.GenerateDeferredSource(expr, locInstance);
                LocalBuilder local = this.gen.DeclareLocal(localType);
                this.gen.Emit(OpCodes.Stloc, local);
                if ((locStoreInMember != null) && flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                if ((mi is FieldInfo) || ((mi is PropertyInfo) && ((PropertyInfo) mi).CanWrite))
                {
                    Label label2 = this.gen.DefineLabel();
                    this.GenerateLoadForMemberAccess(locInstance);
                    this.GenerateLoadMember(mi);
                    this.gen.Emit(OpCodes.Ldnull);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brfalse, label2);
                    this.GenerateLoadForMemberAccess(locInstance);
                    ConstructorInfo constructor = memberType.GetConstructor(Type.EmptyTypes);
                    this.gen.Emit(OpCodes.Newobj, constructor);
                    this.GenerateStoreMember(mi);
                    this.gen.MarkLabel(label2);
                }
                this.GenerateLoadForMemberAccess(locInstance);
                this.GenerateLoadMember(mi);
                this.gen.Emit(OpCodes.Ldloc, local);
                MethodInfo meth = memberType.GetMethod("SetSource", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type2 }, null);
                this.gen.Emit(GetMethodCallOpCode(meth), meth);
                this.gen.MarkLabel(label);
            }

            private void GenerateAssignDeferredReference(MetaDataMember mm, LocalBuilder locInstance, SqlExpression expr, LocalBuilder locStoreInMember)
            {
                MemberInfo mi = (mm.StorageMember != null) ? mm.StorageMember : mm.Member;
                Type memberType = TypeSystem.GetMemberType(mi);
                Label label = this.gen.DefineLabel();
                Type type2 = typeof(IEnumerable<>).MakeGenericType(memberType.GetGenericArguments());
                bool flag = this.HasSideEffect(expr);
                if ((locStoreInMember != null) && !flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                Type localType = this.GenerateDeferredSource(expr, locInstance);
                LocalBuilder local = this.gen.DeclareLocal(localType);
                this.gen.Emit(OpCodes.Stloc, local);
                if ((locStoreInMember != null) && flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                this.GenerateLoadForMemberAccess(locInstance);
                this.gen.Emit(OpCodes.Ldloc, local);
                ConstructorInfo con = memberType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type2 }, null);
                this.gen.Emit(OpCodes.Newobj, con);
                this.GenerateStoreMember(mi);
                this.gen.MarkLabel(label);
            }

            private void GenerateAssignEntitySet(MetaDataMember mm, LocalBuilder locInstance, SqlExpression expr, LocalBuilder locStoreInMember)
            {
                MemberInfo mi = (mm.StorageMember != null) ? mm.StorageMember : mm.Member;
                Type memberType = TypeSystem.GetMemberType(mi);
                Label label = this.gen.DefineLabel();
                Type type2 = typeof(IEnumerable<>).MakeGenericType(memberType.GetGenericArguments());
                bool flag = this.HasSideEffect(expr);
                if ((locStoreInMember != null) && !flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                Type localType = this.Generate(expr, mm.DeclaringType.IsEntity ? locInstance : null);
                LocalBuilder local = this.gen.DeclareLocal(localType);
                this.gen.Emit(OpCodes.Stloc, local);
                if ((locStoreInMember != null) && flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                if ((mi is FieldInfo) || ((mi is PropertyInfo) && ((PropertyInfo) mi).CanWrite))
                {
                    Label label2 = this.gen.DefineLabel();
                    this.GenerateLoadForMemberAccess(locInstance);
                    this.GenerateLoadMember(mi);
                    this.gen.Emit(OpCodes.Ldnull);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brfalse, label2);
                    this.GenerateLoadForMemberAccess(locInstance);
                    ConstructorInfo constructor = memberType.GetConstructor(Type.EmptyTypes);
                    this.gen.Emit(OpCodes.Newobj, constructor);
                    this.GenerateStoreMember(mi);
                    this.gen.MarkLabel(label2);
                }
                this.GenerateLoadForMemberAccess(locInstance);
                this.GenerateLoadMember(mi);
                this.gen.Emit(OpCodes.Ldloc, local);
                MethodInfo meth = memberType.GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type2 }, null);
                this.gen.Emit(GetMethodCallOpCode(meth), meth);
                this.gen.MarkLabel(label);
            }

            private void GenerateAssignValue(MetaDataMember mm, LocalBuilder locInstance, SqlExpression expr, LocalBuilder locStoreInMember)
            {
                MemberInfo member = (mm.StorageMember != null) ? mm.StorageMember : mm.Member;
                if (!IsAssignable(member))
                {
                    throw System.Data.Linq.SqlClient.Error.CannotAssignToMember(member.Name);
                }
                Type memberType = TypeSystem.GetMemberType(member);
                Label label = this.gen.DefineLabel();
                bool flag = this.HasSideEffect(expr);
                if ((locStoreInMember != null) && !flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                this.GenerateExpressionForType(expr, memberType, mm.DeclaringType.IsEntity ? locInstance : null);
                LocalBuilder local = this.gen.DeclareLocal(memberType);
                this.gen.Emit(OpCodes.Stloc, local);
                if ((locStoreInMember != null) && flag)
                {
                    this.gen.Emit(OpCodes.Ldloc, locStoreInMember);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                }
                this.GenerateLoadForMemberAccess(locInstance);
                this.gen.Emit(OpCodes.Ldloc, local);
                this.GenerateStoreMember(member);
                this.gen.MarkLabel(label);
            }

            internal void GenerateBody(ILGenerator generator, SqlExpression expression)
            {
                this.gen = generator;
                this.globals = new List<object>();
                this.namedColumns = new List<ObjectReaderCompiler.NamedColumn>();
                this.locDataReader = generator.DeclareLocal(this.compiler.dataReaderType);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, this.compiler.readerField);
                generator.Emit(OpCodes.Stloc, this.locDataReader);
                this.GenerateExpressionForType(expression, this.elementType);
                generator.Emit(OpCodes.Ret);
            }

            private Type GenerateClientArray(SqlClientArray ca)
            {
                Type elementType = TypeSystem.GetElementType(ca.ClrType);
                this.GenerateConstInt(ca.Expressions.Count);
                this.gen.Emit(OpCodes.Newarr, elementType);
                int num = 0;
                int count = ca.Expressions.Count;
                while (num < count)
                {
                    this.gen.Emit(OpCodes.Dup);
                    this.GenerateConstInt(num);
                    this.GenerateExpressionForType(ca.Expressions[num], elementType);
                    this.GenerateArrayAssign(elementType);
                    num++;
                }
                return ca.ClrType;
            }

            private Type GenerateClientCase(SqlClientCase scc, bool isDeferred, LocalBuilder locInstance)
            {
                LocalBuilder local = this.gen.DeclareLocal(scc.Expression.ClrType);
                this.GenerateExpressionForType(scc.Expression, scc.Expression.ClrType);
                this.gen.Emit(OpCodes.Stloc, local);
                Label loc = this.gen.DefineLabel();
                Label label = this.gen.DefineLabel();
                int num = 0;
                int count = scc.Whens.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.gen.MarkLabel(loc);
                        loc = this.gen.DefineLabel();
                    }
                    SqlClientWhen when = scc.Whens[num];
                    if (when.Match != null)
                    {
                        this.gen.Emit(OpCodes.Ldloc, local);
                        this.GenerateExpressionForType(when.Match, scc.Expression.ClrType);
                        this.GenerateEquals(local.LocalType);
                        this.gen.Emit(OpCodes.Brfalse, loc);
                    }
                    if (isDeferred)
                    {
                        this.GenerateDeferredSource(when.Value, locInstance);
                    }
                    else
                    {
                        this.GenerateExpressionForType(when.Value, scc.ClrType);
                    }
                    this.gen.Emit(OpCodes.Br, label);
                    num++;
                }
                this.gen.MarkLabel(label);
                return scc.ClrType;
            }

            private Type GenerateClientParameter(SqlClientParameter cp)
            {
                Delegate delegate2 = cp.Accessor.Compile();
                int iGlobal = this.AddGlobal(delegate2.GetType(), delegate2);
                this.GenerateGlobalAccess(iGlobal, delegate2.GetType());
                this.GenerateAccessArguments();
                MethodInfo meth = delegate2.GetType().GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(object[]) }, null);
                this.gen.Emit(GetMethodCallOpCode(meth), meth);
                return delegate2.Method.ReturnType;
            }

            private Type GenerateClientQuery(SqlClientQuery cq, LocalBuilder locInstance)
            {
                Type type = (cq.Query.NodeType == SqlNodeType.Multiset) ? TypeSystem.GetElementType(cq.ClrType) : cq.ClrType;
                this.gen.Emit(OpCodes.Ldarg_0);
                this.GenerateConstInt(cq.Ordinal);
                this.GenerateConstInt(cq.Arguments.Count);
                this.gen.Emit(OpCodes.Newarr, typeof(object));
                int num = 0;
                int count = cq.Arguments.Count;
                while (num < count)
                {
                    this.gen.Emit(OpCodes.Dup);
                    this.GenerateConstInt(num);
                    Type clrType = cq.Arguments[num].ClrType;
                    if (cq.Arguments[num].NodeType == SqlNodeType.ColumnRef)
                    {
                        SqlColumnRef ref2 = (SqlColumnRef) cq.Arguments[num];
                        if (clrType.IsValueType && !TypeSystem.IsNullableType(clrType))
                        {
                            clrType = typeof(Nullable<>).MakeGenericType(new Type[] { clrType });
                        }
                        this.GenerateColumnAccess(clrType, ref2.SqlType, ref2.Column.Ordinal, null);
                    }
                    else
                    {
                        this.GenerateExpressionForType(cq.Arguments[num], cq.Arguments[num].ClrType);
                    }
                    if (clrType.IsValueType)
                    {
                        this.gen.Emit(OpCodes.Box, clrType);
                    }
                    this.GenerateArrayAssign(typeof(object));
                    num++;
                }
                MethodInfo method = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("ExecuteSubQuery", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                this.gen.Emit(GetMethodCallOpCode(method), method);
                Type cls = typeof(IEnumerable<>).MakeGenericType(new Type[] { type });
                this.gen.Emit(OpCodes.Castclass, cls);
                Type expectedType = typeof(List<>).MakeGenericType(new Type[] { type });
                this.GenerateConvertToType(cls, expectedType);
                return expectedType;
            }

            private void GenerateColumnAccess(Type cType, ProviderType pType, int ordinal, LocalBuilder locOrdinal)
            {
                Type closestRuntimeType = pType.GetClosestRuntimeType();
                MethodInfo readerMethod = this.GetReaderMethod(this.compiler.dataReaderType, closestRuntimeType);
                MethodInfo meth = this.GetReaderMethod(typeof(DbDataReader), closestRuntimeType);
                Label label = this.gen.DefineLabel();
                Label label2 = this.gen.DefineLabel();
                Label label3 = this.gen.DefineLabel();
                this.GenerateAccessBufferReader();
                this.gen.Emit(OpCodes.Ldnull);
                this.gen.Emit(OpCodes.Ceq);
                this.gen.Emit(OpCodes.Brfalse, label3);
                this.GenerateAccessDataReader();
                if (locOrdinal != null)
                {
                    this.gen.Emit(OpCodes.Ldloc, locOrdinal);
                }
                else
                {
                    this.GenerateConstInt(ordinal);
                }
                this.gen.Emit(GetMethodCallOpCode(this.compiler.miDRisDBNull), this.compiler.miDRisDBNull);
                this.gen.Emit(OpCodes.Brtrue, label);
                this.GenerateAccessDataReader();
                if (locOrdinal != null)
                {
                    this.gen.Emit(OpCodes.Ldloc, locOrdinal);
                }
                else
                {
                    this.GenerateConstInt(ordinal);
                }
                this.gen.Emit(GetMethodCallOpCode(readerMethod), readerMethod);
                this.GenerateConvertToType(closestRuntimeType, cType, readerMethod.ReturnType);
                this.gen.Emit(OpCodes.Br_S, label2);
                this.gen.MarkLabel(label3);
                this.GenerateAccessBufferReader();
                if (locOrdinal != null)
                {
                    this.gen.Emit(OpCodes.Ldloc, locOrdinal);
                }
                else
                {
                    this.GenerateConstInt(ordinal);
                }
                this.gen.Emit(GetMethodCallOpCode(this.compiler.miBRisDBNull), this.compiler.miBRisDBNull);
                this.gen.Emit(OpCodes.Brtrue, label);
                this.GenerateAccessBufferReader();
                if (locOrdinal != null)
                {
                    this.gen.Emit(OpCodes.Ldloc, locOrdinal);
                }
                else
                {
                    this.GenerateConstInt(ordinal);
                }
                this.gen.Emit(GetMethodCallOpCode(meth), meth);
                this.GenerateConvertToType(closestRuntimeType, cType, meth.ReturnType);
                this.gen.Emit(OpCodes.Br_S, label2);
                this.gen.MarkLabel(label);
                this.GenerateDefault(cType);
                this.gen.MarkLabel(label2);
            }

            private Type GenerateColumnReference(SqlColumnRef cref)
            {
                this.GenerateColumnAccess(cref.ClrType, cref.SqlType, cref.Column.Ordinal, null);
                return cref.ClrType;
            }

            private Type GenerateConstant(Type type, object value)
            {
                if (value == null)
                {
                    if (type.IsValueType)
                    {
                        LocalBuilder local = this.gen.DeclareLocal(type);
                        this.gen.Emit(OpCodes.Ldloca, local);
                        this.gen.Emit(OpCodes.Initobj, type);
                        this.gen.Emit(OpCodes.Ldloc, local);
                        return type;
                    }
                    this.gen.Emit(OpCodes.Ldnull);
                    return type;
                }
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        this.GenerateConstInt(((bool) value) ? 1 : 0);
                        return type;

                    case TypeCode.SByte:
                        this.GenerateConstInt((sbyte) value);
                        this.gen.Emit(OpCodes.Conv_I1);
                        return type;

                    case TypeCode.Int16:
                        this.GenerateConstInt((short) value);
                        this.gen.Emit(OpCodes.Conv_I2);
                        return type;

                    case TypeCode.Int32:
                        this.GenerateConstInt((int) value);
                        return type;

                    case TypeCode.Int64:
                        this.gen.Emit(OpCodes.Ldc_I8, (long) value);
                        return type;

                    case TypeCode.Single:
                        this.gen.Emit(OpCodes.Ldc_R4, (float) value);
                        return type;

                    case TypeCode.Double:
                        this.gen.Emit(OpCodes.Ldc_R8, (double) value);
                        return type;
                }
                int iGlobal = this.AddGlobal(type, value);
                return this.GenerateGlobalAccess(iGlobal, type);
            }

            private void GenerateConstInt(int value)
            {
                switch (value)
                {
                    case 0:
                        this.gen.Emit(OpCodes.Ldc_I4_0);
                        return;

                    case 1:
                        this.gen.Emit(OpCodes.Ldc_I4_1);
                        return;

                    case 2:
                        this.gen.Emit(OpCodes.Ldc_I4_2);
                        return;

                    case 3:
                        this.gen.Emit(OpCodes.Ldc_I4_3);
                        return;

                    case 4:
                        this.gen.Emit(OpCodes.Ldc_I4_4);
                        return;

                    case 5:
                        this.gen.Emit(OpCodes.Ldc_I4_5);
                        return;

                    case 6:
                        this.gen.Emit(OpCodes.Ldc_I4_6);
                        return;

                    case 7:
                        this.gen.Emit(OpCodes.Ldc_I4_7);
                        return;

                    case 8:
                        this.gen.Emit(OpCodes.Ldc_I4_8);
                        return;

                    case -1:
                        this.gen.Emit(OpCodes.Ldc_I4_M1);
                        return;
                }
                if ((value >= -127) && (value < 0x80))
                {
                    this.gen.Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                }
                else
                {
                    this.gen.Emit(OpCodes.Ldc_I4, value);
                }
            }

            private void GenerateConvertToType(Type actualType, Type expectedType)
            {
                if ((expectedType != actualType) && (actualType.IsValueType || !actualType.IsSubclassOf(expectedType)))
                {
                    if (actualType.IsGenericType)
                    {
                        actualType.GetGenericTypeDefinition();
                    }
                    Type type = expectedType.IsGenericType ? expectedType.GetGenericTypeDefinition() : null;
                    Type[] typeArguments = (type != null) ? expectedType.GetGenericArguments() : null;
                    Type elementType = TypeSystem.GetElementType(actualType);
                    Type sequenceType = TypeSystem.GetSequenceType(elementType);
                    bool flag = sequenceType.IsAssignableFrom(actualType);
                    if ((expectedType == typeof(object)) && actualType.IsValueType)
                    {
                        this.gen.Emit(OpCodes.Box, actualType);
                    }
                    else if ((actualType == typeof(object)) && expectedType.IsValueType)
                    {
                        this.gen.Emit(OpCodes.Unbox_Any, expectedType);
                    }
                    else if ((actualType.IsSubclassOf(expectedType) || expectedType.IsSubclassOf(actualType)) && (!actualType.IsValueType && !expectedType.IsValueType))
                    {
                        this.gen.Emit(OpCodes.Castclass, expectedType);
                    }
                    else if ((type == typeof(IEnumerable<>)) && flag)
                    {
                        if ((this.elementType.IsInterface || typeArguments[0].IsInterface) || ((this.elementType.IsSubclassOf(typeArguments[0]) || typeArguments[0].IsSubclassOf(this.elementType)) || (TypeSystem.GetNonNullableType(this.elementType) == TypeSystem.GetNonNullableType(typeArguments[0]))))
                        {
                            MethodInfo meth = TypeSystem.FindSequenceMethod("Cast", new Type[] { sequenceType }, new Type[] { typeArguments[0] });
                            this.gen.Emit(OpCodes.Call, meth);
                        }
                        else
                        {
                            MethodInfo info2 = TypeSystem.FindStaticMethod(typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }), "Convert", new Type[] { sequenceType }, new Type[] { typeArguments[0] });
                            this.gen.Emit(OpCodes.Call, info2);
                        }
                    }
                    else if ((expectedType == elementType) && flag)
                    {
                        MethodInfo info3 = TypeSystem.FindSequenceMethod("SingleOrDefault", new Type[] { sequenceType }, new Type[] { expectedType });
                        this.gen.Emit(OpCodes.Call, info3);
                    }
                    else if (TypeSystem.IsNullableType(expectedType) && (TypeSystem.GetNonNullableType(expectedType) == actualType))
                    {
                        ConstructorInfo constructor = expectedType.GetConstructor(new Type[] { actualType });
                        this.gen.Emit(OpCodes.Newobj, constructor);
                    }
                    else if (TypeSystem.IsNullableType(actualType) && (TypeSystem.GetNonNullableType(actualType) == expectedType))
                    {
                        LocalBuilder local = this.gen.DeclareLocal(actualType);
                        this.gen.Emit(OpCodes.Stloc, local);
                        this.gen.Emit(OpCodes.Ldloca, local);
                        this.GenerateGetValueOrDefault(actualType);
                    }
                    else if ((type == typeof(EntityRef<>)) || (type == typeof(Link<>)))
                    {
                        if (actualType.IsAssignableFrom(typeArguments[0]))
                        {
                            if (actualType != typeArguments[0])
                            {
                                this.GenerateConvertToType(actualType, typeArguments[0]);
                            }
                            ConstructorInfo con = expectedType.GetConstructor(new Type[] { typeArguments[0] });
                            this.gen.Emit(OpCodes.Newobj, con);
                        }
                        else
                        {
                            if (!sequenceType.IsAssignableFrom(actualType))
                            {
                                throw System.Data.Linq.SqlClient.Error.CannotConvertToEntityRef(actualType);
                            }
                            MethodInfo info6 = TypeSystem.FindSequenceMethod("SingleOrDefault", new Type[] { sequenceType }, new Type[] { elementType });
                            this.gen.Emit(OpCodes.Call, info6);
                            ConstructorInfo info7 = expectedType.GetConstructor(new Type[] { elementType });
                            this.gen.Emit(OpCodes.Newobj, info7);
                        }
                    }
                    else if (((expectedType == typeof(IQueryable)) || (expectedType == typeof(IOrderedQueryable))) && typeof(IEnumerable).IsAssignableFrom(actualType))
                    {
                        MethodInfo info8 = TypeSystem.FindQueryableMethod("AsQueryable", new Type[] { typeof(IEnumerable) }, new Type[0]);
                        this.gen.Emit(OpCodes.Call, info8);
                        if (type == typeof(IOrderedQueryable))
                        {
                            this.gen.Emit(OpCodes.Castclass, expectedType);
                        }
                    }
                    else if (((type == typeof(IQueryable<>)) || (type == typeof(IOrderedQueryable<>))) && flag)
                    {
                        if (elementType != typeArguments[0])
                        {
                            sequenceType = typeof(IEnumerable<>).MakeGenericType(typeArguments);
                            this.GenerateConvertToType(actualType, sequenceType);
                            elementType = typeArguments[0];
                        }
                        MethodInfo info9 = TypeSystem.FindQueryableMethod("AsQueryable", new Type[] { sequenceType }, new Type[] { elementType });
                        this.gen.Emit(OpCodes.Call, info9);
                        if (type == typeof(IOrderedQueryable<>))
                        {
                            this.gen.Emit(OpCodes.Castclass, expectedType);
                        }
                    }
                    else if ((type == typeof(IOrderedEnumerable<>)) && flag)
                    {
                        if (elementType != typeArguments[0])
                        {
                            sequenceType = typeof(IEnumerable<>).MakeGenericType(typeArguments);
                            this.GenerateConvertToType(actualType, sequenceType);
                            elementType = typeArguments[0];
                        }
                        MethodInfo info10 = TypeSystem.FindStaticMethod(typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }), "CreateOrderedEnumerable", new Type[] { sequenceType }, new Type[] { elementType });
                        this.gen.Emit(OpCodes.Call, info10);
                    }
                    else if ((type == typeof(EntitySet<>)) && flag)
                    {
                        if (elementType != typeArguments[0])
                        {
                            sequenceType = typeof(IEnumerable<>).MakeGenericType(typeArguments);
                            this.GenerateConvertToType(actualType, sequenceType);
                            actualType = sequenceType;
                            elementType = typeArguments[0];
                        }
                        LocalBuilder builder2 = this.gen.DeclareLocal(actualType);
                        this.gen.Emit(OpCodes.Stloc, builder2);
                        ConstructorInfo info11 = expectedType.GetConstructor(Type.EmptyTypes);
                        this.gen.Emit(OpCodes.Newobj, info11);
                        LocalBuilder builder3 = this.gen.DeclareLocal(expectedType);
                        this.gen.Emit(OpCodes.Stloc, builder3);
                        this.gen.Emit(OpCodes.Ldloc, builder3);
                        this.gen.Emit(OpCodes.Ldloc, builder2);
                        MethodInfo info12 = expectedType.GetMethod("Assign", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { sequenceType }, null);
                        this.gen.Emit(GetMethodCallOpCode(info12), info12);
                        this.gen.Emit(OpCodes.Ldloc, builder3);
                    }
                    else if ((typeof(IEnumerable).IsAssignableFrom(expectedType) && flag) && expectedType.IsAssignableFrom(typeof(List<>).MakeGenericType(new Type[] { elementType })))
                    {
                        ConstructorInfo info13 = typeof(List<>).MakeGenericType(new Type[] { elementType }).GetConstructor(new Type[] { sequenceType });
                        this.gen.Emit(OpCodes.Newobj, info13);
                    }
                    else if (((expectedType.IsArray && (expectedType.GetArrayRank() == 1)) && (!actualType.IsArray && sequenceType.IsAssignableFrom(actualType))) && expectedType.GetElementType().IsAssignableFrom(elementType))
                    {
                        MethodInfo info14 = TypeSystem.FindSequenceMethod("ToArray", new Type[] { sequenceType }, new Type[] { elementType });
                        this.gen.Emit(OpCodes.Call, info14);
                    }
                    else
                    {
                        if ((expectedType.IsClass && typeof(ICollection<>).MakeGenericType(new Type[] { elementType }).IsAssignableFrom(expectedType)) && ((expectedType.GetConstructor(Type.EmptyTypes) != null) && sequenceType.IsAssignableFrom(actualType)))
                        {
                            throw System.Data.Linq.SqlClient.Error.GeneralCollectionMaterializationNotSupported();
                        }
                        if ((expectedType == typeof(bool)) && (actualType == typeof(int)))
                        {
                            Label label = this.gen.DefineLabel();
                            Label label2 = this.gen.DefineLabel();
                            this.gen.Emit(OpCodes.Ldc_I4_0);
                            this.gen.Emit(OpCodes.Ceq);
                            this.gen.Emit(OpCodes.Brtrue_S, label);
                            this.gen.Emit(OpCodes.Ldc_I4_1);
                            this.gen.Emit(OpCodes.Br_S, label2);
                            this.gen.MarkLabel(label);
                            this.gen.Emit(OpCodes.Ldc_I4_0);
                            this.gen.MarkLabel(label2);
                        }
                        else
                        {
                            if (actualType.IsValueType)
                            {
                                this.gen.Emit(OpCodes.Box, actualType);
                            }
                            this.gen.Emit(OpCodes.Ldtoken, expectedType);
                            MethodInfo method = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
                            this.gen.Emit(OpCodes.Call, method);
                            MethodInfo info16 = typeof(DBConvert).GetMethod("ChangeType", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object), typeof(Type) }, null);
                            this.gen.Emit(OpCodes.Call, info16);
                            if (expectedType.IsValueType)
                            {
                                this.gen.Emit(OpCodes.Unbox_Any, expectedType);
                            }
                            else if (expectedType != typeof(object))
                            {
                                this.gen.Emit(OpCodes.Castclass, expectedType);
                            }
                        }
                    }
                }
            }

            private void GenerateConvertToType(Type actualType, Type expectedType, Type readerMethodType)
            {
                this.GenerateConvertToType(readerMethodType, actualType);
                this.GenerateConvertToType(actualType, expectedType);
            }

            private void GenerateDefault(Type type)
            {
                this.GenerateDefault(type, true);
            }

            private void GenerateDefault(Type type, bool throwIfNotNullable)
            {
                if (type.IsValueType)
                {
                    if (!throwIfNotNullable || TypeSystem.IsNullableType(type))
                    {
                        LocalBuilder local = this.gen.DeclareLocal(type);
                        this.gen.Emit(OpCodes.Ldloca, local);
                        this.gen.Emit(OpCodes.Initobj, type);
                        this.gen.Emit(OpCodes.Ldloc, local);
                    }
                    else
                    {
                        this.gen.Emit(OpCodes.Ldtoken, type);
                        this.gen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static));
                        MethodInfo method = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("ErrorAssignmentToNull", BindingFlags.Public | BindingFlags.Static);
                        this.gen.Emit(OpCodes.Call, method);
                        this.gen.Emit(OpCodes.Throw);
                    }
                }
                else
                {
                    this.gen.Emit(OpCodes.Ldnull);
                }
            }

            private Type GenerateDeferredSource(SqlExpression expr, LocalBuilder locInstance)
            {
                if (expr.NodeType == SqlNodeType.ClientCase)
                {
                    return this.GenerateClientCase((SqlClientCase) expr, true, locInstance);
                }
                if (expr.NodeType != SqlNodeType.Link)
                {
                    throw System.Data.Linq.SqlClient.Error.ExpressionNotDeferredQuerySource();
                }
                return this.GenerateLink((SqlLink) expr, locInstance);
            }

            private Type GenerateDiscriminatedType(SqlDiscriminatedType dt)
            {
                LocalBuilder local = this.gen.DeclareLocal(dt.Discriminator.ClrType);
                this.GenerateExpressionForType(dt.Discriminator, dt.Discriminator.ClrType);
                this.gen.Emit(OpCodes.Stloc, local);
                return this.GenerateDiscriminatedType(dt.TargetType, local, dt.Discriminator.SqlType);
            }

            private Type GenerateDiscriminatedType(MetaType targetType, LocalBuilder locDiscriminator, ProviderType discriminatorType)
            {
                MetaType type = null;
                Label label = this.gen.DefineLabel();
                Label label2 = this.gen.DefineLabel();
                foreach (MetaType type2 in targetType.InheritanceTypes)
                {
                    if (type2.InheritanceCode != null)
                    {
                        if (type2.IsInheritanceDefault)
                        {
                            type = type2;
                        }
                        this.gen.Emit(OpCodes.Ldloc, locDiscriminator);
                        object obj2 = InheritanceRules.InheritanceCodeForClientCompare(type2.InheritanceCode, discriminatorType);
                        this.GenerateConstant(locDiscriminator.LocalType, obj2);
                        this.GenerateEquals(locDiscriminator.LocalType);
                        this.gen.Emit(OpCodes.Brfalse, label);
                        this.GenerateConstant(typeof(Type), type2.Type);
                        this.gen.Emit(OpCodes.Br, label2);
                        this.gen.MarkLabel(label);
                        label = this.gen.DefineLabel();
                    }
                }
                this.gen.MarkLabel(label);
                if (type != null)
                {
                    this.GenerateConstant(typeof(Type), type.Type);
                }
                else
                {
                    this.GenerateDefault(typeof(Type));
                }
                this.gen.MarkLabel(label2);
                return typeof(Type);
            }

            private void GenerateEquals(Type type)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Object:
                    case TypeCode.DBNull:
                    case TypeCode.String:
                    {
                        if (type.IsValueType)
                        {
                            LocalBuilder local = this.gen.DeclareLocal(type);
                            LocalBuilder builder2 = this.gen.DeclareLocal(type);
                            this.gen.Emit(OpCodes.Stloc, builder2);
                            this.gen.Emit(OpCodes.Stloc, local);
                            this.gen.Emit(OpCodes.Ldloc, local);
                            this.gen.Emit(OpCodes.Box, type);
                            this.gen.Emit(OpCodes.Ldloc, builder2);
                            this.gen.Emit(OpCodes.Box, type);
                        }
                        MethodInfo method = typeof(object).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);
                        this.gen.Emit(GetMethodCallOpCode(method), method);
                        return;
                    }
                }
                this.gen.Emit(OpCodes.Ceq);
            }

            private Type GenerateExpressionForType(SqlExpression expr, Type type) => 
                this.GenerateExpressionForType(expr, type, null);

            private Type GenerateExpressionForType(SqlExpression expr, Type type, LocalBuilder locInstance)
            {
                Type actualType = this.Generate(expr, locInstance);
                this.GenerateConvertToType(actualType, type);
                return type;
            }

            private void GenerateGetValue(Type nullableType)
            {
                MethodInfo method = nullableType.GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance);
                this.gen.Emit(OpCodes.Call, method);
            }

            private void GenerateGetValueOrDefault(Type nullableType)
            {
                MethodInfo method = nullableType.GetMethod("GetValueOrDefault", Type.EmptyTypes);
                this.gen.Emit(OpCodes.Call, method);
            }

            private Type GenerateGlobalAccess(int iGlobal, Type type)
            {
                this.GenerateAccessGlobals();
                if (type.IsValueType)
                {
                    this.GenerateConstInt(iGlobal);
                    this.gen.Emit(OpCodes.Ldelem_Ref);
                    Type cls = typeof(StrongBox<>).MakeGenericType(new Type[] { type });
                    this.gen.Emit(OpCodes.Castclass, cls);
                    FieldInfo field = cls.GetField("Value", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    this.gen.Emit(OpCodes.Ldfld, field);
                    return type;
                }
                this.GenerateConstInt(iGlobal);
                this.gen.Emit(OpCodes.Ldelem_Ref);
                this.GenerateConvertToType(typeof(object), type);
                this.gen.Emit(OpCodes.Castclass, type);
                return type;
            }

            private Type GenerateGrouping(SqlGrouping grp)
            {
                Type[] genericArguments = grp.ClrType.GetGenericArguments();
                this.GenerateExpressionForType(grp.Key, genericArguments[0]);
                this.Generate(grp.Group);
                MethodInfo meth = TypeSystem.FindStaticMethod(typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }), "CreateGroup", new Type[] { genericArguments[0], typeof(IEnumerable<>).MakeGenericType(new Type[] { genericArguments[1] }) }, genericArguments);
                this.gen.Emit(OpCodes.Call, meth);
                return meth.ReturnType;
            }

            private void GenerateHasValue(Type nullableType)
            {
                MethodInfo method = nullableType.GetMethod("get_HasValue", BindingFlags.Public | BindingFlags.Instance);
                this.gen.Emit(OpCodes.Call, method);
            }

            private Type GenerateJoinedCollection(SqlJoinedCollection jc)
            {
                LocalBuilder local = this.gen.DeclareLocal(typeof(int));
                LocalBuilder builder2 = this.gen.DeclareLocal(typeof(bool));
                Type clrType = jc.Expression.ClrType;
                Type localType = typeof(List<>).MakeGenericType(new Type[] { clrType });
                LocalBuilder builder3 = this.gen.DeclareLocal(localType);
                this.GenerateExpressionForType(jc.Count, typeof(int));
                this.gen.Emit(OpCodes.Stloc, local);
                this.gen.Emit(OpCodes.Ldloc, local);
                ConstructorInfo constructor = localType.GetConstructor(new Type[] { typeof(int) });
                this.gen.Emit(OpCodes.Newobj, constructor);
                this.gen.Emit(OpCodes.Stloc, builder3);
                this.gen.Emit(OpCodes.Ldc_I4_1);
                this.gen.Emit(OpCodes.Stloc, builder2);
                Label label = this.gen.DefineLabel();
                Label loc = this.gen.DefineLabel();
                LocalBuilder builder4 = this.gen.DeclareLocal(typeof(int));
                this.gen.Emit(OpCodes.Ldc_I4_0);
                this.gen.Emit(OpCodes.Stloc, builder4);
                this.gen.Emit(OpCodes.Br, label);
                this.gen.MarkLabel(loc);
                this.gen.Emit(OpCodes.Ldloc, builder4);
                this.gen.Emit(OpCodes.Ldc_I4_0);
                this.gen.Emit(OpCodes.Cgt);
                this.gen.Emit(OpCodes.Ldloc, builder2);
                this.gen.Emit(OpCodes.And);
                Label label3 = this.gen.DefineLabel();
                this.gen.Emit(OpCodes.Brfalse, label3);
                this.gen.Emit(OpCodes.Ldarg_0);
                MethodInfo meth = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("Read", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                this.gen.Emit(GetMethodCallOpCode(meth), meth);
                this.gen.Emit(OpCodes.Stloc, builder2);
                this.gen.MarkLabel(label3);
                Label label4 = this.gen.DefineLabel();
                this.gen.Emit(OpCodes.Ldloc, builder2);
                this.gen.Emit(OpCodes.Brfalse, label4);
                this.gen.Emit(OpCodes.Ldloc, builder3);
                this.GenerateExpressionForType(jc.Expression, clrType);
                MethodInfo info3 = localType.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { clrType }, null);
                this.gen.Emit(GetMethodCallOpCode(info3), info3);
                this.gen.MarkLabel(label4);
                this.gen.Emit(OpCodes.Ldloc, builder4);
                this.gen.Emit(OpCodes.Ldc_I4_1);
                this.gen.Emit(OpCodes.Add);
                this.gen.Emit(OpCodes.Stloc, builder4);
                this.gen.MarkLabel(label);
                this.gen.Emit(OpCodes.Ldloc, builder4);
                this.gen.Emit(OpCodes.Ldloc, local);
                this.gen.Emit(OpCodes.Clt);
                this.gen.Emit(OpCodes.Ldloc, builder2);
                this.gen.Emit(OpCodes.And);
                this.gen.Emit(OpCodes.Brtrue, loc);
                this.gen.Emit(OpCodes.Ldloc, builder3);
                return localType;
            }

            private Type GenerateLift(SqlLift lift) => 
                this.GenerateExpressionForType(lift.Expression, lift.ClrType);

            private Type GenerateLink(SqlLink link, LocalBuilder locInstance)
            {
                this.gen.Emit(OpCodes.Ldarg_0);
                int num = this.AddGlobal(typeof(MetaDataMember), link.Member);
                this.GenerateConstInt(num);
                int num2 = this.AllocateLocal();
                this.GenerateConstInt(num2);
                Type type = (link.Member.IsAssociation && link.Member.Association.IsMany) ? TypeSystem.GetElementType(link.Member.Type) : link.Member.Type;
                if (locInstance != null)
                {
                    this.gen.Emit(OpCodes.Ldloc, locInstance);
                    MethodInfo meth = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("GetNestedLinkSource", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(new Type[] { type });
                    this.gen.Emit(GetMethodCallOpCode(meth), meth);
                }
                else
                {
                    this.GenerateConstInt(link.KeyExpressions.Count);
                    this.gen.Emit(OpCodes.Newarr, typeof(object));
                    int num3 = 0;
                    int count = link.KeyExpressions.Count;
                    while (num3 < count)
                    {
                        this.gen.Emit(OpCodes.Dup);
                        this.GenerateConstInt(num3);
                        this.GenerateExpressionForType(link.KeyExpressions[num3], typeof(object));
                        this.GenerateArrayAssign(typeof(object));
                        num3++;
                    }
                    MethodInfo info3 = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("GetLinkSource", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(new Type[] { type });
                    this.gen.Emit(GetMethodCallOpCode(info3), info3);
                }
                return typeof(IEnumerable<>).MakeGenericType(new Type[] { type });
            }

            private void GenerateLoadForMemberAccess(LocalBuilder loc)
            {
                if (loc.LocalType.IsValueType)
                {
                    this.gen.Emit(OpCodes.Ldloca, loc);
                }
                else
                {
                    this.gen.Emit(OpCodes.Ldloc, loc);
                }
            }

            private void GenerateLoadMember(MemberInfo mi)
            {
                FieldInfo field = mi as FieldInfo;
                if (field != null)
                {
                    this.gen.Emit(OpCodes.Ldfld, field);
                }
                else
                {
                    MethodInfo getMethod = ((PropertyInfo) mi).GetGetMethod(true);
                    this.gen.Emit(GetMethodCallOpCode(getMethod), getMethod);
                }
            }

            private Type GenerateMember(SqlMember m)
            {
                FieldInfo member = m.Member as FieldInfo;
                if (member != null)
                {
                    this.GenerateExpressionForType(m.Expression, m.Expression.ClrType);
                    this.gen.Emit(OpCodes.Ldfld, member);
                    return member.FieldType;
                }
                PropertyInfo info2 = (PropertyInfo) m.Member;
                return this.GenerateMethodCall(new SqlMethodCall(m.ClrType, m.SqlType, info2.GetGetMethod(), m.Expression, null, m.SourceExpression));
            }

            private void GenerateMemberAssignment(MetaDataMember mm, LocalBuilder locInstance, SqlExpression expr, LocalBuilder locStoreInMember)
            {
                MemberInfo mi = (mm.StorageMember != null) ? mm.StorageMember : mm.Member;
                Type memberType = TypeSystem.GetMemberType(mi);
                if (this.IsDeferrableExpression(expr) && ((this.compiler.services.Context.LoadOptions == null) || !this.compiler.services.Context.LoadOptions.IsPreloaded(mm.Member)))
                {
                    if (mm.IsDeferred)
                    {
                        this.gen.Emit(OpCodes.Ldarg_0);
                        MethodInfo getMethod = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetProperty("CanDeferLoad").GetGetMethod();
                        this.gen.Emit(GetMethodCallOpCode(getMethod), getMethod);
                        Label label = this.gen.DefineLabel();
                        this.gen.Emit(OpCodes.Brfalse, label);
                        if (!memberType.IsGenericType)
                        {
                            throw System.Data.Linq.SqlClient.Error.DeferredMemberWrongType();
                        }
                        Type genericTypeDefinition = memberType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(EntitySet<>))
                        {
                            this.GenerateAssignDeferredEntitySet(mm, locInstance, expr, locStoreInMember);
                        }
                        else
                        {
                            if ((genericTypeDefinition != typeof(EntityRef<>)) && (genericTypeDefinition != typeof(Link<>)))
                            {
                                throw System.Data.Linq.SqlClient.Error.DeferredMemberWrongType();
                            }
                            this.GenerateAssignDeferredReference(mm, locInstance, expr, locStoreInMember);
                        }
                        this.gen.MarkLabel(label);
                    }
                }
                else if (memberType.IsGenericType && (memberType.GetGenericTypeDefinition() == typeof(EntitySet<>)))
                {
                    this.GenerateAssignEntitySet(mm, locInstance, expr, locStoreInMember);
                }
                else
                {
                    this.GenerateAssignValue(mm, locInstance, expr, locStoreInMember);
                }
            }

            private Type GenerateMethodCall(SqlMethodCall mc)
            {
                ParameterInfo[] parameters = mc.Method.GetParameters();
                if (mc.Object != null)
                {
                    Type localType = this.GenerateExpressionForType(mc.Object, mc.Object.ClrType);
                    if (localType.IsValueType)
                    {
                        LocalBuilder local = this.gen.DeclareLocal(localType);
                        this.gen.Emit(OpCodes.Stloc, local);
                        this.gen.Emit(OpCodes.Ldloca, local);
                    }
                }
                int index = 0;
                int count = mc.Arguments.Count;
                while (index < count)
                {
                    ParameterInfo info = parameters[index];
                    Type parameterType = info.ParameterType;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                        this.GenerateExpressionForType(mc.Arguments[index], parameterType);
                        LocalBuilder builder2 = this.gen.DeclareLocal(parameterType);
                        this.gen.Emit(OpCodes.Stloc, builder2);
                        this.gen.Emit(OpCodes.Ldloca, builder2);
                    }
                    else
                    {
                        this.GenerateExpressionForType(mc.Arguments[index], parameterType);
                    }
                    index++;
                }
                OpCode methodCallOpCode = GetMethodCallOpCode(mc.Method);
                if (((mc.Object != null) && TypeSystem.IsNullableType(mc.Object.ClrType)) && (methodCallOpCode == OpCodes.Callvirt))
                {
                    this.gen.Emit(OpCodes.Constrained, mc.Object.ClrType);
                }
                this.gen.Emit(methodCallOpCode, mc.Method);
                return mc.Method.ReturnType;
            }

            private Type GenerateNew(SqlNew sn)
            {
                LocalBuilder local = this.gen.DeclareLocal(sn.ClrType);
                LocalBuilder builder2 = null;
                Label label = this.gen.DefineLabel();
                Label label2 = this.gen.DefineLabel();
                if (sn.Args.Count > 0)
                {
                    ParameterInfo[] parameters = sn.Constructor.GetParameters();
                    int index = 0;
                    int count = sn.Args.Count;
                    while (index < count)
                    {
                        this.GenerateExpressionForType(sn.Args[index], parameters[index].ParameterType);
                        index++;
                    }
                }
                if (sn.Constructor != null)
                {
                    this.gen.Emit(OpCodes.Newobj, sn.Constructor);
                    this.gen.Emit(OpCodes.Stloc, local);
                }
                else if (sn.ClrType.IsValueType)
                {
                    this.gen.Emit(OpCodes.Ldloca, local);
                    this.gen.Emit(OpCodes.Initobj, sn.ClrType);
                }
                else
                {
                    ConstructorInfo constructor = sn.ClrType.GetConstructor(Type.EmptyTypes);
                    this.gen.Emit(OpCodes.Newobj, constructor);
                    this.gen.Emit(OpCodes.Stloc, local);
                }
                foreach (SqlMemberAssign assign in from m in sn.Members
                    orderby sn.MetaType.GetDataMember(m.Member).Ordinal
                    select m)
                {
                    MetaDataMember dataMember = sn.MetaType.GetDataMember(assign.Member);
                    if (dataMember.IsPrimaryKey)
                    {
                        this.GenerateMemberAssignment(dataMember, local, assign.Expression, null);
                    }
                }
                int num3 = 0;
                if (sn.MetaType.IsEntity)
                {
                    LocalBuilder builder3 = this.gen.DeclareLocal(sn.ClrType);
                    builder2 = this.gen.DeclareLocal(typeof(bool));
                    Label label3 = this.gen.DefineLabel();
                    num3 = this.AddGlobal(typeof(MetaType), sn.MetaType);
                    Type type = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType });
                    this.gen.Emit(OpCodes.Ldarg_0);
                    this.GenerateConstInt(num3);
                    this.gen.Emit(OpCodes.Ldloc, local);
                    MethodInfo meth = type.GetMethod("InsertLookup", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(object) }, null);
                    this.gen.Emit(GetMethodCallOpCode(meth), meth);
                    this.gen.Emit(OpCodes.Castclass, sn.ClrType);
                    this.gen.Emit(OpCodes.Stloc, builder3);
                    this.gen.Emit(OpCodes.Ldloc, builder3);
                    this.gen.Emit(OpCodes.Ldloc, local);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brfalse, label2);
                    this.GenerateConstInt(1);
                    this.gen.Emit(OpCodes.Stloc, builder2);
                    this.gen.Emit(OpCodes.Br_S, label3);
                    this.gen.MarkLabel(label2);
                    this.gen.Emit(OpCodes.Ldloc, builder3);
                    this.gen.Emit(OpCodes.Stloc, local);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Stloc, builder2);
                    this.gen.MarkLabel(label3);
                }
                foreach (SqlMemberAssign assign2 in from m in sn.Members
                    orderby sn.MetaType.GetDataMember(m.Member).Ordinal
                    select m)
                {
                    MetaDataMember mm = sn.MetaType.GetDataMember(assign2.Member);
                    if (!mm.IsPrimaryKey)
                    {
                        this.GenerateMemberAssignment(mm, local, assign2.Expression, builder2);
                    }
                }
                if (sn.MetaType.IsEntity)
                {
                    this.gen.Emit(OpCodes.Ldloc, builder2);
                    this.GenerateConstInt(0);
                    this.gen.Emit(OpCodes.Ceq);
                    this.gen.Emit(OpCodes.Brtrue, label);
                    this.gen.Emit(OpCodes.Ldarg_0);
                    this.GenerateConstInt(num3);
                    this.gen.Emit(OpCodes.Ldloc, local);
                    MethodInfo info3 = typeof(ObjectMaterializer<>).MakeGenericType(new Type[] { this.compiler.dataReaderType }).GetMethod("SendEntityMaterialized", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(int), typeof(object) }, null);
                    this.gen.Emit(GetMethodCallOpCode(info3), info3);
                }
                this.gen.MarkLabel(label);
                this.gen.Emit(OpCodes.Ldloc, local);
                return sn.ClrType;
            }

            private Type GenerateOptionalValue(SqlOptionalValue opt)
            {
                Label label = this.gen.DefineLabel();
                Label label2 = this.gen.DefineLabel();
                Type localType = this.Generate(opt.HasValue);
                LocalBuilder local = this.gen.DeclareLocal(localType);
                this.gen.Emit(OpCodes.Stloc, local);
                this.gen.Emit(OpCodes.Ldloca, local);
                this.GenerateHasValue(localType);
                this.gen.Emit(OpCodes.Brfalse, label);
                this.GenerateExpressionForType(opt.Value, opt.ClrType);
                this.gen.Emit(OpCodes.Br_S, label2);
                this.gen.MarkLabel(label);
                this.GenerateConstant(opt.ClrType, null);
                this.gen.MarkLabel(label2);
                return opt.ClrType;
            }

            private Type GenerateSearchedCase(SqlSearchedCase ssc)
            {
                Label loc = this.gen.DefineLabel();
                Label label = this.gen.DefineLabel();
                int num = 0;
                int count = ssc.Whens.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.gen.MarkLabel(loc);
                        loc = this.gen.DefineLabel();
                    }
                    SqlWhen when = ssc.Whens[num];
                    if (when.Match != null)
                    {
                        this.GenerateExpressionForType(when.Match, typeof(bool));
                        this.GenerateConstInt(0);
                        this.gen.Emit(OpCodes.Ceq);
                        this.gen.Emit(OpCodes.Brtrue, loc);
                    }
                    this.GenerateExpressionForType(when.Value, ssc.ClrType);
                    this.gen.Emit(OpCodes.Br, label);
                    num++;
                }
                this.gen.MarkLabel(loc);
                if (ssc.Else != null)
                {
                    this.GenerateExpressionForType(ssc.Else, ssc.ClrType);
                }
                this.gen.MarkLabel(label);
                return ssc.ClrType;
            }

            private void GenerateStoreMember(MemberInfo mi)
            {
                FieldInfo field = mi as FieldInfo;
                if (field != null)
                {
                    this.gen.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    MethodInfo setMethod = ((PropertyInfo) mi).GetSetMethod(true);
                    this.gen.Emit(GetMethodCallOpCode(setMethod), setMethod);
                }
            }

            private Type GenerateTypeCase(SqlTypeCase stc)
            {
                LocalBuilder local = this.gen.DeclareLocal(stc.Discriminator.ClrType);
                this.GenerateExpressionForType(stc.Discriminator, stc.Discriminator.ClrType);
                this.gen.Emit(OpCodes.Stloc, local);
                Label loc = this.gen.DefineLabel();
                Label label = this.gen.DefineLabel();
                bool flag = false;
                int num = 0;
                int count = stc.Whens.Count;
                while (num < count)
                {
                    if (num > 0)
                    {
                        this.gen.MarkLabel(loc);
                        loc = this.gen.DefineLabel();
                    }
                    SqlTypeCaseWhen when = stc.Whens[num];
                    if (when.Match != null)
                    {
                        this.gen.Emit(OpCodes.Ldloc, local);
                        SqlValue match = when.Match as SqlValue;
                        this.GenerateConstant(local.LocalType, match.Value);
                        this.GenerateEquals(local.LocalType);
                        this.gen.Emit(OpCodes.Brfalse, loc);
                    }
                    else
                    {
                        flag = true;
                    }
                    this.GenerateExpressionForType(when.TypeBinding, stc.ClrType);
                    this.gen.Emit(OpCodes.Br, label);
                    num++;
                }
                this.gen.MarkLabel(loc);
                if (!flag)
                {
                    this.GenerateConstant(stc.ClrType, null);
                }
                this.gen.MarkLabel(label);
                return stc.ClrType;
            }

            private Type GenerateUserColumn(SqlUserColumn suc)
            {
                if (string.IsNullOrEmpty(suc.Name))
                {
                    this.GenerateColumnAccess(suc.ClrType, suc.SqlType, 0, null);
                    return suc.ClrType;
                }
                int count = this.namedColumns.Count;
                this.namedColumns.Add(new ObjectReaderCompiler.NamedColumn(suc.Name, suc.IsRequired));
                Label label = this.gen.DefineLabel();
                Label label2 = this.gen.DefineLabel();
                LocalBuilder local = this.gen.DeclareLocal(typeof(int));
                this.GenerateAccessOrdinals();
                this.GenerateConstInt(count);
                this.GenerateArrayAccess(typeof(int), false);
                this.gen.Emit(OpCodes.Stloc, local);
                this.gen.Emit(OpCodes.Ldloc, local);
                this.GenerateConstInt(0);
                this.gen.Emit(OpCodes.Clt);
                this.gen.Emit(OpCodes.Brtrue, label);
                this.GenerateColumnAccess(suc.ClrType, suc.SqlType, 0, local);
                this.gen.Emit(OpCodes.Br_S, label2);
                this.gen.MarkLabel(label);
                this.GenerateDefault(suc.ClrType, false);
                this.gen.MarkLabel(label2);
                return suc.ClrType;
            }

            private Type GenerateValue(SqlValue value) => 
                this.GenerateConstant(value.ClrType, value.Value);

            private Type GenerateValueOf(SqlUnary u)
            {
                this.GenerateExpressionForType(u.Operand, u.Operand.ClrType);
                LocalBuilder local = this.gen.DeclareLocal(u.Operand.ClrType);
                this.gen.Emit(OpCodes.Stloc, local);
                this.gen.Emit(OpCodes.Ldloca, local);
                this.GenerateGetValue(u.Operand.ClrType);
                return u.ClrType;
            }

            private static OpCode GetMethodCallOpCode(MethodInfo mi)
            {
                if (!mi.IsStatic && !mi.DeclaringType.IsValueType)
                {
                    return OpCodes.Callvirt;
                }
                return OpCodes.Call;
            }

            private MethodInfo GetReaderMethod(Type readerType, Type valueType)
            {
                string str;
                if (valueType.IsEnum)
                {
                    valueType = valueType.BaseType;
                }
                if (Type.GetTypeCode(valueType) == TypeCode.Single)
                {
                    str = "GetFloat";
                }
                else
                {
                    str = "Get" + valueType.Name;
                }
                MethodInfo info = readerType.GetMethod(str, BindingFlags.Public | BindingFlags.Instance, null, readMethodSignature, null);
                if (info == null)
                {
                    info = readerType.GetMethod("GetValue", BindingFlags.Public | BindingFlags.Instance, null, readMethodSignature, null);
                }
                return info;
            }

            private bool HasSideEffect(SqlNode node) => 
                this.sideEffectChecker.HasSideEffect(node);

            private static bool IsAssignable(MemberInfo member)
            {
                if (member is FieldInfo)
                {
                    return true;
                }
                PropertyInfo info2 = member as PropertyInfo;
                return ((info2 != null) && info2.CanWrite);
            }

            private bool IsDeferrableExpression(SqlExpression expr)
            {
                if (expr.NodeType != SqlNodeType.Link)
                {
                    if (expr.NodeType != SqlNodeType.ClientCase)
                    {
                        return false;
                    }
                    SqlClientCase @case = (SqlClientCase) expr;
                    foreach (SqlClientWhen when in @case.Whens)
                    {
                        if (!this.IsDeferrableExpression(when.Value))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            internal object[] Globals =>
                this.globals.ToArray();

            internal int Locals =>
                this.nLocals;

            internal ObjectReaderCompiler.NamedColumn[] NamedColumns =>
                this.namedColumns.ToArray();
        }

        internal class Group<K, T> : IGrouping<K, T>, IEnumerable<T>, IEnumerable
        {
            private IEnumerable<T> items;
            private K key;

            internal Group(K key, IEnumerable<T> items)
            {
                this.key = key;
                this.items = items;
            }

            public IEnumerator<T> GetEnumerator() => 
                this.items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();

            K IGrouping<K, T>.Key =>
                this.key;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NamedColumn
        {
            private string name;
            private bool isRequired;
            internal NamedColumn(string name, bool isRequired)
            {
                this.name = name;
                this.isRequired = isRequired;
            }

            internal string Name =>
                this.name;
            internal bool IsRequired =>
                this.isRequired;
        }

        private class ObjectReader<TDataReader, TObject> : ObjectReaderCompiler.ObjectReaderBase<TDataReader>, IEnumerator<TObject>, IObjectReader, IEnumerator, IDisposable where TDataReader: DbDataReader
        {
            private TObject current;
            private bool disposeSession;
            private Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize;

            internal ObjectReader(ObjectReaderCompiler.ObjectReaderSession<TDataReader> session, ObjectReaderCompiler.NamedColumn[] namedColumns, object[] globals, object[] arguments, int nLocals, bool disposeSession, Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize) : base(session, namedColumns, globals, arguments, nLocals)
            {
                this.disposeSession = disposeSession;
                this.fnMaterialize = fnMaterialize;
            }

            public void Dispose()
            {
                if (this.disposeSession)
                {
                    base.session.Dispose();
                }
            }

            public bool MoveNext()
            {
                if (this.Read())
                {
                    this.current = this.fnMaterialize(this);
                    return true;
                }
                this.current = default(TObject);
                this.Dispose();
                return false;
            }

            public void Reset()
            {
            }

            public TObject Current =>
                this.current;

            public IObjectReaderSession Session =>
                base.session;

            object IEnumerator.Current =>
                this.Current;
        }

        private abstract class ObjectReaderBase<TDataReader> : ObjectMaterializer<TDataReader> where TDataReader: DbDataReader
        {
            private bool hasCurrentRow;
            private bool hasRead;
            private bool isFinished;
            private IDataServices services;
            protected ObjectReaderCompiler.ObjectReaderSession<TDataReader> session;

            internal ObjectReaderBase(ObjectReaderCompiler.ObjectReaderSession<TDataReader> session, ObjectReaderCompiler.NamedColumn[] namedColumns, object[] globals, object[] arguments, int nLocals)
            {
                this.session = session;
                this.services = session.Provider.Services;
                base.DataReader = session.DataReader;
                base.Globals = globals;
                base.Arguments = arguments;
                if (nLocals > 0)
                {
                    base.Locals = new object[nLocals];
                }
                if (this.session.IsBuffered)
                {
                    this.Buffer();
                }
                base.Ordinals = this.GetColumnOrdinals(namedColumns);
            }

            internal void Buffer()
            {
                if ((base.BufferReader == null) && (this.hasCurrentRow || !this.hasRead))
                {
                    if (this.session.IsBuffered)
                    {
                        base.BufferReader = this.session.GetNextBufferedReader();
                    }
                    else
                    {
                        DataSet set = new DataSet {
                            EnforceConstraints = false
                        };
                        DataTable table = new DataTable();
                        set.Tables.Add(table);
                        string[] activeNames = this.session.GetActiveNames();
                        table.Load(new ObjectReaderCompiler.Rereader(base.DataReader, this.hasCurrentRow, null), LoadOption.OverwriteChanges);
                        base.BufferReader = new ObjectReaderCompiler.Rereader(table.CreateDataReader(), false, activeNames);
                    }
                    if (this.hasCurrentRow)
                    {
                        this.Read();
                    }
                }
            }

            public override IEnumerable ExecuteSubQuery(int iSubQuery, object[] parentArgs)
            {
                if (this.session.ParentArguments != null)
                {
                    int length = this.session.ParentArguments.Length;
                    object[] destinationArray = new object[length + parentArgs.Length];
                    Array.Copy(this.session.ParentArguments, destinationArray, length);
                    Array.Copy(parentArgs, 0, destinationArray, length, parentArgs.Length);
                    parentArgs = destinationArray;
                }
                ICompiledSubQuery query = this.session.SubQueries[iSubQuery];
                return (IEnumerable) query.Execute(this.session.Provider, parentArgs, this.session.UserArguments).ReturnValue;
            }

            private int[] GetColumnOrdinals(ObjectReaderCompiler.NamedColumn[] namedColumns)
            {
                DbDataReader bufferReader = null;
                if (base.BufferReader != null)
                {
                    bufferReader = base.BufferReader;
                }
                else
                {
                    bufferReader = base.DataReader;
                }
                if ((namedColumns == null) || (namedColumns.Length == 0))
                {
                    return null;
                }
                int[] numArray = new int[namedColumns.Length];
                Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                int ordinal = 0;
                int fieldCount = bufferReader.FieldCount;
                while (ordinal < fieldCount)
                {
                    dictionary[SqlIdentifier.QuoteCompoundIdentifier(bufferReader.GetName(ordinal))] = ordinal;
                    ordinal++;
                }
                int index = 0;
                int length = namedColumns.Length;
                while (index < length)
                {
                    int num5;
                    if (dictionary.TryGetValue(SqlIdentifier.QuoteCompoundIdentifier(namedColumns[index].Name), out num5))
                    {
                        numArray[index] = num5;
                    }
                    else
                    {
                        if (namedColumns[index].IsRequired)
                        {
                            throw System.Data.Linq.SqlClient.Error.RequiredColumnDoesNotExist(namedColumns[index].Name);
                        }
                        numArray[index] = -1;
                    }
                    index++;
                }
                return numArray;
            }

            public override IEnumerable<T> GetLinkSource<T>(int iGlobalLink, int iLocalFactory, object[] keyValues)
            {
                IDeferredSourceFactory deferredSourceFactory = (IDeferredSourceFactory) base.Locals[iLocalFactory];
                if (deferredSourceFactory == null)
                {
                    MetaDataMember member = (MetaDataMember) base.Globals[iGlobalLink];
                    deferredSourceFactory = this.services.GetDeferredSourceFactory(member);
                    base.Locals[iLocalFactory] = deferredSourceFactory;
                }
                return (IEnumerable<T>) deferredSourceFactory.CreateDeferredSource(keyValues);
            }

            public override IEnumerable<T> GetNestedLinkSource<T>(int iGlobalLink, int iLocalFactory, object instance)
            {
                IDeferredSourceFactory deferredSourceFactory = (IDeferredSourceFactory) base.Locals[iLocalFactory];
                if (deferredSourceFactory == null)
                {
                    MetaDataMember member = (MetaDataMember) base.Globals[iGlobalLink];
                    deferredSourceFactory = this.services.GetDeferredSourceFactory(member);
                    base.Locals[iLocalFactory] = deferredSourceFactory;
                }
                return (IEnumerable<T>) deferredSourceFactory.CreateDeferredSource(instance);
            }

            public override object InsertLookup(int iMetaType, object instance)
            {
                MetaType type = (MetaType) base.Globals[iMetaType];
                return this.services.InsertLookupCachedObject(type, instance);
            }

            public override bool Read()
            {
                if (this.isFinished)
                {
                    return false;
                }
                if (base.BufferReader != null)
                {
                    this.hasCurrentRow = base.BufferReader.Read();
                }
                else
                {
                    this.hasCurrentRow = this.DataReader.Read();
                }
                if (!this.hasCurrentRow)
                {
                    this.isFinished = true;
                    this.session.Finish((ObjectReaderCompiler.ObjectReaderBase<TDataReader>) this);
                }
                this.hasRead = true;
                return this.hasCurrentRow;
            }

            public override void SendEntityMaterialized(int iMetaType, object instance)
            {
                MetaType type = (MetaType) base.Globals[iMetaType];
                this.services.OnEntityMaterialized(type, instance);
            }

            public override bool CanDeferLoad =>
                this.services.Context.DeferredLoadingEnabled;

            internal bool IsBuffered =>
                (base.BufferReader != null);
        }

        private class ObjectReaderFactory<TDataReader, TObject> : IObjectReaderFactory where TDataReader: DbDataReader
        {
            private Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize;
            private object[] globals;
            private ObjectReaderCompiler.NamedColumn[] namedColumns;
            private int nLocals;

            internal ObjectReaderFactory(Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize, ObjectReaderCompiler.NamedColumn[] namedColumns, object[] globals, int nLocals)
            {
                this.fnMaterialize = fnMaterialize;
                this.namedColumns = namedColumns;
                this.globals = globals;
                this.nLocals = nLocals;
            }

            public IObjectReader Create(DbDataReader dataReader, bool disposeDataReader, IReaderProvider provider, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries)
            {
                ObjectReaderCompiler.ObjectReaderSession<TDataReader> session = new ObjectReaderCompiler.ObjectReaderSession<TDataReader>((TDataReader) dataReader, provider, parentArgs, userArgs, subQueries);
                return session.CreateReader<TObject>(this.fnMaterialize, this.namedColumns, this.globals, this.nLocals, disposeDataReader);
            }

            public IObjectReader GetNextResult(IObjectReaderSession session, bool disposeDataReader)
            {
                ObjectReaderCompiler.ObjectReaderSession<TDataReader> session2 = (ObjectReaderCompiler.ObjectReaderSession<TDataReader>) session;
                IObjectReader reader = session2.GetNextResult<TObject>(this.fnMaterialize, this.namedColumns, this.globals, this.nLocals, disposeDataReader);
                if ((reader == null) && disposeDataReader)
                {
                    session2.Dispose();
                }
                return reader;
            }
        }

        private class ObjectReaderSession<TDataReader> : IObjectReaderSession, IDisposable, IConnectionUser where TDataReader: DbDataReader
        {
            private List<DbDataReader> buffer;
            private ObjectReaderCompiler.ObjectReaderBase<TDataReader> currentReader;
            private TDataReader dataReader;
            private bool hasResults;
            private int iNextBufferedReader;
            private bool isDataReaderDisposed;
            private bool isDisposed;
            private object[] parentArgs;
            private IReaderProvider provider;
            private ICompiledSubQuery[] subQueries;
            private object[] userArgs;

            internal ObjectReaderSession(TDataReader dataReader, IReaderProvider provider, object[] parentArgs, object[] userArgs, ICompiledSubQuery[] subQueries)
            {
                this.dataReader = dataReader;
                this.provider = provider;
                this.parentArgs = parentArgs;
                this.userArgs = userArgs;
                this.subQueries = subQueries;
                this.hasResults = true;
            }

            public void Buffer()
            {
                if (this.buffer == null)
                {
                    if ((this.currentReader != null) && !this.currentReader.IsBuffered)
                    {
                        this.currentReader.Buffer();
                        this.CheckNextResults();
                    }
                    this.buffer = new List<DbDataReader>();
                    while (this.hasResults)
                    {
                        DataSet set = new DataSet {
                            EnforceConstraints = false
                        };
                        DataTable table = new DataTable();
                        set.Tables.Add(table);
                        string[] activeNames = this.GetActiveNames();
                        table.Load(new ObjectReaderCompiler.Rereader(this.dataReader, false, null), LoadOption.OverwriteChanges);
                        this.buffer.Add(new ObjectReaderCompiler.Rereader(table.CreateDataReader(), false, activeNames));
                        this.CheckNextResults();
                    }
                }
            }

            private void CheckNextResults()
            {
                this.hasResults = !this.dataReader.IsClosed && this.dataReader.NextResult();
                this.currentReader = null;
                if (!this.hasResults)
                {
                    this.Dispose();
                }
            }

            public void CompleteUse()
            {
                this.Buffer();
            }

            internal ObjectReaderCompiler.ObjectReader<TDataReader, TObject> CreateReader<TObject>(Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize, ObjectReaderCompiler.NamedColumn[] namedColumns, object[] globals, int nLocals, bool disposeDataReader)
            {
                ObjectReaderCompiler.ObjectReader<TDataReader, TObject> reader = new ObjectReaderCompiler.ObjectReader<TDataReader, TObject>((ObjectReaderCompiler.ObjectReaderSession<TDataReader>) this, namedColumns, globals, this.userArgs, nLocals, disposeDataReader, fnMaterialize);
                this.currentReader = reader;
                return reader;
            }

            public void Dispose()
            {
                if (!this.isDisposed)
                {
                    this.isDisposed = true;
                    if (!this.isDataReaderDisposed)
                    {
                        this.isDataReaderDisposed = true;
                        this.dataReader.Dispose();
                    }
                    this.provider.ConnectionManager.ReleaseConnection(this);
                }
            }

            internal void Finish(ObjectReaderCompiler.ObjectReaderBase<TDataReader> finishedReader)
            {
                if (this.currentReader == finishedReader)
                {
                    this.CheckNextResults();
                }
            }

            internal string[] GetActiveNames()
            {
                string[] strArray = new string[this.DataReader.FieldCount];
                int index = 0;
                int fieldCount = this.DataReader.FieldCount;
                while (index < fieldCount)
                {
                    strArray[index] = this.DataReader.GetName(index);
                    index++;
                }
                return strArray;
            }

            internal DbDataReader GetNextBufferedReader()
            {
                if (this.iNextBufferedReader < this.buffer.Count)
                {
                    return this.buffer[this.iNextBufferedReader++];
                }
                return null;
            }

            internal ObjectReaderCompiler.ObjectReader<TDataReader, TObject> GetNextResult<TObject>(Func<ObjectMaterializer<TDataReader>, TObject> fnMaterialize, ObjectReaderCompiler.NamedColumn[] namedColumns, object[] globals, int nLocals, bool disposeDataReader)
            {
                if (this.buffer != null)
                {
                    if (this.iNextBufferedReader >= this.buffer.Count)
                    {
                        return null;
                    }
                }
                else
                {
                    if (this.currentReader != null)
                    {
                        this.currentReader.Buffer();
                        this.CheckNextResults();
                    }
                    if (!this.hasResults)
                    {
                        return null;
                    }
                }
                ObjectReaderCompiler.ObjectReader<TDataReader, TObject> reader = new ObjectReaderCompiler.ObjectReader<TDataReader, TObject>((ObjectReaderCompiler.ObjectReaderSession<TDataReader>) this, namedColumns, globals, this.userArgs, nLocals, disposeDataReader, fnMaterialize);
                this.currentReader = reader;
                return reader;
            }

            internal ObjectReaderCompiler.ObjectReaderBase<TDataReader> CurrentReader =>
                this.currentReader;

            internal TDataReader DataReader =>
                this.dataReader;

            public bool IsBuffered =>
                (this.buffer != null);

            internal object[] ParentArguments =>
                this.parentArgs;

            internal IReaderProvider Provider =>
                this.provider;

            internal ICompiledSubQuery[] SubQueries =>
                this.subQueries;

            internal object[] UserArguments =>
                this.userArgs;
        }

        internal class OrderedResults<T> : IOrderedEnumerable<T>, IEnumerable<T>, IEnumerable
        {
            private List<T> values;

            internal OrderedResults(IEnumerable<T> results)
            {
                this.values = results as List<T>;
                if (this.values == null)
                {
                    this.values = new List<T>(results);
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
                this.values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => 
                this.values.GetEnumerator();

            IOrderedEnumerable<T> IOrderedEnumerable<T>.CreateOrderedEnumerable<K>(Func<T, K> keySelector, IComparer<K> comparer, bool descending)
            {
                throw System.Data.Linq.SqlClient.Error.NotSupported();
            }
        }

        private class ReaderFactoryCache
        {
            private LinkedList<CacheInfo> list;
            private int maxCacheSize;

            internal ReaderFactoryCache(int maxCacheSize)
            {
                this.maxCacheSize = maxCacheSize;
                this.list = new LinkedList<CacheInfo>();
            }

            internal void AddFactory(Type elementType, Type dataReaderType, object mapping, DataLoadOptions options, SqlExpression projection, IObjectReaderFactory factory)
            {
                this.list.AddFirst(new LinkedListNode<CacheInfo>(new CacheInfo(elementType, dataReaderType, mapping, options, projection, factory)));
                if (this.list.Count > this.maxCacheSize)
                {
                    this.list.RemoveLast();
                }
            }

            internal IObjectReaderFactory GetFactory(Type elementType, Type dataReaderType, object mapping, DataLoadOptions options, SqlExpression projection)
            {
                for (LinkedListNode<CacheInfo> node = this.list.First; node != null; node = node.Next)
                {
                    if ((((elementType == node.Value.elementType) && (dataReaderType == node.Value.dataReaderType)) && ((mapping == node.Value.mapping) && ShapesAreSimilar(options, node.Value.options))) && ObjectReaderCompiler.SqlProjectionComparer.AreSimilar(projection, node.Value.projection))
                    {
                        this.list.Remove(node);
                        this.list.AddFirst(node);
                        return node.Value.factory;
                    }
                }
                return null;
            }

            private static bool ShapesAreSimilar(DataLoadOptions ds1, DataLoadOptions ds2)
            {
                if (ds1 != ds2)
                {
                    if ((ds1 != null) && !ds1.IsEmpty)
                    {
                        return false;
                    }
                    if (ds2 != null)
                    {
                        return ds2.IsEmpty;
                    }
                }
                return true;
            }

            private class CacheInfo
            {
                internal Type dataReaderType;
                internal Type elementType;
                internal IObjectReaderFactory factory;
                internal object mapping;
                internal DataLoadOptions options;
                internal SqlExpression projection;

                public CacheInfo(Type elementType, Type dataReaderType, object mapping, DataLoadOptions options, SqlExpression projection, IObjectReaderFactory factory)
                {
                    this.elementType = elementType;
                    this.dataReaderType = dataReaderType;
                    this.options = options;
                    this.mapping = mapping;
                    this.projection = projection;
                    this.factory = factory;
                }
            }
        }

        private class Rereader : DbDataReader, IDisposable
        {
            private bool first;
            private string[] names;
            private DbDataReader reader;

            internal Rereader(DbDataReader reader, bool hasCurrentRow, string[] names)
            {
                this.reader = reader;
                this.first = hasCurrentRow;
                this.names = names;
            }

            public override void Close()
            {
            }

            public override bool GetBoolean(int i) => 
                this.reader.GetBoolean(i);

            public override byte GetByte(int i) => 
                this.reader.GetByte(i);

            public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferOffset, int length) => 
                this.reader.GetBytes(i, fieldOffset, buffer, bufferOffset, length);

            public override char GetChar(int i) => 
                this.reader.GetChar(i);

            public override long GetChars(int i, long fieldOffset, char[] buffer, int bufferOffset, int length) => 
                this.reader.GetChars(i, fieldOffset, buffer, bufferOffset, length);

            public override string GetDataTypeName(int i) => 
                this.reader.GetDataTypeName(i);

            public override DateTime GetDateTime(int i) => 
                this.reader.GetDateTime(i);

            public override decimal GetDecimal(int i) => 
                this.reader.GetDecimal(i);

            public override double GetDouble(int i) => 
                this.reader.GetDouble(i);

            public override IEnumerator GetEnumerator() => 
                this.reader.GetEnumerator();

            public override Type GetFieldType(int i) => 
                this.reader.GetFieldType(i);

            public override float GetFloat(int i) => 
                this.reader.GetFloat(i);

            public override Guid GetGuid(int i) => 
                this.reader.GetGuid(i);

            public override short GetInt16(int i) => 
                this.reader.GetInt16(i);

            public override int GetInt32(int i) => 
                this.reader.GetInt32(i);

            public override long GetInt64(int i) => 
                this.reader.GetInt64(i);

            public override string GetName(int i)
            {
                if (this.names != null)
                {
                    return this.names[i];
                }
                return this.reader.GetName(i);
            }

            public override int GetOrdinal(string name) => 
                this.reader.GetOrdinal(name);

            public override DataTable GetSchemaTable() => 
                this.reader.GetSchemaTable();

            public override string GetString(int i) => 
                this.reader.GetString(i);

            public override object GetValue(int i) => 
                this.reader.GetValue(i);

            public override int GetValues(object[] values) => 
                this.reader.GetValues(values);

            public override bool IsDBNull(int i) => 
                this.reader.IsDBNull(i);

            public override bool NextResult() => 
                false;

            public override bool Read()
            {
                if (this.first)
                {
                    this.first = false;
                    return true;
                }
                return this.reader.Read();
            }

            public override int Depth =>
                this.reader.Depth;

            public override int FieldCount =>
                this.reader.FieldCount;

            public override bool HasRows
            {
                get
                {
                    if (!this.first)
                    {
                        return this.reader.HasRows;
                    }
                    return true;
                }
            }

            public override bool IsClosed =>
                this.reader.IsClosed;

            public override object this[int i] =>
                this.reader[i];

            public override object this[string name] =>
                this.reader[name];

            public override int RecordsAffected =>
                this.reader.RecordsAffected;
        }

        private class SideEffectChecker : SqlVisitor
        {
            private bool hasSideEffect;

            internal bool HasSideEffect(SqlNode node)
            {
                this.hasSideEffect = false;
                this.Visit(node);
                return this.hasSideEffect;
            }

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq) => 
                cq;

            internal override SqlExpression VisitJoinedCollection(SqlJoinedCollection jc)
            {
                this.hasSideEffect = true;
                return jc;
            }
        }

        private class SourceExpressionRemover : SqlDuplicator.DuplicatingVisitor
        {
            internal SourceExpressionRemover() : base(true)
            {
            }

            internal override SqlNode Visit(SqlNode node)
            {
                node = base.Visit(node);
                if (node != null)
                {
                    node.ClearSourceExpression();
                }
                return node;
            }

            internal override SqlExpression VisitAliasRef(SqlAliasRef aref)
            {
                SqlExpression expression = base.VisitAliasRef(aref);
                if ((expression != null) && (expression == aref))
                {
                    SqlAlias alias = aref.Alias;
                    return new SqlAliasRef(new SqlAlias(new SqlNop(aref.ClrType, aref.SqlType, null)));
                }
                return expression;
            }

            internal override SqlExpression VisitColumnRef(SqlColumnRef cref)
            {
                SqlExpression expression = base.VisitColumnRef(cref);
                if ((expression != null) && (expression == cref))
                {
                    SqlColumn column = cref.Column;
                    SqlColumn col = new SqlColumn(column.ClrType, column.SqlType, column.Name, column.MetaMember, null, column.SourceExpression) {
                        Ordinal = column.Ordinal
                    };
                    expression = new SqlColumnRef(col);
                    col.ClearSourceExpression();
                }
                return expression;
            }
        }

        internal class SqlProjectionComparer
        {
            internal static bool AreSimilar(SqlExpression node1, SqlExpression node2)
            {
                if (node1 == node2)
                {
                    return true;
                }
                if ((node1 != null) && (node2 != null))
                {
                    if (((node1.NodeType != node2.NodeType) || (node1.ClrType != node2.ClrType)) || (node1.SqlType != node2.SqlType))
                    {
                        return false;
                    }
                    switch (node1.NodeType)
                    {
                        case SqlNodeType.ClientArray:
                        {
                            SqlClientArray array = (SqlClientArray) node1;
                            SqlClientArray array2 = (SqlClientArray) node2;
                            if (array.Expressions.Count == array2.Expressions.Count)
                            {
                                int num7 = 0;
                                int count = array.Expressions.Count;
                                while (num7 < count)
                                {
                                    if (!AreSimilar(array.Expressions[num7], array2.Expressions[num7]))
                                    {
                                        return false;
                                    }
                                    num7++;
                                }
                                return true;
                            }
                            return false;
                        }
                        case SqlNodeType.ClientCase:
                        {
                            SqlClientCase @case = (SqlClientCase) node1;
                            SqlClientCase case2 = (SqlClientCase) node2;
                            if (@case.Whens.Count == case2.Whens.Count)
                            {
                                int num9 = 0;
                                int num10 = @case.Whens.Count;
                                while (num9 < num10)
                                {
                                    if (!AreSimilar(@case.Whens[num9].Match, case2.Whens[num9].Match) || !AreSimilar(@case.Whens[num9].Value, case2.Whens[num9].Value))
                                    {
                                        return false;
                                    }
                                    num9++;
                                }
                                return true;
                            }
                            return false;
                        }
                        case SqlNodeType.ClientQuery:
                        {
                            SqlClientQuery query = (SqlClientQuery) node1;
                            SqlClientQuery query2 = (SqlClientQuery) node2;
                            if (query.Arguments.Count == query2.Arguments.Count)
                            {
                                int num15 = 0;
                                int num16 = query.Arguments.Count;
                                while (num15 < num16)
                                {
                                    if (!AreSimilar(query.Arguments[num15], query2.Arguments[num15]))
                                    {
                                        return false;
                                    }
                                    num15++;
                                }
                                return true;
                            }
                            return false;
                        }
                        case SqlNodeType.ColumnRef:
                        {
                            SqlColumnRef ref2 = (SqlColumnRef) node1;
                            SqlColumnRef ref3 = (SqlColumnRef) node2;
                            return (ref2.Column.Ordinal == ref3.Column.Ordinal);
                        }
                        case SqlNodeType.DiscriminatedType:
                        {
                            SqlDiscriminatedType type = (SqlDiscriminatedType) node1;
                            SqlDiscriminatedType type2 = (SqlDiscriminatedType) node2;
                            return AreSimilar(type.Discriminator, type2.Discriminator);
                        }
                        case SqlNodeType.Lift:
                            return AreSimilar(((SqlLift) node1).Expression, ((SqlLift) node2).Expression);

                        case SqlNodeType.Link:
                        {
                            SqlLink link = (SqlLink) node1;
                            SqlLink link2 = (SqlLink) node2;
                            if (MetaPosition.AreSameMember(link.Member.Member, link2.Member.Member))
                            {
                                if (link.KeyExpressions.Count != link2.KeyExpressions.Count)
                                {
                                    return false;
                                }
                                int num5 = 0;
                                int num6 = link.KeyExpressions.Count;
                                while (num5 < num6)
                                {
                                    if (!AreSimilar(link.KeyExpressions[num5], link2.KeyExpressions[num5]))
                                    {
                                        return false;
                                    }
                                    num5++;
                                }
                                return true;
                            }
                            return false;
                        }
                        case SqlNodeType.Grouping:
                        {
                            SqlGrouping grouping = (SqlGrouping) node1;
                            SqlGrouping grouping2 = (SqlGrouping) node2;
                            return (AreSimilar(grouping.Key, grouping2.Key) && AreSimilar(grouping.Group, grouping2.Group));
                        }
                        case SqlNodeType.JoinedCollection:
                        {
                            SqlJoinedCollection joineds = (SqlJoinedCollection) node1;
                            SqlJoinedCollection joineds2 = (SqlJoinedCollection) node2;
                            if (!AreSimilar(joineds.Count, joineds2.Count))
                            {
                                return false;
                            }
                            return AreSimilar(joineds.Expression, joineds2.Expression);
                        }
                        case SqlNodeType.MethodCall:
                        {
                            SqlMethodCall call = (SqlMethodCall) node1;
                            SqlMethodCall call2 = (SqlMethodCall) node2;
                            if ((call.Method == call2.Method) && AreSimilar(call.Object, call2.Object))
                            {
                                if (call.Arguments.Count != call2.Arguments.Count)
                                {
                                    return false;
                                }
                                int num17 = 0;
                                int num18 = call.Arguments.Count;
                                while (num17 < num18)
                                {
                                    if (!AreSimilar(call.Arguments[num17], call2.Arguments[num17]))
                                    {
                                        return false;
                                    }
                                    num17++;
                                }
                                return true;
                            }
                            return false;
                        }
                        case SqlNodeType.Member:
                        {
                            SqlMember member = (SqlMember) node1;
                            SqlMember member2 = (SqlMember) node2;
                            if (member.Member != member2.Member)
                            {
                                return false;
                            }
                            return AreSimilar(member.Expression, member2.Expression);
                        }
                        case SqlNodeType.OptionalValue:
                        {
                            SqlOptionalValue value2 = (SqlOptionalValue) node1;
                            SqlOptionalValue value3 = (SqlOptionalValue) node2;
                            return AreSimilar(value2.Value, value3.Value);
                        }
                        case SqlNodeType.OuterJoinedValue:
                        case SqlNodeType.ValueOf:
                            return AreSimilar(((SqlUnary) node1).Operand, ((SqlUnary) node2).Operand);

                        case SqlNodeType.SearchedCase:
                        {
                            SqlSearchedCase case3 = (SqlSearchedCase) node1;
                            SqlSearchedCase case4 = (SqlSearchedCase) node2;
                            if (case3.Whens.Count != case4.Whens.Count)
                            {
                                return false;
                            }
                            int num11 = 0;
                            int num12 = case3.Whens.Count;
                            while (num11 < num12)
                            {
                                if (!AreSimilar(case3.Whens[num11].Match, case4.Whens[num11].Match) || !AreSimilar(case3.Whens[num11].Value, case4.Whens[num11].Value))
                                {
                                    return false;
                                }
                                num11++;
                            }
                            return AreSimilar(case3.Else, case4.Else);
                        }
                        case SqlNodeType.New:
                        {
                            SqlNew new2 = (SqlNew) node1;
                            SqlNew new3 = (SqlNew) node2;
                            if ((new2.Args.Count != new3.Args.Count) || (new2.Members.Count != new3.Members.Count))
                            {
                                return false;
                            }
                            int num = 0;
                            int num2 = new2.Args.Count;
                            while (num < num2)
                            {
                                if (!AreSimilar(new2.Args[num], new3.Args[num]))
                                {
                                    return false;
                                }
                                num++;
                            }
                            int num3 = 0;
                            int num4 = new2.Members.Count;
                            while (num3 < num4)
                            {
                                if (!MetaPosition.AreSameMember(new2.Members[num3].Member, new3.Members[num3].Member) || !AreSimilar(new2.Members[num3].Expression, new3.Members[num3].Expression))
                                {
                                    return false;
                                }
                                num3++;
                            }
                            return true;
                        }
                        case SqlNodeType.Value:
                            return object.Equals(((SqlValue) node1).Value, ((SqlValue) node2).Value);

                        case SqlNodeType.UserColumn:
                            return (((SqlUserColumn) node1).Name == ((SqlUserColumn) node2).Name);

                        case SqlNodeType.TypeCase:
                        {
                            SqlTypeCase case5 = (SqlTypeCase) node1;
                            SqlTypeCase case6 = (SqlTypeCase) node2;
                            if (!AreSimilar(case5.Discriminator, case6.Discriminator))
                            {
                                return false;
                            }
                            if (case5.Whens.Count != case6.Whens.Count)
                            {
                                return false;
                            }
                            int num13 = 0;
                            int num14 = case5.Whens.Count;
                            while (num13 < num14)
                            {
                                if (!AreSimilar(case5.Whens[num13].Match, case6.Whens[num13].Match))
                                {
                                    return false;
                                }
                                if (!AreSimilar(case5.Whens[num13].TypeBinding, case6.Whens[num13].TypeBinding))
                                {
                                    return false;
                                }
                                num13++;
                            }
                            return true;
                        }
                    }
                }
                return false;
            }

            internal static bool CanBeCompared(SqlExpression node)
            {
                if (node == null)
                {
                    return true;
                }
                switch (node.NodeType)
                {
                    case SqlNodeType.ClientArray:
                    {
                        SqlClientArray array = (SqlClientArray) node;
                        int num7 = 0;
                        int count = array.Expressions.Count;
                        while (num7 < count)
                        {
                            if (!CanBeCompared(array.Expressions[num7]))
                            {
                                return false;
                            }
                            num7++;
                        }
                        return true;
                    }
                    case SqlNodeType.ClientCase:
                    {
                        SqlClientCase @case = (SqlClientCase) node;
                        int num9 = 0;
                        int num10 = @case.Whens.Count;
                        while (num9 < num10)
                        {
                            if (!CanBeCompared(@case.Whens[num9].Match) || !CanBeCompared(@case.Whens[num9].Value))
                            {
                                return false;
                            }
                            num9++;
                        }
                        return true;
                    }
                    case SqlNodeType.ClientQuery:
                        return true;

                    case SqlNodeType.ColumnRef:
                    case SqlNodeType.Value:
                    case SqlNodeType.UserColumn:
                        return true;

                    case SqlNodeType.DiscriminatedType:
                        return CanBeCompared(((SqlDiscriminatedType) node).Discriminator);

                    case SqlNodeType.Lift:
                        return CanBeCompared(((SqlLift) node).Expression);

                    case SqlNodeType.Link:
                    {
                        SqlLink link = (SqlLink) node;
                        int num5 = 0;
                        int num6 = link.KeyExpressions.Count;
                        while (num5 < num6)
                        {
                            if (!CanBeCompared(link.KeyExpressions[num5]))
                            {
                                return false;
                            }
                            num5++;
                        }
                        return true;
                    }
                    case SqlNodeType.Grouping:
                    {
                        SqlGrouping grouping = (SqlGrouping) node;
                        return (CanBeCompared(grouping.Key) && CanBeCompared(grouping.Group));
                    }
                    case SqlNodeType.JoinedCollection:
                    {
                        SqlJoinedCollection joineds = (SqlJoinedCollection) node;
                        if (!CanBeCompared(joineds.Count))
                        {
                            return false;
                        }
                        return CanBeCompared(joineds.Expression);
                    }
                    case SqlNodeType.MethodCall:
                    {
                        SqlMethodCall call = (SqlMethodCall) node;
                        if ((call.Object == null) || CanBeCompared(call.Object))
                        {
                            int num15 = 0;
                            int num16 = call.Arguments.Count;
                            while (num15 < num16)
                            {
                                if (!CanBeCompared(call.Arguments[0]))
                                {
                                    return false;
                                }
                                num15++;
                            }
                            return true;
                        }
                        return false;
                    }
                    case SqlNodeType.Member:
                        return CanBeCompared(((SqlMember) node).Expression);

                    case SqlNodeType.OptionalValue:
                        return CanBeCompared(((SqlOptionalValue) node).Value);

                    case SqlNodeType.OuterJoinedValue:
                    case SqlNodeType.ValueOf:
                        return CanBeCompared(((SqlUnary) node).Operand);

                    case SqlNodeType.SearchedCase:
                    {
                        SqlSearchedCase case2 = (SqlSearchedCase) node;
                        int num11 = 0;
                        int num12 = case2.Whens.Count;
                        while (num11 < num12)
                        {
                            if (!CanBeCompared(case2.Whens[num11].Match) || !CanBeCompared(case2.Whens[num11].Value))
                            {
                                return false;
                            }
                            num11++;
                        }
                        return CanBeCompared(case2.Else);
                    }
                    case SqlNodeType.New:
                    {
                        SqlNew new2 = (SqlNew) node;
                        int num = 0;
                        int num2 = new2.Args.Count;
                        while (num < num2)
                        {
                            if (!CanBeCompared(new2.Args[num]))
                            {
                                return false;
                            }
                            num++;
                        }
                        int num3 = 0;
                        int num4 = new2.Members.Count;
                        while (num3 < num4)
                        {
                            if (!CanBeCompared(new2.Members[num3].Expression))
                            {
                                return false;
                            }
                            num3++;
                        }
                        return true;
                    }
                    case SqlNodeType.TypeCase:
                    {
                        SqlTypeCase case3 = (SqlTypeCase) node;
                        if (!CanBeCompared(case3.Discriminator))
                        {
                            return false;
                        }
                        int num13 = 0;
                        int num14 = case3.Whens.Count;
                        while (num13 < num14)
                        {
                            if (!CanBeCompared(case3.Whens[num13].Match))
                            {
                                return false;
                            }
                            if (!CanBeCompared(case3.Whens[num13].TypeBinding))
                            {
                                return false;
                            }
                            num13++;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

