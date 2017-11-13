namespace System.Collections.ObjectModel
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class ReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        [field: NonSerialized]
        protected event NotifyCollectionChangedEventHandler CollectionChanged;

        [field: NonSerialized]
        protected event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged;

        [field: NonSerialized]
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged;

        public ReadOnlyObservableCollection(ObservableCollection<T> list) : base(list)
        {
            ((INotifyCollectionChanged) base.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(this.HandleCollectionChanged);
            ((INotifyPropertyChanged) base.Items).PropertyChanged += new PropertyChangedEventHandler(this.HandlePropertyChanged);
        }

        private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnCollectionChanged(e);
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }
    }
}

