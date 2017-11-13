namespace System.Windows.Forms
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class KeyPressEventArgs : EventArgs
    {
        private bool handled;
        private char keyChar;

        public KeyPressEventArgs(char keyChar)
        {
            this.keyChar = keyChar;
        }

        public bool Handled
        {
            get => 
                this.handled;
            set
            {
                this.handled = value;
            }
        }

        public char KeyChar
        {
            get => 
                this.keyChar;
            set
            {
                this.keyChar = value;
            }
        }
    }
}

