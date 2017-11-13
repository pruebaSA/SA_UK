namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class TypeSystem
    {
        private static readonly MethodInfo s_getDefaultMethod = typeof(TypeSystem).GetMethod("GetDefault", BindingFlags.NonPublic | BindingFlags.Static);

        private static Type FindIEnumerable(Type seqType)
        {
            if (((seqType != null) && (seqType != typeof(string))) && (seqType != typeof(byte[])))
            {
                if (seqType.IsArray)
                {
                    return typeof(IEnumerable<>).MakeGenericType(new Type[] { seqType.GetElementType() });
                }
                if (seqType.IsGenericType)
                {
                    foreach (Type type in seqType.GetGenericArguments())
                    {
                        Type type2 = typeof(IEnumerable<>).MakeGenericType(new Type[] { type });
                        if (type2.IsAssignableFrom(seqType))
                        {
                            return type2;
                        }
                    }
                }
                Type[] interfaces = seqType.GetInterfaces();
                if ((interfaces != null) && (interfaces.Length > 0))
                {
                    foreach (Type type3 in interfaces)
                    {
                        Type type4 = FindIEnumerable(type3);
                        if (type4 != null)
                        {
                            return type4;
                        }
                    }
                }
                if ((seqType.BaseType != null) && (seqType.BaseType != typeof(object)))
                {
                    return FindIEnumerable(seqType.BaseType);
                }
            }
            return null;
        }

        private static T GetDefault<T>() => 
            default(T);

        internal static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType || (type.IsGenericType && (typeof(Nullable<>) == type.GetGenericTypeDefinition())))
            {
                return null;
            }
            return s_getDefaultMethod.MakeGenericMethod(new Type[] { type }).Invoke(null, new object[0]);
        }

        internal static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        internal static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static bool IsNullableType(Type type) => 
            (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        internal static bool IsSequenceType(Type seqType) => 
            (FindIEnumerable(seqType) != null);

        internal static MemberInfo PropertyOrField(MemberInfo member, out string name, out Type type)
        {
            name = null;
            type = null;
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo info = (FieldInfo) member;
                name = info.Name;
                type = info.FieldType;
                return info;
            }
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo info2 = (PropertyInfo) member;
                if (info2.GetIndexParameters().Length != 0)
                {
                    throw EntityUtil.NotSupported(Strings.ELinq_PropertyIndexNotSupported);
                }
                name = info2.Name;
                type = info2.PropertyType;
                return info2;
            }
            if (member.MemberType == MemberTypes.Method)
            {
                MethodInfo info3 = (MethodInfo) member;
                if (info3.IsSpecialName)
                {
                    foreach (PropertyInfo info4 in info3.DeclaringType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                    {
                        if (info4.CanRead && (info4.GetGetMethod(true) == info3))
                        {
                            return PropertyOrField(info4, out name, out type);
                        }
                    }
                }
            }
            throw EntityUtil.NotSupported(Strings.ELinq_NotPropertyOrField(member.Name));
        }
    }
}

