namespace System.Web
{
    using System;
    using System.Collections;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TraceContextEventArgs : EventArgs
    {
        private ICollection _records;

        public TraceContextEventArgs(ICollection records)
        {
            this._records = records;
        }

        public ICollection TraceRecords =>
            this._records;
    }
}

