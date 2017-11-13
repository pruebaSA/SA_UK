namespace System.Data.Objects
{
    using System;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    internal static class LightweightCodeGenerator
    {
        internal static readonly NamedPermissionSet FullTrustPermission = new NamedPermissionSet("FullTrust");
        internal static readonly ReflectionPermission MemberAccessReflectionPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

        private static Delegate CreateConstructor(Type type)
        {
            ConstructorInfo constructorForType = GetConstructorForType(type);
            DynamicMethod method = CreateDynamicMethod(constructorForType.DeclaringType.Name, typeof(object), Type.EmptyTypes);
            ILGenerator iLGenerator = method.GetILGenerator();
            GenerateNecessaryPermissionDemands(iLGenerator, constructorForType);
            iLGenerator.Emit(OpCodes.Newobj, constructorForType);
            iLGenerator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<object>));
        }

        [SecurityCritical, SecurityTreatAsSafe, ReflectionPermission(SecurityAction.Assert, MemberAccess=true, ReflectionEmit=true)]
        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes) => 
            new DynamicMethod(name, returnType, parameterTypes, typeof(LightweightCodeGenerator).Module, true);

        private static Delegate CreateEntityConstructor(Type type)
        {
            ConstructorInfo constructorForType = GetConstructorForType(type);
            DynamicMethod method = CreateDynamicMethod(constructorForType.DeclaringType.Name, typeof(object), new Type[] { typeof(EntityKey), typeof(ObjectContext), typeof(EntitySet), typeof(MergeOption) });
            ILGenerator iLGenerator = method.GetILGenerator();
            GenerateNecessaryPermissionDemands(iLGenerator, constructorForType);
            iLGenerator.Emit(OpCodes.Newobj, constructorForType);
            bool flag = typeof(IEntityWithKey).IsAssignableFrom(type);
            bool flag2 = typeof(IEntityWithRelationships).IsAssignableFrom(type);
            if (flag || flag2)
            {
                Label label = iLGenerator.DefineLabel();
                iLGenerator.Emit(OpCodes.Ldarg_0);
                iLGenerator.Emit(OpCodes.Brfalse_S, label);
                if (flag)
                {
                    iLGenerator.Emit(OpCodes.Dup);
                    iLGenerator.Emit(OpCodes.Castclass, typeof(IEntityWithKey));
                    iLGenerator.Emit(OpCodes.Ldarg_0);
                    iLGenerator.Emit(OpCodes.Callvirt, typeof(IEntityWithKey).GetMethod("set_EntityKey", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(EntityKey) }, null));
                }
                if (flag2)
                {
                    Label label2 = iLGenerator.DefineLabel();
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    iLGenerator.Emit(OpCodes.Brfalse_S, label2);
                    iLGenerator.Emit(OpCodes.Dup);
                    iLGenerator.Emit(OpCodes.Castclass, typeof(IEntityWithRelationships));
                    iLGenerator.Emit(OpCodes.Ldarg_1);
                    iLGenerator.Emit(OpCodes.Ldarg_2);
                    iLGenerator.Emit(OpCodes.Ldarg_3);
                    iLGenerator.Emit(OpCodes.Call, typeof(EntityUtil).GetMethod("AttachContext", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(IEntityWithRelationships), typeof(ObjectContext), typeof(EntitySet), typeof(MergeOption) }, null));
                    iLGenerator.MarkLabel(label2);
                }
                iLGenerator.MarkLabel(label);
            }
            iLGenerator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Func<EntityKey, ObjectContext, EntitySet, MergeOption, object>));
        }

        private static RelationshipManager.GetRelatedEndMethod CreateGetRelatedEndMethod(AssociationEndMember sourceMember, AssociationEndMember targetMember, RelatedEnd existingRelatedEnd)
        {
            Type clrType = MetadataHelper.GetEntityTypeForEnd(sourceMember).ClrType;
            Type type4 = MetadataHelper.GetEntityTypeForEnd(targetMember).ClrType;
            MethodInfo meth = null;
            switch (targetMember.RelationshipMultiplicity)
            {
                case RelationshipMultiplicity.ZeroOrOne:
                case RelationshipMultiplicity.One:
                {
                    MethodInfo info2 = typeof(RelationshipManager).GetMethod("GetRelatedReference", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(RelationshipMultiplicity), typeof(RelatedEnd) }, null);
                    if (info2 == null)
                    {
                        throw EntityUtil.MissingMethod("GetRelatedReference");
                    }
                    meth = info2.MakeGenericMethod(new Type[] { clrType, type4 });
                    break;
                }
                case RelationshipMultiplicity.Many:
                {
                    MethodInfo info3 = typeof(RelationshipManager).GetMethod("GetRelatedCollection", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(RelationshipMultiplicity), typeof(RelatedEnd) }, null);
                    if (info3 == null)
                    {
                        throw EntityUtil.MissingMethod("GetRelatedCollection");
                    }
                    meth = info3.MakeGenericMethod(new Type[] { clrType, type4 });
                    break;
                }
                default:
                    throw EntityUtil.InvalidEnumerationValue(typeof(RelationshipMultiplicity), (int) targetMember.RelationshipMultiplicity);
            }
            DynamicMethod method = CreateDynamicMethod("GetRelatedEnd", typeof(IRelatedEnd), new Type[] { typeof(RelationshipManager), typeof(RelatedEnd) });
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, meth.DeclaringType);
            iLGenerator.Emit(OpCodes.Ldstr, sourceMember.DeclaringType.FullName);
            iLGenerator.Emit(OpCodes.Ldstr, sourceMember.Name);
            iLGenerator.Emit(OpCodes.Ldstr, targetMember.Name);
            switch (sourceMember.RelationshipMultiplicity)
            {
                case RelationshipMultiplicity.ZeroOrOne:
                    iLGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;

                case RelationshipMultiplicity.One:
                    iLGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;

                case RelationshipMultiplicity.Many:
                    iLGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;

                default:
                    throw EntityUtil.InvalidEnumerationValue(typeof(RelationshipMultiplicity), (int) sourceMember.RelationshipMultiplicity);
            }
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Call, meth);
            iLGenerator.Emit(OpCodes.Ret);
            return (RelationshipManager.GetRelatedEndMethod) method.CreateDelegate(typeof(RelationshipManager.GetRelatedEndMethod));
        }

        private static Func<object, object> CreatePropertyGetter(RuntimeMethodHandle rmh)
        {
            MethodInfo mi = (new RuntimeMethodHandle() != rmh) ? ((MethodInfo) MethodBase.GetMethodFromHandle(rmh)) : null;
            if (mi == null)
            {
                ThrowPropertyNoGetter();
            }
            if (mi.IsStatic)
            {
                ThrowPropertyIsStatic();
            }
            if (mi.DeclaringType.IsValueType)
            {
                ThrowPropertyDeclaringTypeIsValueType();
            }
            if (mi.GetParameters().Length != 0)
            {
                ThrowPropertyIsIndexed();
            }
            Type returnType = mi.ReturnType;
            if ((returnType == null) || (typeof(void) == returnType))
            {
                ThrowPropertyUnsupportedForm();
            }
            if (returnType.IsPointer)
            {
                ThrowPropertyUnsupportedType();
            }
            DynamicMethod method = CreateDynamicMethod(mi.Name, typeof(object), new Type[] { typeof(object) });
            ILGenerator iLGenerator = method.GetILGenerator();
            GenerateNecessaryPermissionDemands(iLGenerator, mi);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, mi.DeclaringType);
            iLGenerator.Emit(mi.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, mi);
            if (returnType.IsValueType)
            {
                Type type2;
                if (returnType.IsGenericType && (typeof(Nullable<>) == returnType.GetGenericTypeDefinition()))
                {
                    type2 = returnType.GetGenericArguments()[0];
                    Label label = iLGenerator.DefineLabel();
                    LocalBuilder local = iLGenerator.DeclareLocal(returnType);
                    iLGenerator.Emit(OpCodes.Stloc_S, local);
                    iLGenerator.Emit(OpCodes.Ldloca_S, local);
                    iLGenerator.Emit(OpCodes.Call, returnType.GetMethod("get_HasValue"));
                    iLGenerator.Emit(OpCodes.Brfalse_S, label);
                    iLGenerator.Emit(OpCodes.Ldloca_S, local);
                    iLGenerator.Emit(OpCodes.Call, returnType.GetMethod("get_Value"));
                    iLGenerator.Emit(OpCodes.Box, type2 = returnType.GetGenericArguments()[0]);
                    iLGenerator.Emit(OpCodes.Ret);
                    iLGenerator.MarkLabel(label);
                    iLGenerator.Emit(OpCodes.Ldnull);
                }
                else
                {
                    type2 = returnType;
                    iLGenerator.Emit(OpCodes.Box, type2);
                }
            }
            iLGenerator.Emit(OpCodes.Ret);
            return (Func<object, object>) method.CreateDelegate(typeof(Func<object, object>));
        }

        private static Action<object, object> CreatePropertySetter(RuntimeMethodHandle rmh, bool allowNull)
        {
            MethodInfo info;
            Type type;
            ValidateSetterProperty(rmh, out info, out type);
            DynamicMethod method = CreateDynamicMethod(info.Name, typeof(void), new Type[] { typeof(object), typeof(object) });
            ILGenerator iLGenerator = method.GetILGenerator();
            GenerateNecessaryPermissionDemands(iLGenerator, info);
            Type cls = type;
            Label label = iLGenerator.DefineLabel();
            Label label2 = iLGenerator.DefineLabel();
            Label label3 = iLGenerator.DefineLabel();
            if (type.IsValueType)
            {
                if (type.IsGenericType && (typeof(Nullable<>) == type.GetGenericTypeDefinition()))
                {
                    cls = type.GetGenericArguments()[0];
                }
                else
                {
                    allowNull = false;
                }
            }
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Castclass, info.DeclaringType);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Isinst, cls);
            if (allowNull)
            {
                iLGenerator.Emit(OpCodes.Ldarg_1);
                if (cls == type)
                {
                    iLGenerator.Emit(OpCodes.Brfalse_S, label);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Brtrue, label2);
                    iLGenerator.Emit(OpCodes.Pop);
                    LocalBuilder local = iLGenerator.DeclareLocal(type);
                    iLGenerator.Emit(OpCodes.Ldloca_S, local);
                    iLGenerator.Emit(OpCodes.Initobj, type);
                    iLGenerator.Emit(OpCodes.Ldloc_0);
                    iLGenerator.Emit(OpCodes.Br_S, label);
                    iLGenerator.MarkLabel(label2);
                }
            }
            iLGenerator.Emit(OpCodes.Dup);
            iLGenerator.Emit(OpCodes.Brfalse_S, label3);
            if (cls.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox_Any, cls);
                if (cls != type)
                {
                    iLGenerator.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { cls }));
                }
            }
            iLGenerator.MarkLabel(label);
            iLGenerator.Emit(info.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, info);
            iLGenerator.Emit(OpCodes.Ret);
            iLGenerator.MarkLabel(label3);
            iLGenerator.Emit(OpCodes.Pop);
            iLGenerator.Emit(OpCodes.Pop);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Ldtoken, cls);
            iLGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static));
            iLGenerator.Emit(OpCodes.Ldstr, info.DeclaringType.Name);
            iLGenerator.Emit(OpCodes.Ldstr, info.Name.Substring(4));
            iLGenerator.Emit(OpCodes.Call, typeof(EntityUtil).GetMethod("ThrowSetInvalidValue", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(object), typeof(Type), typeof(string), typeof(string) }, null));
            iLGenerator.Emit(OpCodes.Ret);
            return (Action<object, object>) method.CreateDelegate(typeof(Action<object, object>));
        }

        private static void GenerateNecessaryPermissionDemands(ILGenerator gen, MethodBase mi)
        {
            if (HasLinkDemand(mi))
            {
                gen.Emit(OpCodes.Ldsfld, typeof(LightweightCodeGenerator).GetField("FullTrustPermission", BindingFlags.NonPublic | BindingFlags.Static));
                gen.Emit(OpCodes.Callvirt, typeof(NamedPermissionSet).GetMethod("Demand"));
            }
            else if (!IsPublic(mi))
            {
                gen.Emit(OpCodes.Ldsfld, typeof(LightweightCodeGenerator).GetField("MemberAccessReflectionPermission", BindingFlags.NonPublic | BindingFlags.Static));
                gen.Emit(OpCodes.Callvirt, typeof(ReflectionPermission).GetMethod("Demand"));
            }
        }

        internal static Delegate GetConstructorDelegateForType(ClrComplexType clrType) => 
            (clrType.Constructor ?? (clrType.Constructor = CreateConstructor(clrType.ClrType)));

        internal static Delegate GetConstructorDelegateForType(ClrEntityType clrType) => 
            (clrType.Constructor ?? (clrType.Constructor = CreateEntityConstructor(clrType.ClrType)));

        internal static Delegate GetConstructorDelegateForType(EdmType clrType)
        {
            if (!Helper.IsEntityType(clrType))
            {
                return GetConstructorDelegateForType((ClrComplexType) clrType);
            }
            return GetConstructorDelegateForType((ClrEntityType) clrType);
        }

        internal static ConstructorInfo GetConstructorForType(Type type)
        {
            ConstructorInfo info = type.GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (info == null)
            {
                ThrowConstructorNoParameterless();
            }
            return info;
        }

        internal static Func<object, object> GetGetterDelegateForProperty(EdmProperty property) => 
            (property.ValueGetter ?? (property.ValueGetter = CreatePropertyGetter(property.PropertyGetterHandle)));

        internal static IRelatedEnd GetRelatedEnd(RelationshipManager sourceRelationshipManager, AssociationEndMember sourceMember, AssociationEndMember targetMember, RelatedEnd existingRelatedEnd)
        {
            RelationshipManager.GetRelatedEndMethod getRelatedEnd = sourceMember.GetRelatedEnd as RelationshipManager.GetRelatedEndMethod;
            return getRelatedEnd?.Invoke(sourceRelationshipManager, existingRelatedEnd);
        }

        internal static Action<object, object> GetSetterDelegateForProperty(EdmProperty property)
        {
            Action<object, object> valueSetter = property.ValueSetter;
            if (valueSetter == null)
            {
                valueSetter = CreatePropertySetter(property.PropertySetterHandle, property.Nullable);
                property.ValueSetter = valueSetter;
            }
            return valueSetter;
        }

        internal static object GetValue(EdmProperty property, object target) => 
            GetGetterDelegateForProperty(property)(target);

        internal static object GetValue(NavigationProperty property, object target)
        {
            Func<object, object> valueGetter = property.ValueGetter;
            return valueGetter?.Invoke(target);
        }

        internal static bool HasLinkDemand(MemberInfo info)
        {
            bool flag = false;
            foreach (SecurityAttribute attribute in info.GetCustomAttributes(typeof(SecurityAttribute), false).Concat<object>(info.DeclaringType.GetCustomAttributes(typeof(SecurityAttribute), false)))
            {
                if (attribute is StrongNameIdentityPermissionAttribute)
                {
                    ThrowPropertyStrongNameIdentity();
                }
                if (SecurityAction.LinkDemand == attribute.Action)
                {
                    flag = true;
                }
            }
            return flag;
        }

        internal static bool IsPublic(MethodBase method) => 
            (method.IsPublic && IsPublic(method.DeclaringType));

        internal static bool IsPublic(Type type) => 
            ((type == null) || (type.IsPublic && IsPublic(type.DeclaringType)));

        internal static bool RequiresPermissionDemands(MethodBase mi)
        {
            if (IsPublic(mi))
            {
                return HasLinkDemand(mi);
            }
            return true;
        }

        internal static void SetValue(EdmProperty property, object target, object value)
        {
            Action<object, object> setterDelegateForProperty = GetSetterDelegateForProperty(property);
            if (Bid.TraceOn)
            {
                EntityBid.Trace("<dobj.LightweightCodeGenerator.SetValue|INFO> Name='%ls'\n", property.Name);
            }
            setterDelegateForProperty(target, value);
        }

        private static void ThrowConstructorNoParameterless()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_ConstructorNoParameterless);
        }

        private static void ThrowPropertyDeclaringTypeIsValueType()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyDeclaringTypeIsValueType);
        }

        private static void ThrowPropertyIsIndexed()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyIsIndexed);
        }

        private static void ThrowPropertyIsStatic()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyIsStatic);
        }

        private static void ThrowPropertyNoGetter()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyNoGetter);
        }

        private static void ThrowPropertyNoSetter()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyNoSetter);
        }

        private static void ThrowPropertyStrongNameIdentity()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyStrongNameIdentity);
        }

        private static void ThrowPropertyUnsupportedForm()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyUnsupportedForm);
        }

        private static void ThrowPropertyUnsupportedType()
        {
            throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.CodeGen_PropertyUnsupportedType);
        }

        internal static void ValidateSetterProperty(RuntimeMethodHandle setterMethodHandle, out MethodInfo setterMethodInfo, out Type realType)
        {
            setterMethodInfo = (new RuntimeMethodHandle() != setterMethodHandle) ? ((MethodInfo) MethodBase.GetMethodFromHandle(setterMethodHandle)) : null;
            if (setterMethodInfo == null)
            {
                ThrowPropertyNoSetter();
            }
            if (setterMethodInfo.IsStatic)
            {
                ThrowPropertyIsStatic();
            }
            if (setterMethodInfo.DeclaringType.IsValueType)
            {
                ThrowPropertyDeclaringTypeIsValueType();
            }
            ParameterInfo[] parameters = setterMethodInfo.GetParameters();
            if ((parameters == null) || (1 != parameters.Length))
            {
                ThrowPropertyIsIndexed();
            }
            realType = setterMethodInfo.ReturnType;
            if ((realType != null) && (typeof(void) != realType))
            {
                ThrowPropertyUnsupportedForm();
            }
            realType = parameters[0].ParameterType;
            if (realType.IsPointer)
            {
                ThrowPropertyUnsupportedType();
            }
        }
    }
}

