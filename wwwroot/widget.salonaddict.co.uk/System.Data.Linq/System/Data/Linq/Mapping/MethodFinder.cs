namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;

    internal static class MethodFinder
    {
        internal static MethodInfo FindMethod(Type type, string name, BindingFlags flags, Type[] argTypes) => 
            FindMethod(type, name, flags, argTypes, true);

        internal static MethodInfo FindMethod(Type type, string name, BindingFlags flags, Type[] argTypes, bool allowInherit)
        {
            while (type != typeof(object))
            {
                MethodInfo info = type.GetMethod(name, flags | BindingFlags.DeclaredOnly, null, argTypes, null);
                if ((info != null) || !allowInherit)
                {
                    return info;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}

