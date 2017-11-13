namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal class UncommonField<T>
    {
        private T _defaultValue;
        private int _globalIndex;
        private bool _hasBeenSet;

        public UncommonField() : this(default(T))
        {
        }

        public UncommonField(T defaultValue)
        {
            this._defaultValue = defaultValue;
            this._hasBeenSet = false;
            lock (DependencyProperty.Synchronized)
            {
                this._globalIndex = DependencyProperty.GetUniqueGlobalIndex(null, null);
                DependencyProperty.RegisteredPropertyList.Add();
            }
        }

        public void ClearValue(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            EntryIndex entry = instance.LookupEntry(this._globalIndex);
            instance.UnsetEffectiveValue(entry, null, null);
        }

        public T GetValue(DependencyObject instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (this._hasBeenSet)
            {
                EntryIndex entry = instance.LookupEntry(this._globalIndex);
                if (entry.Found)
                {
                    object localValue = instance.EffectiveValues[entry.Index].LocalValue;
                    if (localValue != DependencyProperty.UnsetValue)
                    {
                        return (T) localValue;
                    }
                }
            }
            return this._defaultValue;
        }

        public void SetValue(DependencyObject instance, T value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            EntryIndex entry = instance.LookupEntry(this._globalIndex);
            if (!object.ReferenceEquals(value, this._defaultValue))
            {
                instance.SetEffectiveValue(entry, null, this._globalIndex, null, value, BaseValueSourceInternal.Local);
                this._hasBeenSet = true;
            }
            else
            {
                instance.UnsetEffectiveValue(entry, null, null);
            }
        }

        internal int GlobalIndex =>
            this._globalIndex;
    }
}

