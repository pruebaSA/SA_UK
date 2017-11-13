namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    public interface ISupportInitializeNotification : ISupportInitialize
    {
        event EventHandler Initialized;

        bool IsInitialized { get; }
    }
}

