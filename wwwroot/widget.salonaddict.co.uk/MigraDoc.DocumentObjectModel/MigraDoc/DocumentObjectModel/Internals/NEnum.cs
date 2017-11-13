namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NEnum : INullableValue
    {
        private System.Type type;
        private int val;
        public NEnum(int val, System.Type type)
        {
            this.type = type;
            this.val = val;
        }

        private NEnum(int value)
        {
            this.type = null;
            this.val = value;
        }

        internal System.Type Type
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
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
                if (this.type == typeof(SymbolName))
                {
                    this.val = value;
                }
                else
                {
                    if (!Enum.IsDefined(this.type, value))
                    {
                        throw new InvalidEnumArgumentException("value");
                    }
                    this.val = value;
                }
            }
        }
        object INullableValue.GetValue() => 
            this.ToObject();

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
        public object ToObject()
        {
            if (this.val != -2147483648)
            {
                return Enum.ToObject(this.type, this.val);
            }
            return Enum.ToObject(this.type, 0);
        }

        public override bool Equals(object value) => 
            ((value is NEnum) && (this == ((NEnum) value)));

        public override int GetHashCode() => 
            this.val.GetHashCode();

        public static bool operator ==(NEnum l, NEnum r)
        {
            if (l.IsNull)
            {
                return r.IsNull;
            }
            if (r.IsNull)
            {
                return false;
            }
            return ((l.type == r.type) && (l.Value == r.Value));
        }

        public static bool operator !=(NEnum l, NEnum r) => 
            !(l == r);

        public static NEnum NullValue(System.Type fieldType) => 
            new NEnum(-2147483648, fieldType);
    }
}

