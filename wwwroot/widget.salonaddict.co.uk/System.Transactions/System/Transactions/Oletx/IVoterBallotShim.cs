namespace System.Transactions.Oletx
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [ComImport, Guid("A5FAB903-21CB-49eb-93AE-EF72CD45169E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
    internal interface IVoterBallotShim
    {
        void Vote([MarshalAs(UnmanagedType.Bool)] bool voteYes);
    }
}

