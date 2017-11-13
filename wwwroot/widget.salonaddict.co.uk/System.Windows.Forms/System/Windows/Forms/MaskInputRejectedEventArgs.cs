namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    public class MaskInputRejectedEventArgs : EventArgs
    {
        private MaskedTextResultHint hint;
        private int position;

        public MaskInputRejectedEventArgs(int position, MaskedTextResultHint rejectionHint)
        {
            this.position = position;
            this.hint = rejectionHint;
        }

        public int Position =>
            this.position;

        public MaskedTextResultHint RejectionHint =>
            this.hint;
    }
}

