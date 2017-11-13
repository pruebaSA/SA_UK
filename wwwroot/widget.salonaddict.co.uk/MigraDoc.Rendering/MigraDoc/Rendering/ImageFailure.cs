namespace MigraDoc.Rendering
{
    using System;

    internal enum ImageFailure
    {
        None,
        FileNotFound,
        InvalidType,
        NotRead,
        EmptySize
    }
}

