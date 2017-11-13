namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NInt : INullableValue
    {
        public static readonly NInt NullValue;
        private int val;
        public NInt(int val)
        {
            this.val = val;
        }

        public int Value
        {
            get
            {
                if (this.val == -2147483648)
                {
                    return 0;
                }
                return this.val;
            }
            set
            {
                this.val = value;
            }
        }
        object INullableValue.GetValue() => 
            this.Value;

        void INullableValue.SetValue(object value)
        {
            this.val = (int) value;
        }

        public void SetNull()
        {
            this.val = -2147483648;
        }

        public bool IsNull =>
            (this.val == -2147483648);
        public static implicit operator NInt(int val) => 
            new NInt(val);

        public static implicit operator int(NInt val) => 
            val.Value;

        public override bool Equals(object value) => 
            ((value is NInt) && (this == ((NInt) value)));

        public override int GetHashCode() => 
            this.val.GetHashCode();

        public static bool operator ==(NInt l, NInt r)
        {
            if (l.IsNull)
            {
                return r.IsNull;
            }
            if (r.IsNull)
            {
                return false;
            }
            return (l.Value == r.Value);
        }

        public static bool operator !=(NInt l, NInt r) => 
            !(l == r);

        static NInt()
        {
            NullValue = new NInt(-2147483648);
        }
    }
}

