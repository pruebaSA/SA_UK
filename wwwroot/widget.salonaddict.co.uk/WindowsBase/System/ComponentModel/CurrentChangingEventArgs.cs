namespace System.ComponentModel
{
    using System;
    using System.Windows;

    public class CurrentChangingEventArgs : EventArgs
    {
        private bool _cancel;
        private bool _isCancelable;

        public CurrentChangingEventArgs()
        {
            this.Initialize(true);
        }

        public CurrentChangingEventArgs(bool isCancelable)
        {
            this.Initialize(isCancelable);
        }

        private void Initialize(bool isCancelable)
        {
            this._isCancelable = isCancelable;
        }

        public bool Cancel
        {
            get => 
                this._cancel;
            set
            {
                if (this.IsCancelable)
                {
                    this._cancel = value;
                }
                else if (value)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("CurrentChangingCannotBeCanceled"));
                }
            }
        }

        public bool IsCancelable =>
            this._isCancelable;
    }
}

