namespace MS.Internal.ComponentModel
{
    using System;
    using System.Windows;

    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class DependencyPropertyAttribute : Attribute
    {
        private System.Windows.DependencyProperty _dp;
        private bool _isAttached;

        internal DependencyPropertyAttribute(System.Windows.DependencyProperty dependencyProperty, bool isAttached)
        {
            if (dependencyProperty == null)
            {
                throw new ArgumentNullException("dependencyProperty");
            }
            this._dp = dependencyProperty;
            this._isAttached = isAttached;
        }

        public override bool Equals(object value)
        {
            DependencyPropertyAttribute attribute = value as DependencyPropertyAttribute;
            return (((attribute != null) && object.ReferenceEquals(attribute._dp, this._dp)) && (attribute._isAttached == this._isAttached));
        }

        public override int GetHashCode() => 
            this._dp.GetHashCode();

        internal System.Windows.DependencyProperty DependencyProperty =>
            this._dp;

        internal bool IsAttached =>
            this._isAttached;

        public override object TypeId =>
            typeof(DependencyPropertyAttribute);
    }
}

