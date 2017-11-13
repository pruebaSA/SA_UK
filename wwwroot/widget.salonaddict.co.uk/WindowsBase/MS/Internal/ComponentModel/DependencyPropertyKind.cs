namespace MS.Internal.ComponentModel
{
    using System;
    using System.Windows;

    internal class DependencyPropertyKind
    {
        private readonly DependencyProperty _dp;
        private bool _isAttached;
        private bool _isAttachedChecked;
        private bool _isDirect;
        private bool _isDirectChecked;
        private bool _isInternal;
        private bool _isInternalChecked;
        private readonly Type _targetType;

        internal DependencyPropertyKind(DependencyProperty dp, Type targetType)
        {
            this._dp = dp;
            this._targetType = targetType;
        }

        internal bool IsAttached
        {
            get
            {
                if (!this._isAttachedChecked)
                {
                    if ((!this.IsDirect && (((this._dp.OwnerType == this._targetType) || this._dp.OwnerType.IsAssignableFrom(this._targetType)) || (DependencyProperty.FromName(this._dp.Name, this._targetType) != this._dp))) && (DependencyObjectPropertyDescriptor.GetAttachedPropertyMethod(this._dp) != null))
                    {
                        this._isAttached = true;
                    }
                    this._isAttachedChecked = true;
                }
                return this._isAttached;
            }
        }

        internal bool IsDirect
        {
            get
            {
                if (!this._isDirectChecked)
                {
                    if ((!this._isAttached && (DependencyProperty.FromName(this._dp.Name, this._targetType) == this._dp)) && (this._targetType.GetProperty(this._dp.Name) != null))
                    {
                        this._isDirect = true;
                        this._isAttachedChecked = true;
                    }
                    this._isDirectChecked = true;
                }
                return this._isDirect;
            }
        }

        internal bool IsInternal
        {
            get
            {
                if (!this._isInternalChecked)
                {
                    if ((!this._isAttached && !this._isDirect) && ((DependencyObjectPropertyDescriptor.GetAttachedPropertyMethod(this._dp) == null) && (this._dp.OwnerType.GetProperty(this._dp.Name) == null)))
                    {
                        this._isInternal = true;
                    }
                    this._isInternalChecked = true;
                }
                return this._isInternal;
            }
        }
    }
}

