namespace System.Windows
{
    using System;

    internal sealed class DependencySource
    {
        private System.Windows.DependencyObject _d;
        private System.Windows.DependencyProperty _dp;

        public DependencySource(System.Windows.DependencyObject d, System.Windows.DependencyProperty dp)
        {
            this._d = d;
            this._dp = dp;
        }

        public System.Windows.DependencyObject DependencyObject =>
            this._d;

        public System.Windows.DependencyProperty DependencyProperty =>
            this._dp;
    }
}

