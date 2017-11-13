﻿namespace System.ComponentModel.Design
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class DesignerTransactionCloseEventArgs : EventArgs
    {
        private bool commit;
        private bool lastTransaction;

        [Obsolete("This constructor is obsolete. Use DesignerTransactionCloseEventArgs(bool, bool) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public DesignerTransactionCloseEventArgs(bool commit) : this(commit, true)
        {
        }

        public DesignerTransactionCloseEventArgs(bool commit, bool lastTransaction)
        {
            this.commit = commit;
            this.lastTransaction = lastTransaction;
        }

        public bool LastTransaction =>
            this.lastTransaction;

        public bool TransactionCommitted =>
            this.commit;
    }
}

