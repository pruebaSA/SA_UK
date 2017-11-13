namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Xml;

    internal class DataContractJsonSerializerOperationFormatter : DataContractSerializerOperationFormatter
    {
        private bool isBareMessageContractReply;
        private bool isBareMessageContractRequest;
        private bool isWrapped;
        private bool useAspNetAjaxJson;

        public DataContractJsonSerializerOperationFormatter(OperationDescription description, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool isWrapped, bool useAspNetAjaxJson) : base(description, TypeLoader.DefaultDataContractFormatAttribute, new DataContractJsonSerializerOperationBehavior(description, maxItemsInObjectGraph, ignoreExtensionDataObject, dataContractSurrogate, useAspNetAjaxJson))
        {
            if (base.requestMessageInfo != null)
            {
                if (base.requestMessageInfo.WrapperName == null)
                {
                    this.isBareMessageContractRequest = true;
                }
                else
                {
                    base.requestMessageInfo.WrapperName = JsonGlobals.rootDictionaryString;
                    base.requestMessageInfo.WrapperNamespace = XmlDictionaryString.Empty;
                }
            }
            if (base.replyMessageInfo != null)
            {
                if (base.replyMessageInfo.WrapperName == null)
                {
                    this.isBareMessageContractReply = true;
                }
                else
                {
                    if (useAspNetAjaxJson)
                    {
                        base.replyMessageInfo.WrapperName = JsonGlobals.dDictionaryString;
                    }
                    else
                    {
                        base.replyMessageInfo.WrapperName = JsonGlobals.rootDictionaryString;
                    }
                    base.replyMessageInfo.WrapperNamespace = XmlDictionaryString.Empty;
                }
            }
            if ((base.requestStreamFormatter != null) && (base.requestStreamFormatter.WrapperName != null))
            {
                base.requestStreamFormatter.WrapperName = "root";
                base.requestStreamFormatter.WrapperNamespace = string.Empty;
            }
            if ((base.replyStreamFormatter != null) && (base.replyStreamFormatter.WrapperName != null))
            {
                base.replyStreamFormatter.WrapperName = "root";
                base.replyStreamFormatter.WrapperNamespace = string.Empty;
            }
            this.isWrapped = isWrapped;
            this.useAspNetAjaxJson = useAspNetAjaxJson;
        }

        protected override void AddHeadersToMessage(Message message, MessageDescription messageDescription, object[] parameters, bool isRequest)
        {
            if (message != null)
            {
                message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.JsonProperty);
            }
            base.AddHeadersToMessage(message, messageDescription, parameters, isRequest);
        }

        private object DeserializeBareMessageContract(XmlDictionaryReader reader, object[] parameters, bool isRequest)
        {
            DataContractSerializerOperationFormatter.MessageInfo requestMessageInfo;
            if (isRequest)
            {
                requestMessageInfo = base.requestMessageInfo;
            }
            else
            {
                requestMessageInfo = base.replyMessageInfo;
            }
            if (this.useAspNetAjaxJson && !isRequest)
            {
                this.ReadRootElement(reader);
                if (requestMessageInfo.BodyParts.Length == 0)
                {
                    this.ReadVoidReturn(reader);
                }
            }
            if (requestMessageInfo.BodyParts.Length > 0)
            {
                DataContractSerializerOperationFormatter.PartInfo info2 = requestMessageInfo.BodyParts[0];
                DataContractJsonSerializer serializer = info2.Serializer as DataContractJsonSerializer;
                if (this.useAspNetAjaxJson && !isRequest)
                {
                    serializer = RecreateDataContractJsonSerializer(serializer, "d");
                }
                else
                {
                    serializer = RecreateDataContractJsonSerializer(serializer, "root");
                }
                while (reader.IsStartElement())
                {
                    if (serializer.IsStartObject(reader))
                    {
                        try
                        {
                            parameters[info2.Description.Index] = serializer.ReadObject(reader, false);
                            break;
                        }
                        catch (InvalidOperationException exception)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { info2.Description.Namespace, info2.Description.Name }), exception));
                        }
                        catch (InvalidDataContractException exception2)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { info2.Description.Namespace, info2.Description.Name }), exception2));
                        }
                        catch (FormatException exception3)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { info2.Description.Namespace, info2.Description.Name, exception3.Message }), exception3));
                        }
                        catch (SerializationException exception4)
                        {
                            throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { info2.Description.Namespace, info2.Description.Name, exception4.Message }), exception4));
                        }
                    }
                    OperationFormatter.TraceAndSkipElement(reader);
                }
                while (reader.IsStartElement())
                {
                    OperationFormatter.TraceAndSkipElement(reader);
                }
            }
            if (this.useAspNetAjaxJson && !isRequest)
            {
                reader.ReadEndElement();
            }
            return null;
        }

        protected override object DeserializeBody(XmlDictionaryReader reader, MessageVersion version, string action, MessageDescription messageDescription, object[] parameters, bool isRequest)
        {
            if (reader == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("reader"));
            }
            if (parameters == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parameters"));
            }
            if (reader.EOF)
            {
                return null;
            }
            if ((isRequest && this.isBareMessageContractRequest) || (!isRequest && this.isBareMessageContractReply))
            {
                return this.DeserializeBareMessageContract(reader, parameters, isRequest);
            }
            object obj2 = null;
            if (isRequest || (this.isWrapped && !this.useAspNetAjaxJson))
            {
                this.ValidateTypeObjectAttribute(reader, isRequest);
                return this.DeserializeBodyCore(reader, parameters, isRequest);
            }
            if (this.useAspNetAjaxJson)
            {
                this.ReadRootElement(reader);
            }
            if (this.useAspNetAjaxJson && messageDescription.IsVoid)
            {
                this.ReadVoidReturn(reader);
            }
            else
            {
                if (base.replyMessageInfo.ReturnPart != null)
                {
                    DataContractSerializerOperationFormatter.PartInfo returnPart = base.replyMessageInfo.ReturnPart;
                    DataContractJsonSerializer serializer = returnPart.Serializer as DataContractJsonSerializer;
                    if (this.useAspNetAjaxJson)
                    {
                        serializer = RecreateDataContractJsonSerializer(serializer, "d");
                        this.VerifyIsStartElement(reader, "d");
                    }
                    else
                    {
                        serializer = RecreateDataContractJsonSerializer(serializer, "root");
                        this.VerifyIsStartElement(reader, "root");
                    }
                    if (!serializer.IsStartObject(reader))
                    {
                        goto Label_028D;
                    }
                    try
                    {
                        obj2 = serializer.ReadObject(reader, false);
                        goto Label_028D;
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { returnPart.Description.Namespace, returnPart.Description.Name }), exception));
                    }
                    catch (InvalidDataContractException exception2)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { returnPart.Description.Namespace, returnPart.Description.Name }), exception2));
                    }
                    catch (FormatException exception3)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { returnPart.Description.Namespace, returnPart.Description.Name, exception3.Message }), exception3));
                    }
                    catch (SerializationException exception4)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { returnPart.Description.Namespace, returnPart.Description.Name, exception4.Message }), exception4));
                    }
                }
                if (base.replyMessageInfo.BodyParts != null)
                {
                    this.ValidateTypeObjectAttribute(reader, isRequest);
                    obj2 = this.DeserializeBodyCore(reader, parameters, isRequest);
                }
            }
        Label_028D:
            while (reader.IsStartElement())
            {
                OperationFormatter.TraceAndSkipElement(reader);
            }
            if (this.useAspNetAjaxJson)
            {
                reader.ReadEndElement();
            }
            return obj2;
        }

        private object DeserializeBodyCore(XmlDictionaryReader reader, object[] parameters, bool isRequest)
        {
            DataContractSerializerOperationFormatter.MessageInfo requestMessageInfo;
            if (isRequest)
            {
                requestMessageInfo = base.requestMessageInfo;
            }
            else
            {
                requestMessageInfo = base.replyMessageInfo;
            }
            if (requestMessageInfo.WrapperName != null)
            {
                this.VerifyIsStartElement(reader, requestMessageInfo.WrapperName, requestMessageInfo.WrapperNamespace);
                bool isEmptyElement = reader.IsEmptyElement;
                reader.Read();
                if (isEmptyElement)
                {
                    return null;
                }
            }
            object returnValue = null;
            this.DeserializeParameters(reader, requestMessageInfo.BodyParts, parameters, requestMessageInfo.ReturnPart, ref returnValue);
            if (requestMessageInfo.WrapperName != null)
            {
                reader.ReadEndElement();
            }
            return returnValue;
        }

        private object DeserializeParameter(XmlDictionaryReader reader, DataContractSerializerOperationFormatter.PartInfo part)
        {
            if (!part.Description.Multiple)
            {
                return this.DeserializeParameterPart(reader, part);
            }
            ArrayList list = new ArrayList();
            while (part.Serializer.IsStartObject(reader))
            {
                list.Add(this.DeserializeParameterPart(reader, part));
            }
            return list.ToArray(part.Description.Type);
        }

        private object DeserializeParameterPart(XmlDictionaryReader reader, DataContractSerializerOperationFormatter.PartInfo part)
        {
            object obj2;
            XmlObjectSerializer serializer = part.Serializer;
            try
            {
                obj2 = serializer.ReadObject(reader, false);
            }
            catch (InvalidOperationException exception)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { part.Description.Namespace, part.Description.Name }), exception));
            }
            catch (InvalidDataContractException exception2)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameter", new object[] { part.Description.Namespace, part.Description.Name }), exception2));
            }
            catch (FormatException exception3)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { part.Description.Namespace, part.Description.Name, exception3.Message }), exception3));
            }
            catch (SerializationException exception4)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(OperationFormatter.CreateDeserializationFailedFault(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorDeserializingParameterMore", new object[] { part.Description.Namespace, part.Description.Name, exception4.Message }), exception4));
            }
            return obj2;
        }

        private void DeserializeParameters(XmlDictionaryReader reader, DataContractSerializerOperationFormatter.PartInfo[] parts, object[] parameters, DataContractSerializerOperationFormatter.PartInfo returnInfo, ref object returnValue)
        {
            bool[] flagArray = new bool[parameters.Length];
            bool flag = false;
            int num = 0;
            while (reader.IsStartElement())
            {
                bool flag2 = false;
                int num2 = 0;
                for (int j = num; num2 < parts.Length; j = (j + 1) % parts.Length)
                {
                    DataContractSerializerOperationFormatter.PartInfo part = parts[j];
                    if (part.Serializer.IsStartObject(reader))
                    {
                        num = num2;
                        parameters[part.Description.Index] = this.DeserializeParameter(reader, part);
                        flagArray[part.Description.Index] = true;
                        flag2 = true;
                    }
                    num2++;
                }
                if (!flag2)
                {
                    if (((returnInfo != null) && !flag) && returnInfo.Serializer.IsStartObject(reader))
                    {
                        returnValue = this.DeserializeParameter(reader, returnInfo);
                        flag = true;
                    }
                    else
                    {
                        OperationFormatter.TraceAndSkipElement(reader);
                    }
                }
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                if (!flagArray[i])
                {
                    parameters[i] = null;
                }
            }
        }

        protected override void GetHeadersFromMessage(Message message, MessageDescription messageDescription, object[] parameters, bool isRequest)
        {
            if (message != null)
            {
                object obj2;
                message.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj2);
                WebBodyFormatMessageProperty property = obj2 as WebBodyFormatMessageProperty;
                if (property == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.MessageFormatPropertyNotFound2, new object[] { base.OperationName })));
                }
                if (property.Format != WebContentFormat.Json)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.InvalidHttpMessageFormat3, new object[] { base.OperationName, property.Format, WebContentFormat.Json })));
                }
            }
            base.GetHeadersFromMessage(message, messageDescription, parameters, isRequest);
        }

        internal static bool IsJsonLocalName(XmlDictionaryReader reader, string elementName) => 
            ((reader.IsStartElement(JsonGlobals.itemDictionaryString, JsonGlobals.itemDictionaryString) && reader.MoveToAttribute("item")) && (reader.Value == elementName));

        internal static bool IsStartElement(XmlDictionaryReader reader, string elementName) => 
            (reader.IsStartElement(elementName) || IsJsonLocalName(reader, elementName));

        internal static bool IsStartElement(XmlDictionaryReader reader, XmlDictionaryString elementName, XmlDictionaryString elementNamespace) => 
            (reader.IsStartElement(elementName, elementNamespace) || IsJsonLocalName(reader, elementName?.Value));

        private void ReadRootElement(XmlDictionaryReader reader)
        {
            if (!IsStartElement(reader, "root"))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBody", new object[] { "root", string.Empty, reader.NodeType, reader.Name, reader.NamespaceURI })));
            }
            string attribute = reader.GetAttribute("type");
            if (!attribute.Equals("object", StringComparison.Ordinal))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR2.GetString(SR2.JsonFormatterExpectedAttributeObject, new object[] { attribute })));
            }
            bool isEmptyElement = reader.IsEmptyElement;
            reader.Read();
        }

        private void ReadVoidReturn(XmlDictionaryReader reader)
        {
            this.VerifyIsStartElement(reader, "d");
            string attribute = reader.GetAttribute("type");
            if (!attribute.Equals("null", StringComparison.Ordinal))
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR2.GetString(SR2.JsonFormatterExpectedAttributeNull, new object[] { attribute })));
            }
            OperationFormatter.TraceAndSkipElement(reader);
        }

        private static DataContractJsonSerializer RecreateDataContractJsonSerializer(DataContractJsonSerializer serializer, string newRootName) => 
            new DataContractJsonSerializer(serializer.GetDeserializeType(), newRootName, serializer.KnownTypes, serializer.MaxItemsInObjectGraph, serializer.IgnoreExtensionDataObject, serializer.DataContractSurrogate, serializer.AlwaysEmitTypeInformation);

        private void SerializeBareMessageContract(XmlDictionaryWriter writer, object[] parameters, bool isRequest)
        {
            DataContractSerializerOperationFormatter.MessageInfo requestMessageInfo;
            if (writer == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("writer"));
            }
            if (parameters == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parameters"));
            }
            if (isRequest)
            {
                requestMessageInfo = base.requestMessageInfo;
            }
            else
            {
                requestMessageInfo = base.replyMessageInfo;
            }
            if (this.useAspNetAjaxJson && !isRequest)
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
                if (requestMessageInfo.BodyParts.Length == 0)
                {
                    this.WriteVoidReturn(writer);
                }
            }
            if (requestMessageInfo.BodyParts.Length > 0)
            {
                DataContractSerializerOperationFormatter.PartInfo info2 = requestMessageInfo.BodyParts[0];
                DataContractJsonSerializer serializer = info2.Serializer as DataContractJsonSerializer;
                if (this.useAspNetAjaxJson && !isRequest)
                {
                    serializer = RecreateDataContractJsonSerializer(serializer, "d");
                }
                else
                {
                    serializer = RecreateDataContractJsonSerializer(serializer, "root");
                }
                object graph = parameters[info2.Description.Index];
                try
                {
                    serializer.WriteObject(writer, graph);
                }
                catch (SerializationException exception)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorSerializingParameter", new object[] { info2.Description.Namespace, info2.Description.Name, exception.Message }), exception));
                }
            }
            if (this.useAspNetAjaxJson && !isRequest)
            {
                writer.WriteEndElement();
            }
        }

        private void SerializeBody(XmlDictionaryWriter writer, object returnValue, object[] parameters, bool isRequest)
        {
            DataContractSerializerOperationFormatter.MessageInfo requestMessageInfo;
            if (writer == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("writer"));
            }
            if (parameters == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("parameters"));
            }
            if (isRequest)
            {
                requestMessageInfo = base.requestMessageInfo;
            }
            else
            {
                requestMessageInfo = base.replyMessageInfo;
            }
            if (requestMessageInfo.WrapperName != null)
            {
                writer.WriteStartElement(requestMessageInfo.WrapperName, requestMessageInfo.WrapperNamespace);
                writer.WriteAttributeString("type", "object");
            }
            if (requestMessageInfo.ReturnPart != null)
            {
                this.SerializeParameter(writer, requestMessageInfo.ReturnPart, returnValue);
            }
            this.SerializeParameters(writer, requestMessageInfo.BodyParts, parameters);
            if (requestMessageInfo.WrapperName != null)
            {
                writer.WriteEndElement();
            }
        }

        protected override void SerializeBody(XmlDictionaryWriter writer, MessageVersion version, string action, MessageDescription messageDescription, object returnValue, object[] parameters, bool isRequest)
        {
            if ((isRequest && this.isBareMessageContractRequest) || (!isRequest && this.isBareMessageContractReply))
            {
                this.SerializeBareMessageContract(writer, parameters, isRequest);
                return;
            }
            if (isRequest || (this.isWrapped && !this.useAspNetAjaxJson))
            {
                this.SerializeBody(writer, returnValue, parameters, isRequest);
                return;
            }
            if (this.useAspNetAjaxJson)
            {
                writer.WriteStartElement("root");
                writer.WriteAttributeString("type", "object");
            }
            if (this.useAspNetAjaxJson && messageDescription.IsVoid)
            {
                this.WriteVoidReturn(writer);
            }
            else
            {
                if (base.replyMessageInfo.ReturnPart != null)
                {
                    DataContractJsonSerializer serializer = base.replyMessageInfo.ReturnPart.Serializer as DataContractJsonSerializer;
                    if (this.useAspNetAjaxJson)
                    {
                        serializer = RecreateDataContractJsonSerializer(serializer, "d");
                    }
                    else
                    {
                        serializer = RecreateDataContractJsonSerializer(serializer, "root");
                    }
                    try
                    {
                        serializer.WriteObject(writer, returnValue);
                        goto Label_0150;
                    }
                    catch (SerializationException exception)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorSerializingParameter", new object[] { base.replyMessageInfo.ReturnPart.Description.Namespace, base.replyMessageInfo.ReturnPart.Description.Name, exception.Message }), exception));
                    }
                }
                if (base.replyMessageInfo.BodyParts != null)
                {
                    this.SerializeBody(writer, returnValue, parameters, isRequest);
                }
            }
        Label_0150:
            if (this.useAspNetAjaxJson)
            {
                writer.WriteEndElement();
            }
        }

        private void SerializeParameter(XmlDictionaryWriter writer, DataContractSerializerOperationFormatter.PartInfo part, object graph)
        {
            if (part.Description.Multiple)
            {
                if (graph != null)
                {
                    foreach (object obj2 in (IEnumerable) graph)
                    {
                        this.SerializeParameterPart(writer, part, obj2);
                    }
                }
            }
            else
            {
                this.SerializeParameterPart(writer, part, graph);
            }
        }

        private void SerializeParameterPart(XmlDictionaryWriter writer, DataContractSerializerOperationFormatter.PartInfo part, object graph)
        {
            try
            {
                part.Serializer.WriteObject(writer, graph);
            }
            catch (SerializationException exception)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBodyErrorSerializingParameter", new object[] { part.Description.Namespace, part.Description.Name, exception.Message }), exception));
            }
        }

        private void SerializeParameters(XmlDictionaryWriter writer, DataContractSerializerOperationFormatter.PartInfo[] parts, object[] parameters)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                DataContractSerializerOperationFormatter.PartInfo part = parts[i];
                object graph = parameters[part.Description.Index];
                this.SerializeParameter(writer, part, graph);
            }
        }

        private void ValidateTypeObjectAttribute(XmlDictionaryReader reader, bool isRequest)
        {
            DataContractSerializerOperationFormatter.MessageInfo info = isRequest ? base.requestMessageInfo : base.replyMessageInfo;
            if (info.WrapperName != null)
            {
                if (!IsStartElement(reader, info.WrapperName, info.WrapperNamespace))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBody", new object[] { info.WrapperName, info.WrapperNamespace, reader.NodeType, reader.Name, reader.NamespaceURI })));
                }
                string attribute = reader.GetAttribute("type");
                if (!attribute.Equals("object", StringComparison.Ordinal))
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR2.GetString(SR2.JsonFormatterExpectedAttributeObject, new object[] { attribute })));
                }
            }
        }

        private void VerifyIsStartElement(XmlDictionaryReader reader, string elementName)
        {
            bool flag = false;
            while (reader.IsStartElement())
            {
                if (IsStartElement(reader, elementName))
                {
                    flag = true;
                    break;
                }
                OperationFormatter.TraceAndSkipElement(reader);
            }
            if (!flag)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBody", new object[] { elementName, string.Empty, reader.NodeType, reader.Name, reader.NamespaceURI })));
            }
        }

        private void VerifyIsStartElement(XmlDictionaryReader reader, XmlDictionaryString elementName, XmlDictionaryString elementNamespace)
        {
            bool flag = false;
            while (reader.IsStartElement())
            {
                if (IsStartElement(reader, elementName, elementNamespace))
                {
                    flag = true;
                    break;
                }
                OperationFormatter.TraceAndSkipElement(reader);
            }
            if (!flag)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(System.ServiceModel.SR.GetString("SFxInvalidMessageBody", new object[] { elementName, elementNamespace, reader.NodeType, reader.Name, reader.NamespaceURI })));
            }
        }

        private void WriteVoidReturn(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("d");
            writer.WriteAttributeString("type", "null");
            writer.WriteEndElement();
        }
    }
}

