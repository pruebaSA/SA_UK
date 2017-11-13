namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), FriendAccessAllowed]
    internal struct EffectiveValueEntry
    {
        private object _value;
        private short _propertyIndex;
        private System.Windows.FullValueSource _source;
        internal static EffectiveValueEntry CreateDefaultValueEntry(DependencyProperty dp, object value) => 
            new EffectiveValueEntry(dp, System.Windows.BaseValueSourceInternal.Default) { Value = value };

        internal EffectiveValueEntry(DependencyProperty dp)
        {
            this._propertyIndex = (short) dp.GlobalIndex;
            this._value = null;
            this._source = (System.Windows.FullValueSource) 0;
        }

        internal EffectiveValueEntry(DependencyProperty dp, System.Windows.BaseValueSourceInternal valueSource)
        {
            this._propertyIndex = (short) dp.GlobalIndex;
            this._value = DependencyProperty.UnsetValue;
            this._source = (System.Windows.FullValueSource) valueSource;
        }

        internal EffectiveValueEntry(DependencyProperty dp, System.Windows.FullValueSource fullValueSource)
        {
            this._propertyIndex = (short) dp.GlobalIndex;
            this._value = DependencyProperty.UnsetValue;
            this._source = fullValueSource;
        }

        internal void SetExpressionValue(object value, object baseValue)
        {
            this.EnsureModifiedValue().ExpressionValue = value;
            this.IsExpression = true;
            this.IsDeferredReference = value is DeferredReference;
        }

        internal void SetAnimatedValue(object value, object baseValue)
        {
            this.EnsureModifiedValue().AnimatedValue = value;
            this.IsAnimated = true;
            this.IsDeferredReference = false;
        }

        internal void SetCoercedValue(object value, object baseValue, bool skipBaseValueChecks)
        {
            this.EnsureModifiedValue().CoercedValue = value;
            this.IsCoerced = true;
            this.IsDeferredReference = false;
        }

        internal void ResetExpressionValue()
        {
            if (this.IsExpression)
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.ExpressionValue = null;
                this.IsExpression = false;
                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
            }
        }

        internal void ResetAnimatedValue()
        {
            if (this.IsAnimated)
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.AnimatedValue = null;
                this.IsAnimated = false;
                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
                else
                {
                    this.ComputeIsDeferred();
                }
            }
        }

        internal void ResetCoercedValue()
        {
            if (this.IsCoerced)
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.CoercedValue = null;
                this.IsCoerced = false;
                if (!this.HasModifiers)
                {
                    this.Value = modifiedValue.BaseValue;
                }
                else
                {
                    this.ComputeIsDeferred();
                }
            }
        }

        internal void ResetValue(object value, bool hasExpressionMarker)
        {
            this._source = (System.Windows.FullValueSource) ((short) (this._source & System.Windows.FullValueSource.ValueSourceMask));
            this._value = value;
            if (hasExpressionMarker)
            {
                this.HasExpressionMarker = true;
            }
            else
            {
                this.ComputeIsDeferred();
            }
        }

        internal void RestoreExpressionMarker()
        {
            if (this.HasModifiers)
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                modifiedValue.ExpressionValue = modifiedValue.BaseValue;
                modifiedValue.BaseValue = DependencyObject.ExpressionInAlternativeStore;
                this._source = (System.Windows.FullValueSource) ((short) (this._source | System.Windows.FullValueSource.HasExpressionMarker | System.Windows.FullValueSource.IsExpression));
                this.ComputeIsDeferred();
            }
            else
            {
                object obj2 = this.Value;
                this.Value = DependencyObject.ExpressionInAlternativeStore;
                this.SetExpressionValue(obj2, DependencyObject.ExpressionInAlternativeStore);
                this._source = (System.Windows.FullValueSource) ((short) (this._source | System.Windows.FullValueSource.HasExpressionMarker));
            }
        }

        public int PropertyIndex
        {
            get => 
                this._propertyIndex;
            set
            {
                this._propertyIndex = (short) value;
            }
        }
        internal object Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
                this.IsDeferredReference = value is DeferredReference;
            }
        }
        internal System.Windows.BaseValueSourceInternal BaseValueSourceInternal
        {
            get => 
                ((System.Windows.BaseValueSourceInternal) ((short) (this._source & System.Windows.FullValueSource.ValueSourceMask)));
            set
            {
                this._source = ((System.Windows.FullValueSource) ((short) (this._source & ~System.Windows.FullValueSource.ValueSourceMask))) | ((System.Windows.FullValueSource) value);
            }
        }
        internal bool IsDeferredReference
        {
            get
            {
                bool flag = this.ReadPrivateFlag(System.Windows.FullValueSource.IsPotentiallyADeferredReference);
                if (flag)
                {
                    this.ComputeIsDeferred();
                    flag = this.ReadPrivateFlag(System.Windows.FullValueSource.IsPotentiallyADeferredReference);
                }
                return flag;
            }
            private set
            {
                this.WritePrivateFlag(System.Windows.FullValueSource.IsPotentiallyADeferredReference, value);
            }
        }
        internal bool IsExpression
        {
            get => 
                this.ReadPrivateFlag(System.Windows.FullValueSource.IsExpression);
            private set
            {
                this.WritePrivateFlag(System.Windows.FullValueSource.IsExpression, value);
            }
        }
        internal bool IsAnimated
        {
            get => 
                this.ReadPrivateFlag(System.Windows.FullValueSource.IsAnimated);
            private set
            {
                this.WritePrivateFlag(System.Windows.FullValueSource.IsAnimated, value);
            }
        }
        internal bool IsCoerced
        {
            get => 
                this.ReadPrivateFlag(System.Windows.FullValueSource.IsCoerced);
            private set
            {
                this.WritePrivateFlag(System.Windows.FullValueSource.IsCoerced, value);
            }
        }
        internal bool HasModifiers =>
            (((short) (this._source & System.Windows.FullValueSource.ModifiersMask)) != 0);
        internal System.Windows.FullValueSource FullValueSource =>
            this._source;
        internal bool HasExpressionMarker
        {
            get => 
                this.ReadPrivateFlag(System.Windows.FullValueSource.HasExpressionMarker);
            set
            {
                this.WritePrivateFlag(System.Windows.FullValueSource.HasExpressionMarker, value);
            }
        }
        internal EffectiveValueEntry GetFlattenedEntry(RequestFlags requests)
        {
            if (((short) (this._source & (System.Windows.FullValueSource.HasExpressionMarker | System.Windows.FullValueSource.IsAnimated | System.Windows.FullValueSource.IsCoerced | System.Windows.FullValueSource.IsExpression))) == 0)
            {
                return this;
            }
            if (!this.HasModifiers)
            {
                return new EffectiveValueEntry { 
                    BaseValueSourceInternal = this.BaseValueSourceInternal,
                    PropertyIndex = this.PropertyIndex
                };
            }
            EffectiveValueEntry entry2 = new EffectiveValueEntry {
                BaseValueSourceInternal = this.BaseValueSourceInternal,
                PropertyIndex = this.PropertyIndex,
                IsDeferredReference = this.IsDeferredReference
            };
            System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
            if (this.IsCoerced)
            {
                if ((requests & RequestFlags.CoercionBaseValue) == RequestFlags.FullyResolved)
                {
                    entry2.Value = modifiedValue.CoercedValue;
                    return entry2;
                }
                if (this.IsAnimated && ((requests & RequestFlags.AnimationBaseValue) == RequestFlags.FullyResolved))
                {
                    entry2.Value = modifiedValue.AnimatedValue;
                    return entry2;
                }
                if (this.IsExpression)
                {
                    entry2.Value = modifiedValue.ExpressionValue;
                    return entry2;
                }
                entry2.Value = modifiedValue.BaseValue;
                return entry2;
            }
            if (this.IsAnimated)
            {
                if ((requests & RequestFlags.AnimationBaseValue) == RequestFlags.FullyResolved)
                {
                    entry2.Value = modifiedValue.AnimatedValue;
                    return entry2;
                }
                if (this.IsExpression)
                {
                    entry2.Value = modifiedValue.ExpressionValue;
                    return entry2;
                }
                entry2.Value = modifiedValue.BaseValue;
                return entry2;
            }
            object expressionValue = modifiedValue.ExpressionValue;
            entry2.Value = expressionValue;
            return entry2;
        }

        internal void SetAnimationBaseValue(object animationBaseValue)
        {
            if (!this.HasModifiers)
            {
                this.Value = animationBaseValue;
            }
            else
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                if (this.IsExpression)
                {
                    modifiedValue.ExpressionValue = animationBaseValue;
                }
                else
                {
                    modifiedValue.BaseValue = animationBaseValue;
                }
                this.ComputeIsDeferred();
            }
        }

        internal void SetCoersionBaseValue(object coersionBaseValue)
        {
            if (!this.HasModifiers)
            {
                this.Value = coersionBaseValue;
            }
            else
            {
                System.Windows.ModifiedValue modifiedValue = this.ModifiedValue;
                if (this.IsAnimated)
                {
                    modifiedValue.AnimatedValue = coersionBaseValue;
                }
                else if (this.IsExpression)
                {
                    modifiedValue.ExpressionValue = coersionBaseValue;
                }
                else
                {
                    modifiedValue.BaseValue = coersionBaseValue;
                }
                this.ComputeIsDeferred();
            }
        }

        internal object LocalValue
        {
            get
            {
                if (this.BaseValueSourceInternal != System.Windows.BaseValueSourceInternal.Local)
                {
                    return DependencyProperty.UnsetValue;
                }
                if (!this.HasModifiers)
                {
                    return this.Value;
                }
                return this.ModifiedValue.BaseValue;
            }
            set
            {
                if (!this.HasModifiers)
                {
                    this.Value = value;
                }
                else
                {
                    this.ModifiedValue.BaseValue = value;
                }
            }
        }
        internal System.Windows.ModifiedValue ModifiedValue
        {
            get
            {
                if (this._value != null)
                {
                    return (this._value as System.Windows.ModifiedValue);
                }
                return null;
            }
        }
        private System.Windows.ModifiedValue EnsureModifiedValue()
        {
            System.Windows.ModifiedValue value2 = null;
            if (this._value == null)
            {
                this._value = value2 = new System.Windows.ModifiedValue();
                return value2;
            }
            value2 = this._value as System.Windows.ModifiedValue;
            if (value2 == null)
            {
                value2 = new System.Windows.ModifiedValue {
                    BaseValue = this._value
                };
                this._value = value2;
            }
            return value2;
        }

        internal void Clear()
        {
            this._propertyIndex = -1;
            this._value = null;
            this._source = (System.Windows.FullValueSource) 0;
        }

        private void ComputeIsDeferred()
        {
            bool flag = false;
            if (!this.HasModifiers)
            {
                flag = this.Value is DeferredReference;
            }
            else if ((this.ModifiedValue != null) && this.IsExpression)
            {
                flag = this.ModifiedValue.ExpressionValue is DeferredReference;
            }
            this.IsDeferredReference = flag;
        }

        private void WritePrivateFlag(System.Windows.FullValueSource bit, bool value)
        {
            if (value)
            {
                this._source = (System.Windows.FullValueSource) ((short) (this._source | bit));
            }
            else
            {
                this._source = (System.Windows.FullValueSource) ((short) (this._source & ((short) ~bit)));
            }
        }

        private bool ReadPrivateFlag(System.Windows.FullValueSource bit) => 
            (((short) (this._source & bit)) != 0);
    }
}

