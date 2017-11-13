namespace System.ServiceModel
{
    using System;
    using System.Runtime.CompilerServices;

    public interface IOnlineStatus
    {
        event EventHandler Offline;

        event EventHandler Online;

        bool IsOnline { get; }
    }
}

