namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct Int64Aggregator
    {
        private long result;
        private int cnt;
        public void Create()
        {
            this.cnt = 0;
        }

        public void Sum(long value)
        {
            if (this.cnt == 0)
            {
                this.result = value;
                this.cnt = 1;
            }
            else
            {
                this.result += value;
            }
        }

        public void Average(long value)
        {
            if (this.cnt == 0)
            {
                this.result = value;
            }
            else
            {
                this.result += value;
            }
            this.cnt++;
        }

        public void Minimum(long value)
        {
            if ((this.cnt == 0) || (value < this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public void Maximum(long value)
        {
            if ((this.cnt == 0) || (value > this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public long SumResult =>
            this.result;
        public long AverageResult =>
            (this.result / ((long) this.cnt));
        public long MinimumResult =>
            this.result;
        public long MaximumResult =>
            this.result;
        public bool IsEmpty =>
            (this.cnt == 0);
    }
}

