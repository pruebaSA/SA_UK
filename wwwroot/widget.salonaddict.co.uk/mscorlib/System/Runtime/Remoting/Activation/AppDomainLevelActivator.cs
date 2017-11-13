namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    internal class AppDomainLevelActivator : IActivator
    {
        private IActivator m_NextActivator;
        private string m_RemActivatorURL;

        internal AppDomainLevelActivator(string remActivatorURL)
        {
            this.m_RemActivatorURL = remActivatorURL;
        }

        internal AppDomainLevelActivator(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.m_NextActivator = (IActivator) info.GetValue("m_NextActivator", typeof(IActivator));
        }

        [ComVisible(true)]
        public virtual IConstructionReturnMessage Activate(IConstructionCallMessage ctorMsg)
        {
            ctorMsg.Activator = this.m_NextActivator;
            return ActivationServices.GetActivator().Activate(ctorMsg);
        }

        public virtual ActivatorLevel Level =>
            ActivatorLevel.AppDomain;

        public virtual IActivator NextActivator
        {
            get => 
                this.m_NextActivator;
            set
            {
                this.m_NextActivator = value;
            }
        }
    }
}

