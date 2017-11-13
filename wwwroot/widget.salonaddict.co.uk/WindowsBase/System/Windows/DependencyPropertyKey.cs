namespace System.Windows
{
    using System;

    public sealed class DependencyPropertyKey
    {
        private System.Windows.DependencyProperty _dp;

        internal DependencyPropertyKey(System.Windows.DependencyProperty dp)
        {
            this._dp = dp;
        }

        public void OverrideMetadata(Type forType, PropertyMetadata typeMetadata)
        {
            if (this._dp == null)
            {
                throw new InvalidOperationException();
            }
            this._dp.OverrideMetadata(forType, typeMetadata, this);
        }

        internal void SetDependencyProperty(System.Windows.DependencyProperty dp)
        {
            this._dp = dp;
        }

        public System.Windows.DependencyProperty DependencyProperty =>
            this._dp;
    }
}

