namespace System.Transactions.Oletx
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [ComImport, SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5EC35E09-B285-422c-83F5-1372384A42CC")]
    internal interface IEnlistmentShim
    {
        void PrepareRequestDone(OletxPrepareVoteType voteType);
        void CommitRequestDone();
        void AbortRequestDone();
    }
}

