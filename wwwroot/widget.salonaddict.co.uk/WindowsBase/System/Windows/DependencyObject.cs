namespace System.Windows
{
    using MS.Internal.ComponentModel;
    using MS.Internal.KnownBoxes;
    using MS.Internal.WindowsBase;
    using MS.Utility;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Threading;

    [TypeDescriptionProvider(typeof(DependencyObjectProvider))]
    public class DependencyObject : DispatcherObject
    {
        internal object _contextStorage;
        private System.Windows.DependencyObjectType _dType;
        private EffectiveValueEntry[] _effectiveValues;
        private static AlternativeExpressionStorageCallback _getExpressionCore;
        private uint _packedData;
        internal static readonly UncommonField<object> DependentListMapField = new UncommonField<object>();
        internal static System.Windows.DependencyObjectType DType = System.Windows.DependencyObjectType.FromSystemTypeInternal(typeof(DependencyObject));
        [FriendAccessAllowed]
        internal static readonly object ExpressionInAlternativeStore = new NamedObject("ExpressionInAlternativeStore");
        private static readonly UncommonField<EventHandler> InheritanceContextChangedHandlersField = new UncommonField<EventHandler>();
        private const int NestedOperationMaximum = 0x99;

        internal event EventHandler InheritanceContextChanged
        {
            [FriendAccessAllowed] add
            {
                EventHandler a = InheritanceContextChangedHandlersField.GetValue(this);
                if (a != null)
                {
                    a = (EventHandler) Delegate.Combine(a, value);
                }
                else
                {
                    a = value;
                }
                InheritanceContextChangedHandlersField.SetValue(this, a);
            }
            [FriendAccessAllowed] remove
            {
                EventHandler source = InheritanceContextChangedHandlersField.GetValue(this);
                if (source != null)
                {
                    source = (EventHandler) Delegate.Remove(source, value);
                    if (source == null)
                    {
                        InheritanceContextChangedHandlersField.ClearValue(this);
                    }
                    else
                    {
                        InheritanceContextChangedHandlersField.SetValue(this, source);
                    }
                }
            }
        }

        public DependencyObject()
        {
            this.Initialize();
        }

        internal virtual void AddInheritanceContext(DependencyObject context, DependencyProperty property)
        {
        }

        [FriendAccessAllowed]
        internal void BeginPropertyInitialization()
        {
            this.IsInPropertyInitialization = true;
        }

        internal static void ChangeExpressionSources(Expression expr, DependencyObject d, DependencyProperty dp, DependencySource[] newSources)
        {
            if (!expr.ForwardsInvalidations)
            {
                EntryIndex entry = d.LookupEntry(dp.GlobalIndex);
                if (!entry.Found || (d._effectiveValues[entry.Index].LocalValue != expr))
                {
                    throw new ArgumentException(System.Windows.SR.Get("SourceChangeExpressionMismatch"));
                }
            }
            DependencySource[] sources = expr.GetSources();
            if (sources != null)
            {
                UpdateSourceDependentLists(d, dp, sources, expr, false);
            }
            if (newSources != null)
            {
                UpdateSourceDependentLists(d, dp, newSources, expr, true);
            }
        }

        private EntryIndex CheckEntryIndex(EntryIndex entryIndex, int targetIndex)
        {
            if ((this.EffectiveValuesCount > 0) && (this._effectiveValues.Length > entryIndex.Index))
            {
                EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
                if (entry.PropertyIndex == targetIndex)
                {
                    return new EntryIndex(entryIndex.Index);
                }
            }
            return this.LookupEntry(targetIndex);
        }

        public void ClearValue(DependencyProperty dp)
        {
            base.VerifyAccess();
            PropertyMetadata metadata = this.SetupPropertyChange(dp);
            EntryIndex entry = this.LookupEntry(dp.GlobalIndex);
            this.ClearValueCommon(entry, dp, metadata);
        }

        public void ClearValue(DependencyPropertyKey key)
        {
            DependencyProperty property;
            base.VerifyAccess();
            PropertyMetadata metadata = this.SetupPropertyChange(key, out property);
            EntryIndex entry = this.LookupEntry(property.GlobalIndex);
            this.ClearValueCommon(entry, property, metadata);
        }

        private void ClearValueCommon(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata)
        {
            if (this.IsSealed)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("ClearOnReadOnlyObjectNotAllowed", new object[] { this }));
            }
            EffectiveValueEntry oldEntry = this.GetValueEntry(entryIndex, dp, metadata, RequestFlags.RawEntry);
            object localValue = oldEntry.LocalValue;
            Expression expr = oldEntry.IsExpression ? (localValue as Expression) : null;
            if (expr != null)
            {
                DependencySource[] sources = expr.GetSources();
                UpdateSourceDependentLists(this, dp, sources, expr, false);
                expr.OnDetach(this, dp);
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
            }
            EffectiveValueEntry newEntry = new EffectiveValueEntry(dp, BaseValueSourceInternal.Local);
            this.UpdateEffectiveValue(entryIndex, dp, metadata, oldEntry, ref newEntry, false, OperationType.Unknown);
        }

        public void CoerceValue(DependencyProperty dp)
        {
            base.VerifyAccess();
            EffectiveValueEntry newEntry = new EffectiveValueEntry(dp, FullValueSource.IsCoerced);
            this.UpdateEffectiveValue(this.LookupEntry(dp.GlobalIndex), dp, dp.GetMetadata(this.DependencyObjectType), new EffectiveValueEntry(), ref newEntry, false, OperationType.Unknown);
        }

        internal bool ContainsValue(DependencyProperty dp)
        {
            EntryIndex entry = this.LookupEntry(dp.GlobalIndex);
            if (!entry.Found)
            {
                return false;
            }
            return !object.ReferenceEquals(this._effectiveValues[entry.Index].LocalValue, DependencyProperty.UnsetValue);
        }

        [Conditional("DEBUG")]
        internal void Debug_AssertNoInheritanceContextListeners()
        {
        }

        [FriendAccessAllowed]
        internal void EndPropertyInitialization()
        {
            this.IsInPropertyInitialization = false;
            if (this._effectiveValues != null)
            {
                uint effectiveValuesCount = this.EffectiveValuesCount;
                if (effectiveValuesCount != 0)
                {
                    uint num2 = effectiveValuesCount;
                    if ((((float) num2) / ((float) this._effectiveValues.Length)) < 0.8)
                    {
                        EffectiveValueEntry[] destinationArray = new EffectiveValueEntry[num2];
                        Array.Copy(this._effectiveValues, 0L, destinationArray, 0L, (long) effectiveValuesCount);
                        this._effectiveValues = destinationArray;
                    }
                }
            }
        }

        public sealed override bool Equals(object obj) => 
            base.Equals(obj);

        private bool Equals(DependencyProperty dp, object value1, object value2)
        {
            if (!dp.IsValueType && !dp.IsStringType)
            {
                return object.ReferenceEquals(value1, value2);
            }
            return object.Equals(value1, value2);
        }

        internal virtual void EvaluateAnimatedValueCore(DependencyProperty dp, PropertyMetadata metadata, ref EffectiveValueEntry newEntry)
        {
        }

        internal virtual void EvaluateBaseValueCore(DependencyProperty dp, PropertyMetadata metadata, ref EffectiveValueEntry newEntry)
        {
        }

        private EffectiveValueEntry EvaluateEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, EffectiveValueEntry oldEntry, EffectiveValueEntry newEntry, OperationType operationType)
        {
            object unsetValue = DependencyProperty.UnsetValue;
            bool flag = newEntry.BaseValueSourceInternal == BaseValueSourceInternal.Local;
            bool flag2 = flag && (newEntry.Value == DependencyProperty.UnsetValue);
            bool flag3 = false;
            if (flag2)
            {
                newEntry.BaseValueSourceInternal = BaseValueSourceInternal.Unknown;
            }
            else
            {
                unsetValue = flag ? newEntry.LocalValue : oldEntry.LocalValue;
                if (unsetValue == ExpressionInAlternativeStore)
                {
                    unsetValue = DependencyProperty.UnsetValue;
                }
                else
                {
                    flag3 = flag ? newEntry.IsExpression : oldEntry.IsExpression;
                }
            }
            if (unsetValue != DependencyProperty.UnsetValue)
            {
                newEntry = new EffectiveValueEntry(dp, BaseValueSourceInternal.Local);
                newEntry.Value = unsetValue;
                if (flag3)
                {
                    newEntry = this.EvaluateExpression(entryIndex, dp, (Expression) unsetValue, metadata, oldEntry, newEntry);
                    entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                    unsetValue = newEntry.ModifiedValue.ExpressionValue;
                }
            }
            if (!dp.ReadOnly)
            {
                this.EvaluateBaseValueCore(dp, metadata, ref newEntry);
                if (newEntry.BaseValueSourceInternal == BaseValueSourceInternal.Unknown)
                {
                    newEntry = EffectiveValueEntry.CreateDefaultValueEntry(dp, metadata.GetDefaultValue(this, dp));
                }
                unsetValue = newEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value;
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                if (oldEntry.IsAnimated)
                {
                    newEntry.ResetCoercedValue();
                    this.EvaluateAnimatedValueCore(dp, metadata, ref newEntry);
                    unsetValue = newEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value;
                }
            }
            if (unsetValue == DependencyProperty.UnsetValue)
            {
                newEntry = EffectiveValueEntry.CreateDefaultValueEntry(dp, metadata.GetDefaultValue(this, dp));
            }
            return newEntry;
        }

        private EffectiveValueEntry EvaluateExpression(EntryIndex entryIndex, DependencyProperty dp, Expression expr, PropertyMetadata metadata, EffectiveValueEntry oldEntry, EffectiveValueEntry newEntry)
        {
            object unsetValue = expr.GetValue(this, dp);
            if ((unsetValue != DependencyProperty.UnsetValue) && (unsetValue != Expression.NoValue))
            {
                if (!(unsetValue is DeferredReference) && !dp.IsValidValue(unsetValue))
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("InvalidPropertyValue", new object[] { unsetValue, dp.Name }));
                }
            }
            else
            {
                if (unsetValue == Expression.NoValue)
                {
                    newEntry.SetExpressionValue(Expression.NoValue, expr);
                    if (!dp.ReadOnly)
                    {
                        this.EvaluateBaseValueCore(dp, metadata, ref newEntry);
                        unsetValue = newEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value;
                    }
                    else
                    {
                        unsetValue = DependencyProperty.UnsetValue;
                    }
                }
                if (unsetValue == DependencyProperty.UnsetValue)
                {
                    unsetValue = metadata.GetDefaultValue(this, dp);
                }
            }
            newEntry.SetExpressionValue(unsetValue, expr);
            return newEntry;
        }

        private EffectiveValueEntry GetEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, RequestFlags requests)
        {
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            EffectiveValueEntry flattenedEntry = entry.GetFlattenedEntry(requests);
            if (((requests & (RequestFlags.RawEntry | RequestFlags.DeferredReferences)) == RequestFlags.FullyResolved) && flattenedEntry.IsDeferredReference)
            {
                if (!entry.HasModifiers)
                {
                    if (entry.HasExpressionMarker)
                    {
                        return flattenedEntry;
                    }
                    object obj2 = ((DeferredReference) entry.Value).GetValue(entry.BaseValueSourceInternal);
                    if (!dp.IsValidValue(obj2))
                    {
                        throw new InvalidOperationException(System.Windows.SR.Get("InvalidPropertyValue", new object[] { obj2, dp.Name }));
                    }
                    entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                    entry.Value = obj2;
                    this._effectiveValues[entryIndex.Index] = entry;
                    return entry;
                }
                ModifiedValue modifiedValue = entry.ModifiedValue;
                object obj3 = ((DeferredReference) modifiedValue.ExpressionValue).GetValue(entry.BaseValueSourceInternal);
                if (!dp.IsValidValue(obj3))
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("InvalidPropertyValue", new object[] { obj3, dp.Name }));
                }
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                modifiedValue.ExpressionValue = obj3;
                this._effectiveValues[entryIndex.Index] = entry;
                flattenedEntry.Value = obj3;
            }
            return flattenedEntry;
        }

        private static Expression GetExpression(DependencyObject d, DependencyProperty dp, PropertyMetadata metadata)
        {
            EntryIndex index = d.LookupEntry(dp.GlobalIndex);
            if (index.Found)
            {
                EffectiveValueEntry entry = d._effectiveValues[index.Index];
                if (entry.HasExpressionMarker)
                {
                    if (_getExpressionCore != null)
                    {
                        return _getExpressionCore(d, dp, metadata);
                    }
                    return null;
                }
                if (entry.IsExpression)
                {
                    return (Expression) entry.LocalValue;
                }
            }
            return null;
        }

        public sealed override int GetHashCode() => 
            base.GetHashCode();

        public LocalValueEnumerator GetLocalValueEnumerator()
        {
            base.VerifyAccess();
            uint effectiveValuesCount = this.EffectiveValuesCount;
            LocalValueEntry[] snapshot = new LocalValueEntry[effectiveValuesCount];
            int count = 0;
            for (uint i = 0; i < effectiveValuesCount; i++)
            {
                DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[this._effectiveValues[i].PropertyIndex];
                if (dp != null)
                {
                    object obj2 = this.ReadLocalValueEntry(new EntryIndex(i), dp, false);
                    if (obj2 != DependencyProperty.UnsetValue)
                    {
                        snapshot[count++] = new LocalValueEntry(dp, obj2);
                    }
                }
            }
            return new LocalValueEnumerator(snapshot, count);
        }

        public object GetValue(DependencyProperty dp)
        {
            base.VerifyAccess();
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            return this.GetValueEntry(this.LookupEntry(dp.GlobalIndex), dp, null, RequestFlags.FullyResolved).Value;
        }

        [FriendAccessAllowed]
        internal EffectiveValueEntry GetValueEntry(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, RequestFlags requests)
        {
            EffectiveValueEntry entry;
            if (dp.ReadOnly)
            {
                if (metadata == null)
                {
                    metadata = dp.GetMetadata(this.DependencyObjectType);
                }
                GetReadOnlyValueCallback getReadOnlyValueCallback = metadata.GetReadOnlyValueCallback;
                if (getReadOnlyValueCallback != null)
                {
                    BaseValueSourceInternal internal2;
                    return new EffectiveValueEntry(dp) { 
                        Value = getReadOnlyValueCallback(this, out internal2),
                        BaseValueSourceInternal = internal2
                    };
                }
            }
            if (entryIndex.Found)
            {
                if ((requests & RequestFlags.RawEntry) != RequestFlags.FullyResolved)
                {
                    entry = this._effectiveValues[entryIndex.Index];
                }
                else
                {
                    entry = this.GetEffectiveValue(entryIndex, dp, requests);
                }
            }
            else
            {
                entry = new EffectiveValueEntry(dp, BaseValueSourceInternal.Unknown);
            }
            if (entry.Value != DependencyProperty.UnsetValue)
            {
                return entry;
            }
            if (dp.IsPotentiallyInherited)
            {
                if (metadata == null)
                {
                    metadata = dp.GetMetadata(this.DependencyObjectType);
                }
                if (metadata.IsInherited)
                {
                    DependencyObject inheritanceParent = this.InheritanceParent;
                    if (inheritanceParent != null)
                    {
                        entryIndex = inheritanceParent.LookupEntry(dp.GlobalIndex);
                        if (entryIndex.Found)
                        {
                            entry = inheritanceParent.GetEffectiveValue(entryIndex, dp, requests & RequestFlags.DeferredReferences);
                            entry.BaseValueSourceInternal = BaseValueSourceInternal.Inherited;
                        }
                    }
                }
                if (entry.Value != DependencyProperty.UnsetValue)
                {
                    return entry;
                }
            }
            if ((requests & RequestFlags.SkipDefault) != RequestFlags.FullyResolved)
            {
                return entry;
            }
            if (dp.IsPotentiallyUsingDefaultValueFactory)
            {
                if (metadata == null)
                {
                    metadata = dp.GetMetadata(this.DependencyObjectType);
                }
                if (((requests & (RequestFlags.RawEntry | RequestFlags.DeferredReferences)) != RequestFlags.FullyResolved) && metadata.UsingDefaultValueFactory)
                {
                    entry.BaseValueSourceInternal = BaseValueSourceInternal.Default;
                    entry.Value = new DeferredMutableDefaultReference(metadata, this, dp);
                    return entry;
                }
            }
            else if (!dp.IsDefaultValueChanged)
            {
                return EffectiveValueEntry.CreateDefaultValueEntry(dp, dp.DefaultMetadata.DefaultValue);
            }
            if (metadata == null)
            {
                metadata = dp.GetMetadata(this.DependencyObjectType);
            }
            return EffectiveValueEntry.CreateDefaultValueEntry(dp, metadata.GetDefaultValue(this, dp));
        }

        [FriendAccessAllowed]
        internal BaseValueSourceInternal GetValueSource(DependencyProperty dp, PropertyMetadata metadata, out bool hasModifiers)
        {
            bool flag;
            bool flag2;
            bool flag3;
            return this.GetValueSource(dp, metadata, out hasModifiers, out flag, out flag2, out flag3);
        }

        [FriendAccessAllowed]
        internal BaseValueSourceInternal GetValueSource(DependencyProperty dp, PropertyMetadata metadata, out bool hasModifiers, out bool isExpression, out bool isAnimated, out bool isCoerced)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            EntryIndex index = this.LookupEntry(dp.GlobalIndex);
            if (index.Found)
            {
                EffectiveValueEntry entry = this._effectiveValues[index.Index];
                hasModifiers = entry.HasModifiers;
                isExpression = entry.IsExpression;
                isAnimated = entry.IsAnimated;
                isCoerced = entry.IsCoerced;
                return entry.BaseValueSourceInternal;
            }
            isExpression = false;
            isAnimated = false;
            isCoerced = false;
            if (dp.ReadOnly)
            {
                if (metadata == null)
                {
                    metadata = dp.GetMetadata(this.DependencyObjectType);
                }
                GetReadOnlyValueCallback getReadOnlyValueCallback = metadata.GetReadOnlyValueCallback;
                if (getReadOnlyValueCallback != null)
                {
                    BaseValueSourceInternal internal2;
                    getReadOnlyValueCallback(this, out internal2);
                    hasModifiers = false;
                    return internal2;
                }
            }
            if (dp.IsPotentiallyInherited)
            {
                if (metadata == null)
                {
                    metadata = dp.GetMetadata(this.DependencyObjectType);
                }
                if (metadata.IsInherited)
                {
                    DependencyObject inheritanceParent = this.InheritanceParent;
                    if ((inheritanceParent != null) && inheritanceParent.LookupEntry(dp.GlobalIndex).Found)
                    {
                        hasModifiers = false;
                        return BaseValueSourceInternal.Inherited;
                    }
                }
            }
            hasModifiers = false;
            return BaseValueSourceInternal.Default;
        }

        internal bool HasAnyExpression()
        {
            EffectiveValueEntry[] effectiveValues = this.EffectiveValues;
            uint effectiveValuesCount = this.EffectiveValuesCount;
            for (uint i = 0; i < effectiveValuesCount; i++)
            {
                DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[effectiveValues[i].PropertyIndex];
                if (dp != null)
                {
                    EntryIndex entryIndex = new EntryIndex(i);
                    if (this.HasExpression(entryIndex, dp))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [FriendAccessAllowed]
        internal bool HasExpression(EntryIndex entryIndex, DependencyProperty dp)
        {
            if (!entryIndex.Found)
            {
                return false;
            }
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            object localValue = entry.LocalValue;
            return (entry.HasExpressionMarker || (localValue is Expression));
        }

        private void Initialize()
        {
            this.CanBeInheritanceContext = true;
            this.CanModifyEffectiveValues = true;
        }

        private void InsertEntry(EffectiveValueEntry entry, uint entryIndex)
        {
            if (!this.CanModifyEffectiveValues)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("LocalValueEnumerationInvalidated"));
            }
            uint effectiveValuesCount = this.EffectiveValuesCount;
            if (effectiveValuesCount > 0)
            {
                if (this._effectiveValues.Length == effectiveValuesCount)
                {
                    int num2 = (int) (effectiveValuesCount * (this.IsInPropertyInitialization ? 2.0 : 1.2));
                    if (num2 == effectiveValuesCount)
                    {
                        num2++;
                    }
                    EffectiveValueEntry[] destinationArray = new EffectiveValueEntry[num2];
                    Array.Copy(this._effectiveValues, 0L, destinationArray, 0L, (long) entryIndex);
                    destinationArray[entryIndex] = entry;
                    Array.Copy(this._effectiveValues, (long) entryIndex, destinationArray, (long) (entryIndex + 1), (long) (effectiveValuesCount - entryIndex));
                    this._effectiveValues = destinationArray;
                }
                else
                {
                    Array.Copy(this._effectiveValues, (long) entryIndex, this._effectiveValues, (long) (entryIndex + 1), (long) (effectiveValuesCount - entryIndex));
                    this._effectiveValues[entryIndex] = entry;
                }
            }
            else
            {
                if (this._effectiveValues == null)
                {
                    this._effectiveValues = new EffectiveValueEntry[this.EffectiveValuesInitialSize];
                }
                this._effectiveValues[0] = entry;
            }
            this.EffectiveValuesCount = effectiveValuesCount + 1;
        }

        public void InvalidateProperty(DependencyProperty dp)
        {
            base.VerifyAccess();
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            EffectiveValueEntry newEntry = new EffectiveValueEntry(dp, BaseValueSourceInternal.Unknown);
            this.UpdateEffectiveValue(this.LookupEntry(dp.GlobalIndex), dp, dp.GetMetadata(this.DependencyObjectType), new EffectiveValueEntry(), ref newEntry, false, OperationType.Unknown);
        }

        [FriendAccessAllowed]
        internal void InvalidateSubProperty(DependencyProperty dp)
        {
            this.NotifyPropertyChange(new DependencyPropertyChangedEventArgs(dp, dp.GetMetadata(this.DependencyObjectType), this.GetValue(dp)));
        }

        [FriendAccessAllowed]
        internal EntryIndex LookupEntry(int targetIndex)
        {
            uint index = 0;
            uint effectiveValuesCount = this.EffectiveValuesCount;
            if (effectiveValuesCount > 0)
            {
                int propertyIndex;
                while ((effectiveValuesCount - index) > 3)
                {
                    uint num4 = (effectiveValuesCount + index) / 2;
                    propertyIndex = this._effectiveValues[num4].PropertyIndex;
                    if (targetIndex == propertyIndex)
                    {
                        return new EntryIndex(num4);
                    }
                    if (targetIndex <= propertyIndex)
                    {
                        effectiveValuesCount = num4;
                    }
                    else
                    {
                        index = num4 + 1;
                    }
                }
            Label_004B:
                propertyIndex = this._effectiveValues[index].PropertyIndex;
                if (propertyIndex == targetIndex)
                {
                    return new EntryIndex(index);
                }
                if (propertyIndex <= targetIndex)
                {
                    index++;
                    if (index < effectiveValuesCount)
                    {
                        goto Label_004B;
                    }
                }
                return new EntryIndex(index, false);
            }
            return new EntryIndex(0, false);
        }

        private void MergeInheritableProperties(DependencyObject inheritanceParent)
        {
            EffectiveValueEntry[] effectiveValues = inheritanceParent.EffectiveValues;
            uint effectiveValuesCount = inheritanceParent.EffectiveValuesCount;
            for (uint i = 0; i < effectiveValuesCount; i++)
            {
                EffectiveValueEntry entry = effectiveValues[i];
                DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[entry.PropertyIndex];
                if (dp != null)
                {
                    PropertyMetadata metadata = dp.GetMetadata(this.DependencyObjectType);
                    if (metadata.IsInherited)
                    {
                        object obj2 = inheritanceParent.GetValueEntry(new EntryIndex(i), dp, metadata, RequestFlags.SkipDefault | RequestFlags.DeferredReferences).Value;
                        if (obj2 != DependencyProperty.UnsetValue)
                        {
                            EntryIndex entryIndex = this.LookupEntry(dp.GlobalIndex);
                            this.SetEffectiveValue(entryIndex, dp, dp.GlobalIndex, metadata, obj2, BaseValueSourceInternal.Inherited);
                        }
                    }
                }
            }
        }

        [FriendAccessAllowed]
        internal void NotifyPropertyChange(DependencyPropertyChangedEventArgs args)
        {
            this.OnPropertyChanged(args);
            if (args.IsAValueChange || args.IsASubPropertyChange)
            {
                DependencyProperty property = args.Property;
                object obj2 = DependentListMapField.GetValue(this);
                if (obj2 != null)
                {
                    FrugalMap map = (FrugalMap) obj2;
                    object obj3 = map[property.GlobalIndex];
                    if (obj3 != DependencyProperty.UnsetValue)
                    {
                        if (((DependentList) obj3).IsEmpty)
                        {
                            map[property.GlobalIndex] = DependencyProperty.UnsetValue;
                        }
                        else
                        {
                            ((DependentList) obj3).InvalidateDependents(this, args);
                        }
                    }
                }
            }
        }

        [FriendAccessAllowed]
        internal void NotifySubPropertyChange(DependencyProperty dp)
        {
            this.InvalidateSubProperty(dp);
            Freezable freezable = this as Freezable;
            if (freezable != null)
            {
                freezable.FireChanged();
            }
        }

        [FriendAccessAllowed]
        internal void OnInheritanceContextChanged(EventArgs args)
        {
            EventHandler handler = InheritanceContextChangedHandlersField.GetValue(this);
            if (handler != null)
            {
                handler(this, args);
            }
            this.CanModifyEffectiveValues = false;
            try
            {
                uint effectiveValuesCount = this.EffectiveValuesCount;
                for (uint i = 0; i < effectiveValuesCount; i++)
                {
                    DependencyProperty dp = DependencyProperty.RegisteredPropertyList.List[this._effectiveValues[i].PropertyIndex];
                    if (dp != null)
                    {
                        object obj2 = this.ReadLocalValueEntry(new EntryIndex(i), dp, true);
                        if (obj2 != DependencyProperty.UnsetValue)
                        {
                            DependencyObject obj3 = obj2 as DependencyObject;
                            if ((obj3 != null) && (obj3.InheritanceContext == this))
                            {
                                obj3.OnInheritanceContextChanged(args);
                            }
                        }
                    }
                }
            }
            finally
            {
                this.CanModifyEffectiveValues = true;
            }
            this.OnInheritanceContextChangedCore(args);
        }

        [FriendAccessAllowed]
        internal virtual void OnInheritanceContextChangedCore(EventArgs args)
        {
        }

        protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == null)
            {
                throw new ArgumentNullException("e.Property");
            }
            if ((e.IsAValueChange || e.IsASubPropertyChange) || (e.OperationType == OperationType.ChangeMutableDefaultValue))
            {
                PropertyMetadata metadata = e.Metadata;
                if ((metadata != null) && (metadata.PropertyChangedCallback != null))
                {
                    metadata.PropertyChangedCallback(this, e);
                }
            }
        }

        private void ProcessCoerceValue(DependencyProperty dp, PropertyMetadata metadata, ref EntryIndex entryIndex, ref int targetIndex, ref EffectiveValueEntry newEntry, ref EffectiveValueEntry oldEntry, ref object oldValue, object baseValue, CoerceValueCallback coerceValueCallback, bool coerceWithDeferredReference, bool skipBaseValueChecks)
        {
            if (newEntry.IsDeferredReference && (!coerceWithDeferredReference || (dp.OwnerType != metadata.CoerceValueCallback.Method.DeclaringType)))
            {
                baseValue = ((DeferredReference) baseValue).GetValue(newEntry.BaseValueSourceInternal);
                newEntry.SetCoersionBaseValue(baseValue);
                entryIndex = this.CheckEntryIndex(entryIndex, targetIndex);
            }
            object obj2 = coerceValueCallback(this, baseValue);
            entryIndex = this.CheckEntryIndex(entryIndex, targetIndex);
            if (!this.Equals(dp, obj2, baseValue))
            {
                if (obj2 == DependencyProperty.UnsetValue)
                {
                    if (oldEntry.IsDeferredReference)
                    {
                        oldValue = ((DeferredReference) oldValue).GetValue(oldEntry.BaseValueSourceInternal);
                        entryIndex = this.CheckEntryIndex(entryIndex, targetIndex);
                    }
                    obj2 = oldValue;
                }
                if (!dp.IsValidValue(obj2))
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidPropertyValue", new object[] { obj2, dp.Name }));
                }
                newEntry.SetCoercedValue(obj2, baseValue, skipBaseValueChecks);
            }
        }

        [FriendAccessAllowed]
        internal bool ProvideSelfAsInheritanceContext(object value, DependencyProperty dp)
        {
            DependencyObject doValue = value as DependencyObject;
            return ((doValue != null) && this.ProvideSelfAsInheritanceContext(doValue, dp));
        }

        [FriendAccessAllowed]
        internal bool ProvideSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (((doValue == null) || !this.ShouldProvideInheritanceContext(doValue, dp)) || (!(doValue is Freezable) && (!this.CanBeInheritanceContext || doValue.IsInheritanceContextSealed)))
            {
                return false;
            }
            DependencyObject inheritanceContext = doValue.InheritanceContext;
            doValue.AddInheritanceContext(this, dp);
            return ((this == doValue.InheritanceContext) && (this != inheritanceContext));
        }

        public object ReadLocalValue(DependencyProperty dp)
        {
            base.VerifyAccess();
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            EntryIndex entry = this.LookupEntry(dp.GlobalIndex);
            return this.ReadLocalValueEntry(entry, dp, false);
        }

        internal object ReadLocalValueEntry(EntryIndex entryIndex, DependencyProperty dp, bool allowDeferredReferences)
        {
            if (!entryIndex.Found)
            {
                return DependencyProperty.UnsetValue;
            }
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            object localValue = entry.LocalValue;
            if (!allowDeferredReferences && entry.IsDeferredReference)
            {
                DeferredReference reference = localValue as DeferredReference;
                if (reference != null)
                {
                    localValue = reference.GetValue(entry.BaseValueSourceInternal);
                }
            }
            if (localValue == ExpressionInAlternativeStore)
            {
                localValue = DependencyProperty.UnsetValue;
            }
            return localValue;
        }

        [FriendAccessAllowed]
        internal static void RegisterForAlternativeExpressionStorage(AlternativeExpressionStorageCallback getExpressionCore, out AlternativeExpressionStorageCallback getExpression)
        {
            _getExpressionCore = getExpressionCore;
            getExpression = new AlternativeExpressionStorageCallback(DependencyObject.GetExpression);
        }

        private void RemoveEntry(uint entryIndex, DependencyProperty dp)
        {
            if (!this.CanModifyEffectiveValues)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("LocalValueEnumerationInvalidated"));
            }
            uint effectiveValuesCount = this.EffectiveValuesCount;
            Array.Copy(this._effectiveValues, (long) (entryIndex + 1), this._effectiveValues, (long) entryIndex, (long) ((effectiveValuesCount - entryIndex) - 1));
            effectiveValuesCount--;
            this.EffectiveValuesCount = effectiveValuesCount;
            this._effectiveValues[effectiveValuesCount].Clear();
        }

        internal virtual void RemoveInheritanceContext(DependencyObject context, DependencyProperty property)
        {
        }

        [FriendAccessAllowed]
        internal bool RemoveSelfAsInheritanceContext(object value, DependencyProperty dp)
        {
            DependencyObject doValue = value as DependencyObject;
            return ((doValue != null) && this.RemoveSelfAsInheritanceContext(doValue, dp));
        }

        [FriendAccessAllowed]
        internal bool RemoveSelfAsInheritanceContext(DependencyObject doValue, DependencyProperty dp)
        {
            if (((doValue == null) || !this.ShouldProvideInheritanceContext(doValue, dp)) || (!(doValue is Freezable) && (!this.CanBeInheritanceContext || doValue.IsInheritanceContextSealed)))
            {
                return false;
            }
            DependencyObject inheritanceContext = doValue.InheritanceContext;
            doValue.RemoveInheritanceContext(this, dp);
            return ((this == inheritanceContext) && (doValue.InheritanceContext != inheritanceContext));
        }

        [FriendAccessAllowed]
        internal virtual void Seal()
        {
            PropertyMetadata.PromoteAllCachedDefaultValues(this);
            DependentListMapField.ClearValue(this);
            this.DO_Sealed = true;
        }

        [FriendAccessAllowed]
        internal void SetAnimatedValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, object value, object baseValue)
        {
            if (baseValue == DependencyProperty.UnsetValue)
            {
                baseValue = metadata.GetDefaultValue(this, dp);
                if (!entryIndex.Found || (this._effectiveValues[entryIndex.Index].BaseValueSourceInternal == BaseValueSourceInternal.Unknown))
                {
                    this.SetEffectiveValue(entryIndex, dp, dp.GlobalIndex, metadata, baseValue, BaseValueSourceInternal.Default);
                    entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                }
            }
            else if ((!entryIndex.Found && !this.IsSelfInheritanceParent) && metadata.IsInherited)
            {
                this.SetIsSelfInheritanceParent();
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
            }
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            entry.SetAnimatedValue(value, baseValue);
            entry.ResetCoercedValue();
            this._effectiveValues[entryIndex.Index] = entry;
        }

        private EffectiveValueEntry SetCoercedValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, object value, object baseValue)
        {
            object defaultValue = metadata.GetDefaultValue(this, dp);
            if (this.Equals(dp, baseValue, defaultValue))
            {
                if (!entryIndex.Found || (this._effectiveValues[entryIndex.Index].BaseValueSourceInternal == BaseValueSourceInternal.Unknown))
                {
                    this.SetEffectiveValue(entryIndex, dp, dp.GlobalIndex, metadata, defaultValue, BaseValueSourceInternal.Default);
                    entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                }
            }
            else if ((!entryIndex.Found && !this.IsSelfInheritanceParent) && metadata.IsInherited)
            {
                this.SetIsSelfInheritanceParent();
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
            }
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            entry.SetCoercedValue(value, baseValue, false);
            this._effectiveValues[entryIndex.Index] = entry;
            return entry;
        }

        [FriendAccessAllowed]
        internal void SetDeferredValue(DependencyProperty dp, DeferredReference deferredReference)
        {
            PropertyMetadata metadata = this.SetupPropertyChange(dp);
            this.SetValueCommon(dp, deferredReference, metadata, true, OperationType.Unknown, false);
        }

        internal void SetEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, EffectiveValueEntry newEntry, EffectiveValueEntry oldEntry)
        {
            if (((metadata != null) && metadata.IsInherited) && ((newEntry.BaseValueSourceInternal != BaseValueSourceInternal.Inherited) && !this.IsSelfInheritanceParent))
            {
                this.SetIsSelfInheritanceParent();
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
            }
            bool flag = false;
            if (oldEntry.HasExpressionMarker && !newEntry.HasExpressionMarker)
            {
                BaseValueSourceInternal baseValueSourceInternal = newEntry.BaseValueSourceInternal;
                flag = ((((baseValueSourceInternal == BaseValueSourceInternal.ThemeStyle) || (baseValueSourceInternal == BaseValueSourceInternal.ThemeStyleTrigger)) || ((baseValueSourceInternal == BaseValueSourceInternal.Style) || (baseValueSourceInternal == BaseValueSourceInternal.TemplateTrigger))) || ((baseValueSourceInternal == BaseValueSourceInternal.StyleTrigger) || (baseValueSourceInternal == BaseValueSourceInternal.ParentTemplate))) || (baseValueSourceInternal == BaseValueSourceInternal.ParentTemplateTrigger);
            }
            if (flag)
            {
                newEntry.RestoreExpressionMarker();
            }
            else if (oldEntry.IsExpression && (oldEntry.ModifiedValue.ExpressionValue == Expression.NoValue))
            {
                newEntry.SetExpressionValue(newEntry.Value, oldEntry.ModifiedValue.BaseValue);
            }
            if (entryIndex.Found)
            {
                this._effectiveValues[entryIndex.Index] = newEntry;
            }
            else
            {
                this.InsertEntry(newEntry, entryIndex.Index);
                if ((metadata != null) && metadata.IsInherited)
                {
                    this.InheritableEffectiveValuesCount++;
                }
            }
        }

        [FriendAccessAllowed]
        internal void SetEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, int targetIndex, PropertyMetadata metadata, object value, BaseValueSourceInternal valueSource)
        {
            EffectiveValueEntry entry;
            if (((metadata != null) && metadata.IsInherited) && ((valueSource != BaseValueSourceInternal.Inherited) && !this.IsSelfInheritanceParent))
            {
                this.SetIsSelfInheritanceParent();
                entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
            }
            if (entryIndex.Found)
            {
                entry = this._effectiveValues[entryIndex.Index];
            }
            else
            {
                entry = new EffectiveValueEntry {
                    PropertyIndex = targetIndex
                };
                this.InsertEntry(entry, entryIndex.Index);
                if ((metadata != null) && metadata.IsInherited)
                {
                    this.InheritableEffectiveValuesCount++;
                }
            }
            bool hasExpressionMarker = value == ExpressionInAlternativeStore;
            if ((!hasExpressionMarker && entry.HasExpressionMarker) && ((((valueSource == BaseValueSourceInternal.ThemeStyle) || (valueSource == BaseValueSourceInternal.ThemeStyleTrigger)) || ((valueSource == BaseValueSourceInternal.Style) || (valueSource == BaseValueSourceInternal.TemplateTrigger))) || (((valueSource == BaseValueSourceInternal.StyleTrigger) || (valueSource == BaseValueSourceInternal.ParentTemplate)) || (valueSource == BaseValueSourceInternal.ParentTemplateTrigger))))
            {
                entry.BaseValueSourceInternal = valueSource;
                entry.SetExpressionValue(value, ExpressionInAlternativeStore);
                entry.ResetAnimatedValue();
                entry.ResetCoercedValue();
            }
            else if (entry.IsExpression && (entry.ModifiedValue.ExpressionValue == Expression.NoValue))
            {
                entry.SetExpressionValue(value, entry.ModifiedValue.BaseValue);
            }
            else
            {
                entry.BaseValueSourceInternal = valueSource;
                entry.ResetValue(value, hasExpressionMarker);
            }
            this._effectiveValues[entryIndex.Index] = entry;
        }

        private void SetExpressionValue(EntryIndex entryIndex, object value, object baseValue)
        {
            EffectiveValueEntry entry = this._effectiveValues[entryIndex.Index];
            entry.SetExpressionValue(value, baseValue);
            entry.ResetAnimatedValue();
            entry.ResetCoercedValue();
            this._effectiveValues[entryIndex.Index] = entry;
        }

        private void SetInheritanceParent(DependencyObject newParent)
        {
            if (this._contextStorage != null)
            {
                this._contextStorage = newParent;
            }
            else if (newParent != null)
            {
                if (this.IsSelfInheritanceParent)
                {
                    this.MergeInheritableProperties(newParent);
                }
                else
                {
                    this._contextStorage = newParent;
                }
            }
        }

        [FriendAccessAllowed]
        internal void SetIsSelfInheritanceParent()
        {
            DependencyObject inheritanceParent = this.InheritanceParent;
            if (inheritanceParent != null)
            {
                this.MergeInheritableProperties(inheritanceParent);
                this.SetInheritanceParent(null);
            }
            this._packedData |= 0x100000;
        }

        [FriendAccessAllowed]
        internal void SetMutableDefaultValue(DependencyProperty dp, object value)
        {
            PropertyMetadata metadata = this.SetupPropertyChange(dp);
            this.SetValueCommon(dp, value, metadata, false, OperationType.ChangeMutableDefaultValue, false);
        }

        private PropertyMetadata SetupPropertyChange(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (dp.ReadOnly)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("ReadOnlyChangeNotAllowed", new object[] { dp.Name }));
            }
            return dp.GetMetadata(this.DependencyObjectType);
        }

        private PropertyMetadata SetupPropertyChange(DependencyPropertyKey key, out DependencyProperty dp)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            dp = key.DependencyProperty;
            if (dp == null)
            {
                throw new ArgumentException(System.Windows.SR.Get("ReadOnlyKeyNotAuthorized", new object[] { dp.Name }));
            }
            dp.VerifyReadOnlyKey(key);
            return dp.GetMetadata(this.DependencyObjectType);
        }

        [FriendAccessAllowed]
        internal void SetValue(DependencyProperty dp, bool value)
        {
            this.SetValue(dp, BooleanBoxes.Box(value));
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            base.VerifyAccess();
            PropertyMetadata metadata = this.SetupPropertyChange(dp);
            this.SetValueCommon(dp, value, metadata, false, OperationType.Unknown, false);
        }

        [FriendAccessAllowed]
        internal void SetValue(DependencyPropertyKey dp, bool value)
        {
            this.SetValue(dp, BooleanBoxes.Box(value));
        }

        public void SetValue(DependencyPropertyKey key, object value)
        {
            DependencyProperty property;
            base.VerifyAccess();
            PropertyMetadata metadata = this.SetupPropertyChange(key, out property);
            this.SetValueCommon(property, value, metadata, false, OperationType.Unknown, false);
        }

        private void SetValueCommon(DependencyProperty dp, object value, PropertyMetadata metadata, bool coerceWithDeferredReference, OperationType operationType, bool isInternal)
        {
            if (this.IsSealed)
            {
                throw new InvalidOperationException(System.Windows.SR.Get("SetOnReadOnlyObjectNotAllowed", new object[] { this }));
            }
            Expression expr = null;
            DependencySource[] newSources = null;
            EntryIndex entryIndex = this.LookupEntry(dp.GlobalIndex);
            if (value == DependencyProperty.UnsetValue)
            {
                this.ClearValueCommon(entryIndex, dp, metadata);
            }
            else
            {
                EffectiveValueEntry entry;
                EffectiveValueEntry entry2;
                bool flag = false;
                bool flag2 = value == ExpressionInAlternativeStore;
                if (!flag2)
                {
                    bool flag3 = isInternal ? dp.IsValidValueInternal(value) : dp.IsValidValue(value);
                    if (!flag3 || dp.IsObjectType)
                    {
                        expr = value as Expression;
                        if (expr != null)
                        {
                            if (!expr.Attachable)
                            {
                                throw new ArgumentException(System.Windows.SR.Get("SharingNonSharableExpression"));
                            }
                            newSources = expr.GetSources();
                            ValidateSources(this, newSources, expr);
                        }
                        else
                        {
                            flag = value is DeferredReference;
                            if (!flag && !flag3)
                            {
                                throw new ArgumentException(System.Windows.SR.Get("InvalidPropertyValue", new object[] { value, dp.Name }));
                            }
                        }
                    }
                }
                if (operationType == OperationType.ChangeMutableDefaultValue)
                {
                    entry = new EffectiveValueEntry(dp, BaseValueSourceInternal.Default) {
                        Value = value
                    };
                }
                else
                {
                    entry = this.GetValueEntry(entryIndex, dp, metadata, RequestFlags.RawEntry);
                }
                object localValue = entry.LocalValue;
                Expression expression2 = null;
                Expression expression3 = null;
                if (entry.HasExpressionMarker)
                {
                    if (expr == null)
                    {
                        expression3 = _getExpressionCore(this, dp, metadata);
                    }
                    if (expression3 != null)
                    {
                        localValue = expression3;
                        expression2 = expression3;
                    }
                    else
                    {
                        localValue = DependencyProperty.UnsetValue;
                    }
                }
                else
                {
                    expression2 = entry.IsExpression ? (localValue as Expression) : null;
                }
                bool flag5 = false;
                if ((expression2 != null) && (expr == null))
                {
                    if (flag)
                    {
                        value = ((DeferredReference) value).GetValue(BaseValueSourceInternal.Local);
                    }
                    flag5 = expression2.SetValue(this, dp, value);
                    entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                }
                if (flag5)
                {
                    if (entryIndex.Found)
                    {
                        entry2 = this._effectiveValues[entryIndex.Index];
                    }
                    else
                    {
                        entry2 = EffectiveValueEntry.CreateDefaultValueEntry(dp, metadata.GetDefaultValue(this, dp));
                    }
                }
                else
                {
                    entry2 = new EffectiveValueEntry(dp, BaseValueSourceInternal.Local);
                    if ((expression2 != null) && (expression2 != expression3))
                    {
                        DependencySource[] sources = expression2.GetSources();
                        UpdateSourceDependentLists(this, dp, sources, expression2, false);
                        expression2.OnDetach(this, dp);
                        entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                    }
                    if (expr == null)
                    {
                        entry2.Value = value;
                        entry2.HasExpressionMarker = flag2;
                    }
                    else
                    {
                        this.SetEffectiveValue(entryIndex, dp, dp.GlobalIndex, metadata, expr, BaseValueSourceInternal.Local);
                        object defaultValue = metadata.GetDefaultValue(this, dp);
                        entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                        this.SetExpressionValue(entryIndex, defaultValue, expr);
                        UpdateSourceDependentLists(this, dp, newSources, expr, true);
                        expr.MarkAttached();
                        expr.OnAttach(this, dp);
                        entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                        entry2 = this.EvaluateExpression(entryIndex, dp, expr, metadata, entry, this._effectiveValues[entryIndex.Index]);
                        entryIndex = this.CheckEntryIndex(entryIndex, dp.GlobalIndex);
                    }
                }
                this.UpdateEffectiveValue(entryIndex, dp, metadata, entry, ref entry2, coerceWithDeferredReference, operationType);
            }
        }

        [FriendAccessAllowed]
        internal void SetValueInternal(DependencyProperty dp, object value)
        {
            base.VerifyAccess();
            PropertyMetadata metadata = this.SetupPropertyChange(dp);
            this.SetValueCommon(dp, value, metadata, false, OperationType.Unknown, true);
        }

        internal virtual bool ShouldProvideInheritanceContext(DependencyObject target, DependencyProperty property) => 
            true;

        protected internal virtual bool ShouldSerializeProperty(DependencyProperty dp) => 
            this.ContainsValue(dp);

        [FriendAccessAllowed]
        internal void SynchronizeInheritanceParent(DependencyObject parent)
        {
            if (!this.IsSelfInheritanceParent)
            {
                if (parent != null)
                {
                    if (!parent.IsSelfInheritanceParent)
                    {
                        this.SetInheritanceParent(parent.InheritanceParent);
                    }
                    else
                    {
                        this.SetInheritanceParent(parent);
                    }
                }
                else
                {
                    this.SetInheritanceParent(null);
                }
            }
        }

        internal void UnsetEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata)
        {
            if (entryIndex.Found)
            {
                this.RemoveEntry(entryIndex.Index, dp);
                if ((metadata != null) && metadata.IsInherited)
                {
                    this.InheritableEffectiveValuesCount--;
                }
            }
        }

        [FriendAccessAllowed]
        internal UpdateResult UpdateEffectiveValue(EntryIndex entryIndex, DependencyProperty dp, PropertyMetadata metadata, EffectiveValueEntry oldEntry, ref EffectiveValueEntry newEntry, bool coerceWithDeferredReference, OperationType operationType)
        {
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            int globalIndex = dp.GlobalIndex;
            if (oldEntry.BaseValueSourceInternal == BaseValueSourceInternal.Unknown)
            {
                oldEntry = this.GetValueEntry(entryIndex, dp, metadata, RequestFlags.RawEntry);
            }
            object oldValue = oldEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value;
            if ((newEntry.BaseValueSourceInternal != BaseValueSourceInternal.Unknown) && (newEntry.BaseValueSourceInternal < oldEntry.BaseValueSourceInternal))
            {
                return (UpdateResult) 0;
            }
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            if (newEntry.Value == DependencyProperty.UnsetValue)
            {
                flag2 = newEntry.FullValueSource == FullValueSource.IsCoerced;
                flag = true;
                if (newEntry.BaseValueSourceInternal == BaseValueSourceInternal.Local)
                {
                    flag3 = true;
                }
            }
            if (flag || (!newEntry.IsAnimated && (oldEntry.IsAnimated || ((oldEntry.IsExpression && newEntry.IsExpression) && (newEntry.ModifiedValue.BaseValue == oldEntry.ModifiedValue.BaseValue)))))
            {
                if (!flag2)
                {
                    newEntry = this.EvaluateEffectiveValue(entryIndex, dp, metadata, oldEntry, newEntry, operationType);
                    entryIndex = this.CheckEntryIndex(entryIndex, globalIndex);
                    bool flag4 = newEntry.Value != DependencyProperty.UnsetValue;
                    if (!flag4 && metadata.IsInherited)
                    {
                        DependencyObject inheritanceParent = this.InheritanceParent;
                        if (inheritanceParent != null)
                        {
                            EntryIndex entry = inheritanceParent.LookupEntry(dp.GlobalIndex);
                            if (entry.Found)
                            {
                                flag4 = true;
                                newEntry = inheritanceParent._effectiveValues[entry.Index].GetFlattenedEntry(RequestFlags.FullyResolved);
                                newEntry.BaseValueSourceInternal = BaseValueSourceInternal.Inherited;
                            }
                        }
                    }
                    if (!flag4)
                    {
                        newEntry = EffectiveValueEntry.CreateDefaultValueEntry(dp, metadata.GetDefaultValue(this, dp));
                    }
                }
                else if (!oldEntry.HasModifiers)
                {
                    newEntry = oldEntry;
                }
                else
                {
                    newEntry = new EffectiveValueEntry(dp, oldEntry.BaseValueSourceInternal);
                    ModifiedValue modifiedValue = oldEntry.ModifiedValue;
                    object baseValue = modifiedValue.BaseValue;
                    newEntry.Value = baseValue;
                    newEntry.HasExpressionMarker = oldEntry.HasExpressionMarker;
                    if (oldEntry.IsExpression)
                    {
                        newEntry.SetExpressionValue(modifiedValue.ExpressionValue, baseValue);
                    }
                    if (oldEntry.IsAnimated)
                    {
                        newEntry.SetAnimatedValue(modifiedValue.AnimatedValue, baseValue);
                    }
                }
            }
            if ((metadata.CoerceValueCallback != null) && (!flag3 || (newEntry.FullValueSource != ((FullValueSource) 1))))
            {
                object obj5 = newEntry.GetFlattenedEntry(RequestFlags.CoercionBaseValue).Value;
                this.ProcessCoerceValue(dp, metadata, ref entryIndex, ref globalIndex, ref newEntry, ref oldEntry, ref oldValue, obj5, metadata.CoerceValueCallback, coerceWithDeferredReference, false);
                entryIndex = this.CheckEntryIndex(entryIndex, globalIndex);
            }
            if (dp.DesignerCoerceValueCallback != null)
            {
                this.ProcessCoerceValue(dp, metadata, ref entryIndex, ref globalIndex, ref newEntry, ref oldEntry, ref oldValue, newEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value, dp.DesignerCoerceValueCallback, false, true);
                entryIndex = this.CheckEntryIndex(entryIndex, globalIndex);
            }
            if (newEntry.FullValueSource != ((FullValueSource) 1))
            {
                if ((newEntry.BaseValueSourceInternal == BaseValueSourceInternal.Inherited) && !this.IsSelfInheritanceParent)
                {
                    this.UnsetEffectiveValue(entryIndex, dp, metadata);
                }
                else
                {
                    this.SetEffectiveValue(entryIndex, dp, metadata, newEntry, oldEntry);
                }
            }
            else
            {
                this.UnsetEffectiveValue(entryIndex, dp, metadata);
            }
            bool isAValueChange = !this.Equals(dp, oldValue, newEntry.GetFlattenedEntry(RequestFlags.FullyResolved).Value);
            UpdateResult result = isAValueChange ? UpdateResult.ValueChanged : ((UpdateResult) 0);
            if ((isAValueChange || ((operationType == OperationType.ChangeMutableDefaultValue) && (oldEntry.BaseValueSourceInternal != newEntry.BaseValueSourceInternal))) || (((metadata.IsInherited && (oldEntry.BaseValueSourceInternal != newEntry.BaseValueSourceInternal)) && ((operationType != OperationType.AddChild) && (operationType != OperationType.RemoveChild))) && (operationType != OperationType.Inherit)))
            {
                result |= UpdateResult.NotificationSent;
                this.NotifyPropertyChange(new DependencyPropertyChangedEventArgs(dp, metadata, isAValueChange, oldEntry, newEntry, operationType));
            }
            bool flag6 = oldEntry.FullValueSource == ((FullValueSource) 11);
            bool flag7 = newEntry.FullValueSource == ((FullValueSource) 11);
            if ((result != ((UpdateResult) 0)) || (flag6 != flag7))
            {
                if (flag6)
                {
                    this.RemoveSelfAsInheritanceContext(oldEntry.LocalValue, dp);
                }
                if (flag7)
                {
                    this.ProvideSelfAsInheritanceContext(newEntry.LocalValue, dp);
                }
            }
            return result;
        }

        internal static void UpdateSourceDependentLists(DependencyObject d, DependencyProperty dp, DependencySource[] sources, Expression expr, bool add)
        {
            if (sources != null)
            {
                if (expr.ForwardsInvalidations)
                {
                    d = null;
                    dp = null;
                }
                for (int i = 0; i < sources.Length; i++)
                {
                    DependencySource source = sources[i];
                    if (!source.DependencyObject.IsSealed)
                    {
                        FrugalMap map;
                        object obj2 = DependentListMapField.GetValue(source.DependencyObject);
                        if (obj2 != null)
                        {
                            map = (FrugalMap) obj2;
                        }
                        else
                        {
                            map = new FrugalMap();
                        }
                        object obj3 = map[source.DependencyProperty.GlobalIndex];
                        if (add)
                        {
                            DependentList list;
                            if (obj3 == DependencyProperty.UnsetValue)
                            {
                                map[source.DependencyProperty.GlobalIndex] = list = new DependentList();
                            }
                            else
                            {
                                list = (DependentList) obj3;
                            }
                            list.Add(d, dp, expr);
                        }
                        else if (obj3 != DependencyProperty.UnsetValue)
                        {
                            DependentList list2 = (DependentList) obj3;
                            list2.Remove(d, dp, expr);
                            if (list2.IsEmpty)
                            {
                                map[source.DependencyProperty.GlobalIndex] = DependencyProperty.UnsetValue;
                            }
                        }
                        DependentListMapField.SetValue(source.DependencyObject, map);
                    }
                }
            }
        }

        internal static void ValidateSources(DependencyObject d, DependencySource[] newSources, Expression expr)
        {
            if (newSources != null)
            {
                Dispatcher dispatcher = d.Dispatcher;
                for (int i = 0; i < newSources.Length; i++)
                {
                    Dispatcher dispatcher2 = newSources[i].DependencyObject.Dispatcher;
                    if ((dispatcher2 != dispatcher) && (!expr.SupportsUnboundSources || (dispatcher2 != null)))
                    {
                        throw new ArgumentException(System.Windows.SR.Get("SourcesMustBeInSameThread"));
                    }
                }
            }
        }

        internal bool Animatable_IsResourceInvalidationNecessary
        {
            [FriendAccessAllowed]
            get => 
                ((this._packedData & 0x40000000) != 0);
            [FriendAccessAllowed]
            set
            {
                if (value)
                {
                    this._packedData |= 0x40000000;
                }
                else
                {
                    this._packedData &= 0xbfffffff;
                }
            }
        }

        internal bool CanBeInheritanceContext
        {
            [FriendAccessAllowed]
            get => 
                ((this._packedData & 0x200000) != 0);
            [FriendAccessAllowed]
            set
            {
                if (value)
                {
                    this._packedData |= 0x200000;
                }
                else
                {
                    this._packedData &= 0xffdfffff;
                }
            }
        }

        private bool CanModifyEffectiveValues
        {
            get => 
                ((this._packedData & 0x80000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x80000;
                }
                else
                {
                    this._packedData &= 0xfff7ffff;
                }
            }
        }

        public System.Windows.DependencyObjectType DependencyObjectType
        {
            get
            {
                if (this._dType == null)
                {
                    this._dType = System.Windows.DependencyObjectType.FromSystemTypeInternal(base.GetType());
                }
                return this._dType;
            }
        }

        private bool DO_Sealed
        {
            get => 
                ((this._packedData & 0x400000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x400000;
                }
                else
                {
                    this._packedData &= 0xffbfffff;
                }
            }
        }

        internal EffectiveValueEntry[] EffectiveValues =>
            this._effectiveValues;

        internal uint EffectiveValuesCount
        {
            [FriendAccessAllowed]
            get => 
                (this._packedData & 0x3ff);
            private set
            {
                this._packedData = (this._packedData & 0xfffffc00) | (value & 0x3ff);
            }
        }

        [FriendAccessAllowed]
        internal virtual int EffectiveValuesInitialSize =>
            2;

        internal bool Freezable_Frozen
        {
            get => 
                this.DO_Sealed;
            set
            {
                this.DO_Sealed = value;
            }
        }

        internal bool Freezable_HasMultipleInheritanceContexts
        {
            get => 
                ((this._packedData & 0x2000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x2000000;
                }
                else
                {
                    this._packedData &= 0xfdffffff;
                }
            }
        }

        internal bool Freezable_UsingContextList
        {
            get => 
                ((this._packedData & 0x8000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x8000000;
                }
                else
                {
                    this._packedData &= 0xf7ffffff;
                }
            }
        }

        internal bool Freezable_UsingHandlerList
        {
            get => 
                ((this._packedData & 0x4000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x4000000;
                }
                else
                {
                    this._packedData &= 0xfbffffff;
                }
            }
        }

        internal bool Freezable_UsingSingletonContext
        {
            get => 
                ((this._packedData & 0x20000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x20000000;
                }
                else
                {
                    this._packedData &= 0xdfffffff;
                }
            }
        }

        internal bool Freezable_UsingSingletonHandler
        {
            get => 
                ((this._packedData & 0x10000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x10000000;
                }
                else
                {
                    this._packedData &= 0xefffffff;
                }
            }
        }

        internal virtual bool HasMultipleInheritanceContexts =>
            false;

        internal bool IAnimatable_HasAnimatedProperties
        {
            [FriendAccessAllowed]
            get => 
                ((this._packedData & 0x80000000) != 0);
            [FriendAccessAllowed]
            set
            {
                if (value)
                {
                    this._packedData |= 0x80000000;
                }
                else
                {
                    this._packedData &= 0x7fffffff;
                }
            }
        }

        internal uint InheritableEffectiveValuesCount
        {
            [FriendAccessAllowed]
            get => 
                ((this._packedData >> 10) & 0x1ff);
            set
            {
                this._packedData = ((uint) ((value & 0x1ff) << 10)) | (this._packedData & 0xfff803ff);
            }
        }

        internal virtual DependencyObject InheritanceContext =>
            null;

        internal DependencyObject InheritanceParent
        {
            [FriendAccessAllowed]
            get
            {
                if ((this._packedData & 0x3e100000) == 0)
                {
                    return (DependencyObject) this._contextStorage;
                }
                return null;
            }
        }

        [FriendAccessAllowed]
        internal bool IsInheritanceContextSealed
        {
            get => 
                ((this._packedData & 0x1000000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x1000000;
                }
                else
                {
                    this._packedData &= 0xfeffffff;
                }
            }
        }

        private bool IsInPropertyInitialization
        {
            get => 
                ((this._packedData & 0x800000) != 0);
            set
            {
                if (value)
                {
                    this._packedData |= 0x800000;
                }
                else
                {
                    this._packedData &= 0xff7fffff;
                }
            }
        }

        public bool IsSealed =>
            this.DO_Sealed;

        internal bool IsSelfInheritanceParent =>
            ((this._packedData & 0x100000) != 0);
    }
}

