namespace System.Drawing
{
    using System;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;

    internal static class SecurityUtils
    {
        internal static object SecureConstructorInvoke(Type type, Type[] argTypes, object[] args, bool allowNonPublic) => 
            SecureConstructorInvoke(type, argTypes, args, allowNonPublic, BindingFlags.Default);

        internal static object SecureConstructorInvoke(Type type, Type[] argTypes, object[] args, bool allowNonPublic, BindingFlags extraFlags)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            BindingFlags bindingAttr = (BindingFlags.Public | BindingFlags.Instance) | extraFlags;
            if (type.Assembly == typeof(System.Drawing.SecurityUtils).Assembly)
            {
                if (!type.IsPublic && !type.IsNestedPublic)
                {
                    new ReflectionPermission(PermissionState.Unrestricted).Demand();
                }
                else if (allowNonPublic && !HasReflectionPermission)
                {
                    allowNonPublic = false;
                }
            }
            if (allowNonPublic)
            {
                bindingAttr |= BindingFlags.NonPublic;
            }
            ConstructorInfo info = type.GetConstructor(bindingAttr, null, argTypes, null);
            if (info != null)
            {
                return info.Invoke(args);
            }
            return null;
        }

        internal static object SecureCreateInstance(Type type) => 
            SecureCreateInstance(type, null);

        internal static object SecureCreateInstance(Type type, object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (((type.Assembly == typeof(System.Drawing.SecurityUtils).Assembly) && !type.IsPublic) && !type.IsNestedPublic)
            {
                new ReflectionPermission(PermissionState.Unrestricted).Demand();
            }
            return Activator.CreateInstance(type, args);
        }

        internal static object SecureCreateInstance(Type type, object[] args, bool allowNonPublic)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            BindingFlags bindingAttr = BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance;
            if (type.Assembly == typeof(System.Drawing.SecurityUtils).Assembly)
            {
                if (!type.IsPublic && !type.IsNestedPublic)
                {
                    new ReflectionPermission(PermissionState.Unrestricted).Demand();
                }
                else if (allowNonPublic && !HasReflectionPermission)
                {
                    allowNonPublic = false;
                }
            }
            if (allowNonPublic)
            {
                bindingAttr |= BindingFlags.NonPublic;
            }
            return Activator.CreateInstance(type, bindingAttr, null, args, null);
        }

        private static bool HasReflectionPermission
        {
            get
            {
                try
                {
                    new ReflectionPermission(PermissionState.Unrestricted).Demand();
                    return true;
                }
                catch (SecurityException)
                {
                }
                return false;
            }
        }
    }
}

