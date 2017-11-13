namespace PdfSharp.Pdf.IO
{
    using System;

    internal enum Symbol
    {
        None,
        Comment,
        Null,
        Integer,
        UInteger,
        Real,
        Boolean,
        String,
        HexString,
        UnicodeString,
        UnicodeHexString,
        Name,
        Keyword,
        BeginStream,
        EndStream,
        BeginArray,
        EndArray,
        BeginDictionary,
        EndDictionary,
        Obj,
        EndObj,
        R,
        XRef,
        Trailer,
        StartXRef,
        Eof
    }
}

