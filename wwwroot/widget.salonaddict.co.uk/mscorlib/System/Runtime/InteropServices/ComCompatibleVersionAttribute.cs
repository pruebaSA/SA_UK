namespace System.Runtime.InteropServices
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, Inherited=false), ComVisible(true)]
    public sealed class ComCompatibleVersionAttribute : Attribute
    {
        internal int _build;
        internal int _major;
        internal int _minor;
        internal int _revision;

        public ComCompatibleVersionAttribute(int major, int minor, int build, int revision)
        {
            this._major = major;
            this._minor = minor;
            this._build = build;
            this._revision = revision;
        }

        public int BuildNumber =>
            this._build;

        public int MajorVersion =>
            this._major;

        public int MinorVersion =>
            this._minor;

        public int RevisionNumber =>
            this._revision;
    }
}

