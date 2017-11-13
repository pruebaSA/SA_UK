namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class InternalMessageWrapper
    {
        protected IMessage WrappedMessage;

        public InternalMessageWrapper(IMessage msg)
        {
            this.WrappedMessage = msg;
        }

        internal object GetIdentityObject()
        {
            IInternalMessage wrappedMessage = this.WrappedMessage as IInternalMessage;
            if (wrappedMessage != null)
            {
                return wrappedMessage.IdentityObject;
            }
            InternalMessageWrapper wrapper = this.WrappedMessage as InternalMessageWrapper;
            if (wrapper != null)
            {
                return wrapper.GetIdentityObject();
            }
            return null;
        }

        internal object GetServerIdentityObject()
        {
            IInternalMessage wrappedMessage = this.WrappedMessage as IInternalMessage;
            if (wrappedMessage != null)
            {
                return wrappedMessage.ServerIdentityObject;
            }
            InternalMessageWrapper wrapper = this.WrappedMessage as InternalMessageWrapper;
            if (wrapper != null)
            {
                return wrapper.GetServerIdentityObject();
            }
            return null;
        }
    }
}

