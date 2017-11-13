namespace System.Web.Services.Description
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using System.Xml;
    using System.Xml.Serialization;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class ProtocolReflector
    {
        private System.Web.Services.Description.Binding binding;
        private System.Web.Services.Description.ServiceDescription bindingServiceDescription;
        private bool emptyBinding;
        private MessageCollection headerMessages;
        private Message inputMessage;
        private LogicalMethodInfo method;
        private WebMethodAttribute methodAttr;
        private System.Web.Services.Description.Operation operation;
        private System.Web.Services.Description.OperationBinding operationBinding;
        private Message outputMessage;
        private System.Web.Services.Description.Port port;
        private CodeIdentifiers portNames;
        private System.Web.Services.Description.PortType portType;
        private ServiceDescriptionReflector reflector;

        protected ProtocolReflector()
        {
        }

        private void AddImport(string ns, string location)
        {
            foreach (Import import in this.ServiceDescription.Imports)
            {
                if ((import.Namespace == ns) && (import.Location == location))
                {
                    return;
                }
            }
            Import import2 = new Import {
                Namespace = ns,
                Location = location
            };
            this.ServiceDescription.Imports.Add(import2);
        }

        protected virtual void BeginClass()
        {
        }

        protected virtual void EndClass()
        {
        }

        public System.Web.Services.Description.ServiceDescription GetServiceDescription(string ns)
        {
            System.Web.Services.Description.ServiceDescription serviceDescription = this.ServiceDescriptions[ns];
            if (serviceDescription == null)
            {
                serviceDescription = new System.Web.Services.Description.ServiceDescription {
                    TargetNamespace = ns
                };
                this.ServiceDescriptions.Add(serviceDescription);
            }
            return serviceDescription;
        }

        internal void Initialize(ServiceDescriptionReflector reflector)
        {
            this.reflector = reflector;
        }

        private void MoveToMethod(LogicalMethodInfo method)
        {
            this.method = method;
            this.methodAttr = method.MethodAttribute;
        }

        internal void Reflect()
        {
            this.emptyBinding = false;
            Hashtable hashtable = new Hashtable();
            Hashtable hashtable2 = new Hashtable();
            for (int i = 0; i < this.reflector.Methods.Length; i++)
            {
                this.MoveToMethod(this.reflector.Methods[i]);
                string str = this.ReflectMethodBinding();
                if (str == null)
                {
                    str = string.Empty;
                }
                ReflectedBinding binding = (ReflectedBinding) hashtable2[str];
                if (binding == null)
                {
                    binding = new ReflectedBinding {
                        bindingAttr = WebServiceBindingReflector.GetAttribute(this.method, str)
                    };
                    if ((binding.bindingAttr == null) || ((str.Length == 0) && (binding.bindingAttr.Location.Length > 0)))
                    {
                        binding.bindingAttr = new WebServiceBindingAttribute();
                    }
                    hashtable2.Add(str, binding);
                }
                if (binding.bindingAttr.Location.Length == 0)
                {
                    if (binding.methodList == null)
                    {
                        binding.methodList = new ArrayList();
                    }
                    binding.methodList.Add(this.method);
                    hashtable[binding.bindingAttr.Name] = this.method;
                }
                else
                {
                    this.AddImport(binding.bindingAttr.Namespace, binding.bindingAttr.Location);
                }
            }
            foreach (ReflectedBinding binding2 in hashtable2.Values)
            {
                this.ReflectBinding(binding2);
            }
            if (hashtable2.Count == 0)
            {
                this.emptyBinding = true;
                ReflectedBinding reflectedBinding = null;
                foreach (WebServiceBindingAttribute attribute in this.ServiceType.GetCustomAttributes(typeof(WebServiceBindingAttribute), false))
                {
                    if (hashtable[attribute.Name] == null)
                    {
                        if (reflectedBinding != null)
                        {
                            reflectedBinding = null;
                            break;
                        }
                        reflectedBinding = new ReflectedBinding(attribute);
                    }
                }
                if (reflectedBinding != null)
                {
                    this.ReflectBinding(reflectedBinding);
                }
            }
            foreach (Type type in this.ServiceType.GetInterfaces())
            {
                foreach (WebServiceBindingAttribute attribute2 in type.GetCustomAttributes(typeof(WebServiceBindingAttribute), false))
                {
                    if (hashtable[attribute2.Name] == null)
                    {
                        this.ReflectBinding(new ReflectedBinding(attribute2));
                    }
                }
            }
            this.ReflectDescription();
        }

        private void ReflectBinding(ReflectedBinding reflectedBinding)
        {
            string identifier = XmlConvert.EncodeLocalName(reflectedBinding.bindingAttr.Name);
            string ns = reflectedBinding.bindingAttr.Namespace;
            if (identifier.Length == 0)
            {
                identifier = this.Service.Name + this.ProtocolName;
            }
            if (ns.Length == 0)
            {
                ns = this.ServiceDescription.TargetNamespace;
            }
            WsiProfiles none = WsiProfiles.None;
            if (reflectedBinding.bindingAttr.Location.Length > 0)
            {
                this.portType = null;
                this.binding = null;
            }
            else
            {
                this.bindingServiceDescription = this.GetServiceDescription(ns);
                CodeIdentifiers identifiers = new CodeIdentifiers();
                foreach (System.Web.Services.Description.Binding binding in this.bindingServiceDescription.Bindings)
                {
                    identifiers.AddReserved(binding.Name);
                }
                identifier = identifiers.AddUnique(identifier, this.binding);
                this.portType = new System.Web.Services.Description.PortType();
                this.binding = new System.Web.Services.Description.Binding();
                this.portType.Name = identifier;
                this.binding.Name = identifier;
                this.binding.Type = new XmlQualifiedName(this.portType.Name, ns);
                none = reflectedBinding.bindingAttr.ConformsTo & this.ConformsTo;
                if (reflectedBinding.bindingAttr.EmitConformanceClaims && (none != WsiProfiles.None))
                {
                    System.Web.Services.Description.ServiceDescription.AddConformanceClaims(this.binding.GetDocumentationElement(), none);
                }
                this.bindingServiceDescription.Bindings.Add(this.binding);
                this.bindingServiceDescription.PortTypes.Add(this.portType);
            }
            if (this.portNames == null)
            {
                this.portNames = new CodeIdentifiers();
                foreach (System.Web.Services.Description.Port port in this.Service.Ports)
                {
                    this.portNames.AddReserved(port.Name);
                }
            }
            this.port = new System.Web.Services.Description.Port();
            this.port.Binding = new XmlQualifiedName(identifier, ns);
            this.port.Name = this.portNames.AddUnique(identifier, this.port);
            this.Service.Ports.Add(this.port);
            this.BeginClass();
            if ((reflectedBinding.methodList != null) && (reflectedBinding.methodList.Count > 0))
            {
                foreach (LogicalMethodInfo info in reflectedBinding.methodList)
                {
                    this.MoveToMethod(info);
                    this.operation = new System.Web.Services.Description.Operation();
                    this.operation.Name = XmlConvert.EncodeLocalName(info.Name);
                    if ((this.methodAttr.Description != null) && (this.methodAttr.Description.Length > 0))
                    {
                        this.operation.Documentation = this.methodAttr.Description;
                    }
                    this.operationBinding = new System.Web.Services.Description.OperationBinding();
                    this.operationBinding.Name = this.operation.Name;
                    this.inputMessage = null;
                    this.outputMessage = null;
                    this.headerMessages = null;
                    if (this.ReflectMethod())
                    {
                        if (this.inputMessage != null)
                        {
                            this.bindingServiceDescription.Messages.Add(this.inputMessage);
                        }
                        if (this.outputMessage != null)
                        {
                            this.bindingServiceDescription.Messages.Add(this.outputMessage);
                        }
                        if (this.headerMessages != null)
                        {
                            foreach (Message message in this.headerMessages)
                            {
                                this.bindingServiceDescription.Messages.Add(message);
                            }
                        }
                        this.binding.Operations.Add(this.operationBinding);
                        this.portType.Operations.Add(this.operation);
                    }
                }
            }
            if (((this.binding != null) && (none == WsiProfiles.BasicProfile1_1)) && (this.ProtocolName == "Soap"))
            {
                BasicProfileViolationCollection violations = new BasicProfileViolationCollection();
                WebServicesInteroperability.AnalyzeBinding(this.binding, this.bindingServiceDescription, this.ServiceDescriptions, violations);
                if (violations.Count > 0)
                {
                    throw new InvalidOperationException(System.Web.Services.Res.GetString("WebWsiViolation", new object[] { this.ServiceType.FullName, violations.ToString() }));
                }
            }
            this.EndClass();
        }

        protected virtual void ReflectDescription()
        {
        }

        protected abstract bool ReflectMethod();
        protected virtual string ReflectMethodBinding() => 
            string.Empty;

        public System.Web.Services.Description.Binding Binding =>
            this.binding;

        internal virtual WsiProfiles ConformsTo =>
            WsiProfiles.None;

        public string DefaultNamespace =>
            this.reflector.ServiceAttribute.Namespace;

        public MessageCollection HeaderMessages
        {
            get
            {
                if (this.headerMessages == null)
                {
                    this.headerMessages = new MessageCollection(this.bindingServiceDescription);
                }
                return this.headerMessages;
            }
        }

        public Message InputMessage
        {
            get
            {
                if (this.inputMessage == null)
                {
                    string str = XmlConvert.EncodeLocalName((this.methodAttr.MessageName.Length == 0) ? this.Method.Name : this.methodAttr.MessageName);
                    bool flag = str != this.Method.Name;
                    this.inputMessage = new Message();
                    this.inputMessage.Name = str + this.ProtocolName + "In";
                    OperationInput operationMessage = new OperationInput();
                    if (flag)
                    {
                        operationMessage.Name = str;
                    }
                    operationMessage.Message = new XmlQualifiedName(this.inputMessage.Name, this.bindingServiceDescription.TargetNamespace);
                    this.operation.Messages.Add(operationMessage);
                    this.OperationBinding.Input = new InputBinding();
                    if (flag)
                    {
                        this.OperationBinding.Input.Name = str;
                    }
                }
                return this.inputMessage;
            }
        }

        internal bool IsEmptyBinding =>
            this.emptyBinding;

        public LogicalMethodInfo Method =>
            this.method;

        public WebMethodAttribute MethodAttribute =>
            this.methodAttr;

        public LogicalMethodInfo[] Methods =>
            this.reflector.Methods;

        public System.Web.Services.Description.Operation Operation =>
            this.operation;

        public System.Web.Services.Description.OperationBinding OperationBinding =>
            this.operationBinding;

        public Message OutputMessage
        {
            get
            {
                if (this.outputMessage == null)
                {
                    string str = XmlConvert.EncodeLocalName((this.methodAttr.MessageName.Length == 0) ? this.Method.Name : this.methodAttr.MessageName);
                    bool flag = str != this.Method.Name;
                    this.outputMessage = new Message();
                    this.outputMessage.Name = str + this.ProtocolName + "Out";
                    OperationOutput operationMessage = new OperationOutput();
                    if (flag)
                    {
                        operationMessage.Name = str;
                    }
                    operationMessage.Message = new XmlQualifiedName(this.outputMessage.Name, this.bindingServiceDescription.TargetNamespace);
                    this.operation.Messages.Add(operationMessage);
                    this.OperationBinding.Output = new OutputBinding();
                    if (flag)
                    {
                        this.OperationBinding.Output.Name = str;
                    }
                }
                return this.outputMessage;
            }
        }

        public System.Web.Services.Description.Port Port =>
            this.port;

        public System.Web.Services.Description.PortType PortType =>
            this.portType;

        public abstract string ProtocolName { get; }

        internal Hashtable ReflectionContext =>
            this.reflector.ReflectionContext;

        public XmlReflectionImporter ReflectionImporter =>
            this.reflector.ReflectionImporter;

        public XmlSchemaExporter SchemaExporter =>
            this.reflector.SchemaExporter;

        public XmlSchemas Schemas =>
            this.reflector.Schemas;

        public System.Web.Services.Description.Service Service =>
            this.reflector.Service;

        public System.Web.Services.Description.ServiceDescription ServiceDescription =>
            this.reflector.ServiceDescription;

        public ServiceDescriptionCollection ServiceDescriptions =>
            this.reflector.ServiceDescriptions;

        public Type ServiceType =>
            this.reflector.ServiceType;

        public string ServiceUrl =>
            this.reflector.ServiceUrl;

        private class ReflectedBinding
        {
            public WebServiceBindingAttribute bindingAttr;
            public ArrayList methodList;

            internal ReflectedBinding()
            {
            }

            internal ReflectedBinding(WebServiceBindingAttribute bindingAttr)
            {
                this.bindingAttr = bindingAttr;
            }
        }
    }
}

