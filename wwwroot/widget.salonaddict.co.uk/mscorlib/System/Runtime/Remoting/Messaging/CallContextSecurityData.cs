namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Security.Principal;

    [Serializable]
    internal class CallContextSecurityData : ICloneable
    {
        private IPrincipal _principal;

        public object Clone() => 
            new CallContextSecurityData { _principal = this._principal };

        internal bool HasInfo =>
            (null != this._principal);

        internal IPrincipal Principal
        {
            get => 
                this._principal;
            set
            {
                this._principal = value;
            }
        }
    }
}

