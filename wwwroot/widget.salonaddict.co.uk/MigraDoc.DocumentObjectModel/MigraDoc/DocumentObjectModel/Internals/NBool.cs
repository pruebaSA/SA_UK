namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NBool : INullableValue
    {
        public static readonly NBool NullValue;
        private sbyte val;
        public NBool(bool value)
        {
            this.val = value ? ((sbyte) 1) : ((sbyte) 0);
        }

        private NBool(sbyte value)
        {
            this.val = value;
        }

        public bool Value
        {
            get => 
                (this.val == 1);
            set
            {
                this.val = value ? ((sbyte) 1) : ((sbyte) 0);
            }
        }
        object INullableValue.GetValue() => 
            this.Value;

        void INullableValue.SetValue(object value)
        {
            this.val = ((bool) value) ? ((sbyte) 1) : ((sbyte) 0);
        }

        public void SetNull()
        {
            this.val = -1;
        }

        public bool IsNull =>
            (this.val == -1);
        public override bool Equals(object value) => 
            ((value is NBool) && (this == ((NBool) value)));

        public override int GetHashCode() => 
            this.val.GetHashCode();

        public static bool operator ==(NBool l, NBool r)
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

        public static bool operator !=(NBool l, NBool r) => 
            !(l == r);

        static NBool()
        {
            NullValue = new NBool(-1);
        }
    }
}

