namespace System.Windows
{
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows.Threading;

    public class PropertyMetadata
    {
        private System.Windows.CoerceValueCallback _coerceValueCallback;
        private static System.Windows.FreezeValueCallback _defaultFreezeValueCallback = new System.Windows.FreezeValueCallback(PropertyMetadata.DefaultFreezeValueCallback);
        private object _defaultValue;
        private static readonly UncommonField<FrugalMapBase> _defaultValueFactoryCache = new UncommonField<FrugalMapBase>();
        [FriendAccessAllowed]
        internal MetadataFlags _flags;
        private System.Windows.FreezeValueCallback _freezeValueCallback;
        private static FrugalMapIterationCallback _promotionCallback = new FrugalMapIterationCallback(PropertyMetadata.DefaultValueCachePromotionCallback);
        private System.Windows.PropertyChangedCallback _propertyChangedCallback;
        private static FrugalMapIterationCallback _removalCallback = new FrugalMapIterationCallback(PropertyMetadata.DefaultValueCacheRemovalCallback);

        public PropertyMetadata()
        {
        }

        public PropertyMetadata(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        public PropertyMetadata(System.Windows.PropertyChangedCallback propertyChangedCallback)
        {
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(object defaultValue, System.Windows.PropertyChangedCallback propertyChangedCallback)
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        public PropertyMetadata(object defaultValue, System.Windows.PropertyChangedCallback propertyChangedCallback, System.Windows.CoerceValueCallback coerceValueCallback)
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
            this.CoerceValueCallback = coerceValueCallback;
        }

        internal void ClearCachedDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            FrugalMapBase base2 = _defaultValueFactoryCache.GetValue(owner);
            if (base2.Count == 1)
            {
                _defaultValueFactoryCache.ClearValue(owner);
            }
            else
            {
                base2.RemoveEntry(property.GlobalIndex);
            }
        }

        internal PropertyMetadata Copy(DependencyProperty dp)
        {
            PropertyMetadata metadata = this.CreateInstance();
            metadata.InvokeMerge(this, dp);
            return metadata;
        }

        internal virtual PropertyMetadata CreateInstance() => 
            new PropertyMetadata();

        private static bool DefaultFreezeValueCallback(DependencyObject d, DependencyProperty dp, EntryIndex entryIndex, PropertyMetadata metadata, bool isChecking)
        {
            if (isChecking && d.HasExpression(entryIndex, dp))
            {
                if (TraceFreezable.IsEnabled)
                {
                    TraceFreezable.Trace(TraceEventType.Warning, TraceFreezable.UnableToFreezeExpression, d, dp, dp.OwnerType);
                }
                return false;
            }
            if (!dp.IsValueType)
            {
                object obj2 = d.GetValueEntry(entryIndex, dp, metadata, RequestFlags.FullyResolved).Value;
                if (obj2 != null)
                {
                    Freezable freezable = obj2 as Freezable;
                    if (freezable != null)
                    {
                        if (!freezable.Freeze(isChecking))
                        {
                            if (TraceFreezable.IsEnabled)
                            {
                                TraceFreezable.Trace(TraceEventType.Warning, TraceFreezable.UnableToFreezeFreezableSubProperty, d, dp, dp.OwnerType);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        DispatcherObject obj3 = obj2 as DispatcherObject;
                        if ((obj3 != null) && (obj3.Dispatcher != null))
                        {
                            if (TraceFreezable.IsEnabled)
                            {
                                TraceFreezable.Trace(TraceEventType.Warning, TraceFreezable.UnableToFreezeDispatcherObjectWithThreadAffinity, new object[] { d, dp, dp.OwnerType, obj3 });
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static void DefaultValueCachePromotionCallback(ArrayList list, int key, object value)
        {
            Freezable freezable = value as Freezable;
            if (freezable != null)
            {
                freezable.FireChanged();
            }
        }

        private static void DefaultValueCacheRemovalCallback(ArrayList list, int key, object value)
        {
            Freezable freezable = value as Freezable;
            if (freezable != null)
            {
                freezable.ClearContextAndHandlers();
                freezable.Freeze();
            }
        }

        internal bool DefaultValueWasSet() => 
            this.IsModified(MetadataFlags.DefaultValueModifiedID);

        private object GetCachedDefaultValue(DependencyObject owner, DependencyProperty property) => 
            _defaultValueFactoryCache.GetValue(owner)?.Search(property.GlobalIndex);

        [FriendAccessAllowed]
        internal object GetDefaultValue(DependencyObject owner, DependencyProperty property)
        {
            DefaultValueFactory factory = this._defaultValue as DefaultValueFactory;
            if (factory == null)
            {
                return this._defaultValue;
            }
            if (owner.IsSealed)
            {
                return factory.DefaultValue;
            }
            object cachedDefaultValue = this.GetCachedDefaultValue(owner, property);
            if (cachedDefaultValue == DependencyProperty.UnsetValue)
            {
                cachedDefaultValue = factory.CreateDefaultValue(owner, property);
                property.ValidateFactoryDefaultValue(cachedDefaultValue);
                this.SetCachedDefaultValue(owner, property, cachedDefaultValue);
            }
            return cachedDefaultValue;
        }

        internal void InvokeMerge(PropertyMetadata baseMetadata, DependencyProperty dp)
        {
            this.Merge(baseMetadata, dp);
        }

        private bool IsModified(MetadataFlags id) => 
            ((id & this._flags) != ((MetadataFlags) 0));

        protected virtual void Merge(PropertyMetadata baseMetadata, DependencyProperty dp)
        {
            if (baseMetadata == null)
            {
                throw new ArgumentNullException("baseMetadata");
            }
            if (this.Sealed)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("TypeMetadataCannotChangeAfterUse"));
            }
            if (!this.IsModified(MetadataFlags.DefaultValueModifiedID))
            {
                this._defaultValue = baseMetadata.DefaultValue;
            }
            if (baseMetadata.PropertyChangedCallback != null)
            {
                Delegate[] invocationList = baseMetadata.PropertyChangedCallback.GetInvocationList();
                if (invocationList.Length > 0)
                {
                    System.Windows.PropertyChangedCallback a = (System.Windows.PropertyChangedCallback) invocationList[0];
                    for (int i = 1; i < invocationList.Length; i++)
                    {
                        a = (System.Windows.PropertyChangedCallback) Delegate.Combine(a, (System.Windows.PropertyChangedCallback) invocationList[i]);
                    }
                    a = (System.Windows.PropertyChangedCallback) Delegate.Combine(a, this._propertyChangedCallback);
                    this._propertyChangedCallback = a;
                }
            }
            if (this._coerceValueCallback == null)
            {
                this._coerceValueCallback = baseMetadata.CoerceValueCallback;
            }
            if (this._freezeValueCallback == null)
            {
                this._freezeValueCallback = baseMetadata.FreezeValueCallback;
            }
        }

        protected virtual void OnApply(DependencyProperty dp, Type targetType)
        {
        }

        internal static void PromoteAllCachedDefaultValues(DependencyObject owner)
        {
            FrugalMapBase base2 = _defaultValueFactoryCache.GetValue(owner);
            if (base2 != null)
            {
                base2.Iterate(null, _promotionCallback);
            }
        }

        [FriendAccessAllowed]
        internal bool ReadFlag(MetadataFlags id) => 
            ((id & this._flags) != ((MetadataFlags) 0));

        internal static void RemoveAllCachedDefaultValues(Freezable owner)
        {
            FrugalMapBase base2 = _defaultValueFactoryCache.GetValue(owner);
            if (base2 != null)
            {
                base2.Iterate(null, _removalCallback);
                _defaultValueFactoryCache.ClearValue(owner);
            }
        }

        internal void Seal(DependencyProperty dp, Type targetType)
        {
            this.OnApply(dp, targetType);
            this.Sealed = true;
        }

        private void SetCachedDefaultValue(DependencyObject owner, DependencyProperty property, object value)
        {
            FrugalMapBase base2 = _defaultValueFactoryCache.GetValue(owner);
            if (base2 == null)
            {
                base2 = new SingleObjectMap();
                _defaultValueFactoryCache.SetValue(owner, base2);
            }
            else if (!(base2 is HashObjectMap))
            {
                FrugalMapBase newMap = new HashObjectMap();
                base2.Promote(newMap);
                base2 = newMap;
                _defaultValueFactoryCache.SetValue(owner, base2);
            }
            base2.InsertEntry(property.GlobalIndex, value);
        }

        private void SetModified(MetadataFlags id)
        {
            this._flags |= id;
        }

        [FriendAccessAllowed]
        internal void WriteFlag(MetadataFlags id, bool value)
        {
            if (value)
            {
                this._flags |= id;
            }
            else
            {
                this._flags &= ~id;
            }
        }

        public System.Windows.CoerceValueCallback CoerceValueCallback
        {
            get => 
                this._coerceValueCallback;
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("TypeMetadataCannotChangeAfterUse"));
                }
                this._coerceValueCallback = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                DefaultValueFactory factory = this._defaultValue as DefaultValueFactory;
                return factory?.DefaultValue;
            }
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("TypeMetadataCannotChangeAfterUse"));
                }
                if (value == DependencyProperty.UnsetValue)
                {
                    throw new ArgumentException(System.Windows.SR.Get("DefaultValueMayNotBeUnset"));
                }
                this._defaultValue = value;
                this.SetModified(MetadataFlags.DefaultValueModifiedID);
            }
        }

        [FriendAccessAllowed]
        internal System.Windows.FreezeValueCallback FreezeValueCallback
        {
            get
            {
                if (this._freezeValueCallback != null)
                {
                    return this._freezeValueCallback;
                }
                return _defaultFreezeValueCallback;
            }
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("TypeMetadataCannotChangeAfterUse"));
                }
                this._freezeValueCallback = value;
            }
        }

        [FriendAccessAllowed]
        internal virtual System.Windows.GetReadOnlyValueCallback GetReadOnlyValueCallback =>
            null;

        internal bool IsDefaultValueModified =>
            this.IsModified(MetadataFlags.DefaultValueModifiedID);

        internal bool IsInherited
        {
            get => 
                ((MetadataFlags.Inherited & this._flags) != ((MetadataFlags) 0));
            set
            {
                if (value)
                {
                    this._flags |= MetadataFlags.Inherited;
                }
                else
                {
                    this._flags &= ~MetadataFlags.Inherited;
                }
            }
        }

        protected bool IsSealed =>
            this.Sealed;

        public System.Windows.PropertyChangedCallback PropertyChangedCallback
        {
            get => 
                this._propertyChangedCallback;
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("TypeMetadataCannotChangeAfterUse"));
                }
                this._propertyChangedCallback = value;
            }
        }

        internal bool Sealed
        {
            [FriendAccessAllowed]
            get => 
                this.ReadFlag(MetadataFlags.SealedID);
            set
            {
                this.WriteFlag(MetadataFlags.SealedID, value);
            }
        }

        internal bool UsingDefaultValueFactory =>
            (this._defaultValue is DefaultValueFactory);

        [FriendAccessAllowed]
        internal enum MetadataFlags : uint
        {
            DefaultValueModifiedID = 1,
            FW_AffectsArrangeID = 0x80,
            FW_AffectsMeasureID = 0x40,
            FW_AffectsParentArrangeID = 0x200,
            FW_AffectsParentMeasureID = 0x100,
            FW_AffectsRenderID = 0x400,
            FW_BindsTwoWayByDefaultID = 0x2000,
            FW_DefaultUpdateSourceTriggerEnumBit1 = 0x40000000,
            FW_DefaultUpdateSourceTriggerEnumBit2 = 0x80000000,
            FW_DefaultUpdateSourceTriggerModifiedID = 0x4000000,
            FW_InheritsModifiedID = 0x100000,
            FW_IsNotDataBindableID = 0x1000,
            FW_OverridesInheritanceBehaviorID = 0x800,
            FW_OverridesInheritanceBehaviorModifiedID = 0x200000,
            FW_ReadOnlyID = 0x8000000,
            FW_ShouldBeJournaledID = 0x4000,
            FW_ShouldBeJournaledModifiedID = 0x1000000,
            FW_SubPropertiesDoNotAffectRenderID = 0x8000,
            FW_SubPropertiesDoNotAffectRenderModifiedID = 0x10000,
            FW_UpdatesSourceOnLostFocusByDefaultID = 0x2000000,
            Inherited = 0x10,
            SealedID = 2,
            UI_IsAnimationProhibitedID = 0x20
        }
    }
}

