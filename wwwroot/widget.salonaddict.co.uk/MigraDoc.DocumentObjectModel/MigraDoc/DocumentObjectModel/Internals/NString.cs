namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NString : INullableValue
    {
        public static readonly NString NullValue;
        private string val;
        public string Value
        {
            get
            {
                if (this.val == null)
                {
                    return "";
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
            this.val = (string) value;
        }

        public void SetNull()
        {
            this.val = null;
        }

        public bool IsNull =>
            (this.val == null);
        public override bool Equals(object value) => 
            ((value is NString) && (this == ((NString) value)));

        public override int GetHashCode()
        {
            if (this.val != null)
            {
                return this.val.GetHashCode();
            }
            return 0;
        }

        public static bool operator ==(NString l, NString r)
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

        public static bool operator !=(NString l, NString r) => 
            !(l == r);

        static NString()
        {
            NullValue = new NString();
        }
    }
}

