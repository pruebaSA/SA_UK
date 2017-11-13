namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.Xml;

    public class QueryStringConverter
    {
        private Hashtable defaultSupportedQueryStringTypes = new Hashtable();
        private Hashtable typeConverterCache;

        public QueryStringConverter()
        {
            this.defaultSupportedQueryStringTypes.Add(typeof(byte), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(sbyte), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(short), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(int), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(long), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(ushort), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(uint), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(ulong), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(float), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(double), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(bool), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(char), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(decimal), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(string), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(object), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(DateTime), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(TimeSpan), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(byte[]), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(Guid), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(Uri), null);
            this.defaultSupportedQueryStringTypes.Add(typeof(DateTimeOffset), null);
            this.typeConverterCache = new Hashtable();
        }

        public virtual bool CanConvert(Type type) => 
            (this.defaultSupportedQueryStringTypes.ContainsKey(type) || (typeof(Enum).IsAssignableFrom(type) || (this.GetStringConverter(type) != null)));

        public virtual object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameterType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
            }
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Boolean:
                    return ((parameter == null) ? ((object) 0) : ((object) Convert.ToBoolean(parameter, CultureInfo.InvariantCulture)));

                case TypeCode.Char:
                    return ((parameter == null) ? 0 : XmlConvert.ToChar(parameter));

                case TypeCode.SByte:
                    return ((parameter == null) ? 0 : XmlConvert.ToSByte(parameter));

                case TypeCode.Byte:
                    return ((parameter == null) ? 0 : XmlConvert.ToByte(parameter));

                case TypeCode.Int16:
                    return ((parameter == null) ? 0 : XmlConvert.ToInt16(parameter));

                case TypeCode.UInt16:
                    return ((parameter == null) ? 0 : XmlConvert.ToUInt16(parameter));

                case TypeCode.Int32:
                    if (!typeof(Enum).IsAssignableFrom(parameterType))
                    {
                        return ((parameter == null) ? 0 : XmlConvert.ToInt32(parameter));
                    }
                    return Enum.Parse(parameterType, parameter, true);

                case TypeCode.UInt32:
                    return ((parameter == null) ? ((object) 0) : ((object) XmlConvert.ToUInt32(parameter)));

                case TypeCode.Int64:
                    return ((parameter == null) ? 0L : XmlConvert.ToInt64(parameter));

                case TypeCode.UInt64:
                    return ((parameter == null) ? ((object) 0L) : ((object) XmlConvert.ToUInt64(parameter)));

                case TypeCode.Single:
                    return ((parameter == null) ? 0f : XmlConvert.ToSingle(parameter));

                case TypeCode.Double:
                    return ((parameter == null) ? 0.0 : XmlConvert.ToDouble(parameter));

                case TypeCode.Decimal:
                    return ((parameter == null) ? 0M : XmlConvert.ToDecimal(parameter));

                case TypeCode.DateTime:
                    return ((parameter == null) ? new DateTime() : DateTime.Parse(parameter, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

                case TypeCode.String:
                    return parameter;
            }
            if (parameterType == typeof(TimeSpan))
            {
                TimeSpan span;
                if (!TimeSpan.TryParse(parameter, out span))
                {
                    span = (parameter == null) ? new TimeSpan() : XmlConvert.ToTimeSpan(parameter);
                }
                return span;
            }
            if (parameterType == typeof(Guid))
            {
                return ((parameter == null) ? new Guid() : XmlConvert.ToGuid(parameter));
            }
            if (parameterType == typeof(DateTimeOffset))
            {
                return ((parameter == null) ? new DateTimeOffset() : DateTimeOffset.Parse(parameter, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces));
            }
            if (parameterType == typeof(byte[]))
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    return new byte[0];
                }
                return Convert.FromBase64String(parameter);
            }
            if (parameterType == typeof(Uri))
            {
                if (string.IsNullOrEmpty(parameter))
                {
                    return null;
                }
                return new Uri(parameter, UriKind.RelativeOrAbsolute);
            }
            if (parameterType == typeof(object))
            {
                return parameter;
            }
            return this.GetStringConverter(parameterType)?.ConvertFromInvariantString(parameter);
        }

        public virtual string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameterType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
            }
            if (parameterType.IsValueType && (parameter == null))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameter");
            }
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Boolean:
                    return XmlConvert.ToString((bool) parameter);

                case TypeCode.Char:
                    return XmlConvert.ToString((char) parameter);

                case TypeCode.SByte:
                    return XmlConvert.ToString((sbyte) parameter);

                case TypeCode.Byte:
                    return XmlConvert.ToString((byte) parameter);

                case TypeCode.Int16:
                    return XmlConvert.ToString((short) parameter);

                case TypeCode.UInt16:
                    return XmlConvert.ToString((ushort) parameter);

                case TypeCode.Int32:
                    if (!typeof(Enum).IsAssignableFrom(parameterType))
                    {
                        return XmlConvert.ToString((int) parameter);
                    }
                    return Enum.Format(parameterType, parameter, "G");

                case TypeCode.UInt32:
                    return XmlConvert.ToString((uint) parameter);

                case TypeCode.Int64:
                    return XmlConvert.ToString((long) parameter);

                case TypeCode.UInt64:
                    return XmlConvert.ToString((ulong) parameter);

                case TypeCode.Single:
                    return XmlConvert.ToString((float) parameter);

                case TypeCode.Double:
                    return XmlConvert.ToString((double) parameter);

                case TypeCode.Decimal:
                    return XmlConvert.ToString((decimal) parameter);

                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime) parameter, XmlDateTimeSerializationMode.RoundtripKind);

                case TypeCode.String:
                    return (string) parameter;
            }
            if (parameterType == typeof(TimeSpan))
            {
                return XmlConvert.ToString((TimeSpan) parameter);
            }
            if (parameterType == typeof(Guid))
            {
                return XmlConvert.ToString((Guid) parameter);
            }
            if (parameterType == typeof(DateTimeOffset))
            {
                return XmlConvert.ToString((DateTimeOffset) parameter);
            }
            if (parameterType == typeof(byte[]))
            {
                if (parameter == null)
                {
                    return null;
                }
                return Convert.ToBase64String((byte[]) parameter, Base64FormattingOptions.None);
            }
            if ((parameterType == typeof(Uri)) || (parameterType == typeof(object)))
            {
                if (parameter == null)
                {
                    return null;
                }
                return Convert.ToString(parameter, CultureInfo.InvariantCulture);
            }
            return this.GetStringConverter(parameterType)?.ConvertToInvariantString(parameter);
        }

        private TypeConverter GetStringConverter(Type parameterType)
        {
            if (this.typeConverterCache.ContainsKey(parameterType))
            {
                return (TypeConverter) this.typeConverterCache[parameterType];
            }
            TypeConverterAttribute[] customAttributes = parameterType.GetCustomAttributes(typeof(TypeConverterAttribute), true) as TypeConverterAttribute[];
            if (customAttributes != null)
            {
                foreach (TypeConverterAttribute attribute in customAttributes)
                {
                    Type type = Type.GetType(attribute.ConverterTypeName, false, true);
                    if (type != null)
                    {
                        TypeConverter converter = null;
                        Exception exception = null;
                        try
                        {
                            converter = (TypeConverter) Activator.CreateInstance(type);
                        }
                        catch (TargetInvocationException exception2)
                        {
                            exception = exception2;
                        }
                        catch (MemberAccessException exception3)
                        {
                            exception = exception3;
                        }
                        catch (TypeLoadException exception4)
                        {
                            exception = exception4;
                        }
                        catch (COMException exception5)
                        {
                            exception = exception5;
                        }
                        catch (InvalidComObjectException exception6)
                        {
                            exception = exception6;
                        }
                        finally
                        {
                            if (exception != null)
                            {
                                if (DiagnosticUtility.IsFatal(exception))
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
                                }
                                if (DiagnosticUtility.ShouldTraceWarning)
                                {
                                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                                }
                            }
                        }
                        if (((converter != null) && converter.CanConvertTo(typeof(string))) && converter.CanConvertFrom(typeof(string)))
                        {
                            this.typeConverterCache.Add(parameterType, converter);
                            return converter;
                        }
                    }
                }
            }
            return null;
        }
    }
}

