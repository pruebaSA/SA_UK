namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {
        private bool handled;
        private readonly Keys keyData;
        private bool suppressKeyPress;

        public KeyEventArgs(Keys keyData)
        {
            this.keyData = keyData;
        }

        public virtual bool Alt =>
            ((this.keyData & Keys.Alt) == Keys.Alt);

        public bool Control =>
            ((this.keyData & Keys.Control) == Keys.Control);

        public bool Handled
        {
            get => 
                this.handled;
            set
            {
                this.handled = value;
            }
        }

        public Keys KeyCode
        {
            get
            {
                Keys keys = this.keyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int) keys))
                {
                    return Keys.None;
                }
                return keys;
            }
        }

        public Keys KeyData =>
            this.keyData;

        public int KeyValue =>
            (((int) this.keyData) & 0xffff);

        public Keys Modifiers =>
            (this.keyData & ~Keys.KeyCode);

        public virtual bool Shift =>
            ((this.keyData & Keys.Shift) == Keys.Shift);

        public bool SuppressKeyPress
        {
            get => 
                this.suppressKeyPress;
            set
            {
                this.suppressKeyPress = value;
                this.handled = value;
            }
        }
    }
}

