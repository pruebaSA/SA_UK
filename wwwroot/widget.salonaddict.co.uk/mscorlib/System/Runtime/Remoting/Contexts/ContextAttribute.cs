namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;
    using System.Security.Permissions;

    [Serializable, AttributeUsage(AttributeTargets.Class), ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ContextAttribute : Attribute, IContextAttribute, IContextProperty
    {
        protected string AttributeName;

        public ContextAttribute(string name)
        {
            this.AttributeName = name;
        }

        public override bool Equals(object o)
        {
            IContextProperty property = o as IContextProperty;
            return ((property != null) && this.AttributeName.Equals(property.Name));
        }

        public virtual void Freeze(Context newContext)
        {
        }

        public override int GetHashCode() => 
            this.AttributeName.GetHashCode();

        public virtual void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            if (ctorMsg == null)
            {
                throw new ArgumentNullException("ctorMsg");
            }
            ctorMsg.ContextProperties.Add(this);
        }

        public virtual bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException("ctx");
            }
            if (ctorMsg == null)
            {
                throw new ArgumentNullException("ctorMsg");
            }
            if (!ctorMsg.ActivationType.IsContextful)
            {
                return true;
            }
            object property = ctx.GetProperty(this.AttributeName);
            return ((property != null) && this.Equals(property));
        }

        public virtual bool IsNewContextOK(Context newCtx) => 
            true;

        public virtual string Name =>
            this.AttributeName;
    }
}

