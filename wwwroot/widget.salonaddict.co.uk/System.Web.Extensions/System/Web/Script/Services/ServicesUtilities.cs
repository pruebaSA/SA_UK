namespace System.Web.Script.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    internal static class ServicesUtilities
    {
        internal static string GetClientTypeFromServerType(WebServiceData webServiceData, Type type)
        {
            if (webServiceData.ClientTypeNameDictionary.ContainsKey(type))
            {
                return webServiceData.ClientTypeNameDictionary[type];
            }
            if (type.IsEnum)
            {
                return GetClientTypeName(type.FullName);
            }
            if ((type == typeof(string)) || (type == typeof(char)))
            {
                return "String";
            }
            if (type.IsPrimitive)
            {
                if (type == typeof(bool))
                {
                    return "Boolean";
                }
                return "Number";
            }
            if (type.IsValueType)
            {
                if (type == typeof(DateTime))
                {
                    return "Date";
                }
                if (type == typeof(Guid))
                {
                    return "String";
                }
                if (type == typeof(decimal))
                {
                    return "Number";
                }
            }
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return "Object";
            }
            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type;
                if (!type.IsGenericTypeDefinition)
                {
                    genericTypeDefinition = type.GetGenericTypeDefinition();
                }
                if (genericTypeDefinition == typeof(IDictionary<,>))
                {
                    return "Object";
                }
            }
            if (!type.IsArray && !typeof(IEnumerable).IsAssignableFrom(type))
            {
                return "";
            }
            return "Array";
        }

        internal static string GetClientTypeName(string name) => 
            name.Replace('+', '_');

        internal static Type UnwrapNullableType(Type type)
        {
            if ((type.IsGenericType && !type.IsGenericTypeDefinition) && (type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        internal static string XmlSerializeObjectToString(object obj)
        {
            string str;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream w = new MemoryStream();
            using (XmlTextWriter writer = new XmlTextWriter(w, Encoding.UTF8))
            {
                serializer.Serialize((XmlWriter) writer, obj);
                w.Position = 0L;
                using (StreamReader reader = new StreamReader(w))
                {
                    str = reader.ReadToEnd();
                }
            }
            return str;
        }
    }
}

