namespace System.Collections.Specialized
{
    using System;
    using System.Runtime.CompilerServices;

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}

