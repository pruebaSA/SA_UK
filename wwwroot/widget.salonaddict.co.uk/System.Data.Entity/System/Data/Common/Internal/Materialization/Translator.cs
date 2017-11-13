namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.QueryCache;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Data.Objects.ELinq;
    using System.Data.Objects.Internal;
    using System.Data.Query.InternalTrees;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    internal class Translator : ColumnMapVisitorWithResults<TranslatorResult, TranslatorArg>
    {
        private CoordinatorScratchpad _currentCoordinatorScratchpad;
        private bool _hasLinkDemand;
        private bool _hasNonPublicMembers;
        private readonly MergeOption _mergeOption;
        private readonly Dictionary<EdmType, ObjectTypeMapping> _objectTypeMappings = new Dictionary<EdmType, ObjectTypeMapping>();
        private CoordinatorScratchpad _rootCoordinatorScratchpad;
        private readonly SpanIndex _spanIndex;
        private int _stateSlotCount;
        private readonly List<Expression> _userExpressions = new List<Expression>();
        private readonly MetadataWorkspace _workspace;
        private static readonly MethodInfo DbDataReader_GetBoolean = typeof(DbDataReader).GetMethod("GetBoolean");
        private static readonly MethodInfo DbDataReader_GetByte = typeof(DbDataReader).GetMethod("GetByte");
        private static readonly MethodInfo DbDataReader_GetDateTime = typeof(DbDataReader).GetMethod("GetDateTime");
        private static readonly MethodInfo DbDataReader_GetDecimal = typeof(DbDataReader).GetMethod("GetDecimal");
        private static readonly MethodInfo DbDataReader_GetDouble = typeof(DbDataReader).GetMethod("GetDouble");
        private static readonly MethodInfo DbDataReader_GetFloat = typeof(DbDataReader).GetMethod("GetFloat");
        private static readonly MethodInfo DbDataReader_GetGuid = typeof(DbDataReader).GetMethod("GetGuid");
        private static readonly MethodInfo DbDataReader_GetInt16 = typeof(DbDataReader).GetMethod("GetInt16");
        private static readonly MethodInfo DbDataReader_GetInt32 = typeof(DbDataReader).GetMethod("GetInt32");
        private static readonly MethodInfo DbDataReader_GetInt64 = typeof(DbDataReader).GetMethod("GetInt64");
        private static readonly MethodInfo DbDataReader_GetString = typeof(DbDataReader).GetMethod("GetString");
        private static readonly MethodInfo DbDataReader_GetValue = typeof(DbDataReader).GetMethod("GetValue");
        private static readonly MethodInfo DbDataReader_IsDBNull = typeof(DbDataReader).GetMethod("IsDBNull");
        private static readonly Expression DBNull_Value = Expression.Constant(DBNull.Value, typeof(object));
        private static readonly ConstructorInfo EntityKey_ctor_CompositeKey = typeof(EntityKey).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(EntitySet), typeof(object[]) }, null);
        private static readonly ConstructorInfo EntityKey_ctor_SingleKey = typeof(EntityKey).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(EntitySet), typeof(object) }, null);
        private static readonly MethodInfo IEntityKeyWithKey_EntityKey = typeof(IEntityWithKey).GetProperty("EntityKey").GetSetMethod();
        private static readonly MethodInfo IEqualityComparerOfString_Equals = typeof(IEqualityComparer<string>).GetMethod("Equals", new Type[] { typeof(string), typeof(string) });
        private readonly bool IsValueLayer;
        private static readonly ConstructorInfo MaterializedDataRecord_ctor = typeof(MaterializedDataRecord).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(MetadataWorkspace), typeof(TypeUsage), typeof(object[]) }, null);
        private static readonly MethodInfo RecordState_GatherData = typeof(RecordState).GetMethod("GatherData", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo RecordState_SetNullRecord = typeof(RecordState).GetMethod("SetNullRecord", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo Shaper_Discriminate = typeof(Shaper).GetMethod("Discriminate");
        private static readonly MethodInfo Shaper_GetColumnValueWithErrorHandling = typeof(Shaper).GetMethod("GetColumnValueWithErrorHandling");
        private static readonly MethodInfo Shaper_GetPropertyValueWithErrorHandling = typeof(Shaper).GetMethod("GetPropertyValueWithErrorHandling");
        private static readonly MethodInfo Shaper_HandleEntity = typeof(Shaper).GetMethod("HandleEntity");
        private static readonly MethodInfo Shaper_HandleEntityAppendOnly = typeof(Shaper).GetMethod("HandleEntityAppendOnly");
        private static readonly MethodInfo Shaper_HandleFullSpanCollection = typeof(Shaper).GetMethod("HandleFullSpanCollection");
        private static readonly MethodInfo Shaper_HandleFullSpanElement = typeof(Shaper).GetMethod("HandleFullSpanElement");
        private static readonly MethodInfo Shaper_HandleIEntityWithKey = typeof(Shaper).GetMethod("HandleIEntityWithKey");
        private static readonly MethodInfo Shaper_HandleIEntityWithRelationships = typeof(Shaper).GetMethod("HandleIEntityWithRelationships");
        private static readonly MethodInfo Shaper_HandleRelationshipSpan = typeof(Shaper).GetMethod("HandleRelationshipSpan");
        internal static readonly ParameterExpression Shaper_Parameter = Expression.Parameter(typeof(Shaper), "shaper");
        private static readonly Expression Shaper_Reader = Expression.Field(Shaper_Parameter, typeof(Shaper).GetField("Reader"));
        private static readonly MethodInfo Shaper_SetColumnValue = typeof(Shaper).GetMethod("SetColumnValue");
        private static readonly MethodInfo Shaper_SetEntityRecordInfo = typeof(Shaper).GetMethod("SetEntityRecordInfo");
        private static readonly MethodInfo Shaper_SetState = typeof(Shaper).GetMethod("SetState");
        private static readonly MethodInfo Shaper_SetStatePassthrough = typeof(Shaper).GetMethod("SetStatePassthrough");
        private static readonly Expression Shaper_State = Expression.Field(Shaper_Parameter, typeof(Shaper).GetField("State"));
        private static readonly Expression Shaper_Workspace = Expression.Field(Shaper_Parameter, typeof(Shaper).GetField("Workspace"));
        private static readonly MethodInfo Translator_BinaryEquals = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("BinaryEquals", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo Translator_CheckedConvert = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("CheckedConvert", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo Translator_Compile = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("Compile", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(Expression) }, null);
        private static readonly MethodInfo Translator_MultipleDiscriminatorPolymorphicColumnMapHelper = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("MultipleDiscriminatorPolymorphicColumnMapHelper", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo Translator_TestCompile = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("TestCompile", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo Translator_TypedCreateInlineDelegate = typeof(System.Data.Common.Internal.Materialization.Translator).GetMethod("TypedCreateInlineDelegate", BindingFlags.NonPublic | BindingFlags.Instance);

        private Translator(MetadataWorkspace workspace, SpanIndex spanIndex, MergeOption mergeOption, bool valueLayer)
        {
            this._workspace = workspace;
            this._spanIndex = spanIndex;
            this._mergeOption = mergeOption;
            this.IsValueLayer = valueLayer;
        }

        private static TranslatorResult AcceptWithMappedType(System.Data.Common.Internal.Materialization.Translator translator, ColumnMap columnMap, ColumnMap parent)
        {
            Type requestedType = translator.DetermineClrType(columnMap.Type);
            return columnMap.Accept<TranslatorResult, TranslatorArg>(translator, new TranslatorArg(requestedType));
        }

        private int AllocateStateSlot() => 
            this._stateSlotCount++;

        private static bool BinaryEquals(byte[] left, byte[] right)
        {
            if (left == null)
            {
                return (null == right);
            }
            if (right == null)
            {
                return false;
            }
            if (left.Length != right.Length)
            {
                return false;
            }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            return true;
        }

        private Expression BuildExpressionToGetCoordinator(Type elementType, Expression element, Expression[] keyReaders, Expression discriminator, object discriminatorValue, CoordinatorScratchpad coordinatorScratchpad)
        {
            int stateSlotNumber = this.AllocateStateSlot();
            coordinatorScratchpad.StateSlotNumber = stateSlotNumber;
            coordinatorScratchpad.Element = Emit_EnsureType(element, elementType);
            List<Expression> operands = new List<Expression>(keyReaders.Length);
            List<Expression> list2 = new List<Expression>(keyReaders.Length);
            foreach (Expression expression in keyReaders)
            {
                int num2 = this.AllocateStateSlot();
                operands.Add(Emit_Shaper_SetState(num2, expression));
                list2.Add(Emit_Equal(Emit_Shaper_GetState(num2, expression.Type), expression));
            }
            coordinatorScratchpad.SetKeys = Emit_BitwiseOr(operands);
            coordinatorScratchpad.CheckKeys = Emit_AndAlso(list2);
            if (discriminator != null)
            {
                coordinatorScratchpad.HasData = Emit_Equal(Expression.Constant(discriminatorValue, discriminator.Type), discriminator);
            }
            return Emit_Shaper_GetState(stateSlotNumber, typeof(Coordinator<>).MakeGenericType(new Type[] { elementType }));
        }

        private Expression BuildExpressionToGetRecordState(StructuredColumnMap columnMap, Expression entityKeyReader, Expression entitySetReader, Expression nullCheckExpression)
        {
            RecordStateScratchpad scratchpad = this._currentCoordinatorScratchpad.CreateRecordStateScratchpad();
            int num = this.AllocateStateSlot();
            scratchpad.StateSlotNumber = num;
            int length = columnMap.Properties.Length;
            int num3 = (entityKeyReader != null) ? (length + 1) : length;
            scratchpad.ColumnCount = length;
            EntityType type = null;
            if (TypeHelpers.TryGetEdmType<EntityType>(columnMap.Type, out type))
            {
                scratchpad.DataRecordInfo = new EntityRecordInfo(type, EntityKey.EntityNotValidKey, null);
            }
            else
            {
                TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(columnMap.Type);
                scratchpad.DataRecordInfo = new DataRecordInfo(modelTypeUsage);
            }
            Expression[] operands = new Expression[num3];
            string[] strArray = new string[scratchpad.ColumnCount];
            TypeUsage[] usageArray = new TypeUsage[scratchpad.ColumnCount];
            for (int i = 0; i < length; i++)
            {
                Expression left = columnMap.Properties[i].Accept<TranslatorResult, TranslatorArg>(this, new TranslatorArg(typeof(object))).Expression;
                operands[i] = Expression.Call(Shaper_Parameter, Shaper_SetColumnValue, new Expression[] { Expression.Constant(num), Expression.Constant(i), Expression.Coalesce(left, DBNull_Value) });
                strArray[i] = columnMap.Properties[i].Name;
                usageArray[i] = columnMap.Properties[i].Type;
            }
            if (entityKeyReader != null)
            {
                operands[num3 - 1] = Expression.Call(Shaper_Parameter, Shaper_SetEntityRecordInfo, new Expression[] { Expression.Constant(num), entityKeyReader, entitySetReader });
            }
            scratchpad.GatherData = Emit_BitwiseOr(operands);
            scratchpad.PropertyNames = strArray;
            scratchpad.TypeUsages = usageArray;
            Expression ifFalse = Expression.Call(Emit_Shaper_GetState(num, typeof(RecordState)), RecordState_GatherData, new Expression[] { Shaper_Parameter });
            if (nullCheckExpression != null)
            {
                Expression ifTrue = Expression.Call(Emit_Shaper_GetState(num, typeof(RecordState)), RecordState_SetNullRecord, new Expression[] { Shaper_Parameter });
                ifFalse = Expression.Condition(nullCheckExpression, ifTrue, ifFalse);
            }
            return ifFalse;
        }

        private static TTarget CheckedConvert<TSource, TTarget>(TSource value)
        {
            TTarget local;
            try
            {
                local = (TTarget) value;
            }
            catch (InvalidCastException)
            {
                throw EntityUtil.ValueInvalidCast(value.GetType(), typeof(TTarget));
            }
            catch (NullReferenceException)
            {
                throw EntityUtil.ValueNullReferenceCast(typeof(TTarget));
            }
            return local;
        }

        [SecurityTreatAsSafe, SecurityCritical, ReflectionPermission(SecurityAction.Assert, MemberAccess=true, ReflectionEmit=true)]
        internal static Func<Shaper, TResult> Compile<TResult>(Expression body) => 
            Expression.Lambda<Func<Shaper, TResult>>(body, new ParameterExpression[] { Shaper_Parameter }).Compile();

        internal static object Compile(Type resultType, Expression body) => 
            Translator_Compile.MakeGenericMethod(new Type[] { resultType }).Invoke(null, new object[] { body });

        private LambdaExpression CreateInlineDelegate(Expression body)
        {
            Type type = body.Type;
            return (LambdaExpression) Translator_TypedCreateInlineDelegate.MakeGenericMethod(new Type[] { type }).Invoke(this, new object[] { body });
        }

        private List<MemberBinding> CreatePropertyBindings(StructuredColumnMap columnMap, Type clrType, ReadOnlyMetadataCollection<EdmProperty> properties)
        {
            List<MemberBinding> list = new List<MemberBinding>(columnMap.Properties.Length);
            ObjectTypeMapping objectMapping = this.LookupObjectMapping(columnMap.Type.EdmType);
            for (int i = 0; i < columnMap.Properties.Length; i++)
            {
                MethodInfo info;
                Type type;
                LightweightCodeGenerator.ValidateSetterProperty(objectMapping.GetPropertyMap(properties[i].Name).ClrProperty.PropertySetterHandle, out info, out type);
                if (LightweightCodeGenerator.HasLinkDemand(info))
                {
                    this._hasLinkDemand = true;
                }
                if (!LightweightCodeGenerator.IsPublic(info))
                {
                    this._hasNonPublicMembers = true;
                }
                Expression expression = columnMap.Properties[i].Accept<TranslatorResult, TranslatorArg>(this, new TranslatorArg(type)).Expression;
                ScalarColumnMap map = columnMap.Properties[i] as ScalarColumnMap;
                if (map != null)
                {
                    string propertyName = info.Name.Substring(4);
                    Expression expressionWithErrorHandling = Emit_Shaper_GetPropertyValueWithErrorHandling(type, map.ColumnPos, propertyName, info.DeclaringType.Name);
                    this._currentCoordinatorScratchpad.AddExpressionWithErrorHandling(expression, expressionWithErrorHandling);
                }
                MemberBinding item = Expression.Bind(info, expression);
                list.Add(item);
            }
            return list;
        }

        private static void DemandFullTrust()
        {
            LightweightCodeGenerator.FullTrustPermission.Demand();
        }

        private static void DemandMemberAccess()
        {
            LightweightCodeGenerator.MemberAccessReflectionPermission.Demand();
        }

        private Type DetermineClrType(EdmType edmType)
        {
            Type clrEquivalentType = null;
            edmType = this.ResolveSpanType(edmType);
            switch (edmType.BuiltInTypeKind)
            {
                case BuiltInTypeKind.PrimitiveType:
                    clrEquivalentType = ((PrimitiveType) edmType).ClrEquivalentType;
                    if (clrEquivalentType.IsValueType)
                    {
                        clrEquivalentType = typeof(Nullable<>).MakeGenericType(new Type[] { clrEquivalentType });
                    }
                    return clrEquivalentType;

                case BuiltInTypeKind.RefType:
                    return typeof(EntityKey);

                case BuiltInTypeKind.RowType:
                {
                    if (this.IsValueLayer)
                    {
                        return typeof(RecordState);
                    }
                    InitializerMetadata initializerMetadata = ((RowType) edmType).InitializerMetadata;
                    if (initializerMetadata != null)
                    {
                        return initializerMetadata.ClrType;
                    }
                    return typeof(DbDataRecord);
                }
                case BuiltInTypeKind.CollectionType:
                    if (!this.IsValueLayer)
                    {
                        EdmType type2 = ((CollectionType) edmType).TypeUsage.EdmType;
                        clrEquivalentType = this.DetermineClrType(type2);
                        return typeof(IEnumerable<>).MakeGenericType(new Type[] { clrEquivalentType });
                    }
                    return typeof(Coordinator<RecordState>);

                case BuiltInTypeKind.ComplexType:
                case BuiltInTypeKind.EntityType:
                    if (this.IsValueLayer)
                    {
                        return typeof(RecordState);
                    }
                    return this.LookupObjectMapping(edmType).ClrType.ClrType;
            }
            throw EntityUtil.UnexpectedMetadataType(edmType);
        }

        private Type DetermineClrType(TypeUsage typeUsage) => 
            this.DetermineClrType(typeUsage.EdmType);

        private Type DetermineElementType(Type collectionType, CollectionColumnMap columnMap)
        {
            Type elementType = null;
            if (this.IsValueLayer)
            {
                return typeof(RecordState);
            }
            elementType = TypeSystem.GetElementType(collectionType);
            if (elementType == collectionType)
            {
                TypeUsage typeUsage = ((CollectionType) columnMap.Type.EdmType).TypeUsage;
                elementType = this.DetermineClrType(typeUsage);
            }
            return elementType;
        }

        private static Expression Emit_AndAlso(IEnumerable<Expression> operands)
        {
            Expression left = null;
            foreach (Expression expression2 in operands)
            {
                if (left == null)
                {
                    left = expression2;
                }
                else
                {
                    left = Expression.AndAlso(left, expression2);
                }
            }
            return left;
        }

        private static Expression Emit_BitwiseOr(IEnumerable<Expression> operands)
        {
            Expression left = null;
            foreach (Expression expression2 in operands)
            {
                if (left == null)
                {
                    left = expression2;
                }
                else
                {
                    left = Expression.Or(left, expression2);
                }
            }
            return left;
        }

        internal static Expression Emit_EnsureType(Expression input, Type type)
        {
            Expression expression = input;
            if (input.Type == type)
            {
                return expression;
            }
            if (type.IsAssignableFrom(input.Type))
            {
                return Expression.Convert(input, type);
            }
            return Expression.Call(Translator_CheckedConvert.MakeGenericMethod(new Type[] { input.Type, type }), new Expression[] { input });
        }

        private static Expression Emit_EntityKey_ctor(System.Data.Common.Internal.Materialization.Translator translator, EntityIdentity entityIdentity, bool isForColumnValue, out Expression entitySetReader)
        {
            Expression expression;
            Expression left = null;
            Expression expression5;
            List<Expression> initializers = new List<Expression>(entityIdentity.Keys.Length);
            for (int i = 0; i < entityIdentity.Keys.Length; i++)
            {
                Expression item = entityIdentity.Keys[i].Accept<TranslatorResult, TranslatorArg>(translator, new TranslatorArg(typeof(object))).Expression;
                initializers.Add(item);
            }
            SimpleEntityIdentity identity = entityIdentity as SimpleEntityIdentity;
            if (identity != null)
            {
                entitySetReader = Expression.Constant(identity.EntitySet, typeof(EntitySet));
            }
            else
            {
                DiscriminatedEntityIdentity identity2 = (DiscriminatedEntityIdentity) entityIdentity;
                Expression expression4 = identity2.EntitySetColumnMap.Accept<TranslatorResult, TranslatorArg>(translator, new TranslatorArg(typeof(int?))).Expression;
                EntitySet[] entitySetMap = identity2.EntitySetMap;
                entitySetReader = Expression.Constant(null, typeof(EntitySet));
                for (int j = 0; j < entitySetMap.Length; j++)
                {
                    entitySetReader = Expression.Condition(Expression.Equal(expression4, Expression.Constant(j, typeof(int?))), Expression.Constant(entitySetMap[j], typeof(EntitySet)), entitySetReader);
                }
                int stateSlotNumber = translator.AllocateStateSlot();
                left = Emit_Shaper_SetStatePassthrough(stateSlotNumber, entitySetReader);
                entitySetReader = Emit_Shaper_GetState(stateSlotNumber, typeof(EntitySet));
            }
            if (1 == entityIdentity.Keys.Length)
            {
                expression = Expression.New(EntityKey_ctor_SingleKey, new Expression[] { entitySetReader, initializers[0] });
            }
            else
            {
                expression = Expression.New(EntityKey_ctor_CompositeKey, new Expression[] { entitySetReader, Expression.NewArrayInit(typeof(object), initializers) });
            }
            if (left == null)
            {
                return expression;
            }
            if (translator.IsValueLayer && !isForColumnValue)
            {
                expression5 = Expression.Constant(EntityKey.NoEntitySetKey, typeof(EntityKey));
            }
            else
            {
                expression5 = Expression.Constant(null, typeof(EntityKey));
            }
            return Expression.Condition(Expression.Equal(left, Expression.Constant(null, typeof(EntitySet))), expression5, expression);
        }

        private static Expression Emit_EntityKey_HasValue(SimpleColumnMap[] keyColumns) => 
            Expression.Not(Emit_Reader_IsDBNull(keyColumns[0]));

        private static Expression Emit_Equal(Expression left, Expression right)
        {
            if (typeof(byte[]) == left.Type)
            {
                return Expression.Call(Translator_BinaryEquals, new Expression[] { left, right });
            }
            return Expression.Equal(left, right);
        }

        internal static Expression Emit_NullConstant(Type type)
        {
            EntityUtil.CheckArgumentNull<Type>(type, "type");
            if (type.IsClass || TypeSystem.IsNullableType(type))
            {
                return Expression.Constant(null, type);
            }
            return Emit_EnsureType(Expression.Constant(null, typeof(object)), type);
        }

        private static Expression Emit_Reader_GetValue(int ordinal, Type type) => 
            Emit_EnsureType(Expression.Call(Shaper_Reader, DbDataReader_GetValue, new Expression[] { Expression.Constant(ordinal) }), type);

        private static Expression Emit_Reader_GetXXX(int ordinal, Type type, MethodInfo dataReaderMethod) => 
            Emit_EnsureType(Expression.Call(Shaper_Reader, dataReaderMethod, new Expression[] { Expression.Constant(ordinal) }), type);

        private static Expression Emit_Reader_IsDBNull(ColumnMap columnMap) => 
            Emit_Reader_IsDBNull(((ScalarColumnMap) columnMap).ColumnPos);

        private static Expression Emit_Reader_IsDBNull(int ordinal) => 
            Expression.Call(Shaper_Reader, DbDataReader_IsDBNull, new Expression[] { Expression.Constant(ordinal) });

        private static Expression Emit_Shaper_GetColumnValueWithErrorHandling(Type columnType, int ordinal) => 
            Expression.Call(Shaper_Parameter, Shaper_GetColumnValueWithErrorHandling.MakeGenericMethod(new Type[] { columnType }), new Expression[] { Expression.Constant(ordinal) });

        private static Expression Emit_Shaper_GetPropertyValueWithErrorHandling(Type propertyType, int ordinal, string propertyName, string typeName) => 
            Expression.Call(Shaper_Parameter, Shaper_GetPropertyValueWithErrorHandling.MakeGenericMethod(new Type[] { propertyType }), new Expression[] { Expression.Constant(ordinal), Expression.Constant(propertyName), Expression.Constant(typeName) });

        private static Expression Emit_Shaper_GetState(int stateSlotNumber, Type type) => 
            Emit_EnsureType(Expression.ArrayIndex(Shaper_State, Expression.Constant(stateSlotNumber)), type);

        private static Expression Emit_Shaper_SetState(int stateSlotNumber, Expression value) => 
            Expression.Call(Shaper_Parameter, Shaper_SetState.MakeGenericMethod(new Type[] { value.Type }), new Expression[] { Expression.Constant(stateSlotNumber), value });

        private static Expression Emit_Shaper_SetStatePassthrough(int stateSlotNumber, Expression value) => 
            Expression.Call(Shaper_Parameter, Shaper_SetStatePassthrough.MakeGenericMethod(new Type[] { value.Type }), new Expression[] { Expression.Constant(stateSlotNumber), value });

        private void EnterCoordinatorTranslateScope(CoordinatorScratchpad coordinatorScratchpad)
        {
            if (this._rootCoordinatorScratchpad == null)
            {
                coordinatorScratchpad.Depth = 0;
                this._rootCoordinatorScratchpad = coordinatorScratchpad;
                this._currentCoordinatorScratchpad = coordinatorScratchpad;
            }
            else
            {
                coordinatorScratchpad.Depth = this._currentCoordinatorScratchpad.Depth + 1;
                this._currentCoordinatorScratchpad.AddNestedCoordinator(coordinatorScratchpad);
                this._currentCoordinatorScratchpad = coordinatorScratchpad;
            }
        }

        private void ExitCoodinatorTranslateScope()
        {
            this._currentCoordinatorScratchpad = this._currentCoordinatorScratchpad.Parent;
        }

        private Action GetCheckPermissionsDelegate()
        {
            if (this._hasLinkDemand)
            {
                return new Action(System.Data.Common.Internal.Materialization.Translator.DemandFullTrust);
            }
            if (this._hasNonPublicMembers)
            {
                return new Action(System.Data.Common.Internal.Materialization.Translator.DemandMemberAccess);
            }
            return null;
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            ConstructorInfo constructorForType = null;
            if (!type.IsAbstract)
            {
                constructorForType = LightweightCodeGenerator.GetConstructorForType(type);
                if (LightweightCodeGenerator.HasLinkDemand(constructorForType))
                {
                    this._hasLinkDemand = true;
                }
                if (!LightweightCodeGenerator.IsPublic(constructorForType))
                {
                    this._hasNonPublicMembers = true;
                }
            }
            return constructorForType;
        }

        internal static MethodInfo GetReaderMethod(Type type, out bool isNullable)
        {
            MethodInfo info;
            isNullable = false;
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                isNullable = true;
                type = underlyingType;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return DbDataReader_GetBoolean;

                case TypeCode.Byte:
                    return DbDataReader_GetByte;

                case TypeCode.Int16:
                    return DbDataReader_GetInt16;

                case TypeCode.Int32:
                    return DbDataReader_GetInt32;

                case TypeCode.Int64:
                    return DbDataReader_GetInt64;

                case TypeCode.Single:
                    return DbDataReader_GetFloat;

                case TypeCode.Double:
                    return DbDataReader_GetDouble;

                case TypeCode.Decimal:
                    return DbDataReader_GetDecimal;

                case TypeCode.DateTime:
                    return DbDataReader_GetDateTime;

                case TypeCode.String:
                    info = DbDataReader_GetString;
                    isNullable = true;
                    return info;
            }
            if (typeof(Guid) == type)
            {
                return DbDataReader_GetGuid;
            }
            if ((typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type))
            {
                return DbDataReader_GetValue;
            }
            if (typeof(object) == type)
            {
                return DbDataReader_GetValue;
            }
            info = DbDataReader_GetValue;
            isNullable = true;
            return info;
        }

        private Expression HandleLinqRecord(RecordColumnMap columnMap, InitializerMetadata initializerMetadata)
        {
            List<TranslatorResult> propertyTranslatorResults = new List<TranslatorResult>(columnMap.Properties.Length);
            foreach (KeyValuePair<ColumnMap, Type> pair in columnMap.Properties.Zip<ColumnMap, Type>(initializerMetadata.GetChildTypes()))
            {
                ColumnMap key = pair.Key;
                Type requestedType = pair.Value;
                if (requestedType == null)
                {
                    requestedType = this.DetermineClrType(key.Type);
                }
                TranslatorResult item = key.Accept<TranslatorResult, TranslatorArg>(this, new TranslatorArg(requestedType));
                propertyTranslatorResults.Add(item);
            }
            return initializerMetadata.Emit(this, propertyTranslatorResults);
        }

        private Expression HandleRegularRecord(RecordColumnMap columnMap, TranslatorArg arg, RowType spanRowType)
        {
            Expression[] initializers = new Expression[columnMap.Properties.Length];
            for (int i = 0; i < initializers.Length; i++)
            {
                Expression input = AcceptWithMappedType(this, columnMap.Properties[i], columnMap).Expression;
                initializers[i] = Expression.Coalesce(Emit_EnsureType(input, typeof(object)), DBNull_Value);
            }
            Expression expression2 = Expression.NewArrayInit(typeof(object), initializers);
            TypeUsage type = columnMap.Type;
            if (this._spanIndex != null)
            {
                type = this._spanIndex.GetSpannedRowType(spanRowType) ?? type;
            }
            Expression expression3 = Expression.Constant(type, typeof(TypeUsage));
            return Emit_EnsureType(Expression.New(MaterializedDataRecord_ctor, new Expression[] { Shaper_Workspace, expression3, expression2 }), arg.RequestedType);
        }

        private Expression HandleSpandexRecord(RecordColumnMap columnMap, TranslatorArg arg, RowType spanRowType)
        {
            Dictionary<int, AssociationEndMember> spanMap = this._spanIndex.GetSpanMap(spanRowType);
            Expression input = columnMap.Properties[0].Accept<TranslatorResult, TranslatorArg>(this, arg).Expression;
            for (int i = 1; i < columnMap.Properties.Length; i++)
            {
                AssociationEndMember member = spanMap[i];
                TranslatorResult result = AcceptWithMappedType(this, columnMap.Properties[i], columnMap);
                Expression expression = result.Expression;
                CollectionTranslatorResult result2 = result as CollectionTranslatorResult;
                if (result2 != null)
                {
                    Expression expressionToGetCoordinator = result2.ExpressionToGetCoordinator;
                    Type type = expression.Type.GetGenericArguments()[0];
                    MethodInfo method = Shaper_HandleFullSpanCollection.MakeGenericMethod(new Type[] { arg.RequestedType, type });
                    input = Expression.Call(Shaper_Parameter, method, new Expression[] { Emit_EnsureType(input, arg.RequestedType), expressionToGetCoordinator, Expression.Constant(member) });
                }
                else if (typeof(EntityKey) == expression.Type)
                {
                    MethodInfo info2 = Shaper_HandleRelationshipSpan.MakeGenericMethod(new Type[] { arg.RequestedType });
                    input = Expression.Call(Shaper_Parameter, info2, new Expression[] { Emit_EnsureType(input, arg.RequestedType), expression, Expression.Constant(member) });
                }
                else
                {
                    MethodInfo info3 = Shaper_HandleFullSpanElement.MakeGenericMethod(new Type[] { arg.RequestedType, expression.Type });
                    input = Expression.Call(Shaper_Parameter, info3, new Expression[] { Emit_EnsureType(input, arg.RequestedType), expression, Expression.Constant(member) });
                }
            }
            return input;
        }

        private ObjectTypeMapping LookupObjectMapping(EdmType edmType)
        {
            ObjectTypeMapping objectMapping;
            EdmType key = this.ResolveSpanType(edmType);
            if (key == null)
            {
                key = edmType;
            }
            if (!this._objectTypeMappings.TryGetValue(key, out objectMapping))
            {
                objectMapping = System.Data.Common.Internal.Materialization.Util.GetObjectMapping(key, this._workspace);
                this._objectTypeMappings.Add(key, objectMapping);
            }
            return objectMapping;
        }

        private Expression MultipleDiscriminatorPolymorphicColumnMapHelper<TElement>(MultipleDiscriminatorPolymorphicColumnMap columnMap, TranslatorArg arg)
        {
            Expression[] initializers = new Expression[columnMap.TypeDiscriminators.Length];
            for (int i = 0; i < initializers.Length; i++)
            {
                initializers[i] = columnMap.TypeDiscriminators[i].Accept<TranslatorResult, TranslatorArg>(this, new TranslatorArg(typeof(object))).Expression;
            }
            Expression expression = Expression.NewArrayInit(typeof(object), initializers);
            List<Expression> list = new List<Expression>();
            Type type = typeof(KeyValuePair<EntityType, Func<Shaper, TElement>>);
            ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(EntityType), typeof(Func<Shaper, TElement>) });
            foreach (KeyValuePair<EntityType, TypedColumnMap> pair in columnMap.TypeChoices)
            {
                Expression body = Emit_EnsureType(AcceptWithMappedType(this, pair.Value, columnMap).Expression, typeof(TElement));
                LambdaExpression expression3 = this.CreateInlineDelegate(body);
                Expression item = Expression.New(constructor, new Expression[] { Expression.Constant(pair.Key), expression3 });
                list.Add(item);
            }
            MethodInfo method = Shaper_Discriminate.MakeGenericMethod(new Type[] { typeof(TElement) });
            return Expression.Call(Shaper_Parameter, method, new Expression[] { expression, Expression.Constant(columnMap.Discriminate), Expression.NewArrayInit(type, list) });
        }

        private TranslatorResult ProcessCollectionColumnMap(CollectionColumnMap columnMap, TranslatorArg arg) => 
            this.ProcessCollectionColumnMap(columnMap, arg, null, null);

        private TranslatorResult ProcessCollectionColumnMap(CollectionColumnMap columnMap, TranslatorArg arg, ColumnMap discriminatorColumnMap, object discriminatorValue)
        {
            Expression[] expressionArray;
            Expression expression5;
            Type elementType = this.DetermineElementType(arg.RequestedType, columnMap);
            CoordinatorScratchpad coordinatorScratchpad = new CoordinatorScratchpad(elementType);
            this.EnterCoordinatorTranslateScope(coordinatorScratchpad);
            ColumnMap element = columnMap.Element;
            if (this.IsValueLayer && !(element is StructuredColumnMap))
            {
                ColumnMap[] properties = new ColumnMap[] { columnMap.Element };
                element = new RecordColumnMap(columnMap.Element.Type, columnMap.Element.Name, properties, null);
            }
            Expression expression = element.Accept<TranslatorResult, TranslatorArg>(this, new TranslatorArg(elementType)).Expression;
            if (columnMap.Keys != null)
            {
                expressionArray = new Expression[columnMap.Keys.Length];
                for (int i = 0; i < expressionArray.Length; i++)
                {
                    expressionArray[i] = AcceptWithMappedType(this, columnMap.Keys[i], columnMap).Expression;
                }
            }
            else
            {
                expressionArray = new Expression[0];
            }
            Expression discriminator = null;
            if (discriminatorColumnMap != null)
            {
                discriminator = AcceptWithMappedType(this, discriminatorColumnMap, columnMap).Expression;
            }
            Expression instance = this.BuildExpressionToGetCoordinator(elementType, expression, expressionArray, discriminator, discriminatorValue, coordinatorScratchpad);
            MethodInfo method = typeof(Coordinator<>).MakeGenericType(new Type[] { elementType }).GetMethod("GetElements", BindingFlags.NonPublic | BindingFlags.Instance);
            if (this.IsValueLayer)
            {
                expression5 = instance;
            }
            else
            {
                expression5 = Expression.Call(instance, method);
                if (!arg.RequestedType.IsAssignableFrom(expression5.Type))
                {
                    if (typeof(ObjectQuery).IsAssignableFrom(arg.RequestedType))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.ELinq_UnsupportedObjectQueryMaterialization);
                    }
                    if (!arg.RequestedType.IsAssignableFrom(typeof(IOrderedQueryable<>).MakeGenericType(new Type[] { elementType })) && !arg.RequestedType.IsAssignableFrom(typeof(IOrderedEnumerable<>).MakeGenericType(new Type[] { elementType })))
                    {
                        throw EntityUtil.NotSupported(System.Data.Entity.Strings.Materializer_UnsupportedCollectionType(arg.RequestedType));
                    }
                    Type type = typeof(CompensatingCollection<>).MakeGenericType(new Type[] { elementType });
                    ConstructorInfo constructor = type.GetConstructors()[0];
                    expression5 = Emit_EnsureType(Expression.New(constructor, new Expression[] { expression5 }), type);
                }
            }
            this.ExitCoodinatorTranslateScope();
            return new CollectionTranslatorResult(expression5, columnMap, arg.RequestedType, instance);
        }

        internal void RegisterUserExpression(Expression expression)
        {
            this._userExpressions.Add(expression);
        }

        private EdmType ResolveSpanType(EdmType edmType)
        {
            EdmType elementType = edmType;
            BuiltInTypeKind builtInTypeKind = elementType.BuiltInTypeKind;
            if (builtInTypeKind != BuiltInTypeKind.CollectionType)
            {
                if (builtInTypeKind != BuiltInTypeKind.RowType)
                {
                    return elementType;
                }
            }
            else
            {
                elementType = this.ResolveSpanType(((CollectionType) elementType).TypeUsage.EdmType);
                if (elementType != null)
                {
                    elementType = new CollectionType(elementType);
                }
                return elementType;
            }
            RowType spanRowType = (RowType) elementType;
            if ((this._spanIndex != null) && this._spanIndex.HasSpanMap(spanRowType))
            {
                elementType = spanRowType.Members[0].TypeUsage.EdmType;
            }
            return elementType;
        }

        private static void TestCompile<T>(Expression expression)
        {
            Expression.Lambda<Func<T>>(expression, new ParameterExpression[0]).Compile();
        }

        internal static ShaperFactory<TRequestedType> TranslateColumnMap<TRequestedType>(QueryCacheManager queryCacheManager, ColumnMap columnMap, MetadataWorkspace workspace, SpanIndex spanIndex, MergeOption mergeOption, bool valueLayer)
        {
            ShaperFactory<TRequestedType> target;
            ShaperFactoryQueryCacheKey<TRequestedType> key = new ShaperFactoryQueryCacheKey<TRequestedType>(ColumnMapKeyBuilder.GetColumnMapKey(columnMap, spanIndex), mergeOption, valueLayer);
            if (!queryCacheManager.TryCacheLookup<TRequestedType>(key, out target))
            {
                System.Data.Common.Internal.Materialization.Translator visitor = new System.Data.Common.Internal.Materialization.Translator(workspace, spanIndex, mergeOption, valueLayer);
                columnMap.Accept<TranslatorResult, TranslatorArg>(visitor, new TranslatorArg(typeof(IEnumerable<>).MakeGenericType(new Type[] { typeof(TRequestedType) })));
                visitor.VerifyUserExpressions();
                CoordinatorFactory<TRequestedType> rootCoordinatorFactory = (CoordinatorFactory<TRequestedType>) visitor._rootCoordinatorScratchpad.Compile();
                Action checkPermissionsDelegate = visitor.GetCheckPermissionsDelegate();
                target = new ShaperFactory<TRequestedType>(visitor._stateSlotCount, rootCoordinatorFactory, checkPermissionsDelegate, mergeOption);
                QueryCacheEntry inQueryCacheEntry = new ShaperFactoryQueryCacheEntry<TRequestedType>(key, target);
                if (queryCacheManager.TryLookupAndAdd(inQueryCacheEntry, out inQueryCacheEntry))
                {
                    target = ((ShaperFactoryQueryCacheEntry<TRequestedType>) inQueryCacheEntry).GetTarget();
                }
            }
            return target;
        }

        private Expression<Func<Shaper, T>> TypedCreateInlineDelegate<T>(Expression body)
        {
            Expression<Func<Shaper, T>> expression = Expression.Lambda<Func<Shaper, T>>(body, new ParameterExpression[] { Shaper_Parameter });
            this._currentCoordinatorScratchpad.AddInlineDelegate(expression);
            return expression;
        }

        private void VerifyUserExpressions()
        {
            foreach (Expression expression in this._userExpressions)
            {
                Translator_TestCompile.MakeGenericMethod(new Type[] { expression.Type }).Invoke(null, new object[] { expression });
            }
        }

        internal override TranslatorResult Visit(ComplexTypeColumnMap columnMap, TranslatorArg arg)
        {
            Expression ifFalse = null;
            Expression nullCheckExpression = null;
            if (columnMap.NullSentinel != null)
            {
                nullCheckExpression = Emit_Reader_IsDBNull(columnMap.NullSentinel);
            }
            if (this.IsValueLayer)
            {
                ifFalse = this.BuildExpressionToGetRecordState(columnMap, null, null, nullCheckExpression);
            }
            else
            {
                ComplexType edmType = (ComplexType) columnMap.Type.EdmType;
                Type type = this.DetermineClrType(edmType);
                ConstructorInfo constructor = this.GetConstructor(type);
                List<MemberBinding> bindings = this.CreatePropertyBindings(columnMap, type, edmType.Properties);
                ifFalse = Expression.MemberInit(Expression.New(constructor), bindings);
                if (nullCheckExpression != null)
                {
                    ifFalse = Expression.Condition(nullCheckExpression, Emit_NullConstant(ifFalse.Type), ifFalse);
                }
            }
            return new TranslatorResult(ifFalse, arg.RequestedType);
        }

        internal override TranslatorResult Visit(DiscriminatedCollectionColumnMap columnMap, TranslatorArg arg) => 
            this.ProcessCollectionColumnMap(columnMap, arg, columnMap.Discriminator, columnMap.DiscriminatorValue);

        internal override TranslatorResult Visit(EntityColumnMap columnMap, TranslatorArg arg)
        {
            Expression expression;
            EntityIdentity entityIdentity = columnMap.EntityIdentity;
            Expression entitySetReader = null;
            Expression entityKeyReader = Emit_EntityKey_ctor(this, entityIdentity, false, out entitySetReader);
            if (this.IsValueLayer)
            {
                Expression nullCheckExpression = Expression.Not(Emit_EntityKey_HasValue(entityIdentity.Keys));
                expression = this.BuildExpressionToGetRecordState(columnMap, entityKeyReader, entitySetReader, nullCheckExpression);
            }
            else
            {
                Expression body = null;
                EntityType edmType = (EntityType) columnMap.Type.EdmType;
                Type type = this.DetermineClrType(edmType);
                ConstructorInfo constructor = this.GetConstructor(type);
                List<MemberBinding> bindings = this.CreatePropertyBindings(columnMap, type, edmType.Properties);
                bool flag = typeof(IEntityWithKey).IsAssignableFrom(type);
                bool flag2 = typeof(IEntityWithRelationships).IsAssignableFrom(type);
                if (flag)
                {
                    MemberBinding item = Expression.Bind(IEntityKeyWithKey_EntityKey, entityKeyReader);
                    bindings.Add(item);
                }
                body = Expression.MemberInit(Expression.New(constructor), bindings);
                if (flag2)
                {
                    body = Expression.Call(Shaper_Parameter, Shaper_HandleIEntityWithRelationships.MakeGenericMethod(new Type[] { type }), new Expression[] { body, entitySetReader });
                }
                if (MergeOption.NoTracking != this._mergeOption)
                {
                    if (flag && (this._mergeOption != MergeOption.AppendOnly))
                    {
                        body = Expression.Call(Shaper_Parameter, Shaper_HandleIEntityWithKey.MakeGenericMethod(new Type[] { type }), new Expression[] { body, entitySetReader });
                    }
                    else if (this._mergeOption == MergeOption.AppendOnly)
                    {
                        LambdaExpression expression6 = this.CreateInlineDelegate(body);
                        body = Expression.Call(Shaper_Parameter, Shaper_HandleEntityAppendOnly.MakeGenericMethod(new Type[] { type }), new Expression[] { expression6, entityKeyReader, entitySetReader });
                    }
                    else
                    {
                        body = Expression.Call(Shaper_Parameter, Shaper_HandleEntity.MakeGenericMethod(new Type[] { type }), new Expression[] { body, entityKeyReader, entitySetReader });
                    }
                }
                expression = Expression.Condition(Emit_EntityKey_HasValue(entityIdentity.Keys), body, Emit_NullConstant(type));
            }
            return new TranslatorResult(expression, arg.RequestedType);
        }

        internal override TranslatorResult Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, TranslatorArg arg) => 
            new TranslatorResult((Expression) Translator_MultipleDiscriminatorPolymorphicColumnMapHelper.MakeGenericMethod(new Type[] { arg.RequestedType }).Invoke(this, new object[] { columnMap, arg }), arg.RequestedType);

        internal override TranslatorResult Visit(RecordColumnMap columnMap, TranslatorArg arg)
        {
            Expression ifFalse = null;
            Expression nullCheckExpression = null;
            if (columnMap.NullSentinel != null)
            {
                nullCheckExpression = Emit_Reader_IsDBNull(columnMap.NullSentinel);
            }
            if (this.IsValueLayer)
            {
                ifFalse = this.BuildExpressionToGetRecordState(columnMap, null, null, nullCheckExpression);
            }
            else
            {
                InitializerMetadata metadata;
                if (InitializerMetadata.TryGetInitializerMetadata(columnMap.Type, out metadata))
                {
                    ifFalse = this.HandleLinqRecord(columnMap, metadata);
                }
                else
                {
                    RowType edmType = (RowType) columnMap.Type.EdmType;
                    if ((this._spanIndex != null) && this._spanIndex.HasSpanMap(edmType))
                    {
                        ifFalse = this.HandleSpandexRecord(columnMap, arg, edmType);
                    }
                    else
                    {
                        ifFalse = this.HandleRegularRecord(columnMap, arg, edmType);
                    }
                }
                if (nullCheckExpression != null)
                {
                    ifFalse = Expression.Condition(nullCheckExpression, Emit_NullConstant(ifFalse.Type), ifFalse);
                }
            }
            return new TranslatorResult(ifFalse, arg.RequestedType);
        }

        internal override TranslatorResult Visit(RefColumnMap columnMap, TranslatorArg arg)
        {
            Expression expression;
            EntityIdentity entityIdentity = columnMap.EntityIdentity;
            return new TranslatorResult(Expression.Condition(Emit_EntityKey_HasValue(entityIdentity.Keys), Emit_EntityKey_ctor(this, entityIdentity, true, out expression), Expression.Constant(null, typeof(EntityKey))), arg.RequestedType);
        }

        internal override TranslatorResult Visit(ScalarColumnMap columnMap, TranslatorArg arg)
        {
            bool flag;
            Type requestedType = arg.RequestedType;
            int columnPos = columnMap.ColumnPos;
            MethodInfo readerMethod = GetReaderMethod(requestedType, out flag);
            Expression ifFalse = Emit_Reader_GetXXX(columnPos, requestedType, readerMethod);
            if (flag)
            {
                ifFalse = Expression.Condition(Emit_Reader_IsDBNull(columnPos), Expression.Constant(TypeSystem.GetDefaultValue(arg.RequestedType), arg.RequestedType), ifFalse);
            }
            Expression expressionWithErrorHandling = Emit_Shaper_GetColumnValueWithErrorHandling(arg.RequestedType, columnPos);
            this._currentCoordinatorScratchpad.AddExpressionWithErrorHandling(ifFalse, expressionWithErrorHandling);
            return new TranslatorResult(ifFalse, arg.RequestedType);
        }

        internal override TranslatorResult Visit(SimpleCollectionColumnMap columnMap, TranslatorArg arg) => 
            this.ProcessCollectionColumnMap(columnMap, arg);

        internal override TranslatorResult Visit(SimplePolymorphicColumnMap columnMap, TranslatorArg arg)
        {
            Expression expression;
            Expression right = AcceptWithMappedType(this, columnMap.TypeDiscriminator, columnMap).Expression;
            if (this.IsValueLayer)
            {
                expression = Emit_EnsureType(this.BuildExpressionToGetRecordState(columnMap, null, null, Expression.Constant(true)), arg.RequestedType);
            }
            else
            {
                expression = Emit_NullConstant(arg.RequestedType);
            }
            foreach (KeyValuePair<object, TypedColumnMap> pair in columnMap.TypeChoices)
            {
                if (!this.DetermineClrType(pair.Value.Type).IsAbstract)
                {
                    Expression expression4;
                    Expression left = Expression.Constant(pair.Key, right.Type);
                    if (right.Type == typeof(string))
                    {
                        expression4 = Expression.Call(Expression.Constant(TrailingSpaceStringComparer.Instance), IEqualityComparerOfString_Equals, new Expression[] { left, right });
                    }
                    else
                    {
                        expression4 = Emit_Equal(left, right);
                    }
                    expression = Expression.Condition(expression4, pair.Value.Accept<TranslatorResult, TranslatorArg>(this, arg).Expression, expression);
                }
            }
            return new TranslatorResult(expression, arg.RequestedType);
        }

        internal override TranslatorResult Visit(VarRefColumnMap columnMap, TranslatorArg arg)
        {
            throw EntityUtil.InvalidOperation(string.Empty);
        }
    }
}

