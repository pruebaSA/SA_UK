namespace System.Runtime.InteropServices
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited=false), ComVisible(true)]
    public sealed class ProgIdAttribute : Attribute
    {
        internal string _val;

        public ProgIdAttribute(string progId)
        {
            this._val = progId;
        }

        public string Value =>
            this._val;
    }
}

