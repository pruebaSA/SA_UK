namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable]
    internal class ContextLevelActivator : IActivator
    {
        private IActivator m_NextActivator;

        internal ContextLevelActivator()
        {
            this.m_NextActivator = null;
        }

        internal ContextLevelActivator(SerializationInfo info, StreamingContext context)
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
            ctorMsg.Activator = ctorMsg.Activator.NextActivator;
            return ActivationServices.DoCrossContextActivation(ctorMsg);
        }

        public virtual ActivatorLevel Level =>
            ActivatorLevel.Context;

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

