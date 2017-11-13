namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class TypeSystem
    {
        private static readonly Dictionary<MethodInfo, string> expressionMethodMap = new Dictionary<MethodInfo, string>(0x18, EqualityComparer<MethodInfo>.Default);
        private static readonly Dictionary<string, string> expressionVBMethodMap;
        private static readonly Dictionary<PropertyInfo, MethodInfo> propertiesAsMethodsMap;
        private const string VisualBasicAssemblyFullName = "Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        static TypeSystem()
        {
            expressionMethodMap.Add(typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), "substringof");
            expressionMethodMap.Add(typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), "endswith");
            expressionMethodMap.Add(typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), "startswith");
            expressionMethodMap.Add(typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) }), "indexof");
            expressionMethodMap.Add(typeof(string).GetMethod("Replace", new Type[] { typeof(string), typeof(string) }), "replace");
            expressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int) }), "substring");
            expressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }), "substring");
            expressionMethodMap.Add(typeof(string).GetMethod("ToLower", Type.EmptyTypes), "tolower");
            expressionMethodMap.Add(typeof(string).GetMethod("ToUpper", Type.EmptyTypes), "toupper");
            expressionMethodMap.Add(typeof(string).GetMethod("Trim", Type.EmptyTypes), "trim");
            expressionMethodMap.Add(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }, null), "concat");
            expressionMethodMap.Add(typeof(string).GetProperty("Length", typeof(int)).GetGetMethod(), "length");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod(), "day");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod(), "hour");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod(), "month");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod(), "minute");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod(), "second");
            expressionMethodMap.Add(typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod(), "year");
            expressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(double) }), "round");
            expressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(decimal) }), "round");
            expressionMethodMap.Add(typeof(Math).GetMethod("Floor", new Type[] { typeof(double) }), "floor");
            expressionMethodMap.Add(typeof(Math).GetMethod("Floor", new Type[] { typeof(decimal) }), "floor");
            expressionMethodMap.Add(typeof(Math).GetMethod("Ceiling", new Type[] { typeof(double) }), "ceiling");
            expressionMethodMap.Add(typeof(Math).GetMethod("Ceiling", new Type[] { typeof(decimal) }), "ceiling");
            expressionVBMethodMap = new Dictionary<string, string>(EqualityComparer<string>.Default);
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Trim", "trim");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Len", "length");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Mid", "substring");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.UCase", "toupper");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.LCase", "tolower");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Year", "year");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Month", "month");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Day", "day");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Hour", "hour");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Minute", "minute");
            expressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Second", "second");
            propertiesAsMethodsMap = new Dictionary<PropertyInfo, MethodInfo>(EqualityComparer<PropertyInfo>.Default);
            propertiesAsMethodsMap.Add(typeof(string).GetProperty("Length", typeof(int)), typeof(string).GetProperty("Length", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Day", typeof(int)), typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Hour", typeof(int)), typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Minute", typeof(int)), typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Second", typeof(int)), typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Month", typeof(int)), typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod());
            propertiesAsMethodsMap.Add(typeof(DateTime).GetProperty("Year", typeof(int)), typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod());
        }

        internal static Type FindIEnumerable(Type seqType)
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

        internal static Type GetElementType(Type seqType)
        {
            Type type = FindIEnumerable(seqType);
            if (type == null)
            {
                return seqType;
            }
            return type.GetGenericArguments()[0];
        }

        internal static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo info = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (info != null)
            {
                return info.IsPrivate;
            }
            return true;
        }

        internal static bool TryGetPropertyAsMethod(PropertyInfo pi, out MethodInfo mi) => 
            propertiesAsMethodsMap.TryGetValue(pi, out mi);

        internal static bool TryGetQueryOptionMethod(MethodInfo mi, out string methodName) => 
            (expressionMethodMap.TryGetValue(mi, out methodName) || ((mi.DeclaringType.Assembly.FullName == "Microsoft.VisualBasic, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") && expressionVBMethodMap.TryGetValue(mi.DeclaringType.FullName + "." + mi.Name, out methodName)));
    }
}

