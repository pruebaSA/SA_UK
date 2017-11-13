namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal interface ISealable
    {
        void Seal();

        bool CanSeal { get; }

        bool IsSealed { get; }
    }
}

