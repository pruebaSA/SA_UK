namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfInteger : PdfNumber, IConvertible
    {
        private readonly int value;

        public PdfInteger()
        {
        }

        public PdfInteger(int value)
        {
            this.value = value;
        }

        public TypeCode GetTypeCode() => 
            TypeCode.Int32;

        bool IConvertible.ToBoolean(IFormatProvider provider) => 
            Convert.ToBoolean(this.value);

        byte IConvertible.ToByte(IFormatProvider provider) => 
            Convert.ToByte(this.value);

        char IConvertible.ToChar(IFormatProvider provider) => 
            Convert.ToChar(this.value);

        DateTime IConvertible.ToDateTime(IFormatProvider provider) => 
            new DateTime();

        decimal IConvertible.ToDecimal(IFormatProvider provider) => 
            this.value;

        double IConvertible.ToDouble(IFormatProvider provider) => 
            ((double) this.value);

        short IConvertible.ToInt16(IFormatProvider provider) => 
            Convert.ToInt16(this.value);

        int IConvertible.ToInt32(IFormatProvider provider) => 
            this.value;

        long IConvertible.ToInt64(IFormatProvider provider) => 
            ((long) this.value);

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        float IConvertible.ToSingle(IFormatProvider provider) => 
            ((float) this.value);

        string IConvertible.ToString(IFormatProvider provider) => 
            this.value.ToString(provider);

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => 
            null;

        ushort IConvertible.ToUInt16(IFormatProvider provider) => 
            Convert.ToUInt16(this.value);

        uint IConvertible.ToUInt32(IFormatProvider provider) => 
            Convert.ToUInt32(this.value);

        ulong IConvertible.ToUInt64(IFormatProvider provider) => 
            Convert.ToUInt64(this.value);

        public override string ToString() => 
            this.value.ToString(CultureInfo.InvariantCulture);

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public int Value =>
            this.value;
    }
}

