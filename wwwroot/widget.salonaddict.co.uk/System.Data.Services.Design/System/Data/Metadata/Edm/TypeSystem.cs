namespace System.Data.Metadata.Edm
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeSystem
    {
        private static readonly MethodInfo s_getDefaultMethod = typeof(TypeSystem).GetMethod("GetDefault", BindingFlags.NonPublic | BindingFlags.Static);

        private static T GetDefault<T>() => 
            default(T);

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType || (type.IsGenericType && (typeof(Nullable<>) == type.GetGenericTypeDefinition())))
            {
                return null;
            }
            return s_getDefaultMethod.MakeGenericMethod(new Type[] { type }).Invoke(null, new object[0]);
        }
    }
}

