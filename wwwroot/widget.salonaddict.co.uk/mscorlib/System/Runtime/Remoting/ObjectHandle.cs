namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Lifetime;
    using System.Security.Permissions;

    [ComVisible(true), ClassInterface(ClassInterfaceType.AutoDual)]
    public class ObjectHandle : MarshalByRefObject, IObjectHandle
    {
        private object WrappedObject;

        private ObjectHandle()
        {
        }

        public ObjectHandle(object o)
        {
            this.WrappedObject = o;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            MarshalByRefObject wrappedObject = this.WrappedObject as MarshalByRefObject;
            if ((wrappedObject != null) && (wrappedObject.InitializeLifetimeService() == null))
            {
                return null;
            }
            return (ILease) base.InitializeLifetimeService();
        }

        public object Unwrap() => 
            this.WrappedObject;
    }
}

