namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class ItemCheckEventArgs : EventArgs
    {
        private readonly CheckState currentValue;
        private readonly int index;
        private CheckState newValue;

        public ItemCheckEventArgs(int index, CheckState newCheckValue, CheckState currentValue)
        {
            this.index = index;
            this.newValue = newCheckValue;
            this.currentValue = currentValue;
        }

        public CheckState CurrentValue =>
            this.currentValue;

        public int Index =>
            this.index;

        public CheckState NewValue
        {
            get => 
                this.newValue;
            set
            {
                this.newValue = value;
            }
        }
    }
}

