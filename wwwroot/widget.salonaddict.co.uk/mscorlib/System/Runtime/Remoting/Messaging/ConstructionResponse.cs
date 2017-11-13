namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(true), CLSCompliant(false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ConstructionResponse : MethodResponse, IConstructionReturnMessage, IMethodReturnMessage, IMethodMessage, IMessage
    {
        public ConstructionResponse(Header[] h, IMethodCallMessage mcm) : base(h, mcm)
        {
        }

        internal ConstructionResponse(SerializationInfo info, StreamingContext context) : base(info, context)
        {
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
                        base.ExternalProperties = new CRMDictionary(this, base.InternalProperties);
                    }
                    return base.ExternalProperties;
                }
            }
        }
    }
}

