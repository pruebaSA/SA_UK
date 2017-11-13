﻿namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.Remoting;
    using System.Threading;

    [Serializable]
    internal class CrossAppDomainData
    {
        private object _ContextID = 0;
        private int _DomainID;
        private string _processGuid;

        internal CrossAppDomainData(IntPtr ctxId, int domainID, string processGuid)
        {
            this._DomainID = domainID;
            this._processGuid = processGuid;
            this._ContextID = ctxId.ToInt32();
        }

        internal bool IsFromThisAppDomain() => 
            (this.IsFromThisProcess() && (Thread.GetDomain().GetId() == this._DomainID));

        internal bool IsFromThisProcess() => 
            Identity.ProcessGuid.Equals(this._processGuid);

        internal virtual IntPtr ContextID =>
            new IntPtr((int) this._ContextID);

        internal virtual int DomainID =>
            this._DomainID;

        internal virtual string ProcessGuid =>
            this._processGuid;
    }
}

