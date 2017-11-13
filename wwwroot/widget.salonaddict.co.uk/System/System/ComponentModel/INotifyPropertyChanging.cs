namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    public interface INotifyPropertyChanging
    {
        event PropertyChangingEventHandler PropertyChanging;
    }
}

