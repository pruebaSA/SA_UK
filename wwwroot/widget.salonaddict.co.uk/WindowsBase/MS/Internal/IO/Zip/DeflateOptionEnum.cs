namespace MS.Internal.IO.Zip
{
    using System;

    internal enum DeflateOptionEnum : byte
    {
        Fast = 4,
        Maximum = 2,
        None = 0xff,
        Normal = 0,
        SuperFast = 6
    }
}

