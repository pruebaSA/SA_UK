namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct Int32Aggregator
    {
        private int result;
        private int cnt;
        public void Create()
        {
            this.cnt = 0;
        }

        public void Sum(int value)
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

        public void Average(int value)
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

        public void Minimum(int value)
        {
            if ((this.cnt == 0) || (value < this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public void Maximum(int value)
        {
            if ((this.cnt == 0) || (value > this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public int SumResult =>
            this.result;
        public int AverageResult =>
            (this.result / this.cnt);
        public int MinimumResult =>
            this.result;
        public int MaximumResult =>
            this.result;
        public bool IsEmpty =>
            (this.cnt == 0);
    }
}

