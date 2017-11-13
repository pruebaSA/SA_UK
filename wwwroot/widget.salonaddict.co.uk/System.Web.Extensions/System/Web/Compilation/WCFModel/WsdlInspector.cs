namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Web.Resources;
    using System.Web.Services.Description;
    using System.Xml;

    internal class WsdlInspector
    {
        private IList<ProxyGenerationError> importErrors;
        private Dictionary<XmlQualifiedName, Message> messages;
        private Dictionary<XmlQualifiedName, PortType> portTypes;

        private WsdlInspector(IList<ProxyGenerationError> importErrors)
        {
            this.importErrors = importErrors;
            this.portTypes = new Dictionary<XmlQualifiedName, PortType>();
            this.messages = new Dictionary<XmlQualifiedName, Message>();
        }

        internal static void CheckDuplicatedWsdlItems(ICollection<ServiceDescription> wsdlFiles, IList<ProxyGenerationError> importErrors)
        {
            new WsdlInspector(importErrors).CheckServiceDescriptions(wsdlFiles);
        }

        private void CheckServiceDescriptions(ICollection<ServiceDescription> wsdlFiles)
        {
            foreach (ServiceDescription description in wsdlFiles)
            {
                string targetNamespace = description.TargetNamespace;
                if (string.IsNullOrEmpty(targetNamespace))
                {
                    targetNamespace = string.Empty;
                }
                foreach (PortType type in description.PortTypes)
                {
                    PortType type2;
                    XmlQualifiedName key = new XmlQualifiedName(type.Name, targetNamespace);
                    if (this.portTypes.TryGetValue(key, out type2))
                    {
                        this.MatchPortTypes(type2, type);
                    }
                    else
                    {
                        this.portTypes.Add(key, type);
                    }
                }
                foreach (Message message in description.Messages)
                {
                    Message message2;
                    XmlQualifiedName name2 = new XmlQualifiedName(message.Name, targetNamespace);
                    if (this.messages.TryGetValue(name2, out message2))
                    {
                        this.MatchMessages(message2, message);
                    }
                    else
                    {
                        this.messages.Add(name2, message);
                    }
                }
            }
        }

        private bool MatchCollections<T>(T[] x, T[] y, MatchCollectionItemDelegate<T> compareItems) where T: class
        {
            T local;
            T local2;
            IEnumerator enumerator = x.GetEnumerator();
            IEnumerator enumerator2 = y.GetEnumerator();
            do
            {
                local = enumerator.MoveNext() ? ((T) enumerator.Current) : default(T);
                local2 = enumerator2.MoveNext() ? ((T) enumerator2.Current) : default(T);
                if (((local != null) && (local2 != null)) && !compareItems(local, local2))
                {
                    return false;
                }
            }
            while ((local != null) && (local2 != null));
            return compareItems(local, local2);
        }

        private bool MatchMessageParts(MessagePart partX, MessagePart partY)
        {
            if (this.MatchXmlQualifiedNames(partX.Type, partY.Type) && this.MatchXmlQualifiedNames(partX.Element, partY.Element))
            {
                return true;
            }
            this.ReportMessageDefinedDifferently(partX, partX.Message, partY.Message);
            return false;
        }

        private void MatchMessages(Message x, Message y)
        {
            MessagePart[] array = new MessagePart[x.Parts.Count];
            x.Parts.CopyTo(array, 0);
            Array.Sort<MessagePart>(array, new MessagePartComparer());
            MessagePart[] partArray2 = new MessagePart[y.Parts.Count];
            y.Parts.CopyTo(partArray2, 0);
            Array.Sort<MessagePart>(partArray2, new MessagePartComparer());
            this.MatchCollections<MessagePart>(array, partArray2, delegate (MessagePart partX, MessagePart partY) {
                if ((partX != null) && (partY != null))
                {
                    int num = string.Compare(partX.Name, partY.Name, StringComparison.Ordinal);
                    if (num < 0)
                    {
                        this.ReportUniqueMessagePart(partX, x, y);
                        return false;
                    }
                    if (num > 0)
                    {
                        this.ReportUniqueMessagePart(partY, y, x);
                        return false;
                    }
                    if (!this.MatchMessageParts(partX, partY))
                    {
                        return false;
                    }
                    return true;
                }
                if (partX != null)
                {
                    this.ReportUniqueMessagePart(partX, x, y);
                    return false;
                }
                if (partY != null)
                {
                    this.ReportUniqueMessagePart(partY, y, x);
                    return false;
                }
                return true;
            });
        }

        private bool MatchOperationMessages(OperationMessage x, OperationMessage y) => 
            (((x == null) && (y == null)) || (((x != null) && (y != null)) && this.MatchXmlQualifiedNames(x.Message, y.Message)));

        private bool MatchOperations(Operation x, Operation y)
        {
            if (!this.MatchOperationMessages(x.Messages.Input, y.Messages.Input))
            {
                this.ReportOperationDefinedDifferently(x, y);
                return false;
            }
            if (!this.MatchOperationMessages(x.Messages.Output, y.Messages.Output))
            {
                this.ReportOperationDefinedDifferently(x, y);
                return false;
            }
            OperationFault[] array = new OperationFault[x.Faults.Count];
            x.Faults.CopyTo(array, 0);
            Array.Sort<OperationFault>(array, new OperationFaultComparer());
            OperationFault[] faultArray2 = new OperationFault[y.Faults.Count];
            y.Faults.CopyTo(faultArray2, 0);
            Array.Sort<OperationFault>(faultArray2, new OperationFaultComparer());
            if (!this.MatchCollections<OperationFault>(array, faultArray2, delegate (OperationFault faultX, OperationFault faultY) {
                if ((faultX != null) && (faultY != null))
                {
                    return this.MatchXmlQualifiedNames(faultX.Message, faultY.Message);
                }
                return (faultX == null) && (faultY == null);
            }))
            {
                this.ReportOperationDefinedDifferently(x, y);
                return false;
            }
            return true;
        }

        private void MatchPortTypes(PortType x, PortType y)
        {
            Operation[] array = new Operation[x.Operations.Count];
            x.Operations.CopyTo(array, 0);
            Array.Sort<Operation>(array, new OperationComparer());
            Operation[] operationArray2 = new Operation[y.Operations.Count];
            y.Operations.CopyTo(operationArray2, 0);
            Array.Sort<Operation>(operationArray2, new OperationComparer());
            this.MatchCollections<Operation>(array, operationArray2, delegate (Operation operationX, Operation operationY) {
                if ((operationX != null) && (operationY != null))
                {
                    int num = string.Compare(operationX.Name, operationY.Name, StringComparison.Ordinal);
                    if (num < 0)
                    {
                        this.ReportUniqueOperation(operationX, x, y);
                        return false;
                    }
                    if (num > 0)
                    {
                        this.ReportUniqueOperation(operationY, y, x);
                        return false;
                    }
                    if (!this.MatchOperations(operationX, operationY))
                    {
                        return false;
                    }
                    return true;
                }
                if (operationX != null)
                {
                    this.ReportUniqueOperation(operationX, x, y);
                    return false;
                }
                if (operationY != null)
                {
                    this.ReportUniqueOperation(operationY, y, x);
                    return false;
                }
                return true;
            });
        }

        private bool MatchXmlQualifiedNames(XmlQualifiedName x, XmlQualifiedName y)
        {
            if ((x != null) && (y != null))
            {
                return (x == y);
            }
            return ((x == null) && (y == null));
        }

        private void ReportMessageDefinedDifferently(MessagePart part, Message x, Message y)
        {
            this.importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_FieldDefinedDifferentlyInDuplicatedMessage, new object[] { part.Name, x.Name, x.ServiceDescription.RetrievalUrl, y.ServiceDescription.RetrievalUrl }))));
        }

        private void ReportOperationDefinedDifferently(Operation x, Operation y)
        {
            this.importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_OperationDefinedDifferently, new object[] { x.Name, x.PortType.Name, x.PortType.ServiceDescription.RetrievalUrl, y.PortType.ServiceDescription.RetrievalUrl }))));
        }

        private void ReportUniqueMessagePart(MessagePart part, Message message1, Message message2)
        {
            this.importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_FieldDefinedInOneOfDuplicatedMessage, new object[] { message1.Name, message1.ServiceDescription.RetrievalUrl, message2.ServiceDescription.RetrievalUrl, part.Name }))));
        }

        private void ReportUniqueOperation(Operation operation, PortType portType1, PortType portType2)
        {
            this.importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.MergeMetadata, string.Empty, new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_OperationDefinedInOneOfDuplicatedServiceContract, new object[] { portType1.Name, portType1.ServiceDescription.RetrievalUrl, portType2.ServiceDescription.RetrievalUrl, operation.Name }))));
        }

        private delegate bool MatchCollectionItemDelegate<T>(T x, T y);

        private class MessagePartComparer : IComparer<MessagePart>
        {
            public int Compare(MessagePart x, MessagePart y) => 
                string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        private class OperationComparer : IComparer<Operation>
        {
            public int Compare(Operation x, Operation y) => 
                string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        private class OperationFaultComparer : IComparer<OperationFault>
        {
            public int Compare(OperationFault x, OperationFault y)
            {
                int num = string.Compare(x.Message.Namespace, y.Message.Namespace, StringComparison.Ordinal);
                if (num != 0)
                {
                    return num;
                }
                return string.Compare(x.Message.Name, y.Message.Name, StringComparison.Ordinal);
            }
        }
    }
}

