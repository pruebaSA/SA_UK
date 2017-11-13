namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SequenceRange
    {
        private long lower;
        private long upper;
        public SequenceRange(long number) : this(number, number)
        {
        }

        public SequenceRange(long lower, long upper)
        {
            if (lower < 0L)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            if (lower > upper)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            this.lower = lower;
            this.upper = upper;
        }

        public long Lower =>
            this.lower;
        public long Upper =>
            this.upper;
        public static bool operator ==(SequenceRange a, SequenceRange b) => 
            ((a.lower == b.lower) && (a.upper == b.upper));

        public static bool operator !=(SequenceRange a, SequenceRange b) => 
            !(a == b);

        public bool Contains(long number) => 
            ((number >= this.lower) && (number <= this.upper));

        public bool Contains(SequenceRange range) => 
            ((range.Lower >= this.lower) && (range.Upper <= this.upper));

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return ((obj is SequenceRange) && (this == ((SequenceRange) obj)));
        }

        public override int GetHashCode()
        {
            long num = this.upper ^ (this.upper - this.lower);
            return (int) ((num << 0x20) ^ (num >> 0x20));
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}-{1}", new object[] { this.lower, this.upper });
    }
}

