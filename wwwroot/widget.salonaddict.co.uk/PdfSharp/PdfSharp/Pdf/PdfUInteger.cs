namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfUInteger : PdfNumber, IConvertible
    {
        private uint value;

        public PdfUInteger()
        {
        }

        public PdfUInteger(uint value)
        {
            this.value = value;
        }

        public TypeCode GetTypeCode() => 
            TypeCode.Int32;

        string IConvertible.ToString(IFormatProvider provider) => 
            this.value.ToString(provider);

        public bool ToBoolean(IFormatProvider provider) => 
            Convert.ToBoolean(this.value);

        public byte ToByte(IFormatProvider provider) => 
            Convert.ToByte(this.value);

        public char ToChar(IFormatProvider provider) => 
            Convert.ToChar(this.value);

        public DateTime ToDateTime(IFormatProvider provider) => 
            new DateTime();

        public decimal ToDecimal(IFormatProvider provider) => 
            this.value;

        public double ToDouble(IFormatProvider provider) => 
            ((double) this.value);

        public short ToInt16(IFormatProvider provider) => 
            Convert.ToInt16(this.value);

        public int ToInt32(IFormatProvider provider) => 
            Convert.ToInt32(this.value);

        public long ToInt64(IFormatProvider provider) => 
            ((long) this.value);

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException();
        }

        public float ToSingle(IFormatProvider provider) => 
            ((float) this.value);

        public override string ToString() => 
            this.value.ToString(CultureInfo.InvariantCulture);

        public object ToType(Type conversionType, IFormatProvider provider) => 
            null;

        public ushort ToUInt16(IFormatProvider provider) => 
            Convert.ToUInt16(this.value);

        public uint ToUInt32(IFormatProvider provider) => 
            Convert.ToUInt32(this.value);

        public ulong ToUInt64(IFormatProvider provider) => 
            Convert.ToUInt64(this.value);

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public uint Value =>
            this.value;
    }
}

