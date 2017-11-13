namespace System.Data.Objects
{
    using System;
    using System.ComponentModel;

    internal interface IObjectView
    {
        void CollectionChanged(object sender, CollectionChangeEventArgs e);
        void EntityPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}

