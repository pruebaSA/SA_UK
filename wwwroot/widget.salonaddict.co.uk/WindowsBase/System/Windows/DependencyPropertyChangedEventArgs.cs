namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DependencyPropertyChangedEventArgs
    {
        private DependencyProperty _property;
        private PropertyMetadata _metadata;
        private PrivateFlags _flags;
        private EffectiveValueEntry _oldEntry;
        private EffectiveValueEntry _newEntry;
        private System.Windows.OperationType _operationType;
        public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
        {
            this._property = property;
            this._metadata = null;
            this._oldEntry = new EffectiveValueEntry(property);
            this._newEntry = this._oldEntry;
            this._oldEntry.Value = oldValue;
            this._newEntry.Value = newValue;
            this._flags = (PrivateFlags) 0;
            this._operationType = System.Windows.OperationType.Unknown;
            this.IsAValueChange = true;
        }

        [FriendAccessAllowed]
        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, object oldValue, object newValue)
        {
            this._property = property;
            this._metadata = metadata;
            this._oldEntry = new EffectiveValueEntry(property);
            this._newEntry = this._oldEntry;
            this._oldEntry.Value = oldValue;
            this._newEntry.Value = newValue;
            this._flags = (PrivateFlags) 0;
            this._operationType = System.Windows.OperationType.Unknown;
            this.IsAValueChange = true;
        }

        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, object value)
        {
            this._property = property;
            this._metadata = metadata;
            this._oldEntry = new EffectiveValueEntry(property);
            this._oldEntry.Value = value;
            this._newEntry = this._oldEntry;
            this._flags = (PrivateFlags) 0;
            this._operationType = System.Windows.OperationType.Unknown;
            this.IsASubPropertyChange = true;
        }

        internal DependencyPropertyChangedEventArgs(DependencyProperty property, PropertyMetadata metadata, bool isAValueChange, EffectiveValueEntry oldEntry, EffectiveValueEntry newEntry, System.Windows.OperationType operationType)
        {
            this._property = property;
            this._metadata = metadata;
            this._oldEntry = oldEntry;
            this._newEntry = newEntry;
            this._flags = (PrivateFlags) 0;
            this._operationType = operationType;
            this.IsAValueChange = isAValueChange;
            this.IsASubPropertyChange = operationType == System.Windows.OperationType.ChangeMutableDefaultValue;
        }

        public DependencyProperty Property =>
            this._property;
        [FriendAccessAllowed]
        internal bool IsAValueChange
        {
            get => 
                this.ReadPrivateFlag(PrivateFlags.IsAValueChange);
            set
            {
                this.WritePrivateFlag(PrivateFlags.IsAValueChange, value);
            }
        }
        [FriendAccessAllowed]
        internal bool IsASubPropertyChange
        {
            get => 
                this.ReadPrivateFlag(PrivateFlags.IsASubPropertyChange);
            set
            {
                this.WritePrivateFlag(PrivateFlags.IsASubPropertyChange, value);
            }
        }
        [FriendAccessAllowed]
        internal PropertyMetadata Metadata =>
            this._metadata;
        [FriendAccessAllowed]
        internal System.Windows.OperationType OperationType =>
            this._operationType;
        public object OldValue
        {
            get
            {
                EffectiveValueEntry flattenedEntry = this.OldEntry.GetFlattenedEntry(RequestFlags.FullyResolved);
                if (flattenedEntry.IsDeferredReference)
                {
                    flattenedEntry.Value = ((DeferredReference) flattenedEntry.Value).GetValue(flattenedEntry.BaseValueSourceInternal);
                }
                return flattenedEntry.Value;
            }
        }
        [FriendAccessAllowed]
        internal EffectiveValueEntry OldEntry =>
            this._oldEntry;
        [FriendAccessAllowed]
        internal BaseValueSourceInternal OldValueSource =>
            this._oldEntry.BaseValueSourceInternal;
        [FriendAccessAllowed]
        internal bool IsOldValueModified =>
            this._oldEntry.HasModifiers;
        [FriendAccessAllowed]
        internal bool IsOldValueDeferred =>
            this._oldEntry.IsDeferredReference;
        public object NewValue
        {
            get
            {
                EffectiveValueEntry flattenedEntry = this.NewEntry.GetFlattenedEntry(RequestFlags.FullyResolved);
                if (flattenedEntry.IsDeferredReference)
                {
                    flattenedEntry.Value = ((DeferredReference) flattenedEntry.Value).GetValue(flattenedEntry.BaseValueSourceInternal);
                }
                return flattenedEntry.Value;
            }
        }
        [FriendAccessAllowed]
        internal EffectiveValueEntry NewEntry =>
            this._newEntry;
        [FriendAccessAllowed]
        internal BaseValueSourceInternal NewValueSource =>
            this._newEntry.BaseValueSourceInternal;
        [FriendAccessAllowed]
        internal bool IsNewValueModified =>
            this._newEntry.HasModifiers;
        [FriendAccessAllowed]
        internal bool IsNewValueDeferred =>
            this._newEntry.IsDeferredReference;
        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool Equals(object obj) => 
            this.Equals((DependencyPropertyChangedEventArgs) obj);

        public bool Equals(DependencyPropertyChangedEventArgs args) => 
            ((((((this._property == args._property) && (this._metadata == args._metadata)) && ((this._oldEntry.Value == args._oldEntry.Value) && (this._newEntry.Value == args._newEntry.Value))) && (((this._flags == args._flags) && (this._oldEntry.BaseValueSourceInternal == args._oldEntry.BaseValueSourceInternal)) && ((this._newEntry.BaseValueSourceInternal == args._newEntry.BaseValueSourceInternal) && (this._oldEntry.HasModifiers == args._oldEntry.HasModifiers)))) && (((this._newEntry.HasModifiers == args._newEntry.HasModifiers) && (this._oldEntry.IsDeferredReference == args._oldEntry.IsDeferredReference)) && (this._newEntry.IsDeferredReference == args._newEntry.IsDeferredReference))) && (this._operationType == args._operationType));

        public static bool operator ==(DependencyPropertyChangedEventArgs left, DependencyPropertyChangedEventArgs right) => 
            left.Equals(right);

        public static bool operator !=(DependencyPropertyChangedEventArgs left, DependencyPropertyChangedEventArgs right) => 
            !left.Equals(right);

        private void WritePrivateFlag(PrivateFlags bit, bool value)
        {
            if (value)
            {
                this._flags = (PrivateFlags) ((byte) (this._flags | bit));
            }
            else
            {
                this._flags = (PrivateFlags) ((byte) (this._flags & ((byte) ~bit)));
            }
        }

        private bool ReadPrivateFlag(PrivateFlags bit) => 
            (((byte) (this._flags & bit)) != 0);
        private enum PrivateFlags : byte
        {
            IsASubPropertyChange = 2,
            IsAValueChange = 1
        }
    }
}

