namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Windows;

    [FriendAccessAllowed]
    internal class FreezableDefaultValueFactory : DefaultValueFactory
    {
        private readonly Freezable _defaultValuePrototype;

        internal FreezableDefaultValueFactory(Freezable defaultValue)
        {
            this._defaultValuePrototype = defaultValue.GetAsFrozen();
        }

        internal override object CreateDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            Freezable mutableDefaultValue = this._defaultValuePrototype;
            Freezable freezable2 = owner as Freezable;
            if ((freezable2 == null) || !freezable2.IsFrozen)
            {
                mutableDefaultValue = this._defaultValuePrototype.Clone();
                FreezableDefaultPromoter promoter = new FreezableDefaultPromoter(owner, property);
                promoter.SetFreezableDefaultValue(mutableDefaultValue);
                mutableDefaultValue.Changed += new EventHandler(promoter.OnDefaultValueChanged);
            }
            return mutableDefaultValue;
        }

        internal override object DefaultValue =>
            this._defaultValuePrototype;

        private class FreezableDefaultPromoter
        {
            private Freezable _mutableDefaultValue;
            private readonly DependencyObject _owner;
            private readonly DependencyProperty _property;

            internal FreezableDefaultPromoter(DependencyObject owner, DependencyProperty property)
            {
                this._owner = owner;
                this._property = property;
            }

            internal void OnDefaultValueChanged(object sender, EventArgs e)
            {
                this._property.GetMetadata(this._owner.DependencyObjectType).ClearCachedDefaultValue(this._owner, this._property);
                if (!this._mutableDefaultValue.IsFrozen)
                {
                    this._mutableDefaultValue.Changed -= new EventHandler(this.OnDefaultValueChanged);
                }
                if (this._owner.ReadLocalValue(this._property) == DependencyProperty.UnsetValue)
                {
                    this._owner.SetMutableDefaultValue(this._property, this._mutableDefaultValue);
                }
            }

            internal void SetFreezableDefaultValue(Freezable mutableDefaultValue)
            {
                this._mutableDefaultValue = mutableDefaultValue;
            }
        }
    }
}

