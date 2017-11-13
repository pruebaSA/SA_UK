namespace MS.Internal.ComponentModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PropertyKey : IEquatable<PropertyKey>
    {
        internal System.Windows.DependencyProperty DependencyProperty;
        internal Type AttachedType;
        private int _hashCode;
        internal PropertyKey(Type attachedType, System.Windows.DependencyProperty prop)
        {
            this.DependencyProperty = prop;
            this.AttachedType = attachedType;
            this._hashCode = this.AttachedType.GetHashCode() ^ this.DependencyProperty.GetHashCode();
        }

        public override int GetHashCode() => 
            this._hashCode;

        public override bool Equals(object obj) => 
            this.Equals((PropertyKey) obj);

        public bool Equals(PropertyKey key) => 
            ((key.AttachedType == this.AttachedType) && (key.DependencyProperty == this.DependencyProperty));

        public static bool operator ==(PropertyKey key1, PropertyKey key2) => 
            ((key1.AttachedType == key2.AttachedType) && (key1.DependencyProperty == key2.DependencyProperty));

        public static bool operator !=(PropertyKey key1, PropertyKey key2)
        {
            if (key1.AttachedType == key2.AttachedType)
            {
                return (key1.DependencyProperty != key2.DependencyProperty);
            }
            return true;
        }
    }
}

