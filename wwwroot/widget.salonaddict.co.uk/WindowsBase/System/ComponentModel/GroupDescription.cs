namespace System.ComponentModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    public abstract class GroupDescription : INotifyPropertyChanged
    {
        private ObservableCollection<object> _explicitGroupNames = new ObservableCollection<object>();

        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged;

        protected GroupDescription()
        {
            this._explicitGroupNames.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupNamesChanged);
        }

        public abstract object GroupNameFromItem(object item, int level, CultureInfo culture);
        public virtual bool NamesMatch(object groupName, object itemName) => 
            object.Equals(groupName, itemName);

        private void OnGroupNamesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs("GroupNames"));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeGroupNames() => 
            (this._explicitGroupNames.Count > 0);

        public ObservableCollection<object> GroupNames =>
            this._explicitGroupNames;
    }
}

