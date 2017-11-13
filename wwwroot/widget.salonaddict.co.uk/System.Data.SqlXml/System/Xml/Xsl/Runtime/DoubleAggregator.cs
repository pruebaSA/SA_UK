namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), EditorBrowsable(EditorBrowsableState.Never)]
    public struct DoubleAggregator
    {
        private double result;
        private int cnt;
        public void Create()
        {
            this.cnt = 0;
        }

        public void Sum(double value)
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

        public void Average(double value)
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

        public void Minimum(double value)
        {
            if (((this.cnt == 0) || (value < this.result)) || double.IsNaN(value))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public void Maximum(double value)
        {
            if (((this.cnt == 0) || (value > this.result)) || double.IsNaN(value))
            {
                this.result = value;
            }
            this.cnt = 1;
        }

        public double SumResult =>
            this.result;
        public double AverageResult =>
            (this.result / ((double) this.cnt));
        public double MinimumResult =>
            this.result;
        public double MaximumResult =>
            this.result;
        public bool IsEmpty =>
            (this.cnt == 0);
    }
}

