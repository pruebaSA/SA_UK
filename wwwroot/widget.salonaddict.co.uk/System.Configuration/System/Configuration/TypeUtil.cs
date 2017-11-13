namespace System.Configuration
{
    using System;
    using System.Configuration.Internal;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Web;

    internal static class TypeUtil
    {
        private static readonly AspNetHostingPermission s_aspNetHostingPermission = new AspNetHostingPermission(AspNetHostingPermissionLevel.Minimal);
        private static PermissionSet s_fullTrustPermissionSet;
        private static readonly ReflectionPermission s_memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);

        private static bool CallerHasMemberAccessOrAspNetPermission()
        {
            try
            {
                s_memberAccessPermission.Demand();
                return true;
            }
            catch (SecurityException)
            {
            }
            try
            {
                s_aspNetHostingPermission.Demand();
                return true;
            }
            catch (SecurityException)
            {
            }
            return false;
        }

        internal static Delegate CreateDelegateRestricted(Type callingType, Type delegateType, MethodInfo targetMethod)
        {
            if (CallerHasMemberAccessOrAspNetPermission())
            {
                return Delegate.CreateDelegate(delegateType, targetMethod);
            }
            DynamicMethod method = CreateDynamicMethod(callingType, typeof(Delegate), new Type[] { typeof(Type), typeof(MethodInfo) });
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Call, typeof(Delegate).GetMethod("CreateDelegate", new Type[] { typeof(Type), typeof(MethodInfo) }));
            iLGenerator.Emit(OpCodes.Ret);
            CreateDelegateInvoker invoker = (CreateDelegateInvoker) method.CreateDelegate(typeof(CreateDelegateInvoker));
            return invoker(delegateType, targetMethod);
        }

        private static DynamicMethod CreateDynamicMethod(Type owner, Type returnType, Type[] parameterTypes)
        {
            if (owner != null)
            {
                return CreateDynamicMethodWithUnrestrictedPermission(owner, returnType, parameterTypes);
            }
            return new DynamicMethod("temp-dynamic-method", returnType, parameterTypes);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private static DynamicMethod CreateDynamicMethodWithUnrestrictedPermission(Type owner, Type returnType, Type[] parameterTypes) => 
            new DynamicMethod("temp-dynamic-method", returnType, parameterTypes, owner);

        internal static T CreateInstance<T>(string typeString) => 
            CreateInstanceRestricted<T>(null, typeString);

        internal static T CreateInstanceRestricted<T>(Type callingType, string typeString)
        {
            Type typeImpl = GetTypeImpl(typeString, true);
            VerifyAssignableType(typeof(T), typeImpl, true);
            return (T) CreateInstanceRestricted(callingType, typeImpl);
        }

        internal static object CreateInstanceRestricted(Type callingType, Type targetType)
        {
            if (CallerHasMemberAccessOrAspNetPermission())
            {
                return CreateInstanceWithReflectionPermission(targetType);
            }
            DynamicMethod method = CreateDynamicMethod(callingType, typeof(object), new Type[] { typeof(Type) });
            ILGenerator iLGenerator = method.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldc_I4_1);
            iLGenerator.Emit(OpCodes.Call, typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type), typeof(bool) }));
            iLGenerator.Emit(OpCodes.Ret);
            CreateInstanceInvoker invoker = (CreateInstanceInvoker) method.CreateDelegate(typeof(CreateInstanceInvoker));
            return invoker(targetType);
        }

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.MemberAccess | ReflectionPermissionFlag.TypeInformation)]
        internal static object CreateInstanceWithReflectionPermission(Type type) => 
            Activator.CreateInstance(type, true);

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.TypeInformation)]
        internal static ConstructorInfo GetConstructorWithReflectionPermission(Type type, Type baseType, bool throwOnError)
        {
            type = VerifyAssignableType(baseType, type, throwOnError);
            if (type == null)
            {
                return null;
            }
            BindingFlags bindingAttr = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            ConstructorInfo info = type.GetConstructor(bindingAttr, null, CallingConventions.HasThis, Type.EmptyTypes, null);
            if ((info == null) && throwOnError)
            {
                throw new TypeLoadException(System.Configuration.SR.GetString("TypeNotPublic", new object[] { type.AssemblyQualifiedName }));
            }
            return info;
        }

        private static Type GetLegacyType(string typeString)
        {
            Type type = null;
            try
            {
                type = typeof(ConfigurationException).Assembly.GetType(typeString, false);
            }
            catch
            {
            }
            return type;
        }

        private static Type GetTypeImpl(string typeString, bool throwOnError)
        {
            Type legacyType = null;
            Exception exception = null;
            try
            {
                legacyType = Type.GetType(typeString, throwOnError);
            }
            catch (Exception exception2)
            {
                exception = exception2;
            }
            if (legacyType == null)
            {
                legacyType = GetLegacyType(typeString);
                if ((legacyType == null) && (exception != null))
                {
                    throw exception;
                }
            }
            return legacyType;
        }

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.TypeInformation)]
        internal static Type GetTypeWithReflectionPermission(string typeString, bool throwOnError) => 
            GetTypeImpl(typeString, throwOnError);

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.TypeInformation)]
        internal static Type GetTypeWithReflectionPermission(IInternalConfigHost host, string typeString, bool throwOnError)
        {
            Type configType = null;
            Exception exception = null;
            try
            {
                configType = host.GetConfigType(typeString, throwOnError);
            }
            catch (Exception exception2)
            {
                exception = exception2;
            }
            if (configType == null)
            {
                configType = GetLegacyType(typeString);
                if ((configType == null) && (exception != null))
                {
                    throw exception;
                }
            }
            return configType;
        }

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.MemberAccess | ReflectionPermissionFlag.TypeInformation)]
        private static bool HasAptcaBit(Assembly assembly)
        {
            object[] customAttributes = assembly.GetCustomAttributes(typeof(AllowPartiallyTrustedCallersAttribute), false);
            return ((customAttributes != null) && (customAttributes.Length > 0));
        }

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.MemberAccess | ReflectionPermissionFlag.TypeInformation)]
        internal static object InvokeCtorWithReflectionPermission(ConstructorInfo ctor) => 
            ctor.Invoke(null);

        internal static bool IsTypeAllowedInConfig(Type t)
        {
            if (IsCallerFullTrust)
            {
                return true;
            }
            Assembly assembly = t.Assembly;
            return (!assembly.GlobalAssemblyCache || HasAptcaBit(assembly));
        }

        internal static bool IsTypeFromTrustedAssemblyWithoutAptca(Type type)
        {
            Assembly assembly = type.Assembly;
            return (assembly.GlobalAssemblyCache && !HasAptcaBit(assembly));
        }

        internal static Type VerifyAssignableType(Type baseType, Type type, bool throwOnError)
        {
            if (baseType.IsAssignableFrom(type))
            {
                return type;
            }
            if (throwOnError)
            {
                throw new TypeLoadException(System.Configuration.SR.GetString("Config_type_doesnt_inherit_from_type", new object[] { type.FullName, baseType.FullName }));
            }
            return null;
        }

        internal static bool IsCallerFullTrust
        {
            get
            {
                bool flag = false;
                try
                {
                    if (s_fullTrustPermissionSet == null)
                    {
                        s_fullTrustPermissionSet = new PermissionSet(PermissionState.Unrestricted);
                    }
                    s_fullTrustPermissionSet.Demand();
                    flag = true;
                }
                catch
                {
                }
                return flag;
            }
        }

        private delegate Delegate CreateDelegateInvoker(Type type, MethodInfo method);

        private delegate object CreateInstanceInvoker(Type type);
    }
}

