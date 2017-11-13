namespace System.Net
{
    using System;

    [Flags]
    internal enum FtpMethodFlags
    {
        DoesNotTakeParameter = 0x10,
        HasHttpCommand = 0x80,
        IsDownload = 1,
        IsUpload = 2,
        MayTakeParameter = 8,
        None = 0,
        ParameterIsDirectory = 0x20,
        ShouldParseForResponseUri = 0x40,
        TakesParameter = 4
    }
}

