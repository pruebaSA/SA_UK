namespace System.Windows.Forms
{
    using System;

    public class PreviewKeyDownEventArgs : EventArgs
    {
        private bool _isInputKey;
        private readonly Keys _keyData;

        public PreviewKeyDownEventArgs(Keys keyData)
        {
            this._keyData = keyData;
        }

        public bool Alt =>
            ((this._keyData & Keys.Alt) == Keys.Alt);

        public bool Control =>
            ((this._keyData & Keys.Control) == Keys.Control);

        public bool IsInputKey
        {
            get => 
                this._isInputKey;
            set
            {
                this._isInputKey = value;
            }
        }

        public Keys KeyCode
        {
            get
            {
                Keys keys = this._keyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int) keys))
                {
                    return Keys.None;
                }
                return keys;
            }
        }

        public Keys KeyData =>
            this._keyData;

        public int KeyValue =>
            (((int) this._keyData) & 0xffff);

        public Keys Modifiers =>
            (this._keyData & ~Keys.KeyCode);

        public bool Shift =>
            ((this._keyData & Keys.Shift) == Keys.Shift);
    }
}

