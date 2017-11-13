namespace System
{
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ConsoleKeyInfo
    {
        private char _keyChar;
        private ConsoleKey _key;
        private ConsoleModifiers _mods;
        public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
        {
            if ((key < ((ConsoleKey) 0)) || (key > ((ConsoleKey) 0xff)))
            {
                throw new ArgumentOutOfRangeException("key", Environment.GetResourceString("ArgumentOutOfRange_ConsoleKey"));
            }
            this._keyChar = keyChar;
            this._key = key;
            this._mods = 0;
            if (shift)
            {
                this._mods |= ConsoleModifiers.Shift;
            }
            if (alt)
            {
                this._mods |= ConsoleModifiers.Alt;
            }
            if (control)
            {
                this._mods |= ConsoleModifiers.Control;
            }
        }

        public char KeyChar =>
            this._keyChar;
        public ConsoleKey Key =>
            this._key;
        public ConsoleModifiers Modifiers =>
            this._mods;
        public override bool Equals(object value) => 
            ((value is ConsoleKeyInfo) && this.Equals((ConsoleKeyInfo) value));

        public bool Equals(ConsoleKeyInfo obj) => 
            (((obj._keyChar == this._keyChar) && (obj._key == this._key)) && (obj._mods == this._mods));

        public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b) => 
            a.Equals(b);

        public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b) => 
            !(a == b);

        public override int GetHashCode() => 
            ((int) (((ConsoleModifiers) this._keyChar) | this._mods));
    }
}

