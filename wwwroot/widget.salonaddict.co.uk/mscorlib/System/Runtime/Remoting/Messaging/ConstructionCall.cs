namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true), CLSCompliant(false), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ConstructionCall : MethodCall, IConstructionCallMessage, IMethodCallMessage, IMethodMessage, IMessage
    {
        internal Type _activationType;
        internal string _activationTypeName;
        internal IActivator _activator;
        internal object[] _callSiteActivationAttributes;
        internal IList _contextProperties;

        public ConstructionCall(Header[] headers) : base(headers)
        {
        }

        public ConstructionCall(IMessage m) : base(m)
        {
        }

        internal ConstructionCall(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal override bool FillSpecialHeader(string key, object value)
        {
            if (key != null)
            {
                if (!key.Equals("__ActivationType"))
                {
                    if (!key.Equals("__ContextProperties"))
                    {
                        if (!key.Equals("__CallSiteActivationAttributes"))
                        {
                            if (!key.Equals("__Activator"))
                            {
                                if (!key.Equals("__ActivationTypeName"))
                                {
                                    return base.FillSpecialHeader(key, value);
                                }
                                this._activationTypeName = (string) value;
                            }
                            else
                            {
                                this._activator = (IActivator) value;
                            }
                        }
                        else
                        {
                            this._callSiteActivationAttributes = (object[]) value;
                        }
                    }
                    else
                    {
                        this._contextProperties = (IList) value;
                    }
                }
                else
                {
                    this._activationType = null;
                }
            }
            return true;
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

        public override IDictionary Properties
        {
            get
            {
                lock (this)
                {
                    if (base.InternalProperties == null)
                    {
                        base.InternalProperties = new Hashtable();
                    }
                    if (base.ExternalProperties == null)
                    {
                        base.ExternalProperties = new CCMDictionary(this, base.InternalProperties);
                    }
                    return base.ExternalProperties;
                }
            }
        }
    }
}

