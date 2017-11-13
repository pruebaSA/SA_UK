namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Xml;

    public class JsonQueryStringConverter : QueryStringConverter
    {
        private DataContractSerializerOperationBehavior dataContractSerializerOperationBehavior;
        private OperationDescription operationDescription;

        public JsonQueryStringConverter()
        {
        }

        internal JsonQueryStringConverter(OperationDescription operationDescription)
        {
            if (operationDescription == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operationDescription");
            }
            this.operationDescription = operationDescription;
            this.dataContractSerializerOperationBehavior = this.operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>();
        }

        public override bool CanConvert(Type type)
        {
            XsdDataContractExporter exporter = new XsdDataContractExporter();
            return exporter.CanExport(type);
        }

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameterType == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parameterType");
            }
            switch (Type.GetTypeCode(parameterType))
            {
                case TypeCode.Boolean:
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
                    return base.ConvertStringToValue(parameter, parameterType);

                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.String:
                    if (!this.IsFirstCharacterReservedCharacter(parameter, '"'))
                    {
                        return base.ConvertStringToValue(parameter, parameterType);
                    }
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
            }
            if (parameterType == typeof(Guid))
            {
                if (parameter == null)
                {
                    return new Guid();
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '"'))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(Uri))
            {
                if (parameter == null)
                {
                    return null;
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '"'))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(TimeSpan))
            {
                if (parameter == null)
                {
                    return new TimeSpan();
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '"'))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(byte[]))
            {
                if (parameter == null)
                {
                    return null;
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '['))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(DateTimeOffset))
            {
                if (parameter == null)
                {
                    return new DateTimeOffset();
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '{'))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameterType == typeof(object))
            {
                if (parameter == null)
                {
                    return null;
                }
                if (this.IsFirstCharacterReservedCharacter(parameter, '{'))
                {
                    return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
                }
                return base.ConvertStringToValue(parameter, parameterType);
            }
            if (parameter == null)
            {
                return null;
            }
            return this.CreateJsonDeserializedObject(parameter.Trim(), parameterType);
        }

        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameter == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream();
            XmlDictionaryWriter writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8);
            this.GetDataContractJsonSerializer(parameterType).WriteObject(writer, parameter);
            writer.Flush();
            stream.Seek(0L, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(stream.GetBuffer(), (int) stream.Position, (int) stream.Length);
        }

        private object CreateJsonDeserializedObject(string parameter, Type parameterType)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(parameter);
            XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(bytes, 0, bytes.Length, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null);
            return this.GetDataContractJsonSerializer(parameterType).ReadObject(reader);
        }

        private DataContractJsonSerializer GetDataContractJsonSerializer(Type parameterType)
        {
            if (this.operationDescription == null)
            {
                return new DataContractJsonSerializer(parameterType);
            }
            if (this.dataContractSerializerOperationBehavior == null)
            {
                return new DataContractJsonSerializer(parameterType, this.operationDescription.KnownTypes);
            }
            return new DataContractJsonSerializer(parameterType, this.operationDescription.KnownTypes, this.dataContractSerializerOperationBehavior.maxItemsInObjectGraph, this.dataContractSerializerOperationBehavior.IgnoreExtensionDataObject, this.dataContractSerializerOperationBehavior.DataContractSurrogate, false);
        }

        private bool IsFirstCharacterReservedCharacter(string parameter, char reservedCharacter)
        {
            if (parameter == null)
            {
                return false;
            }
            string str = parameter.Trim();
            if (str == string.Empty)
            {
                return false;
            }
            return (str[0] == reservedCharacter);
        }
    }
}

