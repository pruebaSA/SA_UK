namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable]
    internal class ConstructionLevelActivator : IActivator
    {
        internal ConstructionLevelActivator()
        {
        }

        [ComVisible(true)]
        public virtual IConstructionReturnMessage Activate(IConstructionCallMessage ctorMsg)
        {
            ctorMsg.Activator = ctorMsg.Activator.NextActivator;
            return ActivationServices.DoServerContextActivation(ctorMsg);
        }

        public virtual ActivatorLevel Level =>
            ActivatorLevel.Construction;

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

