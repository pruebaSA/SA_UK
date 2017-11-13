namespace System.Windows
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=true)]
    public sealed class AttachedPropertyBrowsableForTypeAttribute : AttachedPropertyBrowsableAttribute
    {
        private DependencyObjectType _dTargetType;
        private bool _dTargetTypeChecked;
        private Type _targetType;

        public AttachedPropertyBrowsableForTypeAttribute(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            this._targetType = targetType;
        }

        public override bool Equals(object obj)
        {
            AttachedPropertyBrowsableForTypeAttribute attribute = obj as AttachedPropertyBrowsableForTypeAttribute;
            if (attribute == null)
            {
                return false;
            }
            return (this._targetType == attribute._targetType);
        }

        public override int GetHashCode() => 
            this._targetType.GetHashCode();

        internal override bool IsBrowsable(DependencyObject d, DependencyProperty dp)
        {
            if (d == null)
            {
                throw new ArgumentNullException("d");
            }
            if (dp == null)
            {
                throw new ArgumentNullException("dp");
            }
            if (!this._dTargetTypeChecked)
            {
                try
                {
                    this._dTargetType = DependencyObjectType.FromSystemType(this._targetType);
                }
                catch (ArgumentException)
                {
                }
                this._dTargetTypeChecked = true;
            }
            return ((this._dTargetType != null) && this._dTargetType.IsInstanceOfType(d));
        }

        public Type TargetType =>
            this._targetType;

        public override object TypeId =>
            this;

        internal override bool UnionResults =>
            true;
    }
}

