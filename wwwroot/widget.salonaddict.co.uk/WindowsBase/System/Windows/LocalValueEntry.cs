namespace System.Windows
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LocalValueEntry
    {
        internal DependencyProperty _dp;
        internal object _value;
        public override int GetHashCode() => 
            base.GetHashCode();

        public override bool Equals(object obj)
        {
            LocalValueEntry entry = (LocalValueEntry) obj;
            return ((this._dp == entry._dp) && (this._value == entry._value));
        }

        public static bool operator ==(LocalValueEntry obj1, LocalValueEntry obj2) => 
            obj1.Equals(obj2);

        public static bool operator !=(LocalValueEntry obj1, LocalValueEntry obj2) => 
            !(obj1 == obj2);

        public DependencyProperty Property =>
            this._dp;
        public object Value =>
            this._value;
        internal LocalValueEntry(DependencyProperty dp, object value)
        {
            this._dp = dp;
            this._value = value;
        }
    }
}

