namespace System.Text.RegularExpressions
{
    using System;

    [Serializable]
    public class Capture
    {
        internal int _index;
        internal int _length;
        internal string _text;

        internal Capture(string text, int i, int l)
        {
            this._text = text;
            this._index = i;
            this._length = l;
        }

        internal string GetLeftSubstring() => 
            this._text.Substring(0, this._index);

        internal string GetOriginalString() => 
            this._text;

        internal string GetRightSubstring() => 
            this._text.Substring(this._index + this._length, (this._text.Length - this._index) - this._length);

        public override string ToString() => 
            this.Value;

        public int Index =>
            this._index;

        public int Length =>
            this._length;

        public string Value =>
            this._text.Substring(this._index, this._length);
    }
}

