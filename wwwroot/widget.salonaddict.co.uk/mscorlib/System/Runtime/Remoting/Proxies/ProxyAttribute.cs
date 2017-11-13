namespace System.Runtime.Remoting.Proxies
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Contexts;
    using System.Security.Permissions;

    [ComVisible(true), AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ProxyAttribute : Attribute, IContextAttribute
    {
        public virtual MarshalByRefObject CreateInstance(Type serverType)
        {
            if (!serverType.IsContextful)
            {
                throw new RemotingException(Environment.GetResourceString("Remoting_Activation_MBR_ProxyAttribute"));
            }
            if (serverType.IsAbstract)
            {
                throw new RemotingException(Environment.GetResourceString("Acc_CreateAbst"));
            }
            return this.CreateInstanceInternal(serverType);
        }

        internal MarshalByRefObject CreateInstanceInternal(Type serverType) => 
            ActivationServices.CreateInstance(serverType);

        public virtual RealProxy CreateProxy(ObjRef objRef, Type serverType, object serverObject, Context serverContext)
        {
            RemotingProxy rp = new RemotingProxy(serverType);
            if (serverContext != null)
            {
                RealProxy.SetStubData(rp, serverContext.InternalContextID);
            }
            if ((objRef != null) && objRef.GetServerIdentity().IsAllocated)
            {
                rp.SetSrvInfo(objRef.GetServerIdentity(), objRef.GetDomainID());
            }
            rp.Initialized = true;
            Type type = serverType;
            if ((!type.IsContextful && !type.IsMarshalByRef) && (serverContext != null))
            {
                throw new RemotingException(Environment.GetResourceString("Remoting_Activation_MBR_ProxyAttribute"));
            }
            return rp;
        }

        [ComVisible(true)]
        public void GetPropertiesForNewContext(IConstructionCallMessage msg)
        {
        }

        [ComVisible(true)]
        public bool IsContextOK(Context ctx, IConstructionCallMessage msg) => 
            true;
    }
}

