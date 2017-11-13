namespace System.ComponentModel
{
    using System;
    using System.Runtime.CompilerServices;

    public interface INotifyPropertyChanged
    {
        event PropertyChangedEventHandler PropertyChanged;
    }
}

