namespace System.Runtime.Remoting.Activation
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Contexts;

    internal class RemotePropertyHolderAttribute : IContextAttribute
    {
        private IList _cp;

        internal RemotePropertyHolderAttribute(IList cp)
        {
            this._cp = cp;
        }

        [ComVisible(true)]
        public virtual void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
        {
            for (int i = 0; i < this._cp.Count; i++)
            {
                ctorMsg.ContextProperties.Add(this._cp[i]);
            }
        }

        [ComVisible(true)]
        public virtual bool IsContextOK(Context ctx, IConstructionCallMessage msg) => 
            false;
    }
}

