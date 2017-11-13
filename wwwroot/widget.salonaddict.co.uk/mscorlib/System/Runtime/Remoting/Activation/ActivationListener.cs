namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    internal class ActivationListener : MarshalByRefObject, IActivator
    {
        [ComVisible(true)]
        public virtual IConstructionReturnMessage Activate(IConstructionCallMessage ctorMsg)
        {
            if ((ctorMsg == null) || RemotingServices.IsTransparentProxy(ctorMsg))
            {
                throw new ArgumentNullException("ctorMsg");
            }
            ctorMsg.Properties["Permission"] = "allowed";
            if (!RemotingConfigHandler.IsActivationAllowed(ctorMsg.ActivationTypeName))
            {
                throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Activation_PermissionDenied"), new object[] { ctorMsg.ActivationTypeName }));
            }
            if (ctorMsg.ActivationType == null)
            {
                throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_BadType"), new object[] { ctorMsg.ActivationTypeName }));
            }
            return ActivationServices.GetActivator().Activate(ctorMsg);
        }

        public override object InitializeLifetimeService() => 
            null;

        public virtual ActivatorLevel Level =>
            ActivatorLevel.AppDomain;

        public virtual IActivator NextActivator
        {
            get => 
                null;
            set
            {
                throw new InvalidOperationException();
            }
        }
    }
}

