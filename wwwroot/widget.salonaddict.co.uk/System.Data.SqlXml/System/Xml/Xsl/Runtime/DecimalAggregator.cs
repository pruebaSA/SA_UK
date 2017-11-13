namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct DecimalAggregator
    {
        private decimal result;
        private int cnt;
        public void Create()
        {
            this.cnt = 0;
        }

        public void Sum(decimal value)
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

        public void Average(decimal value)
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

        public void Minimum(decimal value)
        {
            if ((this.cnt == 0) || (value < this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public void Maximum(decimal value)
        {
            if ((this.cnt == 0) || (value > this.result))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public decimal SumResult =>
            this.result;
        public decimal AverageResult =>
            (this.result / this.cnt);
        public decimal MinimumResult =>
            this.result;
        public decimal MaximumResult =>
            this.result;
        public bool IsEmpty =>
            (this.cnt == 0);
    }
}

