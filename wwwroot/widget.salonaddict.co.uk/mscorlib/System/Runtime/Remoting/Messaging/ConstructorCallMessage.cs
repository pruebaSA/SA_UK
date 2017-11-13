namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Proxies;
    using System.Threading;

    internal class ConstructorCallMessage : IConstructionCallMessage, IMethodCallMessage, IMethodMessage, IMessage
    {
        [NonSerialized]
        private Type _activationType;
        private string _activationTypeName;
        private IActivator _activator;
        private ArgMapper _argMapper;
        private object[] _callSiteActivationAttributes;
        private IList _contextProperties;
        private int _iFlags;
        private Message _message;
        private object _properties;
        private object[] _typeAttributes;
        private object[] _womGlobalAttributes;
        private const int CCM_ACTIVATEINCONTEXT = 1;

        private ConstructorCallMessage()
        {
        }

        internal ConstructorCallMessage(object[] callSiteActivationAttributes, object[] womAttr, object[] typeAttr, Type serverType)
        {
            this._activationType = serverType;
            this._activationTypeName = RemotingServices.GetDefaultQualifiedTypeName(this._activationType);
            this._callSiteActivationAttributes = callSiteActivationAttributes;
            this._womGlobalAttributes = womAttr;
            this._typeAttributes = typeAttr;
        }

        public object GetArg(int argNum) => 
            this._message?.GetArg(argNum);

        public string GetArgName(int index) => 
            this._message?.GetArgName(index);

        public object GetInArg(int argNum) => 
            this._argMapper?.GetArg(argNum);

        public string GetInArgName(int index) => 
            this._argMapper?.GetArgName(index);

        internal System.Runtime.Remoting.Messaging.LogicalCallContext GetLogicalCallContext() => 
            this._message?.GetLogicalCallContext();

        internal Message GetMessage() => 
            this._message;

        public object GetThisPtr() => 
            this._message?.GetThisPtr();

        internal object[] GetTypeAttributes() => 
            this._typeAttributes;

        internal object[] GetWOMAttributes() => 
            this._womGlobalAttributes;

        internal void SetFrame(MessageData msgData)
        {
            this._message = new Message();
            this._message.InitFields(msgData);
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext SetLogicalCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext ctx) => 
            this._message?.SetLogicalCallContext(ctx);

        internal bool ActivateInContext
        {
            get => 
                ((this._iFlags & 1) != 0);
            set
            {
                this._iFlags = value ? (this._iFlags | 1) : (this._iFlags & -2);
            }
        }

        public Type ActivationType
        {
            get
            {
                if ((this._activationType == null) && (this._activationTypeName != null))
                {
                    this._activationType = RemotingServices.InternalGetTypeFromQualifiedTypeName(this._activationTypeName, false);
                }
                return this._activationType;
            }
        }

        public string ActivationTypeName =>
            this._activationTypeName;

        public IActivator Activator
        {
            get => 
                this._activator;
            set
            {
                this._activator = value;
            }
        }

        public int ArgCount =>
            this._message?.ArgCount;

        public object[] Args =>
            this._message?.Args;

        public object[] CallSiteActivationAttributes =>
            this._callSiteActivationAttributes;

        public IList ContextProperties
        {
            get
            {
                if (this._contextProperties == null)
                {
                    this._contextProperties = new ArrayList();
                }
                return this._contextProperties;
            }
        }

        public bool HasVarArgs =>
            this._message?.HasVarArgs;

        public int InArgCount =>
            this._argMapper?.ArgCount;

        public object[] InArgs =>
            this._argMapper?.Args;

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this.GetLogicalCallContext();

        public System.Reflection.MethodBase MethodBase =>
            this._message?.MethodBase;

        public string MethodName =>
            this._message?.MethodName;

        public object MethodSignature =>
            this._message?.MethodSignature;

        public IDictionary Properties
        {
            get
            {
                if (this._properties == null)
                {
                    object obj2 = new CCMDictionary(this, new Hashtable());
                    Interlocked.CompareExchange(ref this._properties, obj2, null);
                }
                return (IDictionary) this._properties;
            }
        }

        public string TypeName =>
            this._message?.TypeName;

        public string Uri
        {
            get => 
                this._message?.Uri;
            set
            {
                if (this._message == null)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InternalState"));
                }
                this._message.Uri = value;
            }
        }
    }
}

