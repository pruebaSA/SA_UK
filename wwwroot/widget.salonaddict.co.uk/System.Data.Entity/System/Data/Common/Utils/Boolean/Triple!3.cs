namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct Triple<T1, T2, T3> : IEquatable<Triple<T1, T2, T3>> where T1: IEquatable<T1> where T2: IEquatable<T2> where T3: IEquatable<T3>
    {
        private readonly T1 _value1;
        private readonly T2 _value2;
        private readonly T3 _value3;
        internal Triple(T1 value1, T2 value2, T3 value3)
        {
            this._value1 = value1;
            this._value2 = value2;
            this._value3 = value3;
        }

        public bool Equals(Triple<T1, T2, T3> other) => 
            ((this._value1.Equals(other._value1) && this._value2.Equals(other._value2)) && this._value3.Equals(other._value3));

        public override bool Equals(object obj) => 
            base.Equals(obj);

        public override int GetHashCode() => 
            ((this._value1.GetHashCode() ^ this._value2.GetHashCode()) ^ this._value3.GetHashCode());
    }
}

