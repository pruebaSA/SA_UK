namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class TypeSystem
    {
        private static ILookup<string, MethodInfo> _queryMethods;
        private static ILookup<string, MethodInfo> _sequenceMethods;

        private static bool ArgsMatchExact(MethodInfo m, Type[] argTypes, Type[] typeArgs)
        {
            ParameterInfo[] parameters = m.GetParameters();
            if (parameters.Length != argTypes.Length)
            {
                return false;
            }
            if ((!m.IsGenericMethodDefinition && m.IsGenericMethod) && m.ContainsGenericParameters)
            {
                m = m.GetGenericMethodDefinition();
            }
            if (m.IsGenericMethodDefinition)
            {
                if ((typeArgs == null) || (typeArgs.Length == 0))
                {
                    return false;
                }
                if (m.GetGenericArguments().Length != typeArgs.Length)
                {
                    return false;
                }
                m = m.MakeGenericMethod(typeArgs);
                parameters = m.GetParameters();
            }
            else if ((typeArgs != null) && (typeArgs.Length > 0))
            {
                return false;
            }
            int index = 0;
            int length = argTypes.Length;
            while (index < length)
            {
                Type parameterType = parameters[index].ParameterType;
                if (parameterType == null)
                {
                    return false;
                }
                Type c = argTypes[index];
                if (!parameterType.IsAssignableFrom(c))
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if ((seqType != null) && (seqType != typeof(string)))
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

        internal static MethodInfo FindQueryableMethod(string name, Type[] args, params Type[] typeArgs)
        {
            if (_queryMethods == null)
            {
                _queryMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).ToLookup<MethodInfo, string>(m => m.Name);
            }
            MethodInfo info = _queryMethods[name].FirstOrDefault<MethodInfo>(m => ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                throw System.Data.Linq.SqlClient.Error.NoMethodInTypeMatchingArguments(typeof(Queryable));
            }
            if (typeArgs != null)
            {
                return info.MakeGenericMethod(typeArgs);
            }
            return info;
        }

        internal static MethodInfo FindSequenceMethod(string name, IEnumerable sequence) => 
            FindSequenceMethod(name, new Type[] { sequence.GetType() }, new Type[] { GetElementType(sequence.GetType()) });

        internal static MethodInfo FindSequenceMethod(string name, Type[] args, params Type[] typeArgs)
        {
            if (_sequenceMethods == null)
            {
                _sequenceMethods = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).ToLookup<MethodInfo, string>(m => m.Name);
            }
            MethodInfo info = _sequenceMethods[name].FirstOrDefault<MethodInfo>(m => ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                return null;
            }
            if (typeArgs != null)
            {
                return info.MakeGenericMethod(typeArgs);
            }
            return info;
        }

        internal static MethodInfo FindStaticMethod(Type type, string name, Type[] args, params Type[] typeArgs)
        {
            MethodInfo info = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).FirstOrDefault<MethodInfo>(m => (m.Name == name) && ArgsMatchExact(m, args, typeArgs));
            if (info == null)
            {
                throw System.Data.Linq.SqlClient.Error.NoMethodInTypeMatchingArguments(type);
            }
            if (typeArgs != null)
            {
                return info.MakeGenericMethod(typeArgs);
            }
            return info;
        }

        internal static IEnumerable<FieldInfo> GetAllFields(Type type, BindingFlags flags)
        {
            Dictionary<MetaPosition, FieldInfo> dictionary = new Dictionary<MetaPosition, FieldInfo>();
            Type baseType = type;
            do
            {
                foreach (FieldInfo info in baseType.GetFields(flags))
                {
                    if (info.IsPrivate || (type == baseType))
                    {
                        MetaPosition position = new MetaPosition(info);
                        dictionary[position] = info;
                    }
                }
                baseType = baseType.BaseType;
            }
            while (baseType != null);
            return dictionary.Values;
        }

        internal static IEnumerable<PropertyInfo> GetAllProperties(Type type, BindingFlags flags)
        {
            Dictionary<MetaPosition, PropertyInfo> dictionary = new Dictionary<MetaPosition, PropertyInfo>();
            Type baseType = type;
            do
            {
                foreach (PropertyInfo info in baseType.GetProperties(flags))
                {
                    if ((type == baseType) || IsPrivate(info))
                    {
                        MetaPosition position = new MetaPosition(info);
                        dictionary[position] = info;
                    }
                }
                baseType = baseType.BaseType;
            }
            while (baseType != null);
            return dictionary.Values;
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

        internal static Type GetFlatSequenceType(Type elementType)
        {
            Type type = FindIEnumerable(elementType);
            if (type != null)
            {
                return type;
            }
            return typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });
        }

        internal static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo info = mi as FieldInfo;
            if (info != null)
            {
                return info.FieldType;
            }
            PropertyInfo info2 = mi as PropertyInfo;
            if (info2 != null)
            {
                return info2.PropertyType;
            }
            EventInfo info3 = mi as EventInfo;
            if (info3 != null)
            {
                return info3.EventHandlerType;
            }
            return null;
        }

        internal static Type GetNonNullableType(Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static Type GetSequenceType(Type elementType) => 
            typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });

        internal static bool HasIEnumerable(Type seqType) => 
            (FindIEnumerable(seqType) != null);

        internal static bool IsNullableType(Type type) => 
            (((type != null) && type.IsGenericType) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));

        internal static bool IsNullAssignable(Type type)
        {
            if (type.IsValueType)
            {
                return IsNullableType(type);
            }
            return true;
        }

        private static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo info = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (info != null)
            {
                return info.IsPrivate;
            }
            return true;
        }

        internal static bool IsSequenceType(Type seqType) => 
            ((((seqType != typeof(string)) && (seqType != typeof(byte[]))) && (seqType != typeof(char[]))) && (FindIEnumerable(seqType) != null));

        internal static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                type = type.GetGenericArguments()[0];
            }
            if (type.IsEnum)
            {
                return true;
            }
            if (type == typeof(Guid))
            {
                return true;
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    return ((typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type));

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
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
            }
            return false;
        }
    }
}

