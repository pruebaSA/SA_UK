namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Linq.SqlClient;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Permissions;
    using System.Xml.Linq;

    public static class DBConvert
    {
        private static Type[] StringArg = new Type[] { typeof(string) };

        public static T ChangeType<T>(object value) => 
            ((T) ChangeType(value, typeof(T)));

        public static object ChangeType(object value, Type type)
        {
            object obj3;
            if (value == null)
            {
                return null;
            }
            type = TypeSystem.GetNonNullableType(type);
            Type enumType = value.GetType();
            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }
            if (type == typeof(Binary))
            {
                byte[] buffer;
                if (enumType == typeof(byte[]))
                {
                    return new Binary((byte[]) value);
                }
                if (enumType == typeof(Guid))
                {
                    Guid guid = (Guid) value;
                    return new Binary(guid.ToByteArray());
                }
                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, value);
                    buffer = stream.ToArray();
                }
                return new Binary(buffer);
            }
            if (type == typeof(byte[]))
            {
                if (enumType == typeof(Binary))
                {
                    return ((Binary) value).ToArray();
                }
                if (enumType == typeof(Guid))
                {
                    Guid guid2 = (Guid) value;
                    return guid2.ToByteArray();
                }
                BinaryFormatter formatter2 = new BinaryFormatter();
                using (MemoryStream stream2 = new MemoryStream())
                {
                    formatter2.Serialize(stream2, value);
                    return stream2.ToArray();
                }
            }
            if (enumType == typeof(byte[]))
            {
                if (type == typeof(Guid))
                {
                    return new Guid((byte[]) value);
                }
                BinaryFormatter formatter3 = new BinaryFormatter();
                using (MemoryStream stream3 = new MemoryStream((byte[]) value))
                {
                    return ChangeType(formatter3.Deserialize(stream3), type);
                }
            }
            if (enumType == typeof(Binary))
            {
                if (type == typeof(Guid))
                {
                    return new Guid(((Binary) value).ToArray());
                }
                BinaryFormatter formatter4 = new BinaryFormatter();
                using (MemoryStream stream4 = new MemoryStream(((Binary) value).ToArray(), false))
                {
                    return ChangeType(formatter4.Deserialize(stream4), type);
                }
            }
            if (type.IsEnum)
            {
                if (enumType == typeof(string))
                {
                    string str = ((string) value).Trim();
                    return Enum.Parse(type, str);
                }
                return Enum.ToObject(type, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture));
            }
            if (enumType.IsEnum)
            {
                if (type == typeof(string))
                {
                    return Enum.GetName(enumType, value);
                }
                return Convert.ChangeType(Convert.ChangeType(value, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture), type, CultureInfo.InvariantCulture);
            }
            if (type == typeof(TimeSpan))
            {
                if (enumType == typeof(string))
                {
                    return TimeSpan.Parse((string) value);
                }
                if (enumType == typeof(DateTime))
                {
                    return DateTime.Parse(value.ToString(), CultureInfo.InvariantCulture).TimeOfDay;
                }
                if (enumType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(value.ToString(), CultureInfo.InvariantCulture).TimeOfDay;
                }
                return new TimeSpan((long) Convert.ChangeType(value, typeof(long), CultureInfo.InvariantCulture));
            }
            if (enumType == typeof(TimeSpan))
            {
                if (type == typeof(string))
                {
                    return value.ToString();
                }
                if (type == typeof(DateTime))
                {
                    DateTime time = new DateTime();
                    return time.Add((TimeSpan) value);
                }
                if (type == typeof(DateTimeOffset))
                {
                    DateTimeOffset offset = new DateTimeOffset();
                    return offset.Add((TimeSpan) value);
                }
                TimeSpan span = (TimeSpan) value;
                return Convert.ChangeType(span.Ticks, type, CultureInfo.InvariantCulture);
            }
            if ((type == typeof(DateTime)) && (enumType == typeof(DateTimeOffset)))
            {
                DateTimeOffset offset3 = (DateTimeOffset) value;
                return offset3.DateTime;
            }
            if ((type == typeof(DateTimeOffset)) && (enumType == typeof(DateTime)))
            {
                return new DateTimeOffset((DateTime) value);
            }
            if ((type == typeof(string)) && !typeof(IConvertible).IsAssignableFrom(enumType))
            {
                if (enumType == typeof(char[]))
                {
                    return new string((char[]) value);
                }
                return value.ToString();
            }
            if (enumType == typeof(string))
            {
                MethodInfo info;
                if (type == typeof(Guid))
                {
                    return new Guid((string) value);
                }
                if (type == typeof(char[]))
                {
                    return ((string) value).ToCharArray();
                }
                if ((type == typeof(XDocument)) && (((string) value) == string.Empty))
                {
                    return new XDocument();
                }
                if (!typeof(IConvertible).IsAssignableFrom(type) && ((info = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, StringArg, null)) != null))
                {
                    if (!type.IsVisible)
                    {
                        new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
                    }
                    try
                    {
                        return info.Invoke(null, new object[] { value });
                    }
                    catch (TargetInvocationException exception)
                    {
                        throw exception.GetBaseException();
                    }
                }
                return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            if ((type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IQueryable<>))) && typeof(IEnumerable<>).MakeGenericType(new Type[] { type.GetGenericArguments()[0] }).IsAssignableFrom(enumType))
            {
                return ((IEnumerable) value).AsQueryable();
            }
            try
            {
                obj3 = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            catch (InvalidCastException)
            {
                throw System.Data.Linq.Error.CouldNotConvert(value.GetType(), type);
            }
            return obj3;
        }
    }
}

