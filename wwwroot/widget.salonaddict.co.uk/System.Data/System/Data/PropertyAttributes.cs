namespace System.Data
{
    using System;
    using System.ComponentModel;

    [Flags, EditorBrowsable(EditorBrowsableState.Never), Obsolete("PropertyAttributes has been deprecated.  http://go.microsoft.com/fwlink/?linkid=14202")]
    public enum PropertyAttributes
    {
        NotSupported = 0,
        Optional = 2,
        Read = 0x200,
        Required = 1,
        Write = 0x400
    }
}

