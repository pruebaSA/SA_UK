namespace MigraDoc.DocumentObjectModel.Internals
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NDouble : INullableValue
    {
        public static readonly NDouble NullValue;
        private double val;
        public NDouble(double value)
        {
            this.val = value;
        }

        public double Value
        {
            get
            {
                if (!double.IsNaN(this.val))
                {
                    return this.val;
                }
                return 0.0;
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
            this.val = (double) value;
        }

        public void SetNull()
        {
            this.val = double.NaN;
        }

        public bool IsNull =>
            double.IsNaN(this.val);
        public override bool Equals(object value) => 
            ((value is NDouble) && (this == ((NDouble) value)));

        public override int GetHashCode() => 
            this.val.GetHashCode();

        public static bool operator ==(NDouble l, NDouble r)
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

        public static bool operator !=(NDouble l, NDouble r) => 
            !(l == r);

        static NDouble()
        {
            NullValue = new NDouble(double.NaN);
        }
    }
}

